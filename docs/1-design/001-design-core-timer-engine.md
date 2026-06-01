# CicloTimer — Design 001 — Core timer engine

**Tipo documento:** documento di design  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-01  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md  

---

## 1. Scopo del documento

Questo documento definisce il design tecnico del motore core del timer di CicloTimer.

Il motore core è la parte dell'applicazione che contiene le regole essenziali del timer ciclico.

Il suo compito è gestire:

1. configurazione della durata della sessione;
2. configurazione della durata dell'avviso finale;
3. validazione logica della configurazione;
4. stati stabili del timer;
5. avanzamento del tempo;
6. ingresso nella finestra finale di avviso;
7. fine sessione;
8. incremento del contatore delle sessioni completate;
9. ripartenza automatica della sessione successiva;
10. pausa;
11. ripresa;
12. reset;
13. errori neutri;
14. eventi neutri;
15. dati dinamici di stato.

Questo documento è il primo design tecnico della cartella `docs/1-design/`.

Il documento non definisce ancora:

1. UI WPF;
2. layout XAML;
3. controlli accessibili;
4. annunci NVDA;
5. riproduzione audio concreta;
6. gestione del volume di altre applicazioni;
7. testi finali per l'utente;
8. modello mostrabile completo del bridge;
9. classi definitive del livello UI;
10. coding plan operativo;
11. todo eseguibile da Cursor.

Questi aspetti saranno trattati in documenti successivi.

---

## 2. Obiettivo del design

L'obiettivo è progettare un core timer engine semplice, isolato e testabile.

Il core deve poter funzionare senza:

1. aprire finestre;
2. usare WPF;
3. conoscere NVDA;
4. riprodurre suoni;
5. chiamare API Windows;
6. generare testi utente finali;
7. leggere input direttamente dalla UI;
8. formattare il tempo in stringhe visuali;
9. gestire notifiche;
10. dipendere da thread UI.

Il core deve ricevere comandi e valori neutri.

Il core deve restituire o esporre stati, dati, errori ed eventi neutri.

Il core deve essere testabile con test automatici puramente logici.

---

## 3. Perimetro del design

Questo design può definire:

1. responsabilità del core timer engine;
2. modello concettuale dei dati core;
3. comandi supportati dal core;
4. stati stabili;
5. transizioni tra stati;
6. validazione logica;
7. comportamento del tick;
8. comportamento dell'avviso finale;
9. comportamento della fine sessione;
10. comportamento della ripartenza automatica;
11. comportamento di pausa, ripresa e reset;
12. errori neutri;
13. eventi neutri;
14. criteri minimi di testabilità;
15. vincoli per la futura implementazione C#.

Questo design non può definire:

1. aspetto grafico della finestra;
2. ordine di tabulazione;
3. AutomationProperties;
4. Live Region;
5. annunci screen reader;
6. file audio;
7. libreria audio;
8. API Windows da usare;
9. struttura definitiva delle cartelle;
10. namespace definitivi;
11. nomi definitivi di tutte le classi;
12. strategia completa di localizzazione;
13. gestione avanzata dell'audio di sistema;
14. persistenza del contatore;
15. storico sessioni;
16. massimo numero di sessioni;
17. funzioni economiche, clienti, tariffe o fatturazione.

---

## 4. Principio guida del core

Il principio guida è:

```text
il core conosce solo il timer
````

Il core deve conoscere:

```text
durata sessione
durata avviso finale
tempo rimanente
stato corrente
sessioni completate
comandi ricevuti
transizioni logiche
eventi neutri
errori neutri
```

Il core non deve conoscere:

```text
pulsanti
etichette
finestre
screen reader
NVDA
UI Automation
suoni
file audio
volume
notifiche
testi finali
formattazione mm:ss
```

Il core deve restare piccolo.

La semplicità è un requisito tecnico.

---

## 5. Unità di tempo interna

Il core deve rappresentare le durate in secondi interi.

Unità interna prevista:

```text
secondi
```

Dati minimi:

```text
SessionDurationSeconds
FinalAlertDurationSeconds
RemainingSeconds
```

Regole:

```text
SessionDurationSeconds > 0
FinalAlertDurationSeconds >= 0
FinalAlertDurationSeconds < SessionDurationSeconds
RemainingSeconds >= 0
```

La UI potrà raccogliere minuti e secondi separati.

Il bridge convertirà minuti e secondi in secondi totali.

Il core non deve ricevere minuti e secondi come campi UI separati.

Esempio:

```text
Input UI:
5 minuti
0 secondi

