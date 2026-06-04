# DESIGN 007 — UI WPF minima

## Metadati

* Tipo documento: Design tecnico
* Codice: 007
* Titolo: UI WPF minima
* Versione: 0.2.0
* Stato: APPROVATO
* Progetto: CicloTimer
* Repository: donato81/CicloTimer
* Data: 2026-06-04
* Documenti collegati:

  * docs/0-architecture/document-standards.md
  * docs/1-design/001-DESIGN_core-timer-engine.md
  * docs/1-design/002-DESIGN_bridge-ui-logica-timer.md
  * docs/1-design/003-DESIGN_sistema-testi-centralizzati.md
  * docs/1-design/004-DESIGN_audio-service-e-audio-focus.md
  * docs/1-design/005-DESIGN_orchestratore-applicativo-timer.md
  * docs/1-design/006-DESIGN_gestore-timer-reale_v0.1.0.md

---

## 1. Scopo del documento

Questo documento definisce il design del blocco 007, dedicato alla prima interfaccia grafica WPF utilizzabile dell'applicazione CicloTimer.

I blocchi precedenti hanno già introdotto:

* il motore logico del timer;
* il bridge tra logica e modello pronto per la UI;
* il sistema di testi centralizzati;
* il servizio audio;
* l'orchestratore applicativo;
* il gestore del tempo reale.

Questi componenti permettono all'applicazione di funzionare a livello logico e applicativo, ma non esiste ancora una finestra WPF che permetta all'utente di usare realmente il timer.

Il blocco 007 introduce quindi la **UI WPF minima**, cioè la prima finestra grafica che consente all'utente di configurare, avviare, mettere in pausa, riprendere, resettare e osservare il timer.

---

## 2. Stato precedente

Alla fine del blocco 006 il progetto dispone di:

* Core timer engine;
* Bridge UI-logica;
* Localization;
* AudioService;
* TimerAppOrchestrator;
* RealtimeTimerRunner.

Il sistema può già:

* ricevere una configurazione timer;
* produrre modelli display tramite il Bridge;
* coordinare logica e audio tramite l'orchestratore;
* produrre tick reali tramite il runner;
* gestire Start, Pause, Resume, Reset e Tick a livello applicativo.

Manca ancora:

* una finestra WPF reale;
* un ViewModel che collega UI e servizi applicativi;
* un layout grafico minimo;
* controlli utente per configurazione e comandi;
* visualizzazione del tempo rimanente;
* visualizzazione dello stato;
* visualizzazione delle sessioni completate;
* scelta da parte dell'utente se il timer deve essere ciclico oppure singolo.

---

## 3. Problema da risolvere

Attualmente l'applicazione è funzionante sul piano interno, ma non è ancora utilizzabile da un utente finale.

L'utente non ha ancora un'interfaccia per:

* scegliere durata del ciclo;
* scegliere durata dell'avviso finale;
* scegliere se il timer deve ripetersi automaticamente;
* avviare il timer;
* mettere in pausa il timer;
* riprendere il timer;
* resettare il timer;
* vedere il tempo rimanente;
* vedere lo stato;
* vedere quante sessioni sono state completate.

Il blocco 007 deve colmare questo vuoto, introducendo una UI WPF minima ma ordinata.

---

## 4. Obiettivo del blocco 007

L'obiettivo è creare la prima UI WPF funzionante dell'applicazione.

La UI deve:

* essere ospitata nel progetto UI già previsto nella cartella `views/`;
* usare una finestra compatta;
* usare un layout a card;
* usare un tema chiaro pulito;
* permettere configurazione guidata della durata;
* evitare TextBox libere per i tempi;
* permettere la scelta della durata dell'avviso finale;
* permettere la scelta timer ciclico sì/no, se già supportata dai contratti esistenti;
* mostrare il countdown in modo grande e centrale;
* mostrare lo stato del timer;
* mostrare il numero di sessioni completate;
* esporre un pulsante principale dinamico;
* esporre un pulsante Reset separato;
* collegarsi a `ITimerAppOrchestrator`;
* collegarsi a `IRealtimeTimerRunner`;
* rispettare il modello architetturale già definito.

---

## 5. Non obiettivi

Questo blocco non deve introdurre:

* accessibilità avanzata;
* gestione NVDA completa;
* Live Region;
* annunci vocali;
* scorciatoie tastiera avanzate;
* tema scuro;
* selezione tema;
* salvataggio preferenze;
* configurazione persistente;
* notifiche Windows;
* minimizzazione in tray;
* packaging;
* installer;
* icone definitive;
* animazioni avanzate;
* personalizzazione estetica;
* logging strutturato;
* nuove regole del Core;
* nuove regole del Bridge;
* nuove regole audio;
* nuove regole del runner;
* nuove funzionalità timer non già previste dai contratti esistenti.

L'accessibilità avanzata sarà affrontata nel blocco 008.

Il blocco 007 deve limitarsi a rendere l'applicazione usabile tramite una prima UI WPF minimale.

---

## 6. Posizione architetturale

La UI WPF si colloca sopra l'orchestratore applicativo e sopra il runner temporale.

Flusso previsto dei comandi utente:

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

Flusso previsto del tempo reale:

```text
IRealtimeTimerRunner
   ↓
ITimerAppOrchestrator.Tick(...)
   ↓
Bridge
   ↓
Core
   ↓
Display model aggiornato
   ↓
ViewModel
   ↓
View WPF
```

