\# CicloTimer — Coding Plan 001 — Core timer engine



\*\*Tipo documento:\*\* coding plan

\*\*Stato:\*\* APPROVED

\*\*Versione:\*\* 0.2.0

\*\*Data:\*\* 2026-06-01

\*\*Repository:\*\* donato81/CicloTimer

\*\*Documenti collegati:\*\* docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md



\---



\## 1. Scopo del documento



Questo documento traduce il documento `docs/1-design/001-design-core-timer-engine.md` in un piano operativo di codifica.



Il suo obiettivo è preparare l'implementazione del core timer engine di CicloTimer in modo ordinato, piccolo, verificabile e adatto a essere eseguito da Cursor o da un altro agente di coding.



Il coding plan non modifica il design.



Il coding plan non introduce nuovi requisiti.



Il coding plan non autorizza funzionalità fuori perimetro.



Il coding plan stabilisce:



1\. quali parti creare;

2\. in quale ordine crearle;

3\. quali responsabilità assegnare a ogni parte;

4\. quali test minimi preparare;

5\. quali vincoli rispettare;

6\. quali file o aree evitare;

7\. quali criteri usare per considerare completata l'implementazione.



\---



\## 2. Documento di riferimento principale



Il documento di riferimento principale è:



```text

docs/1-design/001-design-core-timer-engine.md

````



Tutte le decisioni operative devono rispettare quel design.



In caso di dubbio, prevale il design approvato.



In caso di conflitto tra codice esistente e documentazione approvata, prevale la documentazione approvata.



Se durante l'implementazione emerge una necessità non coperta dal design, Cursor non deve inventare una soluzione architetturale autonoma.



In quel caso il lavoro deve fermarsi e il project owner deve valutare se aggiornare il design.



\---



\## 3. Obiettivo operativo



L'obiettivo operativo è creare il primo blocco realmente testabile dell'app:



```text

core timer engine

\\+

tipi neutri minimi

\\+

test automatici del core

```



Il risultato atteso è un motore logico capace di gestire:



1\. configurazione;

2\. validazione;

3\. stati stabili;

4\. comandi;

5\. tick;

6\. ingresso in avviso finale;

7\. fine sessione;

8\. incremento contatore;

9\. ripartenza automatica;

10\. pausa;

11\. ripresa;

12\. reset;

13\. errori neutri;

14\. eventi neutri;

15\. disponibilità dei comandi.



Questo primo ciclo deve poter essere verificato senza UI.



\---



\## 4. Perimetro autorizzato



Questo coding plan autorizza solo il lavoro sul core timer engine e sui test collegati.



Sono autorizzati:



1\. creazione o aggiornamento di una cartella per la logica core;

2\. creazione dei tipi neutri necessari;

3\. creazione del core timer engine;

4\. creazione dei risultati dei comandi;

5\. creazione degli stati stabili;

6\. creazione degli errori neutri;

7\. creazione degli eventi neutri;

8\. creazione della configurazione del timer;

9\. creazione dei test automatici del core;

10\. eventuale aggiornamento minimo del progetto o della solution per includere i test.



Sono autorizzate solo modifiche strettamente necessarie a compilare ed eseguire i test del core.



\---



\## 5. Fuori perimetro



Questo coding plan non autorizza:



1\. UI WPF;

2\. modifiche a `MainWindow.xaml`;

3\. modifiche a `MainWindow.xaml.cs`, salvo eventuali adattamenti minimi già presenti e indispensabili alla compilazione, da evitare se possibile;

4\. layout grafico;

5\. controlli accessibili;

6\. NVDA;

7\. UI Automation;

8\. AutomationProperties;

9\. Live Region;

10\. audio reale;

11\. riproduzione di suoni;

12\. gestione volume;

13\. API Windows;

14\. notifiche;

15\. bridge UI-logica completo;

16\. viewmodel UI completo;

17\. testi utente finali;

18\. localizzazione;

19\. persistenza;

20\. storico sessioni;

21\. numero massimo di sessioni;

22\. timer lavoro/pausa;

23\. funzioni economiche;

24\. database;

25\. cloud;

26\. server locale;

27\. plugin;

28\. funzioni extra non previste dal design.



Cursor non deve “preparare già” UI, audio o bridge per comodità.



Il primo ciclo deve rimanere core + test.



\---



\## 6. Struttura logica consigliata



La struttura fisica definitiva del progetto non è ancora fissata da un documento di design dedicato.



Per questo primo ciclo è comunque utile mantenere una struttura semplice e leggibile.



Struttura consigliata:



```text

src/

\&#x20; CicloTimer.Core/

\&#x20;   Timer/

\&#x20;     TimerEngine.cs

\&#x20;     TimerConfiguration.cs

\&#x20;     TimerState.cs

\&#x20;     TimerCommandResult.cs

