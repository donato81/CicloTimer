# CicloTimer — API interna e contratti tra livelli

**Tipo documento:** contratti interni / API interna  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-01  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md  

---

## 1. Scopo del documento

Questo documento definisce l'API interna concettuale di CicloTimer.

Per API interna non si intende una API web, REST, HTTP o pubblica.

In questo progetto, API interna significa:

```text
insieme controllato di comandi, stati, eventi, errori e dati neutri
che i livelli interni dell'app usano per comunicare tra loro
````

Lo scopo del documento è stabilire un contratto architetturale tra:

1. UI;
2. bridge UI-logica;
3. logica core;
4. testi utente;
5. gestione errori;
6. interazioni con il sistema operativo;
7. test.

Questo documento non definisce ancora:

1. nomi definitivi delle classi C#;
2. firme definitive dei metodi;
3. struttura fisica definitiva delle cartelle;
4. interfacce C# finali;
5. codice;
6. XAML;
7. implementazione audio;
8. implementazione di UI Automation;
9. test automatizzati completi.

Questi dettagli saranno definiti nei documenti di design successivi.

Questo documento stabilisce invece:

1. quali informazioni possono attraversare i confini tra livelli;
2. quali informazioni non devono attraversarli;
3. quali comandi deve poter ricevere la logica core;
4. quali stati deve poter esporre la logica core;
5. quali eventi deve poter produrre la logica core;
6. quali errori neutri devono essere previsti;
7. quali dati dinamici devono essere trattati come valori di stato;
8. quali trasformazioni spettano al bridge;
9. quali trasformazioni non spettano alla UI;
10. quali vincoli devono rispettare gli agenti AI nei design successivi.

---

## 2. Principio guida

Il principio guida dell'API interna è:

```text
la logica core comunica solo tramite dati neutri
```

La logica core non deve comunicare usando testi finali per l'utente.

La logica core non deve conoscere:

1. UI;
2. WPF;
3. XAML;
4. NVDA;
5. UI Automation;
6. audio;
7. notifiche Windows;
8. file di localizzazione;
9. messaggi finali;
10. formattazione visuale.

La logica core deve invece esporre:

1. stati neutri;
2. eventi neutri;
3. errori neutri;
4. valori numerici o strutturati;
5. risultato dei comandi;
6. informazioni sufficienti perché il bridge prepari la rappresentazione utente.

Il bridge è il livello che traduce questi dati neutri in informazioni pronte per UI, accessibilità, testi utente e livello sistema operativo.

---

## 3. Relazione con l'architettura generale

L'architettura generale stabilisce che la logica del timer deve essere indipendente dalla UI e dal sistema operativo.

Questo documento rende quel principio più concreto.

Schema logico:

```text
UI
↓
Bridge UI-logica
↓
Logica core
```

Flusso di ritorno:

```text
Logica core
↓
stati / eventi / errori / dati neutri
↓
Bridge UI-logica
↓
testi, dati formattati, richieste verso UI o sistema operativo
↓
UI / sistema operativo
```

La UI non deve parlare direttamente con dettagli interni della logica core.

Il bridge deve restare sottile.

La logica core deve restare autonoma e testabile senza UI.

---

## 4. Definizione di API interna

Nel progetto CicloTimer, l'API interna comprende:

1. comandi;
2. configurazione del timer;
3. stati stabili;
4. eventi;
5. errori neutri;
6. dati dinamici;
7. risultato delle operazioni;
8. richieste verso il sistema operativo;
9. modello dati mostrabile prodotto dal bridge.

Non fanno parte dell'API interna:

1. endpoint HTTP;
2. controller web;
3. database remoto;
4. serializzazione pubblica;
5. protocolli di rete;
6. plugin esterni;
7. formati di esportazione;
8. integrazioni cloud.

Ogni agente AI deve interpretare `internal-api.md` come documento di confine interno tra livelli, non come richiesta di creare una API pubblica.

---

## 5. Comandi interni del timer

La logica core deve poter ricevere comandi neutri.

Comandi principali previsti:

```text
ConfigureTimer
StartTimer
PauseTimer
ResumeTimer
ResetTimer
Tick
```

Significato:

```text
ConfigureTimer
Imposta o aggiorna la configurazione logica del timer.

