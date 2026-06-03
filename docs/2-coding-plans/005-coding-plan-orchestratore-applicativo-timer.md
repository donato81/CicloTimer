# CicloTimer — Coding Plan 005 — Orchestratore applicativo timer

**Tipo documento:** coding plan  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/1-design/004-design-audio-service-e-audio-focus.md, docs/1-design/005-design-orchestratore-applicativo-timer.md, docs/2-coding-plans/002-coding-plan-bridge-ui-logica-timer.md, docs/2-coding-plans/004-coding-plan-audio-service-e-audio-focus.md, docs/3-todos/002-todo-bridge-ui-logica-timer.md, docs/3-todos/004-todo-audio-service-e-audio-focus.md  

---

## 1. Scopo del documento

Questo documento traduce il Design 005 approvato in un piano operativo di codifica.

Il documento di riferimento principale è:

```text
docs/1-design/005-design-orchestratore-applicativo-timer.md
````

Il Design 005 stabilisce che l’orchestratore applicativo deve essere un componente separato, collocato in:

```text
services/CicloTimer.App/
```

con file progetto:

```text
services/CicloTimer.App/CicloTimer.App.csproj
```

Il progetto dovrà chiamarsi:

```text
CicloTimer.App
```

e avere test in:

```text
tests/CicloTimer.App.Tests/
```

Questo coding plan definisce:

1. quali cartelle creare;
2. quali file creare;
3. quale progetto .NET creare;
4. quale progetto test creare;
5. quali dipendenze autorizzare;
6. quali dipendenze vietare;
7. quali astrazioni introdurre per Bridge e Audio;
8. quale classe orchestratore creare;
9. quale risultato applicativo restituire;
10. come conservare lo stato corrente;
11. come eseguire `SystemActions`;
12. come gestire `AudioServiceResult`;
13. come gestire `Shutdown/Dispose`;
14. quali test automatici creare;
15. quali aree non toccare;
16. quali verifiche finali eseguire.

Questo coding plan non cambia il Design 005.

Questo coding plan non introduce UI.

Questo coding plan non introduce timer reale.

Questo coding plan non introduce `DispatcherTimer`.

Questo coding plan non collega direttamente l’orchestratore alla futura UI WPF.

Questo coding plan non autorizza modifiche a Core, Localization, Bridge o Audio, salvo la creazione di adapter interni al progetto App.

---

## 2. Obiettivo operativo

L’obiettivo operativo è creare il progetto:

```text
CicloTimer.App
```

nel percorso:

```text
services/CicloTimer.App/
```

e il relativo progetto test:

```text
tests/CicloTimer.App.Tests/
```

Il progetto App deve:

1. coordinare Bridge e Audio;
2. ricevere comandi applicativi;
3. inoltrare i comandi al Bridge;
4. conservare l’ultimo modello mostrabile prodotto dal Bridge;
5. conservare l’ultimo risultato applicativo;
6. conservare l’ultimo risultato tecnico audio, se presente;
7. leggere `SystemActions` prodotte dal Bridge;
8. chiamare il servizio audio solo in risposta a `SystemActions`;
9. distinguere successo audio pieno, successo parziale e fallimento tecnico;
10. trattare errori audio come non bloccanti;
11. ricevere `Tick(elapsedSeconds)` da fuori;
12. non generare tick reali;
13. non usare UI;
14. non dipendere direttamente da Core;
15. non dipendere direttamente da Localization;
16. non dipendere dal progetto WPF root;
17. non modificare Core, Localization, Bridge o Audio;
18. essere testabile senza UI, timer reale o audio reale.

---

## 3. Perimetro autorizzato

Questo coding plan autorizza:

1. creazione di `services/CicloTimer.App/`;
2. creazione di `services/CicloTimer.App/CicloTimer.App.csproj`;
3. creazione dei file C# del progetto App;
4. aggiunta del progetto App a `CicloTimer.sln`;
5. creazione di `tests/CicloTimer.App.Tests/`;
6. creazione di `tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj`;
7. aggiunta del progetto test App a `CicloTimer.sln`;
8. riferimento del progetto App a:

   * `view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj`;
   * `services/CicloTimer.Audio/CicloTimer.Audio.csproj`;
9. riferimento del progetto test App a:

   * `services/CicloTimer.App/CicloTimer.App.csproj`;
10. creazione di astrazioni interne al progetto App per rendere testabile Bridge e Audio;
11. creazione di adapter interni verso Bridge e Audio;
12. eventuale modifica minima di `ciclotimer.csproj` solo se la build WPF include ricorsivamente `services/CicloTimer.App/**`;
13. eventuale aggiornamento minimo di `.gitignore` solo se emergono nuovi artefatti non ignorati;
14. esecuzione build e test.

---

## 4. Fuori perimetro

Questo coding plan non autorizza:

1. modifica di `models/CicloTimer.Core/`;
2. modifica di `locales/CicloTimer.Localization/`;
3. modifica di `view-models/CicloTimer.Bridge/`;
4. modifica di `services/CicloTimer.Audio/`;
5. modifica dei test core;
6. modifica dei test localization;
7. modifica dei test bridge;
8. modifica dei test audio;
9. modifica di `MainWindow.xaml`;
10. modifica di `MainWindow.xaml.cs`;
11. modifica di `App.xaml`;
12. modifica di `App.xaml.cs`;
13. modifica UI WPF;
14. creazione ViewModel WPF;
15. creazione orchestratore dentro UI;
16. creazione timer reale;
17. uso di `DispatcherTimer`;
18. uso di `System.Timers.Timer`;
19. uso di `System.Threading.Timer`;
20. uso di `Task.Delay` in loop;
21. uso di thread temporali;
22. uso di `ICommand`;
23. uso di `INotifyPropertyChanged`;
24. uso di XAML;
25. uso di NVDA;
26. uso di UI Automation;
27. uso di Live Region;
28. generazione testi utente;
29. modifica localization;
30. persistenza preferenze;
31. accesso database;
32. accesso cloud;
33. creazione cartella `src/`;
34. creazione cartella `orchestrators/`;
35. dipendenza diretta da Core;
36. dipendenza diretta da Localization;
37. dipendenza dal progetto WPF root.

Se durante l’implementazione emerge la necessità di modificare Core, Localization, Bridge, Audio, UI o architettura, Cursor deve fermarsi e segnalarlo.

---

## 5. Struttura fisica obbligatoria

La struttura da creare è:

```text
services/
  CicloTimer.App/
    CicloTimer.App.csproj
    AppCommandResult.cs
    TimerAppOrchestrator.cs
    TimerAppState.cs
    ITimerAppOrchestrator.cs
    ITimerBridgePort.cs
    IAudioServicePort.cs
    TimerBridgeAdapter.cs
    AudioServiceAdapter.cs
    SystemActionDispatcher.cs
    SystemActionDispatchResult.cs

tests/
  CicloTimer.App.Tests/
    CicloTimer.App.Tests.csproj
    TimerAppOrchestratorInitializationTests.cs
    TimerAppOrchestratorCommandTests.cs
    TimerAppOrchestratorSystemActionTests.cs
    TimerAppOrchestratorAudioResultTests.cs
    TimerAppOrchestratorDisposeTests.cs
    TimerAppOrchestratorSafetyTests.cs
    ProjectDependencyTests.cs
```

I nomi dei file possono essere adattati solo se il significato rimane identico e il report finale documenta la deviazione.

Sono vietate collocazioni alternative:

```text
src/
orchestrators/
models/CicloTimer.App/
locales/CicloTimer.App/
view-models/CicloTimer.App/
services/CicloTimer.Audio/CicloTimer.App/
CicloTimer.App/ nella root
```

---

## 6. Progetto CicloTimer.App

### 6.1 Percorso

```text
services/CicloTimer.App/
```

File progetto:

```text
services/CicloTimer.App/CicloTimer.App.csproj
```

### 6.2 Target framework

Il progetto deve usare:

```text
net9.0-windows
```

Motivo:

```text
CicloTimer.App deve referenziare CicloTimer.Audio, che usa net9.0-windows.
Il target net9.0-windows è necessario per compatibilità transitiva con il progetto Audio.
```

Questa scelta non trasforma `CicloTimer.App` in un progetto UI.

Questa scelta non autorizza l’uso di:

```text
WPF
XAML
DispatcherTimer
System.Windows
PresentationFramework
UIAutomation
ICommand
INotifyPropertyChanged
timer reale
```

Il progetto App resta un progetto di orchestrazione applicativa, non un progetto grafico.

### 6.3 Riferimenti autorizzati

Il progetto App può referenziare:

```text
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

### 6.4 Riferimenti vietati

Il progetto App non deve referenziare direttamente:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
ciclotimer.csproj
tests/*
```

Il progetto App non deve dipendere da:

```text
WPF
PresentationFramework
System.Windows
UIAutomation
```

### 6.5 Contenuto indicativo del csproj

Forma indicativa:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>CicloTimer.App</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\view-models\CicloTimer.Bridge\CicloTimer.Bridge.csproj" />
    <ProjectReference Include="..\CicloTimer.Audio\CicloTimer.Audio.csproj" />
  </ItemGroup>
</Project>
```

Il percorso relativo dovrà essere verificato durante l’implementazione.

---

## 7. Aggiornamento solution

Dopo la creazione del progetto App, aggiungerlo subito alla solution:

```bash
dotnet sln CicloTimer.sln add services/CicloTimer.App/CicloTimer.App.csproj
```

Dopo la creazione del progetto test App, aggiungerlo subito alla solution:

```bash
dotnet sln CicloTimer.sln add tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj
```

I comandi devono essere eseguiti dalla root del repository.

Se `dotnet sln add` fallisce, Cursor deve fermarsi e segnalare il problema.

Non deve modificare manualmente la solution salvo esplicita necessità documentata nel report finale.

---

## 8. Possibile esclusione in ciclotimer.csproj

Il progetto WPF root può includere ricorsivamente file `.cs` sotto la root del repository.

Dopo TODO 004 dovrebbe già esistere l’esclusione:

```xml
<Compile Remove="services/**" />
```

Per il progetto App, Cursor non deve modificare preventivamente `ciclotimer.csproj`.

Ordine obbligatorio:

1. creare il progetto App;
2. creare il progetto test App;
3. aggiungere entrambi alla solution;
4. eseguire `dotnet build CicloTimer.sln` dalla root;
5. solo se la build fallisce perché il progetto WPF include file sotto `services/**`, allora modificare `ciclotimer.csproj`.

In quel solo caso Cursor può aggiungere o correggere:

```xml
<Compile Remove="services/**" />
```

Questa modifica è autorizzata solo se necessaria.

Cursor deve documentare chiaramente questa eventuale modifica nel report finale.

---

## 9. Ricognizione obbligatoria sulle API reali

Prima di implementare porte, adapter e orchestratore, Cursor deve leggere le API reali dei progetti già esistenti:

```text
view-models/CicloTimer.Bridge/
services/CicloTimer.Audio/
```

Deve identificare:

1. tipo reale del modello/update prodotto dal Bridge;
2. tipo reale delle `SystemActions`;
3. nome reale dei valori `StartFinalAlertSound` e `StopFinalAlertSound`;
4. metodo reale per ottenere il modello iniziale dal Bridge;
5. metodo reale per `Configure`;
6. metodo reale per `Start`;
7. metodo reale per `Pause`;
8. metodo reale per `Resume`;
9. metodo reale per `Reset`;
10. metodo reale per `Tick`;
11. tipo reale di `elapsedSeconds` usato dal Bridge;
12. tipo reale di `AudioServiceResult`;
13. metodi reali di `AudioService`.

Regola fondamentale:

```text
Cursor non deve inventare nomi di tipi o metodi se sono già presenti nel codice.
```

Se una API necessaria non esiste, Cursor deve fermarsi e segnalarlo.

Non deve modificare Bridge o Audio senza autorizzazione.

---

## 10. Astrazioni interne: principio

L’orchestratore deve essere testabile con bridge finto e audio finto.

Per questo il progetto App deve introdurre porte interne.

Porte concettuali:

```text
ITimerBridgePort
IAudioServicePort
```

Queste porte sono astrazioni interne al progetto App.

Servono a evitare che i test dell’orchestratore debbano usare obbligatoriamente bridge reale o audio reale.

Regola importante:

```text
le porte non sostituiscono il Bridge e non sostituiscono AudioService
```

Sono solo adapter boundary del progetto App.

---

## 11. ITimerBridgePort

### 11.1 Percorso

```text
services/CicloTimer.App/ITimerBridgePort.cs
```

### 11.2 Responsabilità

Astrazione interna usata dall’orchestratore per parlare con il Bridge.

Deve esporre operazioni concettuali equivalenti a:

```text
GetCurrentModel
Configure
Start
Pause
Resume
Reset
Tick
```

Le firme devono usare i tipi C# reali trovati nel progetto `CicloTimer.Bridge`.

Le firme non devono usare `object` se esistono tipi reali.

Esempi di tipi reali da cercare e usare, se presenti:

```text
TimerBridgeUpdate
TimerDisplayModel
TimerBridgeConfiguration
TimerSystemAction
SystemAction
```

I nomi sopra sono solo esempi. Cursor deve usare i nomi effettivi presenti nel codice.

Forma concettuale vietata in implementazione reale se esistono tipi concreti:

```csharp
object GetCurrentModel();
object Start();
```

Regola vincolante:

```text
Le firme con object sono solo illustrative.
L’implementazione deve sostituirle con tipi reali o con astrazioni tipizzate definite nel progetto App.
Usare object senza necessità è errore di implementazione.
```

Regole:

1. non creare duplicati dei tipi bridge se i tipi esistono già;
2. non ricostruire modelli mostrabili;
3. non generare testi;
4. non chiamare Core;
5. non chiamare Localization;
6. non introdurre UI.

---

## 12. IAudioServicePort

### 12.1 Percorso

```text
services/CicloTimer.App/IAudioServicePort.cs
```

### 12.2 Responsabilità

Astrazione interna usata dall’orchestratore per parlare con il servizio audio.

Deve esporre operazioni concettuali equivalenti a:

```text
StartFinalAlertSound
StopFinalAlertSound
```

Forma concettuale:

```csharp
public interface IAudioServicePort
{
    AudioServiceResult StartFinalAlertSound();

    AudioServiceResult StopFinalAlertSound();
}
```

Regole:

1. usare il tipo `AudioServiceResult` reale del progetto `CicloTimer.Audio`;
2. non usare `object` per il risultato audio;
3. non riprodurre direttamente audio;
4. non usare `SoundPlayer`;
5. non gestire file audio;
6. non gestire focus audio;
7. non trasformare risultati audio in testi utente.

---

## 13. Tipo SystemAction del Bridge

Il Bridge deve esporre nel proprio modello/update una forma tipizzata delle azioni di sistema.

Cursor deve verificare il tipo reale.

Potrebbe essere:

```text
enum
record
classe
lista di azioni
proprietà dentro update
```

Valori concettuali attesi:

```text
StartFinalAlertSound
StopFinalAlertSound
```

Regole:

1. usare il tipo reale già esistente nel Bridge;
2. non creare un nuovo tipo duplicato se il Bridge ne espone già uno;
3. non interpretare azioni tramite stringhe libere;
4. non usare `ToString()` per decidere l’azione;
5. non usare reflection;
6. non modificare Bridge per aggiungere SystemActions;
7. se il Bridge non espone SystemActions in modo utilizzabile, fermarsi e segnalarlo.

Il `SystemActionDispatcher` deve lavorare su questo tipo reale o su una porta tipizzata coerente.

---

## 14. Tipo Tick / elapsedSeconds

Il metodo `Tick` dell’orchestratore deve usare lo stesso tipo già esposto dal Bridge.

Cursor non deve scegliere arbitrariamente tra:

```text
int
double
decimal
TimeSpan
```

Regola:

```text
Il tipo di elapsedSeconds deve essere ricavato dall’API reale del Bridge.
```

Esempi:

```text
se il Bridge espone Tick(int elapsedSeconds), usare int;
se il Bridge espone Tick(double elapsedSeconds), usare double;
se il Bridge espone un altro tipo, usare quel tipo.
```

Se il Bridge non espone un comando Tick utilizzabile, Cursor deve fermarsi e segnalarlo.

Non deve modificare Bridge.

---

## 15. TimerBridgeAdapter

### 15.1 Percorso

```text
services/CicloTimer.App/TimerBridgeAdapter.cs
```

### 15.2 Responsabilità

Adapter concreto tra `ITimerBridgePort` e il bridge reale.

Il coding plan dovrà verificare i tipi e metodi reali esposti da `CicloTimer.Bridge`.

L’adapter deve:

1. ricevere il bridge reale;
2. chiamare i suoi metodi pubblici;
3. restituire gli update/modelli prodotti dal bridge;
4. esporre SystemActions in forma tipizzata;
5. non modificare i dati del bridge;
6. non ricostruire testi;
7. non chiamare Core direttamente;
8. non chiamare Localization direttamente.

Se il bridge attuale non espone un metodo per ottenere il modello iniziale, Cursor deve fermarsi e segnalarlo.

Se il bridge attuale non espone SystemActions in forma utilizzabile, Cursor deve fermarsi e segnalarlo.

Non deve modificare il bridge senza autorizzazione.

---

## 16. AudioServiceAdapter

### 16.1 Percorso

```text
services/CicloTimer.App/AudioServiceAdapter.cs
```

### 16.2 Responsabilità

Adapter concreto tra `IAudioServicePort` e `AudioService`.

L’adapter deve:

1. ricevere il servizio audio reale;
2. chiamare `StartFinalAlertSound`;
3. chiamare `StopFinalAlertSound`;
4. restituire `AudioServiceResult`;
5. non interpretare come testo il risultato audio;
6. non bloccare il timer per errori audio;
7. non usare API audio direttamente.

---

## 17. TimerAppState

### 17.1 Percorso

```text
services/CicloTimer.App/TimerAppState.cs
```

### 17.2 Responsabilità

Rappresentare lo stato corrente dell’orchestratore.

Deve contenere almeno:

1. modello mostrabile corrente;
2. ultimo risultato audio;
3. ultimo risultato applicativo.

Forma concettuale:

```csharp
public sealed record TimerAppState(
    /* Bridge model type */ CurrentModel,
    AudioServiceResult? LastAudioResult,
    AppCommandResult? LastCommandResult);
