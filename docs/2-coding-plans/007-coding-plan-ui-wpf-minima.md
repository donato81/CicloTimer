# CicloTimer — Coding Plan 007 — UI WPF minima

**Tipo documento:** coding plan
**Stato:** APPROVATO
**Versione:** 0.2.0
**Data:** 2026-06-04
**Repository:** donato81/CicloTimer
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/1-design/004-design-audio-service-avviso-finale.md, docs/1-design/005-design-orchestratore-applicativo-timer.md, docs/1-design/006-DESIGN_gestore-timer-reale_v0.1.0.md, docs/1-design/007-DESIGN_ui-wpf-minima.md

---

## 1. Scopo del documento

Questo documento traduce il Design 007 approvato in un piano operativo di codifica.

Il documento di riferimento principale è:

```text
docs/1-design/007-DESIGN_ui-wpf-minima.md
```

Il Design 007 stabilisce che il blocco deve introdurre la prima UI WPF minima e realmente utilizzabile dell'applicazione CicloTimer.

Questo coding plan definisce:

1. quali cartelle verificare;
2. quali file creare;
3. quali file modificare;
4. quali componenti UI introdurre;
5. quale ViewModel introdurre;
6. come collegare View, ViewModel, orchestratore e runner;
7. come gestire input guidati per durata ciclo e avviso finale;
8. come gestire la scelta timer ciclico;
9. come esporre comandi WPF;
10. come aggiornare la UI dal runner reale;
11. come integrare i testi utente con localization;
12. quali dipendenze sono autorizzate;
13. quali dipendenze sono vietate;
14. quali test automatici creare;
15. quali controlli architetturali eseguire;
16. quali verifiche finali eseguire.

Questo coding plan non cambia il Design 007.

Questo coding plan non introduce accessibilità avanzata.

Questo coding plan non introduce packaging.

Questo coding plan non introduce salvataggio preferenze.

Questo coding plan non introduce tema scuro.

Questo coding plan non modifica la logica Core, Bridge, Audio, Orchestrator o Runner.

Se durante la codifica emerge una necessità non prevista dal Design 007, Cursor deve fermarsi e segnalarla.

---

## 2. Obiettivo operativo

L'obiettivo operativo è creare una prima interfaccia WPF funzionante, minima ma gradevole, collocata nella cartella:

```text
views/
```

e collegata a un ViewModel collocato nella cartella:

```text
view-models/
```

La UI deve permettere all'utente di:

1. impostare durata ciclo in minuti e secondi;
2. impostare durata avviso finale in secondi;
3. scegliere se il timer deve ripetersi automaticamente, solo se il supporto ciclico è già presente nei contratti esistenti;
4. avviare il timer;
5. mettere in pausa il timer;
6. riprendere il timer;
7. resettare il timer;
8. vedere il tempo rimanente;
9. vedere lo stato corrente;
10. vedere il numero di sessioni completate.

La UI deve usare:

1. layout a card;
2. tema chiaro pulito;
3. finestra compatta;
4. tempo rimanente grande e centrale;
5. controlli numerici guidati;
6. CheckBox per timer ciclico solo se supportata dai contratti reali;
7. pulsante principale dinamico;
8. pulsante Reset separato.

---

## 3. Perimetro autorizzato

Questo coding plan autorizza:

1. verifica della struttura esistente in `views/`;
2. verifica della struttura esistente in `view-models/`;
3. creazione o modifica della finestra WPF principale in `views/`;
4. creazione di un ViewModel principale in `view-models/`;
5. creazione di un controllo numerico guidato riutilizzabile, se necessario;
6. creazione di `RelayCommand` o equivalente minimale, se non già esistente;
7. creazione di un adapter minimale per marshalling UI asincrono, se necessario;
8. aggiunta dei riferimenti necessari verso `CicloTimer.App`;
9. aggiunta dei riferimenti necessari verso `CicloTimer.App.Timing`;
10. aggiunta dei riferimenti necessari verso `CicloTimer.Localization`;
11. aggiunta delle chiavi di testo strettamente necessarie alla UI minima;
12. creazione o aggiornamento dei test del ViewModel;
13. aggiornamento minimo dei file `.csproj` necessari alla compilazione;
14. aggiornamento minimo della solution solo se manca il progetto da compilare;
15. esecuzione di build e test;
16. gestione pulita della chiusura della finestra tramite stop/dispose controllato del ViewModel o del runner.

---

## 4. Fuori perimetro

Questo coding plan non autorizza:

1. modifica della logica Core;
2. modifica del Bridge;
3. modifica del servizio Audio;
4. modifica dell'orchestratore applicativo;
5. modifica del runner reale;
6. modifica dei test dei blocchi 001–006 salvo necessità tecnica documentata;
7. introduzione di nuove funzionalità timer;
8. introduzione di accessibilità avanzata;
9. introduzione di supporto NVDA completo;
10. introduzione di Live Region;
11. introduzione di annunci vocali;
12. introduzione di scorciatoie tastiera avanzate;
13. introduzione di tema scuro;
14. introduzione di selezione tema;
15. introduzione di salvataggio preferenze;
16. introduzione di configurazione persistente;
17. introduzione di notifiche Windows;
18. introduzione di minimizzazione in tray;
19. introduzione di packaging;
20. introduzione di installer;
21. introduzione di icone definitive;
22. introduzione di animazioni avanzate;
23. uso di TextBox libere per i tempi;
24. uso di Dispatcher nel Core;
25. uso di Dispatcher nel Bridge;
26. uso di Dispatcher nell'Audio;
27. uso di Dispatcher nell'Orchestrator;
28. uso di Dispatcher nel Runner;
29. uso di `Dispatcher.Invoke` per aggiornamenti ordinari del timer;
30. chiamate dirette dalla UI a `TimerEngine`;
31. chiamate dirette dalla UI a `TimerBridgeAdapter`;
32. chiamate dirette dalla UI a `SystemActionDispatcher`;
33. chiamate dirette dalla UI ad `AudioService`;
34. introduzione di polling automatico per aggiornare il countdown;
35. introduzione di timer UI paralleli;
36. modifica del Core per aggiungere il supporto ciclico se non esiste;
37. modifica del Bridge per aggiungere il supporto ciclico se non esiste;
38. modifica dell'Orchestratore per aggiungere il supporto ciclico se non esiste;
39. modifica del Runner per aggiungere notifiche UI.