StartTimer
Avvia il timer dalla configurazione valida.

PauseTimer
Mette in pausa la sessione corrente mantenendo il tempo rimanente.

ResumeTimer
Riprende il timer dalla pausa.

ResetTimer
Annulla la sessione corrente e riporta il timer alla durata iniziale configurata.

Tick
Avanza la logica del timer secondo il tempo trascorso.
```

Il comando `Tick` rappresenta l'avanzamento logico del tempo.

`Tick` è un comando infrastrutturale riservato all'interazione tra la sorgente temporale e la logica core.

`Tick` non è un comando utente.

`Tick` non deve essere esposto come pulsante, voce di menu, scorciatoia o azione manuale nella UI.

Questo documento non decide ancora se il timer reale userà un timer di sistema, un dispatcher timer, un servizio temporale o altra soluzione tecnica.

La logica core deve ricevere un'informazione temporale neutra sufficiente per aggiornare il proprio stato.

Esempi concettuali:

```text
Tick di 1 secondo
Tick con tempo trascorso pari a 1000 millisecondi
Tick calcolato da orologio monotono
```

La scelta precisa sarà definita nel design tecnico del core timer.

La UI non deve calcolare autonomamente il tempo rimanente.

La UI non deve decidere quando inviare eventi come sessione completata o avviso finale iniziato.

---

## 6. Configurazione del timer

La configurazione logica minima del timer contiene:

```text
durata sessione
durata avviso finale
```

La configurazione deve essere espressa in forma neutra.

Forma concettuale consigliata:

```text
SessionDurationSeconds
FinalAlertDurationSeconds
```

Regole logiche:

```text
durata sessione > 0
durata avviso finale >= 0
durata avviso finale < durata sessione
```

La durata dell'avviso finale può essere pari a zero.

Il valore zero significa:

```text
avviso sonoro finale disattivato
```

La logica core valida queste regole.

La UI può verificare formato e presenza dei valori inseriti, ma non deve duplicare la validazione logica.

Il bridge può convertire valori provenienti dalla UI in forma neutra.

Esempio:

```text
UI:
minuti = 5
secondi = 0

Bridge:
SessionDurationSeconds = 300
```

La logica core deve ricevere la durata in forma neutra, non in campi UI separati.

---

## 7. Stati stabili del timer

Gli stati stabili principali sono:

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

Questi nomi sono concettuali.

I design tecnici successivi potranno scegliere nomi C# definitivi, purché mantengano il significato.

La fine della sessione non è uno stato stabile permanente.

La fine della sessione è un evento.

Non deve essere creato uno stato stabile permanente come:

```text
Completed
SessionEnded
Finished
```

se questo viene usato per sostituire l'evento di fine sessione.

Uno stato tecnico temporaneo interno potrà essere valutato solo se giustificato da un design tecnico, ma non deve cambiare il modello architetturale:

```text
la sessione completata è un evento
non uno stato stabile permanente
```

---

## 8. Eventi del timer

La logica core deve poter produrre eventi neutri.

Eventi principali previsti:

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

Significato:

```text
TimerConfigured
La configurazione è stata accettata.

TimerStarted
Il timer è stato avviato.

TimerPaused
Il timer è stato messo in pausa.

TimerResumed
Il timer è stato ripreso.

TimerReset
Il timer è stato resettato.

FinalAlertStarted
Il tempo rimanente è entrato nella finestra finale di avviso.

SessionCompleted
La sessione corrente è arrivata a zero ed è completata.

SessionCounterIncremented
Il contatore delle sessioni completate è aumentato.

NextSessionStarted
Una nuova sessione è partita automaticamente.

