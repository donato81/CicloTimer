# CicloTimer — TODO 003 — Sistema centralizzato testi e messaggi applicativi

**Tipo documento:** todo operativo  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-02  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/2-coding-plans/003-coding-plan-sistema-testi-centralizzati.md  

---

## 1. Scopo del TODO

Questo documento traduce il Coding Plan 003 in una lista operativa eseguibile da Cursor.

Il suo obiettivo è implementare il sistema centralizzato dei testi e dei messaggi applicativi di CicloTimer.

Il sistema deve essere creato come progetto .NET separato in:

```text
locales/CicloTimer.Localization/
````

I test devono essere creati in:

```text
tests/CicloTimer.Localization.Tests/
```

Il TODO deve guidare Cursor in modo vincolato, evitando che vengano modificati core, bridge, UI, audio o parti fuori perimetro.

---

## 2. Principio operativo

Il principio operativo è:

```text
se una stringa può essere vista, letta o annunciata all'utente, deve stare in CicloTimer.Localization
```

Il sistema localization deve essere il punto ufficiale per:

1. testi timer;
2. testi comandi;
3. testi errori;
4. testi accessibili;
5. testi UI;
6. template con parametri.

Il progetto deve essere semplice, testabile e indipendente.

---

## 3. Perimetro autorizzato

Cursor può:

1. creare `locales/CicloTimer.Localization/`;
2. creare `locales/CicloTimer.Localization/CicloTimer.Localization.csproj`;
3. aggiungere subito `CicloTimer.Localization.csproj` alla solution `CicloTimer.sln`;
4. creare `SupportedLanguage.cs`;
5. creare `LocalizationKeys.cs`;
6. creare `LocalizationService.cs`;
7. creare `locales/CicloTimer.Localization/Locales/It/`;
8. creare `ItalianTimerTexts.cs`;
9. creare `ItalianCommandTexts.cs`;
10. creare `ItalianErrorTexts.cs`;
11. creare `ItalianAccessibilityTexts.cs`;
12. creare `ItalianUiTexts.cs`;
13. creare `tests/CicloTimer.Localization.Tests/`;
14. creare `tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj`;
15. aggiungere subito `CicloTimer.Localization.Tests.csproj` alla solution `CicloTimer.sln`;
16. creare test automatici;
17. creare test di completezza enum;
18. eseguire build e test dalla root del repository;
19. produrre report finale.

Cursor può modificare:

```text
CicloTimer.sln
```

solo per aggiungere i nuovi progetti.

Cursor può modificare `.gitignore` solo se emergono nuovi artefatti non già ignorati.

---

## 4. Fuori perimetro assoluto

Cursor non deve:

1. modificare `models/CicloTimer.Core/`;
2. modificare `view-models/CicloTimer.Bridge/`;
3. modificare `MainWindow.xaml`;
4. modificare `MainWindow.xaml.cs`;
5. modificare `App.xaml`;
6. modificare `App.xaml.cs`;
7. modificare `ciclotimer.csproj`;
8. implementare bridge;
9. implementare UI;
10. implementare audio;
11. implementare orchestratore;
12. usare NVDA;
13. usare UI Automation;
14. creare Live Region;
15. usare API Windows;
16. usare timer reali;
17. usare JSON;
18. usare XML;
19. usare RESX;
20. usare database;
21. usare cloud;
22. usare `CultureInfo`;
23. rilevare la lingua di Windows;
24. creare selettore lingua;
25. salvare preferenze lingua;
26. implementare inglese o altre lingue;
27. creare `Locales/En/`;
28. creare `Locales/Fr/`;
29. creare `Locales/Es/`;
30. usare reflection;
31. usare `ToString()` sugli enum del core per generare chiavi;
32. usare stringhe libere come chiavi;
33. aggiungere stringhe utente in core, bridge o UI.

Se Cursor ritiene necessaria una modifica fuori perimetro, deve fermarsi e segnalarla nel report.

---

## 5. Struttura finale attesa

La struttura finale deve essere:

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

Non devono essere create cartelle alternative come:

```text
src/
resources/
strings/
models/CicloTimer.Localization/
view-models/CicloTimer.Localization/
```

---

## 6. FASE 0 — Ricognizione iniziale

### TODO 003.00 — Verificare repository prima di modificare

Cursor deve prima leggere e verificare:

1. presenza di `CicloTimer.sln`;
2. presenza di `locales/`;
3. presenza di `tests/`;
4. presenza di `models/CicloTimer.Core/`;
5. presenza di `docs/1-design/003-design-sistema-testi-centralizzati.md`;
6. presenza di `docs/2-coding-plans/003-coding-plan-sistema-testi-centralizzati.md`;
7. contenuto attuale di `.gitignore`;
8. assenza di `locales/CicloTimer.Localization/`;
9. assenza di `tests/CicloTimer.Localization.Tests/`.

Risultato atteso:

```text
ricognizione completata
nessuna modifica ancora eseguita
```

Se `locales/CicloTimer.Localization/` o `tests/CicloTimer.Localization.Tests/` esistono già, Cursor deve segnalarlo e non sovrascrivere file senza controllo.

---

## 7. FASE 1 — Creazione progetto Localization

### TODO 003.01 — Creare progetto CicloTimer.Localization

Creare:

```text
locales/CicloTimer.Localization/
```

Creare il file:

```text
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

