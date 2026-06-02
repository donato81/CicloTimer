# CicloTimer — Design 002 — Bridge UI-logica e modello mostrabile del timer

**Tipo documento:** documento di design  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-02  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md  

---

## 1. Scopo del documento

Questo documento definisce il design tecnico del Bridge UI-logica del timer di CicloTimer.

Il Design 001 ha definito e isolato il core timer engine.

Il core timer engine espone dati neutri, stati neutri, eventi neutri, errori neutri e risultati di comando.

Il Bridge UI-logica ha il compito di collegare questi dati neutri alla futura UI, senza contaminare il core e senza trasformare la UI in una seconda logica applicativa.

Il bridge previsto da questo documento deve essere collocato nella cartella fisica già prevista dal progetto:

```text
view-models
````

Questa cartella è parte della struttura fissa del repository e non deve essere sostituita con `src`, `models` o altre cartelle inventate dagli agenti.

Lo scopo di questo documento è definire:

1. responsabilità del bridge;
2. responsabilità vietate al bridge;
3. modello mostrabile del timer;
4. trasformazione dei dati neutri in dati mostrabili;
5. conversione dei valori provenienti dalla UI;
6. formattazione del tempo rimanente;
7. trasformazione di stati, eventi ed errori in testi utente;
8. preparazione dei testi accessibili;
9. richieste concettuali verso il futuro livello sistema operativo;
10. criteri di testabilità del bridge;
11. collocazione fisica corretta nella cartella `view-models`.

Questo documento non definisce ancora:

1. UI WPF;
2. XAML;
3. ordine di tabulazione;
4. controlli accessibili concreti;
5. `AutomationProperties`;
6. Live Region;
7. annunci NVDA;
8. audio reale;
9. file audio;
10. API Windows;
11. gestione avanzata del volume di sistema;
12. notifiche Windows concrete;
13. coding plan operativo;
14. todo eseguibile da Cursor.

Questi aspetti saranno trattati in documenti successivi.

---

## 2. Obiettivo del design

L'obiettivo è progettare un bridge semplice, sottile e testabile.

Il bridge deve ricevere comandi dalla futura UI, chiamare il core e produrre un modello mostrabile pronto per la UI.

Il bridge deve trasformare dati neutri come:

```text
CurrentState = Running
RemainingSeconds = 299
CompletedSessions = 3
CanPause = true
Events = [TimerStarted]
Errors = []
```

in dati mostrabili come:

```text
Tempo rimanente: 04:59
Stato timer: Sessione in corso
Sessioni completate: 3
Azione principale: Pausa
Messaggio evento: Timer avviato
Messaggio errore: nessuno
```

Il bridge non deve decidere le regole del timer.

Il bridge non deve calcolare quando una sessione finisce.

Il bridge non deve incrementare il contatore.

Il bridge non deve simulare la ripartenza automatica.

Il bridge deve essere un traduttore controllato tra:

```text
UI
↓
Bridge / modello mostrabile
↓
Core
```

e tra:

```text
Core
↓
Bridge / modello mostrabile
↓
UI futura / futuro livello sistema operativo
```

---

## 3. Perimetro del design

Questo design può definire:

1. responsabilità del bridge UI-logica;
2. dati che la UI può passare al bridge;
3. comandi che il bridge può inviare al core;
4. dati che il bridge può leggere dal core;
5. struttura concettuale del modello mostrabile;
6. testi utente che il bridge deve preparare;
7. formato del tempo rimanente;
8. regole di conversione minuti/secondi in secondi totali;
9. regole di conversione stati/eventi/errori in testi;
10. richieste concettuali verso il livello sistema operativo;
11. criteri minimi di testabilità del bridge;
12. vincoli per la futura implementazione C#;
13. collocazione nella cartella `view-models`.

Questo design non può definire:

1. layout grafico della finestra;
2. `MainWindow.xaml`;
3. `MainWindow.xaml.cs`;
4. controlli WPF concreti;
5. stile visuale;
6. ordine di tabulazione;
7. proprietà UI Automation concrete;
8. annunci automatici NVDA;
9. implementazione audio;
10. timer reale di sistema;
11. dispatcher timer;
12. thread UI;
13. notifiche Windows;
14. persistenza;
15. storico sessioni;
16. funzioni economiche;
17. massimo numero di sessioni;
18. gestione avanzata audio;
19. modifica del core già approvato.

---

## 4. Principio guida del bridge

Il principio guida è:

```text
il bridge traduce, non decide
```

Il bridge deve conoscere:

```text
comandi UI ammessi
valori inseriti dall'utente
contratto pubblico del core
stati neutri
eventi neutri
errori neutri
dati dinamici neutri
testi utente centralizzati
formattazione mostrabile
richieste concettuali verso il futuro livello sistema operativo
```

Il bridge non deve conoscere:

```text
dettagli XAML
controlli grafici concreti
NVDA come tecnologia diretta
UI Automation concreta
file audio
API Windows
regole interne non pubbliche del core
database
cloud
persistenza
```

Il bridge deve restare piccolo.

Il bridge non deve diventare un secondo core.

---

## 5. Relazione con il core

Il core è già responsabile di:

1. configurazione logica;
2. validazione logica;
3. stati stabili;
4. eventi neutri;
5. errori neutri;
6. tempo rimanente;
7. contatore sessioni;
8. pausa;
9. ripresa;
10. reset;
11. tick;
12. avviso finale;
13. completamento sessione;
14. ripartenza automatica.

Il bridge può chiamare i comandi pubblici del core:

```text
ConfigureTimer
StartTimer
PauseTimer
ResumeTimer
ResetTimer
Tick
```

Il bridge può leggere dal core:

```text
CurrentState
RemainingSeconds
CompletedSessions
IsConfigured
IsFinalAlertActive
CanStart
CanPause
CanResume
CanReset
```

Il bridge può leggere dal risultato del comando:

```text
Success
State
RemainingSeconds
CompletedSessions
IsConfigured
Errors
Events
```

Nota:

```text
TimerCommandResult rappresenta uno snapshot del comando corrente.
La fonte primaria dello stato corrente resta TimerEngine.
```

Il bridge deve quindi usare il risultato del comando per capire l'esito immediato dell'azione, ma deve considerare il core come fonte principale dello stato corrente.

---

## 6. Responsabilità autorizzate del bridge

Il bridge può:

1. ricevere dalla UI valori di durata sessione;
2. ricevere dalla UI valori di durata avviso finale;
3. convertire minuti e secondi in secondi totali;
4. creare una configurazione neutra per il core;
5. chiamare `ConfigureTimer`;
6. chiamare `StartTimer`;
7. chiamare `PauseTimer`;
8. chiamare `ResumeTimer`;
9. chiamare `ResetTimer`;
10. inoltrare `Tick` al core quando riceve il comando da un orchestratore esterno;
11. leggere lo stato aggiornato dal core;
12. leggere il risultato dell'ultimo comando;
13. trasformare `TimerState` in testo utente;
14. trasformare `TimerError` in messaggio utente;
15. trasformare `TimerEvent` in messaggio evento;
16. formattare `RemainingSeconds` in `mm:ss`;
17. formattare `CompletedSessions` in testo mostrabile;
18. calcolare il testo dell'azione principale;
19. copiare o esporre le disponibilità `CanStart`, `CanPause`, `CanResume`, `CanReset` nel modello mostrabile;
20. preparare un testo accessibile coerente con il testo visivo;
21. preparare richieste concettuali verso il futuro livello sistema operativo.

Il bridge può produrre un modello mostrabile completo dopo ogni comando ricevuto.

Il bridge può anche produrre un modello mostrabile a richiesta, senza eseguire un nuovo comando, leggendo lo stato corrente del core.

Il bridge non possiede una sorgente temporale propria.

Il bridge non deve generare tick.

Il bridge non deve avviare timer reali.

Il bridge non deve usare `DispatcherTimer`, `System.Timers.Timer`, thread o loop temporali.

La sorgente dei tick sarà definita in un futuro design UI/orchestrazione.

---

## 7. Responsabilità vietate al bridge

Il bridge non deve:

1. validare logicamente la configurazione al posto del core;
2. decidere che `SessionDurationSeconds > 0` è valido o non valido come regola finale;
3. decidere che `FinalAlertDurationSeconds < SessionDurationSeconds` è valido o non valido come regola finale;
4. calcolare il tempo rimanente sottraendo secondi autonomamente;
5. decidere quando entrare in `FinalAlert`;
6. decidere quando una sessione è completata;
7. incrementare direttamente `CompletedSessions`;
8. azzerare direttamente `CompletedSessions`;
9. simulare `NextSessionStarted`;
10. richiamare `StartTimer` dopo `SessionCompleted` per simulare la ripartenza;
11. produrre eventi core autonomi;
12. trasformare un evento mancante inventandolo;
13. duplicare eventi ricevuti dal core;
14. contenere timer reali;
15. contenere dispatcher timer;
16. contenere codice audio reale;
17. chiamare API Windows;
18. conoscere `AutomationProperties`;
19. conoscere Live Region;
20. contenere stringhe utente hardcoded sparse;
21. contenere logica UI concreta;
22. modificare controlli grafici;
23. aprire finestre;
24. accedere a database;
25. gestire storico sessioni.

Il bridge deve restare deterministico e testabile.

---

## 8. Input provenienti dalla UI

La futura UI potrà passare al bridge valori legati alla configurazione del timer.

Forma concettuale degli input UI:

```text
SessionMinutes
SessionSeconds
FinalAlertMinutes
FinalAlertSeconds
```

Questi nomi sono concettuali.

La UI potrà raccogliere minuti e secondi separati perché questo è comprensibile per l'utente.

Il core invece deve ricevere solo secondi totali.

Il bridge deve quindi convertire:

```text
SessionMinutes + SessionSeconds
↓
SessionDurationSeconds
```

e:

```text
FinalAlertMinutes + FinalAlertSeconds
↓
FinalAlertDurationSeconds
```

Esempio:

```text
SessionMinutes = 5
SessionSeconds = 0

