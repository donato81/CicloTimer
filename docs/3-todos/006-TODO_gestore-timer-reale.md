# TODO 006 — Gestore timer reale

## Metadati

* Tipo documento: TODO operativo
* Codice: 006
* Titolo: Gestore timer reale
* Versione: 0.2.0
* Stato: APPROVATO
* Progetto: CicloTimer
* Repository: donato81/CicloTimer
* Data: 2026-06-03
* Design di riferimento: `docs/1-design/006-DESIGN_gestore-timer-reale_v0.3.0.md`
* Coding plan di riferimento: `docs/2-coding-plans/006-PLAN_gestore-timer-reale.md`

---

## 1. Scopo del TODO

Questo documento traduce il Coding Plan 006 in una checklist operativa per la codifica reale.

Il blocco 006 deve introdurre il **gestore timer reale**, cioè il componente che misura il tempo effettivamente trascorso e invia all'orchestratore i secondi interi maturati.

Il componente deve restare fuori dalla UI WPF e deve parlare solo con:

```text
ITimerAppOrchestrator
```

---

## 2. Stato iniziale richiesto

Prima di iniziare, verificare che siano già presenti i blocchi 001–005:

```text
001 — Core timer engine
002 — Bridge UI-logica
003 — Sistema testi/localization
004 — Audio service e audio focus
005 — Orchestratore applicativo timer
```

Verificare che esistano:

```text
services/CicloTimer.App/
tests/CicloTimer.App.Tests/
```

Verificare che esista l'interfaccia:

```text
ITimerAppOrchestrator
```

e che contenga il metodo:

```csharp
Tick(int elapsedSeconds)
```

---

## 3. Vincoli assoluti

Durante questo blocco non introdurre:

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
Task.Delay come loop principale
System.Timers.Timer
System.Threading.Timer
```

Non modificare, salvo necessità documentata:

```text
models/CicloTimer.Core/
view-models/CicloTimer.Bridge/
services/CicloTimer.Audio/
locales/CicloTimer.Localization/
```

---

## 4. Preflight obbligatorio

Prima di creare o modificare file, eseguire questi controlli.

### 4.1 Verifica solution

* [ ] Aprire la solution del repository.
* [ ] Verificare che i progetti 001–005 compilino.
* [ ] Verificare che `services/CicloTimer.App` sia su `.NET 9`.
* [ ] Verificare che `tests/CicloTimer.App.Tests` sia su `.NET 9`.

### 4.2 Verifica interfaccia orchestratore

* [ ] Aprire il file che contiene `ITimerAppOrchestrator`.
* [ ] Annotare il namespace reale.
* [ ] Verificare la firma reale di `Tick(int elapsedSeconds)`.
* [ ] Verificare gli altri membri richiesti dall'interfaccia, per creare un fake di test coerente.

### 4.3 Verifica stile test esistente

* [ ] Aprire i test già presenti in `tests/CicloTimer.App.Tests`.
* [ ] Verificare framework usato.
* [ ] Verificare naming dei test.
* [ ] Verificare eventuale uso già presente di fakes/stub.
* [ ] Adattare i nuovi test allo stile esistente.

---

## 5. Creazione struttura file

### 5.1 Cartella produzione

* [ ] Creare cartella:

```text
services/CicloTimer.App/Timing/
```

### 5.2 File produzione

* [ ] Creare:

```text
services/CicloTimer.App/Timing/IRealtimeTimerRunner.cs
```

* [ ] Creare:

```text
services/CicloTimer.App/Timing/RealtimeTimerRunner.cs
```

### 5.3 Cartella test

* [ ] Creare cartella:

```text
tests/CicloTimer.App.Tests/Timing/
```

### 5.4 File test

* [ ] Creare:

```text
tests/CicloTimer.App.Tests/Timing/RealtimeTimerRunnerTests.cs
```

* [ ] Creare, se coerente con lo stile del repo:

```text
tests/CicloTimer.App.Tests/Timing/Fakes/FakeTimerAppOrchestrator.cs
```

Se i test esistenti usano una struttura diversa, adattare il percorso mantenendo chiaro che il fake appartiene ai test del blocco 006.

---

## 6. Implementazione interfaccia `IRealtimeTimerRunner`

Nel file:

```text
services/CicloTimer.App/Timing/IRealtimeTimerRunner.cs
```

implementare l'interfaccia minima:

```csharp
namespace CicloTimer.App.Timing;

