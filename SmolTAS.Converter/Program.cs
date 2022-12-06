using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmolTAS.Converter;

internal class Program {
    private static readonly Regex ActionRegex = new(@"^([WSAD]+)(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex CleanComment = new(@"^\((.+)\)$", RegexOptions.Compiled);

    public static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Please drag and drop SmolTAS file or folder");
            return;
        }

        string path = args[0];
        FileAttributes attr = File.GetAttributes(path);

        if (attr.HasFlag(FileAttributes.Directory)) {
            ConvertFolder(path);
        } else {
            ConvertFile(path);
        }
    }

    private static void ConvertFolder(string folderPath) {
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

        foreach (FileInfo fileInfo in directoryInfo.GetFiles()) {
            if (fileInfo.Extension == ".txt") {
                ConvertFile(fileInfo.FullName);
            }
        }

        foreach (DirectoryInfo subDir in directoryInfo.GetDirectories()) {
            ConvertFolder(subDir.FullName);
        }
    }

    private static void ConvertFile(string filePath) {
        string newFilePath = filePath.Substring(0, filePath.Length - 3) + "tas";
        Console.WriteLine($"Converting {filePath} -> {newFilePath}");

        List<string> result = new();

        if (GetCharacter(filePath) is { } characterCommand) {
            result.Add(characterCommand);
        }

        if (GetLoadCommand(filePath) is { } loadCommand) {
            result.Add(loadCommand);
            result.Add("   1");
            result.Add("");
        }

        int frames = 1;
        List<string> allLines = File.ReadAllLines(filePath).ToList();

        for (int i = 0; i < allLines.Count; i++) {
            ParseLine(allLines[i], out string action, out string comment);
            ParseLine(i == allLines.Count - 1 ? null : allLines[i + 1], out string nextAction, out string _);

            if (action != nextAction || comment.Length > 0) {
                if (comment.Length > 0) {
                    result.Add($"#{comment}");
                }

                if (action.Length == 0) {
                    result.Add(frames.ToString().PadLeft(4));
                } else {
                    result.Add(frames.ToString().PadLeft(4) + "," + action);
                }

                frames = 1;
            } else {
                frames++;
            }
        }

        File.WriteAllText(newFilePath, string.Join("\n", result));
    }

    private static string GetCharacter(string filePath) {
        string name = Directory.GetParent(filePath)?.Name.ToUpper();
        int? index = name switch {
            "AME" => 1,
            "BLOOP" => 2,
            "KORONE" => 3,
            "ONIGIRI" => 4,
            "STACHE" => 5,
            "GURAWEIRD" => 6,
            "USHI" => 7,
            "SHUBA" => 8,
            "CATGURA" => 9,
            "COCO" => 10,
            "OLLIE" => 11,
            "REINE" => 12,
            _ => null,
        };

        return index == null ? null : $"Character {index}";
    }

    private static string GetLoadCommand(string filePath) {
        string fileName = Path.GetFileNameWithoutExtension(filePath).ToUpper();
        int? buildIndex;
        if (fileName.StartsWith("MAIN")) {
            buildIndex = 0;
        } else if (fileName.StartsWith("AOR")) {
            buildIndex = 5;
        } else if (fileName.StartsWith("AO")) {
            buildIndex = 1;
        } else if (fileName.StartsWith("POR")) {
            buildIndex = 2;
        } else if (fileName.StartsWith("RH")) {
            buildIndex = 3;
        } else if (fileName.StartsWith("PL")) {
            buildIndex = 4;
        } else if (fileName.StartsWith("TTM")) {
            buildIndex = 6;
        } else if (fileName.StartsWith("NO")) {
            buildIndex = 7;
        } else if (fileName.StartsWith("MM")) {
            buildIndex = 8;
        } else if (fileName.StartsWith("IM")) {
            buildIndex = 9;
        } else if (fileName.StartsWith("RU")) {
            buildIndex = 10;
        } else if (fileName.StartsWith("INA")) {
            buildIndex = 11;
        } else if (fileName.StartsWith("HCH")) {
            buildIndex = 12;
        } else if (fileName.StartsWith("REF")) {
            buildIndex = 13;
        } else {
            buildIndex = null;
        }

        return buildIndex == null ? null : $"Load {buildIndex}";
    }

    private static void ParseLine(string line, out string action, out string comment) {
        if (line == null) {
            action = null;
            comment = "";
            return;
        }

        action = "";
        comment = "";


        if (ActionRegex.IsMatch(line)) {
            Match match = ActionRegex.Match(line);
            List<char> actions = match.Groups[1].Value.ToUpper().ToArray().Select(c => c switch {
                'W' => 'U',
                'S' => 'D',
                'A' => 'L',
                'D' => 'R',
                _ => '\0',
            }).ToList();
            actions.Sort(comparison);
            action = string.Join(",", actions);
            comment = CleanComment.Replace(match.Groups[2].Value.Trim(), "$1");
        }
    }

    // Order: L R U D
    private static readonly Comparison<char> comparison = (c1, c2) => {
        if (c1 == 'D') {
            c1 = 'Z';
        }
        if (c2 == 'D') {
            c2 = 'Z';
        }

        return c1 - c2;
    };
}