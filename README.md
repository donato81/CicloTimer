# CicloTimer

**Versione corrente:** 0.8.4  
**Stato:** build di test  
**Piattaforma:** Windows  
**Tecnologia:** .NET 9, WPF, C#

---

## Cos'è CicloTimer

CicloTimer è una piccola applicazione desktop per Windows che permette di usare un timer a sessioni ripetute.

L'utente imposta:

```text
durata della sessione
durata dell'avviso finale
````

Poi può:

```text
avviare il timer
metterlo in pausa
riprenderlo
resettarlo
seguire il tempo rimanente
vedere lo stato corrente
vedere le sessioni completate
```

L'app nasce come progetto semplice, ma costruito con attenzione all'architettura, alla separazione delle responsabilità e all'accessibilità.

---

## Obiettivo del progetto

L'obiettivo di CicloTimer è fornire un timer ciclico essenziale, chiaro e utilizzabile.

Il progetto è pensato per:

```text
uso personale
test manuale
sessioni temporizzate
attività ripetute
prove di accessibilità su desktop Windows
```

Esempi pratici:

```text
impostare una sessione da 25 minuti
ricevere un avviso finale negli ultimi secondi
lasciare che il timer riparta automaticamente
tenere il conto delle sessioni completate
```

---

## Stato attuale

La versione attuale è:

```text
0.8.4
```

Questa versione contiene i blocchi funzionali da 001 a 008.

In pratica, l'app oggi dispone di:

```text
motore timer funzionante
bridge tra logica e UI
testi centralizzati
servizio audio
orchestratore applicativo
runner temporale reale
interfaccia WPF minima
accessibilità base per tastiera e NVDA
riepilogo timer leggibile da tastiera
```

La build è considerata adatta a una prima consegna di test.

Non è ancora una versione definitiva.

---

## Funzionalità disponibili

### Timer

Il timer permette di configurare una durata composta da:

```text
minuti
secondi
```

Durante l'esecuzione mostra:

```text
tempo rimanente
stato corrente
sessioni completate
```

---

### Avviso finale

È possibile impostare una durata per l'avviso finale.

Quando il timer entra nella fase finale, l'app può riprodurre un avviso sonoro.

---

### Comandi principali

L'app espone i seguenti comandi:

```text
Avvia
Pausa
Riprendi
Reset
```

Il pulsante principale cambia significato in base allo stato del timer.

Esempio:

```text
timer fermo       → Avvia
timer in corso    → Pausa
timer in pausa    → Riprendi
```

---

### Sessioni completate

L'app tiene traccia delle sessioni completate.

Il contatore viene mostrato nella UI e incluso nel riepilogo accessibile.

---

## Accessibilità

CicloTimer include una prima base di accessibilità per uso da tastiera e screen reader.

Il riferimento principale è:

```text
NVDA su Windows
```

### Navigazione da tastiera

La UI è navigabile con Tab.

L'ordine previsto è:

```text
durata sessione, minuti
durata sessione, secondi
durata avviso finale
Avvia / Pausa / Riprendi
Reset
Riepilogo timer
```

---

### Controlli numerici

I controlli numerici possono essere modificati da tastiera.

Comandi supportati:

```text
Freccia destra o Freccia su      → aumenta il valore
Freccia sinistra o Freccia giù   → diminuisce il valore
PageUp                           → aumenta più velocemente
PageDown                         → diminuisce più velocemente
Home                             → valore minimo
End                              → valore massimo
```

I controlli numerici sono pensati per essere letti da NVDA con nome e valore.

Esempio:

```text
Durata sessione, minuti 5
```

---

### Riepilogo timer

La UI contiene un punto raggiungibile con Tab chiamato:

```text
Riepilogo timer
```

Questo riepilogo permette a NVDA di leggere:

```text
tempo rimanente
stato corrente
sessioni completate
```

Esempio:

```text
Tempo rimanente: 04:52. Stato: sessione in corso. Sessioni completate: 0.
```

Il riepilogo non annuncia automaticamente il countdown ogni secondo.

L'utente può raggiungerlo con Tab quando vuole ascoltare lo stato del timer.

---

## Architettura

Il progetto è organizzato in layer separati.

Questa separazione serve a evitare che la logica del timer venga mescolata con UI, audio o accessibilità.

---

### Core

Cartella:

```text
models/CicloTimer.Core
```

Responsabilità:

```text
motore timer puro
stati del timer
transizioni
regole di dominio
conteggio sessioni
```

Il Core non conosce:

```text
WPF
UI
audio
NVDA
screen reader
testi utente
```

---

### Bridge

Cartella:

```text
view-models/CicloTimer.Bridge
```

Responsabilità:

```text
adattamento tra Core e livelli applicativi
modelli di input
modelli di output/display
```

---

### Localization

Cartella:

```text
locales/CicloTimer.Localization
```

Responsabilità:

```text
testi utente
etichette
messaggi
testi accessibili
messaggi errore
```

---

### Audio

Cartella:

```text
services/CicloTimer.Audio
```

Responsabilità:

```text
riproduzione suoni
avviso finale
gestione audio Windows
```

---

### App Layer

Cartella:

```text
services/CicloTimer.App
```

Responsabilità:

```text
orchestrazione applicativa
collegamento tra timer, runner e servizi
azioni applicative
```

---

### ViewModels

Cartella:

```text
view-models/CicloTimer.ViewModels
```

Responsabilità:

```text
stato presentabile dalla UI
comandi
binding WPF
testi accessibili esposti alla UI
```

---

### UI WPF

Cartella:

```text
views/ciclotimer
```

Responsabilità:

```text
finestra desktop
layout WPF
interazione utente
focus
Tab order
proprietà accessibili WPF
```

---

## Struttura principale del repository

```text
docs/
models/
view-models/
locales/
services/
views/
tests/
CHANGELOG.md
README.md
```

### Documentazione

```text
docs/0-architecture/
```

Contiene le regole architetturali e i vincoli del progetto.

```text
docs/1-design/
```

Contiene i documenti di design approvati.

```text
docs/2-coding-plans/
```

Contiene i piani di codifica.

```text
docs/3-todos/
```

Contiene i TODO operativi.

---

## Metodo di sviluppo

Il progetto segue un metodo documentale rigoroso.

Il flusso previsto è:

```text
Design
↓
Coding Plan
↓
TODO operativo
↓
Validazione
↓
Codifica
↓
Build/Test
↓
Review finale
↓
Commit
↓
Push
```

Le modifiche non vengono introdotte liberamente nel codice.

Prima vengono progettate e documentate.

---

## Versionamento

Il progetto usa una versione semantica semplice:

```text
MAJOR.MINOR.PATCH
```

Dove:

```text
MAJOR = 0 finché l'app non è considerata stabile definitiva
MINOR = blocco funzionale principale raggiunto
PATCH = correzioni successive sul blocco corrente
```

La versione corrente è:

```text
0.8.4
```

Significato:

```text
0 = progetto non ancora definitivo
8 = completati i blocchi funzionali 001–008
4 = correzioni successive importanti sul blocco 008
```

Il dettaglio delle versioni è documentato in:

```text
CHANGELOG.md
```

---

## Requisiti

Per compilare ed eseguire l'app servono:

```text
Windows
.NET 9 SDK
```

Ambiente di sviluppo usato nel progetto:

```text
Windows
Visual Studio Code
GitHub Desktop
NVDA
```

---

## Avvio dell'app

Dalla cartella principale del repository, usare:

```powershell
dotnet run --project .\views\ciclotimer\ciclotimer.csproj
```

---

## Build

Dalla cartella principale del repository:

```powershell
dotnet build
```

---

## Test

Dalla cartella principale del repository:

```powershell
dotnet test
```

Al momento della versione 0.8.4 risultano:

```text
337 test passati
0 test falliti
```

---

## Stato della build 0.8.4

La versione 0.8.4 include:

```text
build verde
test automatici verdi
UI WPF avviabile
timer funzionante
audio finale integrato
controlli base accessibili da tastiera
riepilogo timer accessibile
```

---

## Limitazioni note

Questa versione non include ancora:

```text
scelta tra timer ciclico e timer a singola esecuzione
installer Windows
icona definitiva
tema grafico definitivo
salvataggio preferenze
notifiche Windows toast
pacchetto di distribuzione finale
```

---

### Modalità ciclica

Nella versione attuale il timer segue il comportamento ciclico già implementato.

Non esiste ancora un controllo nella UI per scegliere tra:

```text
una sola esecuzione
ripetizione automatica
```

Questa funzionalità è prevista come possibile blocco futuro.

---

## Roadmap possibile

Possibili sviluppi futuri:

```text
modalità timer singola / ciclica
installer Windows
icona applicazione
miglioramento UI visiva
salvataggio impostazioni
pacchetto di distribuzione
ulteriore validazione accessibilità
```

Il blocco futuro più importante è:

```text
009 — Modalità timer singola / ciclica
```

Obiettivo:

```text
permettere all'utente di scegliere se il timer deve fermarsi dopo una sessione
oppure ripartire automaticamente
```

---

## Destinatari della build di test

La build corrente può essere consegnata per una prima prova manuale.

Lo scopo del test è capire:

```text
se il timer è utile
se la durata è facile da impostare
se pausa/ripresa/reset sono chiari
se l'avviso finale è comprensibile
se il comportamento ciclico è adatto oppure va reso configurabile
```

---

## Note sull'accessibilità

L'accessibilità è stata curata principalmente per:

```text
uso da tastiera
NVDA
navigazione con Tab
lettura dei controlli principali
lettura del riepilogo timer
```

La validazione finale resta pratica: l'app deve essere provata realmente con NVDA.

---

## Licenza

Licenza non ancora definita.

---

## Stato finale

CicloTimer 0.8.4 è una build di test funzionante.

Non è una versione definitiva, ma rappresenta una base concreta per:

```text
prova utente
raccolta feedback
decisione sulle prossime funzionalità
```
