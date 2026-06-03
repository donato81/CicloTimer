# CODING PLAN 006 — Gestore timer reale

## Metadati

- Tipo documento: Coding plan
- Codice: 006
- Titolo: Gestore timer reale
- Versione: 0.2.0
- Stato: APPROVATO
- Progetto: CicloTimer
- Repository: donato81/CicloTimer
- Data: 2026-06-03
- Design di riferimento: `docs/1-design/006-DESIGN_gestore-timer-reale_v0.3.0.md`

---

## 1. Scopo del coding plan

Questo documento traduce il Design 006 in un piano operativo di codifica.

Il blocco 006 deve introdurre il componente che fa avanzare il timer usando il tempo reale.

Il componente dovrà:

- usare `TimeProvider`;
- calcolare il tempo trascorso tramite timestamp;
- convertire il tempo trascorso in secondi interi maturati;
- conservare il residuo inferiore a un secondo come `TimeSpan`;
- chiamare solo `ITimerAppOrchestrator.Tick(...)`;
- evitare loop multipli;
- evitare tick paralleli;
- gestire correttamente `Start()`, `Stop()` e `Dispose()`;
- restare completamente fuori dalla UI WPF.

---

## 2. Stato iniziale atteso

Prima di iniziare la codifica, verificare che esistano già:

```text
services/CicloTimer.App/
tests/CicloTimer.App.Tests/
````

Verificare inoltre che nel progetto App esista già l'interfaccia:

```text
ITimerAppOrchestrator
```

con metodo:

```csharp
Tick(int elapsedSeconds)
```

Il runner dovrà dipendere solo da questa interfaccia e non dai componenti concreti già implementati nei blocchi precedenti.

---

## 3. Vincoli assoluti

Durante questo blocco è vietato introdurre:

```text
DispatcherTimer
System.Windows.Threading.DispatcherTimer
Dispatcher
System.Windows
UI WPF
MainWindow
XAML
ICommand
INotifyPropertyChanged
NVDA
Live Region
nuove stringhe utente
nuove chiavi di localizzazione
nuove regole Core
nuove regole Bridge
nuove regole Audio
```

Il blocco 006 non deve modificare:

```text
models/CicloTimer.Core/
view-models/CicloTimer.Bridge/
services/CicloTimer.Audio/
locales/CicloTimer.Localization/
```

Salvo aggiustamenti minimi inevitabili, che dovranno essere motivati e documentati.

---

## 4. File da creare

Creare la nuova cartella:

```text
services/CicloTimer.App/Timing/
```

Creare i file:

```text
services/CicloTimer.App/Timing/IRealtimeTimerRunner.cs
services/CicloTimer.App/Timing/RealtimeTimerRunner.cs
```

Creare la cartella test:

```text
tests/CicloTimer.App.Tests/Timing/
```

Creare i file test:

```text
tests/CicloTimer.App.Tests/Timing/RealtimeTimerRunnerTests.cs
tests/CicloTimer.App.Tests/Timing/Fakes/FakeTimerAppOrchestrator.cs
```

Se la cartella `Fakes` non è coerente con lo stile dei test già esistenti, adattarsi allo stile reale del repository.

---

## 5. Interfaccia da implementare

Creare `IRealtimeTimerRunner` con contratto minimo:

```csharp
namespace CicloTimer.App.Timing;

public interface IRealtimeTimerRunner : IDisposable
{
    bool IsRunning { get; }

    void Start();

    void Stop();
}
```

Regole:

* non aggiungere `LastError`;
* non aggiungere eventi;
* non aggiungere riferimenti UI;
* non aggiungere metodi asincroni pubblici in questa fase.

---

## 6. Implementazione `RealtimeTimerRunner`

Creare `RealtimeTimerRunner` in:

```text
services/CicloTimer.App/Timing/RealtimeTimerRunner.cs
```

Dipendenze del costruttore:

```csharp
ITimerAppOrchestrator orchestrator
TimeProvider? timeProvider = null
```

Regole del costruttore:

* `orchestrator` è obbligatorio;
* se `orchestrator` è `null`, generare `ArgumentNullException`;
* se `timeProvider` è `null`, usare `TimeProvider.System`.

Esempio concettuale:

```csharp
public RealtimeTimerRunner(
    ITimerAppOrchestrator orchestrator,
    TimeProvider? timeProvider = null)
{
    _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
    _timeProvider = timeProvider ?? TimeProvider.System;
}
```

---

## 7. Stato interno richiesto

L'implementazione deve gestire almeno questi stati interni:

```csharp
private readonly object _sync = new();

