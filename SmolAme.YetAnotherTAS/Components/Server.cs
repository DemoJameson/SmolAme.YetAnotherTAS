using BepInEx.Configuration;
using TAS.Core;

namespace SmolAme.YetAnotherTAS.Components;

public class Server : PluginComponent {
    private ConfigEntry<int> port;

    private void Awake() {
        port = Plugin.Instance.Config.Bind("Advanced", "Communication Server Port", 19982);
        port.SettingChanged += (_, _) => CommunicationServer.Start(port.Value);
        CommunicationServer.Start(port.Value);
    }

    private void OnDestroy() {
        CommunicationServer.Stop();
    }
}