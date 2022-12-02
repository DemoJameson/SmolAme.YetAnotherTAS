using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TAS.Shared;
using TAS.Shared.Communication.GameToStudio;
using TAS.Shared.Communication.StudioToGame;
using TAS.Studio.Communication;
using TAS.Studio.Input;
using TAS.Studio.RichText;
using Keys = System.Windows.Forms.Keys;

namespace TAS.Studio;

public partial class Studio : BaseForm {
    private const string MaxStatusHeight20Line = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n";
    public static Studio Instance;
    public static Version Version { get; private set; }
    private static List<string> RecentFiles => Settings.Instance.RecentFiles;
    public readonly List<InputFrame> InputRecords = new() {new InputFrame("")};
    private readonly StringBuilder statusBarBuilder = new();

    private DateTime lastChanged = DateTime.MinValue;
    private FormWindowState lastWindowState = FormWindowState.Normal;
    private States tasStates;
    private ToolTip tooltip;
    private ToolTip titleBarTooltip;
    private int totalFrames, currentFrame;
    private bool updating;

    private bool DisableTyping => tasStates.HasFlag(States.Enable) && !tasStates.HasFlag(States.FrameStep) && CommunicationClient.Connected ||
                                  CommunicationWrapper.Forwarding;

    private string TitleBarText =>
        (string.IsNullOrEmpty(CurrentFileName) ? "game.tas" : Path.GetFileName(CurrentFileName))
        + " - Studio v"
        + Version.ToString(3)
        + (string.IsNullOrEmpty(CurrentFileName) ? string.Empty : "   " + CurrentFileName);

    public static string CurrentFileName {
        get => Instance.richText.CurrentFileName;
        set => Instance.richText.CurrentFileName = value;
    }

    private Tuple<string, int> previousFile;

    public Studio(string[] args) {
        Instance = this;
        Version = Assembly.GetExecutingAssembly().GetName().Version;

        InitializeComponent();
        InitSettings();
        InitLocationSize();
        InitTitleBarTooltip();
        InitMenu();
        InitDragDrop();
        InitFont(Settings.Instance.Font ?? fontDialog.Font);
        EnableStudio(false);
        TryOpenFile(args);
        CommunicationClient.Connect();

        Text = TitleBarText;
    }

    private void InitSettings() {
        Settings.Load();
        Settings.StartWatcher();
        VisibleChanged += (_, _) => {
            if (!Visible) {
                SaveSettings();
            }
        };
    }

    private void InitLocationSize() {
        DesktopLocation = Settings.Instance.Location;
        Size = Settings.Instance.Size;

        if (!IsTitleBarVisible()) {
            DesktopLocation = new Point(100, 100);
            Size = new Size(400, 800);
        }
    }

    private void InitTitleBarTooltip() {
        NonClientMouseHover += (_, _) => {
            if (TitleRectangle.Contains(Cursor.Position) && !string.IsNullOrEmpty(CurrentFileName)) {
                titleBarTooltip ??= new ToolTip();
                titleBarTooltip.Show(CurrentFileName, this, TitleRectangle.Left - Left + 1, TitleRectangle.Bottom - Top, int.MaxValue);
            }
        };

        NonClientMouseLeave += (_, _) => {
            if (!TitleRectangle.Contains(Cursor.Position)) {
                titleBarTooltip?.Hide(this);
            }
        };

        richText.MouseHover += (_, _) => titleBarTooltip?.Hide(this);
    }

    private void InitMenu() {
        richText.MouseClick += (_, args) => {
            if (DisableTyping) {
                return;
            }

            if ((args.Button & MouseButtons.Right) == MouseButtons.Right) {
                if (richText.Selection.IsEmpty) {
                    richText.Selection.Start = richText.PointToPlace(args.Location);
                    richText.Invalidate();
                }

                tasTextContextMenuStrip.Show(Cursor.Position);
            } else if (ModifierKeys == Keys.Control && (args.Button & MouseButtons.Left) == MouseButtons.Left) {
                TryOpenReadFile();
                TryGoToPlayLine();
            }
        };

        statusBar.MouseClick += (_, args) => {
            if ((args.Button & MouseButtons.Right) == 0) {
                return;
            }

            statusBarContextMenuStrip.Show(Cursor.Position);
        };

        openRecentMenuItem.DropDownItemClicked += (_, args) => {
            ToolStripItem clickedItem = args.ClickedItem;
            if (clickedItem.Text == "Clear") {
                openPreviousFileToolStripMenuItem.Enabled = false;
                RecentFiles.Clear();
                if (File.Exists(CurrentFileName)) {
                    RecentFiles.Add(CurrentFileName);
                }

                return;
            }

            if (!File.Exists(clickedItem.Text)) {
                openRecentMenuItem.Owner.Hide();
                RecentFiles.Remove(clickedItem.Text);
            }

            OpenFile(clickedItem.Text);
        };

        openBackupToolStripMenuItem.DropDownItemClicked += (_, args) => {
            ToolStripItem clickedItem = args.ClickedItem;
            string backupFolder = richText.BackupFolder;
            if (clickedItem.Text == "Delete All Files") {
                Directory.Delete(backupFolder, true);
                return;
            } else if (clickedItem.Text == "Show Older Files...") {
                if (!Directory.Exists(backupFolder)) {
                    Directory.CreateDirectory(backupFolder);
                }

                Process.Start(backupFolder);
                return;
            }

            string filePath = Path.Combine(backupFolder, clickedItem.Text);
            if (!File.Exists(filePath)) {
                openRecentMenuItem.Owner.Hide();
            }

            OpenFile(filePath);
        };

        settingsToolStripMenuItem.DropDown.Opacity = 0f;
    }

    private void InitDragDrop() {
        richText.DragDrop += (_, args) => {
            string[] fileList = (string[]) args.Data.GetData(DataFormats.FileDrop, false);
            if (fileList.Length > 0 && fileList[0].EndsWith(".tas")) {
                OpenFile(fileList[0]);
            }
        };

        richText.DragEnter += (_, args) => {
            string[] fileList = (string[]) args.Data.GetData(DataFormats.FileDrop, false);
            if (fileList.Length > 0 && fileList[0].EndsWith(".tas")) {
                args.Effect = DragDropEffects.Copy;
            }
        };
    }