private readonly ITimerAppOrchestrator _orchestrator;
private readonly TimeProvider _timeProvider;

private CancellationTokenSource? _cancellationTokenSource;
private Task? _loopTask;
private bool _isRunning;
private bool _disposed;

private DateTimeOffset _lastProcessedTime;
private TimeSpan _remainder;
```

La forma precisa può variare, ma devono essere coperti questi concetti:

* sincronizzazione;
* stato running;
* stato disposed;
* ciclo asincrono interno;
* cancellazione;
* ultimo timestamp elaborato;
* residuo temporale.

Nota:

lo stato `_disposed` deve essere letto e scritto in modo thread-safe. È accettabile gestirlo sempre dentro `lock`; in alternativa può essere usato `volatile` se l'implementazione lo rende più chiaro. La scelta va mantenuta semplice e coerente.

---

## 8. Proprietà `IsRunning`

Implementare `IsRunning` in modo thread-safe.

Esempio concettuale:

```csharp
public bool IsRunning
{
    get
    {
        lock (_sync)
        {
            return _isRunning;
        }
    }
}
```

Regola:

`IsRunning` deve essere `true` solo quando il runner è considerato attivo.

Alla fine del loop, sia per stop normale sia per errore inatteso, lo stato deve tornare a:

```text
IsRunning = false
```

dentro una sezione sincronizzata.

---

## 9. Metodo `Start()`

`Start()` deve:

1. entrare in sezione sincronizzata;
2. verificare se l'oggetto è già disposed;
3. se è disposed, generare `ObjectDisposedException`;
4. se il runner è già attivo, non fare nulla;
5. creare un nuovo `CancellationTokenSource`;
6. catturare il `CancellationToken` in una variabile locale immutabile;
7. inizializzare `_lastProcessedTime` con `timeProvider.GetUtcNow()`;
8. inizializzare `_remainder` a `TimeSpan.Zero`;
9. impostare `_isRunning = true`;
10. avviare il loop asincrono interno passando il token locale.

Regola fondamentale:

```text
Start chiamato due volte non deve creare due loop.
```

Dopo:

```text
Start → Start
```

deve esistere un solo loop attivo.

### 9.1 Cattura atomica del token

La creazione del `CancellationTokenSource`, la cattura del token e l'avvio del loop devono essere coerenti.

Schema concettuale:

```csharp
CancellationToken token;

lock (_sync)
{
    ThrowIfDisposed();

    if (_isRunning)
    {
        return;
    }

    _cancellationTokenSource = new CancellationTokenSource();
    token = _cancellationTokenSource.Token;

    _lastProcessedTime = _timeProvider.GetUtcNow();
    _remainder = TimeSpan.Zero;
    _isRunning = true;

    _loopTask = RunLoopAsync(token);
}
```

Regole:

* `RunLoopAsync` deve ricevere il token come parametro;
* il loop non deve rileggere `_cancellationTokenSource` durante l'esecuzione;
* il token passato al loop rappresenta quel singolo ciclo di vita;
* un successivo `Start → Stop → Start` deve creare un nuovo token e un nuovo loop.

Questo evita race condition e loop zombie.

---

## 10. Metodo `Stop()`

`Stop()` deve:

1. essere sicuro se il runner è già fermo;
2. essere sicuro se l'istanza è già disposed;
3. richiedere la cancellazione del loop tramite `CancellationTokenSource`;
4. impostare `_isRunning = false`;
5. non generare errore se chiamato due volte;
6. non bloccare indefinitamente il chiamante;
7. non fare `await`;
8. non smaltire risorse in modo tale da rompere un loop ancora in uscita.

Regola fondamentale:

```text
Stop chiamato due volte non deve generare errore.
```

Dopo:

```text
Start → Stop → Start
```

il runner deve ripartire correttamente con un nuovo ciclo, un nuovo token e residuo azzerato.

### 10.1 Gestione prudente del CancellationTokenSource

`Stop()` deve cancellare il token, ma lo smaltimento effettivo del `CancellationTokenSource` deve essere fatto in modo sicuro.

Regola:

```text
Stop cancella il ciclo.
Stop non deve causare ObjectDisposedException spurie nel loop ancora in uscita.
```

È accettabile:

* cancellare il token in `Stop()`;
* lasciare che la risorsa venga smaltita in modo controllato quando il loop è terminato;
* oppure smaltirla in `Dispose()` dell'oggetto principale.

La scelta precisa deve evitare loop zombie e oggetti smaltiti mentre sono ancora letti dal loop.

---

## 11. Metodo `Dispose()`

`Dispose()` deve:

1. essere sicuro se chiamato più volte;
2. fermare il runner se è attivo;
3. cancellare il loop;
4. rilasciare il `CancellationTokenSource` in modo sicuro;
5. marcare l'istanza come disposed;
6. impedire ogni riavvio successivo.

Regola fondamentale:

```text
Dopo Dispose, Start deve generare ObjectDisposedException.
```

`Stop()` dopo `Dispose()` deve essere un no-op sicuro.

---

## 12. Loop temporale interno

Il loop interno deve essere asincrono e non deve bloccare il thread chiamante.

Firma concettuale:

```csharp
private async Task RunLoopAsync(CancellationToken cancellationToken)
```

Il loop deve:

1. ricevere il `CancellationToken` come parametro locale;
2. non rileggere `_cancellationTokenSource`;
3. attendere il risveglio periodico;
4. leggere `timeProvider.GetUtcNow()`;
5. calcolare il delta reale tramite `TimeSpan`;
6. sommare il delta al residuo precedente;
7. estrarre i secondi interi maturati;
8. chiamare `orchestrator.Tick(elapsedSeconds)` solo se `elapsedSeconds > 0`;
9. conservare il nuovo residuo;
10. aggiornare il timestamp elaborato;
11. ripetere finché non arriva cancellazione;
12. riportare lo stato a fermo alla fine del loop.

Schema concettuale:

```csharp
var now = _timeProvider.GetUtcNow();
var delta = now - _lastProcessedTime;
var total = _remainder + delta;

