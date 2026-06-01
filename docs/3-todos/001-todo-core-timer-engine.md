# CicloTimer — TODO 001 — Core timer engine

**Tipo documento:** todo operativo  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-01  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/2-coding-plans/001-coding-plan-core-timer-engine.md  

---

## 1. Scopo del documento

Questo documento definisce il TODO operativo per implementare il core timer engine di CicloTimer.

Il TODO deriva da:

```text
docs/1-design/001-design-core-timer-engine.md
docs/2-coding-plans/001-coding-plan-core-timer-engine.md
````

Il suo scopo è fornire a Cursor una lista ordinata, verificabile e limitata di attività da eseguire.

Questo TODO non modifica il design.

Questo TODO non modifica il coding plan.

Questo TODO non autorizza funzionalità nuove.

Questo TODO deve guidare un solo ciclo operativo:

```text
implementazione core timer engine
+
tipi neutri
+
test unitari del core
```

---

## 2. Regola principale

Cursor deve rispettare questa regola:

```text
implementare solo il core timer engine e i relativi test unitari
```

Il lavoro è valido solo se resta nel perimetro del coding plan 001.

---

## 3. Fuori perimetro operativo

Durante questo TODO Cursor non deve lavorare su:

1. UI WPF;
2. `MainWindow.xaml`;
3. `MainWindow.xaml.cs`;
4. layout grafico;
5. controlli accessibili;
6. NVDA;
7. UI Automation;
8. AutomationProperties;
9. Live Region;
10. audio reale;
11. riproduzione suoni;
12. volume di sistema;
13. notifiche Windows;
14. API Windows;
15. bridge UI-logica completo;
16. viewmodel UI;
17. testi utente finali;
18. localizzazione;
19. persistenza;
20. storico sessioni;
21. numero massimo di sessioni;
22. timer lavoro/pausa;
23. funzioni economiche;
24. database;
25. cloud;
26. server locale;
27. plugin;
28. funzioni extra.

La UI può rimanere scollegata dal core.

Il successo del TODO si misura sui test del core.

---

## 4. Preparazione

### TODO 001.01 — Leggere i documenti approvati

Cursor deve leggere prima di codificare:

```text
docs/0-architecture/vision.md
docs/0-architecture/architecture.md
docs/0-architecture/accessibility-rules.md
docs/0-architecture/document-standards.md
docs/0-architecture/internal-api.md
docs/1-design/001-design-core-timer-engine.md
docs/2-coding-plans/001-coding-plan-core-timer-engine.md
```

Criterio di completamento:

```text
Cursor conferma di aver letto i documenti e di usare come fonte operativa principale il coding plan 001.
```

---

### TODO 001.02 — Analizzare la struttura reale del repository

Cursor deve verificare:

1. presenza di file `.sln`;
2. presenza di file `.csproj`;
3. target framework del progetto esistente;
4. cartelle esistenti;
5. eventuale presenza di `src/`;
6. eventuale presenza di `tests/`;
7. eventuale progetto WPF esistente.

Criterio di completamento:

```text
Cursor riporta la struttura rilevante prima di creare nuovi file.
```

---

### TODO 001.03 — Identificare il target framework

Cursor deve leggere il target framework dal progetto esistente.

Esempi possibili:

```text
net8.0-windows
net9.0-windows
```

Regole:

1. il progetto core deve usare un target compatibile;
2. il progetto core non deve usare `-windows` se non necessario;
3. il progetto test deve usare un target compatibile;
4. Cursor non deve scegliere una versione .NET casuale.

Criterio di completamento:

```text
Cursor dichiara il target framework rilevato e quello scelto per Core e Tests.
```

---

## 5. Creazione progetti

### TODO 001.04 — Creare progetto core separato

Cursor deve creare un progetto separato per il core.

Nome consigliato:

```text
CicloTimer.Core
```

Percorso consigliato:

```text
src/CicloTimer.Core/
```

Tipo progetto consigliato:

```text
class library
```

Vincoli:

1. non deve dipendere dal progetto WPF;
2. non deve referenziare WPF;
3. non deve referenziare Windows Forms;
4. non deve referenziare UI Automation;
5. non deve referenziare librerie audio;
6. non deve referenziare API Windows specifiche.

Criterio di completamento:

```text
CicloTimer.Core esiste e compila come progetto separato.
```

---

### TODO 001.05 — Creare progetto test separato

Cursor deve creare un progetto test separato.

Nome consigliato:

```text
CicloTimer.Core.Tests
```

Percorso consigliato:

```text
tests/CicloTimer.Core.Tests/
```

Framework test consigliato:

```text
xUnit
```

Vincoli:

1. il progetto test deve referenziare `CicloTimer.Core`;
2. il progetto test non deve referenziare il progetto WPF;
3. deve essere usato un solo framework di test.

Criterio di completamento:

```text
CicloTimer.Core.Tests esiste e referenzia CicloTimer.Core.
```

---

### TODO 001.06 — Aggiornare la solution se presente

Se esiste una solution `.sln`, Cursor deve aggiungere i nuovi progetti alla solution.

Comandi indicativi, da adattare ai percorsi reali:

```text
dotnet sln add src/CicloTimer.Core/CicloTimer.Core.csproj
dotnet sln add tests/CicloTimer.Core.Tests/CicloTimer.Core.Tests.csproj
```

Se non esiste una solution, Cursor può crearne una minima solo se necessario per eseguire build e test dalla radice.

Criterio di completamento:

```text
dotnet build e dotnet test dalla radice trovano i progetti creati, se la struttura del repository lo consente.
```

---

## 6. Tipi neutri

### TODO 001.07 — Creare TimerState

Creare un tipo neutro per gli stati stabili.

Nome consigliato:

```text
TimerState
```

Valori obbligatori:

```text
Stopped
Running
FinalAlert
Paused
```

Regole:

1. il tipo deve essere dichiarato `public`;
2. può essere implementato come enum;
3. non creare `Completed`;
4. non creare `Finished`;
5. non creare `SessionEnded`.

Criterio di completamento:

```text
TimerState contiene solo gli stati stabili approvati ed è accessibile fuori dall'assembly Core.
```

---

### TODO 001.08 — Creare TimerError

Creare un tipo neutro per gli errori logici prevedibili.

Nome consigliato:

```text
TimerError
```

Valori obbligatori:

```text
InvalidSessionDuration
InvalidFinalAlertDuration
FinalAlertNotLessThanSessionDuration
TimerNotConfigured
CannotStart
CannotPause
CannotResume
CannotReset
InvalidTickDuration
```

Regole:

1. il tipo deve essere dichiarato `public`;
2. può essere implementato come enum;
3. non inserire testi utente;
4. non inserire messaggi italiani;
5. non usare eccezioni al posto degli errori neutri.

Criterio di completamento:

```text
TimerError contiene tutti e solo gli errori neutri previsti ed è accessibile fuori dall'assembly Core.
```

---

### TODO 001.09 — Creare TimerEvent

Creare un tipo neutro per gli eventi del timer.

Nome consigliato:

```text
TimerEvent
```

Valori obbligatori:

```text
TimerConfigured
TimerStarted
TimerPaused
TimerResumed
TimerReset
FinalAlertStarted
SessionCompleted
SessionCounterIncremented
NextSessionStarted
ValidationFailed
```

Regole:

1. il tipo deve essere dichiarato `public`;
2. usare una rappresentazione semplice;
3. per il primo ciclo è sufficiente un enum;
4. non creare event bus;
5. non creare observer;
6. non creare eventi asincroni.

Nota importante:

```text
ValidationFailed è un evento neutro.
La causa specifica del fallimento deve stare in TimerError.
```

Esempio corretto:

```text
Events = [ValidationFailed]
Errors = [InvalidSessionDuration]
```

Esempio scorretto:

```text
Events = [InvalidSessionDuration]
```

Criterio di completamento:

```text
TimerEvent contiene gli eventi neutri previsti, inclusa ValidationFailed, senza canali complessi.
```

---

### TODO 001.10 — Creare TimerConfiguration

Creare il tipo di configurazione.

Nome consigliato:

```text
TimerConfiguration
```

Campi obbligatori:

```text
SessionDurationSeconds
FinalAlertDurationSeconds
```

Regole:

1. il tipo deve essere dichiarato `public`;
2. valori interi;
3. nessun campo minuti/secondi UI separato;
4. nessun testo utente;
5. nessun dato audio;
6. nessuna informazione NVDA;
7. nessuna persistenza.

Criterio di completamento:

```text
TimerConfiguration rappresenta solo durate neutre in secondi ed è accessibile fuori dall'assembly Core.
```

---

### TODO 001.11 — Creare TimerCommandResult

Creare il risultato dei comandi.

Nome consigliato:

```text
TimerCommandResult
```

Campi consigliati:

```text
Success
State
RemainingSeconds
CompletedSessions
IsConfigured
Errors
Events
```

Regole generali:

1. il tipo deve essere dichiarato `public`;
2. il result rappresenta l'esito del comando corrente;
3. il result non deve essere persistente;
4. il result non deve diventare una seconda fonte di verità del core;
5. la fonte primaria dello stato corrente resta `TimerEngine`.

Regole su `State`, `RemainingSeconds`, `CompletedSessions`, `IsConfigured`:

```text
questi campi sono uno snapshot dell'esito del comando appena eseguito.
```

Non devono essere usati come stato autonomo separato dal `TimerEngine`.

Regole su `Errors`:

```text
Errors contiene solo errori del comando corrente.
```

Regole su `Events`:

```text
Events contiene solo eventi del comando corrente.
Events non è uno storico cumulativo.
Events è nuova per ogni comando.
Events può essere vuota.
Events preserva l'ordine cronologico.
```

Divieti:

1. non includere testi utente;
2. non includere oggetti WPF;
3. non includere riferimenti audio;
4. non includere codici Windows;
5. non includere `CanStart`;
6. non includere `CanPause`;
7. non includere `CanResume`;
8. non includere `CanReset`.

Le proprietà `CanStart`, `CanPause`, `CanResume`, `CanReset` appartengono al `TimerEngine` come proprietà pubbliche di sola lettura calcolate dinamicamente.

Criterio di completamento:

```text
TimerCommandResult rappresenta solo l'esito del comando corrente, contiene uno snapshot coerente e non sostituisce lo stato del TimerEngine.
```

---

## 7. TimerEngine

### TODO 001.12 — Creare TimerEngine

Creare il motore core.

Nome consigliato:

```text
TimerEngine
```

Regole:

1. il tipo deve essere dichiarato `public`;
2. deve poter essere istanziato senza UI;
3. non deve dipendere da WPF;
4. non deve dipendere da audio;
5. non deve dipendere da API Windows;
6. non deve dipendere da NVDA o UI Automation.

Stato iniziale obbligatorio:

```text
CurrentState = Stopped
CompletedSessions = 0
IsConfigured = false
```

Il core non deve inventare valori di default UI.

Criterio di completamento:

```text
TimerEngine esiste, è public e può essere istanziato senza UI, audio o Windows API.
```

---

### TODO 001.13 — Esporre dati dinamici del core

TimerEngine deve esporre dati dinamici neutri.

Dati minimi:

```text
CurrentState
RemainingSeconds
CompletedSessions
IsConfigured
IsFinalAlertActive
```

Regole:

1. `RemainingSeconds` è un numero intero;
2. non formattare `RemainingSeconds` in `mm:ss`;
3. non generare testi utente;
4. non generare testi accessibili;
5. `IsFinalAlertActive` deve essere derivato da `CurrentState == FinalAlert`;
6. `IsFinalAlertActive` non deve introdurre uno stato parallelo;
7. `IsFinalAlertActive` non deve diventare una seconda fonte di verità.

Criterio di completamento:

```text
Il core espone stato e dati dinamici senza stringhe utente; IsFinalAlertActive è derivato da CurrentState.
```

---

### TODO 001.14 — Esporre disponibilità comandi

TimerEngine deve esporre proprietà pubbliche di sola lettura:

```text
CanStart
CanPause
CanResume
CanReset
```

Regole:

```text
CanStart = IsConfigured && CurrentState == Stopped
CanPause = CurrentState == Running || CurrentState == FinalAlert
CanResume = CurrentState == Paused
CanReset = IsConfigured
```

Queste proprietà devono essere calcolate dinamicamente sullo stato corrente.

Non devono essere duplicate in `TimerCommandResult`.

Criterio di completamento:

```text
Le proprietà Can* sono leggibili dal TimerEngine e non sono presenti nel result object.
```

---

## 8. Comandi core

### TODO 001.15 — Implementare ConfigureTimer

Implementare:

```text
ConfigureTimer(TimerConfiguration configuration)
```

Configurazione valida:

```text
SessionDurationSeconds > 0
FinalAlertDurationSeconds >= 0
FinalAlertDurationSeconds < SessionDurationSeconds
```

Comportamento atteso con configurazione valida:

```text
Success = true
IsConfigured = true
CurrentState = Stopped
RemainingSeconds = SessionDurationSeconds
Events = [TimerConfigured]
Errors = []
```

Comportamento atteso con configurazione non valida:

```text
Success = false
configurazione precedente invariata
stato precedente invariato
Events = [ValidationFailed]
Errors contiene errore neutro appropriato
```

Regola su `ValidationFailed`:

```text
ValidationFailed deve stare nella lista Events.
L'errore specifico deve stare nella lista Errors.
```

Esempio:

```text
Events = [ValidationFailed]
Errors = [FinalAlertNotLessThanSessionDuration]
```

Criterio di completamento:

```text
ConfigureTimer valida correttamente la configurazione, non lascia stati incoerenti e separa ValidationFailed dagli errori specifici.
```

---

### TODO 001.16 — Implementare StartTimer

Implementare:

```text
StartTimer()
```

Ammesso quando:

```text
IsConfigured = true
CurrentState = Stopped
```

Comportamento valido:

```text
Success = true
CurrentState = Running
RemainingSeconds = SessionDurationSeconds
Events = [TimerStarted]
Errors = []
```

Comportamento non valido:

```text
Success = false
Errors contiene TimerNotConfigured oppure CannotStart
```

Regola:

```text
StartTimer non deve partire direttamente in FinalAlert.
```

Criterio di completamento:

```text
StartTimer avvia solo timer configurati e fermi.
```

---

### TODO 001.17 — Implementare PauseTimer

Implementare:

```text
PauseTimer()
```

Ammesso quando:

```text
CurrentState = Running
oppure
CurrentState = FinalAlert
```

Comportamento valido:

```text
Success = true
CurrentState = Paused
RemainingSeconds invariato
CompletedSessions invariato
Events = [TimerPaused]
```

Comportamento non valido:

```text
Success = false
Errors contiene CannotPause
```

Criterio di completamento:

```text
PauseTimer mette in pausa senza completare sessioni e senza modificare contatore.
```

---

### TODO 001.18 — Implementare ResumeTimer fuori dalla finestra finale

Implementare la ripresa da pausa fuori dalla finestra finale.

Condizione:

```text
CurrentState = Paused
RemainingSeconds > FinalAlertDurationSeconds
```

oppure:

```text
FinalAlertDurationSeconds = 0
```

Comportamento atteso:

```text
Success = true
CurrentState = Running
RemainingSeconds invariato
Events = [TimerResumed]
```

Criterio di completamento:

```text
ResumeTimer fuori dalla finestra finale torna a Running e produce solo TimerResumed.
```

---

### TODO 001.19 — Implementare ResumeTimer dentro la finestra finale

Implementare la ripresa da pausa dentro la finestra finale.

Condizione:

```text
CurrentState = Paused
FinalAlertDurationSeconds > 0
RemainingSeconds <= FinalAlertDurationSeconds
RemainingSeconds > 0
```

Comportamento obbligatorio:

```text
Success = true
CurrentState = FinalAlert
RemainingSeconds invariato
Events = [TimerResumed, FinalAlertStarted]
```

L'ordine degli eventi è obbligatorio:

```text
TimerResumed
↓
FinalAlertStarted
```

Criterio di completamento:

```text
ResumeTimer dentro la finestra finale produce TimerResumed e poi FinalAlertStarted.
```

---

### TODO 001.20 — Implementare ResumeTimer non valido

Se `ResumeTimer` viene chiamato fuori da `Paused`, deve fallire.

Comportamento atteso:

```text
Success = false
Errors contiene CannotResume
```

Criterio di completamento:

```text
ResumeTimer non è ammesso fuori dallo stato Paused.
```

---

### TODO 001.21 — Implementare ResetTimer

Implementare:

```text
ResetTimer()
```

Ammesso quando:

```text
IsConfigured = true
```

Comportamento valido da qualsiasi stato configurato:

```text
Success = true
CurrentState = Stopped
RemainingSeconds = SessionDurationSeconds
CompletedSessions invariato
Events = [TimerReset]
```

Regole:

1. reset non completa sessioni;
2. reset non incrementa il contatore;
3. reset non azzera il contatore;
4. reset non produce `SessionCompleted`;
5. reset non produce `SessionCounterIncremented`;
6. reset non produce `NextSessionStarted`.

Criterio di completamento:

```text
ResetTimer riporta a Stopped senza alterare CompletedSessions.
```

---

### TODO 001.22 — Implementare ResetTimer da Stopped

Se il timer è già `Stopped` e configurato, `ResetTimer` deve comunque produrre `TimerReset`.

Comportamento atteso:

```text
Success = true
CurrentState = Stopped
RemainingSeconds = SessionDurationSeconds
CompletedSessions invariato
Events = [TimerReset]
```

Criterio di completamento:

```text
ResetTimer da Stopped produce TimerReset e nessun evento di ciclo.
```

---

### TODO 001.23 — Implementare ResetTimer non valido

Se `ResetTimer` viene chiamato senza configurazione valida, deve fallire.

Comportamento atteso:

```text
Success = false
Errors contiene TimerNotConfigured oppure CannotReset
```

Criterio di completamento:

```text
ResetTimer non configurato non modifica lo stato e produce errore neutro.
```

---

## 9. Tick e transizioni temporali

### TODO 001.24 — Implementare validazione di Tick

Implementare:

```text
Tick(int elapsedSeconds)
```

Regola:

```text
elapsedSeconds >= 1
```

Se `elapsedSeconds <= 0`:

```text
Success = false
Errors = [InvalidTickDuration]
nessuna modifica allo stato
nessun evento di ciclo
```

Criterio di completamento:

```text
Tick con durata zero o negativa produce InvalidTickDuration.
```

---

### TODO 001.25 — Implementare Tick da Stopped

Se `Tick` viene chiamato in `Stopped`, deve consumare l'impulso senza effetti collaterali.

Comportamento atteso:

```text
Success = true
CurrentState = Stopped
RemainingSeconds invariato
CompletedSessions invariato
Events = []
Errors = []
```

Criterio di completamento:

```text
Tick da Stopped non modifica lo stato e non produce eventi di ciclo.
```

---

### TODO 001.26 — Implementare Tick da Paused

Se `Tick` viene chiamato in `Paused`, deve consumare l'impulso senza effetti collaterali.

Comportamento atteso:

```text
Success = true
CurrentState = Paused
RemainingSeconds invariato
CompletedSessions invariato
Events = []
Errors = []
```

Criterio di completamento:

```text
Tick da Paused non modifica lo stato e non produce eventi di ciclo.
```

---

### TODO 001.27 — Implementare Tick da Running

Se `Tick` viene chiamato in `Running`, deve ridurre `RemainingSeconds`.

Regola:

```text
RemainingSeconds = max(0, RemainingSeconds - elapsedSeconds)
```

Se non entra in avviso finale e non arriva a zero:

```text
CurrentState = Running
Events = []
```

Criterio di completamento:

```text
Tick da Running riduce RemainingSeconds senza produrre eventi inutili.
```

---

### TODO 001.28 — Implementare ingresso in FinalAlert

Quando il tempo entra nella finestra finale:

```text
FinalAlertDurationSeconds > 0
RemainingSeconds <= FinalAlertDurationSeconds
RemainingSeconds > 0
```

il core deve:

```text
CurrentState = FinalAlert
Events = [FinalAlertStarted]
```

Criterio di completamento:

```text
Il core produce FinalAlertStarted solo al primo ingresso nella finestra finale.
```

---

### TODO 001.29 — Evitare FinalAlertStarted ripetuto

Durante la stessa sessione, dopo essere entrato in `FinalAlert`, i tick successivi non devono produrre di nuovo `FinalAlertStarted`.

Criterio di completamento:

```text
FinalAlertStarted viene prodotto una sola volta per sessione, salvo ripresa da pausa dentro finestra finale.
```

---

### TODO 001.30 — Gestire avviso finale pari a zero

Se:

```text
FinalAlertDurationSeconds = 0
```

il core non deve mai entrare in `FinalAlert`.

Non deve mai produrre:

```text
FinalAlertStarted
```

Criterio di completamento:

```text
Avviso finale pari a zero disattiva FinalAlert e FinalAlertStarted.
```

---

### TODO 001.31 — Implementare completamento sessione

Quando `RemainingSeconds` arriva a zero, il core deve completare la sessione.

Eventi obbligatori:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

Ordine obbligatorio:

```text
SessionCompleted
↓
SessionCounterIncremented
↓
NextSessionStarted
```

Effetti:

```text
CompletedSessions aumenta di 1
RemainingSeconds = SessionDurationSeconds
CurrentState = Running
```

Criterio di completamento:

```text
La sessione completata incrementa il contatore e riparte automaticamente in Running.
```

---

### TODO 001.32 — Scartare tempo eccedente nel Tick

Se `elapsedSeconds` supera il tempo rimanente, l'eccedenza non deve essere trasferita alla nuova sessione.

Esempio:

```text
RemainingSeconds = 3
elapsedSeconds = 5
SessionDurationSeconds = 300
```

Risultato corretto:

```text
CompletedSessions aumenta di 1
RemainingSeconds = 300
CurrentState = Running
```

Risultato vietato:

```text
RemainingSeconds = 298
```

Criterio di completamento:

```text
Il tick eccedente completa la sessione e riparte da SessionDurationSeconds pieno.
```

---

### TODO 001.33 — Evitare FinalAlertStarted su tick diretto a zero

Se un tick porta direttamente a zero, il core deve completare la sessione senza produrre obbligatoriamente `FinalAlertStarted`.

Esempio:

```text
RemainingSeconds = 10
FinalAlertDurationSeconds = 5
Tick(10)
```

Eventi attesi:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

Non è richiesto:

```text
FinalAlertStarted
```

Criterio di completamento:

```text
Il completamento diretto ha priorità sull'ingresso in FinalAlert.
```

---

## 10. Test unitari

### TODO 001.34 — Creare test di configurazione

Creare test per:

1. configurazione valida;
2. durata sessione zero;
3. avviso finale negativo;
4. avviso finale uguale alla durata sessione;
5. avviso finale maggiore della durata sessione;
6. configurazione non valida dopo configurazione valida;
7. `ValidationFailed` nella lista eventi in caso di configurazione non valida;
8. errore specifico nella lista errori in caso di configurazione non valida.

Criterio di completamento:

```text
La validazione configurazione è coperta da test, distinguendo ValidationFailed dagli errori specifici.
```

---

### TODO 001.35 — Creare test per StartTimer

Creare test per:

1. avvio senza configurazione;
2. avvio con configurazione valida;
3. avvio da stato non ammesso;
4. stato `Running` dopo start;
5. evento `TimerStarted`.

Criterio di completamento:

```text
StartTimer è coperto nei casi validi e non validi.
```

---

### TODO 001.36 — Creare test per PauseTimer

Creare test per:

1. pausa da `Running`;
2. pausa da `FinalAlert`;
3. pausa da stato non valido;
4. `RemainingSeconds` invariato;
5. `CompletedSessions` invariato;
6. evento `TimerPaused`.

Criterio di completamento:

```text
PauseTimer è coperto nei casi validi e non validi.
```

---

### TODO 001.37 — Creare test per ResumeTimer

Creare test per:

1. ripresa fuori finestra finale;
2. ripresa dentro finestra finale;
3. ripresa da stato non valido;
4. evento `TimerResumed`;
5. evento `FinalAlertStarted` dopo ripresa dentro finestra finale;
6. ordine eventi `[TimerResumed, FinalAlertStarted]`.

Criterio di completamento:

```text
ResumeTimer è coperto e la ripresa in FinalAlert produce eventi ordinati corretti.
```

---

### TODO 001.38 — Creare test per ResetTimer

Creare test per:

1. reset da `Running`;
2. reset da `FinalAlert`;
3. reset da `Paused`;
4. reset da `Stopped`;
5. reset senza configurazione;
6. evento `TimerReset` da `Stopped`;
7. assenza di eventi di ciclo;
8. contatore invariato.

Criterio di completamento:

```text
ResetTimer è coperto e non altera CompletedSessions.
```

---

### TODO 001.39 — Creare test per Tick

Creare test per:

1. tick da `Running`;
2. tick da `FinalAlert`;
3. tick da `Paused`;
4. tick da `Stopped`;
5. tick con zero;
6. tick negativo;
7. tick maggiore del tempo rimanente;
8. scarto del tempo eccedente;
9. nessun effetto collaterale in `Paused` e `Stopped`.

Criterio di completamento:

```text
Tick è coperto nei casi validi, invalidi e inattivi.
```

---

### TODO 001.40 — Creare test per FinalAlert

Creare test per:

1. ingresso in `FinalAlert`;
2. evento `FinalAlertStarted`;
3. `FinalAlertStarted` prodotto una sola volta per sessione;
4. `IsFinalAlertActive` vero quando `CurrentState = FinalAlert`;
5. `IsFinalAlertActive` falso quando `CurrentState != FinalAlert`;
6. avviso finale pari a zero;
7. assenza di `FinalAlertStarted` con avviso zero;
8. assenza di `FinalAlertStarted` se tick porta direttamente a zero.

Criterio di completamento:

```text
La logica FinalAlert è coperta senza audio e senza UI.
```

---

### TODO 001.41 — Creare test per fine sessione e ripartenza automatica

Creare test per:

1. completamento sessione;
2. incremento `CompletedSessions`;
3. ordine eventi `[SessionCompleted, SessionCounterIncremented, NextSessionStarted]`;
4. `RemainingSeconds = SessionDurationSeconds` dopo ripartenza;
5. `CurrentState = Running` subito dopo ripartenza;
6. nessuno stato stabile `Completed`;
7. ripartenza senza intervento del bridge.

Criterio di completamento:

```text
La ripartenza automatica è coperta e resta responsabilità del core.
```

---

### TODO 001.42 — Creare test per disponibilità comandi

Creare test per:

1. `CanStart`;
2. `CanPause`;
3. `CanResume`;
4. `CanReset`;
5. variazione delle proprietà dopo configurazione;
6. variazione delle proprietà dopo start;
7. variazione delle proprietà dopo pausa;
8. variazione delle proprietà dopo resume;
9. variazione delle proprietà dopo reset;
10. assenza delle proprietà `Can*` nel `TimerCommandResult`.

Criterio di completamento:

```text
Le proprietà Can* sono calcolate correttamente dal TimerEngine e non sono duplicate nel result object.
```

---

### TODO 001.43 — Creare test per lista eventi non cumulativa

Creare test che verifichi:

1. ogni comando restituisce una nuova lista eventi;
2. eventi di un comando precedente non compaiono nel comando successivo;
3. una lista eventi può essere vuota;
4. l'ordine eventi è preservato quando rilevante.

Criterio di completamento:

```text
Events è ordinata, fresca per comando e non cumulativa.
```

---

### TODO 001.44 — Creare test per snapshot del TimerCommandResult

Creare test che verifichi:

1. `TimerCommandResult.State` rappresenta lo stato dopo il comando;
2. `TimerCommandResult.RemainingSeconds` rappresenta il tempo rimanente dopo il comando;
3. `TimerCommandResult.CompletedSessions` rappresenta il contatore dopo il comando;
4. `TimerCommandResult.IsConfigured` rappresenta la configurazione dopo il comando;
5. il result non viene usato come stato persistente autonomo;
6. lo stato corrente resta leggibile dal `TimerEngine`.

Criterio di completamento:

```text
TimerCommandResult contiene uno snapshot coerente del comando corrente senza sostituire il TimerEngine.
```

---

### TODO 001.45 — Verificare tipi public

Creare o verificare test/controlli di compilazione che garantiscano accessibilità pubblica dei tipi principali.

Tipi che devono essere `public`:

```text
TimerState
TimerError
TimerEvent
TimerConfiguration
TimerCommandResult
TimerEngine
```

Criterio di completamento:

```text
I tipi principali del core sono accessibili dal progetto test e saranno accessibili da futuri assembly.
```

---

### TODO 001.46 — Organizzare i test per area funzionale

I test devono essere raggruppati in modo leggibile.

Raggruppamenti consigliati:

```text
configurazione
start / pausa / ripresa / reset
tick
avviso finale
fine sessione e ripartenza automatica
disponibilità comandi
errori
eventi
```

Si possono usare uno o più file.

Non è obbligatorio usare `#region`.