La UI non deve parlare direttamente con:

* Core;
* Bridge;
* AudioService;
* TimerEngine;
* TimerBridgeAdapter;
* SystemActionDispatcher.

La UI deve usare i contratti applicativi già esistenti.

---

## 7. Posizione nel repository

Il progetto UI deve essere collocato nella cartella già presente:

```text
views/
```

La cartella `views/` è la sede della UI WPF.

Il ViewModel deve essere collocato nella cartella già prevista:

```text
view-models/
```

Il blocco 007 può modificare o creare file solo nelle aree necessarie alla UI:

```text
views/
view-models/
tests/
```

Eventuali riferimenti ai progetti applicativi esistenti devono essere aggiunti solo se necessari per collegare UI, orchestratore e runner.

Il blocco 007 non deve spostare file dei blocchi precedenti.

---

## 8. Livelli coinvolti

Il blocco 007 coinvolge:

* View WPF;
* ViewModel;
* collegamento applicativo verso orchestratore;
* collegamento applicativo verso runner;
* testi utente già disponibili tramite localization.

Il blocco 007 non coinvolge direttamente:

* Core;
* Bridge;
* Audio;
* Timing interno;
* logica di calcolo del timer;
* logica audio;
* logica di produzione tick.

---

## 9. Regola fondamentale sulla UI

La UI non deve contenere logica timer.

La UI non deve calcolare:

* stato del timer;
* tempo rimanente;
* sessioni completate;
* transizioni tra stati;
* ingresso in avviso finale;
* completamento sessione;
* comportamento audio.

La UI deve solo:

* raccogliere input utente;
* inviare comandi al ViewModel;
* visualizzare dati già preparati;
* aggiornarsi quando il ViewModel espone nuovi valori.

---

## 10. ViewModel

Il blocco 007 deve introdurre un ViewModel dedicato alla finestra principale.

Nome concettuale:

```text
MainTimerViewModel
```

Il nome definitivo potrà essere scelto nel coding plan, ma il ruolo deve restare chiaro.

Il ViewModel deve:

* ricevere o creare i servizi applicativi necessari;
* esporre proprietà bindabili per la View;
* esporre comandi per la View;
* convertire input UI guidati in configurazione applicativa;
* esporre lo stato di validazione della configurazione;
* chiamare `ITimerAppOrchestrator`;
* chiamare `IRealtimeTimerRunner`;
* aggiornare la UI con il modello restituito dall'orchestratore.

Il ViewModel non deve:

* implementare logica Core;
* duplicare il Bridge;
* generare testi hardcoded fuori dal sistema già esistente;
* controllare direttamente l'audio;
* calcolare il tempo reale;
* usare timer propri.

---

## 11. Deroga MVVM controllata

Nei blocchi precedenti era corretto vietare elementi UI come `ICommand` e `INotifyPropertyChanged`, perché quei blocchi non appartenevano al livello di presentazione.

Nel blocco 007, invece, il progetto entra esplicitamente nel livello WPF/MVVM.

Per questo motivo è autorizzato l'uso controllato di:

```text
INotifyPropertyChanged
ICommand
RelayCommand o equivalente minimale
```

Questa deroga è valida solo per:

```text
views/
view-models/
test del ViewModel
```

Questi elementi non devono entrare in:

```text
models/CicloTimer.Core/
services/CicloTimer.App/
services/CicloTimer.Audio/
services/CicloTimer.App/Timing/
services/CicloTimer.Localization/
```

Motivazione:

* WPF richiede notifiche di cambio proprietà per aggiornare correttamente la UI;
* WPF usa comandi per il binding delle azioni utente;
* senza `INotifyPropertyChanged` e `ICommand`, il progetto rischierebbe di spostare logica nel code-behind, violando MVVM.

---

## 12. Collegamento con orchestratore e runner

Il ViewModel deve coordinare due componenti principali:

```text
ITimerAppOrchestrator
IRealtimeTimerRunner
```

Regola generale:

```text
La UI comanda l'orchestratore.
Il runner produce il tempo.
L'orchestratore produce lo stato applicativo.
```

Esempio logico:

```text
Avvia:
1. ViewModel costruisce la configurazione.
2. ViewModel chiama orchestrator.Configure(...).
3. ViewModel chiama orchestrator.Start().
4. ViewModel chiama runner.Start().
5. ViewModel aggiorna le proprietà visuali.

Pausa:
1. ViewModel chiama orchestrator.Pause().
2. ViewModel chiama runner.Stop().
3. ViewModel aggiorna le proprietà visuali.

Riprendi:
1. ViewModel chiama orchestrator.Resume().
2. ViewModel chiama runner.Start().
3. ViewModel aggiorna le proprietà visuali.

Reset:
1. ViewModel chiama runner.Stop().
2. ViewModel chiama orchestrator.Reset().
3. ViewModel aggiorna le proprietà visuali.
```

Il coding plan dovrà verificare se l'ordine preciso deve essere affinato in base ai contratti reali di `TimerAppOrchestrator`.

---

## 13. Aggiornamento UI dopo tick

Il runner non aggiorna direttamente la UI.

Il blocco 006 ha già stabilito che il runner lavora fuori dal thread grafico WPF.

Quindi il blocco 007 deve prevedere che gli aggiornamenti UI siano portati sul thread corretto.