    private void InitFont(Font font) {
        richText.Font = font;
        lblStatus.Font = new Font(font.FontFamily, (font.Size - 1) * 0.8f, font.Style);
    }

    private void CreateRecentFilesMenu() {
        openRecentMenuItem.DropDownItems.Clear();
        if (RecentFiles.Count == 0) {
            openRecentMenuItem.DropDownItems.Add(new ToolStripMenuItem("Nothing") {
                Enabled = false
            });
        } else {
            for (var i = RecentFiles.Count - 1; i >= 20; i--) {
                RecentFiles.Remove(RecentFiles[i]);
            }

            foreach (var fileName in RecentFiles) {
                openRecentMenuItem.DropDownItems.Add(new ToolStripMenuItem(fileName) {
                    Checked = CurrentFileName == fileName
                });
            }

            openRecentMenuItem.DropDownItems.Add(new ToolStripSeparator());
            openRecentMenuItem.DropDownItems.Add(new ToolStripMenuItem("Clear"));
        }
    }

    private void CreateBackupFilesMenu() {
        openBackupToolStripMenuItem.DropDownItems.Clear();
        string backupFolder = richText.BackupFolder;
        List<string> files = Directory.Exists(backupFolder) ? Directory.GetFiles(backupFolder).ToList() : new List<string>();
        if (files.Count == 0) {
            openBackupToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("Nothing") {
                Enabled = false
            });
        } else {
            files.Sort();
            files.Reverse();

            foreach (string filePath in files.Take(20)) {
                openBackupToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(Path.GetFileName(filePath)) {
                    Checked = CurrentFileName == filePath
                });
            }