Bridge:
SessionDurationSeconds = 300
```

Esempio:

```text
FinalAlertMinutes = 0
FinalAlertSeconds = 20

Bridge:
FinalAlertDurationSeconds = 20
```

La conversione aritmetica minuti/secondi in secondi appartiene al bridge.

La validazione logica della configurazione appartiene al core.

---

## 9. Validazione UI e validazione core

La UI può fare controlli minimi di formato.

Esempi di controlli UI ammessi:

```text
campo vuoto
caratteri non numerici
valore non convertibile in intero
```

Questi controlli riguardano la leggibilità dell'input, non la validità logica del timer.

Il bridge può ricevere dalla UI valori già convertiti in interi, oppure può aiutare a convertire valori grezzi se il futuro design UI lo richiederà.

Il bridge può produrre errori di conversione o input non parsabile.

Questi errori non devono sostituire gli errori core.

Errori logici che appartengono al core:

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

Esempio corretto:

```text
UI:
l'utente inserisce 0 minuti e 0 secondi per la sessione

Bridge:
converte a SessionDurationSeconds = 0

Core:
restituisce InvalidSessionDuration

Bridge:
prepara messaggio utente
```

Esempio scorretto:

```text
Bridge:
decide autonomamente che SessionDurationSeconds = 0 è logicamente non valido
e non chiama il core
```

La validazione logica deve restare nel core.

---

## 10. Modello mostrabile del timer

Il bridge deve produrre un modello mostrabile.

Nome concettuale:

```text
TimerDisplayModel
```

Questo nome non impone una classe definitiva, ma definisce il concetto.

Si usa `TimerDisplayModel` e non `TimerViewModel` perché questo design non introduce ancora un vero ViewModel WPF, non introduce binding, non introduce `INotifyPropertyChanged` e non introduce `ICommand`.

Il modello mostrabile può contenere:

```text
RemainingTimeText
TimerStateText
CompletedSessionsText
PrimaryActionText
CanStart
CanPause
CanResume
CanReset
IsConfigured
IsFinalAlertActive
ErrorMessageText
EventMessageText
AccessibleStatusText
AccessibleEventText
```

Significato:

```text
RemainingTimeText
Tempo rimanente formattato, per esempio "04:59".