Regola:

```text
Ogni aggiornamento di proprietà bindate dalla UI deve avvenire sul thread WPF corretto.
```

Il marshalling verso la UI deve essere asincrono.

Sono ammesse soluzioni come:

```text
Dispatcher.BeginInvoke
SynchronizationContext.Post
adapter minimale equivalente
```

È vietato usare `Dispatcher.Invoke` per gli aggiornamenti ordinari prodotti dal timer.

Motivazione:

* il runner reale non deve restare bloccato in attesa del rendering grafico;
* se la UI rallenta, il tempo reale non deve essere rallentato dalla UI;
* il blocco 006 ha già introdotto un runner capace di compensare drift, e il blocco 007 non deve reintrodurre drift causato da marshalling sincrono.

Il design non impone ancora una classe concreta di dispatcher UI, ma il coding plan dovrà definire come ottenere un marshalling sicuro.

Soluzioni vietate:

* far conoscere il Dispatcher al Core;
* far conoscere il Dispatcher al Bridge;
* far conoscere il Dispatcher all'Audio;
* far conoscere il Dispatcher al RealtimeTimerRunner;
* usare marshalling sincrono ordinario per ogni tick.

---

## 14. Layout generale

La finestra deve usare un layout a card.

Card previste:

```text
Card configurazione
Card timer
Card comandi
Card stato
```

Le card devono essere disposte in modo semplice, verticale e leggibile.

Layout concettuale:

```text
Titolo applicazione

[Card configurazione]
  Durata ciclo
  Minuti
  Secondi
  Durata avviso finale
  Ripeti automaticamente il ciclo

[Card timer]
  Tempo rimanente grande

[Card comandi]
  Pulsante principale
  Reset

[Card stato]
  Stato corrente
  Sessioni completate
```

La struttura deve restare compatta e adatta a una finestra piccola.

---

## 15. Dimensione finestra

La finestra deve essere compatta.

Dimensione indicativa:

```text
larghezza: 420–500 px
altezza: 540–720 px
```

La dimensione precisa sarà definita nel coding plan o durante l'implementazione, purché resti coerente con l'idea di una piccola app timer.

La finestra non deve occupare tutto lo schermo.

---

## 16. Tema visivo

La UI deve usare un tema chiaro pulito.

Caratteristiche richieste:

* sfondo chiaro;
* card leggermente separate dallo sfondo;
* testo leggibile;
* tempo rimanente molto visibile;
* pulsanti chiari;
* spaziatura ordinata;
* nessun sovraccarico grafico.

Il blocco 007 non deve introdurre:

* tema scuro;
* scelta tema;
* animazioni complesse;
* risorse grafiche definitive;
* icone definitive.

---

## 17. Visualizzazione del tempo rimanente

Il tempo rimanente è l'elemento principale della finestra.

Deve essere:

* grande;
* centrale;
* facilmente leggibile;
* aggiornato durante l'esecuzione del timer.

Formato richiesto:

```text
MM:SS
```

Esempio:

```text
24:59
```

Il valore deve provenire dal modello già preparato dai layer applicativi, non da un calcolo diretto della View.

---

## 18. Configurazione durata ciclo

La durata del ciclo deve essere configurabile tramite:

```text
Minuti
Secondi
```

La UI non deve usare TextBox libere per l'inserimento del tempo.

Motivazione:

* ridurre errori di input;
* evitare valori testuali non validi;
* impedire inserimenti come lettere, valori negativi o formati ambigui;
* semplificare il lavoro del ViewModel;
* rendere più prevedibile il comportamento.

Il controllo previsto è un controllo numerico guidato con incremento e decremento.

Esempio concettuale:

```text
Minuti
[-] 25 [+]

Secondi
[-] 00 [+]
```

Il controllo può essere implementato come:

* UserControl WPF dedicato;
* controllo composto nel layout;
* altra soluzione minimale coerente con WPF.

Il coding plan definirà la soluzione concreta.

---

## 19. Regole durata ciclo

Regole richieste:

```text
Minuti: 0 o maggiore
Secondi: 0–59
Durata totale minima: 1 secondo
```

La UI deve impedire o correggere valori non validi.

Esempi non validi:

```text
0 minuti, 0 secondi
-1 minuti
70 secondi
valore non numerico
```

Dato che non vengono usate TextBox libere, i valori non numerici non dovrebbero poter essere inseriti.

Il ViewModel deve comunque validare la configurazione prima di inviarla all'orchestratore.

---

## 20. Configurazione avviso finale

L'avviso finale deve essere configurabile dall'utente.

La UI deve esporre un valore in secondi.

La UI non deve usare TextBox libera per questo valore.

Esempio concettuale:

```text
Avviso finale
[-] 10 [+]
secondi
```

Valore minimo:

```text
0 secondi
```

Il valore `0` significa che non si vuole avviso finale oppure che l'avviso finale è disabilitato, se il Core/Bridge supporta questa semantica.

Se i contratti esistenti non supportano `0`, il coding plan dovrà fermarsi e segnalare il conflitto prima dell'implementazione.

Valore massimo iniziale consigliato:

```text
60 secondi
```

Il coding plan può confermare o correggere questo limite in base ai contratti reali del Core.

---

## 21. Regole avviso finale

L'avviso finale non deve essere maggiore della durata totale del ciclo.

