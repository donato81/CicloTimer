# TODO 007 — UI WPF minima

## Metadati

* Tipo documento: TODO operativo
* Codice: 007
* Titolo: UI WPF minima
* Versione: 0.2.0
* Stato: APPROVATO
* Progetto: CicloTimer
* Repository: donato81/CicloTimer
* Data: 2026-06-04
* Design di riferimento: `docs/1-design/007-DESIGN_ui-wpf-minima.md`
* Coding plan di riferimento: `docs/2-coding-plans/007-PLAN_ui-wpf-minima.md`

---

## 1. Scopo del TODO

Questo documento traduce il Coding Plan 007 in una checklist operativa per la codifica reale.

Il blocco 007 deve introdurre la **prima UI WPF minima** dell'applicazione CicloTimer.

La UI deve permettere all'utente di:

```text
configurare durata ciclo
configurare durata avviso finale
avviare
mettere in pausa
riprendere
resettare
vedere il tempo rimanente
vedere lo stato corrente
vedere le sessioni completate
```

La UI deve essere:

```text
minima
funzionante
compatta
a card
chiara
pulita
```

Il blocco 007 non deve introdurre accessibilità avanzata.

Il blocco 007 non deve modificare la logica Core, Bridge, Audio, Orchestrator o Runner.

---

## 2. Stato iniziale richiesto

Prima di iniziare, verificare che siano già presenti e chiusi i blocchi:

```text
001 — Core timer engine
002 — Bridge UI-logica
003 — Sistema testi/localization
004 — Audio service e audio focus
005 — Orchestratore applicativo timer
006 — Gestore timer reale
```

Verificare che esistano:

```text
views/
view-models/
services/CicloTimer.App/
services/CicloTimer.App/Timing/
locales/CicloTimer.Localization/
tests/
```

Verificare che esistano i contratti:

```text
ITimerAppOrchestrator
IRealtimeTimerRunner
```

Verificare che il Design 007 e il Coding Plan 007 siano presenti e approvati.

---

## 3. Vincoli assoluti

Durante questo blocco non introdurre:

```text
nuove regole Core
nuove regole Bridge
nuove regole Audio
nuove regole Orchestrator
nuove regole Runner
tema scuro
salvataggio preferenze
packaging
installer
tray icon
notifiche Windows
accessibilità avanzata
NVDA completo
Live Region
annunci vocali
shortcut avanzate
animazioni avanzate
icone definitive
TextBox libere per i tempi
polling automatico countdown
timer UI parallelo
DispatcherTimer
System.Timers.Timer
System.Threading.Timer per aggiornare la UI
Dispatcher.Invoke per aggiornamenti ordinari del timer
```

Non chiamare direttamente dalla UI o dal ViewModel:

```text
TimerEngine
TimerBridgeAdapter
SystemActionDispatcher
AudioService
CicloTimer.Core
```

Non modificare, salvo necessità documentata e approvata:

```text
models/CicloTimer.Core/
view-models/CicloTimer.Bridge/
services/CicloTimer.App/Timing/
services/CicloTimer.Audio/
locales/CicloTimer.Localization/
```

Eccezione ammessa:

```text
locales/CicloTimer.Localization/
```

solo per aggiungere chiavi di testo strettamente necessarie alla UI 007.

---

## 4. Preflight obbligatorio

Prima di creare o modificare file, eseguire questi controlli.

### 4.1 Verifica repository

* [ ] Eseguire `git status`.
* [ ] Verificare che il working tree sia pulito.
* [ ] Se il working tree non è pulito, fermarsi e segnalare.

### 4.2 Verifica solution

* [ ] Aprire la solution del repository.
* [ ] Verificare che i progetti 001–006 compilino.
* [ ] Verificare che i test 001–006 siano presenti.
* [ ] Verificare che il progetto WPF in `views/` esista.
* [ ] Verificare che il progetto o cartella ViewModel in `view-models/` esista.
* [ ] Verificare che i progetti siano su `.NET 9` o coerenti con la solution.

### 4.3 Build iniziale

Eseguire:

```bash
dotnet build
```

Checklist:

* [ ] Build completata.
* [ ] Nessun errore preesistente.
* [ ] Se ci sono errori preesistenti fuori perimetro, fermarsi e segnalare.

### 4.4 Test iniziali

Eseguire:

```bash
dotnet test
```

Checklist:

* [ ] Test completati.
* [ ] Nessuna regressione preesistente.
* [ ] Se ci sono test rotti fuori perimetro, fermarsi e segnalare.

---

## 5. Lettura obbligatoria documenti

Prima di codificare, leggere integralmente:

```text
docs/0-architecture/document-standards.md
docs/0-architecture/architecture.md
docs/0-architecture/accessibility-rules.md
docs/0-architecture/internal-api.md
docs/1-design/007-DESIGN_ui-wpf-minima.md
docs/2-coding-plans/007-PLAN_ui-wpf-minima.md
```

Checklist:

* [ ] Confermare che il blocco 007 riguarda solo UI WPF minima.
* [ ] Confermare che accessibilità avanzata è rinviata al blocco 008.
* [ ] Confermare che la UI vive in `views/`.
* [ ] Confermare che i ViewModel vivono in `view-models/`.
* [ ] Confermare che la UI non deve contenere logica timer.
* [ ] Confermare che i testi utente devono essere centralizzati.
* [ ] Confermare che `INotifyPropertyChanged`, `ICommand` e `RelayCommand` sono ammessi solo nel layer presentazione.
* [ ] Confermare che il marshalling UI deve essere asincrono.
* [ ] Confermare che `Dispatcher.Invoke` è vietato per aggiornamenti ordinari del timer.