\&#x20;     TimerEvent.cs

\&#x20;     TimerError.cs



tests/

\&#x20; CicloTimer.Core.Tests/

\&#x20;   TimerEngineTests.cs

```



Questa struttura è una raccomandazione operativa per il primo ciclo.



Se il repository esistente usa già una struttura diversa, Cursor può adattarsi, ma deve rispettare questi principi:



1\. il core deve restare separato dalla UI;

2\. i test devono restare separati dal codice applicativo;

3\. i tipi neutri devono stare vicino al core;

4\. la UI non deve contenere il motore del timer;

5\. il core non deve dipendere dal progetto WPF.



Se l'attuale progetto non contiene ancora una solution o un progetto test, Cursor può crearli nel modo più semplice compatibile con .NET/C#.



\---



\## 7. Tecnologia test consigliata



Il coding plan autorizza l'uso di un framework di test standard per .NET.



Scelta consigliata:



```text

xUnit

```



Motivo:



1\. è diffuso;

2\. è semplice;

3\. è adatto a test unitari del core;

4\. non richiede UI;

5\. funziona bene con progetti .NET moderni.



Sono accettabili anche MSTest o NUnit se il repository già usa uno di questi.



Cursor non deve introdurre più framework di test.



Deve usarne uno solo.



\---



\## 8. Ordine operativo degli interventi



L'implementazione deve procedere in piccoli passi.



Ordine consigliato:



```text

1\\. Verificare struttura attuale del repository

2\\. Identificare target framework e solution esistente

3\\. Preparare progetto core separato o area core separata

4\\. Preparare progetto test

5\\. Aggiungere i progetti alla solution, se esiste una solution

6\\. Definire tipi neutri

7\\. Definire configurazione

8\\. Definire risultato comandi

9\\. Implementare TimerEngine

10\\. Implementare validazione configurazione

11\\. Implementare Start/Pause/Resume/Reset

12\\. Implementare Tick

13\\. Implementare avviso finale

14\\. Implementare fine sessione e ripartenza automatica

15\\. Implementare disponibilità dei comandi

16\\. Scrivere test unitari

17\\. Eseguire test

18\\. Correggere solo difetti nel perimetro

```



Cursor non deve saltare direttamente a una soluzione completa non testata.



Ogni blocco deve essere semplice e verificabile.



\---



\## 9. Passo 1 — Verifica struttura repository



Prima di modificare il codice, Cursor deve osservare la struttura reale del repository.



Deve identificare:



1\. file `.sln`, se presente;

2\. file `.csproj`, se presenti;

3\. progetto WPF esistente;

4\. versione target di .NET;

5\. eventuali cartelle già presenti per `src` o `tests`;

6\. eventuale codice già scritto nel core o nella UI.



Questa verifica serve solo a scegliere dove inserire i file.



Non autorizza modifiche architetturali.



\---



\## 10. Passo 2 — Identificare target framework e solution



Prima di creare nuovi progetti, Cursor deve leggere il file `.csproj` del progetto esistente.



Deve identificare il target framework usato dal progetto WPF o dal progetto principale.



Esempio:



```text

net8.0-windows

net9.0-windows

```



I nuovi progetti devono usare target framework compatibili con il progetto esistente.



Regola consigliata:



```text

se il progetto WPF usa net8.0-windows,

il progetto core può usare net8.0 se non richiede Windows,

e il progetto test deve usare un target compatibile.

```



Il core non deve usare un target `-windows` se non è necessario.



Il core deve restare il più indipendente possibile da Windows.



Cursor non deve scegliere una versione di .NET casuale o diversa da quella già usata nel repository senza motivo esplicito.



Se esiste una solution `.sln`, Cursor deve aggiungere alla solution i nuovi progetti creati.



Esempi di comandi, da adattare ai percorsi reali del repository:



```text

dotnet sln add src/CicloTimer.Core/CicloTimer.Core.csproj

dotnet sln add tests/CicloTimer.Core.Tests/CicloTimer.Core.Tests.csproj

```



Se non esiste una solution, Cursor può crearne una minima solo se necessario per rendere eseguibili `dotnet build` e `dotnet test` dalla radice del repository.



\---



\## 11. Passo 3 — Preparare il progetto core



Il core deve essere separato dalla UI.



Soluzione consigliata:



```text

creare un progetto class library per CicloTimer.Core

```



Nome consigliato:



```text

CicloTimer.Core

```



Il progetto core non deve dipendere dal progetto WPF.



Il progetto core non deve referenziare:



1\. WPF;

2\. Windows Forms;

3\. PresentationFramework;

4\. UI Automation;

5\. librerie audio;

6\. API Windows specifiche;

7\. pacchetti per notifiche;

8\. framework UI.



Il progetto core può dipendere solo da librerie .NET standard necessarie.



Se il repository è ancora molto piccolo e non ha una solution strutturata, Cursor può creare una solution minima.



Obiettivo:



```text

