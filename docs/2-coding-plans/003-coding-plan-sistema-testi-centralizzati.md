# CicloTimer — Coding Plan 003 — Sistema centralizzato testi e messaggi applicativi

**Tipo documento:** coding plan  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-02  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/003-design-sistema-testi-centralizzati.md  

---

## 1. Scopo del documento

Questo documento traduce il Design 003 in un piano operativo di codifica.

Il documento di riferimento principale è:

```text
docs/1-design/003-design-sistema-testi-centralizzati.md
````

Il Design 003 ha stabilito che il sistema centralizzato testi deve essere realizzato come progetto .NET separato, collocato in:

```text
locales/CicloTimer.Localization/
```

Il file progetto deve essere:

```text
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

Questo coding plan definisce:

1. quali cartelle creare;
2. quali file creare;
3. quale progetto .NET creare;
4. quale progetto test creare;
5. quali testi minimi inserire;
6. quali chiavi tipizzate esporre;
7. come gestire la lingua italiana iniziale;
8. come predisporre la struttura per lingue future;
9. come gestire il fallback italiano;
10. quali test automatici creare;
11. quali test di completezza enum rendere obbligatori;
12. quali aree non toccare;
13. quali verifiche finali eseguire.

Questo coding plan non cambia il Design 003.

Questo coding plan non introduce nuove funzionalità.

Questo coding plan non autorizza implementazione bridge, UI, audio, NVDA, UI Automation, API Windows, JSON, RESX, XML, database o selettore lingua.

---

## 2. Obiettivo operativo

L'obiettivo operativo è creare il progetto:

```text
CicloTimer.Localization
```

e il relativo progetto test:

```text
CicloTimer.Localization.Tests
```

Il sistema deve fornire testi centralizzati per:

1. stati timer;
2. eventi timer;
3. comandi utente;
4. errori utente;
5. etichette UI;
6. testi accessibili;
7. template con parametri.

La prima lingua implementata deve essere solo:

```text
it
```

La struttura deve però essere predisposta per future lingue tramite:

```text
Locales/It/
```

In futuro potranno essere aggiunte altre cartelle, per esempio:

```text
Locales/En/
```

ma questo coding plan non autorizza l'implementazione di altre lingue.

---

## 3. Perimetro autorizzato

Questo coding plan autorizza:

1. creazione di `locales/CicloTimer.Localization/`;
2. creazione di `locales/CicloTimer.Localization/CicloTimer.Localization.csproj`;
3. creazione di `SupportedLanguage.cs`;
4. creazione di `LocalizationKeys.cs`;
5. creazione di `LocalizationService.cs`;
6. creazione della cartella `locales/CicloTimer.Localization/Locales/It/`;
7. creazione di `ItalianTimerTexts.cs`;
8. creazione di `ItalianCommandTexts.cs`;
9. creazione di `ItalianErrorTexts.cs`;
10. creazione di `ItalianAccessibilityTexts.cs`;
11. creazione di `ItalianUiTexts.cs`;
12. creazione di `tests/CicloTimer.Localization.Tests/`;
13. creazione di `tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj`;
14. creazione dei test automatici del sistema testi;
15. creazione dei test di completezza enum;
16. aggiunta dei due progetti alla solution `CicloTimer.sln`;
17. eventuale aggiornamento minimo di `.gitignore` solo se emergono nuovi artefatti non ignorati.

---

## 4. Fuori perimetro

Questo coding plan non autorizza:

1. modifica del core;
2. modifica del bridge;
3. implementazione del bridge;
4. modifica della UI;
5. modifica di `MainWindow.xaml`;
6. modifica di `MainWindow.xaml.cs`;
7. modifica di `App.xaml`;
8. modifica di `App.xaml.cs`;
9. aggiunta di stringhe in XAML;
10. audio reale;
11. NVDA operativo;
12. UI Automation;
13. Live Region;
14. API Windows;
15. timer reale;
16. orchestratore;
17. JSON;
18. XML;
19. RESX;
20. database;
21. cloud;
22. selettore lingua;
23. salvataggio preferenze lingua;
24. rilevamento lingua Windows;
25. `CultureInfo`;
26. lingue diverse dall'italiano;
27. `FrozenDictionary` come requisito;
28. reflection;
29. `ToString()` sugli enum del core per generare chiavi;
30. stringhe magiche come chiavi.