```

Il tipo reale di `CurrentModel` dovrà essere il tipo prodotto dal Bridge.

Regole:

1. non deve essere ViewModel WPF;
2. non deve implementare `INotifyPropertyChanged`;
3. non deve contenere controlli UI;
4. non deve contenere testi generati dall’orchestratore;
5. non deve contenere timer reale;
6. deve essere immutabile o trattato come snapshot non modificabile dall’esterno.

---

## 18. AppCommandResult

### 18.1 Percorso

```text
services/CicloTimer.App/AppCommandResult.cs
```

### 18.2 Responsabilità

Rappresentare il risultato applicativo neutro di un comando orchestrato.

Deve contenere almeno:

1. `CurrentModel`;
2. `LastAudioResult`;
3. `Success`.

Forma concettuale:

```csharp
public sealed record AppCommandResult(
    /* Bridge model type */ CurrentModel,
    AudioServiceResult? LastAudioResult,
    bool Success);
```

Sono ammessi campi aggiuntivi tecnici se utili:

```text
HasAudioWarning
HasTechnicalError
UnhandledActionCount
```

ma non sono obbligatori.

Regole:

1. non contenere testi utente nuovi;
2. non contenere stringhe hardcoded;
3. non contenere tipi WPF;
4. non contenere eccezioni esposte all’utente;
5. non considerare `AudioFocusUnavailable` come errore bloccante se il playback è riuscito;
6. non rendere `Success = false` solo perché l’audio fallisce, se il comando timer è stato gestito.

---

## 19. SystemActionDispatcher

### 19.1 Percorso

```text
services/CicloTimer.App/SystemActionDispatcher.cs
```

### 19.2 Responsabilità

Eseguire le `SystemActions` prodotte dal Bridge.

Mappatura:

```text
StartFinalAlertSound → IAudioServicePort.StartFinalAlertSound
StopFinalAlertSound → IAudioServicePort.StopFinalAlertSound
```

Il dispatcher deve:

1. ricevere lista azioni tipizzata;
2. eseguirle nell’ordine ricevuto;
3. chiamare audio solo per azioni audio;
4. conservare i risultati audio;
5. trattare errori audio come non bloccanti;
6. restituire un risultato tecnico di dispatch.

Il dispatcher non deve:

1. decidere quando l’audio deve partire;
2. decidere quando l’audio deve fermarsi;
3. leggere direttamente stati core;
4. leggere direttamente stati bridge per dedurre audio;
5. chiamare audio senza `SystemActions`;
6. generare testi utente;
7. interpretare azioni tramite stringhe libere.

---

## 20. SystemActionDispatchResult

### 20.1 Percorso

```text
services/CicloTimer.App/SystemActionDispatchResult.cs
```

### 20.2 Responsabilità

Rappresentare l’esito tecnico dell’esecuzione delle azioni di sistema.

Deve contenere almeno:

1. ultimo risultato audio;
2. numero azioni eseguite;
3. numero azioni ignorate/non gestite;
4. eventuale flag tecnico di warning.

Forma concettuale:

```csharp
public sealed record SystemActionDispatchResult(
    AudioServiceResult? LastAudioResult,
    int ExecutedActions,
    int IgnoredActions,
    bool HasWarning);
