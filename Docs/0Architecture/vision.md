# CicloTimer — Visione del progetto

**Tipo documento:** visione architetturale iniziale  
**Stato:** APPROVED  
**Versione:** 0.4.0  
**Data:** 2026-06-01  
**Repository:** donato81/CicloTimer  

---

## 1. Scopo del progetto

CicloTimer è una piccola applicazione desktop per Windows, scritta in C#, pensata per impostare e gestire timer ciclici a sessioni ripetute.

L'app deve permettere all'utente di configurare una durata di sessione, impostare una soglia di avviso finale, avviare il timer e lasciare che le sessioni si ripetano automaticamente finché l'utente non le interrompe.

Il comportamento base è il seguente:

```text
l'utente imposta una durata totale della sessione
↓
l'utente imposta una durata della finestra finale di avviso
↓
il timer parte
↓
quando il tempo rimanente raggiunge la soglia configurata, parte l'avviso sonoro
↓
l'avviso continua durante la finestra finale configurata
↓
la sessione termina
↓
il contatore delle sessioni completate aumenta di 1
↓
se l'utente non interrompe il timer, parte subito una nuova sessione
```

Esempio:

```text
Durata sessione: 5 minuti
Avviso finale: ultimi 20 secondi

Da 05:00 a 00:21 il timer scorre normalmente.
Da 00:20 a 00:00 l'app emette un avviso sonoro.
A 00:00 la sessione viene contata come completata.
Il timer riparte automaticamente da 05:00.
```

Il progetto nasce con due obiettivi principali:

1. creare uno strumento semplice, stabile e accessibile per la gestione di sessioni temporizzate ripetute;
2. sperimentare un nuovo flusso di sviluppo assistito da agenti AI, mantenendo però una struttura professionale, ordinata e controllabile.

CicloTimer non deve diventare un progetto complesso nella prima fase. La priorità è costruire una base piccola, chiara, verificabile e facilmente estendibile.

---

## 2. Utente principale

L'utente principale è una persona che usa Windows e ha bisogno di un timer ciclico semplice, controllabile e accessibile.

Un caso d'uso reale previsto è quello di un professionista che lavora a sessioni con durata fissa e vuole tenere il conto delle sessioni completate.

Il conteggio delle sessioni serve a sapere quante unità di lavoro sono state svolte.

La prima versione deve mostrare il numero di sessioni completate, ma non deve calcolare compensi, tariffe, clienti, fatture o dati economici.

Il progetto deve prestare particolare attenzione agli utenti che utilizzano screen reader, in particolare NVDA.

L'app deve quindi essere utilizzabile anche senza affidarsi alla vista.

Questo significa che ogni funzione essenziale deve essere:

- raggiungibile da tastiera;
- comprensibile tramite screen reader;
- espressa con testi chiari;
- priva di informazioni disponibili solo visivamente.

---

## 3. Piattaforma di riferimento

La piattaforma di riferimento della prima versione è:

```text
Windows desktop
```

Il linguaggio di sviluppo previsto è:

```text
C#
```

La tecnologia UI scelta dovrà supportare correttamente le API di accessibilità di Windows, in particolare UI Automation, così da esporre controlli, nomi, stati e messaggi a NVDA.

La prima versione non deve essere progettata come applicazione mobile, web o cloud.

Eventuali estensioni future verso altre piattaforme potranno essere valutate solo dopo il completamento della prima versione stabile.

---

## 4. Prima versione minima

La prima versione minima deve concentrarsi sul funzionamento essenziale del timer ciclico a sessioni ripetute.

Le funzionalità minime previste sono:

1. impostare la durata totale della sessione;
2. impostare la durata della finestra di avviso finale;
3. esprimere le durate in minuti e secondi, con valori numerici interi;
4. avviare il timer;
5. mettere in pausa il timer mantenendo il tempo rimanente;
6. riprendere il timer dalla pausa;
7. resettare il timer riportandolo allo stato iniziale configurato;
8. mostrare a schermo il tempo rimanente della sessione corrente;
9. rendere il tempo rimanente disponibile allo screen reader senza annunciarlo automaticamente a ogni secondo;
10. permettere all'utente di consultare il tempo rimanente a richiesta tramite navigazione da tastiera o comando dedicato;
11. emettere un avviso sonoro durante la finestra finale configurata, se tale finestra è maggiore di zero;
12. dare priorità percettiva all'avviso del timer rispetto ad altri suoni attivi nel sistema, dove tecnicamente possibile;
13. terminare la sessione quando il timer arriva a zero;
14. aumentare di 1 il contatore delle sessioni completate alla fine di ogni sessione completata;
15. riavviare automaticamente una nuova sessione dopo la scadenza, salvo intervento dell'utente;
16. usare le funzioni principali da tastiera;
17. leggere correttamente stato, comandi e conteggio sessioni tramite screen reader.

