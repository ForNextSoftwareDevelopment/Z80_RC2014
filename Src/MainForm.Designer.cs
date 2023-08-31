
namespace Z80_RC2014
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetRAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetPortsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.resetSimulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disAssemblerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxFlags = new System.Windows.Forms.GroupBox();
            this.chkFlagN = new System.Windows.Forms.CheckBox();
            this.lblFlagN = new System.Windows.Forms.Label();
            this.chkFlagPV = new System.Windows.Forms.CheckBox();
            this.lblFlagPV = new System.Windows.Forms.Label();
            this.chkFlagC = new System.Windows.Forms.CheckBox();
            this.chkFlagH = new System.Windows.Forms.CheckBox();
            this.chkFlagZ = new System.Windows.Forms.CheckBox();
            this.chkFlagS = new System.Windows.Forms.CheckBox();
            this.lblFlagC = new System.Windows.Forms.Label();
            this.lblFlagH = new System.Windows.Forms.Label();
            this.lblFlagZ = new System.Windows.Forms.Label();
            this.lblFlagS = new System.Windows.Forms.Label();
            this.groupBoxRegisters = new System.Windows.Forms.GroupBox();
            this.labelRRegister = new System.Windows.Forms.Label();
            this.lblR = new System.Windows.Forms.Label();
            this.labelIRegister = new System.Windows.Forms.Label();
            this.lblI = new System.Windows.Forms.Label();
            this.lblAltHL = new System.Windows.Forms.Label();
            this.labelLaltRegister = new System.Windows.Forms.Label();
            this.labelHaltRegister = new System.Windows.Forms.Label();
            this.labelEaltRegister = new System.Windows.Forms.Label();
            this.labelDaltRegister = new System.Windows.Forms.Label();
            this.labelCaltRegister = new System.Windows.Forms.Label();
            this.labelBaltRegister = new System.Windows.Forms.Label();
            this.labelAaltRegister = new System.Windows.Forms.Label();
            this.labelIYRegister = new System.Windows.Forms.Label();
            this.lblIY = new System.Windows.Forms.Label();
            this.labelIXRegister = new System.Windows.Forms.Label();
            this.lblIX = new System.Windows.Forms.Label();
            this.labelSPRegister = new System.Windows.Forms.Label();
            this.labelPCRegister = new System.Windows.Forms.Label();
            this.labelLRegister = new System.Windows.Forms.Label();
            this.labelHRegister = new System.Windows.Forms.Label();
            this.labelERegister = new System.Windows.Forms.Label();
            this.labelDRegister = new System.Windows.Forms.Label();
            this.labelCRegister = new System.Windows.Forms.Label();
            this.labelBRegister = new System.Windows.Forms.Label();
            this.labelARegister = new System.Windows.Forms.Label();
            this.lblSP = new System.Windows.Forms.Label();
            this.lblPC = new System.Windows.Forms.Label();
            this.lblHL = new System.Windows.Forms.Label();
            this.lblE = new System.Windows.Forms.Label();
            this.lblD = new System.Windows.Forms.Label();
            this.lblC = new System.Windows.Forms.Label();
            this.lblB = new System.Windows.Forms.Label();
            this.lblA = new System.Windows.Forms.Label();
            this.panelMemoryInfo = new System.Windows.Forms.Panel();
            this.lblValueMemory = new System.Windows.Forms.Label();
            this.lblMemoryAddress = new System.Windows.Forms.Label();
            this.btnMemoryWrite = new System.Windows.Forms.Button();
            this.numMemoryAddress = new System.Windows.Forms.NumericUpDown();
            this.tbMemoryUpdateByte = new System.Windows.Forms.TextBox();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.btnMemoryStartAddress = new System.Windows.Forms.Button();
            this.tbMemoryStartAddress = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.btnPortWrite = new System.Windows.Forms.Button();
            this.tbPortUpdateByte = new System.Windows.Forms.TextBox();
            this.panelPortInfo = new System.Windows.Forms.Panel();
            this.btnClearPORT = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRestartSimulator = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonStartDebug = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRun = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFast = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStep = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this.toolTipRegisterBinary = new System.Windows.Forms.ToolTip(this.components);
            this.lblLine = new System.Windows.Forms.Label();
            this.lblColumn = new System.Windows.Forms.Label();
            this.btnViewProgram = new System.Windows.Forms.Button();
            this.btnViewSymbolTable = new System.Windows.Forms.Button();
            this.panelMemory = new System.Windows.Forms.Panel();
            this.btnViewSP = new System.Windows.Forms.Button();
            this.btnViewPC = new System.Windows.Forms.Button();
            this.chkLock = new System.Windows.Forms.CheckBox();
            this.panelWriteMemory = new System.Windows.Forms.Panel();
            this.panelPorts = new System.Windows.Forms.Panel();
            this.panelUpdatePort = new System.Windows.Forms.Panel();
            this.lblValuePort = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.richTextBoxProgram = new System.Windows.Forms.RichTextBox();
            this.btnClearBreakPoint = new System.Windows.Forms.Button();
            this.pbBreakPoint = new System.Windows.Forms.PictureBox();
            this.numericUpDownDelay = new System.Windows.Forms.NumericUpDown();
            this.lblDelay = new System.Windows.Forms.Label();
            this.lblSetProgramCounter = new System.Windows.Forms.Label();
            this.tbSetProgramCounter = new System.Windows.Forms.TextBox();
            this.chkTerminal = new System.Windows.Forms.CheckBox();
            this.panelInterrupt = new System.Windows.Forms.Panel();
            this.lblIM = new System.Windows.Forms.Label();
            this.lblInterrupts = new System.Windows.Forms.Label();
            this.tcInstructions = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.groupBoxAltFlags = new System.Windows.Forms.GroupBox();
            this.chkFlagNalt = new System.Windows.Forms.CheckBox();
            this.lblFlagNalt = new System.Windows.Forms.Label();
            this.chkFlagPValt = new System.Windows.Forms.CheckBox();
            this.lblFlagPValt = new System.Windows.Forms.Label();
            this.chkFlagCalt = new System.Windows.Forms.CheckBox();
            this.chkFlagHalt = new System.Windows.Forms.CheckBox();
            this.chkFlagZalt = new System.Windows.Forms.CheckBox();
            this.chkFlagSalt = new System.Windows.Forms.CheckBox();
            this.lblFlagCalt = new System.Windows.Forms.Label();
            this.lblFlagHalt = new System.Windows.Forms.Label();
            this.lblFlagZalt = new System.Windows.Forms.Label();
            this.lblFlagSalt = new System.Windows.Forms.Label();
            this.chkBreakOnExternalCode = new System.Windows.Forms.CheckBox();
            this.tcSources = new System.Windows.Forms.TabControl();
            this.tpProgram = new System.Windows.Forms.TabPage();
            this.tpCompactFlash = new System.Windows.Forms.TabPage();
            this.richTextBoxCompactFlash = new System.Windows.Forms.RichTextBox();
            this.tcMemoryCompactFlash = new System.Windows.Forms.TabControl();
            this.tpMemory = new System.Windows.Forms.TabPage();
            this.tpCFCard = new System.Windows.Forms.TabPage();
            this.compactFlash = new Z80_RC2014.CompactFlash();
            this.chkShowExternalCode = new System.Windows.Forms.CheckBox();
            this.chkBreakOnAddress = new System.Windows.Forms.CheckBox();
            this.tbBreakOnAddress = new System.Windows.Forms.TextBox();
            this.menuStrip.SuspendLayout();
            this.groupBoxFlags.SuspendLayout();
            this.groupBoxRegisters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMemoryAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.panelMemory.SuspendLayout();
            this.panelWriteMemory.SuspendLayout();
            this.panelPorts.SuspendLayout();
            this.panelUpdatePort.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBreakPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDelay)).BeginInit();
            this.panelInterrupt.SuspendLayout();
            this.tcInstructions.SuspendLayout();
            this.groupBoxAltFlags.SuspendLayout();
            this.tcSources.SuspendLayout();
            this.tpProgram.SuspendLayout();
            this.tpCompactFlash.SuspendLayout();
            this.tcMemoryCompactFlash.SuspendLayout();
            this.tpMemory.SuspendLayout();
            this.tpCFCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.disAssemblerToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(9, 8);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(227, 24);
            this.menuStrip.TabIndex = 13;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileToolStripMenuItem,
            this.openToolStripMenuItem,
            this.loadBinaryToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveBinaryToolStripMenuItem,
            this.appendBinaryToolStripMenuItem,
            this.toolStripSeparator4,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newFileToolStripMenuItem
            // 
            this.newFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newFileToolStripMenuItem.Image")));
            this.newFileToolStripMenuItem.Name = "newFileToolStripMenuItem";
            this.newFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newFileToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.newFileToolStripMenuItem.Text = "&New";
            this.newFileToolStripMenuItem.Click += new System.EventHandler(this.new_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::Z80_RC2014.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.open_Click);
            // 
            // loadBinaryToolStripMenuItem
            // 
            this.loadBinaryToolStripMenuItem.Name = "loadBinaryToolStripMenuItem";
            this.loadBinaryToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.loadBinaryToolStripMenuItem.Text = "Load Binary";
            this.loadBinaryToolStripMenuItem.Click += new System.EventHandler(this.loadBinary_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(204, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::Z80_RC2014.Properties.Resources.save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.save_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = global::Z80_RC2014.Properties.Resources.save_as;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAs_Click);
            // 
            // saveBinaryToolStripMenuItem
            // 
            this.saveBinaryToolStripMenuItem.Image = global::Z80_RC2014.Properties.Resources.save_binary;
            this.saveBinaryToolStripMenuItem.Name = "saveBinaryToolStripMenuItem";
            this.saveBinaryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
            this.saveBinaryToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.saveBinaryToolStripMenuItem.Text = "Save &Binary";
            this.saveBinaryToolStripMenuItem.Click += new System.EventHandler(this.saveBinary_Click);
            // 
            // appendBinaryToolStripMenuItem
            // 
            this.appendBinaryToolStripMenuItem.Image = global::Z80_RC2014.Properties.Resources.save_binary;
            this.appendBinaryToolStripMenuItem.Name = "appendBinaryToolStripMenuItem";
            this.appendBinaryToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.appendBinaryToolStripMenuItem.Text = "Append Binary";
            this.appendBinaryToolStripMenuItem.Click += new System.EventHandler(this.appendBinaryToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(204, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quit_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetRAMToolStripMenuItem,
            this.resetPortsToolStripMenuItem,
            this.toolStripSeparator5,
            this.resetSimulatorToolStripMenuItem});
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.resetToolStripMenuItem.Text = "&Reset";
            // 
            // resetRAMToolStripMenuItem
            // 
            this.resetRAMToolStripMenuItem.Name = "resetRAMToolStripMenuItem";
            this.resetRAMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.R)));
            this.resetRAMToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.resetRAMToolStripMenuItem.Text = "Reset RAM";
            this.resetRAMToolStripMenuItem.Click += new System.EventHandler(this.resetRAM_Click);
            // 
            // resetPortsToolStripMenuItem
            // 
            this.resetPortsToolStripMenuItem.Name = "resetPortsToolStripMenuItem";
            this.resetPortsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
            this.resetPortsToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.resetPortsToolStripMenuItem.Text = "Reset Ports";
            this.resetPortsToolStripMenuItem.Click += new System.EventHandler(this.resetPorts_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(202, 6);
            // 
            // resetSimulatorToolStripMenuItem
            // 
            this.resetSimulatorToolStripMenuItem.Image = global::Z80_RC2014.Properties.Resources.reset;
            this.resetSimulatorToolStripMenuItem.Name = "resetSimulatorToolStripMenuItem";
            this.resetSimulatorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resetSimulatorToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.resetSimulatorToolStripMenuItem.Text = "&Reset Simulator";
            this.resetSimulatorToolStripMenuItem.Click += new System.EventHandler(this.resetSimulator_Click);
            // 
            // disAssemblerToolStripMenuItem
            // 
            this.disAssemblerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBinaryToolStripMenuItem});
            this.disAssemblerToolStripMenuItem.Name = "disAssemblerToolStripMenuItem";
            this.disAssemblerToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.disAssemblerToolStripMenuItem.Text = "DisAssembler";
            // 
            // openBinaryToolStripMenuItem
            // 
            this.openBinaryToolStripMenuItem.Image = global::Z80_RC2014.Properties.Resources.open;
            this.openBinaryToolStripMenuItem.Name = "openBinaryToolStripMenuItem";
            this.openBinaryToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.openBinaryToolStripMenuItem.Text = "Open Binary";
            this.openBinaryToolStripMenuItem.Click += new System.EventHandler(this.openBinary_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // viewHelpToolStripMenuItem
            // 
            this.viewHelpToolStripMenuItem.Image = global::Z80_RC2014.Properties.Resources.help;
            this.viewHelpToolStripMenuItem.Name = "viewHelpToolStripMenuItem";
            this.viewHelpToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.viewHelpToolStripMenuItem.Text = "View Help";
            this.viewHelpToolStripMenuItem.Click += new System.EventHandler(this.viewHelp_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.about_Click);
            // 
            // groupBoxFlags
            // 
            this.groupBoxFlags.BackColor = System.Drawing.SystemColors.Info;
            this.groupBoxFlags.Controls.Add(this.chkFlagN);
            this.groupBoxFlags.Controls.Add(this.lblFlagN);
            this.groupBoxFlags.Controls.Add(this.chkFlagPV);
            this.groupBoxFlags.Controls.Add(this.lblFlagPV);
            this.groupBoxFlags.Controls.Add(this.chkFlagC);
            this.groupBoxFlags.Controls.Add(this.chkFlagH);
            this.groupBoxFlags.Controls.Add(this.chkFlagZ);
            this.groupBoxFlags.Controls.Add(this.chkFlagS);
            this.groupBoxFlags.Controls.Add(this.lblFlagC);
            this.groupBoxFlags.Controls.Add(this.lblFlagH);
            this.groupBoxFlags.Controls.Add(this.lblFlagZ);
            this.groupBoxFlags.Controls.Add(this.lblFlagS);
            this.groupBoxFlags.Location = new System.Drawing.Point(12, 204);
            this.groupBoxFlags.Name = "groupBoxFlags";
            this.groupBoxFlags.Size = new System.Drawing.Size(296, 53);
            this.groupBoxFlags.TabIndex = 3;
            this.groupBoxFlags.TabStop = false;
            this.groupBoxFlags.Text = "Flags";
            // 
            // chkFlagN
            // 
            this.chkFlagN.AutoSize = true;
            this.chkFlagN.Location = new System.Drawing.Point(220, 33);
            this.chkFlagN.Name = "chkFlagN";
            this.chkFlagN.Size = new System.Drawing.Size(15, 14);
            this.chkFlagN.TabIndex = 22;
            this.chkFlagN.UseVisualStyleBackColor = true;
            // 
            // lblFlagN
            // 
            this.lblFlagN.AutoSize = true;
            this.lblFlagN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagN.Location = new System.Drawing.Point(219, 17);
            this.lblFlagN.Name = "lblFlagN";
            this.lblFlagN.Size = new System.Drawing.Size(16, 13);
            this.lblFlagN.TabIndex = 21;
            this.lblFlagN.Text = "N";
            // 
            // chkFlagPV
            // 
            this.chkFlagPV.AutoSize = true;
            this.chkFlagPV.Location = new System.Drawing.Point(166, 33);
            this.chkFlagPV.Name = "chkFlagPV";
            this.chkFlagPV.Size = new System.Drawing.Size(15, 14);
            this.chkFlagPV.TabIndex = 20;
            this.chkFlagPV.UseVisualStyleBackColor = true;
            // 
            // lblFlagPV
            // 
            this.lblFlagPV.AutoSize = true;
            this.lblFlagPV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagPV.Location = new System.Drawing.Point(162, 17);
            this.lblFlagPV.Name = "lblFlagPV";
            this.lblFlagPV.Size = new System.Drawing.Size(29, 13);
            this.lblFlagPV.TabIndex = 19;
            this.lblFlagPV.Text = "P/V";
            // 
            // chkFlagC
            // 
            this.chkFlagC.AutoSize = true;
            this.chkFlagC.Location = new System.Drawing.Point(270, 33);
            this.chkFlagC.Name = "chkFlagC";
            this.chkFlagC.Size = new System.Drawing.Size(15, 14);
            this.chkFlagC.TabIndex = 18;
            this.chkFlagC.UseVisualStyleBackColor = true;
            this.chkFlagC.CheckedChanged += new System.EventHandler(this.chkFlagC_CheckedChanged);
            // 
            // chkFlagH
            // 
            this.chkFlagH.AutoSize = true;
            this.chkFlagH.Location = new System.Drawing.Point(114, 33);
            this.chkFlagH.Name = "chkFlagH";
            this.chkFlagH.Size = new System.Drawing.Size(15, 14);
            this.chkFlagH.TabIndex = 16;
            this.chkFlagH.UseVisualStyleBackColor = true;
            this.chkFlagH.CheckedChanged += new System.EventHandler(this.chkFlagH_CheckedChanged);
            // 
            // chkFlagZ
            // 
            this.chkFlagZ.AutoSize = true;
            this.chkFlagZ.Location = new System.Drawing.Point(62, 33);
            this.chkFlagZ.Name = "chkFlagZ";
            this.chkFlagZ.Size = new System.Drawing.Size(15, 14);
            this.chkFlagZ.TabIndex = 15;
            this.chkFlagZ.UseVisualStyleBackColor = true;
            this.chkFlagZ.CheckedChanged += new System.EventHandler(this.chkFlagZ_CheckedChanged);
            // 
            // chkFlagS
            // 
            this.chkFlagS.AutoSize = true;
            this.chkFlagS.Location = new System.Drawing.Point(10, 33);
            this.chkFlagS.Name = "chkFlagS";
            this.chkFlagS.Size = new System.Drawing.Size(15, 14);
            this.chkFlagS.TabIndex = 10;
            this.chkFlagS.UseVisualStyleBackColor = true;
            this.chkFlagS.CheckedChanged += new System.EventHandler(this.chkFlagS_CheckedChanged);
            // 
            // lblFlagC
            // 
            this.lblFlagC.AutoSize = true;
            this.lblFlagC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagC.Location = new System.Drawing.Point(270, 17);
            this.lblFlagC.Name = "lblFlagC";
            this.lblFlagC.Size = new System.Drawing.Size(15, 13);
            this.lblFlagC.TabIndex = 7;
            this.lblFlagC.Text = "C";
            // 
            // lblFlagH
            // 
            this.lblFlagH.AutoSize = true;
            this.lblFlagH.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagH.Location = new System.Drawing.Point(113, 17);
            this.lblFlagH.Name = "lblFlagH";
            this.lblFlagH.Size = new System.Drawing.Size(16, 13);
            this.lblFlagH.TabIndex = 3;
            this.lblFlagH.Text = "H";
            // 
            // lblFlagZ
            // 
            this.lblFlagZ.AutoSize = true;
            this.lblFlagZ.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagZ.Location = new System.Drawing.Point(62, 16);
            this.lblFlagZ.Name = "lblFlagZ";
            this.lblFlagZ.Size = new System.Drawing.Size(15, 13);
            this.lblFlagZ.TabIndex = 1;
            this.lblFlagZ.Text = "Z";
            // 
            // lblFlagS
            // 
            this.lblFlagS.AutoSize = true;
            this.lblFlagS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagS.Location = new System.Drawing.Point(10, 16);
            this.lblFlagS.Name = "lblFlagS";
            this.lblFlagS.Size = new System.Drawing.Size(15, 13);
            this.lblFlagS.TabIndex = 0;
            this.lblFlagS.Text = "S";
            // 
            // groupBoxRegisters
            // 
            this.groupBoxRegisters.BackColor = System.Drawing.SystemColors.Info;
            this.groupBoxRegisters.Controls.Add(this.labelRRegister);
            this.groupBoxRegisters.Controls.Add(this.lblR);
            this.groupBoxRegisters.Controls.Add(this.labelIRegister);
            this.groupBoxRegisters.Controls.Add(this.lblI);
            this.groupBoxRegisters.Controls.Add(this.lblAltHL);
            this.groupBoxRegisters.Controls.Add(this.labelLaltRegister);
            this.groupBoxRegisters.Controls.Add(this.labelHaltRegister);
            this.groupBoxRegisters.Controls.Add(this.labelEaltRegister);
            this.groupBoxRegisters.Controls.Add(this.labelDaltRegister);
            this.groupBoxRegisters.Controls.Add(this.labelCaltRegister);
            this.groupBoxRegisters.Controls.Add(this.labelBaltRegister);
            this.groupBoxRegisters.Controls.Add(this.labelAaltRegister);
            this.groupBoxRegisters.Controls.Add(this.labelIYRegister);
            this.groupBoxRegisters.Controls.Add(this.lblIY);
            this.groupBoxRegisters.Controls.Add(this.labelIXRegister);
            this.groupBoxRegisters.Controls.Add(this.lblIX);
            this.groupBoxRegisters.Controls.Add(this.labelSPRegister);
            this.groupBoxRegisters.Controls.Add(this.labelPCRegister);
            this.groupBoxRegisters.Controls.Add(this.labelLRegister);
            this.groupBoxRegisters.Controls.Add(this.labelHRegister);
            this.groupBoxRegisters.Controls.Add(this.labelERegister);
            this.groupBoxRegisters.Controls.Add(this.labelDRegister);
            this.groupBoxRegisters.Controls.Add(this.labelCRegister);
            this.groupBoxRegisters.Controls.Add(this.labelBRegister);
            this.groupBoxRegisters.Controls.Add(this.labelARegister);
            this.groupBoxRegisters.Controls.Add(this.lblSP);
            this.groupBoxRegisters.Controls.Add(this.lblPC);
            this.groupBoxRegisters.Controls.Add(this.lblHL);
            this.groupBoxRegisters.Controls.Add(this.lblE);
            this.groupBoxRegisters.Controls.Add(this.lblD);
            this.groupBoxRegisters.Controls.Add(this.lblC);
            this.groupBoxRegisters.Controls.Add(this.lblB);
            this.groupBoxRegisters.Controls.Add(this.lblA);
            this.groupBoxRegisters.Location = new System.Drawing.Point(12, 40);
            this.groupBoxRegisters.Name = "groupBoxRegisters";
            this.groupBoxRegisters.Size = new System.Drawing.Size(296, 158);
            this.groupBoxRegisters.TabIndex = 2;
            this.groupBoxRegisters.TabStop = false;
            this.groupBoxRegisters.Text = "Registers";
            // 
            // labelRRegister
            // 
            this.labelRRegister.AutoSize = true;
            this.labelRRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRRegister.Location = new System.Drawing.Point(233, 132);
            this.labelRRegister.Name = "labelRRegister";
            this.labelRRegister.Size = new System.Drawing.Size(27, 20);
            this.labelRRegister.TabIndex = 43;
            this.labelRRegister.Text = "00";
            // 
            // lblR
            // 
            this.lblR.AutoSize = true;
            this.lblR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblR.Location = new System.Drawing.Point(197, 132);
            this.lblR.Name = "lblR";
            this.lblR.Size = new System.Drawing.Size(22, 20);
            this.lblR.TabIndex = 42;
            this.lblR.Text = "R";
            // 
            // labelIRegister
            // 
            this.labelIRegister.AutoSize = true;
            this.labelIRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIRegister.Location = new System.Drawing.Point(233, 112);
            this.labelIRegister.Name = "labelIRegister";
            this.labelIRegister.Size = new System.Drawing.Size(27, 20);
            this.labelIRegister.TabIndex = 41;
            this.labelIRegister.Text = "00";
            // 
            // lblI
            // 
            this.lblI.AutoSize = true;
            this.lblI.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblI.Location = new System.Drawing.Point(197, 112);
            this.lblI.Name = "lblI";
            this.lblI.Size = new System.Drawing.Size(15, 20);
            this.lblI.TabIndex = 40;
            this.lblI.Text = "I";
            // 
            // lblAltHL
            // 
            this.lblAltHL.AutoSize = true;
            this.lblAltHL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAltHL.Location = new System.Drawing.Point(6, 132);
            this.lblAltHL.Name = "lblAltHL";
            this.lblAltHL.Size = new System.Drawing.Size(40, 20);
            this.lblAltHL.TabIndex = 39;
            this.lblAltHL.Text = "H\'L\'";
            // 
            // labelLaltRegister
            // 
            this.labelLaltRegister.AutoSize = true;
            this.labelLaltRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLaltRegister.Location = new System.Drawing.Point(88, 132);
            this.labelLaltRegister.Name = "labelLaltRegister";
            this.labelLaltRegister.Size = new System.Drawing.Size(27, 20);
            this.labelLaltRegister.TabIndex = 38;
            this.labelLaltRegister.Text = "00";
            // 
            // labelHaltRegister
            // 
            this.labelHaltRegister.AutoSize = true;
            this.labelHaltRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHaltRegister.Location = new System.Drawing.Point(55, 132);
            this.labelHaltRegister.Name = "labelHaltRegister";
            this.labelHaltRegister.Size = new System.Drawing.Size(27, 20);
            this.labelHaltRegister.TabIndex = 37;
            this.labelHaltRegister.Text = "00";
            // 
            // labelEaltRegister
            // 
            this.labelEaltRegister.AutoSize = true;
            this.labelEaltRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEaltRegister.Location = new System.Drawing.Point(88, 92);
            this.labelEaltRegister.Name = "labelEaltRegister";
            this.labelEaltRegister.Size = new System.Drawing.Size(27, 20);
            this.labelEaltRegister.TabIndex = 36;
            this.labelEaltRegister.Text = "00";
            // 
            // labelDaltRegister
            // 
            this.labelDaltRegister.AutoSize = true;
            this.labelDaltRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDaltRegister.Location = new System.Drawing.Point(88, 73);
            this.labelDaltRegister.Name = "labelDaltRegister";
            this.labelDaltRegister.Size = new System.Drawing.Size(27, 20);
            this.labelDaltRegister.TabIndex = 35;
            this.labelDaltRegister.Text = "00";
            // 
            // labelCaltRegister
            // 
            this.labelCaltRegister.AutoSize = true;
            this.labelCaltRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCaltRegister.Location = new System.Drawing.Point(88, 54);
            this.labelCaltRegister.Name = "labelCaltRegister";
            this.labelCaltRegister.Size = new System.Drawing.Size(27, 20);
            this.labelCaltRegister.TabIndex = 34;
            this.labelCaltRegister.Text = "00";
            // 
            // labelBaltRegister
            // 
            this.labelBaltRegister.AutoSize = true;
            this.labelBaltRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBaltRegister.Location = new System.Drawing.Point(88, 35);
            this.labelBaltRegister.Name = "labelBaltRegister";
            this.labelBaltRegister.Size = new System.Drawing.Size(27, 20);
            this.labelBaltRegister.TabIndex = 33;
            this.labelBaltRegister.Text = "00";
            // 
            // labelAaltRegister
            // 
            this.labelAaltRegister.AutoSize = true;
            this.labelAaltRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAaltRegister.Location = new System.Drawing.Point(88, 16);
            this.labelAaltRegister.Name = "labelAaltRegister";
            this.labelAaltRegister.Size = new System.Drawing.Size(27, 20);
            this.labelAaltRegister.TabIndex = 32;
            this.labelAaltRegister.Text = "00";
            // 
            // labelIYRegister
            // 
            this.labelIYRegister.AutoSize = true;
            this.labelIYRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIYRegister.Location = new System.Drawing.Point(233, 73);
            this.labelIYRegister.Name = "labelIYRegister";
            this.labelIYRegister.Size = new System.Drawing.Size(45, 20);
            this.labelIYRegister.TabIndex = 31;
            this.labelIYRegister.Text = "0000";
            // 
            // lblIY
            // 
            this.lblIY.AutoSize = true;
            this.lblIY.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIY.Location = new System.Drawing.Point(197, 73);
            this.lblIY.Name = "lblIY";
            this.lblIY.Size = new System.Drawing.Size(27, 20);
            this.lblIY.TabIndex = 30;
            this.lblIY.Text = "IY";
            // 
            // labelIXRegister
            // 
            this.labelIXRegister.AutoSize = true;
            this.labelIXRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIXRegister.Location = new System.Drawing.Point(233, 54);
            this.labelIXRegister.Name = "labelIXRegister";
            this.labelIXRegister.Size = new System.Drawing.Size(45, 20);
            this.labelIXRegister.TabIndex = 29;
            this.labelIXRegister.Text = "0000";
            // 
            // lblIX
            // 
            this.lblIX.AutoSize = true;
            this.lblIX.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIX.Location = new System.Drawing.Point(197, 54);
            this.lblIX.Name = "lblIX";
            this.lblIX.Size = new System.Drawing.Size(27, 20);
            this.lblIX.TabIndex = 28;
            this.lblIX.Text = "IX";
            // 
            // labelSPRegister
            // 
            this.labelSPRegister.AutoSize = true;
            this.labelSPRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSPRegister.Location = new System.Drawing.Point(233, 35);
            this.labelSPRegister.Name = "labelSPRegister";
            this.labelSPRegister.Size = new System.Drawing.Size(45, 20);
            this.labelSPRegister.TabIndex = 27;
            this.labelSPRegister.Text = "0000";
            this.labelSPRegister.MouseHover += new System.EventHandler(this.labelSPRegister_MouseHover);
            // 
            // labelPCRegister
            // 
            this.labelPCRegister.AutoSize = true;
            this.labelPCRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPCRegister.Location = new System.Drawing.Point(233, 16);
            this.labelPCRegister.Name = "labelPCRegister";
            this.labelPCRegister.Size = new System.Drawing.Size(45, 20);
            this.labelPCRegister.TabIndex = 26;
            this.labelPCRegister.Text = "0000";
            this.labelPCRegister.MouseHover += new System.EventHandler(this.labelPCRegister_MouseHover);
            // 
            // labelLRegister
            // 
            this.labelLRegister.AutoSize = true;
            this.labelLRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLRegister.Location = new System.Drawing.Point(88, 112);
            this.labelLRegister.Name = "labelLRegister";
            this.labelLRegister.Size = new System.Drawing.Size(27, 20);
            this.labelLRegister.TabIndex = 25;
            this.labelLRegister.Text = "00";
            this.labelLRegister.MouseHover += new System.EventHandler(this.labelLRegister_MouseHover);
            // 
            // labelHRegister
            // 
            this.labelHRegister.AutoSize = true;
            this.labelHRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHRegister.Location = new System.Drawing.Point(55, 112);
            this.labelHRegister.Name = "labelHRegister";
            this.labelHRegister.Size = new System.Drawing.Size(27, 20);
            this.labelHRegister.TabIndex = 24;
            this.labelHRegister.Text = "00";
            this.labelHRegister.MouseHover += new System.EventHandler(this.labelHRegister_MouseHover);
            // 
            // labelERegister
            // 
            this.labelERegister.AutoSize = true;
            this.labelERegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelERegister.Location = new System.Drawing.Point(55, 92);
            this.labelERegister.Name = "labelERegister";
            this.labelERegister.Size = new System.Drawing.Size(27, 20);
            this.labelERegister.TabIndex = 23;
            this.labelERegister.Text = "00";
            this.labelERegister.MouseHover += new System.EventHandler(this.labelERegister_MouseHover);
            // 
            // labelDRegister
            // 
            this.labelDRegister.AutoSize = true;
            this.labelDRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDRegister.Location = new System.Drawing.Point(55, 73);
            this.labelDRegister.Name = "labelDRegister";
            this.labelDRegister.Size = new System.Drawing.Size(27, 20);
            this.labelDRegister.TabIndex = 22;
            this.labelDRegister.Text = "00";
            this.labelDRegister.MouseHover += new System.EventHandler(this.labelDRegister_MouseHover);
            // 
            // labelCRegister
            // 
            this.labelCRegister.AutoSize = true;
            this.labelCRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCRegister.Location = new System.Drawing.Point(55, 54);
            this.labelCRegister.Name = "labelCRegister";
            this.labelCRegister.Size = new System.Drawing.Size(27, 20);
            this.labelCRegister.TabIndex = 21;
            this.labelCRegister.Text = "00";
            this.labelCRegister.MouseHover += new System.EventHandler(this.labelCRegister_MouseHover);
            // 
            // labelBRegister
            // 
            this.labelBRegister.AutoSize = true;
            this.labelBRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBRegister.Location = new System.Drawing.Point(55, 35);
            this.labelBRegister.Name = "labelBRegister";
            this.labelBRegister.Size = new System.Drawing.Size(27, 20);
            this.labelBRegister.TabIndex = 20;
            this.labelBRegister.Text = "00";
            this.labelBRegister.MouseHover += new System.EventHandler(this.labelBRegister_MouseHover);
            // 
            // labelARegister
            // 
            this.labelARegister.AutoSize = true;
            this.labelARegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelARegister.Location = new System.Drawing.Point(55, 16);
            this.labelARegister.Name = "labelARegister";
            this.labelARegister.Size = new System.Drawing.Size(27, 20);
            this.labelARegister.TabIndex = 19;
            this.labelARegister.Text = "00";
            this.labelARegister.MouseHover += new System.EventHandler(this.labelARegister_MouseHover);
            // 
            // lblSP
            // 
            this.lblSP.AutoSize = true;
            this.lblSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSP.Location = new System.Drawing.Point(195, 35);
            this.lblSP.Name = "lblSP";
            this.lblSP.Size = new System.Drawing.Size(32, 20);
            this.lblSP.TabIndex = 18;
            this.lblSP.Text = "SP";
            // 
            // lblPC
            // 
            this.lblPC.AutoSize = true;
            this.lblPC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPC.Location = new System.Drawing.Point(195, 16);
            this.lblPC.Name = "lblPC";
            this.lblPC.Size = new System.Drawing.Size(32, 20);
            this.lblPC.TabIndex = 17;
            this.lblPC.Text = "PC";
            // 
            // lblHL
            // 
            this.lblHL.AutoSize = true;
            this.lblHL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHL.Location = new System.Drawing.Point(6, 112);
            this.lblHL.Name = "lblHL";
            this.lblHL.Size = new System.Drawing.Size(32, 20);
            this.lblHL.TabIndex = 15;
            this.lblHL.Text = "HL";
            // 
            // lblE
            // 
            this.lblE.AutoSize = true;
            this.lblE.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblE.Location = new System.Drawing.Point(6, 92);
            this.lblE.Name = "lblE";
            this.lblE.Size = new System.Drawing.Size(42, 20);
            this.lblE.TabIndex = 14;
            this.lblE.Text = "E/E\'";
            // 
            // lblD
            // 
            this.lblD.AutoSize = true;
            this.lblD.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblD.Location = new System.Drawing.Point(6, 73);
            this.lblD.Name = "lblD";
            this.lblD.Size = new System.Drawing.Size(44, 20);
            this.lblD.TabIndex = 13;
            this.lblD.Text = "D/D\'";
            // 
            // lblC
            // 
            this.lblC.AutoSize = true;
            this.lblC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblC.Location = new System.Drawing.Point(6, 54);
            this.lblC.Name = "lblC";
            this.lblC.Size = new System.Drawing.Size(42, 20);
            this.lblC.TabIndex = 12;
            this.lblC.Text = "C/C\'";
            // 
            // lblB
            // 
            this.lblB.AutoSize = true;
            this.lblB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblB.Location = new System.Drawing.Point(6, 35);
            this.lblB.Name = "lblB";
            this.lblB.Size = new System.Drawing.Size(42, 20);
            this.lblB.TabIndex = 11;
            this.lblB.Text = "B/B\'";
            // 
            // lblA
            // 
            this.lblA.AutoSize = true;
            this.lblA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblA.Location = new System.Drawing.Point(6, 16);
            this.lblA.Name = "lblA";
            this.lblA.Size = new System.Drawing.Size(42, 20);
            this.lblA.TabIndex = 10;
            this.lblA.Text = "A/A\'";
            // 
            // panelMemoryInfo
            // 
            this.panelMemoryInfo.BackColor = System.Drawing.SystemColors.Info;
            this.panelMemoryInfo.Location = new System.Drawing.Point(6, 32);
            this.panelMemoryInfo.Name = "panelMemoryInfo";
            this.panelMemoryInfo.Size = new System.Drawing.Size(534, 348);
            this.panelMemoryInfo.TabIndex = 6;
            // 
            // lblValueMemory
            // 
            this.lblValueMemory.AutoSize = true;
            this.lblValueMemory.Location = new System.Drawing.Point(76, 4);
            this.lblValueMemory.Name = "lblValueMemory";
            this.lblValueMemory.Size = new System.Drawing.Size(34, 13);
            this.lblValueMemory.TabIndex = 10;
            this.lblValueMemory.Text = "Value";
            // 
            // lblMemoryAddress
            // 
            this.lblMemoryAddress.AutoSize = true;
            this.lblMemoryAddress.Location = new System.Drawing.Point(3, 4);
            this.lblMemoryAddress.Name = "lblMemoryAddress";
            this.lblMemoryAddress.Size = new System.Drawing.Size(45, 13);
            this.lblMemoryAddress.TabIndex = 9;
            this.lblMemoryAddress.Text = "Address";
            // 
            // btnMemoryWrite
            // 
            this.btnMemoryWrite.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.btnMemoryWrite.Location = new System.Drawing.Point(116, 17);
            this.btnMemoryWrite.Name = "btnMemoryWrite";
            this.btnMemoryWrite.Size = new System.Drawing.Size(72, 23);
            this.btnMemoryWrite.TabIndex = 8;
            this.btnMemoryWrite.Text = "Write";
            this.btnMemoryWrite.UseVisualStyleBackColor = false;
            this.btnMemoryWrite.Click += new System.EventHandler(this.btnMemoryWrite_Click);
            // 
            // numMemoryAddress
            // 
            this.numMemoryAddress.Hexadecimal = true;
            this.numMemoryAddress.Location = new System.Drawing.Point(7, 20);
            this.numMemoryAddress.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numMemoryAddress.Name = "numMemoryAddress";
            this.numMemoryAddress.Size = new System.Drawing.Size(67, 20);
            this.numMemoryAddress.TabIndex = 7;
            this.numMemoryAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numMemoryAddress.Value = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            // 
            // tbMemoryUpdateByte
            // 
            this.tbMemoryUpdateByte.Location = new System.Drawing.Point(80, 20);
            this.tbMemoryUpdateByte.Name = "tbMemoryUpdateByte";
            this.tbMemoryUpdateByte.Size = new System.Drawing.Size(30, 20);
            this.tbMemoryUpdateByte.TabIndex = 0;
            this.tbMemoryUpdateByte.Text = "00";
            this.tbMemoryUpdateByte.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnNextPage
            // 
            this.btnNextPage.BackColor = System.Drawing.Color.LightGreen;
            this.btnNextPage.Location = new System.Drawing.Point(491, 385);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(49, 23);
            this.btnNextPage.TabIndex = 8;
            this.btnNextPage.Text = "==>";
            this.btnNextPage.UseVisualStyleBackColor = false;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.BackColor = System.Drawing.Color.LightGreen;
            this.btnPrevPage.Location = new System.Drawing.Point(439, 385);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(49, 23);
            this.btnPrevPage.TabIndex = 7;
            this.btnPrevPage.Text = "<==";
            this.btnPrevPage.UseVisualStyleBackColor = false;
            this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);
            // 
            // btnMemoryStartAddress
            // 
            this.btnMemoryStartAddress.BackColor = System.Drawing.Color.LightGreen;
            this.btnMemoryStartAddress.Location = new System.Drawing.Point(107, 6);
            this.btnMemoryStartAddress.Name = "btnMemoryStartAddress";
            this.btnMemoryStartAddress.Size = new System.Drawing.Size(83, 23);
            this.btnMemoryStartAddress.TabIndex = 4;
            this.btnMemoryStartAddress.Text = "View Address";
            this.btnMemoryStartAddress.UseVisualStyleBackColor = false;
            this.btnMemoryStartAddress.Click += new System.EventHandler(this.btnMemoryStartAddress_Click);
            // 
            // tbMemoryStartAddress
            // 
            this.tbMemoryStartAddress.Location = new System.Drawing.Point(59, 8);
            this.tbMemoryStartAddress.MaxLength = 4;
            this.tbMemoryStartAddress.Name = "tbMemoryStartAddress";
            this.tbMemoryStartAddress.Size = new System.Drawing.Size(42, 20);
            this.tbMemoryStartAddress.TabIndex = 3;
            this.tbMemoryStartAddress.Text = "0000";
            this.tbMemoryStartAddress.TextChanged += new System.EventHandler(this.tbMemoryStartAddress_TextChanged);
            this.tbMemoryStartAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMemoryStartAddress_KeyPress);
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(8, 11);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(45, 13);
            this.lblAddress.TabIndex = 2;
            this.lblAddress.Text = "Address";
            // 
            // numPort
            // 
            this.numPort.Hexadecimal = true;
            this.numPort.Location = new System.Drawing.Point(6, 22);
            this.numPort.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(67, 20);
            this.numPort.TabIndex = 9;
            this.numPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPortWrite
            // 
            this.btnPortWrite.BackColor = System.Drawing.Color.LightYellow;
            this.btnPortWrite.Location = new System.Drawing.Point(115, 20);
            this.btnPortWrite.Name = "btnPortWrite";
            this.btnPortWrite.Size = new System.Drawing.Size(72, 23);
            this.btnPortWrite.TabIndex = 8;
            this.btnPortWrite.Text = "Write";
            this.btnPortWrite.UseVisualStyleBackColor = false;
            this.btnPortWrite.Click += new System.EventHandler(this.btnPortWrite_Click);
            // 
            // tbPortUpdateByte
            // 
            this.tbPortUpdateByte.Location = new System.Drawing.Point(79, 22);
            this.tbPortUpdateByte.Name = "tbPortUpdateByte";
            this.tbPortUpdateByte.Size = new System.Drawing.Size(30, 20);
            this.tbPortUpdateByte.TabIndex = 0;
            this.tbPortUpdateByte.Text = "00";
            this.tbPortUpdateByte.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panelPortInfo
            // 
            this.panelPortInfo.BackColor = System.Drawing.SystemColors.Info;
            this.panelPortInfo.Location = new System.Drawing.Point(6, 3);
            this.panelPortInfo.Name = "panelPortInfo";
            this.panelPortInfo.Size = new System.Drawing.Size(534, 348);
            this.panelPortInfo.TabIndex = 8;
            // 
            // btnClearPORT
            // 
            this.btnClearPORT.BackColor = System.Drawing.Color.LightGreen;
            this.btnClearPORT.Location = new System.Drawing.Point(439, 357);
            this.btnClearPORT.Name = "btnClearPORT";
            this.btnClearPORT.Size = new System.Drawing.Size(101, 23);
            this.btnClearPORT.TabIndex = 7;
            this.btnClearPORT.Text = "Clear Ports";
            this.btnClearPORT.UseVisualStyleBackColor = false;
            this.btnClearPORT.Click += new System.EventHandler(this.btnClearPORT_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripButtonOpen,
            this.toolStripButtonSave,
            this.toolStripButtonSaveAs,
            this.toolStripSeparator1,
            this.toolStripButtonRestartSimulator,
            this.toolStripSeparator2,
            this.toolStripButtonStartDebug,
            this.toolStripButtonRun,
            this.toolStripButtonFast,
            this.toolStripButtonStep,
            this.toolStripButtonStop});
            this.toolStrip.Location = new System.Drawing.Point(249, 9);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(245, 25);
            this.toolStrip.TabIndex = 18;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.BackColor = System.Drawing.Color.Transparent;
            this.toolStripButtonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNew.Image")));
            this.toolStripButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNew.Text = "New File";
            this.toolStripButtonNew.Click += new System.EventHandler(this.new_Click);
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.Image = global::Z80_RC2014.Properties.Resources.open;
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpen.ToolTipText = "Open File";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.open_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = global::Z80_RC2014.Properties.Resources.save;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.Click += new System.EventHandler(this.save_Click);
            // 
            // toolStripButtonSaveAs
            // 
            this.toolStripButtonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveAs.Image = global::Z80_RC2014.Properties.Resources.save_as;
            this.toolStripButtonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveAs.Name = "toolStripButtonSaveAs";
            this.toolStripButtonSaveAs.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveAs.Text = "Save As";
            this.toolStripButtonSaveAs.Click += new System.EventHandler(this.saveAs_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRestartSimulator
            // 
            this.toolStripButtonRestartSimulator.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRestartSimulator.Image = global::Z80_RC2014.Properties.Resources.reset;
            this.toolStripButtonRestartSimulator.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRestartSimulator.Name = "toolStripButtonRestartSimulator";
            this.toolStripButtonRestartSimulator.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRestartSimulator.Text = "Reset Simulator";
            this.toolStripButtonRestartSimulator.Click += new System.EventHandler(this.resetSimulator_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonStartDebug
            // 
            this.toolStripButtonStartDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStartDebug.Image = global::Z80_RC2014.Properties.Resources.debug;
            this.toolStripButtonStartDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStartDebug.Name = "toolStripButtonStartDebug";
            this.toolStripButtonStartDebug.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStartDebug.Text = "Start Debugging";
            this.toolStripButtonStartDebug.Click += new System.EventHandler(this.startDebug_Click);
            // 
            // toolStripButtonRun
            // 
            this.toolStripButtonRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRun.Image = global::Z80_RC2014.Properties.Resources.play;
            this.toolStripButtonRun.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripButtonRun.Name = "toolStripButtonRun";
            this.toolStripButtonRun.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRun.Text = "Run";
            this.toolStripButtonRun.Click += new System.EventHandler(this.startRun_Click);
            // 
            // toolStripButtonFast
            // 
            this.toolStripButtonFast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFast.Image = global::Z80_RC2014.Properties.Resources.fast;
            this.toolStripButtonFast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFast.Name = "toolStripButtonFast";
            this.toolStripButtonFast.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFast.Text = "Fast";
            this.toolStripButtonFast.Click += new System.EventHandler(this.startFast_Click);
            // 
            // toolStripButtonStep
            // 
            this.toolStripButtonStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStep.Image = global::Z80_RC2014.Properties.Resources.step;
            this.toolStripButtonStep.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonStep.Name = "toolStripButtonStep";
            this.toolStripButtonStep.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStep.Text = "Step";
            this.toolStripButtonStep.Click += new System.EventHandler(this.startStep_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStop.Image = global::Z80_RC2014.Properties.Resources.stop;
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStop.Text = "Stop";
            this.toolStripButtonStop.ToolTipText = "Stop";
            this.toolStripButtonStop.Click += new System.EventHandler(this.stop_Click);
            // 
            // lblLine
            // 
            this.lblLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLine.AutoSize = true;
            this.lblLine.Location = new System.Drawing.Point(272, 964);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(13, 13);
            this.lblLine.TabIndex = 20;
            this.lblLine.Text = "0";
            // 
            // lblColumn
            // 
            this.lblColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblColumn.AutoSize = true;
            this.lblColumn.Location = new System.Drawing.Point(314, 964);
            this.lblColumn.Name = "lblColumn";
            this.lblColumn.Size = new System.Drawing.Size(13, 13);
            this.lblColumn.TabIndex = 21;
            this.lblColumn.Text = "0";
            // 
            // btnViewProgram
            // 
            this.btnViewProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewProgram.Location = new System.Drawing.Point(1018, 12);
            this.btnViewProgram.Name = "btnViewProgram";
            this.btnViewProgram.Size = new System.Drawing.Size(85, 23);
            this.btnViewProgram.TabIndex = 22;
            this.btnViewProgram.Text = "View Program";
            this.btnViewProgram.UseVisualStyleBackColor = true;
            this.btnViewProgram.Click += new System.EventHandler(this.btnViewProgram_Click);
            // 
            // btnViewSymbolTable
            // 
            this.btnViewSymbolTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewSymbolTable.Location = new System.Drawing.Point(907, 12);
            this.btnViewSymbolTable.Name = "btnViewSymbolTable";
            this.btnViewSymbolTable.Size = new System.Drawing.Size(105, 23);
            this.btnViewSymbolTable.TabIndex = 23;
            this.btnViewSymbolTable.Text = "View Symbol Table";
            this.btnViewSymbolTable.UseVisualStyleBackColor = true;
            this.btnViewSymbolTable.Click += new System.EventHandler(this.btnViewSymbolTable_Click);
            // 
            // panelMemory
            // 
            this.panelMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMemory.BackColor = System.Drawing.Color.LightGray;
            this.panelMemory.Controls.Add(this.btnViewSP);
            this.panelMemory.Controls.Add(this.btnViewPC);
            this.panelMemory.Controls.Add(this.chkLock);
            this.panelMemory.Controls.Add(this.panelWriteMemory);
            this.panelMemory.Controls.Add(this.panelMemoryInfo);
            this.panelMemory.Controls.Add(this.lblAddress);
            this.panelMemory.Controls.Add(this.btnPrevPage);
            this.panelMemory.Controls.Add(this.tbMemoryStartAddress);
            this.panelMemory.Controls.Add(this.btnNextPage);
            this.panelMemory.Controls.Add(this.btnMemoryStartAddress);
            this.panelMemory.Location = new System.Drawing.Point(5, 36);
            this.panelMemory.Name = "panelMemory";
            this.panelMemory.Size = new System.Drawing.Size(546, 439);
            this.panelMemory.TabIndex = 24;
            // 
            // btnViewSP
            // 
            this.btnViewSP.BackColor = System.Drawing.Color.LightGreen;
            this.btnViewSP.Location = new System.Drawing.Point(321, 6);
            this.btnViewSP.Name = "btnViewSP";
            this.btnViewSP.Size = new System.Drawing.Size(119, 23);
            this.btnViewSP.TabIndex = 12;
            this.btnViewSP.Text = "View StackPointer";
            this.btnViewSP.UseVisualStyleBackColor = false;
            this.btnViewSP.Click += new System.EventHandler(this.btnViewSP_Click);
            // 
            // btnViewPC
            // 
            this.btnViewPC.BackColor = System.Drawing.Color.LightGreen;
            this.btnViewPC.Location = new System.Drawing.Point(196, 6);
            this.btnViewPC.Name = "btnViewPC";
            this.btnViewPC.Size = new System.Drawing.Size(119, 23);
            this.btnViewPC.TabIndex = 11;
            this.btnViewPC.Text = "View ProgramCounter";
            this.btnViewPC.UseVisualStyleBackColor = false;
            this.btnViewPC.Click += new System.EventHandler(this.btnViewPC_Click);
            // 
            // chkLock
            // 
            this.chkLock.AutoSize = true;
            this.chkLock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLock.Location = new System.Drawing.Point(481, 8);
            this.chkLock.Name = "chkLock";
            this.chkLock.Size = new System.Drawing.Size(59, 20);
            this.chkLock.TabIndex = 10;
            this.chkLock.Text = "Lock";
            this.chkLock.UseVisualStyleBackColor = true;
            // 
            // panelWriteMemory
            // 
            this.panelWriteMemory.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panelWriteMemory.Controls.Add(this.lblValueMemory);
            this.panelWriteMemory.Controls.Add(this.btnMemoryWrite);
            this.panelWriteMemory.Controls.Add(this.lblMemoryAddress);
            this.panelWriteMemory.Controls.Add(this.tbMemoryUpdateByte);
            this.panelWriteMemory.Controls.Add(this.numMemoryAddress);
            this.panelWriteMemory.Location = new System.Drawing.Point(6, 386);
            this.panelWriteMemory.Name = "panelWriteMemory";
            this.panelWriteMemory.Size = new System.Drawing.Size(199, 47);
            this.panelWriteMemory.TabIndex = 9;
            // 
            // panelPorts
            // 
            this.panelPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPorts.BackColor = System.Drawing.Color.LightGray;
            this.panelPorts.Controls.Add(this.panelUpdatePort);
            this.panelPorts.Controls.Add(this.panelPortInfo);
            this.panelPorts.Controls.Add(this.btnClearPORT);
            this.panelPorts.Location = new System.Drawing.Point(5, 479);
            this.panelPorts.Name = "panelPorts";
            this.panelPorts.Size = new System.Drawing.Size(546, 410);
            this.panelPorts.TabIndex = 25;
            // 
            // panelUpdatePort
            // 
            this.panelUpdatePort.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panelUpdatePort.Controls.Add(this.lblValuePort);
            this.panelUpdatePort.Controls.Add(this.lblPort);
            this.panelUpdatePort.Controls.Add(this.numPort);
            this.panelUpdatePort.Controls.Add(this.btnPortWrite);
            this.panelUpdatePort.Controls.Add(this.tbPortUpdateByte);
            this.panelUpdatePort.Location = new System.Drawing.Point(6, 357);
            this.panelUpdatePort.Name = "panelUpdatePort";
            this.panelUpdatePort.Size = new System.Drawing.Size(199, 48);
            this.panelUpdatePort.TabIndex = 12;
            // 
            // lblValuePort
            // 
            this.lblValuePort.AutoSize = true;
            this.lblValuePort.Location = new System.Drawing.Point(76, 5);
            this.lblValuePort.Name = "lblValuePort";
            this.lblValuePort.Size = new System.Drawing.Size(34, 13);
            this.lblValuePort.TabIndex = 12;
            this.lblValuePort.Text = "Value";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(3, 5);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 11;
            this.lblPort.Text = "Port";
            // 
            // richTextBoxProgram
            // 
            this.richTextBoxProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxProgram.BackColor = System.Drawing.Color.White;
            this.richTextBoxProgram.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxProgram.Location = new System.Drawing.Point(6, 6);
            this.richTextBoxProgram.Name = "richTextBoxProgram";
            this.richTextBoxProgram.Size = new System.Drawing.Size(443, 882);
            this.richTextBoxProgram.TabIndex = 5;
            this.richTextBoxProgram.Text = "";
            this.richTextBoxProgram.WordWrap = false;
            this.richTextBoxProgram.SelectionChanged += new System.EventHandler(this.richTextBox_SelectionChanged);
            this.richTextBoxProgram.VScroll += new System.EventHandler(this.richTextBoxProgram_VScroll);
            this.richTextBoxProgram.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            this.richTextBoxProgram.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBoxProgram_KeyDown);
            this.richTextBoxProgram.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richTextBoxProgram_KeyPress);
            this.richTextBoxProgram.MouseDown += new System.Windows.Forms.MouseEventHandler(this.richTextBoxProgram_MouseDown);
            this.richTextBoxProgram.MouseEnter += new System.EventHandler(this.richTextBox_MouseEnter);
            this.richTextBoxProgram.MouseLeave += new System.EventHandler(this.richTextBox_MouseLeave);
            this.richTextBoxProgram.MouseMove += new System.Windows.Forms.MouseEventHandler(this.richTextBoxProgram_MouseMove);
            // 
            // btnClearBreakPoint
            // 
            this.btnClearBreakPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearBreakPoint.Location = new System.Drawing.Point(807, 13);
            this.btnClearBreakPoint.Name = "btnClearBreakPoint";
            this.btnClearBreakPoint.Size = new System.Drawing.Size(94, 22);
            this.btnClearBreakPoint.TabIndex = 27;
            this.btnClearBreakPoint.Text = "Clear Breakpoint";
            this.btnClearBreakPoint.UseVisualStyleBackColor = true;
            this.btnClearBreakPoint.Click += new System.EventHandler(this.btnClearBreakPoint_Click);
            // 
            // pbBreakPoint
            // 
            this.pbBreakPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pbBreakPoint.BackColor = System.Drawing.Color.LightGray;
            this.pbBreakPoint.Location = new System.Drawing.Point(314, 69);
            this.pbBreakPoint.Name = "pbBreakPoint";
            this.pbBreakPoint.Size = new System.Drawing.Size(18, 880);
            this.pbBreakPoint.TabIndex = 28;
            this.pbBreakPoint.TabStop = false;
            this.pbBreakPoint.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbBreakPoint_MouseClick);
            // 
            // numericUpDownDelay
            // 
            this.numericUpDownDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownDelay.Location = new System.Drawing.Point(1317, 9);
            this.numericUpDownDelay.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownDelay.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownDelay.Name = "numericUpDownDelay";
            this.numericUpDownDelay.Size = new System.Drawing.Size(55, 24);
            this.numericUpDownDelay.TabIndex = 29;
            this.numericUpDownDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownDelay.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownDelay.ValueChanged += new System.EventHandler(this.numericUpDownDelay_ValueChanged);
            // 
            // lblDelay
            // 
            this.lblDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDelay.AutoSize = true;
            this.lblDelay.Location = new System.Drawing.Point(1252, 16);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(59, 13);
            this.lblDelay.TabIndex = 30;
            this.lblDelay.Text = "Delay (ms):";
            // 
            // lblSetProgramCounter
            // 
            this.lblSetProgramCounter.AutoSize = true;
            this.lblSetProgramCounter.Location = new System.Drawing.Point(497, 9);
            this.lblSetProgramCounter.Name = "lblSetProgramCounter";
            this.lblSetProgramCounter.Size = new System.Drawing.Size(105, 13);
            this.lblSetProgramCounter.TabIndex = 31;
            this.lblSetProgramCounter.Text = "Set Program Counter";
            // 
            // tbSetProgramCounter
            // 
            this.tbSetProgramCounter.Location = new System.Drawing.Point(608, 5);
            this.tbSetProgramCounter.MaxLength = 4;
            this.tbSetProgramCounter.Name = "tbSetProgramCounter";
            this.tbSetProgramCounter.Size = new System.Drawing.Size(40, 20);
            this.tbSetProgramCounter.TabIndex = 32;
            this.tbSetProgramCounter.Text = "0000";
            this.tbSetProgramCounter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSetProgramCounter.TextChanged += new System.EventHandler(this.tbAddress_TextChanged);
            this.tbSetProgramCounter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSetProgramCounter_KeyPress);
            // 
            // chkTerminal
            // 
            this.chkTerminal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTerminal.AutoSize = true;
            this.chkTerminal.Checked = true;
            this.chkTerminal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTerminal.Location = new System.Drawing.Point(1119, 16);
            this.chkTerminal.Name = "chkTerminal";
            this.chkTerminal.Size = new System.Drawing.Size(66, 17);
            this.chkTerminal.TabIndex = 33;
            this.chkTerminal.Text = "Terminal";
            this.chkTerminal.UseVisualStyleBackColor = true;
            this.chkTerminal.CheckedChanged += new System.EventHandler(this.chkTerminal_CheckedChanged);
            // 
            // panelInterrupt
            // 
            this.panelInterrupt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInterrupt.BackColor = System.Drawing.Color.LightGray;
            this.panelInterrupt.Controls.Add(this.lblIM);
            this.panelInterrupt.Controls.Add(this.lblInterrupts);
            this.panelInterrupt.Location = new System.Drawing.Point(5, 4);
            this.panelInterrupt.Name = "panelInterrupt";
            this.panelInterrupt.Size = new System.Drawing.Size(546, 26);
            this.panelInterrupt.TabIndex = 35;
            // 
            // lblIM
            // 
            this.lblIM.AutoSize = true;
            this.lblIM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIM.Location = new System.Drawing.Point(402, 6);
            this.lblIM.Name = "lblIM";
            this.lblIM.Size = new System.Drawing.Size(138, 16);
            this.lblIM.TabIndex = 32;
            this.lblIM.Text = "Interrupt Mode: im0";
            // 
            // lblInterrupts
            // 
            this.lblInterrupts.AutoSize = true;
            this.lblInterrupts.BackColor = System.Drawing.Color.LightPink;
            this.lblInterrupts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInterrupts.Location = new System.Drawing.Point(3, 3);
            this.lblInterrupts.Name = "lblInterrupts";
            this.lblInterrupts.Size = new System.Drawing.Size(78, 20);
            this.lblInterrupts.TabIndex = 0;
            this.lblInterrupts.Text = "Interrupts";
            // 
            // tcInstructions
            // 
            this.tcInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tcInstructions.Controls.Add(this.tabPage1);
            this.tcInstructions.Controls.Add(this.tabPage2);
            this.tcInstructions.Controls.Add(this.tabPage3);
            this.tcInstructions.Controls.Add(this.tabPage4);
            this.tcInstructions.Controls.Add(this.tabPage5);
            this.tcInstructions.Controls.Add(this.tabPage6);
            this.tcInstructions.Controls.Add(this.tabPage7);
            this.tcInstructions.ItemSize = new System.Drawing.Size(42, 18);
            this.tcInstructions.Location = new System.Drawing.Point(12, 322);
            this.tcInstructions.Name = "tcInstructions";
            this.tcInstructions.SelectedIndex = 0;
            this.tcInstructions.Size = new System.Drawing.Size(300, 643);
            this.tcInstructions.TabIndex = 36;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(292, 617);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(292, 617);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Bit";
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(292, 617);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "IX";
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(292, 617);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "IY";
            // 
            // tabPage5
            // 
            this.tabPage5.AutoScroll = true;
            this.tabPage5.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(292, 617);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Misc.";
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(292, 617);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "BitIX";
            // 
            // tabPage7
            // 
            this.tabPage7.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(292, 617);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "BitIY";
            // 
            // groupBoxAltFlags
            // 
            this.groupBoxAltFlags.BackColor = System.Drawing.SystemColors.Info;
            this.groupBoxAltFlags.Controls.Add(this.chkFlagNalt);
            this.groupBoxAltFlags.Controls.Add(this.lblFlagNalt);
            this.groupBoxAltFlags.Controls.Add(this.chkFlagPValt);
            this.groupBoxAltFlags.Controls.Add(this.lblFlagPValt);
            this.groupBoxAltFlags.Controls.Add(this.chkFlagCalt);
            this.groupBoxAltFlags.Controls.Add(this.chkFlagHalt);
            this.groupBoxAltFlags.Controls.Add(this.chkFlagZalt);
            this.groupBoxAltFlags.Controls.Add(this.chkFlagSalt);
            this.groupBoxAltFlags.Controls.Add(this.lblFlagCalt);
            this.groupBoxAltFlags.Controls.Add(this.lblFlagHalt);
            this.groupBoxAltFlags.Controls.Add(this.lblFlagZalt);
            this.groupBoxAltFlags.Controls.Add(this.lblFlagSalt);
            this.groupBoxAltFlags.Location = new System.Drawing.Point(12, 263);
            this.groupBoxAltFlags.Name = "groupBoxAltFlags";
            this.groupBoxAltFlags.Size = new System.Drawing.Size(296, 53);
            this.groupBoxAltFlags.TabIndex = 37;
            this.groupBoxAltFlags.TabStop = false;
            this.groupBoxAltFlags.Text = "Flags\'";
            // 
            // chkFlagNalt
            // 
            this.chkFlagNalt.AutoSize = true;
            this.chkFlagNalt.Location = new System.Drawing.Point(220, 33);
            this.chkFlagNalt.Name = "chkFlagNalt";
            this.chkFlagNalt.Size = new System.Drawing.Size(15, 14);
            this.chkFlagNalt.TabIndex = 24;
            this.chkFlagNalt.UseVisualStyleBackColor = true;
            // 
            // lblFlagNalt
            // 
            this.lblFlagNalt.AutoSize = true;
            this.lblFlagNalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagNalt.Location = new System.Drawing.Point(220, 16);
            this.lblFlagNalt.Name = "lblFlagNalt";
            this.lblFlagNalt.Size = new System.Drawing.Size(19, 13);
            this.lblFlagNalt.TabIndex = 23;
            this.lblFlagNalt.Text = "N\'";
            // 
            // chkFlagPValt
            // 
            this.chkFlagPValt.AutoSize = true;
            this.chkFlagPValt.Location = new System.Drawing.Point(165, 33);
            this.chkFlagPValt.Name = "chkFlagPValt";
            this.chkFlagPValt.Size = new System.Drawing.Size(15, 14);
            this.chkFlagPValt.TabIndex = 20;
            this.chkFlagPValt.UseVisualStyleBackColor = true;
            // 
            // lblFlagPValt
            // 
            this.lblFlagPValt.AutoSize = true;
            this.lblFlagPValt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagPValt.Location = new System.Drawing.Point(165, 17);
            this.lblFlagPValt.Name = "lblFlagPValt";
            this.lblFlagPValt.Size = new System.Drawing.Size(32, 13);
            this.lblFlagPValt.TabIndex = 19;
            this.lblFlagPValt.Text = "P/V\'";
            // 
            // chkFlagCalt
            // 
            this.chkFlagCalt.AutoSize = true;
            this.chkFlagCalt.Location = new System.Drawing.Point(270, 33);
            this.chkFlagCalt.Name = "chkFlagCalt";
            this.chkFlagCalt.Size = new System.Drawing.Size(15, 14);
            this.chkFlagCalt.TabIndex = 18;
            this.chkFlagCalt.UseVisualStyleBackColor = true;
            // 
            // chkFlagHalt
            // 
            this.chkFlagHalt.AutoSize = true;
            this.chkFlagHalt.Location = new System.Drawing.Point(114, 33);
            this.chkFlagHalt.Name = "chkFlagHalt";
            this.chkFlagHalt.Size = new System.Drawing.Size(15, 14);
            this.chkFlagHalt.TabIndex = 16;
            this.chkFlagHalt.UseVisualStyleBackColor = true;
            // 
            // chkFlagZalt
            // 
            this.chkFlagZalt.AutoSize = true;
            this.chkFlagZalt.Location = new System.Drawing.Point(62, 33);
            this.chkFlagZalt.Name = "chkFlagZalt";
            this.chkFlagZalt.Size = new System.Drawing.Size(15, 14);
            this.chkFlagZalt.TabIndex = 15;
            this.chkFlagZalt.UseVisualStyleBackColor = true;
            // 
            // chkFlagSalt
            // 
            this.chkFlagSalt.AutoSize = true;
            this.chkFlagSalt.Location = new System.Drawing.Point(7, 33);
            this.chkFlagSalt.Name = "chkFlagSalt";
            this.chkFlagSalt.Size = new System.Drawing.Size(15, 14);
            this.chkFlagSalt.TabIndex = 10;
            this.chkFlagSalt.UseVisualStyleBackColor = true;
            // 
            // lblFlagCalt
            // 
            this.lblFlagCalt.AutoSize = true;
            this.lblFlagCalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagCalt.Location = new System.Drawing.Point(270, 17);
            this.lblFlagCalt.Name = "lblFlagCalt";
            this.lblFlagCalt.Size = new System.Drawing.Size(18, 13);
            this.lblFlagCalt.TabIndex = 7;
            this.lblFlagCalt.Text = "C\'";
            // 
            // lblFlagHalt
            // 
            this.lblFlagHalt.AutoSize = true;
            this.lblFlagHalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagHalt.Location = new System.Drawing.Point(111, 17);
            this.lblFlagHalt.Name = "lblFlagHalt";
            this.lblFlagHalt.Size = new System.Drawing.Size(19, 13);
            this.lblFlagHalt.TabIndex = 3;
            this.lblFlagHalt.Text = "H\'";
            // 
            // lblFlagZalt
            // 
            this.lblFlagZalt.AutoSize = true;
            this.lblFlagZalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagZalt.Location = new System.Drawing.Point(62, 16);
            this.lblFlagZalt.Name = "lblFlagZalt";
            this.lblFlagZalt.Size = new System.Drawing.Size(18, 13);
            this.lblFlagZalt.TabIndex = 1;
            this.lblFlagZalt.Text = "Z\'";
            // 
            // lblFlagSalt
            // 
            this.lblFlagSalt.AutoSize = true;
            this.lblFlagSalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFlagSalt.Location = new System.Drawing.Point(7, 16);
            this.lblFlagSalt.Name = "lblFlagSalt";
            this.lblFlagSalt.Size = new System.Drawing.Size(18, 13);
            this.lblFlagSalt.TabIndex = 0;
            this.lblFlagSalt.Text = "S\'";
            // 
            // chkBreakOnExternalCode
            // 
            this.chkBreakOnExternalCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBreakOnExternalCode.AutoSize = true;
            this.chkBreakOnExternalCode.Location = new System.Drawing.Point(654, 7);
            this.chkBreakOnExternalCode.Name = "chkBreakOnExternalCode";
            this.chkBreakOnExternalCode.Size = new System.Drawing.Size(137, 17);
            this.chkBreakOnExternalCode.TabIndex = 38;
            this.chkBreakOnExternalCode.Text = "Break on external Code";
            this.chkBreakOnExternalCode.UseVisualStyleBackColor = true;
            // 
            // tcSources
            // 
            this.tcSources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcSources.Controls.Add(this.tpProgram);
            this.tcSources.Controls.Add(this.tpCompactFlash);
            this.tcSources.Location = new System.Drawing.Point(338, 41);
            this.tcSources.Name = "tcSources";
            this.tcSources.SelectedIndex = 0;
            this.tcSources.Size = new System.Drawing.Size(463, 920);
            this.tcSources.TabIndex = 39;
            this.tcSources.SelectedIndexChanged += new System.EventHandler(this.tcSources_SelectedIndexChanged);
            // 
            // tpProgram
            // 
            this.tpProgram.BackColor = System.Drawing.SystemColors.Control;
            this.tpProgram.Controls.Add(this.richTextBoxProgram);
            this.tpProgram.Location = new System.Drawing.Point(4, 22);
            this.tpProgram.Name = "tpProgram";
            this.tpProgram.Padding = new System.Windows.Forms.Padding(3);
            this.tpProgram.Size = new System.Drawing.Size(455, 894);
            this.tpProgram.TabIndex = 0;
            this.tpProgram.Text = "Program";
            // 
            // tpCompactFlash
            // 
            this.tpCompactFlash.BackColor = System.Drawing.SystemColors.Control;
            this.tpCompactFlash.Controls.Add(this.richTextBoxCompactFlash);
            this.tpCompactFlash.Location = new System.Drawing.Point(4, 22);
            this.tpCompactFlash.Name = "tpCompactFlash";
            this.tpCompactFlash.Padding = new System.Windows.Forms.Padding(3);
            this.tpCompactFlash.Size = new System.Drawing.Size(455, 894);
            this.tpCompactFlash.TabIndex = 1;
            this.tpCompactFlash.Text = "Compact Flash";
            // 
            // richTextBoxCompactFlash
            // 
            this.richTextBoxCompactFlash.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxCompactFlash.BackColor = System.Drawing.Color.White;
            this.richTextBoxCompactFlash.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxCompactFlash.Location = new System.Drawing.Point(6, 6);
            this.richTextBoxCompactFlash.Name = "richTextBoxCompactFlash";
            this.richTextBoxCompactFlash.Size = new System.Drawing.Size(443, 882);
            this.richTextBoxCompactFlash.TabIndex = 6;
            this.richTextBoxCompactFlash.Text = "";
            this.richTextBoxCompactFlash.WordWrap = false;
            this.richTextBoxCompactFlash.SelectionChanged += new System.EventHandler(this.richTextBox_SelectionChanged);
            this.richTextBoxCompactFlash.VScroll += new System.EventHandler(this.richTextBoxCompactFlash_VScroll);
            this.richTextBoxCompactFlash.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            this.richTextBoxCompactFlash.MouseDown += new System.Windows.Forms.MouseEventHandler(this.richTextBoxCompactFlash_MouseDown);
            this.richTextBoxCompactFlash.MouseEnter += new System.EventHandler(this.richTextBox_MouseEnter);
            this.richTextBoxCompactFlash.MouseLeave += new System.EventHandler(this.richTextBox_MouseLeave);
            this.richTextBoxCompactFlash.MouseMove += new System.Windows.Forms.MouseEventHandler(this.richTextBoxCompactFlash_MouseMove);
            // 
            // tcMemoryCompactFlash
            // 
            this.tcMemoryCompactFlash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMemoryCompactFlash.Controls.Add(this.tpMemory);
            this.tcMemoryCompactFlash.Controls.Add(this.tpCFCard);
            this.tcMemoryCompactFlash.Location = new System.Drawing.Point(807, 41);
            this.tcMemoryCompactFlash.Name = "tcMemoryCompactFlash";
            this.tcMemoryCompactFlash.SelectedIndex = 0;
            this.tcMemoryCompactFlash.Size = new System.Drawing.Size(565, 920);
            this.tcMemoryCompactFlash.TabIndex = 40;
            // 
            // tpMemory
            // 
            this.tpMemory.Controls.Add(this.panelInterrupt);
            this.tpMemory.Controls.Add(this.panelPorts);
            this.tpMemory.Controls.Add(this.panelMemory);
            this.tpMemory.Location = new System.Drawing.Point(4, 22);
            this.tpMemory.Name = "tpMemory";
            this.tpMemory.Padding = new System.Windows.Forms.Padding(3);
            this.tpMemory.Size = new System.Drawing.Size(557, 894);
            this.tpMemory.TabIndex = 0;
            this.tpMemory.Text = "Memory";
            this.tpMemory.UseVisualStyleBackColor = true;
            // 
            // tpCFCard
            // 
            this.tpCFCard.Controls.Add(this.compactFlash);
            this.tpCFCard.Location = new System.Drawing.Point(4, 22);
            this.tpCFCard.Name = "tpCFCard";
            this.tpCFCard.Padding = new System.Windows.Forms.Padding(3);
            this.tpCFCard.Size = new System.Drawing.Size(557, 894);
            this.tpCFCard.TabIndex = 1;
            this.tpCFCard.Text = "Compact Flash";
            this.tpCFCard.UseVisualStyleBackColor = true;
            // 
            // compactFlash
            // 
            this.compactFlash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.compactFlash.Location = new System.Drawing.Point(8, 4);
            this.compactFlash.Name = "compactFlash";
            this.compactFlash.Size = new System.Drawing.Size(543, 882);
            this.compactFlash.TabIndex = 7;
            // 
            // chkShowExternalCode
            // 
            this.chkShowExternalCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowExternalCode.AutoSize = true;
            this.chkShowExternalCode.Checked = true;
            this.chkShowExternalCode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowExternalCode.Location = new System.Drawing.Point(500, 31);
            this.chkShowExternalCode.Name = "chkShowExternalCode";
            this.chkShowExternalCode.Size = new System.Drawing.Size(121, 17);
            this.chkShowExternalCode.TabIndex = 41;
            this.chkShowExternalCode.Text = "Show external Code";
            this.chkShowExternalCode.UseVisualStyleBackColor = true;
            // 
            // chkBreakOnAddress
            // 
            this.chkBreakOnAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBreakOnAddress.AutoSize = true;
            this.chkBreakOnAddress.Location = new System.Drawing.Point(654, 29);
            this.chkBreakOnAddress.Name = "chkBreakOnAddress";
            this.chkBreakOnAddress.Size = new System.Drawing.Size(89, 17);
            this.chkBreakOnAddress.TabIndex = 42;
            this.chkBreakOnAddress.Text = "Break on PC:";
            this.chkBreakOnAddress.UseVisualStyleBackColor = true;
            // 
            // tbBreakOnAddress
            // 
            this.tbBreakOnAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBreakOnAddress.Location = new System.Drawing.Point(749, 27);
            this.tbBreakOnAddress.MaxLength = 4;
            this.tbBreakOnAddress.Name = "tbBreakOnAddress";
            this.tbBreakOnAddress.Size = new System.Drawing.Size(42, 20);
            this.tbBreakOnAddress.TabIndex = 43;
            this.tbBreakOnAddress.Text = "0000";
            this.tbBreakOnAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbBreakOnAddress.TextChanged += new System.EventHandler(this.tbAddress_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 981);
            this.Controls.Add(this.tbBreakOnAddress);
            this.Controls.Add(this.chkBreakOnAddress);
            this.Controls.Add(this.chkShowExternalCode);
            this.Controls.Add(this.tcMemoryCompactFlash);
            this.Controls.Add(this.tcSources);
            this.Controls.Add(this.chkBreakOnExternalCode);
            this.Controls.Add(this.groupBoxAltFlags);
            this.Controls.Add(this.tcInstructions);
            this.Controls.Add(this.chkTerminal);
            this.Controls.Add(this.lblSetProgramCounter);
            this.Controls.Add(this.tbSetProgramCounter);
            this.Controls.Add(this.lblDelay);
            this.Controls.Add(this.numericUpDownDelay);
            this.Controls.Add(this.pbBreakPoint);
            this.Controls.Add(this.btnClearBreakPoint);
            this.Controls.Add(this.btnViewSymbolTable);
            this.Controls.Add(this.btnViewProgram);
            this.Controls.Add(this.lblColumn);
            this.Controls.Add(this.lblLine);
            this.Controls.Add(this.groupBoxFlags);
            this.Controls.Add(this.groupBoxRegisters);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1400, 1020);
            this.Name = "MainForm";
            this.Text = "Z80 Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.groupBoxFlags.ResumeLayout(false);
            this.groupBoxFlags.PerformLayout();
            this.groupBoxRegisters.ResumeLayout(false);
            this.groupBoxRegisters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMemoryAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panelMemory.ResumeLayout(false);
            this.panelMemory.PerformLayout();
            this.panelWriteMemory.ResumeLayout(false);
            this.panelWriteMemory.PerformLayout();
            this.panelPorts.ResumeLayout(false);
            this.panelUpdatePort.ResumeLayout(false);
            this.panelUpdatePort.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBreakPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDelay)).EndInit();
            this.panelInterrupt.ResumeLayout(false);
            this.panelInterrupt.PerformLayout();
            this.tcInstructions.ResumeLayout(false);
            this.groupBoxAltFlags.ResumeLayout(false);
            this.groupBoxAltFlags.PerformLayout();
            this.tcSources.ResumeLayout(false);
            this.tpProgram.ResumeLayout(false);
            this.tpCompactFlash.ResumeLayout(false);
            this.tcMemoryCompactFlash.ResumeLayout(false);
            this.tpMemory.ResumeLayout(false);
            this.tpCFCard.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetRAMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetPortsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem resetSimulatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxFlags;
        private System.Windows.Forms.Label lblFlagC;
        private System.Windows.Forms.Label lblFlagH;
        private System.Windows.Forms.Label lblFlagZ;
        private System.Windows.Forms.Label lblFlagS;
        private System.Windows.Forms.GroupBox groupBoxRegisters;
        private System.Windows.Forms.Label labelSPRegister;
        private System.Windows.Forms.Label labelPCRegister;
        private System.Windows.Forms.Label labelLRegister;
        private System.Windows.Forms.Label labelHRegister;
        private System.Windows.Forms.Label labelERegister;
        private System.Windows.Forms.Label labelDRegister;
        private System.Windows.Forms.Label labelCRegister;
        private System.Windows.Forms.Label labelBRegister;
        private System.Windows.Forms.Label labelARegister;
        private System.Windows.Forms.Label lblSP;
        private System.Windows.Forms.Label lblPC;
        private System.Windows.Forms.Label lblHL;
        private System.Windows.Forms.Label lblE;
        private System.Windows.Forms.Label lblD;
        private System.Windows.Forms.Label lblC;
        private System.Windows.Forms.Label lblB;
        private System.Windows.Forms.Label lblA;
        private System.Windows.Forms.Button btnMemoryWrite;
        private System.Windows.Forms.NumericUpDown numMemoryAddress;
        private System.Windows.Forms.TextBox tbMemoryUpdateByte;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.Panel panelMemoryInfo;
        private System.Windows.Forms.Button btnMemoryStartAddress;
        private System.Windows.Forms.TextBox tbMemoryStartAddress;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Button btnPortWrite;
        private System.Windows.Forms.TextBox tbPortUpdateByte;
        private System.Windows.Forms.Panel panelPortInfo;
        private System.Windows.Forms.Button btnClearPORT;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRestartSimulator;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonStartDebug;
        private System.Windows.Forms.ToolStripButton toolStripButtonStep;
        private System.Windows.Forms.Label lblValueMemory;
        private System.Windows.Forms.Label lblMemoryAddress;
        private System.Windows.Forms.ToolStripButton toolStripButtonNew;
        private System.Windows.Forms.ToolTip toolTipRegisterBinary;
        private System.Windows.Forms.Label lblLine;
        private System.Windows.Forms.Label lblColumn;
        private System.Windows.Forms.Button btnViewProgram;
        private System.Windows.Forms.Button btnViewSymbolTable;
        private System.Windows.Forms.CheckBox chkFlagS;
        private System.Windows.Forms.CheckBox chkFlagC;
        private System.Windows.Forms.CheckBox chkFlagH;
        private System.Windows.Forms.CheckBox chkFlagZ;
        private System.Windows.Forms.Panel panelMemory;
        private System.Windows.Forms.Panel panelWriteMemory;
        private System.Windows.Forms.Panel panelPorts;
        private System.Windows.Forms.Panel panelUpdatePort;
        private System.Windows.Forms.Label lblValuePort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.RichTextBox richTextBoxProgram;
        private System.Windows.Forms.ToolStripButton toolStripButtonRun;
        private System.Windows.Forms.Button btnClearBreakPoint;
        private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbBreakPoint;
        private System.Windows.Forms.CheckBox chkFlagPV;
        private System.Windows.Forms.Label lblFlagPV;
        private System.Windows.Forms.ToolStripMenuItem disAssemblerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openBinaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBinaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonStop;
        private System.Windows.Forms.NumericUpDown numericUpDownDelay;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.CheckBox chkLock;
        private System.Windows.Forms.Label lblSetProgramCounter;
        private System.Windows.Forms.TextBox tbSetProgramCounter;
        private System.Windows.Forms.CheckBox chkTerminal;
        private System.Windows.Forms.Panel panelInterrupt;
        private System.Windows.Forms.Label lblInterrupts;
        private System.Windows.Forms.Button btnViewSP;
        private System.Windows.Forms.Button btnViewPC;
        private System.Windows.Forms.TabControl tcInstructions;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label labelIXRegister;
        private System.Windows.Forms.Label lblIX;
        private System.Windows.Forms.Label labelEaltRegister;
        private System.Windows.Forms.Label labelDaltRegister;
        private System.Windows.Forms.Label labelCaltRegister;
        private System.Windows.Forms.Label labelBaltRegister;
        private System.Windows.Forms.Label labelAaltRegister;
        private System.Windows.Forms.Label labelIYRegister;
        private System.Windows.Forms.Label lblIY;
        private System.Windows.Forms.Label labelRRegister;
        private System.Windows.Forms.Label lblR;
        private System.Windows.Forms.Label labelIRegister;
        private System.Windows.Forms.Label lblI;
        private System.Windows.Forms.Label lblAltHL;
        private System.Windows.Forms.Label labelLaltRegister;
        private System.Windows.Forms.Label labelHaltRegister;
        private System.Windows.Forms.GroupBox groupBoxAltFlags;
        private System.Windows.Forms.CheckBox chkFlagPValt;
        private System.Windows.Forms.Label lblFlagPValt;
        private System.Windows.Forms.CheckBox chkFlagCalt;
        private System.Windows.Forms.CheckBox chkFlagHalt;
        private System.Windows.Forms.CheckBox chkFlagZalt;
        private System.Windows.Forms.CheckBox chkFlagSalt;
        private System.Windows.Forms.Label lblFlagCalt;
        private System.Windows.Forms.Label lblFlagHalt;
        private System.Windows.Forms.Label lblFlagZalt;
        private System.Windows.Forms.Label lblFlagSalt;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.CheckBox chkFlagN;
        private System.Windows.Forms.Label lblFlagN;
        private System.Windows.Forms.CheckBox chkFlagNalt;
        private System.Windows.Forms.Label lblFlagNalt;
        private System.Windows.Forms.Label lblIM;
        private System.Windows.Forms.CheckBox chkBreakOnExternalCode;
        private System.Windows.Forms.ToolStripButton toolStripButtonFast;
        private System.Windows.Forms.TabControl tcSources;
        private System.Windows.Forms.TabPage tpProgram;
        private System.Windows.Forms.TabPage tpCompactFlash;
        private System.Windows.Forms.RichTextBox richTextBoxCompactFlash;
        private CompactFlash compactFlash;
        private System.Windows.Forms.TabControl tcMemoryCompactFlash;
        private System.Windows.Forms.TabPage tpMemory;
        private System.Windows.Forms.TabPage tpCFCard;
        private System.Windows.Forms.ToolStripMenuItem appendBinaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadBinaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.CheckBox chkShowExternalCode;
        private System.Windows.Forms.CheckBox chkBreakOnAddress;
        private System.Windows.Forms.TextBox tbBreakOnAddress;
    }
}