Criterio di completamento:

```text
I test sono leggibili e organizzati per area funzionale.
```

---

## 11. Verifica

### TODO 001.47 — Eseguire build

Cursor deve eseguire:

```text
dotnet build
```

oppure comando equivalente coerente con la struttura reale.

Criterio di completamento:

```text
La build passa senza errori.
```

---

### TODO 001.48 — Eseguire test

Cursor deve eseguire:

```text
dotnet test
```

oppure comando equivalente coerente con la struttura reale.

Criterio di completamento:

```text
Tutti i test passano.
```

---

### TODO 001.49 — Verificare assenza di modifiche fuori perimetro

Cursor deve confermare che non ha modificato o introdotto:

1. UI;
2. XAML;
3. audio;
4. NVDA;
5. API Windows;
6. bridge completo;
7. viewmodel UI;
8. testi utente finali;
9. persistenza;
10. storico;
11. funzioni extra.

Criterio di completamento:

```text
Il lavoro resta limitato a core + test.
```

---

## 12. Report finale richiesto a Cursor

Alla fine Cursor deve restituire un report con:

1. file creati;
2. file modificati;
3. target framework rilevato;
4. target framework usato per core e test;
5. comandi eseguiti;
6. eventuali comandi `dotnet sln add`;
7. risultato di `dotnet build`;
8. risultato di `dotnet test`;
9. numero dei test eseguiti;
10. eventuali test falliti;
11. eventuali punti non implementati;
12. eventuali deviazioni dal TODO;
13. conferma che UI, audio, NVDA e Windows API non sono stati toccati;
14. conferma che gli eventi sono solo nel result object;
15. conferma che `Events` non è cumulativa;
16. conferma che il tick eccedente non viene trasferito alla nuova sessione;
17. conferma che `CanStart`, `CanPause`, `CanResume`, `CanReset` sono proprietà del `TimerEngine`;
18. conferma che `TimerCommandResult` è uno snapshot del comando corrente e non una fonte di verità parallela;
19. conferma che `IsFinalAlertActive` è derivato da `CurrentState == FinalAlert`;
20. conferma che `ValidationFailed` è evento e gli errori specifici stanno in `TimerError`;
21. conferma che i tipi principali sono `public`.