            if (files.Count > 20) {
                openBackupToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("Show Older Files..."));
            }

            openBackupToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            openBackupToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("Delete All Files"));
        }
    }

    private bool IsTitleBarVisible() {
        int titleBarHeight = RectangleToScreen(ClientRectangle).Top - Top;
        Rectangle titleBar = new(Left, Top, Width, titleBarHeight);
        foreach (Screen screen in Screen.AllScreens) {
            if (screen.Bounds.IntersectsWith(titleBar)) {
                return true;
            }
        }

        return false;
    }

    private void SaveSettings() {
        Settings.Instance.Location = DesktopLocation;
        Settings.Instance.Size = Size;
        Settings.Save();
    }

    private void ShowTooltip(string text) {
        if (tooltip == null) {
            tooltip = new ToolTip();
        } else {
            tooltip.Hide(this);
        }

        Size textSize = TextRenderer.MeasureText(text, Font);
        tooltip.Show(text, this, Width / 2 - textSize.Width / 2, Height / 2 - textSize.Height / 2, 2000);
    }

    private void TASStudio_FormClosed(object sender, FormClosedEventArgs e) {
        Settings.StopWatcher();
        SaveSettings();
        CommunicationClient.SendMessage(new PathMessage(""));
        Thread.Sleep(50);
    }

    private void Studio_Shown(object sender, EventArgs e) {
        Thread updateThread = new(UpdateLoop);
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
        // if (msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN)
        if (msg.Msg is 0x100 or 0x104) {
            if (!richText.IsChanged && CommunicationWrapper.CheckControls(ref msg)) {
                return true;
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void Studio_KeyDown(object sender, KeyEventArgs e) {
        if (richText.ReadOnly) {
            return;
        }

        try {
            if (e.Modifiers == Keys.Control) {
                switch (e.KeyCode) {
                    case Keys.S: // Ctrl + S
                        richText.SaveFile();
                        break;
                    case Keys.O: // Ctrl + O
                        OpenFile();
                        break;
                    case Keys.OemQuestion: // Ctrl + /
                    case Keys.K: // Ctrl + K
                        CommentText(true);
                        break;
                    case Keys.P: // Ctrl + P
                        ClearUncommentedBreakpoints();
                        break;
                    case Keys.OemPeriod: // Ctrl + OemPeriod -> insert/remove breakpoint
                        InsertOrRemoveText(InputFrame.BreakpointRegex, "***");
                        break;
                    case Keys.R: // Ctrl + R
                        InsertRoomName();
                        break;
                    case Keys.F: // Ctrl + F
                        DialogUtils.ShowFindDialog(richText);
                        break;
                    case Keys.G: // Ctrl + G
                        DialogUtils.ShowGoToDialog(richText);
                        break;
                    case Keys.T: // Ctrl + T
                        InsertTime();
                        break;
                    case Keys.Down: // Ctrl + Down
                        GoDownCommentAndBreakpoint(e);
                        break;
                    case Keys.Up: // Ctrl + Up
                        GoUpCommentAndBreakpoint(e);
                        break;
                    case Keys.L: // Ctrl + L
                        CombineInputs(true);
                        break;
                }
            } else if (e.Modifiers == (Keys.Control | Keys.Shift)) {
                switch (e.KeyCode) {
                    case Keys.OemQuestion: // Ctrl + Shift + /
                    case Keys.K: // Ctrl + Shift + K
                        CommentText(false);
                        break;
                    case Keys.S: // Ctrl + Shift + S
                        SaveAsFile();
                        break;
                    case Keys.P: // Ctrl + Shift + P
                        ClearBreakpoints();
                        break;
                    case Keys.R: // Ctrl + Shift + R
                        if (CommunicationWrapper.GameInfo is {} info) {
                            InsertNewLine($"Load {info.LevelName}");
                        }
                        break;
                    case Keys.C: // Ctrl + Shift + C
                        CopyGameInfo();
                        break;
                    case Keys.D: // Ctrl + Shift + D
                        CommunicationClient.Connect();
                        break;
                    case Keys.L: // Ctrl + Shift + L
                        CombineInputs(false);
                        break;
                }
            } else if (e.Modifiers == (Keys.Control | Keys.Alt)) {
                switch (e.KeyCode) {
                    case Keys.C: // Ctrl + Alt + C
                        CopyFilePath();
                        break;
                    case Keys.P: // Ctrl + Alt + P
                        CommentUncommentAllBreakpoints();
                        break;
                }
            }
        } catch (Exception ex) {
            MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Console.Write(ex);
        }
    }

    private void SaveAsFile() {
        richText.SaveNewFile();
        CommunicationClient.SendMessage(new PathMessage(CurrentFileName));
        Text = TitleBarText;
        UpdateRecentFiles();
    }

    private void GoDownCommentAndBreakpoint(KeyEventArgs e) {
        List<int> commentLine = richText.FindLines(@"^\s*#|^\*\*\*");
        if (commentLine.Count > 0) {
            int line = commentLine.FirstOrDefault(i => i > richText.Selection.Start.iLine);
            if (line == 0) {
                line = richText.LinesCount - 1;
            }

            while (richText.Selection.Start.iLine < line) {
                richText.Selection.GoDown(e.Shift);
            }

            richText.ScrollLeft();
        } else {
            richText.Selection.GoDown(e.Shift);
            richText.ScrollLeft();
        }
    }

    private void GoUpCommentAndBreakpoint(KeyEventArgs e) {
        List<int> commentLine = richText.FindLines(@"^\s*#|^\*\*\*");
        if (commentLine.Count > 0) {
            int line = commentLine.FindLast(i => i < richText.Selection.Start.iLine);
            while (richText.Selection.Start.iLine > line) {
                richText.Selection.GoUp(e.Shift);
            }

            richText.ScrollLeft();
        } else {
            richText.Selection.GoUp(e.Shift);
            richText.ScrollLeft();
        }
    }

    public void TryOpenFile(string[] args) {
        if (args.Length > 0 && args[0] is { } filePath && filePath.EndsWith(".tas", StringComparison.InvariantCultureIgnoreCase) &&
            TryGetExactCasePath(filePath, out string exactPath)) {
            OpenFile(exactPath);
        }
    }

    private static bool TryGetExactCasePath(string path, out string exactPath) {
        bool result = false;
        exactPath = null;

        // DirectoryInfo accepts either a file path or a directory path, and most of its properties work for either.
        // However, its Exists property only works for a directory path.
        DirectoryInfo directory = new(path);
        if (File.Exists(path) || directory.Exists) {
            List<string> parts = new();

            DirectoryInfo parentDirectory = directory.Parent;
            while (parentDirectory != null) {
                FileSystemInfo entry = parentDirectory.EnumerateFileSystemInfos(directory.Name).First();
                parts.Add(entry.Name);

                directory = parentDirectory;
                parentDirectory = directory.Parent;
            }

            // Handle the root part (i.e., drive letter or UNC \\server\share).
            string root = directory.FullName;
            if (root.Contains(':')) {
                root = root.ToUpper();
            } else {
                string[] rootParts = root.Split('\\');
                root = string.Join("\\", rootParts.Select(part => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(part)));
            }

            parts.Add(root);
            parts.Reverse();
            exactPath = Path.Combine(parts.ToArray());
            result = true;
        }

        return result;
    }

    private void OpenFile(string fileName = null, int startLine = 0) {
        if (fileName == CurrentFileName && fileName != null) {
            return;
        }

        Tuple<string, int> tuple = new(CurrentFileName, richText.Selection.Start.iLine);
        if (richText.OpenFile(fileName)) {
            previousFile = tuple;
            UpdateRecentFiles();
            richText.GoHome();
            if (startLine > 0) {
                startLine = Math.Min(startLine, richText.LinesCount - 1);
                richText.Selection = new Range(richText, 0, startLine, 0, startLine);
                richText.DoSelectionVisible();
            }
        }

        CommunicationClient.SendMessage(new PathMessage(CurrentFileName));
        Text = TitleBarText;
    }

    private void TryOpenReadFile() {
        string text = richText.CurrentStartLineText;
        string trimText = richText.CurrentStartLineText.Trim();
        if (trimText.StartsWith("read", StringComparison.InvariantCultureIgnoreCase)) {
            Regex spaceRegex = new(@"^[^,]+?\s+[^,]");

            string[] args = spaceRegex.IsMatch(trimText) ? trimText.Split() : trimText.Split(',');

            bool jumpToEndLabel = false;
            int clickPosition = richText.Selection.Start.iChar;
            int leftSpace = text.Length - text.TrimStart().Length;
            if (args.Length >= 4 && args[0].Length + args[1].Length + args[2].Length + 2 + leftSpace < clickPosition) {
                jumpToEndLabel = true;
            }

            args = args.Select(text => text.Trim()).ToArray();
            if (args[0].Equals("read", StringComparison.InvariantCultureIgnoreCase) && args.Length >= 2) {
                string filePath = args[1];
                string fileDirectory = Path.GetDirectoryName(CurrentFileName);
                filePath = FindTheFile(fileDirectory, filePath);

                if (!File.Exists(filePath)) {
                    // for compatibility with tas files downloaded from discord
                    // discord will replace spaces in the file name with underscores
                    filePath = args[1].Replace(" ", "_");
                    filePath = FindTheFile(fileDirectory, filePath);
                }

                if (!File.Exists(filePath)) {
                    return;
                }

                int startLine = 0;

                if (jumpToEndLabel) {
                    startLine = GetLine(filePath, args[3]);
                } else if (args.Length >= 3) {
                    startLine = GetLine(filePath, args[2]);
                }

                OpenFile(filePath, startLine);
            }
        }

        string FindTheFile(string fileDirectory, string filePath) {
            // Check for full and shortened Read versions
            if (fileDirectory != null) {
                // Path.Combine can handle the case when filePath is an absolute path
                string absoluteOrRelativePath = Path.Combine(fileDirectory, filePath);
                if (File.Exists(absoluteOrRelativePath) && absoluteOrRelativePath != CurrentFileName) {
                    filePath = absoluteOrRelativePath;
                } else {
                    string[] files = Directory.GetFiles(fileDirectory, $"{filePath}*.tas");
                    if (files.FirstOrDefault(path => path != CurrentFileName) is { } shortenedFilePath) {
                        filePath = shortenedFilePath;
                    }
                }
            }

            return filePath;
        }
    }

    private void TryGoToPlayLine() {
        string lineText = richText.CurrentStartLineText.Trim();
        if (!new Regex(@"^#?play", RegexOptions.IgnoreCase).IsMatch(lineText)) {
            return;
        }

        Regex spaceRegex = new(@"^[^,]+?\s+[^,]");

        string[] args = spaceRegex.IsMatch(lineText) ? lineText.Split() : lineText.Split(',');
        args = args.Select(text => text.Trim()).ToArray();
        if (!new Regex(@"^#?play", RegexOptions.IgnoreCase).IsMatch(args[0]) || args.Length < 2) {
            return;
        }

        string lineOrLabel = args[1];
        int? lineNumber = null;
        if (int.TryParse(lineOrLabel, out int parseLine)) {
            lineNumber = parseLine;
        } else {
            List<int> findLines = richText.FindLines($"#{lineOrLabel}");
            if (findLines.Count > 0) {
                lineNumber = findLines.First();
            }
        }

        if (lineNumber.HasValue) {
            richText.GoToLine(lineNumber.Value);
        }
    }

    private static int GetLine(string path, string labelOrLineNumber) {
        if (int.TryParse(labelOrLineNumber, out int lineNumber)) {
            return lineNumber;
        }

        int curLine = 0;
        foreach (string readLine in File.ReadLines(path)) {
            curLine++;
            string line = readLine.Trim();
            if (line == $"#{labelOrLineNumber}") {
                return curLine - 1;
            }
        }

        return 0;
    }

    private void UpdateRecentFiles() {
        if (string.IsNullOrEmpty(CurrentFileName)) {
            return;
        }

        if (RecentFiles.Contains(CurrentFileName)) {
            RecentFiles.Remove(CurrentFileName);
        }

        RecentFiles.Insert(0, CurrentFileName);
        openPreviousFileToolStripMenuItem.Enabled = RecentFiles.Count >= 2;
        Settings.Instance.LastFileName = CurrentFileName;
        SaveSettings();
    }

    private void ClearUncommentedBreakpoints() {
        var line = Math.Min(richText.Selection.Start.iLine, richText.Selection.End.iLine);
        List<int> breakpoints = richText.FindLines(@"^\s*\*\*\*");
        richText.RemoveLines(breakpoints);
        richText.Selection.Start = new Place(0, Math.Min(line, richText.LinesCount - 1));
    }

    private void ClearBreakpoints() {
        var line = Math.Min(richText.Selection.Start.iLine, richText.Selection.End.iLine);
        List<int> breakpoints = richText.FindLines(@"^\s*#*\s*\*\*\*");
        richText.RemoveLines(breakpoints);
        richText.Selection.Start = new Place(0, Math.Min(line, richText.LinesCount - 1));
    }

    private void CopyFilePath() {
        if (string.IsNullOrEmpty(CurrentFileName)) {
            return;
        }

        for (int i = 0; i < 5; i++) {
            try {
                Clipboard.SetDataObject(CurrentFileName, true);
                ShowTooltip("Copy file path success");
                return;
            } catch (ExternalException) {
                Win32Api.UnlockClipboard();
            }
        }

        MessageBox.Show("Failed to copy file path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void CommentUncommentAllBreakpoints() {
        Range range = richText.Selection.Clone();

        List<int> uncommentedBreakpoints = richText.FindLines(@"^\s*\*\*\*");
        if (uncommentedBreakpoints.Count > 0) {
            foreach (int line in uncommentedBreakpoints) {
                richText.Selection = new Range(richText, 0, line, 0, line);
                richText.InsertText("#");
            }
        } else {
            List<int> breakpoints = richText.FindLines(@"^\s*#+\s*\*\*\*");
            foreach (int line in breakpoints) {
                richText.Selection = new Range(richText, 0, line, 0, line);
                richText.RemoveLinePrefix("#");
            }
        }

        richText.Selection = range;
        richText.ScrollLeft();
    }

    private void InsertOrRemoveText(Regex regex, string insertText) {
        int currentLine = richText.Selection.Start.iLine;
        if (regex.IsMatch(richText.Lines[currentLine])) {
            richText.RemoveLine(currentLine);
            if (currentLine == richText.LinesCount) {
                currentLine--;
            }
        } else if (currentLine >= 1 && regex.IsMatch(richText.Lines[currentLine - 1])) {
            currentLine--;
            richText.RemoveLine(currentLine);
        } else {
            InsertNewLine(insertText);
            currentLine++;
        }

        string text = richText.Lines[currentLine];
        InputFrame input = new(text);
        int cursor = 4;
        if (input.Frames == 0 && input.Actions == Actions.None) {
            cursor = text.Length;
        }

        richText.Selection = new Range(richText, cursor, currentLine, cursor, currentLine);
    }

    private void InsertRoomName() => InsertNewLine($"#lvl_{CommunicationWrapper.GameInfo?.LevelName}");

    private void InsertTime() {
        InsertNewLine($"#{CommunicationWrapper.GameInfo?.CurrentTime}");
    }

    private void InsertNewLine(string text) {
        text = text.Trim();
        int startLine = richText.Selection.Start.iLine;
        richText.Selection = new Range(richText, 0, startLine, 0, startLine);
        richText.InsertText(text + "\n");
        richText.Selection = new Range(richText, text.Length, startLine, text.Length, startLine);
    }

    private void CopyGameInfo() {
        if (CommunicationWrapper.GameInfo is { } studioInfo) {
            Clipboard.SetText(studioInfo.GameInfo);
        }
    }

    private void UpdateLoop() {
        bool lastHooked = false;
        while (true) {
            try {
                bool hooked = CommunicationClient.Connected;
                if (lastHooked != hooked) {
                    lastHooked = hooked;
                    Invoke((Action) delegate { EnableStudio(hooked); });
                }

                if (lastChanged.AddSeconds(0.3f) < DateTime.Now) {
                    lastChanged = DateTime.Now;
                    Invoke((Action) delegate {
                        if (!string.IsNullOrEmpty(CurrentFileName) && richText.IsChanged) {
                            richText.SaveFile();
                        }
                    });
                }

                if (hooked) {
                    UpdateValues();
                    FixSomeBugsWhenOutOfMinimized();
                    CommunicationWrapper.CheckForward();
                } else {
                    richText.ReadOnly = DisableTyping;
                }

                Thread.Sleep(16);
            } catch {
                // ignore
            }
        }

        // ReSharper disable once FunctionNeverReturns
    }

    private void EnableStudio(bool hooked) {
        if (hooked) {
            try {
                if (string.IsNullOrEmpty(CurrentFileName)) {
                    newFileToolStripMenuItem_Click(null, null);
                }

                richText.Focus();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        } else {
            UpdateStatusBar();

            if (File.Exists(Settings.Instance.LastFileName)
                && IsFileReadable(Settings.Instance.LastFileName)
                && string.IsNullOrEmpty(CurrentFileName)) {
                CurrentFileName = Settings.Instance.LastFileName;
                richText.ReloadFile();
            }
        }
    }

    private void UpdateValues() {
        if (InvokeRequired) {
            Invoke((Action) UpdateValues);
        } else {
            if (CommunicationWrapper.GameInfo != null) {
                GameInfoMessage gameInfo = CommunicationWrapper.GameInfo.Value;
                richText.PlayingLine = gameInfo.CurrentLine;
                richText.CurrentLineSuffix = gameInfo.CurrentLineSuffix;
                currentFrame = gameInfo.CurrentFrameInTas;
                tasStates = gameInfo.States;
                if (tasStates.HasFlag(States.Enable) && !tasStates.HasFlag(States.FrameStep)) {
                    totalFrames = gameInfo.TotalFrames;
                }
            } else {
                currentFrame = 0;
                richText.PlayingLine = -1;
                richText.CurrentLineSuffix = string.Empty;
                richText.SaveStateLine = -1;
                tasStates = States.None;
            }

            richText.ReadOnly = DisableTyping;
            UpdateStatusBar();
        }
    }

    private void FixSomeBugsWhenOutOfMinimized() {
        if (lastWindowState == FormWindowState.Minimized && WindowState == FormWindowState.Normal) {
            richText.ScrollLeft();
        }

        lastWindowState = WindowState;
    }

    private void tasText_LineRemoved(object sender, LineRemovedEventArgs e) {
        if (updating) {
            return;
        }

        int count = e.Count;
        while (count-- > 0) {
            InputFrame input = InputRecords[e.Index];
            totalFrames -= input.Frames;
            InputRecords.RemoveAt(e.Index);
        }

        UpdateStatusBar();
    }

    private void tasText_LineInserted(object sender, LineInsertedEventArgs e) {
        if (updating) {
            return;
        }

        RichText.RichText tas = (RichText.RichText) sender;
        int count = e.Count;
        while (count-- > 0) {
            InputFrame input = new(tas.GetLineText(e.Index + count));
            InputRecords.Insert(e.Index, input);
            totalFrames += input.Frames;
        }

        UpdateStatusBar();
    }

    private void UpdateStatusBar() {
        if (CommunicationClient.Connected) {
            string gameInfo = CommunicationWrapper.GameInfo?.GameInfo ?? string.Empty;
            statusBarBuilder.Clear();
            if (currentFrame > 0) {
                statusBarBuilder.Append($"{currentFrame}/");
            }

            statusBarBuilder.Append($"{totalFrames}");

            int startLine = richText.Selection.Start.iLine;
            int endLine = richText.Selection.End.iLine;
            if (startLine != endLine) {
                if (endLine < startLine) {
                    int temp = startLine;
                    startLine = endLine;
                    endLine = temp;
                }

                int selectedFrames = 0;
                for (int i = startLine; i <= endLine && i < InputRecords.Count; i++) {
                    InputFrame input = InputRecords[i];
                    if (input.Frames > 0) {
                        selectedFrames += input.Frames;
                    }
                }

                if (selectedFrames > 0) {
                    statusBarBuilder.Append($" Selected: {selectedFrames}");
                }
            }

            statusBarBuilder.AppendLine();
            statusBarBuilder.Append(gameInfo);
            statusBarBuilder.Append(new string('\n', Math.Max(0, 7 - gameInfo.Split('\n').Length)));
            lblStatus.Text = statusBarBuilder.ToString();
        } else {
            lblStatus.Text = $"{totalFrames}\n{Settings.Instance.CommunicationHost}:{Settings.Instance.CommunicationPort} Connecting...";
        }

        int bottomExtraSpace = TextRenderer.MeasureText("\n", lblStatus.Font).Height / 5;
        if (Settings.Instance.ShowGameInfo) {
            int maxHeight = TextRenderer.MeasureText(MaxStatusHeight20Line, lblStatus.Font).Height + bottomExtraSpace;
            int statusBarHeight = TextRenderer.MeasureText(lblStatus.Text.Trim(), lblStatus.Font).Height + bottomExtraSpace;
            statusPanel.Height = Math.Min(maxHeight, statusBarHeight);
            statusPanel.AutoScrollMinSize = new Size(0, statusBarHeight);
            statusPanel.AutoScroll = statusBarHeight > maxHeight;
            statusBar.Height = statusBarHeight;
        } else {
            statusPanel.Height = 0;
        }

        richText.Height = ClientSize.Height - statusPanel.Height - menuStrip.Height;
    }

    private void tasText_TextChanged(object sender, TextChangedEventArgs e) {
        lastChanged = DateTime.Now;
        UpdateLines((RichText.RichText) sender, e.ChangedRange);
    }

    private void CommentText(bool toggle) {
        Range origRange = richText.Selection.Clone();

        origRange.Normalize();
        int start = origRange.Start.iLine;
        int end = origRange.End.iLine;

        List<InputFrame> selection = InputRecords.GetRange(start, end - start + 1);

        bool anyUncomment = selection.Any(record => !record.IsEmpty && !record.IsComment);

        StringBuilder result = new();
        foreach (InputFrame record in selection) {
            if (record.IsCommentRoom || record.IsCommentTime) {
                result.AppendLine(record.ToString());
            } else if (!toggle && anyUncomment || toggle && !record.IsComment) {
                if (!record.IsEmpty) {
                    result.Append("#");
                }

                result.AppendLine(record.ToString());
            } else {
                result.AppendLine(InputFrame.CommentSymbolRegex.Replace(record.ToString(), string.Empty));
            }
        }

        // remove last line break
        result.Length -= Environment.NewLine.Length;

        richText.Selection = new Range(richText, 0, start, richText[end].Count, end);
        richText.SelectedText = result.ToString();

        if (origRange.IsEmpty) {
            if (start < richText.LinesCount - 1) {
                start++;
            }

            richText.Selection = new Range(richText, 0, start, 0, start);
        } else {
            richText.Selection = new Range(richText, 0, start, richText[end].Count, end);
        }

        richText.ScrollLeft();
    }

    private void CombineInputs(bool sameActions) {
        Range origRange = richText.Selection.Clone();

        origRange.Normalize();
        int start = origRange.Start.iLine;
        int end = origRange.End.iLine;

        if (start == end) {
            if (!sameActions) {
                return;
            }

            InputFrame currentFrame = InputRecords[start];
            if (!currentFrame.IsInput) {
                return;
            }

            while (start > 1) {
                InputFrame prev = InputRecords[start - 1];
                if ((prev.IsInput || prev.IsEmpty) && prev.ToActionsString() == currentFrame.ToActionsString()) {
                    start--;
                } else {
                    break;
                }
            }

            while (end < InputRecords.Count - 1) {
                InputFrame next = InputRecords[end + 1];
                if ((next.IsInput || next.IsEmpty) && next.Actions == currentFrame.Actions) {
                    end++;
                } else {
                    break;
                }
            }
        } else if (!sameActions) {
            // skip non input line
            while (start < end) {
                InputFrame current = InputRecords[start];
                if (!current.IsInput) {
                    start++;
                } else {
                    break;
                }
            }
        }

        if (start == end) {
            return;
        }

        List<InputFrame> selection = InputRecords.GetRange(start, end - start + 1);
        SortedDictionary<int, List<InputFrame>> groups = new();

        if (sameActions) {
            int index = start;
            InputFrame current = InputRecords[index];
            int currentIndex = index;
            groups[currentIndex] = new List<InputFrame> {current};
            while (++index <= end) {
                InputFrame next = InputRecords[index];

                // ignore empty line if combine succeeds
                int? nextIndex = null;
                if (next.IsEmptyOrZeroFrameInput && next.Next(record => !record.IsEmptyOrZeroFrameInput) is {IsInput: true} nextInput) {
                    nextIndex = InputRecords.IndexOf(nextInput);
                    if (nextIndex <= end) {
                        next = nextInput;
                    } else {
                        nextIndex = null;
                    }
                }

                if (current.IsInput && next.IsInput && current.ToActionsString() == next.ToActionsString() && !next.IsScreenTransition()) {
                    groups[currentIndex].Add(next);
                    if (nextIndex.HasValue) {
                        index = nextIndex.Value;
                    }
                } else {
                    current = InputRecords[index];
                    currentIndex = index;
                    groups[currentIndex] = new List<InputFrame> {current};
                }
            }
        } else {
            selection = selection.Where(record => !record.IsEmptyOrZeroFrameInput).ToList();
            selection.Sort((a, b) => !a.IsInput && b.IsInput ? 1 : 0);
            for (int i = 0; i < selection.Count; i++) {
                InputFrame inputFrame = selection[i];
                int groupIndex = inputFrame.IsInput ? 0 : i;
                if (!groups.ContainsKey(groupIndex)) {
                    groups[groupIndex] = new List<InputFrame>();
                }

                groups[groupIndex].Add(inputFrame);
            }
        }

        StringBuilder result = new();
        foreach (List<InputFrame> groupInputs in groups.Values) {
            if (groupInputs.Count > 1 && groupInputs.First().IsInput) {
                int combinedFrames = groupInputs.Sum(record => record.Frames);
                if (combinedFrames < 10000) {
                    result.AppendLine(new InputFrame(combinedFrames, groupInputs.First().ToActionsString()).ToString());
                } else {
                    ShowTooltip("Combine failed because the combined frames were greater than 9999");
                    if (sameActions) {
                        foreach (InputFrame inputRecord in groupInputs) {
                            result.AppendLine(inputRecord.ToString());
                        }
                    } else {
                        return;
                    }
                }
            } else {
                foreach (InputFrame inputRecord in groupInputs) {
                    result.AppendLine(inputRecord.ToString());
                }
            }
        }

        // remove last line break
        result.Length -= Environment.NewLine.Length;

        richText.Selection = new Range(richText, 0, start, richText[end].Count, end);
        richText.SelectedText = result.ToString();
        richText.ScrollLeft();
    }

    private void UpdateLines(RichText.RichText tas, Range range) {
        if (updating) {
            return;
        }

        updating = true;

        int start = range.Start.iLine;
        int end = range.End.iLine;
        if (start > end) {
            int temp = start;
            start = end;
            end = temp;
        }

        int originalStart = start;

        bool modified = false;
        StringBuilder sb = new();
        Place place = new(0, end);
        while (start <= end) {
            InputFrame oldInput = InputRecords.Count > start ? InputRecords[start] : null;
            string text = tas[start].Text;
            InputFrame newInput = new(text);
            if (oldInput != null) {
                totalFrames -= oldInput.Frames;

                string formattedText = newInput.ToString();

                if (text != formattedText) {
                    Range oldRange = tas.Selection;
                    if (!string.IsNullOrEmpty(formattedText)) {
                        InputFrame.ProcessExclusiveActions(oldInput, newInput);
                        formattedText = newInput.ToString();

                        int index = oldRange.Start.iChar + formattedText.Length - text.Length;
                        if (index < 0) {
                            index = 0;
                        }

                        if (index > 4) {
                            index = 4;
                        }

                        if (oldInput.Frames == newInput.Frames) {
                            index = 4;
                        }

                        place = new Place(index, start);
                    }

                    modified = true;
                } else {
                    place = new Place(4, start);
                }

                text = formattedText;
                InputRecords[start] = newInput;
            } else {
                place = new Place(text.Length, start);
            }

            if (start < end) {
                sb.AppendLine(text);
            } else {
                sb.Append(text);
            }

            totalFrames += newInput.Frames;

            start++;
        }

        if (modified) {
            tas.Selection = new Range(tas, 0, originalStart, tas[end].Count, end);
            tas.SelectedText = sb.ToString();
            tas.Selection = new Range(tas, place.iChar, end, place.iChar, end);
        }

        if (tas.IsChanged) {
            Text = TitleBarText + " ***";
        }

        UpdateStatusBar();

        updating = false;
    }

    private void tasText_NoChanges(object sender, EventArgs e) {
        Text = TitleBarText;
    }

    private void tasText_FileOpening(object sender, EventArgs e) {
        InputRecords.Clear();
        totalFrames = 0;
        UpdateStatusBar();
    }

    private void tasText_LineNeeded(object sender, LineNeededEventArgs e) {
        InputFrame frame = new(e.SourceLineText);
        e.DisplayedLineText = frame.ToString();
    }

    private bool IsFileReadable(string fileName) {
        try {
            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                stream.Close();
            }
        } catch (IOException) {
            return false;
        }

        //file is not locked
        return true;
    }

    private void autoRemoveExclusiveActionsToolStripMenuItem_Click(object sender, EventArgs e) {
        Settings.Instance.AutoRemoveMutuallyExclusiveActions = !Settings.Instance.AutoRemoveMutuallyExclusiveActions;
    }

    private void homeMenuItem_Click(object sender, EventArgs e) {
        Process.Start("https://github.com/DemoJameson/SmolAme.YetAnotherTAS");
    }

    private void settingsToolStripMenuItem_Opened(object sender, EventArgs e) {
        settingsToolStripMenuItem.DropDown.Opacity = 1f;
        sendInputsToCelesteMenuItem.Checked = Settings.Instance.SendHotkeysToGame;
        autoRemoveExclusiveActionsToolStripMenuItem.Checked = Settings.Instance.AutoRemoveMutuallyExclusiveActions;
        showGameInfoToolStripMenuItem.Checked = Settings.Instance.ShowGameInfo;
        enabledAutoBackupToolStripMenuItem.Checked = Settings.Instance.AutoBackupEnabled;
        backupRateToolStripMenuItem.Text = $"Backup Rate (minutes): {Settings.Instance.AutoBackupRate}";
        backupFileCountsToolStripMenuItem.Text = $"Backup File Count: {Settings.Instance.AutoBackupCount}";
        hostToolStripMenuItem.Text = $"Host: {Settings.Instance.CommunicationHost}";
        portToolStripMenuItem.Text = $"Port: {Settings.Instance.CommunicationPort}";
    }

    private void openPreviousFileToolStripMenuItem_Click(object sender, EventArgs e) {
        if (RecentFiles.Count <= 1) {
            return;
        }

        string fileName = RecentFiles[1];

        if (!File.Exists(fileName)) {
            RecentFiles.Remove(fileName);
        }

        int startLine = 0;
        if (previousFile != null && previousFile.Item1 == fileName) {
            startLine = previousFile.Item2;
        }

        OpenFile(fileName, startLine);
    }

    private void sendInputsToCelesteMenuItem_Click(object sender, EventArgs e) {
        Settings.Instance.SendHotkeysToGame = !Settings.Instance.SendHotkeysToGame;
        if (settingsToolStripMenuItem.DropDown.Opacity == 0f) {
            ShowTooltip((Settings.Instance.SendHotkeysToGame ? "Enable" : "Disable") + " Send Hotkeys to Celeste");
        }

        settingsToolStripMenuItem.DropDown.Opacity = 0f;
    }

    private void openFileMenuItem_Click(object sender, EventArgs e) {
        OpenFile();
    }

    private void fileToolStripMenuItem_DropDownOpened(object sender, EventArgs e) {
        CreateRecentFilesMenu();
        CreateBackupFilesMenu();
        openPreviousFileToolStripMenuItem.Enabled = RecentFiles.Count >= 2;
    }

    private void insertRemoveBreakPointToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertOrRemoveText(InputFrame.BreakpointRegex, "***");
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
        SaveAsFile();
    }

    private void commentUncommentTextToolStripMenuItem_Click(object sender, EventArgs e) {
        CommentText(true);
    }

    private void removeAllUncommentedBreakpointsToolStripMenuItem_Click(object sender, EventArgs e) {
        ClearUncommentedBreakpoints();
    }

    private void removeAllBreakpointsToolStripMenuItem_Click(object sender, EventArgs e) {
        ClearBreakpoints();
    }

    private void commentUncommentAllBreakpointsToolStripMenuItem_Click(object sender, EventArgs e) {
        CommentUncommentAllBreakpoints();
    }

    private void insertRoomNameToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertRoomName();
    }

    private void insertCurrentInGameTimeToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertTime();
    }

    private void enforceLegalToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertNewLine("EnforceLegal");
    }

    private void readToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertNewLine("Read, File Name, Starting Line, (Ending Line)");
    }

    private void playToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertNewLine("Play, Starting Line");
    }

    private void recordCountToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertNewLine("RecordCount: 1");
    }

    private void repeatToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertNewLine("Repeat Count");
    }

    private void endRepeatToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertNewLine("EndRepeat");
    }

    private void timeToolStripMenuItem_Click(object sender, EventArgs e) {
        InsertNewLine("Time:"); 
    }

    private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
        if (CommunicationWrapper.GameInfo is { } studioInfo) {
            InsertNewLine($"Load {studioInfo.LevelName}");  
        } else {
            InsertNewLine("Load 3");  
        }
    }

    private void copyGamerDataMenuItem_Click(object sender, EventArgs e) {
        CopyGameInfo();
    }

    private void fontToolStripMenuItem_Click(object sender, EventArgs e) {
        if (fontDialog.ShowDialog() != DialogResult.Cancel) {
            //check monospace font
            SizeF sizeM = RichText.RichText.GetCharSize(fontDialog.Font, 'M');
            SizeF sizeDot = RichText.RichText.GetCharSize(fontDialog.Font, '.');
            if (sizeM == sizeDot) {
                InitFont(fontDialog.Font);
                Settings.Instance.Font = fontDialog.Font;
            } else {
                ShowTooltip("Only monospaced font is allowed");
            }
        }
    }

    private void reconnectStudioAndCelesteToolStripMenuItem_Click(object sender, EventArgs e) {
        CommunicationClient.Connect();
    }

    private void combineConsecutiveSameInputsToolStripMenuItem_Click(object sender, EventArgs e) {
        CombineInputs(true);
    }

    private void forceCombineInputsToolStripMenuItem_Click(object sender, EventArgs e) {
        CombineInputs(false);
    }

    private void openReadFileToolStripMenuItem_Click(object sender, EventArgs e) {
        TryOpenReadFile();
        TryGoToPlayLine();
    }

    private void showGameInfoToolStripMenuItem_Click(object sender, EventArgs e) {
        Settings.Instance.ShowGameInfo = !Settings.Instance.ShowGameInfo;
        SaveSettings();
    }

    private void newFileToolStripMenuItem_Click(object sender, EventArgs e) {
        int index = 1;
        string gamePath = Path.Combine(Directory.GetCurrentDirectory(), "TAS Files");
        if (!Directory.Exists(gamePath)) {
            Directory.CreateDirectory(gamePath);
        }

        string initText = $"RecordCount: 1{Environment.NewLine}";
        initText += $"{Environment.NewLine}#Start{Environment.NewLine}";

        string fileName = Path.Combine(gamePath, $"Untitled-{index}.tas");
        while (File.Exists(fileName) && File.ReadAllText(fileName) != initText) {
            index++;
            fileName = Path.Combine(gamePath, $"Untitled-{index}.tas");
        }

        File.WriteAllText(fileName, initText);

        OpenFile(fileName);
    }

    private void enabledAutoBackupToolStripMenuItem_Click(object sender, EventArgs e) {
        Settings.Instance.AutoBackupEnabled = !Settings.Instance.AutoBackupEnabled;
        SaveSettings();
    }

    private void backupRateToolStripMenuItem_Click(object sender, EventArgs e) {
        string origRate = Settings.Instance.AutoBackupRate.ToString();
        if (!DialogUtils.ShowInputDialog("Backup Rate (minutes)", ref origRate)) {
            return;
        }

        if (string.IsNullOrEmpty(origRate)) {
            Settings.Instance.AutoBackupRate = 0;
        } else if (int.TryParse(origRate, out int count)) {
            Settings.Instance.AutoBackupRate = Math.Max(0, count);
        }

        backupRateToolStripMenuItem.Text = $"Backup Rate (minutes): {Settings.Instance.AutoBackupRate}";
    }

    private void backupFileCountsToolStripMenuItem_Click(object sender, EventArgs e) {
        string origCount = Settings.Instance.AutoBackupCount.ToString();
        if (!DialogUtils.ShowInputDialog("Backup File Count", ref origCount)) {
            return;
        }

        if (string.IsNullOrEmpty(origCount)) {
            Settings.Instance.AutoBackupCount = 0;
        } else if (int.TryParse(origCount, out int count)) {
            Settings.Instance.AutoBackupCount = Math.Max(0, count);
        }

        backupFileCountsToolStripMenuItem.Text = $"Backup File Count: {Settings.Instance.AutoBackupCount}";
    }

    private void hostToolStripMenuItem_Click(object sender, EventArgs e) {
        string host = Settings.Instance.CommunicationHost;
        if (!DialogUtils.ShowInputDialog("Backup Rate (minutes)", ref host)) {
            return;
        }

        if (string.IsNullOrEmpty(host)) {
            Settings.Instance.CommunicationHost = "127.0.0.1";
        } else {
            if (Settings.Instance.CommunicationHost != host) {
                Settings.Instance.CommunicationHost = host;
                CommunicationClient.Connect();
            }
        }

        hostToolStripMenuItem.Text = $"Host: {Settings.Instance.CommunicationHost}";
    }

    private void portToolStripMenuItem_Click(object sender, EventArgs e) {
        string portStr = Settings.Instance.CommunicationPort.ToString();
        if (!DialogUtils.ShowInputDialog("Communication Port", ref portStr)) {
            return;
        }

        if (string.IsNullOrEmpty(portStr)) {
            Settings.Instance.CommunicationPort = 19982;
        } else if (int.TryParse(portStr, out int port)) {
            if (port < 1) {
                port = 1;
            } else if (port > 65535) {
                port = 65535;
            }

            if (Settings.Instance.CommunicationPort != port) {
                Settings.Instance.CommunicationPort = port;
                CommunicationClient.Connect();
            }
        }

        portToolStripMenuItem.Text = $"Port: {Settings.Instance.CommunicationPort}";
    }

    public void SetControlsColor(Themes themes) {
        Color foreColor = ColorUtils.HexToColor(themes.Status);
        Color backColor = ColorUtils.HexToColor(themes.Status, 1);
        Color dividerColor = ColorUtils.HexToColor(themes.ServiceLine);

        BackColor = ColorUtils.HexToColor(themes.Background);

        lblStatus.ForeColor = foreColor;
        lblStatus.BackColor = backColor;

        statusPanel.Controls[0].ForeColor = foreColor;
        statusPanel.Controls[0].BackColor = backColor;

        menuStrip.ForeColor = foreColor;
        menuStrip.BackColor = backColor;
        menuStrip.Renderer = new ThemesRenderer(themes);

        dividerLabel.BackColor = dividerColor;
    }

    private void lightToolStripMenuItem_Click(object sender, EventArgs e) {
        Settings.Instance.ThemesType = ThemesType.Light;
        Themes.ResetThemes();
        SaveSettings();
    }

    private void darkToolStripMenuItem_Click(object sender, EventArgs e) {
        Settings.Instance.ThemesType = ThemesType.Dark;
        Themes.ResetThemes();
        SaveSettings();
    }

    private void customToolStripMenuItem_Click(object sender, EventArgs e) {
        Settings.Instance.ThemesType = ThemesType.Custom;
        Themes.ResetThemes();
        SaveSettings();
    }

    private void themesToolStripMenuItem_DropDownOpened(object sender, EventArgs e) {
        lightToolStripMenuItem.Checked = Settings.Instance.ThemesType == ThemesType.Light;
        darkToolStripMenuItem.Checked = Settings.Instance.ThemesType == ThemesType.Dark;
        customToolStripMenuItem.Checked = Settings.Instance.ThemesType == ThemesType.Custom;
    }
}