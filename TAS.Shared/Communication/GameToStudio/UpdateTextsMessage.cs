using System;
using System.Collections.Generic;

namespace TAS.Shared.Communication.GameToStudio; 

[Serializable]
public record struct UpdateTextsMessage(Dictionary<int, string> Texts) : IGameToStudioMessage {
    public Dictionary<int, string> Texts = Texts;
}