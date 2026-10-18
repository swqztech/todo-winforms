using MojiTodo.Models;
using MojiTodo.Services;
using MojiTodo.Ui;

namespace MojiTodo;

public sealed class MainForm : Form
{
    private readonly TodoStore _store = new();
    private FilterKind _filter = FilterKind.All;
    private int[] _map = [];

    private TableLayoutPanel _root = null!;
    private TextBox _txtInput = null!;
    private Button _btnAdd = null!;
    private Button _btnAll = null!;
    private Button _btnActive = null!;
    private Button _btnDone = null!;
    private Button _btnExport = null!;
    private Button _btnImport = null!;
    private Button _btnPaste = null!;
    private Label _lblStats = null!;
    private CheckedListBox _clbTasks = null!;
    private Button _btnUp = null!;
    private Button _btnDown = null!;
    private Button _btnDelete = null!;
    private bool _suppressCheck;

    public MainForm()
    {
        Text = "墨迹清单";
        MinimumSize = new Size(480, 520);
        ClientSize = new Size(560, 640);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Microsoft YaHei UI", 9f);
        BackColor = MojiTheme.PageBg;
        ForeColor = MojiTheme.Title;
        DoubleBuffered = true;

        _store.Load();
        BuildUi();
        RefreshList();
        UpdateFilterButtons();
    }

    private void BuildUi()
    {
        _root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(20, 12, 20, 16),
            BackColor = MojiTheme.PageBg
        };
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        _root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        _root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _root.Controls.Add(BuildHeader(), 0, 0);
        _root.Controls.Add(BuildInputRow(), 0, 1);
        _root.Controls.Add(BuildToolbar(), 0, 2);
        _root.Controls.Add(BuildList(), 0, 3);
        _root.Controls.Add(BuildBottomBar(), 0, 4);

