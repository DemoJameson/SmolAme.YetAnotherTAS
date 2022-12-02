using System;

namespace TAS.Shared.Communication.GameToStudio;

// ReSharper disable once StructCanBeMadeReadOnly

[Serializable]
public record struct GameInfoMessage : IGameToStudioMessage {
    public readonly int CurrentLine;
    public readonly string CurrentLineSuffix;
    public readonly int CurrentFrameInTas;
    public readonly int TotalFrames;
    public readonly States States;
    public readonly string GameInfo;
    public readonly string LevelName;
    public readonly string CurrentTime;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToPrimaryConstructor
    public GameInfoMessage(
        int currentLine, string currentLineSuffix, int currentFrameInTas, int totalFrames, States states,
        string gameInfo, string levelName, string currentTime) {
        CurrentLine = currentLine;
        CurrentLineSuffix = currentLineSuffix;
        CurrentFrameInTas = currentFrameInTas;
        TotalFrames = totalFrames;
        States = states;
        GameInfo = gameInfo;
        LevelName = levelName;
        CurrentTime = currentTime;
    }
}