# CicloTimer — Standard documentali

**Tipo documento:** regole documentali di progetto  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-01  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md  

---

## 1. Scopo del documento

Questo documento definisce gli standard documentali del progetto CicloTimer.

Il suo compito è stabilire come devono essere scritti, revisionati, approvati e usati i documenti del progetto.

CicloTimer nasce anche come esperimento controllato di sviluppo assistito da agenti AI.

Per questo motivo la documentazione non è un elemento secondario.

La documentazione serve a:

1. prendere decisioni prima dell'implementazione;
2. mantenere chiaro il perimetro del progetto;
3. impedire espansioni non approvate;
4. guidare Cursor e altri agenti AI durante il lavoro operativo;
5. separare le decisioni architetturali dalla codifica;
6. rendere verificabile ogni fase del ciclo di sviluppo;
7. evitare che gli agenti AI inventino funzionalità, cartelle, classi o regole non approvate.

Questo documento non definisce:

1. la logica del timer;
2. l'accessibilità dell'app;
3. la struttura concreta del codice;
4. i contratti interni tra core, bridge e UI;
5. il layout grafico definitivo;
6. la strategia completa di test;
7. le scelte tecniche di implementazione.

Questi aspetti appartengono ad altri documenti.

---

## 2. Principio guida

Il principio guida della documentazione è:

```text
nessuna implementazione senza documento approvato
````

Ogni fase operativa deve essere preceduta da un documento adeguato al suo scopo.

Gli agenti AI non devono usare il codice come luogo in cui prendere decisioni architetturali non documentate.

Le decisioni principali devono essere prima scritte, revisionate e approvate.

Solo dopo possono essere trasformate in codice.

Questo principio non deve però essere interpretato come burocrazia cieca.

Piccole correzioni di bug, refactoring interni che non modificano il comportamento osservabile dall'utente, correzioni di refusi o modifiche di implementazione già coperte da un design approvato non richiedono un nuovo documento.

Queste modifiche devono comunque:

1. restare coerenti con i documenti approvati;
2. non cambiare architettura;
3. non cambiare comportamento funzionale;
4. non peggiorare accessibilità;
5. non introdurre nuove stringhe utente non controllate;
6. essere descritte almeno nel messaggio di commit o nella nota operativa usata per eseguire la modifica.

Ogni modifica che altera comportamento, accessibilità, architettura, testi utente, errori o perimetro funzionale richiede invece un documento adeguato.

---

## 3. Ciclo documentale del progetto

Il ciclo documentale previsto per CicloTimer è:

```text
Visione
↓
Architettura
↓
Regole documentali
↓
Regole di accessibilità
↓
API interna / contratti interni
↓
Design
↓
Coding plan
↓
Todo operativo
↓
Implementazione
↓
Verifica
```

Questo ciclo è orientativo e non deve essere interpretato come una sequenza burocratica rigida.

L'ordine può essere adattato dal project owner quando il progetto lo richiede, purché ogni fase operativa sia guidata da documenti approvati sufficienti per il lavoro da svolgere.

Alcune fasi possono essere saltate, accorpate o rimandate se il contenuto è già coperto da documenti esistenti o se il project owner approva esplicitamente l'adattamento.

Nel progetto reale, alcuni documenti possono quindi essere creati in ordine diverso rispetto allo schema generale.

Questo non invalida il ciclo, purché restino validi questi principi:

1. le decisioni importanti vengono documentate prima dell'implementazione;
2. ogni documento resta coerente con quelli già approvati;
3. il project owner mantiene il controllo del perimetro;
4. gli agenti AI non assumono autonomia architetturale;
5. il lavoro operativo non parte senza un riferimento documentale adeguato.

Il flusso ordinario per i documenti fondativi è:

1. ChatGPT e project owner producono una prima bozza;
2. DeepSeek esegue una prima revisione critica;
3. Gemini esegue una seconda revisione critica;
4. ChatGPT integra, filtra e valida le osservazioni;
5. il project owner approva o richiede modifiche;
6. il documento approvato viene inserito nel repository;
7. solo dopo l'approvazione il documento può guidare i passaggi successivi.

Il ciclo completo con DeepSeek e Gemini è raccomandato per:

1. documenti fondativi;
2. documenti architetturali;
3. regole generali di progetto;
4. modifiche che cambiano il perimetro;
5. decisioni ad alto impatto;
6. correzioni di incoerenze tra documenti approvati.

Per documenti operativi e locali, come design circoscritti, coding plan, todo operativi e verifiche minori, il project owner può usare un flusso più snello.

Il flusso snello può prevedere:

```text
ChatGPT o Cursor
↓
verifica del project owner
↓
applicazione controllata
```

Il flusso snello è ammesso solo se il lavoro resta coerente con i documenti approvati e non introduce decisioni architetturali nuove.

Il consiglio AI ha funzione consultiva.

L'autorità finale sulle decisioni di progetto resta al project owner.

ChatGPT svolge il ruolo di consigliere principale e sintetizzatore finale.

DeepSeek e Gemini svolgono il ruolo di revisori.

Cursor e gli agenti di coding svolgono il ruolo di strumenti esecutivi.

---

## 4. Tipi di documento

Il progetto può contenere diversi tipi di documento.

Ogni documento deve dichiarare il proprio tipo nell'intestazione.

Tipi principali previsti:

```text
visione architetturale iniziale
architettura generale
regole documentali di progetto
regole architetturali di accessibilità
contratti interni / API interna
documento di design
coding plan
todo operativo
rapporto di revisione
rapporto di verifica
```

Ogni tipo ha uno scopo diverso.

Un documento non deve assumere responsabilità che appartengono a un altro tipo.

---

## 5. Visione architetturale iniziale

La visione definisce il senso generale del progetto.

Può descrivere:

1. scopo del progetto;
2. utente principale;
3. piattaforma di riferimento;
4. prima versione minima;
5. funzionalità incluse;
6. funzionalità escluse;
7. principi generali;
8. criteri di successo;
9. regola anti-espansione;
10. metodo di lavoro.

La visione non deve definire:

1. classi concrete;
2. struttura dettagliata delle cartelle di codice;
3. algoritmi definitivi;
4. XAML definitivo;
5. API interne dettagliate;
6. coding plan operativo;
7. todo implementativi.

La visione risponde alla domanda:

```text
che cosa stiamo costruendo e perché?
```

---

## 6. Architettura generale

L'architettura generale definisce la divisione logica del progetto.

Può descrivere:

1. livelli principali;
2. responsabilità di ogni livello;
3. dipendenze ammesse;
4. dipendenze vietate;
5. confini tra UI, bridge, core, sistema operativo, testi, errori e test;
6. principi tecnici generali;
7. regole anti-confusione.

L'architettura generale non deve definire:

1. implementazione concreta;
2. nomi definitivi di classi;
3. file esatti da modificare;
4. codice;
5. layout grafico finale;
6. todo operativi.

L'architettura generale risponde alla domanda:

```text
come deve essere organizzato logicamente il progetto?
```

---

## 7. Regole architetturali specializzate

Alcuni aspetti del progetto richiedono regole dedicate.

Esempi:

```text
accessibilità
standard documentali
API interna / contratti interni
gestione audio
test
errori
```

Questi documenti devono restare coerenti con la visione e l'architettura generale.

Non devono contraddirle.

Non devono introdurre funzionalità fuori perimetro.

Non devono trasformarsi in design tecnici dettagliati, salvo che il loro tipo lo preveda esplicitamente.

Una regola architetturale specializzata risponde alla domanda:

```text
quali vincoli generali devono guidare questo aspetto del progetto?
```

---

## 8. Documento di design

Un documento di design definisce una modifica o una parte specifica da implementare.

Ogni design deve avere un perimetro chiaro.

Un design può definire:

1. obiettivo della modifica;
2. file o cartelle coinvolte;
3. livelli architetturali toccati;
4. comportamento atteso;
5. vincoli;
6. cosa è fuori perimetro;
7. impatto su accessibilità;
8. impatto su testi utente;
9. impatto su errori;
10. impatto su test;
11. criteri di accettazione.

Un design non deve essere una richiesta generica di “migliorare il progetto”.

Un design non deve autorizzare modifiche non dichiarate.

Ogni design deve indicare chiaramente:

```text
questo documento può modificare...
questo documento non può modificare...
```

Il design risponde alla domanda:

```text
che cosa deve essere progettato prima di scrivere codice?
```

---

## 9. Coding plan

Il coding plan traduce un design approvato in un piano operativo per la codifica.

Il coding plan può definire:

1. ordine degli interventi;
2. file da creare;
3. file da modificare;
4. responsabilità di ogni file;
5. dipendenze tra passaggi;
6. rischi operativi;
7. controlli manuali o automatici da eseguire;
8. criteri per sapere che il lavoro è completato.

Il coding plan non deve cambiare il design.

Il coding plan non deve introdurre funzionalità nuove.

Il coding plan non deve allargare il perimetro.

Se durante il coding plan emerge una necessità non prevista dal design, il lavoro deve fermarsi e deve essere aggiornato il design o creato un nuovo documento.

Il coding plan risponde alla domanda:

```text
in quale ordine dobbiamo trasformare il design in codice?
```

---

## 10. Todo operativo

Il todo operativo è una lista di attività eseguibili.

Deve essere concreto e verificabile.

Ogni voce del todo dovrebbe indicare un'azione chiara.

Esempi corretti:

```text
Creare la classe TimerEngine.
Aggiungere il metodo Start.
Aggiungere test per configurazione non valida.
Aggiornare il bridge per formattare il tempo rimanente.
```

Esempi scorretti:

```text
Migliorare il timer.
Rendere tutto più accessibile.
Sistemare il progetto.
Ottimizzare il codice.
```

Il todo operativo non deve contenere decisioni architetturali nuove.

Il todo operativo non deve sostituire il design.

Ogni todo operativo dovrebbe restare piccolo, focalizzato e verificabile.

Se una lista diventa troppo lunga o copre obiettivi diversi, deve essere divisa in più todo separati.

La dimensione corretta di un todo non è definita da un numero fisso di voci, ma dalla sua verificabilità.

Un todo è troppo grande se:

1. contiene obiettivi indipendenti;
2. tocca troppi livelli senza necessità;
3. non permette di capire quando è completato;
4. contiene attività non autorizzate dal design o dal coding plan;
5. richiede decisioni architetturali durante l'esecuzione.

Il todo operativo risponde alla domanda:

```text
quali azioni concrete deve eseguire l'agente di coding?
```

---

## 11. Rapporto di revisione

Un rapporto di revisione serve a valutare un documento o una modifica.

Può essere prodotto da ChatGPT, DeepSeek, Gemini, Cursor o altri strumenti.

Un buon rapporto di revisione deve distinguere tra:

1. problemi bloccanti;
2. correzioni obbligatorie;
3. suggerimenti opzionali;
4. osservazioni fuori perimetro;
5. parti da mantenere;
6. giudizio finale.

Un rapporto di revisione non deve essere accettato automaticamente.

Le osservazioni dei revisori devono essere filtrate rispetto a:

1. visione approvata;
2. architettura approvata;
3. regole di accessibilità approvate;
4. standard documentali approvati;
5. perimetro del documento attivo;
6. semplicità della prima versione.

Il rapporto di revisione risponde alla domanda:

```text
il documento o la modifica è coerente, completa e sicura da approvare?
```

---

## 12. Rapporto di verifica

Un rapporto di verifica serve a controllare il risultato dopo una modifica.

Può riguardare:

1. codice prodotto;
2. documenti creati;
3. conformità al design;
4. conformità al coding plan;
5. conformità al todo;
6. accessibilità;
7. test;
8. regressioni;
9. errori introdotti;
10. funzionalità fuori perimetro.

Un rapporto di verifica non deve introdurre nuovi obiettivi.

Deve limitarsi a confrontare il risultato con ciò che era stato approvato.

Il rapporto di verifica risponde alla domanda:

```text
il risultato corrisponde a ciò che era stato autorizzato?
```

---

## 13. Intestazione obbligatoria dei documenti

Ogni documento ufficiale del progetto deve iniziare con un titolo chiaro.

Subito dopo deve contenere una intestazione strutturata.

Formato minimo:

```markdown
# CicloTimer — Titolo del documento

