# TODO 008 — Accessibilità UI WPF minima

## Metadati

* Tipo documento: TODO operativo
* Codice: 008
* Titolo: Accessibilità UI WPF minima
* Versione: 0.2.0
* Stato: APPROVATO
* Progetto: CicloTimer
* Repository: donato81/CicloTimer
* Data: 2026-06-05
* Design di riferimento: `docs/1-design/008-DESIGN_accessibilita-ui-wpf-minima.md`
* Coding plan di riferimento: `docs/2-coding-plans/008-CODING-PLAN_accessibilita-ui-wpf-minima.md`

---

## 1. Scopo del TODO

Questo documento traduce il Coding Plan 008 in una checklist operativa per la codifica reale.

Il blocco 008 deve rendere accessibile la UI WPF minima già introdotta dal blocco 007.

La UI deve diventare:

```text
usabile senza mouse
navigabile con Tab e Shift+Tab
leggibile da NVDA
chiara nei nomi dei controlli
prevedibile nel focus
comprensibile negli errori
capace di comunicare eventi importanti
silenziosa durante il countdown ordinario
````

Il blocco 008 non deve introdurre nuove funzionalità timer.

Il blocco 008 non deve modificare la logica Core, Bridge, Audio, Orchestrator o Runner, salvo micro-correzioni già previste dal Coding Plan e comunque documentate.

---

## 2. Stato iniziale richiesto

Prima di iniziare, verificare che siano presenti e chiusi i blocchi:

```text
001 — Core timer engine
002 — Bridge UI-logica
003 — Sistema testi / Localization
004 — Audio service e audio focus
005 — Orchestratore applicativo timer
006 — Gestore timer reale
006-bis — StateChanged Notification
007 — UI WPF minima
008 — DESIGN accessibilità UI WPF minima APPROVATO
008 — CODING PLAN accessibilità UI WPF minima APPROVED
```

Verificare che esistano:

```text
views/
view-models/
locales/
tests/
docs/1-design/008-DESIGN_accessibilita-ui-wpf-minima.md
docs/2-coding-plans/008-CODING-PLAN_accessibilita-ui-wpf-minima.md
```

Checklist:

* [ ] Confermare che Design 008 è presente.
* [ ] Confermare che Design 008 è APPROVATO.
* [ ] Confermare che Coding Plan 008 è presente.
* [ ] Confermare che Coding Plan 008 è APPROVED / APPROVATO.
* [ ] Confermare che la UI WPF minima esiste.
* [ ] Confermare che i test di partenza sono verdi.
* [ ] Se uno di questi elementi manca, fermarsi e segnalare.

---

## 3. Vincoli assoluti

Durante questo blocco non introdurre:

```text
nuove funzionalità timer
nuove regole Core
nuovi stati Core
nuovi eventi Core
nuova modalità timer ciclica
nuova CheckBox ciclica
scorciatoie F5
scorciatoie Ctrl+T
nuove scorciatoie locali
scorciatoie globali
notifiche Windows
sintesi vocale proprietaria
chiamate dirette a NVDA
dipendenze dirette da NVDA
annuncio del countdown ogni secondo
focus spostato a ogni tick
timer UI parallelo
polling UI aggiuntivo
redesign completo della finestra
tema scuro
installer
packaging
tray icon
stringhe utente hardcoded
```

Non modificare, salvo necessità documentata e approvata:

```text
models/CicloTimer.Core/
services/CicloTimer.Audio/
services/CicloTimer.App/Timing/
```

Modificare solo se strettamente necessario e dentro il perimetro:

```text
views/
view-models/
locales/
tests/
```

Se una modifica sembra richiedere di uscire da questi vincoli, fermarsi e segnalarla.

---

## 4. Preflight obbligatorio

Prima di creare o modificare file, eseguire questi controlli.

### 4.1 Verifica repository

* [ ] Eseguire `git status`.
* [ ] Verificare che il working tree sia pulito.
* [ ] Se il working tree non è pulito, fermarsi e segnalare.

### 4.2 Verifica solution

* [ ] Aprire la solution del repository.
* [ ] Verificare che i progetti compilino.
* [ ] Verificare che il progetto WPF in `views/` esista.
* [ ] Verificare che il progetto o cartella ViewModel in `view-models/` esista.
* [ ] Verificare che il progetto Localization in `locales/` esista.
* [ ] Verificare che i progetti siano coerenti con `.NET 9`.

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
* [ ] Annotare numero test iniziale.
* [ ] Numero atteso di partenza: 337 / 337, salvo aggiornamenti successivi.
* [ ] Se ci sono test rotti fuori perimetro, fermarsi e segnalare.

---

## 5. Lettura obbligatoria documenti

Prima di codificare, leggere integralmente:

```text
docs/0-architecture/document-standards.md
docs/0-architecture/architecture.md
docs/0-architecture/accessibility-rules.md
docs/0-architecture/internal-api.md
docs/1-design/008-DESIGN_accessibilita-ui-wpf-minima.md
docs/2-coding-plans/008-CODING-PLAN_accessibilita-ui-wpf-minima.md
```

Checklist:

* [ ] Confermare che il blocco 008 riguarda solo accessibilità della UI WPF minima.
* [ ] Confermare che non introduce nuove funzionalità timer.
* [ ] Confermare che il Core non deve conoscere WPF, NVDA o UI Automation.
* [ ] Confermare che la UI non deve calcolare la logica timer.
* [ ] Confermare che i testi utente e accessibili devono passare da Localization.
* [ ] Confermare che il countdown non deve essere annunciato ogni secondo.
* [ ] Confermare che il focus non deve essere spostato a ogni tick.
* [ ] Confermare che non devono essere introdotte scorciatoie nuove.
* [ ] Confermare che la verifica manuale NVDA è richiesta o va preparata per il project owner.

---

## 6. Ricognizione codice esistente

Aprire e leggere i file reali relativi a:

```text
finestra WPF principale
code-behind della finestra, se presente
ViewModel principale
NumericStepControl
Localization
test ViewModel
test Localization
```

Checklist:

* [ ] Annotare file XAML principale.
* [ ] Annotare file code-behind principale, se presente.
* [ ] Annotare file ViewModel principale.
* [ ] Annotare file `NumericStepControl`.
* [ ] Annotare file Localization rilevanti.
* [ ] Annotare test rilevanti.
* [ ] Non indovinare nomi di file.
* [ ] Non creare duplicati se esistono file equivalenti.
* [ ] Se la struttura reale impedisce il piano, fermarsi e segnalare.

---

## 7. Mappatura UI esistente

Mappare i controlli interattivi realmente presenti.

Controlli minimi attesi:

```text
Durata sessione, minuti
Durata sessione, secondi
Durata avviso finale, minuti
Durata avviso finale, secondi
Pulsante principale Avvia/Pausa/Riprendi
Reset
```

Mappare anche:

```text
tempo rimanente
stato corrente
sessioni completate
area messaggi evento, se presente
area errori, se presente
card o contenitori grafici
elementi decorativi
```

Per ogni controllo interattivo annotare:

```text
nome visivo:
nome accessibile attuale:
ruolo WPF/UI Automation:
è tabulabile:
ordine Tab:
stato abilitato/disabilitato:
binding al ViewModel:
```

Checklist:

* [ ] Mappa controlli completata.
* [ ] Mappa Tab order attuale completata.
* [ ] Mappa elementi informativi completata.
* [ ] Mappa elementi decorativi completata.
* [ ] Nessuna modifica eseguita prima della mappatura.

---

## 8. Mappatura Localization

Verificare come sono organizzati i testi esistenti in:

```text
locales/CicloTimer.Localization/
```

Checklist:

* [ ] Identificare chiavi UI esistenti.
* [ ] Identificare chiavi comandi esistenti.
* [ ] Identificare chiavi accessibilità esistenti.
* [ ] Identificare chiavi errori esistenti.
* [ ] Identificare chiavi eventi timer esistenti.
* [ ] Annotare testi mancanti.
* [ ] Non creare un secondo sistema di testi.
* [ ] Non inserire stringhe utente hardcoded in XAML.
* [ ] Non inserire stringhe utente hardcoded nel ViewModel.

---

## 9. Testi Localization da verificare o aggiungere

Verificare che esistano testi centralizzati per:

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

Per `NumericStepControl`, verificare o aggiungere solo se realmente usati:

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

Checklist:

* [ ] Usare solo il sistema Localization esistente.
* [ ] Aggiungere solo testi realmente necessari.
* [ ] Non aggiungere testi inutilizzati.
* [ ] Non aggiungere lingue nuove.
* [ ] Non duplicare chiavi già presenti.
* [ ] Mantenere testi in italiano.
* [ ] Aggiornare o aggiungere test Localization se presenti nel progetto.

---

## 10. ViewModel — dati necessari alla UI accessibile

Verificare se il ViewModel espone già:

```text
testo pulsante principale
stato abilitazione pulsante principale
stato abilitazione Reset
tempo rimanente formattato
stato corrente leggibile
sessioni completate formattate
ultimo messaggio evento
ultimo messaggio errore
testi accessibili dei controlli, se previsti dalla struttura esistente
```

Checklist:

* [ ] Verificare proprietà esistenti.
* [ ] Aggiungere solo proprietà necessarie alla UI accessibile.
* [ ] Usare Localization per testi utente.
* [ ] Non calcolare il tempo nel ViewModel.
* [ ] Non incrementare sessioni nel ViewModel.
* [ ] Non duplicare regole del Core.
* [ ] Non introdurre scorciatoie.
* [ ] Non introdurre nuova logica timer.
* [ ] Non inserire stringhe utente hardcoded.
* [ ] Se una proprietà necessaria richiede modifica fuori perimetro, fermarsi e segnalare.

---

## 11. Pulsante principale — coerenza stati

Verificare il pulsante principale negli stati:

```text
Timer fermo → Avvia
Sessione in corso → Pausa
Avviso finale in corso → Pausa
Timer in pausa → Riprendi
Sessione completata / ritorno a timer fermo → Avvia
```

Per ogni stato verificare coerenza tra:

```text
testo visivo
nome accessibile
comando eseguito
stato abilitato/disabilitato
```

Checklist:

* [ ] Pulsante corretto in stato fermo.
* [ ] Pulsante corretto in sessione in corso.
* [ ] Pulsante corretto in avviso finale.
* [ ] Pulsante corretto in pausa.
* [ ] Pulsante corretto dopo completamento sessione.
* [ ] Nome accessibile coerente con testo visivo.
* [ ] Comando eseguito coerente con stato.
* [ ] Stato abilitato/disabilitato coerente.
* [ ] Aggiungere test ViewModel se tecnicamente possibile.

---

## 12. XAML — nomi accessibili

Nel file XAML principale, verificare o impostare nomi accessibili chiari per:

```text
Durata sessione, minuti
Durata sessione, secondi
Durata avviso finale, minuti
Durata avviso finale, secondi
Pulsante principale Avvia/Pausa/Riprendi
Reset
```

Checklist:

* [ ] Ogni controllo interattivo ha nome accessibile chiaro.
* [ ] I campi numerici sono comprensibili fuori dal contesto visivo.
* [ ] I pulsanti espongono nome e ruolo corretti.
* [ ] I controlli disabilitati comunicano stato disabilitato tramite comportamento WPF standard.
* [ ] Nessun controllo viene letto come `Button`, `TextBox1`, `Input`, `Valore`, `+`, `-` senza contesto.
* [ ] Non inserire stringhe accessibili hardcoded.
* [ ] Usare meccanismi WPF standard compatibili con UI Automation.

---

## 13. Focus iniziale

Impostare il focus iniziale sul primo controllo utile.

Controllo previsto:

```text
Durata sessione, minuti
```

Checklist:

* [ ] Focus iniziale impostato nel livello UI WPF.
* [ ] Focus iniziale non richiede modifica al Core.
* [ ] Focus iniziale non avvia timer o comandi.
* [ ] Focus iniziale non causa annunci ripetuti.
* [ ] Titolo finestra comprensibile per NVDA.
* [ ] Verifica manuale prevista con NVDA.

---

## 14. Tab order

Impostare o correggere il Tab order secondo l’ordine:

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

Gli elementi informativi non devono essere tabulabili salvo necessità documentata:

```text
titolo
card
tempo rimanente
stato corrente
sessioni completate
messaggi evento
messaggi errore non interattivi
elementi decorativi
```

Checklist:

* [ ] Tab dall’inizio alla fine corretto.
* [ ] Shift+Tab dalla fine all’inizio corretto.
* [ ] Nessun salto inatteso.
* [ ] Nessun elemento decorativo nel ciclo Tab.
* [ ] Nessun blocco del focus.
* [ ] Il focus non esce in modo inatteso dalla UI applicativa.
* [ ] Se il comportamento reale lo richiede, usare una proprietà WPF standard come `KeyboardNavigation.TabNavigation="Cycle"` o una soluzione equivalente documentata.
* [ ] Non forzare nel Tab elementi solo informativi.

---

## 15. NumericStepControl

Verificare e rifinire `NumericStepControl`.

Il controllo deve:

```text
essere raggiungibile da tastiera
essere comprensibile da NVDA
comunicare il valore corrente
permettere aumento e diminuzione senza mouse
comportarsi come punto compatto nel flusso Tab
evitare pulsanti interni anonimi
evitare una sequenza Tab troppo lunga
non contenere logica timer
```

Regola operativa:

```text
ogni NumericStepControl deve apparire come un singolo punto chiaro di navigazione
```

Checklist:

* [ ] Focus principale chiaro sul valore numerico.
* [ ] Valore corrente comunicabile da NVDA.
* [ ] Aumento/diminuzione disponibili da tastiera.
* [ ] Preferire frecce direzionali se coerenti con il controllo reale.
* [ ] Evitare molteplici Tab stop interni per `+` e `-`.
* [ ] I pulsanti interni `+` e `-`, se presenti, non devono creare Tab stop aggiuntivi inutili.
* [ ] La soluzione preferita è che il valore numerico sia l’unico punto principale di navigazione Tab e che aumento/diminuzione siano gestibili da tastiera senza attraversare pulsanti interni separati.
* [ ] Se l’agente mantiene pulsanti interni tabulabili, deve motivarlo nel report e dimostrare con NVDA che non appesantiscono la navigazione.
* [ ] Se pulsanti interni restano raggiungibili, devono avere nome accessibile contestuale.
* [ ] Nessun pulsante viene letto solo come `+`, `-` o `Button`.
* [ ] Nessuna logica timer inserita nel controllo.
* [ ] Se il controllo esistente non è recuperabile in modo semplice, fermarsi e segnalare prima di riscriverlo.

---

## 16. Tempo rimanente

Verificare che il tempo rimanente sia leggibile ma non annunciato ogni secondo.

Checklist:

* [ ] Tempo rimanente visibile.
* [ ] Tempo rimanente leggibile da NVDA come informazione.
* [ ] Countdown continua ad aggiornarsi visivamente.
* [ ] Nessuna Live Region aggiornata a ogni secondo per il countdown.
* [ ] Nessun messaggio evento a ogni tick.
* [ ] Nessuno spostamento focus a ogni tick.
* [ ] Nessuna scorciatoia nuova per leggere il tempo.
* [ ] Nessun annuncio tipo `04:59`, `04:58`, `04:57` in sequenza automatica.

---

## 17. Stato corrente e sessioni completate

Verificare che stato corrente e sessioni completate siano leggibili.

Stati utente attesi:

```text
Stato: Timer fermo
Stato: Sessione in corso
Stato: Avviso finale in corso
Stato: Timer in pausa
```

Sessioni completate:

```text
Sessioni completate: 0
Sessioni completate: 1
Sessioni completate: 2
```

Checklist:

* [ ] Stato corrente visibile.
* [ ] Stato corrente leggibile da NVDA.
* [ ] Stato corrente non espone stati tecnici.
* [ ] Sessioni completate visibili.
* [ ] Sessioni completate leggibili da NVDA.
* [ ] UI non calcola questi valori.
* [ ] UI mostra dati già ricevuti da ViewModel / Bridge / Orchestrator.

---

## 18. Area eventi accessibile

Verificare se esiste un’area messaggi evento.

Se non esiste, crearla o introdurre meccanismo WPF equivalente coerente con il Design 008.

Eventi ammessi:

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

Checklist:

* [ ] Area evento presente o meccanismo equivalente presente.
* [ ] Ultimo evento importante mostrato o esposto.
* [ ] Evento leggibile da NVDA.
* [ ] Area evento non riceve focus automaticamente.
* [ ] Focus non viene spostato per annunciare evento.
* [ ] Area evento non viene aggiornata a ogni tick.
* [ ] Area evento non viene usata per il countdown.
* [ ] Annunci ordinari non aggressivi, equivalenti a comportamento Polite.
* [ ] Errori bloccanti possono essere più urgenti, equivalenti ad Assertive.
* [ ] Verificare con NVDA che eventi logicamente distinti con lo stesso testo consecutivo vengano effettivamente annunciati.
* [ ] Se il meccanismo scelto non ripete il testo identico, documentarlo nel report.
* [ ] Se il solo cambio testo non basta, valutare un fallback WPF/UI Automation equivalente e documentarlo.
* [ ] Non introdurre hack, sintesi vocale custom o chiamate dirette a NVDA per forzare l’annuncio.
* [ ] Non chiamare NVDA direttamente.
* [ ] Non usare sintesi vocale custom.

---

## 19. Errori accessibili

Verificare o implementare errori accessibili.

Esempi corretti:

```text
La durata della sessione deve essere maggiore di zero.
La durata dell’avviso finale deve essere minore della durata della sessione.
```

Checklist:

* [ ] Errore visibile.
* [ ] Errore leggibile da NVDA.
* [ ] Errore comprensibile.
* [ ] Errore non basato solo sul colore.
* [ ] Errore non basato solo su bordo rosso.
* [ ] Errore collegato al campo da correggere, se possibile.
* [ ] Focus permette correzione.
* [ ] Testo errore da Localization.
* [ ] Nessun messaggio generico tipo `Errore`, `Campo non valido`, `Invalid input`.
* [ ] Non duplicare regole logiche del Core nel ViewModel.

---

## 20. Rifinitura visiva minima

Sono ammesse solo rifiniture visive collegate all’accessibilità.

Checklist ammessa:

* [ ] Focus visuale più evidente, se necessario.
* [ ] Contrasto migliorato, se necessario.
* [ ] Spaziatura migliorata, se necessario.
* [ ] Messaggio errore visibile, se necessario.
* [ ] Stato non comunicato solo tramite colore.

Vietato:

* [ ] Non fare redesign completo.
* [ ] Non introdurre tema scuro.
* [ ] Non introdurre animazioni.
* [ ] Non introdurre nuova struttura finestra.
* [ ] Non introdurre nuove icone definitive.
* [ ] Non introdurre nuove schermate.

---

## 21. Test automatici da aggiungere o aggiornare

Eseguire sempre i test esistenti.

Comando:

```bash
dotnet test
```

Valutare aggiunta o aggiornamento di test per:

```text
testo pulsante principale nello stato fermo
testo pulsante principale nello stato in corso
testo pulsante principale nello stato pausa
testo e stato abilitazione pulsante principale dopo completamento sessione
coerenza stato timer → testo pulsante
messaggio evento dopo Start
messaggio evento dopo Pause
messaggio evento dopo Resume
messaggio evento dopo Reset
messaggio evento dopo completamento sessione, se già esposto
messaggio errore per configurazione non valida
testi Localization aggiunti
assenza di stringhe mancanti nelle chiavi aggiunte
```

Checklist:

* [ ] Test esistenti eseguiti.
* [ ] Test ViewModel aggiunti o aggiornati se tecnicamente possibile.
* [ ] Test Localization aggiunti o aggiornati se tecnicamente possibile.
* [ ] Eseguire `dotnet test` dopo ogni aggiunta o modifica significativa ai test, per verificare subito che i test nuovi siano validi.
* [ ] Nessun test fragile basato su dettagli visivi non stabili.
* [ ] Nessun test UI Automation complesso obbligatorio.
* [ ] Se un aspetto è solo manuale, dichiararlo nel report.

---

## 22. Build finale

Eseguire:

```bash
dotnet build
```

Checklist:

* [ ] Build finale completata.
* [ ] Nessun errore.
* [ ] Nessun warning nuovo rilevante, se applicabile.
* [ ] Se la build fallisce, correggere solo errori dentro perimetro.
* [ ] Se la build fallisce per cause fuori perimetro, fermarsi e segnalare.

---

## 23. Test finale

Eseguire:

```bash
dotnet test
```

Checklist:

* [ ] Test finali completati.
* [ ] Tutti i test passano.
* [ ] Annotare numero finale test.
* [ ] Se il numero test è aumentato, indicare quanti test sono stati aggiunti.
* [ ] Se un test fallisce per causa fuori perimetro, fermarsi e segnalare.

---

## 24. Verifica manuale NVDA

Il blocco 008 richiede verifica manuale con NVDA.

Checklist:

* [ ] Aprire l’app.
* [ ] Verificare annuncio titolo finestra.
* [ ] Verificare focus iniziale su “Durata sessione, minuti”.
* [ ] Navigare con Tab dall’inizio alla fine.
* [ ] Navigare con Shift+Tab dalla fine all’inizio.
* [ ] Verificare che il focus non esca in modo inatteso dalla UI applicativa.
* [ ] Leggere durata sessione minuti.
* [ ] Leggere durata sessione secondi.
* [ ] Leggere durata avviso finale minuti.
* [ ] Leggere durata avviso finale secondi.
* [ ] Modificare valori da tastiera.
* [ ] Verificare che `NumericStepControl` non appesantisca il Tab.
* [ ] Avviare il timer.
* [ ] Verificare che il pulsante diventi Pausa.
* [ ] Mettere in pausa.
* [ ] Verificare che il pulsante diventi Riprendi.
* [ ] Riprendere.
* [ ] Verificare che il pulsante torni Pausa.
* [ ] Resettare.
* [ ] Leggere stato corrente.
* [ ] Leggere tempo rimanente.
* [ ] Verificare che il tempo non venga annunciato ogni secondo.
* [ ] Leggere sessioni completate.
* [ ] Generare o verificare un errore accessibile.
* [ ] Verificare che il focus aiuti la correzione.
* [ ] Verificare che gli eventi importanti siano percepibili.
* [ ] Verificare con NVDA che eventi logicamente distinti con lo stesso testo consecutivo vengano effettivamente annunciati, se applicabile.
* [ ] Verificare che gli annunci ordinari non interrompano aggressivamente.
* [ ] Chiudere l’app.
* [ ] Verificare che la chiusura dell’app, ad esempio con Alt+F4, avvenga in modo pulito, senza errori, crash o comportamenti anomali percepibili da NVDA.

Se un punto fallisce, annotare:

```text
punto fallito:
comportamento atteso:
comportamento osservato:
correzione applicata:
oppure motivo del rinvio:
```

Se l’agente non può usare NVDA direttamente:

* [ ] Dichiararlo chiaramente.
* [ ] Non fingere verifica NVDA.
* [ ] Fornire checklist pronta per il project owner.
* [ ] Indicare quali parti sono state verificate solo da codice/build/test.

---

## 25. Verifica UI Automation, se disponibile

Se sono disponibili strumenti adeguati, verificare l’albero UI Automation della finestra.

Checklist:

* [ ] Controlli interattivi con nomi comprensibili.
* [ ] Ruoli dei controlli coerenti.
* [ ] Elementi decorativi non disturbano.
* [ ] `NumericStepControl` esposto in modo comprensibile.
* [ ] Area messaggi evento individuabile.
* [ ] Errori leggibili.

Se strumenti non disponibili:

* [ ] Dichiararlo nel report.
* [ ] Specificare che la verifica è stata limitata a codice, build, test e checklist NVDA.

Non aggiungere dipendenze esterne non necessarie solo per questa verifica.

---

## 26. Controlli anti-regressione

Prima del report finale, verificare esplicitamente:

```text
Core non modificato
logica timer non modificata
runner temporale non modificato
audio non modificato
nessuna scorciatoia aggiunta
nessuna nuova funzionalità timer
nessun annuncio countdown ogni secondo
nessuna dipendenza diretta da NVDA
nessuna stringa utente hardcoded
nessuna nuova opzione ciclica
nessuna notifica Windows
nessuna sintesi vocale custom
```

Checklist:

* [ ] Conferme anti-regressione completate.
* [ ] Ogni eventuale eccezione documentata.
* [ ] Se una conferma non può essere data, fermarsi e spiegare.

---

## 27. Report finale richiesto

Produrre un report finale con questa struttura:

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

Nella sezione “Conferme di perimetro” dichiarare:

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

Checklist:

* [ ] Report finale prodotto.
* [ ] File modificati elencati.
* [ ] Build indicata.
* [ ] Test indicati.
* [ ] Numero test finale indicato.
* [ ] Verifica NVDA indicata oppure dichiarata non eseguibile dall’agente.
* [ ] Problemi residui indicati.
* [ ] Conferme di perimetro compilate.

---

## 28. Gestione problemi durante implementazione

Se emerge un problema non previsto, classificarlo così:

```text
A. Problema risolvibile dentro il DESIGN 008
B. Problema che richiede chiarimento del project owner
C. Problema che richiede modifica del design
D. Problema fuori perimetro da rimandare a blocco futuro
```

Per i casi B, C e D:

* [ ] Non procedere autonomamente.
* [ ] Fermarsi.
* [ ] Segnalare problema.
* [ ] Indicare file coinvolti.
* [ ] Spiegare perché non è coperto dal design.
* [ ] Indicare possibili opzioni.
* [ ] Dare raccomandazione.

---

## 29. Cosa non deve comparire nella soluzione finale

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

Checklist:

* [ ] Verifica finale completata.
* [ ] Nessuno degli elementi vietati è presente.
* [ ] Eventuali anomalie segnalate.

---

## 30. Criteri di completamento

Il TODO 008 è completato solo se:

* [ ] Documenti obbligatori letti.
* [ ] Repository pulito prima dell’avvio.
* [ ] Build iniziale eseguita.
* [ ] Test iniziali eseguiti.
* [ ] UI mappata.
* [ ] Localization mappata.
* [ ] Testi necessari aggiunti o verificati.
* [ ] ViewModel rifinito solo se necessario.
* [ ] Nomi accessibili impostati o verificati.
* [ ] Focus iniziale corretto.
* [ ] Tab order corretto.
* [ ] Focus non esce in modo inatteso dalla UI.
* [ ] `NumericStepControl` usabile da tastiera.
* [ ] Tempo rimanente leggibile ma non annunciato ogni secondo.
* [ ] Stato corrente leggibile.
* [ ] Sessioni completate leggibili.
* [ ] Area eventi accessibile presente o equivalente.
* [ ] Eventi identici consecutivi gestiti se applicabile.
* [ ] Errori accessibili.
* [ ] Test automatici aggiunti o aggiornati dove realistico.
* [ ] Build finale verde.
* [ ] Test finali verdi.
* [ ] Verifica NVDA eseguita o dichiarata non eseguibile dall’agente.
* [ ] Chiusura app verificata come pulita con NVDA oppure inclusa nella checklist per il project owner.
* [ ] Verifica UI Automation eseguita se disponibile.
* [ ] Report finale prodotto.
* [ ] Conferme di perimetro compilate.

---

## 31. Decisione finale

Questo TODO è approvato e può guidare la codifica del blocco 008.

Stato finale:

```text
APPROVATO
```

Versione finale approvata:

```text
0.2.0
```

La codifica deve comunque rispettare integralmente:

```text
docs/1-design/008-DESIGN_accessibilita-ui-wpf-minima.md
docs/2-coding-plans/008-CODING-PLAN_accessibilita-ui-wpf-minima.md
docs/3-todos/008-TODO_accessibilita-ui-wpf-minima.md
```

---

# Sintesi operativa

Il TODO 008 deve guidare Cline / Roo Code a fare una cosa precisa:

```text
rendere accessibile la UI WPF esistente
senza cambiare la logica del timer
```

La sequenza corretta è:

```text
preflight
lettura documenti
mappatura codice
Localization
ViewModel
XAML
focus
Tab
NumericStepControl
eventi accessibili
errori accessibili
test
build
NVDA
report
```

Il blocco è corretto solo se migliora l’accessibilità reale senza allargare il progetto.