CicloTimer.Core deve poter compilare da solo

```



\---



\## 12. Passo 4 — Preparare il progetto test



Deve essere creato un progetto di test separato.



Nome consigliato:



```text

CicloTimer.Core.Tests

```



Il progetto test deve referenziare:



```text

CicloTimer.Core

```



Il progetto test non deve referenziare il progetto UI WPF.



I test devono poter essere eseguiti con:



```text

dotnet test

```



o comando equivalente coerente con la struttura reale della solution.



Se esiste una solution `.sln`, il progetto test deve essere aggiunto alla solution.



Se Cursor crea il progetto test con `dotnet new xunit`, deve poi assicurarsi che:



1\. il progetto test referenzi `CicloTimer.Core`;

2\. il progetto test sia incluso nella solution, se esiste;

3\. `dotnet test` dalla radice trovi ed esegua i test.



\---



\## 13. Passo 5 — Definire gli stati stabili



Creare un tipo neutro per gli stati stabili.



Nome concettuale:



```text

TimerState

```



Valori richiesti:



```text

Stopped

Running

FinalAlert

Paused

```



Regole:



1\. non creare `Completed`;

2\. non creare `Finished`;

3\. non creare `SessionEnded`;

4\. la fine sessione deve restare evento, non stato stabile.



Il tipo può essere un enum C#.



\---



\## 14. Passo 6 — Definire gli errori neutri



Creare un tipo neutro per gli errori logici prevedibili.



Nome concettuale:



```text

TimerError

```



Valori richiesti:



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



Il tipo può essere un enum C#.



Questi errori non sono messaggi utente.



Non devono contenere testo italiano finale.



\---



\## 15. Passo 7 — Definire gli eventi neutri



Creare un tipo neutro per gli eventi del timer.



Nome concettuale:



```text

TimerEvent

```



Valori richiesti:



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



Il tipo può essere un enum C# o una struttura semplice se serve associare dati.



Per la prima implementazione è preferibile partire semplice.



Opzione consigliata:



```text

enum TimerEvent

```



oppure:



```text

enum TimerEventType

```



Per il primo ciclo è sufficiente una lista ordinata di enum.



I dati dinamici collegati agli eventi, come `CompletedSessions`, possono essere ricavati dallo stato aggiornato del core o dal result object.



Non introdurre ora:



1\. event bus;

2\. observer;

3\. payload evento complessi;

4\. eventi asincroni;

5\. dispatcher di eventi.



Regola obbligatoria:



```text

uno stesso evento non deve essere duplicato su più canali

```



\---



\## 16. Passo 8 — Definire la configurazione



Creare un tipo per la configurazione.



Nome concettuale:



```text

TimerConfiguration

```



Contenuto minimo:



```text

SessionDurationSeconds

FinalAlertDurationSeconds

```



Regole:



1\. entrambi i valori sono interi;

2\. `SessionDurationSeconds > 0`;

3\. `FinalAlertDurationSeconds >= 0`;

4\. `FinalAlertDurationSeconds < SessionDurationSeconds`.



La configurazione non deve contenere:



1\. minuti e secondi separati della UI;

2\. testi utente;

3\. formato `mm:ss`;

4\. dati audio;

5\. impostazioni NVDA;

6\. dati di persistenza.



\---



\## 17. Passo 9 — Definire il risultato dei comandi



Creare un tipo per il risultato dei comandi.



Nome concettuale:



```text

TimerCommandResult

```



Contenuto consigliato:



```text

Success

State

RemainingSeconds

CompletedSessions

IsConfigured

Errors

Events

```



Dove:



```text

Errors = lista di TimerError

Events = lista ordinata di TimerEvent

```



Questa scelta è ammessa per il primo ciclo perché semplice e testabile.



La lista `Events` nel `TimerCommandResult` contiene solo gli eventi generati dal comando corrente.



Non deve accumulare eventi di comandi precedenti.



Ogni comando deve produrre un nuovo result object con una nuova lista eventi.



La lista eventi può essere vuota se il comando non produce eventi.



La lista eventi deve preservare rigorosamente l'ordine cronologico di generazione.



Esempio:



```text

ResumeTimer dentro la finestra finale:

Events = \\\[TimerResumed, FinalAlertStarted]

```



Esempio:



```text

fine sessione:

Events = \\\[SessionCompleted, SessionCounterIncremented, NextSessionStarted]

```



I test unitari devono verificare l'ordine degli eventi quando l'ordine è rilevante.



Regola obbligatoria:



```text

se gli eventi sono nel result object, non devono essere anche notificati tramite un secondo canale

