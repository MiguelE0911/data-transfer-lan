public partial class MainForm
{
    private void ToggleTelemetryExpanded()
    {
        if (_telemetryCard == null || _outerLayout == null)
            return;

        if (!_telemetryExpanded)
            ExpandTelemetry();
        else
            RestoreTelemetry();

        UpdateZoomButton();
    }

    private void ExpandTelemetry()
    {
        // Guarda el contenedor original para devolver la tarjeta a su sitio.
        _telemetryHome = _telemetryCard!.Parent;
        _telemetryHome?.Controls.Remove(_telemetryCard);

        _telemetryCard.CornerRadius = 0;
        _telemetryCard.Margin = new Padding(0);
        _telemetryCard.Dock = DockStyle.Fill;

        Controls.Add(_telemetryCard);
        _telemetryCard.BringToFront();
        _outerLayout!.Visible = false;
        _telemetryExpanded = true;
    }

    private void RestoreTelemetry()
    {
        Controls.Remove(_telemetryCard);

        _telemetryCard!.CornerRadius = 12;
        _telemetryCard.Margin = new Padding(0, 0, 0, 8);
        _telemetryCard.Dock = DockStyle.Fill;

        if (_telemetryHome is TableLayoutPanel table)
            table.Controls.Add(_telemetryCard, 0, 0);
        else
            _telemetryHome?.Controls.Add(_telemetryCard);

        _outerLayout!.Visible = true;
        _telemetryExpanded = false;
    }

    private void UpdateZoomButton()
    {
        if (_btnZoom == null)
            return;

        _btnZoom.Glyph = _telemetryExpanded ? GlyphRestore : GlyphExpand;
        _toolTip.SetToolTip(_btnZoom, _telemetryExpanded ? "Restaurar telemetría" : "Expandir telemetría");
    }

    // Coloca un botón de icono en la esquina superior derecha de una tarjeta y lo
    // reposiciona cuando la tarjeta cambia de tamaño.
    private void AttachCornerButton(Card card, FlatButton button, string tooltip)
    {
        button.Height = 28;
        button.CornerRadius = 6;
        button.GlyphSize = 14;
        button.Margin = new Padding(0);
        card.Controls.Add(button);
        button.BringToFront();

        void Place()
        {
            int x = card.ClientSize.Width - button.Width - 16;
            button.Location = new Point(Math.Max(0, x), 10);
        }

        card.Resize += (_, _) => Place();
        Place();
        _toolTip.SetToolTip(button, tooltip);
    }
}