public interface IRealtimeTimerRunner : IDisposable
{
    bool IsRunning { get; }

    void Start();

    void Stop();
}
```

Checklist:

* [ ] Namespace coerente con il progetto.
* [ ] Interfaccia pubblica.
* [ ] Estende `IDisposable`.
* [ ] Proprietà `IsRunning`.
* [ ] Metodo `Start()`.
* [ ] Metodo `Stop()`.
* [ ] Nessuna proprietà `LastError`.
* [ ] Nessun evento.
* [ ] Nessun metodo asincrono pubblico.
* [ ] Nessun riferimento UI.
* [ ] Nessuna stringa utente.

---

## 7. Implementazione classe `RealtimeTimerRunner`

Nel file:

```text
services/CicloTimer.App/Timing/RealtimeTimerRunner.cs
```

creare la classe:

```csharp
namespace CicloTimer.App.Timing;

public sealed class RealtimeTimerRunner : IRealtimeTimerRunner
{
}
```

Checklist base:

* [ ] Classe `public`.
* [ ] Classe `sealed`.
* [ ] Implementa `IRealtimeTimerRunner`.
* [ ] Non eredita da classi UI.
* [ ] Non usa WPF.
* [ ] Non usa `Dispatcher`.

---

## 8. Costruttore

Implementare il costruttore con dipendenze:

```csharp
ITimerAppOrchestrator orchestrator
TimeProvider? timeProvider = null
```

Checklist:

* [ ] `orchestrator` obbligatorio.
* [ ] Se `orchestrator` è `null`, lanciare `ArgumentNullException`.
* [ ] Se `timeProvider` è `null`, usare `TimeProvider.System`.
* [ ] Salvare orchestratore in campo privato readonly.
* [ ] Salvare time provider in campo privato readonly.
* [ ] Non istanziare Core.
* [ ] Non istanziare Bridge.
* [ ] Non istanziare AudioService.
* [ ] Non istanziare Localization.

---

## 9. Stato interno

Aggiungere i campi interni necessari.

Concetti obbligatori:

* [ ] oggetto di sincronizzazione;
* [ ] orchestratore;
* [ ] time provider;
* [ ] cancellation token source;
* [ ] task del loop;
* [ ] stato running;
* [ ] stato disposed;
* [ ] ultimo timestamp elaborato;
* [ ] residuo temporale.

Esempio concettuale:

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

Checklist:

* [ ] Lo stato `_disposed` è thread-safe.
* [ ] Lo stato `_isRunning` è thread-safe.
* [ ] `_remainder` è `TimeSpan`.
* [ ] Non usare `double` per il residuo.
* [ ] Non usare `float` per il residuo.

---

## 10. Proprietà `IsRunning`

Implementare `IsRunning`.

Checklist:

* [ ] Lettura thread-safe.
* [ ] Restituisce `true` solo quando il runner è attivo.
* [ ] Restituisce `false` dopo `Stop()`.
* [ ] Restituisce `false` dopo `Dispose()`.
* [ ] Restituisce `false` se il loop termina per errore.
* [ ] Non espone altri stati pubblici.

---

## 11. Metodo `Start()`

Implementare `Start()`.

Checklist funzionale:

* [ ] Entra in sezione sincronizzata.
* [ ] Se `_disposed` è `true`, lancia `ObjectDisposedException`.
* [ ] Se `_isRunning` è `true`, ritorna senza fare nulla.
* [ ] Crea nuovo `CancellationTokenSource`.
* [ ] Cattura il `CancellationToken` in variabile locale.
* [ ] Inizializza `_lastProcessedTime` con `_timeProvider.GetUtcNow()`.
* [ ] Inizializza `_remainder` a `TimeSpan.Zero`.
* [ ] Imposta `_isRunning = true`.
* [ ] Avvia `RunLoopAsync(token)`.
* [ ] Salva il task del loop in `_loopTask`.

Checklist anti-race:

* [ ] Creazione CTS e cattura token avvengono in modo atomico.
* [ ] L'assegnazione `_loopTask = RunLoopAsync(token)` avviene nella stessa sezione sincronizzata che crea il `CancellationTokenSource`, cattura il token e imposta `_isRunning = true`.
* [ ] `RunLoopAsync` riceve il token locale come parametro.
* [ ] Il loop non rilegge `_cancellationTokenSource`.
* [ ] `Start()` chiamato due volte non crea due loop.
* [ ] `Start → Stop → Start` crea un token nuovo e un loop nuovo.

---

## 12. Metodo `Stop()`

Implementare `Stop()`.

Checklist:

* [ ] Sicuro se il runner è già fermo.
* [ ] Sicuro se l'istanza è già disposed.
* [ ] Non lancia eccezione se chiamato due volte.
* [ ] Cancella il token corrente, se presente.
* [ ] Imposta `_isRunning = false`.
* [ ] Non fa `await`.
* [ ] Non blocca indefinitamente.
* [ ] Non smaltisce il CTS in modo pericoloso mentre il loop potrebbe ancora uscire.
* [ ] Non causa `ObjectDisposedException` spurie nel loop.

Regola:

```text
Stop dopo Dispose deve essere no-op sicuro.
```

---

## 13. Metodo `Dispose()`

Implementare `Dispose()`.

Checklist:

* [ ] Sicuro se chiamato una volta.
* [ ] Sicuro se chiamato più volte.
* [ ] Cancella il loop se attivo.
* [ ] Rilascia il `CancellationTokenSource` in modo sicuro.
* [ ] Imposta `_isRunning = false`.
* [ ] Imposta `_disposed = true`.
* [ ] Dopo `Dispose()`, `Start()` lancia `ObjectDisposedException`.
* [ ] Dopo `Dispose()`, `Stop()` è no-op sicuro.
* [ ] Dopo `Dispose()`, non vengono più generati tick.

---

## 14. Metodo privato `RunLoopAsync`

Creare metodo privato:

```csharp
private async Task RunLoopAsync(CancellationToken cancellationToken)
```

Checklist:

* [ ] Riceve il token come parametro.
* [ ] Non legge `_cancellationTokenSource`.
* [ ] Usa timer periodico basato su `TimeProvider`.
* [ ] Usa `.ConfigureAwait(false)` su ogni `await`.
* [ ] Non usa `DispatcherTimer`.
* [ ] Non usa `Task.Delay` come loop principale.
* [ ] Non usa `System.Timers.Timer`.
* [ ] Non usa `System.Threading.Timer`.
* [ ] Non usa UI.
* [ ] Intercetta internamente le eccezioni inattese.
* [ ] Non lascia il task del loop in stato faulted non osservato.

---

## 15. Timer periodico

Implementare il timer periodico.

Soluzione preferita:

```csharp
using var timer = _timeProvider.CreatePeriodicTimer(TimeSpan.FromSeconds(1));
```

Loop concettuale:

```csharp
while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
{
    if (cancellationToken.IsCancellationRequested)
    {
        break;
    }

    ProcessElapsedTime();
}
```

Checklist:

* [ ] Il primo tick non è immediato.
* [ ] Il primo controllo avviene dopo il primo intervallo.
* [ ] Questo comportamento è intenzionale.
* [ ] Il tempo effettivo viene calcolato con timestamp, non contando i risvegli.
* [ ] Ogni `await` usa `.ConfigureAwait(false)`.
* [ ] Verificare `cancellationToken.IsCancellationRequested` prima dell'elaborazione del delta.
* [ ] Non elaborare nuovi tick se la cancellazione è già stata richiesta.
* [ ] Uscire dal loop nel modo più pulito possibile.

### 15.1 Verifica preventiva della cancellazione

Prima di elaborare il tempo trascorso, verificare se è stata richiesta la cancellazione.

Schema concettuale:

```csharp
if (cancellationToken.IsCancellationRequested)
{
    break;
}
```

Checklist:

* [ ] Verificare `cancellationToken.IsCancellationRequested` prima dell'elaborazione del delta.
* [ ] Non elaborare nuovi tick se la cancellazione è già stata richiesta.
* [ ] Uscire dal loop nel modo più pulito possibile.

---

## 16. Calcolo tempo reale

Implementare il calcolo del delta.

Checklist:

* [ ] Leggere il tempo corrente con `_timeProvider.GetUtcNow()`.
* [ ] Calcolare `delta = now - _lastProcessedTime`.
* [ ] Sommare `delta` a `_remainder`.
* [ ] Estrarre i secondi interi con `(int)total.TotalSeconds`.
* [ ] Se `elapsedSeconds > 0`, chiamare `_orchestrator.Tick(elapsedSeconds)`.
* [ ] Se `elapsedSeconds == 0`, non chiamare `Tick`.
* [ ] Conservare residuo con `TimeSpan`.
* [ ] Aggiornare `_lastProcessedTime`.
* [ ] Non chiamare mai `Tick(0)`.
* [ ] Non chiamare mai `Tick` con valori negativi.
* [ ] Non usare `double` o `float` per conservare il residuo.

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

---

## 17. Prevenzione tick paralleli

Checklist:

* [ ] Il loop è sequenziale.
* [ ] Non usare `Task.Run` per chiamare `Tick`.
* [ ] Non usare fire-and-forget.
* [ ] Non usare `Parallel`.
* [ ] Non usare `ThreadPool.QueueUserWorkItem`.
* [ ] Chiamare direttamente `_orchestrator.Tick(elapsedSeconds)`.
* [ ] Proseguire solo dopo il completamento della chiamata.
* [ ] Non chiamare `Tick` dentro un lock globale del runner.

---

## 18. Gestione errori

Implementare gestione errori nel loop.

Checklist:

* [ ] Catturare `OperationCanceledException` come stop normale.
* [ ] Catturare errori inattesi.
* [ ] In caso di errore inatteso, fermare il runner in modo controllato.
* [ ] Non propagare eccezioni non osservate in background.
* [ ] Non lasciare `_loopTask` in stato faulted non osservato.
* [ ] Consumare internamente le eccezioni inattese del loop.
* [ ] Non esporre `LastError`.
* [ ] Non introdurre logging strutturato se non già presente.
* [ ] Usare `finally` o meccanismo equivalente per riportare `_isRunning = false`.

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
catch (Exception)
{
    // consumare l'errore
    // impedire faulted task non osservati
}
finally
{
    MarkStopped();
}
```

