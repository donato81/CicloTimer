# CicloTimer — TODO 005 — Orchestratore applicativo timer

**Tipo documento:** todo operativo  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/1-design/004-design-audio-service-e-audio-focus.md, docs/1-design/005-design-orchestratore-applicativo-timer.md, docs/2-coding-plans/005-coding-plan-orchestratore-applicativo-timer.md  

---

## 1. Scopo del TODO

Questo documento traduce il Coding Plan 005 in una lista operativa eseguibile da Cursor.

L’obiettivo è implementare l’orchestratore applicativo separato di CicloTimer.

Il progetto orchestratore deve stare in:

```text
services/CicloTimer.App/
````

I test dell’orchestratore devono stare in:

```text
tests/CicloTimer.App.Tests/
```

L’orchestratore deve:

1. coordinare Bridge e Audio;
2. ricevere comandi applicativi;
3. inoltrare i comandi al Bridge;
4. conservare l’ultimo modello mostrabile prodotto dal Bridge;
5. conservare l’ultimo risultato applicativo;
6. conservare l’ultimo risultato tecnico audio;
7. leggere le `SystemActions` prodotte dal Bridge;
8. chiamare il servizio audio solo in risposta a `SystemActions`, salvo shutdown;
9. ricevere `Tick(elapsedSeconds)` da fuori;
10. non generare tick reali;
11. non usare UI;
12. non usare timer reale;
13. non dipendere direttamente da Core;
14. non dipendere direttamente da Localization;
15. non dipendere dal progetto WPF root;
16. non modificare Core, Localization, Bridge o Audio.

Questo TODO deve guidare Cursor in modo vincolato, evitando modifiche fuori perimetro.

---

## 2. Principio operativo

Il principio operativo è:

```text
l’orchestratore coordina, non decide la logica del timer
```

Il Bridge decide cosa deriva dal Core e produce il modello mostrabile.

Il Bridge produce eventuali `SystemActions`.

L’orchestratore esegue solo le `SystemActions`.

Il servizio Audio esegue realmente start/stop audio.

L’orchestratore non calcola direttamente il tempo rimanente.

L’orchestratore non decide quando entrare nell’avviso finale.

L’orchestratore non decide quando avviare o fermare il beep guardando gli stati.

L’orchestratore non costruisce testi utente.

---

## 3. Perimetro autorizzato

Cursor può:

1. creare `services/CicloTimer.App/`;
2. creare `services/CicloTimer.App/CicloTimer.App.csproj`;
3. creare i file C# del progetto App;
4. aggiungere il progetto App a `CicloTimer.sln`;
5. creare `tests/CicloTimer.App.Tests/`;
6. creare `tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj`;
7. aggiungere il progetto test App a `CicloTimer.sln`;
8. referenziare da App:

   * `view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj`;
   * `services/CicloTimer.Audio/CicloTimer.Audio.csproj`;
9. referenziare da App.Tests:

   * `services/CicloTimer.App/CicloTimer.App.csproj`;
10. creare porte interne per Bridge e Audio;
11. creare adapter interni verso Bridge e Audio;
12. creare orchestratore;
13. creare test automatici;
14. modificare `ciclotimer.csproj` solo se la build fallisce perché il progetto WPF include `services/**`;
15. aggiornare `.gitignore` solo se emergono artefatti nuovi non ignorati;
16. eseguire build;
17. eseguire test;
18. pulire eventuali artefatti `bin/obj`;
19. produrre report finale.

---

## 4. Fuori perimetro assoluto

Cursor non deve:

1. modificare `models/CicloTimer.Core/`;
2. modificare `locales/CicloTimer.Localization/`;
3. modificare `view-models/CicloTimer.Bridge/`;
4. modificare `services/CicloTimer.Audio/`;
5. modificare `tests/CicloTimer.Core.Tests/`;
6. modificare `tests/CicloTimer.Localization.Tests/`;
7. modificare `tests/CicloTimer.Bridge.Tests/`;
8. modificare `tests/CicloTimer.Audio.Tests/`;
9. modificare `MainWindow.xaml`;
10. modificare `MainWindow.xaml.cs`;
11. modificare `App.xaml`;
12. modificare `App.xaml.cs`;
13. modificare UI WPF;
14. creare ViewModel WPF;
15. creare orchestratore dentro UI;
16. creare timer reale;
17. usare `DispatcherTimer`;
18. usare `System.Timers.Timer`;
19. usare `System.Threading.Timer`;
20. usare `Task.Delay` in loop;
21. usare thread temporali;
22. usare `ICommand`;
23. usare `INotifyPropertyChanged`;
24. usare XAML;
25. usare NVDA;
26. usare UI Automation;
27. usare Live Region;
28. generare testi utente;
29. modificare localization;
30. accedere direttamente al Core;
31. accedere direttamente a Localization;
32. accedere al progetto WPF root;
33. persistere preferenze;
34. accedere database;
35. accedere cloud;
36. creare cartella `src/`;
37. creare cartella `orchestrators/`;
38. fare commit;
39. fare push.

Se Cursor rileva che una modifica fuori perimetro è necessaria, deve fermarsi e segnalarlo nel report.

---

## 5. Struttura finale attesa

La struttura finale deve essere:

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

## 6. FASE 0 — Ricognizione iniziale

### TODO 005.00 — Verificare repository prima di modificare

Cursor deve leggere e verificare:

1. presenza di `CicloTimer.sln`;
2. presenza di `models/CicloTimer.Core/`;
3. presenza di `locales/CicloTimer.Localization/`;
4. presenza di `view-models/CicloTimer.Bridge/`;
5. presenza di `services/CicloTimer.Audio/`;
6. presenza di `tests/CicloTimer.Core.Tests/`;
7. presenza di `tests/CicloTimer.Localization.Tests/`;
8. presenza di `tests/CicloTimer.Bridge.Tests/`;
9. presenza di `tests/CicloTimer.Audio.Tests/`;
10. presenza del Design 005;
11. presenza del Coding Plan 005;
12. contenuto attuale di `.gitignore`;
13. contenuto attuale di `ciclotimer.csproj`;
14. assenza di `services/CicloTimer.App/`;
15. assenza di `tests/CicloTimer.App.Tests/`.

Risultato atteso:

```text
ricognizione completata
nessuna modifica ancora eseguita
```

Se `services/CicloTimer.App/` o `tests/CicloTimer.App.Tests/` esistono già, Cursor deve segnalarlo e non sovrascrivere file senza controllo.

---

## 7. FASE 1 — Ricognizione API Bridge e Audio

### TODO 005.01 — Leggere API reali di Bridge e Audio

Prima di creare porte e adapter, Cursor deve leggere:

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

Regole:

1. non inventare nomi di tipi;
2. non inventare nomi di metodi;
3. non creare tipi duplicati se esistono tipi reali;
4. non usare `object` se esistono tipi reali;
5. non interpretare `SystemActions` tramite stringhe;
6. non usare `ToString()` per decidere azioni;
7. non usare reflection;
8. non modificare Bridge;
9. non modificare Audio.

Il tipo di `Tick(elapsedSeconds)` non deve essere scelto arbitrariamente.

È vietato decidere a priori tra:

```text
int
double
decimal
TimeSpan
```

Cursor deve usare esclusivamente il tipo reale già esposto dal Bridge.

Se una API necessaria non esiste, Cursor deve fermarsi e segnalarlo.

Criterio di completamento:

```text
API reali di Bridge e Audio identificate prima della scrittura dell’orchestratore
```

---

## 8. FASE 2 — Creazione progetto App

### TODO 005.02 — Creare CicloTimer.App

Creare:

```text
services/CicloTimer.App/
```

Creare:

```text
services/CicloTimer.App/CicloTimer.App.csproj
```

Il progetto deve usare:

```text
net9.0-windows
```

Motivo:

```text
CicloTimer.App deve referenziare CicloTimer.Audio, che usa net9.0-windows.
Il target net9.0-windows è necessario per compatibilità transitiva con Audio.
```

Questa scelta non autorizza:

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

Il progetto deve avere:

```text
Nullable enable
ImplicitUsings enable
RootNamespace CicloTimer.App
```

Il progetto deve referenziare solo:

```text
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

Il progetto non deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
ciclotimer.csproj
tests/*
```

Contenuto indicativo:

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

Subito dopo la creazione, aggiungere il progetto alla solution:

```bash
dotnet sln CicloTimer.sln add services/CicloTimer.App/CicloTimer.App.csproj
```

Se `dotnet sln add` fallisce, Cursor deve fermarsi e segnalarlo.

Non deve modificare manualmente la solution salvo necessità esplicita documentata nel report finale.

Criterio di completamento:

```text
CicloTimer.App creato, target net9.0-windows, riferimenti solo a Bridge e Audio, progetto aggiunto alla solution
```

---

## 9. FASE 3 — Porte interne

### TODO 005.03 — Creare ITimerBridgePort

Creare:

```text
services/CicloTimer.App/ITimerBridgePort.cs
```

Responsabilità:

```text
astrazione interna per parlare con il Bridge
```

Deve esporre operazioni equivalenti a:

```text
GetCurrentModel
Configure
Start
Pause
Resume
Reset
Tick
```

Regole:

1. usare tipi reali di Bridge;
2. non usare `object` se esistono tipi reali;
3. non duplicare tipi già presenti nel Bridge;
4. non generare testi;
5. non chiamare Core;
6. non chiamare Localization;
7. non introdurre UI;
8. `Tick` deve usare il tipo reale di `elapsedSeconds` del Bridge;
9. non fissare arbitrariamente `int`, `double`, `decimal` o `TimeSpan`.

Se il Bridge espone tipi come:

```text
TimerBridgeUpdate
TimerDisplayModel
TimerBridgeConfiguration
TimerSystemAction
SystemAction
```

o equivalenti, usare quelli reali.

I nomi sopra sono esempi, non vincoli.

Criterio di completamento:

```text
ITimerBridgePort creato con firme tipizzate coerenti con Bridge
```

---

### TODO 005.04 — Creare IAudioServicePort

Creare:

```text
services/CicloTimer.App/IAudioServicePort.cs
```

Responsabilità:

```text
astrazione interna per parlare con AudioService
```

Deve esporre:

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

1. usare `AudioServiceResult` reale del progetto Audio;
2. non usare `object`;
3. non usare `SoundPlayer`;
4. non gestire file audio;
5. non gestire focus audio;
6. non trasformare risultati audio in testi.

Criterio di completamento:

```text
IAudioServicePort creato con AudioServiceResult reale
```

---

## 10. FASE 4 — Adapter

### TODO 005.05 — Creare TimerBridgeAdapter

Creare:

```text
services/CicloTimer.App/TimerBridgeAdapter.cs
```

Responsabilità:

```text
adapter concreto tra ITimerBridgePort e Bridge reale
```

L’adapter deve:

1. ricevere il bridge reale;
2. chiamare i suoi metodi pubblici;
3. restituire update/modelli prodotti dal Bridge;
4. esporre `SystemActions` in forma tipizzata;
5. non modificare dati del Bridge;
6. non ricostruire testi;
7. non chiamare Core direttamente;
8. non chiamare Localization direttamente.

Se il Bridge non espone un metodo per ottenere modello iniziale, Cursor deve fermarsi.

Se il Bridge non espone `SystemActions` in forma utilizzabile, Cursor deve fermarsi.

Non deve modificare Bridge.

Criterio di completamento:

```text
TimerBridgeAdapter creato senza modificare Bridge
```

---

### TODO 005.06 — Creare AudioServiceAdapter

Creare:

```text
services/CicloTimer.App/AudioServiceAdapter.cs
```

Responsabilità:

```text
adapter concreto tra IAudioServicePort e AudioService reale
```

L’adapter deve:

1. ricevere `AudioService`;
2. chiamare `StartFinalAlertSound`;
3. chiamare `StopFinalAlertSound`;
4. restituire `AudioServiceResult`;
5. non interpretare come testo il risultato audio;
6. non bloccare il timer per errori audio;
7. non usare API audio direttamente.

Criterio di completamento:

```text
AudioServiceAdapter creato senza modificare Audio
```

---

## 11. FASE 5 — Tipi applicativi App

### TODO 005.07 — Creare AppCommandResult

Creare:

```text
services/CicloTimer.App/AppCommandResult.cs
```

Responsabilità:

```text
risultato applicativo neutro di un comando orchestrato
```

Deve contenere almeno:

1. `CurrentModel`;
2. `LastAudioResult`;
3. `Success`.

Regole:

1. `CurrentModel` usa il tipo reale prodotto dal Bridge;
2. `LastAudioResult` usa `AudioServiceResult?`;
3. `Success` non deve diventare false solo perché l’audio fallisce, se il comando timer è stato gestito;
4. `AudioFocusUnavailable` non è errore bloccante se il playback è riuscito;
5. nessun testo utente;
6. nessuna stringa hardcoded;
7. nessun tipo WPF;
8. nessuna eccezione esposta all’utente.

Sono ammessi campi tecnici aggiuntivi:

```text
HasAudioWarning
HasTechnicalError
UnhandledActionCount
```

Criterio di completamento:

```text
AppCommandResult creato, neutro, tipizzato e senza testi utente
```

---

### TODO 005.08 — Creare TimerAppState

Creare:

```text
services/CicloTimer.App/TimerAppState.cs
```

Responsabilità:

```text
stato corrente dell’orchestratore
```

Deve contenere almeno:

1. modello mostrabile corrente;
2. ultimo risultato audio;
3. ultimo risultato applicativo.

Regole:

1. usare tipo reale del Bridge per il modello corrente;
2. usare `AudioServiceResult?` per ultimo risultato audio;
3. usare `AppCommandResult?` o equivalente per ultimo risultato comando;
4. non essere ViewModel WPF;
5. non implementare `INotifyPropertyChanged`;
6. non contenere controlli UI;
7. non contenere testi generati dall’orchestratore;
8. non contenere timer reale;
9. essere immutabile o trattato come snapshot non modificabile dall’esterno.

Criterio di completamento:

```text
TimerAppState creato come snapshot applicativo neutro e sola lettura
```

---

### TODO 005.09 — Creare SystemActionDispatchResult

Creare:

```text
services/CicloTimer.App/SystemActionDispatchResult.cs
```

Responsabilità:

```text
risultato tecnico dell’esecuzione delle SystemActions
```

Deve contenere almeno:

1. ultimo risultato audio;
2. numero azioni eseguite;
3. numero azioni ignorate/non gestite;
4. flag tecnico di warning, se utile.

Regole:

1. nessun testo utente;
2. nessuna localization;
3. nessun tipo WPF;
4. errori audio non bloccanti;
5. azioni sconosciute ignorate o tracciate tecnicamente.

Criterio di completamento:

```text
SystemActionDispatchResult creato
```

---

## 12. FASE 6 — Dispatcher SystemActions

### TODO 005.10 — Creare SystemActionDispatcher

Creare:

```text
services/CicloTimer.App/SystemActionDispatcher.cs
```

Responsabilità:

```text
eseguire SystemActions prodotte dal Bridge
```

Mappatura prevista:

```text
StartFinalAlertSound → IAudioServicePort.StartFinalAlertSound
StopFinalAlertSound → IAudioServicePort.StopFinalAlertSound
```

Regole:

1. ricevere lista azioni tipizzata;
2. eseguire azioni nell’ordine ricevuto;
3. chiamare audio solo per azioni audio;
4. conservare risultati audio;
5. trattare errori audio come non bloccanti;
6. restituire `SystemActionDispatchResult`;
7. non dedurre audio dagli stati;
8. non leggere Core;
9. non leggere Localization;
10. non interpretare azioni tramite stringhe;
11. non usare `ToString()` per decidere azioni;
12. non usare reflection.

Per la prima implementazione è accettabile che `TimerAppOrchestrator` crei internamente `SystemActionDispatcher`.

È anche accettabile riceverlo dall’esterno, purché questo non introduca nuove dipendenze, nuove complessità o modifiche fuori perimetro.

Non è obbligatorio introdurre una nuova interfaccia `ISystemActionDispatcher` nella prima versione.

Criterio di completamento:

```text
SystemActionDispatcher creato e basato su SystemActions tipizzate
```

---

## 13. FASE 7 — Interfaccia orchestratore

### TODO 005.11 — Creare ITimerAppOrchestrator

Creare:

```text
services/CicloTimer.App/ITimerAppOrchestrator.cs
```

Responsabilità:

```text
contratto pubblico per futura UI e futuro gestore timer reale
```

Deve esporre almeno:

```text
CurrentState
Configure
Start
Pause
Resume
Reset
Tick
Dispose o Shutdown equivalente
```

Regole:

1. `CurrentState` sola lettura;
2. `Configure` usa tipi reali del Bridge;
3. `Tick` usa tipo reale di `elapsedSeconds` del Bridge;
4. `Tick` non deve fissare arbitrariamente `int`, `double`, `decimal` o `TimeSpan`;
5. ogni comando restituisce `AppCommandResult`;
6. implementare `IDisposable` o metodo equivalente di shutdown;
7. non usare `ICommand`;
8. non usare `INotifyPropertyChanged`;
9. non usare WPF;
10. non usare timer reale;
11. non usare stringhe utente hardcoded.

Criterio di completamento:

```text
ITimerAppOrchestrator creato come contratto applicativo non UI
```

---

## 14. FASE 8 — Orchestratore principale

### TODO 005.12 — Creare TimerAppOrchestrator

Creare:

```text
services/CicloTimer.App/TimerAppOrchestrator.cs
```

Responsabilità:

```text
classe principale dell’orchestratore
```

Deve:

1. ricevere `ITimerBridgePort`;
2. ricevere `IAudioServicePort`;
3. creare o ricevere `SystemActionDispatcher`;
4. usare injection via costruttore;
5. validare null in ingresso in modo tecnico;
6. ottenere modello iniziale dal Bridge;
7. conservare `CurrentState`;
8. esporre comandi applicativi;
9. inoltrare comandi al Bridge;
10. eseguire `SystemActions`;
11. conservare risultati audio;
12. restituire `AppCommandResult`;
13. implementare chiusura sicura;
14. gestire comandi ravvicinati senza crash.

Deve usare un lock privato semplice o meccanismo equivalente.

Forma concettuale:

```csharp
private readonly object _sync = new();
```

Regole:

1. non creare bridge reale direttamente se non tramite adapter;
2. non creare audio reale direttamente se non tramite adapter;
3. consentire test con fake bridge/audio;
4. nessun testo utente;
5. nessuna UI;
6. nessun timer reale;
7. nessun accesso diretto a Core;
8. nessun accesso diretto a Localization;
9. se crea internamente `SystemActionDispatcher`, deve farlo senza introdurre dipendenze fuori perimetro.

Criterio di completamento:

```text
TimerAppOrchestrator creato con injection, CurrentState e lock interno
```

---

### TODO 005.13 — Implementare inizializzazione

All’inizializzazione, `TimerAppOrchestrator` deve:

1. chiamare il Bridge tramite `ITimerBridgePort`;
2. ottenere il modello iniziale;
3. salvare `CurrentState`;
4. non chiamare Audio;
5. non generare tick;
6. non creare timer reale;
7. non aprire UI;
8. non generare testi.

Se il Bridge non fornisce modello iniziale, Cursor deve fermarsi.

Criterio di completamento:

```text
orchestratore inizializzato con modello iniziale dal Bridge
```

---

### TODO 005.14 — Implementare flusso comune dei comandi

Per ogni comando applicativo, l’orchestratore deve seguire questo ordine:

```text
1. acquisire lock privato o meccanismo equivalente
2. ricevere comando
3. chiamare Bridge tramite port
4. ricevere update/modello dal Bridge
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

Criterio di completamento:

```text
flusso comune comandi implementato senza duplicazioni e senza audio inventato
```

---

### TODO 005.15 — Implementare Configure

`Configure` deve:

1. ricevere configurazione timer usando i tipi previsti dal Bridge;
2. inoltrare al Bridge;
3. aggiornare `CurrentState`;
4. eseguire eventuali `SystemActions`;
5. restituire `AppCommandResult`.

Non deve:

1. validare direttamente le durate;
2. chiamare Core;
3. chiamare Localization;
4. generare testi.

Criterio di completamento:

```text
Configure inoltra al Bridge e aggiorna stato
```

---

### TODO 005.16 — Implementare Start

`Start` deve:

1. inoltrare al Bridge;
2. aggiornare `CurrentState`;
3. eseguire eventuali `SystemActions`;
4. restituire `AppCommandResult`.

Non deve decidere se `Start` è consentito.

Criterio di completamento:

```text
Start inoltra al Bridge e gestisce SystemActions
```

---

### TODO 005.17 — Implementare Pause

`Pause` deve:

1. inoltrare al Bridge;
2. aggiornare `CurrentState`;
3. eseguire eventuali `SystemActions`;
4. restituire `AppCommandResult`.

Caso importante:

```text
se il Bridge produce StopFinalAlertSound
l’orchestratore deve chiamare AudioService.StopFinalAlertSound
```

L’orchestratore non deve dedurre lo stop leggendo stati.

Criterio di completamento:

```text
Pause inoltra al Bridge e ferma audio solo se esiste SystemAction
```

---

### TODO 005.18 — Implementare Resume

`Resume` deve:

1. inoltrare al Bridge;
2. aggiornare `CurrentState`;
3. eseguire eventuali `SystemActions`;
4. restituire `AppCommandResult`.

Caso importante:

```text
se il Bridge produce StartFinalAlertSound
l’orchestratore deve chiamare AudioService.StartFinalAlertSound
```

L’orchestratore non deve dedurre start audio leggendo stati.

Criterio di completamento:

```text
Resume inoltra al Bridge e avvia audio solo se esiste SystemAction
```

---

### TODO 005.19 — Implementare Reset

`Reset` deve:

1. inoltrare al Bridge;
2. aggiornare `CurrentState`;
3. eseguire eventuali `SystemActions`;
4. restituire `AppCommandResult`.

Caso importante:

```text
se il Bridge produce StopFinalAlertSound
l’orchestratore deve chiamare AudioService.StopFinalAlertSound
```

L’orchestratore non deve fermare audio autonomamente nei normali comandi applicativi.

Criterio di completamento:

```text
Reset inoltra al Bridge e ferma audio solo se esiste SystemAction
```

---

### TODO 005.20 — Implementare Tick

`Tick` deve:

1. ricevere `elapsedSeconds` usando il tipo reale del Bridge;
2. non fissare arbitrariamente `int`, `double`, `decimal` o `TimeSpan`;
3. inoltrare al Bridge;
4. aggiornare `CurrentState`;
5. eseguire eventuali `SystemActions`;
6. restituire `AppCommandResult`.

Non deve:

1. generare tick;
2. usare timer reale;
3. usare `DispatcherTimer`;
4. chiamare Core direttamente;
5. chiamare Audio direttamente senza `SystemActions`.

Criterio di completamento:

```text
Tick riceve elapsedSeconds da fuori e lo inoltra al Bridge usando il tipo reale del Bridge
```

---

### TODO 005.21 — Implementare Dispose / Shutdown

`Dispose` o metodo equivalente deve:

1. tentare stop audio sicuro;
2. usare blocco fail-safe;
3. catturare eventuali eccezioni audio;
4. non propagare errori audio;
5. non rilanciare eccezioni audio dal blocco `catch`;
6. non bloccare la chiusura;
7. non chiamare UI;
8. non generare testi utente.

Comportamento previsto:

```text
Dispose
↓
try
  AudioService.StopFinalAlertSound
catch
  cattura errore tecnico audio
  non propaga eccezione
  non rilancia eccezione
↓
chiusura completata
```

Questa è l’unica eccezione ammessa al principio:

```text
audio solo tramite SystemActions
```

Il blocco `catch` può al massimo conservare un risultato tecnico interno se disponibile.

Il blocco `catch` non deve mostrare testi, chiamare UI o trasformare l’errore in messaggio utente.

Criterio di completamento:

```text
Dispose tenta stop audio e non propaga eccezioni audio
```

---

## 15. FASE 9 — Creazione progetto test App

### TODO 005.22 — Creare CicloTimer.App.Tests

Creare:

```text
tests/CicloTimer.App.Tests/
```

Creare:

```text
tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj
```

Target:

```text
net9.0-windows
```

Framework consigliato:

```text
xUnit
```

Il progetto test deve referenziare solo:

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

Subito dopo la creazione, aggiungere il progetto alla solution:

```bash
dotnet sln CicloTimer.sln add tests/CicloTimer.App.Tests/CicloTimer.App.Tests.csproj
```

Criterio di completamento:

```text
progetto test App creato, target net9.0-windows, riferimento solo ad App, aggiunto alla solution
```

---

## 16. FASE 10 — Test inizializzazione

### TODO 005.23 — Creare TimerAppOrchestratorInitializationTests

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

Criterio di completamento:

```text
TimerAppOrchestratorInitializationTests superati
```

---

## 17. FASE 11 — Test comandi

### TODO 005.24 — Creare TimerAppOrchestratorCommandTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorCommandTests.cs
```

Test obbligatori:

1. `Configure` inoltra al Bridge;
2. `Start` inoltra al Bridge;
3. `Pause` inoltra al Bridge;
4. `Resume` inoltra al Bridge;
5. `Reset` inoltra al Bridge;
6. `Tick` inoltra al Bridge con `elapsedSeconds` usando il tipo reale del Bridge;
7. `Tick` non usa un tipo scelto arbitrariamente se il Bridge espone un tipo diverso;
8. ogni comando aggiorna `CurrentState`;
9. ogni comando restituisce `AppCommandResult`;
10. l’orchestratore non valida direttamente le durate;
11. l’orchestratore non genera testi.

Criterio di completamento:

```text
TimerAppOrchestratorCommandTests superati
```

---

## 18. FASE 12 — Test SystemActions

### TODO 005.25 — Creare TimerAppOrchestratorSystemActionTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorSystemActionTests.cs
```

Test obbligatori:

1. se Bridge produce zero azioni, audio non viene chiamato;
2. se Bridge produce `StartFinalAlertSound`, audio start viene chiamato;
3. se Bridge produce `StopFinalAlertSound`, audio stop viene chiamato;
4. azioni multiple vengono eseguite in ordine;
5. azione sconosciuta viene gestita in modo sicuro;
6. Pause non ferma audio se il Bridge non produce `StopFinalAlertSound`;
7. Reset non ferma audio se il Bridge non produce `StopFinalAlertSound`;
8. Resume non avvia audio se il Bridge non produce `StartFinalAlertSound`;
9. l’orchestratore non deduce audio dagli stati;
10. le azioni non sono interpretate tramite stringhe libere o `ToString()`.

Criterio di completamento:

```text
TimerAppOrchestratorSystemActionTests superati
```

---

## 19. FASE 13 — Test risultati audio

### TODO 005.26 — Creare TimerAppOrchestratorAudioResultTests

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

Criterio di completamento:

```text
TimerAppOrchestratorAudioResultTests superati
```

---

## 20. FASE 14 — Test Dispose

### TODO 005.27 — Creare TimerAppOrchestratorDisposeTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorDisposeTests.cs
```

Test obbligatori:

1. `Dispose` tenta stop audio;
2. `Dispose` non chiama Bridge;
3. errore audio durante `Dispose` non genera eccezione non gestita;
4. errore audio durante `Dispose` non viene rilanciato;
5. `Dispose` può essere chiamato più volte;
6. `Dispose` non crea timer reale;
7. `Dispose` non modifica UI;
8. `Dispose` conserva stato sicuro;
9. `Dispose` cattura eventuali eccezioni audio.

Criterio di completamento:

```text
TimerAppOrchestratorDisposeTests superati
```

---

## 21. FASE 15 — Test safety

### TODO 005.28 — Creare TimerAppOrchestratorSafetyTests

Creare:

```text
tests/CicloTimer.App.Tests/TimerAppOrchestratorSafetyTests.cs
```

Test obbligatori:

1. comandi ripetuti non causano crash;
2. `Tick` ripetuti non causano crash;
3. comandi ravvicinati mantengono `CurrentState` coerente;
4. azioni audio non vengono duplicate se non prodotte dal Bridge;
5. eccezione del Bridge viene gestita o propagata come errore tecnico documentato;
6. eccezione audio viene gestita come non bloccante;
7. nessun timer reale viene creato;
8. esiste un meccanismo semplice di serializzazione comandi, per esempio lock privato.

Criterio di completamento:

```text
TimerAppOrchestratorSafetyTests superati
```

---

## 22. FASE 16 — Test dipendenze progetto

### TODO 005.29 — Creare ProjectDependencyTests

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
21. App non usa `ToString()` o reflection per interpretare `SystemActions`;
22. App usa tipi reali del Bridge e Audio, non `object` non necessario;
23. App non fissa arbitrariamente il tipo di `Tick` se il Bridge espone un tipo diverso.

I controlli possono essere test automatizzati o verifica documentata nel report finale.

Criterio di completamento:

```text
ProjectDependencyTests o verifiche equivalenti completati
```

---

## 23. FASE 17 — Build solution

### TODO 005.30 — Eseguire build solution

Eseguire dalla root:

```bash
dotnet build CicloTimer.sln
```

Se la build passa, Cursor non deve modificare `ciclotimer.csproj`.

Se la build fallisce perché il progetto WPF root include file sotto:

```text
services/**
```

allora Cursor può modificare solo `ciclotimer.csproj` aggiungendo o correggendo:

```xml
<Compile Remove="services/**" />
```

Dopo questa eventuale modifica, rieseguire:

```bash
dotnet build CicloTimer.sln
```

Non modificare `ciclotimer.csproj` prima di verificare errore reale di build.

Criterio di completamento:

```text
build solution riuscita, con eventuale modifica ciclotimer.csproj solo se necessaria
```

---

## 24. FASE 18 — Test finali

### TODO 005.31 — Eseguire test finali

Tutti i comandi devono essere eseguiti dalla root del repository.

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

1. test App: tutti superati;
2. test core: tutti ancora superati;
3. test localization: tutti ancora superati;
4. test bridge: tutti ancora superati;
5. test audio: tutti ancora superati;
6. test solution completa: tutti superati;
7. nessun test fallito.

Criterio di completamento:

```text
tutti i test previsti superati
```

---

## 25. FASE 19 — Verifica working tree

### TODO 005.32 — Verificare file modificati

Cursor deve verificare che siano stati creati/modificati solo:

```text
services/CicloTimer.App/
tests/CicloTimer.App.Tests/
CicloTimer.sln
```

Eventualmente:

```text
ciclotimer.csproj
.gitignore
```

solo se necessario e motivato.

Non devono comparire modifiche a:

```text
models/CicloTimer.Core/
locales/CicloTimer.Localization/
view-models/CicloTimer.Bridge/
services/CicloTimer.Audio/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
```

Criterio di completamento:

```text
working tree coerente con il perimetro del TODO
```

---

## 26. FASE 20 — Pulizia artefatti build

### TODO 005.33 — Verificare assenza bin/obj da committare

Cursor deve verificare che nel working tree non risultino file da committare sotto:

```text
bin/
obj/
```

Comandi utili:

```bash
git status --porcelain
```

PowerShell:

```powershell
git status --porcelain | Select-String -Pattern "bin|obj"
```

Se compaiono artefatti `bin/` o `obj/`, Cursor deve rimuovere solo directory chiamate esattamente:

```text
bin
obj
```

Non deve rimuovere:

```text
file .cs
file .csproj
file .sln
documentazione
services/CicloTimer.App/
tests/CicloTimer.App.Tests/
services/CicloTimer.Audio/Assets/
services/CicloTimer.Audio/Assets/final-alert.wav
```

Cursor non deve fare commit.

Criterio di completamento:

```text
nessun artefatto bin/obj da committare
```

---

## 27. FASE 21 — Report finale

### TODO 005.34 — Produrre report finale

Cursor deve produrre un report finale contenente:

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
22. elenco dei tipi reali Bridge identificati durante la ricognizione;
23. elenco dei tipi reali Audio identificati durante la ricognizione;
24. conferma tipo reale delle `SystemActions` usato;
25. conferma `SystemActions` non interpretate con stringhe/ToString/reflection;
26. conferma tipo reale di `Tick` usato;
27. conferma che il tipo di `Tick` non è stato scelto arbitrariamente;
28. conferma modello iniziale ottenuto dal Bridge;
29. conferma `CurrentState` sola lettura/immutabile;
30. conferma comandi implementati;
31. conferma `Tick` riceve `elapsedSeconds` da fuori;
32. conferma nessun timer reale;
33. conferma nessun `DispatcherTimer`;
34. conferma nessun `ICommand`;
35. conferma nessun `INotifyPropertyChanged`;
36. conferma gestione `SystemActions`;
37. conferma audio chiamato solo tramite `SystemActions`, salvo shutdown;
38. conferma gestione `AudioServiceResult`;
39. conferma successi audio parziali non bloccanti;
40. conferma errori audio non bloccanti;
41. conferma `Dispose` tenta stop audio;
42. conferma `Dispose` cattura eccezioni audio;
43. conferma `Dispose` non rilancia eccezioni audio;
44. conferma meccanismo di serializzazione comandi ravvicinati;
45. eventuale modifica a `ciclotimer.csproj` e motivo;
46. comandi eseguiti;
47. risultato build;
48. risultato test App;
49. risultato test core;
50. risultato test localization;
51. risultato test bridge;
52. risultato test audio;
53. risultato `dotnet test`;
54. numero test App;
55. eventuali test falliti;
56. eventuali deviazioni dal TODO;
57. conferma pulizia bin/obj;
58. output finale sintetico di `git status --porcelain`.

Cursor non deve limitarsi a scrivere:

```text
fatto
```

Il report deve essere verificabile.

---

## 28. Checklist sintetica finale

Prima di dichiarare completato il TODO, Cursor deve poter confermare:

```text
[ ] Verificati documenti design/coding plan
[ ] Letti Bridge e Audio reali
[ ] Identificato tipo reale modello/update Bridge
[ ] Identificato tipo reale SystemActions
[ ] Identificati valori StartFinalAlertSound / StopFinalAlertSound
[ ] Identificato tipo reale Tick/elapsedSeconds
[ ] Confermato che Tick non fissa arbitrariamente int/double/decimal/TimeSpan
[ ] Identificato AudioServiceResult reale
[ ] Creato services/CicloTimer.App/
[ ] Creato CicloTimer.App.csproj
[ ] Target App net9.0-windows
[ ] Nessun uso WPF/UI nonostante net9.0-windows
[ ] App referenzia Bridge
[ ] App referenzia Audio
[ ] App non referenzia Core diretto
[ ] App non referenzia Localization diretto
[ ] App non referenzia WPF root
[ ] Creato ITimerBridgePort
[ ] Creato IAudioServicePort
[ ] Porte con tipi reali, non object non necessario
[ ] Creato TimerBridgeAdapter
[ ] Creato AudioServiceAdapter
[ ] Creato AppCommandResult
[ ] Creato TimerAppState
[ ] CurrentState sola lettura/immutabile
[ ] Creato SystemActionDispatchResult
[ ] Creato SystemActionDispatcher
[ ] SystemActions non interpretate con stringhe/ToString/reflection
[ ] Creato ITimerAppOrchestrator
[ ] Creato TimerAppOrchestrator
[ ] Orchestratore inizializzato con modello iniziale dal Bridge
[ ] Implementato Configure
[ ] Implementato Start
[ ] Implementato Pause
[ ] Implementato Resume
[ ] Implementato Reset
[ ] Implementato Tick
[ ] Tick riceve elapsedSeconds da fuori
[ ] Tick usa il tipo reale del Bridge
[ ] Nessun timer reale
[ ] Nessun DispatcherTimer
[ ] Nessun ICommand
[ ] Nessun INotifyPropertyChanged
[ ] Audio chiamato solo via SystemActions, salvo shutdown
[ ] Dispose tenta stop audio
[ ] Dispose cattura errori audio
[ ] Dispose non rilancia errori audio
[ ] Errori audio non bloccanti
[ ] Successi audio parziali non bloccanti
[ ] Lock privato o serializzazione equivalente per comandi ravvicinati
[ ] Creato tests/CicloTimer.App.Tests/
[ ] Target test net9.0-windows
[ ] Creati test inizializzazione
[ ] Creati test comandi
[ ] Creati test SystemActions
[ ] Creati test risultati audio
[ ] Creati test Dispose
[ ] Creati test safety
[ ] Creati ProjectDependencyTests
[ ] dotnet build CicloTimer.sln superato
[ ] dotnet test App superato
[ ] dotnet test Core superato
[ ] dotnet test Localization superato
[ ] dotnet test Bridge superato
[ ] dotnet test Audio superato
[ ] dotnet test generale superato se eseguito
[ ] Nessuna modifica a Core
[ ] Nessuna modifica a Localization
[ ] Nessuna modifica a Bridge
[ ] Nessuna modifica ad Audio
[ ] Nessuna modifica a UI
[ ] Nessun bin/obj da committare
[ ] Report finale prodotto
```

---

## 29. Criteri di completamento globale

Il TODO 005 è completato solo se:

1. il progetto App esiste;
2. il progetto test App esiste;
3. entrambi sono nella solution;
4. App usa `net9.0-windows`;
5. App.Tests usa `net9.0-windows`;
6. App non usa WPF/UI;
7. App dipende solo da Bridge e Audio;
8. App non dipende direttamente da Core;
9. App non dipende direttamente da Localization;
10. App non dipende dal progetto WPF root;
11. le porte usano tipi reali;
12. non viene usato `object` non necessario;
13. `SystemActions` sono tipizzate;
14. `SystemActions` non sono interpretate tramite stringhe;
15. `Tick` usa il tipo reale del Bridge;
16. `Tick` non fissa arbitrariamente `int`, `double`, `decimal` o `TimeSpan`;
17. `CurrentState` è sola lettura o immutabile;
18. l’orchestratore riceve modello iniziale dal Bridge;
19. i comandi applicativi sono implementati;
20. il flusso comune dei comandi è rispettato;
21. audio viene chiamato solo tramite `SystemActions`, salvo shutdown;
22. `Dispose` tenta stop audio;
23. `Dispose` cattura errori audio;
24. `Dispose` non rilancia errori audio;
25. errori audio non bloccano;
26. successi audio parziali non bloccano;
27. comandi ravvicinati non corrompono stato;
28. non viene creato timer reale;
29. non viene usato `DispatcherTimer`;
30. non vengono usati `ICommand` o `INotifyPropertyChanged`;
31. non vengono generati testi utente hardcoded;
32. test App passano;
33. regressioni core/localization/bridge/audio passano;
34. build solution passa;
35. non ci sono artefatti `bin/obj` da committare;
36. il report finale è completo.

---

## 30. Criteri di non validità

L’implementazione non è valida se:

1. crea App fuori da `services/CicloTimer.App/`;
2. crea `src/`;
3. crea `orchestrators/`;
4. mette App in `models/`;
5. mette App in `locales/`;
6. mette App in `view-models/`;
7. mette App dentro `services/CicloTimer.Audio/`;
8. modifica Core;
9. modifica Localization;
10. modifica Bridge;
11. modifica Audio;
12. modifica UI;
13. usa `net9.0` per App causando incompatibilità con Audio;
14. usa `net9.0-windows` per introdurre WPF/UI;
15. App referenzia Core diretto;
16. App referenzia Localization diretto;
17. App referenzia WPF root;
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
30. interpreta `SystemActions` tramite stringhe libere, `ToString()` o reflection;
31. costruisce testi utente;
32. introduce stringhe hardcoded per l’utente;
33. usa `object` nelle porte nonostante esistano tipi reali;
34. sceglie arbitrariamente il tipo di `Tick` invece di usare quello reale del Bridge;
35. blocca il timer se audio fallisce;
36. considera `AudioFocusUnavailable` come errore bloccante se playback è riuscito;
37. non tenta stop audio in `Dispose`;
38. propaga o rilancia eccezioni audio da `Dispose`;
39. non protegge comandi ravvicinati;
40. build fallisce;
41. test falliscono;
42. ci sono artefatti `bin/obj` da committare.

---

## 31. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. il TODO 005 è approvato come checklist operativa dell’orchestratore applicativo;
2. il progetto App resta in `services/CicloTimer.App/`;
3. i test restano in `tests/CicloTimer.App.Tests/`;
4. App usa `net9.0-windows` per compatibilità con Audio;
5. App.Tests usa `net9.0-windows`;
6. `net9.0-windows` non autorizza WPF, XAML, UI o timer reale;
7. App dipende solo da Bridge e Audio;
8. App non dipende direttamente da Core;
9. App non dipende direttamente da Localization;
10. App non dipende da WPF root;
11. le porte devono usare tipi reali;
12. `object` è vietato se esistono tipi reali;
13. `SystemActions` devono essere tipizzate;
14. `SystemActions` non devono essere interpretate tramite stringhe, `ToString()` o reflection;
15. `Tick` deve usare il tipo reale esposto dal Bridge;
16. è vietato scegliere arbitrariamente `int`, `double`, `decimal` o `TimeSpan` per `Tick`;
17. `CurrentState` deve essere sola lettura o immutabile;
18. per la prima implementazione `SystemActionDispatcher` può essere creato internamente o ricevuto, senza nuova interfaccia obbligatoria;
19. `Dispose` deve tentare stop audio;
20. `Dispose` deve catturare errori audio;
21. `Dispose` non deve propagare o rilanciare errori audio;
22. il report finale deve elencare i tipi reali Bridge/Audio identificati durante la ricognizione;
23. errori audio non bloccano il timer;
24. successi audio parziali non bloccano il timer;
25. comandi ravvicinati devono essere protetti da lock privato o serializzazione equivalente;
26. nessuna modifica a Core, Localization, Bridge, Audio o UI è autorizzata.

---

## 32. Stato del documento

Questo documento è approvato come TODO 005 — Orchestratore applicativo timer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione tecnica
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni revisione: divieto di scelta arbitraria del tipo Tick, Dispose fail-safe senza rilancio di eccezioni audio, SystemActionDispatcher accettabile interno o ricevuto senza interfaccia obbligatoria, report finale con elenco dei tipi reali Bridge/Audio identificati
```

Il documento è approvato come base operativa per l’implementazione dell’orchestratore applicativo.

