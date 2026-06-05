# DESIGN 008 — Accessibilità UI e rifinitura interazione

## Metadati

* Tipo documento: Design tecnico
* Codice: 008
* Titolo: Accessibilità UI e rifinitura interazione
* Versione: 0.2.0
* Stato: APPROVATO
* Progetto: CicloTimer
* Repository: donato81/CicloTimer
* Data: 2026-06-05
* Documenti collegati:

  * docs/0-architecture/document-standards.md
  * docs/0-architecture/architecture.md
  * docs/0-architecture/internal-api.md
  * docs/0-architecture/accessibility-rules.md
  * docs/1-design/001-DESIGN_core-timer-engine.md
  * docs/1-design/002-DESIGN_bridge-ui-logica-timer.md
  * docs/1-design/003-DESIGN_sistema-testi-centralizzati.md
  * docs/1-design/004-DESIGN_audio-service-e-audio-focus.md
  * docs/1-design/005-DESIGN_orchestratore-applicativo-timer.md
  * docs/1-design/006-DESIGN_gestore-timer-reale_v0.1.0.md
  * docs/1-design/006-bis-DESIGN_statechanged-notification.md
  * docs/1-design/007-DESIGN_ui-wpf-minima.md

---

## 1. Scopo del documento

Questo documento definisce il design del blocco 008, dedicato all’accessibilità della UI WPF e alla rifinitura dell’interazione utente.

Il blocco 007 ha introdotto la prima finestra WPF minima dell’applicazione CicloTimer.

La UI è già funzionante e permette di:

* configurare la durata della sessione;
* configurare la durata dell’avviso finale;
* avviare il timer;
* mettere in pausa il timer;
* riprendere il timer;
* resettare il timer;
* vedere il tempo rimanente;
* vedere lo stato corrente;
* vedere il numero di sessioni completate.

Il blocco 008 non introduce nuove funzionalità timer.

Il suo scopo è rendere la UI già esistente realmente usabile da tastiera e con screen reader, in particolare NVDA su Windows.

Il blocco 008 deve quindi trasformare la UI minima del blocco 007 in una UI accessibile, prevedibile, comprensibile e verificabile.

---

## 2. Stato precedente

Alla fine del blocco 007 il progetto dispone di:

* Core timer engine completato;
* Bridge UI-logica completato;
* sistema di testi centralizzati completato;
* servizio audio completato;
* orchestratore applicativo completato;
* runner temporale reale completato;
* notifica `StateChanged` sull’orchestratore completata;
* UI WPF minima funzionante;
* ViewModel WPF funzionante;
* comandi principali collegati;
* aggiornamento countdown funzionante;
* build verde;
* test verdi.

La UI si apre correttamente.

Il timer gira.

Il countdown si aggiorna.

L’orchestratore riceve i tick.

Manca ancora una rifinitura specifica sull’accessibilità reale.

In particolare, il blocco 007 non aveva come obiettivo:

* accessibilità avanzata;
* gestione completa con NVDA;
* Live Region o meccanismo WPF equivalente;
* annunci accessibili degli eventi importanti;
* ordine Tab definitivo;
* gestione focus completa;
* nomi accessibili completi;
* errori pienamente accessibili;
* rifinitura dell’esperienza da tastiera.

Questi aspetti appartengono al blocco 008.

---

## 3. Problema da risolvere

Una UI può essere visivamente funzionante ma non essere realmente usabile da una persona non vedente.

Per CicloTimer questo non è accettabile.

L’utente deve poter usare il timer senza mouse e senza affidarsi alla vista.

Il problema da risolvere è quindi questo:

```text
la UI WPF esiste e funziona,
ma deve essere resa accessibile, prevedibile e chiara con tastiera e NVDA
```

In particolare bisogna garantire che:

* l’ordine di tabulazione sia logico;
* ogni controllo interattivo abbia un nome accessibile chiaro;
* i campi numerici siano comprensibili anche fuori dal contesto visivo;
* i pulsanti comunichino correttamente funzione e stato;
* il focus iniziale sia prevedibile;
* il focus non venga perso;
* gli errori siano comunicati in modo comprensibile;
* gli eventi importanti del timer non siano solo visivi;
* il tempo rimanente sia consultabile ma non annunciato ogni secondo;
* lo stato corrente sia leggibile;
* il contatore delle sessioni completate sia leggibile;
* la UI non introduca logica timer;
* i testi restino centralizzati;
* NVDA possa interpretare correttamente la schermata.

---

## 4. Obiettivo del blocco 008

L’obiettivo del blocco 008 è rendere accessibile la UI WPF minima già esistente.

La UI deve:

* essere navigabile da tastiera;
* avere un ordine Tab naturale;
* avere un focus iniziale prevedibile;
* avere nomi accessibili chiari;
* avere etichette corrette per i controlli;
* esporre stato e informazioni essenziali allo screen reader;
* gestire il focus in modo prevedibile;
* comunicare gli errori in modo accessibile;
* comunicare gli eventi principali senza disturbare continuamente l’utente;
* rispettare le regole di accessibilità già approvate;
* continuare a essere visivamente ordinata per utenti vedenti;
* non modificare il comportamento logico del timer;
* non introdurre funzionalità future;
* non anticipare estensioni non approvate.

---

## 5. Non obiettivi

Il blocco 008 non deve introdurre:

* nuove funzionalità timer;
* timer ciclico se non già supportato dai contratti esistenti;
* comando dedicato per lettura rapida del tempo rimanente;
* scorciatoie globali;
* scorciatoie locali nuove non già approvate;
* configurazioni persistenti;
* salvataggio preferenze;
* tema scuro;
* scelta tema;
* notifiche Windows;
* minimizzazione in tray;
* installer;
* packaging;
* nuove icone definitive;
* animazioni avanzate;
* statistiche;
* cronologia sessioni;
* profili timer;
* sintesi vocale proprietaria;
* dipendenze dirette da NVDA;
* automazioni esterne verso NVDA;
* modifiche al Core;
* modifiche alle regole del timer;
* modifiche al Bridge non necessarie;
* modifiche all’orchestratore non necessarie;
* modifiche al runner temporale;
* nuove regole audio;
* nuovi eventi core;
* nuovi stati stabili del timer.

Il blocco 008 non deve trasformarsi in una riscrittura della UI.

La UI può essere rifinita, ma non riprogettata da zero.

---

## 6. Posizione architetturale

Il blocco 008 si colloca nel livello UI e ViewModel.

Flusso architetturale già approvato:

```text
Utente
   ↓
View WPF
   ↓
ViewModel
   ↓
ITimerAppOrchestrator
   ↓
Bridge
   ↓
Core / Audio
```

Il blocco 008 deve rispettare questa separazione.

La UI può occuparsi di:

* esposizione accessibile dei controlli;
* ordine di tabulazione;
* focus;
* proprietà WPF di accessibilità;
* visualizzazione di testi già pronti;
* comunicazione accessibile di eventi e messaggi;
* rifinitura dell’esperienza da tastiera.

La UI non deve occuparsi di:

* calcolare il tempo;
* decidere lo stato del timer;
* incrementare sessioni;
* decidere quando una sessione è completata;
* decidere quando inizia l’avviso finale;
* generare testi utente non centralizzati;
* conoscere dettagli interni del Core;
* conoscere direttamente NVDA;
* usare soluzioni parallele non approvate per parlare con lo screen reader.

---

## 7. Posizione nel repository

Il blocco 008 può modificare principalmente:

```text
views/
view-models/
locales/
tests/
```

La modifica di `locales/` è ammessa solo per aggiungere testi utente o testi accessibili necessari alla UI.

Il blocco 008 non deve modificare, salvo necessità documentata e minima:

```text
models/CicloTimer.Core/
services/CicloTimer.Audio/
services/CicloTimer.App/Timing/
```

Il blocco 008 non deve spostare file dei blocchi precedenti.

Il blocco 008 non deve rinominare cartelle architetturali.

---

## 8. Livelli coinvolti

Il blocco 008 coinvolge:

* View WPF;
* ViewModel;
* Localization;
* test del ViewModel e, se presenti, test UI/accessibilità leggeri;
* verifica manuale con NVDA.

Il blocco 008 non coinvolge direttamente:

* Core timer engine;
* regole del timer;
* calcolo tempo rimanente;
* calcolo sessioni completate;
* logica audio;
* runner temporale reale;
* API interne del timer.

---

## 9. Regola fondamentale

La regola fondamentale del blocco 008 è:

```text
accessibilità della UI, non nuova logica timer
```

Ogni modifica deve essere valutata con questa domanda:

```text
questa modifica rende la UI più usabile e comprensibile
oppure sta introducendo comportamento nuovo del timer?
```

Se introduce comportamento nuovo del timer, è fuori perimetro.

---

## 10. Principi di accessibilità da rispettare

Il blocco 008 deve rispettare i principi già approvati:

```text
l'app deve poter essere usata senza affidarsi alla vista
```

```text
il tempo rimanente deve essere consultabile,
ma non deve essere annunciato automaticamente ogni secondo
```

```text
gli eventi importanti non devono essere solo visivi
```

```text
la logica core non deve conoscere NVDA
```