---

## 19. Metodo privato `MarkStopped`

Creare un metodo privato, o equivalente, per riportare lo stato a fermo.

Checklist:

* [ ] Esegue aggiornamento sotto lock.
* [ ] Imposta `_isRunning = false`.
* [ ] Non rilancia errori.
* [ ] Non cancella token già cancellati in modo pericoloso.
* [ ] Non chiama codice esterno.
* [ ] Non chiama orchestratore.
* [ ] Non usa UI.

Nome suggerito:

```csharp
private void MarkStopped()
```

Il nome può cambiare se lo stile del repo suggerisce altro.

---

## 20. Fake orchestrator per test

Creare fake coerente con `ITimerAppOrchestrator`.

Checklist:

* [ ] Implementa tutti i membri richiesti da `ITimerAppOrchestrator`.
* [ ] Registra le chiamate a `Tick`.
* [ ] Registra i valori passati a `Tick`.
* [ ] Può simulare eccezione durante `Tick`.
* [ ] Può rilevare tick paralleli.
* [ ] Può simulare un `Tick` lento tramite blocco controllato dal test.
* [ ] Può verificare che il runner non avvii un secondo `Tick` mentre il primo è ancora in esecuzione.
* [ ] Non usa timer reali.
* [ ] Non usa UI.
* [ ] Non introduce logica non necessaria.

