\# CicloTimer — Architettura generale



\*\*Tipo documento:\*\* architettura generale  

\*\*Stato:\*\* APPROVED  

\*\*Versione:\*\* 0.3.0  

\*\*Data:\*\* 2026-06-01  

\*\*Repository:\*\* donato81/CicloTimer  

\*\*Documento collegato:\*\* docs/0-architecture/vision.md  



\---



\## 1. Scopo del documento



Questo documento definisce l'architettura generale del progetto CicloTimer.



L'obiettivo è stabilire una divisione chiara tra le parti principali dell'applicazione, in modo che il progetto resti piccolo, ordinato, accessibile e facilmente testabile.



Il documento non descrive ancora il codice definitivo, le classi concrete o il framework UI specifico.



Il suo compito è definire:



1\. quali sono i blocchi principali del progetto;

2\. quali responsabilità ha ogni blocco;

3\. quali responsabilità ogni blocco non deve avere;

4\. come devono comunicare i blocchi tra loro;

5\. come devono essere gestiti testi ed errori rivolti all'utente;

6\. quali regole devono rispettare gli agenti AI durante la futura implementazione.



\---



\## 2. Principio guida



Il principio guida dell'architettura è:



```text

la logica del timer deve essere indipendente dalla UI e dal sistema operativo

```



Questo significa che il cuore del timer deve poter essere capito, testato e modificato senza dipendere da finestre grafiche, screen reader, suoni, notifiche o API specifiche di Windows.



La UI, l'accessibilità e l'audio sono parti importanti del progetto, ma non devono contaminare la logica core.



La logica core deve sapere:



```text

durata sessione

durata avviso finale

tempo rimanente

stato del timer

sessioni completate

transizioni tra stati

eventi del timer

```



La logica core non deve sapere:



```text

come è disegnata la finestra

quale pulsante è stato premuto

come parla NVDA

come viene riprodotto un suono

come Windows gestisce gli altri audio attivi

quali testi finali vengono mostrati all'utente

```



Questa separazione serve a evitare confusione, duplicazioni e dipendenze inutili.



\---



\## 3. Struttura logica del progetto



Il progetto è diviso in questi livelli principali:



```text

UI

Logica core

Bridge UI-logica

Interazioni con il sistema operativo

Testi utente

Gestione errori

Test

Documentazione

```



Ogni livello ha un ruolo preciso.



Nessun livello deve assumere responsabilità che appartengono a un altro livello.



La separazione non deve creare complessità inutile, ma deve impedire che tutto il comportamento dell'app finisca dentro la schermata principale.



\---



\## 4. Livello UI



Il livello UI è responsabile di ciò che l'utente vede e usa.



La UI deve essere pensata sia per utenti vedenti sia per utenti che usano screen reader.



CicloTimer nasce con forte attenzione a NVDA perché il primo sviluppatore e tester usa screen reader, ma l'app deve essere anche chiara, ordinata e intuitiva per una persona vedente.



La UI può occuparsi di:



1\. mostrare il tempo rimanente;

2\. mostrare lo stato del timer;

3\. mostrare il numero di sessioni completate;

4\. mostrare campi, etichette e pulsanti;

5\. ricevere comandi dell'utente;

6\. esporre controlli accessibili;

7\. mostrare messaggi di errore già preparati;

8\. mostrare eventi importanti, come la sessione completata;

9\. offrire una disposizione visiva chiara e comprensibile;

10\. eseguire controlli minimi di input prima dell'invio, limitati a formato e presenza del valore.



La UI non deve occuparsi di:



1\. calcolare direttamente il tempo rimanente;

2\. decidere quando una sessione è completata;

3\. incrementare direttamente il contatore delle sessioni;

4\. decidere quando entrare nella finestra di avviso finale;

5\. gestire direttamente il comportamento del ciclo;

6\. riprodurre direttamente l'audio del timer;

7\. controllare direttamente altri suoni del sistema;

8\. chiamare direttamente API di Windows;

9\. convertire codici, stati o errori in testi finali;

10\. contenere stringhe utente hardcoded sparse;

11\. inventare messaggi di errore propri;

12\. duplicare le regole di validazione logica della logica core.



La UI può verificare che un campo sia compilato o che contenga caratteri numerici validi prima di inviare i dati al bridge.



La UI non deve decidere se una configurazione è logicamente valida.