Esempio non valido:

```text
Durata ciclo: 30 secondi
Avviso finale: 60 secondi
```

La UI deve impedire o segnalare questa configurazione.

La validazione deve avvenire prima di chiamare `orchestrator.Configure(...)`.

La UI non deve affidarsi soltanto al Core per scoprire l'errore.

Il Core resta comunque l'autorità finale sulla validità della configurazione.

---

## 22. Scelta timer ciclico

La UI deve permettere all'utente di scegliere se il timer deve essere ciclico oppure singolo.

Controllo previsto:

```text
CheckBox
```

Etichetta concettuale:

```text
Ripeti automaticamente il ciclo
```

Valori:

```text
selezionato   → timer ciclico
non selezionato → timer singolo
```

Default consigliato:

```text
non selezionato
```

Motivazione:

* è più sicuro;
* evita ripetizioni automatiche non volute;
* l'utente attiva esplicitamente il comportamento ciclico solo quando lo desidera.

Questa scelta deve essere esposta solo se già supportata dai contratti esistenti del Core, Bridge e Orchestratore.

Se durante il coding plan emerge che il supporto ciclico non è disponibile nei contratti reali, il coding plan deve fermarsi e segnalare il conflitto.

Il blocco 007 non autorizza l'aggiunta di nuova logica Core per introdurre la ciclicità.

---

## 23. Regole timer ciclico

Il ViewModel deve esporre una proprietà concettuale:

```text
IsCyclic
```

La proprietà deve essere bindata alla CheckBox.

Quando l'utente avvia il timer, il ViewModel deve passare il valore ciclico alla configurazione applicativa, se il contratto esistente lo prevede.

Il ViewModel non deve implementare manualmente il ciclo.

La ripetizione del timer, se prevista, deve restare responsabilità dei layer già esistenti.

Regola:

```text
La UI sceglie se il timer è ciclico.
La logica del ciclo resta nel Core/Bridge/Orchestratore.
```

---

## 24. Controllo numerico guidato

Il blocco 007 deve introdurre o usare un controllo numerico guidato.

Responsabilità del controllo:

* mostrare un valore numerico;
* permettere incremento;
* permettere decremento;
* rispettare minimo;
* rispettare massimo;
* impedire valori fuori range.

Il controllo non deve:

* conoscere il timer;
* conoscere l'orchestratore;
* conoscere il runner;
* produrre comandi applicativi;
* contenere logica di business.

Il controllo può essere riutilizzato per:

* minuti;
* secondi;
* avviso finale.

Nome concettuale:

```text
NumericStepControl
```

Il nome definitivo sarà stabilito nel coding plan.

---

## 25. Comportamento del NumericStepControl

Il `NumericStepControl` deve usare clamping rigido.

Esempio:

```text
Secondi = 59
utente preme [+]
Secondi resta 59
```

Non deve avvenire overflow automatico.

Esempio vietato:

```text
Secondi = 59
utente preme [+]
Minuti aumenta di 1
Secondi torna a 0
```

Motivazione:

* il controllo numerico resta generico;
* il controllo non conosce altre proprietà;
* la logica complessiva resta nel ViewModel;
* si evita accoppiamento tra controllo secondi e controllo minuti.

Il ViewModel resta responsabile della validazione complessiva.

---

## 26. Stato di validazione configurazione

Il ViewModel deve esporre uno stato di validazione della configurazione.

Proprietà concettuali:

```text
IsConfigurationValid
ValidationErrorText
```

Regole:

* `IsConfigurationValid` è `true` quando la configurazione è avviabile;
* `IsConfigurationValid` è `false` quando la configurazione non è avviabile;
* `ValidationErrorText` contiene un messaggio utente localizzato quando esiste un errore;
* `ValidationErrorText` può essere vuoto quando non esiste errore.

Uso previsto:

* abilitare o disabilitare il comando Avvia;
* mostrare una card o riga errore nella UI;
* impedire la partenza del runner con configurazione non valida.

La UI non deve avviare il timer se `IsConfigurationValid` è `false`.

I messaggi di validazione non devono essere hardcoded.

---

## 27. Pulsante principale dinamico

La UI deve avere un pulsante principale dinamico.

Il testo e il comando del pulsante cambiano in base allo stato corrente.

Mappatura concettuale:

```text
Timer fermo/configurabile → Avvia
Timer in esecuzione → Pausa
Timer in pausa → Riprendi
Timer completato → Avvia
```

Nello stato `Completato`, il pulsante principale deve tornare a mostrare `Avvia`, così l'utente può far ripartire un nuovo ciclo con la stessa configurazione.

Il ViewModel deve determinare il testo e l'azione del pulsante usando lo stato applicativo disponibile.

La View deve solo fare binding al testo e al comando esposti dal ViewModel.

---

## 28. Pulsante Reset

La UI deve avere un pulsante Reset separato.

Il pulsante Reset deve:

* fermare il runner se necessario;
* chiamare il reset applicativo;
* aggiornare la UI.

Il pulsante Reset non deve:

* ricreare direttamente il Core;
* cancellare manualmente dati interni;
* modificare stato non autorizzato.

Il comportamento preciso sarà definito nel coding plan in base ai contratti reali dell'orchestratore.

---

## 29. Stato timer

La UI deve mostrare lo stato corrente del timer.