Per rilevare tick paralleli, usare se utile un contatore atomico:

```csharp
private int _concurrentTicks;
```

Per simulare un `Tick` lento, il fake può usare un blocco controllato dal test, ad esempio un meccanismo basato su `ManualResetEventSlim`, `TaskCompletionSource` o equivalente coerente con lo stile dei test.

---

## 21. Dipendenza test `FakeTimeProvider`

Verificare se il pacchetto è già presente:

```text
Microsoft.Extensions.TimeProvider.Testing
```

Checklist:

* [ ] Se presente, usarlo.
* [ ] Se assente, aggiungerlo solo a `tests/CicloTimer.App.Tests`.
* [ ] Versione coerente con `.NET 9`.
* [ ] Non aggiungere il pacchetto ai progetti di produzione se non necessario.
* [ ] Non creare astrazione custom `ITimerDelay`.

---

## 22. Test funzionali

Implementare test per:

* [ ] `Start()` porta `IsRunning` a `true`.
* [ ] `Stop()` porta `IsRunning` a `false`.
* [ ] `Start()` chiamato due volte non crea due loop.
* [ ] `Stop()` chiamato due volte non genera errore.
* [ ] `Start → Stop → Start` funziona.
* [ ] `Dispose()` ferma il runner.
* [ ] `Dispose()` chiamato due volte non genera errore.
* [ ] `Start()` dopo `Dispose()` genera `ObjectDisposedException`.
* [ ] `Stop()` dopo `Dispose()` non genera errore.