Il progetto deve usare:

```text
net9.0
```

Il progetto non deve usare:

```text
net9.0-windows
```

Il progetto non deve avere riferimenti a:

```text
CicloTimer.Core
CicloTimer.Bridge
ciclotimer WPF
WPF
WindowsBase
PresentationFramework
Windows API
```

Contenuto indicativo:

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

Subito dopo la creazione del file `.csproj`, aggiungere il progetto alla solution dalla root del repository:

```bash
dotnet sln CicloTimer.sln add locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

Il comando deve essere eseguito dalla root del repository, cioè dalla cartella in cui si trova `CicloTimer.sln`.

Criterio di completamento:

```text
CicloTimer.Localization.csproj creato con target net9.0, senza riferimenti vietati e aggiunto a CicloTimer.sln
```

---

## 8. FASE 2 — Lingua supportata

### TODO 003.02 — Creare SupportedLanguage

Creare:

```text
locales/CicloTimer.Localization/SupportedLanguage.cs
```

Contenuto richiesto:

```csharp
namespace CicloTimer.Localization;

public enum SupportedLanguage
{
    It
}
```

Regole:

1. deve esistere solo `It`;
2. non aggiungere `En`;
3. non aggiungere `Fr`;
4. non aggiungere `Es`;
5. non usare stringhe libere per rappresentare la lingua;
6. non usare `CultureInfo`.

Criterio di completamento:

```text
SupportedLanguage contiene solo It
```

---

## 9. FASE 3 — Chiavi tipizzate

### TODO 003.03 — Creare LocalizationKeys

Creare:

```text
locales/CicloTimer.Localization/LocalizationKeys.cs
```

Deve contenere chiavi tipizzate per:

```text
Timer
Commands
Errors
Accessibility
Ui
```

Forma richiesta: enum separati per categoria.

Creare almeno:

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

Regole:

1. non usare stringhe libere come chiavi;
2. non usare dizionari indicizzati da stringhe libere;
3. non usare reflection;
4. non collegare queste chiavi agli enum del core;
5. non referenziare `CicloTimer.Core`.

Criterio di completamento:

```text
LocalizationKeys.cs contiene enum tipizzati per tutte le categorie previste
```

---

## 10. FASE 4 — Cartella lingua italiana

### TODO 003.04 — Creare cartella Locales/It

Creare:

```text
locales/CicloTimer.Localization/Locales/It/
```

Non creare:

```text
Locales/En/
Locales/Fr/
Locales/Es/
```

Criterio di completamento:

```text
esiste Locales/It e non esistono altre lingue
```

---

## 11. FASE 5 — Testi timer italiani

### TODO 003.05 — Creare ItalianTimerTexts

Creare:

```text
locales/CicloTimer.Localization/Locales/It/ItalianTimerTexts.cs
```

La classe deve restituire testi per ogni `TimerTextKey`.

Testi obbligatori:

```text
StateStopped → Timer fermo
StateRunning → Sessione in corso
StateFinalAlert → Avviso finale in corso
StatePaused → Timer in pausa
EventTimerConfigured → Timer configurato.
EventTimerStarted → Timer avviato.
EventTimerPaused → Timer in pausa.
EventTimerResumed → Timer ripreso.
EventTimerReset → Timer resettato.
EventFinalAlertStarted → Avviso finale iniziato.
EventSessionCompleted → Sessione completata.
EventSessionCounterIncremented → Sessioni completate aggiornate.
EventNextSessionStarted → Nuova sessione avviata.
EventValidationFailed → Configurazione o comando non valido.
```

Metodo richiesto o equivalente:

```csharp
public static string Get(TimerTextKey key)
```

Regole:

1. ogni valore enum deve essere gestito;
2. nessun valore deve restituire null;
3. nessun valore deve restituire stringa vuota;
4. chiave non gestita deve fallire in modo controllato;
5. preferenza: `ArgumentOutOfRangeException`.

Criterio di completamento:

```text
tutti i TimerTextKey restituiscono un testo italiano valido
```

---

## 12. FASE 6 — Testi comando italiani

### TODO 003.06 — Creare ItalianCommandTexts

Creare:

```text
locales/CicloTimer.Localization/Locales/It/ItalianCommandTexts.cs
```

Testi obbligatori:

```text
Start → Avvia
Pause → Pausa
Resume → Riprendi
Reset → Reset
Configure → Configura
```

Metodo richiesto o equivalente:

```csharp
public static string Get(CommandTextKey key)
```

Criterio di completamento:

```text
tutti i CommandTextKey restituiscono un testo italiano valido
```

---

## 13. FASE 7 — Testi errore italiani

### TODO 003.07 — Creare ItalianErrorTexts

Creare:

```text
locales/CicloTimer.Localization/Locales/It/ItalianErrorTexts.cs
```

Testi obbligatori:

```text
InvalidSessionDuration → La durata della sessione deve essere maggiore di zero.
InvalidFinalAlertDuration → La durata dell'avviso finale non può essere negativa.
FinalAlertNotLessThanSessionDuration → La durata dell'avviso finale deve essere inferiore alla durata della sessione.
TimerNotConfigured → Configura il timer prima di avviarlo.
CannotStart → Il timer non può essere avviato nello stato corrente.
CannotPause → Il timer non può essere messo in pausa nello stato corrente.
CannotResume → Il timer non può essere ripreso nello stato corrente.
CannotReset → Il timer non può essere resettato nello stato corrente.
InvalidTickDuration → Errore interno: durata tick non valida.
```

Metodo richiesto o equivalente:

```csharp
public static string Get(ErrorTextKey key)
```

Criterio di completamento:

```text
tutti gli ErrorTextKey restituiscono un messaggio italiano valido
```

---

## 14. FASE 8 — Testi accessibili italiani

### TODO 003.08 — Creare ItalianAccessibilityTexts

Creare:

```text
locales/CicloTimer.Localization/Locales/It/ItalianAccessibilityTexts.cs
```

Testi obbligatori:

```text
StatusTemplate → Tempo rimanente: {0}. {1}. {2}.
SessionCompletedTemplate → Sessione completata. Sessioni completate: {0}.
ErrorTemplate → Errore: {0}
StartTimer → Avvia timer
PauseTimer → Metti in pausa il timer
ResumeTimer → Riprendi timer
ResetTimer → Resetta timer
```

Metodo richiesto o equivalente:

```csharp
public static string Get(AccessibilityTextKey key)
```

Regole:

1. la classe contiene solo testi;
2. non usa NVDA;
3. non usa UI Automation;
4. non crea Live Region;
5. non chiama API Windows.

Criterio di completamento:

```text
tutti gli AccessibilityTextKey restituiscono un testo italiano valido
```

---

## 15. FASE 9 — Testi UI italiani

### TODO 003.09 — Creare ItalianUiTexts

Creare:

```text
locales/CicloTimer.Localization/Locales/It/ItalianUiTexts.cs
```

Testi obbligatori:

```text
SessionDuration → Durata sessione
FinalAlertDuration → Durata avviso finale
Minutes → Minuti
Seconds → Secondi
RemainingTime → Tempo rimanente
TimerState → Stato timer
CompletedSessions → Sessioni completate
Message → Messaggio
```

Metodo richiesto o equivalente:

```csharp
public static string Get(UiTextKey key)
```

Criterio di completamento:

```text
tutti gli UiTextKey restituiscono un testo italiano valido
```

---

## 16. FASE 10 — Servizio di localizzazione

### TODO 003.10 — Creare LocalizationService

Creare:

```text
locales/CicloTimer.Localization/LocalizationService.cs
```

Il servizio deve fornire un punto di accesso centralizzato ai testi.

Metodi richiesti o equivalenti:

```csharp
public string GetTimerText(TimerTextKey key, SupportedLanguage? language = null)
public string GetCommandText(CommandTextKey key, SupportedLanguage? language = null)
public string GetErrorText(ErrorTextKey key, SupportedLanguage? language = null)
public string GetAccessibilityText(AccessibilityTextKey key, SupportedLanguage? language = null, params object[] args)
public string GetUiText(UiTextKey key, SupportedLanguage? language = null)
```

Regole:

1. nella V1 il parametro `language` deve ricadere sempre su italiano;
2. non usare `CultureInfo`;
3. non rilevare lingua Windows;
4. non leggere preferenze utente;
5. non salvare preferenze lingua;
6. non usare JSON/XML/RESX;
7. non usare database;
8. non usare cloud;
9. non usare stringhe magiche;
10. non usare reflection.

Fallback richiesto:

```text
qualsiasi lingua non supportata → italiano
```

Dato che `SupportedLanguage` contiene solo `It`, il servizio deve interrogare sempre le classi `Italian*Texts`.

### 16.1 Comportamento dei template con argomenti assenti

Per i testi con parametri, il servizio può usare `string.Format` solo internamente a `CicloTimer.Localization`.

Se `args` è `null` o vuoto, il metodo deve restituire il template grezzo senza formattazione.

Esempio:

```text
template: "Sessione completata. Sessioni completate: {0}."
args: null
risultato: "Sessione completata. Sessioni completate: {0}."
```

Esempio:

```text
template: "Tempo rimanente: {0}. {1}. {2}."
args: []
risultato: "Tempo rimanente: {0}. {1}. {2}."
```

Questo evita eccezioni inutili nella prima versione.

I casi con parametri corretti devono essere formattati normalmente.

Criterio di completamento:

```text
LocalizationService restituisce testi italiani per tutte le categorie, supporta template con parametri e restituisce template grezzo se args è null o vuoto
```

---

## 17. FASE 11 — Progetto test

### TODO 003.11 — Creare progetto CicloTimer.Localization.Tests

Creare:

```text
tests/CicloTimer.Localization.Tests/
```

Creare:

```text
tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Il progetto test deve usare:

```text
net9.0
```

Framework consigliato:

```text
xUnit
```

Il progetto test deve referenziare solo:

```text
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

Non deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
ciclotimer.csproj
```

Subito dopo la creazione del file `.csproj`, aggiungere il progetto test alla solution dalla root del repository:

```bash
dotnet sln CicloTimer.sln add tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Il comando deve essere eseguito dalla root del repository, cioè dalla cartella in cui si trova `CicloTimer.sln`.

Criterio di completamento:

```text
progetto test creato, riferito solo a CicloTimer.Localization e aggiunto a CicloTimer.sln
```

---

## 18. FASE 12 — Test lingua supportata

### TODO 003.12 — Creare SupportedLanguageTests

Creare:

```text
tests/CicloTimer.Localization.Tests/SupportedLanguageTests.cs
```

Test obbligatori:

1. `SupportedLanguage.It` esiste;
2. `SupportedLanguage` contiene un solo valore;
3. non esistono `En`, `Fr`, `Es`.

Criterio di completamento:

```text
test lingua supportata superati
```

---

## 19. FASE 13 — Test chiavi

### TODO 003.13 — Creare LocalizationKeysTests

Creare:

```text
tests/CicloTimer.Localization.Tests/LocalizationKeysTests.cs
```

Test obbligatori:

1. esiste `TimerTextKey`;
2. esiste `CommandTextKey`;
3. esiste `ErrorTextKey`;
4. esiste `AccessibilityTextKey`;
5. esiste `UiTextKey`;
6. nessun enum è vuoto.

Criterio di completamento:

```text
test chiavi superati
```

---

## 20. FASE 14 — Test testi italiani diretti

### TODO 003.14 — Creare test testi italiani

Creare:

```text
tests/CicloTimer.Localization.Tests/ItalianTimerTextsTests.cs
tests/CicloTimer.Localization.Tests/ItalianCommandTextsTests.cs
tests/CicloTimer.Localization.Tests/ItalianErrorTextsTests.cs
tests/CicloTimer.Localization.Tests/ItalianAccessibilityTextsTests.cs
tests/CicloTimer.Localization.Tests/ItalianUiTextsTests.cs
```

I test devono verificare testi esatti italiani.

Esempi ammessi:

```csharp
Assert.Equal("Sessione in corso", text);
Assert.Equal("Avvia", text);
Assert.Equal("La durata della sessione deve essere maggiore di zero.", text);
```

Queste stringhe nei test sono ammesse perché sono attese di verifica, non codice produttivo.

Criterio di completamento:

```text
test dei testi italiani diretti superati
```

---

## 21. FASE 15 — Test LocalizationService

### TODO 003.15 — Creare LocalizationServiceTests

Creare:

```text
tests/CicloTimer.Localization.Tests/LocalizationServiceTests.cs
```

Test obbligatori:

1. `GetTimerText` restituisce testo italiano;
2. `GetCommandText` restituisce testo italiano;
3. `GetErrorText` restituisce testo italiano;
4. `GetAccessibilityText` restituisce testo italiano;
5. `GetUiText` restituisce testo italiano;
6. `language = null` usa italiano;
7. `language = SupportedLanguage.It` usa italiano;
8. nessun metodo richiede `CultureInfo`;
9. nessun metodo richiede lingua Windows;
10. `GetAccessibilityText` con `args = null` restituisce template grezzo;
11. `GetAccessibilityText` con `args` vuoto restituisce template grezzo.

Criterio di completamento:

```text
LocalizationServiceTests superati
```

---

## 22. FASE 16 — Test template con parametri

### TODO 003.16 — Testare template con parametri

Aggiungere test per:

```text
SessionCompletedTemplate con 1 parametro
ErrorTemplate con 1 parametro
StatusTemplate con 3 parametri
```

Esempi attesi:

```text
Sessione completata. Sessioni completate: 3.
Errore: Durata non valida
Tempo rimanente: 04:59. Sessione in corso. Sessioni completate: 3.
```

Regole:

1. non introdurre validatore complesso di parametri;
2. testare solo i casi previsti;
3. usare `string.Format` solo dentro localization o tramite metodo controllato;
4. se `args` è null o vuoto, restituire il template grezzo senza formattazione.

Criterio di completamento:

```text
template con parametri previsti funzionano e template grezzo restituito quando args è null o vuoto
```

---

## 23. FASE 17 — Test completezza enum

### TODO 003.17 — Creare LocalizationCompletenessTests

Creare:

```text
tests/CicloTimer.Localization.Tests/LocalizationCompletenessTests.cs
```

Test obbligatori:

1. ciclare su tutti i valori di `TimerTextKey`;
2. chiamare `LocalizationService.GetTimerText(key)`;
3. verificare testo non null, non vuoto e non solo spazi;
4. ciclare su tutti i valori di `CommandTextKey`;
5. chiamare `LocalizationService.GetCommandText(key)`;
6. verificare testo non null, non vuoto e non solo spazi;
7. ciclare su tutti i valori di `ErrorTextKey`;
8. chiamare `LocalizationService.GetErrorText(key)`;
9. verificare testo non null, non vuoto e non solo spazi;
10. ciclare su tutti i valori di `AccessibilityTextKey`;
11. chiamare `LocalizationService.GetAccessibilityText(key)`;
12. verificare testo non null, non vuoto e non solo spazi;
13. ciclare su tutti i valori di `UiTextKey`;
14. chiamare `LocalizationService.GetUiText(key)`;
15. verificare testo non null, non vuoto e non solo spazi.

Pattern consigliato per Cursor:

```csharp
foreach (TimerTextKey key in Enum.GetValues<TimerTextKey>())
{
    var result = service.GetTimerText(key);
    Assert.NotNull(result);
    Assert.NotEmpty(result);
    Assert.False(string.IsNullOrWhiteSpace(result));
}
```

Lo stesso schema deve essere applicato anche a:

```text
CommandTextKey
ErrorTextKey
AccessibilityTextKey
UiTextKey
```

Scopo del test:

```text
se viene aggiunta una nuova chiave enum ma non il testo italiano, i test devono fallire
```

Criterio di completamento:

```text
tutti gli enum di chiavi sono completamente mappati
```

---

## 24. FASE 18 — Test dipendenze e file vietati

### TODO 003.18 — Creare ProjectDependencyTests

Creare:

```text
tests/CicloTimer.Localization.Tests/ProjectDependencyTests.cs
```

Verifiche obbligatorie o equivalenti nel report:

1. `CicloTimer.Localization.csproj` non contiene `net9.0-windows`;
2. `CicloTimer.Localization.csproj` non contiene riferimento a `CicloTimer.Core`;
3. `CicloTimer.Localization.csproj` non contiene riferimento a `CicloTimer.Bridge`;
4. `CicloTimer.Localization.csproj` non contiene riferimento a `ciclotimer.csproj`;
5. non esistono file `.resx` nel progetto localization;
6. non esistono file `.json` nel progetto localization;
7. non esistono file `.xml` di dati testi nel progetto localization;
8. non esistono cartelle `Locales/En`, `Locales/Fr`, `Locales/Es`;
9. non esistono riferimenti a `CultureInfo`;
10. non esistono riferimenti a `System.Windows`;
11. non esistono riferimenti a `AutomationProperties`;
12. non esistono riferimenti a NVDA.

Criterio di completamento:

```text
nessuna dipendenza o file vietato rilevato
```

---

## 25. FASE 19 — Verifica solution

### TODO 003.19 — Verificare progetti in CicloTimer.sln

Verificare che la solution contenga:

```text
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Questa fase è solo di verifica, perché l'aggiunta alla solution deve essere già avvenuta:

1. subito dopo la creazione di `CicloTimer.Localization.csproj`;
2. subito dopo la creazione di `CicloTimer.Localization.Tests.csproj`.

Criterio di completamento:

```text
CicloTimer.sln contiene CicloTimer.Localization e CicloTimer.Localization.Tests
```

---

## 26. FASE 20 — Build e test

### TODO 003.20 — Eseguire build e test

Tutti i comandi di questa fase devono essere eseguiti dalla root del repository, cioè dalla cartella in cui si trova:

```text
CicloTimer.sln
```

Eseguire:

```bash
dotnet build CicloTimer.sln
```

Eseguire:

```bash
dotnet test tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Eseguire regressione core:

```bash
dotnet test tests/CicloTimer.Core.Tests/CicloTimer.Core.Tests.csproj
```

Se possibile, eseguire anche:

```bash
dotnet test
```

Criteri:

1. build solution: 0 errori;
2. test localization: tutti superati;
3. test core: tutti ancora superati;
4. nessun test fallito.

Se un comando fallisce, Cursor deve correggere solo nel perimetro autorizzato.

Se la correzione richiede modifiche a core, bridge o UI, Cursor deve fermarsi e segnalarlo.

---

## 27. FASE 21 — Verifica finale working tree

### TODO 003.21 — Verificare file modificati

Cursor deve verificare che siano stati creati/modificati solo:

```text
locales/CicloTimer.Localization/
tests/CicloTimer.Localization.Tests/
CicloTimer.sln
```

Eventualmente:

```text
.gitignore
```

solo se necessario.

Non devono comparire modifiche a:

```text
models/CicloTimer.Core/
view-models/CicloTimer.Bridge/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
ciclotimer.csproj
```

Criterio di completamento:

```text
working tree coerente con il perimetro del TODO
```

---

## 28. FASE 22 — Report finale

### TODO 003.22 — Produrre report finale

Cursor deve produrre un report finale contenente:

1. file creati;
2. file modificati;
3. progetti creati;
4. conferma che `CicloTimer.Localization.csproj` è stato aggiunto alla solution subito dopo la creazione;
5. conferma che `CicloTimer.Localization.Tests.csproj` è stato aggiunto alla solution subito dopo la creazione;
6. target framework usati;
7. riferimenti tra progetti;
8. conferma che localization non dipende da core;
9. conferma che localization non dipende da bridge;
10. conferma che localization non dipende da WPF;
11. conferma che i test referenziano solo localization;
12. conferma che non sono stati creati JSON/XML/RESX;
13. conferma che non sono state create lingue diverse da italiano;
14. conferma che `Locales/It/` esiste;
15. conferma che esiste fallback italiano semplice;
16. conferma che non viene usato `CultureInfo`;
17. conferma che non vengono usate stringhe magiche come chiavi;
18. conferma che non viene usata reflection sugli enum del core;
19. conferma che tutti gli enum di chiavi sono completamente mappati;
20. conferma che esistono test di completezza enum;
21. conferma che i template con `args` null o vuoto restituiscono template grezzo;
22. conferma che i comandi `dotnet` sono stati eseguiti dalla root del repository;
23. conferma che non sono stati modificati core, bridge o UI;
24. comandi eseguiti;
25. risultato build;
26. risultato test localization;
27. risultato test core;
28. numero test localization;
29. eventuali test falliti;
30. eventuali deviazioni dal TODO.

Cursor non deve limitarsi a scrivere:

```text
fatto
```

Il report deve essere verificabile.

---

## 29. Checklist sintetica finale

Prima di dichiarare completato il TODO, Cursor deve poter confermare:

```text
[ ] Creato locales/CicloTimer.Localization/
[ ] Creato CicloTimer.Localization.csproj
[ ] Aggiunto CicloTimer.Localization.csproj alla solution subito dopo la creazione
[ ] Target net9.0
[ ] Nessun net9.0-windows
[ ] Nessuna dipendenza da core
[ ] Nessuna dipendenza da bridge
[ ] Nessuna dipendenza da WPF
[ ] Creato SupportedLanguage con solo It
[ ] Creato LocalizationKeys
[ ] Creato Locales/It/
[ ] Creati testi Timer
[ ] Creati testi Commands
[ ] Creati testi Errors
[ ] Creati testi Accessibility
[ ] Creati testi Ui
[ ] Creato LocalizationService
[ ] Fallback italiano semplice
[ ] Template grezzo restituito se args è null o vuoto
[ ] Nessun CultureInfo
[ ] Nessun JSON/XML/RESX
[ ] Nessuna lingua diversa da italiano
[ ] Creato tests/CicloTimer.Localization.Tests/
[ ] Creato CicloTimer.Localization.Tests.csproj
[ ] Aggiunto CicloTimer.Localization.Tests.csproj alla solution subito dopo la creazione
[ ] Test progetto localization creati
[ ] Test completezza enum creati
[ ] Test dipendenze creati
[ ] dotnet build CicloTimer.sln eseguito dalla root e superato
[ ] dotnet test localization eseguito dalla root e superato
[ ] dotnet test core eseguito dalla root e superato
[ ] Nessuna modifica a core, bridge o UI
[ ] Report finale prodotto
```

---

## 30. Criteri di completamento globale

Il TODO 003 è completato solo se:

1. il progetto localization esiste;
2. il progetto test esiste;
3. entrambi sono nella solution;
4. i progetti sono stati aggiunti alla solution subito dopo la creazione dei rispettivi `.csproj`;
5. i testi italiani minimi sono presenti;
6. tutte le chiavi enum sono coperte da testi;
7. il fallback italiano è implementato in modo semplice;
8. i template con `args` null o vuoto restituiscono template grezzo;
9. non esistono lingue diverse da italiano;
10. non esistono JSON/XML/RESX;
11. non esiste uso di `CultureInfo`;
12. non esistono dipendenze da core, bridge o WPF;
13. i test localization passano;
14. i test core passano;
15. la build della solution passa;
16. i comandi sono stati eseguiti dalla root del repository;
17. non sono stati modificati file fuori perimetro;
18. il report finale è completo.

---

## 31. Criteri di non validità

L'implementazione non è valida se:

1. il progetto localization non viene creato;
2. il progetto test non viene creato;
3. il progetto localization non è in `locales/CicloTimer.Localization/`;
4. i test non sono in `tests/CicloTimer.Localization.Tests/`;
5. viene usato `net9.0-windows`;
6. localization referenzia core;
7. localization referenzia bridge;
8. localization referenzia WPF;
9. i test referenziano core, bridge o WPF;
10. vengono introdotti JSON/XML/RESX;
11. viene introdotto `CultureInfo`;
12. viene rilevata la lingua Windows;
13. vengono create lingue diverse da italiano;
14. vengono create stringhe utente in core;
15. vengono create stringhe utente nel bridge;
16. vengono create stringhe utente in XAML;
17. vengono usate stringhe libere come chiavi;
18. viene usata reflection sugli enum del core;
19. viene usato `ToString()` sugli enum del core per generare chiavi;
20. qualche valore enum non ha testo italiano;
21. mancano test di completezza enum;
22. i progetti non vengono aggiunti alla solution;
23. i comandi `dotnet` vengono eseguiti da una sottocartella sbagliata producendo verifiche non affidabili;
24. i template con `args` null o vuoto generano eccezioni invece di restituire il template grezzo;
25. fallisce la build;
26. falliscono i test;
27. vengono modificati core, bridge o UI.

---

## 32. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. `CicloTimer.Localization` in `locales/CicloTimer.Localization/`;
2. `CicloTimer.Localization.Tests` in `tests/CicloTimer.Localization.Tests/`;
3. aggiunta immediata di `CicloTimer.Localization.csproj` alla solution dopo la sua creazione;
4. aggiunta immediata di `CicloTimer.Localization.Tests.csproj` alla solution dopo la sua creazione;
5. esecuzione dei comandi `dotnet` dalla root del repository;
6. target `net9.0`;
7. nessuna dipendenza da core, bridge o WPF;
8. `SupportedLanguage` solo con `It`;
9. `Locales/It/` come unica lingua implementata;
10. nessun JSON/XML/RESX;
11. nessun `CultureInfo`;
12. nessuna lingua futura implementata ora;
13. chiavi tipizzate tramite enum;
14. test di completezza enum obbligatori;
15. template con parametri testati;
16. template grezzo restituito se `args` è null o vuoto;
17. nessuna modifica a core, bridge o UI;
18. report finale dettagliato obbligatorio.

---

## 33. Stato del documento

Questo documento è approvato come TODO 003 — Sistema centralizzato testi e messaggi applicativi.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni consiglieri AI: aggiunta immediata dei progetti alla solution, comandi dotnet eseguiti dalla root del repository, comportamento sicuro dei template con args null o vuoto
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. aggiunta del progetto `CicloTimer.Localization` alla solution subito dopo la creazione del relativo `.csproj`;
2. aggiunta del progetto `CicloTimer.Localization.Tests` alla solution subito dopo la creazione del relativo `.csproj`;
3. trasformazione della FASE 19 in verifica della solution invece che prima aggiunta;
4. chiarimento che tutti i comandi `dotnet` devono essere eseguiti dalla root del repository;
5. chiarimento che `GetAccessibilityText` deve restituire il template grezzo se `args` è null o vuoto;
6. aggiunta di test dedicati per `args` null o vuoto;
7. aggiunta del pattern consigliato per i test di completezza enum;
8. aggiornamento del report finale richiesto a Cursor;
9. aggiornamento della checklist finale;
10. aggiornamento dei criteri di completamento e non validità.

Il documento è approvato dal project owner come base operativa per l'implementazione del sistema centralizzato testi.