TimerStateText
Stato corrente formattato, per esempio "Sessione in corso".

CompletedSessionsText
Contatore formattato, per esempio "Sessioni completate: 3".

PrimaryActionText
Testo dell'azione principale, per esempio "Avvia", "Pausa" o "Riprendi".

CanStart / CanPause / CanResume / CanReset
Disponibilità logica dei comandi letta dal core ed esposta alla UI.

IsConfigured
Indicazione se esiste una configurazione valida.

IsFinalAlertActive
Indicazione neutra utile alla UI per rappresentare lo stato di avviso finale.

ErrorMessageText
Messaggio di errore finale, se presente.

EventMessageText
Messaggio evento finale, se presente.

AccessibleStatusText
Testo accessibile coerente con lo stato visivo.

AccessibleEventText
Testo accessibile collegato a un evento importante, se presente.
```

Il modello mostrabile contiene testi finali e quindi non appartiene al core.

La UI riceve il modello mostrabile e lo espone.

La UI non deve calcolare o tradurre autonomamente i campi del modello.

---

## 11. Testi utente centralizzati

Il bridge deve usare il sistema centralizzato dei testi utente.

La soluzione concreta del sistema testi sarà definita nel coding plan o in un design dedicato se necessario.

Per la prima versione è ammessa una soluzione semplice, per esempio una classe o modulo interno con costanti stringa.

Il principio obbligatorio è:

```text
una sola fonte controllata per i testi statici rivolti all'utente
```

Il bridge non deve contenere testi utente hardcoded sparsi.

Sono testi statici da centralizzare:

```text
Durata sessione
Avviso finale
Avvia
Pausa
Riprendi
Reset
Timer fermo
Sessione in corso
Avviso finale in corso
Timer in pausa
Sessione completata
Sessioni completate
Tempo rimanente
Valore non valido
Configurazione non valida
```

Il bridge può combinare testi statici centralizzati con dati dinamici.

Esempio corretto:

```text
Testo statico centralizzato:
"Tempo rimanente"

Dato dinamico:
"04:59"

Bridge:
"Tempo rimanente: 04:59"
```

Esempio scorretto:

```text
Core:
"Tempo rimanente: 04:59"
```

Altro esempio scorretto:

```text
UI:
se CurrentState == Running allora mostra "Sessione in corso"
```

La trasformazione appartiene al bridge.

---

## 12. Formattazione del tempo

Il core espone il tempo in secondi interi.

Il bridge deve formattare il tempo in una forma leggibile.

Formato minimo previsto:

```text
mm:ss
```

Esempi:

```text
0 secondi     → 00:00
5 secondi     → 00:05
59 secondi    → 00:59
60 secondi    → 01:00
299 secondi   → 04:59
300 secondi   → 05:00
3600 secondi  → 60:00
```

Per la prima versione non è necessario introdurre formato ore/minuti/secondi.

Se una sessione supera 59 minuti, il formato `mm:ss` può continuare a mostrare minuti totali.

Esempio:

```text
3661 secondi → 61:01
```

Questa scelta mantiene il formato semplice e coerente.

Il bridge non deve annunciare automaticamente il tempo a ogni secondo.

La formattazione produce solo testo mostrabile o consultabile.

La decisione su quando aggiornare la UI appartiene al futuro design UI/orchestrazione.

---

## 13. Trasformazione degli stati

Il bridge deve trasformare gli stati neutri in testi utente.

Mappatura concettuale minima:

```text
Stopped     → Timer fermo
Running     → Sessione in corso
FinalAlert  → Avviso finale in corso
Paused      → Timer in pausa
```

Il testo finale deve provenire dal sistema centralizzato dei testi utente.

La UI non deve contenere questa mappatura.

Il core non deve contenere questa mappatura.

Il bridge è responsabile della trasformazione.

---

## 14. Trasformazione degli errori

Il bridge deve trasformare gli errori neutri del core in messaggi utente.

Mappatura concettuale minima:

```text
InvalidSessionDuration
→ La durata della sessione deve essere maggiore di zero.

