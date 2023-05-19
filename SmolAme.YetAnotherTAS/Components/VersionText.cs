using System.Text.RegularExpressions;

namespace SmolAme.YetAnotherTAS.Components; 

public class VersionText : PluginComponent {
    private static string origVersion;
    
    private void Update() {
        if (!HUDScript.HUD) {
            return;
        }

        origVersion = HUDScript.HUD.versionText.text;
        HUDScript.HUD.versionText.text = $"YetAnotherTAS v{MyPluginInfo.PLUGIN_VERSION}\n{HUDScript.HUD.versionText.text}";
        enabled = false;
    }

    private void OnDestroy() {
        HUDScript.HUD.versionText.text = origVersion;
    }

    public static int CurrentVersion() {
        return int.Parse(Regex.Split(origVersion, @"\D+")[5]);
    }
}