**Tipo documento:** ...
**Stato:** ...
**Versione:** ...
**Data:** ...
**Repository:** donato81/CicloTimer
**Documenti collegati:** ...
```

Il campo `Documenti collegati` è obbligatorio.

Se non esistono documenti precedenti direttamente collegati, indicare:

```text
**Documenti collegati:** Nessuno
```

In caso di documenti derivati, devono essere elencati i documenti padre approvati.

Esempio:

```text
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md
```

I documenti approvati devono indicare chiaramente il proprio stato.

I documenti in lavorazione devono restare marcati come `DRAFT`.

---

## 14. Stati dei documenti

Gli stati ammessi sono:

```text
DRAFT
IN REVIEW
APPROVED
SUPERSEDED
REJECTED
```

Significato:

```text
DRAFT
Documento in bozza, non ancora approvato.

IN REVIEW
Documento inviato ai revisori o in fase di valutazione.

APPROVED
Documento approvato dal project owner e valido come riferimento.

SUPERSEDED
Documento sostituito da una versione successiva o da un altro documento.

REJECTED
Documento respinto e non utilizzabile come riferimento.
```

Solo i documenti `APPROVED` possono guidare implementazioni o design successivi.

Un documento `DRAFT` può essere discusso, ma non deve essere usato da Cursor come base operativa definitiva.

---

## 15. Versionamento dei documenti

I documenti devono usare versioni semantiche semplici:

```text
0.1.0
0.2.0
0.3.0
1.0.0
```

Regola generale:

```text
0.x.0 = documento in evoluzione prima della stabilizzazione completa
1.0.0 = documento stabile e pienamente consolidato
```

Nel ciclo iniziale del progetto è accettabile approvare documenti in versione `0.x.0`, se il project owner li considera sufficientemente stabili per guidare la fase successiva.

`APPROVED` non significa necessariamente definitivo per sempre.

Un documento `0.x.0` può essere approvato come riferimento valido per la fase corrente, pur restando aggiornabile nelle versioni successive.

Ogni modifica significativa deve aggiornare la versione.

Esempi:

```text
0.1.0 — prima bozza
0.2.0 — integrazione revisione DeepSeek/Gemini
0.3.0 — correzione finale e approvazione
```

La cronologia delle versioni deve essere riportata nella sezione finale del documento.

---

## 16. Date

Le date nei documenti devono usare il formato:

```text
YYYY-MM-DD
```

Esempio:

```text
2026-06-01
```

La data indica il giorno della versione del documento.

Se un documento viene aggiornato in modo significativo, la data deve essere aggiornata.

---

## 17. Naming dei file

I file documentali devono usare nomi chiari, minuscoli e separati da trattini.

Esempi corretti:

```text
vision.md
architecture.md
accessibility-rules.md
document-standards.md
internal-api.md
```

Esempi da evitare:

```text
Documento Finale.md
nuovo_file.md
bozza2.md
architettura definitiva vera.md
accessibilita ultima versione.md
```

Il nome del file deve indicare il contenuto.

Il nome del file non deve dipendere dallo stato temporaneo della bozza.

Non usare nel nome del file parole come:

```text
finale
nuovo
vecchio
corretto
ultimo
test
bozza2
```

Lo stato deve stare dentro il documento, non nel nome del file.

---

## 18. Cartelle documentali

La documentazione deve essere organizzata per scopo.

Struttura logica prevista:

```text
docs/
0-architecture/
1-design/
2-coding-plans/
3-todos/
4-reviews/
5-verification/
```

La struttura fisica effettiva del repository può essere adattata, ma deve mantenere lo stesso principio:

```text
documenti architetturali separati dai design
design separati dai coding plan
coding plan separati dai todo
todo separati dalle verifiche
```

La cartella `docs/0-architecture/` contiene documenti fondativi e regole generali.

La cartella `docs/1-design/` conterrà documenti di design tecnico o funzionale.

La cartella `docs/2-coding-plans/` conterrà piani operativi di codifica.

La cartella `docs/3-todos/` conterrà liste di attività operative.

La cartella `docs/4-reviews/` potrà contenere rapporti dei revisori.

La cartella `docs/5-verification/` potrà contenere rapporti di verifica dopo implementazione.

Se la struttura reale del repository usa maiuscole o varianti di percorso, il principio logico resta invariato.

---

## 19. Linguaggio dei documenti

La lingua principale dei documenti è l'italiano.

I documenti devono usare frasi semplici, esplicite e non ambigue.

Sono da preferire:

```text
deve
non deve
può
non è richiesto
è fuori perimetro
sarà definito in un design successivo
```

Sono da evitare formule vaghe come:

```text
sarebbe bello
magari
si potrebbe fare tutto
migliorare quanto basta
gestire bene
ottimizzare
rendere moderno
```

Ogni regola deve essere verificabile.

Un agente AI deve poter leggere una frase e capire se una modifica la rispetta o la viola.

---

## 20. Uso di esempi

Gli esempi sono ammessi e utili.

Gli esempi devono essere chiaramente distinguibili dalle regole.

Formato consigliato:

```text
Esempio corretto:
...