ValidationFailed
La configurazione o il comando non è valido.
```

Gli eventi devono essere neutri.

Esempio corretto:

```text
SessionCompleted
```

Esempio scorretto:

```text
Sessione completata. Sessioni completate: 3.
```

Il secondo è un messaggio utente e appartiene al bridge/testi utente, non alla logica core.

Gli eventi possono trasportare dati neutri associati.

Esempio:

```text
SessionCompleted
CompletedSessions = 3
```

Il testo finale viene preparato fuori dal core.

L'evento `FinalAlertStarted` deve essere prodotto quando il timer entra nello stato `FinalAlert`.

Se il timer viene messo in pausa durante la finestra finale e poi ripreso mentre il tempo rimanente è ancora nella finestra finale, il core deve esporre un evento o una transizione sufficiente perché il bridge possa riattivare l'avviso finale.

La soluzione può essere la generazione di `FinalAlertStarted` alla ripresa oppure un evento equivalente definito nel design tecnico.

Il principio obbligatorio è:

```text
la ripresa in finestra finale non deve lasciare il bridge senza informazione utile
per riavviare l'avviso sonoro
```

---

## 9. Dati dinamici di stato

La logica core deve esporre dati dinamici in forma neutra.

Dati dinamici principali:

```text
RemainingSeconds
CompletedSessions
CurrentState
IsFinalAlertActive
IsConfigured
CanStart
CanPause
CanResume
CanReset
```

Significato:

```text
RemainingSeconds
Tempo rimanente della sessione corrente, espresso in secondi.

CompletedSessions
Numero di sessioni effettivamente completate.

CurrentState
Stato stabile corrente del timer.

IsFinalAlertActive
Indicatore neutro della finestra finale di avviso.

IsConfigured
Indica se esiste una configurazione valida.

CanStart
Indica se il comando di avvio è logicamente disponibile.

CanPause
Indica se il comando di pausa è logicamente disponibile.

CanResume
Indica se il comando di ripresa è logicamente disponibile.

CanReset
Indica se il comando di reset è logicamente disponibile.
```

Questi nomi sono concettuali.

I design tecnici successivi potranno raffinarli.

Il principio obbligatorio è che i dati dinamici non devono essere confusi con testi statici.

Esempio corretto:

```text
RemainingSeconds = 299
```

Esempio scorretto:

```text
RemainingText = "Tempo rimanente: 04:59"
```

Il testo formattato appartiene al bridge.

---

## 10. Errori neutri

La logica core deve produrre errori neutri.

Errori logici minimi previsti:

```text
InvalidSessionDuration
InvalidFinalAlertDuration
FinalAlertNotLessThanSessionDuration
TimerNotConfigured
CannotStart
CannotPause
CannotResume
CannotReset
```

Significato:

```text
InvalidSessionDuration
La durata della sessione non è valida.

InvalidFinalAlertDuration
La durata dell'avviso finale non è valida.

FinalAlertNotLessThanSessionDuration
La durata dell'avviso finale è maggiore o uguale alla durata della sessione.

TimerNotConfigured
Il timer non ha una configurazione valida.

CannotStart
Il comando Start non è valido nello stato corrente.

CannotPause
Il comando Pause non è valido nello stato corrente.

CannotResume
Il comando Resume non è valido nello stato corrente.

