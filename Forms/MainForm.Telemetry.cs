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

        LayoutZoomButton();
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

    private void LayoutZoomButton()
    {
        if (_telemetryCard == null || _btnZoom == null)
            return;

        int x = _telemetryCard.ClientSize.Width - _btnZoom.Width - 16;
        _btnZoom.Location = new Point(Math.Max(0, x), 10);
    }

    private void UpdateZoomButton()
    {
        if (_btnZoom == null)
            return;

        _btnZoom.Glyph = _telemetryExpanded ? GlyphRestore : GlyphExpand;
        _toolTip.SetToolTip(_btnZoom, _telemetryExpanded ? "Restaurar telemetría" : "Expandir telemetría");
    }
}
