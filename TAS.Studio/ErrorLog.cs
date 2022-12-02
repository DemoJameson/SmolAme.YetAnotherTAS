using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TAS.Studio;

public static class ErrorLog {
    private const string Filename = "tas_studio_log.txt";
    private const string Marker = "==========================================";
    public static string ModVersion = "Unknown";

    public static void Write(Exception e) {
        Write(e.ToString());
    }

    public static void Write(string str) {
        StringBuilder stringBuilder = new();
        string text = "";
        if (Path.IsPathRooted(Filename)) {
            string directoryName = Path.GetDirectoryName(Filename);
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }
        }

        if (File.Exists(Filename)) {
            text = File.ReadAllText(Filename);
            if (!text.Contains(Marker)) {
                text = "";
            }
        }

        stringBuilder.AppendLine("TAS.Studio Error Log");
        stringBuilder.AppendLine(Marker);
        stringBuilder.AppendLine();

        stringBuilder.Append("TAS.Studio v");
        stringBuilder.Append(Studio.Version.ToString(3));
        stringBuilder.AppendLine($" & TAS.Core v{ModVersion}");

        stringBuilder.AppendLine(DateTime.Now.ToString());
        stringBuilder.AppendLine(str);
        if (text != "") {
            int startIndex = text.IndexOf(Marker) + Marker.Length;
            string value = text.Substring(startIndex);
            stringBuilder.AppendLine(value);
        }

        StreamWriter streamWriter = new(Filename, append: false);
        streamWriter.Write(stringBuilder.ToString());
        streamWriter.Close();
    }

    public static void Open() {
        if (File.Exists(Filename)) {
            Process.Start(Filename);
        }
    }
}