```

Regole:

1. nessun testo utente;
2. nessuna localization;
3. nessun tipo WPF;
4. errori audio non bloccanti;
5. azioni sconosciute ignorate in modo sicuro o tracciate tecnicamente.

---

## 21. ITimerAppOrchestrator

### 21.1 Percorso

```text
services/CicloTimer.App/ITimerAppOrchestrator.cs
```

### 21.2 Responsabilità

Astrazione pubblica del progetto App per la futura UI e il futuro gestore timer reale.

Deve esporre almeno:

```text
CurrentState
Configure
Start
Pause
Resume
Reset
Tick
Shutdown/Dispose
```

Forma concettuale:

```csharp
public interface ITimerAppOrchestrator : IDisposable
{
    TimerAppState CurrentState { get; }

    AppCommandResult Configure(/* config */);

    AppCommandResult Start();

    AppCommandResult Pause();

    AppCommandResult Resume();

    AppCommandResult Reset();

    AppCommandResult Tick(/* same elapsed type as Bridge */ elapsedSeconds);
}
```

Regole:

1. `Tick` deve usare il tipo reale del Bridge;
2. non usare `ICommand`;
3. non usare `INotifyPropertyChanged`;
4. non usare WPF;
5. non usare timer reale;
6. non usare stringhe utente hardcoded.

---

## 22. TimerAppOrchestrator

### 22.1 Percorso

```text
services/CicloTimer.App/TimerAppOrchestrator.cs
```

### 22.2 Responsabilità

Classe principale dell’orchestratore.

Deve:

1. ricevere `ITimerBridgePort`;
2. ricevere `IAudioServicePort`;
3. creare o ricevere `SystemActionDispatcher`;
4. ottenere modello iniziale dal bridge;
5. conservare `CurrentState`;
6. esporre comandi applicativi;
7. inoltrare comandi al bridge;
8. eseguire `SystemActions`;
9. conservare risultati audio;
10. restituire `AppCommandResult`;
11. implementare chiusura sicura;
12. gestire comandi ravvicinati senza crash.

### 22.3 Costruttore

Forma concettuale:

```csharp
public sealed class TimerAppOrchestrator : ITimerAppOrchestrator
{
    private readonly object _sync = new();

