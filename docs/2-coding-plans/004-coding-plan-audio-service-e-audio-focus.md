# CicloTimer — Coding Plan 004 — Audio service e audio focus

**Tipo documento:** coding plan  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/1-design/004-design-audio-service-e-audio-focus.md, docs/2-coding-plans/002-coding-plan-bridge-ui-logica-timer.md, docs/3-todos/002-todo-bridge-ui-logica-timer.md  

---

## 1. Scopo del documento

Questo documento traduce il Design 004 approvato in un piano operativo di codifica.

Il documento di riferimento principale è:

```text
docs/1-design/004-design-audio-service-e-audio-focus.md
````

Il Design 004 stabilisce che il servizio audio deve essere un componente separato, collocato in:

```text
services/CicloTimer.Audio/
```

con file progetto:

```text
services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

Il servizio audio userà target:

```text
net9.0-windows
```

e avrà test in:

```text
tests/CicloTimer.Audio.Tests/
```

Questo coding plan definisce:

1. quali cartelle creare;
2. quali file creare;
3. quale progetto .NET creare;
4. quale progetto test creare;
5. come generare l’asset audio predefinito;
6. come includere l’asset audio nel progetto;
7. come progettare il servizio audio;
8. come isolare player reale e focus manager tramite adapter;
9. quali risultati tecnici produrre;
10. quali errori gestire;
11. quali test automatici creare;
12. quali aree non toccare;
13. quali verifiche finali eseguire.

Questo coding plan non cambia il Design 004.

Questo coding plan non introduce UI.

Questo coding plan non introduce orchestratore.

Questo coding plan non collega ancora il servizio audio al bridge.

Questo coding plan non autorizza modifiche a core, localization, bridge o WPF.

---

## 2. Obiettivo operativo

L’obiettivo operativo è creare il progetto:

```text
CicloTimer.Audio
```

nel percorso:

```text
services/CicloTimer.Audio/
```

e il relativo progetto test:

```text
tests/CicloTimer.Audio.Tests/
```

Il progetto audio deve:

1. esporre un servizio audio separato;
2. gestire avvio avviso finale;
3. gestire stop avviso finale;
4. usare un file audio predefinito incluso nel progetto;
5. usare il file:

```text
services/CicloTimer.Audio/Assets/final-alert.wav
```

6. essere fail-safe;
7. essere idempotente;
8. non bloccare il timer;
9. non dipendere da core;
10. non dipendere da localization;
11. non dipendere da bridge;
12. non dipendere dalla UI WPF;
13. non manipolare processi esterni;
14. non forzare il volume globale di Windows;
15. essere testabile senza hardware audio reale.

---

## 3. Perimetro autorizzato

Questo coding plan autorizza:

1. creazione di `services/CicloTimer.Audio/`;
2. creazione di `services/CicloTimer.Audio/CicloTimer.Audio.csproj`;
3. creazione di `services/CicloTimer.Audio/Assets/`;
4. creazione o generazione di `services/CicloTimer.Audio/Assets/final-alert.wav`;
5. creazione dei file C# del progetto audio;
6. aggiunta del progetto audio a `CicloTimer.sln`;
7. creazione di `tests/CicloTimer.Audio.Tests/`;
8. creazione di `tests/CicloTimer.Audio.Tests/CicloTimer.Audio.Tests.csproj`;
9. aggiunta del progetto test audio a `CicloTimer.sln`;
10. riferimento del progetto test audio al progetto audio;
11. eventuale modifica minima di `ciclotimer.csproj` solo se la build WPF include ricorsivamente `services/**`;
12. eventuale aggiornamento minimo di `.gitignore` solo se emergono nuovi artefatti non ignorati;
13. esecuzione build e test.

---

## 4. Fuori perimetro

Questo coding plan non autorizza:

1. modifica di `models/CicloTimer.Core/`;
2. modifica di `locales/CicloTimer.Localization/`;
3. modifica di `view-models/CicloTimer.Bridge/`;
4. modifica dei test core;
5. modifica dei test localization;
6. modifica dei test bridge;
7. modifica di `MainWindow.xaml`;
8. modifica di `MainWindow.xaml.cs`;
9. modifica di `App.xaml`;
10. modifica di `App.xaml.cs`;
11. modifica UI WPF;
12. creazione ViewModel;
13. creazione orchestratore;
14. collegamento del servizio audio al bridge;
15. collegamento del servizio audio alla UI;
16. uso di `ICommand`;
17. uso di `INotifyPropertyChanged`;
18. uso di NVDA;
19. uso di UI Automation;
20. uso di Live Region;
21. gestione testi utente;
22. modifica localization per errori audio;
23. persistenza preferenze audio;
24. scelta suono utente;
25. slider volume;
26. uso dei suoni di sistema Windows come sorgente principale;
27. download di file audio da internet;
28. copia di suoni Windows;
29. uso di file audio con licenza non chiara;
30. manipolazione processi esterni;
31. chiusura app esterne;
32. forzatura volume globale di Windows;
33. gestione aggressiva di chiamate o riunioni;
34. installazione di librerie audio esterne senza autorizzazione;
35. creazione cartella `src/`.

Se durante l’implementazione emerge la necessità di modificare core, localization, bridge, UI o architettura, Cursor deve fermarsi e segnalarlo.

---

## 5. Struttura fisica obbligatoria

La struttura da creare è:

```text
services/
  CicloTimer.Audio/
    CicloTimer.Audio.csproj
    AudioActionResult.cs
    AudioPlaybackState.cs
    AudioService.cs
    AudioServiceOptions.cs
    AudioServiceResult.cs
    IAudioPlayer.cs
    IAudioFocusManager.cs
    IAudioModificationTracker.cs
    AudioModificationSnapshot.cs
    NullAudioFocusManager.cs
    WindowsAudioPlayer.cs
    WindowsAudioFocusManager.cs
    Assets/
      final-alert.wav

tests/
  CicloTimer.Audio.Tests/
    CicloTimer.Audio.Tests.csproj
    AudioServiceStateTests.cs
    AudioServiceFailSafeTests.cs
    AudioServiceIdempotencyTests.cs
    AudioServiceAssetTests.cs
    AudioServiceFocusTests.cs
    ProjectDependencyTests.cs
```

I nomi dei file possono essere adattati solo se il significato rimane identico e il report finale documenta la deviazione.

Sono vietate collocazioni alternative:

```text
src/
models/CicloTimer.Audio/
locales/CicloTimer.Audio/
view-models/CicloTimer.Audio/
CicloTimer.Audio/ nella root
```

---

## 6. Progetto CicloTimer.Audio

### 6.1 Percorso

```text
services/CicloTimer.Audio/
```

File progetto:

```text
services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

### 6.2 Target framework

Il progetto deve usare:

```text
net9.0-windows
```

Motivo:

```text
il servizio audio è specifico per Windows desktop e può dover interagire con sottosistemi multimediali Windows
```

### 6.3 Riferimenti vietati

Il progetto audio non deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
ciclotimer.csproj
tests/*
```

Il progetto audio non deve dipendere da:

```text
WPF
PresentationFramework
System.Windows
UIAutomation
```

Il target `net9.0-windows` è autorizzato solo per il perimetro audio Windows-specifico.

Non autorizza UI WPF.

Non autorizza XAML.

Non autorizza dipendenze dal progetto WPF.

### 6.4 Contenuto indicativo del csproj

Il progetto dovrà includere l’asset audio.

Forma indicativa:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>CicloTimer.Audio</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <Content Include="Assets\final-alert.wav">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
</Project>
```

La forma concreta può essere adattata, ma il file audio deve essere disponibile a runtime.

---

## 7. Aggiornamento solution

Dopo la creazione del progetto audio, aggiungere subito alla solution:

```bash
dotnet sln CicloTimer.sln add services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

Dopo la creazione del progetto test audio, aggiungere subito alla solution:

```bash
dotnet sln CicloTimer.sln add tests/CicloTimer.Audio.Tests/CicloTimer.Audio.Tests.csproj
```

I comandi devono essere eseguiti dalla root del repository.

Se `dotnet sln add` fallisce, Cursor deve fermarsi e segnalare il problema.

Non deve modificare manualmente la solution salvo esplicita necessità documentata nel report finale.

---

## 8. Possibile esclusione in ciclotimer.csproj

Il progetto WPF root può includere ricorsivamente file `.cs` sotto la root del repository.

In passato è stato necessario escludere:

```xml
<Compile Remove="models/**" />
<Compile Remove="tests/**" />
<Compile Remove="locales/**" />
<Compile Remove="view-models/**" />
```

Per il servizio audio, Cursor non deve modificare preventivamente `ciclotimer.csproj`.

Ordine obbligatorio:

1. creare il progetto audio;
2. creare il progetto test audio;
3. aggiungere entrambi alla solution;
4. eseguire `dotnet build CicloTimer.sln` dalla root;
5. solo se la build fallisce perché il progetto WPF include file sotto `services/**`, allora modificare `ciclotimer.csproj`.

In quel solo caso Cursor può aggiungere:

```xml
<Compile Remove="services/**" />
```

Questa modifica è autorizzata solo se necessaria.

Cursor deve documentare chiaramente questa eventuale modifica nel report finale.

---

## 9. Asset audio predefinito

### 9.1 Percorso

Il file audio predefinito deve essere:

```text
services/CicloTimer.Audio/Assets/final-alert.wav
```

### 9.2 Regole

Il file deve essere:

1. formato `.wav`;
2. incluso nel repository;
3. incluso nel progetto audio;
4. copiato in output;
5. breve;
6. riconoscibile;
7. non aggressivo;
8. adatto a loop o ripetizione;
9. libero da problemi di licenza;
10. non copiato da Windows;
11. non scaricato da internet;
12. non proveniente da fonte non verificata.

### 9.3 Generazione asset

Cursor deve generare un file `.wav` sintetico minimale durante l’implementazione, senza scaricare nulla e senza copiare file esterni.

Il file deve essere generato localmente come WAV PCM valido.

Specifiche obbligatorie per la prima versione:

```text
formato: WAV PCM
canali: mono
bit depth: 16-bit
sample rate: 44100 Hz
durata: circa 300 ms
tono: sinusoidale
frequenza tono: circa 880 Hz
volume: moderato, non aggressivo
```

La generazione può essere fatta con:

1. una piccola funzione C#;
2. uno script locale controllato;
3. altro metodo locale equivalente.

La generazione non deve richiedere download.

La generazione non deve richiedere file di origine esterni.

La generazione non deve usare `C:\Windows\Media`.

La generazione non deve usare audio preso da internet.