var elapsedSeconds = (int)total.TotalSeconds;

if (elapsedSeconds > 0)
{
    _orchestrator.Tick(elapsedSeconds);
    _remainder = total - TimeSpan.FromSeconds(elapsedSeconds);
}
else
{
    _remainder = total;
}

_lastProcessedTime = now;
```

Regole:

* non usare `double` o `float` per salvare il residuo;
* usare `TimeSpan`;
* non chiamare mai `Tick(0)`;
* non chiamare mai `Tick` con valori negativi;
* non basarsi sul semplice conteggio dei risvegli;
* usare timestamp reali tramite `TimeProvider`;
* non avviare tick fire-and-forget;
* non usare Task paralleli per eseguire `Tick`.

### 12.1 Stato a fine loop

Alla fine del loop, sia per cancellazione normale sia per errore inatteso, lo stato interno deve essere riportato in modo coerente.

Regola:

```text
RunLoopAsync termina → _isRunning deve diventare false.
```

Questo aggiornamento deve avvenire dentro una sezione sincronizzata.

---

## 13. Timer periodico

Usare un timer non UI basato su `TimeProvider`.

Soluzione preferita:

```csharp
using var timer = _timeProvider.CreatePeriodicTimer(TimeSpan.FromSeconds(1));
```

Il ciclo dovrà attendere il timer tramite cancellazione.

Schema concettuale:

```csharp
while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
{
    ProcessElapsedTime();
}
```

Adeguare la sintassi esatta alla versione disponibile nel progetto.

Regole:

* non usare `DispatcherTimer`;
* non usare `System.Timers.Timer`;
* non usare `System.Threading.Timer`;
* non usare `Task.Delay` in loop;
* non introdurre dipendenze WPF;
* ogni `await` nel loop interno deve usare `.ConfigureAwait(false)`.

### 13.1 Primo tick

`PeriodicTimer` attende il primo intervallo prima di produrre il primo risveglio.

Questo comportamento è accettato e voluto.

Regola:

```text
Start non deve produrre un Tick immediato.
```

Motivazione:

* `Tick` rappresenta tempo realmente trascorso;
* all'avvio non è ancora trascorso un secondo;
* il primo controllo avviene dopo il primo intervallo temporale;
* eventuali piccoli ritardi saranno compensati dal calcolo con `TimeProvider.GetUtcNow()`.

Quindi il comportamento corretto è:

```text
Start → nessun Tick immediato
dopo circa 1 secondo reale → primo controllo temporale
```

---

## 14. Sincronizzazione tra metodi sincroni e loop asincrono

Questo è il punto più delicato segnalato dai consiglieri.

`Start()`, `Stop()` e `Dispose()` sono sincroni.

Il loop interno è asincrono.

La sincronizzazione deve evitare:

* deadlock;
* race condition;
* loop zombie;
* doppio loop;
* tick dopo `Stop()`;
* tick dopo `Dispose()`.

Regole operative:

1. usare `lock` solo per leggere e modificare stato breve;
2. non fare `await` dentro un `lock`;
3. non chiamare codice lungo dentro un `lock`;
4. non chiamare `orchestrator.Tick(...)` dentro un `lock` globale del runner;
5. creare `CancellationTokenSource` e catturare il token locale in modo atomico;
6. passare il token locale a `RunLoopAsync(token)`;
7. non rileggere `_cancellationTokenSource` dentro il loop;
8. copiare localmente token e stato quando serve;
9. il loop deve uscire se il token viene cancellato;
10. al termine del loop, lo stato deve restare coerente;
11. ogni `await` nel loop deve usare `.ConfigureAwait(false)`.

È accettabile usare `lock` per `Start()`, `Stop()`, `Dispose()` e proprietà `IsRunning`.

È accettabile usare un metodo privato asincrono tipo:

```csharp
private async Task RunLoopAsync(CancellationToken cancellationToken)
```

Non è obbligatorio usare `SemaphoreSlim`, salvo scelta motivata in implementazione.

---

## 15. Prevenzione loop multipli

Prima di creare un nuovo loop, `Start()` deve verificare lo stato `_isRunning`.

Regola:

```text
Se _isRunning è true, Start ritorna senza fare nulla.
```

Test obbligatorio:

```text
Start chiamato due volte produce un solo ciclo di tick.
```

---

## 16. Prevenzione tick paralleli

Il loop deve essere sequenziale.

Il runner deve attendere il completamento logico di `orchestrator.Tick(...)` prima di procedere oltre.

Dato che `Tick` oggi è sincrono, la regola concreta è:

```text
non avviare Tick su Task separati;
non usare fire-and-forget;
non usare Parallel;
non usare ThreadPool.QueueUserWorkItem.
```

Il loop deve chiamare direttamente:

```csharp
_orchestrator.Tick(elapsedSeconds);
```

e poi proseguire.

---

## 17. Gestione degli errori

Se `orchestrator.Tick(...)` genera un'eccezione inattesa:

* catturare l'eccezione nel loop;
* fermare il runner;
* impostare `_isRunning = false`;
* non lasciare loop attivi;
* non esporre `LastError`;
* non generare eccezioni non osservate in background.

Schema concettuale:

```csharp
try
{
    // loop
}
catch (OperationCanceledException)
{
    // stop normale
}
catch
{
    // errore inatteso
    MarkStopped();
}
finally
{
    MarkStopped();
}
```

Nota:

non introdurre logging strutturato in questo blocco, salvo logging già presente nel progetto.

---

## 18. Relazione con orchestratore

Il runner deve chiamare solo:

```csharp
_orchestrator.Tick(elapsedSeconds);
```

Non deve chiamare:

```text
Core
Bridge
AudioService
Localization
SystemActionDispatcher
TimerBridgeAdapter
```

Il runner non deve configurare, avviare, pausare, riprendere o resettare il timer.

Queste azioni restano responsabilità della futura UI/ViewModel nel blocco 007.

---

## 19. Test: fake orchestrator

Creare un fake semplice per `ITimerAppOrchestrator`.

Il fake deve poter registrare:

* quante volte viene chiamato `Tick`;
* con quali valori viene chiamato;
* se deve generare eccezione;
* eventuale blocco controllato per testare tick non paralleli;
* eventuale contatore atomico per rilevare tick concorrenti.

Esempio concettuale:

```csharp
internal sealed class FakeTimerAppOrchestrator : ITimerAppOrchestrator
{
    public List<int> TickCalls { get; } = new();

