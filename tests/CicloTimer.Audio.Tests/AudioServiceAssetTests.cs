using System.Xml.Linq;
using CicloTimer.Audio;

namespace CicloTimer.Audio.Tests;

public sealed class AudioServiceAssetTests
{
    [Fact]
    public void FinalAlertWavExistsAndIsNotEmpty()
    {
        string path = GetRepositoryAssetPath();

        Assert.True(File.Exists(path));
        Assert.Equal(".wav", Path.GetExtension(path));
        Assert.True(new FileInfo(path).Length > 0);
    }

    [Fact]
    public void FinalAlertWavHasExpectedPcmFormat()
    {
        WavInfo info = ReadWavInfo(GetRepositoryAssetPath());

        Assert.Equal(1, info.AudioFormat);
        Assert.Equal(1, info.Channels);
        Assert.Equal(16, info.BitsPerSample);
        Assert.Equal(44100, info.SampleRate);
    }

    [Fact]
    public void FinalAlertWavDurationIsWithinTolerance()
    {
        WavInfo info = ReadWavInfo(GetRepositoryAssetPath());

        Assert.InRange(info.Duration.TotalMilliseconds, 250, 350);
    }

    [Fact]
    public void FinalAlertWavIsIncludedAndCopiedToOutput()
    {
        XDocument project = XDocument.Load(GetAudioProjectPath());
        XElement content = project
            .Descendants("Content")
            .Single(element => (string?)element.Attribute("Include") == @"Assets\final-alert.wav");

        Assert.Equal("PreserveNewest", content.Element("CopyToOutputDirectory")?.Value);
    }

    [Fact]
    public void DefaultOptionsPointToOutputAssetPath()
    {
        string expected = Path.Combine(AppContext.BaseDirectory, "Assets", "final-alert.wav");

        Assert.Equal(expected, AudioServiceOptions.Default.FinalAlertAudioPath);
    }

    [Fact]
    public void MissingFileCaseUsesFalsePathWithoutDeletingRealAsset()
    {
        string realAssetPath = GetRepositoryAssetPath();
        string missingPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.wav");
        AudioService service = AudioServiceTestFactory.Create(missingPath);

        AudioServiceResult result = service.StartFinalAlertSound();

        Assert.Equal(AudioActionResult.AudioFileMissing, result.PlaybackResult);
        Assert.True(File.Exists(realAssetPath));
    }

    private static string GetRepositoryAssetPath()
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "services",
            "CicloTimer.Audio",
            "Assets",
            "final-alert.wav"));
    }

    private static string GetAudioProjectPath()
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "services",
            "CicloTimer.Audio",
            "CicloTimer.Audio.csproj"));
    }

    private static WavInfo ReadWavInfo(string path)
    {
        using BinaryReader reader = new(File.OpenRead(path));
        string riff = new(reader.ReadChars(4));
        _ = reader.ReadInt32();
        string wave = new(reader.ReadChars(4));
        string formatChunkId = new(reader.ReadChars(4));
        int formatChunkSize = reader.ReadInt32();
        short audioFormat = reader.ReadInt16();
        short channels = reader.ReadInt16();
        int sampleRate = reader.ReadInt32();
        _ = reader.ReadInt32();
        _ = reader.ReadInt16();
        short bitsPerSample = reader.ReadInt16();

        if (formatChunkSize > 16)
        {
            reader.ReadBytes(formatChunkSize - 16);
        }

        string dataChunkId = new(reader.ReadChars(4));
        while (dataChunkId != "data")
        {
            int chunkSize = reader.ReadInt32();
            reader.ReadBytes(chunkSize);
            dataChunkId = new(reader.ReadChars(4));
        }

        int dataSize = reader.ReadInt32();

        Assert.Equal("RIFF", riff);
        Assert.Equal("WAVE", wave);
        Assert.Equal("fmt ", formatChunkId);

        int bytesPerSample = bitsPerSample / 8;
        double sampleCount = dataSize / (double)(channels * bytesPerSample);
        TimeSpan duration = TimeSpan.FromSeconds(sampleCount / sampleRate);

        return new WavInfo(audioFormat, channels, sampleRate, bitsPerSample, duration);
    }

    private sealed record WavInfo(
        short AudioFormat,
        short Channels,
        int SampleRate,
        short BitsPerSample,
        TimeSpan Duration);
}