InvalidFinalAlertDuration
→ La durata dell'avviso finale non può essere negativa.

FinalAlertNotLessThanSessionDuration
→ La durata dell'avviso finale deve essere inferiore alla durata della sessione.

TimerNotConfigured
→ Configura il timer prima di avviarlo.

CannotStart
→ Il timer non può essere avviato nello stato corrente.

CannotPause
→ Il timer non può essere messo in pausa nello stato corrente.

CannotResume
→ Il timer non può essere ripreso nello stato corrente.

CannotReset
→ Il timer non può essere resettato nello stato corrente.

InvalidTickDuration
→ Errore interno: durata tick non valida.
```

Questi testi sono esempi di design.

La formulazione finale può essere affinata nel coding plan, ma deve restare centralizzata.

Il bridge deve poter ricevere più errori nello stesso risultato.

Regola obbligatoria per la prima versione:

```text
in presenza di più errori, il bridge mostra il primo errore della lista,
secondo l'ordine restituito dal core
```

Questa scelta è semplice, deterministica e testabile.

Il bridge non deve mostrare eccezioni tecniche grezze.

---

## 15. Trasformazione degli eventi

Il bridge deve trasformare gli eventi neutri del core in messaggi evento.

Mappatura concettuale minima:

```text
TimerConfigured
→ Timer configurato.

TimerStarted
→ Timer avviato.

TimerPaused
→ Timer in pausa.

TimerResumed
→ Timer ripreso.

TimerReset
→ Timer resettato.

FinalAlertStarted
→ Avviso finale iniziato.

SessionCompleted
→ Sessione completata.

SessionCounterIncremented
→ Sessioni completate aggiornate.

NextSessionStarted
→ Nuova sessione avviata.

ValidationFailed
→ Configurazione o comando non valido.
```

Gli eventi arrivano dal canale definito dal core implementato nel Design 001.

Nel Design 001 e nel TODO 001 gli eventi sono nel `TimerCommandResult.Events`.

Il bridge deve trattare `Events` come lista ordinata e non cumulativa del comando corrente.

Il bridge non deve accumulare eventi precedenti.

Il bridge non deve mantenere una coda storica di eventi.

Il bridge non deve ricevere lo stesso evento da più canali.

Il bridge non deve duplicare la gestione di uno stesso evento.

Quando più eventi sono presenti nello stesso result, il bridge deve rispettare l'ordine.

Esempio:

```text
Events = [SessionCompleted, SessionCounterIncremented, NextSessionStarted]
```

Ordine di gestione:

```text
1. SessionCompleted
2. SessionCounterIncremented
3. NextSessionStarted
```

Per il messaggio utente finale, il bridge può sintetizzare più eventi collegati nello stesso result in un solo messaggio.

La sintesi di eventi multipli in un unico messaggio è ammessa solo quando gli eventi sono presenti insieme nello stesso `TimerCommandResult` del comando corrente.

Esempio:

```text
Sessione completata. Sessioni completate: 3.
```

Questo messaggio è prodotto dal bridge, non dal core.

Se nel result del comando corrente è presente un solo evento, il bridge gestisce solo quell'evento.

Se eventi collegati arrivano in result separati, il bridge non deve accumularli artificialmente e non deve mantenere una coda storica per ricostruire un messaggio combinato.

In quel caso, il bridge gestisce solo l'evento o gli eventi presenti nel result corrente.

---

## 16. Azione principale

La UI futura potrebbe avere un pulsante principale che cambia testo in base allo stato.

Il bridge può calcolare `PrimaryActionText`.

Mappatura concettuale:

```text
Stopped + CanStart = true
→ Avvia

Running + CanPause = true
→ Pausa

FinalAlert + CanPause = true
→ Pausa

