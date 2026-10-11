using System.Globalization;
using System.Text;

// Diálogo destacado que resume cada transferencia con las unidades exactas,
// pensado para facilitar las capturas de pantalla de la práctica.
public class ResultDialog : Form
{
    private readonly string _clipboardText;

    public ResultDialog(TransferResult result, bool hashChecked, string round)
    {
        Text = "Resultado de la transferencia";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Font = Theme.BaseFont;
        BackColor = Theme.Bg;
        ClientSize = new Size(500, 430);

        string hashStatus = hashChecked
            ? (result.HashVerified ? "OK" : "NO COINCIDE")
            : "no verificado";

        var accent = result.Success ? Theme.Success : Theme.Danger;
        var card = new Card
        {
            Dock = DockStyle.Fill,
            Title = result.Success ? "Transferencia completada" : "Transferencia con errores",
            Accent = accent,
            Margin = new Padding(0)
        };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Theme.Surface };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));

        // Cifras grandes para la captura.
        var headline = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Theme.Surface, Margin = new Padding(0) };
        headline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        headline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        headline.Controls.Add(CreateBigBlock("Velocidad promedio", result.MegaBytesPerSecond.ToString("F2", CultureInfo.InvariantCulture), "MB/s", Theme.Accent), 0, 0);
        headline.Controls.Add(CreateBigBlock("Tiempo de copia", result.ElapsedSeconds.ToString("F3", CultureInfo.InvariantCulture), "segundos", accent), 1, 0);

        var details = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Theme.Surface, Margin = new Padding(0, 6, 0, 0) };
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        AddDetail(details, "Archivo:", result.FileName);
        AddDetail(details, "Ronda:", round);
        AddDetail(details, "Tamaño:", $"{result.SizeBytes.ToString(CultureInfo.InvariantCulture)} bytes ({Metrics.FormatBytes(result.SizeBytes)})");
        AddDetail(details, "Velocidad:", $"{result.MegaBytesPerSecond:F2} MB/s  ·  {result.MegabitsPerSecond:F2} Mb/s");
        AddDetail(details, "Tiempo ideal:", $"{result.IdealSeconds:F3} s");
        AddDetail(details, "% del límite:", $"{result.PercentOfNominal:F1} %");
        AddDetail(details, "SHA-256:", hashStatus);
        AddDetail(details, "Estado:", result.Success ? "Correcto" : "Error: " + result.Error);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Theme.Surface,
            Padding = new Padding(0, 8, 0, 0),
            Margin = new Padding(0)
        };
        var btnClose = FlatButton.Secondary("Cerrar", "", 110);
        btnClose.DialogResult = DialogResult.OK;
        var btnCopy = FlatButton.Primary("Copiar datos", GlyphCopyText, 150);
        buttons.Controls.Add(btnClose);
        buttons.Controls.Add(btnCopy);

        layout.Controls.Add(headline, 0, 0);
        layout.Controls.Add(details, 0, 1);
        layout.Controls.Add(buttons, 0, 2);
        card.Controls.Add(layout);
        Controls.Add(card);

        AcceptButton = btnClose;
        CancelButton = btnClose;

        _clipboardText = BuildClipboardText(result, round, hashStatus);
        btnCopy.Click += (_, _) =>
        {
            try { Clipboard.SetText(_clipboardText); }
            catch { /* portapapeles ocupado */ }
        };
    }

    private const string GlyphCopyText = "\uE8C8";

    private static Control CreateBigBlock(string caption, string value, string unit, Color color)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Theme.Surface };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));

        panel.Controls.Add(new Label
        {
            Text = caption, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Theme.Muted, Font = Theme.SmallFont, BackColor = Color.Transparent
        }, 0, 0);
        panel.Controls.Add(new Label
        {
            Text = value, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = color, Font = new Font("Segoe UI Semibold", 22F), BackColor = Color.Transparent
        }, 0, 1);
        panel.Controls.Add(new Label
        {
            Text = unit, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Theme.Muted, Font = Theme.SmallFont, BackColor = Color.Transparent
        }, 0, 2);
        return panel;
    }

    private static void AddDetail(TableLayoutPanel table, string label, string value)
    {
        int row = table.RowCount;
        table.RowCount = row + 1;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        table.Controls.Add(new Label
        {
            Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Theme.Muted, Font = Theme.SmallFont, BackColor = Color.Transparent
        }, 0, row);
        table.Controls.Add(new Label
        {
            Text = value, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Theme.Text, Font = Theme.BaseFont, BackColor = Color.Transparent, AutoEllipsis = true
        }, 1, row);
    }

    private static string BuildClipboardText(TransferResult result, string round, string hashStatus)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Archivo: {result.FileName}");
        builder.AppendLine($"Ronda: {round}");
        builder.AppendLine($"Tamaño: {result.SizeBytes} bytes ({Metrics.FormatBytes(result.SizeBytes)})");
        builder.AppendLine($"Tiempo: {result.ElapsedSeconds.ToString("F3", CultureInfo.InvariantCulture)} s");
        builder.AppendLine($"Velocidad: {result.MegaBytesPerSecond.ToString("F2", CultureInfo.InvariantCulture)} MB/s ({result.MegabitsPerSecond.ToString("F2", CultureInfo.InvariantCulture)} Mb/s)");
        builder.AppendLine($"Tiempo ideal: {result.IdealSeconds.ToString("F3", CultureInfo.InvariantCulture)} s");
        builder.AppendLine($"% del límite: {result.PercentOfNominal.ToString("F1", CultureInfo.InvariantCulture)} %");
        builder.AppendLine($"SHA-256: {hashStatus}");
        return builder.ToString();
    }
}