```text
la UI non deve inventare testi accessibili divergenti dai testi visivi
```

```text
i testi visivi e i testi accessibili devono essere coerenti
```

```text
il focus non deve spostarsi in modo inatteso senza motivo
```

---

## 11. Ordine di tabulazione

La navigazione con Tab deve seguire l’ordine naturale di utilizzo.

Ordine logico previsto:

```text
Durata sessione, minuti
↓
Durata sessione, secondi
↓
Durata avviso finale, minuti
↓
Durata avviso finale, secondi
↓
Pulsante principale: Avvia / Pausa / Riprendi
↓
Reset
```

Il controllo `NumericStepControl` deve comportarsi come un controllo compatto nel flusso Tab principale.

Regola:

```text
ogni NumericStepControl deve occupare una posizione chiara e non ripetitiva nel flusso Tab principale
```

Il Tab non deve costringere l’utente ad attraversare troppi elementi interni per modificare un singolo valore numerico.

Il valore principale del controllo deve essere raggiungibile da tastiera.

L’aumento e la diminuzione del valore devono essere disponibili da tastiera senza obbligare l’utente a passare con Tab su pulsanti interni anonimi o ripetitivi.

La soluzione tecnica precisa sarà definita nel Coding Plan, privilegiando un comportamento compatto, semplice e verificabile con NVDA.

Soluzioni ammesse:

* controllo numerico compatto;
* campo numerico con gestione tastiera chiara;
* pulsanti più/meno accessibili solo se non appesantiscono il flusso Tab;
* esposizione del valore corrente in modo comprensibile;
* comportamento verificato manualmente con NVDA.

Soluzioni vietate:

* controllo modificabile solo con mouse;
* pulsanti senza nome;
* ordine Tab casuale;
* focus che entra in elementi decorativi;
* elementi informativi non interattivi inseriti nel Tab senza motivo;
* obbligo di tabulare su tre o più elementi per ogni singolo valore numerico se questo rende la navigazione pesante.

---

## 12. Elementi non tabulabili

Le informazioni statiche o informative non devono essere forzatamente inserite nel ciclo di tabulazione.

Non devono essere tabulabili, salvo motivo specifico:

* titolo applicazione;
* card decorative;
* testo tempo rimanente;
* testo stato corrente;
* testo sessioni completate;
* messaggi informativi non interattivi.

Questi elementi devono però essere accessibili allo screen reader come informazioni leggibili.

Regola:

```text
non tabulabile non significa invisibile allo screen reader
```

La UI deve permettere a NVDA di leggere le informazioni importanti tramite normale esplorazione o tramite meccanismo accessibile appropriato.

---

## 13. Focus iniziale

All’apertura della finestra, il focus deve posizionarsi in modo prevedibile.

Regola:

```text
all’apertura dell’applicazione,
il focus deve posizionarsi sul primo controllo utile della configurazione
```

Il primo controllo utile previsto è:

```text
Durata sessione, minuti
```

Motivo:

```text
un utente NVDA deve capire subito dove si trova
e da dove può iniziare a usare l’applicazione
```

La finestra deve inoltre esporre un titolo comprensibile allo screen reader.

Il focus iniziale non deve andare su:

* elementi decorativi;
* contenitori grafici;
* testi statici non interattivi;
* pulsanti se prima è necessario configurare la durata;
* elementi invisibili;
* controlli disabilitati.

Se in futuro la UI cambierà struttura, il principio resta:

```text
il focus iniziale deve arrivare sul primo controllo realmente utile
```

---

## 14. Nomi accessibili dei campi

Ogni campo numerico deve avere un nome accessibile chiaro.

Nomi minimi richiesti:

```text
Durata sessione, minuti
Durata sessione, secondi
Durata avviso finale, minuti
Durata avviso finale, secondi
```

Il nome deve essere comprensibile anche se ascoltato isolatamente.

Esempio corretto:

```text
Durata sessione, minuti
```

Esempio scorretto:

```text
Minuti
```

Motivo:

```text
"Minuti" da solo non dice se si riferisce alla sessione o all'avviso finale
```

Esempi vietati:

```text
TextBox1
Input
Valore
Campo
NumericBox
```

---

## 15. Nomi accessibili dei pulsanti

Ogni pulsante deve avere un nome accessibile chiaro.

Pulsanti principali:

```text
Avvia
Pausa
Riprendi
Reset
```

Se il pulsante principale cambia testo in base allo stato, anche il nome accessibile deve essere coerente.

Esempio:

```text
Testo visivo: Pausa
Nome accessibile: Pausa
```

Esempio scorretto:

```text
Testo visivo: Pausa
Nome accessibile: Avvia
```

