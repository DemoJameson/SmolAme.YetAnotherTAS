using System;

namespace TAS.Shared.Communication.StudioToGame;

[Serializable]
public record struct PathMessage : IStudioToGameMessage {
    public string Path;
    public PathMessage(string path) {
        if (PlatformUtils.Wine && path.StartsWith("Z:\\", StringComparison.InvariantCultureIgnoreCase)) {
            path = path.Substring(2, path.Length - 2).Replace("\\", "/");
        }
        Path = path;
    }
}