CannotReset
Il comando Reset non è valido nello stato corrente.
```

Gli errori neutri non sono messaggi finali per l'utente.

Esempio corretto:

```text
InvalidSessionDuration
```

Esempio scorretto:

```text
La durata della sessione deve essere maggiore di zero.
```

Il messaggio utente finale viene prodotto dal bridge tramite il sistema centralizzato dei testi utente.

---

## 11. Risultato dei comandi

Ogni comando interno deve produrre un risultato controllato.

Forma concettuale:

```text
CommandResult
```

Un risultato può contenere:

```text
successo o fallimento
stato corrente aggiornato
errori neutri
dati dinamici aggiornati
```

Esempio concettuale di risultato positivo:

```text
Success = true
CurrentState = Running
RemainingSeconds = 300
CompletedSessions = 0
Errors = []
```

Esempio concettuale di risultato con errore:

```text
Success = false
CurrentState = Stopped
Errors = [InvalidSessionDuration]
```

Gli eventi architetturali del timer devono essere esposti dal core tramite un canale coerente e non duplicato.

Questo documento non impone ancora il meccanismo concreto di emissione degli eventi.

Il design tecnico potrà decidere se gli eventi vengono:

1. raccolti nel risultato del comando;
2. pubblicati tramite un meccanismo di notifica;
3. esposti tramite eventi C#;
4. esposti tramite un observer semplice;
5. esposti con altra soluzione semplice e coerente.

La regola obbligatoria è:

```text
uno stesso evento non deve essere notificato due volte attraverso canali diversi
```

Esempio corretto:

```text
TimerStarted viene esposto tramite un solo canale definito dal design tecnico.
```

Esempio scorretto:

```text
TimerStarted viene sia restituito nel CommandResult
sia pubblicato tramite un secondo canale di notifica,
producendo una doppia gestione nel bridge.
```

Questo documento non impone una classe concreta.

Il design tecnico potrà decidere se usare classi, record, enum, result object o altra struttura C# semplice.

Il principio obbligatorio è che gli esiti non devono essere comunicati tramite eccezioni non controllate per casi logici prevedibili.

Gli errori logici prevedibili devono essere risultati controllati.

Le eccezioni tecniche sono riservate a problemi imprevisti o infrastrutturali e devono essere gestite dal livello appropriato.

---

## 12. Contratto del bridge UI-logica

Il bridge riceve comandi dalla UI e li traduce verso la logica core.

Il bridge può occuparsi di:

1. convertire input UI in valori neutri;
2. chiamare la logica core;
3. ricevere risultati dal core;
4. ricevere eventi neutri dal core tramite il canale definito dal design tecnico;
5. trasformare stati neutri in testi utente;
6. trasformare errori neutri in messaggi utente;
7. formattare dati dinamici;
8. preparare un modello mostrabile per la UI;
9. associare eventi neutri ad azioni verso il livello sistema operativo;
10. preparare informazioni accessibili coerenti con i testi visivi.

Il bridge non deve occuparsi di:

1. calcolare il tempo rimanente;
2. decidere quando una sessione è completata;
3. decidere quando parte l'avviso finale;
4. incrementare il contatore;
5. decidere la ripartenza automatica;
6. contenere regole del timer;
7. chiamare direttamente API Windows;
8. contenere stringhe utente sparse;
9. creare regole di validazione logica autonome;
10. ricevere lo stesso evento core da più canali duplicati.

Il bridge è un traduttore e orchestratore leggero.

Non è una seconda logica core.

---

## 13. Modello mostrabile per la UI

Il bridge deve poter produrre un modello semplice per la UI.

Forma concettuale:

```text
TimerViewModel
```

Questo nome è concettuale e non impone una classe definitiva.

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
ErrorMessageText
EventMessageText
AccessibleStatusText
```

Significato:

```text
RemainingTimeText
Tempo rimanente formattato per l'utente, per esempio "04:59".

TimerStateText
Stato corrente formattato per l'utente, per esempio "Sessione in corso".

CompletedSessionsText
Contatore formattato, per esempio "Sessioni completate: 3".

PrimaryActionText
Testo del comando principale, per esempio "Avvia", "Pausa" o "Riprendi".

CanStart / CanPause / CanResume / CanReset
Disponibilità logica dei comandi.

ErrorMessageText
Messaggio di errore finale, se presente.

EventMessageText
Messaggio evento finale, se presente.

AccessibleStatusText
Testo accessibile coerente con lo stato corrente, se necessario.
```

Il modello mostrabile contiene testi finali, quindi non appartiene alla logica core.

La UI riceve dati già pronti da mostrare.

La UI non deve trasformare `Running` in `"Sessione in corso"`.

La UI non deve trasformare `InvalidSessionDuration` in `"La durata della sessione deve essere maggiore di zero."`

Queste trasformazioni appartengono al bridge e al sistema dei testi utente.

---

## 14. Testi utente e API interna

L'API interna deve separare:

```text
codici neutri
↓
testi utente finali
```

Esempio:

```text
Core:
CurrentState = Running

Bridge + Testi utente:
"Sessione in corso"

UI:
mostra "Sessione in corso"
```

Altro esempio:

```text
Core:
Error = InvalidSessionDuration

Bridge + Testi utente:
"La durata della sessione deve essere maggiore di zero."

UI:
mostra il messaggio già pronto
```

I testi statici devono provenire dal sistema centralizzato dei testi utente.

I dati dinamici devono provenire dallo stato dell'applicazione.

Il bridge combina testi statici e dati dinamici.

Esempio:

```text
Testo statico centralizzato:
"Tempo rimanente"

Dato dinamico:
299 secondi

Testo finale:
"Tempo rimanente: 04:59"
```