Se Cursor ritiene necessaria una modifica fuori perimetro, deve fermarsi e produrre un report tecnico.

---

## 5. Preflight obbligatorio

Prima di modificare file, eseguire questi controlli.

### 5.1 Leggere documentazione

Leggere integralmente:

```text
docs/0-architecture/document-standards.md
docs/0-architecture/architecture.md
docs/0-architecture/accessibility-rules.md
docs/0-architecture/internal-api.md
docs/1-design/007-DESIGN_ui-wpf-minima.md
```

Verificare che il coding plan resti coerente con:

1. separazione UI / Core / Bridge / App / Audio / Localization;
2. divieto di logica timer nella UI;
3. testi utente centralizzati;
4. accessibilità avanzata rinviata al blocco 008;
5. nessuna implementazione senza documento approvato.

### 5.2 Verificare stato repository

Eseguire:

```bash
git status
```

Il working tree deve essere pulito prima della codifica.

Se non è pulito, fermarsi e segnalare.

### 5.3 Verificare build e test iniziali

Eseguire:

```bash
dotnet build
```

Eseguire:

```bash
dotnet test
```

Se falliscono per problemi preesistenti, fermarsi e segnalare.

Non correggere problemi preesistenti fuori perimetro.

### 5.4 Verificare struttura progetti

Verificare l'esistenza di:

```text
views/
view-models/
services/CicloTimer.App/
services/CicloTimer.App/Timing/
locales/CicloTimer.Localization/
tests/
```

Verificare il nome reale dei progetti `.csproj` presenti in:

```text
views/
view-models/
tests/
```

Non assumere nomi di progetto senza verifica.

---

## 6. Ricognizione codice esistente

Prima della codifica, leggere il codice reale prodotto nei blocchi 001–006.

In particolare individuare:

```text
ITimerAppOrchestrator
TimerAppOrchestrator
TimerAppState
AppCommandResult
IRealtimeTimerRunner
RealtimeTimerRunner
TimerInput
TimerDisplayModel o modello equivalente
localization service o classe testi esistente
```

Per ogni tipo annotare:

1. namespace reale;
2. file reale;
3. proprietà pubbliche;
4. metodi pubblici;
5. comportamento documentato dai test.

Il coding plan non autorizza a indovinare firme.

Se le firme reali non permettono quanto previsto dal Design 007, fermarsi e segnalare.

---

## 7. Verifica supporto timer ciclico

Il Design 007 autorizza la UI a esporre il timer ciclico solo se supportato dai contratti esistenti.

Prima di implementare la CheckBox ciclica, verificare nel codice reale:

1. se `TimerInput` o configurazione equivalente contiene un flag ciclico;
2. se il Core gestisce la ripetizione automatica;
3. se il Bridge espone tale informazione;
4. se l'Orchestratore accetta tale configurazione;
5. se i test esistenti coprono il comportamento ciclico.

Se il supporto ciclico è già presente:

1. esporre `IsCyclic` nel ViewModel;
2. bindare `IsCyclic` alla CheckBox;
3. passare `IsCyclic` alla configurazione applicativa;
4. includere i relativi test ViewModel;
5. mostrare la CheckBox nella UI.

Se il supporto ciclico non è presente:

1. non implementare la CheckBox ciclica nello XAML;
2. non esporre `IsCyclic` nel ViewModel;
3. non aggiungere proprietà obsolete o non usate;
4. non modificare il Core;
5. non modificare il Bridge;
6. non modificare l'Orchestratore;
7. non inventare nuova logica ciclica nel ViewModel;
8. procedere comunque con la UI minima a ciclo singolo;
9. segnalare nel report finale che il timer ciclico è stato escluso perché non supportato dai contratti reali.

Questa sezione non deve bloccare l'intero blocco 007.

Deve solo impedire che Cursor inventi una funzionalità non presente.

---

## 8. Struttura fisica prevista

La struttura concettuale prevista è:

```text
views/
  CicloTimer.Views/
    MainWindow.xaml
    MainWindow.xaml.cs
    Controls/
      NumericStepControl.xaml
      NumericStepControl.xaml.cs

view-models/
  CicloTimer.ViewModels/
    MainTimerViewModel.cs
    RelayCommand.cs
    IUiDispatcher.cs
    UiDispatcherAdapter.cs
```

Questa struttura è indicativa.

Cursor deve adattarsi alla struttura reale già presente nel repository.

Se il progetto WPF esistente usa nomi diversi, mantenere la struttura reale.

Il significato deve restare:

```text
views/
→ XAML, code-behind minimo, controlli WPF.

view-models/
→ ViewModel, comandi, adapter di presentazione.
```

Sono vietate collocazioni alternative come:

```text
src/
ui/
frontend/
windows/
presentation/
services/CicloTimer.App/ViewModels/
models/CicloTimer.UI/
```