Esempi di validazione logica che non appartengono alla UI:



```text

durata sessione > 0

durata avviso finale >= 0

durata avviso finale < durata sessione

configurazione del timer logicamente valida

```



Queste regole appartengono alla logica core.



La UI deve ricevere dal bridge dati già pronti per essere mostrati.



Esempio corretto:



```text

L'utente preme Avvia.

La UI passa il comando al bridge.

Il bridge invia il comando alla logica core.

La logica core cambia stato.

Il bridge prepara i dati e i testi da mostrare.

La UI mostra il nuovo stato ricevuto.

```



Esempio scorretto:



```text

L'utente preme Avvia.

La UI calcola direttamente il ciclo, il tempo rimanente e il contatore.

```



Altro esempio scorretto:



```text

La UI riceve lo stato tecnico TimerRunning.

La UI decide autonomamente di mostrarlo come "Sessione in corso".

```



La traduzione degli stati tecnici in testi utente non appartiene alla UI.



\---



\## 5. Livello logica core



La logica core è il motore dell'applicazione.



Deve contenere le regole essenziali del timer ciclico a sessioni ripetute.



La logica core può occuparsi di:



1\. validare la configurazione logica del timer;

2\. gestire la durata totale della sessione;

3\. gestire la durata della finestra finale di avviso;

4\. gestire il tempo rimanente;

5\. gestire gli stati stabili del timer;

6\. riconoscere l'ingresso nella finestra finale di avviso;

7\. riconoscere la fine della sessione;

8\. incrementare il contatore delle sessioni completate;

9\. gestire pausa, ripresa e reset;

10\. decidere se il ciclo deve ripartire automaticamente;

11\. riavviare internamente la nuova sessione quando la sessione precedente è completata e il timer non è stato interrotto;

12\. produrre informazioni di stato ed eventi in forma neutra;

13\. produrre errori logici neutri.



Gli stati stabili principali sono:



```text

Timer fermo

Sessione in corso

Avviso finale in corso

Timer in pausa

```



La fine della sessione non deve essere trattata come stato stabile permanente.



La fine della sessione è un evento.



Eventi principali previsti:



```text

Sessione completata

Contatore incrementato

Nuova sessione avviata

Avviso finale iniziato

Timer messo in pausa

Timer ripreso

Timer resettato

Errore logico rilevato

```



Il riavvio automatico della sessione successiva è responsabilità della logica core.



Quando una sessione arriva a zero e il timer non è in pausa né è stato resettato, la logica core deve:



1\. considerare la sessione corrente completata;

2\. incrementare il contatore delle sessioni completate;

3\. avviare internamente una nuova sessione con la stessa durata configurata;

4\. produrre gli eventi neutri necessari agli altri livelli.



Il bridge non deve simulare il riavvio richiamando un comando di avvio dopo l'evento di fine sessione.



La logica core non deve occuparsi di:



1\. creare finestre;

2\. modificare controlli grafici;

3\. conoscere NVDA;

4\. inviare annunci diretti allo screen reader;

5\. riprodurre suoni;

6\. controllare il volume del sistema;

7\. gestire API di Windows;

8\. decidere testi finali destinati all'utente;

9\. contenere stringhe utente hardcoded;

10\. mostrare messaggi di errore finali;

11\. formattare testi finali per la UI;

12\. formattare direttamente valori dinamici in frasi utente complete.



La logica core deve esporre stati, valori, eventi ed errori in forma neutra, così che gli altri livelli possano trasformarli in testi, elementi UI, annunci accessibili o suoni.



Esempio corretto:



```text

Tempo rimanente: 299 secondi

```



Esempio scorretto:



```text

Tempo rimanente: 04:59

```



La conversione in formato leggibile per l'utente appartiene al bridge.



\---



\## 6. Bridge UI-logica



Il bridge UI-logica è il livello di collegamento tra la UI e la logica core.



Serve a evitare che la UI parli direttamente con dettagli interni della logica.



Il bridge deve restare sottile.



Il bridge può occuparsi di:



1\. ricevere comandi dalla UI;

2\. chiamare la logica core;

3\. trasformare lo stato neutro della logica in dati mostrabili;

4\. preparare informazioni per la UI;

5\. collegare eventi della logica a richieste semplici verso il livello sistema operativo;

6\. trasformare errori neutri o tecnici in messaggi utente;

