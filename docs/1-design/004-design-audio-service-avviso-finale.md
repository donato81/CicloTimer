# CicloTimer — Design 004 — Audio service e audio focus

**Tipo documento:** documento di design  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/2-coding-plans/002-coding-plan-bridge-ui-logica-timer.md, docs/3-todos/002-todo-bridge-ui-logica-timer.md  

---

## 1. Scopo del documento

Questo documento definisce il design tecnico del servizio audio di CicloTimer.

I documenti precedenti hanno già definito:

```text
Design 001 → Core timer engine
Design 003 → Sistema centralizzato testi
Design 002 → Bridge UI-logica e modello mostrabile
````

Il bridge già implementato produce richieste concettuali verso il sistema audio:

```text
StartFinalAlertSound
StopFinalAlertSound
```

Questo Design 004 definisce cosa dovrà fare il futuro livello audio quando riceve queste richieste.

Lo scopo è progettare un servizio audio separato, sicuro e testabile, responsabile di:

1. avviare l’avviso sonoro finale;
2. fermare l’avviso sonoro finale;
3. usare un file audio predefinito incluso nel progetto;
4. mantenere l’avviso percepibile per tutta la finestra finale;
5. tentare di dare priorità percettiva al timer rispetto ad altri audio attivi;
6. tentare attenuazione temporanea di altri audio solo tramite meccanismi sicuri e supportati;
7. evitare manipolazioni aggressive o non sicure di processi/sessioni audio esterne;
8. tenere traccia delle modifiche audio effettivamente applicate;
9. tentare di ripristinare solo le modifiche audio effettivamente applicate;
10. gestire errori audio senza bloccare il timer.

Questo documento non definisce ancora:

1. libreria audio concreta;
2. API Windows concreta;
3. implementazione C# dettagliata;
4. contenuto sonoro definitivo del file audio;
5. UI impostazioni audio;
6. selezione suono da parte dell’utente;
7. persistenza preferenze audio;
8. ViewModel;
9. XAML;
10. orchestratore finale;
11. collegamento concreto con `MainWindow`;
12. coding plan operativo;
13. TODO eseguibile da Cursor.

---

## 2. Obiettivo del design

L’obiettivo è progettare un componente audio che riceva richieste concettuali e produca effetti sonori senza contaminare core, localization, bridge o UI.

Il servizio audio deve reagire a:

```text
StartFinalAlertSound
StopFinalAlertSound
```

Quando riceve `StartFinalAlertSound`, deve:

1. avviare l’avviso finale;
2. usare un file audio predefinito incluso nel progetto;
3. mantenere l’avviso percepibile per tutta la finestra finale;
4. tentare, dove possibile e sicuro, di aumentare la priorità percettiva del timer;
5. non bloccare il timer se qualcosa fallisce.

Quando riceve `StopFinalAlertSound`, deve:

1. fermare l’avviso finale;
2. tentare di ripristinare solo eventuali modifiche audio realmente applicate dal servizio;
3. non generare errore grave se l’avviso è già fermo.

Il servizio audio non deve decidere:

1. quando il timer entra in avviso finale;
2. quando il timer finisce;
3. quando una sessione viene completata;
4. quando parte una nuova sessione;
5. quando generare tick;
6. cosa mostrare nella UI;
7. quali testi utente mostrare.

Queste responsabilità restano nei componenti già definiti.

---

## 3. Principio guida

Il principio guida è:

```text
l’audio esegue richieste, non decide il timer
```

Il servizio audio deve sapere:

```text
è arrivata una richiesta StartFinalAlertSound
è arrivata una richiesta StopFinalAlertSound
l’avviso audio è già attivo o no
se esiste un file audio predefinito riproducibile
se è possibile tentare priorità percettiva verso il timer
se ha applicato modifiche audio da ripristinare
```

Il servizio audio non deve sapere:

```text
durata sessione
durata avviso finale
stato interno del core
regole di completamento sessione
numero sessioni completate
testi localization
UI
XAML
NVDA
Live Region
database
cloud
```

---

## 4. Relazione con il bridge

Il bridge produce richieste concettuali:

```text
SystemActionRequest.StartFinalAlertSound
SystemActionRequest.StopFinalAlertSound
```

Il servizio audio dovrà essere invocato da un futuro orchestratore o livello applicativo.

Relazione corretta:

```text
Core
↓
Bridge
↓
TimerBridgeUpdate.SystemActions
↓
Orchestratore futuro
↓
AudioService
```

Relazione vietata:

```text
Core
↓
AudioService
```

Relazione vietata:

```text
Bridge
↓
API Windows audio concreta
```

Relazione vietata:

```text
AudioService
↓
Core
```

Il servizio audio non deve modificare il bridge.

Il servizio audio non deve modificare il core.

Il servizio audio non deve modificare localization.

Il servizio audio non deve essere chiamato direttamente dal core.

---

## 5. Relazione con il core

Il core resta responsabile di:

1. stati del timer;
2. eventi del timer;
3. completamento sessione;
4. avvio nuova sessione;
5. final alert logico;
6. tick;
7. contatore sessioni.

Il servizio audio non deve referenziare `CicloTimer.Core`.

Il servizio audio non deve leggere `TimerEngine`.

Il servizio audio non deve leggere `TimerState`.

Il servizio audio non deve leggere `TimerCommandResult`.

Il servizio audio riceve solo richieste già tradotte da un futuro orchestratore.

Questa separazione è fondamentale perché un errore audio non deve mai compromettere il core.

---

## 6. Relazione con localization

Il servizio audio non deve dipendere da `CicloTimer.Localization` nella prima versione.

Motivo:

```text
il servizio audio non produce testi utente
```

Eventuali messaggi di errore visuali o accessibili saranno trattati da un futuro livello UI/orchestrazione, usando localization.

Il servizio audio può produrre risultati tecnici o stati neutri, ma non testi finali per l’utente.

Esempio ammesso:

```text
AudioActionResult.Success
AudioActionResult.FileMissing
AudioActionResult.PlaybackFailed
```

Esempio vietato:

```text
"Avviso audio non disponibile."
```

Se servirà mostrare quel messaggio, sarà un futuro design a definire chi lo traduce tramite localization.

---

## 7. Responsabilità autorizzate

Il servizio audio può:

1. ricevere comando di avvio avviso finale;
2. ricevere comando di stop avviso finale;
3. caricare o riferirsi a un file audio predefinito dell’app;
4. riprodurre il file audio;
5. ripetere o mantenere percepibile l’audio per tutta la finestra finale;
6. fermare l’audio;
7. sapere se l’avviso audio è attivo;
8. evitare duplicati se riceve start ripetuti;
9. ignorare in modo sicuro stop ripetuti;
10. tentare attenuazione degli altri audio solo tramite API sicure e supportate;
11. tentare sospensione o silenziamento di altri audio solo se sicuro, supportato e tecnicamente possibile;
12. tenere traccia delle modifiche audio effettivamente applicate;
13. tentare ripristino delle modifiche audio effettivamente applicate;
14. restituire esiti tecnici neutri;
15. gestire errori senza eccezioni non controllate;
16. restare testabile con adapter o interfacce.

---

## 8. Responsabilità vietate

Il servizio audio non deve:

1. decidere stati timer;
2. generare tick;
3. modificare `TimerEngine`;
4. modificare `TimerBridge`;
5. modificare `TimerBridgeUpdate`;
6. modificare `CicloTimer.Localization`;
7. modificare UI WPF;
8. aprire finestre;
9. usare XAML;
10. usare `ICommand`;
11. usare `INotifyPropertyChanged`;
12. gestire NVDA;
13. creare Live Region;
14. generare testi utente finali;
15. salvare preferenze utente;
16. permettere selezione suono personalizzato nella prima versione;
17. usare direttamente suoni di sistema Windows come sorgente principale;
18. bloccare il timer in caso di errore;
19. far terminare l’app in caso di errore audio;
20. riprovare all’infinito in caso di fallimento;
21. silenziare chiamate o app di comunicazione in modo aggressivo nella prima versione;
22. assumere che tutte le app audio siano controllabili;
23. assumere che il ripristino audio sia sempre garantito;
24. manipolare processi esterni;
25. chiudere processi esterni;
26. modificare sessioni audio altrui in modo unsafe;
27. forzare il volume globale di Windows.

---

## 9. Decisione 1 — Avviso continuo o ripetuto

L’avviso finale non deve essere un singolo beep isolato.

L’avviso finale deve essere:

```text
continuo o ripetuto per tutta la finestra finale
```

Esempio:

```text
Sessione: 5 minuti
Avviso finale: ultimi 20 secondi