Cursor non deve limitarsi a scrivere:

```text
fatto
```

Il report deve essere verificabile.

---

## 13. Criteri di completamento globale

Il TODO è completato solo se:

1. esiste `CicloTimer.Core`;
2. esiste `CicloTimer.Core.Tests`;
3. i progetti sono nella solution, se esiste una solution;
4. il target framework è coerente con il repository;
5. esistono `TimerState`, `TimerError`, `TimerEvent`, `TimerConfiguration`, `TimerCommandResult`, `TimerEngine`;
6. i tipi principali sono `public`;
7. il core non dipende da UI, audio, NVDA o Windows API;
8. `ConfigureTimer` è implementato;
9. `StartTimer` è implementato;
10. `PauseTimer` è implementato;
11. `ResumeTimer` è implementato;
12. `ResetTimer` è implementato;
13. `Tick` è implementato;
14. `Events` è ordinata, fresca e non cumulativa;
15. `ValidationFailed` è evento e non errore;
16. gli errori specifici sono in `TimerError`;
17. il tempo eccedente nel tick viene scartato;
18. `CanStart`, `CanPause`, `CanResume`, `CanReset` sono proprietà del core;
19. `CanStart`, `CanPause`, `CanResume`, `CanReset` non sono nel result object;
20. `IsFinalAlertActive` è derivato da `CurrentState == FinalAlert`;
21. `TimerCommandResult` contiene uno snapshot coerente del comando corrente;
22. `TimerCommandResult` non sostituisce lo stato del `TimerEngine`;
23. i test coprono i casi richiesti;
24. `dotnet build` passa;
25. `dotnet test` passa;
26. nessuna funzionalità fuori perimetro è stata introdotta.