7\. fornire alla UI un modello semplice da mostrare;

8\. associare stati, eventi ed errori al sistema centralizzato dei testi utente;

9\. formattare valori dinamici neutri in testi leggibili per la UI;

10\. convertire il tempo rimanente in una forma leggibile, per esempio `mm:ss`.



Il bridge può convertire il tempo rimanente, espresso dalla logica core in forma neutra, in una stringa formattata leggibile da mostrare nella UI.



Esempio:



```text

Valore neutro dal core: 299 secondi

Testo statico centralizzato: "Tempo rimanente"

Testo finale preparato dal bridge: "Tempo rimanente: 04:59"

```



La UI deve ricevere già il testo formattato.



Il bridge non deve occuparsi di:



1\. inventare regole del timer;

2\. duplicare i calcoli della logica core;

3\. calcolare durata, soglia di avviso o tempo rimanente;

4\. decidere se una sessione deve ripartire;

5\. incrementare il contatore delle sessioni;

6\. contenere logica audio complessa;

7\. contenere codice specifico di Windows non necessario;

8\. contenere stringhe utente hardcoded sparse;

9\. sostituire il livello dei testi utente;

10\. sostituire la gestione errori;

11\. diventare un contenitore generico di codice non classificato;

12\. validare logicamente la configurazione del timer.



Il bridge non deve contenere logica di business.



Per logica di business si intendono le regole che definiscono il comportamento del timer, ad esempio:



```text

quando una sessione è completata

quando entra l'avviso finale

quando incrementare il contatore

quando riavviare una sessione

quando una configurazione è logicamente valida

```



Il bridge si limita a tradurre comandi, stati, eventi, valori dinamici ed errori.



Il bridge può inoltrare richieste atomiche al livello sistema operativo, ad esempio:



```text

avvia suono

ferma suono

mostra notifica locale semplice

```



Il bridge non deve chiamare direttamente API di Windows.



Esempio corretto:



```text

La logica core segnala: AvvisoFinaleIniziato.

Il bridge interpreta l'evento.

Il bridge chiede al livello sistema operativo di avviare l'avviso sonoro.

La UI riceve lo stato testuale già pronto: "Avviso finale in corso".

```



Esempio scorretto:



```text

Il bridge decide autonomamente quando mancano 20 secondi e avvia il suono senza passare dalla logica core.

```



Altro esempio scorretto:



```text

La logica core segnala: Sessione completata.

Il bridge richiama Avvia per far ripartire il timer.

```



Il riavvio automatico appartiene alla logica core, non al bridge.



\---



\## 7. Interazioni con il sistema operativo



Il livello di interazione con il sistema operativo contiene ciò che dipende da Windows o da servizi esterni alla logica core.



Questo livello può occuparsi di:



1\. riprodurre l'avviso sonoro;

2\. fermare l'avviso sonoro;

3\. gestire eventuali notifiche locali semplici;

4\. isolare le API specifiche di Windows dal resto dell'app;

5\. restituire al bridge eventuali errori tecnici in forma controllata.



Questo livello non deve occuparsi di:



1\. decidere quando una sessione è completata;

2\. incrementare il contatore delle sessioni;

3\. validare le regole del timer;

4\. controllare direttamente la UI;

5\. contenere logica del ciclo;

6\. contenere testi utente hardcoded sparsi;

7\. mostrare messaggi di errore direttamente all'utente;

8\. essere chiamato direttamente dalla UI.



Nella prima versione, l'avviso sonoro può essere riprodotto in modo semplice, senza gestione avanzata del volume degli altri suoni di sistema.



Il requisito di dare priorità percettiva all'avviso sonoro è importante, ma non autorizza automaticamente una gestione audio complessa.



Le soluzioni tecniche per controllare, ridurre, attenuare o sospendere altri suoni del sistema dovranno essere valutate in un design tecnico dedicato.



Qualsiasi interazione con Windows, inclusi audio e notifiche locali semplici, deve passare attraverso questo livello.



La UI non deve chiamare direttamente API di Windows per audio, notifiche o altre integrazioni di sistema.



Il bridge può orchestrare richieste verso questo livello, ma non deve contenere direttamente la logica tecnica specifica di Windows.



\---



\## 8. Testi utente e divieto di stringhe hardcoded



CicloTimer deve evitare stringhe utente hardcoded sparse nel codice.