Lo stato deve essere una stringa già pronta per la visualizzazione oppure derivata dal modello display/localization esistente.

Valori concettuali:

```text
Fermo
In esecuzione
In pausa
Avviso finale
Completato
Errore
```

I valori definitivi non devono essere hardcoded nella View.

Devono provenire dai layer già esistenti o dal sistema di testi centralizzati.

---

## 30. Sessioni completate

La UI deve mostrare il numero di sessioni completate.

Esempio concettuale:

```text
Sessioni completate: 3
```

Il numero deve provenire dal modello applicativo già esistente.

La UI non deve incrementare manualmente il contatore.

---

## 31. Testi utente

Il blocco 007 introduce testi visibili all'utente.

Regola:

```text
Nessuna stringa utente hardcoded nella logica.
```

I testi devono essere gestiti coerentemente con il sistema di localization già introdotto nel blocco 003.

Testi previsti:

* titolo applicazione;
* etichetta durata ciclo;
* etichetta minuti;
* etichetta secondi;
* etichetta avviso finale;
* etichetta ripeti automaticamente il ciclo;
* testo pulsante Avvia;
* testo pulsante Pausa;
* testo pulsante Riprendi;
* testo pulsante Reset;
* etichetta stato;
* etichetta sessioni completate;
* eventuale messaggio di errore configurazione.

Il coding plan dovrà verificare quali chiavi esistono già e quali vanno aggiunte.

---

## 32. Gestione errori

La UI deve gestire errori applicativi in modo minimale.

Tipi di errore possibili:

* configurazione non valida;
* comando non ammesso nello stato corrente;
* errore tecnico restituito dall'orchestratore.

La UI deve mostrare un messaggio semplice e comprensibile.

I messaggi non devono essere hardcoded.

La UI non deve mostrare stack trace.

La UI non deve interpretare direttamente eccezioni interne dei layer inferiori.

Gli errori devono provenire dai risultati applicativi già prodotti dall'orchestratore o dal Bridge.

---

## 33. Accessibilità

Il blocco 007 non implementa accessibilità avanzata.

Tuttavia non deve peggiorare l'accessibilità di base.

Requisiti minimi:

* usare controlli WPF standard quando possibile;
* mantenere ordine visuale coerente;
* usare etichette visibili associate ai controlli;
* evitare controlli puramente grafici non leggibili;
* non introdurre animazioni obbligatorie;
* non produrre aggiornamenti rumorosi ogni secondo per screen reader.

Il blocco 008 definirà:

* tab order dettagliato;
* comportamento NVDA;
* annunci;
* Live Region;
* nomi accessibili;
* gestione del focus;
* eventuali scorciatoie tastiera.

---

## 34. Thread UI

Il RealtimeTimerRunner produce tick fuori dal thread UI.

Il ViewModel deve garantire che le proprietà bindate siano aggiornate sul thread corretto.

Il blocco 007 può introdurre un meccanismo minimo di marshalling UI.

Regole:

* il Dispatcher WPF può essere usato solo nel livello UI/ViewModel;
* il Dispatcher non deve entrare in Core, Bridge, Audio, Orchestrator o Runner;
* gli aggiornamenti verso la View devono essere sicuri per WPF;
* il marshalling deve essere asincrono;
* il runner non deve restare bloccato dalla UI.

Soluzioni ammesse:

```text
Dispatcher.BeginInvoke
SynchronizationContext.Post
adapter minimale equivalente
```

Soluzione vietata per aggiornamenti ordinari del timer:

```text
Dispatcher.Invoke
```

La scelta concreta deve essere documentata nel coding plan.

---

## 35. Dipendenze consentite

La UI può dipendere da:

```text
CicloTimer.App
CicloTimer.App.Timing
CicloTimer.Localization
WPF
System
```

Il ViewModel può dipendere da:

```text
ITimerAppOrchestrator
IRealtimeTimerRunner
modelli display già esistenti
localization service già esistente
INotifyPropertyChanged
ICommand
RelayCommand o equivalente minimale
tipi WPF minimi solo se necessari per marshalling/comandi
```

La UI e il ViewModel non devono dipendere direttamente da:

```text
CicloTimer.Core
CicloTimer.Bridge
CicloTimer.Audio
TimerEngine
TimerBridgeAdapter
SystemActionDispatcher
implementazioni interne non necessarie
```

Se alcune dipendenze sono già transitive o inevitabili per il progetto, il coding plan dovrà documentarle e mantenerle entro il minimo necessario.

---

## 36. File e cartelle coinvolte

Il design autorizza modifiche o creazioni in:

```text
views/
view-models/
tests/
```

Il design può autorizzare aggiornamenti minimi a:

```text
locales/
```

solo per aggiungere chiavi di testo necessarie alla UI.

Il design può autorizzare aggiornamenti minimi ai file di progetto `.csproj` solo per collegare progetti già esistenti e compilare la UI.

Il design non autorizza modifiche a:

```text
models/CicloTimer.Core/
services/CicloTimer.Audio/
services/CicloTimer.App/Timing/
services/CicloTimer.App/Orchestrator/
```

salvo correzioni tecniche minime emerse come necessarie e approvate prima della codifica.

---

## 37. Componenti previsti

Componenti concettuali previsti:

```text
MainWindow
MainTimerViewModel
NumericStepControl
RelayCommand
eventuale UiDispatcherAdapter
```

