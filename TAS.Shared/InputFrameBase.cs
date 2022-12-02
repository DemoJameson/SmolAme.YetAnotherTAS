using System.Collections.Generic;
using System.Text;

namespace TAS.Shared;

public abstract class InputFrameBase {
    public Actions Actions { get; set; }
    public int Frames { get; set; }
    public bool HasActions(Actions actions) => (Actions & actions) != 0;
    
    public string ToActionsString() {
        StringBuilder sb = new();

        foreach (KeyValuePair<char, Actions> pair in ActionsUtils.Chars) {
            if (HasActions(pair.Value)) {
                sb.Append($",{pair.Key}");
            }
        }

        return sb.ToString();
    }
}