---

## 14. Criteri di non validità

L'implementazione non è valida se:

1. il core è implementato dentro la UI;
2. il core dipende da WPF;
3. il core produce testi utente;
4. il core formatta il tempo in `mm:ss`;
5. il core riproduce audio;
6. il core chiama API Windows;
7. il tick è esposto come comando utente;
8. la fine sessione è uno stato stabile;
9. la ripartenza automatica è simulata fuori dal core;
10. il reset incrementa il contatore;
11. il reset azzera il contatore;
12. reset da `Stopped` non produce `TimerReset`;
13. resume dentro finestra finale non produce `FinalAlertStarted`;
14. il tick eccedente viene applicato alla nuova sessione;
15. `Events` accumula eventi precedenti;
16. gli eventi vengono duplicati su più canali;
17. le proprietà `Can*` sono nel result object invece che nel core;
18. `IsFinalAlertActive` diventa uno stato parallelo invece che una proprietà derivata;
19. `TimerCommandResult` diventa fonte di verità persistente al posto del `TimerEngine`;
20. `ValidationFailed` viene trattato come errore invece che come evento;
21. gli errori specifici vengono inseriti nella lista eventi;
22. i tipi principali del core non sono `public`;
23. i test richiedono UI;
24. i test richiedono audio;
25. i test richiedono Windows API;
26. `dotnet build` fallisce;
27. `dotnet test` fallisce;
28. vengono introdotte funzionalità fuori perimetro.

