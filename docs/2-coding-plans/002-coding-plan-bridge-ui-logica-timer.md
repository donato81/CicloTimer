# CicloTimer — Coding Plan 002 — Bridge UI-logica e modello mostrabile del timer

**Tipo documento:** coding plan  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md  

---

## 1. Scopo del documento

Questo documento traduce il Design 002 approvato in un piano operativo di codifica.

Il documento di riferimento principale è:

```text
docs/1-design/002-design-bridge-ui-logica-timer.md
````

Il Design 002 stabilisce che il bridge deve essere un progetto .NET separato, collocato in:

```text
view-models/CicloTimer.Bridge/
```

Il bridge deve collegare:

```text
CicloTimer.Core
CicloTimer.Localization
```

e produrre un aggiornamento mostrabile per la futura UI e per il futuro orchestratore.

Questo coding plan definisce:

1. quali cartelle creare;
2. quali file creare;
3. quale progetto .NET creare;
4. quale progetto test creare;
5. quali dipendenze impostare;
6. quali modelli implementare;
7. quali metodi concettuali trasformare in metodi C#;
8. quali mappature core → localization implementare;
9. quali azioni audio/sistema concettuali produrre;
10. quali test automatici creare;
11. quali aree non toccare;
12. quali verifiche finali eseguire.

Questo coding plan non cambia il Design 002.

Questo coding plan non introduce nuove funzionalità.

Questo coding plan non autorizza UI WPF, XAML, audio reale, audio focus, API Windows, NVDA operativo, UI Automation, Live Region, timer reale, orchestratore, `ICommand`, `INotifyPropertyChanged`, database, cloud o modifiche al core/localization.

---

## 2. Obiettivo operativo

L'obiettivo operativo è creare il progetto:

```text
CicloTimer.Bridge
```

nel percorso:

```text
view-models/CicloTimer.Bridge/
```

e il relativo progetto test:

```text
tests/CicloTimer.Bridge.Tests/
```

Il bridge deve:

1. ricevere comandi concettuali;
2. chiamare il core;
3. leggere il `TimerCommandResult` per eventi ed errori del comando corrente;
4. leggere il `TimerEngine` come fonte primaria dello stato corrente;
5. usare `CicloTimer.Localization` per tutti i testi;
6. produrre sempre `TimerBridgeUpdate`;
7. includere sempre un `TimerDisplayModel`;
8. includere sempre `SystemActions`, anche come lista vuota;
9. produrre richieste audio/sistema solo concettuali;
10. restare testabile senza UI, WPF, audio reale, API Windows o timer reali.

---

## 3. Perimetro autorizzato

Questo coding plan autorizza:

1. creazione di `view-models/CicloTimer.Bridge/`;
2. creazione di `view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj`;
3. aggiunta del progetto bridge a `CicloTimer.sln`;
4. riferimento del bridge a `models/CicloTimer.Core/CicloTimer.Core.csproj`;
5. riferimento del bridge a `locales/CicloTimer.Localization/CicloTimer.Localization.csproj`;
6. creazione dei file C# del bridge;
7. creazione di `tests/CicloTimer.Bridge.Tests/`;
8. creazione di `tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj`;
9. riferimento dei test bridge al progetto bridge;
10. riferimento dei test bridge a `CicloTimer.Core` e `CicloTimer.Localization` per costruire scenari controllati;
11. aggiunta del progetto test bridge a `CicloTimer.sln`;
12. eventuale modifica minima di `ciclotimer.csproj` solo se la build WPF root include ricorsivamente `view-models/**`;
13. eventuale aggiornamento minimo di `.gitignore` solo se emergono nuovi artefatti non ignorati;
14. esecuzione build e test.

---

## 4. Fuori perimetro

Questo coding plan non autorizza:

1. modifica di `models/CicloTimer.Core/`;
2. modifica di `locales/CicloTimer.Localization/`;
3. modifica della UI WPF;
4. modifica di `MainWindow.xaml`;
5. modifica di `MainWindow.xaml.cs`;
6. modifica di `App.xaml`;
7. modifica di `App.xaml.cs`;
8. implementazione audio reale;
9. implementazione audio focus;
10. interruzione, attenuazione o ripristino audio di altre applicazioni;
11. API Windows;
12. NVDA operativo;
13. UI Automation;
14. Live Region;
15. timer reale;
16. `DispatcherTimer`;
17. `System.Timers.Timer`;
18. thread UI;
19. orchestratore;
20. `ICommand`;
21. `INotifyPropertyChanged`;
22. database;
23. cloud;
24. persistenza;
25. file JSON/XML/RESX;
26. nuove lingue;
27. modifica del formato tempo `mm:ss`;
28. modifica dei testi localization;
29. modifica delle regole core;
30. creazione di cartelle `src/`.

Se durante l'implementazione emerge la necessità di modificare core, localization, UI, audio o architettura, Cursor deve fermarsi e segnalarlo.

---

## 5. Struttura fisica obbligatoria

La struttura da creare è:

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

La produzione resta suddivisa in file piccoli e con responsabilità chiare.

I test vengono invece accorpati in file logici più ampi per ridurre frammentazione e rendere più semplice il lavoro di Cursor.

Sono vietate collocazioni alternative:

```text
src/
models/CicloTimer.Bridge/
locales/CicloTimer.Bridge/
view-models/TimerBridge/
```

---

## 6. Progetto CicloTimer.Bridge

### 6.1 Percorso

```text
view-models/CicloTimer.Bridge/
```

File progetto:

```text
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
```

### 6.2 Target framework

Il progetto deve usare:

```text
net9.0
```

Il progetto non deve usare:

```text
net9.0-windows
```

### 6.3 Riferimenti ammessi

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

### 6.4 Contenuto indicativo del csproj

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

Il path concreto dovrà essere verificato da Cursor in base alla posizione reale del file `.csproj`.

---

## 7. Aggiornamento solution

Dopo la creazione del progetto bridge, aggiungere subito alla solution:

```bash
dotnet sln CicloTimer.sln add view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
```

Il comando deve essere eseguito dalla root del repository, cioè dalla cartella dove si trova:

```text
CicloTimer.sln
```

Dopo la creazione del progetto test, aggiungere subito alla solution:

```bash
dotnet sln CicloTimer.sln add tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj
```

---

## 8. Verifica preventiva di CicloTimer.Localization

Prima di implementare il bridge, Cursor deve verificare che `CicloTimer.Localization` esponga già:

```text
AccessibilityTextKey.SessionCompletedTemplate
```

oppure una chiave equivalente già definita e testata per ottenere il testo:

```text
Sessione completata. Sessioni completate: {0}.
```

Se questa chiave non esiste, Cursor deve fermarsi e segnalarlo.

Cursor non deve modificare `CicloTimer.Localization` per aggiungere nuove chiavi o nuovi testi.

Motivo:

```text
il bridge deve usare localization, non estenderla senza un design dedicato
```

---

## 9. Possibile esclusione in ciclotimer.csproj

Il progetto WPF root può includere ricorsivamente file `.cs` sotto la root del repository.

In passato è stato necessario escludere:

```xml
<Compile Remove="models/**" />
<Compile Remove="tests/**" />
<Compile Remove="locales/**" />
```

Per il bridge, Cursor non deve modificare preventivamente `ciclotimer.csproj`.

Ordine obbligatorio:

1. creare il progetto bridge;
2. creare il progetto test bridge;
3. aggiungere entrambi alla solution;
4. eseguire `dotnet build CicloTimer.sln` dalla root;
5. solo se la build fallisce perché il progetto WPF include file sotto `view-models/**`, allora modificare `ciclotimer.csproj`.

In quel solo caso Cursor può aggiungere:

```xml
<Compile Remove="view-models/**" />
```

Questa modifica è autorizzata solo se necessaria per impedire al progetto WPF root di compilare il progetto bridge o i suoi artefatti.

Cursor deve documentare chiaramente questa eventuale modifica nel report finale.

---

## 10. TimerDisplayModel

### 10.1 Percorso

```text
view-models/CicloTimer.Bridge/TimerDisplayModel.cs
```

### 10.2 Responsabilità

Rappresentare il modello già pronto per la futura UI.

Classe o record consigliato:

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

La forma concreta può variare, ma i campi concettuali devono essere presenti.

Regole:

1. nessun campo testuale deve essere `null`;
2. in assenza di errore, `ErrorMessageText = string.Empty`;
3. in assenza di evento, `EventMessageText = string.Empty`;
4. in assenza di evento, `AccessibleEventText = string.Empty`;
5. `AccessibleStatusText` deve essere sempre valorizzato;
6. il modello non deve contenere tipi WPF;
7. il modello non deve contenere audio;
8. il modello non deve contenere comandi UI.

---

## 11. SystemActionRequest

### 11.1 Percorso

```text
view-models/CicloTimer.Bridge/SystemActionRequest.cs
```

### 11.2 Responsabilità

Rappresentare richieste concettuali verso il futuro livello audio/sistema operativo.

Forma consigliata:

```csharp
public enum SystemActionRequest
{
    StartFinalAlertSound,
    StopFinalAlertSound
}
```

Non creare `None` se `TimerBridgeUpdate.SystemActions` usa lista vuota per indicare assenza di azioni.

Regole:

1. non implementare audio reale;
2. non chiamare API Windows;
3. non gestire volume;
4. non gestire audio focus;
5. non creare eventi C#;
6. non creare code persistenti;
7. non usare `Channel`.

---

## 12. TimerBridgeUpdate

### 12.1 Percorso

```text
view-models/CicloTimer.Bridge/TimerBridgeUpdate.cs
```

### 12.2 Responsabilità

Rappresentare l'output sempre restituito dal bridge.

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
4. non usare `null` per assenza di azioni;
5. ogni metodo pubblico del bridge deve restituire `TimerBridgeUpdate`.

---

## 13. TimerInput

### 13.1 Percorso

```text
view-models/CicloTimer.Bridge/TimerInput.cs
```

### 13.2 Responsabilità

Rappresentare i valori provenienti dalla futura UI per la configurazione.

Forma consigliata:

```csharp
public sealed record TimerInput(
    int SessionMinutes,
    int SessionSeconds,
    int FinalAlertMinutes,
    int FinalAlertSeconds);
```

Regole:

1. il bridge converte minuti/secondi in secondi totali;
2. il bridge non valida logicamente la configurazione al posto del core;
3. il bridge non deve decidere se sessione zero sia logicamente valida;
4. la validazione logica resta nel core.

---

## 14. TimeFormatter

### 14.1 Percorso

```text
view-models/CicloTimer.Bridge/TimeFormatter.cs
```

### 14.2 Responsabilità

Formattare secondi interi nel formato:

```text
mm:ss
```

Firma consigliata:

```csharp
public static string Format(int seconds)
```

Esempi obbligatori:

```text
0      → 00:00
5      → 00:05
59     → 00:59
60     → 01:00
299    → 04:59
300    → 05:00
3600   → 60:00
3661   → 61:01
```

Regole:

1. usare `int` come tipo del parametro;
2. non introdurre `hh:mm:ss`;
3. non usare localization per il formato `mm:ss`;
4. non generare annunci;
5. non usare UI;
6. se il valore è negativo, normalizzare a `00:00`.

La scelta sul negativo deve essere coperta da test.

---

## 15. Mapper stati

### 15.1 Percorso

```text
view-models/CicloTimer.Bridge/TimerStateTextMapper.cs
```

### 15.2 Responsabilità

Mappare `TimerState` del core verso `TimerTextKey`.

Mappatura obbligatoria:

```text
Stopped     → TimerTextKey.StateStopped
Running     → TimerTextKey.StateRunning
FinalAlert  → TimerTextKey.StateFinalAlert
Paused      → TimerTextKey.StatePaused
```

Il mapper deve poi ottenere il testo tramite `LocalizationService` o servizio equivalente già implementato.

Regole:

1. niente stringhe hardcoded;
2. niente stringhe libere come chiavi;
3. niente `TimerState.ToString()` per ottenere la chiave;
4. niente reflection;
5. ogni stato deve essere gestito;
6. stato non gestito deve fallire in modo controllato.

---

## 16. Mapper errori

### 16.1 Percorso

```text
view-models/CicloTimer.Bridge/TimerErrorTextMapper.cs
```

### 16.2 Responsabilità

Mappare `TimerError` del core verso `ErrorTextKey`.

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

Regola per più errori:

```text
usare il primo errore della lista secondo l'ordine restituito dal core
```

Se non ci sono errori:

```text
ErrorMessageText = string.Empty
```

Cursor deve verificare la forma reale di `TimerCommandResult.Errors`.

Se il core espone una collezione di errori, usare il primo elemento.

Se il core espone un singolo errore o una forma diversa, Cursor deve adattare il bridge senza modificare il core e deve documentare l'adattamento nel report finale.

Regole:

1. niente stringhe hardcoded;
2. niente stringhe libere come chiavi;
3. niente `TimerError.ToString()` per ottenere la chiave;
4. niente reflection;
5. ogni errore deve essere gestito.

---

## 17. Mapper eventi

### 17.1 Percorso

```text
view-models/CicloTimer.Bridge/TimerEventTextMapper.cs
```

### 17.2 Responsabilità

Mappare `TimerEvent` del core verso `TimerTextKey` o template localization.

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

Regole eventi:

1. leggere solo eventi del `TimerCommandResult` corrente;
2. non accumulare eventi storici;
3. non creare code;
4. non duplicare eventi;
5. rispettare l'ordine del core;
6. se non ci sono eventi, restituire `string.Empty`.

### 17.3 Sintesi sessione completata

Se nello stesso `TimerCommandResult` sono presenti:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

il bridge deve produrre il messaggio sintetico usando:

```text
AccessibilityTextKey.SessionCompletedTemplate
```

o chiave equivalente già presente in localization.

Esempio:

```text
CompletedSessions = 3
↓
Sessione completata. Sessioni completate: 3.
```

La sintesi è ammessa solo se gli eventi collegati sono presenti nello stesso result.

Se gli eventi arrivano in result separati, non sintetizzare artificialmente.

Se il template non esiste in localization, Cursor deve fermarsi e segnalarlo.

---

## 18. PrimaryActionResolver

### 18.1 Percorso

```text
view-models/CicloTimer.Bridge/PrimaryActionResolver.cs
```

### 18.2 Responsabilità

Determinare il testo dell'azione principale usando stato e disponibilità comando dal core.

Mappatura obbligatoria:

```text
Stopped + CanStart = true → CommandTextKey.Start
Running + CanPause = true → CommandTextKey.Pause
FinalAlert + CanPause = true → CommandTextKey.Pause
Paused + CanResume = true → CommandTextKey.Resume
```

Se nessuna azione principale è disponibile:

```text
PrimaryActionText = string.Empty
```

Regole:

1. il bridge non decide disponibilità logica dei comandi;
2. usa solo `CanStart`, `CanPause`, `CanResume`, `CanReset` esposti dal core;
3. il testo finale arriva da localization.

---

## 19. SystemActionResolver

### 19.1 Percorso

```text
view-models/CicloTimer.Bridge/SystemActionResolver.cs
```

### 19.2 Responsabilità

Determinare le richieste concettuali audio/sistema in base al comando corrente, agli eventi del result e allo stato final alert.

Azioni possibili:

```text
StartFinalAlertSound
StopFinalAlertSound
```

Il resolver non deve leggere direttamente dal core.

Il `TimerBridge` deve raccogliere le informazioni necessarie prima e dopo il comando e passarle al resolver.

Parametri concettuali consigliati:

```csharp
IReadOnlyList<TimerEvent> events
bool wasFinalAlertActive
bool isFinalAlertActive
TimerState previousState
TimerState currentState
```

La firma concreta può variare, ma queste informazioni devono essere disponibili alla logica del resolver.

### 19.3 StartFinalAlertSound

Produrre `StartFinalAlertSound` quando il result corrente contiene:

```text
FinalAlertStarted
```

### 19.4 StopFinalAlertSound

Produrre `StopFinalAlertSound` solo con evidenza di avviso finale attivo o sessione in chiusura.

Evidenze ammesse:

1. `wasFinalAlertActive` era `true` prima del comando;
2. `previousState` era `FinalAlert`;
3. result corrente contiene eventi di fine sessione;
4. comando corrente è pausa o reset mentre l'avviso finale era attivo.

Produrre `StopFinalAlertSound` in particolare per:

```text
SessionCompleted / SessionCounterIncremented / NextSessionStarted
TimerPaused durante final alert
TimerReset durante final alert
```

Non produrre `StopFinalAlertSound` quando non c'è evidenza di avviso finale attivo.

Regole:

1. niente audio reale;
2. niente API Windows;
3. niente volume;
4. niente audio focus;
5. solo lista di richieste concettuali.

---

## 20. TimerBridge

### 20.1 Percorso

```text
view-models/CicloTimer.Bridge/TimerBridge.cs
```

### 20.2 Responsabilità

Coordinare core, localization, mapper e produzione di `TimerBridgeUpdate`.

Il bridge deve possedere o ricevere un'istanza di `TimerEngine`.

Forma consigliata semplice:

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

### 20.3 Regola TimerCommandResult / TimerEngine

Dopo ogni comando:

1. salvare lo stato precedente necessario, per esempio `wasFinalAlertActive` e `previousState`;
2. chiamare il core;
3. usare `TimerCommandResult` per eventi ed errori;
4. leggere `TimerEngine` per stato corrente;
5. costruire `TimerDisplayModel`;
6. calcolare `SystemActions`;
7. restituire `TimerBridgeUpdate`.

### 20.4 Configure

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

### 20.5 Tick

`Tick` deve:

1. ricevere `int elapsedSeconds`;
2. inoltrarlo al core;
3. non generare il tempo autonomamente;
4. restituire `TimerBridgeUpdate`;
5. produrre azioni concettuali se il core genera eventi rilevanti.

`Tick` non è comando utente.

---

## 21. CompletedSessionsText

Il bridge deve formattare il contatore sessioni usando testi centralizzati.

Scelta consigliata per V1:

```text
CompletedSessionsText = "<label localization>: <CompletedSessions>"
```

La label deve arrivare da:

```text
UiTextKey.CompletedSessions
```

Esempio:

```text
Sessioni completate: 3
```

Non modificare localization per aggiungere nuovi testi senza design dedicato.

---

## 22. AccessibleStatusText

Il bridge deve produrre sempre `AccessibleStatusText`.

Usare il template localization:

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

Il bridge non deve annunciare automaticamente questo testo ogni secondo.

Il bridge lo prepara soltanto.

---

## 23. AccessibleEventText

Il bridge deve produrre sempre il campo `AccessibleEventText`.

Regole:

1. se c'è un evento rilevante, usare lo stesso testo di `EventMessageText` oppure template accessibile coerente;
2. se non ci sono eventi, usare `string.Empty`;
3. non usare NVDA;
4. non usare UI Automation;
5. non creare Live Region.

---

## 24. Progetto test bridge

### 24.1 Percorso

```text
tests/CicloTimer.Bridge.Tests/
```

File progetto:

```text
tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj
```

Target:

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

La dipendenza diretta da core e localization nei test è autorizzata perché serve a costruire scenari controllati e verificare il bridge contro tipi reali già approvati.

Il progetto test non deve referenziare:

```text
ciclotimer.csproj
WPF
WindowsBase
PresentationFramework
System.Windows
UIAutomation
```

---

## 25. Test TimeFormatter

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

---

## 26. Test mapper e resolver

Creare:

```text
tests/CicloTimer.Bridge.Tests/TimerBridgeMappersAndResolversTests.cs
```

Questo file deve coprire stati, errori, eventi, primary action e system action.

### 26.1 Test mappatura stati

Test obbligatori:

```text
Stopped → Timer fermo
Running → Sessione in corso
FinalAlert → Avviso finale in corso
Paused → Timer in pausa
```

Verificare che il testo finale arrivi da localization attraverso chiavi tipizzate, non da stringhe hardcoded nel bridge.

### 26.2 Test mappatura errori

Test obbligatori:

1. ogni `TimerError` viene mappato a testo utente corretto;
2. in presenza di più errori viene usato il primo;
3. assenza di errori produce `string.Empty`.

### 26.3 Test mappatura eventi

Test obbligatori:

1. ogni `TimerEvent` viene mappato a testo evento corretto;
2. eventi vuoti producono `string.Empty`;
3. eventi singoli producono il messaggio corrispondente;
4. `[SessionCompleted, SessionCounterIncremented, NextSessionStarted]` nello stesso result produce messaggio sintetico;
5. eventi collegati in result separati non devono essere sintetizzati artificialmente.

### 26.4 Test PrimaryActionResolver

Test obbligatori:

```text
Stopped + CanStart true → Avvia
Running + CanPause true → Pausa
FinalAlert + CanPause true → Pausa
Paused + CanResume true → Riprendi
nessuna azione disponibile → string.Empty
```

### 26.5 Test SystemActionResolver

Test obbligatori:

1. `FinalAlertStarted` produce `StartFinalAlertSound`;
2. fine sessione produce `StopFinalAlertSound`;
3. pausa durante final alert produce `StopFinalAlertSound`;
4. reset durante final alert produce `StopFinalAlertSound`;
5. pausa fuori final alert non produce `StopFinalAlertSound`;
6. reset fuori final alert non produce `StopFinalAlertSound`;
7. nessun evento rilevante produce lista vuota;
8. `SystemActions` non è mai null.

---

## 27. Test TimerBridgeUpdate

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

---

## 28. Test interazioni core bridge

Creare:

```text
tests/CicloTimer.Bridge.Tests/TimerBridgeCoreInteractionsTests.cs
```

Questo file deve coprire configurazione, comandi e tick.

### 28.1 Test configurazione

Test obbligatori:

1. `5 min 0 sec` diventa 300 secondi;
2. `0 min 20 sec` avviso finale diventa 20 secondi;
3. configurazione valida produce `RemainingTimeText = 05:00`;
4. configurazione non valida passa dal core e produce errore localization;
5. il bridge non blocca la validazione logica prima del core.

### 28.2 Test comandi

Test obbligatori:

1. `Start()` produce stato mostrabile running;
2. `Pause()` produce stato mostrabile paused;
3. `Resume()` produce stato mostrabile running/final alert secondo core;
4. `Reset()` produce stato stopped;
5. reset non azzera `CompletedSessions`.

### 28.3 Test tick

Test obbligatori:

1. tick riduce stato tramite core, non tramite bridge autonomo;
2. ingresso final alert produce `StartFinalAlertSound`;
3. sessione completata produce messaggio sintetico e `StopFinalAlertSound`;
4. tick senza eventi rilevanti produce `SystemActions` vuota;
5. tick non genera annunci automatici.

---

## 29. ProjectDependencyTests

Creare:

```text
tests/CicloTimer.Bridge.Tests/ProjectDependencyTests.cs
```

Verifiche obbligatorie o equivalenti nel report:

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
14. non contiene `System.Reflection`;
15. non contiene `GetType()` usato per generare chiavi localization;
16. non contiene `typeof(` usato per generare chiavi localization;
17. non contiene `.ToString()` usato per generare chiavi localization;
18. non contiene cartella `src`.

I controlli possono essere test automatizzati o verifica documentata nel report finale.

Se il controllo automatico su `.ToString()`, `GetType()` o `typeof(` risulta fragile, Cursor deve documentare una verifica manuale nel report finale.

---

## 30. Divieto stringhe hardcoded nel bridge

Il bridge non deve contenere stringhe utente finali hardcoded.

Ammesse stringhe tecniche non utente se strettamente necessarie, per esempio nomi di test o messaggi di eccezione tecnica.

Non sono ammesse nel bridge stringhe come:

```text
Sessione in corso
Timer fermo
Avvia
Pausa
Riprendi
Reset
La durata della sessione deve essere maggiore di zero.
Sessione completata. Sessioni completate: {0}.
```

Queste devono arrivare da `CicloTimer.Localization`.

I test possono contenere stringhe attese per verificare il risultato.

---

## 31. Ordine operativo di implementazione

L'implementazione deve seguire questo ordine:

```text
1. Ricognizione repository.
2. Verifica presenza Core e Localization.
3. Verifica presenza AccessibilityTextKey.SessionCompletedTemplate o chiave equivalente.
4. Creazione progetto bridge.
5. Aggiunta bridge alla solution.
6. Creazione modelli TimerDisplayModel, TimerBridgeUpdate, SystemActionRequest, TimerInput.
7. Creazione TimeFormatter.
8. Creazione mapper stato/errore/evento.
9. Creazione PrimaryActionResolver.
10. Creazione SystemActionResolver.
11. Creazione TimerBridge.
12. Creazione progetto test bridge.
13. Aggiunta test bridge alla solution.
14. Scrittura test formatter.
15. Scrittura test mapper e resolver.
16. Scrittura test TimerBridgeUpdate.
17. Scrittura test interazioni core/bridge.
18. Scrittura ProjectDependencyTests.
19. Esecuzione build solution.
20. Solo se la build fallisce per inclusione view-models nel progetto WPF, correzione ciclotimer.csproj.
21. Riesecuzione build.
22. Esecuzione test bridge.
23. Esecuzione test core.
24. Esecuzione test localization.
25. Esecuzione test solution completa.
26. Report finale.
```

---

## 32. Comandi di verifica finali

Tutti i comandi devono essere eseguiti dalla root del repository.

Eseguire:

```bash
dotnet build CicloTimer.sln
```

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

1. build solution: 0 errori;
2. test bridge: tutti superati;
3. test core: tutti ancora superati;
4. test localization: tutti ancora superati;
5. test solution completa: tutti superati.

---

## 33. File da non modificare

Cursor non deve modificare:

```text
models/CicloTimer.Core/
locales/CicloTimer.Localization/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
```

Cursor non deve modificare `ciclotimer.csproj`, salvo il caso specifico autorizzato:

```text
aggiunta di <Compile Remove="view-models/**" />
```

se necessaria per impedire al progetto WPF root di compilare il bridge.

Cursor può modificare:

```text
CicloTimer.sln
```

solo per aggiungere i nuovi progetti.

Cursor può modificare `.gitignore` solo se necessario per artefatti nuovi non ignorati.

---

## 34. Criteri di completamento

Il coding plan è completato quando:

1. esiste `view-models/CicloTimer.Bridge/`;
2. esiste `view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj`;
3. il progetto bridge usa `net9.0`;
4. il progetto bridge non usa `net9.0-windows`;
5. il progetto bridge referenzia core;
6. il progetto bridge referenzia localization;
7. il progetto bridge non referenzia WPF;
8. è stata verificata la presenza di `AccessibilityTextKey.SessionCompletedTemplate` o chiave equivalente;
9. esiste `TimerDisplayModel`;
10. esiste `TimerBridgeUpdate`;
11. esiste `SystemActionRequest`;
12. esiste `TimerInput`;
13. esiste `TimeFormatter`;
14. esistono mapper stato/errore/evento;
15. esiste `PrimaryActionResolver`;
16. esiste `SystemActionResolver`;
17. esiste `TimerBridge`;
18. ogni comando bridge restituisce `TimerBridgeUpdate`;
19. `SystemActions` è sempre non null;
20. senza azioni, `SystemActions` è lista vuota;
21. testi utente arrivano da localization;
22. non ci sono stringhe utente hardcoded nel bridge;
23. non ci sono stringhe libere come chiavi localization;
24. non viene usato `ToString()` sugli enum core per generare chiavi;
25. non viene usata reflection per generare chiavi;
26. non c'è audio reale;
27. non c'è audio focus;
28. non ci sono API Windows;
29. non ci sono timer reali;
30. non ci sono dipendenze WPF;
31. esiste `tests/CicloTimer.Bridge.Tests/`;
32. i test bridge passano;
33. i test core passano;
34. i test localization passano;
35. la solution compila;
36. non sono stati modificati core, localization o UI.

---

## 35. Criteri di non validità

L'implementazione non è valida se:

1. crea il bridge fuori da `view-models/CicloTimer.Bridge/`;
2. crea `src/`;
3. crea `models/CicloTimer.Bridge/`;
4. crea `locales/CicloTimer.Bridge/`;
5. usa `net9.0-windows`;
6. non referenzia core;
7. non referenzia localization;
8. modifica core;
9. modifica localization;
10. modifica UI;
11. usa WPF;
12. usa XAML;
13. usa `MainWindow`;
14. usa `Application`;
15. usa `Dispatcher`;
16. usa audio reale;
17. usa audio focus;
18. usa API Windows;
19. usa `System.Timers.Timer`;
20. usa `DispatcherTimer`;
21. genera tick autonomamente;
22. valida logicamente al posto del core;
23. incrementa `CompletedSessions`;
24. simula ripartenza automatica;
25. usa stringhe hardcoded utente;
26. usa stringhe libere come chiavi localization;
27. usa `ToString()` sugli enum core per generare chiavi;
28. usa reflection per generare chiavi;
29. restituisce `null` per `SystemActions`;
30. non restituisce sempre `TimerBridgeUpdate`;
31. usa event bus, code persistenti o `Channel`;
32. sintetizza eventi provenienti da result separati;
33. produce `StopFinalAlertSound` senza evidenza di final alert attivo;
34. modifica `ciclotimer.csproj` prima di verificare un errore reale di build;
35. non verifica la presenza del template sessione completata in localization;
36. i test falliscono;
37. la build fallisce.

---

## 36. Report finale richiesto a Cursor

Al termine dell'implementazione, Cursor dovrà produrre un report con:

1. file creati;
2. file modificati;
3. progetti creati;
4. progetti aggiunti alla solution;
5. target framework usati;
6. riferimenti tra progetti;
7. conferma bridge → core;
8. conferma bridge → localization;
9. conferma test bridge → core;
10. conferma test bridge → localization;
11. conferma che core non è stato modificato;
12. conferma che localization non è stato modificato;
13. conferma che UI non è stata modificata;
14. conferma presenza di `AccessibilityTextKey.SessionCompletedTemplate` o chiave equivalente;
15. conferma assenza WPF nel bridge;
16. conferma assenza API Windows;
17. conferma assenza audio reale;
18. conferma assenza timer reali;
19. conferma assenza stringhe hardcoded utente nel bridge;
20. conferma assenza stringhe libere come chiavi localization;
21. conferma assenza `ToString()` per chiavi localization;
22. conferma assenza reflection per chiavi localization;
23. conferma `TimerBridgeUpdate` sempre restituito;
24. conferma `SystemActions` sempre non null;
25. conferma lista vuota quando non ci sono azioni;
26. conferma test bridge;
27. conferma test core;
28. conferma test localization;
29. conferma build solution;
30. eventuale modifica a `ciclotimer.csproj` e motivo;
31. comandi eseguiti;
32. numero test bridge;
33. eventuali test falliti;
34. eventuali deviazioni dal coding plan;
35. conferma che il documento non è stato troncato o lasciato con comandi incompleti.

Cursor non deve fare commit.

Cursor non deve fare push.

---

## 37. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. il bridge viene creato in `view-models/CicloTimer.Bridge/`;
2. il bridge usa target `net9.0`;
3. il bridge dipende da core;
4. il bridge dipende da localization;
5. il progetto test bridge può dipendere da bridge, core e localization;
6. il progetto test bridge non può dipendere da WPF;
7. i file di produzione restano separati per responsabilità;
8. i file di test vengono accorpati in gruppi logici;
9. `TimeFormatter` usa `int seconds`;
10. `Tick` usa `int elapsedSeconds`;
11. il template sessione completata deve esistere già in localization;
12. se il template non esiste, Cursor si ferma;
13. `SystemActionResolver` riceve informazioni esplicite su final alert, stato precedente, stato corrente ed eventi;
14. `ciclotimer.csproj` può essere modificato solo dopo errore reale di build;
15. il controllo anti `ToString()` e reflection può essere automatico o documentato nel report finale;
16. non si introduce Roslyn come requisito obbligatorio;
17. non si introduce audio reale;
18. non si introduce API Windows;
19. non si introduce timer reale;
20. non si modifica core;
21. non si modifica localization;
22. non si modifica UI.

---

## 38. Stato del documento

Questo documento è approvato come Coding Plan 002 — Bridge UI-logica e modello mostrabile del timer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni consiglieri AI: verifica template sessione completata, test bridge autorizzati a referenziare core/localization, test accorpati, TimeFormatter int, SystemActionResolver con stato final alert esplicito, modifica ciclotimer.csproj solo dopo errore reale di build, verifica anti ToString/reflection nel report o nei test
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. verifica preventiva di `AccessibilityTextKey.SessionCompletedTemplate`;
2. blocco dell'implementazione se il template non esiste;
3. chiarimento dei parametri concettuali di `SystemActionResolver`;
4. autorizzazione esplicita dei test bridge a referenziare core e localization;
5. riduzione della frammentazione dei file di test;
6. uso esplicito di `int` in `TimeFormatter`;
7. uso esplicito di `int elapsedSeconds` in `Tick`;
8. chiarimento sulla forma reale di `TimerCommandResult.Errors`;
9. modifica a `ciclotimer.csproj` solo dopo errore reale di build;
10. verifica anti `ToString()` e reflection affidata a test o report finale;
11. esclusione di Roslyn come requisito obbligatorio;
12. conferma dei vincoli anti-WPF, anti-audio reale, anti-API Windows e anti-timer reale.

Il documento è approvato dal project owner come base per il successivo TODO 002.