Esempio scorretto:
...
```

Gli esempi non devono introdurre funzionalità non approvate.

Un esempio non deve essere interpretato come obbligo tecnico definitivo, salvo che il testo lo dica esplicitamente.

Se un esempio è solo illustrativo, deve essere chiaro dal contesto.

---

## 21. Cosa è fuori perimetro

Ogni documento di design o documento architetturale specializzato deve contenere, quando utile, una sezione dedicata al fuori perimetro.

La sezione fuori perimetro serve a proteggere il progetto da espansioni non controllate.

Esempi di fuori perimetro generali per la prima versione:

```text
cloud
account utente
database remoto
statistiche avanzate
grafici
gestione clienti
tariffe
fatturazione
mobile
web
plugin
AI integrata nell'app
sistemi audio avanzati non approvati
personalizzazioni complesse
```

Il fuori perimetro non significa “vietato per sempre”.

Significa:

```text
non autorizzato nella fase corrente
```

---

## 22. Regole anti-espansione

Ogni proposta deve essere valutata con questa domanda:

```text
serve direttamente alla prima versione minima del timer ciclico accessibile?
```

Se la risposta è no, la proposta deve essere rimandata.

Gli agenti AI non devono introdurre funzionalità secondarie per rendere la soluzione più elegante, moderna o completa.

La priorità del progetto è:

```text
piccolo
chiaro
accessibile
testabile
controllabile
```

La completezza funzionale avanzata viene dopo.

---

## 23. Regole anti-ambiguità per agenti AI

Ogni documento destinato a guidare agenti AI deve evitare ambiguità operative.

Quando possibile, deve specificare:

1. cosa si può fare;
2. cosa non si può fare;
3. quali livelli si possono toccare;
4. quali livelli non si possono toccare;
5. quali file o cartelle sono coinvolti;
6. quali funzionalità sono escluse;
7. quali criteri definiscono il successo;
8. quali criteri rendono il lavoro non valido.

Gli agenti AI devono essere trattati come esecutori potenti ma non autonomi nelle decisioni di progetto.

Un agente AI non deve dedurre nuovi requisiti dal contesto se non sono scritti nel documento attivo.

---

## 24. Rapporto tra documentazione e codice

Il codice deve seguire la documentazione approvata.

Se il codice e la documentazione entrano in conflitto, la documentazione approvata prevale sempre.

Se il codice appare corretto e la documentazione risulta obsoleta, si aggiorna prima la documentazione e poi si adegua il codice.

Se la documentazione risulta sbagliata, incompleta o non più adatta, deve essere aggiornata prima di usare il codice come nuova verità del progetto.

Non è ammesso correggere l'architettura “direttamente nel codice” senza aggiornare i documenti rilevanti.

---

## 25. Gestione delle modifiche ai documenti approvati

Un documento approvato può essere modificato solo se esiste un motivo chiaro.

Motivi validi:

1. correzione di incoerenza;
2. chiarimento di ambiguità;
3. integrazione di osservazioni del consiglio AI;
4. aggiornamento necessario per un design successivo;
5. errore materiale;
6. decisione esplicita del project owner.

Una modifica a un documento approvato deve:

1. aggiornare la versione;
2. aggiornare la data se necessario;
3. aggiornare la cronologia;
4. non cancellare decisioni importanti senza indicarlo;
5. mantenere coerenza con gli altri documenti approvati.

---

## 26. Gestione delle revisioni del consiglio AI

Le revisioni di DeepSeek e Gemini devono essere analizzate, non applicate automaticamente.

Ogni osservazione deve essere classificata come:

```text
da integrare obbligatoriamente
da integrare parzialmente
suggerimento opzionale
fuori perimetro
da respingere
già coperta dal documento
```

ChatGPT, insieme al project owner, produce la sintesi finale.

La sintesi finale deve spiegare:

1. quali osservazioni vengono accettate;
2. quali osservazioni vengono respinte;
3. quali osservazioni vengono rimandate;
4. perché;
5. quale versione del documento viene prodotta.

Il documento aggiornato deve integrare solo ciò che è coerente con il perimetro approvato.

---

## 27. Regole per Cursor e agenti di coding

Cursor e gli agenti di coding devono lavorare solo su documenti approvati o su prompt operativi derivati da documenti approvati.

Un prompt per Cursor deve indicare, quando il compito lo richiede:

1. documento di riferimento;
2. obiettivo;
3. file o cartelle autorizzati;
4. file o cartelle vietati;
5. funzionalità escluse;
6. criteri di completamento;
7. regole di accessibilità rilevanti;
8. regole di test rilevanti;
9. divieto di introdurre funzionalità non richieste.

Per compiti molto piccoli, come la correzione di un refuso o una modifica locale già coperta da documenti approvati, il prompt può essere semplificato.

Anche in questi casi deve restare chiaro quale area viene toccata e quale documento approvato autorizza quella modifica.

Se il progetto usa file di configurazione per agenti o IDE, come `.cursorrules` o equivalenti, questi possono richiamare i documenti approvati e le regole operative principali.

Tali file non sostituiscono la documentazione ufficiale.

Cursor non deve:

1. cambiare architettura;
2. inventare requisiti;
3. modificare documenti approvati senza richiesta;
4. introdurre funzionalità fuori perimetro;
5. spostare logica tra livelli;
6. creare stringhe utente sparse;
7. creare soluzioni complesse non autorizzate;
8. saltare i test previsti.

---

## 28. Criteri di validità documentale

Un documento è valido se:

1. ha uno scopo chiaro;
2. dichiara tipo, stato, versione, data e repository;
3. indica sempre i documenti collegati o dichiara `Nessuno`;
4. è coerente con i documenti approvati precedenti;
5. non introduce funzionalità fuori perimetro;
6. distingue chiaramente cosa è incluso e cosa è escluso;
7. usa linguaggio chiaro e verificabile;
8. non contiene decisioni appartenenti a documenti successivi;
9. non contraddice l'architettura generale;
10. non contraddice le regole di accessibilità;
11. può essere usato da un agente AI senza generare ambiguità operative;
12. contiene criteri di successo o validità quando necessario;
13. contiene una sezione finale con stato e cronologia.

Se un documento è corretto nel contenuto ma ambiguo per un agente AI, deve essere migliorato prima dell'approvazione.

---

## 29. Criteri di invalidità documentale

Un documento non è valido se:

1. non dichiara il proprio scopo;
2. non dichiara il proprio stato;
3. non indica i documenti collegati o `Nessuno`;
4. mescola visione, design, coding plan e todo senza distinzione;
5. autorizza modifiche non esplicite;
6. introduce funzionalità fuori perimetro;
7. contraddice un documento approvato senza dichiararlo;
8. contiene formule vaghe non verificabili;
9. lascia agli agenti AI decisioni architetturali;
10. non distingue tra regole obbligatorie ed esempi;
11. usa il codice esistente come unica giustificazione contro la documentazione approvata;
12. elimina vincoli di accessibilità senza approvazione;
13. amplia il progetto oltre la prima versione minima.

---

## 30. Stato del documento

Questo documento è approvato come standard documentale del progetto CicloTimer.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione DeepSeek/Gemini e correzione della rigidità del ciclo documentale
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione delle osservazioni dei consiglieri AI su ciclo documentale flessibile, deroghe per modifiche minori, documenti collegati obbligatori, conflitto documentazione/codice, flusso snello per documenti operativi, todo focalizzati e uso controllato di configurazioni Cursor
```

Il documento è stato revisionato dal consiglio AI formato da:

```text
ChatGPT
DeepSeek
Gemini
```

Le osservazioni integrate sono:

1. chiarimento che il ciclo documentale è orientativo e adattabile dal project owner;
2. distinzione tra ciclo completo del consiglio AI e flusso snello per documenti operativi;
3. deroga controllata per bug fix, refactoring interni, refusi e modifiche già coperte da design approvato;
4. obbligo del campo `Documenti collegati`, con valore `Nessuno` quando non applicabile;
5. chiarimento che la documentazione approvata prevale sempre sul codice in caso di conflitto;
6. nota sul fatto che un documento `0.x.0` può essere `APPROVED` pur restando aggiornabile;
7. indicazione che i todo operativi devono restare piccoli, focalizzati e verificabili;
8. nota non vincolante sull'uso di `.cursorrules` o file equivalenti come supporto agli agenti di coding.

Il documento è approvato dal project owner come base per i successivi documenti architetturali, di design, coding plan, todo operativo e verifica.

