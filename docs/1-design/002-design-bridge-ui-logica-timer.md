# CicloTimer — Design 002 — Bridge UI-logica e modello mostrabile del timer

**Tipo documento:** documento di design  
**Stato:** APPROVED  
**Versione:** 0.5.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/003-design-sistema-testi-centralizzati.md  

---

## 1. Scopo del documento

Questo documento definisce il design tecnico del Bridge UI-logica del timer di CicloTimer.

Il Design 001 ha definito e isolato il core timer engine.

Il Design 003 ha definito e implementato il sistema centralizzato dei testi e dei messaggi applicativi.

Il core timer engine espone dati neutri, stati neutri, eventi neutri, errori neutri e risultati di comando.

Il sistema `CicloTimer.Localization` espone testi utente centralizzati, chiavi tipizzate e template testuali in italiano, con struttura predisposta per lingue future.

Il Bridge UI-logica ha il compito di collegare i dati neutri del core alla futura UI, usando il sistema centralizzato dei testi, senza contaminare il core e senza trasformare la UI in una seconda logica applicativa.

Il bridge previsto da questo documento deve essere collocato nella cartella fisica già prevista dal progetto:

```text
view-models
````

Poiché il progetto usa C#/.NET, il bridge deve essere realizzato come progetto .NET separato, compilabile e referenziabile dalla solution.

La collocazione fisica corretta è:

```text
view-models/CicloTimer.Bridge/
```

Questa scelta mantiene il bridge dentro la cartella logica `view-models`, ma evita che il codice del bridge diventi una semplice cartella non compilabile autonomamente.

Lo scopo di questo documento è definire:

1. responsabilità del bridge;
2. responsabilità vietate al bridge;
3. relazione del bridge con `CicloTimer.Core`;
4. relazione del bridge con `CicloTimer.Localization`;
5. modello mostrabile del timer;
6. aggiornamento prodotto dal bridge;
7. trasformazione dei dati neutri in dati mostrabili;
8. conversione dei valori provenienti dalla UI;
9. formattazione del tempo rimanente;
10. trasformazione di stati, eventi ed errori in testi utente centralizzati;
11. preparazione dei testi accessibili;
12. richieste concettuali verso il futuro livello audio/sistema operativo;
13. criteri di testabilità del bridge;
14. collocazione fisica corretta nella cartella `view-models`;
15. relazione progettuale con core, localization, UI futura, audio futuro e test.

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
11. gestione audio focus;
12. interruzione o attenuazione dell'audio di altre applicazioni;
13. ripristino dei volumi di altre applicazioni;
14. notifiche Windows concrete;
15. coding plan operativo;
16. todo eseguibile da Cursor.

Questi aspetti saranno trattati in documenti successivi.

---

## 2. Obiettivo del design

L'obiettivo è progettare un bridge semplice, sottile, testabile e compilabile come modulo autonomo.

Il bridge deve ricevere comandi dalla futura UI, chiamare il core e produrre un aggiornamento mostrabile pronto per la UI e per il futuro orchestratore.

Il bridge deve usare `CicloTimer.Localization` per tutti i testi utente.

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
Messaggio evento: Timer avviato.
Messaggio errore: nessuno
```

Il bridge può inoltre produrre richieste concettuali verso un futuro livello audio/sistema operativo.

Esempio:

```text
FinalAlertStarted
↓
Bridge
↓
SystemActionRequest.StartFinalAlertSound
```

Il bridge non deve eseguire direttamente l'audio.

Il bridge non deve fermare direttamente musica, video o altre applicazioni.

Il bridge non deve chiamare API Windows.

Il bridge deve solo produrre richieste concettuali testabili.

Il bridge non deve decidere le regole del timer.

Il bridge non deve calcolare quando una sessione finisce.

Il bridge non deve incrementare il contatore.

Il bridge non deve simulare la ripartenza automatica.

Il bridge deve essere un traduttore controllato tra:

```text
UI futura
↓
Bridge / aggiornamento mostrabile
↓
Core
```

e tra:

```text
Core
↓
Bridge / aggiornamento mostrabile + richieste concettuali
↓
UI futura / orchestratore futuro / audio service futuro
```

---

## 3. Perimetro del design

Questo design può definire:

1. responsabilità del bridge UI-logica;
2. dati che la UI può passare al bridge;
3. comandi che il bridge può inviare al core;
4. dati che il bridge può leggere dal core;
5. chiavi localization che il bridge deve usare;
6. mappature core → localization key;
7. struttura concettuale del modello mostrabile;
8. struttura concettuale dell'aggiornamento prodotto dal bridge;
9. formato del tempo rimanente;
10. regole di conversione minuti/secondi in secondi totali;
11. regole di conversione stati/eventi/errori in testi;
12. richieste concettuali verso il futuro livello audio/sistema operativo;
13. criteri minimi di testabilità del bridge;
14. vincoli per la futura implementazione C#;
15. collocazione nella cartella `view-models`;
16. creazione di un progetto .NET separato per il bridge;
17. dipendenza del bridge dal core;
18. dipendenza del bridge da localization;
19. futura dipendenza della UI dal bridge.

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
10. file audio;
11. audio focus reale;
12. sospensione, muting o attenuazione di altre app;
13. timer reale di sistema;
14. dispatcher timer;
15. thread UI;
16. notifiche Windows;
17. persistenza;
18. storico sessioni;
19. funzioni economiche;
20. massimo numero di sessioni;
21. modifica del core già approvato;
22. modifica del sistema localization già approvato.

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
chiavi tipizzate di CicloTimer.Localization
formattazione mostrabile
richieste concettuali verso il futuro livello audio/sistema operativo
```

Il bridge non deve conoscere:

```text
dettagli XAML
controlli grafici concreti
NVDA come tecnologia diretta
UI Automation concreta
file audio
API Windows
audio focus
regole interne non pubbliche del core
database
cloud
persistenza
```

Il bridge deve restare piccolo.

Il bridge non deve diventare un secondo core.

Il bridge non deve diventare un servizio audio.

Il bridge non deve diventare una UI.

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

Regola di sincronizzazione:

```text
TimerCommandResult descrive cosa è successo nel comando appena eseguito.
TimerEngine descrive qual è lo stato corrente del timer dopo il comando.
```

Quindi, dopo un comando:

1. il bridge usa `TimerCommandResult` per eventi ed errori del comando corrente;
2. il bridge legge il `TimerEngine` per costruire lo stato mostrabile corrente;
3. il bridge non deve mantenere uno stato parallelo che sostituisce il core.

Esempio:

```text
StartTimer restituisce TimerStarted.
Il bridge usa TimerCommandResult.Events per EventMessageText.
Poi legge TimerEngine.CurrentState e TimerEngine.RemainingSeconds per costruire TimerDisplayModel.
```

Il progetto bridge deve dipendere dal progetto core:

```text
CicloTimer.Bridge
↓
CicloTimer.Core
```

Il progetto core non deve dipendere dal bridge.

Il progetto core non deve dipendere da localization.

Il progetto core non deve dipendere dall'audio.

---

## 6. Relazione con CicloTimer.Localization

Il bridge deve dipendere dal progetto:

```text
CicloTimer.Localization
```

Il progetto bridge deve quindi avere queste dipendenze:

```text
CicloTimer.Bridge
  dipende da CicloTimer.Core
  dipende da CicloTimer.Localization