Valore ricevuto dal core:
SessionDurationSeconds = 300
```

Il core non formatta `300` in `05:00`.

La formattazione appartiene al bridge.

---

## 6. Configurazione del timer

Il core deve ricevere una configurazione neutra.

Forma concettuale:

```text
TimerConfiguration
```

Contenuto minimo:

```text
SessionDurationSeconds
FinalAlertDurationSeconds
```

La configurazione è valida solo se:

```text
SessionDurationSeconds > 0
FinalAlertDurationSeconds >= 0
FinalAlertDurationSeconds < SessionDurationSeconds
```

La durata dell'avviso finale può essere zero.

Il valore zero significa:

```text
avviso sonoro finale disattivato
```

Se la configurazione è valida, il core deve accettarla.

Se la configurazione non è valida, il core deve produrre un errore neutro.

Errori previsti:

```text
InvalidSessionDuration
InvalidFinalAlertDuration
FinalAlertNotLessThanSessionDuration
```

Il comando di configurazione non deve produrre messaggi utente finali.

Esempio corretto:

```text
Errore core:
InvalidSessionDuration
```

Esempio scorretto:

```text
La durata della sessione deve essere maggiore di zero.
```

---

## 7. Stato iniziale

Dopo la creazione del core timer engine, lo stato iniziale deve essere:

```text
CurrentState = Stopped
CompletedSessions = 0
```

Per il tempo rimanente ci sono due possibilità:

1. se esiste una configurazione predefinita già applicata, `RemainingSeconds` corrisponde alla durata sessione configurata;
2. se non esiste ancora configurazione valida, il core risulta non configurato.

La scelta pratica dipenderà dal design successivo di bootstrap dell'app.

Per questo design valgono i principi:

```text
il core non inventa valori di default UI
il core accetta una configurazione valida
il core espone se è configurato oppure no
```

Dato dinamico previsto:

```text
IsConfigured
```

Se `IsConfigured = false`, il comando `StartTimer` non deve avviare il timer.

Errore previsto:

```text
TimerNotConfigured
```

---

## 8. Stati stabili

Gli stati stabili del core sono:

```text
Stopped
Running
FinalAlert
Paused
```

Significato:

```text
Stopped
Timer fermo o non ancora avviato.

Running
Sessione in corso fuori dalla finestra finale di avviso.

FinalAlert
Sessione in corso dentro la finestra finale di avviso.

Paused
Timer in pausa con tempo rimanente conservato.
```

La fine sessione non è uno stato stabile.

La fine sessione è un evento.

Non devono essere introdotti stati stabili come:

```text
Completed
Finished
SessionEnded
```

salvo motivazione tecnica esplicita in un design successivo.

---

## 9. Dati dinamici esposti dal core

Il core deve poter esporre almeno questi dati dinamici:

```text
CurrentState
RemainingSeconds
CompletedSessions
IsConfigured
CanStart
CanPause
CanResume
CanReset
IsFinalAlertActive
```

Significato:

```text
CurrentState
Stato stabile corrente.

RemainingSeconds
Tempo rimanente della sessione corrente in secondi.

CompletedSessions
Numero di sessioni effettivamente completate.

IsConfigured
Indica se esiste una configurazione valida.

CanStart
Indica se StartTimer è logicamente ammesso.

CanPause
Indica se PauseTimer è logicamente ammesso.

CanResume
Indica se ResumeTimer è logicamente ammesso.

CanReset
Indica se ResetTimer è logicamente ammesso.