    public void Tick(int elapsedSeconds)
    {
        TickCalls.Add(elapsedSeconds);
    }

    // implementare gli altri membri dell'interfaccia
    // con comportamento minimo necessario ai test
}
```

Per il test sui tick paralleli è possibile usare un contatore atomico:

```csharp
private int _concurrentTicks;

public void Tick(int elapsedSeconds)
{
    if (Interlocked.Increment(ref _concurrentTicks) > 1)
    {
        throw new InvalidOperationException("Tick paralleli rilevati.");
    }

    try
    {
        // lavoro controllato dal test
    }
    finally
    {
        Interlocked.Decrement(ref _concurrentTicks);
    }
}
```

Adattare il fake alla firma reale di `ITimerAppOrchestrator`.

---

## 20. Test: TimeProvider controllabile

Usare `FakeTimeProvider` o equivalente del pacchetto Microsoft.

Se il pacchetto non è già presente, aggiungerlo solo al progetto test:

```text
Microsoft.Extensions.TimeProvider.Testing
```

Verificare la versione coerente con `.NET 9`.

I test devono far avanzare il tempo artificialmente, senza attese reali.

Esempio concettuale:

```csharp
var timeProvider = new FakeTimeProvider();
var runner = new RealtimeTimerRunner(fakeOrchestrator, timeProvider);