```

Il progetto `CicloTimer.Localization` non deve dipendere dal bridge.

Il progetto `CicloTimer.Localization` non deve dipendere dal core.

La responsabilità di mappare i tipi del core verso le chiavi localization appartiene al bridge.

Esempio corretto:

```text
TimerState.Running
↓
Bridge
↓
TimerTextKey.StateRunning
↓
CicloTimer.Localization
↓
"Sessione in corso"
```

Esempio corretto:

```text
TimerError.InvalidSessionDuration
↓
Bridge
↓
ErrorTextKey.InvalidSessionDuration
↓
CicloTimer.Localization
↓
"La durata della sessione deve essere maggiore di zero."
```

Esempio corretto:

```text
TimerEvent.TimerStarted
↓
Bridge
↓
TimerTextKey.EventTimerStarted
↓
CicloTimer.Localization
↓
"Timer avviato."
```

Il bridge non deve usare stringhe libere come chiavi.

Esempi vietati:

```text
GetText("Running")
GetText("InvalidSessionDuration")
GetText("TimerStarted")
```

Il bridge non deve usare `ToString()` sugli enum del core per generare chiavi.

Esempi vietati:

```text
TimerState.Running.ToString()
TimerError.InvalidSessionDuration.ToString()
TimerEvent.TimerStarted.ToString()
```

Il bridge non deve usare reflection sugli enum del core per generare chiavi.

La mappatura deve essere esplicita, deterministica e testabile.

---

## 7. Responsabilità autorizzate del bridge

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
13. trasformare `TimerState` in `TimerTextKey`;
14. trasformare `TimerError` in `ErrorTextKey`;
15. trasformare `TimerEvent` in `TimerTextKey`;
16. richiedere testi a `CicloTimer.Localization`;
17. formattare `RemainingSeconds` in `mm:ss`;
18. formattare `CompletedSessions` in testo mostrabile usando template/testi centralizzati;
19. calcolare il testo dell'azione principale usando `CommandTextKey`;
20. copiare o esporre le disponibilità `CanStart`, `CanPause`, `CanResume`, `CanReset` nel modello mostrabile;
21. preparare un testo accessibile coerente con il testo visivo;
22. preparare richieste concettuali verso il futuro livello audio/sistema operativo.

Il bridge può produrre un aggiornamento mostrabile completo dopo ogni comando ricevuto.

Il bridge può anche produrre un aggiornamento mostrabile a richiesta, senza eseguire un nuovo comando, leggendo lo stato corrente del core.

Il bridge non possiede una sorgente temporale propria.

Il bridge non deve generare tick.

Il bridge non deve avviare timer reali.

Il bridge non deve usare `DispatcherTimer`, `System.Timers.Timer`, thread o loop temporali.

La sorgente dei tick sarà definita in un futuro design UI/orchestrazione.

---

## 8. Responsabilità vietate al bridge

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
17. riprodurre suoni;
18. fermare, abbassare o ripristinare il volume di altre app;
19. chiamare API Windows;
20. conoscere `AutomationProperties`;
21. conoscere Live Region;
22. contenere stringhe utente hardcoded sparse;
23. usare stringhe libere come chiavi localization;
24. usare `ToString()` sugli enum del core per generare chiavi localization;
25. usare reflection sugli enum del core per generare chiavi localization;
26. contenere logica UI concreta;
27. modificare controlli grafici;
28. aprire finestre;
29. accedere a database;
30. gestire storico sessioni.

Il bridge deve restare deterministico e testabile.

---

## 9. Interfaccia concettuale del bridge

Questo design non impone ancora firme C# definitive, ma definisce i comandi concettuali che il bridge dovrà esporre o supportare nel coding plan.

Metodi concettuali:

```text
Configure(sessionMinutes, sessionSeconds, finalAlertMinutes, finalAlertSeconds)
Start()
Pause()
Resume()
Reset()
Tick(elapsedSeconds)
GetCurrentUpdate()
```

Significato:

```text
Configure(...)
Converte minuti/secondi in secondi totali e chiama ConfigureTimer del core.

Start()
Chiama StartTimer del core.

Pause()
Chiama PauseTimer del core.

Resume()
Chiama ResumeTimer del core.

Reset()
Chiama ResetTimer del core.

Tick(elapsedSeconds)
Inoltra il tick al core. Il bridge non genera il tick.

GetCurrentUpdate()
Produce un TimerBridgeUpdate leggendo lo stato corrente del core, senza eseguire nuovi comandi.
```

Ogni metodo che esegue un comando deve restituire concettualmente un `TimerBridgeUpdate`.

Il coding plan potrà definire i nomi C# definitivi, ma non deve cambiare le responsabilità.

---

## 10. Input provenienti dalla UI

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

## 11. Validazione UI e validazione core

La UI può fare controlli minimi di formato.

Esempi di controlli UI ammessi:

```text
campo vuoto
caratteri non numerici
valore non convertibile in intero
```

Questi controlli riguardano la leggibilità dell'input, non la validità logica del timer.

Il bridge può ricevere dalla UI valori già convertiti in interi, oppure può aiutare a convertire valori grezzi se il futuro design UI lo richiederà.

Il bridge può produrre errori di conversione o input non parsabile solo se il coding plan li definisce esplicitamente.

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
mappa TimerError.InvalidSessionDuration verso ErrorTextKey.InvalidSessionDuration

Localization:
restituisce il messaggio italiano
```