Questa prima versione deve dimostrare che il cuore dell'app funziona in modo affidabile prima di aggiungere funzioni secondarie.

Non è richiesto nella prima versione un numero massimo configurabile di sessioni. Il timer può continuare a ripartire finché l'utente non lo mette in pausa o lo resetta.

---

## 5. Comportamento del ciclo

CicloTimer non è un timer lavoro/pausa.

La prima versione non gestisce due fasi alternate, come attività e recupero.

Il ciclo previsto è composto da una sola sessione temporizzata che si ripete automaticamente.

Ogni sessione ha:

1. una durata totale;
2. una finestra finale di avviso sonoro, opzionalmente disattivabile impostandola a zero;
3. una fine sessione;
4. un incremento del contatore delle sessioni completate;
5. una ripartenza automatica immediata, salvo intervento dell'utente.

Esempio logico:

```text
Sessione 1
↓
Avviso negli ultimi N secondi, se configurato
↓
Fine sessione
↓
Sessioni completate: 1
↓
Sessione 2
↓
Avviso negli ultimi N secondi, se configurato
↓
Fine sessione
↓
Sessioni completate: 2
```

Il calcolo economico collegato alle sessioni completate è fuori perimetro nella prima versione.

### 5.1 Pausa, reset e contatore

La pausa interrompe temporaneamente il timer mantenendo il tempo rimanente della sessione corrente.

Dopo una pausa, il timer riparte solo quando l'utente esegue il comando di ripresa.

Il reset annulla la sessione corrente e riporta il timer alla durata iniziale configurata.

Una sessione annullata tramite reset non deve incrementare il contatore delle sessioni completate.

Il reset non azzera automaticamente il contatore delle sessioni già completate.

Il contatore rappresenta le sessioni effettivamente concluse e mantiene il valore già raggiunto, salvo eventuale funzione esplicita futura di azzeramento.

Nella prima versione, l'intervento dell'utente che interrompe la ripartenza automatica è rappresentato almeno da:

1. pausa;
2. reset.

Se l'utente mette in pausa o resetta il timer, il ciclo automatico non deve ripartire autonomamente finché l'utente non esegue un nuovo comando esplicito.

### 5.2 Stati stabili ed eventi

Il timer deve distinguere tra stati stabili ed eventi.

Gli stati stabili descrivono la condizione corrente del timer.

Gli stati stabili principali sono:

```text
Timer fermo
Sessione in corso
Avviso finale in corso
Timer in pausa
```

La fine della sessione non è uno stato stabile permanente.

La fine della sessione è un evento.

L'evento di fine sessione produce almeno questi effetti:

1. comunicazione accessibile della sessione completata;
2. incremento del contatore delle sessioni completate;
3. ripartenza automatica della sessione successiva, se il timer non è stato interrotto dall'utente.

Esempio di messaggio evento:

```text
Sessione completata. Sessioni completate: 3.
```

La modalità tecnica precisa con cui l'evento viene comunicato allo screen reader sarà definita nei documenti di accessibilità o nei design tecnici successivi.

---

## 6. Funzionalità escluse dalla prima versione

Per mantenere il progetto piccolo e controllabile, la prima versione non include:

- account utente;
- sincronizzazione cloud;
- database remoto;
- statistiche avanzate;
- grafici;
- temi grafici complessi;
- sistema avanzato di profili;
- integrazione mobile;
- calendario;
- storico dettagliato delle sessioni;
- notifiche complesse;
- intelligenza artificiale integrata nell'app;
- funzioni social;
- esportazione dati;
- plugin o estensioni;
- numero massimo configurabile di sessioni;
- editor avanzato di sequenze o routine;
- fasi lavoro/pausa alternate;
- calcolo automatico del compenso;
- gestione clienti;
- gestione tariffe;
- fatturazione;
- azzeramento avanzato o storico del contatore;
- gestione avanzata dell'audio per singola applicazione.

Queste funzionalità non sono vietate per sempre, ma sono fuori perimetro nella prima fase.

Ogni proposta che introduce una di queste funzionalità deve essere respinta, salvo approvazione esplicita in un documento di design successivo.

---

## 7. Principi di accessibilità

L'accessibilità non è una rifinitura finale, ma un requisito fondativo del progetto.

La prima versione deve rispettare questi principi:

1. l'app deve essere completamente utilizzabile da tastiera;
2. ogni pulsante deve avere un nome chiaro;
3. ogni campo deve avere un'etichetta comprensibile;
4. lo stato del timer deve essere leggibile da screen reader;
5. il contatore delle sessioni completate deve essere leggibile da screen reader;
6. i cambiamenti importanti di stato devono essere esposti tramite testo chiaro e accessibile;
7. gli eventi importanti, come la sessione completata, devono essere comunicati in modo accessibile;
8. non devono esistere informazioni disponibili solo tramite colore, posizione o animazione;
9. i messaggi devono essere brevi, chiari e non ambigui;
10. l'ordine di navigazione tramite tastiera deve essere logico;
11. l'utente deve poter capire sempre se il timer è fermo, in sessione, in avviso finale o in pausa;
12. il tempo rimanente non deve essere annunciato automaticamente ogni secondo;
13. la tecnologia UI deve esporre correttamente controlli, nomi, ruoli e stati tramite UI Automation o meccanismo equivalente compatibile con NVDA.

Il riferimento operativo principale per i test di accessibilità è NVDA su Windows.

---

## 8. Avvisi e comunicazione dello stato

Durante la finestra finale configurata, l'app deve produrre un avviso sonoro, se la durata della finestra di avviso è maggiore di zero.

L'avviso sonoro ha lo scopo di informare l'utente che la sessione sta per terminare.

Se la durata della finestra di avviso finale è impostata a zero, l'avviso sonoro finale è disattivato.

Durante la finestra finale, l'avviso del timer deve essere chiaramente percepibile anche in presenza di altri suoni attivi nel sistema.

Dove tecnicamente possibile, l'app deve ridurre, sospendere o attenuare temporaneamente gli altri suoni durante la finestra di avviso, ripristinando la situazione precedente al termine dell'avviso.

Questa richiesta esprime un obiettivo funzionale dell'app, non una scelta tecnica già definitiva.

La soluzione tecnica precisa per dare priorità al suono del timer rispetto agli altri audio di sistema sarà definita nei design tecnici successivi.

L'avviso sonoro deve essere percepibile e non puramente visivo.

La modalità esatta dell'avviso, per esempio suono continuo, beep ripetuto o altra forma sonora, sarà definita nei design tecnici successivi.

Alla fine della sessione, l'app deve comunicare l'evento in modo accessibile.

L'avviso minimo previsto consiste in:

1. un suono durante gli ultimi secondi configurati della sessione, se l'avviso finale è attivo;
2. aggiornamento di un testo di stato leggibile da screen reader;
3. messaggio evento chiaro nell'interfaccia, ad esempio "Sessione completata";
4. aggiornamento del contatore delle sessioni completate.

L'app non deve basarsi solo su colore, animazione o elementi visivi.

Gli stati principali devono essere espressi in forma testuale, ad esempio:

```text
Timer fermo
Sessione in corso
Avviso finale in corso
Timer in pausa
Sessioni completate: 3
```

La fine della sessione deve essere trattata come evento accessibile, non come stato stabile permanente.

Esempio di evento:

```text
Sessione completata. Sessioni completate: 3.
```

Il tempo rimanente deve essere disponibile a richiesta, ma non deve essere annunciato automaticamente a ogni secondo.

---

## 9. Validazione degli input

L'app deve impedire l'avvio del timer se i valori inseriti non sono validi.

Sono considerati non validi:

- campi vuoti;
- valori negativi;
- valori non numerici;
- durata della sessione pari a zero;
- durata dell'avviso finale maggiore o uguale alla durata totale della sessione.

La durata dell'avviso finale può essere pari a zero.

Il valore zero per l'avviso finale significa:

```text
avviso sonoro finale disattivato
```

Regola sintetica:

```text
durata sessione > 0
durata avviso finale >= 0
durata avviso finale < durata sessione
```

Esempi:

```text
Durata sessione: 5 minuti
Avviso finale: 20 secondi
Valido

Durata sessione: 5 minuti
Avviso finale: 0 secondi
Valido, avviso sonoro disattivato

Durata sessione: 5 minuti
Avviso finale: 5 minuti
Non valido

Durata sessione: 0 minuti
Avviso finale: 20 secondi
Non valido
```

In caso di input non valido, l'app deve mostrare un messaggio di errore testuale, chiaro e leggibile da screen reader.

La prima versione può prevedere valori predefiniti per facilitare l'avvio rapido del timer.

Esempio di valori predefiniti possibili:

```text
Durata sessione: 5 minuti
Avviso finale: 20 secondi
```

Questi valori sono solo esempi e potranno essere confermati o modificati in un documento di design successivo.

---

## 10. Principi tecnici

Il progetto deve essere organizzato in modo semplice e separato.

La separazione logica prevista è:

```text
UI
Logica core
Collegamento tra UI e logica
Interazioni con il sistema operativo
Test
Documentazione
```

La UI deve occuparsi di mostrare informazioni e ricevere comandi dall'utente.

La logica core deve occuparsi delle regole del timer, senza dipendere direttamente dalla UI.

Il collegamento tra UI e logica deve tradurre le azioni dell'utente in comandi comprensibili dalla logica e trasformare lo stato interno in informazioni mostrate dalla UI.

