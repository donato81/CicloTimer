# DESIGN 006 — Gestore timer reale

## Metadati

- Tipo documento: Design tecnico
- Codice: 006
- Titolo: Gestore timer reale
- Versione: 0.3.0
- Stato: DRAFT
- Progetto: CicloTimer
- Repository: donato81/CicloTimer
- Data: 2026-06-03

---

## 1. Scopo del documento

Questo documento definisce il design del componente responsabile di far avanzare realmente il tempo dell'applicazione.

I blocchi precedenti hanno già introdotto:

- il motore logico del timer;
- il bridge tra logica e dati pronti per la UI;
- il sistema di testi centralizzati;
- il servizio audio;
- l'orchestratore applicativo.

Questi componenti sanno reagire al passare del tempo, ma non generano autonomamente il tempo reale.

Il blocco 006 introduce quindi il **gestore timer reale**, cioè il componente che osserva il passare del tempo fisico e invia all'orchestratore i secondi effettivamente maturati.

---

## 2. Stato precedente

Il Core espone già il metodo:

```csharp
Tick(int elapsedSeconds)
````

Il Bridge espone già un metodo equivalente.

L'orchestratore applicativo espone già:

```csharp
Tick(int elapsedSeconds)
```

Il blocco 005 ha introdotto l'orchestratore, ma ha escluso esplicitamente:

* timer reale;
* `DispatcherTimer`;
* `System.Timers.Timer`;
* `System.Threading.Timer`;
* `Task.Delay` in loop;
* UI WPF;
* `ICommand`;
* `INotifyPropertyChanged`;
* accessibilità UI;
* Live Region.

Quindi il blocco 006 deve colmare solo il vuoto rimasto: produrre il tempo reale e consegnarlo all'orchestratore.

---

## 3. Problema da risolvere

Attualmente l'applicazione sa cosa fare quando riceve un tick, ma non ha ancora un componente che produca tick reali.

Serve un servizio che:

* avvii un ciclo temporale reale;
* misuri il tempo trascorso;
* trasformi il tempo reale in secondi interi maturati;
* chiami l'orchestratore;
* eviti tick paralleli;
* si fermi in modo sicuro;
* possa essere avviato, fermato e riavviato;
* sia testabile senza attendere secondi reali;
* non dipenda dalla UI WPF.

---

## 4. Obiettivo del blocco 006

L'obiettivo è introdurre un runner temporale applicativo.

Il runner deve:

* vivere fuori dal Core;
* non modificare la logica del timer;
* non conoscere direttamente il Bridge;
* non conoscere direttamente il servizio audio;
* non conoscere la UI;
* comunicare solo con `ITimerAppOrchestrator`;
* usare il tempo reale in modo controllato;
* compensare eventuali ritardi del sistema;
* essere idempotente su `Start()` e `Stop()`;
* essere smaltibile con `Dispose()`;
* essere testabile tramite astrazione standard del tempo.

---

## 5. Non obiettivi

Questo blocco non deve introdurre:

* UI WPF;
* `MainWindow`;
* XAML;
* binding grafici;
* `ICommand`;
* `INotifyPropertyChanged`;
* gestione NVDA;
* Live Region;
* testi visibili all'utente;
* nuove chiavi di localizzazione utente;
* nuova logica Core;
* nuove regole di stato del timer;
* nuove regole audio;
* nuove regole del Bridge;
* persistenza;
* configurazione utente;
* notifiche di sistema;
* logging strutturato, se non già presente.

Il blocco 006 non decide cosa mostrare.

Il blocco 006 non decide quando suonare.

Il blocco 006 non decide lo stato del timer.

Il blocco 006 produce solo tempo reale verso l'orchestratore.

---

## 6. Posizione architetturale

Il gestore timer reale si colloca sopra l'orchestratore applicativo.

Flusso previsto:

```text
Gestore timer reale
   ↓
ITimerAppOrchestrator.Tick(secondiInteriMaturati)
   ↓
