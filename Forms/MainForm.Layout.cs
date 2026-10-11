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
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 152F));
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
        txtDestino.TabIndex = 0;
        var btnExaminar = FlatButton.Secondary("Examinar", GlyphFolder, 108);
        btnExaminar.TabIndex = 1;
        var btnRonda1 = MakeRondaButton("RONDA_1", "Debe ser el nombre de la primera carpeta");
        var btnRonda2 = MakeRondaButton("RONDA_2", "Debe ser el nombre de la segunda carpeta");
        var btnSimultanea = MakeRondaButton("SIMULTANEA", "Debe ser el nombre de la carpeta de copia simultánea");
        row0.Controls.Add(MakeLabel("Ruta de destino (UNC):"), 0, 0);
        row0.Controls.Add(txtDestino, 1, 0);
        row0.Controls.Add(btnExaminar, 2, 0);
        row0.Controls.Add(btnRonda1, 3, 0);
        row0.Controls.Add(btnRonda2, 4, 0);
        row0.Controls.Add(btnSimultanea, 5, 0);

        // Fila del estudiante + vista previa de la ruta final resuelta.
        var row1 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, BackColor = Theme.Surface };
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        txtEstudiante.Dock = DockStyle.Fill;
        txtEstudiante.Margin = new Padding(6, 8, 6, 8);
        txtEstudiante.TabIndex = 5;
        chkCrearCarpetas.Margin = new Padding(12, 0, 12, 0);
        chkCrearCarpetas.Anchor = AnchorStyles.Left;
        chkCrearCarpetas.TabIndex = 6;
        lblRutaFinal.Margin = new Padding(0);
        _toolTip.SetToolTip(lblRutaFinal, "");
        row1.Controls.Add(MakeLabel("Carpeta del estudiante:"), 0, 0);
        row1.Controls.Add(txtEstudiante, 1, 0);
        row1.Controls.Add(chkCrearCarpetas, 2, 0);
        row1.Controls.Add(lblRutaFinal, 3, 0);

        var row2 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 8, BackColor = Theme.Surface };
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        txtUsuario.Dock = DockStyle.Fill;
        txtUsuario.Margin = new Padding(6, 8, 6, 8);
        txtUsuario.TabIndex = 7;
        txtPassword.Dock = DockStyle.Fill;
        txtPassword.Margin = new Padding(6, 8, 12, 8);
        txtPassword.TabIndex = 8;
        btnConectar.Margin = new Padding(0, 5, 8, 5);
        btnConectar.Anchor = AnchorStyles.Left;
        btnConectar.TabIndex = 9;
        btnVerificar.Margin = new Padding(0, 5, 8, 5);
        btnVerificar.Anchor = AnchorStyles.Left;
        btnVerificar.TabIndex = 10;
        btnDesconectar.Margin = new Padding(0, 5, 0, 5);
        btnDesconectar.Anchor = AnchorStyles.Left;
        btnDesconectar.TabIndex = 11;
        row2.Controls.Add(MakeLabel("Usuario:"), 0, 0);
        row2.Controls.Add(txtUsuario, 1, 0);
        row2.Controls.Add(MakeLabel("Contraseña:"), 2, 0);
        row2.Controls.Add(txtPassword, 3, 0);
        row2.Controls.Add(btnConectar, 4, 0);
        row2.Controls.Add(btnVerificar, 5, 0);
        row2.Controls.Add(btnDesconectar, 6, 0);

        layout.Controls.Add(row0, 0, 0);
        layout.Controls.Add(row1, 0, 1);
        layout.Controls.Add(row2, 0, 2);
        card.Controls.Add(layout);

        btnExaminar.Click += BtnExaminar_Click;
        btnConectar.Click += BtnConectar_Click;
        btnDesconectar.Click += BtnDesconectar_Click;
        btnVerificar.Click += BtnVerificar_Click;

        // Botón para borrar las credenciales/opciones guardadas en disco.
        var btnLimpiarCredenciales = FlatButton.Secondary("", GlyphRemove, 34);
        btnLimpiarCredenciales.Click += (_, _) =>
        {
            AppSettings.Clear();
            txtPassword.Clear();
            AppendLog("Credenciales guardadas eliminadas.");
        };
        AttachCornerButton(card, btnLimpiarCredenciales, "Limpiar credenciales guardadas");

        txtDestino.TextChanged += (_, _) => RefreshResolvedPath();
        txtEstudiante.TextChanged += (_, _) => { RefreshResolvedPath(); RefreshSummary(); };
        chkCrearCarpetas.CheckedChanged += (_, _) => RefreshResolvedPath();

        return card;
    }

    private FlatButton MakeRondaButton(string text, string tip)
    {
        var button = FlatButton.Secondary(text, GlyphFolder, 138);
        button.Margin = new Padding(6, 3, 0, 3);
        button.Tag = text;
        button.Click += BtnRonda_Click;
        button.TabIndex = 2 + _roundButtons.Count;
        _toolTip.SetToolTip(button, tip);
        _roundButtons.Add(button);
        return button;
    }

    private Card BuildConfigCard()
    {
        var card = new Card { Title = "Configuración", Dock = DockStyle.Fill, Accent = Theme.Success };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Surface };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));

        var row0 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, BackColor = Theme.Surface };
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row0.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        numLimiteMbps.Anchor = AnchorStyles.Left;
        numLimiteMbps.Margin = new Padding(6, 0, 12, 0);
        numLimiteMbps.TabIndex = 12;
        numLimiteMbps.ValueChanged += (_, _) => { RefreshSummary(); RefreshGaugeLimit(); };
        btnDetectar.Margin = new Padding(0, 0, 16, 0);
        btnDetectar.Anchor = AnchorStyles.Left;
        btnDetectar.TabIndex = 13;
        row0.Controls.Add(MakeLabel("Límite de enlace (Mbps):"), 0, 0);
        row0.Controls.Add(numLimiteMbps, 1, 0);
        row0.Controls.Add(btnDetectar, 2, 0);
        row0.Controls.Add(MakeHint("Tope teórico usado en los cálculos del informe."), 3, 0);

        var row1 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 7, BackColor = Theme.Surface };
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        numBufferMb.Anchor = AnchorStyles.Left;
        numBufferMb.Margin = new Padding(6, 0, 16, 0);
        numBufferMb.TabIndex = 14;
        numReintentos.Anchor = AnchorStyles.Left;
        numReintentos.Margin = new Padding(6, 0, 16, 0);
        numReintentos.TabIndex = 16;
        chkHash.Anchor = AnchorStyles.Left;
        chkHash.Margin = new Padding(0, 0, 20, 0);
        chkHash.TabIndex = 17;
        chkMostrarResultado.Anchor = AnchorStyles.Left;
        chkMostrarResultado.Margin = new Padding(0, 0, 20, 0);
        chkMostrarResultado.TabIndex = 18;
        row1.Controls.Add(MakeLabel("Búfer (MB):"), 0, 0);
        row1.Controls.Add(numBufferMb, 1, 0);
        row1.Controls.Add(MakeLabel("Reintentos:"), 2, 0);
        row1.Controls.Add(numReintentos, 3, 0);
        row1.Controls.Add(chkHash, 4, 0);
        row1.Controls.Add(chkMostrarResultado, 5, 0);
        row1.Controls.Add(MakeHint("La verificación SHA-256 relee el destino y aumenta el tiempo total."), 6, 0);

        layout.Controls.Add(row0, 0, 0);
        layout.Controls.Add(row1, 0, 1);
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
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Theme.Surface };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        lstArchivos.Columns.Add("Archivo", 200);
        lstArchivos.Columns.Add("Tamaño", 90);
        lstArchivos.Columns.Add("Estado", 90);
        FitFileColumns();
        lstArchivos.TabIndex = 21;
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, BackColor = Theme.Surface, Padding = new Padding(0, 10, 0, 0) };
        btnAgregarArchivos.Margin = new Padding(0);
        btnAgregarArchivos.TabIndex = 19;
        btnAgregarCarpeta.Margin = new Padding(8, 0, 0, 0);
        btnAgregarCarpeta.TabIndex = 20;
        btnQuitar.Margin = new Padding(8, 0, 0, 0);
        btnQuitar.TabIndex = 21;
        buttons.Controls.Add(btnAgregarArchivos);
        buttons.Controls.Add(btnAgregarCarpeta);
        buttons.Controls.Add(btnQuitar);
        lblResumen.Margin = new Padding(2, 0, 0, 0);
        layout.Controls.Add(lstArchivos, 0, 0);
        layout.Controls.Add(buttons, 0, 1);
        layout.Controls.Add(lblResumen, 0, 2);
        card.Controls.Add(layout);

        btnAgregarArchivos.Click += BtnAgregar_Click;
        btnAgregarCarpeta.Click += BtnAgregarCarpeta_Click;
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
        foreach (DataGridViewColumn column in dgvResultados.Columns)
            column.SortMode = DataGridViewColumnSortMode.Automatic;

        dgvResultados.CellDoubleClick += DgvResultados_CellDoubleClick;

        _btnLimpiar = FlatButton.Secondary("", GlyphRemove, 34);
        _btnLimpiar.Click += (_, _) => ClearResults();
        AttachCornerButton(card, _btnLimpiar, "Limpiar resultados");
        return card;
    }

    private Card BuildTelemetryCard()
    {
        var card = new Card
        {
            Title = "Telemetría de transferencia", Dock = DockStyle.Fill, Accent = Theme.Success,
            Margin = new Padding(0, 0, 0, 8)
        };

        // La fila del gráfico es la única flexible; el progreso, la barra global y los
        // botones tienen alto propio para que nunca se recorten.
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4, BackColor = Theme.Surface };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));

        speedChart.Margin = new Padding(0, 0, 10, 0);
        speedGauge.Margin = new Padding(10, 0, 0, 0);
        lblVelocidad.Anchor = AnchorStyles.None;
        progressArchivo.Margin = new Padding(0, 6, 10, 0);
        progressTanda.Margin = new Padding(0, 4, 0, 0);

        layout.Controls.Add(speedChart, 0, 0);
        layout.Controls.Add(speedGauge, 1, 0);
        layout.Controls.Add(progressArchivo, 0, 1);
        layout.Controls.Add(lblVelocidad, 1, 1);
        layout.Controls.Add(progressTanda, 0, 2);
        layout.SetColumnSpan(progressTanda, 2);

        var buttonRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1,
            BackColor = Theme.Surface, Margin = new Padding(0)
        };
        buttonRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        buttonRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        buttonRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        btnCopiar.Anchor = AnchorStyles.Left;
        btnCopiar.Margin = new Padding(0, 0, 10, 0);
        btnCopiar.TabIndex = 22;
        btnCancelar.Anchor = AnchorStyles.Left;
        btnCancelar.Margin = new Padding(0, 0, 10, 0);
        btnCancelar.Enabled = false;
        btnCancelar.TabIndex = 23;
        lblTanda.Margin = new Padding(0);
        buttonRow.Controls.Add(btnCopiar, 0, 0);
        buttonRow.Controls.Add(btnCancelar, 1, 0);
        buttonRow.Controls.Add(lblTanda, 2, 0);
        layout.Controls.Add(buttonRow, 0, 3);
        layout.SetColumnSpan(buttonRow, 2);

        card.Controls.Add(layout);
        _telemetryCard = card;

        // Botón para ampliar/restaurar la tarjeta de telemetría a pantalla completa.
        _btnZoom = FlatButton.Secondary("", GlyphExpand, 34);
        _btnZoom.Click += (_, _) => ToggleTelemetryExpanded();
        AttachCornerButton(card, _btnZoom, "Expandir telemetría");

        btnCopiar.Click += BtnCopiar_Click;
        btnCancelar.Click += BtnCancelar_Click;
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
        btnExportar.TabIndex = 25;
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

    private static Label MakeHint(string text) => new()
    {
        Text = text, AutoSize = false, Dock = DockStyle.Fill, AutoEllipsis = true,
        ForeColor = Theme.Muted, BackColor = Color.Transparent, Font = Theme.SmallFont,
        TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(0)
    };
}
