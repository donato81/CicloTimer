# CicloTimer — TODO 004 — Audio service e audio focus

**Tipo documento:** todo operativo  
**Stato:** APPROVED  
**Versione:** 0.2.0  
**Data:** 2026-06-03  
**Repository:** donato81/CicloTimer  
**Documenti collegati:** docs/0-architecture/vision.md, docs/0-architecture/architecture.md, docs/0-architecture/accessibility-rules.md, docs/0-architecture/document-standards.md, docs/0-architecture/internal-api.md, docs/1-design/001-design-core-timer-engine.md, docs/1-design/002-design-bridge-ui-logica-timer.md, docs/1-design/003-design-sistema-testi-centralizzati.md, docs/1-design/004-design-audio-service-e-audio-focus.md, docs/2-coding-plans/004-coding-plan-audio-service-e-audio-focus.md  

---

## 1. Scopo del TODO

Questo documento traduce il Coding Plan 004 in una lista operativa eseguibile da Cursor.

L’obiettivo è implementare il servizio audio separato di CicloTimer.

Il servizio audio deve stare in:

```text
services/CicloTimer.Audio/
````

I test del servizio audio devono stare in:

```text
tests/CicloTimer.Audio.Tests/
```

L’asset audio predefinito deve stare in:

```text
services/CicloTimer.Audio/Assets/final-alert.wav
```

Il servizio audio deve:

1. gestire l’avvio dell’avviso finale;
2. gestire lo stop dell’avviso finale;
3. usare un file WAV incluso nel progetto;
4. essere fail-safe;
5. essere idempotente;
6. restituire risultato composito;
7. non bloccare mai il timer;
8. non dipendere da core;
9. non dipendere da localization;
10. non dipendere da bridge;
11. non dipendere dalla UI WPF;
12. non installare librerie esterne senza autorizzazione;
13. non manipolare processi esterni;
14. non forzare il volume globale di Windows.

Questo TODO deve guidare Cursor in modo vincolato, evitando modifiche fuori perimetro.

---

## 2. Principio operativo

Il principio operativo è:

```text
l’audio esegue richieste, non decide il timer
```

Il servizio audio deve esporre comandi concettuali:

```text
StartFinalAlertSound
StopFinalAlertSound
```

Il servizio audio non deve sapere:

```text
stato del timer
durata della sessione
durata dell’avviso finale
numero sessioni completate
eventi del core
testi localization
UI
NVDA
Live Region
orchestratore
```

Il servizio audio deve limitarsi a:

1. avviare audio;
2. fermare audio;
3. tentare focus audio solo in modo sicuro;
4. gestire errori;
5. restituire esiti tecnici.

---

## 3. Perimetro autorizzato

Cursor può:

1. creare `services/CicloTimer.Audio/`;
2. creare `services/CicloTimer.Audio/CicloTimer.Audio.csproj`;
3. creare `services/CicloTimer.Audio/Assets/`;
4. generare `services/CicloTimer.Audio/Assets/final-alert.wav`;
5. creare i file C# del servizio audio;
6. aggiungere il progetto audio a `CicloTimer.sln`;
7. creare `tests/CicloTimer.Audio.Tests/`;
8. creare `tests/CicloTimer.Audio.Tests/CicloTimer.Audio.Tests.csproj`;
9. aggiungere il progetto test audio a `CicloTimer.sln`;
10. referenziare dal progetto test audio solo il progetto audio;
11. modificare `ciclotimer.csproj` solo se la build fallisce perché il progetto WPF include `services/**`;
12. aggiornare `.gitignore` solo se emergono artefatti nuovi non ignorati;
13. eseguire build;
14. eseguire test;
15. pulire eventuali artefatti `bin/obj`;
16. produrre report finale.

---

## 4. Fuori perimetro assoluto

Cursor non deve:

1. modificare `models/CicloTimer.Core/`;
2. modificare `locales/CicloTimer.Localization/`;
3. modificare `view-models/CicloTimer.Bridge/`;
4. modificare `tests/CicloTimer.Core.Tests/`;
5. modificare `tests/CicloTimer.Localization.Tests/`;
6. modificare `tests/CicloTimer.Bridge.Tests/`;
7. modificare `MainWindow.xaml`;
8. modificare `MainWindow.xaml.cs`;
9. modificare `App.xaml`;
10. modificare `App.xaml.cs`;
11. modificare UI WPF;
12. creare ViewModel;
13. creare orchestratore;
14. collegare audio al bridge;
15. collegare audio alla UI;
16. usare `ICommand`;
17. usare `INotifyPropertyChanged`;
18. usare NVDA;
19. usare UI Automation;
20. usare Live Region;
21. generare testi utente;
22. modificare localization;
23. aggiungere messaggi utente;
24. introdurre impostazioni audio;
25. introdurre scelta suono utente;
26. introdurre volume configurabile;
27. introdurre slider volume;
28. usare suoni di sistema Windows come sorgente principale;
29. copiare file da `C:\Windows\Media`;
30. scaricare file audio da internet;
31. usare file audio con licenza non chiara;
32. installare `NAudio` o altre librerie audio esterne senza autorizzazione;
33. manipolare processi esterni;
34. chiudere app esterne;
35. forzare il volume globale di Windows;
36. gestire aggressivamente chiamate o riunioni;
37. creare cartella `src/`;
38. fare commit;
39. fare push.

Se Cursor rileva che una modifica fuori perimetro è necessaria, deve fermarsi e segnalarlo nel report.

---

## 5. Struttura finale attesa

La struttura finale deve essere:

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

Sono vietate collocazioni alternative:

```text
src/
models/CicloTimer.Audio/
locales/CicloTimer.Audio/
view-models/CicloTimer.Audio/
CicloTimer.Audio/ nella root
```

---

## 6. FASE 0 — Ricognizione iniziale

### TODO 004.00 — Verificare repository prima di modificare

Cursor deve leggere e verificare:

1. presenza di `CicloTimer.sln`;
2. presenza di `models/CicloTimer.Core/`;
3. presenza di `locales/CicloTimer.Localization/`;
4. presenza di `view-models/CicloTimer.Bridge/`;
5. presenza di `tests/CicloTimer.Core.Tests/`;
6. presenza di `tests/CicloTimer.Localization.Tests/`;
7. presenza di `tests/CicloTimer.Bridge.Tests/`;
8. presenza di `services/`;
9. presenza del Design 004;
10. presenza del Coding Plan 004;
11. contenuto attuale di `.gitignore`;
12. contenuto attuale di `ciclotimer.csproj`;
13. assenza di `services/CicloTimer.Audio/`;
14. assenza di `tests/CicloTimer.Audio.Tests/`.

Risultato atteso:

```text
ricognizione completata
nessuna modifica ancora eseguita
```

Se `services/CicloTimer.Audio/` o `tests/CicloTimer.Audio.Tests/` esistono già, Cursor deve segnalarlo e non sovrascrivere file senza controllo.

---

## 7. FASE 1 — Creazione progetto audio

### TODO 004.01 — Creare CicloTimer.Audio

Creare:

```text
services/CicloTimer.Audio/
```

Creare:

```text
services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

Il progetto deve usare:

```text
net9.0-windows
```

Il progetto non deve referenziare:

```text
models/CicloTimer.Core/CicloTimer.Core.csproj
locales/CicloTimer.Localization/CicloTimer.Localization.csproj
view-models/CicloTimer.Bridge/CicloTimer.Bridge.csproj
ciclotimer.csproj
tests/*
```

Il progetto non deve dipendere da:

```text
WPF
PresentationFramework
System.Windows
UIAutomation
```

Contenuto indicativo:

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

L’asset `final-alert.wav` può essere aggiunto al `.csproj` dopo la sua generazione.

Subito dopo la creazione del progetto, aggiungerlo alla solution:

```bash
dotnet sln CicloTimer.sln add services/CicloTimer.Audio/CicloTimer.Audio.csproj
```

Se `dotnet sln add` fallisce, Cursor deve fermarsi e segnalarlo.

Non deve modificare manualmente la solution salvo esplicita necessità documentata nel report finale.

Criterio di completamento:

```text
CicloTimer.Audio creato, target net9.0-windows, nessun riferimento non autorizzato, progetto aggiunto alla solution
```

---

## 8. FASE 2 — Asset audio

### TODO 004.02 — Creare cartella Assets

Creare:

```text
services/CicloTimer.Audio/Assets/
```

Criterio di completamento:

```text
cartella Assets creata nel progetto audio
```

---

### TODO 004.03 — Generare final-alert.wav

Generare localmente:

```text
services/CicloTimer.Audio/Assets/final-alert.wav
```

Il file deve essere un WAV valido con queste specifiche:

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

Per evitare falsi fallimenti nei test, la durata deve essere verificata con tolleranza accettabile:

```text
durata attesa: circa 300 ms
tolleranza test consigliata: 250–350 ms
```

Cursor può generare il file usando:

1. `System.IO.File.WriteAllBytes` e un array di byte costruito localmente;
2. piccola funzione C# temporanea;
3. script PowerShell temporaneo;
4. altro metodo locale equivalente.

Il codice o lo script usato solo per generare `final-alert.wav` non deve necessariamente restare nella soluzione finale.

Se Cursor decide di lasciare nel repository il codice o lo script di generazione, deve motivarlo esplicitamente nel report finale.

Cursor non deve:

1. scaricare audio da internet;
2. copiare file da `C:\Windows\Media`;
3. usare audio protetto da copyright;
4. usare file audio di provenienza non chiara;
5. generare file enorme;
6. generare file aggressivo o troppo forte.

Il report finale deve indicare:

1. metodo usato per generare il file;
2. durata;
3. sample rate;
4. bit depth;
5. canali;
6. frequenza tono indicativa;
7. se il generatore temporaneo è stato rimosso o lasciato nel repository.

Criterio di completamento:

```text
final-alert.wav generato localmente, valido, breve, non aggressivo e senza dipendenze esterne
```

---

### TODO 004.04 — Includere asset nel progetto

Aggiornare `CicloTimer.Audio.csproj` affinché:

```text
Assets/final-alert.wav
```

sia incluso nel progetto e copiato in output.

Forma indicativa:

```xml
<ItemGroup>
  <Content Include="Assets\final-alert.wav">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

Criterio di completamento:

```text
final-alert.wav incluso nel progetto e copiato in output
```

---

## 9. FASE 3 — Risultati e stati tecnici

### TODO 004.05 — Creare AudioActionResult

Creare:

```text
services/CicloTimer.Audio/AudioActionResult.cs
```

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

1. valori tecnici;
2. nessun testo utente;
3. nessuna localization;
4. usati dal risultato composito.

Criterio di completamento:

```text
AudioActionResult creato come enum tecnico
```

---

### TODO 004.06 — Creare AudioServiceResult

Creare:

```text
services/CicloTimer.Audio/AudioServiceResult.cs
```

Questo risultato è obbligatorio.

Forma consigliata:

```csharp
public sealed record AudioServiceResult(
    AudioActionResult PlaybackResult,
    AudioActionResult FocusResult,
    AudioActionResult RestoreResult);
```

Regole:

1. `StartFinalAlertSound` restituisce `AudioServiceResult`;
2. `StopFinalAlertSound` restituisce `AudioServiceResult`;
3. distinguere playback/focus/restore;
4. non usare solo `AudioActionResult` come ritorno pubblico principale;
5. nessun testo utente;
6. nessuna localization.

Esempio corretto:

```text
PlaybackResult = Success
FocusResult = AudioFocusUnavailable
RestoreResult = NotAttempted
```

Significato:

```text
il suono del timer è partito
il focus audio non era disponibile
il servizio resta valido
```

Criterio di completamento:

```text
AudioServiceResult creato e usato come risultato pubblico composito
```

---

### TODO 004.07 — Creare AudioPlaybackState

Creare:

```text
services/CicloTimer.Audio/AudioPlaybackState.cs
```

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
2. servono solo al servizio audio;
3. non devono influenzare core;
4. servono per fail-safe e idempotenza.

Criterio di completamento:

```text
AudioPlaybackState creato come stato tecnico interno
```

---

### TODO 004.08 — Creare AudioServiceOptions

Creare:

```text
services/CicloTimer.Audio/AudioServiceOptions.cs
```

Forma consigliata:

```csharp
public sealed record AudioServiceOptions(
    string FinalAlertAudioPath);
```

Regole:

1. path default verso `Assets/final-alert.wav` copiato in output;
2. nessuna preferenza utente;
3. nessun volume configurabile;
4. nessuna scelta suono utente;
5. testabile con path falso.

Criterio di completamento:

```text
AudioServiceOptions creato per configurazione tecnica interna
```

---

## 10. FASE 4 — Interfacce e adapter

### TODO 004.09 — Creare IAudioPlayer

Creare:

```text
services/CicloTimer.Audio/IAudioPlayer.cs
```

Interfaccia concettuale consigliata:

```csharp
public interface IAudioPlayer
{
    AudioActionResult StartLoop(string audioFilePath);
    AudioActionResult Stop();
    bool IsPlaying { get; }
}
```

Regole:

1. astrae la riproduzione reale;
2. nessuna UI;
3. nessun WPF;
4. nessun testo utente;
5. deve permettere test con fake player.

Criterio di completamento:

```text
IAudioPlayer creato
```

---

### TODO 004.10 — Creare IAudioFocusManager

Creare:

```text
services/CicloTimer.Audio/IAudioFocusManager.cs
```

Interfaccia concettuale consigliata:

```csharp
public interface IAudioFocusManager
{
    AudioActionResult TryApplyFocus();
    AudioActionResult TryRestoreFocus();
}
```

Regole:

1. astrae il tentativo sicuro di audio focus;
2. non promette controllo totale sugli altri audio;
3. non manipola processi esterni;
4. non chiude app esterne;
5. non forza volume globale Windows;
6. se non supportato, restituisce `AudioFocusUnavailable`.

Criterio di completamento:

```text
IAudioFocusManager creato
```

---

### TODO 004.11 — Creare IAudioModificationTracker

Creare:

```text
services/CicloTimer.Audio/IAudioModificationTracker.cs
```

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

Regole:

1. traccia solo modifiche realmente applicate;
2. non inventa ripristini;
3. se `NullAudioFocusManager` non applica modifiche reali, può restare vuoto;
4. nessun riferimento unsafe a processi esterni.

Criterio di completamento:

```text
IAudioModificationTracker creato o equivalente coerente implementato
```

---

### TODO 004.12 — Creare AudioModificationSnapshot

Creare:

```text
services/CicloTimer.Audio/AudioModificationSnapshot.cs
```

Forma minimale consigliata:

```csharp
public sealed record AudioModificationSnapshot(
    string Scope,
    string Description);
```

Regole:

1. nessun handle unsafe;
2. nessun processo esterno manipolato;
3. solo supporto tecnico a testabilità/ripristino;
4. può restare minimale.

Criterio di completamento:

```text
AudioModificationSnapshot creato o equivalente coerente implementato
```

---

## 11. FASE 5 — Focus manager sicuro

### TODO 004.13 — Creare NullAudioFocusManager

Creare:

```text
services/CicloTimer.Audio/NullAudioFocusManager.cs
```

Comportamento:

```text
TryApplyFocus → AudioFocusUnavailable
TryRestoreFocus → Success
```

Regole:

1. non modifica audio esterno;
2. non traccia modifiche;
3. non fallisce in modo grave;
4. consente al servizio audio di funzionare;
5. è fallback sicuro.

Criterio di completamento:

```text
NullAudioFocusManager creato come fallback sicuro
```

---

### TODO 004.14 — Creare WindowsAudioFocusManager

Creare:

```text
services/CicloTimer.Audio/WindowsAudioFocusManager.cs
```

Prima versione:

```text
safe-first
```

Regola obbligatoria:

```text
se non esiste una strada semplice, sicura e chiaramente supportata per il ducking, usare NullAudioFocusManager o restituire AudioFocusUnavailable
```

È accettabile che `WindowsAudioFocusManager` nella prima versione:

1. deleghi a `NullAudioFocusManager`;
2. restituisca `AudioFocusUnavailable`;
3. documenti nel report che il ducking reale è rinviato.

È vietato:

1. manipolare processi esterni;
2. chiudere app esterne;
3. forzare mixer globale Windows;
4. manipolare sessioni audio arbitrarie in modo unsafe;
5. introdurre codice fragile per browser, Spotify, YouTube o chiamate.

Se Cursor ritiene necessaria una API avanzata come `IAudioSessionManager2`, deve fermarsi e segnalarlo nel report, senza implementare codice non richiesto.

Criterio di completamento:

```text
WindowsAudioFocusManager creato in forma sicura o fallback documentato
```

---

## 12. FASE 6 — Player audio Windows

### TODO 004.15 — Creare WindowsAudioPlayer

Creare:

```text
services/CicloTimer.Audio/WindowsAudioPlayer.cs
```

Prima scelta obbligatoria:

```text
System.Media.SoundPlayer
```

Regole:

1. usare `System.Media.SoundPlayer` se sufficiente;
2. riprodurre file `.wav`;
3. supportare loop/ripetizione;
4. supportare stop;
5. gestire errori in modo controllato;
6. non usare WPF;
7. non usare UI;
8. non usare librerie esterne.

`NAudio` o altre librerie audio esterne non sono autorizzate nella prima implementazione.

Se `System.Media.SoundPlayer` non è sufficiente per stop sicuro o loop/ripetizione:

1. fermarsi;
2. non installare pacchetti;
3. produrre report di blocco;
4. spiegare il problema;
5. proporre alternativa per revisione successiva.

Criterio di completamento:

```text
WindowsAudioPlayer creato con System.Media.SoundPlayer oppure blocco tecnico documentato
```

---

## 13. FASE 7 — AudioService

### TODO 004.16 — Creare AudioService

Creare:

```text
services/CicloTimer.Audio/AudioService.cs
```

Forma consigliata:

```csharp
public sealed class AudioService
{
    public AudioPlaybackState State { get; }

    public AudioServiceResult StartFinalAlertSound();

    public AudioServiceResult StopFinalAlertSound();
}
```

Regole:

1. i metodi pubblici restituiscono `AudioServiceResult`;
2. non usare solo `AudioActionResult` come ritorno pubblico principale;
3. coordinare `IAudioPlayer`;
4. coordinare `IAudioFocusManager`;
5. coordinare tracker modifiche;
6. mantenere stato tecnico interno;
7. nessuna UI;
8. nessuna localization;
9. nessun core;
10. nessun bridge.

Criterio di completamento:

```text
AudioService creato come coordinatore audio fail-safe
```

---

### TODO 004.17 — Implementare StartFinalAlertSound

`StartFinalAlertSound` deve:

1. se già `PlayingFinalAlert`, restituire `PlaybackResult = AlreadyPlaying`;
2. non avviare duplicati;
3. verificare disponibilità file audio;
4. se file mancante, restituire `PlaybackResult = AudioFileMissing`;
5. tentare avvio riproduzione;
6. se playback fallisce, restituire `PlaybackResult = PlaybackFailed`;
7. se playback riesce, impostare stato `PlayingFinalAlert`;
8. tentare focus/attenuazione solo dopo avvio o in ordine sicuro documentato;
9. se focus non disponibile, restituire `FocusResult = AudioFocusUnavailable`;
10. non trattare focus non disponibile come fallimento totale se playback riuscito;
11. non lanciare eccezioni non gestite;
12. non bloccare il timer.

Esempio corretto:

```text
PlaybackResult = Success
FocusResult = AudioFocusUnavailable
RestoreResult = NotAttempted
State = PlayingFinalAlert
```

Criterio di completamento:

```text
StartFinalAlertSound implementato, idempotente e fail-safe
```

---

### TODO 004.18 — Implementare StopFinalAlertSound

`StopFinalAlertSound` deve:

1. se già `Idle`, restituire `PlaybackResult = AlreadyStopped`;
2. se `Idle`, non invocare `IAudioPlayer.Stop`;
3. se `Idle`, non invocare `IAudioFocusManager.TryRestoreFocus`;
4. se playing, tentare stop riproduzione;
5. prima di chiamare `IAudioFocusManager.TryRestoreFocus`, verificare che esistano modifiche tracciate;
6. tentare ripristino solo se sono state tracciate modifiche;
7. se non ci sono modifiche tracciate, non chiamare `TryRestoreFocus`;
8. non tentare ripristini inventati;
9. tornare a `Idle` se possibile;
10. se ripristino fallisce, valorizzare `RestoreResult = RestoreFailed`;
11. non lanciare eccezioni non gestite;
12. non bloccare il timer.

Criterio di completamento:

```text
StopFinalAlertSound implementato, idempotente e fail-safe
```

---

## 14. FASE 8 — Creazione progetto test audio

### TODO 004.19 — Creare CicloTimer.Audio.Tests

Creare:

```text
tests/CicloTimer.Audio.Tests/
```

Creare:

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

Il progetto test deve referenziare solo:

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

Subito dopo la creazione, aggiungere il progetto alla solution:

```bash
dotnet sln CicloTimer.sln add tests/CicloTimer.Audio.Tests/CicloTimer.Audio.Tests.csproj
```

Criterio di completamento:

```text
progetto test audio creato, target corretto, riferimento solo ad Audio, aggiunto alla solution
```

---

## 15. FASE 9 — Test stato

### TODO 004.20 — Creare AudioServiceStateTests

Creare:

```text
tests/CicloTimer.Audio.Tests/AudioServiceStateTests.cs
```

Test obbligatori:

1. stato iniziale `Idle`;
2. start valido porta a `PlayingFinalAlert`;
3. `StartFinalAlertSound` con file esistente e player valido produce `PlaybackResult = Success`;
4. `StartFinalAlertSound` con file esistente e player valido produce `State = PlayingFinalAlert`;
5. stop valido torna a `Idle`;
6. file mancante porta a risultato controllato e stato sicuro;
7. playback fallito porta a risultato controllato e stato sicuro;
8. focus non disponibile non impedisce playback riuscito;
9. `AudioServiceResult` distingue playback e focus.

Criterio di completamento:

```text
AudioServiceStateTests superati
```

---

## 16. FASE 10 — Test idempotenza

### TODO 004.21 — Creare AudioServiceIdempotencyTests

Creare:

```text
tests/CicloTimer.Audio.Tests/AudioServiceIdempotencyTests.cs
```

Test obbligatori:

1. start su `Idle` avvia una sola riproduzione;
2. start ripetuto mentre già playing non avvia duplicati;
3. start ripetuto restituisce `AlreadyPlaying` in `PlaybackResult`;
4. stop su playing ferma;
5. stop ripetuto non genera errore grave;
6. stop ripetuto restituisce `AlreadyStopped` in `PlaybackResult`;
7. stop da `Idle` non invoca `IAudioPlayer.Stop`;
8. stop da `Idle` non invoca `IAudioFocusManager.TryRestoreFocus`;
9. nessuna sovrapposizione audio logica.

Criterio di completamento:

```text
AudioServiceIdempotencyTests superati
```

---

## 17. FASE 11 — Test fail-safe

### TODO 004.22 — Creare AudioServiceFailSafeTests

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

Criterio di completamento:

```text
AudioServiceFailSafeTests superati
```

---

## 18. FASE 12 — Test asset

### TODO 004.23 — Creare AudioServiceAssetTests

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
9. la durata viene verificata con tolleranza, ad esempio 250–350 ms;
10. il file è incluso nel `.csproj`;
11. il file viene copiato in output;
12. il path default del servizio punta al file copiato in output o a risorsa equivalente;
13. caso file mancante testato usando path falso o mock, non cancellando il file reale.

Criterio di completamento:

```text
AudioServiceAssetTests superati
```

---

## 19. FASE 13 — Test focus

### TODO 004.24 — Creare AudioServiceFocusTests

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
6. se non ci sono modifiche tracciate, `TryRestoreFocus` non viene chiamato;
7. se ci sono modifiche tracciate, restore viene tentato;
8. dopo restore riuscito, tracce vengono pulite;
9. restore fallito produce errore controllato;
10. tracker può restare vuoto quando viene usato `NullAudioFocusManager`.

Criterio di completamento:

```text
AudioServiceFocusTests superati
```

---

## 20. FASE 14 — Test dipendenze progetto

### TODO 004.25 — Creare ProjectDependencyTests

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
16. non contiene riferimenti a `NAudio` o altre librerie audio esterne;
17. usa `System.Media.SoundPlayer` come prima scelta per `WindowsAudioPlayer`, salvo blocco documentato.

I controlli possono essere test automatizzati o verifica documentata nel report finale.

Criterio di completamento:

```text
ProjectDependencyTests o verifiche equivalenti completati
```

---

## 21. FASE 15 — Build iniziale solution

### TODO 004.26 — Eseguire build prima di modificare ciclotimer.csproj

Eseguire dalla root:

```bash
dotnet build CicloTimer.sln
```

Se la build passa, Cursor non deve modificare `ciclotimer.csproj`.

Se la build fallisce perché il progetto WPF root include file sotto:

```text
services/**
```

allora Cursor può modificare solo `ciclotimer.csproj` aggiungendo:

```xml
<Compile Remove="services/**" />
```

Dopo questa eventuale modifica, rieseguire:

```bash
dotnet build CicloTimer.sln
```

Criterio di completamento:

```text
build solution riuscita, con eventuale modifica ciclotimer.csproj solo se necessaria
```

---

## 22. FASE 16 — Test finali

### TODO 004.27 — Eseguire test finali

Tutti i comandi devono essere eseguiti dalla root del repository.

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

1. test audio: tutti superati;
2. test core: tutti ancora superati;
3. test localization: tutti ancora superati;
4. test bridge: tutti ancora superati;
5. test solution completa: tutti superati;
6. nessun test fallito.

Criterio di completamento:

```text
tutti i test previsti superati
```

---

## 23. FASE 17 — Verifica working tree

### TODO 004.28 — Verificare file modificati

Cursor deve verificare che siano stati creati/modificati solo:

```text
services/CicloTimer.Audio/
tests/CicloTimer.Audio.Tests/
CicloTimer.sln
```

Eventualmente:

```text
ciclotimer.csproj
.gitignore
```

solo se necessario e motivato.

Non devono comparire modifiche a:

```text
models/CicloTimer.Core/
locales/CicloTimer.Localization/
view-models/CicloTimer.Bridge/
MainWindow.xaml
MainWindow.xaml.cs
App.xaml
App.xaml.cs
```

Criterio di completamento:

```text
working tree coerente con il perimetro del TODO
```

---

## 24. FASE 18 — Pulizia artefatti build

### TODO 004.29 — Verificare assenza bin/obj da committare

Cursor deve verificare che nel working tree non risultino file da committare sotto:

```text
bin/
obj/
```

Comandi utili:

```bash
git status --porcelain
```

PowerShell:

```powershell
git status --porcelain | Select-String -Pattern "bin|obj"
```

Se compaiono artefatti `bin/` o `obj/`, Cursor deve rimuovere solo directory chiamate esattamente:

```text
bin
obj
```

Non deve rimuovere:

```text
final-alert.wav
Assets/
file .cs
file .csproj
file .sln
documentazione
services/CicloTimer.Audio/
tests/CicloTimer.Audio.Tests/
```

Cursor non deve fare commit.

Criterio di completamento:

```text
nessun artefatto bin/obj da committare
```

---

## 25. FASE 19 — Report finale

### TODO 004.30 — Produrre report finale

Cursor deve produrre un report finale contenente:

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
11. conferma se il generatore WAV temporaneo è stato rimosso o lasciato nel repository;
12. se il generatore è stato lasciato, motivazione;
13. conferma asset incluso nel `.csproj`;
14. conferma asset copiato in output;
15. conferma audio non dipende da core;
16. conferma audio non dipende da localization;
17. conferma audio non dipende da bridge;
18. conferma audio non dipende da WPF root;
19. conferma UI non modificata;
20. conferma core non modificato;
21. conferma localization non modificato;
22. conferma bridge non modificato;
23. conferma assenza audio da internet;
24. conferma assenza audio copiato da Windows;
25. conferma assenza manipolazioni unsafe di processi esterni;
26. conferma assenza forzatura volume globale Windows;
27. conferma `AudioServiceResult` composito;
28. conferma distinzione playback/focus/restore;
29. conferma uso di `System.Media.SoundPlayer` o blocco tecnico documentato;
30. conferma assenza librerie audio esterne non autorizzate;
31. conferma idempotenza start;
32. conferma start valido con `PlaybackResult = Success` e `State = PlayingFinalAlert`;
33. conferma idempotenza stop;
34. conferma stop da Idle senza chiamate inutili a player/focus;
35. conferma `TryRestoreFocus` chiamato solo se esistono modifiche tracciate;
36. conferma fail-safe file mancante;
37. conferma fail-safe playback fallito;
38. conferma focus unavailable non blocca playback;
39. conferma restore fallito gestito;
40. eventuale uso di `NullAudioFocusManager` come fallback;
41. eventuale modifica a `ciclotimer.csproj` e motivo;
42. comandi eseguiti;
43. risultato build;
44. risultato test audio;
45. risultato test core;
46. risultato test localization;
47. risultato test bridge;
48. risultato `dotnet test`;
49. numero test audio;
50. eventuali test falliti;
51. eventuali deviazioni dal TODO;
52. conferma pulizia bin/obj;
53. output finale sintetico di `git status --porcelain`.

Cursor non deve limitarsi a scrivere:

```text
fatto
```

Il report deve essere verificabile.

---

## 26. Checklist sintetica finale

Prima di dichiarare completato il TODO, Cursor deve poter confermare:

```text
[ ] Verificati documenti design/coding plan
[ ] Creato services/CicloTimer.Audio/
[ ] Creato CicloTimer.Audio.csproj
[ ] Target net9.0-windows
[ ] Nessun riferimento a Core
[ ] Nessun riferimento a Localization
[ ] Nessun riferimento a Bridge
[ ] Nessun riferimento a WPF root
[ ] Creato Assets/
[ ] Generato final-alert.wav
[ ] final-alert.wav è WAV PCM valido
[ ] final-alert.wav è 16-bit mono 44100 Hz circa 300 ms
[ ] durata testata con tolleranza 250–350 ms
[ ] generatore WAV temporaneo rimosso o motivato nel report
[ ] final-alert.wav incluso nel csproj
[ ] final-alert.wav copiato in output
[ ] Creato AudioActionResult
[ ] Creato AudioServiceResult composito
[ ] Creato AudioPlaybackState
[ ] Creato AudioServiceOptions
[ ] Creato IAudioPlayer
[ ] Creato IAudioFocusManager
[ ] Creato IAudioModificationTracker o equivalente
[ ] Creato AudioModificationSnapshot o equivalente
[ ] Creato NullAudioFocusManager
[ ] Creato WindowsAudioFocusManager safe-first/fallback
[ ] Creato WindowsAudioPlayer con System.Media.SoundPlayer
[ ] Nessuna libreria audio esterna installata
[ ] Creato AudioService
[ ] StartFinalAlertSound idempotente
[ ] Start valido produce PlaybackResult Success e State PlayingFinalAlert
[ ] StopFinalAlertSound idempotente
[ ] Stop da Idle non invoca player/focus
[ ] TryRestoreFocus chiamato solo se ci sono modifiche tracciate
[ ] Focus unavailable non blocca playback
[ ] Restore fallito gestito
[ ] Creato tests/CicloTimer.Audio.Tests/
[ ] Creati test stato
[ ] Creati test idempotenza
[ ] Creati test fail-safe
[ ] Creati test asset
[ ] Creati test focus
[ ] Creati ProjectDependencyTests
[ ] dotnet build CicloTimer.sln superato
[ ] dotnet test audio superato
[ ] dotnet test core superato
[ ] dotnet test localization superato
[ ] dotnet test bridge superato
[ ] dotnet test generale superato se eseguito
[ ] Nessuna modifica a core
[ ] Nessuna modifica a localization
[ ] Nessuna modifica a bridge
[ ] Nessuna modifica a UI
[ ] Nessun audio da internet
[ ] Nessun audio da Windows Media
[ ] Nessuna manipolazione processi esterni
[ ] Nessuna forzatura volume globale Windows
[ ] Nessun bin/obj da committare
[ ] Report finale prodotto
```

---

## 27. Criteri di completamento globale

Il TODO 004 è completato solo se:

1. il progetto audio esiste;
2. il progetto test audio esiste;
3. entrambi sono nella solution;
4. il progetto audio usa `net9.0-windows`;
5. il progetto test audio usa `net9.0-windows`;
6. il progetto audio non referenzia core;
7. il progetto audio non referenzia localization;
8. il progetto audio non referenzia bridge;
9. il progetto audio non referenzia WPF root;
10. esiste `final-alert.wav`;
11. `final-alert.wav` è valido;
12. `final-alert.wav` è incluso nel progetto;
13. `final-alert.wav` viene copiato in output;
14. durata WAV testata con tolleranza 250–350 ms;
15. eventuale generatore temporaneo WAV è rimosso o motivato;
16. `AudioServiceResult` è composito;
17. playback/focus/restore sono distinguibili;
18. `System.Media.SoundPlayer` è usato come prima scelta o è stato prodotto blocco tecnico;
19. non sono state installate librerie esterne non autorizzate;
20. start è idempotente;
21. start valido produce `PlaybackResult = Success` e `State = PlayingFinalAlert`;
22. stop è idempotente;
23. stop da idle non chiama player/focus;
24. restore viene tentato solo con modifiche tracciate;
25. errori audio sono controllati;
26. focus unavailable non blocca playback;
27. restore fallito è controllato;
28. non ci sono manipolazioni unsafe;
29. non viene forzato volume globale Windows;
30. test audio passano;
31. regressioni core/localization/bridge passano;
32. build solution passa;
33. non ci sono artefatti `bin/obj` da committare;
34. il report finale è completo.

---

## 28. Criteri di non validità

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
21. la durata del WAV è fuori dalla tolleranza accettata;
22. usa file audio da internet;
23. usa file audio da Windows;
24. usa file audio con licenza non chiara;
25. lascia un generatore WAV temporaneo nel repository senza motivazione;
26. start ripetuto crea più riproduzioni;
27. start valido non produce `PlaybackResult = Success`;
28. start valido non porta a `State = PlayingFinalAlert`;
29. stop ripetuto genera errore grave;
30. stop da Idle invoca player/focus senza motivo;
31. restore viene tentato senza modifiche tracciate;
32. errore audio blocca il timer;
33. file mancante genera eccezione non gestita;
34. playback fallito genera eccezione non gestita;
35. focus fallito blocca playback;
36. restore fallito genera eccezione non gestita;
37. manipola processi esterni;
38. chiude app esterne;
39. forza volume globale Windows;
40. promette controllo totale sugli altri audio;
41. usa testi utente hardcoded;
42. dipende da localization per messaggi;
43. richiede hardware audio reale nei test;
44. richiede Spotify/browser/YouTube/chiamate nei test;
45. installa NAudio o librerie audio esterne senza autorizzazione;
46. build fallisce;
47. test falliscono;
48. ci sono artefatti `bin/obj` da committare.

---

## 29. Decisioni consolidate dopo revisione

Le seguenti decisioni sono consolidate in questa versione:

1. il TODO 004 è approvato come checklist operativa del servizio audio;
2. il servizio audio resta in `services/CicloTimer.Audio/`;
3. i test restano in `tests/CicloTimer.Audio.Tests/`;
4. il target è `net9.0-windows`;
5. il servizio audio non dipende da core, localization, bridge o UI;
6. l’asset audio è `services/CicloTimer.Audio/Assets/final-alert.wav`;
7. il file audio deve essere generato localmente;
8. il file audio deve essere WAV PCM 16-bit mono 44100 Hz, circa 300 ms, tono sinusoidale circa 880 Hz;
9. la durata deve essere testata con tolleranza 250–350 ms;
10. è vietato usare audio da internet, Windows Media o fonti con licenza non chiara;
11. il generatore WAV può essere temporaneo e non deve necessariamente restare nella soluzione finale;
12. se il generatore resta nel repository, deve essere motivato nel report finale;
13. `AudioServiceResult` composito è obbligatorio;
14. `AudioServiceResult` deve distinguere playback, focus e restore;
15. `System.Media.SoundPlayer` è la prima scelta per la riproduzione reale;
16. NAudio o altre librerie esterne non sono autorizzate nella prima implementazione;
17. se `SoundPlayer` non basta, Cursor deve fermarsi e produrre report di blocco;
18. `WindowsAudioFocusManager` segue criterio safe-first;
19. `NullAudioFocusManager` è fallback ammesso e coerente;
20. il tracker può restare vuoto se il focus manager non applica modifiche reali;
21. start valido deve produrre `PlaybackResult = Success` e `State = PlayingFinalAlert`;
22. stop da `Idle` non invoca player/focus;
23. `TryRestoreFocus` viene chiamato solo se esistono modifiche tracciate;
24. l’audio focus resta tentativo sicuro, non garanzia di controllo sugli altri audio;
25. nessuna manipolazione unsafe di processi esterni;
26. nessuna forzatura del volume globale Windows;
27. nessuna gestione aggressiva di chiamate o riunioni.

---

## 30. Stato del documento

Questo documento è approvato come TODO 004 — Audio service e audio focus.

Versione corrente:

```text
0.2.0 — approvazione dopo revisione tecnica
```

Cronologia:

```text
0.1.0 — prima bozza ChatGPT
0.2.0 — integrazione osservazioni revisione: generatore WAV temporaneo, tolleranza durata 250–350 ms, test esplicito StartFinalAlertSound valido con PlaybackResult Success e State PlayingFinalAlert, TryRestoreFocus solo con modifiche tracciate
```

Il documento è approvato come base operativa per l’implementazione del servizio audio.