00:20 → parte avviso sonoro
00:19 → avviso ancora percepibile
...
00:00 → stop avviso
```

Il design non sceglie ancora se la forma concreta sarà:

1. file audio in loop;
2. file audio ripetuto;
3. altro meccanismo equivalente.

La decisione tecnica concreta sarà presa nel coding plan.

Regola funzionale:

```text
l’avviso deve restare percepibile per tutta la finestra finale
```

---

## 10. Decisione 2 — File audio incluso nel progetto

L’avviso finale userà un file audio predefinito incluso nel progetto.

Il file audio sarà trattato come risorsa dell’app.

La prima versione non userà direttamente suoni di sistema Windows come sorgente principale.

Motivo:

```text
i suoni dell’app sono controllati e prevedibili
i suoni di Windows possono cambiare, mancare o dipendere dal tema/configurazione dell’utente
```

La scelta di un suono personalizzato, inclusi eventuali suoni di sistema Windows, è rinviata a un design futuro dedicato alle impostazioni audio.

La prima versione non deve introdurre:

1. selettore file;
2. impostazioni suono;
3. preferenze audio persistenti;
4. scelta tra suoni Windows;
5. scelta tra suoni personalizzati;
6. libreria di suoni utente.

Se il file audio predefinito non è disponibile o non è riproducibile, il timer deve continuare a funzionare e l’errore deve essere gestito senza crash.

---

## 11. Decisione 3 — Volume

Nella prima versione il volume dell’avviso finale non è configurabile dall’utente.

Il file audio dell’avviso viene riprodotto usando il volume normale dell’app/sistema.

La priorità percettiva dell’avviso finale deve essere cercata, dove tecnicamente possibile e sicuro, tramite meccanismi audio supportati da Windows o dalla libreria scelta.

Il servizio audio non deve forzare il volume globale di Windows.

Il servizio audio non deve alzare aggressivamente il volume del timer al massimo come strategia principale.

Una futura configurazione del volume dell’avviso potrà essere introdotta in un design successivo dedicato alle impostazioni audio.

La prima versione non deve introdurre:

1. slider volume;
2. preferenza volume;
3. salvataggio volume utente;
4. profili audio;
5. volume timer forzato al massimo;
6. modifica del volume globale del sistema.

---

## 12. Decisione 4 — Relazione con audio di altre app

Durante la finestra finale, CicloTimer deve dare priorità percettiva all’avviso del timer rispetto agli altri audio attivi.

Questa è una priorità funzionale desiderata, non una garanzia assoluta di controllo su Windows.

Strategia principale:

```text
riprodurre in modo affidabile il file audio dell’avviso finale
```

Strategia secondaria, solo dove possibile:

```text
tentare attenuazione temporanea degli altri audio tramite API sicure e supportate
```

Strategia ammessa solo se tecnicamente sicura:

```text
sospendere o silenziare temporaneamente altri audio solo se supportato da API sicure, senza manipolazioni aggressive di processi esterni
```

Vincolo Windows:

```text
Windows desktop non garantisce un meccanismo universale, semplice e sicuro per attenuare o silenziare qualunque altra applicazione audio arbitraria.
```

Di conseguenza, il servizio audio non deve promettere controllo totale su:

1. browser;
2. player multimediali;
3. app moderne;
4. app di comunicazione;
5. chiamate;
6. riunioni;
7. sessioni audio arbitrarie.

Se la gestione degli altri audio fallisce o non è disponibile:

```text
il timer continua
l’avviso del timer viene comunque riprodotto se possibile
l’app non deve andare in crash
```

La gestione aggressiva di chiamate, riunioni o app di comunicazione resta fuori perimetro della prima versione.

---

## 13. Decisione 5 — Fail-safe

Il servizio audio deve essere fail-safe.

Un errore audio non deve mai:

1. bloccare il timer;
2. fermare il ciclo;
3. causare crash dell’app;
4. modificare lo stato logico del core;
5. impedire la ripartenza automatica delle sessioni.

Gli errori di:

1. file audio mancante;
2. file audio non riproducibile;
3. riproduzione fallita;
4. attenuazione altri audio fallita;
5. sospensione altri audio fallita;
6. ripristino audio fallito;

devono essere gestiti in modo controllato.

Il timer deve continuare a funzionare anche se l’audio fallisce.

---

## 14. Idempotenza dei comandi audio

I comandi audio devono essere idempotenti.

Significato:

```text
ricevere due volte lo stesso comando non deve creare uno stato rotto
```

Regole:

```text
StartFinalAlertSound se l’avviso è già attivo → non deve avviare duplicati
StopFinalAlertSound se l’avviso è già fermo → non deve produrre errore grave
```

Esempio:

```text
StartFinalAlertSound
StartFinalAlertSound
```

Risultato atteso:

```text
un solo avviso audio attivo
nessun duplicato
nessun crash
```

Esempio:

```text
StopFinalAlertSound
StopFinalAlertSound
```

Risultato atteso:

```text
audio fermo
nessun errore grave
nessun crash
```

---

## 15. Stato interno del servizio audio

Il servizio audio può mantenere un proprio stato tecnico minimo.

Stati concettuali possibili:

```text
Idle
PlayingFinalAlert
Stopping
Failed
```

Questi stati non sono stati del timer.

Sono solo stati tecnici del servizio audio.

Il servizio audio non deve esporre questi stati come stati core.

Il servizio audio non deve usare questi stati per decidere logica del timer.

Lo stato minimo serve solo a:

1. evitare doppie riproduzioni;
2. sapere se serve fermare l’audio;
3. sapere se serve tentare ripristino;
4. gestire errori in modo controllato.

---

## 16. Esiti tecnici del servizio audio

Il servizio audio può restituire esiti neutri.

Esempi concettuali:

```text
AudioActionResult.Success
AudioActionResult.AlreadyPlaying
AudioActionResult.AlreadyStopped
AudioActionResult.AudioFileMissing
AudioActionResult.PlaybackFailed
AudioActionResult.AudioFocusUnavailable
AudioActionResult.AudioFocusFailed
AudioActionResult.RestoreFailed
```

Questi esiti non sono testi utente.

Questi esiti non devono essere mostrati direttamente nella UI.

Se in futuro la UI dovrà mostrare un messaggio, servirà una mappatura tramite localization in un design successivo.

---

## 17. File audio predefinito

Il design stabilisce che deve esistere un file audio predefinito dell’app.

Percorso stabilito:

```text
services/CicloTimer.Audio/Assets/final-alert.wav
```

Formato stabilito per la prima versione:

```text
.wav
```

Nome file stabilito:

```text
final-alert.wav
```

Il file audio predefinito deve essere trattato come asset interno del progetto audio.

Struttura prevista:

```text
services/
  CicloTimer.Audio/
    CicloTimer.Audio.csproj
    Assets/
      final-alert.wav