IsFinalAlertActive
Indica se il timer è nella finestra finale.
```

Questi dati devono essere neutri.

Esempio corretto:

```text
RemainingSeconds = 299
```

Esempio scorretto:

```text
RemainingText = "04:59"
```

---

## 10. Comandi supportati

Il core deve supportare questi comandi concettuali:

```text
ConfigureTimer
StartTimer
PauseTimer
ResumeTimer
ResetTimer
Tick
```

Questi nomi sono concettuali.

Il design tecnico finale potrà scegliere nomi C# equivalenti, purché il significato resti invariato.

### 10.1 ConfigureTimer

Configura il timer con una configurazione neutra.

Effetti se valido:

```text
IsConfigured = true
RemainingSeconds = SessionDurationSeconds
CurrentState = Stopped
```

Evento previsto:

```text
TimerConfigured
```

Effetti se non valido:

```text
IsConfigured = false oppure configurazione precedente invariata
CurrentState invariato
RemainingSeconds invariato oppure coerente con configurazione precedente
```

Errori possibili:

```text
InvalidSessionDuration
InvalidFinalAlertDuration
FinalAlertNotLessThanSessionDuration
```

Regola importante:

```text
una configurazione non valida non deve lasciare il core in stato incoerente
```

Se esiste già una configurazione valida e arriva una nuova configurazione non valida, la configurazione precedente deve restare invariata.

Regola approvata:

```text
configurazione non valida = configurazione precedente invariata
```

### 10.2 StartTimer

Avvia il timer se esiste una configurazione valida e lo stato lo consente.

Ammesso quando:

```text
IsConfigured = true
CurrentState = Stopped
```

Effetti:

```text
CurrentState = Running
RemainingSeconds = SessionDurationSeconds
```

Con le regole approvate, `FinalAlertDurationSeconds` è sempre minore di `SessionDurationSeconds`.

Per questo motivo, una sessione valida non inizia direttamente in `FinalAlert`.

Evento previsto:

```text
TimerStarted
```

Errore possibile:

```text
TimerNotConfigured
CannotStart
```

### 10.3 PauseTimer

Mette in pausa il timer mantenendo il tempo rimanente.

Ammesso quando:

```text
CurrentState = Running
oppure
CurrentState = FinalAlert
```

Effetti:

```text
CurrentState = Paused
RemainingSeconds invariato
CompletedSessions invariato
```

Evento previsto:

```text
TimerPaused
```

Errore possibile:

```text
CannotPause
```

Nota:

se la pausa avviene durante `FinalAlert`, il core deve conservare l'informazione sufficiente per sapere che, alla ripresa, il tempo rimanente si trova ancora nella finestra finale.

Questa informazione può derivare dal valore `RemainingSeconds` e dalla configurazione corrente.

### 10.4 ResumeTimer

Riprende il timer dalla pausa.

Ammesso quando:

```text
CurrentState = Paused
```

Effetti:

```text
CurrentState = Running oppure FinalAlert
RemainingSeconds invariato
CompletedSessions invariato
```

Se `RemainingSeconds <= FinalAlertDurationSeconds` e `FinalAlertDurationSeconds > 0`, la ripresa deve portare a:

```text
CurrentState = FinalAlert
```

In questo caso il core deve produrre obbligatoriamente:

```text
TimerResumed
FinalAlertStarted
```

`FinalAlertStarted` è necessario affinché il bridge possa riattivare l'avviso sonoro.

Se la ripresa avviene fuori dalla finestra finale, il core deve produrre:

```text
TimerResumed
```

e lo stato deve tornare a:

```text
Running
```

Errore possibile:

```text
CannotResume
```

Regola obbligatoria:

```text
la ripresa dentro la finestra finale deve produrre FinalAlertStarted
per permettere al bridge di riavviare l'avviso sonoro
```

### 10.5 ResetTimer

Annulla la sessione corrente e riporta il timer allo stato fermo.

Ammesso quando:

```text
IsConfigured = true
```

Effetti:

```text
CurrentState = Stopped
RemainingSeconds = SessionDurationSeconds
CompletedSessions invariato
```

Evento previsto:

```text
TimerReset
```

Una sessione annullata tramite reset non incrementa il contatore.

Il reset non azzera il contatore delle sessioni completate.

Nella prima versione minima non è previsto un comando globale di azzeramento contatore.

Se `ResetTimer` viene ricevuto quando il timer è già `Stopped`, il core deve:

```text
mantenere CurrentState = Stopped
reimpostare RemainingSeconds = SessionDurationSeconds
mantenere CompletedSessions invariato
produrre TimerReset
non produrre SessionCompleted
non produrre SessionCounterIncremented
non produrre NextSessionStarted
```

Regola approvata:

```text
ResetTimer valido produce TimerReset anche da Stopped
```

purché non generi eventi di sessione o ciclo.

Errore possibile:

```text
CannotReset
TimerNotConfigured
```

### 10.6 Tick

`Tick` avanza la logica del timer secondo il tempo trascorso.

`Tick` è un comando infrastrutturale.

Non è un comando utente.

Non deve essere esposto nella UI.

Ammesso quando:

```text
CurrentState = Running
oppure
CurrentState = FinalAlert
```

Se il timer è:

```text
Stopped
Paused
```

il tick non deve far avanzare il tempo.

Regola approvata:

```text
Tick in Stopped o Paused = esito controllato senza cambiamenti
```

In questi stati, `Tick` deve consumare l'impulso senza:

1. ridurre `RemainingSeconds`;
2. produrre eventi di ciclo;
3. produrre errori;
4. lanciare eccezioni;
5. causare effetti collaterali.

Il tick deve ricevere un'informazione temporale neutra.

Forma concettuale:

```text
ElapsedSeconds
```

Regola:

```text
ElapsedSeconds è un intero positivo maggiore o uguale a 1
```

Valori pari a zero o negativi non sono validi.

Errore previsto:

```text
InvalidTickDuration
```

Nella prima implementazione è ammesso usare tick da un secondo.

Il core non deve dipendere da un timer reale di sistema.

---

## 11. Logica del Tick

Quando `Tick` viene applicato in stato `Running` o `FinalAlert`, il core deve ridurre `RemainingSeconds`.

Esempio:

```text
RemainingSeconds = 300
Tick(1)
RemainingSeconds = 299
```

Il tempo rimanente non deve diventare negativo.

Regola:

```text
RemainingSeconds = max(0, RemainingSeconds - ElapsedSeconds)
```

Questa regola descrive il comportamento logico, non impone una specifica implementazione matematica.

Dopo ogni tick il core deve valutare:

1. se entra nella finestra finale;
2. se la sessione è completata;
3. se deve incrementare il contatore;
4. se deve ripartire automaticamente.

Ordine logico approvato:

```text
applica riduzione tempo
↓
se RemainingSeconds == 0: completa sessione
↓
altrimenti se entra nella finestra finale: passa a FinalAlert
↓
altrimenti resta Running
```

Questa sequenza evita di generare `FinalAlertStarted` quando un tick molto grande porta direttamente a zero.

Esempio:

```text
RemainingSeconds = 10
FinalAlertDurationSeconds = 5
Tick(10)
```

Risultato corretto:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

Non è necessario produrre `FinalAlertStarted`, perché la sessione è terminata direttamente.

---

## 12. Ingresso nella finestra finale

La finestra finale è attiva quando:

```text
FinalAlertDurationSeconds > 0
RemainingSeconds <= FinalAlertDurationSeconds
RemainingSeconds > 0
```

Il core deve produrre `FinalAlertStarted` solo quando avviene una transizione verso la finestra finale.

Esempio:

```text
SessionDurationSeconds = 300
FinalAlertDurationSeconds = 20

