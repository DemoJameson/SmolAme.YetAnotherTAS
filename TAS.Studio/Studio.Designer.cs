using TAS.Studio.RichText;

namespace TAS.Studio {
	partial class Studio {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Studio));
            this.hotkeyToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusBarContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyGameDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reconnectStudioAndCelesteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.openFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPreviousFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRecentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendInputsToCelesteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoRemoveExclusiveActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGameInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enabledAutoBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupFileCountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.themesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.communicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dividerLabel = new System.Windows.Forms.Label();
            this.tasTextContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertRemoveBreakPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllUncommentedBreakpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllBreakpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentUncommentAllBreakpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.commentUncommentTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertRoomNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertCurrentInGameTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertOtherCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enforceLegalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.readToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.repeatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endRepeatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.recordCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.combineConsecutiveSameInputsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceCombineInputsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.openReadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richText = new TAS.Studio.RichText.RichText();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarContextMenuStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.tasTextContextMenuStrip.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // hotkeyToolTip
            // 
            this.hotkeyToolTip.AutomaticDelay = 200;
            this.hotkeyToolTip.AutoPopDelay = 5000;
            this.hotkeyToolTip.InitialDelay = 200;
            this.hotkeyToolTip.IsBalloon = true;
            this.hotkeyToolTip.ReshowDelay = 200;
            this.hotkeyToolTip.ShowAlways = true;
            this.hotkeyToolTip.ToolTipTitle = "Fact: Birds are hard to catch";
            // 
            // statusBarContextMenuStrip
            // 
            this.statusBarContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBarContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyGameDataMenuItem,
            this.reconnectStudioAndCelesteToolStripMenuItem});
            this.statusBarContextMenuStrip.Name = "statusBarMenuStrip";
            this.statusBarContextMenuStrip.Size = new System.Drawing.Size(329, 48);
            // 
            // copyGameDataMenuItem
            // 
            this.copyGameDataMenuItem.Name = "copyGameDataMenuItem";
            this.copyGameDataMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.copyGameDataMenuItem.Size = new System.Drawing.Size(328, 22);
            this.copyGameDataMenuItem.Text = "Copy Game Info to Clipboard";
            this.copyGameDataMenuItem.Click += new System.EventHandler(this.copyGamerDataMenuItem_Click);
            // 
            // reconnectStudioAndCelesteToolStripMenuItem
            // 
            this.reconnectStudioAndCelesteToolStripMenuItem.Name = "reconnectStudioAndCelesteToolStripMenuItem";
            this.reconnectStudioAndCelesteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
            this.reconnectStudioAndCelesteToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
            this.reconnectStudioAndCelesteToolStripMenuItem.Text = "Reconnect Studio and Game";
            this.reconnectStudioAndCelesteToolStripMenuItem.Click += new System.EventHandler(this.reconnectStudioAndCelesteToolStripMenuItem_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(286, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileToolStripMenuItem,
            this.toolStripSeparator7,
            this.openFileMenuItem,
            this.openPreviousFileToolStripMenuItem,
            this.openRecentMenuItem,
            this.openBackupToolStripMenuItem,
            this.toolStripSeparator15,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpened += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpened);
            // 
            // newFileToolStripMenuItem
            // 
            this.newFileToolStripMenuItem.Name = "newFileToolStripMenuItem";
            this.newFileToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.newFileToolStripMenuItem.Text = "&New File";
            this.newFileToolStripMenuItem.Click += new System.EventHandler(this.newFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(286, 6);
            // 
            // openFileMenuItem
            // 
            this.openFileMenuItem.Name = "openFileMenuItem";
            this.openFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openFileMenuItem.Size = new System.Drawing.Size(289, 22);
            this.openFileMenuItem.Text = "&Open File...";
            this.openFileMenuItem.Click += new System.EventHandler(this.openFileMenuItem_Click);
            // 
            // openPreviousFileToolStripMenuItem
            // 
            this.openPreviousFileToolStripMenuItem.Name = "openPreviousFileToolStripMenuItem";
            this.openPreviousFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.openPreviousFileToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.openPreviousFileToolStripMenuItem.Text = "Open &Previous File";
            this.openPreviousFileToolStripMenuItem.Click += new System.EventHandler(this.openPreviousFileToolStripMenuItem_Click);
            // 
            // openRecentMenuItem
            // 
            this.openRecentMenuItem.Name = "openRecentMenuItem";
            this.openRecentMenuItem.Size = new System.Drawing.Size(289, 22);
            this.openRecentMenuItem.Text = "Open &Recent";
            // 
            // openBackupToolStripMenuItem
            // 
            this.openBackupToolStripMenuItem.Name = "openBackupToolStripMenuItem";
            this.openBackupToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.openBackupToolStripMenuItem.Text = "Open &Backup";
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(286, 6);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.saveAsToolStripMenuItem.Text = "&Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendInputsToCelesteMenuItem,
            this.autoRemoveExclusiveActionsToolStripMenuItem,
            this.showGameInfoToolStripMenuItem,
            this.autoBackupToolStripMenuItem,
            this.fontToolStripMenuItem,
            this.themesToolStripMenuItem,
            this.communicationToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.DropDownOpened += new System.EventHandler(this.settingsToolStripMenuItem_Opened);
            // 
            // sendInputsToCelesteMenuItem
            // 
            this.sendInputsToCelesteMenuItem.Name = "sendInputsToCelesteMenuItem";
            this.sendInputsToCelesteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.sendInputsToCelesteMenuItem.Size = new System.Drawing.Size(378, 22);
            this.sendInputsToCelesteMenuItem.Text = "&Send Hotkeys to Game";
            this.sendInputsToCelesteMenuItem.Click += new System.EventHandler(this.sendInputsToCelesteMenuItem_Click);
            // 
            // autoRemoveExclusiveActionsToolStripMenuItem
            // 
            this.autoRemoveExclusiveActionsToolStripMenuItem.Name = "autoRemoveExclusiveActionsToolStripMenuItem";
            this.autoRemoveExclusiveActionsToolStripMenuItem.Size = new System.Drawing.Size(378, 22);
            this.autoRemoveExclusiveActionsToolStripMenuItem.Text = "Auto Remove Mutually Exclusive Actions";
            this.autoRemoveExclusiveActionsToolStripMenuItem.Click += new System.EventHandler(this.autoRemoveExclusiveActionsToolStripMenuItem_Click);
            // 
            // showGameInfoToolStripMenuItem
            // 
            this.showGameInfoToolStripMenuItem.Name = "showGameInfoToolStripMenuItem";
            this.showGameInfoToolStripMenuItem.Size = new System.Drawing.Size(378, 22);
            this.showGameInfoToolStripMenuItem.Text = "Show Game Info";
            this.showGameInfoToolStripMenuItem.Click += new System.EventHandler(this.showGameInfoToolStripMenuItem_Click);
            // 
            // autoBackupToolStripMenuItem
            // 
            this.autoBackupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enabledAutoBackupToolStripMenuItem,
            this.backupRateToolStripMenuItem,
            this.backupFileCountsToolStripMenuItem});
            this.autoBackupToolStripMenuItem.Name = "autoBackupToolStripMenuItem";
            this.autoBackupToolStripMenuItem.Size = new System.Drawing.Size(378, 22);
            this.autoBackupToolStripMenuItem.Text = "Automatic Backup";
            // 
            // enabledAutoBackupToolStripMenuItem
            // 
            this.enabledAutoBackupToolStripMenuItem.Checked = true;
            this.enabledAutoBackupToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enabledAutoBackupToolStripMenuItem.Name = "enabledAutoBackupToolStripMenuItem";
            this.enabledAutoBackupToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.enabledAutoBackupToolStripMenuItem.Text = "Enabled";
            this.enabledAutoBackupToolStripMenuItem.Click += new System.EventHandler(this.enabledAutoBackupToolStripMenuItem_Click);
            // 
            // backupRateToolStripMenuItem
            // 
            this.backupRateToolStripMenuItem.Name = "backupRateToolStripMenuItem";
            this.backupRateToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.backupRateToolStripMenuItem.Text = "Backup Rate (minutes)";
            this.backupRateToolStripMenuItem.ToolTipText = "0 means the file will be backed up every time it is modified";
            this.backupRateToolStripMenuItem.Click += new System.EventHandler(this.backupRateToolStripMenuItem_Click);
            // 
            // backupFileCountsToolStripMenuItem
            // 
            this.backupFileCountsToolStripMenuItem.Name = "backupFileCountsToolStripMenuItem";
            this.backupFileCountsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.backupFileCountsToolStripMenuItem.Text = "Backup File Count";
            this.backupFileCountsToolStripMenuItem.ToolTipText = "0 means no limit";
            this.backupFileCountsToolStripMenuItem.Click += new System.EventHandler(this.backupFileCountsToolStripMenuItem_Click);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(378, 22);
            this.fontToolStripMenuItem.Text = "Font...";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // themesToolStripMenuItem
            // 
            this.themesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lightToolStripMenuItem,
            this.darkToolStripMenuItem,
            this.customToolStripMenuItem});
            this.themesToolStripMenuItem.Name = "themesToolStripMenuItem";
            this.themesToolStripMenuItem.Size = new System.Drawing.Size(378, 22);
            this.themesToolStripMenuItem.Text = "Themes";
            this.themesToolStripMenuItem.DropDownOpened += new System.EventHandler(this.themesToolStripMenuItem_DropDownOpened);
            // 
            // lightToolStripMenuItem
            // 
            this.lightToolStripMenuItem.Name = "lightToolStripMenuItem";
            this.lightToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.lightToolStripMenuItem.Text = "Light";
            this.lightToolStripMenuItem.Click += new System.EventHandler(this.lightToolStripMenuItem_Click);
            // 
            // darkToolStripMenuItem
            // 
            this.darkToolStripMenuItem.Name = "darkToolStripMenuItem";
            this.darkToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.darkToolStripMenuItem.Text = "Dark";
            this.darkToolStripMenuItem.Click += new System.EventHandler(this.darkToolStripMenuItem_Click);
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.customToolStripMenuItem.Text = "Custom";
            this.customToolStripMenuItem.Click += new System.EventHandler(this.customToolStripMenuItem_Click);
            // 
            // communicationToolStripMenuItem
            // 
            this.communicationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hostToolStripMenuItem,
            this.portToolStripMenuItem});
            this.communicationToolStripMenuItem.Name = "communicationToolStripMenuItem";
            this.communicationToolStripMenuItem.Size = new System.Drawing.Size(378, 22);
            this.communicationToolStripMenuItem.Text = "Communication";
            // 
            // hostToolStripMenuItem
            // 
            this.hostToolStripMenuItem.Name = "hostToolStripMenuItem";
            this.hostToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.hostToolStripMenuItem.Text = "Host";
            this.hostToolStripMenuItem.Click += new System.EventHandler(this.hostToolStripMenuItem_Click);
            // 
            // portToolStripMenuItem
            // 
            this.portToolStripMenuItem.Name = "portToolStripMenuItem";
            this.portToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.portToolStripMenuItem.Text = "Port";
            this.portToolStripMenuItem.Click += new System.EventHandler(this.portToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.homeMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // homeMenuItem
            // 
            this.homeMenuItem.Name = "homeMenuItem";
            this.homeMenuItem.Size = new System.Drawing.Size(106, 22);
            this.homeMenuItem.Text = "&Home";
            this.homeMenuItem.Click += new System.EventHandler(this.homeMenuItem_Click);
            // 
            // dividerLabel
            // 
            this.dividerLabel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.dividerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.dividerLabel.Location = new System.Drawing.Point(0, 24);
            this.dividerLabel.Name = "dividerLabel";
            this.dividerLabel.Size = new System.Drawing.Size(286, 1);
            this.dividerLabel.TabIndex = 4;
            // 
            // tasTextContextMenuStrip
            // 
            this.tasTextContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tasTextContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertRemoveBreakPointToolStripMenuItem,
            this.removeAllUncommentedBreakpointsToolStripMenuItem,
            this.removeAllBreakpointsToolStripMenuItem,
            this.commentUncommentAllBreakpointsToolStripMenuItem,
            this.toolStripSeparator2,
            this.commentUncommentTextToolStripMenuItem,
            this.insertRoomNameToolStripMenuItem,
            this.insertCurrentInGameTimeToolStripMenuItem,
            this.insertOtherCommandToolStripMenuItem,
            this.toolStripSeparator6,
            this.combineConsecutiveSameInputsToolStripMenuItem,
            this.forceCombineInputsToolStripMenuItem,
            this.toolStripSeparator12,
            this.openReadFileToolStripMenuItem});
            this.tasTextContextMenuStrip.Name = "tasTextContextMenuStrip";
            this.tasTextContextMenuStrip.Size = new System.Drawing.Size(368, 264);
            // 
            // insertRemoveBreakPointToolStripMenuItem
            // 
            this.insertRemoveBreakPointToolStripMenuItem.Name = "insertRemoveBreakPointToolStripMenuItem";
            this.insertRemoveBreakPointToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemPeriod)));
            this.insertRemoveBreakPointToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.insertRemoveBreakPointToolStripMenuItem.Text = "Insert/Remove Breakpoint";
            this.insertRemoveBreakPointToolStripMenuItem.Click += new System.EventHandler(this.insertRemoveBreakPointToolStripMenuItem_Click);
            // 
            // removeAllUncommentedBreakpointsToolStripMenuItem
            // 
            this.removeAllUncommentedBreakpointsToolStripMenuItem.Name = "removeAllUncommentedBreakpointsToolStripMenuItem";
            this.removeAllUncommentedBreakpointsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.removeAllUncommentedBreakpointsToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.removeAllUncommentedBreakpointsToolStripMenuItem.Text = "Remove All Uncommented Breakpoints";
            this.removeAllUncommentedBreakpointsToolStripMenuItem.Click += new System.EventHandler(this.removeAllUncommentedBreakpointsToolStripMenuItem_Click);
            // 
            // removeAllBreakpointsToolStripMenuItem
            // 
            this.removeAllBreakpointsToolStripMenuItem.Name = "removeAllBreakpointsToolStripMenuItem";
            this.removeAllBreakpointsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
            this.removeAllBreakpointsToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.removeAllBreakpointsToolStripMenuItem.Text = "Remove All Breakpoints";
            this.removeAllBreakpointsToolStripMenuItem.Click += new System.EventHandler(this.removeAllBreakpointsToolStripMenuItem_Click);
            // 
            // commentUncommentAllBreakpointsToolStripMenuItem
            // 
            this.commentUncommentAllBreakpointsToolStripMenuItem.Name = "commentUncommentAllBreakpointsToolStripMenuItem";
            this.commentUncommentAllBreakpointsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.P)));
            this.commentUncommentAllBreakpointsToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.commentUncommentAllBreakpointsToolStripMenuItem.Text = "Comment/Uncomment All Breakpoints";
            this.commentUncommentAllBreakpointsToolStripMenuItem.Click += new System.EventHandler(this.commentUncommentAllBreakpointsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(364, 6);
            // 
            // commentUncommentTextToolStripMenuItem
            // 
            this.commentUncommentTextToolStripMenuItem.Name = "commentUncommentTextToolStripMenuItem";
            this.commentUncommentTextToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.commentUncommentTextToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.commentUncommentTextToolStripMenuItem.Text = "Comment/Uncomment Text";
            this.commentUncommentTextToolStripMenuItem.Click += new System.EventHandler(this.commentUncommentTextToolStripMenuItem_Click);
            // 
            // insertRoomNameToolStripMenuItem
            // 
            this.insertRoomNameToolStripMenuItem.Name = "insertRoomNameToolStripMenuItem";
            this.insertRoomNameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.insertRoomNameToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.insertRoomNameToolStripMenuItem.Text = "Insert Room Name";
            this.insertRoomNameToolStripMenuItem.Click += new System.EventHandler(this.insertRoomNameToolStripMenuItem_Click);
            // 
            // insertCurrentInGameTimeToolStripMenuItem
            // 
            this.insertCurrentInGameTimeToolStripMenuItem.Name = "insertCurrentInGameTimeToolStripMenuItem";
            this.insertCurrentInGameTimeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.insertCurrentInGameTimeToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.insertCurrentInGameTimeToolStripMenuItem.Text = "Insert Current In-Game Time";
            this.insertCurrentInGameTimeToolStripMenuItem.Click += new System.EventHandler(this.insertCurrentInGameTimeToolStripMenuItem_Click);
            // 
            // insertOtherCommandToolStripMenuItem
            // 
            this.insertOtherCommandToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enforceLegalToolStripMenuItem,
            this.toolStripSeparator18,
            this.readToolStripMenuItem,
            this.playToolStripMenuItem,
            this.toolStripSeparator17,
            this.repeatToolStripMenuItem,
            this.endRepeatToolStripMenuItem,
            this.toolStripSeparator16,
            this.recordCountToolStripMenuItem,
            this.timeToolStripMenuItem,
            this.toolStripSeparator21,
            this.loadToolStripMenuItem});
            this.insertOtherCommandToolStripMenuItem.Name = "insertOtherCommandToolStripMenuItem";
            this.insertOtherCommandToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.insertOtherCommandToolStripMenuItem.Text = "Insert Other Command";
            // 
            // enforceLegalToolStripMenuItem
            // 
            this.enforceLegalToolStripMenuItem.Name = "enforceLegalToolStripMenuItem";
            this.enforceLegalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.enforceLegalToolStripMenuItem.Text = "EnforceLegal";
            this.enforceLegalToolStripMenuItem.ToolTipText = "This is used at the start of fullgame files.\r\nIt prevents the use of commands whi" +
    "ch would not be legal in a run.";
            this.enforceLegalToolStripMenuItem.Click += new System.EventHandler(this.enforceLegalToolStripMenuItem_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(177, 6);
            // 
            // readToolStripMenuItem
            // 
            this.readToolStripMenuItem.Name = "readToolStripMenuItem";
            this.readToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.readToolStripMenuItem.Text = "Read";
            this.readToolStripMenuItem.ToolTipText = "Will read inputs from the specified file.";
            this.readToolStripMenuItem.Click += new System.EventHandler(this.readToolStripMenuItem_Click);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.ToolTipText = "A simplified Read command which skips to the starting line in the current file.\r\n" +
    "Useful for splitting a large level into larger chunks.";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(177, 6);
            // 
            // repeatToolStripMenuItem
            // 
            this.repeatToolStripMenuItem.Name = "repeatToolStripMenuItem";
            this.repeatToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.repeatToolStripMenuItem.Text = "Repeat";
            this.repeatToolStripMenuItem.ToolTipText = "Repeat the inputs between \"Repeat\" and \"EndRepeat\" several times, nesting is not " +
    "supported.";
            this.repeatToolStripMenuItem.Click += new System.EventHandler(this.repeatToolStripMenuItem_Click);
            // 
            // endRepeatToolStripMenuItem
            // 
            this.endRepeatToolStripMenuItem.Name = "endRepeatToolStripMenuItem";
            this.endRepeatToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.endRepeatToolStripMenuItem.Text = "EndRepeat";
            this.endRepeatToolStripMenuItem.ToolTipText = "Repeat the inputs between \"Repeat\" and \"EndRepeat\" several times, nesting is not " +
    "supported.";
            this.endRepeatToolStripMenuItem.Click += new System.EventHandler(this.endRepeatToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(177, 6);
            // 
            // recordCountToolStripMenuItem
            // 
            this.recordCountToolStripMenuItem.Name = "recordCountToolStripMenuItem";
            this.recordCountToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.recordCountToolStripMenuItem.Text = "Record Count";
            this.recordCountToolStripMenuItem.ToolTipText = "Every time you run tas after modifying the current input file, the record count a" +
    "uto increases by one.";
            this.recordCountToolStripMenuItem.Click += new System.EventHandler(this.recordCountToolStripMenuItem_Click);
            // 
            // timeToolStripMenuItem
            // 
            this.timeToolStripMenuItem.Name = "timeToolStripMenuItem";
            this.timeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.timeToolStripMenuItem.Text = "Time";
            this.timeToolStripMenuItem.Click += new System.EventHandler(this.timeToolStripMenuItem_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(177, 6);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(364, 6);
            // 
            // combineConsecutiveSameInputsToolStripMenuItem
            // 
            this.combineConsecutiveSameInputsToolStripMenuItem.Name = "combineConsecutiveSameInputsToolStripMenuItem";
            this.combineConsecutiveSameInputsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.combineConsecutiveSameInputsToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.combineConsecutiveSameInputsToolStripMenuItem.Text = "Combine Consecutive Same Inputs";
            this.combineConsecutiveSameInputsToolStripMenuItem.Click += new System.EventHandler(this.combineConsecutiveSameInputsToolStripMenuItem_Click);
            // 
            // forceCombineInputsToolStripMenuItem
            // 
            this.forceCombineInputsToolStripMenuItem.Name = "forceCombineInputsToolStripMenuItem";
            this.forceCombineInputsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.forceCombineInputsToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.forceCombineInputsToolStripMenuItem.Text = "Force Combine Inputs Frames";
            this.forceCombineInputsToolStripMenuItem.Click += new System.EventHandler(this.forceCombineInputsToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(364, 6);
            // 
            // openReadFileToolStripMenuItem
            // 
            this.openReadFileToolStripMenuItem.Name = "openReadFileToolStripMenuItem";
            this.openReadFileToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.openReadFileToolStripMenuItem.Text = "Open Read File / Go to Play Line";
            this.openReadFileToolStripMenuItem.Click += new System.EventHandler(this.openReadFileToolStripMenuItem_Click);
            // 
            // richText
            // 
            this.richText.AllowDrop = true;
            this.richText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richText.AutoIndent = false;
            this.richText.AutoScrollMinSize = new System.Drawing.Size(33, 84);
            this.richText.BackBrush = null;
            this.richText.ChangedLineBgColor = System.Drawing.Color.DarkOrange;
            this.richText.ChangedLineTextColor = System.Drawing.Color.Teal;
            this.richText.CommentPrefix = "#";
            this.richText.CurrentFileName = null;
            this.richText.CurrentLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richText.CurrentLineSuffix = null;
            this.richText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.richText.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.richText.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richText.ForeColor = System.Drawing.Color.Black;
            this.richText.Language = TAS.Studio.RichText.Language.TAS;
            this.richText.LineNumberColor = System.Drawing.Color.Black;
            this.richText.Location = new System.Drawing.Point(0, 24);
            this.richText.Name = "richText";
            this.richText.Paddings = new System.Windows.Forms.Padding(0);
            this.richText.PlayingLineBgColor = System.Drawing.Color.Lime;
            this.richText.PlayingLineTextColor = System.Drawing.Color.Teal;
            this.richText.SaveStateBgColor = System.Drawing.Color.SteelBlue;
            this.richText.SaveStateLine = -1;
            this.richText.SaveStateTextColor = System.Drawing.Color.White;
            this.richText.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.richText.Size = new System.Drawing.Size(286, 496);
            this.richText.TabIndex = 0;
            this.richText.TabLength = 0;
            this.richText.TextChanged += new System.EventHandler<TAS.Studio.RichText.TextChangedEventArgs>(this.tasText_TextChanged);
            this.richText.NoChanges += new System.EventHandler(this.tasText_NoChanges);
            this.richText.FileOpening += new System.EventHandler(this.tasText_FileOpening);
            this.richText.LineInserted += new System.EventHandler<TAS.Studio.RichText.LineInsertedEventArgs>(this.tasText_LineInserted);
            this.richText.LineNeeded += new System.EventHandler<TAS.Studio.RichText.LineNeededEventArgs>(this.tasText_LineNeeded);
            this.richText.LineRemoved += new System.EventHandler<TAS.Studio.RichText.LineRemovedEventArgs>(this.tasText_LineRemoved);
            // 
            // fontDialog
            // 
            this.fontDialog.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // statusPanel
            // 
            this.statusPanel.AutoScroll = true;
            this.statusPanel.Controls.Add(this.statusBar);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusPanel.Location = new System.Drawing.Point(0, 502);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(286, 100);
            this.statusPanel.TabIndex = 5;
            // 
            // statusBar
            // 
            this.statusBar.AutoSize = false;
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusBar.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusBar.Location = new System.Drawing.Point(0, 0);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(286, 100);
            this.statusBar.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.lblStatus.Size = new System.Drawing.Size(271, 95);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Searching...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // Studio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(286, 602);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.dividerLabel);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.richText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(192, 205);
            this.Name = "Studio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Studio";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TASStudio_FormClosed);
            this.Shown += new System.EventHandler(this.Studio_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Studio_KeyDown);
            this.statusBarContextMenuStrip.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tasTextContextMenuStrip.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem themesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem darkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem repeatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endRepeatToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem combineConsecutiveSameInputsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceCombineInputsToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem recordCountToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem openBackupToolStripMenuItem;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;

        private System.Windows.Forms.ToolStripMenuItem backupFileCountsToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem backupRateToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem autoBackupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enabledAutoBackupToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem openPreviousFileToolStripMenuItem;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;

        private System.Windows.Forms.ToolStripMenuItem openReadFileToolStripMenuItem;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;

        private System.Windows.Forms.ToolStripMenuItem newFileToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem commentUncommentAllBreakpointsToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem removeAllUncommentedBreakpointsToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem autoRemoveExclusiveActionsToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem showGameInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;

        #endregion
		public TAS.Studio.RichText.RichText richText;
        private System.Windows.Forms.ToolTip hotkeyToolTip;
        private System.Windows.Forms.ContextMenuStrip statusBarContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyGameDataMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Label dividerLabel;
        private System.Windows.Forms.ToolStripMenuItem openFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRecentMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendInputsToCelesteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem homeMenuItem;
        private System.Windows.Forms.ContextMenuStrip tasTextContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem insertRemoveBreakPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commentUncommentTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllBreakpointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem insertRoomNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertCurrentInGameTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertOtherCommandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enforceLegalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reconnectStudioAndCelesteToolStripMenuItem;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripMenuItem communicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem portToolStripMenuItem;
    }
}