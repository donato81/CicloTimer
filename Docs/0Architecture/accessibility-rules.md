# CicloTimer — Regole di accessibilità

**Tipo documento:** regole architetturali di accessibilità  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-01  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md  

---

## 1. Scopo del documento

Questo documento definisce le regole di accessibilità che devono guidare la progettazione e la futura implementazione di CicloTimer.

Il documento nasce dopo la visione del progetto e l'architettura generale.

Il suo compito è trasformare il principio generale di accessibilità in regole operative chiare, verificabili e vincolanti.

CicloTimer deve essere utilizzabile da una persona che usa Windows con screen reader, in particolare NVDA.

L'accessibilità non deve essere aggiunta alla fine come correzione estetica o come adattamento secondario.

L'accessibilità è parte della struttura del progetto.

Questo documento non definisce ancora:

1. il codice definitivo;
2. le classi concrete;
3. il dettaglio XAML definitivo;
4. il layout grafico finale;
5. una strategia completa di test automatizzati;
6. una libreria specifica per annunci screen reader;
7. una soluzione tecnica definitiva per ogni API di Windows.

Questi aspetti saranno definiti nei documenti di design successivi.

Questo documento stabilisce invece:

1. quali informazioni devono essere accessibili;
2. quali comandi devono essere raggiungibili da tastiera;
3. come devono essere trattati focus, stati, eventi ed errori;
4. quali annunci automatici sono ammessi;
5. quali annunci automatici devono essere evitati;
6. quali responsabilità appartengono alla UI, al bridge, alla logica core e agli altri livelli;
7. quali errori architetturali devono essere evitati dagli agenti AI.

---

## 2. Principio guida

Il principio guida dell'accessibilità di CicloTimer è:

```text
l'app deve poter essere usata senza affidarsi alla vista
````

Questo significa che tutte le funzioni essenziali devono essere disponibili anche tramite tastiera e screen reader.

L'utente deve poter:

1. capire lo stato corrente del timer;
2. configurare la durata della sessione;
3. configurare la durata dell'avviso finale;
4. avviare il timer;
5. mettere in pausa il timer;
6. riprendere il timer;
7. resettare il timer;
8. conoscere il tempo rimanente;
9. conoscere il numero di sessioni completate;
10. ricevere gli eventi importanti;
11. comprendere gli errori;
12. interrompere o controllare il ciclo senza usare il mouse.

L'app non deve richiedere vista, colore, posizione sullo schermo o animazioni per comprendere informazioni essenziali.

---

## 3. Utente di riferimento

L'utente di riferimento per l'accessibilità è una persona che usa Windows con NVDA.

NVDA è il riferimento operativo principale per le verifiche manuali.

Questo non significa che l'app debba funzionare solo con NVDA.

La UI deve usare controlli, nomi, ruoli, stati e testi compatibili con le API di accessibilità di Windows, in modo che anche altri strumenti assistivi possano interpretare correttamente l'interfaccia.

La prima versione deve dare priorità a:

```text
Windows desktop
+
C#
+
UI compatibile con UI Automation
+
NVDA
```

Il progetto non deve introdurre soluzioni personalizzate che aggirano il sistema di accessibilità della piattaforma senza necessità documentata.

---

## 4. Relazione con l'architettura generale

Le regole di accessibilità devono rispettare l'architettura generale del progetto.

In particolare:

1. la logica core non deve conoscere NVDA;
2. la logica core non deve inviare annunci allo screen reader;
3. la logica core non deve produrre testi finali per l'utente;
4. la UI deve esporre controlli accessibili;
5. il bridge deve trasformare stati, eventi, errori e valori dinamici in dati e testi mostrabili;
6. il sistema centralizzato dei testi utente deve essere usato anche per i testi accessibili statici;
7. i testi visivi e i testi accessibili devono essere coerenti;
8. gli eventi della logica core devono poter essere trasformati in comunicazioni accessibili;
9. la gestione degli errori deve produrre messaggi comprensibili anche tramite screen reader.

Schema logico:

```text
Logica core
↓
stati / eventi / errori / valori neutri
↓
Bridge
↓
testi e dati pronti per UI e accessibilità
↓
UI
↓
controlli, focus, lettura screen reader
```

La UI non deve inventare autonomamente testi accessibili divergenti dai testi visivi.

Il bridge non deve contenere codice specifico di NVDA.

L'eventuale uso di API di sistema per annunci o notifiche accessibili dovrà essere isolato in un livello appropriato e approvato tramite design tecnico.

---

## 5. Comandi essenziali da tastiera

Tutte le funzioni principali devono essere raggiungibili da tastiera.

I comandi essenziali sono:

1. modificare la durata della sessione;
2. modificare la durata dell'avviso finale;
3. avviare il timer;
4. mettere in pausa il timer;
5. riprendere il timer;
6. resettare il timer;
7. consultare il tempo rimanente;
8. consultare lo stato del timer;
9. consultare il contatore delle sessioni completate;
10. leggere eventuali messaggi di errore;
11. uscire dai campi di input senza restare bloccati;
12. spostarsi tra i controlli interattivi in ordine logico.

La navigazione tramite tasto Tab deve seguire l'ordine naturale di utilizzo dell'app sui controlli interattivi, come campi di input e pulsanti.

Le informazioni statiche o informative, come tempo rimanente, stato del timer, sessioni completate e messaggi evento o errore, non devono essere inserite nel ciclo di tabulazione se non sono controlli interattivi.

Questi elementi devono invece essere esposti come elementi informativi accessibili e leggibili tramite la normale navigazione dello screen reader, tramite l'esplorazione dell'albero di automazione o tramite meccanismi accessibili equivalenti definiti nei design successivi.

Ordine logico minimo previsto per la tabulazione:

```text
Durata sessione
↓
Durata avviso finale
↓
Avvia / Pausa / Riprendi
↓
Reset
```

Il tempo rimanente, lo stato del timer, il contatore delle sessioni completate e i messaggi di evento o errore restano informazioni essenziali, ma la loro consultazione non deve essere ottenuta rendendoli forzatamente tabulabili.

Il dettaglio finale dell'ordine di tabulazione sarà definito nel design UI.

Il principio obbligatorio è che l'utente non deve dover esplorare casualmente l'interfaccia per capire come usare il timer.

---

## 6. Focus

La gestione del focus deve essere prevedibile.

Il focus non deve spostarsi in modo inatteso senza motivo.

Quando l'utente preme un comando, il focus deve restare in una posizione comprensibile.

Esempi:

```text
L'utente preme Avvia.
Il timer parte.
Il focus non deve sparire.
Il focus può restare sul comando o spostarsi solo se il design lo giustifica chiaramente.
```

```text
L'utente inserisce un valore non valido.
L'app mostra un errore.
Il focus deve permettere all'utente di correggere facilmente il campo interessato.
```

In caso di errore di input, è preferibile che l'utente possa tornare rapidamente al campo da correggere.

La soluzione precisa sarà definita nel design UI, ma non è ammesso un comportamento in cui:

1. il focus viene perso;
2. il focus finisce su un elemento non interattivo senza utilità;
3. il focus torna all'inizio della finestra senza motivo;
4. l'utente non capisce dove si trova;
5. il messaggio di errore viene mostrato ma non è raggiungibile o non è comunicato.

Il focus non deve essere usato per simulare annunci continui del timer.

---

## 7. Controlli accessibili

Ogni controllo interattivo deve avere un nome chiaro.

I campi numerici devono avere etichette comprensibili.

Esempi di etichette corrette:

```text
Durata sessione, minuti
Durata sessione, secondi
Avviso finale, minuti
Avviso finale, secondi
Avvia
Pausa
Riprendi
Reset
```

Esempi da evitare:

```text
Campo 1
Input
Valore
Button
StartButton
TextBox2
```

Il nome accessibile deve essere comprensibile anche fuori dal contesto visivo.

Un utente che ascolta solo il nome del controllo deve poter capire a cosa serve.

I pulsanti devono esporre correttamente il proprio ruolo di pulsante.

I campi di inserimento devono esporre correttamente il proprio ruolo di campo modificabile.

I testi informativi importanti devono essere leggibili dallo screen reader.

I controlli disabilitati devono comunicare correttamente il loro stato disabilitato.

Se un comando non è disponibile in uno stato del timer, l'utente deve poterlo capire.

---

## 8. Stati del timer

Gli stati principali del timer devono essere sempre comunicabili in forma testuale.

Gli stati stabili principali sono:

```text
Timer fermo
Sessione in corso
Avviso finale in corso
Timer in pausa
```

Lo stato corrente deve essere visibile e leggibile da screen reader.

La UI deve mostrare uno stato già preparato dal bridge.

La UI non deve inventare varianti testuali autonome.

Esempio corretto:

```text
Stato: Sessione in corso
```

Esempio scorretto:

```text
Testo visivo: Timer attivo
Testo screen reader: Running
```

Gli stati tecnici interni non devono essere esposti direttamente all'utente.

Esempio scorretto:

```text
TimerState.Running
```

Esempio corretto:

```text
Sessione in corso
```

---

## 9. Eventi accessibili

Gli eventi importanti devono essere comunicati in modo accessibile.

Eventi principali:

```text
Timer avviato
Timer messo in pausa
Timer ripreso
Timer resettato
Avviso finale iniziato
Sessione completata
Nuova sessione avviata
Errore rilevato
```

La fine della sessione non è uno stato stabile permanente.

La fine della sessione è un evento.

Esempio di messaggio evento:

```text
Sessione completata. Sessioni completate: 3.
```

Gli eventi devono essere comunicati senza creare confusione con lo stato stabile successivo.

Esempio:

```text
Evento: Sessione completata. Sessioni completate: 3.
Stato successivo: Sessione in corso.
```

La modalità tecnica precisa con cui gli eventi vengono esposti allo screen reader sarà definita in un design tecnico successivo.

Nei design tecnici successivi, gli eventi candidati ad annuncio automatico potranno essere realizzati usando meccanismi nativi di accessibilità della piattaforma Windows, come notifiche UI Automation, proprietà equivalenti di Live Region o altri strumenti compatibili con la piattaforma scelta.

Questa indicazione non impone ancora una proprietà XAML specifica e non autorizza workaround non documentati, popup invisibili, sintesi vocale proprietaria o soluzioni parallele non necessarie.

Il principio obbligatorio è:

```text
gli eventi importanti non devono essere solo visivi
```

---

## 10. Tempo rimanente

Il tempo rimanente è un'informazione essenziale.

Deve essere visibile e consultabile tramite screen reader.

Il tempo rimanente non deve essere annunciato automaticamente a ogni secondo.

Regola obbligatoria:

```text
vietato annunciare automaticamente il tempo rimanente ogni secondo
```

Motivo:

```text
un annuncio continuo renderebbe l'app invasiva e difficilmente utilizzabile con screen reader
```

Il tempo rimanente deve essere disponibile a richiesta.

Esempi di modalità ammissibili, da definire nei design successivi:

1. testo visibile e raggiungibile tramite navigazione screen reader;
2. elemento informativo leggibile quando l'utente lo consulta;
3. comando dedicato per consultare il tempo rimanente;
4. scorciatoia da tastiera documentata;
5. pulsante o comando "Leggi tempo rimanente".

Queste sono possibilità, non decisioni definitive.

Il principio obbligatorio è:

```text
il tempo rimanente deve essere consultabile, ma non deve interrompere continuamente l'utente
```

Il tempo rimanente visualizzato, per esempio `04:59`, è un dato dinamico di stato.

L'etichetta che lo accompagna, per esempio `Tempo rimanente`, è un testo statico e deve provenire dal sistema centralizzato dei testi utente.

Il bridge prepara il testo finale da mostrare.

Esempio corretto:

```text
Tempo rimanente: 04:59
```

La logica core non deve produrre direttamente questa frase.

La logica core produce il valore neutro.

Esempio:

```text
299 secondi
```

Il bridge lo trasforma in testo leggibile.

---

## 11. Contatore delle sessioni completate

Il contatore delle sessioni completate deve essere visibile e leggibile da screen reader.

Il testo deve essere chiaro.

Esempio:

```text
Sessioni completate: 3
```

Il numero delle sessioni completate è un dato dinamico di stato.

L'etichetta `Sessioni completate` è un testo statico e deve provenire dal sistema centralizzato dei testi utente.

Il contatore deve rappresentare solo le sessioni effettivamente concluse.

Una sessione annullata tramite reset non deve incrementare il contatore.

Il reset non deve azzerare automaticamente il contatore.

Queste regole appartengono alla logica core, ma il risultato deve essere esposto in modo accessibile dalla UI.

Il contatore non deve essere comunicato solo con colore, posizione o simboli.

Esempio scorretto:

```text
Mostrare solo un numero grande senza etichetta accessibile.
```

Esempio corretto:

```text
Sessioni completate: 3
```

---

## 12. Avviso finale e accessibilità

Durante la finestra finale configurata, l'app deve produrre un avviso sonoro se la durata dell'avviso finale è maggiore di zero.

L'avviso finale è un evento percepibile tramite audio.

Lo stato "Avviso finale in corso" deve essere anche esposto in forma testuale.

Il suono non sostituisce il testo accessibile.

Il testo non sostituisce il suono.

Nella prima versione devono coesistere almeno:

1. avviso sonoro, se configurato;
2. stato testuale leggibile;
3. evento accessibile di ingresso nella finestra finale;
4. evento accessibile di fine sessione.

Se l'avviso finale è impostato a zero, il suono finale è disattivato.

In quel caso l'app deve comunque comunicare correttamente la fine della sessione tramite testo accessibile ed evento.

Il requisito di priorità percettiva dell'avviso rispetto ad altri suoni resta valido nei limiti tecnici definiti dai design successivi.

Questo documento non approva ancora soluzioni avanzate per modificare il volume di altre applicazioni.

---

## 13. Errori accessibili

Gli errori devono essere espressi con testi chiari e leggibili da screen reader.

Un errore non deve essere comunicato solo con:

1. colore rosso;
2. bordo evidenziato;
3. icona;
4. animazione;
5. vibrazione visiva;
6. posizione nella finestra.

Ogni errore visibile deve avere un testo accessibile equivalente.

Gli errori devono mantenere la distinzione tra:

```text
errore neutro o tecnico
↓
messaggio finale comprensibile per l'utente
```

Esempio di errore UI neutro:

```text
caratteri non numerici nei campi di configurazione del tempo
```

Esempio di messaggio utente accessibile:

```text
Inserisci solo numeri interi nei campi del tempo.
```

Altri esempi di messaggi utente corretti:

```text
La durata della sessione deve essere maggiore di zero.
L'avviso finale deve essere minore della durata della sessione.
Impossibile riprodurre l'avviso sonoro.
```

Gli errori tecnici grezzi non devono essere letti all'utente.

Esempio scorretto:

```text
System.IO.FileNotFoundException
```

Esempio corretto:

```text
Impossibile riprodurre l'avviso sonoro.
```

La UI non deve inventare messaggi di errore propri.

La logica core produce errori neutri.

Il bridge e il sistema centralizzato dei testi utente preparano i messaggi finali.

Il messaggio finale deve essere mostrato e reso accessibile.

---

## 14. Testi visivi e testi accessibili

I testi visivi e i testi accessibili devono essere coerenti.

Questo non significa che debbano essere sempre identici parola per parola.

Significa che non devono comunicare concetti diversi.

Esempio accettabile:

```text
Testo visivo: Avvia
Nome accessibile: Avvia timer
```

Esempio scorretto:

```text
Testo visivo: Avvia
Nome accessibile: Conferma
```

Esempio accettabile:

```text
Testo visivo: 04:59
Testo accessibile: Tempo rimanente: 4 minuti e 59 secondi
```

Esempio scorretto:

```text
Testo visivo: 04:59
Testo accessibile: valore
```

I testi statici devono provenire dal sistema centralizzato dei testi utente.

Non devono essere create stringhe accessibili sparse nei file UI.

Esempi di stringhe accessibili che devono essere controllate:

```text
Avvia timer
Pausa timer
Riprendi timer
Reset timer
Tempo rimanente
Sessioni completate
Stato timer
Errore
Sessione completata
```

---

## 15. Informazioni non solo visive

Nessuna informazione essenziale deve essere affidata solo a elementi visivi.

Sono vietate soluzioni in cui l'utente deve capire lo stato solo da:

1. colore;
2. posizione;
3. dimensione;
4. animazione;
5. icona;
6. lampeggio;
7. bordo;
8. immagine;
9. suono senza testo equivalente, quando l'informazione è persistente.

Esempio scorretto:

```text
Il timer diventa rosso durante l'avviso finale, ma non cambia il testo dello stato.
```

Esempio corretto:

```text
Il timer può diventare rosso, ma lo stato deve anche dire: "Avviso finale in corso".
```

Esempio scorretto:

```text
La sessione completata viene indicata solo con un'animazione.
```

Esempio corretto:

```text
La sessione completata produce un messaggio accessibile: "Sessione completata. Sessioni completate: 3."
```

Gli elementi visivi possono migliorare l'esperienza, ma non devono essere l'unico modo per comprendere l'app.

---

## 16. Annunci automatici

Gli annunci automatici devono essere pochi, utili e non invasivi.

Sono candidati ad annuncio automatico:

1. timer avviato;
2. timer messo in pausa;
3. timer ripreso;
4. timer resettato;
5. avviso finale iniziato;
6. sessione completata;
7. errore di input;
8. errore tecnico importante.

Non deve essere annunciato automaticamente:

1. ogni secondo del tempo rimanente;
2. ogni aggiornamento grafico non essenziale;
3. ogni cambiamento numerico del display;
4. informazioni ridondanti già lette dal focus;
5. eventi interni non utili all'utente.

Esempio scorretto:

```text
04:59
04:58
04:57
04:56
...
```

Esempio corretto:

```text
Avviso finale iniziato.
Sessione completata. Sessioni completate: 3.
```

La frequenza, la modalità tecnica e il canale degli annunci saranno definiti in un design tecnico dedicato.

Nei design successivi, gli annunci automatici dovranno preferire meccanismi nativi di accessibilità della piattaforma Windows, quando disponibili e adatti allo scopo.

Il principio architetturale è che gli annunci devono aiutare l'utente, non sovraccaricarlo.

---

## 17. Uso di controlli standard

Quando possibile, la UI deve usare controlli standard della piattaforma.

I controlli standard sono preferibili perché espongono più facilmente:

1. ruolo;
2. nome;
3. stato;
4. valore;
5. focus;
6. comportamento da tastiera;
7. compatibilità con UI Automation.

I controlli personalizzati devono essere evitati nella prima versione, salvo necessità documentata.

Se un controllo personalizzato viene introdotto, deve essere accessibile almeno quanto un controllo standard equivalente.

Non è accettabile introdurre un controllo personalizzato solo per motivi estetici se questo peggiora l'accessibilità.

---

## 18. Finestre, dialoghi e notifiche

La prima versione deve evitare finestre, dialoghi o notifiche complesse non necessarie.

Se vengono mostrati messaggi importanti, questi devono essere accessibili.

Un eventuale messaggio di errore deve:

1. essere testuale;
2. essere leggibile da screen reader;
3. non bloccare inutilmente il flusso;
4. permettere all'utente di correggere l'errore;
5. non causare perdita del focus.

Le notifiche locali semplici, se introdotte, devono passare dal livello di interazione con il sistema operativo.

La UI non deve chiamare direttamente API Windows per notifiche o annunci.

La necessità e il comportamento di eventuali notifiche saranno definiti nei design successivi.

---

## 19. Scorciatoie da tastiera

La prima versione può prevedere scorciatoie da tastiera, ma non deve dipendere esclusivamente da esse.

Ogni funzione essenziale deve essere raggiungibile anche tramite normale navigazione da tastiera.

Le scorciatoie possono essere utili per:

1. avviare o mettere in pausa;
2. resettare;
3. leggere il tempo rimanente;
4. leggere il numero di sessioni completate;
5. leggere lo stato corrente.

Questo documento non approva ancora scorciatoie specifiche.

Le scorciatoie definitive saranno definite nel design UI o in un design accessibilità tecnico.

Regola obbligatoria:

```text
nessuna scorciatoia deve essere l'unico modo per eseguire una funzione essenziale
```

---

## 20. Compatibilità con NVDA

NVDA è il riferimento principale per la verifica manuale della prima versione.

Una futura implementazione dovrà essere verificata almeno per questi comportamenti:

1. NVDA legge il titolo della finestra o il contesto principale;
2. NVDA legge correttamente le etichette dei campi;
3. NVDA legge correttamente i pulsanti;
4. NVDA comunica quando un pulsante è disabilitato;
5. NVDA permette di consultare il tempo rimanente;
6. NVDA permette di consultare lo stato del timer;
7. NVDA permette di consultare il contatore delle sessioni completate;
8. NVDA riceve o può leggere gli eventi importanti;
9. NVDA legge gli errori in modo comprensibile;
10. NVDA non viene saturato da annunci ogni secondo;
11. la navigazione da tastiera resta prevedibile;
12. il focus non viene perso dopo i comandi principali.

Questa lista non sostituisce un piano di test.

Serve come base architetturale per i futuri criteri di verifica.

---

## 21. Cosa è fuori perimetro in questo documento

Questo documento non introduce:

1. supporto multi-screen reader avanzato;
2. standard WCAG completi per applicazioni web;
3. temi ad alto contrasto personalizzati;
4. sintesi vocale interna all'app;
5. lettore vocale proprietario;
6. sistema avanzato di notifiche;
7. tutorial interattivo;
8. gestione profili di accessibilità;
9. configurazione dettagliata degli annunci;
10. supporto Braille specifico;
11. automazioni avanzate NVDA;
12. script NVDA dedicati;
13. gesture touch;
14. layout grafico definitivo;
15. sistema di localizzazione multilingua;
16. test automatizzati completi della UI accessibile.

Questi elementi non sono vietati per sempre.

Sono fuori perimetro nella prima fase, salvo approvazione esplicita in un design successivo.

---

## 22. Regole per gli agenti AI

Gli agenti AI che lavoreranno sul progetto devono rispettare queste regole:

1. non introdurre logica del timer nella UI per facilitare l'accessibilità;
2. non far dipendere la logica core da NVDA, UI Automation o API Windows;
3. non creare stringhe accessibili sparse nei file UI;
4. non duplicare testi visivi e accessibili con significati diversi;
5. non annunciare automaticamente il tempo rimanente ogni secondo;
6. non affidare stati o errori solo a colore, icone o animazioni;
7. non introdurre controlli personalizzati non accessibili;
8. non introdurre scorciatoie come unico modo per usare una funzione;
9. non creare notifiche o annunci complessi senza design approvato;
10. non spostare il focus in modo arbitrario;
11. non trasformare eventi interni non utili in annunci utente;
12. non modificare più livelli architetturali senza autorizzazione del design attivo;
13. non sostituire il sistema centralizzato dei testi utente con stringhe locali;
14. non confondere dati dinamici con stringhe statiche;
15. non introdurre funzionalità accessorie fuori perimetro;
16. non rendere tabulabili elementi informativi statici solo per farli leggere dallo screen reader;
17. non usare popup invisibili, sintesi vocale proprietaria o workaround non documentati per simulare annunci accessibili.

Gli agenti AI devono considerare l'accessibilità come requisito strutturale, non come refactoring finale.

---

## 23. Criteri di validità accessibile

Una futura implementazione sarà considerata coerente con questo documento solo se:

1. tutte le funzioni essenziali sono raggiungibili da tastiera;
2. l'ordine di navigazione tra controlli interattivi è logico;
3. gli elementi informativi statici non sono resi forzatamente tabulabili;
4. i campi hanno etichette comprensibili;
5. i pulsanti hanno nomi chiari;
6. lo stato del timer è leggibile da screen reader;
7. il tempo rimanente è consultabile tramite screen reader;
8. il tempo rimanente non viene annunciato automaticamente a ogni secondo;
9. il contatore delle sessioni completate è leggibile da screen reader;
10. gli eventi importanti sono comunicati in modo accessibile;
11. gli errori sono testuali e leggibili da screen reader;
12. il focus non viene perso durante l'uso normale;
13. i testi visivi e accessibili sono coerenti;
14. le informazioni essenziali non sono affidate solo alla vista;
15. la logica core resta indipendente da NVDA, UI e Windows;
16. la UI non inventa testi finali propri;
17. il bridge prepara testi e dati mostrabili;
18. i testi statici sono controllati tramite il sistema centralizzato dei testi utente;
19. i dati dinamici sono trattati come valori di stato;
20. NVDA può leggere correttamente i controlli principali;
21. non sono state introdotte funzionalità fuori perimetro.

Se una soluzione funziona visivamente ma non è utilizzabile con NVDA e tastiera, non deve essere considerata valida.

Se una soluzione è accessibile ma viola la separazione architetturale approvata, non deve essere considerata valida.

---

## 24. Stato del documento

Questo documento è approvato come regole architetturali di accessibilità del progetto CicloTimer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini e correzione della distinzione tra tabulazione, elementi informativi, dati dinamici e annunci accessibili
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione delle osservazioni dei consiglieri AI su ordine di tabulazione, elementi informativi statici, errori UI, dati dinamici e orientamento controllato a UI Automation / Live Region
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. distinzione tra controlli interattivi tabulabili ed elementi informativi leggibili tramite screen reader;
2. divieto di rendere forzatamente tabulabili tempo rimanente, stato, contatore e messaggi;
3. chiarimento tra errore neutro e messaggio utente accessibile;
4. richiamo alla distinzione tra dati dinamici e testi statici centralizzati;
5. nota non vincolante sull'uso futuro di meccanismi nativi Windows, UI Automation o Live Region per annunci accessibili.

Il documento è approvato dal project owner come base per i successivi documenti architetturali e di design.