Esempio scorretto:

```text
Bridge:
decide autonomamente che SessionDurationSeconds = 0 è logicamente non valido
e non chiama il core
```

La validazione logica deve restare nel core.

---

## 12. Modello mostrabile del timer

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

Se non ci sono errori nel comando corrente:

```text
ErrorMessageText = string.Empty
```

Se non ci sono eventi rilevanti nel comando corrente:

```text
EventMessageText = string.Empty
AccessibleEventText = string.Empty
```

Il modello mostrabile contiene testi finali e quindi non appartiene al core.

La UI riceve il modello mostrabile e lo espone.

La UI non deve calcolare o tradurre autonomamente i campi del modello.

---

## 13. Aggiornamento prodotto dal bridge

Il bridge deve restituire sempre un aggiornamento concettuale unico.

Nome concettuale:

```text
TimerBridgeUpdate
```

Contenuto concettuale obbligatorio:

```text
TimerDisplayModel DisplayModel
IReadOnlyList<SystemActionRequest> SystemActions
```

Regola obbligatoria:

```text
ogni comando del bridge restituisce TimerBridgeUpdate
```

Se non ci sono richieste verso il futuro livello audio/sistema operativo:

```text
SystemActions = lista vuota
```

Questo evita ambiguità tra comandi che producono solo modello e comandi che producono modello più azioni.

Il coding plan definirà la forma C# concreta più semplice.

Non sono ammissibili in questa fase:

1. event bus;
2. observer complessi;
3. code persistenti;
4. `Channel`;
5. eventi C# come meccanismo principale;
6. dipendenze MVVM/WPF;
7. `INotifyPropertyChanged`;
8. `ICommand`.

La scelta di `TimerBridgeUpdate` mantiene il bridge deterministico e facilmente testabile.

---

## 14. Richieste concettuali audio/sistema

Il bridge può produrre richieste concettuali verso un futuro livello audio/sistema operativo.

Nome concettuale:

```text
SystemActionRequest
```

Valori concettuali minimi:

```text
StartFinalAlertSound
StopFinalAlertSound
```

È ammessa una lista vuota quando non ci sono azioni da eseguire.

Forma consigliata:

```text
IReadOnlyList<SystemActionRequest>
```

Il bridge non deve eseguire queste richieste.

Il bridge non deve sapere come il futuro audio service le implementerà.

Il futuro audio service potrà occuparsi di:

```text
riproduzione del suono
loop del suono
stop del suono
audio focus
attenuazione o sospensione audio di altre app
ripristino audio di altre app
gestione errori audio
API Windows
```

Tutto questo è fuori dal bridge.

Il bridge produce solo segnali concettuali.

---

## 15. Testi utente centralizzati

Il bridge deve usare esclusivamente `CicloTimer.Localization` per i testi utente statici e i template testuali.

Sono testi statici da ottenere tramite localization:

```text
Avvia
Pausa
Riprendi
Reset
Configura
Timer fermo
Sessione in corso
Avviso finale in corso
Timer in pausa
Timer configurato.
Timer avviato.
Timer ripreso.
Timer resettato.
Avviso finale iniziato.
Sessione completata.
Sessioni completate aggiornate.
Nuova sessione avviata.
Configurazione o comando non valido.
La durata della sessione deve essere maggiore di zero.
Tempo rimanente: {0}. {1}. {2}.
Sessione completata. Sessioni completate: {0}.
Errore: {0}
```

Il bridge può combinare testi statici centralizzati con dati dinamici.

Esempio corretto:

```text
Localization:
"Tempo rimanente: {0}. {1}. {2}."

Dati dinamici:
"04:59"
"Sessione in corso"
"Sessioni completate: 3"

Bridge:
AccessibleStatusText = "Tempo rimanente: 04:59. Sessione in corso. Sessioni completate: 3."
```

Esempio scorretto:

```text
Bridge:
return "Sessione in corso";
```

Altro esempio scorretto:

```text
UI:
se CurrentState == Running allora mostra "Sessione in corso"
```

La trasformazione appartiene al bridge, ma i testi appartengono a localization.

---

## 16. Formattazione del tempo

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

## 17. Trasformazione degli stati

Il bridge deve trasformare gli stati neutri in chiavi localization.

Mappatura concettuale minima:

```text
Stopped     → TimerTextKey.StateStopped
Running     → TimerTextKey.StateRunning
FinalAlert  → TimerTextKey.StateFinalAlert
Paused      → TimerTextKey.StatePaused
```

Il testo finale deve provenire da `CicloTimer.Localization`.

La UI non deve contenere questa mappatura.

Il core non deve contenere questa mappatura.

Il bridge è responsabile della trasformazione core enum → localization key.

---

## 18. Trasformazione degli errori

Il bridge deve trasformare gli errori neutri del core in chiavi localization.

Mappatura concettuale minima:

```text
InvalidSessionDuration
→ ErrorTextKey.InvalidSessionDuration

InvalidFinalAlertDuration
→ ErrorTextKey.InvalidFinalAlertDuration

FinalAlertNotLessThanSessionDuration
→ ErrorTextKey.FinalAlertNotLessThanSessionDuration

TimerNotConfigured
→ ErrorTextKey.TimerNotConfigured

CannotStart
→ ErrorTextKey.CannotStart

CannotPause
→ ErrorTextKey.CannotPause

CannotResume
→ ErrorTextKey.CannotResume

CannotReset
→ ErrorTextKey.CannotReset

InvalidTickDuration
→ ErrorTextKey.InvalidTickDuration
```

Il bridge deve poter ricevere più errori nello stesso risultato.

Regola obbligatoria per la prima versione:

```text
in presenza di più errori, il bridge mostra il primo errore della lista,
secondo l'ordine restituito dal core
```

Questa scelta è semplice, deterministica e testabile.

Il bridge non deve mostrare eccezioni tecniche grezze.

Il bridge non deve generare testi di errore propri.

---

## 19. Trasformazione degli eventi

Il bridge deve trasformare gli eventi neutri del core in chiavi localization.

Mappatura concettuale minima:

```text
TimerConfigured
→ TimerTextKey.EventTimerConfigured

TimerStarted
→ TimerTextKey.EventTimerStarted

TimerPaused
→ TimerTextKey.EventTimerPaused

TimerResumed
→ TimerTextKey.EventTimerResumed

TimerReset
→ TimerTextKey.EventTimerReset

FinalAlertStarted
→ TimerTextKey.EventFinalAlertStarted

SessionCompleted
→ TimerTextKey.EventSessionCompleted

SessionCounterIncremented
→ TimerTextKey.EventSessionCounterIncremented

NextSessionStarted
→ TimerTextKey.EventNextSessionStarted

ValidationFailed
→ TimerTextKey.EventValidationFailed
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

Per il messaggio utente finale, il bridge può sintetizzare più eventi collegati nello stesso result in un solo messaggio usando un template di localization.

Template da usare:

```text
AccessibilityTextKey.SessionCompletedTemplate
```

oppure chiave equivalente già presente in `CicloTimer.Localization`.

Esempio:

```text
AccessibilityTextKey.SessionCompletedTemplate
+
CompletedSessions = 3
↓
"Sessione completata. Sessioni completate: 3."
```

La sintesi di eventi multipli in un unico messaggio è ammessa solo quando gli eventi sono presenti insieme nello stesso `TimerCommandResult` del comando corrente.

Se nel result del comando corrente è presente un solo evento, il bridge gestisce solo quell'evento.

Se eventi collegati arrivano in result separati, il bridge non deve accumularli artificialmente e non deve mantenere una coda storica per ricostruire un messaggio combinato.

Se non ci sono eventi nel result corrente:

```text
EventMessageText = string.Empty
AccessibleEventText = string.Empty
```

---

## 20. Azione principale

La UI futura potrebbe avere un pulsante principale che cambia testo in base allo stato.

Il bridge può calcolare `PrimaryActionText`.

Mappatura concettuale:

```text
Stopped + CanStart = true
→ CommandTextKey.Start

Running + CanPause = true
→ CommandTextKey.Pause

FinalAlert + CanPause = true
→ CommandTextKey.Pause

Paused + CanResume = true
→ CommandTextKey.Resume
```

Il testo finale deve arrivare da `CicloTimer.Localization`.

Se nessuna azione principale è disponibile, il bridge può restituire testo vuoto.

Questa scelta precisa può essere definita nel coding plan.

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

## 21. Reset e contatore

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

## 22. Avviso finale e richieste audio concettuali

Il bridge può interpretare eventi neutri legati all'avviso finale.

Quando riceve:

```text
FinalAlertStarted
```

può preparare:

```text
TimerStateText = testo da TimerTextKey.StateFinalAlert
EventMessageText = testo da TimerTextKey.EventFinalAlertStarted
SystemActionRequest.StartFinalAlertSound
```

Il bridge può produrre `StopFinalAlertSound` solo quando ha evidenza che l'avviso finale fosse attivo o che il comando corrente stia terminando una fase di avviso finale.

L'evidenza può arrivare da:

1. `TimerEngine.IsFinalAlertActive` prima del comando;
2. `TimerEngine.IsFinalAlertActive` dopo il comando;
3. stato precedente `FinalAlert`;
4. stato corrente `FinalAlert`;
5. eventi di completamento sessione nel result corrente;
6. comando corrente di pausa o reset eseguito mentre l'avviso finale era attivo.

Quando riceve eventi di fine sessione, come:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

può produrre una richiesta concettuale:

```text
SystemActionRequest.StopFinalAlertSound
```

se l'avviso finale era attivo o se il result corrente indica conclusione della sessione partita da avviso finale.

Quando riceve:

```text
TimerPaused
```

e lo stato precedente o corrente indica che l'avviso finale era attivo, può produrre una richiesta concettuale:

```text
SystemActionRequest.StopFinalAlertSound
```

Quando riceve:

```text
TimerReset
```

e lo stato precedente o corrente indica che l'avviso finale era attivo, può produrre una richiesta concettuale:

```text
SystemActionRequest.StopFinalAlertSound
```

Quando riceve nello stesso result:

```text
TimerResumed
FinalAlertStarted
```

può produrre una richiesta concettuale:

```text
SystemActionRequest.StartFinalAlertSound
```

Questo design non implementa audio.

Questo design non sceglie librerie audio.

Questo design non chiama API Windows.

Questo design non ferma davvero musica o video di altre app.

Questo design non gestisce volumi di sistema.

Questo design definisce solo le richieste concettuali che il bridge potrà restituire a un orchestratore o audio service futuro.

---

## 23. Richieste concettuali verso il livello audio/sistema operativo

Il bridge può produrre richieste neutre verso il futuro livello audio/sistema operativo.

Nome concettuale:

```text
SystemActionRequest
```

Valori concettuali minimi:

```text
StartFinalAlertSound
StopFinalAlertSound
```

Questi nomi definiscono il contratto concettuale, non obbligano ancora a una forma C# definitiva.

Il coding plan potrà decidere la forma concreta più semplice, per esempio:

1. enum;
2. lista di enum;
3. proprietà su `TimerBridgeUpdate`.

Il design definitivo del livello audio/sistema operativo sarà affrontato in un design successivo.

Il bridge non deve eseguire direttamente queste richieste.

Il bridge può solo restituirle o esporle dentro `TimerBridgeUpdate.SystemActions`.

Regola obbligatoria:

```text
il bridge non deve chiamare direttamente API Windows
```

Il futuro audio service dovrà decidere:

1. file audio;
2. loop;
3. volume;
4. priorità audio;
5. attenuazione/interruzione di altre app;
6. ripristino di altre app;
7. gestione degli errori audio;
8. limiti tecnici di Windows.

---

## 24. Testi accessibili

Il bridge deve preparare testi accessibili coerenti con i testi visivi usando `CicloTimer.Localization`.

Per la prima versione il bridge deve produrre sempre i testi accessibili previsti dal modello, anche quando coincidono con i testi visivi.

Questa scelta rende il comportamento più semplice, più prevedibile e più testabile.

Esempio:

```text
TimerStateText = "Sessione in corso"
AccessibleStatusText = "Tempo rimanente: 04:59. Sessione in corso. Sessioni completate: 3."
```

Esempio:

```text
EventMessageText = "Sessione completata. Sessioni completate: 3."
AccessibleEventText = "Sessione completata. Sessioni completate: 3."
```

Se non ci sono eventi rilevanti nel result corrente:

```text
AccessibleEventText = string.Empty
```

Il bridge non deve usare API NVDA.

Il bridge non deve impostare `AutomationProperties`.

Il bridge non deve creare Live Region.

Il bridge prepara solo testi coerenti.

La futura UI deciderà come esporli tramite strumenti accessibili della piattaforma.

Il tempo rimanente deve essere consultabile, ma non annunciato automaticamente ogni secondo.

Il bridge può produrre:

```text
RemainingTimeText = "04:59"
```

e:

```text
AccessibleStatusText = "Tempo rimanente: 04:59. Sessione in corso. Sessioni completate: 3."
```

ma non deve generare un evento accessibile automatico per ogni tick.

La decisione tecnica su Live Region o altri meccanismi sarà nel design UI/accessibilità operativa.

---

## 25. Output del bridge

Il bridge deve produrre sempre:

```text
TimerBridgeUpdate
```

`TimerBridgeUpdate` contiene:

```text
TimerDisplayModel DisplayModel
IReadOnlyList<SystemActionRequest> SystemActions
```

Regole:

1. `DisplayModel` deve essere sempre presente;
2. `SystemActions` deve essere sempre presente;
3. se non ci sono azioni concettuali, `SystemActions` deve essere lista vuota;
4. non usare `null` per rappresentare assenza di azioni;
5. non usare eventi C# come meccanismo principale;
6. non usare code persistenti;
7. non usare `Channel`;
8. non usare event bus.

Regola obbligatoria:

```text
dopo ogni comando la UI o l'orchestratore futuro deve poter ricevere un TimerBridgeUpdate coerente
```

---

## 26. Sequenza: configurazione valida

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
Bridge usa TimerCommandResult per eventi/errori
↓
Bridge legge TimerEngine per stato corrente
↓
Bridge produce TimerBridgeUpdate
↓
UI può mostrare:
Tempo rimanente: 05:00
Timer fermo
Sessioni completate: 0
```