Per stringhe utente si intendono tutti i testi statici visibili o leggibili dall'utente, inclusi:



1\. etichette dei campi;

2\. testi dei pulsanti;

3\. messaggi di stato;

4\. messaggi di errore;

5\. messaggi evento;

6\. testi accessibili per screen reader;

7\. descrizioni o istruzioni;

8\. etichette collegate al contatore sessioni;

9\. etichette collegate al tempo rimanente.



Esempi di stringhe utente statiche:



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

```



Questi testi devono essere gestiti in un punto dedicato del progetto.



La soluzione concreta potrà essere definita nei design tecnici successivi.



Nella prima versione, per evitare overengineering, è ammessa una soluzione semplice, per esempio una classe o modulo interno con costanti stringa.



Non sono richiesti database, servizi esterni, file di localizzazione complessi o sistemi avanzati di traduzione.



Il principio obbligatorio è:



```text

una sola fonte controllata per i testi statici rivolti all'utente

```



La UI visiva e l'accessibilità devono usare testi coerenti tra loro.



La UI non deve convertire codici, stati, eventi o errori in testi finali.



La responsabilità della conversione appartiene al bridge, che usa il sistema centralizzato dei testi utente.



Flusso corretto:



```text

Logica core

↓

stato/evento/errore neutro

↓

Bridge

↓

testo utente finale tramite Testi utente

↓

UI

```



Esempio corretto:



```text

La logica core espone lo stato: SessionRunning.

Il bridge lo associa al testo centralizzato: "Sessione in corso".

La UI mostra "Sessione in corso".

Lo screen reader legge "Sessione in corso".

```



Esempio scorretto:



```text

La logica core espone SessionRunning.

La UI decide autonomamente di mostrarlo come "Timer attivo".

Il bridge usa invece "Sessione in corso".

Il testo visivo e quello accessibile divergono.

```



La logica core non deve contenere testi finali destinati all'utente.



La logica core deve produrre stati, codici, eventi ed errori neutri.



Gli altri livelli trasformano questi elementi in testi tramite il sistema centralizzato dei testi utente.



\### 8.1 Testi statici e dati dinamici



Il sistema dei testi utente gestisce testi statici, etichette e messaggi fissi.



Non deve contenere i valori dinamici che cambiano durante l'esecuzione.



Sono dati dinamici, non stringhe statiche:



```text

04:59

04:58

04:57

3 sessioni completate

20 secondi rimanenti

```



I dati dinamici appartengono allo stato dell'applicazione.



Il bridge può formattarli usando etichette centralizzate.



Esempio corretto:



```text

Etichetta statica centralizzata: "Tempo rimanente"

Dato dinamico: 04:59

Testo finale preparato dal bridge: "Tempo rimanente: 04:59"

```



Altro esempio corretto:



```text

Etichetta statica centralizzata: "Sessioni completate"

Dato dinamico: 3

Testo finale preparato dal bridge: "Sessioni completate: 3"

```



Esempio scorretto:



```text

Il sistema dei testi utente contiene valori come "04:59", "04:58", "04:57".

```



Il tempo rimanente e il numero delle sessioni completate devono essere trattati come valori di stato, non come stringhe statiche.



\---



\## 9. Gestione degli errori



CicloTimer deve evitare errori gestiti in modo casuale o sparsi senza controllo nei vari livelli.



Gli errori devono essere classificati in base al livello di origine.



Categorie principali previste:



```text

Errori core

Errori bridge

Errori UI

Errori sistema operativo

```



Ogni livello può produrre solo errori coerenti con la propria responsabilità.



Esempi:



```text

Errore core:

durata sessione non valida



Errore bridge:

comando non traducibile o stato non mappato



Errore UI:

inserimento di caratteri alfabetici o non numerici nei campi di configurazione del tempo



Errore sistema operativo:

impossibile riprodurre l'avviso sonoro

```



La UI può produrre solo errori legati all'interazione immediata con i controlli, come un formato non numerico in un campo numerico.



La UI non deve produrre errori legati alla validazione logica della configurazione.



Esempi di errori che appartengono alla logica core, non alla UI:



```text

durata sessione pari a zero

durata avviso finale negativa

durata avviso finale maggiore o uguale alla durata sessione

```



La logica core deve produrre errori neutri, non messaggi finali per l'utente.



Esempio corretto:



```text

