namespace CicloTimer.Audio;

public sealed class AudioModificationTracker : IAudioModificationTracker
{
    private readonly List<AudioModificationSnapshot> snapshots = [];

    public bool HasTrackedModifications => snapshots.Count > 0;

    public void Track(AudioModificationSnapshot snapshot)
    {
        snapshots.Add(snapshot);
    }

    public IReadOnlyList<AudioModificationSnapshot> GetTrackedModifications()
    {
        return snapshots.ToArray();
    }

    public void Clear()
    {
        snapshots.Clear();
    }
}