```



Il result object non deve contenere:



1\. messaggi utente;

2\. testi accessibili;

3\. stringhe UI;

4\. codici Windows;

5\. oggetti WPF;

6\. riferimenti audio;

7\. proprietà `CanStart`, `CanPause`, `CanResume`, `CanReset`.



Le proprietà `CanStart`, `CanPause`, `CanResume`, `CanReset` appartengono al `TimerEngine` come proprietà pubbliche di sola lettura calcolate dinamicamente.



\---



\## 18. Passo 10 — Implementare TimerEngine



Creare il motore core.



Nome concettuale:



```text

TimerEngine

```



Responsabilità:



1\. mantenere configurazione valida;

2\. mantenere stato corrente;

3\. mantenere tempo rimanente;

4\. mantenere contatore sessioni completate;

5\. esporre dati dinamici;

6\. esporre disponibilità dei comandi;

7\. gestire comandi;

8\. produrre risultati controllati;

9\. produrre eventi neutri nel result object;

10\. produrre errori neutri.



Il motore deve iniziare con:



```text

CurrentState = Stopped

CompletedSessions = 0

IsConfigured = false

```



Se l'implementazione sceglie di applicare una configurazione predefinita fuori dal core, questa scelta deve avvenire fuori dal core.



Il core non deve inventare valori predefiniti UI.



Il `TimerEngine` deve esporre proprietà pubbliche di sola lettura calcolate dinamicamente:



```text

CanStart

CanPause

CanResume

CanReset

```



Regole minime:



```text

CanStart = IsConfigured \\\&\\\& CurrentState == Stopped

CanPause = CurrentState == Running || CurrentState == FinalAlert

CanResume = CurrentState == Paused

CanReset = IsConfigured

```



Queste proprietà non devono essere incluse nel `TimerCommandResult`.



Devono essere leggibili separatamente dal bridge senza eseguire comandi fittizi.



\---



\## 19. Passo 11 — Implementare ConfigureTimer



Metodo concettuale:



```text

ConfigureTimer(TimerConfiguration configuration)

```



Comportamento con configurazione valida:



```text

IsConfigured = true

CurrentState = Stopped

RemainingSeconds = SessionDurationSeconds

CompletedSessions invariato

Events = \\\[TimerConfigured]

Errors = \\\[]

Success = true

```



Comportamento con configurazione non valida:



```text

configurazione precedente invariata

CurrentState invariato

RemainingSeconds invariato

CompletedSessions invariato

Success = false

Errors = errore o errori neutri pertinenti

Events = \\\[ValidationFailed]

```



Errori possibili:



```text

InvalidSessionDuration

InvalidFinalAlertDuration

FinalAlertNotLessThanSessionDuration

```



La configurazione non valida non deve lasciare il core in stato incoerente.



\---



\## 20. Passo 12 — Implementare StartTimer



Metodo concettuale:



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

CurrentState = Running

RemainingSeconds = SessionDurationSeconds

CompletedSessions invariato

Events = \\\[TimerStarted]

Errors = \\\[]

Success = true

```



Con una configurazione valida, lo start non deve partire direttamente in `FinalAlert`, perché:



```text

FinalAlertDurationSeconds < SessionDurationSeconds

```



Comportamento non valido:



```text

Success = false

Errors = \\\[TimerNotConfigured] oppure \\\[CannotStart]

Events = \\\[ValidationFailed] oppure \\\[]

```



La scelta deve essere coerente nei test.



\---



\## 21. Passo 13 — Implementare PauseTimer



Metodo concettuale:



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

CurrentState = Paused

RemainingSeconds invariato

CompletedSessions invariato

Events = \\\[TimerPaused]

Success = true

```



Comportamento non valido:



```text

Success = false

Errors = \\\[CannotPause]

Events = \\\[ValidationFailed] oppure \\\[]

```



La scelta deve essere coerente nei test.



La pausa non deve:



1\. completare la sessione;

2\. incrementare il contatore;

3\. resettare il tempo;

4\. produrre `SessionCompleted`;

5\. produrre `NextSessionStarted`.



\---



\## 22. Passo 14 — Implementare ResumeTimer



Metodo concettuale:



```text

ResumeTimer()

```



Ammesso quando:



```text

CurrentState = Paused

```



Se il tempo rimanente è fuori dalla finestra finale:



```text

RemainingSeconds > FinalAlertDurationSeconds

```



oppure:



```text

FinalAlertDurationSeconds = 0

```



comportamento:



```text

CurrentState = Running

Events = \\\[TimerResumed]

Success = true

```



Se il tempo rimanente è dentro la finestra finale:



```text

FinalAlertDurationSeconds > 0

RemainingSeconds <= FinalAlertDurationSeconds

RemainingSeconds > 0

```



comportamento obbligatorio:



```text

CurrentState = FinalAlert

Events = \\\[TimerResumed, FinalAlertStarted]

Success = true

```



L'ordine degli eventi è obbligatorio:



```text