InvalidSessionDuration

```



Esempio scorretto:



```text

La durata della sessione deve essere maggiore di zero.

```



Il messaggio finale rivolto all'utente deve essere preparato tramite il bridge e il sistema centralizzato dei testi utente.



Flusso corretto:



```text

Logica core

↓

errore neutro

↓

Bridge

↓

testo utente centralizzato

↓

UI

```



La UI mostra errori già preparati.



La UI non deve inventare messaggi di errore propri.



Il livello sistema operativo deve isolare gli errori legati a Windows, audio o notifiche, senza farli propagare in forma grezza verso la UI.



Esempio scorretto:



```text

System.IO.FileNotFoundException: beep.wav not found

```



Esempio corretto:



```text

Impossibile riprodurre l'avviso sonoro.

```



Gli errori tecnici possono essere conservati internamente per diagnosi futura, ma non devono essere mostrati direttamente all'utente nella prima versione.



La soluzione concreta per organizzare gli errori potrà prevedere file o cartelle dedicate, eventualmente separati per livello.



Questo documento non impone ancora una struttura fisica definitiva per gli errori.



Il principio obbligatorio è:



```text

gli errori devono essere classificati, controllati e trasformati in messaggi utente coerenti

```



Non sono richiesti nella prima versione:



```text

logging avanzato

telemetria

database errori

sistemi globali complessi di gestione eccezioni

servizi esterni di monitoraggio

```



\---



\## 10. Accessibilità e UI visiva



L'app deve essere progettata con una doppia attenzione:



```text

accessibilità per screen reader

\+

chiarezza visiva per utenti vedenti

```



Queste due esigenze non sono in conflitto.



Una UI accessibile non deve essere povera, confusa o brutta.



Una UI visivamente gradevole non deve nascondere informazioni importanti allo screen reader.



La UI deve quindi rispettare questi principi:



1\. controlli raggiungibili da tastiera;

2\. etichette chiare;

3\. ordine di navigazione logico;

4\. testo visibile coerente con il testo accessibile;

5\. stato del timer sempre comprensibile;

6\. contatore sessioni ben visibile e leggibile da screen reader;

7\. tempo rimanente ben visibile e consultabile a richiesta;

8\. nessuna informazione affidata solo al colore;

9\. messaggi di errore visibili e accessibili;

10\. layout semplice, ordinato e intuitivo.



Il dettaglio operativo delle regole NVDA sarà definito nel documento dedicato alle regole di accessibilità.



Il dettaglio grafico della UI sarà definito nei design tecnici successivi.



\---



\## 11. Test



I test devono concentrarsi prima sulla logica core.



La logica core deve essere testabile senza aprire finestre, senza usare NVDA e senza riprodurre suoni reali.



I test minimi futuri dovranno verificare:



1\. configurazione valida del timer;

2\. configurazione non valida;

3\. avvio del timer;

4\. pausa;

5\. ripresa;

6\. reset;

7\. ingresso nella finestra finale di avviso;

8\. fine sessione;

9\. incremento del contatore;

10\. mancato incremento del contatore dopo reset;

11\. ripartenza automatica;

12\. avviso finale disattivato con valore zero;

13\. separazione tra stati stabili ed eventi;

14\. produzione di errori core neutri;

15\. trasformazione controllata degli errori in messaggi utente;

16\. formattazione del tempo rimanente tramite bridge;

17\. assenza di duplicazione della validazione logica nella UI.



I test del livello sistema operativo e dell'audio potranno essere più limitati o simulati, perché dipendono dall'ambiente Windows.



La priorità iniziale è garantire che la logica del timer sia corretta e prevedibile.



\---



\## 12. Regole di dipendenza



Le dipendenze devono seguire una direzione semplice.



Regola generale:



```text

la logica core non dipende da nessun livello esterno

```



Direzione desiderata:



```text

UI

↓

Bridge UI-logica

↓

Logica core

```



Il bridge può chiamare il livello sistema operativo quando riceve eventi dalla logica che richiedono audio o integrazioni Windows.



Il bridge può usare il sistema centralizzato dei testi utente per preparare testi finali destinati alla UI.



Il bridge può usare la gestione errori per trasformare errori neutri o tecnici in messaggi utente coerenti.



Schema logico:



```text

UI

↓

Bridge UI-logica

