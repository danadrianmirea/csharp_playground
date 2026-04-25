namespace TextEditor;

public partial class MainForm : Form
{
    private string? _currentFilePath = null;
    private bool _isModified = false;

    public MainForm()
    {
        InitializeComponent();
        UpdateTitle();
        UpdateEditMenuStates();
    }

    // ──────────────────────────────
    //  File menu handlers
    // ──────────────────────────────

    private void NewMenuItem_Click(object? sender, EventArgs e)
    {
        if (!PromptSaveIfModified()) return;

        txtEditor.Clear();
        _currentFilePath = null;
        _isModified = false;
        UpdateTitle();
    }

    private void OpenMenuItem_Click(object? sender, EventArgs e)
    {
        if (!PromptSaveIfModified()) return;

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                txtEditor.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
                _currentFilePath = openFileDialog.FileName;
                _isModified = false;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open file:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void SaveMenuItem_Click(object? sender, EventArgs e)
    {
        if (_currentFilePath != null)
        {
            SaveFile(_currentFilePath);
        }
        else
        {
            SaveAsMenuItem_Click(sender, e);
        }
    }

    private void SaveAsMenuItem_Click(object? sender, EventArgs e)
    {
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            SaveFile(saveFileDialog.FileName);
        }
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e)
    {
        if (!PromptSaveIfModified()) return;
        Application.Exit();
    }

    // ──────────────────────────────
    //  Edit menu handlers
    // ──────────────────────────────

    private void UndoMenuItem_Click(object? sender, EventArgs e)
    {
        if (txtEditor.CanUndo)
        {
            txtEditor.Undo();
        }
    }

    private void CutMenuItem_Click(object? sender, EventArgs e)
    {
        txtEditor.Cut();
    }

    private void CopyMenuItem_Click(object? sender, EventArgs e)
    {
        txtEditor.Copy();
    }

    private void PasteMenuItem_Click(object? sender, EventArgs e)
    {
        txtEditor.Paste();
    }

    private void DeleteMenuItem_Click(object? sender, EventArgs e)
    {
        int selStart = txtEditor.SelectionStart;
        int selLen = txtEditor.SelectionLength;
        if (selLen > 0)
        {
            txtEditor.Text = txtEditor.Text.Remove(selStart, selLen);
        }
    }

    private void SelectAllMenuItem_Click(object? sender, EventArgs e)
    {
        txtEditor.SelectAll();
    }

    private void TimeDateMenuItem_Click(object? sender, EventArgs e)
    {
        txtEditor.SelectedText = DateTime.Now.ToString("HH:mm tt");
    }

    // ──────────────────────────────
    //  Format menu handlers
    // ──────────────────────────────

    private void WordWrapMenuItem_Click(object? sender, EventArgs e)
    {
        wordWrapMenuItem.Checked = !wordWrapMenuItem.Checked;
        txtEditor.WordWrap = wordWrapMenuItem.Checked;
    }

    private void FontMenuItem_Click(object? sender, EventArgs e)
    {
        fontDialog.Font = txtEditor.Font;
        if (fontDialog.ShowDialog() == DialogResult.OK)
        {
            txtEditor.Font = fontDialog.Font;
        }
    }

    // ──────────────────────────────
    //  View menu handlers
    // ──────────────────────────────

    private void StatusBarMenuItem_Click(object? sender, EventArgs e)
    {
        statusBarMenuItem.Checked = !statusBarMenuItem.Checked;
        statusStrip.Visible = statusBarMenuItem.Checked;
    }

    // ──────────────────────────────
    //  Help menu handlers
    // ──────────────────────────────

    private void AboutMenuItem_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "TextEditor v1.0\nA basic text editor built with Windows Forms.",
            "About TextEditor",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    // ──────────────────────────────
    //  Editor events
    // ──────────────────────────────

    private void TxtEditor_TextChanged(object? sender, EventArgs e)
    {
        if (!_isModified)
        {
            _isModified = true;
            UpdateTitle();
        }
        UpdateWordCount();
        UpdateEditMenuStates();
    }

    private void TxtEditor_SelectionChanged(object? sender, EventArgs e)
    {
        UpdateLineColumn();
        UpdateEditMenuStates();
    }

    // ──────────────────────────────
    //  Helpers
    // ──────────────────────────────

    private void SaveFile(string path)
    {
        try
        {
            txtEditor.SaveFile(path, RichTextBoxStreamType.PlainText);
            _currentFilePath = path;
            _isModified = false;
            UpdateTitle();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not save file:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool PromptSaveIfModified()
    {
        if (!_isModified) return true;

        string fileName = _currentFilePath != null
            ? Path.GetFileName(_currentFilePath)
            : "Untitled";

        var result = MessageBox.Show(
            $"Do you want to save changes to {fileName}?",
            "TextEditor",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning);

        switch (result)
        {
            case DialogResult.Yes:
                SaveMenuItem_Click(null, EventArgs.Empty);
                return !_isModified; // false if save failed
            case DialogResult.No:
                return true;
            case DialogResult.Cancel:
                return false;
            default:
                return false;
        }
    }

    private void UpdateTitle()
    {
        string prefix = _isModified ? "*" : "";
        string fileName = _currentFilePath != null
            ? Path.GetFileName(_currentFilePath)
            : "Untitled";
        Text = $"{prefix}{fileName} - TextEditor";
    }

    private void UpdateLineColumn()
    {
        int line = txtEditor.GetLineFromCharIndex(txtEditor.SelectionStart) + 1;
        int col = txtEditor.SelectionStart - txtEditor.GetFirstCharIndexOfCurrentLine() + 1;
        lblLineColumn.Text = $"Ln {line}, Col {col}";
    }

    private void UpdateWordCount()
    {
        string text = txtEditor.Text.Trim();
        int count = string.IsNullOrWhiteSpace(text) ? 0 : text.Split(
            new[] { ' ', '\t', '\n', '\r' },
            StringSplitOptions.RemoveEmptyEntries).Length;
        lblWordCount.Text = $"Words: {count}";
    }

    private void UpdateEditMenuStates()
    {
        undoMenuItem.Enabled = txtEditor.CanUndo;
        cutMenuItem.Enabled = txtEditor.SelectionLength > 0;
        copyMenuItem.Enabled = txtEditor.SelectionLength > 0;
        deleteMenuItem.Enabled = txtEditor.SelectionLength > 0;
        pasteMenuItem.Enabled = Clipboard.ContainsText();
    }
}