```

Il coding plan dovrà definire:

1. come generare o procurare il file audio;
2. come includerlo nel progetto;
3. se usarlo come `Content` o risorsa equivalente;
4. come copiarlo in output;
5. come caricarlo a runtime.

Vincoli funzionali:

1. il file deve essere incluso nel progetto audio;
2. deve essere disponibile a runtime;
3. deve essere breve o comunque adatto a ripetizione/loop;
4. deve essere riconoscibile;
5. non deve essere eccessivamente aggressivo;
6. deve supportare avviso percepibile durante tutta la finestra finale;
7. non deve provenire da fonti con licenza non chiara;
8. non deve essere copiato da Windows o da internet senza verifica dei diritti d’uso.

Se il file manca o non è riproducibile:

```text
il servizio audio restituisce errore tecnico controllato
il timer continua
l’app non va in crash
```

---

## 18. Audio continuo, loop o ripetizione

Il design non sceglie ancora tra:

```text
loop continuo del file
ripetizione periodica del file
altro meccanismo equivalente
```

Il coding plan dovrà scegliere la forma più semplice e affidabile.

Criterio funzionale invariabile:

```text
dopo StartFinalAlertSound, l’avviso resta percepibile fino a StopFinalAlertSound
```

Criterio di stop:

```text
StopFinalAlertSound deve fermare l’avviso rapidamente
```

---

## 19. Audio focus e priorità percettiva

Il termine “audio focus” in questo documento indica il tentativo di rendere il timer più percepibile rispetto ad altri audio attivi.

Non significa che il servizio audio possa sempre controllare tutte le app.

Non significa che CicloTimer possa sempre abbassare Spotify, browser, YouTube, player video, chiamate o app moderne.

Strategia primaria obbligatoria:

```text
riprodurre in modo affidabile l’avviso del timer
```

Strategia secondaria condizionata:

```text
tentare attenuazione di altri audio solo se supportata da API sicure, stabili e compatibili con il perimetro dell’app
```

Strategia vietata:

```text
manipolare processi esterni
chiudere app esterne
alterare sessioni audio arbitrarie in modo unsafe
forzare il mixer globale di Windows
entrare in loop alla ricerca di controllo totale sugli altri audio
```

Fallback obbligatorio:

```text
se non è possibile gestire gli altri audio, riprodurre comunque il suono del timer
```

Il coding plan dovrà verificare quali possibilità tecniche reali esistono su Windows.

Il design non promette che tutte le app audio siano controllabili.

---

## 20. Tracciamento modifiche audio e ripristino

Se il servizio audio modifica temporaneamente uno stato audio, deve mantenere una traccia tecnica delle modifiche effettivamente applicate.

Esempi concettuali:

```text
sessione audio attenuata
valore precedente memorizzato
azione di ripristino necessaria
```

Il servizio audio deve tentare il ripristino quando riceve:

```text
StopFinalAlertSound
```

oppure quando l’avviso viene fermato per:

1. fine sessione;
2. pausa;
3. reset;
4. errore interno gestito;
5. chiusura controllata del servizio audio.

Regola importante:

```text
il servizio audio deve ripristinare solo ciò che ha realmente modificato
```

Se il servizio audio non ha modificato altri audio:

```text
non deve tentare ripristini inventati
```

Il ripristino deve essere tentato in modo controllato.

Il servizio audio non deve entrare in loop infinito se il ripristino fallisce.

Se il ripristino fallisce:

```text
il timer continua
l’app non va in crash
l’errore viene restituito o registrato come esito tecnico
```

---

## 21. Chiamate, riunioni e app di comunicazione

La prima versione non deve tentare una gestione aggressiva di:

1. chiamate vocali;
2. videoconferenze;
3. app di comunicazione;
4. dispositivi di input/output speciali;
5. audio di emergenza o sistema.

Motivo:

```text
silenziare o sospendere una comunicazione attiva può avere effetti indesiderati
```

Il coding plan potrà valutare se esistono API sicure per attenuazione generale, ma non deve progettare comportamenti invasivi verso comunicazioni attive senza un nuovo design.

---

## 22. Errori audio

Gli errori audio devono essere trattati come errori tecnici controllati.

Il servizio audio deve gestire almeno:

1. file mancante;
2. file non riproducibile;
3. riproduzione fallita;
4. stop fallito;
5. attenuazione altri audio non disponibile;
6. attenuazione altri audio fallita;
7. sospensione altri audio non disponibile;
8. sospensione altri audio fallita;
9. ripristino altri audio fallito;
10. comando start ripetuto;
11. comando stop ripetuto.

Gli errori audio non devono:

1. lanciare eccezioni non gestite;
2. bloccare thread principali;
3. bloccare il timer;
4. modificare core;
5. modificare bridge;
6. interrompere il ciclo delle sessioni.

---

## 23. Relazione con UI futura

Il servizio audio non comunica direttamente con la UI.

Relazione futura corretta:

```text
AudioService
↓
esito tecnico neutro
↓
Orchestratore futuro
↓
UI futura / localization futura
```

La UI futura potrà eventualmente mostrare messaggi come:

```text
Avviso audio non disponibile.
```

ma questo design non definisce ancora quei testi.

Questo design non autorizza modifiche a:

1. `MainWindow.xaml`;
2. `MainWindow.xaml.cs`;
3. ViewModel WPF;
4. binding;
5. impostazioni UI.

---

## 24. Relazione con accessibilità

Il servizio audio è importante per l’accessibilità pratica del timer, perché rende percepibile la finestra finale.

Tuttavia, il servizio audio non deve gestire direttamente:

1. NVDA;
2. screen reader;
3. Live Region;
4. UI Automation;
5. annunci vocali;
6. testi accessibili.

L’accessibilità testuale e gli annunci futuri restano responsabilità di UI/orchestrazione/accessibility design successivi.

Regola importante:

```text
se l’audio fallisce, il timer deve continuare e la UI futura dovrà poter segnalare l’errore in modo accessibile
```

Questo documento definisce solo il comportamento tecnico del servizio audio.

---

## 25. Collocazione fisica obbligatoria

Il servizio audio deve essere collocato in:

```text
services/CicloTimer.Audio/
```

Il file progetto deve essere:

```text
services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