Il bridge non decide se 300 e 20 sono logicamente validi.

Lo decide il core.

---

## 27. Sequenza: configurazione non valida

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
Bridge legge TimerCommandResult.Errors
↓
Bridge mappa il primo errore verso ErrorTextKey.InvalidSessionDuration
↓
Localization restituisce il messaggio utente
↓
Bridge legge TimerEngine per stato corrente
↓
Bridge produce TimerBridgeUpdate con ErrorMessageText
↓
UI mostra errore già pronto
```

Esempio messaggio:

```text
La durata della sessione deve essere maggiore di zero.
```

Il messaggio non deve essere generato dal core.

---

## 28. Sequenza: avvio timer

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
Bridge usa TimerCommandResult.Events per EventMessageText
↓
Bridge legge TimerEngine.CurrentState e RemainingSeconds
↓
Bridge mappa Running verso TimerTextKey.StateRunning
↓
Bridge mappa TimerStarted verso TimerTextKey.EventTimerStarted
↓
Localization restituisce i testi
↓
Bridge produce TimerBridgeUpdate
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

## 29. Sequenza: tick

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
bridge usa TimerCommandResult per eventi/errori del tick
↓
bridge legge TimerEngine per stato corrente
↓
bridge formatta RemainingTimeText
↓
bridge produce TimerBridgeUpdate
↓
UI può aggiornare il testo mostrato
↓
orchestratore futuro può leggere eventuali SystemActions
```

Il tick non è un comando utente.

Il bridge non deve esporre tick come pulsante.

Il bridge non deve creare annunci accessibili automatici a ogni tick.

Il bridge non deve decidere se la sessione è completata.

Il bridge non possiede la sorgente temporale.

Il bridge non genera tick.

Il bridge non deve creare `DispatcherTimer`, `System.Timers.Timer`, thread o loop temporali.

---

## 30. Sequenza: ingresso in avviso finale

Sequenza concettuale:

```text
core riceve Tick
↓
core rileva ingresso in FinalAlert
↓
core espone FinalAlertStarted
↓
bridge riceve FinalAlertStarted nel TimerCommandResult
↓
bridge legge TimerEngine per stato corrente
↓
bridge prepara testi tramite localization
↓
bridge prepara richiesta SystemActionRequest.StartFinalAlertSound
↓
bridge produce TimerBridgeUpdate con DisplayModel + SystemActions
```

Il bridge non calcola la soglia di avviso.

Il bridge non decide quando parte l'avviso.

Il bridge non riproduce direttamente il suono.

---

## 31. Sequenza: pausa durante avviso finale

Sequenza concettuale:

```text
timer è in FinalAlert
↓
UI invia Pausa
↓
bridge rileva che IsFinalAlertActive era true prima del comando
↓
bridge chiama PauseTimer
↓
core passa a Paused
↓
core espone TimerPaused
↓
bridge usa TimerCommandResult per evento TimerPaused
↓
bridge legge TimerEngine per stato corrente
↓
bridge prepara testi tramite localization
↓
bridge produce richiesta SystemActionRequest.StopFinalAlertSound
↓
bridge produce TimerBridgeUpdate con DisplayModel + SystemActions
```

Il bridge non deve perdere il tempo rimanente.

Il bridge non deve decidere se la sessione resta nella finestra finale.

Il tempo rimanente resta nel core.

---

## 32. Sequenza: ripresa dentro avviso finale

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
bridge usa TimerCommandResult per eventi
↓
bridge legge TimerEngine per stato corrente
↓
bridge prepara testi tramite localization
↓
bridge produce richiesta SystemActionRequest.StartFinalAlertSound
↓
bridge produce TimerBridgeUpdate con DisplayModel + SystemActions
```

Il bridge non deve inventare `FinalAlertStarted`.

Il bridge deve reagire all'evento o alla transizione esposta dal core.

---

## 33. Sequenza: fine sessione e ripartenza automatica

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
bridge prepara messaggio tramite template localization:
"Sessione completata. Sessioni completate: 3."
↓
bridge produce eventuale richiesta SystemActionRequest.StopFinalAlertSound
↓
bridge legge stato aggiornato dal TimerEngine
↓
bridge produce TimerBridgeUpdate con DisplayModel + SystemActions
```

Il bridge non deve chiamare `StartTimer`.

La ripartenza automatica appartiene al core.

Il bridge non deve mantenere una coda storica degli eventi.

La sintesi del messaggio "Sessione completata. Sessioni completate: 3." è valida solo se gli eventi collegati sono presenti insieme nello stesso `TimerCommandResult`.

---