TimerResumed

↓

FinalAlertStarted

```



Il core deve produrre `FinalAlertStarted` in questo caso.



Questo evento serve al bridge per riattivare l'avviso finale.



Comportamento non valido:



```text

Success = false

Errors = \\\[CannotResume]

Events = \\\[ValidationFailed] oppure \\\[]

```



La scelta deve essere coerente nei test.



\---



\## 23. Passo 15 — Implementare ResetTimer



Metodo concettuale:



```text

ResetTimer()

```



Ammesso quando:



```text

IsConfigured = true

```



Comportamento valido da qualunque stato configurato:



```text

CurrentState = Stopped

RemainingSeconds = SessionDurationSeconds

CompletedSessions invariato

Events = \\\[TimerReset]

Success = true

```



Questo vale anche se il timer è già:



```text

Stopped

```



Reset da `Stopped` deve produrre:



```text

TimerReset

```



Non deve produrre:



```text

SessionCompleted

SessionCounterIncremented

NextSessionStarted

```



Reset non deve:



1\. completare la sessione;

2\. incrementare il contatore;

3\. azzerare il contatore;

4\. riavviare il ciclo.



Comportamento non valido:



```text

Success = false

Errors = \\\[TimerNotConfigured] oppure \\\[CannotReset]

Events = \\\[ValidationFailed] oppure \\\[]

```



La scelta deve essere coerente nei test.



\---



\## 24. Passo 16 — Implementare Tick



Metodo concettuale:



```text

Tick(int elapsedSeconds)

```



Regola input:



```text

elapsedSeconds >= 1

```



Se `elapsedSeconds <= 0`:



```text

Success = false

Errors = \\\[InvalidTickDuration]

nessuna modifica allo stato

nessun evento di ciclo

```



Se il timer è `Stopped` o `Paused`:



```text

Success = true

nessuna modifica a RemainingSeconds

nessuna modifica a CompletedSessions

nessun evento di ciclo

nessun errore

nessuna eccezione

```



Se il timer è `Running` o `FinalAlert`, il tick deve ridurre `RemainingSeconds` senza farlo diventare negativo.



Regola logica:



```text

RemainingSeconds = max(0, RemainingSeconds - elapsedSeconds)

```



Il tempo eccedente non viene trasferito alla sessione successiva.



Esempio:



```text

RemainingSeconds = 3

elapsedSeconds = 5

SessionDurationSeconds = 300

```



Risultato corretto:



```text

la sessione corrente termina

CompletedSessions aumenta di 1

parte una nuova sessione

RemainingSeconds = 300

```



Risultato scorretto:



```text

RemainingSeconds = 298

```



L'eccedenza di 2 secondi viene scartata.



Questa scelta mantiene il core semplice, prevedibile e coerente con la prima versione minima.



Dopo la riduzione:



```text

se RemainingSeconds == 0:

\&#x20;   completa sessione

altrimenti se entra nella finestra finale:

\&#x20;   passa a FinalAlert e produce FinalAlertStarted

altrimenti:

\&#x20;   resta Running

```



Il tick non deve produrre annunci accessibili.



Il tick non deve produrre testi utente.



\---



\## 25. Passo 17 — Implementare ingresso in FinalAlert



Il core entra in `FinalAlert` quando:



```text

FinalAlertDurationSeconds > 0

RemainingSeconds <= FinalAlertDurationSeconds

RemainingSeconds > 0

```



La transizione deve produrre:



```text

FinalAlertStarted

```



solo una volta per sessione.



Non deve produrre `FinalAlertStarted` a ogni tick successivo.



Se `FinalAlertDurationSeconds = 0`:



```text

non esiste stato FinalAlert

non viene prodotto FinalAlertStarted

```



Se un tick porta direttamente a zero, il core deve completare la sessione senza produrre obbligatoriamente `FinalAlertStarted`.



\---



\## 26. Passo 18 — Implementare fine sessione e ripartenza automatica



Quando `RemainingSeconds` arriva a zero, il core deve:



1\. produrre `SessionCompleted`;

2\. incrementare `CompletedSessions` di 1;

3\. produrre `SessionCounterIncremented`;

4\. avviare internamente una nuova sessione;

5\. produrre `NextSessionStarted`;

6\. impostare `RemainingSeconds = SessionDurationSeconds`;

7\. impostare immediatamente `CurrentState = Running`.



L'ordine degli eventi è obbligatorio:



```text

SessionCompleted

↓

SessionCounterIncremented

↓

NextSessionStarted

```



La ripartenza automatica deve essere atomica.



Non deve attendere un tick successivo per rendere coerente lo stato.



Dopo la ripartenza, devono essere coerenti:



```text

CurrentState = Running

RemainingSeconds = SessionDurationSeconds

IsFinalAlertActive = false