Il file audio predefinito deve essere collocato in:

```text
services/CicloTimer.Audio/Assets/final-alert.wav
```

I test devono essere collocati in:

```text
tests/CicloTimer.Audio.Tests/
```

Il servizio audio non deve stare in:

```text
src/
models/
locales/
view-models/
```

Il servizio audio non è core.

Il servizio audio non è localization.

Il servizio audio non è bridge/view-model.

Il servizio audio non è UI.

Regola vincolante:

```text
non collocare l’audio dentro Core
non collocare l’audio dentro Localization
non collocare l’audio dentro Bridge
non collocare l’audio dentro UI WPF
non creare src/
```

---

## 26. Dipendenze previste

Il servizio audio non deve dipendere da:

1. `CicloTimer.Core`;
2. `CicloTimer.Localization`;
3. `CicloTimer.Bridge`;
4. progetto WPF;
5. test project.

Il servizio audio potrà dipendere da:

1. librerie .NET audio;
2. adapter interni;
3. interfacce tecniche definite nel progetto audio;
4. eventuali API Windows scelte nel coding plan, solo se isolate.

Target framework stabilito:

```text
net9.0-windows
```

Motivo:

```text
il servizio audio è specifico per Windows desktop e può dover interagire con sottosistemi multimediali Windows
```

