# CicloTimer — Coding Plan 008 — Accessibilità UI WPF minima

**Tipo documento:** coding plan  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-05  
**Repository:** donato81/CicloTimer  
**Documento di design collegato:** `docs/1-design/008-DESIGN_accessibilita-ui-wpf-minima.md`  
**Documenti architetturali collegati:** `docs/0-architecture/`  

---

## 1. Scopo del documento

Questo documento traduce il DESIGN 008 approvato in un piano operativo di codifica.

Il documento di riferimento principale è:

```text
docs/1-design/008-DESIGN_accessibilita-ui-wpf-minima.md
````

Il DESIGN 008 ha stabilito che la UI WPF minima introdotta dal blocco 007 deve essere resa realmente accessibile da tastiera e con screen reader, in particolare NVDA su Windows.

Questo coding plan definisce:

1. quali file ispezionare prima di modificare;
2. quali aree del codice possono essere modificate;
3. quali aree del codice non possono essere modificate;
4. quale ordine operativo seguire;
5. come intervenire su Tab order;
6. come intervenire sul focus iniziale;
7. come intervenire sui nomi accessibili;
8. come intervenire sul `NumericStepControl`;
9. come intervenire sui messaggi evento accessibili;
10. come intervenire sugli errori accessibili;
11. come intervenire sulla localizzazione;
12. quali test automatici aggiungere o aggiornare;
13. quali verifiche manuali NVDA eseguire;
14. quale report finale produrre.

Questo coding plan non cambia il DESIGN 008.

Questo coding plan non introduce nuove funzionalità timer.

Questo coding plan non autorizza modifiche al Core, al runner temporale, alla logica audio o alle regole del timer.

---

## 2. Obiettivo operativo

L’obiettivo operativo è rifinire la UI WPF esistente affinché sia:

* usabile senza mouse;
* navigabile con Tab e Shift+Tab;
* leggibile da NVDA;
* chiara nei nomi dei controlli;
* prevedibile nel focus;
* comprensibile nei messaggi di errore;
* capace di comunicare eventi importanti senza parlare continuamente;
* coerente con il sistema centralizzato dei testi.

Il lavoro deve riguardare la UI già esistente.

Non deve essere creata una nuova UI.

Non deve essere modificata la logica del timer.

---

## 3. Precondizioni obbligatorie

Prima di modificare codice, l’agente di codifica deve leggere:

```text
docs/0-architecture/document-standards.md
docs/0-architecture/architecture.md
docs/0-architecture/accessibility-rules.md
docs/0-architecture/internal-api.md
docs/1-design/008-DESIGN_accessibilita-ui-wpf-minima.md
```

Poi deve ispezionare i file reali della UI e dei ViewModel esistenti.

In particolare deve cercare e leggere:

```text
views/
view-models/
locales/
tests/
```

L’agente non deve assumere nomi di file non verificati.

Se i nomi reali differiscono da quelli indicati in questo piano, deve usare i nomi reali trovati nel repository, mantenendo però il perimetro del DESIGN 008.

---

## 4. Perimetro autorizzato

Questo coding plan autorizza modifiche solo nelle seguenti aree:

```text
views/
view-models/
locales/
tests/
```

Sono autorizzate modifiche per:

1. impostare il focus iniziale della finestra;
2. definire o correggere il Tab order;
3. migliorare i nomi accessibili dei controlli;
4. migliorare il comportamento accessibile del `NumericStepControl`;
5. esporre il tempo rimanente come informazione leggibile ma non annunciata ogni secondo;
6. esporre lo stato corrente come informazione leggibile;
7. esporre il contatore delle sessioni completate come informazione leggibile;
8. aggiungere o correggere messaggi evento accessibili;
9. aggiungere o correggere messaggi errore accessibili;
10. aggiungere testi alla Localization;
11. aggiornare ViewModel solo per proprietà già coerenti con la UI e con il DESIGN 008;
12. aggiungere o aggiornare test automatici sul ViewModel e sulla Localization;
13. aggiungere verifiche manuali NVDA nel report finale.

Sono ammesse piccole rifiniture visive solo se collegate ad accessibilità, leggibilità, focus o comprensione dello stato.

---

## 5. Fuori perimetro

Questo coding plan non autorizza:

1. modifica del Core timer engine;
2. modifica delle regole del timer;
3. nuovi stati stabili del timer;
4. nuovi eventi Core;
5. modifica del runner temporale;
6. modifica della logica audio;
7. modifica dell’orchestratore, salvo micro-correzioni già necessarie e documentate per consumo di dati esistenti;
8. nuova funzionalità timer;
9. nuova modalità timer ciclica non già presente;
10. nuova checkbox o opzione ciclica;
11. persistenza;
12. salvataggio preferenze;
13. notifiche Windows;
14. tema scuro;
15. selettore tema;
16. installer;
17. packaging;
18. minimizzazione in tray;
19. scorciatoie globali;
20. nuove scorciatoie locali;
21. comando dedicato per leggere rapidamente il tempo rimanente;
22. sintesi vocale proprietaria;
23. dipendenza diretta da NVDA;
24. automazioni esterne verso NVDA;
25. chiamate dirette a librerie NVDA;
26. annuncio automatico del countdown ogni secondo;
27. annunci relativi a funzionalità cicliche future non presenti nella UI attuale;
28. nuove stringhe utente hardcoded in XAML o ViewModel;
29. redesign completo della finestra;
30. nuova architettura di UI.

Se durante l’implementazione sembra necessario uno di questi punti, l’agente deve fermarsi e segnalarlo nel report.

---

## 6. Strategia generale di intervento

L’agente deve lavorare in questo ordine:

```text
1. Ispezione baseline
2. Mappatura UI esistente
3. Mappatura testi esistenti
4. Mappatura test esistenti
5. Localizzazione testi mancanti
6. Rifinitura ViewModel
7. Rifinitura XAML accessibile
8. Focus iniziale
9. Tab order
10. NumericStepControl
11. Area evento accessibile
12. Errori accessibili
13. Test automatici
14. Build e test
15. Verifica manuale NVDA
16. Report finale
```

Non deve passare alla fase successiva se la fase corrente ha lasciato ambiguità bloccanti.

Se emerge un problema non coperto dal DESIGN 008, deve fermarsi.

---

## 7. Fase 1 — Ispezione baseline

L’agente deve eseguire una lettura iniziale senza modifiche.

Deve identificare:

* file XAML principale della finestra;
* code-behind della finestra, se presente;
* ViewModel collegato;
* implementazione del `NumericStepControl`;
* servizi o classi di Localization usati dalla UI;
* test esistenti relativi a UI, ViewModel e Localization;
* eventuali messaggi evento già presenti;
* eventuali messaggi errore già presenti.

Deve annotare nel report operativo:

```text
file UI principale:
file ViewModel principale:
file NumericStepControl:
file Localization rilevanti:
test rilevanti:
```

Questa fase non deve modificare codice.

---

## 8. Fase 2 — Mappatura UI esistente

L’agente deve mappare i controlli interattivi realmente presenti.

Controlli minimi attesi:

```text
Durata sessione, minuti
Durata sessione, secondi
Durata avviso finale, minuti
Durata avviso finale, secondi
Pulsante principale Avvia/Pausa/Riprendi
Reset
```

Deve verificare anche:

* testo tempo rimanente;
* testo stato corrente;
* testo sessioni completate;
* eventuale area messaggi;
* eventuale area errori;
* contenitori grafici o card;
* elementi decorativi.

Per ogni controllo interattivo deve stabilire:

```text
nome visivo:
nome accessibile attuale:
ruolo WPF/UI Automation:
è tabulabile:
ordine Tab:
stato abilitato/disabilitato:
binding al ViewModel:
```

Questa mappatura serve a evitare modifiche casuali.

---

## 9. Fase 3 — Mappatura Localization

L’agente deve verificare come sono organizzati i testi esistenti.

Deve controllare:

```text
locales/CicloTimer.Localization/
```

e identificare:

* chiavi già esistenti per UI;
* chiavi già esistenti per comandi;
* chiavi già esistenti per accessibilità;
* chiavi già esistenti per errori;
* chiavi già esistenti per eventi timer.

Non deve inventare un secondo sistema di testi.

Non deve creare stringhe parallele in XAML.

Se una chiave necessaria manca, deve aggiungerla nel sistema Localization esistente seguendo lo stile reale del progetto.

---

## 10. Fase 4 — Testi da aggiungere o verificare

L’agente deve verificare che esistano testi centralizzati per almeno:

```text
Durata sessione, minuti
Durata sessione, secondi
Durata avviso finale, minuti
Durata avviso finale, secondi
Timer avviato.
Timer messo in pausa.
Timer ripreso.
Timer resettato.
Avviso finale iniziato.
Sessione completata. Sessioni completate: {0}.
La durata della sessione deve essere maggiore di zero.
La durata dell’avviso finale deve essere minore della durata della sessione.
```

Per `NumericStepControl`, deve verificare o aggiungere testi accessibili concettuali come:

```text
Aumenta durata sessione, minuti
Diminuisci durata sessione, minuti
Aumenta durata sessione, secondi
Diminuisci durata sessione, secondi
Aumenta durata avviso finale, minuti
Diminuisci durata avviso finale, minuti
Aumenta durata avviso finale, secondi
Diminuisci durata avviso finale, secondi
```

Questi testi devono essere aggiunti solo se servono realmente alla soluzione tecnica scelta.

Non devono essere aggiunti testi inutilizzati.

Tutti i testi devono restare in italiano.

Non devono essere aggiunte altre lingue.

---

## 11. Fase 5 — Rifinitura ViewModel

L’agente deve verificare se il ViewModel espone già tutto ciò che serve alla UI accessibile.

Proprietà o dati attesi, se coerenti con il codice reale:

* testo del pulsante principale;
* stato di abilitazione del pulsante principale;
* stato di abilitazione del Reset;
* tempo rimanente formattato;
* stato corrente in forma leggibile;
* sessioni completate formattate;
* ultimo messaggio evento;
* ultimo messaggio errore;
* eventuali testi accessibili dei controlli.

Il ViewModel può essere modificato solo per fornire alla UI dati o testi già previsti dal DESIGN 008.

Il ViewModel non deve:

* calcolare il tempo;
* decidere eventi Core;
* incrementare sessioni;
* avviare nuove sessioni;
* duplicare regole logiche del Core;
* contenere stringhe utente hardcoded;
* introdurre scorciatoie;
* introdurre nuova logica timer.

Se il ViewModel deve costruire messaggi utente, deve usare Localization.

---

## 12. Fase 6 — Coerenza del pulsante principale

Il pulsante principale deve essere verificato in tutti gli stati.

Stati minimi:

```text
Timer fermo → Avvia
Sessione in corso → Pausa
Avviso finale in corso → Pausa
Timer in pausa → Riprendi
Sessione completata / ritorno a timer fermo → Avvia
```

Per ogni stato, devono essere coerenti:

* testo visivo;
* nome accessibile;
* comando eseguito;
* stato abilitato/disabilitato.

Sono vietate situazioni come:

```text
testo visivo Pausa
nome accessibile Avvia
```

oppure:

```text
nome accessibile Riprendi
comando reale Pausa
```

Questa coerenza deve essere coperta da test ViewModel, se tecnicamente possibile.

---

## 13. Fase 7 — Rifinitura XAML accessibile

Nel file XAML principale, l’agente deve impostare o correggere le proprietà accessibili necessarie.

Obiettivi:

* ogni controllo interattivo deve avere nome accessibile chiaro;
* i campi numerici devono essere comprensibili isolatamente;
* i pulsanti devono esporre nome e ruolo corretti;
* i testi informativi devono essere leggibili;
* gli elementi decorativi non devono disturbare la navigazione;
* il tempo rimanente deve essere leggibile ma non annunciato ogni secondo;
* stato corrente e sessioni completate devono essere leggibili;
* gli errori devono essere leggibili.

Il piano non impone una singola proprietà XAML specifica.

L’agente deve usare meccanismi WPF standard compatibili con UI Automation.

Sono ammessi, se coerenti con il codice reale:

* `AutomationProperties.Name`;
* `AutomationProperties.HelpText`;
* `AutomationProperties.LabeledBy`;
* `IsTabStop`;
* `TabIndex`;
* proprietà equivalenti WPF/UI Automation;
* pattern accessibili del controllo, se già disponibili.

Sono vietati:

* stringhe accessibili hardcoded;
* focus forzato continuo;
* hack per far parlare NVDA;
* finestre invisibili;
* sintesi vocale custom;
* chiamate a NVDA.

---

## 14. Fase 8 — Focus iniziale

All’apertura della finestra, il focus deve posizionarsi sul primo controllo utile.

Controllo previsto:

```text
Durata sessione, minuti
```

L’agente deve implementare il focus iniziale nel livello UI WPF.

Il focus iniziale non deve richiedere modifiche al Core.

Il focus iniziale non deve avviare timer o comandi.

Il focus iniziale non deve causare annunci ripetuti.

Il titolo della finestra deve essere comprensibile da NVDA.

Verifica manuale richiesta:

```text
aprire l’app con NVDA attivo
verificare che NVDA annunci la finestra e il primo controllo utile
```

---

## 15. Fase 9 — Tab order

Il Tab order deve seguire l’ordine:

```text
Durata sessione, minuti
↓
Durata sessione, secondi
↓
Durata avviso finale, minuti
↓
Durata avviso finale, secondi
↓
Pulsante principale Avvia/Pausa/Riprendi
↓
Reset
```

Gli elementi informativi non devono essere forzatamente inseriti nel Tab.

Non devono essere tabulabili, salvo necessità documentata:

* titolo;
* card;
* tempo rimanente;
* stato corrente;
* sessioni completate;
* messaggi evento;
* messaggi errore non interattivi;
* elementi decorativi.

Devono però restare leggibili da NVDA tramite normale esplorazione o meccanismo accessibile equivalente.

L’agente deve verificare se il ciclo Tab resta confinato nella finestra principale.

Se il focus rischia di uscire dalla UI applicativa in modo inatteso, deve usare una soluzione WPF standard per mantenere la navigazione all’interno della finestra, documentando la scelta nel report.

Il piano non impone in questa fase una singola proprietà XAML obbligatoria: la scelta tecnica deve derivare dalla struttura reale della finestra.

Verifiche richieste:

* Tab dall’inizio alla fine;
* Shift+Tab dalla fine all’inizio;
* nessun salto inatteso;
* nessun elemento decorativo nel ciclo Tab;
* nessun blocco del focus;
* nessuna uscita inattesa del focus dalla UI applicativa.

---

## 16. Fase 10 — NumericStepControl

L’agente deve verificare il comportamento reale del `NumericStepControl`.

Il controllo deve:

* essere raggiungibile da tastiera;
* essere comprensibile da NVDA;
* comunicare il valore corrente;
* permettere aumento e diminuzione senza mouse;
* comportarsi in modo compatto nel flusso Tab;
* evitare pulsanti interni anonimi;
* evitare una sequenza Tab troppo lunga;
* non contenere logica timer.

Regola operativa:

```text
il Tab deve trattare ogni valore numerico come un punto chiaro della navigazione,
non come una sequenza confusa di elementi interni
```

In caso di dubbio implementativo, dare priorità alla chiarezza della navigazione Tab:

```text
ogni NumericStepControl deve apparire come un singolo punto di navigazione
```

La modifica del valore tramite tastiera, preferibilmente con frecce direzionali quando coerente con il controllo esistente, è preferibile rispetto all’aggiunta di molteplici Tab stop interni per i pulsanti `+` e `-`.

La soluzione tecnica precisa va scelta in base al controllo reale.

Soluzioni possibili:

* focus principale sul valore numerico;
* aumento/diminuzione tramite tastiera;
* pulsanti interni accessibili ma non invadenti;
* proprietà accessibili sui pulsanti interni se rimangono raggiungibili;
* comportamento equivalente WPF/UI Automation.

Se vengono usati pulsanti interni più/meno, non devono essere letti da NVDA come:

```text
+
-
Button
```

Devono avere contesto, per esempio:

```text
Aumenta durata sessione, minuti
Diminuisci durata sessione, minuti
```

L’agente non deve introdurre un nuovo controllo complesso se basta rifinire quello esistente.

Se il controllo esistente non è recuperabile in modo semplice, l’agente deve segnalarlo nel report prima di tentare una riscrittura radicale.

---

## 17. Fase 11 — Tempo rimanente

Il tempo rimanente deve essere leggibile ma non annunciato automaticamente ogni secondo.

L’agente deve verificare che:

* il testo visivo del tempo rimanente continui ad aggiornarsi;
* NVDA possa leggerlo come informazione;
* non venga usata Live Region per ogni aggiornamento del countdown;
* non venga generato un messaggio evento a ogni tick;
* non venga spostato il focus a ogni tick;
* non venga introdotta una scorciatoia per leggerlo.

Esempio corretto:

```text
Tempo rimanente: 04:59
```

Esempio vietato:

```text
NVDA annuncia 04:59, 04:58, 04:57, 04:56...
```

---

## 18. Fase 12 — Stato corrente e sessioni completate

Lo stato corrente deve essere leggibile in forma utente.

Esempi:

```text
Stato: Timer fermo
Stato: Sessione in corso
Stato: Avviso finale in corso
Stato: Timer in pausa
```

Non devono essere mostrati o annunciati stati tecnici come:

```text
Running
Paused
FinalAlert
TimerState.Running
```

Le sessioni completate devono essere leggibili in forma:

```text
Sessioni completate: 0
Sessioni completate: 1
Sessioni completate: 2
```

La UI non deve calcolare questi valori.

Deve solo mostrare ciò che riceve dal ViewModel/Bridge/orchestratore.

---

## 19. Fase 13 — Area eventi accessibile

L’agente deve verificare se esiste già un’area messaggi evento.

Se non esiste, può crearne una nella UI, purché resti coerente con il DESIGN 008.

L’area evento deve:

* mostrare l’ultimo evento importante;
* essere visibile o comunque coerente con la UI;
* essere leggibile da NVDA;
* non ricevere focus automaticamente;
* non spostare il focus;
* non essere aggiornata a ogni tick;
* non essere usata per il countdown.

Eventi ammessi nel blocco 008:

```text
Timer avviato
Timer messo in pausa
Timer ripreso
Timer resettato
Avviso finale iniziato
Sessione completata
Errore rilevato
```

Eventi non ammessi:

```text
Nuova sessione ciclica avviata
Ciclo successivo avviato
Annuncio a ogni tick
Annuncio a ogni secondo
```

Gli annunci ordinari devono essere non aggressivi, equivalenti a comportamento `Polite`.

Solo errori bloccanti possono usare comportamento più urgente, equivalente ad `Assertive`.

L’area messaggi deve notificare correttamente anche eventi logicamente distinti che producono lo stesso testo consecutivo.

Se il solo aggiornamento del testo non è sufficiente, l’agente deve usare un meccanismo WPF/UI Automation equivalente, senza chiamare NVDA direttamente.

La soluzione tecnica deve usare WPF/UI Automation.

Se una semplice area testuale aggiornata non viene letta da NVDA, il coding plan autorizza un fallback WPF/UI Automation equivalente, purché:

* non chiami NVDA direttamente;
* non usi sintesi vocale custom;
* non sposti il focus;
* non introduca dipendenze esterne non necessarie;
* sia documentato nel report finale.

---

## 20. Fase 14 — Errori accessibili

Gli errori devono essere:

* visibili;
* leggibili da NVDA;
* comprensibili;
* non basati solo sul colore;
* collegati al campo da correggere quando possibile;
* espressi tramite testi centralizzati.

Esempi corretti:

```text
La durata della sessione deve essere maggiore di zero.
La durata dell’avviso finale deve essere minore della durata della sessione.
```

Esempi vietati:

```text
Errore
Campo non valido
Invalid input
```

Dopo un errore, il focus deve permettere all’utente di correggere.

Il focus può tornare al campo problematico se questo è il comportamento più chiaro.

L’agente non deve duplicare nel ViewModel regole logiche che appartengono al Core.

---

## 21. Fase 15 — Aspetto visivo minimo

Sono ammesse solo rifiniture visive collegate all’accessibilità.

Esempi ammessi:

* focus visuale più evidente;
* contrasto migliore;
* spaziatura più chiara;
* errore con testo visibile;
* stato non comunicato solo con colore.

Esempi vietati:

* redesign completo;
* tema scuro;
* nuove animazioni;
* nuova struttura della finestra;
* nuove icone definitive;
* schermate nuove.

---

## 22. Fase 16 — Test automatici

L’agente deve eseguire tutti i test esistenti.

Comando obbligatorio:

```bash
dotnet test
```

Inoltre deve valutare l’aggiunta o aggiornamento di test automatici per:

* testo del pulsante principale nello stato fermo;
* testo del pulsante principale nello stato in corso;
* testo del pulsante principale nello stato pausa;
* testo e stato di abilitazione del pulsante principale dopo il completamento di una sessione;
* coerenza stato timer → testo pulsante;
* messaggio evento dopo Start;
* messaggio evento dopo Pause;
* messaggio evento dopo Resume;
* messaggio evento dopo Reset;
* messaggio evento dopo completamento sessione, se già esposto dal comportamento attuale;
* messaggio errore per configurazione non valida;
* testi di Localization aggiunti;
* assenza di stringhe mancanti nelle chiavi aggiunte.

I test devono essere realistici.

Non è obbligatorio introdurre test UI Automation completi.

Se un aspetto è verificabile solo manualmente con NVDA, deve essere dichiarato nel report finale.

---

## 23. Fase 17 — Build

L’agente deve eseguire:

```bash
dotnet build
```

e poi:

```bash
dotnet test
```

La build deve essere verde.

I test devono essere verdi.

Numero atteso di partenza:

```text
337 / 337
```

Se vengono aggiunti test, il numero finale può aumentare.

Nel report finale l’agente deve indicare:

```text
Build:
Test:
Numero test:
```

---

## 24. Fase 18 — Verifica manuale NVDA

Il blocco 008 richiede verifica manuale con NVDA.

Checklist minima:

1. aprire l’app;
2. verificare annuncio titolo finestra;
3. verificare focus iniziale su “Durata sessione, minuti”;
4. navigare con Tab dall’inizio alla fine;
5. navigare con Shift+Tab dalla fine all’inizio;
6. verificare che il focus non esca in modo inatteso dalla UI applicativa;
7. leggere durata sessione minuti;
8. leggere durata sessione secondi;
9. leggere durata avviso finale minuti;
10. leggere durata avviso finale secondi;
11. modificare valori da tastiera;
12. verificare che `NumericStepControl` non appesantisca il Tab;
13. avviare il timer;
14. verificare che il pulsante diventi Pausa;
15. mettere in pausa;
16. verificare che il pulsante diventi Riprendi;
17. riprendere;
18. verificare che il pulsante torni Pausa;
19. resettare;
20. leggere stato corrente;
21. leggere tempo rimanente;
22. verificare che il tempo non venga annunciato ogni secondo;
23. leggere sessioni completate;
24. generare o verificare un errore accessibile;
25. verificare che il focus aiuti la correzione;
26. verificare che gli eventi importanti siano percepibili;
27. verificare che eventi logicamente distinti con lo stesso testo consecutivo siano comunque notificati correttamente, se applicabile;
28. verificare che gli annunci ordinari non interrompano aggressivamente;
29. chiudere l’app.

Per ogni punto della checklist NVDA che fallisce, l’agente deve annotare nel report finale:

1. punto della checklist fallito;
2. comportamento atteso;
3. comportamento effettivamente osservato;
4. azione correttiva intrapresa oppure motivazione per cui non è stata risolta in questo blocco.

Se l’agente non può usare NVDA direttamente, deve dichiararlo chiaramente e fornire una checklist pronta per il project owner.

Non deve fingere di aver verificato NVDA.

---

## 25. Verifica UI Automation, se disponibile

Se dispone di strumenti adeguati, l’agente deve verificare l’albero UI Automation generato dalla finestra.

Questa verifica serve a controllare che:

* i controlli interattivi abbiano nomi comprensibili;
* i ruoli dei controlli siano coerenti;
* gli elementi decorativi non disturbino;
* il `NumericStepControl` sia esposto in modo comprensibile;
* l’area messaggi evento sia individuabile;
* gli errori siano leggibili.

Se l’agente non dispone di strumenti adeguati per ispezionare l’albero UI Automation, deve dichiararlo nel report finale.

In quel caso deve specificare che la verifica è stata limitata a:

* lettura del codice;
* build;
* test automatici;
* checklist manuale NVDA da eseguire o già eseguita dal project owner.

Questa sezione non autorizza l’aggiunta di dipendenze esterne non necessarie.

---

## 26. Sequenza operativa obbligatoria per Cline/Roo Code

L’agente di codifica deve seguire questa sequenza:

```text
1. Leggere documenti architetturali.
2. Leggere DESIGN 008 approvato.
3. Ispezionare UI, ViewModel, Localization e test.
4. Mappare controlli e Tab order attuale.
5. Mappare testi esistenti.
6. Aggiungere solo i testi mancanti necessari.
7. Aggiornare ViewModel solo dove serve alla UI accessibile.
8. Aggiornare XAML con proprietà accessibili.
9. Implementare focus iniziale.
10. Correggere Tab order.
11. Verificare che il focus non esca in modo inatteso dalla finestra.
12. Rifinire NumericStepControl.
13. Implementare o correggere area evento accessibile.
14. Gestire eventi identici consecutivi, se il meccanismo scelto lo richiede.
15. Rendere errori accessibili.
16. Aggiungere o aggiornare test automatici.
17. Eseguire dotnet build.
18. Eseguire dotnet test.
19. Eseguire o preparare verifica NVDA.
20. Verificare UI Automation se strumenti disponibili.
21. Produrre report finale.
```

Non deve saltare direttamente alla modifica dello XAML senza prima leggere ViewModel e Localization.

Non deve aggiungere stringhe direttamente nella UI se esiste il sistema Localization.

---

## 27. Regole anti-errore per l’agente

L’agente deve rispettare queste regole:

```text
non modificare il Core
non modificare la logica timer
non aggiungere funzionalità
non aggiungere timer ciclico
non aggiungere scorciatoie
non annunciare il tempo ogni secondo
non spostare il focus a ogni tick
non chiamare NVDA direttamente
non aggiungere sintesi vocale
non creare testi hardcoded
non creare nuova architettura UI
non riscrivere la finestra da zero
```

Se una modifica sembra necessaria ma viola una di queste regole, l’agente deve fermarsi e segnalarla.

---

## 28. Criteri di completamento

Il lavoro è completo solo se:

* i documenti richiesti sono stati letti;
* il perimetro è stato rispettato;
* la UI si apre;
* il focus iniziale è prevedibile;
* il Tab order è corretto;
* Shift+Tab è corretto;
* il focus non esce in modo inatteso dalla UI applicativa;
* i controlli hanno nomi accessibili chiari;
* il `NumericStepControl` è usabile da tastiera;
* il `NumericStepControl` non appesantisce inutilmente Tab;
* tempo rimanente, stato e sessioni completate sono leggibili;
* il tempo rimanente non viene annunciato ogni secondo;
* gli eventi importanti sono comunicati;
* eventi identici consecutivi sono gestiti correttamente se applicabile;
* gli annunci ordinari sono non aggressivi;
* gli errori sono accessibili;
* non sono state aggiunte scorciatoie;
* non sono state aggiunte funzionalità timer;
* non sono state introdotte stringhe hardcoded;
* la build passa;
* i test passano;
* la verifica NVDA è stata eseguita o dichiarata come da eseguire manualmente dal project owner;
* la verifica UI Automation è stata eseguita se gli strumenti erano disponibili, oppure dichiarata come non eseguibile.

---

## 29. Report finale richiesto

L’agente deve produrre un report finale con questa struttura:

```text
# Report implementazione blocco 008