Il testo visivo, il nome accessibile e lo stato di abilitazione del pulsante principale devono rimanere sempre coerenti tra loro.

Il pulsante non deve mai presentarsi a NVDA con una combinazione ambigua, per esempio:

```text
Nome accessibile: Avvia
Stato reale: timer già in corso
```

Oppure:

```text
Nome accessibile: Pausa
Comando reale: Riprendi
```

I pulsanti disabilitati devono comunicare il proprio stato disabilitato attraverso il comportamento WPF standard.

Non devono essere creati falsi pulsanti usando elementi non interattivi.

---

## 16. Stato corrente

Lo stato corrente del timer deve essere sempre visibile e leggibile.

Forma concettuale:

```text
Stato: Timer fermo
Stato: Sessione in corso
Stato: Avviso finale in corso
Stato: Timer in pausa
```

La UI deve mostrare testi già preparati dal livello appropriato.

La UI non deve esporre stati tecnici come:

```text
Stopped
Running
FinalAlert
Paused
TimerState.Running
```

Gli stati tecnici appartengono ai livelli interni.

L’utente deve sentire testi comprensibili.

---

## 17. Tempo rimanente

Il tempo rimanente è informazione essenziale.

Deve essere:

* visibile;
* grande e chiaro per utenti vedenti;
* leggibile da screen reader;
* aggiornato durante il timer;
* non annunciato automaticamente ogni secondo.

Regola obbligatoria:

```text
vietato annunciare automaticamente il tempo rimanente ogni secondo
```

Motivo:

```text
un timer che parla ogni secondo rende l'app inutilizzabile con screen reader
```

Il blocco 008 deve garantire che il testo del tempo rimanente sia accessibile come informazione consultabile.

Esempio corretto:

```text
Tempo rimanente: 04:59
```

Il blocco 008 non introduce un comando dedicato di lettura rapida del tempo rimanente.

Tale comando potrà essere valutato in un blocco futuro se l’uso reale con NVDA lo renderà necessario.

---

## 18. Sessioni completate

Il contatore delle sessioni completate deve essere visibile e leggibile.

Forma prevista:

```text
Sessioni completate: 0
Sessioni completate: 1
Sessioni completate: 2
```

La UI non deve incrementare il contatore.

La UI deve solo mostrare il dato ricevuto.

Il reset non deve introdurre nuove regole sul contatore.

Il blocco 008 non modifica il comportamento del contatore.

---

## 19. Messaggi evento

Gli eventi importanti devono essere comunicati in modo accessibile.

Eventi candidati a comunicazione accessibile nel blocco 008:

```text
Timer avviato
Timer messo in pausa
Timer ripreso
Timer resettato
Avviso finale iniziato
Sessione completata
Errore rilevato
```

Il blocco 008 non deve introdurre annunci relativi a funzionalità non ancora presenti.

In particolare, non deve introdurre annunci relativi a:

```text
nuova sessione ciclica avviata
ciclo automatico successivo
sequenze future non ancora progettate
```

Motivo:

```text
il blocco 007 non ha introdotto un timer ciclico,
quindi il blocco 008 non deve anticipare eventi ciclici futuri
```

Il blocco 008 deve prevedere un’area messaggi accessibile nella UI oppure un meccanismo WPF equivalente.

Questa area deve:

* mostrare l’ultimo evento importante;
* essere leggibile da screen reader;
* non ricevere focus in modo aggressivo;
* non interrompere continuamente l’utente;
* non essere usata per il tempo rimanente a ogni secondo.

Esempio di messaggio evento:

```text
Timer avviato.
```

```text
Sessione completata. Sessioni completate: 3.
```

La modalità tecnica precisa sarà definita nel Coding Plan.

Sono ammesse soluzioni WPF standard compatibili con UI Automation.

Sono vietate soluzioni come:

* sintesi vocale proprietaria;
* chiamate dirette a NVDA;
* popup invisibili non documentati;
* finestre temporanee finte;
* focus spostato solo per far leggere un messaggio;
* annuncio del countdown ogni secondo.

---

## 20. Errori accessibili

Gli errori devono essere comprensibili e raggiungibili.

Quando l’utente inserisce una configurazione non valida:

* il messaggio deve essere visualizzato;
* il messaggio deve essere leggibile da screen reader;
* il focus deve permettere di correggere il campo interessato;
* l’utente non deve restare bloccato;
* l’errore non deve essere solo colore;
* l’errore non deve essere solo bordo rosso;
* il testo dell’errore deve venire dal sistema centralizzato dei testi.

Esempio corretto:

```text
La durata della sessione deve essere maggiore di zero.
```

Esempio scorretto:

```text
Errore
```

Esempio scorretto:

```text
Campo non valido
```

Il messaggio deve spiegare cosa non va e, se possibile, come correggere.

---

## 21. Focus

La gestione del focus deve essere prevedibile.

Regole:

* il focus iniziale deve arrivare sul primo controllo utile;
* il focus non deve sparire;
* il focus non deve finire su elementi decorativi;
* il focus non deve tornare all’inizio della finestra senza motivo;
* il focus non deve spostarsi continuamente durante il countdown;
* il focus non deve essere usato per simulare annunci;
* dopo un errore, il focus deve aiutare l’utente a correggere;
* dopo Avvia/Pausa/Riprendi/Reset, il focus deve restare in una posizione comprensibile.

Comportamento consigliato:

```text
Dopo Avvia:
il focus resta sul pulsante principale, che ora diventa Pausa.

Dopo Pausa:
il focus resta sul pulsante principale, che ora diventa Riprendi.

Dopo Riprendi:
il focus resta sul pulsante principale, che ora diventa Pausa.

Dopo Reset:
il focus resta su Reset oppure torna al primo campo configurazione solo se motivato.
```

Il coding plan potrà raffinare il comportamento finale, ma deve rispettare la prevedibilità.

---

## 22. NumericStepControl

Il blocco 007 ha introdotto o previsto un controllo numerico guidato.

Nel blocco 008 questo controllo deve essere verificato e rifinito dal punto di vista accessibile.

Il controllo deve:

* avere nome accessibile chiaro;
* essere usabile da tastiera;
* comunicare il valore corrente;
* permettere aumento e diminuzione senza mouse;
* non creare un ordine Tab confuso;
* comportarsi come controllo compatto nel flusso Tab principale;
* non esporre pulsanti anonimi;
* non leggere solo simboli come “più” o “meno” senza contesto.

Regola:

```text
il NumericStepControl deve essere esposto a NVDA
come controllo numerico comprensibile, compatto e usabile da tastiera
```

Il Tab non deve diventare ripetitivo o faticoso.

L’utente deve poter capire:

* quale valore sta modificando;
* qual è il valore corrente;
* come aumentarlo;
* come diminuirlo;
* quando il valore non può andare oltre i limiti ammessi.

Esempi corretti di concetto accessibile:

```text
Durata sessione, minuti: 5
Aumenta durata sessione, minuti
Diminuisci durata sessione, minuti
```

Esempi scorretti:

```text
+
-
Button
0
```

Gli esempi sopra indicano l’intenzione accessibile.

Il dettaglio tecnico preciso, compresa l’eventuale gestione tramite frecce direzionali o altri meccanismi WPF adeguati, sarà definito nel Coding Plan.

Il controllo non deve contenere logica timer.

Deve solo permettere all’utente di scegliere valori numerici.

---

## 23. Testi accessibili e Localization

Tutti i testi utente nuovi devono passare dal sistema centralizzato di localizzazione.

Il blocco 008 può aggiungere chiavi per:

* nomi accessibili dei controlli;
* descrizioni accessibili;
* messaggi evento;
* messaggi errore;
* istruzioni brevi;
* etichette più chiare.

Non sono ammesse stringhe utente hardcoded sparse in XAML o nel ViewModel, salvo costanti tecniche non rivolte all’utente.

Esempi di testi da centralizzare:

```text
Durata sessione, minuti
Durata sessione, secondi
Durata avviso finale, minuti
Durata avviso finale, secondi
Aumenta durata sessione, minuti
Diminuisci durata sessione, minuti
Timer avviato.
Timer messo in pausa.
Timer ripreso.
Timer resettato.
Avviso finale iniziato.
Sessione completata. Sessioni completate: {0}.
```

Il coding plan dovrà verificare la struttura reale del progetto Localization prima di indicare i nomi definitivi delle chiavi.

---

## 24. Messaggi visivi e messaggi accessibili

I messaggi visivi e accessibili devono essere coerenti.

Esempio corretto:

```text
Visivo:
Timer avviato.

Screen reader:
Timer avviato.
```

Esempio ammesso:

```text
Visivo:
Sessione completata.

Screen reader:
Sessione completata. Sessioni completate: 3.
```

Questa differenza è ammessa solo se il testo accessibile aggiunge contesto utile senza contraddire il testo visivo.

Esempio scorretto:

```text
Visivo:
Timer avviato.

Screen reader:
Errore.
```

Esempio scorretto:

```text
Visivo:
Sessione in corso.

Screen reader:
Running.
```

---

## 25. Annunci automatici

Il blocco 008 ammette annunci automatici solo per eventi importanti già appartenenti al comportamento attuale dell’applicazione.

Eventi ammessi:

* timer avviato;
* timer messo in pausa;
* timer ripreso;
* timer resettato;
* avviso finale iniziato;
* sessione completata;
* errore rilevato.

Eventi non ammessi:

* aggiornamento del tempo ogni secondo;
* aggiornamento visuale ordinario del countdown;
* ogni cambio minimo di valore;
* ogni tick del runner;
* annunci di nuove sessioni cicliche future;
* annunci di sequenze automatiche non ancora progettate.

Regola:

```text
annunciare eventi, non il flusso continuo del tempo
```

Gli annunci ordinari devono avere comportamento non aggressivo.

Regola:

```text
gli annunci ordinari devono essere equivalenti a un comportamento Polite
```

Significa:

```text
l’annuncio non deve interrompere bruscamente l’utente
mentre sta leggendo o navigando nella finestra
```

Solo gli errori bloccanti possono usare un comportamento più urgente, equivalente ad `Assertive`.

---

## 26. Live Region / meccanismo equivalente

Il blocco 008 può introdurre una soluzione WPF standard per rendere leggibili gli eventi importanti.

Il design non impone ancora una specifica proprietà XAML definitiva.

Il coding plan dovrà valutare la soluzione più coerente con WPF, UI Automation e NVDA.

Sono ammissibili:

* proprietà WPF di accessibilità;
* AutomationProperties appropriate;
* LiveSetting, se disponibile e coerente;
* area testuale accessibile aggiornata solo sugli eventi;
* meccanismo equivalente compatibile con screen reader.

Regola:

```text
il meccanismo di annuncio deve essere validato realmente con NVDA
```

Se la sola area testuale aggiornata non viene annunciata da NVDA, il Coding Plan dovrà prevedere un meccanismo WPF/UI Automation equivalente.

Il Design non impone il dettaglio tecnico del fallback.

Il dettaglio tecnico appartiene al Coding Plan.

Sono vietati:

* dipendenza diretta da NVDA;
* sintesi vocale custom;
* lettura forzata tramite focus continuo;
* finestre invisibili create solo per far parlare lo screen reader;
* workaround non documentati.

---

## 27. Tastiera

Tutte le funzioni essenziali devono essere disponibili da tastiera.

Funzioni essenziali:

* modificare durata sessione;
* modificare durata avviso finale;
* avviare;
* mettere in pausa;
* riprendere;
* resettare;
* leggere stato;
* leggere tempo rimanente tramite normale esplorazione accessibile della UI;
* leggere sessioni completate;
* comprendere errori.

Il blocco 008 non introduce scorciatoie avanzate.

Sono fuori perimetro:

* scorciatoie globali di Windows;
* hotkey attive quando la finestra non è in primo piano;
* configurazione scorciatoie;
* comandi vocali;
* comando dedicato per annunciare il tempo rimanente;
* sistema complesso di KeyBinding.

Se in futuro si riterrà utile introdurre una scorciatoia locale per leggere rapidamente tempo rimanente e stato, questa sarà progettata in un blocco separato o in una revisione esplicita approvata dal project owner.

---

## 28. Aspetto visivo

Il blocco 008 può migliorare piccoli aspetti visivi se servono anche alla chiarezza.

Sono ammessi:

* migliorare contrasto;
* migliorare spaziatura;
* rendere più chiari errori e stati;
* migliorare leggibilità dei testi;
* rendere più evidente il focus visivo;
* evitare informazioni affidate solo al colore.

Non sono ammessi:

* redesign completo;
* cambio tema;
* tema scuro;
* animazioni;
* nuove icone definitive;
* nuova struttura radicale della finestra.

L’accessibilità non deve peggiorare l’esperienza per utenti vedenti.

---

## 29. Validazione manuale con NVDA

Il blocco 008 deve prevedere una verifica manuale con NVDA.

Scenari minimi da verificare:

1. apertura app;
2. annuncio corretto del titolo finestra;
3. focus iniziale sul primo controllo utile;
4. navigazione con Tab dall’inizio alla fine;
5. navigazione inversa con Shift+Tab;
6. lettura dei campi durata sessione;
7. lettura dei campi durata avviso finale;
8. modifica valori da tastiera;
9. verifica che `NumericStepControl` non appesantisca il flusso Tab;
10. avvio timer;
11. pausa timer;
12. ripresa timer;
13. reset timer;
14. lettura stato corrente;
15. lettura tempo rimanente senza annuncio ogni secondo;
16. lettura sessioni completate;
17. errore su configurazione non valida;
18. ritorno al campo da correggere;
19. verifica che il focus non sparisca;
20. verifica che i pulsanti cambino nome correttamente;
21. verifica che testo visivo, nome accessibile e stato del pulsante principale siano coerenti;
22. verifica che gli eventi importanti siano percepibili;
23. verifica che gli annunci ordinari non interrompano aggressivamente l’utente;
24. verifica che il countdown non venga annunciato a ogni tick;
25. chiusura app.