↓

Logica core



Bridge UI-logica

↓

Interazioni con il sistema operativo



Bridge UI-logica

↓

Testi utente



Bridge UI-logica

↓

Gestione errori

↓

Testi utente

```



Regole obbligatorie:



1\. la UI non deve duplicare la logica del timer;

2\. la UI non deve convertire stati, eventi o errori in testi finali;

3\. la UI non deve chiamare direttamente API di Windows;

4\. la UI non deve duplicare la validazione logica della configurazione;

5\. la logica core non deve conoscere la UI;

6\. la logica core non deve conoscere Windows;

7\. la logica core non deve conoscere NVDA;

8\. la logica core non deve conoscere i testi utente finali;

9\. il livello sistema operativo non deve decidere regole del timer;

10\. i testi utente non devono essere sparsi nel codice;

11\. gli errori non devono essere gestiti in modo casuale nei vari livelli;

12\. il bridge non deve diventare un contenitore di logica non controllata;

13\. il riavvio automatico della sessione appartiene alla logica core;

14\. la formattazione dei valori dinamici per la UI appartiene al bridge.



\---



\## 13. Regole anti-confusione



Per mantenere il progetto piccolo e controllabile, valgono queste regole:



1\. nessuna funzionalità fuori perimetro deve essere introdotta senza design approvato;

2\. nessun agente AI deve modificare più livelli senza autorizzazione esplicita;

3\. ogni design deve indicare quali livelli può toccare;

4\. ogni coding plan deve indicare i file o le cartelle coinvolte;

5\. ogni todo operativo deve essere verificabile;

6\. le stringhe utente non devono essere create liberamente nei file di codice;

7\. gli errori non devono essere creati o mostrati liberamente senza classificazione;

8\. la UI non deve diventare il centro della logica;

9\. il bridge non deve diventare una seconda logica core;

10\. l'audio non deve diventare il centro della logica;

11\. l'accessibilità deve essere progettata, non aggiunta a posteriori;

12\. la semplicità ha priorità sulle soluzioni eleganti ma inutilmente complesse;

13\. i valori dinamici non devono essere confusi con testi statici;

14\. la UI non deve validare regole che appartengono alla logica core.



\---



\## 14. Criteri di validità architetturale



Una futura implementazione sarà considerata coerente con questa architettura solo se:



1\. la logica del timer è separata dalla UI;

2\. la logica del timer è separata dalle API di Windows;

3\. pausa, ripresa, reset, avviso finale, riavvio automatico e contatore sono gestiti dalla logica core;

4\. la UI mostra stati e comandi senza duplicare regole;

5\. la UI non traduce autonomamente codici tecnici in testi finali;

6\. la UI non duplica la validazione logica della configurazione;

7\. l'audio è gestito tramite un livello dedicato;

8\. eventuali notifiche locali passano tramite il livello sistema operativo;

9\. i testi utente statici sono centralizzati o comunque non sparsi senza controllo;

10\. i dati dinamici sono trattati come valori di stato, non come stringhe statiche;

11\. il tempo rimanente è formattato dal bridge prima di arrivare alla UI;

12\. gli errori sono classificati e trasformati in messaggi utente coerenti;

13\. i testi visivi e quelli accessibili sono coerenti;

14\. gli stati stabili e gli eventi sono distinti;

15\. la prima versione non include funzionalità fuori perimetro;

16\. il progetto resta comprensibile anche leggendo solo documentazione e struttura delle cartelle.



Se una soluzione funziona ma viola queste regole, deve essere corretta prima di essere considerata stabile.



\---



\## 15. Stato del documento



Questo documento è approvato come architettura generale iniziale del progetto CicloTimer.



Versione corrente:



```text

0.3.0 — approvazione dopo integrazione delle ultime osservazioni su formattazione tempo e validazione UI

```



Cronologia:



```text

0.1.0 — prima bozza ChatGPT

0.2.0 — correzioni su audio, riavvio automatico, bridge, testi utente, dati dinamici, dipendenze OS e gestione errori

0.3.0 — chiarimento finale su formattazione del tempo rimanente nel bridge e limiti della validazione UI

```



Il documento integra le osservazioni iniziali del consiglio AI formato da:



```text

ChatGPT

DeepSeek

Gemini

```



Il documento è approvato dal project owner come base per i successivi documenti architetturali e di design.