runner.Start();

timeProvider.Advance(TimeSpan.FromSeconds(1));
```

Adattare la sintassi alle API effettive del pacchetto.

---

## 21. Test obbligatori funzionali

Implementare test per:

1. `Start()` porta `IsRunning` a `true`.
2. `Stop()` porta `IsRunning` a `false`.
3. `Start()` chiamato due volte non crea due loop.
4. `Stop()` chiamato due volte non genera errore.
5. `Start → Stop → Start` funziona.
6. `Dispose()` ferma il runner.
7. `Dispose()` chiamato due volte non genera errore.
8. `Start()` dopo `Dispose()` genera `ObjectDisposedException`.
9. `Stop()` dopo `Dispose()` non genera errore.

---

## 22. Test obbligatori sul tempo

Implementare test per:

1. `Start()` non produce un tick immediato.
2. dopo 1 secondo simulato viene chiamato `Tick(1)`;
3. dopo 2 secondi simulati viene chiamato `Tick(2)` oppure due chiamate equivalenti solo se motivato;
4. non viene mai chiamato `Tick(0)`;
5. un avanzamento inferiore a 1 secondo non produce tick;
6. il residuo inferiore a 1 secondo viene conservato;
7. due avanzamenti frazionari che insieme superano 1 secondo producono `Tick(1)`;
8. il delta viene calcolato tramite tempo reale simulato, non contando solo i risvegli.

Preferenza del Design 006:

```text
se maturano più secondi insieme, chiamare Tick(n)
```

quindi il test principale deve aspettarsi `Tick(2)` se maturano due secondi reali.

---

## 23. Test obbligatori su sicurezza e concorrenza

Implementare test per:

1. nessun tick dopo `Stop()`;
2. nessun tick dopo `Dispose()`;
3. nessun doppio loop dopo `Start()` ripetuto;
4. nessun tick parallelo;
5. eccezione durante `Tick` ferma il runner;
6. eccezione durante `Tick` non lascia `IsRunning = true`;
7. eccezione durante `Tick` non lascia loop zombie;
8. dopo completamento normale del loop, `IsRunning` torna a `false`;
9. `RunLoopAsync` usa il token locale ricevuto e non rilegge `_cancellationTokenSource`.

Per il test sui tick paralleli, il fake orchestrator può bloccare temporaneamente una chiamata e verificare che una seconda non parta in parallelo.

---

## 24. Test architetturali / grep anti-regressione

Aggiungere verifiche per assicurare che `services/CicloTimer.App/Timing/` non contenga riferimenti vietati.

Controlli da eseguire:

```text
DispatcherTimer
System.Windows.Threading
System.Windows
Dispatcher
CicloTimer.Core
CicloTimer.Bridge
CicloTimer.Audio
CicloTimer.Localization
TimerBridgeAdapter
SystemActionDispatcher
```

Il coding agent deve segnalare errore se uno di questi riferimenti compare nei file del runner.

È accettabile che il progetto `CicloTimer.App` contenga riferimenti a Bridge e Audio altrove, perché l'orchestratore già li usa. Il divieto riguarda specificamente la cartella:

```text
services/CicloTimer.App/Timing/
```

---

## 25. Test anti-stringhe utente

Verificare che i file in `Timing` non introducano testi utente.

Il runner non deve produrre messaggi localizzati, messaggi accessibili o messaggi UI.

Sono ammesse solo eccezioni tecniche standard, ad esempio:

```csharp
ArgumentNullException
ObjectDisposedException
```

---

## 26. Verifiche da eseguire

Al termine della codifica, eseguire:

```bash
dotnet test
```

Se la solution richiede comandi più specifici, eseguire almeno:

```bash
dotnet test tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj
```

e poi l’intera suite.

Verificare anche:

```bash
dotnet build
```

oppure il comando già usato nel progetto per validare la solution.

---

## 27. Ordine operativo consigliato

Seguire questo ordine:

1. Leggere `006-DESIGN_gestore-timer-reale_v0.3.0.md`.
2. Verificare `ITimerAppOrchestrator`.
3. Creare cartella `Timing`.
4. Creare `IRealtimeTimerRunner`.
5. Creare scheletro `RealtimeTimerRunner`.
6. Implementare costruttore e proprietà `IsRunning`.
7. Implementare `Start()` con cattura atomica del token.
8. Implementare `Stop()`.
9. Implementare `Dispose()`.
10. Implementare `RunLoopAsync(CancellationToken cancellationToken)`.
11. Implementare timer periodico con `TimeProvider`.
12. Applicare `.ConfigureAwait(false)` agli `await` del loop.
13. Implementare calcolo `TimeSpan` delta/residuo.
14. Implementare primo tick non immediato.
15. Implementare gestione errori e `finally` di stop coerente.
16. Creare fake orchestrator per test.
17. Aggiungere supporto `FakeTimeProvider` nei test.
18. Scrivere test Start/Stop/Dispose.
19. Scrivere test tempo/drift/residuo.
20. Scrivere test no-loop-multipli/no-tick-paralleli.
21. Scrivere controlli architetturali.
22. Eseguire test App.
23. Eseguire tutti i test.
24. Correggere eventuali regressioni.
25. Verificare che nessun blocco 001–005 sia stato alterato inutilmente.

---

## 28. Criteri di accettazione

Il blocco 006 è accettabile solo se:

* `IRealtimeTimerRunner` esiste;
* `RealtimeTimerRunner` esiste;
* il runner usa `TimeProvider`;
* il runner usa `TimeSpan` per delta e residuo;
* il runner non usa `double` o `float` per il residuo;
* il runner chiama solo `ITimerAppOrchestrator.Tick(...)`;
* il runner non chiama `Tick(0)`;
* il runner compensa drift tramite `Tick(n)`;
* `Start()` è idempotente;
* `Start()` cattura il token locale in modo atomico;
* `RunLoopAsync` riceve il token come parametro;
* il loop non rilegge `_cancellationTokenSource`;
* `Stop()` è idempotente;
* `Stop()` dopo `Dispose()` è no-op sicuro;
* `Start → Stop → Start` funziona;
* `Dispose()` è idempotente;
* `Start()` dopo `Dispose()` genera `ObjectDisposedException`;
* `Start()` non produce tick immediato;
* il primo tick avviene solo dopo il primo intervallo temporale;
* ogni `await` nel loop usa `.ConfigureAwait(false)`;
* a fine loop `IsRunning` torna a `false`;
* non ci sono loop multipli;
* non ci sono tick paralleli;
* non ci sono dipendenze WPF;
* non ci sono dipendenze dirette da Core, Bridge, Audio o Localization nei file `Timing`;
* non ci sono stringhe utente;
* i test nuovi passano;
* i test precedenti passano.

---

## 29. Fuori perimetro esplicito

Non implementare in questo blocco:

* UI WPF;
* ViewModel;
* binding;
* pulsanti;
* visualizzazione tempo;
* comandi utente;
* accessibilità UI;
* annunci NVDA;
* gestione thread UI WPF;
* avvio automatico del runner da interfaccia grafica;
* configurazione durata da UI;
* logging strutturato;
* salvataggio impostazioni.

Il collegamento operativo tra UI, orchestratore e runner sarà definito nel blocco:

```text
007 — UI WPF minima
```

---

## 30. Note per Cursor / agente di codifica

L’agente deve rispettare il design senza inventare nuove responsabilità.

In particolare:

* non spostare logica nel Core;
* non modificare il Bridge per adattarlo al runner;
* non modificare l’AudioService;
* non introdurre UI;
* non semplificare il drift tornando a `Tick(1)` fisso;
* non sostituire `TimeProvider` con un’interfaccia custom;
* non usare `Task.Delay` come loop principale;
* non usare `DispatcherTimer`;
* non leggere `_cancellationTokenSource` dentro il loop;
* non fare `await` senza `.ConfigureAwait(false)` nel loop;
* non generare tick immediato allo `Start`.

Se durante la codifica emerge un problema con `TimeProvider.CreatePeriodicTimer`, fermarsi e documentare l’alternativa minima coerente con il design, senza introdurre dipendenze UI.

---

## 31. Esito atteso

Alla fine di questo coding plan il progetto deve avere un runner temporale reale, testabile e indipendente dalla UI.

Il sistema sarà pronto per il blocco successivo:

```text
007 — UI WPF minima
```

dove verrà collegato a una prima interfaccia utilizzabile.