Il risultato della verifica deve essere riportato nel report dell’agente di codifica.

---

## 30. Test automatici

Il blocco 008 deve mantenere tutti i test esistenti verdi.

Test obbligatori:

```text
dotnet test
```

Devono continuare a passare:

```text
337 / 337
```

oppure il numero aggiornato di test, se il blocco introduce test nuovi.

Sono consigliati test aggiuntivi sul ViewModel per:

* testo del pulsante principale;
* stato dei comandi;
* messaggio evento dopo Start;
* messaggio evento dopo Pause;
* messaggio evento dopo Resume;
* messaggio evento dopo Reset;
* messaggio errore per configurazione non valida;
* coerenza tra stato timer e testo del pulsante principale;
* aggiornamento proprietà accessibili, se esposte dal ViewModel.

Non è obbligatorio introdurre test UI Automation completi in questo blocco, salvo decisione successiva.

---

## 31. Criteri di accettazione

Il blocco 008 è accettabile se:

* la build resta verde;
* tutti i test automatici passano;
* la UI resta funzionante;
* il timer continua a partire;
* il timer continua a fermarsi in pausa;
* il timer continua a riprendere;
* il timer continua a resettarsi;
* il countdown continua ad aggiornarsi;
* il focus iniziale è prevedibile;
* l’ordine Tab è logico;
* Shift+Tab funziona in modo prevedibile;
* ogni controllo interattivo ha nome accessibile chiaro;
* i controlli numerici sono usabili da tastiera;
* i controlli numerici non appesantiscono inutilmente il flusso Tab;
* i pulsanti hanno nomi coerenti con la loro funzione;
* testo visivo, nome accessibile e stato del pulsante principale restano coerenti;
* il focus non viene perso;
* gli errori sono leggibili e comprensibili;
* lo stato corrente è leggibile;
* il tempo rimanente è leggibile ma non annunciato ogni secondo;
* il contatore sessioni è leggibile;
* gli eventi importanti sono comunicati in modo accessibile;
* gli annunci ordinari non interrompono aggressivamente l’utente;
* il meccanismo di annuncio eventi è verificato con NVDA;
* non sono stati introdotti annunci di funzionalità cicliche future;
* non sono state introdotte nuove funzionalità timer;
* non sono state introdotte scorciatoie nuove;
* non sono state modificate regole del Core;
* non sono state introdotte stringhe utente hardcoded;
* NVDA consente di usare il timer senza mouse.

---

## 32. Cosa può modificare questo documento

Questo documento può autorizzare modifiche a:

```text
views/
view-models/
locales/
tests/
```

Può autorizzare:

* proprietà di accessibilità in XAML;
* TabIndex;
* focus iniziale;
* etichette accessibili;
* miglioramento focus;
* area messaggi evento;
* meccanismo WPF/UI Automation equivalente per annunci accessibili;
* testi accessibili centralizzati;
* piccole proprietà ViewModel necessarie alla UI;
* test ViewModel;
* piccole rifiniture visive legate ad accessibilità.

---

## 33. Cosa non può modificare questo documento

Questo documento non autorizza modifiche a:

* regole Core;
* stati stabili del timer;
* eventi Core;
* comportamento del ciclo;
* durata timer;
* calcolo tempo rimanente;
* contatore sessioni;
* logica audio;
* runner temporale;
* architettura generale;
* struttura delle cartelle;
* formato dei documenti;
* funzionalità future;
* persistenza;
* notifiche Windows;
* installer;
* tema scuro;
* scorciatoie globali;
* scorciatoie locali nuove non approvate;
* comando dedicato per leggere il tempo rimanente;
* integrazione diretta con NVDA.

---

## 34. Rischi principali

### Rischio 1 — Annunci troppo frequenti

Se il tempo rimanente viene annunciato ogni secondo, l’app diventa inutilizzabile.

Mitigazione:

```text
annunciare solo eventi importanti,
non ogni tick del timer
```

---

### Rischio 2 — Focus spostato in modo aggressivo

Se il focus viene spostato per far leggere messaggi, l’utente perde il controllo.

Mitigazione:

```text
non usare il focus come sistema di annuncio ordinario
```

---

### Rischio 3 — Nomi accessibili generici

Se i controlli si chiamano solo “minuti”, “secondi” o “pulsante”, NVDA non dà contesto sufficiente.

Mitigazione:

```text
usare nomi accessibili completi e contestuali
```