## 1. Documenti letti

## 2. File modificati

## 3. File creati

## 4. Modifiche Localization

## 5. Modifiche ViewModel

## 6. Modifiche XAML/UI

## 7. Focus iniziale

## 8. Tab order finale

## 9. NumericStepControl

## 10. Area eventi accessibile

## 11. Gestione eventi identici consecutivi

## 12. Errori accessibili

## 13. Test automatici aggiunti o modificati

## 14. Build

## 15. Test

## 16. Verifica NVDA

## 17. Verifica UI Automation, se disponibile

## 18. Problemi residui

## 19. Conferme di perimetro
```

Nella sezione “Conferme di perimetro” deve dichiarare esplicitamente:

```text
Core non modificato:
Logica timer non modificata:
Runner temporale non modificato:
Audio non modificato:
Nessuna scorciatoia aggiunta:
Nessuna nuova funzionalità timer:
Nessun annuncio countdown ogni secondo:
Nessuna dipendenza diretta da NVDA:
Nessuna stringa utente hardcoded:
```

Se un punto della verifica NVDA fallisce, il report deve indicare:

```text
punto fallito:
comportamento atteso:
comportamento osservato:
correzione applicata:
oppure motivo del rinvio:
```

---

## 30. Gestione problemi durante implementazione

Se durante l’implementazione emerge un problema non previsto, l’agente deve classificarlo così:

```text
A. Problema risolvibile dentro il DESIGN 008
B. Problema che richiede chiarimento del project owner
C. Problema che richiede modifica del design
D. Problema fuori perimetro da rimandare a blocco futuro
```

Per i casi B, C e D l’agente non deve procedere autonomamente.

Deve fermarsi e riportare:

```text
problema rilevato
file coinvolti
perché non è coperto dal design
possibili opzioni
raccomandazione
```

---

## 31. Cosa non deve comparire nella soluzione finale

Nella soluzione finale non devono comparire:

```text
nuove classi nel Core per accessibilità
nuovi eventi Core per la UI accessibile
nuovi stati timer
nuovi comandi timer
riferimenti a NVDA nel Core
riferimenti a WPF nel Core
riferimenti a UI Automation nel Core
stringhe italiane hardcoded in XAML o ViewModel
annunci a ogni secondo
spostamenti focus a ogni tick
scorciatoie F5
scorciatoie Ctrl+T
notifiche Windows
sintesi vocale
nuove schermate
nuove impostazioni
nuove opzioni cicliche
```

---

## 32. Decisione finale

Questo coding plan è approvato come base per la scrittura del TODO operativo 008.

Stato finale:

```text
APPROVED
```

Versione finale approvata:

```text
0.2.0
```

Il documento autorizza solo la preparazione del TODO operativo.

Non autorizza ancora la codifica diretta senza TODO approvato.

---

# Sintesi operativa

Il Coding Plan 008 deve guidare l’agente a fare una cosa precisa:

```text
rendere accessibile la UI WPF esistente
senza cambiare la logica del timer
```

La sequenza corretta è:

```text
leggere
mappare
localizzare
rifinire ViewModel
rifinire XAML
verificare Tab e focus
verificare NVDA
build
test
report
```

Il blocco è corretto solo se migliora l’accessibilità reale senza allargare il progetto.

```
```