Questa scelta non autorizza il servizio audio a invadere UI WPF o logica applicativa.

`net9.0-windows` serve solo a delimitare correttamente un servizio tecnico Windows-specifico.

---

## 27. Testabilità

Il servizio audio deve essere progettato in modo testabile.

I test non devono dipendere obbligatoriamente da:

1. casse fisiche;
2. file audio reale non controllato;
3. volume reale del sistema;
4. app esterne realmente in riproduzione;
5. Spotify;
6. browser;
7. YouTube;
8. chiamate reali.

Per testare la logica, il coding plan dovrà prevedere adapter o interfacce.

Interfacce concettuali consigliate:

```text
IAudioPlayer
IAudioFocusManager
```

Questi nomi sono concettuali e non impongono firme definitive.

Scopo:

```text
separare la logica del servizio audio dalle dipendenze reali di sistema
```

Esempi di test concettuali:

```text
StartFinalAlertSound quando Idle → avvia una sola riproduzione
StartFinalAlertSound quando già Playing → non duplica
StopFinalAlertSound quando Playing → ferma
StopFinalAlertSound quando Idle → nessun errore grave
file audio mancante → errore tecnico controllato
attenuazione non disponibile → riproduzione timer continua
attenuazione fallita → riproduzione timer continua
ripristino fallito → errore tecnico controllato
```