CanPause = true

CanReset = true

```



La ripartenza automatica non deve:



1\. fermare il timer;

2\. chiedere intervento utente;

3\. azzerare il contatore;

4\. essere simulata dal bridge;

5\. produrre stato stabile `Completed`.



\---



\## 27. Passo 19 — Implementare disponibilità comandi



Il core deve esporre la disponibilità logica dei comandi.



Le proprietà devono stare sul `TimerEngine`.



Devono essere pubbliche, di sola lettura e calcolate dinamicamente dallo stato corrente.



Regole minime:



```text

CanStart = IsConfigured \\\&\\\& CurrentState == Stopped

CanPause = CurrentState == Running || CurrentState == FinalAlert

CanResume = CurrentState == Paused

CanReset = IsConfigured

```



Queste proprietà non devono essere duplicate nel `TimerCommandResult`.



La UI non deve calcolarle autonomamente.



Il bridge potrà leggerle dal core senza eseguire comandi fittizi.



\---



\## 28. Passo 20 — Scrivere test unitari



I test unitari devono coprire il comportamento previsto dal design.



Il progetto test deve verificare almeno:



1\. stato iniziale;

2\. configurazione valida;

3\. configurazione con durata sessione zero;

4\. configurazione con avviso finale negativo;

5\. configurazione con avviso finale uguale alla durata sessione;

6\. configurazione con avviso finale maggiore della durata sessione;

7\. configurazione non valida dopo configurazione valida;

8\. avvio senza configurazione;

9\. avvio con configurazione valida;

10\. pausa da Running;

11\. pausa da FinalAlert;

12\. pausa da stato non valido;

13\. ripresa da Paused fuori finestra finale;

14\. ripresa da Paused dentro finestra finale;

15\. produzione ordinata di `TimerResumed` e `FinalAlertStarted` dopo ripresa in finestra finale;

16\. reset da Running;

17\. reset da FinalAlert;

18\. reset da Paused;

19\. reset da Stopped;

20\. produzione di `TimerReset` da Stopped;

21\. tick da Running;

22\. tick da FinalAlert;

23\. tick da Paused senza effetti collaterali;

24\. tick da Stopped senza effetti collaterali;

25\. tick con durata zero;

26\. tick con durata negativa;

27\. tick maggiore del tempo rimanente;

28\. scarto del tempo eccedente nel tick;

29\. ingresso singolo in FinalAlert;

30\. assenza di FinalAlert se avviso finale è zero;

31\. completamento sessione;

32\. incremento contatore;

33\. mancato incremento dopo reset;

34\. ripartenza automatica;

35\. stato immediatamente `Running` dopo ripartenza automatica;

36\. ordine eventi su completamento sessione;

37\. disponibilità logica dei comandi;

38\. produzione di eventi neutri;

39\. produzione di errori neutri;

40\. assenza di duplicazione degli eventi;

41\. lista eventi fresca per ogni comando e non cumulativa.



Non tutti i test devono essere giganteschi.



È preferibile avere test piccoli, leggibili e focalizzati.



I test devono essere raggruppati per area funzionale.



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



I test possono stare in un unico file o in più file, purché restino leggibili.



Non è obbligatorio usare `#region`.



\---



\## 29. Strategia minima dei test



I test devono essere scritti in modo leggibile.



Ogni test dovrebbe seguire lo schema:



```text

Arrange

Act

Assert

```



Oppure equivalente.



Esempio concettuale:



```text

Dato un timer configurato con sessione 300 e avviso 20

Quando viene avviato

Allora lo stato è Running

E RemainingSeconds è 300

E viene prodotto TimerStarted

```



I test devono verificare dati neutri, non testi utente.



Esempio corretto:



```text

Assert State == Running

Assert RemainingSeconds == 300

Assert Events contains TimerStarted

```



Esempio scorretto:



```text

Assert testo == "Sessione in corso"

```



Il testo utente non appartiene al core.



Quando l'ordine degli eventi è rilevante, i test devono verificarlo.



Esempio:



```text

ResumeTimer dentro la finestra finale

↓

Events\\\[0] = TimerResumed

Events\\\[1] = FinalAlertStarted

```



Altro esempio:



```text

Tick completa sessione

↓

Events\\\[0] = SessionCompleted

Events\\\[1] = SessionCounterIncremented

Events\\\[2] = NextSessionStarted

```



\---



\## 30. Comandi di verifica attesi



Alla fine del lavoro Cursor deve indicare quali comandi sono stati eseguiti.



Comandi attesi, se compatibili con la struttura del progetto:



```text

dotnet build

dotnet test

```



Se esiste una solution nella radice, questi comandi dovrebbero funzionare dalla radice del repository.



Se la solution o i progetti richiedono comandi diversi, Cursor deve indicarlo chiaramente.



