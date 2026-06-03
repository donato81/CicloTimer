# CicloTimer — Design 005 — Orchestratore applicativo timer

**Tipo documento:** documento di design  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/1-design/004-design-audio-service-e-audio-focus.md, docs/2-coding-plans/002-coding-plan-bridge-ui-logica-timer.md, docs/2-coding-plans/004-coding-plan-audio-service-e-audio-focus.md, docs/3-todos/002-todo-bridge-ui-logica-timer.md, docs/3-todos/004-todo-audio-service-e-audio-focus.md  

---

## 1. Scopo del documento

Questo documento definisce il design dell’orchestratore applicativo del timer di CicloTimer.

I documenti precedenti hanno già definito e implementato blocchi separati:

```text
Design 001 → Core timer engine
Design 003 → Sistema centralizzato testi
Design 002 → Bridge UI-logica timer
Design 004 → Audio service e audio focus
````

Il core gestisce la logica del timer.

Il sistema localization contiene testi centralizzati.

Il bridge trasforma stati, eventi ed errori in un modello mostrabile e produce richieste concettuali di sistema.

Il servizio audio esegue le richieste audio, come avvio e stop dell’avviso finale.

Questo Design 005 definisce il componente che coordina questi blocchi già esistenti.

L’orchestratore applicativo ha il compito di:

1. ricevere comandi applicativi;
2. inviare i comandi al bridge;
3. conservare l’ultimo modello mostrabile prodotto dal bridge;
4. leggere le azioni di sistema prodotte dal bridge;
5. chiamare il servizio audio quando necessario;
6. trattare gli errori audio come errori tecnici non bloccanti;
7. esporre uno stato applicativo corrente alla futura UI;
8. ricevere tick da un futuro gestore del timer reale;
9. restare separato dalla UI WPF;
10. restare separato dal timer reale;
11. non modificare core, bridge, localization o audio.

Questo documento non definisce ancora:

1. timer reale;
2. sorgente periodica dei tick;
3. `DispatcherTimer`;
4. UI WPF;
5. XAML;
6. binding;
7. `INotifyPropertyChanged`;
8. `ICommand`;
9. NVDA operativo;
10. UI Automation;
11. Live Region;
12. layout grafico;
13. gestione focus tastiera;
14. ciclo completo di interazione nella finestra.

Questi aspetti saranno trattati nei design successivi.

---

## 2. Obiettivo del design

L’obiettivo è introdurre un componente applicativo che faccia da coordinatore tra:

```text
Bridge
AudioService
```

L’orchestratore non deve sostituire il bridge.

L’orchestratore non deve sostituire il core.

L’orchestratore non deve sostituire il servizio audio.

L’orchestratore non deve sostituire la futura UI.

L’orchestratore deve solo coordinare.

Flusso concettuale:

```text
Comando applicativo
↓
Orchestratore
↓
Bridge
↓
Core + Localization
↓
Bridge update
↓
Orchestratore
↓
SystemActions
↓
AudioService
↓
Stato applicativo corrente aggiornato
```

Esempio pratico:

```text
Start
↓
Orchestratore riceve Start
↓
chiama il bridge
↓
il bridge aggiorna il core e produce un modello mostrabile
↓
l’orchestratore salva quel modello
↓
se il bridge produce azioni audio, l’orchestratore chiama AudioService
```

---

## 3. Principio guida

Il principio guida è:

```text
l’orchestratore coordina, non decide la logica del timer
```

Il core decide:

1. se il timer può partire;
2. se il timer può andare in pausa;
3. se il timer può riprendere;
4. se il timer può essere resettato;
5. quando entrare nella finestra finale;
6. quando completare una sessione;
7. quando incrementare il contatore;
8. quando ripartire con una nuova sessione.

Il bridge decide:

1. come trasformare dati neutri in modello mostrabile;
2. quali testi centralizzati usare;
3. quali azioni di sistema concettuali esporre;
4. quando produrre `StartFinalAlertSound`;
5. quando produrre `StopFinalAlertSound`.

Il servizio audio decide:

1. come avviare il suono;
2. come fermare il suono;
3. come gestire il file audio;
4. come trattare errori audio;
5. come restituire risultati tecnici.

L’orchestratore decide solo:

1. quale comando applicativo inoltrare;
2. quando aggiornare il proprio stato corrente;
3. quando eseguire le azioni di sistema ricevute dal bridge;
4. come conservare l’esito tecnico delle azioni audio;
5. come restare in stato sicuro se l’audio fallisce;
6. come chiudersi in modo sicuro fermando eventuale audio attivo.

---

## 4. Collocazione fisica

L’orchestratore deve essere collocato in:

```text
services/CicloTimer.App/
```

Il file progetto deve essere:

```text
services/CicloTimer.App/CicloTimer.App.csproj
```

Il nome del progetto deve essere:

```text
CicloTimer.App
```

Motivo:

```text
l’orchestratore è un servizio applicativo di coordinamento
```

Non deve stare in:

```text
models/
locales/
view-models/
services/CicloTimer.Audio/
progetto WPF root
src/
orchestrators/
```

Struttura prevista:

```text
services/
  CicloTimer.Audio/
  CicloTimer.App/
