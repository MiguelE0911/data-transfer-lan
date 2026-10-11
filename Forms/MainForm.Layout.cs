public partial class MainForm
{
    private void BuildUi()
    {
        _outerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Bg
        };
        _outerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
        _outerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _outerLayout.Controls.Add(BuildHeader(), 0, 0);
        _outerLayout.Controls.Add(BuildContent(), 0, 1);
        Controls.Add(_outerLayout);
    }

    private Panel BuildHeader()
    {
        var header = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Header, Padding = new Padding(20, 0, 20, 0) };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, BackColor = Theme.Header };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var icon = new PictureBox
        {
            Image = IconFactory.Glyph(GlyphNetwork, 30, Theme.HeaderAccent),
            SizeMode = PictureBoxSizeMode.CenterImage,
            Size = new Size(38, 38),
            Anchor = AnchorStyles.Left
        };

        // El panel de títulos usa dos filas proporcionales para que el título y el
        // subtítulo queden centrados verticalmente y nunca se recorten.
        var titles = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Header
        };
        titles.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        titles.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        titles.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        var title = new Label
        {
            Text = "Medidor de Transferencia LAN", AutoSize = true, ForeColor = Theme.HeaderText,
            Font = Theme.AppTitleFont, BackColor = Color.Transparent,
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom, Margin = new Padding(0, 0, 0, 1)
        };
        var subtitle = new Label
        {
            Text = "Organización y Arquitectura de Computadoras", AutoSize = true, ForeColor = Theme.HeaderMuted,
            Font = Theme.SmallFont, BackColor = Color.Transparent,
            Anchor = AnchorStyles.Left | AnchorStyles.Top, Margin = new Padding(2, 1, 0, 0)
        };
        titles.Controls.Add(title, 0, 0);
        titles.Controls.Add(subtitle, 0, 1);

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
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 210F));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
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
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));

        var row0 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, BackColor = Theme.Surface };
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        txtDestino.Dock = DockStyle.Fill;
        txtDestino.Margin = new Padding(6, 8, 6, 8);
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
        txtEstudiante.Margin = new Padding(6, 8, 6, 8);
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
        txtUsuario.Margin = new Padding(6, 8, 6, 8);
        txtPassword.Dock = DockStyle.Fill;
        txtPassword.Margin = new Padding(6, 8, 12, 8);
        btnConectar.Margin = new Padding(0, 5, 8, 5);
        btnConectar.Anchor = AnchorStyles.Left;
        btnVerificar.Margin = new Padding(0, 5, 0, 5);
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

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 1, BackColor = Theme.Surface };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        numLimiteMbps.Anchor = AnchorStyles.Left;
        numLimiteMbps.Margin = new Padding(6, 0, 12, 0);
        btnDetectar.Margin = new Padding(0, 0, 16, 0);
        btnDetectar.Anchor = AnchorStyles.Left;
        chkHash.Anchor = AnchorStyles.Left;
        chkHash.Margin = new Padding(0);

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
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        lstArchivos.Columns.Add("Archivo", 240);
        lstArchivos.Columns.Add("Tamaño", 120);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, BackColor = Theme.Surface, Padding = new Padding(0, 10, 0, 0) };
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

        // La fila del gráfico es la única flexible; la barra de progreso y la fila
        // de botones tienen alto propio para que nunca se recorten.
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, BackColor = Theme.Surface };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));

        speedChart.Margin = new Padding(0, 0, 10, 0);
        speedGauge.Margin = new Padding(10, 0, 0, 0);
        lblVelocidad.Anchor = AnchorStyles.None;
        progressArchivo.Margin = new Padding(0, 6, 10, 0);

        layout.Controls.Add(speedChart, 0, 0);
        layout.Controls.Add(speedGauge, 1, 0);
        layout.Controls.Add(progressArchivo, 0, 1);
        layout.Controls.Add(lblVelocidad, 1, 1);

        var buttonRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
            BackColor = Theme.Surface, Margin = new Padding(0)
        };
        buttonRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        buttonRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        btnCopiar.Anchor = AnchorStyles.Left;
        btnCopiar.Margin = new Padding(0, 0, 10, 0);
        btnCancelar.Anchor = AnchorStyles.Left;
        btnCancelar.Margin = new Padding(0);
        buttonRow.Controls.Add(btnCopiar, 0, 0);
        buttonRow.Controls.Add(btnCancelar, 1, 0);
        layout.Controls.Add(buttonRow, 0, 2);
        layout.SetColumnSpan(buttonRow, 2);

        card.Controls.Add(layout);
        _telemetryCard = card;

        // Botón para ampliar/restaurar la tarjeta de telemetría a pantalla completa.
        _btnZoom = FlatButton.Secondary("", GlyphExpand, 34);
        _btnZoom.Height = 28;
        _btnZoom.CornerRadius = 6;
        _btnZoom.GlyphSize = 14;
        _btnZoom.Margin = new Padding(0);
        _btnZoom.Click += (_, _) => ToggleTelemetryExpanded();
        card.Controls.Add(_btnZoom);
        _btnZoom.BringToFront();
        card.Resize += (_, _) => LayoutZoomButton();
        _toolTip.SetToolTip(_btnZoom, "Expandir telemetría");

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
}