---

## 5. Struttura fisica obbligatoria

La struttura da creare è:

```text
locales/
  CicloTimer.Localization/
    CicloTimer.Localization.csproj
    SupportedLanguage.cs
    LocalizationKeys.cs
    LocalizationService.cs
    Locales/
      It/
        ItalianTimerTexts.cs
        ItalianCommandTexts.cs
        ItalianErrorTexts.cs
        ItalianAccessibilityTexts.cs
        ItalianUiTexts.cs

tests/
  CicloTimer.Localization.Tests/
    CicloTimer.Localization.Tests.csproj
    SupportedLanguageTests.cs
    LocalizationKeysTests.cs
    LocalizationServiceTests.cs
    ItalianTimerTextsTests.cs
    ItalianCommandTextsTests.cs
    ItalianErrorTextsTests.cs
    ItalianAccessibilityTextsTests.cs
    ItalianUiTextsTests.cs
    LocalizationCompletenessTests.cs
    ProjectDependencyTests.cs
```

Sono vietate collocazioni alternative come:

```text
src/
resources/
strings/
models/CicloTimer.Localization/
view-models/CicloTimer.Localization/
```

---

## 6. Progetto CicloTimer.Localization

### 6.1 Percorso

Il progetto deve stare in:

```text
locales/CicloTimer.Localization/
```

Il file progetto deve essere:

```text
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
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

### 6.3 Contenuto concettuale del csproj

Esempio indicativo:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>CicloTimer.Localization</RootNamespace>
  </PropertyGroup>
</Project>
```

Il progetto non deve referenziare:

```text
CicloTimer.Core
CicloTimer.Bridge
ciclotimer WPF
WPF
WindowsBase
PresentationFramework
Windows API
```

---

## 7. Progetto CicloTimer.Localization.Tests

### 7.1 Percorso

Il progetto test deve stare in:

```text
tests/CicloTimer.Localization.Tests/
```

Il file progetto deve essere:

```text
tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

### 7.2 Target framework

Il progetto test deve usare:

```text
net9.0
```

### 7.3 Framework di test

Il framework consigliato è:

```text
xUnit
```

### 7.4 Riferimenti

Il progetto test deve referenziare solo:

```text
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

Il progetto test non deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
ciclotimer.csproj
```

---

## 8. Aggiornamento della solution

Cursor deve aggiungere alla solution:

```text
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Comandi indicativi:

```bash
dotnet sln CicloTimer.sln add locales/CicloTimer.Localization/CicloTimer.Localization.csproj
dotnet sln CicloTimer.sln add tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

La solution finale deve contenere almeno:

```text
ciclotimer
CicloTimer.Core
CicloTimer.Core.Tests
CicloTimer.Localization
CicloTimer.Localization.Tests
```

Se il progetto bridge esiste già o verrà aggiunto dopo, non deve essere modificato da questo coding plan.

---

## 9. SupportedLanguage

### 9.1 Percorso

```text
locales/CicloTimer.Localization/SupportedLanguage.cs
```

### 9.2 Responsabilità

Rappresentare le lingue supportate dal sistema testi.

Per la prima versione deve esistere solo:

```text
It
```

Esempio concettuale:

```csharp
public enum SupportedLanguage
{
    It
}
```

Non aggiungere `En`, `Fr`, `Es` nella prima versione.

La struttura delle cartelle lascia aperta questa possibilità, ma il codice non deve implementare lingue non richieste.

---

## 10. LocalizationKeys

### 10.1 Percorso

```text
locales/CicloTimer.Localization/LocalizationKeys.cs
```

### 10.2 Responsabilità

Esporre chiavi controllate e tipizzate per richiedere testi.

Il codice applicativo non deve richiedere testi usando stringhe libere.

Sono vietati esempi come:

```text
GetText("Running")
GetText("InvalidSessionDuration")
GetText("Start")
```

La struttura delle chiavi deve coprire almeno:

```text
Timer
Commands
Errors
Accessibility
Ui
```

### 10.3 Forma consigliata

Soluzione consigliata: enum separati per categoria.

Esempio concettuale:

```csharp
public enum TimerTextKey
{
    StateStopped,
    StateRunning,
    StateFinalAlert,
    StatePaused,
    EventTimerConfigured,
    EventTimerStarted,
    EventTimerPaused,
    EventTimerResumed,
    EventTimerReset,
    EventFinalAlertStarted,
    EventSessionCompleted,
    EventSessionCounterIncremented,
    EventNextSessionStarted,
    EventValidationFailed
}
```

```csharp
public enum CommandTextKey
{
    Start,
    Pause,
    Resume,
    Reset,
    Configure
}
```

```csharp
public enum ErrorTextKey
{
    InvalidSessionDuration,
    InvalidFinalAlertDuration,
    FinalAlertNotLessThanSessionDuration,
    TimerNotConfigured,
    CannotStart,
    CannotPause,
    CannotResume,
    CannotReset,
    InvalidTickDuration
}
```

```csharp
public enum AccessibilityTextKey
{
    StatusTemplate,
    SessionCompletedTemplate,
    ErrorTemplate,
    StartTimer,
    PauseTimer,
    ResumeTimer,
    ResetTimer
}
```

```csharp
public enum UiTextKey
{
    SessionDuration,
    FinalAlertDuration,
    Minutes,
    Seconds,
    RemainingTime,
    TimerState,
    CompletedSessions,
    Message
}
```

Il coding agent può usare una forma equivalente, purché sia tipizzata, semplice e senza stringhe magiche.

---

## 11. Locales/It

La cartella obbligatoria è:

```text
locales/CicloTimer.Localization/Locales/It/
```

Deve contenere solo testi italiani.

Non devono essere create nella prima versione:

```text
Locales/En/
Locales/Fr/
Locales/Es/
```

La cartella `Locales/It/` prepara il progetto a future lingue senza obbligare a refactor.

---

## 12. ItalianTimerTexts

### 12.1 Percorso

```text
locales/CicloTimer.Localization/Locales/It/ItalianTimerTexts.cs
```

### 12.2 Responsabilità

Contenere testi italiani per stati ed eventi del timer.

Testi minimi obbligatori:

```text
Timer fermo
Sessione in corso
Avviso finale in corso
Timer in pausa
Timer configurato.
Timer avviato.
Timer in pausa.
Timer ripreso.
Timer resettato.
Avviso finale iniziato.
Sessione completata.
Sessioni completate aggiornate.
Nuova sessione avviata.
Configurazione o comando non valido.
```

Deve esporre un metodo o proprietà che restituisca testo a partire da `TimerTextKey`.

Esempio concettuale:

```csharp
public static string Get(TimerTextKey key)
```

Ogni valore di `TimerTextKey` deve avere un testo italiano corrispondente.

Non sono ammessi valori enum privi di testo.

---

## 13. ItalianCommandTexts

### 13.1 Percorso

```text
locales/CicloTimer.Localization/Locales/It/ItalianCommandTexts.cs
```

### 13.2 Responsabilità

Contenere testi italiani per i comandi utente.

Testi minimi obbligatori:

```text
Avvia
Pausa
Riprendi
Reset
Configura
```

Deve esporre un metodo o proprietà che restituisca testo a partire da `CommandTextKey`.

Ogni valore di `CommandTextKey` deve avere un testo italiano corrispondente.

Non sono ammessi valori enum privi di testo.

---

## 14. ItalianErrorTexts

### 14.1 Percorso

```text
locales/CicloTimer.Localization/Locales/It/ItalianErrorTexts.cs
```

### 14.2 Responsabilità

Contenere messaggi di errore italiani.

Testi minimi obbligatori:

```text
La durata della sessione deve essere maggiore di zero.
La durata dell'avviso finale non può essere negativa.
La durata dell'avviso finale deve essere inferiore alla durata della sessione.
Configura il timer prima di avviarlo.
Il timer non può essere avviato nello stato corrente.
Il timer non può essere messo in pausa nello stato corrente.
Il timer non può essere ripreso nello stato corrente.
Il timer non può essere resettato nello stato corrente.
Errore interno: durata tick non valida.
```

Deve esporre un metodo o proprietà che restituisca testo a partire da `ErrorTextKey`.

Ogni valore di `ErrorTextKey` deve avere un testo italiano corrispondente.

Non sono ammessi valori enum privi di testo.

---

## 15. ItalianAccessibilityTexts

### 15.1 Percorso

```text
locales/CicloTimer.Localization/Locales/It/ItalianAccessibilityTexts.cs
```

### 15.2 Responsabilità

Contenere testi e template italiani per accessibilità.

Testi minimi obbligatori:

```text
Tempo rimanente: {0}. {1}. {2}.
Sessione completata. Sessioni completate: {0}.
Errore: {0}
Avvia timer
Metti in pausa il timer
Riprendi timer
Resetta timer
```

Deve esporre testi a partire da `AccessibilityTextKey`.

Deve supportare template con parametri.

La classe non deve usare NVDA, UI Automation o Live Region.

Contiene solo testo.

Ogni valore di `AccessibilityTextKey` deve avere un testo italiano corrispondente.

Non sono ammessi valori enum privi di testo.

---

## 16. ItalianUiTexts

### 16.1 Percorso

```text
locales/CicloTimer.Localization/Locales/It/ItalianUiTexts.cs
```

### 16.2 Responsabilità

Contenere etichette UI italiane minime.

Testi minimi obbligatori:

```text
Durata sessione
Durata avviso finale
Minuti
Secondi
Tempo rimanente
Stato timer
Sessioni completate
Messaggio
```

Deve esporre testi a partire da `UiTextKey`.

La UI non viene implementata in questo coding plan.

Questi testi sono preparati per design e coding plan successivi.

Ogni valore di `UiTextKey` deve avere un testo italiano corrispondente.

Non sono ammessi valori enum privi di testo.

---

## 17. LocalizationService

### 17.1 Percorso

```text
locales/CicloTimer.Localization/LocalizationService.cs
```

### 17.2 Responsabilità

Fornire un punto di accesso unico ai testi.

Deve supportare:

1. lingua corrente iniziale italiana;
2. richiesta di testi per categoria;
3. fallback italiano per lingua non supportata;
4. template con parametri.

### 17.3 Lingua corrente

La prima versione deve assumere:

```text
SupportedLanguage.It
```

come lingua corrente.

Non deve implementare:

```text
CultureInfo
rilevamento lingua Windows
lettura preferenze utente
salvataggio lingua
selettore lingua
```

### 17.4 Fallback

Regola semplice:

```text
se la lingua richiesta non è supportata → usa italiano
```

Poiché nella prima versione esiste solo `It`, ogni richiesta valida deve restituire testi italiani.

Nella V1 il parametro lingua, se presente, non deve attivare nessuna logica multilingua reale.

Non deve leggere la lingua di sistema.

Non deve usare `CultureInfo`.

Non deve usare preferenze utente.

Deve interrogare sempre le classi italiane `Italian*Texts`.

### 17.5 Metodi consigliati

Metodi possibili:

```csharp
string GetTimerText(TimerTextKey key, SupportedLanguage? language = null)
string GetCommandText(CommandTextKey key, SupportedLanguage? language = null)
string GetErrorText(ErrorTextKey key, SupportedLanguage? language = null)
string GetAccessibilityText(AccessibilityTextKey key, SupportedLanguage? language = null, params object[] args)
string GetUiText(UiTextKey key, SupportedLanguage? language = null)
```

La forma concreta può variare, ma deve restare:

1. tipizzata;
2. semplice;
3. senza stringhe magiche;
4. testabile;
5. indipendente da core, bridge e UI.

Il parametro `language` è ammesso per predisposizione futura, ma nella prima versione deve ricadere sempre su italiano.

---

## 18. Parametri e template

Il sistema deve supportare template con parametri.

Esempi:

```text
Sessione completata. Sessioni completate: {0}.
Errore: {0}
Tempo rimanente: {0}. {1}. {2}.
```

Il coding agent può usare `string.Format` solo internamente al sistema localization o tramite metodi controllati.

È vietato usare `string.Format` in altri progetti per ricostruire testi utente partendo da template hardcoded fuori da localization.

Esempio ammesso:

```text
LocalizationService.GetAccessibilityText(SessionCompletedTemplate, 3)
```

Esempio non ammesso fuori da localization:

```text
string.Format("Sessione completata. Sessioni completate: {0}.", completedSessions)
```

I test devono verificare esplicitamente il comportamento dei template con il numero di parametri previsto.

Non è richiesto introdurre nella V1 un sistema complesso di validazione dei parametri.

Devono però essere coperti i casi previsti dal design:

```text
SessionCompletedTemplate con 1 parametro
ErrorTemplate con 1 parametro
StatusTemplate con 3 parametri
```

---

## 19. Divieto di dipendenza dal core

Il progetto `CicloTimer.Localization` non deve conoscere:

```text
TimerState
TimerEvent
TimerError
TimerCommandResult
TimerEngine
TimerConfiguration
```

Quindi non deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
```

