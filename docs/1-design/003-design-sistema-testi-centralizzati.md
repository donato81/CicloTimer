# CicloTimer — Design 003 — Sistema centralizzato testi e messaggi applicativi

**Tipo documento:** documento di design  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-02  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md  

---

## 1. Scopo del documento

Questo documento definisce il design del sistema centralizzato dei testi e dei messaggi applicativi di CicloTimer.

Il progetto ha già definito due elementi fondamentali:

1. il core timer engine, che deve restare neutro e non contenere testi rivolti all'utente;
2. il bridge UI-logica, che dovrà trasformare stati, eventi ed errori neutri in dati mostrabili.

Perché il bridge e la futura UI possano rispettare le regole architetturali, serve prima un sistema ufficiale e centralizzato per i testi utente.

Lo scopo di questo documento è definire:

1. dove devono stare i testi utente;
2. come devono essere organizzati;
3. quale progetto .NET deve contenerli;
4. quali testi minimi servono nella prima versione;
5. come lasciare aperta la possibilità di aggiungere lingue future;
6. quali livelli possono usare il sistema testi;
7. quali livelli non devono contenerli direttamente;
8. quali regole impediscono stringhe hardcoded sparse;
9. come evitare stringhe magiche, riflessione e conversioni automatiche degli enum in chiavi testuali;
10. come testare il sistema testi;
11. come questo sistema si collega ai design successivi.

Questo documento non implementa ancora il bridge.

Questo documento non implementa la UI.

Questo documento non implementa audio.

Questo documento non implementa NVDA o UI Automation.

Questo documento definisce solo la base centralizzata dei testi.

---

## 2. Problema da risolvere

L'architettura approvata stabilisce che:

```text
la logica core non deve contenere testi utente
la UI non deve inventare testi finali
il bridge non deve contenere stringhe hardcoded sparse
i messaggi rivolti all'utente devono essere centralizzati
````

Senza un sistema testi centralizzato, ogni livello rischia di scrivere frasi proprie.

Esempio scorretto:

```text
TimerBridge.cs
if state == Running return "Sessione in corso";
```

Esempio scorretto:

```text
MainWindow.xaml
<Button Content="Avvia" />
```

Esempio scorretto:

```text
TimerEngine.cs
return "La durata della sessione deve essere maggiore di zero.";
```

Questi esempi sono vietati perché spargono stringhe utente in punti diversi del codice.

Il sistema testi serve a creare una sola fonte ufficiale.

Esempio corretto:

```text
TimerState.Running
↓
Bridge
↓
LocalizationKeys.Timer.StateRunning
↓
CicloTimer.Localization
↓
"Sessione in corso"
```

Il bridge potrà usare il sistema testi per produrre il modello mostrabile.

La UI potrà usare il sistema testi per etichette, pulsanti e messaggi propri della schermata.

Il core continuerà a produrre solo stati, eventi ed errori neutri.

---

## 3. Principio guida

Il principio guida è:

```text
nessuna stringa utente hardcoded fuori dal sistema centralizzato testi
```

Questo significa che i testi rivolti all'utente devono stare nel progetto:

```text
locales/CicloTimer.Localization/
```

I testi utente non devono stare direttamente in:

```text
models/CicloTimer.Core/
view-models/CicloTimer.Bridge/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
servizi audio
orchestratori futuri
```

I test possono contenere stringhe attese per verificare il comportamento.

I test non sono la fonte produttiva dei testi.

---

## 4. Collocazione fisica obbligatoria

Il sistema testi deve essere collocato nella cartella root:

```text
locales/
```

Il progetto .NET dedicato deve essere:

```text
locales/CicloTimer.Localization/
```

Il file progetto deve essere:

```text
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
```

La struttura prevista è:

```text
locales/
  CicloTimer.Localization/
    CicloTimer.Localization.csproj
    SupportedLanguage.cs
    LocalizationService.cs
    LocalizationKeys.cs
    Locales/
      It/
        ItalianTimerTexts.cs
        ItalianCommandTexts.cs
        ItalianErrorTexts.cs
        ItalianAccessibilityTexts.cs
        ItalianUiTexts.cs
