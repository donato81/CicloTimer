# CicloTimer — TODO 002 — Bridge UI-logica e modello mostrabile del timer

**Tipo documento:** todo operativo  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/2-coding-plans/002-coding-plan-bridge-ui-logica-timer.md  

---

## 1. Scopo del TODO

Questo documento traduce il Coding Plan 002 in una lista operativa eseguibile da Cursor.

L'obiettivo è implementare il bridge UI-logica del timer come progetto .NET separato.

Il bridge deve stare in:

```text
view-models/CicloTimer.Bridge/
````

I test del bridge devono stare in:

```text
tests/CicloTimer.Bridge.Tests/
```

Il bridge deve collegare:

```text
CicloTimer.Core
CicloTimer.Localization
```

e produrre sempre:

```text
TimerBridgeUpdate
```

composto da:

```text
TimerDisplayModel
SystemActions
```

Il TODO deve guidare Cursor in modo vincolato, evitando modifiche fuori perimetro a core, localization, UI, audio reale, WPF o orchestrazione.

---

## 2. Principio operativo

Il principio operativo è:

```text
il bridge traduce, non decide
```

Il bridge deve:

1. ricevere comandi concettuali;
2. inoltrarli al core;
3. usare `TimerCommandResult` per eventi ed errori del comando corrente;
4. usare `TimerEngine` come fonte primaria dello stato corrente;
5. usare `CicloTimer.Localization` per tutti i testi utente;
6. produrre sempre `TimerBridgeUpdate`;
7. produrre richieste audio/sistema solo concettuali.

Il bridge non deve:

1. implementare logica core;
2. implementare UI;
3. implementare audio reale;
4. implementare timer reale;
5. chiamare API Windows;
6. generare tick autonomamente;
7. contenere stringhe utente hardcoded.

---

## 3. Perimetro autorizzato

Cursor può:

1. creare `view-models/CicloTimer.Bridge/`;
2. creare `view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj`;
3. aggiungere subito il progetto bridge a `CicloTimer.sln`;
4. referenziare dal bridge:

   * `models/CicloTimer.Core/CicloTimer.Core.csproj`;
   * `locales/CicloTimer.Localization/CicloTimer.Localization.csproj`;
5. creare i file di produzione del bridge;
6. creare `tests/CicloTimer.Bridge.Tests/`;
7. creare `tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj`;
8. aggiungere subito il progetto test bridge a `CicloTimer.sln`;
9. referenziare dal progetto test:

   * `view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj`;
   * `models/CicloTimer.Core/CicloTimer.Core.csproj`;
   * `locales/CicloTimer.Localization/CicloTimer.Localization.csproj`;
10. creare i test automatici previsti;
11. eseguire build e test dalla root del repository;
12. modificare `ciclotimer.csproj` solo se la build fallisce perché il progetto WPF include `view-models/**`;
13. aggiornare `.gitignore` solo se emergono nuovi artefatti non ignorati;
14. produrre report finale.

---

## 4. Fuori perimetro assoluto

Cursor non deve:

1. modificare `models/CicloTimer.Core/`;
2. modificare `locales/CicloTimer.Localization/`;
3. modificare `MainWindow.xaml`;
4. modificare `MainWindow.xaml.cs`;
5. modificare `App.xaml`;
6. modificare `App.xaml.cs`;
7. modificare UI WPF;
8. implementare audio reale;
9. implementare audio focus;
10. interrompere, attenuare o ripristinare audio di altre applicazioni;
11. chiamare API Windows;
12. usare NVDA operativo;
13. usare UI Automation;
14. creare Live Region;
15. creare timer reale;
16. usare `DispatcherTimer`;
17. usare `System.Timers.Timer`;
18. generare tick autonomamente;
19. usare thread UI;
20. implementare orchestratore;
21. usare `ICommand`;
22. usare `INotifyPropertyChanged`;
23. usare WPF;
24. usare XAML;
25. usare database;
26. usare cloud;
27. creare file JSON/XML/RESX;
28. creare nuove lingue;
29. modificare formato tempo `mm:ss`;
30. modificare testi localization;
31. modificare regole core;
32. creare cartelle `src/`;
33. creare `models/CicloTimer.Bridge/`;
34. creare `locales/CicloTimer.Bridge/`;
35. creare `view-models/TimerBridge/`.

Se Cursor rileva che una modifica fuori perimetro è necessaria, deve fermarsi e segnalarlo nel report.

---

## 5. Struttura finale attesa

La struttura finale deve essere:

```text
view-models/
  CicloTimer.Bridge/
    CicloTimer.Bridge.csproj
    TimerBridge.cs
    TimerBridgeUpdate.cs
    TimerDisplayModel.cs
    SystemActionRequest.cs
    TimerInput.cs
    TimeFormatter.cs
    TimerStateTextMapper.cs
    TimerErrorTextMapper.cs
    TimerEventTextMapper.cs
    PrimaryActionResolver.cs
    SystemActionResolver.cs

tests/
  CicloTimer.Bridge.Tests/
    CicloTimer.Bridge.Tests.csproj
    TimeFormatterTests.cs
    TimerBridgeMappersAndResolversTests.cs
    TimerBridgeCoreInteractionsTests.cs
    TimerBridgeUpdateTests.cs
    ProjectDependencyTests.cs
```

Sono vietate collocazioni alternative:

```text
src/
models/CicloTimer.Bridge/
locales/CicloTimer.Bridge/
view-models/TimerBridge/
```

---

## 6. FASE 0 — Ricognizione iniziale

### TODO 002.00 — Verificare repository prima di modificare

Cursor deve leggere e verificare:

1. presenza di `CicloTimer.sln`;
2. presenza di `models/CicloTimer.Core/`;
3. presenza di `locales/CicloTimer.Localization/`;
4. presenza di `tests/CicloTimer.Core.Tests/`;
5. presenza di `tests/CicloTimer.Localization.Tests/`;
6. presenza di `view-models/`;
7. presenza di `docs/1-design/002-design-bridge-ui-logica-timer.md`;
8. presenza di `docs/2-coding-plans/002-coding-plan-bridge-ui-logica-timer.md`;
9. contenuto attuale di `.gitignore`;
10. contenuto attuale di `ciclotimer.csproj`;
11. assenza di `view-models/CicloTimer.Bridge/`;
12. assenza di `tests/CicloTimer.Bridge.Tests/`.

Risultato atteso:

```text
ricognizione completata
nessuna modifica ancora eseguita
```

Se `view-models/CicloTimer.Bridge/` o `tests/CicloTimer.Bridge.Tests/` esistono già, Cursor deve segnalarlo e non sovrascrivere file senza controllo.

---

## 7. FASE 1 — Verifica preventiva localization

### TODO 002.01 — Verificare template sessione completata

Prima di creare il bridge, Cursor deve verificare che `CicloTimer.Localization` esponga già:

```text
AccessibilityTextKey.SessionCompletedTemplate
```

oppure una chiave equivalente già definita e testata per produrre:

```text
Sessione completata. Sessioni completate: {0}.
```

Cursor deve verificare anche che esistano le chiavi localization necessarie per:

```text
TimerTextKey
CommandTextKey
ErrorTextKey
AccessibilityTextKey
UiTextKey
```

Criterio di completamento:

```text
template e chiavi localization necessarie presenti
```

Se il template non esiste:

```text
Cursor deve fermarsi
Cursor non deve modificare localization
Cursor deve segnalarlo nel report
```

---

## 8. FASE 2 — Creazione progetto Bridge

### TODO 002.02 — Creare progetto CicloTimer.Bridge

Creare:

```text
view-models/CicloTimer.Bridge/
```

Creare:

```text
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
```

Il progetto deve usare:

```text
net9.0
```

Il progetto non deve usare:

```text
net9.0-windows
```

Il progetto bridge deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

Il progetto bridge non deve referenziare:

```text
ciclotimer.csproj
tests/*
WPF
WindowsBase
PresentationFramework
System.Windows
UIAutomation
```

Contenuto indicativo:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>CicloTimer.Bridge</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\models\CicloTimer.Core\CicloTimer.Core.csproj" />
    <ProjectReference Include="..\..\locales\CicloTimer.Localization\CicloTimer.Localization.csproj" />
  </ItemGroup>
</Project>
```

Il path concreto deve essere verificato da Cursor.

Subito dopo la creazione, aggiungere il progetto alla solution dalla root del repository:

```bash
dotnet sln CicloTimer.sln add view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
```

Criterio di completamento:

```text
CicloTimer.Bridge creato, target net9.0, riferimenti corretti, aggiunto alla solution
```

---

## 9. FASE 3 — Modelli dati bridge

### TODO 002.03 — Creare TimerDisplayModel

Creare:

```text
view-models/CicloTimer.Bridge/TimerDisplayModel.cs
```

Forma consigliata:

```csharp
public sealed record TimerDisplayModel(
    string RemainingTimeText,
    string TimerStateText,
    string CompletedSessionsText,
    string PrimaryActionText,
    bool CanStart,
    bool CanPause,
    bool CanResume,
    bool CanReset,
    bool IsConfigured,
    bool IsFinalAlertActive,
    string ErrorMessageText,
    string EventMessageText,
    string AccessibleStatusText,
    string AccessibleEventText);
```

Regole:

1. nessun campo testuale deve essere `null`;
2. in assenza di errore, `ErrorMessageText = string.Empty`;
3. in assenza di evento, `EventMessageText = string.Empty`;
4. in assenza di evento, `AccessibleEventText = string.Empty`;
5. `AccessibleStatusText` deve essere sempre valorizzato;
6. nessun tipo WPF;
7. nessun audio;
8. nessun comando UI.

Criterio di completamento:

```text
TimerDisplayModel creato e indipendente da UI/WPF/audio
```

---

### TODO 002.04 — Creare SystemActionRequest

Creare:

```text
view-models/CicloTimer.Bridge/SystemActionRequest.cs
```

Forma consigliata:

```csharp
public enum SystemActionRequest
{
    StartFinalAlertSound,
    StopFinalAlertSound
}
```

Regole:

1. non creare `None`;
2. assenza di azioni = lista vuota in `TimerBridgeUpdate`;
3. nessun audio reale;
4. nessuna API Windows;
5. nessun audio focus;
6. nessuna coda persistente;
7. nessun `Channel`;
8. nessun evento C# come meccanismo principale.

Criterio di completamento:

```text
SystemActionRequest creato come enum concettuale puro
```

---

### TODO 002.05 — Creare TimerBridgeUpdate

Creare:

```text
view-models/CicloTimer.Bridge/TimerBridgeUpdate.cs
```

Forma consigliata:

```csharp
public sealed record TimerBridgeUpdate(
    TimerDisplayModel DisplayModel,
    IReadOnlyList<SystemActionRequest> SystemActions);
```

Regole:

1. `DisplayModel` non deve essere `null`;
2. `SystemActions` non deve essere `null`;
3. se non ci sono azioni, usare lista vuota;
4. ogni metodo pubblico del bridge deve restituire `TimerBridgeUpdate`.

Criterio di completamento:

```text
TimerBridgeUpdate creato con DisplayModel e SystemActions sempre presenti
```

---

### TODO 002.06 — Creare TimerInput

Creare:

```text
view-models/CicloTimer.Bridge/TimerInput.cs
```

Forma consigliata:

```csharp
public sealed record TimerInput(
    int SessionMinutes,
    int SessionSeconds,
    int FinalAlertMinutes,
    int FinalAlertSeconds);
```

Regole:

1. usare `int`;
2. il bridge converte minuti/secondi in secondi totali;
3. il bridge non valida logicamente al posto del core.

Criterio di completamento:

```text
TimerInput creato per input UI futuro
```

---

## 10. FASE 4 — Formatter tempo

### TODO 002.07 — Creare TimeFormatter

Creare:

```text
view-models/CicloTimer.Bridge/TimeFormatter.cs
```

Firma consigliata:

```csharp
public static string Format(int seconds)
```

Formato:

```text
mm:ss
```

Casi obbligatori:

```text
0      → 00:00
5      → 00:05
59     → 00:59
60     → 01:00
299    → 04:59
300    → 05:00
3600   → 60:00
3661   → 61:01
valore negativo → 00:00
```

Regole:

1. usare `int`;
2. non introdurre `hh:mm:ss`;
3. non usare localization per il formato `mm:ss`;
4. non generare annunci;
5. non usare UI.

Criterio di completamento:

```text
TimeFormatter implementato e testabile
```

---

## 11. FASE 5 — Mapper testi

### TODO 002.08 — Creare TimerStateTextMapper

Creare:

```text
view-models/CicloTimer.Bridge/TimerStateTextMapper.cs
```

Mappatura obbligatoria:

```text
Stopped     → TimerTextKey.StateStopped
Running     → TimerTextKey.StateRunning
FinalAlert  → TimerTextKey.StateFinalAlert
Paused      → TimerTextKey.StatePaused
```

Regole:

1. usare `LocalizationService` o servizio equivalente già implementato;
2. niente stringhe hardcoded;
3. niente stringhe libere come chiavi;
4. niente `TimerState.ToString()` per ottenere chiavi;
5. niente reflection;
6. ogni stato deve essere gestito.

Criterio di completamento:

```text
stati core mappati verso testi localization
```

---

### TODO 002.09 — Creare TimerErrorTextMapper

Creare:

```text
view-models/CicloTimer.Bridge/TimerErrorTextMapper.cs
```

Mappatura obbligatoria:

```text
InvalidSessionDuration → ErrorTextKey.InvalidSessionDuration
InvalidFinalAlertDuration → ErrorTextKey.InvalidFinalAlertDuration
FinalAlertNotLessThanSessionDuration → ErrorTextKey.FinalAlertNotLessThanSessionDuration
TimerNotConfigured → ErrorTextKey.TimerNotConfigured
CannotStart → ErrorTextKey.CannotStart
CannotPause → ErrorTextKey.CannotPause
CannotResume → ErrorTextKey.CannotResume
CannotReset → ErrorTextKey.CannotReset
InvalidTickDuration → ErrorTextKey.InvalidTickDuration
```

Regole:

1. usare `TimerCommandResult.Errors` come lista;
2. se `TimerCommandResult.Errors` contiene più errori, usare il primo secondo l'ordine restituito dal core;
3. se `TimerCommandResult.Errors` è vuota, restituire `string.Empty`;
4. niente stringhe hardcoded;
5. niente `TimerError.ToString()` per ottenere chiavi;
6. niente reflection.

Criterio di completamento:

```text
errori core mappati verso messaggi localization
```

---

### TODO 002.10 — Creare TimerEventTextMapper

Creare:

```text
view-models/CicloTimer.Bridge/TimerEventTextMapper.cs
```

Mappatura obbligatoria:

```text
TimerConfigured → TimerTextKey.EventTimerConfigured
TimerStarted → TimerTextKey.EventTimerStarted
TimerPaused → TimerTextKey.EventTimerPaused
TimerResumed → TimerTextKey.EventTimerResumed
TimerReset → TimerTextKey.EventTimerReset
FinalAlertStarted → TimerTextKey.EventFinalAlertStarted
SessionCompleted → TimerTextKey.EventSessionCompleted
SessionCounterIncremented → TimerTextKey.EventSessionCounterIncremented
NextSessionStarted → TimerTextKey.EventNextSessionStarted
ValidationFailed → TimerTextKey.EventValidationFailed
```

Regole:

1. leggere solo eventi del `TimerCommandResult` corrente;
2. non accumulare eventi storici;
3. non creare code;
4. non duplicare eventi;
5. rispettare ordine core;
6. se non ci sono eventi, restituire `string.Empty`.

Sintesi sessione completata:

se nello stesso result sono presenti:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

produrre messaggio sintetico usando:

```text
AccessibilityTextKey.SessionCompletedTemplate
```

Esempio:

```text
Sessione completata. Sessioni completate: 3.
```

Se il template non esiste, Cursor deve fermarsi e segnalarlo.

Criterio di completamento:

```text
eventi core mappati verso testi localization e sintesi sessione completata gestita
```

---

## 12. FASE 6 — Resolver

### TODO 002.11 — Creare PrimaryActionResolver

Creare:

```text
view-models/CicloTimer.Bridge/PrimaryActionResolver.cs
```

Mappatura obbligatoria:

```text
Stopped + CanStart true → Avvia
Running + CanPause true → Pausa
FinalAlert + CanPause true → Pausa
Paused + CanResume true → Riprendi
nessuna azione disponibile → string.Empty
```

Regole:

1. usare `CommandTextKey`;
2. testo finale da localization;
3. il bridge non decide disponibilità logica;
4. usare solo `CanStart`, `CanPause`, `CanResume`, `CanReset` dal core.

Criterio di completamento:

```text
azione principale risolta tramite localization
```

---

### TODO 002.12 — Creare SystemActionResolver

Creare:

```text
view-models/CicloTimer.Bridge/SystemActionResolver.cs
```

Responsabilità:

```text
determinare StartFinalAlertSound e StopFinalAlertSound come richieste concettuali
```

Il resolver non deve leggere direttamente dal core.

Il `TimerBridge` deve passare al resolver le informazioni necessarie.

Parametri concettuali consigliati:

```csharp
IReadOnlyList<TimerEvent> events
bool wasFinalAlertActive
bool isFinalAlertActive
```

La firma concreta può variare, ma queste informazioni devono essere disponibili.

Regole `StartFinalAlertSound`:

```text
se events contiene FinalAlertStarted → StartFinalAlertSound
```

Regole `StopFinalAlertSound`:

produrre solo se esiste evidenza di final alert attivo o sessione in chiusura.

Evidenze ammesse:

1. `wasFinalAlertActive == true`;
2. result corrente contiene eventi di fine sessione;
3. comando corrente è pausa o reset mentre final alert era attivo.

Casi obbligatori:

```text
SessionCompleted / SessionCounterIncremented / NextSessionStarted → StopFinalAlertSound
TimerPaused durante final alert → StopFinalAlertSound
TimerReset durante final alert → StopFinalAlertSound
pausa fuori final alert → nessuna azione
reset fuori final alert → nessuna azione
nessun evento rilevante → lista vuota
```

Regole:

1. niente audio reale;
2. niente API Windows;
3. niente volume;
4. niente audio focus;
5. solo lista di richieste concettuali;
6. lista mai `null`.

Criterio di completamento:

```text
azioni audio/sistema concettuali prodotte solo quando coerenti
```

---

## 13. FASE 7 — TimerBridge

### TODO 002.13 — Creare TimerBridge

Creare:

```text
view-models/CicloTimer.Bridge/TimerBridge.cs
```

Forma consigliata:

```csharp
public sealed class TimerBridge
{
    public TimerBridgeUpdate Configure(TimerInput input);
    public TimerBridgeUpdate Start();
    public TimerBridgeUpdate Pause();
    public TimerBridgeUpdate Resume();
    public TimerBridgeUpdate Reset();
    public TimerBridgeUpdate Tick(int elapsedSeconds);
    public TimerBridgeUpdate GetCurrentUpdate();
}
```

I nomi possono essere adattati, ma i comandi concettuali devono esserci.

Regole generali dopo ogni comando:

1. salvare stato precedente necessario, almeno `wasFinalAlertActive`;
2. chiamare il core;
3. usare `TimerCommandResult` per eventi ed errori;
4. leggere `TimerEngine` per stato corrente;
5. costruire `TimerDisplayModel`;
6. calcolare `SystemActions`;
7. restituire `TimerBridgeUpdate`.

### Configure

`Configure` deve:

1. convertire `TimerInput` in secondi totali;
2. creare `TimerConfiguration` del core;
3. chiamare `ConfigureTimer`;
4. restituire `TimerBridgeUpdate`.

Conversione:

```text
SessionDurationSeconds = SessionMinutes * 60 + SessionSeconds
FinalAlertDurationSeconds = FinalAlertMinutes * 60 + FinalAlertSeconds
```

Il bridge non deve bloccare valori logicamente non validi prima del core.

### Tick

`Tick` deve:

1. ricevere `int elapsedSeconds`;
2. inoltrarlo al core così come ricevuto;
3. non aggregare tick autonomamente;
4. non correggere o simulare il passo temporale;
5. non generare tempo autonomamente;
6. non creare timer reali;
7. restituire `TimerBridgeUpdate`;
8. produrre azioni concettuali se il core genera eventi rilevanti.

Nel futuro ciclo reale dell'app, il valore ordinario previsto sarà:

```text
elapsedSeconds = 1
```

Il bridge non deve però possedere la sorgente temporale e non deve decidere autonomamente la cadenza dei tick.

`Tick` non è comando utente.

### GetCurrentUpdate

`GetCurrentUpdate` deve:

1. non chiamare nuovi comandi core;
2. leggere stato corrente del `TimerEngine`;
3. produrre `TimerDisplayModel`;
4. restituire `TimerBridgeUpdate` con `SystemActions` vuota.

Criterio di completamento:

```text
TimerBridge coordina core/localization/mapper/resolver e restituisce sempre TimerBridgeUpdate
```

---

## 14. FASE 8 — CompletedSessionsText

### TODO 002.14 — Implementare CompletedSessionsText

Il bridge deve produrre:

```text
Sessioni completate: X
```

senza hardcodare la label.

Regole:

1. recuperare label da `UiTextKey.CompletedSessions`;
2. combinare label e valore numerico;
3. non modificare localization;
4. non aggiungere nuovi template.

Esempio:

```text
UiTextKey.CompletedSessions → Sessioni completate
CompletedSessions = 3
CompletedSessionsText = Sessioni completate: 3
```

Criterio di completamento:

```text
contatore sessioni mostrabile prodotto usando localization
```

---

## 15. FASE 9 — AccessibleStatusText

### TODO 002.15 — Implementare AccessibleStatusText

Il bridge deve produrre sempre `AccessibleStatusText`.

Usare:

```text
AccessibilityTextKey.StatusTemplate
```

con tre parametri:

```text
RemainingTimeText
TimerStateText
CompletedSessionsText
```

Esempio:

```text
Tempo rimanente: 04:59. Sessione in corso. Sessioni completate: 3.
```

Regole:

1. non annunciare automaticamente il testo;
2. non usare NVDA;
3. non usare UI Automation;
4. non creare Live Region;
5. preparare solo il testo.

Criterio di completamento:

```text
AccessibleStatusText sempre valorizzato tramite localization
```

---

## 16. FASE 10 — AccessibleEventText

### TODO 002.16 — Implementare AccessibleEventText

Regole:

1. se c'è evento rilevante, usare stesso testo di `EventMessageText` oppure template accessibile coerente;
2. se non ci sono eventi, usare `string.Empty`;
3. non usare NVDA;
4. non usare UI Automation;
5. non creare Live Region.

Criterio di completamento:

```text
AccessibleEventText coerente e vuoto in assenza di eventi
```

---

## 17. FASE 11 — Creazione progetto test

### TODO 002.17 — Creare CicloTimer.Bridge.Tests

Creare:

```text
tests/CicloTimer.Bridge.Tests/
```

Creare:

```text
tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj
```

Il progetto deve usare:

```text
net9.0
```

Framework consigliato:

```text
xUnit
```

Il progetto test deve referenziare:

```text
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

Il progetto test non deve referenziare:

```text
ciclotimer.csproj
WPF
WindowsBase
PresentationFramework
System.Windows
UIAutomation
```

Subito dopo la creazione, aggiungere il progetto alla solution dalla root:

```bash
dotnet sln CicloTimer.sln add tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj
```

Criterio di completamento:

```text
progetto test bridge creato, riferimenti corretti, aggiunto alla solution
```

---

## 18. FASE 12 — Test TimeFormatter

### TODO 002.18 — Creare TimeFormatterTests

Creare:

```text
tests/CicloTimer.Bridge.Tests/TimeFormatterTests.cs
```

Test obbligatori:

```text
0 → 00:00
5 → 00:05
59 → 00:59
60 → 01:00
299 → 04:59
300 → 05:00
3600 → 60:00
3661 → 61:01
valore negativo → 00:00
```

Criterio di completamento:

```text
TimeFormatterTests superati
```

---

## 19. FASE 13 — Test mapper e resolver

### TODO 002.19 — Creare TimerBridgeMappersAndResolversTests

Creare:

```text
tests/CicloTimer.Bridge.Tests/TimerBridgeMappersAndResolversTests.cs
```

Test obbligatori stati:

```text
Stopped → Timer fermo
Running → Sessione in corso
FinalAlert → Avviso finale in corso
Paused → Timer in pausa
```

Test obbligatori errori:

1. ogni `TimerError` viene mappato a testo utente corretto;
2. in presenza di più errori viene usato il primo;
3. assenza di errori produce `string.Empty`.

Test obbligatori eventi:

1. ogni `TimerEvent` viene mappato a testo evento corretto;
2. eventi vuoti producono `string.Empty`;
3. eventi singoli producono il messaggio corrispondente;
4. `[SessionCompleted, SessionCounterIncremented, NextSessionStarted]` nello stesso result produce messaggio sintetico;
5. eventi collegati in result separati non devono essere sintetizzati artificialmente.

Test obbligatori primary action:

```text
Stopped + CanStart true → Avvia
Running + CanPause true → Pausa
FinalAlert + CanPause true → Pausa
Paused + CanResume true → Riprendi
nessuna azione disponibile → string.Empty
```

Test obbligatori system action:

1. `FinalAlertStarted` produce `StartFinalAlertSound`;
2. fine sessione produce `StopFinalAlertSound`;
3. pausa durante final alert produce `StopFinalAlertSound`;
4. reset durante final alert produce `StopFinalAlertSound`;
5. pausa fuori final alert non produce `StopFinalAlertSound`;
6. reset fuori final alert non produce `StopFinalAlertSound`;
7. nessun evento rilevante produce lista vuota;
8. `SystemActions` non è mai null.

Criterio di completamento:

```text
test mapper e resolver superati
```

---

## 20. FASE 14 — Test TimerBridgeUpdate

### TODO 002.20 — Creare TimerBridgeUpdateTests

Creare:

```text
tests/CicloTimer.Bridge.Tests/TimerBridgeUpdateTests.cs
```

Test obbligatori:

1. ogni comando restituisce `TimerBridgeUpdate`;
2. `DisplayModel` non è null;
3. `SystemActions` non è null;
4. senza azioni, `SystemActions` è lista vuota;
5. `EventMessageText` è vuoto senza eventi;
6. `AccessibleEventText` è vuoto senza eventi;
7. `AccessibleStatusText` è sempre valorizzato.

Criterio di completamento:

```text
TimerBridgeUpdateTests superati
```

---

## 21. FASE 15 — Test interazioni core bridge

### TODO 002.21 — Creare TimerBridgeCoreInteractionsTests

Creare:

```text
tests/CicloTimer.Bridge.Tests/TimerBridgeCoreInteractionsTests.cs
```

Test configurazione:

1. `5 min 0 sec` diventa 300 secondi;
2. `0 min 20 sec` avviso finale diventa 20 secondi;
3. configurazione valida produce `RemainingTimeText = 05:00`;
4. configurazione non valida passa dal core e produce errore localization;
5. il bridge non blocca la validazione logica prima del core.

Test comandi:

1. `Start()` produce stato mostrabile running;
2. `Pause()` produce stato mostrabile paused;
3. `Resume()` produce stato mostrabile running/final alert secondo core;
4. `Reset()` produce stato stopped;
5. reset non azzera `CompletedSessions`.

Test tick:

1. tick riduce stato tramite core, non tramite bridge autonomo;
2. `Tick(1)` è il caso ordinario previsto per il futuro timer reale;
3. il bridge inoltra il valore ricevuto al core senza aggregarlo o modificarlo;
4. ingresso final alert produce `StartFinalAlertSound`;
5. sessione completata produce messaggio sintetico e `StopFinalAlertSound`;
6. tick senza eventi rilevanti produce `SystemActions` vuota;
7. tick non genera annunci automatici.

Test `CompletedSessionsText`:

1. `CompletedSessionsText` usa la label da `UiTextKey.CompletedSessions`;
2. il valore numerico viene concatenato correttamente;
3. esempio atteso: `Sessioni completate: 3`;
4. la label non deve essere hardcoded nel bridge.

Criterio di completamento:

```text
TimerBridgeCoreInteractionsTests superati
```

---

## 22. FASE 16 — ProjectDependencyTests

### TODO 002.22 — Creare ProjectDependencyTests

Creare:

```text
tests/CicloTimer.Bridge.Tests/ProjectDependencyTests.cs
```

Verifiche automatizzate obbligatorie, se semplici e robuste:

1. `CicloTimer.Bridge.csproj` usa `net9.0`;
2. non usa `net9.0-windows`;
3. referenzia `CicloTimer.Core`;
4. referenzia `CicloTimer.Localization`;
5. non referenzia `ciclotimer.csproj`;
6. non referenzia WPF;
7. non contiene `System.Windows`;
8. non contiene `DispatcherTimer`;
9. non contiene `System.Timers.Timer`;
10. non contiene `AutomationProperties`;
11. non contiene NVDA;
12. non contiene API Windows;
13. non contiene audio API;
14. non contiene cartella `src`.

Le seguenti verifiche non devono generare test fragili se non sono semplici da automatizzare:

```text
assenza di .ToString() usato per generare chiavi localization
assenza di GetType() usato per generare chiavi localization
assenza di typeof( usato per generare chiavi localization
assenza di System.Reflection usato per generare chiavi localization
```

Per questi punti Cursor deve fare una verifica documentata nel report finale.

Criterio di completamento:

```text
ProjectDependencyTests o verifica equivalente completati
```

---

## 23. FASE 17 — Build iniziale solution

### TODO 002.23 — Eseguire build prima di modificare ciclotimer.csproj

Eseguire dalla root:

```bash
dotnet build CicloTimer.sln
```

Se la build passa, Cursor non deve modificare `ciclotimer.csproj`.

Se la build fallisce perché il progetto WPF root include file sotto:

```text
view-models/**
```

allora Cursor può modificare solo `ciclotimer.csproj` aggiungendo:

```xml
<Compile Remove="view-models/**" />
```

Dopo questa eventuale modifica, rieseguire:

```bash
dotnet build CicloTimer.sln
```

Criterio di completamento:

```text
build solution riuscita, con eventuale modifica ciclotimer.csproj solo se necessaria
```

---

## 24. FASE 18 — Test finali

### TODO 002.24 — Eseguire test finali

Tutti i comandi devono essere eseguiti dalla root del repository.

Eseguire:

```bash
dotnet test tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj
```

Eseguire regressione core:

```bash
dotnet test tests/CicloTimer.Core.Tests/CicloTimer.Core.Tests.csproj
```

Eseguire regressione localization:

```bash
dotnet test tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Se possibile, eseguire:

```bash
dotnet test
```

Criteri:

1. test bridge: tutti superati;
2. test core: tutti ancora superati;
3. test localization: tutti ancora superati;
4. test solution completa: tutti superati;
5. nessun test fallito.

Criterio di completamento:

```text
tutti i test previsti superati
```

---

## 25. FASE 19 — Verifica working tree

### TODO 002.25 — Verificare file modificati

Cursor deve verificare che siano stati creati/modificati solo:

```text
view-models/CicloTimer.Bridge/
tests/CicloTimer.Bridge.Tests/
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

## 26. FASE 20 — Verifica artefatti build

### TODO 002.26 — Verificare assenza bin/obj da committare

Cursor deve verificare che nel working tree non risultino file da committare sotto:

```text
bin/
obj/
```

Comandi utili:

```bash
git status --porcelain
git status --porcelain | findstr /i "bin obj"
```

Se compaiono artefatti `bin/` o `obj/`, Cursor deve segnalarlo e non fare commit.

Cursor non deve fare commit in ogni caso.

Criterio di completamento:

```text
nessun artefatto bin/obj da committare
```

---

## 27. FASE 21 — Report finale

### TODO 002.27 — Produrre report finale

Cursor deve produrre un report finale contenente:

1. file creati;
2. file modificati;
3. progetti creati;
4. progetti aggiunti alla solution;
5. target framework usati;
6. riferimenti tra progetti;
7. conferma bridge → core;
8. conferma bridge → localization;
9. conferma test bridge → bridge;
10. conferma test bridge → core;
11. conferma test bridge → localization;
12. conferma che core non è stato modificato;
13. conferma che localization non è stato modificato;
14. conferma che UI non è stata modificata;
15. conferma presenza di `AccessibilityTextKey.SessionCompletedTemplate` o chiave equivalente;
16. conferma assenza WPF nel bridge;
17. conferma assenza API Windows;
18. conferma assenza audio reale;
19. conferma assenza timer reali;
20. conferma assenza stringhe hardcoded utente nel bridge;
21. conferma assenza stringhe libere come chiavi localization;
22. conferma verifica manuale assenza `ToString()` per chiavi localization;
23. conferma verifica manuale assenza reflection per chiavi localization;
24. conferma `TimerBridgeUpdate` sempre restituito;
25. conferma `SystemActions` sempre non null;
26. conferma lista vuota quando non ci sono azioni;
27. conferma `Tick(int elapsedSeconds)` inoltra il valore al core senza generare tick autonomi;
28. conferma test specifico su `CompletedSessionsText`;
29. conferma test bridge;
30. conferma test core;
31. conferma test localization;
32. conferma build solution;
33. eventuale modifica a `ciclotimer.csproj` e motivo;
34. comandi eseguiti;
35. numero test bridge;
36. eventuali test falliti;
37. eventuali deviazioni dal TODO;
38. conferma assenza artefatti `bin/obj` da committare.

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
[ ] Verificato template sessione completata in localization
[ ] Creato view-models/CicloTimer.Bridge/
[ ] Creato CicloTimer.Bridge.csproj
[ ] Target net9.0
[ ] Nessun net9.0-windows
[ ] Bridge referenzia Core
[ ] Bridge referenzia Localization
[ ] Bridge non referenzia WPF
[ ] Progetto bridge aggiunto alla solution
[ ] Creato TimerDisplayModel
[ ] Creato SystemActionRequest
[ ] Creato TimerBridgeUpdate
[ ] Creato TimerInput
[ ] Creato TimeFormatter
[ ] Creati mapper stato/errore/evento
[ ] Creato PrimaryActionResolver
[ ] Creato SystemActionResolver
[ ] Creato TimerBridge
[ ] Creato tests/CicloTimer.Bridge.Tests/
[ ] Creato progetto test bridge
[ ] Test bridge referenzia Bridge/Core/Localization
[ ] Test bridge non referenzia WPF
[ ] Progetto test bridge aggiunto alla solution
[ ] Creati TimeFormatterTests
[ ] Creati TimerBridgeMappersAndResolversTests
[ ] Creati TimerBridgeUpdateTests
[ ] Creati TimerBridgeCoreInteractionsTests
[ ] Creati ProjectDependencyTests
[ ] Test esplicito su CompletedSessionsText
[ ] dotnet build CicloTimer.sln superato
[ ] dotnet test bridge superato
[ ] dotnet test core superato
[ ] dotnet test localization superato
[ ] dotnet test generale superato se eseguito
[ ] Nessuna modifica a core
[ ] Nessuna modifica a localization
[ ] Nessuna modifica a UI
[ ] Nessun audio reale
[ ] Nessuna API Windows
[ ] Nessun timer reale
[ ] Nessuna stringa utente hardcoded nel bridge
[ ] Nessuna stringa libera come chiave localization
[ ] Nessun ToString/reflection per chiavi localization verificato nel report
[ ] Nessun bin/obj da committare
[ ] Report finale prodotto
```

---

## 29. Criteri di completamento globale

Il TODO 002 è completato solo se:

1. il progetto bridge esiste;
2. il progetto test bridge esiste;
3. entrambi sono nella solution;
4. il bridge usa `net9.0`;
5. il bridge referenzia core e localization;
6. il bridge non referenzia WPF;
7. il bridge produce sempre `TimerBridgeUpdate`;
8. `TimerBridgeUpdate.DisplayModel` è sempre presente;
9. `TimerBridgeUpdate.SystemActions` è sempre presente;
10. `SystemActions` è lista vuota quando non ci sono azioni;
11. il bridge usa localization per testi utente;
12. il bridge non contiene stringhe utente hardcoded;
13. il bridge non usa stringhe libere come chiavi;
14. il bridge non usa `ToString()` sugli enum core per chiavi;
15. il bridge non usa reflection per chiavi;
16. il bridge non implementa audio reale;
17. il bridge non chiama API Windows;
18. il bridge non genera tick autonomamente;
19. `Tick(int elapsedSeconds)` inoltra il valore al core senza aggregarlo o correggerlo autonomamente;
20. il bridge non modifica core;
21. il bridge non modifica localization;
22. il bridge non modifica UI;
23. esiste test esplicito su `CompletedSessionsText`;
24. build solution passa;
25. test bridge passano;
26. test core passano;
27. test localization passano;
28. non ci sono artefatti `bin/obj` da committare;
29. il report finale è completo.

---

## 30. Criteri di non validità

L'implementazione non è valida se:

1. il bridge viene creato fuori da `view-models/CicloTimer.Bridge/`;
2. viene creata cartella `src/`;
3. viene creato `models/CicloTimer.Bridge/`;
4. viene creato `locales/CicloTimer.Bridge/`;
5. il bridge usa `net9.0-windows`;
6. il bridge non referenzia core;
7. il bridge non referenzia localization;
8. il bridge referenzia WPF;
9. il bridge modifica core;
10. il bridge modifica localization;
11. il bridge modifica UI;
12. il bridge usa audio reale;
13. il bridge usa audio focus;
14. il bridge usa API Windows;
15. il bridge usa `System.Timers.Timer`;
16. il bridge usa `DispatcherTimer`;
17. il bridge genera tick autonomamente;
18. il bridge valida logicamente al posto del core;
19. il bridge incrementa `CompletedSessions`;
20. il bridge simula ripartenza automatica;
21. il bridge usa stringhe hardcoded utente;
22. il bridge usa stringhe libere come chiavi localization;
23. il bridge usa `ToString()` sugli enum core per generare chiavi;
24. il bridge usa reflection per generare chiavi;
25. `SystemActions` può essere `null`;
26. i comandi non restituiscono sempre `TimerBridgeUpdate`;
27. viene usato event bus, code persistenti o `Channel`;
28. vengono sintetizzati eventi provenienti da result separati;
29. viene prodotto `StopFinalAlertSound` senza evidenza di final alert attivo;
30. `ciclotimer.csproj` viene modificato prima di verificare errore reale di build;
31. non viene verificata la presenza del template sessione completata;
32. non viene chiarito nel report che `Tick` non genera tempo autonomamente;
33. manca test esplicito su `CompletedSessionsText`;
34. i test falliscono;
35. la build fallisce;
36. ci sono artefatti `bin/obj` da committare;
37. il documento finale risulta troncato o con comandi incompleti.

---

## 31. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. il TODO viene approvato come documento operativo per Cursor;
2. il bridge resta in `view-models/CicloTimer.Bridge/`;
3. i test restano in `tests/CicloTimer.Bridge.Tests/`;
4. il bridge usa `net9.0`;
5. il bridge dipende da core e localization;
6. il progetto test bridge può dipendere da bridge, core e localization;
7. `TimerCommandResult.Errors` viene trattato come lista;
8. in caso di più errori si usa il primo;
9. `SystemActionResolver` viene semplificato a eventi + `wasFinalAlertActive` + `isFinalAlertActive`;
10. `Tick(int elapsedSeconds)` inoltra al core il valore ricevuto;
11. il valore ordinario futuro sarà `elapsedSeconds = 1`;
12. il bridge non aggrega, corregge o simula tick;
13. il controllo anti `ToString()` e reflection può essere verifica manuale documentata nel report;
14. viene aggiunto un test esplicito per `CompletedSessionsText`;
15. la forma corretta dell'esclusione WPF, se necessaria, resta `<Compile Remove="view-models/**" />`;
16. il documento finale deve essere completo e non troncato.

---

## 32. Stato del documento

Questo documento è approvato come TODO 002 — Bridge UI-logica e modello mostrabile del timer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni consiglieri AI: TimerCommandResult.Errors trattato come lista, SystemActionResolver semplificato, Tick chiarito, verifica anti ToString/reflection spostata a report se non automatizzabile, test esplicito CompletedSessionsText, mantenuta esclusione view-models/**, documento finale completo
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. semplificazione di `TimerCommandResult.Errors`;
2. semplificazione di `SystemActionResolver`;
3. chiarimento di `Tick(int elapsedSeconds)`;
4. declassamento del controllo anti `.ToString()`, `GetType()`, `typeof(` e `System.Reflection` a verifica documentata nel report se non automatizzabile;
5. aggiunta del test esplicito su `CompletedSessionsText`;
6. conferma della forma corretta `<Compile Remove="view-models/**" />`;
7. chiusura completa del documento, incluse fasi finali, checklist, criteri e stato documento.

Il documento è approvato dal project owner come base operativa per l'implementazione del bridge UI-logica.
