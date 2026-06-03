namespace CicloTimer.Audio;

public sealed record AudioServiceResult(
    AudioActionResult PlaybackResult,
    AudioActionResult FocusResult,
    AudioActionResult RestoreResult);