---

## 15. Stato del documento

Questo documento è approvato come TODO operativo per il Coding Plan 001 — Core timer engine.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini e chiarimenti su IsFinalAlertActive, TimerCommandResult, tipi public e ValidationFailed
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione delle osservazioni dei consiglieri AI su IsFinalAlertActive come proprietà derivata, TimerCommandResult come snapshot del comando corrente, tipi neutri public, distinzione tra evento ValidationFailed ed errori specifici TimerError, e conferma del mantenimento di State/RemainingSeconds/CompletedSessions/IsConfigured nel result object in coerenza con il Coding Plan 001
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. mantenimento di `IsFinalAlertActive`, con chiarimento che è derivato da `CurrentState == FinalAlert`;
2. mantenimento di `TimerCommandResult` con `Success`, `State`, `RemainingSeconds`, `CompletedSessions`, `IsConfigured`, `Errors`, `Events`;
3. chiarimento che `TimerCommandResult` è uno snapshot del comando corrente e non una seconda fonte di verità;
4. conferma che la fonte primaria dello stato corrente resta `TimerEngine`;
5. conferma che `CanStart`, `CanPause`, `CanResume`, `CanReset` non appartengono al result object;
6. obbligo di dichiarare `public` i tipi principali del core;
7. chiarimento che `ValidationFailed` è un evento neutro;
8. chiarimento che gli errori specifici, come `InvalidSessionDuration`, appartengono a `TimerError`;
9. aggiornamento dei test, dei criteri di completamento, dei criteri di non validità e del report finale richiesto a Cursor.

Il documento è approvato dal project owner come base per la prima esecuzione controllata tramite Cursor del core timer engine.
