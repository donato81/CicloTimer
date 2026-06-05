# Changelog

Tutte le modifiche rilevanti del progetto CicloTimer sono documentate in questo file.

Il progetto usa una versione semantica semplice:

```text
MAJOR.MINOR.PATCH
````

Dove:

```text
MAJOR = 0 finché l'app non è considerata stabile definitiva
MINOR = blocco funzionale principale raggiunto
PATCH = correzioni o revisioni successive sul blocco corrente
```

La versione corrente è:

```text
0.8.4
```

Motivazione:

```text
0 = progetto non ancora definitivo
8 = completati i blocchi funzionali 001–008
4 = correzioni successive importanti sul blocco 008, soprattutto accessibilità e riepilogo timer
```

---

## [0.8.4] - 2026-06-05

### Stato della versione

Prima build testabile di CicloTimer con:

```text
motore timer funzionante
UI WPF minima
audio finale
accessibilità base per NVDA
riepilogo timer leggibile da tastiera
```

Questa versione è adatta a una prima prova manuale da parte di un utente esterno.

Non è ancora una versione definitiva.

---

### Aggiunto

#### Blocco 001 — Core timer engine

Aggiunto il motore principale del timer.

Funzioni principali:

```text
gestione durata sessione
gestione durata avviso finale
stati del timer
transizioni del timer
conteggio delle sessioni completate
```

Il Core contiene la logica pura del timer.

Il Core non conosce:

```text
UI
WPF
audio
screen reader
NVDA
testi utente
```

---

#### Blocco 002 — Bridge UI-logica

Aggiunto il livello Bridge tra logica pura e livelli superiori.

Responsabilità principali:

```text
adattare input utente verso il Core
convertire dati del Core in dati usabili dalla UI
mantenere separazione tra motore e interfaccia
```

Introdotto il modello di input timer usato dalla UI.

---

#### Blocco 003 — Localization

Aggiunto il sistema centralizzato dei testi.

Responsabilità principali:

```text
testi visibili all'utente
etichette UI
messaggi di errore
messaggi accessibili
testi italiani
```

Da questa versione i testi utente non devono essere dispersi nel codice.

---

#### Blocco 004 — Audio service

Aggiunto il servizio audio.

Funzioni principali:

```text
riproduzione audio di avviso finale
gestione file audio
separazione audio dalla UI
```

Il servizio audio è separato dal Core e dalla UI.

---

#### Blocco 005 — Timer App Orchestrator

Aggiunto l'orchestratore applicativo del timer.

Responsabilità principali:

```text
Configure
Start
Pause
Resume
Reset
Tick
```

L'orchestratore collega la logica del timer al comportamento applicativo.

---

#### Blocco 006 — Realtime Timer Runner

Aggiunto il runner temporale reale.

Responsabilità principali:

```text
generare il passare del tempo reale
inviare Tick all'orchestratore
mantenere il timer in avanzamento durante l'esecuzione
```

---

#### Blocco 006-bis — StateChanged Notification

Aggiunto evento di notifica cambio stato.

Motivazione:

```text
la UI doveva sapere quando lo stato del timer cambiava
```

Aggiunto evento:

```text
StateChanged
```

su:

```text
ITimerAppOrchestrator
```

Aggiunto modello evento:

```text
TimerAppStateChangedEventArgs
```

La UI può ora aggiornarsi dopo modifiche di stato e Tick.

---

#### Blocco 007 — UI WPF minima

Aggiunta la prima UI desktop WPF.

Elementi principali:

```text
finestra principale
layout a card
tempo rimanente centrale
stato timer
sessioni completate
controlli durata sessione
controllo avviso finale
pulsante Avvia/Pausa/Riprendi
pulsante Reset
NumericStepControl
```

La UI permette:

```text
impostazione durata timer
impostazione durata avviso finale
avvio timer
pausa timer
ripresa timer
reset timer
visualizzazione stato
visualizzazione sessioni completate
```

Scelta importante:

```text
non è stata aggiunta una casella per scegliere timer ciclico/non ciclico
```

Motivazione:

```text
il modello TimerInput non contiene ancora un'opzione per la modalità ciclica
```

Questa funzionalità è rimandata a un blocco futuro.

---

#### Blocco 008 — Accessibilità UI WPF minima

Aggiunta accessibilità base della UI WPF.

Obiettivi principali:

```text
uso da tastiera
ordine Tab logico
focus iniziale prevedibile
nomi accessibili per i controlli
compatibilità pratica con NVDA
errori accessibili
eventi accessibili
riepilogo timer leggibile
```

Interventi principali:

```text
focus iniziale sul primo controllo utile
Tab order definito
NumericStepControl raggiungibile da Tab
NumericStepControl modificabile da tastiera
nomi accessibili contestuali
area eventi accessibile
messaggi errore accessibili
riepilogo timer raggiungibile da Tab
```

Il riepilogo timer permette di leggere con NVDA:

```text
tempo rimanente
stato corrente
sessioni completate
```

senza annunciare automaticamente il countdown ogni secondo.

---

### Corretto

#### Accessibilità NumericStepControl

Corretto il comportamento del controllo numerico.

Problemi risolti:

```text
il controllo non era raggiungibile correttamente con Tab
il valore non veniva letto in modo utile da NVDA
il valore non era modificabile comodamente da tastiera
i pulsanti + e - appesantivano la navigazione
```

Comportamento attuale:

```text
Tab raggiunge ogni controllo numerico come punto logico
NVDA legge il nome del controllo e il valore
Freccia destra o su incrementa il valore
Freccia sinistra o giù decrementa il valore
PageUp/PageDown modificano più velocemente
Home/End portano al minimo/massimo
```

---

#### Riepilogo timer accessibile

Aggiunto un punto raggiungibile da Tab per leggere lo stato del timer.

Problema risolto:

```text
tempo rimanente, stato e sessioni completate erano visibili,
ma non facilmente raggiungibili da tastiera con NVDA
```

Comportamento attuale:

```text
Tab raggiunge il riepilogo timer
NVDA può leggere tempo rimanente, stato corrente e sessioni completate
il countdown non viene annunciato automaticamente ogni secondo
```

---

#### Eventi e messaggi accessibili

Aggiunti o rifiniti messaggi per eventi principali:

```text
timer avviato
timer messo in pausa
timer ripreso
timer resettato
avviso finale
sessione completata
errori di configurazione
```

Gli eventi ordinari non devono interrompere in modo aggressivo l'utente.

Gli errori devono essere più evidenti e comprensibili.

---

### Modificato

#### UI WPF

La UI mantiene la struttura minima introdotta nel blocco 007, ma ora è più adatta all'uso da tastiera.

Migliorie principali:

```text
navigazione Tab più ordinata
controlli numerici più accessibili
pulsanti con nomi accessibili coerenti
riepilogo timer consultabile da tastiera
```

Non è stato eseguito un redesign completo.

---

#### ViewModel

Il ViewModel è stato esteso per esporre testi utili alla UI accessibile.

Aggiunte o rifinite proprietà per:

```text
nomi accessibili
messaggi evento
messaggi errore
riepilogo timer accessibile
```

La logica del timer non è stata spostata nel ViewModel.

---

#### Localization

Aggiornati o usati testi localizzati per:

```text
etichette UI
nomi accessibili
stati timer
messaggi evento
messaggi errore
riepilogo timer
```

I testi utente restano centralizzati.

---

### Verificato

Per questa versione risultano verificati:

```text
build completata con successo
test automatici passati
337 test passati
UI avviabile
timer avviabile
pausa funzionante
ripresa funzionante
reset funzionante
controlli numerici modificabili da tastiera
riepilogo timer raggiungibile con Tab
```

---

### Limitazioni note

Questa versione non include ancora:

```text
scelta tra timer a singola esecuzione e timer ciclico
installer Windows
icona definitiva dell'app
tema grafico definitivo
sistema di impostazioni persistenti
salvataggio preferenze utente
notifiche Windows toast
```

Nota importante sulla modalità timer:

```text
in questa versione il timer lavora secondo il comportamento ciclico attuale.
La possibilità di scegliere tra una sola esecuzione e ripetizione automatica sarà valutata in un blocco futuro.
```

---

### Compatibilità

Ambiente previsto:

```text
Windows
.NET 9
WPF
```

Accessibilità testata principalmente con:

```text
NVDA
tastiera
navigazione Tab
```

---

## [0.8.3] - 2026-06-05

### Corretto

Correzione accessibilità del riepilogo timer.

Il riepilogo timer è stato reso raggiungibile tramite Tab usando un controllo realmente focusabile.

Motivazione:

```text
la prima soluzione con Label non risultava raggiungibile in modo affidabile da NVDA
```

---

## [0.8.2] - 2026-06-05

### Aggiunto

Aggiunta proprietà riepilogativa accessibile per il timer.

Introdotto un riepilogo contenente:

```text
tempo rimanente
stato corrente
sessioni completate
```

Obiettivo:

```text
permettere a NVDA di leggere lo stato complessivo del timer su richiesta dell'utente
```

---

## [0.8.1] - 2026-06-05

### Corretto

Corretto `NumericStepControl`.

Migliorie:

```text
raggiungibile con Tab
valore letto insieme al nome del controllo
modificabile con frecce da tastiera
pulsanti interni + e - esclusi dal flusso Tab principale
```

---

## [0.8.0] - 2026-06-05

### Aggiunto

Prima implementazione del blocco 008.

Accessibilità UI WPF minima:

```text
focus iniziale
ordine Tab
nomi accessibili
area eventi accessibile
messaggi errore accessibili
prime modifiche a NumericStepControl
```

---

## [0.7.0] - 2026-06-05

### Aggiunto

Implementata UI WPF minima.

Elementi principali:

```text
finestra principale
layout a card
tempo centrale
stato timer
sessioni completate
controlli durata
pulsante principale Avvia/Pausa/Riprendi
pulsante Reset
NumericStepControl
```

---

## [0.6.1] - 2026-06-05

### Aggiunto

Aggiunta notifica `StateChanged`.

Motivazione:

```text
la UI aveva bisogno di essere informata quando lo stato del timer cambiava
```

---

## [0.6.0] - 2026-06-05

### Aggiunto

Implementato Realtime Timer Runner.

Il runner gestisce il passare reale del tempo e invia Tick all'orchestratore.

---

## [0.5.0] - 2026-06-05

### Aggiunto

Implementato Timer App Orchestrator.

Funzioni principali:

```text
Configure
Start
Pause
Resume
Reset
Tick
```

---

## [0.4.0] - 2026-06-05

### Aggiunto

Approvata e documentata la visione generale del progetto.

Definiti:

```text
obiettivo del progetto
principi architetturali
separazione dei layer
accessibilità come vincolo importante
metodo di lavoro documentale
```

Implementato anche il servizio audio e le prime basi applicative necessarie ai blocchi successivi.

---

## [0.3.0] - 2026-06-05

### Aggiunto

Implementato sistema Localization.

Obiettivo:

```text
centralizzare testi utente, etichette, errori e testi accessibili
```

---

## [0.2.0] - 2026-06-05

### Aggiunto

Implementato Bridge UI-logica.

Obiettivo:

```text
separare la logica pura del timer dai modelli usati dalla UI
```

---

## [0.1.0] - 2026-06-05

### Aggiunto

Implementato Core timer engine.

Funzioni principali:

```text
stati timer
transizioni
durata sessione
durata avviso finale
sessioni completate
regole base del dominio timer
```

---

# Roadmap futura

## Possibile versione 0.9.0

Funzionalità candidata:

```text
modalità timer singola / ciclica
```

Obiettivo:

```text
permettere all'utente di scegliere se il timer deve fermarsi dopo una sessione
oppure ripartire automaticamente in ciclo
```

Questa modifica richiederà un blocco dedicato perché non riguarda solo la UI, ma anche il contratto di input e la logica applicativa.

---

## Possibile versione 1.0.0

Prima versione stabile.

Criteri possibili:

```text
timer configurabile
modalità singola/ciclica disponibile
UI stabile
audio verificato
accessibilità sufficiente
README completo
CHANGELOG aggiornato
build distribuibile
feedback utente raccolto e integrato
```