Questi nomi sono indicativi.

Il coding plan potrà confermare o adattare i nomi in base alla struttura reale del progetto.

Responsabilità:

```text
MainWindow
→ definisce layout WPF e binding.

MainTimerViewModel
→ espone stato, comandi e proprietà per la UI.

NumericStepControl
→ espone un selettore numerico guidato riutilizzabile.

RelayCommand
→ abilita binding dei comandi WPF.

UiDispatcherAdapter
→ se introdotto, isola il marshalling UI asincrono.
```

---

## 38. Comandi UI

Il ViewModel deve esporre almeno:

```text
PrimaryCommand
ResetCommand
```

Il `PrimaryCommand` deve rappresentare l'azione dinamica:

```text
Avvia / Pausa / Riprendi / Avvia dopo completamento
```

Il `ResetCommand` deve rappresentare il reset.

Il blocco 007 può introdurre un'implementazione minimale di comando WPF, ad esempio un `RelayCommand`, se non esiste già nel repository.

Se `RelayCommand` o equivalente esiste già, il coding plan dovrà riutilizzarlo.

---

## 39. Stato del pulsante principale

Il ViewModel deve esporre almeno:

```text
PrimaryButtonText
CanExecutePrimaryCommand
CanExecuteResetCommand
```

Il testo del pulsante principale deve essere coerente con lo stato corrente.

Esempio:

```text
Fermo → Avvia
In esecuzione → Pausa
In pausa → Riprendi
Completato → Avvia
```

Il pulsante non deve eseguire azioni non permesse dallo stato corrente.

In stato iniziale o completato, il comando Avvia deve essere eseguibile solo se `IsConfigurationValid` è `true`.

---

## 40. Aggiornamento display

Il ViewModel deve esporre almeno:

```text
RemainingTimeText
TimerStateText
CompletedSessionsText
```

Questi valori devono essere derivati dal modello applicativo o dalla localization esistente.

La View non deve formattare direttamente dati grezzi.

La View deve solo visualizzare le proprietà bindate.

---

## 41. Input configurazione

Il ViewModel deve esporre proprietà per:

```text
CycleMinutes
CycleSeconds
FinalAlertSeconds
IsCyclic
IsConfigurationValid
ValidationErrorText
```

Tali proprietà devono essere usate dai controlli guidati e dalla CheckBox.

Regole:

```text
CycleMinutes >= 0
CycleSeconds tra 0 e 59
FinalAlertSeconds >= 0
FinalAlertSeconds <= durata totale ciclo
durata totale ciclo >= 1 secondo
IsCyclic boolean
```

Il ViewModel deve impedire o correggere valori non validi.

---

## 42. Comportamento iniziale

All'avvio della finestra:

* il timer deve essere fermo;
* devono essere mostrati valori predefiniti ragionevoli;
* il pulsante principale deve mostrare `Avvia`;
* il pulsante Reset può essere disabilitato o abilitato in modo coerente con lo stato iniziale;
* il tempo rimanente deve riflettere la configurazione iniziale;
* la CheckBox del timer ciclico deve essere non selezionata;
* la configurazione deve essere valida.

Valori predefiniti proposti:

```text
Durata ciclo: 25 minuti, 0 secondi
Avviso finale: 10 secondi
Timer ciclico: no
```

Questi valori possono essere confermati o modificati nel coding plan se i documenti precedenti indicano default diversi.

---

## 43. Comportamento Avvia

Quando l'utente attiva il pulsante principale in stato iniziale o completato:

1. il ViewModel valida la configurazione;
2. se la configurazione non è valida, il comando non deve proseguire;
3. il ViewModel costruisce la configurazione applicativa includendo durata, avviso finale e flag ciclico, se supportato dal contratto;
4. il ViewModel invia la configurazione all'orchestratore;
5. il ViewModel chiama `Start()` sull'orchestratore;
6. il ViewModel avvia il runner;
7. la UI si aggiorna allo stato in esecuzione;
8. il pulsante principale diventa `Pausa`.

Se la configurazione non è valida:

* il runner non deve partire;
* l'orchestratore non deve entrare in stato running;
* la UI deve mostrare errore di configurazione;
* `ValidationErrorText` deve contenere un testo localizzato.

---

## 44. Comportamento Pausa

Quando l'utente attiva il pulsante principale in stato di esecuzione:

1. il ViewModel chiama `Pause()` sull'orchestratore;
2. il ViewModel ferma il runner;
3. la UI si aggiorna allo stato pausa;
4. il pulsante principale diventa `Riprendi`.

La pausa non deve resettare il tempo.

---

## 45. Comportamento Riprendi

Quando l'utente attiva il pulsante principale in stato pausa:

1. il ViewModel chiama `Resume()` sull'orchestratore;
2. il ViewModel riavvia il runner;
3. la UI si aggiorna allo stato esecuzione;
4. il pulsante principale diventa `Pausa`.

---

## 46. Comportamento Reset

Quando l'utente preme Reset:

1. il ViewModel ferma il runner;
2. il ViewModel chiama `Reset()` sull'orchestratore;
3. la UI torna allo stato iniziale o configurato;
4. il pulsante principale torna a `Avvia`.

Reset non deve modificare i valori di configurazione scelti dall'utente, salvo diversa indicazione emersa nel coding plan.

Reset non deve modificare il valore `IsCyclic` scelto dall'utente.