---

## 28. Sequenza normale

Sequenza prevista:

```text
Bridge produce StartFinalAlertSound
↓
Orchestratore futuro invia StartFinalAlertSound ad AudioService
↓
AudioService avvia avviso finale
↓
AudioService tenta priorità percettiva se supportata in modo sicuro
↓
Bridge produce StopFinalAlertSound
↓
Orchestratore futuro invia StopFinalAlertSound ad AudioService
↓
AudioService ferma avviso finale
↓
AudioService tenta ripristino solo delle modifiche realmente applicate
```

Il servizio audio non deve ricevere tick.

Il servizio audio non deve sapere quanti secondi mancano.

Il servizio audio non deve sapere perché è arrivato lo stop.

---

## 29. Sequenza errore file audio mancante

```text
StartFinalAlertSound
↓
AudioService cerca file audio predefinito
↓
file mancante
↓
AudioService restituisce errore tecnico controllato
↓
timer continua
↓
nessun crash
```

Il servizio audio non deve tentare di modificare core o bridge.

---

## 30. Sequenza attenuazione non disponibile o fallita

```text
StartFinalAlertSound
↓
AudioService avvia avviso timer
↓
AudioService verifica se la gestione sicura degli altri audio è disponibile
↓
gestione non disponibile o fallita
↓
AudioService continua avviso timer
↓
AudioService restituisce o registra errore tecnico controllato
↓
timer continua
```

