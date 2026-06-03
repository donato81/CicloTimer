using CicloTimer.Audio;
using CicloTimer.Bridge;

namespace CicloTimer.App;

public sealed class SystemActionDispatcher
{
    private readonly IAudioServicePort audioService;

    public SystemActionDispatcher(IAudioServicePort audioService)
    {
        this.audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
    }

    public SystemActionDispatchResult Dispatch(IReadOnlyList<SystemActionRequest> systemActions)
    {
        ArgumentNullException.ThrowIfNull(systemActions);

        AudioServiceResult? lastAudioResult = null;
        var executedActions = 0;
        var ignoredActions = 0;
        var hasWarning = false;
        var hasTechnicalError = false;

        foreach (var action in systemActions)
        {
            AudioServiceResult? audioResult;
            switch (action)
            {
                case SystemActionRequest.StartFinalAlertSound:
                    audioResult = ExecuteAudioAction(audioService.StartFinalAlertSound);
                    executedActions++;
                    break;

                case SystemActionRequest.StopFinalAlertSound:
                    audioResult = ExecuteAudioAction(audioService.StopFinalAlertSound);
                    executedActions++;
                    break;

                default:
                    ignoredActions++;
                    hasWarning = true;
                    continue;
            }

            lastAudioResult = audioResult;
            hasWarning |= IsAudioWarning(audioResult);
            hasTechnicalError |= IsAudioTechnicalError(audioResult);
        }

        return new SystemActionDispatchResult(
            lastAudioResult,
            executedActions,
            ignoredActions,
            hasWarning,
            hasTechnicalError);
    }

    private static AudioServiceResult ExecuteAudioAction(Func<AudioServiceResult> audioAction)
    {
        try
        {
            return audioAction();
        }
        catch (Exception)
        {
            return new AudioServiceResult(
                AudioActionResult.PlaybackFailed,
                AudioActionResult.NotAttempted,
                AudioActionResult.NotAttempted);
        }
    }

    private static bool IsAudioWarning(AudioServiceResult result)
    {
        return IsWarningResult(result.PlaybackResult)
            || IsWarningResult(result.FocusResult)
            || IsWarningResult(result.RestoreResult);
    }

    private static bool IsAudioTechnicalError(AudioServiceResult result)
    {
        return IsTechnicalErrorResult(result.PlaybackResult)
            || IsTechnicalErrorResult(result.FocusResult)
            || IsTechnicalErrorResult(result.RestoreResult);
    }

    private static bool IsWarningResult(AudioActionResult result)
    {
        return result is AudioActionResult.AudioFocusUnavailable
            or AudioActionResult.AudioFocusFailed
            or AudioActionResult.RestoreFailed
            or AudioActionResult.AudioFileMissing
            or AudioActionResult.PlaybackFailed;
    }

    private static bool IsTechnicalErrorResult(AudioActionResult result)
    {
        return result is AudioActionResult.AudioFocusFailed
            or AudioActionResult.RestoreFailed
            or AudioActionResult.AudioFileMissing
            or AudioActionResult.PlaybackFailed;
    }
}