    public TimerAppOrchestrator(
        ITimerBridgePort bridge,
        IAudioServicePort audioService)
    {
        ...
    }
}
```

Regole:

1. usare injection via costruttore;
2. non creare direttamente bridge reale se non tramite adapter esterno;
3. non creare direttamente audio reale se non tramite adapter esterno;
4. consentire test con fake bridge/audio;
5. validare null in ingresso in modo tecnico;
6. nessun testo utente;
7. usare un lock privato semplice o meccanismo equivalente per serializzare i comandi.

---

## 23. Inizializzazione

All’inizializzazione, `TimerAppOrchestrator` deve ottenere dal bridge il modello iniziale.

Sequenza:

```text
costruttore orchestratore
↓
chiama bridge port per ottenere modello iniziale
↓
salva CurrentState
↓
non avvia audio
↓
non avvia timer reale
```

Se il bridge non fornisce modello iniziale:

1. Cursor deve fermarsi;
2. non deve modificare il bridge;
3. deve segnalare il problema nel report.

Il coding plan autorizza solo l’adattamento lato App, non la modifica del Bridge.

`CurrentState` deve essere esposto come proprietà di sola lettura o snapshot immutabile.

La futura UI non dovrà poter modificare direttamente lo stato dell’orchestratore.

---

## 24. Implementazione comando generico

Per evitare duplicazione, `TimerAppOrchestrator` può usare una funzione interna comune per gestire i comandi.

Sequenza comune:

```text
1. acquisire lock privato o meccanismo equivalente
2. ricevere comando
3. chiamare bridge port
4. ricevere update/modello dal bridge
5. aggiornare CurrentState con modello
6. leggere SystemActions tipizzate dal modello/update
7. passare SystemActions al dispatcher
8. ricevere risultato dispatch
9. aggiornare LastAudioResult
10. costruire AppCommandResult
11. aggiornare CurrentState.LastCommandResult
12. restituire AppCommandResult
```

Regole:

1. non chiamare audio prima di avere `SystemActions`;
2. non dedurre audio dagli stati;
3. non modificare testi del modello;
4. non bloccare per errori audio;
5. non lasciare che comandi ravvicinati corrompano `CurrentState`.

---

## 25. Configure

`Configure` deve:

1. ricevere configurazione timer usando i tipi previsti dal Bridge;
2. inoltrare al bridge;
3. aggiornare `CurrentState`;
4. eseguire eventuali `SystemActions`;
5. restituire `AppCommandResult`.

Non deve:

1. validare direttamente le durate;
2. chiamare Core;
3. chiamare Localization;
4. generare testi.

---

## 26. Start

`Start` deve:

1. inoltrare al bridge;
2. aggiornare `CurrentState`;
3. eseguire eventuali `SystemActions`;
4. restituire `AppCommandResult`.

Non deve decidere se Start è consentito.

---

## 27. Pause

`Pause` deve:

1. inoltrare al bridge;
2. aggiornare `CurrentState`;
3. eseguire eventuali `SystemActions`;
4. restituire `AppCommandResult`.

Caso importante:

```text
se il bridge produce StopFinalAlertSound
l’orchestratore deve chiamare AudioService.StopFinalAlertSound
```

L’orchestratore non deve dedurre lo stop leggendo stati.

---

## 28. Resume

`Resume` deve:

1. inoltrare al bridge;
2. aggiornare `CurrentState`;
3. eseguire eventuali `SystemActions`;
4. restituire `AppCommandResult`.

Caso importante:

```text
se il bridge produce StartFinalAlertSound
l’orchestratore deve chiamare AudioService.StartFinalAlertSound
```

L’orchestratore non deve dedurre start audio leggendo stati.

---

## 29. Reset

`Reset` deve:

1. inoltrare al bridge;
2. aggiornare `CurrentState`;
3. eseguire eventuali `SystemActions`;
4. restituire `AppCommandResult`.

Caso importante:

```text
se il bridge produce StopFinalAlertSound
l’orchestratore deve chiamare AudioService.StopFinalAlertSound
```

L’orchestratore non deve fermare audio autonomamente nei normali comandi applicativi.

---

## 30. Tick

`Tick` deve:

1. ricevere `elapsedSeconds` usando il tipo reale del Bridge;
2. inoltrare al bridge;
3. aggiornare `CurrentState`;
4. eseguire eventuali `SystemActions`;
5. restituire `AppCommandResult`.

Non deve:

1. generare tick;
2. usare timer reale;
3. usare `DispatcherTimer`;
4. chiamare Core direttamente;
5. chiamare Audio direttamente senza `SystemActions`.

---

## 31. Shutdown / Dispose

`Dispose` o metodo equivalente deve:

1. tentare stop audio sicuro;
2. non bloccare la chiusura;
3. non lanciare eccezioni non gestite;
4. non chiamare UI;
5. non generare testi utente.

Comportamento previsto:

```text
Dispose
↓
try
  AudioService.StopFinalAlertSound