---

## 23. Test sul tempo

Implementare test per:

* [ ] `Start()` non produce un tick immediato.
* [ ] Dopo 1 secondo simulato viene chiamato `Tick(1)`.
* [ ] Dopo 2 secondi simulati viene chiamato `Tick(2)`.
* [ ] Non viene mai chiamato `Tick(0)`.
* [ ] Un avanzamento inferiore a 1 secondo non produce tick.
* [ ] Il residuo inferiore a 1 secondo viene conservato.
* [ ] Due avanzamenti frazionari che insieme superano 1 secondo producono `Tick(1)`.
* [ ] Il delta viene calcolato tramite tempo reale simulato, non contando solo i risvegli.
* [ ] Il residuo è gestito come `TimeSpan`.

Nota:

la preferenza del Design 006 è:

```text
se maturano più secondi insieme, chiamare Tick(n)
```

quindi il comportamento atteso per due secondi maturati insieme è:

```text
Tick(2)
```

---

## 24. Test sicurezza e concorrenza

Implementare test per:

* [ ] Nessun tick dopo `Stop()`.
* [ ] Nessun tick dopo `Dispose()`.
* [ ] Nessun doppio loop dopo `Start()` ripetuto.
* [ ] Nessun tick parallelo.
* [ ] Eccezione durante `Tick` ferma il runner.
* [ ] Eccezione durante `Tick` non lascia `IsRunning = true`.
* [ ] Eccezione durante `Tick` non lascia loop zombie.
* [ ] Dopo completamento normale del loop, `IsRunning` torna a `false`.
* [ ] `RunLoopAsync` usa il token locale ricevuto.
* [ ] Il loop non rilegge `_cancellationTokenSource`.
* [ ] Nessuna eccezione non osservata proveniente da `_loopTask`.
* [ ] Il loop termina in stato completato o cancellato, mai faulted non osservato.

---

## 25. Test architetturali / grep anti-regressione

Verificare che i file in:

```text
services/CicloTimer.App/Timing/
```

non contengano riferimenti vietati.