Il livello di interazione con il sistema operativo deve occuparsi di funzioni come riproduzione audio, notifiche locali semplici e altre integrazioni Windows approvate.

Il livello di interazione con il sistema operativo dovrà anche valutare le possibilità tecniche per dare priorità al suono del timer rispetto agli altri audio di sistema durante la finestra di avviso.

La prima versione non deve introdurre una gestione avanzata dell'audio per singola applicazione, salvo che tale soluzione risulti necessaria e venga approvata in un design tecnico dedicato.

I test devono verificare soprattutto la logica del timer e, quando possibile, il comportamento dei passaggi principali.

---

## 11. Criteri di successo della prima versione

La prima versione può essere considerata riuscita solo se:

1. la durata della sessione può essere configurata;
2. la finestra finale di avviso può essere configurata;
3. la finestra finale di avviso può essere impostata a zero per disattivare l'avviso sonoro;
4. il timer può essere avviato, messo in pausa, ripreso e resettato;
5. l'avviso sonoro parte quando il tempo rimanente raggiunge la soglia configurata, se l'avviso è attivo;
6. l'avviso sonoro prosegue durante la finestra finale prevista, secondo la modalità definita dal design tecnico;
7. l'avviso del timer è chiaramente percepibile anche in presenza di altri suoni attivi, nei limiti tecnici definiti dal design;
8. la sessione termina correttamente quando il timer arriva a zero;
9. il contatore delle sessioni completate aumenta correttamente solo per le sessioni concluse;
10. una sessione annullata tramite reset non incrementa il contatore;
11. il reset non azzera automaticamente il contatore delle sessioni già completate;
12. il timer riparte automaticamente dopo ogni sessione, salvo intervento dell'utente;
13. pausa e reset interrompono la ripartenza automatica finché l'utente non dà un nuovo comando esplicito;
14. l'app non perde lo stato durante l'uso normale;
15. i comandi principali sono raggiungibili da tastiera;
16. NVDA legge correttamente i controlli principali;
17. lo stato del timer è sempre comprensibile;
18. gli eventi importanti, come la sessione completata, sono comunicati in modo accessibile;
19. il tempo rimanente è consultabile senza annunci continui e invasivi;
20. la logica del timer è separata dalla UI;
21. il progetto resta piccolo, leggibile e facilmente modificabile;
22. non sono state introdotte funzionalità fuori perimetro.

Se anche una sola funzione avanzata viene aggiunta a scapito della stabilità del timer base, la versione non deve essere considerata riuscita.

---

## 12. Regola anti-espansione

CicloTimer deve restare piccolo nella prima fase.

Ogni nuova proposta deve essere valutata con questa domanda:

```text
Serve direttamente a costruire la prima versione minima del timer ciclico accessibile a sessioni ripetute?
```

Se la risposta è no, la proposta deve essere rimandata.

Gli agenti AI, gli strumenti di sviluppo e i documenti tecnici non devono introdurre funzionalità non approvate.

Ogni modifica deve rispettare il perimetro stabilito dal documento di design attivo.

Il requisito di priorità dell'avviso sonoro rispetto agli altri audio di sistema non autorizza automaticamente l'introduzione di una gestione audio complessa o fuori perimetro.

Le soluzioni tecniche avanzate per il controllo dell'audio devono essere approvate tramite design dedicato.

---

## 13. Metodo di lavoro

Il progetto segue un flusso ordinato:

```text
Visione
↓
Architettura
↓
Regole documentali
↓
Regole di accessibilità
↓
Design
↓
Coding plan
↓
Todo operativo
↓
Implementazione
↓
Verifica
```

Nessuna fase di implementazione deve partire senza un documento di design approvato.

Cursor o altri agenti di coding devono essere usati come strumenti esecutivi, non come autorità architetturali autonome.

Le decisioni architetturali principali devono essere prese prima nei documenti, poi applicate al codice.

---

## 14. Stato del documento

Questo documento è approvato come visione iniziale del progetto CicloTimer.

Cronologia della bozza:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione delle osservazioni di DeepSeek e Gemini
0.3.0 — correzione del modello di ciclo dopo chiarimento del project owner
0.4.0 — integrazione finale di reset/contatore, avviso opzionale a zero, stati/eventi, priorità audio e approvazione del documento
```

Il modello corretto del progetto è:

```text
sessione temporizzata ripetuta
+
avviso sonoro negli ultimi N secondi, se configurato
+
priorità percettiva dell'avviso rispetto agli altri suoni, dove tecnicamente possibile
+
ripartenza automatica
+
conteggio sessioni completate
```

Il documento integra le osservazioni iniziali del consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Il documento è approvato dal project owner come base per i successivi documenti architetturali e di design.