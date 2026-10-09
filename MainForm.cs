using System.Globalization;

public class MainForm : Form
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
    private readonly CheckBox chkHash = new()
    {
        Text = "Verificar integridad SHA-256", AutoSize = true,
        ForeColor = Theme.Text, BackColor = Color.Transparent
    };

    private readonly RoundedProgress progressArchivo = new() { Dock = DockStyle.Fill };
    private readonly Label lblVelocidad = new()
    {
        Text = "0.00 MB/s", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter,
        ForeColor = Theme.Accent, Font = new Font("Segoe UI Semibold", 10F),
        Anchor = AnchorStyles.None
    };
    private readonly SpeedChart speedChart = new() { Dock = DockStyle.Fill };
    private readonly SpeedGauge speedGauge = new() { Dock = DockStyle.Fill };

    private readonly FlatButton btnCopiar = FlatButton.Success("Copiar", GlyphCopy, 128);
    private readonly FlatButton btnCancelar = FlatButton.Danger("Cancelar", GlyphCancel, 128);
    private readonly FlatButton btnExportar = FlatButton.Primary("Exportar log (CSV)...", GlyphSave, 190);
    private readonly FlatButton btnConectar = FlatButton.Primary("Conectar", GlyphLink, 112);
    private readonly FlatButton btnDetectar = FlatButton.Primary("Detectar enlace", GlyphRefresh, 156);
    private readonly FlatButton btnVerificar = FlatButton.Secondary("Verificar conexión", GlyphCheck, 168);

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
    private SplitContainer? _split;
    private string _subcarpeta = "RONDA_1";
    private CancellationTokenSource? _cts;

    private sealed class FileEntry
    {
        public string Path { get; init; } = "";
        public long Bytes { get; init; }
    }

    public MainForm()
    {
        Text = "Medidor de Transferencia LAN";
        Size = new Size(1140, 980);
        MinimumSize = new Size(1000, 720);
        StartPosition = FormStartPosition.CenterScreen;
        Font = Theme.BaseFont;
        BackColor = Theme.Bg;

        StyleInputs();
        ConfigureGrid();
        ConfigureListView();
        BuildUi();
        AppendLog("Aplicación iniciada. Configure destino, usuario y archivos.");

        Load += (_, _) =>
        {
            if (_split == null || _split.Width <= 0)
                return;
            _split.Panel1MinSize = 300;
            _split.Panel2MinSize = 420;
            int distance = Math.Clamp((int)(_split.Width * 0.46), 300, _split.Width - 420);
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
            Color fore = selected ? Color.White : Theme.Text;
            Color back = selected ? Theme.Selection : (e.ItemIndex % 2 == 0 ? Theme.Surface : Theme.RowAlt);
            using var brush = new SolidBrush(back);
            e.Graphics.FillRectangle(brush, e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.SubItem?.Text ?? "", Theme.BaseFont,
                new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height), fore,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        };
    }

    private void BuildUi()
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Bg
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        outer.Controls.Add(BuildHeader(), 0, 0);
        outer.Controls.Add(BuildContent(), 0, 1);
        Controls.Add(outer);
    }

    private Panel BuildHeader()
    {
        var header = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Header, Padding = new Padding(20, 0, 20, 0) };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, BackColor = Theme.Header };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var icon = new PictureBox
        {
            Image = IconFactory.Glyph(GlyphNetwork, 30, Theme.HeaderAccent),
            SizeMode = PictureBoxSizeMode.CenterImage,
            Size = new Size(38, 38),
            Anchor = AnchorStyles.Left
        };

        var titles = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Header };
        var title = new Label
        {
            Text = "Medidor de Transferencia LAN", AutoSize = true, ForeColor = Theme.HeaderText,
            Font = Theme.AppTitleFont, BackColor = Color.Transparent, Location = new Point(0, 10)
        };
        var subtitle = new Label
        {
            Text = "Organización y Arquitectura de Computadoras", AutoSize = true, ForeColor = Theme.HeaderMuted,
            Font = Theme.SmallFont, BackColor = Color.Transparent, Location = new Point(2, 38)
        };
        titles.Controls.Add(title);
        titles.Controls.Add(subtitle);

        var status = new Label
        {
            Text = @"BOSGAME · \\192.168.100.10", AutoSize = true, ForeColor = Theme.HeaderMuted,
            Font = Theme.SmallFont, BackColor = Color.Transparent,
            Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight
        };

        layout.Controls.Add(icon, 0, 0);
        layout.Controls.Add(titles, 1, 0);
        layout.Controls.Add(status, 2, 0);
        header.Controls.Add(layout);
        return header;
    }

    private TableLayoutPanel BuildContent()
    {
        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
            Padding = new Padding(16, 12, 16, 16), BackColor = Theme.Bg
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 176F));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 98F));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        content.Controls.Add(BuildDestinoCard(), 0, 0);
        content.Controls.Add(BuildConfigCard(), 0, 1);
        content.Controls.Add(BuildSplit(), 0, 2);
        return content;
    }

    private SplitContainer BuildSplit()
    {
        _split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterWidth = 8,
            BackColor = Theme.Bg,
            Size = new Size(1100, 600)
        };
        _split.Panel1.BackColor = Theme.Bg;
        _split.Panel2.BackColor = Theme.Bg;

        var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Bg };
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
        left.Controls.Add(BuildFilesCard(), 0, 0);
        left.Controls.Add(BuildLogCard(), 0, 1);

        var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Bg };
        right.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
        right.Controls.Add(BuildTelemetryCard(), 0, 0);
        right.Controls.Add(BuildResultsCard(), 0, 1);

        _split.Panel1.Controls.Add(left);
        _split.Panel2.Controls.Add(right);
        return _split;
    }

    private Card BuildDestinoCard()
    {
        var card = new Card { Title = "Destino", Dock = DockStyle.Fill, Accent = Theme.Accent };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Theme.Surface };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

        var row0 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, BackColor = Theme.Surface };
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        txtDestino.Dock = DockStyle.Fill;
        txtDestino.Margin = new Padding(6, 6, 6, 6);
        var btnExaminar = FlatButton.Secondary("Examinar", GlyphFolder, 108);
        var btnRonda1 = MakeRondaButton("RONDA_1");
        var btnRonda2 = MakeRondaButton("RONDA_2");
        var btnSimultanea = MakeRondaButton("SIMULTANEA");
        row0.Controls.Add(MakeLabel("Ruta de destino (UNC):"), 0, 0);
        row0.Controls.Add(txtDestino, 1, 0);
        row0.Controls.Add(btnExaminar, 2, 0);
        row0.Controls.Add(btnRonda1, 3, 0);
        row0.Controls.Add(btnRonda2, 4, 0);
        row0.Controls.Add(btnSimultanea, 5, 0);

        var row1 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, BackColor = Theme.Surface };
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        txtEstudiante.Dock = DockStyle.Fill;
        txtEstudiante.Margin = new Padding(6, 6, 6, 6);
        chkCrearCarpetas.Margin = new Padding(12, 0, 0, 0);
        chkCrearCarpetas.Anchor = AnchorStyles.Left;
        row1.Controls.Add(MakeLabel("Carpeta del estudiante:"), 0, 0);
        row1.Controls.Add(txtEstudiante, 1, 0);
        row1.Controls.Add(chkCrearCarpetas, 2, 0);

        var row2 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, BackColor = Theme.Surface };
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        txtUsuario.Dock = DockStyle.Fill;
        txtUsuario.Margin = new Padding(6, 6, 6, 6);
        txtPassword.Dock = DockStyle.Fill;
        txtPassword.Margin = new Padding(6, 6, 12, 6);
        btnConectar.Margin = new Padding(0, 3, 8, 3);
        btnConectar.Anchor = AnchorStyles.Left;
        btnVerificar.Margin = new Padding(0, 3, 0, 3);
        btnVerificar.Anchor = AnchorStyles.Left;
        row2.Controls.Add(MakeLabel("Usuario:"), 0, 0);
        row2.Controls.Add(txtUsuario, 1, 0);
        row2.Controls.Add(MakeLabel("Contraseña:"), 2, 0);
        row2.Controls.Add(txtPassword, 3, 0);
        row2.Controls.Add(btnConectar, 4, 0);
        row2.Controls.Add(btnVerificar, 5, 0);

        layout.Controls.Add(row0, 0, 0);
        layout.Controls.Add(row1, 0, 1);
        layout.Controls.Add(row2, 0, 2);
        card.Controls.Add(layout);

        btnExaminar.Click += BtnExaminar_Click;
        btnConectar.Click += BtnConectar_Click;
        btnVerificar.Click += BtnVerificar_Click;
        return card;
    }

    private FlatButton MakeRondaButton(string text)
    {
        var button = FlatButton.Secondary(text, GlyphFolder, 138);
        button.Margin = new Padding(6, 3, 0, 3);
        button.Click += BtnRonda_Click;
        return button;
    }

    private Card BuildConfigCard()
    {
        var card = new Card { Title = "Configuración", Dock = DockStyle.Fill, Accent = Theme.Success };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, BackColor = Theme.Surface };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        numLimiteMbps.Anchor = AnchorStyles.Left;
        numLimiteMbps.Margin = new Padding(6, 0, 12, 0);
        btnDetectar.Margin = new Padding(0, 0, 16, 0);
        chkHash.Anchor = AnchorStyles.Left;

        var hint = new Label
        {
            Text = "Tope teórico usado en los cálculos del informe.", AutoSize = true,
            ForeColor = Theme.Muted, BackColor = Color.Transparent, Font = Theme.SmallFont,
            Anchor = AnchorStyles.Left
        };

        layout.Controls.Add(MakeLabel("Límite de enlace (Mbps):"), 0, 0);
        layout.Controls.Add(numLimiteMbps, 1, 0);
        layout.Controls.Add(btnDetectar, 2, 0);
        layout.Controls.Add(chkHash, 3, 0);
        layout.Controls.Add(hint, 4, 0);
        card.Controls.Add(layout);

        btnDetectar.Click += BtnDetectar_Click;
        return card;
    }

    private Card BuildFilesCard()
    {
        var card = new Card
        {
            Title = "Archivos a copiar", Dock = DockStyle.Fill, Accent = Theme.Accent,
            Margin = new Padding(0, 0, 0, 8)
        };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Surface };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        lstArchivos.Columns.Add("Archivo", 240);
        lstArchivos.Columns.Add("Tamaño", 120);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, BackColor = Theme.Surface, Padding = new Padding(0, 6, 0, 0) };
        var btnAgregar = FlatButton.Primary("Agregar archivos...", GlyphAdd, 182);
        var btnQuitar = FlatButton.Secondary("Quitar", GlyphRemove, 104);
        btnQuitar.Margin = new Padding(8, 0, 0, 0);
        buttons.Controls.Add(btnAgregar);
        buttons.Controls.Add(btnQuitar);
        layout.Controls.Add(lstArchivos, 0, 0);
        layout.Controls.Add(buttons, 0, 1);
        card.Controls.Add(layout);

        btnAgregar.Click += BtnAgregar_Click;
        btnQuitar.Click += BtnQuitar_Click;
        return card;
    }

    private Card BuildResultsCard()
    {
        var card = new Card
        {
            Title = "Resultados", Dock = DockStyle.Fill, Accent = Theme.HeaderAccent,
            Margin = new Padding(0, 8, 0, 0)
        };
        card.Controls.Add(dgvResultados);

        dgvResultados.Columns.Add("Archivo", "Archivo");
        dgvResultados.Columns.Add("Bytes", "Bytes");
        dgvResultados.Columns.Add("Tiempo", "Tiempo (s)");
        dgvResultados.Columns.Add("MBs", "MB/s");
        dgvResultados.Columns.Add("Mbs", "Mb/s");
        dgvResultados.Columns.Add("Ideal", "Ideal (s)");
        dgvResultados.Columns.Add("Porcentaje", "%");
        dgvResultados.Columns.Add("Hash", "Hash");
        return card;
    }

    private Card BuildTelemetryCard()
    {
        var card = new Card
        {
            Title = "Telemetría de transferencia", Dock = DockStyle.Fill, Accent = Theme.Success,
            Margin = new Padding(0, 0, 0, 8)
        };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, BackColor = Theme.Surface };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

        speedChart.Margin = new Padding(0, 0, 10, 0);
        speedGauge.Margin = new Padding(10, 0, 0, 0);
        lblVelocidad.Anchor = AnchorStyles.None;
        progressArchivo.Margin = new Padding(0, 6, 10, 0);

        layout.Controls.Add(speedChart, 0, 0);
        layout.Controls.Add(speedGauge, 1, 0);
        layout.Controls.Add(progressArchivo, 0, 1);
        layout.Controls.Add(lblVelocidad, 1, 1);

        var buttonRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, BackColor = Theme.Surface, Padding = new Padding(0, 6, 0, 0) };
        btnCopiar.Margin = new Padding(0, 0, 10, 0);
        buttonRow.Controls.Add(btnCopiar);
        buttonRow.Controls.Add(btnCancelar);
        layout.Controls.Add(buttonRow, 0, 2);
        layout.SetColumnSpan(buttonRow, 2);

        card.Controls.Add(layout);

        btnCopiar.Click += BtnCopiar_Click;
        btnCancelar.Click += (_, _) => _cts?.Cancel();
        return card;
    }

    private Card BuildLogCard()
    {
        var card = new Card
        {
            Title = "Registro (Log)", Dock = DockStyle.Fill, Accent = Theme.Danger,
            Margin = new Padding(0, 8, 0, 0)
        };

        var container = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Surface };
        container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        container.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));

        var logHost = new Panel { Dock = DockStyle.Fill, BackColor = Theme.LogBg, Padding = new Padding(8, 6, 8, 6) };
        logHost.Controls.Add(txtLog);

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Theme.Surface };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var hint = new Label
        {
            Text = "Cada operación se registra con archivo, tiempo, velocidad, ideal y resultado.",
            AutoSize = true, ForeColor = Theme.Muted, Font = Theme.SmallFont, BackColor = Color.Transparent,
            Anchor = AnchorStyles.Left
        };
        btnExportar.Anchor = AnchorStyles.Right;
        footer.Controls.Add(hint, 0, 0);
        footer.Controls.Add(btnExportar, 1, 0);

        container.Controls.Add(logHost, 0, 0);
        container.Controls.Add(footer, 0, 1);
        card.Controls.Add(container);

        btnExportar.Click += BtnExportar_Click;
        return card;
    }

    private static Label MakeLabel(string text) => new()
    {
        Text = text, AutoSize = true, ForeColor = Theme.Text, BackColor = Color.Transparent,
        Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft
    };

    private void BtnExaminar_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Seleccione la carpeta de destino en la BOSGAME",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true,
            InitialDirectory = txtDestino.Text.Trim()
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            txtDestino.Text = dialog.SelectedPath;
    }

    private void BtnRonda_Click(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            _subcarpeta = button.Text;
            AppendLog($"Subcarpeta activa: {_subcarpeta}");
        }
    }

    private void BtnConectar_Click(object? sender, EventArgs e)
    {
        var unc = txtDestino.Text.Trim();
        if (string.IsNullOrWhiteSpace(unc))
        {
            AppendLog("La ruta de destino está vacía.");
            return;
        }

        bool ok = SmbConnection.Connect(unc, txtUsuario.Text.Trim(), txtPassword.Text, out var message);
        AppendLog($"Conexión a {unc}: {(ok ? "correcta" : "fallida")}. {message}");
    }

    private async void BtnVerificar_Click(object? sender, EventArgs e)
    {
        var destino = txtDestino.Text.Trim();
        if (string.IsNullOrWhiteSpace(destino))
        {
            AppendLog("La ruta de destino está vacía.");
            return;
        }

        btnVerificar.Enabled = false;
        AppendLog($"Verificando conexión con \"{destino}\"...");
        try
        {
            var (ok, message) = await Task.Run(() => Connectivity.Check(destino));
            AppendLog($"Verificación {(ok ? "correcta" : "con problemas")}: {message}");
        }
        catch (Exception ex)
        {
            AppendLog("ERROR al verificar la conexión: " + ex.Message);
        }
        finally
        {
            btnVerificar.Enabled = true;
        }
    }

    private void BtnDetectar_Click(object? sender, EventArgs e)
    {
        try
        {
            var adapters = NetworkInfo.GetActiveAdapters();
            if (adapters.Count == 0)
            {
                AppendLog("No se detectaron adaptadores de red activos.");
                return;
            }

            var preferred = NetworkInfo.GetAdapterForTarget(txtDestino.Text.Trim());
            int preferredIndex = 0;
            if (preferred != null)
            {
                for (int i = 0; i < adapters.Count; i++)
                {
                    if (string.Equals(adapters[i].Name, preferred.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        preferredIndex = i;
                        break;
                    }
                }
            }

            using var dialog = new AdapterDialog(adapters, preferredIndex);
            if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedIndex >= 0)
            {
                var adapter = adapters[dialog.SelectedIndex];
                var bitsPerSecond = adapter.SpeedBitsPerSecond > 0
                    ? adapter.SpeedBitsPerSecond
                    : Metrics.ParseLinkSpeedToBitsPerSecond(adapter.LinkSpeed);
                if (bitsPerSecond > 0)
                {
                    var mbps = (decimal)(bitsPerSecond / 1_000_000.0);
                    numLimiteMbps.Value = Math.Clamp(mbps, numLimiteMbps.Minimum, numLimiteMbps.Maximum);
                }
                AppendLog($"Enlace detectado: {adapter.Name} | {adapter.Description} | {adapter.LinkSpeed} | {adapter.Status}" +
                          $" => {bitsPerSecond / 1_000_000.0:F1} Mbps");
            }
        }
        catch (Exception ex)
        {
            AppendLog("ERROR al detectar el enlace: " + ex.Message);
        }
    }

    private void BtnAgregar_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Multiselect = true, Title = "Seleccione los archivos a copiar" };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        foreach (var path in dialog.FileNames)
        {
            if (lstArchivos.Items.Cast<ListViewItem>().Any(i => i.Tag is FileEntry f && f.Path == path))
                continue;

            long bytes = 0;
            try { bytes = new FileInfo(path).Length; }
            catch { }

            var item = new ListViewItem(Path.GetFileName(path)) { Tag = new FileEntry { Path = path, Bytes = bytes } };
            item.SubItems.Add(HumanSize(bytes));
            lstArchivos.Items.Add(item);
        }
    }

    private void BtnQuitar_Click(object? sender, EventArgs e)
    {
        foreach (ListViewItem item in lstArchivos.SelectedItems.Cast<ListViewItem>().ToList())
            lstArchivos.Items.Remove(item);
    }

    private async void BtnCopiar_Click(object? sender, EventArgs e)
    {
        if (lstArchivos.Items.Count == 0)
        {
            AppendLog("No hay archivos en la lista.");
            return;
        }

        var destinoBase = txtDestino.Text.Trim();
        if (string.IsNullOrWhiteSpace(destinoBase))
        {
            AppendLog("La ruta de destino está vacía.");
            return;
        }

        SetRunning(true);
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            double nominalBitsPerSecond = (double)numLimiteMbps.Value * 1_000_000.0;
            double nominalMegaBytes = nominalBitsPerSecond / 8.0 / 1_000_000.0;

            foreach (ListViewItem item in lstArchivos.Items)
            {
                if (token.IsCancellationRequested)
                    break;
                if (item.Tag is not FileEntry entry)
                    continue;

                var destinationDirectory = BuildDestinationDirectory(destinoBase);
                Directory.CreateDirectory(destinationDirectory);
                var destinationFile = Path.Combine(destinationDirectory, Path.GetFileName(entry.Path));

                var engine = new TransferEngine
                {
                    VerifyHash = chkHash.Checked,
                    NominalBitsPerSecond = nominalBitsPerSecond
                };

                speedChart.Reset();
                speedChart.Subtitle = entry.Path;
                speedGauge.Maximum = Math.Max(1, nominalMegaBytes);
                speedGauge.Value = 0;

                var progress = new Progress<ProgressInfo>(info =>
                {
                    progressArchivo.Maximum = 100;
                    progressArchivo.Value = Math.Clamp(info.PercentComplete, 0, 100);
                    lblVelocidad.Text = $"Instantánea: {info.InstantMegaBytesPerSecond:F2} MB/s";
                    speedChart.AddSample(info.ElapsedSeconds, info.InstantMegaBytesPerSecond);
                    double average = info.ElapsedSeconds > 0
                        ? info.BytesCopied / (info.ElapsedSeconds * 1_000_000.0)
                        : 0;
                    speedGauge.Value = average;
                });

                AppendLog($"Copiando \"{entry.Path}\" -> \"{destinationFile}\"");
                var result = await engine.CopyAsync(entry.Path, destinationFile, progress, token);

                _resultados.Add(result);
                AddResultRow(result);
                LogResult(result);

                speedGauge.Value = result.MegaBytesPerSecond;
                progressArchivo.Value = result.Success ? 100 : 0;
                lblVelocidad.Text = $"{result.MegaBytesPerSecond:F2} MB/s";
            }

            if (token.IsCancellationRequested)
                AppendLog("Copia cancelada por el usuario.");
            else
                AppendLog("Copia finalizada.");
        }
        catch (Exception ex)
        {
            AppendLog("ERROR durante la copia: " + ex.Message);
        }
        finally
        {
            SetRunning(false);
            _cts?.Dispose();
            _cts = null;
        }
    }

    private string BuildDestinationDirectory(string destinoBase)
    {
        var directory = destinoBase;
        if (!chkCrearCarpetas.Checked)
            return directory;

        var student = txtEstudiante.Text.Trim();
        if (student.Length > 0)
        {
            if (!student.StartsWith("FAST-", StringComparison.OrdinalIgnoreCase))
                student = "FAST-" + student;
            directory = Path.Combine(directory, student);
        }

        return Path.Combine(directory, _subcarpeta);
    }

    private void BtnExportar_Click(object? sender, EventArgs e)
    {
        try
        {
            using var dialog = new SaveFileDialog
            {
                FileName = "resultados_transferencia.csv",
                DefaultExt = "csv",
                Filter = "Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                CsvLogger.Export(dialog.FileName, _resultados);
                AppendLog($"Log exportado: {dialog.FileName} ({_resultados.Count} registros)");
            }
        }
        catch (Exception ex)
        {
            AppendLog("ERROR al exportar: " + ex.Message);
        }
    }

    private void SetRunning(bool running)
    {
        btnCopiar.Enabled = !running;
        btnCancelar.Enabled = running;
        btnDetectar.Enabled = !running;
        btnConectar.Enabled = !running;
        btnVerificar.Enabled = !running;
    }

    private void AddResultRow(TransferResult result)
    {
        dgvResultados.Rows.Add(
            result.FileName,
            result.SizeBytes.ToString(CultureInfo.InvariantCulture),
            result.ElapsedSeconds.ToString("F3", CultureInfo.InvariantCulture),
            result.MegaBytesPerSecond.ToString("F2", CultureInfo.InvariantCulture),
            result.MegabitsPerSecond.ToString("F2", CultureInfo.InvariantCulture),
            result.IdealSeconds.ToString("F3", CultureInfo.InvariantCulture),
            result.PercentOfNominal.ToString("F1", CultureInfo.InvariantCulture),
            chkHash.Checked ? (result.HashVerified ? "OK" : "NO") : "-");
    }

    private void LogResult(TransferResult result)
    {
        if (result.Success)
        {
            var hashStatus = chkHash.Checked
                ? (result.HashVerified ? "verificado " + result.Hash : "NO COINCIDE")
                : "no verificado";
            AppendLog(string.Format(CultureInfo.InvariantCulture,
                "OK | {0} | {1} bytes | {2:F3} s | {3:F2} MB/s | {4:F2} Mb/s | ideal {5:F3} s | {6:F1} % | SHA-256: {7}",
                result.FileName, result.SizeBytes, result.ElapsedSeconds, result.MegaBytesPerSecond,
                result.MegabitsPerSecond, result.IdealSeconds, result.PercentOfNominal, hashStatus));
        }
        else
        {
            AppendLog($"FALLO | {result.FileName} | {result.Error}");
        }
    }

    private void AppendLog(string message)
    {
        Color color = Theme.LogText;
        if (message.Contains("FALLO") || message.Contains("ERROR") || message.Contains("problemas"))
            color = Color.FromArgb(0xFF, 0x7A, 0x7A);
        else if (message.Contains("correcta") || message.Contains("finalizada") || message.Contains("OK |") || message.Contains("exportado"))
            color = Color.FromArgb(0x6B, 0xE0, 0x93);
        else if (message.Contains("detectado") || message.Contains("Subcarpeta"))
            color = Color.FromArgb(0x7F, 0xB9, 0xFF);

        txtLog.SelectionStart = txtLog.TextLength;
        txtLog.SelectionLength = 0;
        txtLog.SelectionColor = Theme.Muted;
        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ");
        txtLog.SelectionColor = color;
        txtLog.AppendText(message + Environment.NewLine);
        txtLog.SelectionColor = Theme.LogText;
        txtLog.SelectionStart = txtLog.TextLength;
        txtLog.ScrollToCaret();
    }

    private static string HumanSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double size = bytes;
        int unit = 0;
        while (size >= 1000 && unit < units.Length - 1)
        {
            size /= 1000;
            unit++;
        }
        return unit == 0
            ? $"{bytes} B"
            : $"{size.ToString("F2", CultureInfo.InvariantCulture)} {units[unit]}";
    }

    private sealed class AdapterDialog : Form
    {
        private readonly ListBox _list = new()
        {
            Dock = DockStyle.Fill,
            IntegralHeight = false,
            BorderStyle = BorderStyle.None,
            DrawMode = DrawMode.OwnerDrawFixed,
            ItemHeight = 34,
            BackColor = Theme.Surface,
            ForeColor = Theme.Text,
            Font = Theme.BaseFont
        };

        public int SelectedIndex => _list.SelectedIndex;

        public AdapterDialog(List<AdapterInfo> adapters, int preferredIndex = 0)
        {
            Text = "Adaptadores de red activos";
            Size = new Size(680, 400);
            MinimumSize = new Size(520, 300);
            StartPosition = FormStartPosition.CenterParent;
            Font = Theme.BaseFont;
            BackColor = Theme.Bg;
            Padding = new Padding(16);

            foreach (var adapter in adapters)
                _list.Items.Add($"{adapter.Name}|{adapter.Description}|{adapter.LinkSpeed}");

            _list.DrawItem += DrawAdapterItem;

            var card = new Card { Dock = DockStyle.Fill, Title = "Seleccione el adaptador del recorrido", Accent = Theme.Accent, Padding = new Padding(12, 44, 12, 12) };
            card.Controls.Add(_list);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 54,
                FlowDirection = FlowDirection.RightToLeft,
                BackColor = Theme.Bg,
                Padding = new Padding(0, 10, 0, 0)
            };
            var btnCancel = FlatButton.Secondary("Cancelar", "", 110);
            btnCancel.DialogResult = DialogResult.Cancel;
            var btnOk = FlatButton.Primary("Usar seleccionado", "", 160);
            btnOk.DialogResult = DialogResult.OK;
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnOk);

            Controls.Add(card);
            Controls.Add(bottom);
            AcceptButton = btnOk;
            CancelButton = btnCancel;

            _list.SelectedIndex = preferredIndex >= 0 && preferredIndex < adapters.Count
                ? preferredIndex
                : FindBest(adapters);
        }

        private void DrawAdapterItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();
            var parts = _list.Items[e.Index]?.ToString()?.Split('|') ?? Array.Empty<string>();
            string name = parts.Length > 0 ? parts[0] : "";
            string description = parts.Length > 1 ? parts[1] : "";
            string speed = parts.Length > 2 ? parts[2] : "";

            bool selected = (e.State & DrawItemState.Selected) != 0;
            Color back = selected ? Theme.Selection : Theme.Surface;
            using (var brush = new SolidBrush(back))
                e.Graphics.FillRectangle(brush, e.Bounds);

            TextRenderer.DrawText(e.Graphics, name, Theme.BoldFont,
                new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 220, e.Bounds.Height), Theme.Text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(e.Graphics, description, Theme.SmallFont,
                new Rectangle(e.Bounds.X + 160, e.Bounds.Y, e.Bounds.Width - 320, e.Bounds.Height), Theme.Muted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            string speedText = speed;
            Size speedSize = TextRenderer.MeasureText(speedText, Theme.BoldFont, Size.Empty, TextFormatFlags.NoPadding);
            var badge = new Rectangle(e.Bounds.Right - speedSize.Width - 28, e.Bounds.Y + 6, speedSize.Width + 16, e.Bounds.Height - 12);
            using (var path = Theme.Rounded(badge, 8))
            using (var brush = new SolidBrush(selected ? Theme.Accent : Color.FromArgb(0x24, 0x30, 0x48)))
                e.Graphics.FillPath(brush, path);
            TextRenderer.DrawText(e.Graphics, speedText, Theme.BoldFont, badge, selected ? Color.White : Theme.Accent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static int FindBest(List<AdapterInfo> adapters)
        {
            for (int i = 0; i < adapters.Count; i++)
            {
                var description = adapters[i].Description.ToLowerInvariant();
                if (description.Contains("ethernet") &&
                    !description.Contains("virtual") &&
                    !description.Contains("wi-fi") &&
                    !description.Contains("wireless"))
                    return i;
            }
            return 0;
        }
    }
}