RemainingSeconds passa da 21 a 20
```

Risultato:

```text
CurrentState = FinalAlert
Evento = FinalAlertStarted
```

Il core non deve produrre `FinalAlertStarted` a ogni tick successivo.

Esempio scorretto:

```text
00:20 → FinalAlertStarted
00:19 → FinalAlertStarted
00:18 → FinalAlertStarted
```

Esempio corretto:

```text
00:20 → FinalAlertStarted
00:19 → nessun nuovo FinalAlertStarted
00:18 → nessun nuovo FinalAlertStarted
```

Se `FinalAlertDurationSeconds = 0`, il core non deve mai entrare nello stato `FinalAlert`.

In quel caso il timer resta `Running` fino al completamento della sessione.

Nota sulla ripresa dalla pausa:

```text
la ripresa dalla pausa dentro la finestra finale deve essere trattata come
una nuova attivazione dell'avviso finale verso il bridge
```

Per questo motivo, come stabilito nella sezione 10.4, `ResumeTimer` dentro la finestra finale deve produrre anche:

```text
FinalAlertStarted
```

---

## 13. Fine sessione

La sessione è completata quando:

```text
RemainingSeconds == 0
```

Quando la sessione è completata, il core deve:

1. produrre evento `SessionCompleted`;
2. incrementare `CompletedSessions` di 1;
3. produrre evento `SessionCounterIncremented`;
4. avviare una nuova sessione con la stessa configurazione;
5. produrre evento `NextSessionStarted`;
6. impostare `RemainingSeconds = SessionDurationSeconds`;
7. impostare immediatamente `CurrentState = Running`.

La ripartenza automatica deve essere atomica.

Questo significa che il core non deve attendere un tick successivo per rendere coerenti:

```text
RemainingSeconds
CurrentState
IsFinalAlertActive
CanPause
CanReset
```

Dopo la ripartenza automatica, lo stato della nuova sessione deve essere immediatamente coerente con la configurazione valida.

Poiché una configurazione valida impone:

```text
FinalAlertDurationSeconds < SessionDurationSeconds
```

una nuova sessione valida non inizia direttamente in `FinalAlert`.

Quindi, nella prima versione minima, dopo la ripartenza automatica lo stato deve essere:

```text
CurrentState = Running
```

Il core non deve fermarsi automaticamente dopo una sessione completata.

La prima versione non prevede un numero massimo di sessioni.

Esempio:

```text
SessionDurationSeconds = 300
FinalAlertDurationSeconds = 20
CompletedSessions = 2
RemainingSeconds = 1

