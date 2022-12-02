using TAS.Core.Input;

namespace TAS.Core; 

public interface IGame {
    string CurrentTime { get; }
    float FastForwardSpeed { get; }
    float SlowForwardSpeed { get; }
    string GameInfo { get; }
    string LevelName { get; }
    ulong FrameCount { get; }
    bool IsLoading { get; }
    void SetInputs(InputFrame inputFrame);
    void SetFrameRate(float multiple);
}