Il report finale deve indicare:

1. metodo usato per generare il file;
2. durata;
3. sample rate;
4. bit depth;
5. numero canali;
6. frequenza indicativa del tono.

### 9.4 Divieti

Cursor non deve:

1. scaricare audio da internet;
2. copiare file da `C:\Windows\Media`;
3. usare audio protetto da copyright;
4. usare suoni di provenienza non chiara;
5. creare asset audio enorme;
6. creare asset audio eccessivamente forte o aggressivo.

---

## 10. AudioActionResult

### 10.1 Percorso

```text
services/CicloTimer.Audio/AudioActionResult.cs
```

### 10.2 Responsabilità

Rappresentare l’esito tecnico di una singola sotto-operazione audio.

Enum consigliato:

```csharp
public enum AudioActionResult
{
    NotAttempted,
    Success,
    AlreadyPlaying,
    AlreadyStopped,
    AudioFileMissing,
    PlaybackFailed,
    AudioFocusUnavailable,
    AudioFocusFailed,
    RestoreFailed
}
```

Regole:

1. questi valori non sono testi utente;
2. non devono dipendere da localization;
3. non devono essere mostrati direttamente dalla UI;
4. possono essere usati dai test e da un futuro orchestratore;
5. devono distinguere playback, focus e restore tramite `AudioServiceResult`.

---

## 11. AudioServiceResult

### 11.1 Percorso

```text
services/CicloTimer.Audio/AudioServiceResult.cs
```

### 11.2 Responsabilità

Rappresentare il risultato composito obbligatorio di un comando del servizio audio.

Questo risultato è obbligatorio.

Non lasciare a Cursor la scelta tra enum singolo e risultato composito.

Forma consigliata:

```csharp
public sealed record AudioServiceResult(
    AudioActionResult PlaybackResult,
    AudioActionResult FocusResult,
    AudioActionResult RestoreResult);
```

È ammesso aggiungere proprietà tecniche se utili, per esempio:

```csharp
public bool IsPlaybackSuccessful { get; }
```

ma non è obbligatorio.

### 11.3 Regola fondamentale

Il risultato deve distinguere almeno:

```text
risultato playback
risultato focus
risultato restore
```

Motivo:

```text
playback riuscito + focus non disponibile non è un fallimento totale
```

Esempio corretto:

```text
PlaybackResult = Success
FocusResult = AudioFocusUnavailable
RestoreResult = NotAttempted
```

Questo caso significa:

```text
il timer audio ha suonato
il focus audio non era disponibile
il servizio resta valido
```

Esempio diverso:

```text
PlaybackResult = PlaybackFailed
FocusResult = NotAttempted
RestoreResult = NotAttempted
```

Questo significa:

```text
l’audio del timer non è partito
il focus non è stato tentato
il servizio ha gestito errore controllato
```

### 11.4 Regole

1. `StartFinalAlertSound` deve restituire `AudioServiceResult`;
2. `StopFinalAlertSound` deve restituire `AudioServiceResult`;
3. non usare solo `AudioActionResult` come ritorno pubblico principale;
4. non perdere informazioni tra playback/focus/restore;
5. nessun testo utente nel risultato;
6. nessuna dipendenza da localization.

---

## 12. AudioPlaybackState

### 12.1 Percorso

```text
services/CicloTimer.Audio/AudioPlaybackState.cs
```

### 12.2 Responsabilità

Rappresentare lo stato tecnico interno del servizio audio.

Enum consigliato:

```csharp
public enum AudioPlaybackState
{
    Idle,
    PlayingFinalAlert,
    Stopping,
    Failed
}
```

Regole:

1. questi stati non sono stati del timer;
2. non devono uscire dal perimetro audio come logica business;
3. servono per idempotenza e fail-safe;
4. non devono influenzare il core.

---

## 13. AudioServiceOptions

### 13.1 Percorso

```text
services/CicloTimer.Audio/AudioServiceOptions.cs
```

### 13.2 Responsabilità

Rappresentare configurazione tecnica interna del servizio audio.

Contenuto consigliato:

```csharp
public sealed record AudioServiceOptions(
    string FinalAlertAudioPath);
```

Regole:

1. path default verso `Assets/final-alert.wav` copiato in output;
2. nessuna preferenza utente;
3. nessun volume configurabile;
4. nessuna scelta suono utente;
5. utile nei test per simulare file mancante.

Il path può essere calcolato dal servizio usando `AppContext.BaseDirectory`, ma deve restare testabile.

---

## 14. IAudioPlayer

### 14.1 Percorso

```text
services/CicloTimer.Audio/IAudioPlayer.cs
```

### 14.2 Responsabilità

Astrarre la riproduzione audio reale.

Interfaccia concettuale consigliata:

```csharp
public interface IAudioPlayer
{
    AudioActionResult StartLoop(string audioFilePath);
    AudioActionResult Stop();
    bool IsPlaying { get; }
}
```

Le firme definitive possono variare, ma devono supportare:

1. start audio;
2. stop audio;
3. stato riproduzione;
4. errore controllato.

Regole:

1. nessuna eccezione non gestita;
2. no UI;
3. no WPF;
4. no testi utente;
5. testabilità tramite implementazione finta nei test.

---

## 15. IAudioFocusManager

### 15.1 Percorso

```text
services/CicloTimer.Audio/IAudioFocusManager.cs
```