Tick(1)
```

Risultato:

```text
SessionCompleted
CompletedSessions = 3
SessionCounterIncremented
NextSessionStarted
RemainingSeconds = 300
CurrentState = Running
```

La fine sessione non deve essere esposta come stato stabile permanente.

---

## 14. Ripartenza automatica

La ripartenza automatica appartiene al core.

Il bridge non deve simulare la ripartenza richiamando `StartTimer`.

Esempio scorretto:

```text
Core produce SessionCompleted
Bridge chiama StartTimer
```

Esempio corretto:

```text
Core produce SessionCompleted
Core incrementa il contatore
Core avvia internamente la sessione successiva
Core produce NextSessionStarted
Core espone immediatamente CurrentState = Running
```

La nuova sessione deve usare la stessa configurazione valida.

La ripartenza automatica non deve azzerare il contatore.

La ripartenza automatica non deve richiedere un comando utente.

La ripartenza automatica deve fermarsi solo se l'utente ha eseguito prima un comando che interrompe il ciclo, come pausa o reset.

La ripartenza automatica deve lasciare il core in uno stato coerente nello stesso risultato logico che produce gli eventi di completamento e nuova sessione.

---

## 15. Pausa e ripartenza automatica

Se il timer è in pausa, il ciclo automatico non deve proseguire.

Durante `Paused`:

```text
Tick non riduce RemainingSeconds
SessionCompleted non viene prodotto
NextSessionStarted non viene prodotto
```

Dopo `ResumeTimer`, il timer continua dalla quantità di tempo rimasta.

Se il timer viene messo in pausa a:

```text
RemainingSeconds = 10
```

e poi ripreso, continuerà da:

```text
RemainingSeconds = 10
```

La pausa non completa la sessione.

La pausa non incrementa il contatore.

La pausa non resetta la sessione.

Se la pausa avviene dentro la finestra finale e la ripresa avviene ancora dentro la finestra finale, il core deve produrre `FinalAlertStarted` insieme a `TimerResumed`, così che il bridge possa riattivare l'avviso finale.

---

## 16. Reset e contatore

Il reset annulla la sessione corrente.

Il reset non completa la sessione.

Il reset non incrementa il contatore.

Il reset non azzera il contatore.

Esempio:

```text
CompletedSessions = 3
CurrentState = Running
RemainingSeconds = 100

ResetTimer
```

Risultato:

```text
CompletedSessions = 3
CurrentState = Stopped
RemainingSeconds = SessionDurationSeconds
TimerReset
```

Nella prima versione minima non esiste comando separato:

```text
ResetCompletedSessions
```

Un eventuale comando futuro per azzerare il contatore richiederà un design dedicato.

Se il reset viene eseguito quando il timer è già `Stopped`, deve comunque produrre `TimerReset` come conferma del comando valido, senza generare eventi di sessione o ciclo.

---

## 17. Eventi neutri previsti

Il core deve produrre eventi neutri.

Eventi previsti da questo design:

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

Gli eventi non sono messaggi utente.

Esempio corretto:

```text
SessionCompleted
```

Esempio scorretto:

```text
Sessione completata. Sessioni completate: 3.
```

Gli eventi possono contenere dati neutri associati.

Esempio:

```text
SessionCompleted
CompletedSessions = 3
```

Il canale tecnico degli eventi non viene deciso definitivamente in questo documento.

Il design tecnico di implementazione potrà scegliere se usare:

1. result object con eventi;
2. eventi C#;
3. observer semplice;
4. altra soluzione piccola e coerente.

Regola obbligatoria:

```text
uno stesso evento non deve essere notificato due volte attraverso canali diversi
```

Eventi obbligatori in casi specifici:

```text
ResumeTimer dentro la finestra finale:
TimerResumed
FinalAlertStarted

ResetTimer da Stopped:
TimerReset

Completamento sessione:
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

---

## 18. Errori neutri previsti

Il core deve produrre errori neutri per condizioni logiche prevedibili.

Errori previsti:

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

Significato:

```text
InvalidSessionDuration
Durata sessione non valida.

InvalidFinalAlertDuration
Durata avviso finale non valida.

FinalAlertNotLessThanSessionDuration
Avviso finale maggiore o uguale alla durata sessione.

TimerNotConfigured
Timer non configurato.

CannotStart
Start non ammesso nello stato corrente.

CannotPause
Pause non ammesso nello stato corrente.

CannotResume
Resume non ammesso nello stato corrente.

CannotReset
Reset non ammesso nello stato corrente.

InvalidTickDuration
Tick con durata non valida.
```

Gli errori neutri non devono essere testi utente finali.