Il core non deve conoscere il testo finale.

---

## 15. Contratto con il livello sistema operativo

Il livello sistema operativo gestisce interazioni dipendenti da Windows o dall'ambiente esterno.

Richieste concettuali ammesse verso il livello sistema operativo:

```text
StartFinalAlertSound
StopFinalAlertSound
ShowLocalNotification
```

Significato:

```text
StartFinalAlertSound
Avvia l'avviso sonoro finale.

StopFinalAlertSound
Ferma l'avviso sonoro finale.

ShowLocalNotification
Mostra una notifica locale semplice, se prevista da un design successivo.
```

Questi nomi sono concettuali.

Il documento non impone ancora implementazione, librerie o API Windows specifiche.

Il bridge può richiedere queste azioni quando riceve eventi neutri dal core.

Esempio:

```text
Core produce:
FinalAlertStarted

Bridge richiede:
StartFinalAlertSound

UI mostra:
"Avviso finale in corso"
```

Il livello sistema operativo non deve decidere quando parte l'avviso finale.

Il livello sistema operativo non deve decidere quando termina la sessione.

Il livello sistema operativo non deve incrementare il contatore.

Il livello sistema operativo non deve conoscere regole del timer.

---

## 16. Avviso finale e API interna

L'avviso finale è governato dalla logica core.

La logica core decide quando il tempo rimanente entra nella finestra finale.

Quando ciò accade, la logica core produce:

```text
FinalAlertStarted
CurrentState = FinalAlert
```

Il bridge interpreta l'evento e può chiedere al livello sistema operativo:

```text
StartFinalAlertSound
```

Quando la sessione termina, la logica core produce:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

Il bridge può chiedere al livello sistema operativo:

```text
StopFinalAlertSound
```

se l'avviso sonoro era attivo.

Se il timer viene messo in pausa durante la finestra finale, il bridge può richiedere al livello sistema operativo di fermare temporaneamente l'avviso sonoro, secondo quanto sarà definito nel design tecnico.

Se il timer viene ripreso e il tempo rimanente è ancora nella finestra finale, il core deve esporre una transizione o un evento sufficiente perché il bridge possa richiedere nuovamente l'avvio dell'avviso sonoro.

Il requisito di priorità percettiva dell'avviso rispetto ad altri suoni non viene risolto da questo documento.

Soluzioni tecniche per ridurre, sospendere o attenuare altri suoni di sistema richiedono un design tecnico dedicato.

---

## 17. Ripartenza automatica

La ripartenza automatica appartiene alla logica core.

Quando la sessione arriva a zero e il timer non è stato interrotto dall'utente, la logica core deve:

1. considerare completata la sessione corrente;
2. incrementare il contatore;
3. avviare internamente una nuova sessione con la stessa configurazione;
4. produrre gli eventi neutri necessari.

Eventi attesi:

```text
SessionCompleted
SessionCounterIncremented
NextSessionStarted
```

Il bridge non deve simulare la ripartenza automatica richiamando `StartTimer`.

Esempio scorretto:

```text
Core:
SessionCompleted

Bridge:
chiama StartTimer per far ripartire la sessione
```

Esempio corretto:

```text
Core:
SessionCompleted
SessionCounterIncremented
NextSessionStarted
CurrentState = Running
RemainingSeconds = durata sessione
```

Il bridge mostra e comunica quanto ricevuto dal core.

---

## 18. Pausa, reset e ciclo automatico

La pausa interrompe temporaneamente il timer mantenendo il tempo rimanente.

Comando:

```text
PauseTimer
```

Risultato atteso:

```text
CurrentState = Paused
RemainingSeconds conservato
Evento: TimerPaused
```

La ripresa riparte dalla pausa.

Comando:

```text
ResumeTimer
```

Risultato atteso:

```text
CurrentState = Running oppure FinalAlert
RemainingSeconds conservato
Evento: TimerResumed
```

Se il tempo rimanente è già dentro la finestra finale, la ripresa deve mantenere coerenza con lo stato di avviso finale.

In questo caso il core deve esporre un evento o una transizione sufficiente perché il bridge possa riattivare l'avviso finale, come indicato nelle sezioni dedicate agli eventi e all'avviso finale.