### 15.2 Responsabilità

Astrarre l’eventuale gestione sicura della priorità percettiva/audio focus.

Interfaccia concettuale consigliata:

```csharp
public interface IAudioFocusManager
{
    AudioActionResult TryApplyFocus();
    AudioActionResult TryRestoreFocus();
}
```

Le firme definitive possono variare, ma devono supportare:

1. tentativo di applicare focus/attenuazione;
2. tentativo di ripristino;
3. fallimento controllato;
4. focus non disponibile.

Regole:

1. non promettere controllo totale sugli altri audio;
2. non manipolare processi esterni;
3. non chiudere app esterne;
4. non forzare volume globale di Windows;
5. non silenziare aggressivamente chiamate/riunioni;
6. se non supportato, restituire `AudioFocusUnavailable`.

---

## 16. IAudioModificationTracker

### 16.1 Percorso

```text
services/CicloTimer.Audio/IAudioModificationTracker.cs
```

### 16.2 Responsabilità

Tenere traccia delle modifiche audio effettivamente applicate, per poter tentare ripristino.

Interfaccia o classe concettuale:

```csharp
public interface IAudioModificationTracker
{
    bool HasTrackedModifications { get; }
    void Track(AudioModificationSnapshot snapshot);
    IReadOnlyList<AudioModificationSnapshot> GetTrackedModifications();
    void Clear();
}
```

Questa struttura è concettuale.

Il coding agent può implementarla in forma più semplice se coerente.

Regole:

1. tracciare solo modifiche realmente applicate;
2. non inventare ripristini;
3. cancellare tracce dopo ripristino riuscito o gestione controllata;
4. non contenere riferimenti a processi esterni manipolati unsafe;
5. se `NullAudioFocusManager` non applica modifiche reali, il tracker può restare vuoto.

---

## 17. AudioModificationSnapshot

### 17.1 Percorso

```text
services/CicloTimer.Audio/AudioModificationSnapshot.cs
```

### 17.2 Responsabilità

Rappresentare una modifica audio applicata.

Nella prima versione può essere minimale.

Esempio concettuale:

```csharp
public sealed record AudioModificationSnapshot(
    string Scope,
    string Description);
```

Regole:

1. non deve contenere handle unsafe;
2. non deve contenere processi esterni manipolati;
3. serve solo per testabilità e ripristino concettuale;
4. può restare vuoto/minimale se `NullAudioFocusManager` non applica modifiche reali.

---

## 18. NullAudioFocusManager

### 18.1 Percorso

```text
services/CicloTimer.Audio/NullAudioFocusManager.cs
```

### 18.2 Responsabilità

Implementare un focus manager sicuro che non modifica altri audio.

Questo manager serve come fallback predefinito se non viene scelta una API sicura per audio focus nella prima implementazione.

Comportamento:

```text
TryApplyFocus → AudioFocusUnavailable
TryRestoreFocus → Success
```

Regole:

1. non modifica audio esterno;
2. non fallisce in modo grave;
3. consente al servizio audio di funzionare;
4. garantisce che il timer continui;
5. evita implementazioni unsafe;
6. non deve tracciare modifiche perché non applica modifiche reali.

Questa scelta è coerente con il Design 004: la riproduzione del suono è obbligatoria, il controllo degli altri audio è condizionato alla disponibilità di API sicure.

---

## 19. WindowsAudioFocusManager

### 19.1 Percorso

```text
services/CicloTimer.Audio/WindowsAudioFocusManager.cs
```

### 19.2 Responsabilità

Rappresentare l’eventuale implementazione Windows-specific del focus manager.

Nella prima versione il criterio è safe-first.

Regola obbligatoria:

```text
se non esiste una strada semplice, sicura e chiaramente supportata per il ducking, usare NullAudioFocusManager o restituire AudioFocusUnavailable
```

Nella prima versione è accettabile che `WindowsAudioFocusManager`:

1. deleghi internamente a `NullAudioFocusManager`;
2. restituisca `AudioFocusUnavailable`;
3. documenti nel report che il ducking reale è rinviato.

Non è obbligatorio implementare ducking reale nella prima versione.

È vietato:

1. manipolare processi esterni;
2. chiudere app esterne;
3. forzare il mixer globale di Windows;
4. manipolare sessioni audio arbitrarie in modo unsafe;
5. introdurre codice fragile per controllare browser, Spotify, YouTube o chiamate.

Se Cursor ritiene necessaria una API avanzata come `IAudioSessionManager2`, deve fermarsi e segnalarlo nel report, senza introdurre implementazioni non richieste.

---

## 20. WindowsAudioPlayer

### 20.1 Percorso

```text
services/CicloTimer.Audio/WindowsAudioPlayer.cs
```

### 20.2 Responsabilità

Implementare la riproduzione audio reale su Windows.

### 20.3 Tecnologia di prima versione

La prima scelta obbligatoria è:

```text
System.Media.SoundPlayer
```

Motivo:

```text
è nativo, semplice, compatibile con WAV, non richiede pacchetti esterni e rispetta il perimetro del progetto
```

`WindowsAudioPlayer` deve usare `System.Media.SoundPlayer` se sufficiente per:

1. caricare il file `.wav`;
2. avviare riproduzione;
3. mantenere avviso percepibile tramite loop/ripetizione;
4. fermare la riproduzione;
5. gestire errori in modo controllato.