L’avviso del timer ha priorità rispetto alla riuscita della gestione degli altri audio.

---

## 31. Sequenza pausa/reset durante avviso

```text
StopFinalAlertSound
↓
AudioService ferma avviso timer
↓
AudioService tenta ripristino delle modifiche realmente applicate
↓
AudioService torna Idle se possibile
```

Se lo stop arriva più volte:

```text
StopFinalAlertSound
StopFinalAlertSound
```

il risultato deve restare sicuro.

---

## 32. Criteri di validità

Il design sarà rispettato se una futura implementazione:

1. crea un servizio audio separato;
2. crea il servizio in `services/CicloTimer.Audio/`;
3. usa progetto `services/CicloTimer.Audio/CicloTimer.Audio.csproj`;
4. usa target `net9.0-windows`;
5. colloca il file audio in `services/CicloTimer.Audio/Assets/final-alert.wav`;
6. crea test in `tests/CicloTimer.Audio.Tests/`;
7. non modifica core;
8. non modifica localization;
9. non modifica bridge;
10. non modifica UI;
11. riceve richieste audio concettuali;
12. gestisce `StartFinalAlertSound`;
13. gestisce `StopFinalAlertSound`;
14. usa un file audio predefinito incluso nel progetto;
15. non usa suoni di sistema Windows come sorgente principale;
16. non introduce selezione suono utente;
17. non introduce impostazioni audio utente;
18. mantiene avviso percepibile fino allo stop;
19. non usa un singolo beep isolato;
20. tenta priorità percettiva solo dove possibile e sicuro;
21. tenta attenuazione altri audio solo tramite API sicure e supportate;
22. sospende/silenzia altri audio solo se possibile, sicuro e supportato;
23. non promette controllo totale su tutte le app;
24. non manipola processi esterni;
25. non forza il volume globale di Windows;
26. tiene traccia delle modifiche audio effettivamente applicate;
27. tenta ripristino solo delle modifiche effettivamente applicate;
28. non gestisce aggressivamente chiamate/riunioni;
29. gestisce errori audio senza crash;
30. non blocca il timer;
31. non modifica stato del core;
32. rende `StartFinalAlertSound` idempotente;
33. rende `StopFinalAlertSound` idempotente;
34. è testabile tramite adapter/interfacce;
35. non richiede app audio esterne reali nei test;
36. non crea `src/`;
37. non colloca audio dentro Core/Localization/Bridge/UI.

---