Bridge
   ↓
Core
   ↓
SystemActionDispatcher
   ↓
AudioService
```

Il nuovo componente non parla direttamente con:

* Core;
* Bridge;
* AudioService;
* Localization;
* UI WPF.

Il suo unico interlocutore applicativo è:

```csharp
ITimerAppOrchestrator
```

---

## 7. Posizione nel repository

Il componente deve essere inserito nel progetto applicativo già esistente:

```text
services/CicloTimer.App/
```

Cartella proposta:

```text
services/CicloTimer.App/Timing/
```

Motivazione:

* il runner non appartiene alla logica pura del Core;
* non appartiene al Bridge;
* non appartiene all'Audio;
* non appartiene alla UI;
* è infrastruttura applicativa che serve a far vivere l'orchestrazione già esistente;
* evitare un nuovo progetto separato mantiene il blocco 006 più semplice e coerente con l'attuale struttura.

La cartella si chiama `Timing` e non `Runtime` perché il suo scopo è specifico: gestione del tempo reale.

---

## 8. Vincolo di dipendenza interna

Anche se il runner vive dentro `CicloTimer.App`, deve rispettare un confine rigoroso.

I file in:

```text
services/CicloTimer.App/Timing/
```

possono dipendere solo da:

* `ITimerAppOrchestrator`;
* tipi di sistema .NET;
* `TimeProvider`;
* tipi minimi necessari per threading e cancellazione.

Non devono dipendere direttamente da:

* `TimerBridgeAdapter`;
* `SystemActionDispatcher`;
* Core;
* Bridge;
* AudioService;
* Localization;
* WPF.

Questo vincolo dovrà essere verificato nel coding plan e nel todo tramite test architetturale, controllo statico o grep mirato.

---

## 9. Nome del servizio

Nome proposto dell'interfaccia:

```csharp
IRealtimeTimerRunner
```

Nome proposto dell'implementazione:

```csharp
RealtimeTimerRunner
```

Il termine `Runner` indica che il componente fa girare il tempo reale, ma non contiene la logica del timer.

---

## 10. Interfaccia pubblica proposta

L'interfaccia pubblica deve restare minima.

```csharp
public interface IRealtimeTimerRunner : IDisposable
{
    bool IsRunning { get; }

    void Start();