---

## 47. Comportamento a fine sessione

Quando il timer completa una sessione:

* l'orchestratore aggiorna lo stato;
* il modello display espone il nuovo stato;
* la UI aggiorna tempo, stato e sessioni completate.

Se il timer non è ciclico:

* il runner deve fermarsi o essere fermato in modo coerente con lo stato applicativo;
* il pulsante principale deve tornare a `Avvia`;
* l'utente può avviare un nuovo ciclo con la stessa configurazione.

Se il timer è ciclico:

* la ripetizione deve essere gestita dai layer già esistenti;
* la UI deve visualizzare lo stato aggiornato;
* la UI non deve implementare manualmente la ripetizione.

Il blocco 007 non deve inventare nuove regole di ripetizione, loop o completamento.

---

## 48. Relazione con audio

La UI non deve controllare direttamente il servizio audio.

L'audio resta coordinato dall'orchestratore e dai layer già implementati.

La UI può mostrare stato o errore applicativo collegato all'audio solo se tale informazione è già esposta dai risultati applicativi.

La UI non deve chiamare `AudioService`.

---

## 49. Relazione con localization

Il blocco 007 deve usare il sistema di testi centralizzati.

La UI non deve introdurre stringhe utente hardcoded nel codice C#.

Nel file XAML possono comparire testi provvisori solo se il coding plan stabilisce un percorso chiaro per sostituirli con binding/localization, ma la soluzione finale del blocco non deve lasciare testi utente non centralizzati.

Il coding plan dovrà definire se i testi vengono forniti:

* dal ViewModel;
* da binding a localization service;
* da risorse WPF generate a partire dalla localization esistente;
* da altra soluzione coerente con il blocco 003.

---

## 50. Test previsti

Il blocco 007 deve prevedere test per:

* ViewModel iniziale;
* valori predefiniti;
* validazione durata ciclo;
* validazione avviso finale;
* validazione avviso finale maggiore della durata ciclo;
* validazione dello stato ciclico;
* pulsante principale in stato iniziale;
* pulsante principale in stato running;
* pulsante principale in stato paused;
* pulsante principale in stato completed;
* Reset;
* aggiornamento display dopo comando;
* mancato avvio con configurazione non valida;
* `IsConfigurationValid`;
* `ValidationErrorText`;
* `IsCyclic` passato alla configurazione, se supportato dal contratto;
* nessuna chiamata diretta a Core;
* nessuna chiamata diretta ad AudioService;
* nessuna chiamata diretta a Bridge;
* nessuna logica timer duplicata nel ViewModel;
* testi utente non hardcoded nel C#;
* uso autorizzato di `INotifyPropertyChanged` e `ICommand` solo in `view-models/`;
* marshalling UI asincrono se introdotto.

I test UI visuali completi non sono obbligatori in questo blocco, salvo strumenti già presenti.

Il test principale deve essere sul ViewModel.

---

## 51. Verifiche architetturali

Il coding plan e il todo dovranno prevedere controlli per verificare che `views/` e `view-models/` rispettino i confini del design.

Controlli consigliati:

```text
nessun riferimento diretto a TimerEngine
nessun riferimento diretto a AudioService
nessun riferimento diretto a TimerBridgeAdapter
nessun riferimento diretto a SystemActionDispatcher
nessuna nuova logica di calcolo timer
nessun Dispatcher fuori dal livello UI/ViewModel
nessun Dispatcher.Invoke per aggiornamenti ordinari del timer
nessuna modifica non autorizzata ai blocchi 001–006
INotifyPropertyChanged solo nel livello presentazione
ICommand solo nel livello presentazione
```

---

## 52. Rischi principali

### Rischio 1 — Logica timer nella UI

La UI potrebbe iniziare a calcolare tempo rimanente o transizioni di stato.

Mitigazione:

```text
La UI visualizza solo dati esposti dal ViewModel e dai modelli applicativi.
```

---

### Rischio 2 — ViewModel troppo intelligente

Il ViewModel potrebbe duplicare logica del Bridge o dell'orchestratore.

Mitigazione:

```text
Il ViewModel coordina comandi e binding, ma non interpreta internamente la logica timer.
```

---

### Rischio 3 — Dispatcher nei layer sbagliati

La necessità di aggiornare la UI sul thread WPF potrebbe contaminare i layer applicativi.

Mitigazione:

```text
Il Dispatcher può restare solo nel livello UI/ViewModel o in un adapter UI dedicato.
```

---

### Rischio 4 — Marshalling sincrono che rallenta il runner

La UI potrebbe usare `Dispatcher.Invoke` e bloccare il thread del runner.

Mitigazione:

```text
Usare marshalling asincrono tramite Dispatcher.BeginInvoke, SynchronizationContext.Post o equivalente.
```

---

### Rischio 5 — Accessibilità anticipata male

Il blocco 007 potrebbe introdurre una accessibilità incompleta e confusa.

Mitigazione:

```text
Il blocco 007 mantiene requisiti minimi e rinvia accessibilità avanzata al blocco 008.
```

---

### Rischio 6 — Input testuale libero

Caselle di testo libere per il tempo potrebbero generare errori evitabili.

Mitigazione:

```text
Usare controlli numerici guidati con incremento e decremento.
```

---

### Rischio 7 — UI gradevole ma troppo grande