```

I test devono stare nella cartella root già usata per tutti i test:

```text
tests/
```

Il progetto test deve essere:

```text
tests/CicloTimer.Localization.Tests/
```

Il file progetto test deve essere:

```text
tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Struttura test prevista:

```text
tests/
  CicloTimer.Localization.Tests/
    CicloTimer.Localization.Tests.csproj
    ItalianTimerTextsTests.cs
    ItalianCommandTextsTests.cs
    ItalianErrorTextsTests.cs
    ItalianAccessibilityTextsTests.cs
    ItalianUiTextsTests.cs
    LocalizationServiceTests.cs
    LocalizationKeysTests.cs
```

Sono vietate collocazioni alternative come:

```text
src/
resources/
strings/
models/CicloTimer.Localization/
view-models/CicloTimer.Localization/
```

---

## 5. Progetto .NET separato

Il sistema testi deve essere un progetto .NET separato.

Nome progetto:

```text
CicloTimer.Localization
```

Target previsto:

```text
net9.0
```

Il progetto non deve usare:

```text
net9.0-windows
```

Il progetto non deve dipendere da:

```text
WPF
XAML
Windows API
NVDA
UI Automation
audio
database
cloud
CicloTimer.Core
CicloTimer.Bridge
ciclotimer WPF
```

Il progetto deve essere compilabile e testabile in modo indipendente.

Il progetto deve poter essere referenziato in futuro da:

```text
CicloTimer.Bridge
ciclotimer WPF
eventuali servizi futuri che devono mostrare testi utente
```

Il sistema testi deve essere autonomo.

---

## 6. Lingua iniziale e predisposizione multilingua

La prima versione implementa solo la lingua italiana.

Lingua iniziale:

```text
it
```

La struttura deve però essere già predisposta per aggiungere lingue future senza spostare i testi italiani.

Per questo i testi italiani devono stare sotto:

```text
Locales/It/
```

In futuro potranno essere aggiunte cartelle come:

```text
Locales/En/
Locales/Fr/
Locales/Es/
```

La prima versione non deve implementare realmente inglese, francese o altre lingue.

La prima versione non deve introdurre una UI per cambiare lingua.

La prima versione non deve salvare preferenze lingua.

La prima versione può assumere che la lingua corrente sia sempre:

```text
it
```

Il sistema deve però essere progettato in modo che il bridge e la UI non debbano cambiare struttura quando verranno aggiunte nuove lingue.

Esempio futuro:

```text
Lingua corrente = it
Timer.Running = "Sessione in corso"

Lingua corrente = en
Timer.Running = "Session running"
```

Nella prima versione esiste solo il primo caso.

---

## 7. Responsabilità del sistema testi

Il sistema testi può occuparsi di:

1. conservare testi statici utente;
2. conservare messaggi di errore;
3. conservare testi per stati del timer;
4. conservare testi per eventi del timer;
5. conservare testi per comandi;
6. conservare etichette UI;
7. conservare testi accessibili;
8. formattare frasi semplici con parametri;
9. esporre testi in base a una chiave;
10. esporre testi in base alla lingua corrente;
11. fornire fallback controllato alla lingua italiana;
12. permettere test automatici sui testi presenti.

Il sistema testi non deve occuparsi di:

1. calcolare stati del timer;
2. validare la logica del timer;
3. decidere quando una sessione è completata;
4. incrementare il contatore;
5. formattare il tempo `mm:ss`;
6. mappare enum del core su chiavi di localizzazione;
7. gestire UI;
8. aprire finestre;
9. usare XAML;
10. usare NVDA;
11. usare UI Automation;
12. riprodurre audio;
13. chiamare API Windows;
14. leggere file esterni nella prima versione;
15. usare database;
16. usare cloud;
17. decidere flussi applicativi.