## 33. Criteri di non validità

L’implementazione non sarà valida se:

1. il servizio audio viene messo nel core;
2. il servizio audio viene messo in localization;
3. il servizio audio viene messo nel bridge;
4. il servizio audio viene messo direttamente nella UI WPF;
5. viene creata cartella `src/`;
6. l’audio modifica `TimerEngine`;
7. l’audio legge stati core;
8. l’audio genera tick;
9. l’audio decide quando parte l’avviso finale;
10. l’audio decide quando finisce una sessione;
11. l’audio blocca il timer in caso di errore;
12. l’audio causa crash se il file manca;
13. start ripetuti creano più suoni sovrapposti;
14. stop ripetuti generano errore grave;
15. si usa un singolo beep isolato come comportamento principale;
16. si usano suoni Windows come unica sorgente principale;
17. si introduce selezione suono utente nella prima versione;
18. si introduce volume configurabile nella prima versione;
19. si promette di controllare sempre ogni audio di ogni app;
20. si silenziano aggressivamente chiamate o riunioni senza nuovo design;
21. non viene tracciata alcuna modifica audio effettivamente applicata;
22. viene tentato ripristino di modifiche mai applicate;
23. i test richiedono Spotify, browser, YouTube o chiamate reali;
24. il servizio produce testi utente hardcoded;
25. il servizio dipende da localization senza motivo;
26. il servizio dipende dal bridge;
27. il servizio dipende dal core;
28. il servizio viene creato fuori da `services/CicloTimer.Audio/`;
29. il progetto non usa `net9.0-windows`;
30. il file audio predefinito viene messo fuori da `services/CicloTimer.Audio/Assets/final-alert.wav`.

---

## 34. Decisioni consolidate

Decisioni consolidate in questa versione:

1. l’avviso finale deve essere continuo o ripetuto, non un beep isolato;
2. l’avviso deve restare percepibile per tutta la finestra finale;
3. l’avviso usa un file audio predefinito incluso nel progetto;
4. il file audio predefinito si chiama `final-alert.wav`;
5. il file audio predefinito sta in `services/CicloTimer.Audio/Assets/final-alert.wav`;
6. il formato della prima versione è `.wav`;
7. i suoni di sistema Windows non sono sorgente principale nella prima versione;
8. la scelta utente del suono è rinviata a design futuro;
9. il volume non è configurabile nella prima versione;
10. il volume usa comportamento normale app/sistema;
11. il servizio audio non forza il volume globale di Windows;
12. la priorità percettiva va cercata solo con meccanismi sicuri e supportati;
13. sospendere/silenziare altri audio è ammesso solo se possibile, sicuro e supportato;
14. chiamate/riunioni sono fuori perimetro per gestione aggressiva;
15. il servizio audio è fail-safe;
16. errori audio non bloccano il timer;
17. start/stop sono idempotenti;
18. il servizio audio non dipende da core/localization/bridge/UI;
19. il servizio audio sarà implementato come componente separato;
20. il percorso definitivo è `services/CicloTimer.Audio/`;
21. i test saranno in `tests/CicloTimer.Audio.Tests/`;
22. il target è `net9.0-windows`;
23. la scelta libreria/API audio sarà decisa nel coding plan;
24. il servizio audio deve tenere traccia delle modifiche audio effettivamente applicate;
25. il ripristino riguarda solo modifiche realmente applicate;
26. la testabilità deve essere garantita tramite adapter/interfacce concettuali come `IAudioPlayer` e `IAudioFocusManager`.

---

## 35. Stato del documento

Questo documento è approvato come Design 004 — Audio service e audio focus.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione tecnica e consolidamento percorso services/asset audio
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni revisione: percorso services/CicloTimer.Audio/, target net9.0-windows, asset services/CicloTimer.Audio/Assets/final-alert.wav, limiti realistici su audio focus Windows, divieto manipolazioni unsafe, tracciamento modifiche audio applicate, ripristino solo delle modifiche effettive, testabilità tramite adapter/interfacce
```

Il documento è approvato come base per il successivo Coding Plan 004.