Si potrebbe perdere tempo in estetica non necessaria.

Mitigazione:

```text
Tema chiaro pulito, card semplici, nessuna rifinitura avanzata.
```

---

### Rischio 8 — Funzione ciclica non supportata dai contratti reali

La UI potrebbe provare a esporre il flag ciclico anche se i contratti esistenti non lo supportano.

Mitigazione:

```text
Il coding plan deve verificare i contratti reali.
Se il flag non è supportato, fermarsi e segnalare il conflitto.
Non modificare il Core nel blocco 007 per aggiungere la ciclicità.
```

---

## 53. Fuori perimetro esplicito

Questo design non autorizza:

```text
accessibilità avanzata
NVDA completo
Live Region
annunci vocali
shortcut avanzate
tema scuro
salvataggio preferenze
packaging
installer
minimizzazione tray
notifiche Windows
icone definitive
animazioni avanzate
nuove funzionalità timer
nuove regole Core
nuove regole audio
nuove regole runner
```

Ogni richiesta di questo tipo deve essere rinviata a design successivi.

---

## 54. Criteri di accettazione

Il blocco 007 è accettato quando:

* esiste una finestra WPF avviabile;
* la UI risiede nella cartella `views/`;
* il ViewModel risiede nella cartella `view-models/`;
* la finestra usa layout a card;
* la finestra è compatta;
* il tema è chiaro e pulito;
* la durata ciclo è configurabile tramite controlli guidati;
* l'avviso finale è configurabile tramite controlli guidati;
* il timer ciclico è configurabile tramite CheckBox se supportato dai contratti esistenti;
* non sono usate TextBox libere per i tempi;
* il `NumericStepControl` usa clamping rigido;
* il tempo rimanente è mostrato grande al centro;
* il pulsante principale è dinamico;
* in stato `Completato` il pulsante principale torna a `Avvia`;
* il pulsante Reset è separato;
* lo stato del timer è visibile;
* le sessioni completate sono visibili;
* esistono `IsConfigurationValid` e `ValidationErrorText` o equivalenti;
* Start funziona;
* Pause funziona;
* Resume funziona;
* Reset funziona;
* il runner viene avviato e fermato in modo coerente;
* il marshalling UI è asincrono se necessario;
* la UI non contiene logica Core;
* la UI non controlla direttamente l'audio;
* la UI non calcola direttamente il tempo reale;
* i testi utente sono gestiti tramite localization o percorso coerente con il blocco 003;
* `INotifyPropertyChanged` e `ICommand` restano confinati al layer di presentazione;
* accessibilità avanzata non viene anticipata;
* i test previsti passano;
* i test 001–006 restano verdi.

---

## 55. Decisioni finali del Design 007

Le decisioni per questa versione sono:

1. La UI vive nella cartella `views/`.
2. I ViewModel vivono nella cartella `view-models/`.
3. Il layout è a card.
4. Il tema è chiaro e pulito.
5. La finestra è compatta.
6. Il tempo rimanente è grande e centrale.
7. La durata ciclo usa minuti e secondi.
8. La durata ciclo non usa TextBox libere.
9. L'avviso finale è configurabile.
10. L'avviso finale non usa TextBox libera.
11. Il timer ciclico è configurabile tramite CheckBox, se supportato dai contratti esistenti.
12. Il default del timer ciclico è non selezionato.
13. Si usa un controllo numerico guidato.
14. Il controllo numerico guidato usa clamping rigido.
15. Il pulsante principale è dinamico.
16. In stato `Completato`, il pulsante principale torna a `Avvia`.
17. Il pulsante Reset è separato.
18. Le sessioni completate sono visibili.
19. La UI usa orchestratore e runner.
20. La UI non parla direttamente con Core, Bridge o Audio.
21. `INotifyPropertyChanged`, `ICommand` e `RelayCommand` sono autorizzati solo nel livello presentazione.
22. Il Dispatcher, se necessario, resta nel livello UI/ViewModel.
23. Il marshalling UI deve essere asincrono.
24. `Dispatcher.Invoke` è vietato per aggiornamenti ordinari del timer.
25. Lo stato di validazione deve essere esposto dal ViewModel.
26. L'accessibilità avanzata è rinviata al blocco 008.
27. Packaging, tema scuro e preferenze sono fuori perimetro.
28. Il blocco 007 non modifica Core, Bridge, Audio, Orchestrator o Runner per aggiungere funzionalità non presenti.

---

## 56. Cronologia versioni

```text
0.1.0 — Prima bozza del Design 007 per revisione del Consiglio AI.
0.2.0 — Integra correzioni Consiglio AI: deroga MVVM, marshalling asincrono, validazione ViewModel, clamping NumericStepControl, stato Completato, timer ciclico configurabile.
```

---

## 57. Esito atteso

Alla fine del blocco 007 CicloTimer avrà una prima interfaccia WPF reale, minima ma utilizzabile.

L'utente potrà configurare e usare il timer da una finestra compatta e pulita.

L'utente potrà scegliere:

* durata ciclo;
* durata avviso finale;
* timer singolo o ciclico, se supportato dai contratti esistenti.

Il blocco successivo naturale sarà:

```text
008 — Accessibilità UI e rifinitura interazione
```

Il blocco 008 si occuperà di rendere l'esperienza pienamente accessibile e rifinita, senza riaprire la logica timer già definita.