## 34. Sequenza: reset

Sequenza concettuale:

```text
UI invia Reset
↓
bridge rileva se IsFinalAlertActive era true prima del comando
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
bridge usa TimerCommandResult per evento TimerReset
↓
bridge legge TimerEngine per stato corrente
↓
bridge prepara "Timer resettato." tramite localization
↓
se l'avviso finale era attivo prima del reset, produce StopFinalAlertSound
↓
bridge produce TimerBridgeUpdate
```

Il bridge non deve azzerare il contatore.

Il bridge non deve simulare il reset.

Il bridge non deve generare eventi di ciclo.

---

## 35. Sequenza: stato corrente senza comando

Sequenza concettuale:

```text
UI futura o orchestratore futuro richiede stato corrente
↓
Bridge non chiama nuovi comandi core
↓
Bridge legge TimerEngine
↓
Bridge produce TimerDisplayModel
↓
Bridge restituisce TimerBridgeUpdate con SystemActions vuota
```

Questa sequenza è utile per inizializzare la UI futura o aggiornare la visualizzazione senza eseguire azioni.

---

## 36. Testabilità del bridge

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

```text
Events = [FinalAlertStarted]
↓
SystemActions contiene StartFinalAlertSound
```

```text
Events = [SessionCompleted, SessionCounterIncremented, NextSessionStarted]
↓
SystemActions contiene StopFinalAlertSound
```

```text
Events = []
↓
EventMessageText = string.Empty
AccessibleEventText = string.Empty
SystemActions = lista vuota
```

I test devono essere deterministici.

---

## 37. Dipendenze del bridge

Il bridge può dipendere da:

1. `CicloTimer.Core`;
2. `CicloTimer.Localization`;
3. eventuali tipi interni del modello mostrabile;
4. eventuali enum interni di richieste concettuali audio/sistema.

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

Il bridge deve essere un progetto .NET separato e deve usare un target non Windows-specifico, coerente con il core e localization.

Target previsto:

```text
net9.0
```

Il bridge non deve usare:

```text
net9.0-windows
```

---

## 38. Collocazione fisica obbligatoria

La struttura fisica del progetto è fissata dal project owner e non deve essere reinterpretata dagli agenti.

La logica core già implementata si trova in:

```text
models/CicloTimer.Core/
```

Il sistema localization già implementato si trova in:

```text
locales/CicloTimer.Localization/
```

Il bridge / modello mostrabile deve invece essere collocato in:

```text
view-models/CicloTimer.Bridge/
```

Il progetto bridge deve avere un proprio file `.csproj`:

```text
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
```

La struttura corretta è:

```text
models/
  CicloTimer.Core/
    CicloTimer.Core.csproj

locales/
  CicloTimer.Localization/
    CicloTimer.Localization.csproj

view-models/
  CicloTimer.Bridge/
    CicloTimer.Bridge.csproj
```

Regola vincolante:

```text
non creare src/
non creare models/CicloTimer.Bridge/
non creare locales/CicloTimer.Bridge/
non creare view-models/TimerBridge/ come semplice cartella di codice non progettuale
non collocare il bridge nella UI WPF
non collocare il bridge nel progetto core
non collocare il bridge nel progetto localization
```

Il progetto `CicloTimer.Bridge` deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

Il progetto `CicloTimer.Core` non deve referenziare il bridge.

Il progetto `CicloTimer.Localization` non deve referenziare il bridge.

Il progetto WPF non deve ancora essere modificato in questo design.

In futuro, quando sarà progettata la UI, il progetto WPF potrà referenziare `CicloTimer.Bridge`.

I test del bridge dovranno essere collocati in:

```text
tests/CicloTimer.Bridge.Tests/
```

e dovranno referenziare il progetto bridge, non il progetto WPF.

---

## 39. Relazione con i futuri design

Questo design prepara ma non implementa:

1. UI WPF minima;
2. accessibilità operativa;
3. audio dell'avviso finale;
4. audio focus;
5. interruzione, attenuazione o ripristino audio di altre applicazioni;
6. sorgente temporale reale;
7. orchestratore;
8. collegamento effettivo con `MainWindow`.

Sequenza logica futura aggiornata:

```text
Design 001 — Core timer engine
↓
Design 003 — Sistema centralizzato testi e messaggi applicativi
↓
Design 002 — Bridge UI-logica e modello mostrabile
↓
Design 004 — Audio service / audio focus / avviso finale
↓
Design 005 — UI WPF minima accessibile
↓
Design 006 — Integrazione timer reale / orchestrazione
```

La numerazione reale resta quella dei file già creati, ma la sequenza logica di implementazione è:

```text
core
↓
localization
↓
bridge
↓
audio service
↓
UI
↓
orchestrazione finale
```

La catena di dipendenze prevista a regime è:

```text
ciclotimer WPF
↓
CicloTimer.Bridge
↓
CicloTimer.Core
CicloTimer.Localization
```

Il futuro audio service non deve essere dipendenza diretta obbligatoria del bridge.

La relazione corretta sarà:

```text
Bridge produce SystemActionRequest
↓
Orchestratore futuro legge SystemActionRequest
↓
Audio service futuro esegue l'azione
```

La catena di test prevista è:

```text
CicloTimer.Core.Tests
↓
CicloTimer.Core
```

```text
CicloTimer.Localization.Tests
↓
CicloTimer.Localization
```

```text
CicloTimer.Bridge.Tests
↓
CicloTimer.Bridge
↓
CicloTimer.Core
↓
CicloTimer.Localization
```

---

## 40. Criteri di validità

Il design sarà rispettato se una futura implementazione:

1. crea il bridge dentro `view-models/CicloTimer.Bridge/`;
2. crea `view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj`;
3. non crea `src`;
4. non crea `models/CicloTimer.Bridge`;
5. non crea `locales/CicloTimer.Bridge`;
6. non crea `view-models/TimerBridge/` come semplice cartella di codice non progettuale;
7. mantiene il core indipendente dal bridge;
8. mantiene localization indipendente dal bridge;
9. fa dipendere il bridge dal core;
10. fa dipendere il bridge da localization;
11. non modifica il comportamento del core;
12. non modifica il comportamento di localization;
13. converte minuti e secondi UI in secondi totali;
14. invia configurazioni neutre al core;
15. usa `TimerCommandResult` per eventi/errori del comando corrente;
16. usa `TimerEngine` come fonte primaria dello stato corrente;
17. formatta `RemainingSeconds` fuori dal core;
18. produce `RemainingTimeText`;
19. produce `TimerStateText`;
20. produce `CompletedSessionsText`;
21. produce sempre `TimerBridgeUpdate`;
22. produce sempre `TimerDisplayModel`;
23. produce sempre `SystemActions`, anche come lista vuota;
24. produce messaggi errore da errori neutri usando localization;
25. produce messaggi evento da eventi neutri usando localization;
26. usa testi statici centralizzati da `CicloTimer.Localization`;
27. non inserisce stringhe utente hardcoded sparse;
28. non usa stringhe libere come chiavi localization;
29. non usa `ToString()` sugli enum core per generare chiavi;
30. non usa reflection sugli enum core per generare chiavi;
31. non duplica la validazione logica;
32. non incrementa il contatore fuori dal core;
33. non simula la ripartenza automatica;
34. non possiede una sorgente temporale;
35. non genera tick;
36. non chiama direttamente API Windows;
37. non riproduce audio reale;
38. non gestisce audio focus reale;
39. non ferma o attenua audio di altre app;
40. non usa WPF;
41. non usa XAML;
42. non usa NVDA;
43. non usa UI Automation;
44. è testabile senza UI;
45. produce un aggiornamento mostrabile coerente dopo ogni comando;
46. produce richieste concettuali audio/sistema quando necessario;
47. produce `StartFinalAlertSound` su `FinalAlertStarted`;
48. produce `StopFinalAlertSound` solo con evidenza di avviso finale attivo o sessione in chiusura;
49. mantiene eventi non cumulativi e ordinati;
50. sintetizza eventi multipli solo se presenti nello stesso `TimerCommandResult`;
51. usa `AccessibilityTextKey.SessionCompletedTemplate` o chiave equivalente per la sintesi sessione completata;
52. non gestisce lo stesso evento tramite più canali;
53. usa il primo errore della lista in caso di errori multipli;
54. produce sempre `AccessibleStatusText`;
55. produce `AccessibleEventText` vuoto se non ci sono eventi;
56. crea test bridge in `tests/CicloTimer.Bridge.Tests/`;
57. i test del bridge referenziano il progetto bridge e non il progetto WPF;
58. i test del bridge verificano assenza di dipendenze WPF/audio/API Windows.

---

## 41. Criteri di non validità

L'implementazione non è valida se:

1. il bridge viene creato in `src`;
2. il bridge viene creato in `models/CicloTimer.Bridge`;
3. il bridge viene creato in `locales/CicloTimer.Bridge`;
4. il bridge viene creato come semplice cartella `view-models/TimerBridge/` priva di progetto `.csproj`;
5. il bridge viene creato dentro il progetto WPF;
6. il bridge viene creato dentro il progetto core;
7. il bridge viene creato dentro il progetto localization;
8. il bridge contiene logica del timer;
9. il bridge decide quando una sessione è completata;
10. il bridge decrementa il tempo rimanente autonomamente;
11. il bridge incrementa `CompletedSessions`;
12. il bridge chiama `StartTimer` per simulare la ripartenza automatica;
13. il bridge valida logicamente la configurazione al posto del core;
14. il bridge contiene testi utente sparsi non centralizzati;
15. il bridge usa stringhe libere come chiavi localization;
16. il bridge usa `ToString()` sugli enum core per generare chiavi localization;
17. il bridge usa reflection sugli enum core per generare chiavi localization;
18. il bridge chiama direttamente WPF;
19. il bridge modifica controlli UI;
20. il bridge usa `MainWindow`;
21. il bridge usa XAML;
22. il bridge usa NVDA;
23. il bridge imposta `AutomationProperties`;
24. il bridge crea Live Region;
25. il bridge riproduce audio;
26. il bridge gestisce audio focus;
27. il bridge ferma o attenua audio di altre app;
28. il bridge chiama API Windows;
29. il bridge usa timer reali;
30. il bridge genera tick;
31. il bridge crea `DispatcherTimer`;
32. il bridge crea `System.Timers.Timer`;
33. il bridge accumula eventi come storico permanente;
34. il bridge duplica eventi già gestiti;
35. il bridge sintetizza eventi multipli provenienti da result separati;
36. il bridge usa event bus, code persistenti o `Channel` per le azioni audio;
37. il bridge restituisce `null` per `SystemActions`;
38. la UI futura deve tradurre codici tecnici in testi;
39. il core viene modificato per esigenze del bridge senza nuovo design;
40. localization viene modificato per esigenze del bridge senza nuovo design;
41. il progetto bridge usa `net9.0-windows`;
42. i test del bridge referenziano il progetto WPF invece del progetto bridge;
43. vengono introdotte funzionalità fuori perimetro.

---

## 42. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. il bridge deve stare in `view-models/CicloTimer.Bridge/`;
2. il bridge deve essere un progetto .NET separato con proprio `.csproj`;
3. non si usa `src`;
4. non si usa `models/CicloTimer.Bridge/`;
5. non si usa `locales/CicloTimer.Bridge/`;
6. non si usa `view-models/TimerBridge/` come semplice cartella non progettuale;
7. il progetto bridge usa target `net9.0`;
8. il progetto bridge referenzia `CicloTimer.Core`;
9. il progetto bridge referenzia `CicloTimer.Localization`;
10. il progetto core non referenzia il bridge;
11. il progetto localization non referenzia il bridge;
12. i test bridge referenziano il progetto bridge;
13. la UI WPF referenzierà il bridge solo in un design successivo;
14. il nome concettuale del modello è `TimerDisplayModel`;
15. il bridge restituisce sempre `TimerBridgeUpdate`;
16. `TimerBridgeUpdate` contiene `TimerDisplayModel` e `SystemActions`;
17. `SystemActions` è lista vuota quando non ci sono richieste;
18. `SystemActionRequest` resta concettuale fino al coding plan;
19. in caso di più errori, il bridge mostra il primo errore della lista;
20. il formato del tempo resta `mm:ss`;
21. il bridge usa `TimerCommandResult` per eventi/errori del comando corrente;
22. il bridge usa `TimerEngine` come fonte primaria dello stato corrente;
23. il bridge produce sempre i testi accessibili previsti dal modello;
24. `EventMessageText` e `AccessibleEventText` sono vuoti quando non ci sono eventi;
25. il bridge non possiede e non genera tick;
26. il bridge non mantiene coda storica di eventi;
27. gli eventi multipli vengono sintetizzati solo se presenti nello stesso `TimerCommandResult`;
28. il bridge usa il template localization per la sintesi sessione completata;
29. il bridge può produrre azioni concettuali `StartFinalAlertSound` e `StopFinalAlertSound`;
30. `StopFinalAlertSound` richiede evidenza di avviso finale attivo o sessione in chiusura;
31. il bridge non esegue audio reale;
32. il bridge non gestisce audio focus;
33. il bridge non chiama API Windows;
34. il bridge espone concettualmente `Configure`, `Start`, `Pause`, `Resume`, `Reset`, `Tick`, `GetCurrentUpdate`.