Il sistema testi è un dizionario applicativo, non un motore logico.

---

## 8. Categorie di testi

I testi devono essere organizzati per area funzionale.

Categorie minime:

```text
Timer
Commands
Errors
Accessibility
Ui
```

### 8.1 Timer

Contiene testi legati a stati ed eventi del timer.

Esempi:

```text
Timer fermo
Sessione in corso
Avviso finale in corso
Timer in pausa
Timer configurato.
Timer avviato.
Timer ripreso.
Timer resettato.
Avviso finale iniziato.
Sessione completata.
Nuova sessione avviata.
Sessioni completate aggiornate.
```

### 8.2 Commands

Contiene testi dei comandi utente.

Esempi:

```text
Avvia
Pausa
Riprendi
Reset
Configura
```

### 8.3 Errors

Contiene messaggi di errore utente.

Esempi:

```text
La durata della sessione deve essere maggiore di zero.
La durata dell'avviso finale non può essere negativa.
La durata dell'avviso finale deve essere inferiore alla durata della sessione.
Configura il timer prima di avviarlo.
Il timer non può essere avviato nello stato corrente.
Il timer non può essere messo in pausa nello stato corrente.
Il timer non può essere ripreso nello stato corrente.
Il timer non può essere resettato nello stato corrente.
Errore interno: durata tick non valida.
```

### 8.4 Accessibility

Contiene testi e template accessibili.

Esempi:

```text
Tempo rimanente: {0}. {1}. {2}.
Sessione completata. Sessioni completate: {0}.
Errore: {0}
```

### 8.5 Ui

Contiene etichette e testi visibili della UI.

Esempi:

```text
Durata sessione
Durata avviso finale
Minuti
Secondi
Tempo rimanente
Stato timer
Sessioni completate
Messaggio
```

---

## 9. Chiavi testuali

Il sistema testi deve esporre i testi tramite chiavi controllate.

Le chiavi non devono essere stringhe libere sparse.

È consigliato usare una struttura tipizzata, per esempio:

```text
LocalizationKeys.Timer.StateStopped
LocalizationKeys.Timer.StateRunning
LocalizationKeys.Commands.Start
LocalizationKeys.Errors.InvalidSessionDuration
LocalizationKeys.Accessibility.StatusTemplate
LocalizationKeys.Ui.SessionDurationLabel
```

Il nome concreto potrà essere definito nel coding plan.

Il principio obbligatorio è:

```text
il codice applicativo non deve richiedere testi usando stringhe libere non controllate
```

Esempio scorretto:

```text
GetText("testo_timer_corrente_qualcosa")
```

Esempio scorretto:

```text
GetText("InvalidSessionDuration")
```

Esempio corretto:

```text
GetText(LocalizationKeys.Timer.StateRunning)
```

Oppure, se viene scelta una soluzione più semplice:

```text
TimerTexts.StateRunning
```

Il coding plan dovrà scegliere la soluzione più semplice e testabile.

---

## 10. Servizio di localizzazione

Il sistema deve esporre un punto di accesso controllato ai testi.

Nome concettuale:

```text
LocalizationService
```

Responsabilità:

1. sapere qual è la lingua corrente;
2. restituire testi italiani nella prima versione;
3. usare fallback italiano se la lingua richiesta non è supportata;
4. fornire metodi per testi semplici;
5. fornire metodi per testi con parametri;
6. non contenere logica applicativa;
7. non dipendere da UI o core.

Lingua corrente iniziale:

```text
it
```

Lingue supportate nella prima versione:

```text
it
```

Lingue future:

```text
en
fr
es
```

solo come possibilità strutturale, non come implementazione attuale.

Il servizio di localizzazione della prima versione deve restare semplice.

Non deve implementare:

```text
CultureInfo
rilevamento lingua di Windows
parsing di culture regionali
lettura preferenze utente
algoritmi complessi di fallback
```

Regola per la prima versione:

```text
se la lingua richiesta non è "it", il servizio restituisce comunque i testi italiani
```

Questo mantiene il progetto predisposto al multilingua, ma evita overengineering nella prima versione.

---

## 11. Testi con parametri

Il sistema testi deve supportare frasi con valori dinamici.

Esempi:

```text
Sessioni completate: {0}
Sessione completata. Sessioni completate: {0}.
Tempo rimanente: {0}. {1}. {2}.
Errore: {0}
```

I valori dinamici non sono testi statici.

I valori dinamici arrivano da altri livelli.

Esempio:

```text
CompletedSessions = 3
```

Il sistema testi fornisce il template:

```text
Sessioni completate: {0}
```

Il bridge o la UI lo usa per produrre:

```text
Sessioni completate: 3
```

Il sistema testi non deve sapere come viene calcolato `CompletedSessions`.

Il sistema testi non deve sapere come viene calcolato il tempo rimanente.

---

## 12. Relazione con il core

Il progetto core deve restare indipendente dal sistema testi.

Il core produce:

```text
TimerState
TimerEvent
TimerError
TimerCommandResult
valori numerici
```

Il core non deve produrre:

```text
"Sessione in corso"
"Timer avviato"
"La durata della sessione deve essere maggiore di zero."
```

Il core non deve referenziare:

```text
CicloTimer.Localization
```

La catena corretta è:

```text
CicloTimer.Core
```

indipendente.

Il sistema testi è separato:

```text
CicloTimer.Localization
```

Il bridge userà entrambi:

```text
CicloTimer.Bridge
↓
CicloTimer.Core
CicloTimer.Localization
```

---

## 13. Relazione con il bridge

Il Design 002 ha definito il bridge come traduttore tra core e dati mostrabili.

Il bridge deve dipendere dal sistema testi.

Il bridge deve usare `CicloTimer.Localization` per produrre testi finali a partire dai dati neutri del core.

La responsabilità di mappare i tipi neutri del core sulle chiavi di localizzazione appartiene al bridge, non al sistema testi.

Quindi:

```text
TimerState.Running
↓
Bridge
↓
LocalizationKeys.Timer.StateRunning
↓
CicloTimer.Localization
↓
"Sessione in corso"
```

Esempio:

```text
TimerError.InvalidSessionDuration
↓
Bridge
↓
LocalizationKeys.Errors.InvalidSessionDuration
↓
CicloTimer.Localization
↓
"La durata della sessione deve essere maggiore di zero."
```

Esempio:

```text
TimerEvent.SessionCompleted
↓
Bridge
↓
LocalizationKeys.Timer.SessionCompleted
↓
CicloTimer.Localization
↓
"Sessione completata."
```

Il progetto `CicloTimer.Localization` non deve conoscere:

```text
TimerState
TimerError
TimerEvent
TimerCommandResult
```

perché questi tipi appartengono al core.

Il bridge conosce sia core sia localization e quindi può fare la mappatura in modo esplicito.

Sono vietati nel bridge:

```text
TimerError.InvalidSessionDuration.ToString()
TimerState.Running.ToString()
TimerEvent.SessionCompleted.ToString()
```

se usati per costruire chiavi testuali.

Sono vietate anche stringhe magiche come:

```text
GetText("InvalidSessionDuration")
GetText("Running")
GetText("SessionCompleted")
```

Il bridge deve usare una mappatura esplicita e tipizzata.

Esempio corretto:

```text
TimerError.InvalidSessionDuration
→ LocalizationKeys.Errors.InvalidSessionDuration
```

oppure una forma equivalente approvata dal coding plan.

Il bridge può combinare testi e valori dinamici, ma i testi e i template devono provenire dal sistema testi.

---

## 14. Relazione con la UI

La UI futura deve usare il sistema testi per:

1. etichette dei campi;
2. testo dei pulsanti;
3. titoli;
4. messaggi visibili;
5. eventuali label accessibili non prodotte dal bridge.

La UI non deve contenere stringhe utente hardcoded.

Esempio scorretto:

```text
<Button Content="Avvia" />
```

Esempio corretto:

```text
Button.Content = localization.GetText(LocalizationKeys.Commands.Start)
```

La forma concreta dipenderà dal design UI.

Questo documento non implementa ancora la UI.

---

## 15. Relazione con l'accessibilità

Il sistema testi deve contenere anche testi e template accessibili.

L'accessibilità non deve essere aggiunta come testo sparso nella UI.

Esempio scorretto:

```text
AutomationProperties.Name = "Avvia timer"
```

scritto direttamente nella UI.

Esempio corretto:

```text
AutomationProperties.Name = localization.GetText(LocalizationKeys.Accessibility.StartTimer)
```

La forma concreta sarà definita nel design UI/accessibilità operativa.

Il sistema testi deve solo rendere disponibili i testi.

Non deve usare NVDA.

Non deve usare UI Automation.

Non deve creare Live Region.

---

## 16. Relazione con audio e sistema operativo

Il sistema testi non deve occuparsi di audio.

Il sistema testi non deve sapere quando parte un suono.

Il sistema testi può contenere eventuali messaggi testuali legati ad audio o stato sonoro, se in futuro servono alla UI.

Nella prima versione del sistema testi non è necessario introdurre testi audio-specifici, salvo quelli già richiesti da stati ed eventi del timer.

Il sistema testi non deve chiamare API Windows.

---

## 17. Formato tecnico della prima versione

La prima versione deve usare classi C# semplici.

Non sono richiesti nella prima versione:

```text
file JSON
file XML
file RESX
database
cloud
librerie esterne di localizzazione
rilevamento automatico lingua di Windows
UI per scelta lingua
salvataggio preferenze lingua
FrozenDictionary obbligatorio
reflection
```

Questi elementi potranno essere valutati in futuro solo se necessari.

La soluzione consigliata per la prima versione è:

```text
classi C# statiche o provider C# semplici
```

Esempio concettuale:

```text
Locales/It/ItalianTimerTexts.cs
Locales/It/ItalianCommandTexts.cs
Locales/It/ItalianErrorTexts.cs
Locales/It/ItalianAccessibilityTexts.cs
Locales/It/ItalianUiTexts.cs
```

Il coding plan definirà la forma concreta più semplice.

---

## 18. Fallback lingua

La prima versione supporta solo:

```text
it
```

Se viene richiesta una lingua non supportata, il sistema deve ricadere sull'italiano.

Regola:

```text
lingua non supportata → italiano
```

Nella prima versione il fallback deve essere semplice.

Non deve usare:

```text
CultureInfo
lingua di sistema Windows
preferenze utente
file esterni
database
```

Il fallback può essere implementato come:

```text
se language != it → usa it
```

Il fallback deve essere previsto nel design per evitare refactor futuri, ma non deve diventare un motore di internazionalizzazione complesso.

---

## 19. Dipendenze

Il progetto `CicloTimer.Localization` non deve dipendere da altri progetti applicativi.

Non deve referenziare:

```text
CicloTimer.Core
CicloTimer.Bridge
ciclotimer WPF
```

Il progetto potrà essere referenziato da:

```text
CicloTimer.Bridge
ciclotimer WPF
eventuali progetti futuri che mostrano testi utente
```

La direzione corretta è:

```text
CicloTimer.Bridge
↓
CicloTimer.Localization
```

non il contrario.

---

## 20. Test del sistema testi

I test del sistema testi devono stare in:

```text
tests/CicloTimer.Localization.Tests/
```

I test devono verificare:

1. che il progetto compili;
2. che la lingua italiana sia supportata;
3. che la lingua corrente iniziale sia italiana;
4. che una lingua non supportata ricada sull'italiano;
5. che le chiavi principali restituiscano testi non vuoti;
6. che i testi timer principali siano presenti;
7. che i testi comandi principali siano presenti;
8. che i testi errori principali siano presenti;
9. che i testi accessibili principali siano presenti;
10. che i testi UI principali siano presenti;
11. che i template con parametri funzionino;
12. che non servano dipendenze da WPF;
13. che non servano dipendenze da core;
14. che non servano dipendenze da bridge;
15. che non servano API Windows;
16. che non vengano usate stringhe magiche come chiavi libere.

I test possono contenere stringhe attese per verificare che il testo italiano sia corretto.

Esempio ammesso nei test:

```text
Assert.Equal("Sessione in corso", text);
```

Questa non è una violazione della regola anti-stringhe hardcoded perché il test non è codice produttivo mostrato all'utente.

I test non devono diventare fonte produttiva dei testi.

---

## 21. Testi minimi obbligatori prima versione

Il sistema deve contenere almeno i testi necessari per i design già approvati.

### 21.1 Stati timer

```text
Timer fermo
Sessione in corso
Avviso finale in corso
Timer in pausa
```

### 21.2 Eventi timer

```text
Timer configurato.
Timer avviato.
Timer in pausa.
Timer ripreso.
Timer resettato.
Avviso finale iniziato.
Sessione completata.
Sessioni completate aggiornate.
Nuova sessione avviata.
Configurazione o comando non valido.
```

### 21.3 Comandi

```text
Avvia
Pausa
Riprendi
Reset
Configura
```

### 21.4 Errori

```text
La durata della sessione deve essere maggiore di zero.
La durata dell'avviso finale non può essere negativa.
La durata dell'avviso finale deve essere inferiore alla durata della sessione.
Configura il timer prima di avviarlo.
Il timer non può essere avviato nello stato corrente.
Il timer non può essere messo in pausa nello stato corrente.
Il timer non può essere ripreso nello stato corrente.
Il timer non può essere resettato nello stato corrente.
Errore interno: durata tick non valida.
```

### 21.5 Etichette UI

```text
Durata sessione
Durata avviso finale
Minuti
Secondi
Tempo rimanente
Stato timer
Sessioni completate
Messaggio
```

### 21.6 Testi accessibili e template

```text
Tempo rimanente: {0}. {1}. {2}.
Sessione completata. Sessioni completate: {0}.
Errore: {0}
Avvia timer
Metti in pausa il timer
Riprendi timer
Resetta timer
```

---

## 22. Regole contro stringhe hardcoded

Sono vietate stringhe utente hardcoded fuori dal progetto `CicloTimer.Localization`.

Sono considerate stringhe utente:

1. testi visibili;
2. etichette;
3. pulsanti;
4. messaggi di errore;
5. messaggi evento;
6. testi accessibili;
7. nomi accessibili;
8. descrizioni rivolte all'utente.

Non sono considerate stringhe utente:

1. nomi di classi;
2. nomi di metodi;
3. nomi di enum;
4. nomi di file;
5. namespace;
6. messaggi interni di test tecnico;
7. commenti tecnici non mostrati all'utente;
8. stringhe attese usate esclusivamente nelle asserzioni dei test.

Regola per Cursor:

```text
se una stringa può essere vista, letta o annunciata all'utente, deve stare in CicloTimer.Localization
```

Eccezione controllata:

```text
i test possono contenere stringhe attese per verificare il contenuto prodotto dal sistema localization
```

Questa eccezione non autorizza stringhe utente nel codice produttivo.

---

## 23. Regole contro stringhe magiche e riflessione

Il sistema deve evitare chiavi testuali libere e fragili.

Sono vietati nel codice produttivo:

```text
GetText("Running")
GetText("InvalidSessionDuration")
GetText("SessionCompleted")
TimerState.Running.ToString()
TimerError.InvalidSessionDuration.ToString()
TimerEvent.SessionCompleted.ToString()
typeof(TimerError).Name
reflection sugli enum del core per costruire chiavi
```