---

### Rischio 4 — Stringhe hardcoded

Se i testi accessibili vengono scritti direttamente in XAML o ViewModel, si rompe la regola della localizzazione centralizzata.

Mitigazione:

```text
aggiungere chiavi nel sistema Localization
```

---

### Rischio 5 — Nuova logica timer nella UI

Lavorando sulla UI, l’agente potrebbe introdurre logica impropria nel ViewModel.

Mitigazione:

```text
la UI visualizza e comanda,
non calcola il timer
```

---

### Rischio 6 — Accessibilità solo teorica

Le proprietà XAML potrebbero sembrare corrette ma non funzionare bene con NVDA.

Mitigazione:

```text
verifica manuale obbligatoria con NVDA
```

---

### Rischio 7 — NumericStepControl troppo pesante nel Tab

Se ogni controllo numerico espone troppi elementi interni nel flusso Tab, l’utente è costretto a navigare una sequenza lunga e faticosa.

Mitigazione:

```text
il NumericStepControl deve comportarsi come controllo compatto
e deve essere verificato con NVDA
```

---

### Rischio 8 — Annuncio pulsante disallineato

Se il testo visivo, il nome accessibile e lo stato di abilitazione del pulsante principale cambiano in momenti diversi, NVDA potrebbe leggere informazioni incoerenti.

Mitigazione:

```text
testo visivo, nome accessibile e stato del pulsante principale
devono essere aggiornati in modo coerente
```

---

### Rischio 9 — Annunci di funzionalità future

Se il blocco 008 introduce annunci relativi a nuove sessioni cicliche o sequenze future, il documento esce dal perimetro.

Mitigazione:

```text
annunciare solo eventi già appartenenti alla UI attuale
```

---

## 35. Report richiesto all’agente di codifica

Alla fine dell’implementazione, l’agente deve produrre un report con:

* file modificati;
* file creati;
* testi aggiunti alla localizzazione;
* proprietà accessibili impostate;
* ordine Tab finale;
* comportamento del focus iniziale;
* gestione focus applicata;
* gestione eventi accessibili applicata;
* comportamento Polite/Assertive adottato;
* eventuale fallback WPF/UI Automation adottato;
* gestione errori accessibili applicata;
* comportamento accessibile del `NumericStepControl`;
* test automatici eseguiti;
* risultato build;
* risultato test;
* verifica manuale NVDA;
* eventuali problemi residui;
* conferma che non sono state introdotte nuove funzionalità timer;
* conferma che non sono state introdotte scorciatoie nuove;
* conferma che non sono stati introdotti annunci di funzionalità cicliche future.

Il report deve essere concreto.

Non basta scrivere:

```text
accessibilità migliorata
```

Deve indicare cosa è stato effettivamente verificato.

---

## 36. Decisione dei consiglieri AI

La bozza iniziale del DESIGN 008 è stata sottoposta a revisione dei consiglieri AI.

Esito complessivo:

```text
APPROVABILE CON MODIFICHE MINORI
```

Osservazioni accolte nel documento:

* aggiunta della regola sul focus iniziale;
* chiarimento sul comportamento compatto del `NumericStepControl`;
* chiarimento sugli annunci ordinari non aggressivi, equivalenti a Polite;
* possibilità di comportamento più urgente solo per errori bloccanti;
* obbligo di validazione reale con NVDA del meccanismo di annuncio;
* previsione di fallback WPF/UI Automation nel Coding Plan se la Live Region non viene letta;
* rischio di disallineamento del pulsante principale;
* rimozione degli annunci relativi a nuove sessioni cicliche future.

Osservazioni non accolte come requisiti del blocco 008:

* introduzione di una scorciatoia F5 o Ctrl+T per leggere il tempo rimanente;
* introduzione di soglie automatiche per sopprimere annunci ciclici futuri;
* obbligo tecnico specifico di una particolare interfaccia UI Automation nel Design.

Motivo:

```text
questi punti rischiano di anticipare funzionalità future
o appartengono al Coding Plan, non al Design
```

---

## 37. Decisione finale attesa

Il blocco 008 potrà passare a Coding Plan solo dopo:

* approvazione del project owner;
* aggiornamento dello stato del documento da `DRAFT` ad `APPROVATO`.

Fino ad allora questo documento non autorizza la codifica.

---

# Sintesi operativa

Il DESIGN 008 definisce un intervento mirato:

```text
rendere accessibile la UI WPF esistente
senza cambiare la logica del timer
```

La priorità è l’uso reale con:

```text
Windows
NVDA
tastiera
WPF
UI Automation
```

Il blocco è corretto solo se migliora l’accessibilità senza allargare il progetto.