L'implementazione non è considerata completa se i test non vengono eseguiti o se falliscono.



Se alcuni test non possono essere eseguiti per limiti dell'ambiente, Cursor deve dichiararlo esplicitamente.



\---



\## 31. Regole sul codice



Il codice prodotto deve essere semplice.



Sono da preferire:



1\. enum per stati, errori ed eventi;

2\. classi o record semplici per configurazione e result;

3\. metodi chiari per i comandi;

4\. proprietà leggibili per stato e dati dinamici;

5\. proprietà `CanStart`, `CanPause`, `CanResume`, `CanReset` calcolate dinamicamente;

6\. test unitari diretti.



Sono da evitare:



1\. event bus complesso;

2\. dependency injection container;

3\. pattern architetturali pesanti;

4\. thread;

5\. timer reali nel core;

6\. async non necessario;

7\. librerie esterne non necessarie;

8\. reflection;

9\. codice generico eccessivo;

10\. classi astratte inutili;

11\. interfacce premature;

12\. overengineering.



Il core deve essere comprensibile leggendo pochi file.



\---



\## 32. Gestione degli eventi nel primo ciclo



Per il primo ciclo operativo, la soluzione più semplice e consigliata è:



```text

gli eventi prodotti da un comando sono restituiti nel TimerCommandResult

```



Questa scelta è coerente purché:



```text

non esista un secondo canale di notifica degli stessi eventi

```



Quindi, in questo primo ciclo, Cursor non deve creare:



1\. eventi C# pubblici sul motore;

2\. observer;

3\. event bus;

4\. notifiche asincrone;

5\. dispatcher di eventi.



Questi meccanismi potranno essere valutati solo se un design successivo li richiederà.



Il result object è sufficiente per testare il core.



La lista eventi del result object deve essere:



```text

ordinata

non cumulativa

relativa solo al comando corrente

nuova per ogni comando

vuota se nessun evento viene prodotto

```



\---



\## 33. Gestione degli errori nel primo ciclo



Gli errori logici prevedibili devono essere restituiti nel result object.



Non devono essere lanciati come eccezioni non controllate.



Esempio corretto:



```text

StartTimer senza configurazione

↓

Success = false

Errors = \\\[TimerNotConfigured]

```



Esempio scorretto:



```text

StartTimer senza configurazione

↓

throw InvalidOperationException

```



Eccezioni tecniche possono essere usate solo per errori di programmazione non previsti, ma non devono sostituire gli errori neutri definiti dal design.



\---



\## 34. Vincoli sulla UI esistente



Se il repository contiene già file WPF iniziali, Cursor non deve usarli per implementare il core.



File come:



```text

App.xaml

MainWindow.xaml

MainWindow.xaml.cs

```



non devono diventare sede della logica del timer.



In questo ciclo, la UI può restare non collegata al core.



È accettabile che l'app grafica non mostri ancora il comportamento del timer.



Il successo di questo coding plan si misura sui test del core, non sull'interfaccia.



\---



\## 35. Criteri di completamento



Il coding plan è completato quando:



1\. il core timer engine esiste;

2\. il core è separato dalla UI;

3\. i tipi neutri minimi esistono;

4\. configurazione, stati, eventi, errori e result sono implementati;

5\. `TimerCommandResult.Events` è una lista ordinata, fresca e non cumulativa;

6\. `ConfigureTimer` funziona;

7\. `StartTimer` funziona;

8\. `PauseTimer` funziona;

9\. `ResumeTimer` funziona;

10\. `ResetTimer` funziona;

11\. `Tick` funziona;

12\. avviso finale zero è gestito;

13\. ingresso in `FinalAlert` è gestito;

14\. `FinalAlertStarted` viene prodotto una sola volta per sessione;

15\. `FinalAlertStarted` viene prodotto anche dopo ripresa in finestra finale;

16\. fine sessione è gestita come evento;

17\. contatore viene incrementato solo su sessione completata;

18\. reset non incrementa e non azzera il contatore;

19\. reset da `Stopped` produce `TimerReset`;

20\. ripartenza automatica è gestita dal core;

21\. ripartenza automatica lascia subito lo stato `Running`;

22\. tick in `Paused` e `Stopped` non produce effetti collaterali;

23\. tick con durata non valida produce `InvalidTickDuration`;

24\. tick eccedente non trasferisce il tempo residuo alla sessione successiva;

25\. disponibilità comandi è esposta come proprietà pubbliche di sola lettura del `TimerEngine`;

26\. disponibilità comandi non è duplicata nel result object;

27\. test unitari coprono i casi principali;

28\. i test verificano l'ordine degli eventi dove rilevante;

29\. i progetti sono aggiunti alla solution se esiste una solution;

30\. il target framework dei nuovi progetti è coerente con il repository;

31\. `dotnet build` passa;

32\. `dotnet test` passa;