Motivo:

```text
le chiavi testuali devono essere esplicite, tipizzate e verificabili
```

La mappatura deve essere esplicita.

Esempio corretto:

```text
TimerError.InvalidSessionDuration
→ LocalizationKeys.Errors.InvalidSessionDuration
```

Esempio corretto:

```text
LocalizationKeys.Commands.Start
→ "Avvia"
```

Il coding plan dovrà specificare la forma concreta delle chiavi tipizzate.

---

## 24. Criteri di validità

Il design sarà rispettato se una futura implementazione:

1. crea `locales/CicloTimer.Localization/`;
2. crea `locales/CicloTimer.Localization/CicloTimer.Localization.csproj`;
3. usa target `net9.0`;
4. non usa `net9.0-windows`;
5. crea una struttura per lingua sotto `Locales/It/`;
6. implementa solo italiano nella prima versione;
7. lascia aperta l'aggiunta futura di altre lingue;
8. crea un punto di accesso centralizzato ai testi;
9. non usa stringhe utente sparse fuori dal progetto localization;
10. non usa stringhe magiche come chiavi;
11. non usa `ToString()` sugli enum del core per costruire chiavi;
12. non usa riflessione sugli enum del core per costruire chiavi;
13. lascia al bridge la responsabilità di mappare tipi core su chiavi localization;
14. non fa dipendere il core da localization;
15. non fa dipendere localization da core;
16. non fa dipendere localization da bridge;
17. non fa dipendere localization da WPF;
18. consente al bridge futuro di usare localization;
19. consente alla UI futura di usare localization;
20. include testi timer;
21. include testi comandi;
22. include testi errori;
23. include testi accessibili;
24. include testi UI minimi;
25. include template con parametri;
26. prevede fallback italiano semplice;
27. non implementa parsing CultureInfo o lingua Windows nella prima versione;
28. crea test in `tests/CicloTimer.Localization.Tests/`;
29. i test possono verificare stringhe attese;
30. i test passano senza UI;
31. la solution compila.

---

## 25. Criteri di non validità

L'implementazione non è valida se:

1. crea i testi in `models/CicloTimer.Core/`;
2. crea i testi in `view-models/CicloTimer.Bridge/`;
3. crea i testi nella UI;
4. crea stringhe utente hardcoded in XAML;
5. crea stringhe utente hardcoded in code-behind;
6. crea stringhe utente hardcoded nel bridge;
7. crea stringhe utente hardcoded nel core;
8. usa JSON, XML, RESX o database senza design dedicato;
9. introduce più lingue reali non richieste;
10. introduce UI per scegliere lingua;
11. salva preferenze lingua;
12. usa API Windows;
13. usa WPF;
14. usa NVDA;
15. usa UI Automation;
16. usa audio;
17. fa dipendere localization dal core;
18. fa dipendere localization dal bridge;
19. fa dipendere localization dalla UI;
20. mette i test fuori da `tests/`;
21. non prevede struttura per lingua;
22. obbliga a refactor per aggiungere una lingua futura;
23. usa stringhe magiche come chiavi;
24. usa `ToString()` sugli enum del core per generare chiavi;
25. usa riflessione sugli enum del core per generare chiavi;
26. sposta nel sistema localization la conoscenza dei tipi `TimerState`, `TimerError` o `TimerEvent`;
27. implementa un motore complesso di internazionalizzazione nella prima versione.

---

## 26. Relazione con il Design 002

Il Design 002 resta valido come direzione del bridge.

Il bridge però non deve essere implementato prima del sistema testi centralizzati.

Il bridge dovrà usare `CicloTimer.Localization`.

Quindi la sequenza corretta è:

```text
Design 001 — Core timer engine
↓
Design 002 — Bridge UI-logica e modello mostrabile
↓
Design 003 — Sistema centralizzato testi e messaggi applicativi
↓
Coding Plan 003 / TODO 003 — implementazione sistema testi
↓
Coding Plan 002 aggiornato / TODO 002 — implementazione bridge
```

Questa sequenza evita che il bridge introduca un provider locale proprietario di stringhe.

Quando il bridge sarà implementato, dovrà essere aggiornato per referenziare sia:

```text
CicloTimer.Core
CicloTimer.Localization
```

ma il sistema localization non dovrà mai referenziare il bridge.

---

## 27. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. collocazione del progetto in `locales/CicloTimer.Localization/`;
2. test in `tests/CicloTimer.Localization.Tests/`;
3. progetto .NET separato;
4. target `net9.0`;
5. nessuna dipendenza da Windows;
6. prima lingua implementata: italiano;
7. struttura predisposta a più lingue tramite `Locales/It/`;
8. nessuna implementazione reale di altre lingue nella prima versione;
9. nessun JSON, XML, RESX o database nella prima versione;
10. classi C# semplici come soluzione preferita;
11. fallback italiano semplice;
12. nessun `CultureInfo` o rilevamento lingua Windows nella prima versione;
13. localization indipendente da core, bridge e UI;
14. bridge responsabile della mappatura core enum → chiavi localization;
15. vietate stringhe magiche, `ToString()` e riflessione sugli enum del core per generare chiavi;
16. stringhe attese nei test ammesse come eccezione controllata;
17. sistema testi obbligatorio prima dell'implementazione del bridge.

---

## 28. Criteri minimi di test futuri

Il futuro coding plan dovrà prevedere test per:

1. presenza del progetto `CicloTimer.Localization`;
2. target `net9.0`;
3. assenza di `net9.0-windows`;
4. lingua italiana supportata;
5. lingua corrente iniziale italiana;
6. fallback su italiano per lingua non supportata;
7. assenza di `CultureInfo` o rilevamento lingua Windows nella prima versione;
8. presenza testi stati timer;
9. presenza testi eventi timer;
10. presenza testi comandi;
11. presenza testi errori;
12. presenza testi accessibili;
13. presenza testi UI;
14. funzionamento template con un parametro;
15. funzionamento template con più parametri;
16. assenza di dipendenza da core;
17. assenza di dipendenza da bridge;
18. assenza di dipendenza da WPF;
19. assenza di API Windows;
20. assenza di stringhe magiche come chiavi pubbliche;
21. build del progetto;
22. test verdi.

---

## 29. Stato del documento

Questo documento è approvato come Design 003 — Sistema centralizzato testi e messaggi applicativi.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni consiglieri AI: mappatura Core→Localization a carico del bridge, divieto di stringhe magiche/ToString/riflessione, fallback italiano semplice senza CultureInfo, chiarimento sulle stringhe attese nei test
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. chiarimento che `CicloTimer.Localization` non deve conoscere `TimerState`, `TimerError` o `TimerEvent`;
2. chiarimento che il bridge mapperà esplicitamente i tipi neutri del core verso chiavi tipizzate di localization;
3. divieto di usare `ToString()` sugli enum del core per costruire chiavi testuali;
4. divieto di usare riflessione sugli enum del core per costruire chiavi testuali;
5. divieto di usare stringhe magiche come chiavi;
6. semplificazione del fallback: se la lingua richiesta non è supportata, usare italiano;
7. esclusione di `CultureInfo`, rilevamento lingua Windows e preferenze utente dalla prima versione;
8. chiarimento che le stringhe attese nei test sono ammesse;
9. conferma della collocazione `locales/CicloTimer.Localization/`;
10. conferma della struttura `Locales/It/`;
11. conferma dei test in `tests/CicloTimer.Localization.Tests/`;
12. conferma dell'uso di classi C# semplici nella prima versione.

Il documento è approvato dal project owner come base per il successivo Coding Plan 003.