La mappatura tra tipi core e chiavi localization sarà fatta dal bridge in un futuro coding plan.

Esempio futuro corretto nel bridge:

```text
TimerError.InvalidSessionDuration
→ ErrorTextKey.InvalidSessionDuration
→ LocalizationService.GetErrorText(...)
```

Esempio vietato nel progetto localization:

```text
GetErrorText(TimerError error)
```

---

## 20. Divieto di stringhe magiche e riflessione

Sono vietati nel codice produttivo:

```text
GetText("Running")
GetText("InvalidSessionDuration")
GetText("SessionCompleted")
TimerState.Running.ToString()
TimerError.InvalidSessionDuration.ToString()
TimerEvent.SessionCompleted.ToString()
reflection sugli enum del core
typeof(TimerError).Name
```

Le chiavi devono essere tipizzate.

Il progetto localization non deve usare reflection per scoprire enum del core.

Il progetto localization non deve usare `ToString()` degli enum core perché non conosce il core.

---

## 21. Regole anti-stringhe hardcoded

Sono vietate stringhe utente hardcoded fuori da `CicloTimer.Localization`.

In questo coding plan, non devono essere introdotte stringhe utente in:

```text
models/CicloTimer.Core/
view-models/CicloTimer.Bridge/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
```

I test possono contenere stringhe attese.

