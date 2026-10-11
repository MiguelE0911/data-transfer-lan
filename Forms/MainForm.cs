public partial class MainForm : Form
{
    private const string GlyphNetwork = "\uE968";
    private const string GlyphFolder = "\uE8B7";
    private const string GlyphAdd = "\uE710";
    private const string GlyphRemove = "\uE74D";
    private const string GlyphCopy = "\uE8C8";
    private const string GlyphCancel = "\uE711";
    private const string GlyphSave = "\uE74E";
    private const string GlyphRefresh = "\uE72C";
    private const string GlyphLink = "\uE71B";
    private const string GlyphCheck = "\uE73E";
    private const string GlyphExpand = "\uE740";
    private const string GlyphRestore = "\uE73F";
    private const string GlyphPower = "\uE7E8";

    private readonly TextBox txtDestino = new() { Text = @"\\192.168.100.10\Velocidad" };
    private readonly TextBox txtEstudiante = new() { Text = "FAST-Miguel" };
    private readonly TextBox txtUsuario = new() { Text = "FAST" };
    private readonly TextBox txtPassword = new() { UseSystemPasswordChar = true };
    private readonly CheckBox chkCrearCarpetas = new()
    {
        Text = "Crear carpeta del estudiante y subcarpetas", Checked = true, AutoSize = true,
        ForeColor = Theme.Text, BackColor = Color.Transparent
    };

    private readonly ListView lstArchivos = new()
    {
        View = View.Details,
        FullRowSelect = true,
        GridLines = false,
        MultiSelect = true,
        Dock = DockStyle.Fill,
        BorderStyle = BorderStyle.None,
        HeaderStyle = ColumnHeaderStyle.Nonclickable,
        OwnerDraw = true,
        AllowDrop = true,
        BackColor = Theme.Surface,
        ForeColor = Theme.Text,
        Font = Theme.BaseFont
    };

    private readonly DataGridView dgvResultados = new()
    {
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        AllowUserToResizeColumns = true,
        RowHeadersVisible = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        Dock = DockStyle.Fill,
        BorderStyle = BorderStyle.None
    };

    private readonly NumericUpDown numLimiteMbps = new()
    {
        Minimum = 1,
        Maximum = 100000,
        DecimalPlaces = 1,
        Increment = 100,
        Value = 1000,
        BorderStyle = BorderStyle.FixedSingle
    };
    private readonly NumericUpDown numBufferMb = new()
    {
        Minimum = 1,
        Maximum = 256,
        Increment = 1,
        Value = 4,
        BorderStyle = BorderStyle.FixedSingle
    };
    private readonly NumericUpDown numReintentos = new()
    {
        Minimum = 0,
        Maximum = 5,
        Increment = 1,
        Value = 1,
        BorderStyle = BorderStyle.FixedSingle
    };
    private readonly CheckBox chkHash = new()
    {
        Text = "Verificar integridad SHA-256", AutoSize = true,
        ForeColor = Theme.Text, BackColor = Color.Transparent
    };
    private readonly CheckBox chkMostrarResultado = new()
    {
        Text = "Mostrar resultado al terminar cada archivo", Checked = true, AutoSize = true,
        ForeColor = Theme.Text, BackColor = Color.Transparent
    };

    private readonly RoundedProgress progressArchivo = new() { Dock = DockStyle.Fill };
    private readonly RoundedProgress progressTanda = new() { Dock = DockStyle.Fill, BarColor = Theme.HeaderAccent };
    private readonly Label lblVelocidad = new()
    {
        Text = "0.00 MB/s", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter,
        ForeColor = Theme.Accent, Font = new Font("Segoe UI Semibold", 10F),
        Anchor = AnchorStyles.None
    };
    private readonly Label lblTanda = new()
    {
        Text = "", AutoSize = true, TextAlign = ContentAlignment.MiddleRight,
        ForeColor = Theme.Muted, Font = Theme.SmallFont,
        Anchor = AnchorStyles.Right
    };
    private readonly Label lblResumen = new()
    {
        Text = "", AutoSize = false, Dock = DockStyle.Fill, AutoEllipsis = true,
        TextAlign = ContentAlignment.MiddleLeft, ForeColor = Theme.Muted, Font = Theme.SmallFont,
        BackColor = Color.Transparent
    };
    private readonly Label lblRutaFinal = new()
    {
        Text = "", AutoSize = false, Dock = DockStyle.Fill, AutoEllipsis = true,
        TextAlign = ContentAlignment.MiddleLeft, ForeColor = Theme.HeaderAccent, Font = Theme.SmallFont,
        BackColor = Color.Transparent
    };
    private readonly SpeedChart speedChart = new() { Dock = DockStyle.Fill };
    private readonly SpeedGauge speedGauge = new() { Dock = DockStyle.Fill };

    private readonly FlatButton btnCopiar = FlatButton.Success("Copiar", GlyphCopy, 128);
    private readonly FlatButton btnCancelar = FlatButton.Danger("Cancelar", GlyphCancel, 128);
    private readonly FlatButton btnExportar = FlatButton.Primary("Exportar log (CSV)...", GlyphSave, 190);
    private readonly FlatButton btnConectar = FlatButton.Primary("Conectar", GlyphLink, 112);
    private readonly FlatButton btnDesconectar = FlatButton.Secondary("Desconectar", GlyphPower, 132);
    private readonly FlatButton btnDetectar = FlatButton.Primary("Detectar enlace", GlyphRefresh, 156);
    private readonly FlatButton btnVerificar = FlatButton.Secondary("Verificar conexión", GlyphCheck, 168);
    private readonly FlatButton btnAgregarArchivos = FlatButton.Primary("Agregar archivos...", GlyphAdd, 168);
    private readonly FlatButton btnAgregarCarpeta = FlatButton.Primary("Agregar carpeta...", GlyphFolder, 160);
    private readonly FlatButton btnQuitar = FlatButton.Secondary("Quitar", GlyphRemove, 96);

    private readonly RichTextBox txtLog = new()
    {
        Multiline = true,
        ReadOnly = true,
        ScrollBars = RichTextBoxScrollBars.Vertical,
        Dock = DockStyle.Fill,
        BorderStyle = BorderStyle.None,
        Font = Theme.MonoFont,
        BackColor = Theme.LogBg,
        ForeColor = Theme.LogText,
        DetectUrls = false
    };

    private readonly List<TransferResult> _resultados = new();
    private readonly List<FlatButton> _roundButtons = new();
    private readonly ToolTip _toolTip = new();
    private SplitContainer? _split;
    private TableLayoutPanel? _outerLayout;
    private Card? _telemetryCard;
    private Control? _telemetryHome;
    private FlatButton? _btnZoom;
    private FlatButton? _btnLimpiar;
    private bool _telemetryExpanded;
    private string _subcarpeta = "RONDA_1";
    private CancellationTokenSource? _cts;
    private int _runTotal;
    private int _runDone;

    private sealed class FileEntry
    {
        public string Path { get; init; } = "";
        public long Bytes { get; init; }
        public string State { get; set; } = "Pendiente";
    }

    public MainForm()
    {
        Text = "Medidor de Transferencia LAN";
        Size = new Size(1160, 1000);
        MinimumSize = new Size(1020, 820);
        StartPosition = FormStartPosition.CenterScreen;
        Font = Theme.BaseFont;
        BackColor = Theme.Bg;
        KeyPreview = true;

        StyleInputs();
        ConfigureGrid();
        ConfigureListView();
        BuildUi();

        AcceptButton = btnCopiar;
        KeyDown += MainForm_KeyDown;
        FormClosing += MainForm_FormClosing;

        LoadSettings();
        RefreshResolvedPath();
        RefreshSummary();
        UpdateRoundButtons();

        AppendLog("Aplicación iniciada. Configure destino, usuario y archivos.");

        Load += (_, _) =>
        {
            if (_split == null || _split.Width <= 0)
                return;
            _split.Panel1MinSize = 340;
            _split.Panel2MinSize = 420;
            int distance = Math.Clamp((int)(_split.Width * 0.50), 340, _split.Width - 420);
            _split.SplitterDistance = distance;
        };
    }

    private void StyleInputs()
    {
        Theme.StyleInput(txtDestino);
        Theme.StyleInput(txtEstudiante);
        Theme.StyleInput(txtUsuario);
        Theme.StyleInput(txtPassword);
        Theme.StyleInput(numLimiteMbps);
        Theme.StyleInput(numBufferMb);
        Theme.StyleInput(numReintentos);
    }

    private void ConfigureGrid()
    {
        dgvResultados.EnableHeadersVisualStyles = false;
        dgvResultados.ColumnHeadersDefaultCellStyle.BackColor = Theme.Header;
        dgvResultados.ColumnHeadersDefaultCellStyle.ForeColor = Theme.Text;
        dgvResultados.ColumnHeadersDefaultCellStyle.Font = Theme.ListHeaderFont;
        dgvResultados.ColumnHeadersDefaultCellStyle.SelectionBackColor = Theme.Header;
        dgvResultados.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvResultados.ColumnHeadersHeight = 36;
        dgvResultados.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvResultados.GridColor = Theme.Border;
        dgvResultados.BackgroundColor = Theme.Surface;
        dgvResultados.RowTemplate.Height = 28;
        dgvResultados.DefaultCellStyle.Font = Theme.BaseFont;
        dgvResultados.DefaultCellStyle.BackColor = Theme.Surface;
        dgvResultados.DefaultCellStyle.ForeColor = Theme.Text;
        dgvResultados.DefaultCellStyle.SelectionBackColor = Theme.Selection;
        dgvResultados.DefaultCellStyle.SelectionForeColor = Color.White;
        dgvResultados.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
        dgvResultados.AlternatingRowsDefaultCellStyle.BackColor = Theme.RowAlt;
    }

    private void ConfigureListView()
    {
        lstArchivos.DrawColumnHeader += (_, e) =>
        {
            using var back = new SolidBrush(Theme.Header);
            e.Graphics.FillRectangle(back, e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.Header?.Text ?? "", Theme.ListHeaderFont,
                new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height), Theme.Text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        };

        lstArchivos.DrawItem += (_, e) =>
        {
            bool selected = e.Item?.Selected ?? false;
            Color back = selected ? Theme.Selection : (e.ItemIndex % 2 == 0 ? Theme.Surface : Theme.RowAlt);
            using var brush = new SolidBrush(back);
            e.Graphics.FillRectangle(brush, e.Bounds);
        };

        lstArchivos.DrawSubItem += (_, e) =>
        {
            bool selected = e.Item?.Selected ?? false;
            Color fore = selected ? Color.White : (e.Item?.ForeColor ?? Theme.Text);
            Color back = selected ? Theme.Selection : (e.ItemIndex % 2 == 0 ? Theme.Surface : Theme.RowAlt);
            using var brush = new SolidBrush(back);
            e.Graphics.FillRectangle(brush, e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.SubItem?.Text ?? "", Theme.BaseFont,
                new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height), fore,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        };

        lstArchivos.DragEnter += LstArchivos_DragEnter;
        lstArchivos.DragDrop += LstArchivos_DragDrop;
        lstArchivos.Resize += (_, _) => FitFileColumns();
    }

    // El encabezado es owner-draw: si las columnas no cubren todo el ancho, el
    // área restante la pinta el control nativo (clara). Estiramos la última
    // columna para cubrir todo el encabezado.
    private void FitFileColumns()
    {
        if (lstArchivos.Columns.Count == 0)
            return;

        int others = 0;
        for (int i = 0; i < lstArchivos.Columns.Count - 1; i++)
            others += lstArchivos.Columns[i].Width;

        int available = lstArchivos.ClientSize.Width - others;
        lstArchivos.Columns[lstArchivos.Columns.Count - 1].Width = Math.Max(80, available);
    }
}