Il core non deve lanciare eccezioni non controllate per errori logici prevedibili.

Le eccezioni tecniche sono riservate a errori imprevisti o di programmazione.

---

## 19. Risultato dei comandi

Ogni comando deve produrre un risultato controllato.

Forma concettuale:

```text
CoreCommandResult
```

Il risultato può contenere:

```text
Success
CurrentState
RemainingSeconds
CompletedSessions
IsConfigured
Errors
```

Il risultato può anche includere dati sufficienti al bridge per aggiornare la UI.

Il documento non impone se gli eventi saranno inclusi nel result object o emessi tramite un canale separato.

La scelta sarà fatta nel coding design o nel coding plan.

Regola obbligatoria:

```text
gli eventi non devono essere duplicati
```

Esempio positivo:

```text
Success = true
CurrentState = Running
RemainingSeconds = 300
CompletedSessions = 0
Errors = []
```

Esempio con errore:

```text
Success = false
CurrentState = Stopped
Errors = [TimerNotConfigured]
```

Esempio di tick consumato senza effetti:

```text
CurrentState = Paused
Tick(1)

Success = true
CurrentState = Paused
RemainingSeconds invariato
CompletedSessions invariato
Errors = []
Eventi di ciclo = nessuno
```

---

## 20. Disponibilità dei comandi

Il core deve esporre la disponibilità logica dei comandi.

Regole minime:

```text
CanStart = IsConfigured && CurrentState == Stopped
CanPause = CurrentState == Running || CurrentState == FinalAlert
CanResume = CurrentState == Paused
CanReset = IsConfigured
```

Queste regole sono concettuali.

Il design tecnico potrà raffinarle se necessario.

La UI non deve decidere autonomamente se un comando è disponibile.

La UI deve ricevere dal bridge informazioni derivate dal core.

Dopo una ripartenza automatica, questi dati devono essere immediatamente coerenti con:

```text
CurrentState = Running
RemainingSeconds = SessionDurationSeconds
```

---

## 21. Casi limite obbligatori

Il core deve gestire almeno questi casi limite:

### 21.1 Avviso finale pari a zero

Configurazione:

```text
SessionDurationSeconds = 300
FinalAlertDurationSeconds = 0
```

Comportamento:

```text
nessuno stato FinalAlert
nessun evento FinalAlertStarted
nessuna richiesta audio derivata dall'avviso finale
sessione completata normalmente a zero
```

### 21.2 Tick maggiore del tempo rimanente

Esempio:

```text
RemainingSeconds = 3
Tick(5)
```

Comportamento:

```text
RemainingSeconds non diventa negativo
la sessione viene completata una sola volta
il contatore aumenta di 1
parte una nuova sessione
CurrentState diventa immediatamente Running
```

### 21.3 Tick in pausa

Esempio:

```text
CurrentState = Paused
Tick(1)
```

Comportamento:

```text
nessuna riduzione di RemainingSeconds
nessun evento di fine sessione
nessuna ripartenza automatica
nessun errore
nessuna eccezione
```

### 21.4 Tick da fermo

Esempio:

```text
CurrentState = Stopped
Tick(1)
```

Comportamento:

```text
nessuna riduzione di RemainingSeconds
nessun errore necessario
nessuna eccezione
nessun evento di ciclo
```

### 21.5 Reset da fermo

Esempio:

```text
CurrentState = Stopped
ResetTimer
```

Comportamento:

```text
CurrentState resta Stopped
RemainingSeconds torna a SessionDurationSeconds
CompletedSessions invariato
TimerReset prodotto
nessun evento SessionCompleted
nessun evento SessionCounterIncremented
nessun evento NextSessionStarted
```

### 21.6 Pausa durante avviso finale e ripresa

Esempio:

```text
CurrentState = FinalAlert
RemainingSeconds = 10
PauseTimer
ResumeTimer
```

Comportamento:

```text
dopo ResumeTimer lo stato deve essere FinalAlert
TimerResumed viene prodotto
FinalAlertStarted viene prodotto
il bridge deve poter riattivare l'avviso sonoro
```

### 21.7 Configurazione non valida dopo configurazione valida

Esempio:

```text
configurazione valida già presente
arriva nuova configurazione non valida
```

Comportamento:

```text
la configurazione valida precedente resta attiva
il core restituisce errore neutro
lo stato non diventa incoerente
```

### 21.8 Tick con durata non valida

Esempio:

```text
Tick(0)
Tick(-1)
```

Comportamento:

```text
nessuna modifica allo stato
nessuna riduzione di RemainingSeconds
errore neutro InvalidTickDuration
nessuna eccezione non controllata
```

---