---

## 6. Ricognizione codice esistente

Aprire e leggere i file reali relativi a:

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

Checklist:

* [ ] Annotare namespace reale di `ITimerAppOrchestrator`.
* [ ] Annotare firma reale di `Configure`, se presente.
* [ ] Annotare firma reale di `Start`.
* [ ] Annotare firma reale di `Pause`.
* [ ] Annotare firma reale di `Resume`.
* [ ] Annotare firma reale di `Reset`.
* [ ] Annotare firma reale di `Tick`.
* [ ] Annotare struttura reale di `TimerAppState`.
* [ ] Annotare struttura reale di `AppCommandResult`.
* [ ] Annotare struttura reale di `TimerInput`.
* [ ] Annotare modello display usato dalla UI o dal Bridge.
* [ ] Annotare metodo reale per ottenere testi localizzati.
* [ ] Non indovinare firme.
* [ ] Se una firma impedisce il piano, fermarsi e segnalare.

---

## 7. Verifica supporto timer ciclico

Prima di implementare la CheckBox del timer ciclico, verificare se il supporto è già presente.

Controllare:

```text
TimerInput
Core
Bridge
Orchestrator
test esistenti
```

Checklist supporto presente:

* [ ] `TimerInput` o configurazione equivalente contiene un flag ciclico.
* [ ] Il Core gestisce il timer ciclico.
* [ ] Il Bridge conserva o espone il comportamento ciclico.
* [ ] L'Orchestratore accetta il flag ciclico.
* [ ] I test esistenti confermano il comportamento.

Se il supporto ciclico è presente:

* [ ] Esporre `IsCyclic` nel ViewModel.
* [ ] Bindare `IsCyclic` alla CheckBox.
* [ ] Passare `IsCyclic` alla configurazione applicativa.
* [ ] Aggiungere test per `IsCyclic`.
* [ ] Mostrare la CheckBox nella UI.

Se il supporto ciclico non è presente:

* [ ] Non implementare la CheckBox ciclica.
* [ ] Non esporre `IsCyclic` nel ViewModel.
* [ ] Non creare proprietà obsolete o non usate.
* [ ] Non modificare Core.
* [ ] Non modificare Bridge.
* [ ] Non modificare Orchestrator.
* [ ] Non inventare logica ciclica nel ViewModel.
* [ ] Procedere con UI minima a ciclo singolo.
* [ ] Segnalare nel report finale che il timer ciclico è escluso perché non supportato dai contratti reali.

Regola:

```text
La mancanza del supporto ciclico non blocca l'intero blocco 007.
Blocca solo la CheckBox ciclica.
```

---

## 8. Verifica aggiornamento dopo tick

Prima di implementare aggiornamenti UI automatici, verificare come il ViewModel può sapere che lo stato è cambiato dopo un tick.

Controllare:

```text
ITimerAppOrchestrator
TimerAppOrchestrator
TimerAppState
IRealtimeTimerRunner
test blocco 005
test blocco 006
```

Checklist:

* [ ] Verificare se l'orchestratore espone eventi pubblici di aggiornamento.
* [ ] Verificare se esiste `StateChanged`, `DisplayModelUpdated` o equivalente.
* [ ] Verificare se il ViewModel può leggere `CurrentState` dopo comandi sincroni.
* [ ] Verificare se esiste già un meccanismo previsto per notificare la UI.
* [ ] Non introdurre polling automatico.
* [ ] Non introdurre timer UI parallelo.
* [ ] Non modificare il runner.
* [ ] Non modificare l'orchestratore.
* [ ] Se non esiste un meccanismo pratico per aggiornare la UI dopo tick, fermarsi e segnalare blocco tecnico.

Regola:

```text
Preferire eventi pubblici già esistenti.
Se non esistono eventi o meccanismi pratici, non inventare polling.
```

---

## 9. Creazione struttura file

La struttura concreta deve seguire quella reale del repository.

### 9.1 File View

Creare o aggiornare:

```text
views/.../MainWindow.xaml
views/.../MainWindow.xaml.cs
```

Checklist:

* [ ] Riutilizzare la finestra esistente se presente.
* [ ] Non creare una seconda finestra principale duplicata.
* [ ] Non creare un nuovo progetto WPF se `views/` contiene già il progetto.
* [ ] Non creare cartelle alternative fuori standard.

### 9.2 File controllo numerico

Creare, se non esiste equivalente:

```text
views/.../Controls/NumericStepControl.xaml
views/.../Controls/NumericStepControl.xaml.cs
```

Checklist:

* [ ] Creare cartella `Controls/` solo se coerente con struttura reale.
* [ ] Non duplicare controlli esistenti equivalenti.
* [ ] Non usare TextBox libera per i tempi.

### 9.3 File ViewModel

Creare o aggiornare:

```text
view-models/.../MainTimerViewModel.cs
```

Checklist:

* [ ] Usare namespace reale del progetto ViewModel.
* [ ] Non collocare ViewModel dentro `views/`.
* [ ] Non collocare ViewModel dentro `services/CicloTimer.App/`.

### 9.4 File comandi

Creare, solo se non esiste già equivalente:

```text
view-models/.../RelayCommand.cs
```

Checklist:

* [ ] Riutilizzare `RelayCommand` esistente se presente.
* [ ] Non creare più implementazioni di comando.
* [ ] Non introdurre `RelayCommand` nei layer non UI.

### 9.5 File dispatcher UI

Creare, solo se necessario:

```text
view-models/.../IUiDispatcher.cs
view-models/.../UiDispatcherAdapter.cs
```

Checklist:

* [ ] Creare adapter solo nel layer presentazione.
* [ ] Non introdurre Dispatcher nei layer inferiori.
* [ ] Adapter deve usare marshalling asincrono.
* [ ] Adapter non deve conoscere Core, Bridge, Audio, Orchestrator o Runner.

### 9.6 File test

Creare o aggiornare test coerenti con struttura reale:

```text
tests/.../MainTimerViewModelTests.cs
tests/.../NumericStepControlTests.cs
tests/.../ProjectDependencyTests.cs
```

Checklist:

* [ ] Riutilizzare progetto test esistente se compatibile.
* [ ] Non creare nuovo progetto test se non necessario.
* [ ] Non duplicare test architetturali esistenti.

---

## 10. Implementazione `MainTimerViewModel`

Implementare o aggiornare `MainTimerViewModel`.

Checklist base:

* [ ] Classe pubblica o interna secondo convenzione reale.
* [ ] Implementa `INotifyPropertyChanged`.
* [ ] Non eredita da classi UI.
* [ ] Non usa `Window`.
* [ ] Non usa `MainWindow`.
* [ ] Non istanzia Core.
* [ ] Non istanzia Bridge.
* [ ] Non istanzia AudioService.
* [ ] Non istanzia TimerEngine.
* [ ] Non istanzia TimerBridgeAdapter.
* [ ] Non istanzia SystemActionDispatcher.
* [ ] Non usa timer propri.
* [ ] Non implementa polling.
* [ ] Non contiene logica ciclica manuale.

---

## 11. Costruttore `MainTimerViewModel`

Il ViewModel deve ricevere le dipendenze tramite costruttore, dove possibile.

Dipendenze concettuali:

```text
ITimerAppOrchestrator orchestrator
IRealtimeTimerRunner runner
localization provider
IUiDispatcher opzionale
```

Checklist:

* [ ] `orchestrator` obbligatorio.
* [ ] `runner` obbligatorio.
* [ ] localization obbligatoria se necessaria ai testi.
* [ ] dispatcher UI opzionale solo se serve.
* [ ] Dipendenze validate con `ArgumentNullException` dove opportuno.
* [ ] Non creare dipendenze applicative direttamente nel ViewModel.
* [ ] Se non esiste DI, comporre dipendenze in `MainWindow.xaml.cs` o `App.xaml.cs`.
* [ ] Documentare nel report finale la strategia di composizione usata.

---

## 12. Proprietà configurazione

Implementare proprietà bindabili:

```text
CycleMinutes
CycleSeconds
FinalAlertSeconds
```

Checklist `CycleMinutes`:

* [ ] Tipo numerico intero.
* [ ] Default 25, salvo default reale diverso.
* [ ] Valore minimo 0.
* [ ] Notifica `PropertyChanged`.
* [ ] Aggiorna validazione.
* [ ] Aggiorna comando principale.

Checklist `CycleSeconds`:

* [ ] Tipo numerico intero.
* [ ] Default 0.
* [ ] Range 0–59.
* [ ] Notifica `PropertyChanged`.
* [ ] Applica clamping o rifiuta valori fuori range.
* [ ] Aggiorna validazione.
* [ ] Aggiorna comando principale.

Checklist `FinalAlertSeconds`:

* [ ] Tipo numerico intero.
* [ ] Default 10, salvo default reale diverso.
* [ ] Valore minimo 0.
* [ ] Valore massimo consigliato 60, se coerente con contratti reali.
* [ ] Non può superare durata totale ciclo.
* [ ] Notifica `PropertyChanged`.
* [ ] Aggiorna validazione.
* [ ] Aggiorna comando principale.

---

## 13. Proprietà timer ciclico

Se supportato dai contratti reali, implementare:

```text
IsCyclic
```

Checklist se supportato:

* [ ] Tipo boolean.
* [ ] Default `false`.
* [ ] Notifica `PropertyChanged`.
* [ ] Bindata a CheckBox.
* [ ] Passata alla configurazione applicativa.
* [ ] Non modificata da Reset.
* [ ] Testata.

Checklist se non supportato:

* [ ] Non creare `IsCyclic`.
* [ ] Non creare CheckBox.
* [ ] Non creare binding vuoto.
* [ ] Non modificare Core/Bridge/Orchestrator.
* [ ] Report finale segnala esclusione.

---

## 14. Proprietà stato visuale

Implementare proprietà bindabili:

```text
RemainingTimeText
TimerStateText
CompletedSessionsText
PrimaryButtonText
```

Checklist:

* [ ] `RemainingTimeText` mostra tempo in formato coerente con modello display.
* [ ] `TimerStateText` mostra stato localizzato.
* [ ] `CompletedSessionsText` mostra sessioni completate.
* [ ] `PrimaryButtonText` mostra Avvia/Pausa/Riprendi/Avvia.
* [ ] Nessuna stringa hardcoded nel ViewModel.
* [ ] Tutti i testi provengono da localization o modello già localizzato.
* [ ] La View non formatta dati grezzi se il modello display li fornisce già.
* [ ] Ogni proprietà notifica `PropertyChanged`.

---

## 15. Proprietà validazione

Implementare proprietà bindabili:

```text
IsConfigurationValid
ValidationErrorText
```

Checklist:

* [ ] `IsConfigurationValid` è `true` solo per configurazione avviabile.
* [ ] `ValidationErrorText` è vuoto quando configurazione valida.
* [ ] `ValidationErrorText` contiene testo localizzato quando configurazione non valida.
* [ ] Durata zero produce configurazione non valida.
* [ ] Avviso finale maggiore della durata produce configurazione non valida.
* [ ] Valori fuori range producono configurazione non valida o sono corretti dal controllo.
* [ ] La validazione aggiorna `CanExecute` del comando principale.
* [ ] Il pulsante Avvia deve disabilitarsi tramite `CanExecute` quando `IsConfigurationValid` è `false`.
* [ ] Il comando Avvia non deve essere eseguibile quando la configurazione non è valida.
* [ ] L'errore dell'orchestratore resta una seconda difesa, non la prima validazione ordinaria.
* [ ] La validazione non duplica tutta la logica Core.
* [ ] Il Core resta autorità finale sulla configurazione.

---

## 16. Implementazione `RelayCommand`

Se non esiste un comando equivalente, implementare `RelayCommand`.

Checklist:

* [ ] Implementa `ICommand`.
* [ ] Accetta azione da eseguire.
* [ ] Accetta predicato `CanExecute`, se necessario.
* [ ] Espone `CanExecuteChanged`.
* [ ] Permette aggiornamento `CanExecute`.
* [ ] Non contiene logica timer.
* [ ] Non usa servizi applicativi.
* [ ] Vive nel layer `view-models/`.
* [ ] Non viene introdotto nei layer inferiori.

---

## 17. Implementazione comandi ViewModel

Implementare:

```text
PrimaryCommand
ResetCommand
```

Checklist `PrimaryCommand`:

* [ ] Esposto come `ICommand`.
* [ ] In stato fermo/configurabile esegue Avvia.
* [ ] In stato running esegue Pausa.
* [ ] In stato pausa esegue Riprendi.
* [ ] In stato completato esegue Avvia.
* [ ] Non esegue Avvia se `IsConfigurationValid` è `false`.
* [ ] Il relativo pulsante deve risultare disabilitato quando `CanExecute` è `false`.
* [ ] Aggiorna UI dopo ogni comando.
* [ ] Non contiene logica Core.

Checklist `ResetCommand`:

* [ ] Esposto come `ICommand`.
* [ ] Ferma runner se necessario.
* [ ] Chiama reset applicativo.
* [ ] Aggiorna UI.
* [ ] Non cancella durata scelta dall'utente.
* [ ] Non modifica `IsCyclic`, se presente.
* [ ] Non ricrea Core/Bridge.
* [ ] Non controlla AudioService direttamente.

---

## 18. Comportamento Avvia

Implementare azione Avvia.

Checklist:

* [ ] Validare configurazione.
* [ ] `PrimaryCommand.CanExecute` deve essere `false` quando la configurazione non è valida.
* [ ] Il pulsante Avvia deve essere disabilitato quando `IsConfigurationValid` è `false`.
* [ ] Se configurazione non valida, non proseguire.
* [ ] Costruire `TimerInput` o configurazione reale equivalente.
* [ ] Includere durata ciclo.
* [ ] Includere durata avviso finale.
* [ ] Includere flag ciclico solo se supportato.
* [ ] Chiamare orchestratore per configurare il timer.
* [ ] Chiamare orchestratore per avviare.
* [ ] Avviare runner.
* [ ] Aggiornare display.
* [ ] Aggiornare `PrimaryButtonText`.
* [ ] Aggiornare `CanExecute`.

Se l'orchestratore restituisce errore:

* [ ] Non avviare runner.
* [ ] Mostrare errore localizzato.
* [ ] Lasciare UI in stato configurabile.

---

## 19. Comportamento Pausa

Implementare azione Pausa.

Checklist:

* [ ] Chiamare `orchestrator.Pause()` o metodo reale equivalente.
* [ ] Fermare runner.
* [ ] Aggiornare display.
* [ ] Aggiornare `PrimaryButtonText` a Riprendi.
* [ ] Non resettare durata.
* [ ] Non resettare sessioni.
* [ ] Non modificare configurazione utente.

---

## 20. Comportamento Riprendi

Implementare azione Riprendi.

Checklist:

* [ ] Chiamare `orchestrator.Resume()` o metodo reale equivalente.
* [ ] Avviare runner.
* [ ] Aggiornare display.
* [ ] Aggiornare `PrimaryButtonText` a Pausa.
* [ ] Non ricreare configurazione se non necessario.
* [ ] Non perdere tempo residuo.

---

## 21. Comportamento Reset

Implementare azione Reset.

Checklist:

* [ ] Fermare runner.
* [ ] Chiamare `orchestrator.Reset()` o metodo reale equivalente.
* [ ] Aggiornare display.
* [ ] Riportare `PrimaryButtonText` ad Avvia.
* [ ] Non cancellare `CycleMinutes`.
* [ ] Non cancellare `CycleSeconds`.
* [ ] Non cancellare `FinalAlertSeconds`.
* [ ] Non modificare `IsCyclic`, se presente.
* [ ] Aggiornare validazione.
* [ ] Aggiornare comandi.

---

## 22. Aggiornamento dopo tick

Implementare aggiornamento UI dopo tick solo se esiste meccanismo pratico già supportato.

Checklist:

* [ ] Usare evento pubblico già esistente dell'orchestratore, se presente.
* [ ] Usare stato pubblico `CurrentState` dopo notifica, se previsto.
* [ ] Non introdurre polling.
* [ ] Non introdurre timer UI parallelo.
* [ ] Non modificare runner.
* [ ] Non modificare orchestratore.
* [ ] Se non esiste meccanismo, fermarsi e segnalare blocco tecnico.

Se si usa evento:

* [ ] Sottoscrivere evento nel ViewModel.
* [ ] Aggiornare proprietà tramite dispatcher UI asincrono.
* [ ] Rimuovere sottoscrizione in cleanup.
* [ ] Usare `-=` per rimuovere ogni sottoscrizione agli eventi dell'orchestratore.
* [ ] Testare che il cleanup rimuova sottoscrizione.
* [ ] Evitare memory leak causati da riferimenti forti dell'orchestratore verso il ViewModel.

---

## 23. Marshalling UI asincrono

Se gli aggiornamenti arrivano da thread non UI, implementare marshalling asincrono.

Checklist:

* [ ] Usare `Dispatcher.BeginInvoke`, `SynchronizationContext.Post` o adapter equivalente.
* [ ] Non usare `Dispatcher.Invoke`.
* [ ] Non bloccare thread runner.
* [ ] Non mettere Dispatcher in Core.
* [ ] Non mettere Dispatcher in Bridge.
* [ ] Non mettere Dispatcher in Audio.
* [ ] Non mettere Dispatcher in Orchestrator.
* [ ] Non mettere Dispatcher in Runner.
* [ ] Se creato adapter, collocarlo in `view-models/`.

---

## 24. Implementazione `IUiDispatcher`

Se necessario, implementare:

```text
IUiDispatcher
UiDispatcherAdapter
```

Checklist `IUiDispatcher`:

* [ ] Vive nel layer `view-models/`.
* [ ] Espone metodo concettuale `Post(Action action)` o equivalente.
* [ ] Non conosce Core.
* [ ] Non conosce Bridge.
* [ ] Non conosce Audio.
* [ ] Non conosce Orchestrator.
* [ ] Non conosce Runner.

Checklist `UiDispatcherAdapter`:

* [ ] Usa dispatcher WPF o synchronization context.
* [ ] Usa marshalling asincrono.
* [ ] Non usa `Dispatcher.Invoke`.
* [ ] Non blocca il chiamante.
* [ ] Gestisce chiamate già sul thread UI, se necessario, senza bloccare.

---

## 25. Implementazione `NumericStepControl`

Implementare controllo numerico guidato se non esiste equivalente.

Checklist base:

* [ ] UserControl WPF o controllo equivalente.
* [ ] Mostra etichetta.
* [ ] Mostra valore numerico.
* [ ] Il valore numerico deve essere mostrato con `TextBlock`, `Label` o controllo non editabile equivalente.
* [ ] Non usare `TextBox`, nemmeno `TextBox` in sola lettura.
* [ ] Pulsante decremento.
* [ ] Pulsante incremento.
* [ ] Non usa TextBox libera.
* [ ] Non accetta testo arbitrario.
* [ ] Non contiene logica timer.
* [ ] Non conosce orchestratore.
* [ ] Non conosce runner.
* [ ] Non conosce altri NumericStepControl.

Checklist DependencyProperty:

* [ ] `Value` è `DependencyProperty`.
* [ ] `Minimum` è `DependencyProperty`.
* [ ] `Maximum` è `DependencyProperty`.
* [ ] `Step` è `DependencyProperty`.
* [ ] `Label` è `DependencyProperty` o testo bindato equivalente.
* [ ] `Value` supporta binding `TwoWay`.
* [ ] Registrare `Value` con `FrameworkPropertyMetadataOptions.BindsTwoWayByDefault`, se coerente con lo stile WPF del progetto.
* [ ] In alternativa, forzare esplicitamente `Mode=TwoWay` nello XAML di consumo.
* [ ] Verificare che il valore modificato dai pulsanti `+` e `-` aggiorni realmente il ViewModel.
* [ ] Non affidarsi a sincronizzazioni procedurali manuali per aggiornare il ViewModel.

Checklist clamping:

* [ ] Click `+` incrementa di `Step`.
* [ ] Click `-` decrementa di `Step`.
* [ ] Se valore supera massimo, resta massimo.
* [ ] Se valore scende sotto minimo, resta minimo.
* [ ] Secondi a 59 + click `+` resta 59.
* [ ] Secondi a 0 + click `-` resta 0.
* [ ] Nessun overflow automatico minuti/secondi.

---

## 26. Implementazione `MainWindow.xaml`

Implementare layout a card.

Checklist finestra:

* [ ] Finestra avviabile.
* [ ] Dimensione compatta.
* [ ] Tema chiaro pulito.
* [ ] Titolo applicazione.
* [ ] Card configurazione.
* [ ] Card timer.
* [ ] Card comandi.
* [ ] Card stato.
* [ ] Nessuna UI eccessivamente complessa.
* [ ] Nessun tema scuro.
* [ ] Nessuna animazione avanzata.
* [ ] Nessuna icona definitiva obbligatoria.

Checklist card configurazione:

* [ ] Controllo minuti.
* [ ] Controllo secondi.
* [ ] Controllo avviso finale.
* [ ] CheckBox ciclica solo se supportata.
* [ ] Etichette visibili.

Checklist card timer:

* [ ] Tempo rimanente grande.
* [ ] Posizione centrale o fortemente evidenziata.
* [ ] Binding a `RemainingTimeText`.

Checklist card comandi:

* [ ] Pulsante principale.
* [ ] Binding testo a `PrimaryButtonText`.
* [ ] Binding comando a `PrimaryCommand`.
* [ ] Il pulsante principale deve disabilitarsi automaticamente quando `PrimaryCommand.CanExecute` è `false`.
* [ ] Pulsante Reset.
* [ ] Binding comando a `ResetCommand`.

Checklist card stato:

* [ ] Stato corrente.
* [ ] Sessioni completate.
* [ ] Messaggio errore validazione.
* [ ] Errore visibile tramite binding XAML.

---

## 27. Gestione errori in XAML