        var topBar = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = MojiTheme.TopBar };
        topBar.Controls.Add(new Label
        {
            Text = "墨迹清单",
            AutoSize = true,
            Location = new Point(20, 12),
            Font = new Font(Font.FontFamily, 10f, FontStyle.Bold),
            ForeColor = MojiTheme.Title
        });
        Controls.Add(_root);
        Controls.Add(topBar);
    }

    private Control BuildHeader()
    {
        var wrap = new TableLayoutPanel { AutoSize = true, ColumnCount = 1, Margin = new Padding(0, 0, 0, 12) };
        wrap.Controls.Add(new Label
        {
            Text = "INK LIST",
            AutoSize = true,
            ForeColor = MojiTheme.Accent,
            Font = new Font(Font.FontFamily, 8.25f, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 2)
        }, 0, 0);
        var now = DateTime.Now;
        var weekdays = new[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        wrap.Controls.Add(new Label
        {
            Text = $"{now.Month}月{now.Day}日{weekdays[(int)now.DayOfWeek]}",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 16f, FontStyle.Bold),
            ForeColor = MojiTheme.Title,
            Margin = new Padding(0, 0, 0, 4)
        }, 0, 1);
        wrap.Controls.Add(new Label
        {
            Text = "写下来，划掉，继续。支持导出 / 导入 / 粘贴 Markdown。",
            AutoSize = true,
            ForeColor = MojiTheme.Sub,
            Margin = new Padding(0, 0, 0, 0)
        }, 0, 2);
        return wrap;
    }

    private Control BuildInputRow()
    {
        var row = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 2,
            Margin = new Padding(0, 0, 0, 10),
            Dock = DockStyle.Top
        };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88));

        _txtInput = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 8, 0) };
        MojiTheme.StyleInput(_txtInput);
        _txtInput.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddFromInput();
            }
        };

        _btnAdd = MojiTheme.MakeBlueButton("加入", new Size(80, 32));
        _btnAdd.Dock = DockStyle.Fill;
        _btnAdd.Click += (_, _) => AddFromInput();

        row.Controls.Add(_txtInput, 0, 0);
        row.Controls.Add(_btnAdd, 1, 0);
        return row;
    }

    private Control BuildToolbar()
    {
        var outer = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 2,
            Margin = new Padding(0, 0, 0, 8),
            Dock = DockStyle.Top
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var flow = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        _btnAll = MojiTheme.MakeChipButton("全部", new Size(64, 30));
        _btnActive = MojiTheme.MakeChipButton("未完成", new Size(72, 30));
        _btnDone = MojiTheme.MakeChipButton("已完成", new Size(72, 30));
        _btnExport = MojiTheme.MakeChipButton("导出", new Size(64, 30));
        _btnImport = MojiTheme.MakeChipButton("导入", new Size(64, 30));
        _btnPaste = MojiTheme.MakeChipButton("粘贴导入", new Size(88, 30));
        _btnAll.Click += (_, _) => SetFilter(FilterKind.All);
        _btnActive.Click += (_, _) => SetFilter(FilterKind.Active);
        _btnDone.Click += (_, _) => SetFilter(FilterKind.Done);
        _btnExport.Click += (_, _) => ExportMd();
        _btnImport.Click += (_, _) => ImportFile();
        _btnPaste.Click += (_, _) => PasteImport();
        flow.Controls.AddRange([_btnAll, _btnActive, _btnDone, _btnExport, _btnImport, _btnPaste]);

        _lblStats = new Label
        {
            AutoSize = true,
            ForeColor = MojiTheme.Muted,
            Anchor = AnchorStyles.Right,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(0, 8, 0, 0)
        };

        outer.Controls.Add(flow, 0, 0);
        outer.Controls.Add(_lblStats, 1, 0);
        return outer;
    }

    private Control BuildList()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(1),
            Margin = new Padding(0, 0, 0, 8)
        };
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(MojiTheme.Border);
            e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
        };

        _clbTasks = new CheckedListBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            CheckOnClick = true,
            IntegralHeight = false,
            Font = new Font(Font.FontFamily, 10f),
            BackColor = Color.White,
            ForeColor = MojiTheme.Title
        };
        _clbTasks.ItemCheck += OnItemCheck;
        _clbTasks.DoubleClick += (_, _) => EditSelected();
        panel.Controls.Add(_clbTasks);
        return panel;
    }

    private Control BuildBottomBar()
    {
        var flow = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
        _btnUp = MojiTheme.MakeChipButton("上移", new Size(72, 32));
        _btnDown = MojiTheme.MakeChipButton("下移", new Size(72, 32));
        _btnDelete = MojiTheme.MakeChipButton("删除", new Size(72, 32));
        _btnUp.Click += (_, _) => MoveSelected(-1);
        _btnDown.Click += (_, _) => MoveSelected(1);
        _btnDelete.Click += (_, _) => DeleteSelected();
        flow.Controls.AddRange([_btnUp, _btnDown, _btnDelete]);
        return flow;
    }

    private void AddFromInput()
    {
        _store.Add(_txtInput.Text);
        _txtInput.Clear();
        _txtInput.Focus();
        RefreshList();
    }

    private void SetFilter(FilterKind kind)
    {
        _filter = kind;
        UpdateFilterButtons();
        RefreshList();
    }

    private void UpdateFilterButtons()
    {
        MojiTheme.StyleChip(_btnAll, _filter == FilterKind.All);
        MojiTheme.StyleChip(_btnActive, _filter == FilterKind.Active);
        MojiTheme.StyleChip(_btnDone, _filter == FilterKind.Done);
    }

    private void RefreshList()
    {
        var indices = _store.FilteredIndices(_filter);
        _map = indices.ToArray();
        _suppressCheck = true;
        _clbTasks.Items.Clear();
        foreach (var idx in _map)
        {
            var item = _store.Items[idx];
            _clbTasks.Items.Add(item.Text, item.Done);
        }
        _suppressCheck = false;
        _lblStats.Text = _store.CountsLabel();
    }

    private int SelectedStoreIndex()
    {
        var li = _clbTasks.SelectedIndex;
        if (li < 0 || li >= _map.Length) return -1;
        return _map[li];
    }

    private void OnItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (_suppressCheck) return;
        if (e.Index < 0 || e.Index >= _map.Length) return;
        var storeIdx = _map[e.Index];
        var done = e.NewValue == CheckState.Checked;
        BeginInvoke(() =>
        {
            _store.SetDone(storeIdx, done);
            RefreshList();
            if (e.Index < _clbTasks.Items.Count)
                _clbTasks.SelectedIndex = e.Index;
        });
    }

    private void EditSelected()
    {
        var storeIdx = SelectedStoreIndex();
        if (storeIdx < 0) return;
        var old = _store.Items[storeIdx].Text;
        using var dlg = new Form
        {
            Text = "编辑待办",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(360, 120),
            MinimizeBox = false,
            MaximizeBox = false,
            BackColor = MojiTheme.PageBg
        };
        var tb = new TextBox { Text = old, Width = 320, Location = new Point(20, 16) };
        MojiTheme.StyleInput(tb);
        var ok = new Button { Text = "确定", DialogResult = DialogResult.OK, Location = new Point(180, 56), Size = new Size(75, 28) };
        var cancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Point(265, 56), Size = new Size(75, 28) };
        dlg.Controls.AddRange([tb, ok, cancel]);
        dlg.AcceptButton = ok;
        dlg.CancelButton = cancel;
        if (dlg.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(tb.Text))
            return;
        _store.UpdateText(storeIdx, tb.Text);
        RefreshList();
    }

    private void DeleteSelected()
    {
        var storeIdx = SelectedStoreIndex();
        if (storeIdx < 0)
        {
            MessageBox.Show(this, "请先选中一条待办", "墨迹清单", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show(this, "删除这条待办？", "墨迹清单", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        _store.RemoveAt(storeIdx);
        RefreshList();
    }

    private void MoveSelected(int delta)
    {
        var storeIdx = SelectedStoreIndex();
        if (storeIdx < 0) return;
        var newIdx = storeIdx + delta;
        if (newIdx < 0 || newIdx >= _store.Items.Count) return;
        _store.MoveItem(storeIdx, newIdx);
        RefreshList();
        var listIdx = Array.IndexOf(_map, newIdx);
        if (listIdx >= 0)
            _clbTasks.SelectedIndex = listIdx;
    }

    private void ExportMd()
    {
        using var dlg = new SaveFileDialog
        {
            Filter = "Markdown|*.md|文本|*.txt",
            DefaultExt = "md",
            FileName = "墨迹清单-export.md"
        };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        File.WriteAllText(dlg.FileName, _store.ExportMarkdown());
        MessageBox.Show(this, "已导出到：\n" + dlg.FileName, "墨迹清单", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ImportFile()
    {
        using var dlg = new OpenFileDialog
        {
            Filter = "Markdown|*.md;*.txt|JSON|*.json|所有文件|*.*"
        };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
        if (ext == ".json")
            _store.ReplaceFromJsonFile(dlg.FileName);
        else
            _store.ImportMarkdown(File.ReadAllText(dlg.FileName));
        RefreshList();
        MessageBox.Show(this, "导入完成", "墨迹清单", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void PasteImport()
    {
        var text = Clipboard.GetText();
        if (string.IsNullOrWhiteSpace(text))
        {
            MessageBox.Show(this, "剪贴板为空", "墨迹清单", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        _store.ImportMarkdown(text);
        RefreshList();
        MessageBox.Show(this, "已从剪贴板导入", "墨迹清单", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