Paused + CanResume = true
→ Riprendi
```

Se nessuna azione principale è disponibile, il bridge può restituire un testo vuoto o una azione disabilitata.

Questa scelta precisa può essere definita nel coding plan o nel design UI.

Il bridge non deve decidere la disponibilità logica dei comandi.

La disponibilità viene dal core tramite:

```text
CanStart
CanPause
CanResume
CanReset
```

Il bridge può solo leggerla ed esporla nel modello mostrabile.

---

## 17. Reset e contatore

Il bridge deve rispettare le regole del core e della visione.

Il reset annulla la sessione corrente e riporta il tempo alla durata configurata.

Il reset non incrementa il contatore.

Il reset non azzera automaticamente il contatore delle sessioni completate.

Il bridge deve quindi mostrare il contatore ricevuto dal core.

Esempio:

```text
CompletedSessions = 3
ResetTimer
CompletedSessions = 3
```

Il bridge non deve modificare questo valore.

Se in futuro verrà introdotta una funzione di azzeramento globale del contatore, dovrà essere definita da un design dedicato.

---

## 18. Avviso finale e richieste verso il sistema operativo

Il bridge può interpretare eventi neutri legati all'avviso finale.

Quando riceve:

```text
FinalAlertStarted
```

può preparare:

```text
TimerStateText = "Avviso finale in corso"
EventMessageText = "Avviso finale iniziato."
```

e può produrre una richiesta concettuale verso il futuro livello sistema operativo:

```text
StartFinalAlertSound
```

Quando riceve eventi di fine sessione, come:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

può produrre una richiesta concettuale:

```text
StopFinalAlertSound
```

se l'avviso finale era attivo.

Quando riceve:

```text
TimerPaused
```

e lo stato precedente o corrente indica che l'avviso finale era attivo, può produrre una richiesta concettuale:

```text
StopFinalAlertSound
```

Quando riceve nello stesso result:

```text
TimerResumed
FinalAlertStarted
```

può produrre una richiesta concettuale:

```text
StartFinalAlertSound
```

Questo design non implementa audio.

Questo design non sceglie librerie audio.

Questo design non chiama API Windows.

Questo design definisce solo le richieste concettuali che il bridge potrà inviare a un livello sistema operativo futuro.

---

## 19. Richieste concettuali verso il livello sistema operativo

Il bridge può produrre richieste neutre verso il futuro livello sistema operativo.

Nome concettuale:

```text
SystemActionRequest
```

Valori concettuali minimi:

```text
None
StartFinalAlertSound
StopFinalAlertSound
```

Questi nomi definiscono il contratto concettuale, non obbligano ancora a creare un tipo C# dedicato.

Il coding plan potrà decidere la forma concreta più semplice, per esempio:

1. enum;
2. lista di enum;
3. struttura semplice;
4. valori costanti interni.

Il design definitivo del livello audio/sistema operativo sarà affrontato in un design successivo.

Il bridge non deve eseguire direttamente queste richieste.

Il bridge può solo restituirle o passarle al livello appropriato quando questo sarà progettato.

Regola obbligatoria:

```text
il bridge non deve chiamare direttamente API Windows
```

---

## 20. Testi accessibili

Il bridge deve preparare testi accessibili coerenti con i testi visivi.

Per la prima versione il bridge deve produrre sempre i testi accessibili previsti dal modello, anche quando coincidono con i testi visivi.

Questa scelta rende il comportamento più semplice, più prevedibile e più testabile.

Esempio:

```text
TimerStateText = "Sessione in corso"
AccessibleStatusText = "Sessione in corso"
```

Esempio:

```text
EventMessageText = "Sessione completata. Sessioni completate: 3."
AccessibleEventText = "Sessione completata. Sessioni completate: 3."
```

Il bridge non deve usare API NVDA.

Il bridge non deve impostare `AutomationProperties`.

Il bridge non deve creare Live Region.

Il bridge prepara solo testi coerenti.

La futura UI deciderà come esporli tramite strumenti accessibili della piattaforma.

Il tempo rimanente deve essere consultabile, ma non annunciato automaticamente ogni secondo.

Quindi il bridge può produrre:

```text
RemainingTimeText = "04:59"
```

e:

```text
AccessibleStatusText = "Tempo rimanente: 04:59. Sessione in corso."
```

ma non deve generare un evento accessibile automatico per ogni tick.

La decisione tecnica su Live Region o altri meccanismi sarà nel design UI/accessibilità operativa.

---

## 21. Output del bridge

Il bridge deve produrre o esporre almeno:

```text
TimerDisplayModel aggiornato
+
eventuali richieste concettuali verso il livello sistema operativo
```

Non viene imposto in questo design un wrapper obbligatorio come `TimerBridgeResult`.

La scelta concreta sarà definita nel coding plan.

Regola obbligatoria:

```text
dopo ogni comando la UI deve poter ricevere o leggere uno stato mostrabile coerente
```

Sono opzioni ammissibili per la futura implementazione:

1. restituire direttamente `TimerDisplayModel`;
2. restituire `TimerDisplayModel` più una lista di richieste concettuali;
3. mantenere un `TimerDisplayModel` aggiornato nel bridge;
4. usare una piccola struttura di risultato solo se il coding plan dimostra che semplifica il codice.

Non sono ammissibili:

1. un wrapper complesso non necessario;
2. un event bus;
3. observer complessi;
4. dipendenze MVVM/WPF;
5. `INotifyPropertyChanged` in questo livello, salvo design UI successivo;
6. `ICommand` in questo livello, salvo design UI successivo.

---

## 22. Sequenza: configurazione valida

Sequenza concettuale:

```text
UI raccoglie:
SessionMinutes = 5
SessionSeconds = 0
FinalAlertMinutes = 0
FinalAlertSeconds = 20
↓
Bridge converte:
SessionDurationSeconds = 300
FinalAlertDurationSeconds = 20
↓
Bridge crea TimerConfiguration
↓
Bridge chiama ConfigureTimer
↓
Core accetta configurazione
↓
Bridge legge stato aggiornato
↓
Bridge produce TimerDisplayModel
↓
UI può mostrare:
Tempo rimanente: 05:00
Timer fermo
Sessioni completate: 0
```

Il bridge non decide se 300 e 20 sono logicamente validi.

Lo decide il core.

---

## 23. Sequenza: configurazione non valida

Sequenza concettuale:

```text
UI raccoglie:
SessionMinutes = 0
SessionSeconds = 0
FinalAlertMinutes = 0
FinalAlertSeconds = 20
↓
Bridge converte:
SessionDurationSeconds = 0
FinalAlertDurationSeconds = 20
↓
Bridge crea TimerConfiguration
↓
Bridge chiama ConfigureTimer
↓
Core rifiuta configurazione
↓
Core restituisce InvalidSessionDuration
↓
Bridge trasforma il primo errore in messaggio utente
↓
Bridge produce TimerDisplayModel con ErrorMessageText
↓
UI mostra errore già pronto
```

Esempio messaggio:

```text
La durata della sessione deve essere maggiore di zero.
```

Il messaggio non deve essere generato dal core.

---

## 24. Sequenza: avvio timer

Sequenza concettuale:

```text
UI invia comando Avvia
↓
Bridge chiama StartTimer
↓
Core passa a Running
↓
Core restituisce TimerStarted
↓
Bridge legge stato aggiornato
↓
Bridge trasforma Running in "Sessione in corso"
↓
Bridge trasforma TimerStarted in "Timer avviato."
↓
Bridge produce TimerDisplayModel aggiornato
↓
UI mostra stato e messaggio evento
```

Punti vietati:

1. la UI non deve chiamare direttamente il core;
2. la UI non deve trasformare `Running` in testo;
3. il bridge non deve decidere se Start è valido;
4. il bridge non deve avviare un timer reale;
5. il bridge non deve riprodurre suoni.

---

## 25. Sequenza: tick

Sequenza concettuale:

```text
sorgente temporale futura produce impulso temporale
↓
orchestratore esterno futuro invia Tick al bridge
↓
bridge inoltra Tick al core
↓
core aggiorna RemainingSeconds
↓
bridge legge RemainingSeconds
↓
bridge formatta RemainingTimeText
↓
bridge produce TimerDisplayModel aggiornato
↓
UI può aggiornare il testo mostrato
```

Il tick non è un comando utente.

Il bridge non deve esporre tick come pulsante.

Il bridge non deve creare annunci accessibili automatici a ogni tick.

Il bridge non deve decidere se la sessione è completata.

Il bridge non possiede la sorgente temporale.

Il bridge non genera tick.

Il bridge non deve creare `DispatcherTimer`, `System.Timers.Timer`, thread o loop temporali.

---

## 26. Sequenza: ingresso in avviso finale

Sequenza concettuale:

```text
core riceve Tick
↓
core rileva ingresso in FinalAlert
↓
core espone FinalAlertStarted
↓
bridge riceve FinalAlertStarted
↓
bridge prepara "Avviso finale in corso"
↓
bridge prepara richiesta StartFinalAlertSound
↓
bridge produce TimerDisplayModel aggiornato
```

Il bridge non calcola la soglia di avviso.

Il bridge non decide quando parte l'avviso.

Il bridge non riproduce direttamente il suono.

---

## 27. Sequenza: pausa durante avviso finale

Sequenza concettuale:

```text
timer è in FinalAlert
↓
UI invia Pausa
↓
bridge chiama PauseTimer
↓
core passa a Paused
↓
core espone TimerPaused
↓
bridge prepara "Timer in pausa"
↓
bridge può produrre richiesta StopFinalAlertSound
↓
bridge produce TimerDisplayModel aggiornato
```

Il bridge non deve perdere il tempo rimanente.

Il bridge non deve decidere se la sessione resta nella finestra finale.

Il tempo rimanente resta nel core.

---

## 28. Sequenza: ripresa dentro avviso finale

Sequenza concettuale:

```text
timer è Paused
tempo rimanente ancora dentro la finestra finale
↓
UI invia Riprendi
↓
bridge chiama ResumeTimer
↓
core passa a FinalAlert
↓
core espone TimerResumed e FinalAlertStarted
↓
bridge prepara "Avviso finale in corso"
↓
bridge può produrre richiesta StartFinalAlertSound
↓
bridge produce TimerDisplayModel aggiornato
```

Il bridge non deve inventare `FinalAlertStarted`.

Il bridge deve reagire all'evento o alla transizione esposta dal core.

---

## 29. Sequenza: fine sessione e ripartenza automatica

Sequenza concettuale:

```text
Tick porta RemainingSeconds a zero
↓
core espone SessionCompleted
↓
core incrementa CompletedSessions
↓
core espone SessionCounterIncremented
↓
core avvia internamente nuova sessione
↓
core espone NextSessionStarted
↓
bridge riceve gli eventi ordinati nello stesso result del comando corrente
↓
bridge prepara messaggio:
"Sessione completata. Sessioni completate: 3."
↓
bridge produce eventuale richiesta StopFinalAlertSound
↓
bridge legge stato aggiornato
↓
bridge produce TimerDisplayModel aggiornato
```

Il bridge non deve chiamare `StartTimer`.

La ripartenza automatica appartiene al core.

Il bridge non deve mantenere una coda storica degli eventi.

La sintesi del messaggio "Sessione completata. Sessioni completate: 3." è valida solo se gli eventi collegati sono presenti insieme nello stesso `TimerCommandResult`.

---

## 30. Sequenza: reset

Sequenza concettuale:

```text
UI invia Reset
↓
bridge chiama ResetTimer
↓
core passa a Stopped
↓
core reimposta RemainingSeconds alla durata configurata
↓
core mantiene CompletedSessions invariato
↓
core espone TimerReset
↓
bridge prepara "Timer resettato."
↓
bridge produce TimerDisplayModel aggiornato
```

Il bridge non deve azzerare il contatore.

Il bridge non deve simulare il reset.

Il bridge non deve generare eventi di ciclo.

---

## 31. Testabilità del bridge

Il bridge deve essere testabile senza:

1. aprire finestre;
2. caricare XAML;
3. usare NVDA;
4. usare UI Automation;
5. riprodurre suoni;
6. chiamare API Windows;
7. usare timer reali;
8. dipendere da thread UI.

I test del bridge devono verificare trasformazioni.

Esempi di test:

```text
RemainingSeconds = 299
↓
RemainingTimeText = "04:59"
```

```text
CurrentState = Running
↓
TimerStateText = "Sessione in corso"
```

```text
Error = InvalidSessionDuration
↓
ErrorMessageText = "La durata della sessione deve essere maggiore di zero."
```

```text
Events = [SessionCompleted, SessionCounterIncremented, NextSessionStarted]
CompletedSessions = 3
↓
EventMessageText = "Sessione completata. Sessioni completate: 3."
```

```text
CurrentState = Paused
CanResume = true
↓
PrimaryActionText = "Riprendi"
```

I test devono essere deterministici.

---

## 32. Dipendenze del bridge

Il bridge può dipendere da:

1. progetto core;
2. sistema centralizzato dei testi utente;
3. eventuali valori concettuali di richiesta verso sistema operativo;
4. tipi del modello mostrabile.

Il bridge non deve dipendere da:

1. WPF;
2. XAML;
3. `MainWindow`;
4. `Application`;
5. `Dispatcher`;
6. NVDA;
7. UI Automation;
8. audio reale;
9. API Windows;
10. database;
11. cloud;
12. file di configurazione esterni non approvati.

Per la prima implementazione, il bridge deve essere collocato in `view-models`.

---

## 33. Collocazione fisica obbligatoria

La struttura fisica del progetto è fissata dal project owner e non deve essere reinterpretata dagli agenti.

La logica core già implementata si trova in:

```text
models/CicloTimer.Core/
```

Il bridge / modello mostrabile deve invece essere collocato in:

```text
view-models/
```

Per la prima versione, la collocazione obbligatoria è:

```text
view-models/TimerBridge/
```

Questa scelta è deliberatamente semplice.

Non viene creato in questo design un progetto separato `CicloTimer.Bridge`.

La struttura corretta è:

```text
models/
  CicloTimer.Core/

