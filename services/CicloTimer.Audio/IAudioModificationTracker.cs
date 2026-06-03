namespace CicloTimer.Audio;

public interface IAudioModificationTracker
{
    bool HasTrackedModifications { get; }

    void Track(AudioModificationSnapshot snapshot);

    IReadOnlyList<AudioModificationSnapshot> GetTrackedModifications();

    void Clear();
}