## 22. Cosa è fuori perimetro

Questo design non autorizza:

1. UI WPF;
2. XAML;
3. accessibilità UI concreta;
4. AutomationProperties;
5. Live Region;
6. suoni reali;
7. gestione audio avanzata;
8. controllo volume di altre applicazioni;
9. notifiche Windows;
10. testi finali utente;
11. localizzazione multilingua;
12. salvataggio configurazioni;
13. persistenza del contatore;
14. storico sessioni;
15. esportazione dati;
16. numero massimo di sessioni;
17. timer lavoro/pausa;
18. routine complesse;
19. profili utente;
20. funzioni economiche;
21. database;
22. cloud;
23. server locale;
24. API REST;
25. plugin;
26. integrazione AI dentro l'app.

---

## 23. Impatto su accessibilità

Il core non implementa direttamente accessibilità.

Il core deve però fornire dati ed eventi sufficienti affinché bridge e UI possano rendere l'app accessibile.

In particolare, il core deve esporre:

1. stato corrente;
2. tempo rimanente;
3. contatore sessioni completate;
4. eventi importanti;
5. disponibilità dei comandi;
6. errori neutri.

Il core non deve generare annunci per NVDA.

Il core non deve produrre testi accessibili finali.

Il core non deve annunciare ogni tick.

La regola architetturale resta:

```text
il tempo rimanente cambia come dato dinamico
ma non diventa annuncio automatico a ogni secondo
```

Il core deve però produrre eventi sufficienti per permettere al bridge di comunicare gli eventi importanti.

In particolare, la ripresa dentro la finestra finale deve produrre `FinalAlertStarted`, perché questo evento permette al bridge di riattivare l'avviso finale e aggiornare correttamente lo stato accessibile.

---

## 24. Impatto su testi utente

Il core non produce testi utente finali.

Il core produce solo codici neutri.

Esempi:

```text
Running
FinalAlertStarted
InvalidSessionDuration
RemainingSeconds = 299
CompletedSessions = 3
```

Il bridge e il sistema testi utente trasformeranno questi elementi in testi.

Esempio:

```text
Core:
RemainingSeconds = 299

Bridge:
"Tempo rimanente: 04:59"
```

Questo design non autorizza stringhe hardcoded dentro il core.

---

## 25. Impatto su errori

Gli errori del core devono essere neutri, controllati e prevedibili.

Il core non deve mostrare errori.

Il core non deve decidere il testo finale dell'errore.

Il core non deve restituire eccezioni tecniche per casi logici attesi.

Gli errori logici devono essere verificabili nei test.

`InvalidTickDuration` deve essere usato per tick con durata pari a zero o negativa.

---

## 26. Impatto su test

Questo design richiede che il core sia testabile in isolamento.

I test futuri dovranno coprire almeno:

1. configurazione valida;
2. configurazione con durata sessione zero;
3. configurazione con avviso finale negativo;
4. configurazione con avviso finale uguale alla durata sessione;
5. configurazione con avviso finale maggiore della durata sessione;
6. avvio senza configurazione;
7. avvio con configurazione valida;
8. pausa da Running;
9. pausa da FinalAlert;
10. ripresa da Paused fuori finestra finale;
11. ripresa da Paused dentro finestra finale;
12. produzione di `FinalAlertStarted` dopo ripresa dentro finestra finale;
13. reset da Running;
14. reset da FinalAlert;
15. reset da Paused;
16. reset da Stopped;
17. produzione di `TimerReset` da Stopped;
18. tick da Running;
19. tick da FinalAlert;
20. tick da Paused senza effetti collaterali;
21. tick da Stopped senza effetti collaterali;
22. tick con durata zero;
23. tick con durata negativa;
24. ingresso singolo in FinalAlert;
25. assenza di FinalAlert se avviso finale è zero;
26. completamento sessione;
27. incremento contatore;
28. mancato incremento dopo reset;
29. ripartenza automatica;
30. stato immediatamente `Running` dopo ripartenza automatica;
31. tick maggiore del tempo rimanente;
32. configurazione non valida dopo configurazione valida;
33. disponibilità logica dei comandi;
34. produzione di eventi neutri;
35. produzione di errori neutri;
36. assenza di duplicazione degli eventi.

I test non devono richiedere:

1. WPF;
2. finestre;
3. NVDA;
4. audio reale;
5. API Windows.

---

## 27. Regole per gli agenti AI

Gli agenti AI che implementeranno questo design devono rispettare queste regole:

1. non creare UI;
2. non modificare XAML;
3. non introdurre audio;
4. non chiamare API Windows;
5. non introdurre NVDA o UI Automation nel core;
6. non generare testi utente nel core;
7. non formattare tempo in `mm:ss` dentro il core;
8. non rendere `Tick` un comando utente;
9. non trasformare la fine sessione in stato stabile permanente;
10. non spostare la ripartenza automatica nel bridge;
11. non incrementare il contatore fuori dal core;
12. non azzerare il contatore con ResetTimer;
13. non introdurre persistenza;
14. non introdurre storico;
15. non introdurre numero massimo di sessioni;
16. non introdurre timer lavoro/pausa;
17. non introdurre funzioni economiche;
18. non duplicare eventi su più canali;
19. non usare eccezioni per errori logici prevedibili;
20. non creare funzionalità fuori perimetro;
21. non lasciare facoltativa la produzione di `FinalAlertStarted` dopo ripresa dentro la finestra finale;
22. non lasciare facoltativa la produzione di `TimerReset` da stato `Stopped`;
23. non attendere un tick successivo per rendere coerente lo stato dopo ripartenza automatica;
24. non trattare tick in pausa o da fermo come errore tecnico;
25. non accettare `ElapsedSeconds` pari a zero o negativo.

---

## 28. Criteri di accettazione del design

Il futuro codice sarà considerato coerente con questo design solo se:

1. esiste un core timer engine isolato dalla UI;
2. il core è testabile senza WPF;
3. le durate interne sono rappresentate in secondi;
4. la configurazione viene validata dal core;
5. gli stati stabili sono `Stopped`, `Running`, `FinalAlert`, `Paused` o equivalenti;
6. la fine sessione è un evento, non uno stato stabile permanente;
7. `Tick` non è esposto come comando utente;
8. `Tick` non avanza il timer in pausa o da fermo;
9. tick in pausa o da fermo produce esito controllato senza effetti collaterali;
10. `ElapsedSeconds` è un intero positivo maggiore o uguale a 1;
11. tick con durata zero o negativa produce errore neutro `InvalidTickDuration`;
12. l'ingresso in `FinalAlert` avviene una sola volta per sessione;
13. con avviso finale zero non viene prodotto `FinalAlertStarted`;
14. `ResumeTimer` dentro la finestra finale produce `TimerResumed` e `FinalAlertStarted`;
15. la sessione completata incrementa il contatore una sola volta;
16. il reset non incrementa il contatore;
17. il reset non azzera il contatore;
18. `ResetTimer` da `Stopped` produce `TimerReset`;
19. `ResetTimer` da `Stopped` non produce eventi di sessione o ciclo;
20. la ripartenza automatica è gestita dal core;
21. dopo la ripartenza automatica lo stato è immediatamente `Running`;
22. pausa e reset interrompono il ciclo finché l'utente non dà un nuovo comando;
23. la ripresa dentro la finestra finale permette di riattivare l'avviso;
24. il core produce errori neutri;
25. il core produce eventi neutri;
26. il core non duplica eventi su più canali;
27. il core non produce testi utente finali;
28. il core non conosce UI, audio, NVDA o Windows;
29. non sono state introdotte funzionalità fuori perimetro;
30. i casi limite della sezione 21 sono gestiti.

---

## 29. Stato del documento

Questo documento è approvato come design del core timer engine di CicloTimer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini e correzione delle ambiguità su ripresa in avviso finale, reset da fermo, tick e ripartenza automatica
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione delle osservazioni dei consiglieri AI su FinalAlertStarted obbligatorio dopo ResumeTimer in finestra finale, TimerReset obbligatorio da Stopped, ripartenza automatica atomica in Running, Tick in Paused/Stopped senza effetti collaterali ed ElapsedSeconds intero positivo
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. produzione obbligatoria di `FinalAlertStarted` quando `ResumeTimer` riprende il timer dentro la finestra finale;
2. produzione obbligatoria di `TimerReset` anche quando `ResetTimer` viene ricevuto da stato `Stopped`;
3. chiarimento che la ripartenza automatica deve essere atomica e lasciare subito il core in `Running`;
4. esclusione del caso `SessionDurationSeconds == FinalAlertDurationSeconds`, perché contraddice la validazione approvata;
5. chiarimento che `Tick` in `Paused` o `Stopped` consuma l'impulso senza effetti collaterali;
6. chiarimento che `ElapsedSeconds` deve essere un intero positivo maggiore o uguale a 1;
7. aggiornamento dei casi limite, dell'impatto sui test, delle regole per agenti AI e dei criteri di accettazione.

Il documento è approvato dal project owner come base per il futuro coding plan, todo operativo e implementazione del core timer engine.