33\. nessun file UI è stato usato per contenere logica core;

34\. non sono state introdotte funzionalità fuori perimetro.



\---



\## 36. Criteri di non validità



L'implementazione non è valida se:



1\. il core dipende dalla UI;

2\. il core dipende da WPF;

3\. il core riproduce audio;

4\. il core contiene testi utente finali;

5\. il tempo viene formattato in `mm:ss` dentro il core;

6\. il tick è esposto come comando utente;

7\. la fine sessione è uno stato stabile permanente;

8\. la ripartenza automatica è demandata al bridge;

9\. il contatore viene incrementato fuori dal core;

10\. ResetTimer azzera il contatore;

11\. ResetTimer da Stopped non produce TimerReset;

12\. ResumeTimer dentro la finestra finale non produce FinalAlertStarted;

13\. la ripartenza automatica non lascia immediatamente lo stato Running;

14\. Tick in Paused o Stopped produce effetti collaterali;

15\. ElapsedSeconds pari a zero o negativo viene accettato come valido;

16\. il tick eccedente viene trasferito alla nuova sessione;

17\. errori logici prevedibili vengono gestiti tramite eccezioni invece che tramite errori neutri;

18\. eventi vengono duplicati su più canali;

19\. la lista eventi accumula eventi di comandi precedenti;

20\. l'ordine degli eventi non è deterministico o non verificabile;

21\. le proprietà `CanStart`, `CanPause`, `CanResume`, `CanReset` sono duplicate nel result object invece di essere esposte dal core;

22\. i nuovi progetti non sono aggiunti alla solution esistente;

23\. i nuovi progetti usano un target framework incoerente senza motivo;

24\. test richiedono UI, audio o Windows API;

25\. sono introdotte funzionalità fuori perimetro;

26\. `dotnet test` fallisce.



\---



\## 37. Output atteso da Cursor



Quando Cursor completa il lavoro, deve restituire un riepilogo che includa:



1\. file creati;

2\. file modificati;

3\. struttura del progetto risultante;

4\. target framework rilevato nel progetto esistente;

5\. target framework usato per i nuovi progetti;

6\. eventuali comandi `dotnet sln add` eseguiti;

7\. comandi di build/test eseguiti;

8\. risultato di `dotnet build`;

9\. risultato di `dotnet test`;

10\. eventuali test falliti;

11\. eventuali punti non implementati;

12\. eventuali deviazioni dal coding plan;

13\. conferma che UI, audio, NVDA e Windows API non sono stati toccati;

14\. conferma che i progetti sono nella solution, se esiste una solution;

15\. conferma che gli eventi sono restituiti solo nel result object e non duplicati su altri canali.



Cursor non deve limitarsi a dire “fatto”.



Deve fornire un report verificabile.



\---



\## 38. Stato del documento



Questo documento è approvato come coding plan del Design 001 — Core timer engine.



Versione corrente:



```text

0.2.0 — approvazione dopo revisione DeepSeek/Gemini e chiarimenti su Events, tick eccedente, proprietà Can\\\*, solution .NET, target framework e organizzazione test

```



Cronologia:



```text

0.1.0 — prima bozza ChatGPT

0.2.0 — integrazione delle osservazioni dei consiglieri AI su lista eventi ordinata e non cumulativa, scarto del tempo eccedente nel tick, esposizione di CanStart/CanPause/CanResume/CanReset come proprietà del TimerEngine, aggiunta dei progetti alla solution, target framework coerente e test raggruppati per area funzionale

```



Il documento è stato revisionato dal consiglio AI formato da:



```text

ChatGPT

DeepSeek

Gemini

```



Le osservazioni integrate sono:



1\. chiarimento che `TimerCommandResult.Events` contiene solo gli eventi del comando corrente;

2\. chiarimento che `Events` non è uno storico cumulativo;

3\. chiarimento che `Events` deve preservare l'ordine cronologico di generazione;

4\. obbligo di verificare l'ordine degli eventi nei test quando rilevante;

5\. chiarimento che il tempo eccedente nel tick non viene trasferito alla nuova sessione;

6\. chiarimento che `CanStart`, `CanPause`, `CanResume`, `CanReset` sono proprietà pubbliche di sola lettura del `TimerEngine`;

7\. chiarimento che le proprietà `Can\\\*` non devono essere incluse nel `TimerCommandResult`;

8\. istruzione di aggiungere i nuovi progetti alla solution `.sln`, se esiste;

9\. istruzione di usare target framework coerente con il progetto esistente;

10\. raccomandazione di raggruppare i test per area funzionale senza imporre una struttura eccessiva;

11\. aggiornamento dei criteri di completamento, non validità e output atteso da Cursor.



Il documento è approvato dal project owner come base per il todo operativo e per la successiva implementazione controllata del core timer engine tramite Cursor.