Esempio ammesso nei test:

```csharp
Assert.Equal("Sessione in corso", text);
```

Questa eccezione è ammessa perché il test non mostra testo all'utente finale.

---

## 22. Completezza degli enum delle chiavi

Ogni enum di chiavi deve essere completamente coperto dal sistema testi italiano.

Enum interessati:

```text
TimerTextKey
CommandTextKey
ErrorTextKey
AccessibilityTextKey
UiTextKey
```

Per ogni valore di ciascun enum, deve esistere un testo italiano valido.

È obbligatorio creare test automatici che ciclino su tutti i valori degli enum e verifichino che il `LocalizationService`:

1. non lanci eccezioni;
2. non restituisca `null`;
3. non restituisca stringa vuota;
4. non restituisca stringa composta solo da spazi.

Esempio logico:

```text
per ogni valore di TimerTextKey
↓
chiamo LocalizationService.GetTimerText(key)
↓
deve restituire una stringa valida
```

Questo test serve a intercettare subito casi come:

```text
aggiunta nuova chiave enum
↓
testo italiano dimenticato
```

Una chiave senza testo è un errore di sviluppo e deve emergere nei test.

---

## 23. Test minimi obbligatori

Il progetto `CicloTimer.Localization.Tests` deve verificare almeno:

1. il progetto localization compila;
2. il target non è Windows-specifico;
3. `SupportedLanguage.It` esiste;
4. non esistono altre lingue supportate nella prima versione;
5. lingua corrente iniziale italiana;
6. fallback italiano per lingua non supportata o nulla;
7. testi timer principali non vuoti;
8. testi comandi principali non vuoti;
9. testi errori principali non vuoti;
10. testi accessibili principali non vuoti;
11. testi UI principali non vuoti;
12. template accessibile con un parametro;
13. template accessibile con tre parametri;
14. completezza di tutti i valori `TimerTextKey`;
15. completezza di tutti i valori `CommandTextKey`;
16. completezza di tutti i valori `ErrorTextKey`;
17. completezza di tutti i valori `AccessibilityTextKey`;
18. completezza di tutti i valori `UiTextKey`;
19. errore sconosciuto o chiave non gestita produce comportamento controllato;
20. nessuna dipendenza da core;
21. nessuna dipendenza da bridge;
22. nessuna dipendenza da WPF;
23. nessuna dipendenza da API Windows;
24. assenza di uso di `CultureInfo` nella prima versione;
25. assenza di file JSON/XML/RESX nel progetto localization;
26. assenza di cartelle `Locales/En`, `Locales/Fr`, `Locales/Es`;
27. solution build verde;
28. test localization verdi.