```

I test futuri dell’orchestratore dovranno stare in:

```text
tests/CicloTimer.App.Tests/
```

Questo design non crea ancora il coding plan né il TODO, ma fissa il percorso.

---

## 5. Dipendenze autorizzate

Il progetto `CicloTimer.App` può dipendere da:

```text
view-models/CicloTimer.Bridge/
services/CicloTimer.Audio/
```

Quindi:

```text
CicloTimer.App
↓
CicloTimer.Bridge

CicloTimer.App
↓
CicloTimer.Audio
```

Questa dipendenza è coerente perché l’orchestratore deve:

1. inviare comandi applicativi al bridge;
2. leggere gli aggiornamenti prodotti dal bridge;
3. leggere le azioni di sistema prodotte dal bridge;
4. chiamare il servizio audio.

Dove possibile, l’orchestratore deve dipendere da astrazioni/interfacce e non da classi concrete.

Esempi concettuali:

```text
ITimerBridge
IAudioService
```

I nomi esatti saranno definiti nel coding plan in base alle API già presenti nei progetti `CicloTimer.Bridge` e `CicloTimer.Audio`.

Il principio vincolante è:

```text
l’orchestratore deve essere testabile con bridge finto e audio finto
```

---

## 6. Dipendenze vietate

Il progetto `CicloTimer.App` non deve dipendere direttamente da:

```text
models/CicloTimer.Core/
locales/CicloTimer.Localization/
progetto WPF root
MainWindow
XAML
DispatcherTimer
NVDA
UI Automation
Live Region
```

### 6.1 Divieto dipendenza diretta dal core

L’orchestratore non deve chiamare direttamente il core.

Motivo:

```text
il bridge è già il punto di contatto tra logica neutra e modello mostrabile
```

Flusso corretto:

```text
Orchestratore
↓
Bridge
↓
Core
```

Flusso vietato:

```text
Orchestratore
↓
Core
```

### 6.2 Divieto dipendenza diretta da localization

L’orchestratore non deve chiamare direttamente localization.

Motivo:

```text
la costruzione dei testi è responsabilità del bridge
```

Flusso corretto:

```text
Orchestratore
↓
Bridge
↓
Localization
```

Flusso vietato:

```text
Orchestratore
↓
Localization
```

### 6.3 Divieto dipendenza da UI

L’orchestratore non deve conoscere:

1. WPF;
2. XAML;
3. `MainWindow`;
4. controlli grafici;
5. binding;
6. `INotifyPropertyChanged`;
7. `ICommand`.

La futura UI chiamerà l’orchestratore, ma l’orchestratore non deve chiamare la UI.

---

## 7. Responsabilità autorizzate

L’orchestratore può:

1. ricevere un’istanza o astrazione del bridge;
2. ricevere un’istanza o astrazione del servizio audio;
3. esporre comandi applicativi;
4. ricevere configurazione timer;
5. inoltrare configurazione al bridge;
6. inoltrare start al bridge;
7. inoltrare pausa al bridge;
8. inoltrare ripresa al bridge;
9. inoltrare reset al bridge;
10. inoltrare tick al bridge;
11. ricevere aggiornamento dal bridge;
12. salvare ultimo modello mostrabile;
13. salvare ultimo risultato applicativo;
14. salvare ultimo risultato tecnico audio;
15. leggere `SystemActions` prodotte dal bridge;
16. chiamare `AudioService.StartFinalAlertSound`;
17. chiamare `AudioService.StopFinalAlertSound`;
18. leggere il risultato composito `AudioServiceResult`;
19. trattare un fallimento parziale dell’audio come non bloccante;
20. ignorare in modo sicuro azioni sconosciute non previste, se mai presenti;
21. restare coerente se l’audio fallisce;
22. restare sicuro con comandi ravvicinati;
23. restituire un risultato applicativo non testuale;
24. tentare stop audio sicuro in fase di chiusura;
25. essere testato senza UI.

---

## 8. Responsabilità vietate

L’orchestratore non deve:

1. calcolare direttamente il tempo rimanente;
2. decidere quando entrare in `FinalAlert`;
3. decidere quando una sessione è completata;
4. incrementare direttamente il contatore sessioni;
5. generare direttamente eventi core;
6. costruire direttamente testi utente;
7. accedere direttamente a localization;
8. riprodurre direttamente file audio;
9. manipolare direttamente `SoundPlayer`;
10. generare `final-alert.wav`;
11. gestire audio focus direttamente;
12. creare timer reale;
13. usare `DispatcherTimer`;
14. usare `System.Timers.Timer`;
15. usare `System.Threading.Timer`;
16. usare `Task.Delay` in loop;
17. aprire finestre;
18. leggere o scrivere controlli WPF;
19. implementare `ICommand`;
20. implementare `INotifyPropertyChanged`;
21. gestire NVDA;
22. gestire UI Automation;
23. creare Live Region;
24. fare binding;
25. salvare preferenze;
26. persistere dati;
27. accedere a file di configurazione utente;
28. usare database;
29. usare cloud;
30. creare cartella `src/`;
31. creare cartella `orchestrators/`;
32. dipendere direttamente da core;
33. dipendere direttamente da localization.

---

## 9. Comandi applicativi previsti

L’orchestratore deve esporre comandi applicativi concettuali.

Comandi previsti:

```text
Configure
Start
Pause
Resume
Reset
Tick
Shutdown/Dispose
```

Questi comandi non sono comandi UI WPF.

Non sono `ICommand`.

Sono metodi applicativi semplici.

Esempio concettuale:

```text
Configure(...)
Start()
Pause()
Resume()
Reset()
Tick(elapsedSeconds)
Dispose()
```

Il coding plan definirà firme concrete.

Un comando esplicito `Stop`, distinto da `Reset`, resta fuori perimetro della prima versione.

Motivo:

```text
per la prima versione Reset copre il ritorno allo stato iniziale configurato
```

Un futuro comando `Stop` potrà essere progettato in un design successivo se emergerà una necessità funzionale distinta.

---

## 10. Configure

Il comando `Configure` serve a configurare il timer.

L’orchestratore deve:

1. ricevere una configurazione applicativa;
2. inoltrarla al bridge;
3. ricevere un aggiornamento dal bridge;
4. salvare lo stato corrente mostrabile;
5. eseguire eventuali azioni di sistema prodotte dal bridge, se presenti.

L’orchestratore non deve validare direttamente le durate.

La validazione resta responsabilità del core, mediata dal bridge.

Esempio:

```text
Configure durata sessione 300 secondi, avviso finale 20 secondi
↓
Orchestratore inoltra al bridge
↓
Bridge/Core validano
↓
Orchestratore conserva il nuovo modello mostrabile
```

---

## 11. Start

Il comando `Start` serve ad avviare il timer.

L’orchestratore deve:

1. inoltrare `Start` al bridge;
2. ricevere l’aggiornamento;
3. salvare ultimo modello mostrabile;
4. eseguire eventuali azioni di sistema.

L’orchestratore non deve decidere se `Start` è consentito.

La disponibilità del comando resta nel core/bridge.

Se `Start` non è consentito, il bridge produrrà un aggiornamento coerente.

---

## 12. Pause

Il comando `Pause` serve a mettere in pausa il timer.

L’orchestratore deve:

1. inoltrare `Pause` al bridge;
2. ricevere l’aggiornamento;
3. salvare ultimo modello mostrabile;
4. eseguire eventuali azioni di sistema.

Caso importante:

```text
se la pausa avviene durante l’avviso finale
il bridge deve produrre StopFinalAlertSound
l’orchestratore deve chiamare AudioService.StopFinalAlertSound
```

L’orchestratore non deve decidere da solo se serve fermare l’audio.

L’orchestratore non deve dedurre lo stop audio leggendo stati interni.

Deve seguire le azioni prodotte dal bridge.

---

## 13. Resume

Il comando `Resume` serve a riprendere il timer dalla pausa.

L’orchestratore deve:

1. inoltrare `Resume` al bridge;
2. ricevere l’aggiornamento;
3. salvare ultimo modello mostrabile;
4. eseguire eventuali azioni di sistema.

Caso importante:

```text
se la ripresa avviene dentro la finestra finale
il bridge deve produrre StartFinalAlertSound
l’orchestratore deve chiamare AudioService.StartFinalAlertSound
```

L’orchestratore non deve dedurre da solo che il timer è in finestra finale.

Deve seguire le azioni prodotte dal bridge.

---

## 14. Reset

Il comando `Reset` serve a riportare il timer allo stato iniziale configurato.

L’orchestratore deve:

1. inoltrare `Reset` al bridge;
2. ricevere l’aggiornamento;
3. salvare ultimo modello mostrabile;
4. eseguire eventuali azioni di sistema.

Caso importante:

```text
se il reset avviene mentre l’audio finale è attivo
il bridge deve produrre StopFinalAlertSound
l’orchestratore deve chiamare AudioService.StopFinalAlertSound
```

L’orchestratore non deve resettare direttamente il servizio audio in modo autonomo durante i normali comandi applicativi.

Deve seguire le azioni prodotte dal bridge.

Lo stop audio autonomo è ammesso solo nella fase di chiusura controllata `Shutdown/Dispose`.

---

## 15. Tick

Il comando `Tick` serve a comunicare che è trascorso un intervallo di tempo.

L’orchestratore deve ricevere:

```text
elapsedSeconds
```

e inoltrarlo al bridge.

Esempio:

```text
Tick(1)
```

significa:

```text
è passato un secondo
```

L’orchestratore non deve generare da solo il tick.

Il tick verrà prodotto in futuro dal gestore del timer reale definito nel Design 006.

Flusso corretto futuro:

```text
Gestore timer reale
↓
Tick(1)
↓
Orchestratore
↓
Bridge
↓
Core
```

Flusso vietato:

```text
Gestore timer reale
↓
Core
```

Flusso vietato:

```text
Gestore timer reale
↓
Bridge
```

Il gestore timer reale parlerà solo con l’orchestratore.

---

## 16. Shutdown / Dispose

L’orchestratore deve supportare una chiusura controllata.

Quando l’orchestratore viene chiuso, deve tentare di fermare eventuale audio attivo.

Esempio:

```text
utente chiude l’app
↓
UI futura chiama Dispose sull’orchestratore
↓
orchestratore tenta AudioService.StopFinalAlertSound
↓
eventuali errori audio vengono trattati come tecnici e non bloccanti
↓
app può chiudersi
```

Regole:

1. `Dispose` o metodo equivalente deve tentare lo stop audio in modo sicuro;
2. eventuali errori audio durante la chiusura non devono bloccare la chiusura;
3. non devono essere lanciate eccezioni non gestite;
4. non deve essere mostrato alcun testo utente;
5. non deve essere chiamata la UI;
6. non deve essere avviato timer reale;
7. non deve essere modificato il core direttamente.

Il coding plan definirà se usare `IDisposable`, `IAsyncDisposable`, un metodo `Shutdown()` o una combinazione.

Per la prima versione è sufficiente una chiusura sincrona e sicura, salvo esigenze tecniche emerse nel coding plan.

---

## 17. Nessun timer reale interno

L’orchestratore non deve contenere una sorgente reale del tempo.

Non deve usare:

1. `DispatcherTimer`;
2. `System.Timers.Timer`;
3. `System.Threading.Timer`;
4. `Task.Delay` in loop;
5. thread dedicati;
6. loop asincroni;
7. scheduler temporali.

Motivo:

```text
la sorgente reale del tempo sarà definita nel Design 006
```

L’orchestratore deve essere testabile chiamando manualmente:

```text
Tick(1)
Tick(5)
Tick(10)
```

senza attendere tempo reale.

---

## 18. Stato iniziale dell’orchestratore

L’orchestratore deve avere uno stato iniziale sicuro.

All’avvio deve ottenere dal bridge il modello iniziale e conservarlo come modello corrente.

Il nome concreto del metodo dipenderà dalle API effettive del bridge e sarà definito nel coding plan.

Esempi concettuali:

```text
GetCurrentUpdate()
GetCurrentModel()
Initialize()
```

Il principio vincolante è:

```text
l’orchestratore non deve inventare il modello iniziale
```

Deve riceverlo dal bridge.

All’avvio l’orchestratore:

1. ottiene il modello iniziale dal bridge;
2. lo conserva come `CurrentModel`;
3. non avvia audio;
4. non avvia timer reale;
5. non produce tick;
6. non apre UI;
7. non genera testi utente.

---

## 19. Stato corrente dell’orchestratore

L’orchestratore deve mantenere uno stato corrente.

Lo stato corrente deve includere almeno:

1. ultimo modello mostrabile prodotto dal bridge;
2. ultimo risultato applicativo;
3. ultimo risultato tecnico audio, se presente.

Lo stato corrente non deve essere un ViewModel WPF.

Non deve implementare:

1. `INotifyPropertyChanged`;
2. binding;
3. `ObservableCollection`;
4. proprietà UI-specifiche.

Esempio concettuale:

```text
CurrentModel
LastAppResult
LastAudioResult
```

Il coding plan definirà i nomi effettivi.

---

## 20. Modello mostrabile corrente

L’orchestratore deve conservare l’ultimo modello mostrabile ricevuto dal bridge.

Questo modello sarà letto dalla futura UI.

Esempio:

```text
CurrentModel.RemainingTimeText = "04:59"
CurrentModel.StatusText = "Timer in esecuzione"
CurrentModel.CompletedSessionsText = "Sessioni completate: 2"
```

L’orchestratore non deve modificare questi testi.

L’orchestratore non deve ricostruire questi testi.

L’orchestratore deve conservarli e renderli disponibili.

Il modello corrente deve essere accessibile tramite proprietà o metodo neutro.

Esempi concettuali:

```text
CurrentModel
GetCurrentModel()
```

Il coding plan definirà la forma concreta in base ai tipi già presenti nel bridge.

---

## 21. SystemActions

Il bridge può produrre azioni di sistema.

Azioni già previste:

```text
StartFinalAlertSound
StopFinalAlertSound
```

L’orchestratore deve leggere le azioni di sistema e chiamare il servizio corrispondente.

Mappatura prevista:

```text
StartFinalAlertSound → AudioService.StartFinalAlertSound
StopFinalAlertSound → AudioService.StopFinalAlertSound
```

L’orchestratore non deve decidere da solo quando generare queste azioni.

L’orchestratore non deve chiamare l’audio guardando direttamente lo stato del core.

L’orchestratore non deve chiamare l’audio deducendo dagli stati del bridge.

L’orchestratore deve fidarsi delle azioni prodotte dal bridge.

---

## 22. Gestione del risultato audio

Il servizio audio restituisce un risultato tecnico composito.

Nel Design 004 e nel relativo TODO è stato previsto:

```text
AudioServiceResult
```

con distinzione tra:

```text
PlaybackResult
FocusResult
RestoreResult
```

L’orchestratore deve:

1. ricevere il risultato tecnico audio;
2. conservarlo come dato tecnico;
3. distinguere successo pieno, successo parziale e fallimento tecnico;
4. non trasformarlo direttamente in testo utente;
5. non bloccare il timer se il risultato audio indica fallimento;
6. non lanciare eccezioni non gestite per errori audio.

Esempio di successo parziale:

```text
PlaybackResult = Success
FocusResult = AudioFocusUnavailable
RestoreResult = NotAttempted
```

Significato:

```text
il beep è partito
non è stato possibile attenuare altri audio
il timer continua
il comando applicativo resta valido
```

Esempio con errore tecnico:

```text
PlaybackResult = PlaybackFailed
FocusResult = NotAttempted
RestoreResult = NotAttempted
```

Significato:

```text
il beep non è partito
l’errore resta tecnico
il timer continua
l’app non va in crash
```

Il coding plan definirà come rappresentare nel risultato applicativo eventuali successi parziali.

---

## 23. Errori audio non bloccanti

Regola fondamentale:

```text
un errore audio non deve mai bloccare il timer
```

L’orchestratore non deve:

1. fermare il ciclo se l’audio fallisce;
2. resettare il timer se l’audio fallisce;
3. modificare lo stato core se l’audio fallisce;
4. lanciare eccezioni non gestite se l’audio fallisce;
5. mostrare testi utente hardcoded se l’audio fallisce.

L’orchestratore può:

1. salvare il risultato tecnico dell’audio;
2. renderlo disponibile a futuri livelli;
3. lasciare che il timer continui.

---

## 24. Risultato applicativo dell’orchestratore

L’orchestratore deve restituire un risultato applicativo neutro per ogni comando.

Nome concettuale:

```text
AppCommandResult
```

Il nome definitivo sarà stabilito nel coding plan.

Il risultato applicativo deve contenere almeno:

1. `CurrentModel`;
2. `LastAudioResult`;
3. `Success`.

### 24.1 CurrentModel

`CurrentModel` rappresenta l’ultimo modello mostrabile prodotto dal bridge.

Non deve essere ricostruito dall’orchestratore.

Non deve contenere testi nuovi generati dall’orchestratore.

### 24.2 LastAudioResult

`LastAudioResult` rappresenta l’eventuale risultato tecnico prodotto dal servizio audio.

Può essere assente se il comando non ha prodotto azioni audio.

### 24.3 Success

`Success` indica che il comando applicativo è stato gestito senza errore bloccante.

Regola importante:

```text
Success non deve diventare false solo perché il focus audio non è disponibile
```

Esempio:

```text
PlaybackResult = Success
FocusResult = AudioFocusUnavailable
```

Questo è un successo applicativo con risultato audio tecnico parziale.

Esempio:

```text
PlaybackResult = PlaybackFailed
```

Anche questo non deve necessariamente rendere falso `Success`, se il comando timer è stato gestito e il timer continua.

Il coding plan definirà se distinguere meglio:

```text
Success
HasAudioWarning
HasTechnicalError
```

o campi equivalenti.

Il design vincola solo il principio:

```text
l’errore audio non è errore bloccante del comando timer
```

---

## 25. Comandi ravvicinati e sicurezza

L’orchestratore deve gestire comandi ravvicinati senza crash.

Esempi:

```text
Start chiamato due volte rapidamente
Pause chiamato mentre arriva Tick
Reset chiamato mentre l’audio sta partendo
Tick chiamato più volte in sequenza
```

L’orchestratore non deve:

1. corrompere lo stato corrente;
2. duplicare azioni audio non richieste dal bridge;
3. lanciare eccezioni non gestite;
4. inventare stati intermedi;
5. decidere direttamente la validità dei comandi.

La validazione dei comandi resta nel core/bridge.

Il coding plan definirà la tecnica concreta per garantire sicurezza, ad esempio serializzazione dei comandi, lock o altra soluzione semplice.

Questo design non impone una tecnica specifica.

---

## 26. Idempotenza e sicurezza

L’orchestratore deve essere sicuro rispetto a comandi ripetuti.

Esempi:

```text
Start chiamato due volte
Pause chiamato quando già in pausa
Resume chiamato quando non in pausa
Reset chiamato più volte
Tick chiamato quando timer fermo
```

L’orchestratore non deve decidere da solo se questi comandi sono validi.

Deve inoltrarli al bridge/core e conservare il risultato.

La validazione resta nella logica già implementata.

L’orchestratore deve solo garantire:

1. nessun crash;
2. stato corrente aggiornato se il bridge produce aggiornamento;
3. nessuna chiamata audio non prevista dal bridge;
4. nessuna eccezione non gestita.

---

## 27. Ordine di elaborazione comando

Per ogni comando applicativo, l’ordine deve essere:

```text
1. ricevere comando
2. inoltrare comando al bridge
3. ricevere update dal bridge
4. salvare modello mostrabile corrente
5. leggere SystemActions
6. eseguire SystemActions
7. salvare risultato tecnico audio
8. restituire risultato applicativo
```

Motivo:

```text
prima si aggiorna la logica, poi si eseguono gli effetti tecnici richiesti
```

L’orchestratore non deve eseguire audio prima di aver ricevuto un’azione dal bridge.

---

## 28. Più azioni di sistema nello stesso update

Il bridge può produrre zero, una o più azioni di sistema.

L’orchestratore deve gestire una lista di azioni.

Regola:

```text
le azioni devono essere eseguite nell’ordine in cui il bridge le produce
```

Esempio teorico:

```text
StopFinalAlertSound
StartFinalAlertSound
```

L’orchestratore deve rispettare l’ordine.

Se una azione fallisce, le altre azioni devono essere valutate comunque, salvo errore tecnico grave non gestibile.

L’audio non deve bloccare il timer.

---

## 29. Azioni sconosciute

Nella prima versione le azioni previste sono:

```text
StartFinalAlertSound
StopFinalAlertSound
```

Se in futuro comparissero azioni non gestite, l’orchestratore deve restare sicuro.

Per la prima versione può:

1. ignorare l’azione sconosciuta;
2. registrare un risultato tecnico;
3. non lanciare eccezioni non gestite.

Il coding plan potrà decidere se usare uno switch esaustivo o un fallback.

---

## 30. Testabilità

L’orchestratore deve essere testabile senza:

1. UI;
2. WPF;
3. XAML;
4. `DispatcherTimer`;
5. timer reale;
6. audio reale obbligatorio;
7. NVDA;
8. UI Automation.

I test dell’orchestratore dovranno usare versioni finte o controllate di:

1. bridge;
2. audio service.

Scopo dei test:

1. verificare inoltro comandi;
2. verificare aggiornamento stato corrente;
3. verificare esecuzione azioni audio;
4. verificare che errori audio non blocchino;
5. verificare che successi parziali audio non blocchino;
6. verificare che `Tick` venga inoltrato senza timer reale;
7. verificare che `Dispose` tenti stop audio sicuro;
8. verificare che comandi ravvicinati non causino crash;
9. verificare che l’orchestratore non dipenda da UI.

---

## 31. Relazione con Design 006

Il Design 006 definirà il gestore del timer reale.

Il gestore del timer reale dovrà:

1. produrre tick reali;
2. sapere quando iniziare a inviare tick;
3. sapere quando fermarsi;
4. parlare solo con l’orchestratore;
5. non chiamare direttamente core;
6. non chiamare direttamente bridge;
7. non chiamare direttamente audio.

Il Design 005 prepara il punto di ingresso:

```text
Tick(elapsedSeconds)
```

ma non implementa la sorgente del tempo.

---

## 32. Relazione con Design 007

Il Design 007 definirà la UI WPF accessibile e l’integrazione finale.

La UI dovrà:

1. leggere stato corrente dall’orchestratore;
2. inviare comandi applicativi all’orchestratore;
3. non chiamare direttamente core;
4. non chiamare direttamente audio;
5. non chiamare direttamente localization;
6. non interpretare direttamente SystemActions;
7. rispettare accessibility-rules;
8. esporre controlli tramite UI Automation;
9. gestire focus tastiera e lettura NVDA.

Il Design 005 non implementa nessuna di queste parti.

---

## 33. Criteri di validità

Il design sarà rispettato se una futura implementazione:

1. crea `services/CicloTimer.App/`;
2. crea progetto `CicloTimer.App`;
3. usa `services/CicloTimer.App/CicloTimer.App.csproj`;
4. crea test in `tests/CicloTimer.App.Tests/`;
5. permette dipendenza da `CicloTimer.Bridge`;
6. permette dipendenza da `CicloTimer.Audio`;
7. usa astrazioni o interfacce per rendere testabile bridge/audio, dove possibile;
8. non dipende direttamente da `CicloTimer.Core`;
9. non dipende direttamente da `CicloTimer.Localization`;
10. non dipende da progetto WPF root;
11. non dipende da WPF;
12. non usa XAML;
13. non usa `DispatcherTimer`;
14. non usa `System.Timers.Timer`;
15. non usa `System.Threading.Timer`;
16. non usa `Task.Delay` in loop;
17. non implementa timer reale;
18. non implementa UI;
19. non implementa `ICommand`;
20. non implementa `INotifyPropertyChanged`;
21. espone comandi applicativi;
22. espone o conserva stato corrente;
23. riceve `Tick(elapsedSeconds)` da fuori;
24. inoltra comandi al bridge;
25. conserva il modello mostrabile prodotto dal bridge;
26. ottiene dal bridge il modello iniziale;
27. legge `SystemActions` dal bridge;
28. chiama `AudioService` solo in risposta a `SystemActions`;
29. conserva risultati audio tecnici;
30. distingue successo audio pieno, parziale e fallito;
31. non blocca il timer per errori audio;
32. tenta stop audio sicuro in `Shutdown/Dispose`;
33. non genera testi utente hardcoded;
34. non modifica core/bridge/localization/audio;
35. non crea cartella `src/`;
36. non crea cartella `orchestrators/`.

---

## 34. Criteri di non validità

L’implementazione non sarà valida se:

1. mette l’orchestratore dentro `models/`;
2. mette l’orchestratore dentro `locales/`;
3. mette l’orchestratore dentro `view-models/`;
4. mette l’orchestratore dentro `services/CicloTimer.Audio/`;
5. mette l’orchestratore nel progetto WPF root;
6. crea `src/`;
7. crea `orchestrators/`;
8. fa dipendere `CicloTimer.App` direttamente da core;
9. fa dipendere `CicloTimer.App` direttamente da localization;
10. fa dipendere `CicloTimer.App` dalla UI WPF;
11. usa `DispatcherTimer`;
12. usa timer reale;
13. usa XAML;
14. usa `ICommand`;
15. usa `INotifyPropertyChanged`;
16. chiama audio senza `SystemActions`;
17. decide direttamente quando parte l’avviso finale;
18. decide direttamente quando si ferma l’avviso finale;
19. costruisce testi utente;
20. introduce stringhe hardcoded per l’utente;
21. gestisce NVDA;
22. gestisce UI Automation;
23. implementa Live Region;
24. modifica core;
25. modifica bridge;
26. modifica localization;
27. modifica audio;
28. blocca il timer se l’audio fallisce;
29. considera `AudioFocusUnavailable` come errore bloccante se il playback è riuscito;
30. genera tick reali;
31. avvia thread o loop temporali;
32. apre finestre;
33. non tenta stop audio in chiusura controllata;
34. deduce start/stop audio leggendo stati invece di usare `SystemActions`.

---

## 35. Decisioni consolidate

Decisioni consolidate in questa versione:

1. l’orchestratore sarà un progetto separato;
2. il percorso sarà `services/CicloTimer.App/`;
3. il nome progetto sarà `CicloTimer.App`;
4. i test saranno in `tests/CicloTimer.App.Tests/`;
5. l’orchestratore dipenderà da `CicloTimer.Bridge`;
6. l’orchestratore dipenderà da `CicloTimer.Audio`;
7. l’orchestratore userà astrazioni/interfacce per Bridge e Audio dove possibile;
8. l’orchestratore non dipenderà direttamente da core;
9. l’orchestratore non dipenderà direttamente da localization;
10. l’orchestratore non dipenderà da UI WPF;
11. l’orchestratore non userà timer reale;
12. l’orchestratore riceverà `Tick(elapsedSeconds)` da fuori;
13. il futuro gestore timer reale parlerà solo con l’orchestratore;
14. la futura UI parlerà solo con l’orchestratore per i comandi applicativi;
15. l’orchestratore manterrà ultimo modello mostrabile prodotto dal bridge;
16. l’orchestratore otterrà dal bridge il modello iniziale;
17. l’orchestratore eseguirà `SystemActions` prodotte dal bridge;
18. l’orchestratore chiamerà il servizio audio solo in risposta a `SystemActions`;
19. l’orchestratore conserverà `AudioServiceResult`;
20. successi parziali audio non saranno bloccanti;
21. errori audio non bloccheranno il timer;
22. l’orchestratore supporterà chiusura controllata con tentativo di stop audio;
23. non sarà introdotto comando `Stop` separato nella prima versione;
24. non saranno introdotti testi utente hardcoded;
25. non saranno introdotti `ICommand` o `INotifyPropertyChanged`;
26. non sarà introdotta UI;
27. non sarà introdotto timer reale;
28. non sarà creata cartella `orchestrators/`.

---

## 36. Stato del documento

Questo documento è approvato come Design 005 — Orchestratore applicativo timer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione tecnica
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni revisione: conferma percorso services/CicloTimer.App/, esclusione orchestrators/, dipendenze solo Bridge e Audio, astrazioni/interfacce per testabilità, AppCommandResult minimo, modello iniziale dal bridge, Shutdown/Dispose con stop audio sicuro, gestione AudioServiceResult composito e successi parziali, Pause/Reset tramite SystemActions, Stop esplicito fuori perimetro, sicurezza su comandi ravvicinati
```

Il documento è approvato come base per il successivo Coding Plan 005.