### 20.4 Pacchetti esterni

`NAudio` o altre librerie audio esterne non sono autorizzate nella prima implementazione.

Se `System.Media.SoundPlayer` si rivela insufficiente per requisiti minimi come stop sicuro o loop/ripetizione, Cursor deve:

1. fermarsi;
2. non installare pacchetti autonomamente;
3. produrre report di blocco;
4. spiegare il motivo tecnico;
5. proporre alternativa per revisione successiva.

### 20.5 Regole

La riproduzione reale deve rispettare:

1. compatibilità con `net9.0-windows`;
2. riproduzione file `.wav`;
3. possibilità di loop o ripetizione;
4. stop rapido;
5. fail-safe;
6. assenza dipendenze WPF;
7. assenza UI;
8. testabilità tramite `IAudioPlayer`.

---

## 21. AudioService

### 21.1 Percorso

```text
services/CicloTimer.Audio/AudioService.cs
```

### 21.2 Responsabilità

Coordinare player, focus manager, stato interno e risultati tecnici.

Forma consigliata:

```csharp
public sealed class AudioService
{
    public AudioPlaybackState State { get; }

    public AudioServiceResult StartFinalAlertSound();

    public AudioServiceResult StopFinalAlertSound();
}
```

Le firme possono variare, ma i due comandi concettuali devono essere presenti e devono restituire `AudioServiceResult`.

### 21.3 StartFinalAlertSound

Deve:

1. se già `PlayingFinalAlert`, restituire risultato composito con `PlaybackResult = AlreadyPlaying`;
2. verificare disponibilità file audio;
3. se file mancante, restituire `PlaybackResult = AudioFileMissing`;
4. tentare avvio riproduzione;
5. se playback fallisce, restituire `PlaybackResult = PlaybackFailed`;
6. se playback parte, impostare stato `PlayingFinalAlert`;
7. tentare focus/attenuazione solo dopo avvio o in ordine sicuro documentato;
8. se focus non disponibile, restituire `FocusResult = AudioFocusUnavailable`;
9. non trattare focus non disponibile come fallimento totale se il playback è riuscito;
10. non bloccare il timer;
11. non lanciare eccezioni non gestite.

Regola importante:

```text
la riproduzione del timer è più importante del successo del focus audio
```

Esempio corretto:

```text
PlaybackResult = Success
FocusResult = AudioFocusUnavailable
RestoreResult = NotAttempted
State = PlayingFinalAlert
```

### 21.4 StopFinalAlertSound

Deve:

1. se già `Idle`, restituire risultato composito con `PlaybackResult = AlreadyStopped`;
2. se `Idle`, non invocare `IAudioPlayer.Stop`;
3. se `Idle`, non invocare `IAudioFocusManager.TryRestoreFocus`;
4. se playing, tentare stop riproduzione;
5. tentare ripristino solo se sono state tracciate modifiche;
6. non tentare ripristini inventati;
7. tornare a `Idle` se possibile;
8. se ripristino fallisce, valorizzare `RestoreResult = RestoreFailed` o risultato tecnico equivalente;
9. non lanciare eccezioni non gestite;
10. non bloccare il timer.

---

## 22. Test progetto audio

### 22.1 Percorso

```text
tests/CicloTimer.Audio.Tests/
```

File progetto:

```text
tests/CicloTimer.Audio.Tests/CicloTimer.Audio.Tests.csproj
```

Target:

```text
net9.0-windows
```

Framework consigliato:

```text
xUnit
```

Il progetto test deve referenziare:

```text
services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

Il progetto test non deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
ciclotimer.csproj
```

---

## 23. Test AudioServiceStateTests

Creare:

```text
tests/CicloTimer.Audio.Tests/AudioServiceStateTests.cs
```

Test obbligatori:

1. stato iniziale `Idle`;
2. start valido porta a `PlayingFinalAlert`;
3. stop valido torna a `Idle`;
4. file mancante porta a risultato controllato e stato sicuro;
5. playback fallito porta a risultato controllato e stato sicuro;
6. focus non disponibile non impedisce playback riuscito;
7. `AudioServiceResult` distingue playback e focus.

---

## 24. Test AudioServiceIdempotencyTests

Creare:

```text
tests/CicloTimer.Audio.Tests/AudioServiceIdempotencyTests.cs
```

Test obbligatori:

1. start su `Idle` avvia una sola riproduzione;
2. start ripetuto mentre già playing non avvia duplicati;
3. start ripetuto restituisce `AlreadyPlaying` o equivalente in `PlaybackResult`;
4. stop su playing ferma;
5. stop ripetuto non genera errore grave;
6. stop ripetuto restituisce `AlreadyStopped` o equivalente in `PlaybackResult`;
7. stop da `Idle` non invoca `IAudioPlayer.Stop`;
8. stop da `Idle` non invoca `IAudioFocusManager.TryRestoreFocus`;
9. nessuna sovrapposizione audio logica.

---

## 25. Test AudioServiceFailSafeTests

Creare:

```text
tests/CicloTimer.Audio.Tests/AudioServiceFailSafeTests.cs
```

Test obbligatori:

1. eccezione del player durante start viene catturata;
2. eccezione del player durante stop viene catturata;
3. eccezione del focus manager durante apply viene catturata;
4. eccezione del focus manager durante restore viene catturata;
5. file mancante non lancia eccezione non gestita;
6. focus fallito non blocca playback già riuscito;
7. restore fallito produce risultato controllato;
8. servizio resta in stato sicuro;
9. `AudioServiceResult` conserva l’informazione del fallimento parziale.

---

## 26. Test AudioServiceAssetTests

Creare:

```text
tests/CicloTimer.Audio.Tests/AudioServiceAssetTests.cs
```

Test obbligatori:

1. `services/CicloTimer.Audio/Assets/final-alert.wav` esiste;
2. il file ha estensione `.wav`;
3. il file non è vuoto;
4. il file è un WAV PCM valido;
5. il file è mono;
6. il file è 16-bit;
7. il file ha sample rate 44100 Hz;
8. il file ha durata indicativa circa 300 ms;
9. il file è incluso nel `.csproj`;
10. il file viene copiato in output;
11. il path default del servizio punta al file copiato in output o a risorsa equivalente;
12. test del caso file mancante usando path falso o mock, non cancellando il file reale.

---

## 27. Test AudioServiceFocusTests

Creare:

```text
tests/CicloTimer.Audio.Tests/AudioServiceFocusTests.cs
```

Test obbligatori:

1. `NullAudioFocusManager.TryApplyFocus` restituisce `AudioFocusUnavailable`;
2. `NullAudioFocusManager.TryRestoreFocus` non fallisce;
3. focus unavailable non impedisce `StartFinalAlertSound`;
4. modifiche audio vengono tracciate solo se applicate;
5. se non ci sono modifiche, restore non tenta ripristini inventati;
6. se ci sono modifiche tracciate, restore viene tentato;
7. dopo restore riuscito, tracce vengono pulite;
8. restore fallito produce errore controllato;
9. tracker può restare vuoto quando viene usato `NullAudioFocusManager`.

---

## 28. ProjectDependencyTests

Creare:

```text
tests/CicloTimer.Audio.Tests/ProjectDependencyTests.cs
```

Verifiche obbligatorie o equivalenti nel report:

1. `CicloTimer.Audio.csproj` usa `net9.0-windows`;
2. non referenzia Core;
3. non referenzia Localization;
4. non referenzia Bridge;
5. non referenzia WPF root;
6. non contiene `System.Windows`;
7. non contiene `PresentationFramework`;
8. non contiene `UIAutomation`;
9. non contiene `ICommand`;
10. non contiene `INotifyPropertyChanged`;
11. non contiene cartella `src`;
12. non contiene stringhe utente finali hardcoded;
13. non manipola processi esterni;
14. non forza volume globale Windows;
15. non usa suoni da `C:\Windows\Media`;
16. non contiene riferimenti a NAudio o altre librerie audio esterne;
17. usa `System.Media.SoundPlayer` come prima scelta per `WindowsAudioPlayer`, salvo blocco documentato.

I controlli possono essere test automatizzati o verifica documentata nel report finale.

---

## 29. Divieto stringhe utente hardcoded

Il progetto audio non deve contenere testi utente finali.

Ammesse stringhe tecniche:

1. path asset;
2. nomi file;
3. messaggi tecnici in test;
4. nomi enum;
5. descrizioni interne non mostrate all’utente.

Non sono ammessi testi come:

```text
Avviso audio non disponibile.
Errore audio.
Riproduzione non riuscita.
```

Questi testi, se serviranno in futuro, passeranno da localization in un design successivo.

---

## 30. Ordine operativo di implementazione

L’implementazione dovrà seguire questo ordine:

```text
1. Ricognizione repository.
2. Verifica Design 004.
3. Creazione progetto services/CicloTimer.Audio.
4. Aggiunta progetto audio alla solution.
5. Creazione Assets/.
6. Generazione final-alert.wav sintetico WAV PCM 16-bit mono 44100 Hz circa 300 ms, 880 Hz.
7. Inclusione asset nel csproj.
8. Creazione enum/record tecnici: AudioActionResult, AudioServiceResult, AudioPlaybackState.
9. Creazione interfacce IAudioPlayer, IAudioFocusManager, IAudioModificationTracker.
10. Creazione tracker/snapshot.
11. Creazione NullAudioFocusManager.
12. Creazione WindowsAudioFocusManager safe-first o fallback a Null.
13. Creazione WindowsAudioPlayer con System.Media.SoundPlayer.
14. Creazione AudioService.
15. Creazione progetto tests/CicloTimer.Audio.Tests.
16. Aggiunta test audio alla solution.
17. Scrittura test stato.
18. Scrittura test idempotenza.
19. Scrittura test fail-safe.
20. Scrittura test asset.
21. Scrittura test focus.
22. Scrittura ProjectDependencyTests.
23. Esecuzione build solution.
24. Solo se necessario, correzione ciclotimer.csproj con services/**.
25. Riesecuzione build.
26. Esecuzione test audio.
27. Esecuzione regressione core/localization/bridge.
28. Esecuzione dotnet test generale.
29. Pulizia bin/obj se necessario.
30. Report finale.
```

---

## 31. Comandi di verifica finali

Tutti i comandi devono essere eseguiti dalla root del repository.

Eseguire:

```bash
dotnet build CicloTimer.sln
```

Eseguire test audio:

```bash
dotnet test tests/CicloTimer.Audio.Tests/CicloTimer.Audio.Tests.csproj
```

Eseguire regressione core:

```bash
dotnet test tests/CicloTimer.Core.Tests/CicloTimer.Core.Tests.csproj
```

Eseguire regressione localization:

```bash
dotnet test tests/CicloTimer.Localization.Tests/CicloTimer.Localization.Tests.csproj
```

Eseguire regressione bridge:

```bash
dotnet test tests/CicloTimer.Bridge.Tests/CicloTimer.Bridge.Tests.csproj
```

Se possibile, eseguire:

```bash
dotnet test
```

Criteri:

1. build solution: 0 errori;
2. test audio: tutti superati;
3. test core: tutti ancora superati;
4. test localization: tutti ancora superati;
5. test bridge: tutti ancora superati;
6. test solution completa: tutti superati.

---

## 32. Pulizia artefatti build

Dopo build/test, non devono restare artefatti da committare sotto:

```text
bin/
obj/
```

Cursor dovrà verificare:

```bash
git status --porcelain
```

e, su PowerShell:

```powershell
git status --porcelain | Select-String -Pattern "bin|obj"
```

Se compaiono artefatti `bin/obj`, devono essere rimossi dal working tree se non tracciati e non necessari.

Non devono essere rimossi file sorgente.

Non devono essere rimossi file `.csproj`.

Non deve essere rimosso `final-alert.wav`.

Non deve essere rimossa la cartella `Assets/`.

---

## 33. File da non modificare

Cursor non deve modificare:

```text
models/CicloTimer.Core/
locales/CicloTimer.Localization/
view-models/CicloTimer.Bridge/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
```

Cursor non deve modificare `ciclotimer.csproj`, salvo il caso specifico autorizzato:

```text
aggiunta di <Compile Remove="services/**" />
```

se necessaria per impedire al progetto WPF root di compilare il servizio audio.

Cursor può modificare:

```text
CicloTimer.sln
```

solo per aggiungere i nuovi progetti.

Cursor può modificare `.gitignore` solo se necessario per artefatti nuovi non ignorati.

---

## 34. Criteri di completamento

Il coding plan è completato quando:

1. esiste `services/CicloTimer.Audio/`;
2. esiste `services/CicloTimer.Audio/CicloTimer.Audio.csproj`;
3. il progetto audio usa `net9.0-windows`;
4. il progetto audio non referenzia core;
5. il progetto audio non referenzia localization;
6. il progetto audio non referenzia bridge;
7. il progetto audio non referenzia WPF root;
8. esiste `services/CicloTimer.Audio/Assets/final-alert.wav`;
9. il file audio è `.wav`;
10. il file audio è WAV PCM valido;
11. il file audio è 16-bit;
12. il file audio è mono;
13. il file audio ha sample rate 44100 Hz;
14. il file audio ha durata circa 300 ms;
15. il file audio è incluso nel progetto;
16. il file audio viene copiato in output;
17. esiste `AudioActionResult`;
18. esiste `AudioServiceResult`;
19. `AudioServiceResult` distingue playback/focus/restore;
20. esiste `AudioPlaybackState`;
21. esiste `AudioServiceOptions`;
22. esiste `IAudioPlayer`;
23. esiste `IAudioFocusManager`;
24. esiste `IAudioModificationTracker` o equivalente;
25. esiste `AudioModificationSnapshot` o equivalente;
26. esiste `NullAudioFocusManager`;
27. esiste `WindowsAudioFocusManager` o wrapper sicuro equivalente;
28. esiste `WindowsAudioPlayer`;
29. `WindowsAudioPlayer` usa `System.Media.SoundPlayer` salvo blocco tecnico documentato;
30. non vengono installate librerie audio esterne senza autorizzazione;
31. esiste `AudioService`;
32. `StartFinalAlertSound` è idempotente;
33. `StopFinalAlertSound` è idempotente;
34. stop da Idle non invoca player/focus;
35. file mancante produce errore controllato;
36. playback fallito produce errore controllato;
37. focus non disponibile non blocca playback;
38. restore fallito produce errore controllato;
39. non ci sono manipolazioni unsafe di processi esterni;
40. non viene forzato volume globale di Windows;
41. non vengono usati suoni Windows come sorgente principale;
42. esiste `tests/CicloTimer.Audio.Tests/`;
43. test audio passano;
44. test core passano;
45. test localization passano;
46. test bridge passano;
47. build solution passa;
48. non ci sono artefatti `bin/obj` da committare;
49. non sono stati modificati core, localization, bridge o UI.

---

## 35. Criteri di non validità

L’implementazione non è valida se:

1. crea audio fuori da `services/CicloTimer.Audio/`;
2. crea `src/`;
3. mette audio in `models/`;
4. mette audio in `locales/`;
5. mette audio in `view-models/`;
6. modifica core;
7. modifica localization;
8. modifica bridge;
9. modifica UI;
10. usa `net9.0` invece di `net9.0-windows`;
11. il progetto audio referenzia core;
12. il progetto audio referenzia localization;
13. il progetto audio referenzia bridge;
14. il progetto audio referenzia WPF root;
15. manca `final-alert.wav`;
16. `final-alert.wav` non è incluso nel progetto;
17. il file WAV non è valido;
18. il file WAV non è PCM;
19. il file WAV non è 16-bit mono;
20. il file WAV non ha sample rate 44100 Hz;
21. usa file audio da internet;
22. usa file audio da Windows;
23. usa file audio con licenza non chiara;
24. start ripetuto crea più riproduzioni;
25. stop ripetuto genera errore grave;
26. stop da Idle invoca player/focus senza motivo;
27. errore audio blocca il timer;
28. file mancante genera eccezione non gestita;
29. playback fallito genera eccezione non gestita;
30. focus fallito blocca playback;
31. restore fallito genera eccezione non gestita;
32. manipola processi esterni;
33. chiude app esterne;
34. forza volume globale Windows;
35. promette controllo totale sugli altri audio;
36. usa testi utente hardcoded;
37. dipende da localization per messaggi;
38. richiede hardware audio reale nei test;
39. richiede Spotify/browser/YouTube/chiamate nei test;
40. installa NAudio o librerie audio esterne senza autorizzazione;
41. build fallisce;
42. test falliscono;
43. ci sono artefatti `bin/obj` da committare.

---

## 36. Report finale richiesto a Cursor

Al termine dell’implementazione, Cursor dovrà produrre un report con:

1. file creati;
2. file modificati;
3. progetti creati;
4. progetti aggiunti alla solution;
5. target framework usati;
6. conferma progetto audio in `services/CicloTimer.Audio/`;
7. conferma test in `tests/CicloTimer.Audio.Tests/`;
8. conferma asset in `services/CicloTimer.Audio/Assets/final-alert.wav`;
9. modalità di generazione del file audio;
10. durata/sample rate/bit depth/canali/frequenza tono del file audio;
11. conferma asset incluso nel `.csproj`;
12. conferma asset copiato in output;
13. conferma audio non dipende da core;
14. conferma audio non dipende da localization;
15. conferma audio non dipende da bridge;
16. conferma audio non dipende da WPF root;
17. conferma UI non modificata;
18. conferma core non modificato;
19. conferma localization non modificato;
20. conferma bridge non modificato;
21. conferma assenza audio da internet;
22. conferma assenza audio copiato da Windows;
23. conferma assenza manipolazioni unsafe di processi esterni;
24. conferma assenza forzatura volume globale Windows;
25. conferma `AudioServiceResult` composito;
26. conferma distinzione playback/focus/restore;
27. conferma uso di `System.Media.SoundPlayer` o blocco tecnico documentato;
28. conferma assenza librerie audio esterne non autorizzate;
29. conferma idempotenza start;
30. conferma idempotenza stop;
31. conferma stop da Idle senza chiamate inutili a player/focus;
32. conferma fail-safe file mancante;
33. conferma fail-safe playback fallito;
34. conferma focus unavailable non blocca playback;
35. conferma restore fallito gestito;
36. eventuale uso di `NullAudioFocusManager` come fallback;
37. eventuale modifica a `ciclotimer.csproj` e motivo;
38. comandi eseguiti;
39. risultato build;
40. risultato test audio;
41. risultato test core;
42. risultato test localization;
43. risultato test bridge;
44. risultato `dotnet test`;
45. numero test audio;
46. eventuali test falliti;
47. eventuali deviazioni dal coding plan;
48. conferma pulizia bin/obj;
49. output finale sintetico di `git status --porcelain`.

Cursor non deve fare commit.

Cursor non deve fare push.

---

## 37. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. il Coding Plan 004 è approvato come piano operativo del servizio audio;
2. il servizio audio resta in `services/CicloTimer.Audio/`;
3. i test restano in `tests/CicloTimer.Audio.Tests/`;
4. il target è `net9.0-windows`;
5. il servizio audio non dipende da core, localization, bridge o UI;
6. l’asset audio è `services/CicloTimer.Audio/Assets/final-alert.wav`;
7. il file audio deve essere generato localmente;
8. il file audio deve essere WAV PCM 16-bit mono 44100 Hz, circa 300 ms, tono sinusoidale circa 880 Hz;
9. è vietato usare audio da internet, Windows Media o fonti con licenza non chiara;
10. `AudioServiceResult` composito è obbligatorio;
11. `AudioServiceResult` deve distinguere playback, focus e restore;
12. `System.Media.SoundPlayer` è la prima scelta per la riproduzione reale;
13. NAudio o altre librerie esterne non sono autorizzate nella prima implementazione;
14. se `SoundPlayer` non basta, Cursor deve fermarsi e produrre report di blocco;
15. `WindowsAudioFocusManager` segue criterio safe-first;
16. `NullAudioFocusManager` è fallback ammesso e coerente;
17. il tracker può restare vuoto se il focus manager non applica modifiche reali;
18. stop da `Idle` non invoca player/focus;
19. l’audio focus resta tentativo sicuro, non garanzia di controllo sugli altri audio;
20. nessuna manipolazione unsafe di processi esterni;
21. nessuna forzatura del volume globale Windows;
22. nessuna gestione aggressiva di chiamate o riunioni.

---

## 38. Stato del documento

Questo documento è approvato come Coding Plan 004 — Audio service e audio focus.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione tecnica
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni revisione: AudioServiceResult composito obbligatorio, generazione WAV PCM dettagliata, System.Media.SoundPlayer come prima scelta, divieto NAudio/librerie esterne senza approvazione, WindowsAudioFocusManager safe-first, NullAudioFocusManager fallback, stop da Idle senza invocare player/focus, tracker vuoto ammesso se nessuna modifica reale
```

Il documento è approvato come base per il successivo TODO 004.
