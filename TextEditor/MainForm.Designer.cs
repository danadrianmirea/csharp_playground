namespace TextEditor;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    // Menu strip
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileMenu;
    private System.Windows.Forms.ToolStripMenuItem newMenuItem;
    private System.Windows.Forms.ToolStripMenuItem openMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
    private System.Windows.Forms.ToolStripSeparator fileSep1;
    private System.Windows.Forms.ToolStripMenuItem exitMenuItem;

    private System.Windows.Forms.ToolStripMenuItem editMenu;
    private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
    private System.Windows.Forms.ToolStripSeparator editSep1;
    private System.Windows.Forms.ToolStripMenuItem cutMenuItem;
    private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
    private System.Windows.Forms.ToolStripMenuItem pasteMenuItem;
    private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
    private System.Windows.Forms.ToolStripSeparator editSep2;
    private System.Windows.Forms.ToolStripMenuItem selectAllMenuItem;
    private System.Windows.Forms.ToolStripMenuItem timeDateMenuItem;

    private System.Windows.Forms.ToolStripMenuItem formatMenu;
    private System.Windows.Forms.ToolStripMenuItem wordWrapMenuItem;
    private System.Windows.Forms.ToolStripMenuItem fontMenuItem;

    private System.Windows.Forms.ToolStripMenuItem viewMenu;
    private System.Windows.Forms.ToolStripMenuItem statusBarMenuItem;

    private System.Windows.Forms.ToolStripMenuItem helpMenu;
    private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;

    // Rich text box for editing
    private System.Windows.Forms.RichTextBox txtEditor;

    // Status strip
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel lblLineColumn;
    private System.Windows.Forms.ToolStripStatusLabel lblWordCount;

    // Open/Save file dialogs
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;

    // Font dialog
    private System.Windows.Forms.FontDialog fontDialog;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // MenuStrip
        this.menuStrip = new System.Windows.Forms.MenuStrip();

        // File menu
        this.fileMenu = new System.Windows.Forms.ToolStripMenuItem("&File");
        this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem("&New", null, NewMenuItem_Click, Keys.Control | Keys.N);
        this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Open...", null, OpenMenuItem_Click, Keys.Control | Keys.O);
        this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Save", null, SaveMenuItem_Click, Keys.Control | Keys.S);
        this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem("Save &As...", null, SaveAsMenuItem_Click);
        this.fileSep1 = new System.Windows.Forms.ToolStripSeparator();
        this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem("E&xit", null, ExitMenuItem_Click);

        this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.fileSep1,
            this.exitMenuItem
        });

        // Edit menu
        this.editMenu = new System.Windows.Forms.ToolStripMenuItem("&Edit");
        this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Undo", null, UndoMenuItem_Click, Keys.Control | Keys.Z);
        this.editSep1 = new System.Windows.Forms.ToolStripSeparator();
        this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem("Cu&t", null, CutMenuItem_Click, Keys.Control | Keys.X);
        this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Copy", null, CopyMenuItem_Click, Keys.Control | Keys.C);
        this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Paste", null, PasteMenuItem_Click, Keys.Control | Keys.V);
        this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Delete", null, DeleteMenuItem_Click, Keys.Delete);
        this.editSep2 = new System.Windows.Forms.ToolStripSeparator();
        this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem("Select &All", null, SelectAllMenuItem_Click, Keys.Control | Keys.A);
        this.timeDateMenuItem = new System.Windows.Forms.ToolStripMenuItem("Time/&Date", null, TimeDateMenuItem_Click, Keys.F5);

        this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.editSep1,
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem,
            this.deleteMenuItem,
            this.editSep2,
            this.selectAllMenuItem,
            this.timeDateMenuItem
        });

        // Format menu
        this.formatMenu = new System.Windows.Forms.ToolStripMenuItem("F&ormat");
        this.wordWrapMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Word Wrap", null, WordWrapMenuItem_Click);
        this.wordWrapMenuItem.Checked = true;
        this.fontMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Font...", null, FontMenuItem_Click);

        this.formatMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wordWrapMenuItem,
            this.fontMenuItem
        });

        // View menu
        this.viewMenu = new System.Windows.Forms.ToolStripMenuItem("&View");
        this.statusBarMenuItem = new System.Windows.Forms.ToolStripMenuItem("&Status Bar", null, StatusBarMenuItem_Click);
        this.statusBarMenuItem.Checked = true;

        this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarMenuItem
        });

        // Help menu
        this.helpMenu = new System.Windows.Forms.ToolStripMenuItem("&Help");
        this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem("&About", null, AboutMenuItem_Click);

        this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem
        });

        this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.editMenu,
            this.formatMenu,
            this.viewMenu,
            this.helpMenu
        });

        // RichTextBox
        this.txtEditor = new System.Windows.Forms.RichTextBox();
        this.txtEditor.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtEditor.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtEditor.AcceptsTab = true;
        this.txtEditor.WordWrap = true;
        this.txtEditor.TextChanged += TxtEditor_TextChanged;
        this.txtEditor.SelectionChanged += TxtEditor_SelectionChanged;

        // StatusStrip
        this.statusStrip = new System.Windows.Forms.StatusStrip();
        this.lblLineColumn = new System.Windows.Forms.ToolStripStatusLabel();
        this.lblLineColumn.Text = "Ln 1, Col 1";
        this.lblLineColumn.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
        this.lblLineColumn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
        this.lblWordCount = new System.Windows.Forms.ToolStripStatusLabel();
        this.lblWordCount.Text = "Words: 0";
        this.lblWordCount.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);

        this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblLineColumn,
            this.lblWordCount
        });

        // OpenFileDialog
        this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
        this.openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        this.openFileDialog.Title = "Open";

        // SaveFileDialog
        this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        this.saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        this.saveFileDialog.Title = "Save As";

        // FontDialog
        this.fontDialog = new System.Windows.Forms.FontDialog();
        this.fontDialog.ShowColor = false;
        this.fontDialog.ShowEffects = false;

        // MainForm
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 500);
        this.Controls.Add(this.txtEditor);
        this.Controls.Add(this.statusStrip);
        this.Controls.Add(this.menuStrip);
        this.MainMenuStrip = this.menuStrip;
        this.MinimumSize = new System.Drawing.Size(400, 250);
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Untitled - TextEditor";
        this.menuStrip.ResumeLayout(false);
        this.menuStrip.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}