    void Stop();
}
```

Significato:

* `IsRunning` indica se il runner sta producendo tempo;
* `Start()` avvia il ciclo temporale;
* `Stop()` ferma il ciclo temporale;
* `Dispose()` ferma il runner e libera le risorse.

Non deve essere esposta una proprietà pubblica `LastError`.

---

## 11. Decisione su LastError

Il runner non deve esporre pubblicamente l'ultimo errore.

Motivazioni:

* l'obiettivo del runner è produrre tick, non diventare un reporter diagnostico;
* esporre errori pubblici spingerebbe la futura UI a interpretarli;
* la gestione diagnostica strutturata è fuori perimetro 006;
* in caso di errore inatteso, il runner deve fermarsi in modo coerente.

Regola:

```text
Nessuna proprietà LastError nell'interfaccia pubblica.
```

L'implementazione può conservare internamente un errore solo per debug locale, ma non deve farne parte del contratto.

---

## 12. Tecnologia temporale scelta

Il progetto usa `.NET 9`, quindi il design adotta lo standard moderno:

```csharp
TimeProvider
```

In produzione il runner userà:

```csharp
TimeProvider.System
```

Nei test si userà un provider controllabile, ad esempio:

```text
FakeTimeProvider
```

o equivalente compatibile con i test del progetto.

Motivazione:

* `TimeProvider` è standard .NET;
* evita interfacce custom non necessarie;
* rende i test veloci e deterministici;
* permette di simulare il passare del tempo senza attendere secondi reali.

Decisione finale:

```text
Usare TimeProvider.
Non introdurre ITimerDelay custom.
```

---

## 13. Tecnologia di ticking

Il runner deve usare un meccanismo temporale non legato alla UI.

Scelta proposta:

```csharp
TimeProvider.CreatePeriodicTimer(...)
```

oppure un uso equivalente di `PeriodicTimer` collegato a `TimeProvider`, se più adatto all'implementazione concreta.

L'intervallo base resta:

```text
1 secondo
```

Tecnologie vietate in questo blocco:

```text
DispatcherTimer
System.Windows.Threading.DispatcherTimer
```

Motivazione:

* `DispatcherTimer` legherebbe il blocco 006 alla UI WPF;
* la UI appartiene al blocco 007;
* il runner deve poter essere testato senza thread grafico.

Nota importante:

il risveglio periodico serve solo ad attivare il controllo del tempo.
Il calcolo del tempo realmente trascorso non deve basarsi sul semplice conteggio dei risvegli del timer.

---

## 14. Correzione del drift temporale

Il runner non deve assumere che ogni risveglio del timer equivalga sempre a un secondo esatto.

I timer del sistema operativo non sono perfettamente precisi. In alcune situazioni il ciclo potrebbe riprendere dopo più di un secondo.

Per evitare che il timer applicativo resti indietro, il runner deve misurare il tempo reale trascorso.

Regola:

```text
Il runner calcola i secondi interi realmente maturati e chiama l'orchestratore con quel valore.
```

Il calcolo deve avvenire confrontando timestamp letti da `TimeProvider`.

Esempio concettuale:

```csharp
DateTimeOffset now = timeProvider.GetUtcNow();
TimeSpan delta = now - lastProcessedTime;
```

Il delta deve essere gestito come `TimeSpan`.

Non devono essere usati `double` o `float` per conservare il residuo temporale.

Esempi logici:

```text
Tempo trascorso reale: 1,0 secondi
→ orchestrator.Tick(1)

Tempo trascorso reale: 2,0 secondi
→ orchestrator.Tick(2)

Tempo trascorso reale: 1,4 secondi
→ orchestrator.Tick(1)
→ conserva internamente 0,4 secondi come TimeSpan residuo

Tempo trascorso reale: 0,8 secondi
→ non chiama Tick
→ conserva il residuo come TimeSpan
```

Questa scelta evita il drift senza modificare Core, Bridge o Orchestratore.

---

## 15. Delta ammesso

Il runner deve chiamare:

```csharp
orchestrator.Tick(elapsedSeconds)
```

dove `elapsedSeconds` è un intero maggiore di zero.

Regole:

* non chiamare `Tick(0)`;
* non chiamare `Tick` con valori negativi;
* chiamare `Tick(1)` nei casi normali;
* chiamare `Tick(n)` quando sono maturati più secondi reali;
* conservare il residuo inferiore a un secondo.

---

## 16. Residuo temporale

Il runner deve conservare internamente il tempo inferiore a un secondo non ancora trasformato in tick.

Il residuo deve essere rappresentato con `TimeSpan`.

Non deve essere rappresentato con `double`, `float` o altri tipi a virgola mobile.

Esempio:

```text
Passano 1,4 secondi:
- invia Tick(1)
- conserva 0,4 secondi come TimeSpan