Il reset annulla la sessione corrente e riporta il timer alla durata iniziale configurata.

Comando:

```text
ResetTimer
```

Risultato atteso:

```text
CurrentState = Stopped
RemainingSeconds = durata sessione configurata
CompletedSessions invariato
Evento: TimerReset
```

Una sessione annullata tramite reset non incrementa il contatore.

Il reset non azzera il contatore delle sessioni completate.

Nella prima versione minima non è previsto un comando separato di azzeramento globale del contatore delle sessioni completate.

Il contatore riparte da zero solo all'avvio di una nuova esecuzione dell'app o tramite futura funzione esplicita approvata da un design dedicato.

Il reset interrompe il ciclo automatico finché l'utente non invia un nuovo comando esplicito.

Se `ResetTimer` viene ricevuto quando il timer è già `Stopped`, il core deve mantenere il timer fermo, reimpostare il tempo rimanente alla durata configurata se esiste una configurazione valida, mantenere invariato il contatore e non produrre eventi di ciclo come `SessionCompleted` o `NextSessionStarted`.

Il design tecnico potrà decidere se, in questo caso, produrre comunque un evento neutro `TimerReset` per confermare il comando o se trattare il comando come operazione senza cambiamento osservabile.

La regola obbligatoria è:

```text
ResetTimer da Stopped non deve completare sessioni,
non deve incrementare il contatore
e non deve riavviare automaticamente il ciclo
```

---

## 19. Consultazione del tempo rimanente

Il tempo rimanente deve essere disponibile come dato neutro:

```text
RemainingSeconds
```

Il bridge può trasformarlo in:

```text
RemainingTimeText
```

Esempio:

```text
299 secondi
↓
04:59
```

Il testo completo accessibile può essere preparato dal bridge:

```text
Tempo rimanente: 04:59
```

La UI può mostrare o rendere consultabile questo testo.

Il tempo rimanente non deve essere annunciato automaticamente a ogni secondo.

L'API interna deve quindi permettere l'aggiornamento del dato senza trasformare ogni aggiornamento in evento accessibile automatico.

Esempio corretto:

```text
RemainingSeconds cambia ogni secondo
nessun evento accessibile obbligatorio a ogni secondo
```

Esempio scorretto:

```text
ogni Tick produce un annuncio "Tempo rimanente: 04:58"
```

---

## 20. Contratto accessibilità

L'API interna deve supportare l'accessibilità senza contaminare la logica core.

La logica core produce dati neutri.

Il bridge prepara testi accessibili coerenti con i testi visivi.

La UI espone controlli e informazioni tramite meccanismi accessibili della piattaforma.

Esempio:

```text
Core:
CurrentState = FinalAlert

Bridge:
TimerStateText = "Avviso finale in corso"
AccessibleStatusText = "Avviso finale in corso"

UI:
mostra il testo
lo espone allo screen reader
```

La logica core non deve produrre:

```text
NVDA says...
AutomationProperties...
LiveRegion...
Speech...
```

L'eventuale uso di meccanismi nativi di accessibilità Windows sarà definito nei design tecnici successivi.

---

## 21. Contratto per i test

La logica core deve essere testabile senza:

1. aprire finestre;
2. usare NVDA;
3. riprodurre suoni reali;
4. chiamare API Windows;
5. usare controlli UI;
6. dipendere da XAML.

I test della logica core devono poter verificare:

1. configurazione valida;
2. configurazione non valida;
3. avvio;
4. pausa;
5. ripresa;
6. reset;
7. ingresso nella finestra finale;
8. fine sessione;
9. incremento del contatore;
10. mancato incremento dopo reset;
11. ripartenza automatica;
12. avviso finale disattivato con valore zero;
13. errori neutri;
14. eventi neutri;
15. stati stabili;
16. ripresa dalla pausa dentro la finestra finale;
17. reset quando il timer è già fermo;
18. assenza di duplicazione degli eventi.

Il bridge potrà essere testato verificando trasformazioni da dati neutri a dati mostrabili.

Esempio:

```text
Input bridge:
RemainingSeconds = 299

Output bridge:
RemainingTimeText = "04:59"
```

---

## 22. Sequenza tipica: avvio timer

Sequenza concettuale:

```text
Utente inserisce durata sessione e avviso finale
↓
UI raccoglie i valori
↓
Bridge converte i valori in secondi
↓
Bridge invia ConfigureTimer al core
↓
Core valida la configurazione
↓
Core restituisce risultato
↓
Bridge prepara eventuali testi
↓
UI mostra stato configurato
↓
Utente preme Avvia
↓
UI invia comando al bridge
↓
Bridge invia StartTimer al core
↓
Core passa a Running
↓
Core produce TimerStarted tramite il canale eventi definito dal design tecnico
↓
Bridge prepara TimerStateText
↓
UI mostra "Sessione in corso"
```

Punti vietati:

1. la UI non valida logicamente la durata dell'avviso finale;
2. la UI non calcola il tempo rimanente;
3. la UI non decide lo stato testuale;
4. il bridge non inventa regole del timer;
5. il core non produce testi utente finali;
6. lo stesso evento non viene gestito due volte tramite canali diversi.

---

## 23. Sequenza tipica: ingresso in avviso finale

Sequenza concettuale:

```text
Timer in corso
↓
Tick aggiorna la logica core
↓
RemainingSeconds raggiunge la soglia di avviso finale
↓
Core passa a FinalAlert
↓
Core produce FinalAlertStarted
↓
Bridge riceve l'evento
↓
Bridge prepara "Avviso finale in corso"
↓
Bridge richiede StartFinalAlertSound al livello sistema operativo
↓
UI mostra lo stato aggiornato
```

Punti vietati:

1. il bridge non decide autonomamente la soglia;
2. la UI non avvia direttamente il suono;
3. il livello sistema operativo non decide quando parte l'avviso;
4. il core non riproduce suoni;
5. il core non conosce Windows.

---

## 24. Sequenza tipica: fine sessione e ripartenza

Sequenza concettuale:

```text
Timer in avviso finale o sessione in corso
↓
Tick porta RemainingSeconds a zero
↓
Core produce SessionCompleted
↓
Core incrementa CompletedSessions
↓
Core produce SessionCounterIncremented
↓
Core avvia internamente la nuova sessione
↓
Core produce NextSessionStarted
↓
Core aggiorna RemainingSeconds alla durata iniziale
↓
Bridge prepara i testi evento e stato
↓
Bridge richiede eventuale StopFinalAlertSound
↓
UI mostra stato e contatore aggiornati
```

Esempio di testo evento prodotto fuori dal core:

```text
Sessione completata. Sessioni completate: 3.
```

Questo testo non deve essere generato dalla logica core.

---

## 25. Sequenza tipica: errore di configurazione

Sequenza concettuale:

```text
Utente inserisce durata sessione non valida
↓
UI raccoglie i valori
↓
Bridge converte ciò che può convertire
↓
Bridge invia ConfigureTimer al core
↓
Core rileva InvalidSessionDuration
↓
Core restituisce risultato fallito
↓
Bridge converte l'errore neutro in messaggio utente
↓
UI mostra messaggio accessibile
```

Esempio:

```text
Errore core:
InvalidSessionDuration

Messaggio utente:
La durata della sessione deve essere maggiore di zero.
```

La UI non deve inventare il messaggio.

La UI non deve mostrare errori tecnici grezzi.

---

## 26. Cosa è fuori perimetro

Questo documento non autorizza:

1. API web;
2. server locale;
3. database;
4. plugin;
5. estensioni;
6. sincronizzazione cloud;
7. REST API;
8. interfacce pubbliche;
9. calcolo compensi;
10. gestione clienti;
11. profili avanzati;
12. notifiche complesse;
13. gestione audio avanzata;
14. script NVDA;
15. sintesi vocale interna;
16. UI definitiva;
17. classi C# definitive;
18. struttura cartelle definitiva;
19. sistemi di localizzazione complessi;
20. serializzazione di stato persistente;
21. comando di azzeramento globale del contatore nella prima versione minima.

Questi elementi non sono vietati per sempre.

Sono fuori perimetro nella prima fase o richiedono documenti dedicati.

---

## 27. Regole per gli agenti AI

Gli agenti AI devono rispettare queste regole:

1. non trasformare `internal-api.md` in una API web;
2. non creare endpoint HTTP;
3. non introdurre server locale;
4. non definire classi C# definitive senza design tecnico;
5. non spostare logica nel bridge;
6. non spostare logica nella UI;
7. non far dipendere il core da UI, NVDA, Windows o audio;
8. non generare testi utente dentro il core;
9. non usare eventi neutri come messaggi finali;
10. non usare errori neutri come messaggi finali;
11. non duplicare validazione logica nella UI;
12. non simulare la ripartenza automatica dal bridge;
13. non far decidere al livello sistema operativo regole del timer;
14. non trasformare ogni Tick in annuncio accessibile;
15. non introdurre funzionalità fuori perimetro;
16. non considerare i nomi concettuali di questo documento come nomi C# obbligatori;
17. non esporre `Tick` come comando utente nella UI;
18. non duplicare lo stesso evento tramite più canali;
19. non introdurre un comando di azzeramento globale del contatore senza design dedicato;
20. non lasciare la ripresa in finestra finale senza informazione utile per riattivare l'avviso sonoro.

I nomi presenti in questo documento servono a stabilire significato e confini.

I design tecnici successivi potranno scegliere nomi implementativi più adatti, purché rispettino i contratti architetturali.

---

## 28. Criteri di validità dell'API interna

Una futura implementazione sarà considerata coerente con questo documento solo se:

1. la logica core riceve comandi neutri;
2. la logica core produce stati neutri;
3. la logica core produce eventi neutri;
4. la logica core produce errori neutri;
5. la logica core espone dati dinamici senza testi finali;
6. il bridge traduce stati, eventi, errori e dati in informazioni mostrabili;
7. la UI riceve testi e dati già pronti;
8. la UI non duplica la logica del timer;
9. la UI non trasforma autonomamente codici tecnici in testi utente;
10. il livello sistema operativo non contiene regole del timer;
11. l'avviso finale è deciso dal core;
12. la ripartenza automatica è gestita dal core;
13. il contatore è incrementato dal core;
14. reset e pausa rispettano le regole approvate;
15. il tempo rimanente è trattato come dato dinamico;
16. gli annunci accessibili non sono prodotti a ogni Tick;
17. i testi statici sono centralizzati;
18. gli errori logici prevedibili sono risultati controllati;
19. il core è testabile senza UI, audio, NVDA o Windows;
20. `Tick` resta un comando infrastrutturale e non utente;
21. gli eventi sono esposti tramite un canale coerente e non duplicato;
22. la ripresa dalla pausa dentro la finestra finale permette al bridge di riattivare l'avviso sonoro;
23. `ResetTimer` da `Stopped` non completa sessioni, non incrementa il contatore e non riavvia il ciclo;
24. nella prima versione minima non è introdotto un comando di azzeramento globale del contatore;
25. non sono state introdotte funzionalità fuori perimetro.

Se una soluzione funziona ma viola questi confini, non deve essere considerata valida.

---

## 29. Stato del documento

Questo documento è approvato come API interna concettuale e contratto tra livelli del progetto CicloTimer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini e chiarimenti su Tick, eventi, reset, contatore e ripresa in avviso finale
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione delle osservazioni dei consiglieri AI su natura infrastrutturale di Tick, canale eventi non duplicato, ripresa dalla pausa in finestra finale, ResetTimer da stato Stopped e assenza di comando di azzeramento globale del contatore nella prima versione minima
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. chiarimento che `Tick` è un comando infrastrutturale e non un comando utente;
2. chiarimento che gli eventi devono essere esposti tramite un canale coerente e non duplicato;
3. rimozione dell'ambiguità secondo cui `CommandResult` debba obbligatoriamente contenere eventi;
4. chiarimento sulla ripresa dalla pausa quando il tempo rimanente è ancora nella finestra finale;
5. chiarimento del comportamento di `ResetTimer` quando il timer è già `Stopped`;
6. chiarimento che il reset non azzera il contatore delle sessioni completate;
7. chiarimento che nella prima versione minima non è previsto un comando globale di azzeramento del contatore;
8. aggiornamento dei criteri di test, delle regole per gli agenti AI e dei criteri di validità dell'API interna.

Il documento è approvato dal project owner come base per i successivi documenti di design tecnico, coding plan, todo operativo e implementazione.