salvo struttura già esistente nel repository e approvata.

---

## 9. Progetto views

### 9.1 Percorso

Il progetto UI vive sotto:

```text
views/
```

Cursor deve verificare se esiste già un progetto WPF.

Se il progetto WPF esiste:

1. riutilizzarlo;
2. non crearne uno duplicato;
3. aggiornare solo i file necessari.

Se il progetto WPF non esiste:

1. fermarsi;
2. segnalare il problema;
3. non creare nuovo progetto WPF senza autorizzazione.

Motivo:

il Design 007 dice che la cartella `views/` è già presente ed è la sede della UI.

### 9.2 Responsabilità

Il progetto views deve contenere:

1. finestra WPF;
2. layout XAML;
3. controlli visuali;
4. binding;
5. code-behind minimo necessario all'avvio e al DataContext;
6. eventuale gestione minimale dell'evento di chiusura per invocare cleanup sul ViewModel.

Il progetto views non deve contenere:

1. logica timer;
2. logica Core;
3. calcolo tempo rimanente;
4. incremento sessioni;
5. gestione audio;
6. formattazione autonoma degli stati;
7. testi utente non centralizzati;
8. logica ciclica.

---

## 10. Progetto view-models

### 10.1 Percorso

Il progetto o cartella ViewModel vive sotto:

```text
view-models/
```

Cursor deve verificare la struttura reale.

Se esiste già un progetto ViewModels:

1. riutilizzarlo;
2. aggiungere i file necessari;
3. rispettare namespace e stile esistenti.

Se non esiste un progetto dedicato ma esiste struttura compatibile:

1. seguirla;
2. documentare nel report finale.

Se non esiste nulla di compatibile:

1. fermarsi;
2. segnalare;
3. non creare nuova architettura senza conferma.

### 10.2 Responsabilità

Il ViewModel deve:

1. implementare `INotifyPropertyChanged`;
2. esporre proprietà bindabili;
3. esporre `ICommand`;
4. ricevere o creare i servizi applicativi necessari secondo struttura reale;
5. coordinare `ITimerAppOrchestrator`;
6. coordinare `IRealtimeTimerRunner`;
7. costruire la configurazione applicativa;
8. validare input UI prima dell'avvio;
9. esporre stato di validazione;
10. aggiornare proprietà UI tramite marshalling asincrono se necessario;
11. esporre un metodo di cleanup o implementare `IDisposable` se gestisce runner o sottoscrizioni.

Il ViewModel non deve:

1. calcolare logica Core;
2. duplicare Bridge;
3. formattare stati ignorando localization;
4. controllare direttamente audio;
5. calcolare tick reali;
6. usare timer propri;
7. implementare manualmente ripetizione ciclica;
8. introdurre polling automatico del countdown.

---

## 11. Deroga MVVM controllata

Nel blocco 007 è autorizzato l'uso di:

```text
INotifyPropertyChanged
ICommand
RelayCommand
```

Solo nei seguenti percorsi:

```text
views/
view-models/
tests del ViewModel
```

È vietato introdurre questi elementi in:

```text
models/CicloTimer.Core/
services/CicloTimer.App/
services/CicloTimer.App/Timing/
services/CicloTimer.Audio/
locales/CicloTimer.Localization/
```

Se esiste già un `RelayCommand` o equivalente:

1. riutilizzarlo;
2. non crearne uno duplicato.

Se non esiste:

1. creare un `RelayCommand` minimale;
2. collocarlo nel livello `view-models/`;
3. testarlo se contiene logica non banale.

---

## 12. MainTimerViewModel

Creare o aggiornare il ViewModel principale.

Nome consigliato:

```text
MainTimerViewModel
```

Il nome può cambiare se la struttura reale del progetto usa una convenzione diversa.

### 12.1 Dipendenze del ViewModel

Il ViewModel deve ricevere le dipendenze principali tramite costruttore, dove possibile:

```text
ITimerAppOrchestrator
IRealtimeTimerRunner
servizio localization o provider testi
adapter marshalling UI asincrono, se necessario
```

Se il progetto non usa ancora dependency injection:

1. è ammessa una composizione minima nel code-behind o in `App.xaml.cs`;
2. la composizione deve restare fuori dal ViewModel;
3. la scelta deve essere documentata nel report finale.

Il ViewModel non deve creare direttamente:

```text
TimerEngine
TimerBridgeAdapter
SystemActionDispatcher
AudioService
```

Se per costruire l'orchestratore servono adapter reali, la composizione deve avvenire nel punto di avvio UI, non dentro la logica del ViewModel.

### 12.2 Cleanup del ViewModel

Se il ViewModel possiede o coordina `IRealtimeTimerRunner`, sottoscrizioni a eventi o adapter UI, deve prevedere cleanup sicuro.

Soluzioni ammesse:

1. implementare `IDisposable`;
2. esporre un metodo `Dispose()` o equivalente;
3. chiamare `runner.Stop()` o `runner.Dispose()` secondo il modello reale di proprietà dell'istanza;
4. rimuovere eventuali sottoscrizioni a eventi pubblici;
5. non lasciare loop attivi dopo la chiusura della finestra.

La chiamata al cleanup può essere effettuata dal code-behind della finestra durante `Closing` o `Closed`.

Il code-behind deve limitarsi a invocare il cleanup e non deve contenere logica timer.

---

## 13. Proprietà bindabili obbligatorie

Il ViewModel deve esporre almeno:

```text
CycleMinutes
CycleSeconds
FinalAlertSeconds
IsConfigurationValid
ValidationErrorText
RemainingTimeText
TimerStateText
CompletedSessionsText
PrimaryButtonText
CanExecutePrimaryCommand
CanExecuteResetCommand
```