---

## 24. Strategia test

I test devono essere semplici e leggibili.

Raggruppamenti consigliati:

```text
SupportedLanguageTests
LocalizationKeysTests
LocalizationServiceTests
ItalianTimerTextsTests
ItalianCommandTextsTests
ItalianErrorTextsTests
ItalianAccessibilityTextsTests
ItalianUiTextsTests
LocalizationCompletenessTests
ProjectDependencyTests
```

I test possono verificare i testi esatti italiani.

Esempio:

```csharp
Assert.Equal("Avvia", text);
```

Questo non viola la regola anti-stringhe perché i test sono verifica, non fonte produttiva.

`LocalizationCompletenessTests` deve contenere i test che attraversano tutti i valori degli enum di chiavi e confermano che ogni chiave è mappata.

---

## 25. ProjectDependencyTests

I test strutturali o verifiche equivalenti devono confermare:

1. `CicloTimer.Localization.csproj` non contiene `net9.0-windows`;
2. `CicloTimer.Localization.csproj` non contiene riferimenti a `CicloTimer.Core`;
3. `CicloTimer.Localization.csproj` non contiene riferimenti a `CicloTimer.Bridge`;
4. `CicloTimer.Localization.csproj` non contiene riferimenti a `ciclotimer.csproj`;
5. il progetto non contiene file `.resx`;
6. il progetto non contiene file `.json`;
7. il progetto non contiene file `.xml` di dati testi;
8. non esistono cartelle di lingue non implementate.

Questi controlli possono essere implementati come test o come verifica documentata nel report finale.

---

## 26. Gestione chiave sconosciuta

Il coding plan deve evitare stati non gestiti.

Per ogni enum key, ogni valore deve essere mappato.

Se una chiave non è gestita, il sistema deve fallire in modo controllato.

Scelte ammissibili:

1. lanciare `ArgumentOutOfRangeException`;
2. restituire un testo tecnico controllato solo se documentato.

Scelta consigliata:

```text
ArgumentOutOfRangeException
```

Motivo:

1. il sistema testi deve essere completo;
2. una chiave non gestita è errore di sviluppo;
3. il bug deve emergere nei test;
4. non è un errore utente.

Questo tipo di eccezione non è un messaggio rivolto all'utente.

La completezza degli enum deve essere verificata nei test, in modo che una chiave non mappata emerga durante la fase di test e non durante l'uso futuro da bridge o UI.

---

## 27. Ordine operativo

L'implementazione deve seguire questo ordine:

```text
1. Verificare struttura attuale repository.
2. Verificare presenza cartella locales/.
3. Creare progetto locales/CicloTimer.Localization/.
4. Configurare target net9.0.
5. Creare SupportedLanguage.cs.
6. Creare LocalizationKeys.cs.
7. Creare cartella Locales/It/.
8. Creare ItalianTimerTexts.cs.
9. Creare ItalianCommandTexts.cs.
10. Creare ItalianErrorTexts.cs.
11. Creare ItalianAccessibilityTexts.cs.
12. Creare ItalianUiTexts.cs.
13. Creare LocalizationService.cs.
14. Creare progetto tests/CicloTimer.Localization.Tests/.
15. Aggiungere riferimento test → localization.
16. Aggiungere entrambi i progetti alla solution.
17. Scrivere test per lingue.
18. Scrivere test per chiavi.
19. Scrivere test per testi italiani.
20. Scrivere test per template.
21. Scrivere test di completezza enum.
22. Scrivere test/controlli per dipendenze vietate.
23. Eseguire dotnet build.
24. Eseguire dotnet test localization.
25. Eseguire dotnet test core per regressione.
26. Produrre report finale.
```

Cursor non deve saltare direttamente all'implementazione senza verificare i percorsi.

---

## 28. Comandi di verifica finali

Alla fine Cursor deve eseguire:

```bash
dotnet build CicloTimer.sln
dotnet test tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
dotnet test tests/CicloTimer.Core.Tests/CicloTimer.Core.Tests.csproj
```

Se possibile, può eseguire anche:

```bash
dotnet test
```

dalla root della solution.

La build deve passare.

I test localization devono passare.

I test core devono continuare a passare.

---

## 29. File da non modificare

Cursor non deve modificare:

```text
models/CicloTimer.Core/
view-models/CicloTimer.Bridge/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
ciclotimer.csproj
```

Eccezioni:

1. `CicloTimer.sln` può essere modificato per aggiungere i nuovi progetti.
2. `.gitignore` può essere modificato solo se necessario per nuovi artefatti.
3. `ciclotimer.csproj` non dovrebbe essere modificato in questo coding plan.

Se Cursor rileva la necessità di modificare il core, il bridge o la UI, deve fermarsi e segnalarlo.

---

## 30. Criteri di completamento

Il coding plan è completato quando:

1. esiste `locales/CicloTimer.Localization/`;
2. esiste `locales/CicloTimer.Localization/CicloTimer.Localization.csproj`;
3. esiste `tests/CicloTimer.Localization.Tests/`;
4. esiste `tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj`;
5. il progetto localization usa `net9.0`;
6. il progetto localization non usa `net9.0-windows`;
7. il progetto localization non referenzia core;
8. il progetto localization non referenzia bridge;
9. il progetto localization non referenzia WPF;
10. esiste `Locales/It/`;
11. non esistono altre lingue implementate;
12. esiste `SupportedLanguage.It`;
13. esistono chiavi tipizzate per Timer, Commands, Errors, Accessibility, Ui;
14. esiste `LocalizationService`;
15. esistono testi italiani minimi richiesti;
16. esistono template con parametri;
17. fallback italiano è implementato in modo semplice;
18. non viene usato `CultureInfo`;
19. non vengono usati JSON/XML/RESX;
20. non vengono usate stringhe magiche come chiavi;
21. non viene usata reflection sugli enum del core;
22. ogni valore di `TimerTextKey` ha un testo italiano;
23. ogni valore di `CommandTextKey` ha un testo italiano;
24. ogni valore di `ErrorTextKey` ha un testo italiano;
25. ogni valore di `AccessibilityTextKey` ha un testo italiano;
26. ogni valore di `UiTextKey` ha un testo italiano;
27. i test di completezza enum passano;
28. i test localization passano;
29. i test core passano;
30. la solution compila;
31. non sono stati modificati core, bridge o UI.

---

## 31. Criteri di non validità

L'implementazione non è valida se:

1. crea il progetto fuori da `locales/CicloTimer.Localization/`;
2. crea test fuori da `tests/CicloTimer.Localization.Tests/`;
3. usa `net9.0-windows`;
4. referenzia `CicloTimer.Core`;
5. referenzia `CicloTimer.Bridge`;
6. referenzia `ciclotimer.csproj`;
7. modifica il core;
8. modifica il bridge;
9. modifica la UI;
10. aggiunge stringhe utente in XAML;
11. aggiunge stringhe utente nel core;
12. aggiunge stringhe utente nel bridge;
13. usa JSON;
14. usa XML;
15. usa RESX;
16. usa database;
17. usa `CultureInfo`;
18. rileva la lingua di Windows;
19. implementa inglese o altre lingue;
20. crea selettore lingua;
21. salva preferenze lingua;
22. usa reflection;
23. usa `ToString()` sugli enum del core per generare chiavi;
24. usa stringhe libere come chiavi;
25. introduce UI Automation;
26. introduce NVDA operativo;
27. introduce audio;
28. introduce API Windows;
29. lascia valori enum senza testo italiano mappato;
30. non crea test di completezza enum;
31. i test falliscono;
32. la build fallisce.