Implementare visualizzazione errore configurazione tramite binding.

Checklist:

* [ ] Binding a `ValidationErrorText` o equivalente.
* [ ] Visibilità gestita da binding.
* [ ] Ammesso `BooleanToVisibilityConverter` o converter equivalente.
* [ ] Vietata manipolazione procedurale dal code-behind.
* [ ] Errore non mostra stack trace.
* [ ] Errore usa testo localizzato.

---

## 28. Localization

Aggiornare localization solo se necessario.

Verificare chiavi esistenti.

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

Checklist:

* [ ] Non duplicare chiavi esistenti.
* [ ] Aggiungere solo chiavi mancanti.
* [ ] Non introdurre stringhe utente hardcoded in C#.
* [ ] Evitare stringhe definitive hardcoded in XAML se contrario allo standard progetto.
* [ ] Esporre testi dal ViewModel se necessario.
* [ ] Aggiornare test localization se presenti.
* [ ] Se timer ciclico non supportato, non aggiungere chiave non usata per CheckBox ciclica salvo scelta coerente.

---

## 29. Code-behind `MainWindow`

Mantenere code-behind minimale.

Checklist consentita:

* [ ] `InitializeComponent`.
* [ ] Assegnazione `DataContext`.
* [ ] Composizione minima dipendenze se DI assente.
* [ ] Cleanup del ViewModel alla chiusura finestra.

Checklist vietata:

* [ ] Nessuna logica timer.
* [ ] Nessun calcolo tempo.
* [ ] Nessuna gestione click manuale se comando disponibile.
* [ ] Nessuna manipolazione errore.
* [ ] Nessun controllo diretto audio.
* [ ] Nessuna logica ciclica.
* [ ] Nessuna stringa utente hardcoded.

---

## 30. Cleanup chiusura finestra

Implementare cleanup sicuro alla chiusura finestra.

Checklist:

* [ ] Gestire l'evento `Closed` della `MainWindow`.
* [ ] Se necessario, usare anche `Closing`, ma il cleanup effettivo deve avvenire in modo idempotente.
* [ ] Se `DataContext` implementa `IDisposable`, chiamare `Dispose()`.
* [ ] Se il ViewModel espone metodo di cleanup equivalente, chiamarlo.
* [ ] Il ViewModel ferma o dispone runner secondo proprietà reale dell'istanza.
* [ ] Il ViewModel rimuove eventuali sottoscrizioni a eventi dell'orchestratore usando `-=`.
* [ ] Nessuna sottoscrizione a eventi dell'orchestratore deve restare attiva dopo la chiusura.
* [ ] Nessun loop runner resta attivo dopo chiusura finestra.
* [ ] Cleanup non contiene logica business.
* [ ] Cleanup non causa eccezioni se chiamato più volte.
* [ ] Testare cleanup se possibile.

---

## 31. Test ViewModel

Creare test per `MainTimerViewModel`.

Checklist stato iniziale:

* [ ] Valori default corretti.
* [ ] `CycleMinutes` default.
* [ ] `CycleSeconds` default.
* [ ] `FinalAlertSeconds` default.
* [ ] `IsConfigurationValid` true con default valido.
* [ ] `ValidationErrorText` vuoto con default valido.
* [ ] `PrimaryButtonText` iniziale Avvia.
* [ ] Stato iniziale coerente.

Checklist validazione:

* [ ] Durata zero non valida.
* [ ] Avviso finale maggiore durata non valido.
* [ ] Valori fuori range non ammessi o corretti.
* [ ] `ValidationErrorText` valorizzato su errore.
* [ ] `CanExecute` aggiornato quando cambia validazione.
* [ ] `PrimaryCommand.CanExecute` è `false` quando `IsConfigurationValid` è `false`.
* [ ] Avvia non viene eseguito se `CanExecute` è `false`.

Checklist comandi:

* [ ] Avvia configura orchestratore.
* [ ] Avvia chiama start orchestratore.
* [ ] Avvia avvia runner.
* [ ] Avvia non parte con configurazione non valida.
* [ ] Pausa chiama orchestratore e ferma runner.
* [ ] Riprendi chiama orchestratore e avvia runner.
* [ ] Reset ferma runner e chiama reset.
* [ ] Reset non cancella configurazione utente.
* [ ] Stato completato porta pulsante ad Avvia.

Checklist proprietà:

* [ ] `PropertyChanged` emesso per proprietà rilevanti.
* [ ] `RemainingTimeText` aggiornato.
* [ ] `TimerStateText` aggiornato.
* [ ] `CompletedSessionsText` aggiornato.
* [ ] `PrimaryButtonText` aggiornato.

Checklist timer ciclico se supportato:

* [ ] `IsCyclic` default false.
* [ ] `IsCyclic` passato alla configurazione.
* [ ] Reset non modifica `IsCyclic`.

Checklist timer ciclico se non supportato:

* [ ] `IsCyclic` non esposto.
* [ ] Nessun test richiede CheckBox ciclica.
* [ ] Report finale segnala esclusione.

Checklist cleanup:

* [ ] Dispose/cleanup ferma runner.
* [ ] Dispose/cleanup rimuove sottoscrizioni usando `-=`.
* [ ] Dispose/cleanup è idempotente.
* [ ] Dopo cleanup, eventi dell'orchestratore non trattengono più il ViewModel.

---

## 32. Test `NumericStepControl`

Se il progetto supporta test WPF o test del controllo, implementare test.

Checklist:

* [ ] Valore iniziale.
* [ ] Incremento.
* [ ] Decremento.
* [ ] Rispetto minimo.
* [ ] Rispetto massimo.
* [ ] Clamping rigido.
* [ ] Nessun overflow verso altri controlli.
* [ ] `Value` è DependencyProperty.
* [ ] `Value` usa `BindsTwoWayByDefault` oppure il binding di consumo forza `Mode=TwoWay`.
* [ ] Modifica del valore aggiorna il ViewModel.
* [ ] Binding TwoWay supportato.
* [ ] Nessuna TextBox libera.
* [ ] Nessuna TextBox readonly usata per mostrare il valore.
* [ ] Valore mostrato con `TextBlock`, `Label` o controllo non editabile equivalente.

Se non sono presenti test UI/XAML:

* [ ] Documentare verifiche manuali nel report finale.
* [ ] Non introdurre infrastruttura test UI complessa fuori perimetro.

---

## 33. Test architetturali

Aggiungere o aggiornare test architetturali.

Verificare che `views/` non contenga:

```text
TimerEngine
TimerBridgeAdapter
SystemActionDispatcher
AudioService
CicloTimer.Core
```

Verificare che `view-models/` non contenga:

```text
TimerEngine
TimerBridgeAdapter
SystemActionDispatcher
AudioService
CicloTimer.Core
```

Verificare che i layer vietati non contengano:

```text
INotifyPropertyChanged
ICommand
RelayCommand
```

nei percorsi:

```text
models/CicloTimer.Core/
services/CicloTimer.App/
services/CicloTimer.App/Timing/
services/CicloTimer.Audio/
locales/CicloTimer.Localization/
```

Verificare che non siano usati per aggiornare la UI:

```text
Dispatcher.Invoke
DispatcherTimer
System.Timers.Timer
System.Threading.Timer
Task.Delay come timer UI parallelo
```

Sono ammessi nel layer presentazione:

```text
Dispatcher.BeginInvoke
SynchronizationContext.Post
adapter equivalente
```

---

## 34. Test localization

Se vengono aggiunte chiavi localization:

* [ ] Verificare presenza chiave titolo app.
* [ ] Verificare presenza chiave durata ciclo.
* [ ] Verificare presenza chiave minuti.
* [ ] Verificare presenza chiave secondi.
* [ ] Verificare presenza chiave avviso finale.
* [ ] Verificare presenza chiave avvia.
* [ ] Verificare presenza chiave pausa.
* [ ] Verificare presenza chiave riprendi.
* [ ] Verificare presenza chiave reset.
* [ ] Verificare presenza chiave stato.
* [ ] Verificare presenza chiave sessioni completate.
* [ ] Verificare presenza chiave errore durata zero.
* [ ] Verificare presenza chiave errore avviso finale maggiore durata.
* [ ] Verificare chiave timer ciclico solo se usata.
* [ ] Verificare che nessuna chiave necessaria sia vuota.

---

## 35. Verifiche manuali UI

Dopo implementazione, eseguire verifica manuale.

Checklist:

* [ ] App avviabile.
* [ ] Finestra visualizzata.
* [ ] Layout a card visibile.
* [ ] Tema chiaro pulito.
* [ ] Finestra compatta.
* [ ] Tempo grande centrale.
* [ ] Incremento minuti funziona.
* [ ] Decremento minuti funziona.
* [ ] Incremento secondi funziona.
* [ ] Decremento secondi funziona.
* [ ] Secondi non superano 59.
* [ ] Secondi non scendono sotto 0.
* [ ] Il valore numerico non è editabile come testo.
* [ ] Nessun cursore di inserimento testo compare sui valori numerici.
* [ ] Avviso finale configurabile.
* [ ] Avviso finale non supera durata ciclo.
* [ ] Pulsante Avvia disabilitato quando configurazione non valida.
* [ ] CheckBox ciclica visibile solo se supportata.
* [ ] Avvia funziona.
* [ ] Pausa funziona.
* [ ] Riprendi funziona.
* [ ] Reset funziona.
* [ ] Stato visibile.
* [ ] Sessioni completate visibili.
* [ ] Errore configurazione visibile.
* [ ] Errore configurazione sparisce quando configurazione torna valida.
* [ ] Nessun crash durante tick.
* [ ] Nessun freeze evidente.
* [ ] Chiusura finestra non lascia runner attivo.
* [ ] Chiusura finestra rimuove sottoscrizioni agli eventi.
* [ ] Nessun test avanzato NVDA eseguito in questo blocco.

---

## 36. Build finale

Eseguire:

```bash
dotnet build
```

Checklist:

* [ ] Build completata.
* [ ] Nessun errore.
* [ ] Nessun warning nuovo rilevante.
* [ ] Progetti 001–006 ancora compilano.
* [ ] Progetto UI compila.
* [ ] Progetto ViewModel compila.

---

## 37. Test finale

Eseguire:

```bash
dotnet test
```

Checklist:

* [ ] Test completati.
* [ ] Test nuovi passano.
* [ ] Test 001–006 passano.
* [ ] Test localization passano.
* [ ] Test architetturali passano.
* [ ] Nessuna regressione.

---

## 38. Pulizia finale

Prima di chiudere il blocco, verificare:

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

Checklist:

* [ ] Nessun file temporaneo.
* [ ] Nessun file fantasma.
* [ ] Nessun file duplicato inutile.
* [ ] Nessun artefatto `bin/` o `obj/` tracciato.
* [ ] Nessun using inutilizzato evidente.
* [ ] Nessun namespace morto evidente.
* [ ] Nessuna modifica fuori perimetro.
* [ ] Nessun progetto creato senza autorizzazione.

---

## 39. Git status finale

Eseguire:

```bash
git status
```

Checklist:

* [ ] Verificare elenco file modificati.
* [ ] Verificare che i file siano coerenti con blocco 007.
* [ ] Verificare assenza artefatti generati.
* [ ] Verificare assenza file fantasma.
* [ ] Non eseguire commit.
* [ ] Non eseguire push.

---

## 40. File attesi a fine blocco

I file esatti dipendono dalla struttura reale, ma a fine blocco sono attesi file equivalenti a:

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
tests/.../ProjectDependencyTests.cs
```

Possibili file aggiuntivi:

```text
tests/.../NumericStepControlTests.cs
locales/... file testi aggiornato
```

solo se coerenti con struttura reale.

Se un file non viene creato perché esiste già equivalente:

* [ ] Segnalarlo nel report finale.
* [ ] Riutilizzare il file esistente.
* [ ] Non duplicare responsabilità.

---

## 41. Criteri di completamento

Il blocco 007 è completato solo se:

* [ ] Esiste una finestra WPF avviabile.
* [ ] La UI vive in `views/`.
* [ ] Il ViewModel vive in `view-models/`.
* [ ] La finestra usa layout a card.
* [ ] La finestra è compatta.
* [ ] Il tema è chiaro e pulito.
* [ ] La durata ciclo usa controlli guidati.
* [ ] L'avviso finale usa controlli guidati.
* [ ] Non esistono TextBox libere per i tempi.
* [ ] Non esistono TextBox readonly per mostrare i valori numerici.
* [ ] Il timer ciclico è configurabile solo se supportato.
* [ ] Se timer ciclico non supportato, UI resta a ciclo singolo.
* [ ] `NumericStepControl` usa clamping rigido.
* [ ] `NumericStepControl.Value` supporta binding TwoWay reale.
* [ ] `Value` usa `BindsTwoWayByDefault` oppure binding XAML esplicito `Mode=TwoWay`.
* [ ] Il tempo rimanente è grande e centrale.
* [ ] Il pulsante principale è dinamico.
* [ ] Il pulsante Avvia è disabilitato quando configurazione non valida.
* [ ] In stato completato il pulsante principale torna ad Avvia.
* [ ] Reset è separato.
* [ ] Stato timer visibile.
* [ ] Sessioni completate visibili.
* [ ] `IsConfigurationValid` o equivalente esiste.
* [ ] `ValidationErrorText` o equivalente esiste.
* [ ] Errore visibile tramite binding XAML.
* [ ] Start funziona.
* [ ] Pause funziona.
* [ ] Resume funziona.
* [ ] Reset funziona.
* [ ] Runner avviato e fermato in modo coerente.
* [ ] Marshalling UI asincrono se necessario.
* [ ] Nessun Dispatcher nei layer inferiori.
* [ ] Nessun `Dispatcher.Invoke` per aggiornamenti ordinari timer.
* [ ] Nessun polling automatico countdown.
* [ ] Nessun timer UI parallelo countdown.
* [ ] Cleanup chiusura finestra presente.
* [ ] Cleanup rimuove sottoscrizioni agli eventi con `-=`.
* [ ] Nessuna logica Core nella UI.
* [ ] Nessun controllo diretto audio nella UI.
* [ ] Nessun calcolo diretto tempo reale nella UI.
* [ ] Testi utente centralizzati.
* [ ] `INotifyPropertyChanged` e `ICommand` confinati al layer presentazione.
* [ ] Accessibilità avanzata non anticipata.
* [ ] Build passa.
* [ ] Test passano.
* [ ] Working tree non contiene file fantasma o artefatti generati.

---

## 42. Fuori perimetro finale

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

Questi aspetti appartengono a blocchi successivi.

Il blocco successivo naturale è:

```text
008 — Accessibilità UI e rifinitura interazione
```

---

## 43. Report finale richiesto

Al termine della codifica, produrre un report con:

* [ ] File creati.
* [ ] File modificati.
* [ ] File riutilizzati.
* [ ] File non creati perché già esistenti.
* [ ] Decisioni implementative adottate.
* [ ] Eventuali scostamenti dal Design 007.
* [ ] Eventuali scostamenti dal Coding Plan 007.
* [ ] Esito verifica supporto timer ciclico.
* [ ] Decisione finale sul timer ciclico.
* [ ] Strategia localization usata.
* [ ] Strategia marshalling UI usata.
* [ ] Strategia aggiornamento dopo tick.
* [ ] Strategia cleanup chiusura finestra.
* [ ] Conferma rimozione sottoscrizioni evento con `-=`, se eventi usati.
* [ ] Conferma binding TwoWay reale del `NumericStepControl.Value`.
* [ ] Esito build.
* [ ] Esito test.
* [ ] Esito git status.
* [ ] Problemi residui.
* [ ] Conferma assenza artefatti `bin/` e `obj/`.
* [ ] Conferma assenza file fantasma.
* [ ] Conferma che non sono stati eseguiti commit o push.

---

## 44. Esito atteso

Alla fine del blocco 007 l'applicazione CicloTimer deve avere una prima UI WPF minima e funzionante.

L'utente deve poter:

```text
configurare durata ciclo
configurare avviso finale
avviare
mettere in pausa
riprendere
resettare
vedere countdown
vedere stato
vedere sessioni completate
```

Se il timer ciclico è supportato dai contratti reali, l'utente deve poter scegliere anche:

```text
ripeti automaticamente il ciclo
```

Se il timer ciclico non è supportato dai contratti reali, la UI deve restare a ciclo singolo e il report finale deve segnalarlo.

Il risultato non deve essere la UI definitiva.

Deve essere una prima interfaccia pulita, compatta, corretta e pronta per il blocco 008.