Se il supporto ciclico è presente nei contratti reali, deve esporre anche:

```text
IsCyclic
```

Le proprietà devono notificare cambiamento tramite `INotifyPropertyChanged`.

### 13.1 CycleMinutes

Regole:

```text
CycleMinutes >= 0
```

Valore default consigliato:

```text
25
```

### 13.2 CycleSeconds

Regole:

```text
0 <= CycleSeconds <= 59
```

Valore default consigliato:

```text
0
```

### 13.3 FinalAlertSeconds

Regole:

```text
FinalAlertSeconds >= 0
FinalAlertSeconds <= durata totale ciclo
```

Valore massimo iniziale consigliato:

```text
60
```

Valore default consigliato:

```text
10
```

### 13.4 IsCyclic

Regole:

```text
boolean
default false
```

La proprietà deve esistere solo se supportata dai contratti reali.

Se il supporto ciclico non esiste, non creare proprietà fittizie.

### 13.5 IsConfigurationValid

Deve essere `true` solo quando:

```text
durata totale ciclo >= 1 secondo
FinalAlertSeconds <= durata totale ciclo
configurazione accettabile dai contratti applicativi
```

### 13.6 ValidationErrorText

Deve contenere testo localizzato quando la configurazione non è valida.

Non deve contenere testo hardcoded.

Può essere stringa vuota quando la configurazione è valida.

---

## 14. Comandi obbligatori

Il ViewModel deve esporre almeno:

```text
PrimaryCommand
ResetCommand
```

Entrambi devono essere `ICommand`.

### 14.1 PrimaryCommand

Il comando principale deve eseguire azione diversa in base allo stato:

```text
Fermo/configurabile → Avvia
In esecuzione → Pausa
In pausa → Riprendi
Completato → Avvia
```

Il comando principale non deve partire se:

```text
IsConfigurationValid == false
```

negli stati in cui l'azione è `Avvia`.

### 14.2 ResetCommand

Il comando Reset deve:

1. fermare il runner se necessario;
2. chiamare il reset applicativo;
3. aggiornare le proprietà visuali.

Reset non deve:

1. cancellare la configurazione scelta dall'utente;
2. modificare `IsCyclic`, se presente;
3. creare nuovi servizi;
4. ricreare Core o Bridge.

---

## 15. Stato del pulsante principale

Il ViewModel deve aggiornare `PrimaryButtonText`.

Mappatura concettuale:

```text
Fermo → Avvia
In esecuzione → Pausa
In pausa → Riprendi
Completato → Avvia
```

Il testo deve provenire dalla localization o da proprietà già localizzate.

Non usare stringhe hardcoded nel ViewModel.

Se il modello applicativo non espone uno stato `Completato` stabile ma solo evento di completamento, il coding plan deve adattarsi al comportamento reale:

1. se timer singolo completa, mostrare `Avvia`;
2. se timer ciclico prosegue, mantenere comportamento coerente con lo stato corrente esposto.

Non inventare uno stato stabile nuovo nel Core.

---

## 16. Validazione configurazione

La validazione UI deve essere minima e preventiva.

Il ViewModel deve impedire l'avvio quando:

```text
CycleMinutes == 0 && CycleSeconds == 0
FinalAlertSeconds > durata totale ciclo
valori fuori range
contratto applicativo rifiuta la configurazione
```

Il ViewModel può validare:

1. range numerici;
2. presenza di durata minima;
3. relazione tra avviso finale e durata ciclo.

Il ViewModel non deve duplicare tutte le regole logiche del Core.

Il Core resta l'autorità finale.

Se l'orchestratore o il Core restituiscono errore di configurazione, il ViewModel deve:

1. non avviare il runner;
2. aggiornare `IsConfigurationValid`;
3. aggiornare `ValidationErrorText`;
4. lasciare UI in stato configurabile.

---

## 17. NumericStepControl

Creare un controllo numerico guidato se non esiste già.

Nome consigliato:

```text
NumericStepControl
```

Percorso consigliato:

```text
views/.../Controls/NumericStepControl.xaml
views/.../Controls/NumericStepControl.xaml.cs
```

Il percorso deve adattarsi alla struttura reale del progetto.

### 17.1 Responsabilità

Il controllo deve:

1. mostrare un valore numerico;
2. permettere incremento;
3. permettere decremento;
4. rispettare valore minimo;
5. rispettare valore massimo;
6. esporre proprietà bindabili;
7. non usare TextBox libera per l'inserimento diretto.

### 17.2 Proprietà obbligatorie o consigliate

Se implementato come `UserControl` WPF, le proprietà principali devono essere `DependencyProperty`.

Proprietà consigliate:

```text
Value
Minimum
Maximum
Step
Label
```

Regole:

1. `Value` deve essere una `DependencyProperty`;
2. `Minimum` deve essere una `DependencyProperty`;
3. `Maximum` deve essere una `DependencyProperty`;
4. `Step` deve essere una `DependencyProperty`;
5. `Label` deve essere una `DependencyProperty` o ricevere testo tramite binding coerente;
6. il binding di `Value` deve supportare modalità `TwoWay`;
7. il controllo non deve aggiornare il ViewModel tramite chiamate procedurali manuali.

Il nome definitivo può cambiare se coerente con WPF e con lo stile del progetto.

### 17.3 Clamping rigido

Il controllo deve usare clamping rigido.

Esempio:

```text
Value = Maximum
utente preme +
Value resta Maximum
```

Esempio vietato:

```text
Secondi = 59
utente preme +
Minuti aumenta e secondi torna a 0
```