---

## 32. Report finale richiesto a Cursor

Al termine Cursor deve produrre un report con:

1. file creati;
2. file modificati;
3. progetti creati;
4. progetti aggiunti alla solution;
5. target framework usati;
6. riferimenti tra progetti;
7. conferma che localization non dipende da core;
8. conferma che localization non dipende da bridge;
9. conferma che localization non dipende da WPF;
10. conferma che i test referenziano solo localization;
11. conferma che non sono stati creati JSON/XML/RESX;
12. conferma che non sono state create lingue diverse da italiano;
13. conferma che `Locales/It/` esiste;
14. conferma che esiste fallback italiano semplice;
15. conferma che non viene usato `CultureInfo`;
16. conferma che non vengono usate stringhe magiche come chiavi;
17. conferma che non viene usata reflection sugli enum del core;
18. conferma che tutti gli enum di chiavi sono completamente mappati;
19. conferma che esistono test di completezza enum;
20. conferma che non sono stati modificati core, bridge o UI;
21. comandi eseguiti;
22. risultato build;
23. risultato test localization;
24. risultato test core;
25. numero test localization;
26. eventuali test falliti;
27. eventuali deviazioni dal coding plan.

Cursor non deve limitarsi a scrivere “fatto”.

Il report deve essere verificabile.

---

## 33. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. `CicloTimer.Localization` resta progetto separato in `locales/CicloTimer.Localization/`;
2. `CicloTimer.Localization.Tests` resta progetto test in `tests/CicloTimer.Localization.Tests/`;
3. target `net9.0`;
4. nessuna dipendenza da core, bridge, WPF o UI;
5. solo italiano nella prima versione;
6. struttura `Locales/It/`;
7. nessuna lingua futura implementata ora;
8. nessun JSON, XML, RESX, database o `CultureInfo`;
9. fallback semplice verso italiano;
10. chiavi tipizzate tramite enum o forma equivalente;
11. nessuna stringa magica;
12. nessuna reflection;
13. nessun `ToString()` sugli enum core per generare chiavi;
14. test di completezza obbligatori su tutti gli enum delle chiavi;
15. test espliciti sui template con parametri previsti;
16. stringhe attese nei test ammesse come verifica;
17. nessuna modifica a core, bridge o UI.

---

## 34. Stato del documento

Questo documento è approvato come Coding Plan 003 — Sistema centralizzato testi e messaggi applicativi.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni consiglieri AI: test obbligatori di completezza enum, maggiore chiarezza su fallback italiano semplice, test espliciti sui template con parametri e conferma dei vincoli anti-overengineering
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. aggiunta di test obbligatori che attraversano tutti i valori di `TimerTextKey`;
2. aggiunta di test obbligatori che attraversano tutti i valori di `CommandTextKey`;
3. aggiunta di test obbligatori che attraversano tutti i valori di `ErrorTextKey`;
4. aggiunta di test obbligatori che attraversano tutti i valori di `AccessibilityTextKey`;
5. aggiunta di test obbligatori che attraversano tutti i valori di `UiTextKey`;
6. conferma che ogni valore enum deve restituire testo non nullo, non vuoto e non composto solo da spazi;
7. chiarimento che il parametro lingua nella V1 ricade sempre su italiano;
8. conferma dell'esclusione di `CultureInfo`;
9. chiarimento sui test dei template con parametri previsti;
10. conferma che `FrozenDictionary` o dizionari avanzati non sono requisiti obbligatori.

Il documento è approvato dal project owner come base per il successivo TODO 003.