Cercare:

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
Task.Delay
System.Timers.Timer
System.Threading.Timer
```

Checklist:

* [ ] Nessun riferimento a `DispatcherTimer`.
* [ ] Nessun riferimento a `System.Windows.Threading`.
* [ ] Nessun riferimento a `System.Windows`.
* [ ] Nessun riferimento a `Dispatcher`.
* [ ] Nessun riferimento diretto a Core.
* [ ] Nessun riferimento diretto a Bridge.
* [ ] Nessun riferimento diretto ad Audio.
* [ ] Nessun riferimento diretto a Localization.
* [ ] Nessun riferimento a `TimerBridgeAdapter`.
* [ ] Nessun riferimento a `SystemActionDispatcher`.
* [ ] Nessun uso di `Task.Delay` come loop principale.
* [ ] Nessun uso di `System.Timers.Timer`.
* [ ] Nessun uso di `System.Threading.Timer`.

Nota:

è accettabile che `services/CicloTimer.App` usi Bridge e Audio altrove.
Il divieto riguarda specificamente la cartella `Timing`.

---

## 26. Test anti-stringhe utente

Verificare che i file in `Timing` non introducano testi utente.

Checklist:

* [ ] Nessun messaggio UI.
* [ ] Nessun messaggio accessibile.
* [ ] Nessuna nuova chiave localization.
* [ ] Nessuna frase utente.
* [ ] Ammesse solo eccezioni tecniche standard, come `ArgumentNullException` e `ObjectDisposedException`.

---

## 27. Build e test

Eseguire:

```bash
dotnet build
```

Eseguire:

```bash
dotnet test
```

Se serve una verifica mirata:

```bash
dotnet test tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj
```

Checklist:

* [ ] Build completata.
* [ ] Test App completati.
* [ ] Test intera solution completati.
* [ ] Nessun test 001–005 rotto.
* [ ] Nessun warning nuovo rilevante, se il progetto tratta i warning come qualità.
* [ ] Nessuna regressione architetturale.

---

## 28. Verifica manuale finale

Prima di chiudere il blocco, verificare manualmente:

* [ ] `IRealtimeTimerRunner` è minimale.
* [ ] `RealtimeTimerRunner` usa `TimeProvider`.
* [ ] `RealtimeTimerRunner` usa `TimeSpan` per residuo.
* [ ] Non esiste `LastError`.
* [ ] Non esistono eventi pubblici.
* [ ] Non esistono metodi async pubblici.
* [ ] `Start()` è idempotente.
* [ ] `Stop()` è idempotente.
* [ ] `Dispose()` è idempotente.
* [ ] `Start()` dopo `Dispose()` genera `ObjectDisposedException`.
* [ ] `Stop()` dopo `Dispose()` è no-op sicuro.
* [ ] Il primo tick non è immediato.
* [ ] Il token viene catturato localmente.
* [ ] `RunLoopAsync` riceve il token come parametro.
* [ ] Il loop non legge `_cancellationTokenSource`.
* [ ] Gli `await` usano `.ConfigureAwait(false)`.
* [ ] `_loopTask` non può rimanere faulted senza osservazione.
* [ ] Le eccezioni inattese del loop vengono consumate internamente.
* [ ] Il loop verifica `IsCancellationRequested` prima dell'elaborazione del delta.
* [ ] `_loopTask = RunLoopAsync(token)` avviene nella stessa sezione sincronizzata che crea il token.
* [ ] Nessuna UI è stata introdotta.
* [ ] Nessuna modifica inutile ai blocchi 001–005.

---

## 29. File attesi a fine blocco

A fine implementazione devono esistere almeno:

```text
services/CicloTimer.App/Timing/IRealtimeTimerRunner.cs
services/CicloTimer.App/Timing/RealtimeTimerRunner.cs
tests/CicloTimer.App.Tests/Timing/RealtimeTimerRunnerTests.cs
```

Possibile file aggiuntivo:

```text
tests/CicloTimer.App.Tests/Timing/Fakes/FakeTimerAppOrchestrator.cs
```

solo se coerente con lo stile dei test.

---

## 30. Criteri di completamento

Il blocco 006 è completato solo se:

* [ ] tutti i file previsti sono stati creati;
* [ ] il runner comunica solo con `ITimerAppOrchestrator`;
* [ ] il runner usa `TimeProvider`;
* [ ] il runner usa `TimeSpan`;
* [ ] il runner compensa il drift temporale;
* [ ] il runner non chiama `Tick(0)`;
* [ ] il runner chiama `Tick(n)` quando maturano più secondi;
* [ ] il runner conserva il residuo inferiore al secondo;
* [ ] non ci sono loop multipli;
* [ ] non ci sono tick paralleli;
* [ ] `Start → Stop → Start` funziona;
* [ ] `Dispose()` chiude correttamente il runner;
* [ ] `Start()` dopo `Dispose()` genera `ObjectDisposedException`;
* [ ] `Stop()` dopo `Dispose()` è no-op sicuro;
* [ ] Nessuna eccezione non osservata proveniente dal loop asincrono;
* [ ] Il loop consuma internamente gli errori inattesi;
* [ ] Il loop verifica la cancellazione prima dell'elaborazione del delta;
* [ ] `_loopTask` viene creato nella stessa sezione sincronizzata che crea il token;
* [ ] i test nuovi passano;
* [ ] i test vecchi passano;
* [ ] non sono state introdotte dipendenze UI;
* [ ] non sono state introdotte stringhe utente;
* [ ] non sono state modificate responsabilità di Core, Bridge, Audio o Localization.

---

## 31. Fuori perimetro

Non fare in questo blocco:

```text
UI WPF
ViewModel
binding
pulsanti
visualizzazione tempo
comandi utente
accessibilità UI
annunci NVDA
gestione thread UI
avvio automatico da interfaccia grafica
configurazione durata da UI
logging strutturato
persistenza impostazioni
```

Questi aspetti appartengono al blocco successivo:

```text
007 — UI WPF minima
```

---

## 32. Esito atteso

Alla fine del blocco 006 il progetto avrà un runner reale e testabile che fa avanzare il timer usando il tempo fisico.

L'applicazione non avrà ancora una UI completa, ma sarà pronta per collegare orchestratore e runner alla futura interfaccia WPF.