Poi passano altri 0,7 secondi:
- totale residuo = 1,1 secondi
- invia Tick(1)
- conserva 0,1 secondi come TimeSpan
```

Questo rende il timer più fedele al tempo reale senza cambiare il contratto esistente basato su secondi interi.

Implementazione concettuale:

```csharp
TimeSpan total = accumulatedRemainder + (now - lastProcessedTime);
int elapsedSeconds = (int)total.TotalSeconds;
TimeSpan remainder = total - TimeSpan.FromSeconds(elapsedSeconds);
```

Il coding plan dovrà definire i dettagli esatti, ma il principio è vincolante.

---

## 17. Avvio del runner

Quando `Start()` viene chiamato:

* se il runner è fermo, deve creare un nuovo ciclo temporale;
* deve creare un nuovo `CancellationTokenSource`;
* deve iniziare il loop interno;
* deve impostare `IsRunning = true`;
* deve inizializzare il riferimento temporale di partenza tramite `TimeProvider.GetUtcNow()`;
* deve inizializzare il residuo temporale interno a `TimeSpan.Zero`.

Se `Start()` viene chiamato mentre il runner è già attivo:

```text
non deve creare un secondo loop.
```

`Start()` deve essere idempotente.

---

## 18. Stop del runner

Quando `Stop()` viene chiamato:

* deve richiedere la cancellazione del ciclo tramite `CancellationTokenSource`;
* deve impedire nuovi tick;
* deve rilasciare le risorse temporali interne;
* deve impostare `IsRunning = false`;
* deve essere sicuro anche se il runner è già fermo.

Se `Stop()` viene chiamato due volte:

```text
non deve generare errore.
```

`Stop()` deve essere idempotente.

---

## 19. Riavvio dopo Stop

Dopo uno `Stop()`, deve essere possibile chiamare nuovamente `Start()`.

Il nuovo avvio deve:

* creare un nuovo ciclo;
* creare un nuovo `CancellationTokenSource`;
* ripartire da un riferimento temporale pulito;
* ripartire con residuo temporale a `TimeSpan.Zero`;
* non riutilizzare un timer già cancellato o già disposto.

Regola:

```text
Start → Stop → Start deve funzionare.
```

---

## 20. Dispose

`Dispose()` deve:

* fermare il runner se è attivo;
* cancellare il ciclo temporale;
* rilasciare il `CancellationTokenSource`;
* rilasciare il timer periodico;
* essere sicuro se chiamato più volte.

Dopo `Dispose()`:

* il runner non deve generare altri tick;
* l'istanza è considerata definitivamente non riutilizzabile.

Decisione finale:

```text
Dopo Dispose, una chiamata a Start() deve generare ObjectDisposedException.
```

Motivazione:

* è il comportamento più coerente con gli oggetti .NET disposable;
* evita riutilizzi ambigui;
* rende il contratto più chiaro;
* consente test espliciti.

`Stop()` dopo `Dispose()` può essere ignorato in modo sicuro oppure trattato come no-op, purché il coding plan lo renda esplicito.

---

## 21. Prevenzione di loop multipli

Il runner deve garantire che esista al massimo un ciclo temporale attivo.

Questo è più importante della semplice prevenzione dei tick sovrapposti.

Rischio da evitare:

```text
Start chiamato due volte → due loop paralleli → doppia velocità del timer
```

Regola:

```text
mai più di un loop attivo per istanza del runner.
```

Implementazione prevista:

* lock interno;
* oppure meccanismo atomico equivalente;
* stato `_isRunning`;
* stato `_disposed`;
* controllo del `CancellationTokenSource` attivo.

La scelta precisa sarà definita nel coding plan, ma il vincolo è obbligatorio.

Nota per il coding plan:

il loop principale sarà asincrono, mentre `Start()` e `Stop()` saranno sincroni.
La sincronizzazione tra stato sincrono e ciclo asincrono dovrà essere progettata con attenzione, evitando deadlock e loop zombie.

---

## 22. Prevenzione di tick sovrapposti

Il runner deve attendere il completamento di un tick prima di procedere con il successivo.

Regola:

```text
Il runner non deve eseguire due chiamate a orchestrator.Tick(...) contemporaneamente.
```

Anche se l'orchestratore è già protetto da lock interno, il runner deve mantenere un comportamento ordinato.

Il loop interno deve essere sequenziale:

```text
attendi risveglio temporale
leggi TimeProvider.GetUtcNow()
calcola delta come TimeSpan
calcola secondi interi maturati
chiama orchestrator.Tick(...)
attendi completamento logico della chiamata
aggiorna riferimento temporale e residuo
ripeti
```

---

## 23. Gestione errori

Se durante il tick si verifica un errore inatteso:

* il runner deve intercettarlo;
* deve fermarsi in modo controllato;
* deve impostare `IsRunning = false`;
* non deve lasciare loop zombie attivi;
* non deve propagare eccezioni non osservate in background.

Il runner non deve esporre pubblicamente `LastError`.

Se in futuro verrà introdotto un sistema di logging o diagnostica, potrà essere integrato in un blocco dedicato o in una revisione successiva.

Per il blocco 006 la regola è:

```text
errore inatteso durante il loop → stop controllato
```

---

## 24. Relazione con pausa, ripresa e reset

Il runner non interpreta i comandi applicativi.

Non deve sapere se il timer è:

* fermo;
* in esecuzione;
* in pausa;
* in avviso finale.

Queste informazioni appartengono all'orchestratore, al Bridge e al Core.

Il runner si limita a produrre tempo quando è attivo.

La futura UI o il futuro ViewModel decideranno quando chiamare:

* `orchestrator.Start()`;
* `runner.Start()`;
* `orchestrator.Pause()`;
* `runner.Stop()`;
* `orchestrator.Resume()`;
* `runner.Start()`;
* `orchestrator.Reset()`;
* `runner.Stop()`.

Queste connessioni operative appartengono al blocco 007.

---

## 25. Nota per il blocco 007

Il runner non aggiorna direttamente la UI.

Poiché il timer reale lavora fuori dal thread grafico WPF, il blocco 007 dovrà gestire correttamente il passaggio verso il thread UI.

Regola per il futuro blocco 007:

```text
La UI/ViewModel dovrà aggiornare le proprietà grafiche sul thread WPF corretto.
```

Il blocco 006 non introduce `Dispatcher`, non introduce ViewModel e non introduce logica UI.

---

## 26. Testabilità

I test del runner non devono attendere secondi reali.

Dato che il progetto usa .NET 9, i test devono sfruttare `TimeProvider` controllabile.

Obiettivi dei test:

* simulare il passare di 1 secondo;
* simulare il passare di più secondi;
* simulare residui inferiori al secondo;
* verificare che `Tick` venga chiamato con il delta corretto;
* verificare Start/Stop/Dispose senza attese reali;
* verificare assenza di loop multipli;
* verificare assenza di tick paralleli;
* verificare `ObjectDisposedException` dopo `Dispose()` e successivo `Start()`.

---

## 27. File previsti

File principali proposti:

```text
services/CicloTimer.App/Timing/IRealtimeTimerRunner.cs
services/CicloTimer.App/Timing/RealtimeTimerRunner.cs
```

Eventuali file interni, se utili:

```text
services/CicloTimer.App/Timing/RealtimeTimerRunnerOptions.cs
```

Solo se serve configurare intervallo interno o comportamento di test.

File di test proposti:

```text
tests/CicloTimer.App.Tests/Timing/RealtimeTimerRunnerTests.cs
tests/CicloTimer.App.Tests/Timing/Fakes/FakeTimerAppOrchestrator.cs
```

Se il pacchetto di test per `FakeTimeProvider` viene usato direttamente, non serve creare un fake temporale custom.

---

## 28. Dipendenze consentite

Il runner può dipendere da:

```text
CicloTimer.App.Abstractions.ITimerAppOrchestrator
System
System.Threading
System.Threading.Tasks
System.TimeProvider
```

Il runner non deve dipendere da:

```text
CicloTimer.Core
CicloTimer.Bridge
CicloTimer.Audio
CicloTimer.Localization
System.Windows
System.Windows.Threading
WPF
```

Nota:

Il progetto `CicloTimer.App` può già dipendere da Bridge e Audio per l'orchestratore, ma il nuovo runner non deve usarli direttamente.

---

## 29. Regole architetturali obbligatorie

Il runner deve rispettare queste regole:

1. Non calcola il tempo rimanente visuale.
2. Non cambia direttamente lo stato del Core.
3. Non produce testi utente.
4. Non produce audio.
5. Non conosce la UI.
6. Non usa `DispatcherTimer`.
7. Non usa `Dispatcher`.
8. Non crea più di un loop attivo.
9. Non esegue tick paralleli.
10. Non espone `LastError`.
11. Non duplica responsabilità dell'orchestratore.
12. Non modifica i contratti 001–005 senza necessità.
13. Non rompe i test già esistenti.
14. Usa `TimeProvider`.
15. Calcola il delta tramite timestamp ottenuti da `TimeProvider.GetUtcNow()`.
16. Usa `TimeSpan` per delta e residuo.
17. Non usa `double` o `float` per il residuo temporale.
18. Compensa il drift temporale tramite secondi interi maturati e residuo interno.
19. Dopo `Dispose()`, `Start()` genera `ObjectDisposedException`.

---

## 30. Test obbligatori

Il blocco 006 deve prevedere test per verificare che:

1. `Start()` avvia il runner.
2. `Start()` chiamato due volte non crea due loop.
3. `Stop()` ferma il runner.
4. `Stop()` chiamato due volte non genera errore.
5. Dopo `Stop()` è possibile chiamare di nuovo `Start()`.
6. `Dispose()` ferma il runner.
7. `Dispose()` chiamato più volte non genera errore.
8. Dopo `Dispose()` il runner non genera più tick.
9. Dopo `Dispose()`, una chiamata a `Start()` genera `ObjectDisposedException`.
10. Il runner chiama `ITimerAppOrchestrator.Tick(1)` quando matura un secondo.
11. Il runner chiama `ITimerAppOrchestrator.Tick(2)` quando maturano due secondi reali.
12. Il runner non chiama `Tick(0)`.
13. Il runner conserva il residuo inferiore a un secondo.
14. Il residuo viene gestito come `TimeSpan`.
15. Il runner calcola il delta tramite `TimeProvider.GetUtcNow()`.
16. Il runner non esegue tick paralleli.
17. Un errore durante `Tick` ferma il runner in modo controllato.
18. Un errore durante `Tick` non lascia loop zombie.
19. Il runner non chiama direttamente Core.
20. Il runner non chiama direttamente Bridge.
21. Il runner non chiama direttamente AudioService.
22. Il runner non chiama direttamente Localization.
23. Il runner non usa WPF.
24. Il runner non usa `DispatcherTimer`.
25. Il runner non usa `Dispatcher`.
26. I test non attendono secondi reali.
27. I test usano tempo controllabile tramite `TimeProvider`.
28. Tutti i test dei blocchi 001–005 continuano a passare.

---

## 31. Criterio di completamento

Il blocco 006 è completato quando:

* esiste `IRealtimeTimerRunner`;
* esiste `RealtimeTimerRunner`;
* il runner usa `TimeProvider`;
* il runner genera avanzamento temporale reale;
* il runner calcola il tempo trascorso tramite timestamp ottenuti da `TimeProvider`;
* il runner usa `TimeSpan` per delta e residuo;
* il runner chiama l'orchestratore con i secondi interi maturati;
* il runner compensa ritardi tramite calcolo del delta reale;
* il runner conserva residui inferiori al secondo;
* `Start()` è idempotente;
* `Stop()` è idempotente;
* `Start → Stop → Start` funziona;
* `Dispose()` è sicuro;
* `Start()` dopo `Dispose()` genera `ObjectDisposedException`;
* non esistono loop multipli;
* non esistono tick paralleli;
* non vengono introdotte dipendenze UI;
* non vengono modificati Core, Bridge, Audio o Localization;
* tutti i test 001–006 passano.

---

## 32. Rischi principali

### Rischio 1 — Duplicazione della logica timer

Il runner potrebbe iniziare a decidere stati, tempo rimanente o sessioni.

Mitigazione:

```text
Il runner misura solo tempo reale e invia secondi maturati all'orchestratore.
```

---

### Rischio 2 — Dipendenza prematura da WPF

L'uso di `DispatcherTimer` o `Dispatcher` porterebbe logica UI dentro il blocco 006.

Mitigazione:

```text
Usare TimeProvider e timer non UI.
Rinviare ogni gestione WPF al blocco 007.
```

---

### Rischio 3 — Drift temporale

Se ogni risveglio venisse trattato come un secondo esatto, il timer potrebbe accumulare ritardo.

Mitigazione:

```text
Calcolare il delta reale tramite TimeProvider.GetUtcNow().
Usare TimeSpan.
Chiamare Tick(secondiInteriMaturati).
Conservare il residuo temporale come TimeSpan.
```

---

### Rischio 4 — Loop multipli

Due chiamate consecutive a `Start()` potrebbero creare due cicli temporali.

Mitigazione:

```text
Start idempotente e protezione interna con lock o meccanismo atomico.
```

---

### Rischio 5 — Tick paralleli

Un nuovo tick potrebbe partire mentre il precedente non è ancora terminato.

Mitigazione:

```text
Loop sequenziale: attendere il completamento di Tick prima di procedere.
```

---

### Rischio 6 — Stop incompleto

Se il `CancellationTokenSource` non viene gestito correttamente, il runner potrebbe continuare a girare dopo `Stop()`.

Mitigazione:

```text
Stop cancella il token, rilascia le risorse e porta IsRunning a false.
```

---

### Rischio 7 — Test lenti o instabili

Test basati su secondi reali sarebbero fragili.

Mitigazione:

```text
Usare TimeProvider controllabile nei test.
```

---

### Rischio 8 — Riutilizzo dopo Dispose

Un'istanza già dismessa potrebbe essere riavviata per errore.

Mitigazione:

```text
Dopo Dispose, Start genera ObjectDisposedException.
```

---

### Rischio 9 — Sincronizzazione errata tra codice sincrono e loop asincrono

`Start()` e `Stop()` sono sincroni, mentre il loop temporale è asincrono.

Mitigazione:

```text
Il coding plan dovrà definire una strategia di sincronizzazione chiara.
La soluzione dovrà evitare deadlock, race condition e loop zombie.
```

---

## 33. Decisioni finali del Design 006

Le decisioni per questa versione sono:

1. Il componente si chiama `RealtimeTimerRunner`.
2. L'interfaccia si chiama `IRealtimeTimerRunner`.
3. Il componente vive in `services/CicloTimer.App/Timing/`.
4. Non viene creato un nuovo progetto `CicloTimer.Runtime`.
5. Il runner comunica solo con `ITimerAppOrchestrator`.
6. Il runner usa `TimeProvider`.
7. Non viene introdotto `ITimerDelay`.
8. Il runner non chiama sempre `Tick(1)` in modo cieco.
9. Il runner calcola i secondi interi maturati.
10. Il runner calcola il delta tramite timestamp ottenuti da `TimeProvider.GetUtcNow()`.
11. Il runner usa `TimeSpan` per delta e residuo.
12. Il runner conserva il residuo inferiore al secondo.
13. Il runner non usa tipi a virgola mobile per il residuo temporale.
14. Il runner non espone `LastError`.
15. `Start()` è idempotente.
16. `Stop()` è idempotente.
17. `Dispose()` è sicuro.
18. Dopo `Dispose()`, `Start()` genera `ObjectDisposedException`.
19. Non ci devono essere loop multipli.
20. Non ci devono essere tick paralleli.
21. Nessuna UI WPF viene introdotta.
22. Il marshalling verso il thread UI è rinviato al blocco 007.
23. La sincronizzazione tra metodi sincroni e loop asincrono sarà dettagliata nel coding plan.

---

## 34. Esito atteso

Alla fine del blocco 006 l'applicazione avrà un componente capace di far avanzare il timer usando il tempo reale.

Non esisterà ancora una UI completa.

Il sistema sarà però pronto per essere collegato alla futura UI WPF minima.

Il blocco successivo naturale sarà:

```text
007 — UI WPF minima
```

Il blocco 007 userà orchestratore e runner per rendere l'applicazione utilizzabile dall'utente.