catch
  cattura errore tecnico audio
  non propaga eccezione
↓
chiusura completata
```

Il metodo di chiusura può chiamare audio anche senza `SystemActions`, perché questa è una fase di shutdown controllato.

Questa è l’unica eccezione ammessa al principio “audio solo tramite SystemActions”.

Regola vincolante:

```text
nessuna eccezione audio deve impedire la chiusura
```

---

## 32. AudioServiceResult e successo parziale

`AudioServiceResult` contiene almeno:

```text
PlaybackResult
FocusResult
RestoreResult
```

L’orchestratore deve distinguere:

1. successo audio pieno;
2. successo audio parziale;
3. fallimento audio tecnico.

Esempio di successo parziale:

```text
PlaybackResult = Success
FocusResult = AudioFocusUnavailable
RestoreResult = NotAttempted
```

Questo non deve bloccare il comando applicativo.

Regola:

```text
AudioFocusUnavailable non rende fallito il comando se il playback è riuscito
```

Esempio di fallimento tecnico audio:

```text
PlaybackResult = PlaybackFailed
```

Anche questo non deve necessariamente rendere `Success = false`, se il comando timer è stato gestito.

Il coding agent deve preservare l’informazione tecnica in `LastAudioResult`.

---

## 33. Comandi ravvicinati

L’orchestratore deve gestire comandi ravvicinati senza crash.

Possibili scenari:

```text
Start chiamato due volte rapidamente
Pause chiamato mentre arriva Tick
Reset chiamato mentre arriva Tick
Tick chiamato più volte in sequenza
Dispose chiamato mentre audio è attivo
```

Per la prima implementazione è consigliato un lock privato semplice.

Esempio concettuale:

```csharp
private readonly object _sync = new();
```

Regole:

1. nessuna corruzione di `CurrentState`;
2. nessuna eccezione non gestita;
3. nessuna azione audio inventata;
4. nessun timer reale;
5. nessun thread temporale;
6. nessuna scelta arbitraria async se non necessaria.

---

## 34. Test progetto App

### 34.1 Percorso

```text
tests/CicloTimer.App.Tests/
```

File progetto:

```text
tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj
```

Target:

```text
net9.0-windows
```

Motivo:

```text
il progetto test deve essere compatibile con CicloTimer.App, che targetta net9.0-windows per compatibilità con CicloTimer.Audio
```

Framework consigliato:

```text
xUnit
```

Il progetto test deve referenziare:

```text
services/CicloTimer.App/CicloTimer.App.csproj
```

Il progetto test non deve referenziare direttamente:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
services/CicloTimer.Audio/CicloTimer.Audio.csproj
ciclotimer.csproj
```