---

## 43. Criteri minimi di test futuri

Il futuro coding plan dovrà prevedere test per:

1. creazione progetto bridge in `view-models/CicloTimer.Bridge/`;
2. target `net9.0`;
3. dipendenza da `CicloTimer.Core`;
4. dipendenza da `CicloTimer.Localization`;
5. assenza di dipendenza da WPF;
6. assenza di dipendenza da API Windows;
7. assenza di dipendenza da audio reale;
8. conversione minuti/secondi in secondi totali;
9. formattazione `0 → 00:00`;
10. formattazione `5 → 00:05`;
11. formattazione `59 → 00:59`;
12. formattazione `60 → 01:00`;
13. formattazione `299 → 04:59`;
14. formattazione `3600 → 60:00`;
15. trasformazione `Stopped → TimerTextKey.StateStopped → Timer fermo`;
16. trasformazione `Running → TimerTextKey.StateRunning → Sessione in corso`;
17. trasformazione `FinalAlert → TimerTextKey.StateFinalAlert → Avviso finale in corso`;
18. trasformazione `Paused → TimerTextKey.StatePaused → Timer in pausa`;
19. trasformazione errori core in messaggi utente via `ErrorTextKey`;
20. scelta del primo errore in caso di più errori;
21. trasformazione eventi core in messaggi evento via `TimerTextKey`;
22. sintesi evento sessione completata con contatore solo se eventi presenti nello stesso result;
23. mancata sintesi di eventi provenienti da result separati;
24. uso di `AccessibilityTextKey.SessionCompletedTemplate` o chiave equivalente;
25. calcolo `PrimaryActionText` via `CommandTextKey`;
26. esposizione `CanStart`, `CanPause`, `CanResume`, `CanReset`;
27. produzione costante di `AccessibleStatusText`;
28. produzione di `AccessibleEventText` vuoto quando non ci sono eventi;
29. produzione costante di `TimerBridgeUpdate`;
30. produzione di `SystemActions` come lista vuota quando non ci sono richieste;
31. assenza di stringhe utente hardcoded nel bridge;
32. assenza di stringhe libere come chiavi localization;
33. assenza di `ToString()` sugli enum core per chiavi localization;
34. assenza di reflection sugli enum core per chiavi localization;
35. assenza di timer reali nel bridge;
36. assenza di generazione autonoma di tick;
37. assenza di modifiche al core;
38. assenza di modifiche a localization;
39. produzione di richiesta `StartFinalAlertSound` su `FinalAlertStarted`;
40. produzione di richiesta `StopFinalAlertSound` su fine sessione;
41. produzione di richiesta `StopFinalAlertSound` su pausa durante avviso finale;
42. produzione di richiesta `StopFinalAlertSound` su reset durante avviso finale;
43. non produzione di `StopFinalAlertSound` quando non c'è evidenza di avviso finale attivo;
44. mancata produzione di annunci automatici a ogni tick;
45. gestione coerente di result con eventi vuoti;
46. gestione coerente di result con errori;
47. corretta dipendenza `CicloTimer.Bridge → CicloTimer.Core`;
48. corretta dipendenza `CicloTimer.Bridge → CicloTimer.Localization`;
49. assenza di dipendenza da progetto WPF nei test bridge.

---

## 44. Stato del documento

Questo documento è approvato come Design 002 — Bridge UI-logica e modello mostrabile del timer.

Versione corrente:

```text
0.5.0 — approvazione dopo revisione DeepSeek/Gemini su integrazione Localization e azioni audio concettuali
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT con collocazione vincolante in view-models
0.2.0 — integrazione delle osservazioni dei consiglieri AI: collocazione view-models/TimerBridge/, divieto di tick generati dal bridge, produzione costante dei testi accessibili, sintesi eventi solo nello stesso TimerCommandResult e conferma del modello TimerDisplayModel
0.3.0 — revisione dopo chiarimento su struttura C#/.NET: il bridge diventa progetto separato in view-models/CicloTimer.Bridge/ con dipendenza da CicloTimer.Core
0.4.0 — revisione dopo Design 003: il bridge usa CicloTimer.Localization, vieta stringhe hardcoded/stringhe magiche/ToString/reflection e produce richieste audio concettuali senza implementare audio reale
0.5.0 — integrazione osservazioni consiglieri: TimerBridgeUpdate sempre restituito, SystemActions sempre presente, sincronizzazione TimerCommandResult/TimerEngine chiarita, StopFinalAlertSound prodotto solo con evidenza di avviso finale attivo, template localization esplicitato, EventMessageText e AccessibleEventText vuoti senza eventi
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. chiarimento che il bridge usa `TimerCommandResult` per eventi/errori del comando corrente;
2. chiarimento che il bridge usa `TimerEngine` come fonte primaria dello stato corrente;
3. introduzione di `TimerBridgeUpdate` come output sempre restituito;
4. obbligo di `SystemActions` sempre presente, anche come lista vuota;
5. chiarimento che `StopFinalAlertSound` richiede evidenza di avviso finale attivo o sessione in chiusura;
6. esplicitazione dell'uso di `AccessibilityTextKey.SessionCompletedTemplate` o chiave equivalente per la sintesi sessione completata;
7. chiarimento che `EventMessageText` e `AccessibleEventText` sono `string.Empty` quando non ci sono eventi;
8. aggiunta dell'interfaccia concettuale del bridge;
9. conferma del formato `mm:ss`;
10. esclusione di eventi C#, code persistenti, `Channel`, event bus, audio reale, audio focus e API Windows dal bridge.

Il documento è approvato dal project owner come base per il successivo coding plan del Bridge UI-logica.
