using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAS.Core.Utils;
using TAS.Shared.Communication.GameToStudio;

namespace TAS.Core.Input.Commands;

public static class MetadataCommand {
    [TasCommand("RecordCount", AliasNames = new[] {"RecordCount:", "RecordCount："}, CalcChecksum = false)]
    private static void RecordCountCommand() {
        // dummy
    }

    public static void UpdateRecordCount() {
        UpdateAll(
            "RecordCount",
            command => (int.Parse(command.Args.FirstOrDefault() ?? "0") + 1).ToString(),
            command => int.TryParse(command.Args.FirstOrDefault() ?? "0", out int _));
    }

    public static void UpdateAll(string commandName, Func<Command, string> getMetadata, Func<Command, bool> predicate = null) {
        InputController inputController = Manager.Controller;
        string tasFilePath = InputController.TasFilePath;
        IEnumerable<Command> metadataCommands = inputController.Commands.SelectMany(pair => pair.Value)
            .Where(command => command.Is(commandName) && command.FilePath == InputController.TasFilePath)
            .Where(predicate ?? (_ => true))
            .ToList();

        Dictionary<int, string> updateLines = metadataCommands.Where(command => {
            string metadata = getMetadata(command);
            if (metadata.IsNullOrEmpty()) {
                return false;
            }

            if (command.Args.Length > 0 && command.Args[0] == metadata) {
                return false;
            }

            return true;
        }).ToDictionary(command => command.StudioLineNumber, command => $"{command.Attribute.Name}: {getMetadata(command)}");

        if (updateLines.IsEmpty()) {
            return;
        }

        string[] allLines = File.ReadAllLines(tasFilePath);
        foreach (int lineNumber in updateLines.Keys) {
            allLines[lineNumber] = updateLines[lineNumber];
        }

        File.WriteAllLines(tasFilePath, allLines);
        if (inputController.UsedFiles.ContainsKey(tasFilePath)) {
            inputController.UsedFiles[tasFilePath] = File.GetLastWriteTime(tasFilePath);
        }
        CommunicationServer.SendMessage(new UpdateTextsMessage(updateLines));
    }
}