Nota:

```text
il test project usa App come unità sotto test; le dipendenze Bridge/Audio arrivano transitivamente tramite App o sono sostituite da fake port interni
```

---

## 35. TimerAppOrchestratorInitializationTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorInitializationTests.cs
```

Test obbligatori:

1. costruttore richiede bridge port non null;
2. costruttore richiede audio port non null;
3. all’inizializzazione viene richiesto modello iniziale al bridge;
4. `CurrentState` contiene modello iniziale;
5. `CurrentState` è sola lettura o snapshot immutabile;
6. all’inizializzazione non viene chiamato audio;
7. all’inizializzazione non viene generato tick;
8. all’inizializzazione non viene creato timer reale.

---

## 36. TimerAppOrchestratorCommandTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorCommandTests.cs
```

Test obbligatori:

1. `Configure` inoltra al bridge;
2. `Start` inoltra al bridge;
3. `Pause` inoltra al bridge;
4. `Resume` inoltra al bridge;
5. `Reset` inoltra al bridge;
6. `Tick` inoltra al bridge con `elapsedSeconds` usando il tipo reale del Bridge;
7. ogni comando aggiorna `CurrentState`;
8. ogni comando restituisce `AppCommandResult`;
9. l’orchestratore non valida direttamente le durate;
10. l’orchestratore non genera testi.

---

## 37. TimerAppOrchestratorSystemActionTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorSystemActionTests.cs
```

Test obbligatori:

1. se bridge produce zero azioni, audio non viene chiamato;
2. se bridge produce `StartFinalAlertSound`, audio start viene chiamato;
3. se bridge produce `StopFinalAlertSound`, audio stop viene chiamato;
4. azioni multiple vengono eseguite in ordine;
5. azione sconosciuta viene gestita in modo sicuro;
6. Pause non ferma audio se il bridge non produce `StopFinalAlertSound`;
7. Reset non ferma audio se il bridge non produce `StopFinalAlertSound`;
8. Resume non avvia audio se il bridge non produce `StartFinalAlertSound`;
9. l’orchestratore non deduce audio dagli stati;
10. le azioni non sono interpretate tramite stringhe libere o `ToString()`.

---

## 38. TimerAppOrchestratorAudioResultTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorAudioResultTests.cs
```

Test obbligatori:

1. risultato audio viene conservato in `LastAudioResult`;
2. `AudioFocusUnavailable` con playback riuscito non rende fallito il comando;
3. `PlaybackFailed` non blocca il comando timer;
4. errore audio non lancia eccezione non gestita;
5. successi parziali sono distinguibili;
6. `AppCommandResult` contiene `CurrentModel`;
7. `AppCommandResult` contiene `LastAudioResult`;
8. `AppCommandResult.Success` rispetta il principio “audio non bloccante”.

---

## 39. TimerAppOrchestratorDisposeTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorDisposeTests.cs
```

Test obbligatori:

1. `Dispose` tenta stop audio;
2. `Dispose` non chiama bridge;
3. errore audio durante `Dispose` non genera eccezione non gestita;
4. `Dispose` può essere chiamato più volte;
5. `Dispose` non crea timer reale;
6. `Dispose` non modifica UI;
7. `Dispose` conserva stato sicuro;
8. `Dispose` cattura eventuali eccezioni audio.

---

## 40. TimerAppOrchestratorSafetyTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorSafetyTests.cs
```

Test obbligatori:

1. comandi ripetuti non causano crash;
2. `Tick` ripetuti non causano crash;
3. comandi ravvicinati mantengono `CurrentState` coerente;
4. azioni audio non vengono duplicate se non prodotte dal bridge;
5. eccezione del bridge viene gestita o propagata come errore tecnico documentato;
6. eccezione audio viene gestita come non bloccante;
7. nessun timer reale viene creato;
8. esiste un meccanismo semplice di serializzazione comandi, per esempio lock privato.

Il trattamento delle eccezioni bridge deve essere definito in modo prudente nel coding plan operativo: se il bridge fallisce, può essere errore bloccante tecnico del comando, ma non deve essere trasformato in testo utente hardcoded.

---

## 41. ProjectDependencyTests

Creare:

```text
tests/CicloTimer.App.Tests/ProjectDependencyTests.cs
```

Verifiche obbligatorie o equivalenti nel report:

1. `CicloTimer.App.csproj` usa `net9.0-windows`;
2. `CicloTimer.App.Tests.csproj` usa `net9.0-windows`;
3. App referenzia Bridge;
4. App referenzia Audio;
5. App non referenzia Core diretto;
6. App non referenzia Localization diretto;
7. App non referenzia WPF root;
8. App non contiene `System.Windows`;
9. App non contiene `PresentationFramework`;
10. App non contiene `UIAutomation`;
11. App non contiene `DispatcherTimer`;
12. App non contiene `System.Timers.Timer`;
13. App non contiene `System.Threading.Timer`;
14. App non contiene `Task.Delay`;
15. App non contiene `ICommand`;
16. App non contiene `INotifyPropertyChanged`;
17. App non contiene cartella `src`;
18. App non contiene cartella `orchestrators`;
19. App non contiene stringhe utente finali hardcoded;
20. App non modifica Bridge/Audio/Core/Localization;
21. App non usa `ToString()` o reflection per interpretare SystemActions;
22. App usa tipi reali del Bridge e Audio, non `object` non necessario.

I controlli possono essere test automatizzati o verifica documentata nel report finale.

---

## 42. Divieto stringhe utente hardcoded

Il progetto App non deve contenere testi utente finali.

Ammesse stringhe tecniche:

1. nomi enum;
2. nomi campi;
3. messaggi tecnici in test;
4. nomi file;
5. descrizioni interne non mostrate all’utente.

Non sono ammessi testi come:

```text
Timer avviato.
Errore audio.
Avviso non disponibile.
Timer in pausa.
```

Questi testi, se servono, devono provenire dal Bridge/Localization, non dall’orchestratore.

---

## 43. Ordine operativo di implementazione

L’implementazione dovrà seguire questo ordine:

```text
1. Ricognizione repository.
2. Verifica Design 005.
3. Verifica API reali di CicloTimer.Bridge.
4. Verifica API reali di CicloTimer.Audio.
5. Verifica tipo reale delle SystemActions.
6. Verifica tipo reale di Tick/elapsedSeconds.
7. Creazione progetto services/CicloTimer.App con target net9.0-windows.
8. Aggiunta progetto App alla solution.
9. Creazione porte ITimerBridgePort e IAudioServicePort con tipi reali.
10. Creazione adapter TimerBridgeAdapter e AudioServiceAdapter.
11. Creazione AppCommandResult.
12. Creazione TimerAppState.
13. Creazione SystemActionDispatchResult.
14. Creazione SystemActionDispatcher.
15. Creazione ITimerAppOrchestrator.
16. Creazione TimerAppOrchestrator.
17. Creazione progetto tests/CicloTimer.App.Tests con target net9.0-windows.
18. Aggiunta progetto test App alla solution.
19. Scrittura test inizializzazione.
20. Scrittura test comandi.
21. Scrittura test SystemActions.
22. Scrittura test risultati audio.
23. Scrittura test Dispose.
24. Scrittura test safety.
25. Scrittura ProjectDependencyTests.
26. Esecuzione build solution.
27. Solo se necessario, correzione ciclotimer.csproj con services/**.
28. Riesecuzione build.
29. Esecuzione test App.
30. Esecuzione regressione core/localization/bridge/audio.
31. Esecuzione dotnet test generale.
32. Pulizia bin/obj se necessario.
33. Report finale.
```

---

## 44. Comandi di verifica finali

Tutti i comandi devono essere eseguiti dalla root del repository.

Eseguire:

```bash
dotnet build CicloTimer.sln
```

Eseguire test App:

```bash
dotnet test tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj
```

Eseguire regressione core:

```bash
dotnet test tests/CicloTimer.Core.Tests/CicloTimer.Core.Tests.csproj
```

Eseguire regressione localization:

```bash
dotnet test tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Eseguire regressione bridge:

```bash
dotnet test tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj
```

Eseguire regressione audio:

```bash
dotnet test tests/CicloTimer.Audio.Tests/CicloTimer.Audio.Tests.csproj
```

Se possibile, eseguire:

```bash
dotnet test
```

Criteri:

1. build solution: 0 errori;
2. test App: tutti superati;
3. test core: tutti ancora superati;
4. test localization: tutti ancora superati;
5. test bridge: tutti ancora superati;
6. test audio: tutti ancora superati;
7. test solution completa: tutti superati.

---

## 45. Pulizia artefatti build

Dopo build/test, non devono restare artefatti da committare sotto:

```text
bin/
obj/
```

Cursor dovrà verificare:

```bash
git status --porcelain
```

e, su PowerShell:

```powershell
git status --porcelain | Select-String -Pattern "bin|obj"
```

Se compaiono artefatti `bin/obj`, devono essere rimossi dal working tree se non tracciati e non necessari.

Non devono essere rimossi file sorgente.

Non devono essere rimossi file `.csproj`.

Non devono essere rimossi file audio.

Non deve essere rimossa la cartella `services/CicloTimer.Audio/Assets/`.

---

## 46. File da non modificare

Cursor non deve modificare:

```text
models/CicloTimer.Core/
locales/CicloTimer.Localization/
view-models/CicloTimer.Bridge/
services/CicloTimer.Audio/
tests/CicloTimer.Core.Tests/
tests/CicloTimer.Localization.Tests/
tests/CicloTimer.Bridge.Tests/
tests/CicloTimer.Audio.Tests/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
```

Cursor non deve modificare `ciclotimer.csproj`, salvo il caso specifico autorizzato:

```text
aggiunta o correzione di <Compile Remove="services/**" />
```

se necessaria per impedire al progetto WPF root di compilare il servizio App.

Cursor può modificare:

```text
CicloTimer.sln
```

solo per aggiungere i nuovi progetti.

Cursor può modificare `.gitignore` solo se necessario per artefatti nuovi non ignorati.

---

## 47. Criteri di completamento

Il coding plan è completato quando:

1. esiste `services/CicloTimer.App/`;
2. esiste `services/CicloTimer.App/CicloTimer.App.csproj`;
3. il progetto App usa `net9.0-windows`;
4. il progetto App resta privo di WPF/UI nonostante `net9.0-windows`;
5. il progetto App referenzia Bridge;
6. il progetto App referenzia Audio;
7. il progetto App non referenzia Core diretto;
8. il progetto App non referenzia Localization diretto;
9. il progetto App non referenzia WPF root;
10. esiste `ITimerBridgePort`;
11. esiste `IAudioServicePort`;
12. le porte usano tipi reali, non `object` non necessario;
13. esiste `TimerBridgeAdapter`;
14. esiste `AudioServiceAdapter`;
15. esiste `AppCommandResult`;
16. esiste `TimerAppState`;
17. `TimerAppState` è snapshot immutabile o sola lettura;
18. esiste `SystemActionDispatcher`;
19. esiste `SystemActionDispatchResult`;
20. esiste `ITimerAppOrchestrator`;
21. esiste `TimerAppOrchestrator`;
22. orchestratore ottiene modello iniziale dal bridge;
23. orchestratore espone `CurrentState` in sola lettura;
24. orchestratore implementa `Configure`;
25. orchestratore implementa `Start`;
26. orchestratore implementa `Pause`;
27. orchestratore implementa `Resume`;
28. orchestratore implementa `Reset`;
29. orchestratore implementa `Tick` con il tipo reale del Bridge;
30. orchestratore implementa `Dispose` o shutdown equivalente;
31. audio viene chiamato solo in risposta a `SystemActions`, salvo shutdown;
32. `StartFinalAlertSound` chiama audio start;
33. `StopFinalAlertSound` chiama audio stop;
34. SystemActions sono interpretate tramite tipo reale, non stringhe;
35. errori audio non bloccano;
36. successi parziali audio non bloccano;
37. Dispose cattura eccezioni audio;
38. comandi ravvicinati sono serializzati con lock privato o equivalente;
39. non vengono generati testi utente hardcoded;
40. non viene creato timer reale;
41. non viene usato `DispatcherTimer`;
42. non viene usato `ICommand`;
43. non viene usato `INotifyPropertyChanged`;
44. esiste `tests/CicloTimer.App.Tests/`;
45. test App usano `net9.0-windows`;
46. test App passano;
47. test core passano;
48. test localization passano;
49. test bridge passano;
50. test audio passano;
51. build solution passa;
52. non ci sono artefatti `bin/obj` da committare;
53. non sono stati modificati core, localization, bridge, audio o UI.

---

## 48. Criteri di non validità

L’implementazione non è valida se:

1. crea App fuori da `services/CicloTimer.App/`;
2. crea `src/`;
3. crea `orchestrators/`;
4. mette App in `models/`;
5. mette App in `locales/`;
6. mette App in `view-models/`;
7. mette App dentro `services/CicloTimer.Audio/`;
8. modifica core;
9. modifica localization;
10. modifica bridge;
11. modifica audio;
12. modifica UI;
13. usa `net9.0` per App causando incompatibilità con Audio;
14. usa `net9.0-windows` per introdurre WPF/UI;
15. il progetto App referenzia core diretto;
16. il progetto App referenzia localization diretto;
17. il progetto App referenzia WPF root;
18. usa WPF;
19. usa XAML;
20. usa `DispatcherTimer`;
21. usa `System.Timers.Timer`;
22. usa `System.Threading.Timer`;
23. usa `Task.Delay` in loop;
24. implementa timer reale;
25. implementa UI;
26. implementa `ICommand`;
27. implementa `INotifyPropertyChanged`;
28. chiama audio senza `SystemActions`, salvo shutdown;
29. deduce start/stop audio dagli stati invece che da `SystemActions`;
30. interpreta SystemActions tramite stringhe libere, `ToString()` o reflection;
31. costruisce testi utente;
32. introduce stringhe hardcoded per l’utente;
33. usa `object` nelle porte nonostante esistano tipi reali;
34. blocca il timer se audio fallisce;
35. considera `AudioFocusUnavailable` come errore bloccante se playback è riuscito;
36. non tenta stop audio in `Dispose`;
37. propaga eccezioni audio da `Dispose`;
38. non protegge comandi ravvicinati;
39. build fallisce;
40. test falliscono;
41. ci sono artefatti `bin/obj` da committare.

---

## 49. Report finale richiesto a Cursor

Al termine dell’implementazione, Cursor dovrà produrre un report con:

1. file creati;
2. file modificati;
3. progetti creati;
4. progetti aggiunti alla solution;
5. target framework usati;
6. conferma progetto App in `services/CicloTimer.App/`;
7. conferma test in `tests/CicloTimer.App.Tests/`;
8. conferma App usa `net9.0-windows`;
9. conferma App non usa WPF/UI;
10. conferma App referenzia Bridge;
11. conferma App referenzia Audio;
12. conferma App non referenzia Core diretto;
13. conferma App non referenzia Localization diretto;
14. conferma App non referenzia WPF root;
15. conferma UI non modificata;
16. conferma core non modificato;
17. conferma localization non modificato;
18. conferma bridge non modificato;
19. conferma audio non modificato;
20. conferma presenza porte/adapters;
21. conferma porte con tipi reali;
22. conferma tipo reale delle SystemActions usato;
23. conferma SystemActions non interpretate con stringhe/ToString/reflection;
24. conferma tipo reale di Tick usato;
25. conferma modello iniziale ottenuto dal bridge;
26. conferma CurrentState sola lettura/immutabile;
27. conferma comandi implementati;
28. conferma `Tick` riceve elapsedSeconds da fuori;
29. conferma nessun timer reale;
30. conferma nessun `DispatcherTimer`;
31. conferma nessun `ICommand`;
32. conferma nessun `INotifyPropertyChanged`;
33. conferma gestione `SystemActions`;
34. conferma audio chiamato solo tramite `SystemActions`, salvo shutdown;
35. conferma gestione `AudioServiceResult`;
36. conferma successi audio parziali non bloccanti;
37. conferma errori audio non bloccanti;
38. conferma `Dispose` tenta stop audio;
39. conferma `Dispose` cattura eccezioni audio;
40. conferma meccanismo di serializzazione comandi ravvicinati;
41. eventuale modifica a `ciclotimer.csproj` e motivo;
42. comandi eseguiti;
43. risultato build;
44. risultato test App;
45. risultato test core;
46. risultato test localization;
47. risultato test bridge;
48. risultato test audio;
49. risultato `dotnet test`;
50. numero test App;
51. eventuali test falliti;
52. eventuali deviazioni dal coding plan;
53. conferma pulizia bin/obj;
54. output finale sintetico di `git status --porcelain`.

Cursor non deve fare commit.

Cursor non deve fare push.

---

## 50. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. `CicloTimer.App` resta in `services/CicloTimer.App/`;
2. `CicloTimer.App.Tests` resta in `tests/CicloTimer.App.Tests/`;
3. `CicloTimer.App` usa `net9.0-windows` per compatibilità transitiva con `CicloTimer.Audio`;
4. `net9.0-windows` non autorizza uso di UI, WPF, XAML o timer reale;
5. `CicloTimer.App.Tests` usa `net9.0-windows`;
6. App dipende solo da Bridge e Audio;
7. App non dipende direttamente da Core;
8. App non dipende direttamente da Localization;
9. App non dipende da WPF root;
10. porte e adapter devono usare tipi reali quando disponibili;
11. `object` non deve essere usato se esistono tipi reali;
12. il tipo reale delle SystemActions deve essere verificato e usato;
13. le SystemActions non devono essere interpretate con stringhe, `ToString()` o reflection;
14. il tipo reale di `Tick(elapsedSeconds)` deve essere ricavato dal Bridge;
15. `CurrentState` deve essere sola lettura o snapshot immutabile;
16. `Dispose` deve essere fail-safe e catturare errori audio;
17. per comandi ravvicinati è consigliato un lock privato semplice;
18. nessun timer reale viene introdotto;
19. nessuna UI viene introdotta;
20. nessuna modifica a Core, Localization, Bridge o Audio è autorizzata.

---

## 51. Stato del documento

Questo documento è approvato come Coding Plan 005 — Orchestratore applicativo timer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione tecnica
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni revisione: target App e App.Tests a net9.0-windows per compatibilità con Audio, chiarimento no UI/WPF nonostante target Windows, porte con tipi reali non object, ricognizione tipo SystemAction, Tick con tipo reale del Bridge, Dispose fail-safe con eccezioni audio catturate, CurrentState sola lettura/immutabile, lock privato semplice per comandi ravvicinati
```

Il documento è approvato come base per il successivo TODO 005.