view-models/
  TimerBridge/
```

Regola vincolante:

```text
non creare src/
non creare models/CicloTimer.Bridge/
non creare view-models/CicloTimer.Bridge/ in questa versione
non collocare il bridge nella UI WPF
non collocare il bridge nel progetto core
```

Il codice del bridge può referenziare `CicloTimer.Core`.

Il progetto `CicloTimer.Core` non deve referenziare il bridge.

Il progetto WPF non deve ancora essere modificato in questo design.

I test del bridge potranno essere collocati in:

```text
tests/CicloTimer.Bridge.Tests/
```

Questa collocazione dei test resta coerente con la struttura già usata per il core.

Il coding plan dovrà verificare la struttura reale del repository prima di creare file, ma non potrà cambiare la cartella radice `view-models/TimerBridge/`.

---

## 34. Relazione con i futuri design

Questo design prepara ma non implementa:

1. UI WPF minima;
2. accessibilità operativa;
3. audio dell'avviso finale;
4. sorgente temporale reale;
5. collegamento effettivo con `MainWindow`.

Sequenza logica futura:

```text
Design 001 — Core timer engine
↓
Design 002 — Bridge UI-logica e modello mostrabile
↓
Design 003 — UI WPF minima accessibile
↓
Design 004 — Audio avviso finale
↓
Design 005 — Integrazione timer reale / orchestrazione UI
```

La numerazione futura potrà cambiare, ma il principio resta:

```text
prima tradurre il core in dati mostrabili
poi collegare la UI
poi collegare audio e integrazioni Windows
```

---

## 35. Criteri di validità

Il design sarà rispettato se una futura implementazione:

1. crea il bridge dentro `view-models/TimerBridge/`;
2. non crea `src`;
3. non crea `models/CicloTimer.Bridge`;
4. non crea `view-models/CicloTimer.Bridge` in questa versione;
5. mantiene il core indipendente dal bridge;
6. non modifica il comportamento del core;
7. converte minuti e secondi UI in secondi totali;
8. invia configurazioni neutre al core;
9. formatta `RemainingSeconds` fuori dal core;
10. produce `RemainingTimeText`;
11. produce `TimerStateText`;
12. produce `CompletedSessionsText`;
13. produce messaggi errore da errori neutri;
14. produce messaggi evento da eventi neutri;
15. usa testi statici centralizzati;
16. non inserisce stringhe utente hardcoded sparse;
17. non duplica la validazione logica;
18. non incrementa il contatore fuori dal core;
19. non simula la ripartenza automatica;
20. non possiede una sorgente temporale;
21. non genera tick;
22. non chiama direttamente API Windows;
23. non riproduce audio reale;
24. non usa WPF;
25. non usa XAML;
26. non usa NVDA;
27. non usa UI Automation;
28. è testabile senza UI;
29. produce un modello mostrabile coerente dopo ogni comando;
30. mantiene eventi non cumulativi e ordinati;
31. sintetizza eventi multipli solo se presenti nello stesso `TimerCommandResult`;
32. non gestisce lo stesso evento tramite più canali;
33. usa il primo errore della lista in caso di errori multipli;
34. produce sempre `AccessibleStatusText`;
35. produce sempre `AccessibleEventText`, anche se vuoto o uguale al testo evento visivo.

---

## 36. Criteri di non validità

L'implementazione non è valida se:

1. il bridge viene creato in `src`;
2. il bridge viene creato in `models/CicloTimer.Bridge`;
3. il bridge viene creato in `view-models/CicloTimer.Bridge` in questa versione;
4. il bridge viene creato dentro il progetto WPF;
5. il bridge viene creato dentro il progetto core;
6. il bridge contiene logica del timer;
7. il bridge decide quando una sessione è completata;
8. il bridge decrementa il tempo rimanente autonomamente;
9. il bridge incrementa `CompletedSessions`;
10. il bridge chiama `StartTimer` per simulare la ripartenza automatica;
11. il bridge valida logicamente la configurazione al posto del core;
12. il bridge contiene testi utente sparsi non centralizzati;
13. il bridge chiama direttamente WPF;
14. il bridge modifica controlli UI;
15. il bridge usa `MainWindow`;
16. il bridge usa XAML;
17. il bridge usa NVDA;
18. il bridge imposta `AutomationProperties`;
19. il bridge crea Live Region;
20. il bridge riproduce audio;
21. il bridge chiama API Windows;
22. il bridge usa timer reali;
23. il bridge genera tick;
24. il bridge crea `DispatcherTimer`;
25. il bridge crea `System.Timers.Timer`;
26. il bridge accumula eventi come storico permanente;
27. il bridge duplica eventi già gestiti;
28. il bridge sintetizza eventi multipli provenienti da result separati;
29. la UI futura deve tradurre codici tecnici in testi;
30. il core viene modificato per esigenze del bridge senza nuovo design;
31. vengono introdotte funzionalità fuori perimetro.

---

## 37. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. il bridge deve stare in `view-models/TimerBridge/`;
2. non si usa `src`;
3. non si usa `models/CicloTimer.Bridge/`;
4. non si usa `view-models/CicloTimer.Bridge/` in questa versione;
5. il nome concettuale del modello è `TimerDisplayModel`;
6. non viene imposto un wrapper obbligatorio `TimerBridgeResult`;
7. `SystemActionRequest` resta un concetto, non una classe obbligatoria;
8. in caso di più errori, il bridge mostra il primo errore della lista;
9. il formato del tempo resta `mm:ss`;
10. il bridge produce sempre i testi accessibili previsti dal modello;
11. il bridge non possiede e non genera tick;
12. il bridge non mantiene coda storica di eventi;
13. gli eventi multipli vengono sintetizzati solo se presenti nello stesso `TimerCommandResult`.

---

## 38. Criteri minimi di test futuri

Il futuro coding plan dovrà prevedere test per:

1. conversione minuti/secondi in secondi totali;
2. formattazione `0 → 00:00`;
3. formattazione `5 → 00:05`;
4. formattazione `59 → 00:59`;
5. formattazione `60 → 01:00`;
6. formattazione `299 → 04:59`;
7. formattazione `3600 → 60:00`;
8. trasformazione `Stopped → Timer fermo`;
9. trasformazione `Running → Sessione in corso`;
10. trasformazione `FinalAlert → Avviso finale in corso`;
11. trasformazione `Paused → Timer in pausa`;
12. trasformazione errori core in messaggi utente;
13. scelta del primo errore in caso di più errori;
14. trasformazione eventi core in messaggi evento;
15. sintesi evento sessione completata con contatore solo se eventi presenti nello stesso result;
16. mancata sintesi di eventi provenienti da result separati;
17. calcolo `PrimaryActionText`;
18. esposizione `CanStart`, `CanPause`, `CanResume`, `CanReset`;
19. produzione costante di `AccessibleStatusText`;
20. produzione costante di `AccessibleEventText`;
21. assenza di dipendenze WPF;
22. assenza di dipendenze audio;
23. assenza di dipendenze Windows API;
24. assenza di timer reali nel bridge;
25. assenza di generazione autonoma di tick;
26. assenza di modifiche al core;
27. assenza di duplicazione della validazione logica;
28. produzione di richiesta `StartFinalAlertSound` su `FinalAlertStarted`;
29. produzione di richiesta `StopFinalAlertSound` su fine sessione o pausa in avviso finale;
30. mancata produzione di annunci automatici a ogni tick;
31. gestione coerente di result con eventi vuoti;
32. gestione coerente di result con errori.

---

## 39. Stato del documento

Questo documento è approvato come Design 002 — Bridge UI-logica e modello mostrabile del timer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini e consolidamento della collocazione in view-models/TimerBridge/
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT con collocazione vincolante in view-models
0.2.0 — integrazione delle osservazioni dei consiglieri AI: collocazione definitiva view-models/TimerBridge/, divieto di tick generati dal bridge, produzione costante dei testi accessibili, sintesi eventi solo nello stesso TimerCommandResult e conferma del modello TimerDisplayModel
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. chiarimento che il bridge non genera tick e non possiede sorgente temporale;
2. scelta della cartella semplice `view-models/TimerBridge/` per la prima versione;
3. conferma del nome `TimerDisplayModel`;
4. conferma della non obbligatorietà di `TimerBridgeResult`;
5. conferma di `SystemActionRequest` come concetto e non tipo obbligatorio;
6. scelta del primo errore della lista in caso di errori multipli;
7. produzione costante di `AccessibleStatusText` e `AccessibleEventText`;
8. chiarimento che gli eventi multipli possono essere sintetizzati solo se presenti nello stesso `TimerCommandResult`;
9. conferma del formato tempo `mm:ss`;
10. conferma del divieto di UI, XAML, WPF, NVDA, UI Automation, audio reale, API Windows e timer reali nel bridge.

Il documento è approvato dal project owner come base per il successivo coding plan del Bridge UI-logica.