Il controllo non deve conoscere altri controlli.

Il ViewModel resta responsabile della validazione complessiva.

---

## 18. CheckBox timer ciclico

La CheckBox timer ciclico deve essere aggiunta solo se i contratti reali supportano il flag ciclico.

Etichetta concettuale:

```text
Ripeti automaticamente il ciclo
```

Binding:

```text
IsChecked ↔ IsCyclic
```

Default:

```text
false
```

Se i contratti reali non supportano `IsCyclic`:

1. non creare la CheckBox;
2. non creare binding fittizi;
3. non creare proprietà `IsCyclic`;
4. procedere con UI minima a ciclo singolo;
5. riportare la scelta nel report finale.

La CheckBox non deve implementare direttamente logica di ripetizione.

---

## 19. MainWindow

Creare o aggiornare la finestra principale WPF.

Nome consigliato:

```text
MainWindow
```

Percorso consigliato:

```text
views/.../MainWindow.xaml
views/.../MainWindow.xaml.cs
```

Se il progetto ha già una finestra principale con altro nome, riutilizzarla.

### 19.1 Layout

La finestra deve usare layout verticale a card.

Card previste:

```text
Card configurazione
Card timer
Card comandi
Card stato
```

Contenuto:

```text
Card configurazione
- durata ciclo minuti
- durata ciclo secondi
- avviso finale secondi
- ripeti automaticamente il ciclo, solo se supportato dai contratti reali

Card timer
- tempo rimanente grande

Card comandi
- pulsante principale dinamico
- pulsante Reset

Card stato
- stato corrente
- sessioni completate
- eventuale errore validazione
```

### 19.2 Stile

Usare tema chiaro pulito.

La UI deve essere gradevole ma non elaborata.

Sono ammessi:

1. margini;
2. padding;
3. border radius se coerente con WPF;
4. card con sfondo chiaro;
5. tempo grande centrale;
6. pulsanti leggibili.

Sono vietati:

1. animazioni avanzate;
2. icone definitive;
3. tema scuro;
4. effetti complessi;
5. dipendenze UI esterne non già presenti.

### 19.3 Dimensione

La finestra deve essere compatta.

Indicativamente:

```text
larghezza 420–500 px
altezza 540–720 px
```

Il coding agent può adattare le dimensioni alla resa reale.

### 19.4 Chiusura finestra

Alla chiusura della finestra, l'applicazione deve fermare o smaltire in modo sicuro il runner e le eventuali sottoscrizioni.

Soluzione consigliata:

1. se il `DataContext` implementa `IDisposable`, chiamare `Dispose()` da `Closing` o `Closed`;
2. il ViewModel si occupa di fermare il runner e rimuovere sottoscrizioni;
3. il code-behind non deve contenere logica timer;
4. il code-behind può solo invocare cleanup minimale.

Non lasciare loop del runner attivi dopo la chiusura della finestra.

---

## 20. Code-behind

Il code-behind deve restare minimale.

È autorizzato per:

1. `InitializeComponent`;
2. assegnazione del DataContext;
3. composizione minima delle dipendenze se non esiste DI;
4. eventuale inizializzazione UI strettamente necessaria;
5. invocazione del cleanup del ViewModel alla chiusura della finestra.

Il code-behind non deve contenere:

1. logica timer;
2. gestione manuale dei click se sostituibile con command binding;
3. calcolo tempo rimanente;
4. logica audio;
5. logica ciclica;
6. stringhe utente non centralizzate;
7. manipolazione procedurale della visibilità degli errori.

Se il code-behind contiene più di composizione minima o cleanup minimo, motivare nel report finale.

---

## 21. Composizione dipendenze

Il blocco 007 deve collegare concretamente:

```text
View
ViewModel
ITimerAppOrchestrator
IRealtimeTimerRunner
Localization
```

Prima di implementare, verificare come oggi vengono costruiti:

1. Bridge;
2. AudioService;
3. Orchestrator;
4. Runner;
5. Localization.

Possibili strategie:

### 21.1 DI già presente

Se esiste dependency injection:

1. registrare i servizi necessari;
2. registrare il ViewModel;
3. collegare il DataContext tramite DI;
4. non creare istanze manuali duplicate.

### 21.2 DI assente

Se non esiste dependency injection:

1. usare composizione manuale minima nel punto di avvio UI;
2. creare orchestratore e runner nel punto di composizione;
3. passare dipendenze al ViewModel via costruttore;
4. non creare dipendenze dentro il ViewModel;
5. documentare nel report finale.

### 21.3 Caso non chiaro

Se il punto di composizione non è chiaro:

1. fermarsi;
2. segnalare al project owner;
3. non creare un secondo sistema di composizione.

---

## 22. Marshalling UI asincrono

Gli aggiornamenti provenienti dal runner reale possono arrivare da thread non UI.

Il ViewModel deve aggiornare proprietà bindate sul thread WPF corretto.

Il Design 007 richiede marshalling asincrono.

Sono ammessi:

```text
Dispatcher.BeginInvoke
SynchronizationContext.Post
adapter minimale equivalente
```

È vietato:

```text
Dispatcher.Invoke
```

per aggiornamenti ordinari del timer.

### 22.1 Adapter consigliato

Se necessario, introdurre un adapter minimale nel livello `view-models/`.

Nome consigliato:

```text
IUiDispatcher
UiDispatcherAdapter
```

Responsabilità:

1. ricevere un'azione;
2. postarla sul thread UI;
3. non bloccare il chiamante;
4. non conoscere Core, Bridge, Audio, Orchestrator o Runner.

Esempio concettuale:

```text
Post(Action action)
```

Non introdurre dispatcher nei layer inferiori.

---

## 23. Aggiornamento display dopo comandi

Dopo ogni comando applicativo, il ViewModel deve aggiornare le proprietà bindate.

Comandi interessati:

```text
Configure/Start
Pause
Resume
Reset
Tick indiretto tramite orchestratore
```

Il ViewModel deve ottenere dal risultato applicativo o dallo stato corrente:

1. testo tempo rimanente;
2. testo stato;
3. sessioni completate;
4. eventuale errore;
5. stato pulsante principale;
6. abilitazione comandi.

Non deve calcolare questi dati direttamente se già disponibili dai modelli display.

---

## 24. Gestione tick e aggiornamento ViewModel

Il runner chiama:

```text
ITimerAppOrchestrator.Tick(...)
```

Il ViewModel deve aggiornarsi dopo i tick senza introdurre polling automatico e senza introdurre timer UI paralleli.

Prima di implementare, verificare come il runner e l'orchestratore comunicano lo stato aggiornato:

1. il runner chiama direttamente l'orchestratore;
2. l'orchestratore conserva `CurrentState`;
3. il ViewModel può leggere `CurrentState`;
4. l'orchestratore espone eventi pubblici di aggiornamento, se presenti;
5. il ViewModel può ricevere notifiche tramite eventi pubblici già esistenti, se presenti.

Strategia autorizzata:

1. usare eventi pubblici già esistenti dell'orchestratore, se presenti;
2. usare proprietà pubbliche già esistenti dell'orchestratore dopo comandi sincroni;
3. usare un adapter UI/ViewModel solo per marshalling, non per generare tick;
4. non modificare il runner;
5. non modificare l'orchestratore;
6. non introdurre polling;
7. non introdurre timer UI paralleli.

Se non esiste nessun meccanismo pratico per aggiornare la UI dopo i tick:

1. fermarsi;
2. segnalare il blocco tecnico;
3. non inventare polling;
4. non modificare il runner;
5. non modificare l'orchestratore;
6. attendere decisione architetturale successiva.

---

## 25. Localization

Il blocco 007 introduce testi utente.

Prima di scrivere stringhe, verificare il sistema di localization reale.

Individuare:

1. file principale dei testi;
2. formato delle chiavi;
3. metodo di accesso ai testi;
4. test esistenti sui testi.

Testi necessari:

```text
titolo app
durata ciclo
minuti
secondi
avviso finale
ripeti automaticamente il ciclo
avvia
pausa
riprendi
reset
stato
sessioni completate
errore durata zero
errore avviso finale maggiore durata
```

La chiave `ripeti automaticamente il ciclo` è necessaria solo se il supporto ciclico è presente.

Regole:

1. non usare stringhe utente hardcoded nel C#;
2. non lasciare stringhe definitive hardcoded in XAML se lo standard del progetto lo vieta;
3. se XAML richiede binding a proprietà localizzate, esporle dal ViewModel;
4. aggiungere test sulle nuove chiavi, se lo stile del progetto lo prevede.

---

## 26. Gestione errori UI

Il ViewModel deve gestire errori in modo minimale.

Tipi di errore:

1. configurazione non valida;
2. comando non consentito;
3. errore tecnico restituito dall'orchestratore.

Regole:

1. non mostrare stack trace;
2. non inventare messaggi fuori localization;
3. non interpretare eccezioni interne;
4. non avviare il runner se la configurazione non è valida;
5. non lasciare UI in stato incoerente dopo errore.

La UI deve mostrare `ValidationErrorText` o equivalente.

La visibilità del messaggio o pannello di errore deve essere gestita tramite binding XAML.

Sono ammesse soluzioni come:

```text
BooleanToVisibilityConverter
converter equivalente
binding su IsConfigurationValid
binding su presenza di ValidationErrorText
```

È vietato manipolare proceduralmente dal code-behind la visibilità degli elementi di errore.

---

## 27. Accessibilità minima del blocco 007

L'accessibilità avanzata resta fuori perimetro.

Tuttavia Cursor deve rispettare questi minimi:

1. usare controlli WPF standard quando possibile;
2. usare etichette visibili;
3. mantenere ordine visuale coerente;
4. evitare controlli solo grafici;
5. non aggiornare testi accessibili rumorosi ogni secondo in modo intenzionale;
6. non introdurre live region;
7. non introdurre annunci NVDA;
8. non introdurre scorciatoie avanzate.

Il blocco 008 farà la rifinitura accessibile.

---

## 28. File da creare o modificare

L'elenco concreto deve essere confermato dopo ricognizione, ma i file attesi sono:

```text
views/.../MainWindow.xaml
views/.../MainWindow.xaml.cs
views/.../Controls/NumericStepControl.xaml
views/.../Controls/NumericStepControl.xaml.cs

view-models/.../MainTimerViewModel.cs
view-models/.../RelayCommand.cs
view-models/.../IUiDispatcher.cs
view-models/.../UiDispatcherAdapter.cs

tests/.../MainTimerViewModelTests.cs
tests/.../NumericStepControlTests.cs se applicabile
tests/.../ProjectDependencyTests.cs se già esiste pattern analogo
```

Se il supporto ciclico non è presente, non creare file o proprietà dedicate solo al comportamento ciclico.

Non creare tutti questi file se esistono già equivalenti.

Riutilizzare prima di duplicare.

---

## 29. Ordine operativo consigliato

Seguire questo ordine:

1. leggere documentazione architetturale;
2. leggere Design 007;
3. eseguire `git status`;
4. eseguire `dotnet build`;
5. eseguire `dotnet test`;
6. verificare struttura `views/`;
7. verificare struttura `view-models/`;
8. verificare localization;
9. verificare contratti orchestratore/runner;
10. verificare supporto timer ciclico;
11. decidere se includere o escludere la CheckBox ciclica in base ai contratti reali;
12. verificare meccanismo di aggiornamento dopo tick;
13. se non esiste meccanismo pratico di aggiornamento dopo tick, fermarsi e segnalare;
14. verificare modalità di composizione dipendenze;
15. creare o aggiornare `RelayCommand`;
16. creare o aggiornare adapter marshalling UI;
17. creare `MainTimerViewModel`;
18. aggiungere proprietà bindabili;
19. aggiungere validazione configurazione;
20. aggiungere comandi;
21. creare o aggiornare `NumericStepControl`;
22. creare o aggiornare `MainWindow`;
23. collegare DataContext;
24. integrare localization;
25. implementare cleanup alla chiusura finestra;
26. scrivere test ViewModel;
27. scrivere test controllo numerico, se testabile;
28. scrivere test architetturali;
29. eseguire build;
30. eseguire test;
31. pulire artefatti non utili;
32. eseguire `git status`;
33. produrre report finale.

---

## 30. Test obbligatori ViewModel

Creare test per:

1. stato iniziale;
2. valori default;
3. `CycleMinutes` default;
4. `CycleSeconds` default;
5. `FinalAlertSeconds` default;
6. `IsConfigurationValid` true in configurazione default;
7. durata zero non valida;
8. secondi fuori range non ammessi o corretti;
9. avviso finale maggiore durata non valido;
10. `ValidationErrorText` valorizzato su errore;
11. `ValidationErrorText` vuoto su configurazione valida;
12. `PrimaryButtonText` iniziale = testo Avvia localizzato;
13. `PrimaryCommand` in stato iniziale chiama configurazione e start;
14. `PrimaryCommand` non avvia se configurazione non valida;
15. stato running cambia pulsante in Pausa;
16. stato pausa cambia pulsante in Riprendi;
17. stato completato cambia pulsante in Avvia;
18. `ResetCommand` chiama stop runner e reset orchestratore;
19. Reset non cancella configurazione utente;
20. notifiche `PropertyChanged` emesse per proprietà rilevanti;
21. `CanExecute` aggiornato quando cambia validazione;
22. cleanup del ViewModel ferma o dispone il runner;
23. cleanup del ViewModel rimuove eventuali sottoscrizioni;
24. nessuna chiamata diretta ad AudioService;
25. nessuna chiamata diretta a Core;
26. nessuna chiamata diretta a Bridge.

Se il supporto ciclico è presente, aggiungere anche test per:

1. `IsCyclic` default false;
2. Reset non modifica `IsCyclic`;
3. `IsCyclic` viene passato alla configurazione.

Se il supporto ciclico non è presente, aggiungere o verificare che:

1. `IsCyclic` non sia esposto;
2. la CheckBox non sia presente;
3. il report finale segnali l'esclusione del timer ciclico dal blocco 007.

---

## 31. Test obbligatori NumericStepControl

Se il controllo viene creato, prevedere test o verifiche per:

1. valore iniziale;
2. incremento;
3. decremento;
4. rispetto minimo;
5. rispetto massimo;
6. clamping rigido;
7. nessun overflow verso altri controlli;
8. binding di `Value`;
9. `Value` come `DependencyProperty`, se il controllo è un UserControl;
10. binding `TwoWay` supportato;
11. nessuna TextBox libera.

Se i test XAML/UI non sono già supportati nel repository, documentare nel report finale quali verifiche sono manuali.

---

## 32. Test architetturali

Aggiungere o aggiornare test per verificare che `views/` e `view-models/` non violino i confini.

Controllare che `views/` non contenga riferimenti diretti a:

```text
TimerEngine
TimerBridgeAdapter
SystemActionDispatcher
AudioService
CicloTimer.Core
```

Controllare che `view-models/` non contenga riferimenti diretti a:

```text
TimerEngine
TimerBridgeAdapter
SystemActionDispatcher
AudioService
CicloTimer.Core
```

Controllare che `INotifyPropertyChanged`, `ICommand` e `RelayCommand` non siano introdotti nei layer vietati:

```text
models/CicloTimer.Core/
services/CicloTimer.App/
services/CicloTimer.App/Timing/
services/CicloTimer.Audio/
locales/CicloTimer.Localization/
```

Controllare che `Dispatcher.Invoke` non sia usato per aggiornamenti ordinari del timer.

Controllare che non siano introdotti:

```text
DispatcherTimer
System.Timers.Timer
System.Threading.Timer
Task.Delay come timer UI parallelo
```

per aggiornare il countdown della UI.

Sono ammessi:

```text
Dispatcher.BeginInvoke
SynchronizationContext.Post
adapter equivalente
```

nel livello UI/ViewModel.

---

## 33. Test localization

Se vengono aggiunte chiavi di testo:

1. aggiungere test che verificano presenza delle chiavi;
2. verificare che nessuna chiave necessaria sia vuota;
3. verificare che i testi pulsante siano disponibili;
4. verificare che gli errori di validazione siano disponibili;
5. verificare che `Ripeti automaticamente il ciclo` o equivalente sia disponibile solo se il supporto ciclico è implementato.

Non introdurre stringhe utente sparse.

---

## 34. Verifiche manuali UI

Dopo l'implementazione, eseguire verifica manuale minimale:

1. avvio applicazione;
2. finestra visualizzata;
3. layout a card visibile;
4. tema chiaro pulito;
5. finestra compatta;
6. tempo grande centrale;
7. incremento minuti;
8. decremento minuti;
9. incremento secondi;
10. decremento secondi;
11. secondi bloccati a 59;
12. secondi bloccati a 0;
13. avviso finale configurabile;
14. avviso finale non supera durata;
15. CheckBox ciclica visibile solo se supportata;
16. CheckBox ciclica assente se non supportata;
17. Avvia funziona;
18. Pausa funziona;
19. Riprendi funziona;
20. Reset funziona;
21. sessioni completate visibili;
22. errore configurazione visibile tramite binding;
23. nessun crash durante tick;
24. nessun freeze UI evidente;
25. chiusura finestra ferma o dispone il runner senza errori.

Non eseguire test avanzati NVDA in questo blocco.

---

## 35. Build e test finali

Al termine eseguire:

```bash
dotnet build
```

Poi:

```bash
dotnet test
```

Poi:

```bash
git status
```

Se `dotnet build` o `dotnet test` modificano artefatti `bin/` o `obj/`, verificare che siano ignorati.

Non committare artefatti generati.

---

## 36. Pulizia finale

Prima del report finale:

1. cercare file temporanei;
2. rimuovere file fantasma;
3. verificare using inutilizzati;
4. verificare namespace non usati;
5. verificare file duplicati;
6. verificare che non siano stati creati progetti non autorizzati;
7. verificare che non siano stati modificati file fuori perimetro.

Cercare almeno:

```text
*.tmp
*.bak
*.old
*.orig
*.rej
*_new.cs
*_copy.cs
*_temp.cs
*_old.cs
scratch.cs
test.cs
debug-*.log
```

Non cancellare file non compresi senza conferma.

---

## 37. Report finale richiesto

Cursor deve produrre un report finale con:

1. file creati;
2. file modificati;
3. file non modificati perché riutilizzati;
4. decisioni implementative adottate;
5. eventuali scostamenti dal Design 007;
6. eventuali punti non implementati;
7. verifica supporto timer ciclico;
8. decisione presa sul timer ciclico;
9. strategia di localization usata;
10. strategia di marshalling UI usata;
11. strategia di aggiornamento UI dopo tick;
12. strategia di cleanup alla chiusura finestra;
13. esito `dotnet build`;
14. esito `dotnet test`;
15. esito `git status`;
16. eventuali problemi residui;
17. conferma che non sono stati committati artefatti `bin/` o `obj/`.

Non eseguire commit.

Non eseguire push.

---

## 38. Criteri di accettazione

Il blocco 007 è completato quando:

1. esiste una finestra WPF avviabile;
2. la UI risiede in `views/`;
3. il ViewModel risiede in `view-models/`;
4. la finestra usa layout a card;
5. la finestra è compatta;
6. il tema è chiaro e pulito;
7. la durata ciclo usa controlli guidati;
8. l'avviso finale usa controlli guidati;
9. non esistono TextBox libere per i tempi;
10. il timer ciclico è configurabile solo se supportato dai contratti esistenti;
11. se il timer ciclico non è supportato, la UI resta correttamente a ciclo singolo;
12. `NumericStepControl` usa clamping rigido;
13. `NumericStepControl.Value` supporta binding `TwoWay`;
14. il tempo rimanente è grande e centrale;
15. il pulsante principale è dinamico;
16. in stato completato il pulsante principale torna ad Avvia;
17. Reset è separato;
18. stato timer visibile;
19. sessioni completate visibili;
20. `IsConfigurationValid` o equivalente esiste;
21. `ValidationErrorText` o equivalente esiste;
22. errore di validazione visibile tramite binding XAML;
23. Start funziona;
24. Pause funziona;
25. Resume funziona;
26. Reset funziona;
27. runner avviato e fermato in modo coerente;
28. marshalling UI asincrono se necessario;
29. nessun Dispatcher nei layer inferiori;
30. nessun `Dispatcher.Invoke` per aggiornamenti ordinari del timer;
31. nessun polling automatico per countdown;
32. nessun timer UI parallelo per countdown;
33. cleanup alla chiusura finestra presente;
34. nessuna logica Core nella UI;
35. nessun controllo diretto dell'audio nella UI;
36. nessun calcolo diretto del tempo reale nella UI;
37. testi utente centralizzati;
38. `INotifyPropertyChanged` e `ICommand` confinati al layer presentazione;
39. accessibilità avanzata non anticipata;
40. test nuovi passano;
41. test 001–006 restano verdi;
42. working tree non contiene file fantasma o artefatti generati.

---

## 39. Fuori perimetro operativo finale

Non fare in questo blocco:

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
nuove regole Bridge
nuove regole Audio
nuove regole Runner
polling countdown
timer UI parallelo
```

Il blocco successivo naturale resta:

```text
008 — Accessibilità UI e rifinitura interazione
```

---

## 40. Esito atteso

Alla fine del blocco 007 l'applicazione CicloTimer deve avere una prima UI WPF minima e funzionante.

L'utente deve poter:

1. configurare durata ciclo;
2. configurare avviso finale;
3. scegliere timer singolo o ciclico, se supportato;
4. avviare;
5. mettere in pausa;
6. riprendere;
7. resettare;
8. vedere countdown;
9. vedere stato;
10. vedere sessioni completate.

Se il supporto ciclico non è presente nei contratti reali, l'utente userà una UI minima a ciclo singolo e il report finale dovrà segnalarlo esplicitamente.

Il risultato non deve essere ancora la UI definitiva.

Deve essere una prima interfaccia pulita, compatta e corretta, pronta per il successivo lavoro di accessibilità del blocco 008.
