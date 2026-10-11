public partial class MainForm
{
    private void LoadSettings()
    {
        var settings = AppSettings.Load();
        txtDestino.Text = settings.Destination;
        txtEstudiante.Text = settings.Student;
        txtUsuario.Text = settings.User;
        txtPassword.Text = settings.Password;
        numLimiteMbps.Value = Math.Clamp(settings.LimitMbps, numLimiteMbps.Minimum, numLimiteMbps.Maximum);
        numBufferMb.Value = Math.Clamp(settings.BufferMb, numBufferMb.Minimum, numBufferMb.Maximum);
        numReintentos.Value = Math.Clamp(settings.Retries, numReintentos.Minimum, numReintentos.Maximum);
        chkCrearCarpetas.Checked = settings.CreateFolders;
        chkHash.Checked = settings.VerifyHash;
        chkMostrarResultado.Checked = settings.ShowResult;
        RefreshGaugeLimit();
    }

    private AppSettings CaptureSettings() => new()
    {
        Destination = txtDestino.Text.Trim(),
        Student = txtEstudiante.Text.Trim(),
        User = txtUsuario.Text.Trim(),
        Password = txtPassword.Text,
        LimitMbps = numLimiteMbps.Value,
        BufferMb = (int)numBufferMb.Value,
        Retries = (int)numReintentos.Value,
        CreateFolders = chkCrearCarpetas.Checked,
        VerifyHash = chkHash.Checked,
        ShowResult = chkMostrarResultado.Checked
    };

    private void RefreshResolvedPath()
    {
        var resolved = TransferPaths.BuildDestinationDirectory(
            txtDestino.Text.Trim(), txtEstudiante.Text, _subcarpeta, chkCrearCarpetas.Checked);
        lblRutaFinal.Text = "Ruta final: " + resolved;
        _toolTip.SetToolTip(lblRutaFinal, resolved);
    }

    private void RefreshSummary()
    {
        int count = lstArchivos.Items.Count;
        long total = lstArchivos.Items.Cast<ListViewItem>()
            .Sum(i => i.Tag is FileEntry f ? f.Bytes : 0);

        double limit = (double)numLimiteMbps.Value * 1_000_000.0;
        double ideal = Metrics.IdealSeconds(total, limit);

        lblResumen.Text = count == 0
            ? "Sin archivos. Arrástrelos aquí o use \"Agregar archivos...\"."
            : $"{count} archivo(s) · {Metrics.FormatBytes(total)} · ronda {_subcarpeta} · tiempo ideal estimado {ideal.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)} s";
    }

    private void UpdateRoundButtons()
    {
        foreach (var button in _roundButtons)
        {
            bool active = string.Equals(button.Tag as string, _subcarpeta, StringComparison.Ordinal);
            if (active)
                button.ApplyPrimary();
            else
                button.ApplySecondary();
        }
    }

    private void RefreshGaugeLimit()
    {
        double nominalBits = (double)numLimiteMbps.Value * 1_000_000.0;
        speedGauge.Maximum = Math.Max(1, nominalBits / 8.0 / 1_000_000.0);
    }

    private void SetTelemetryStatus(string status, Color color)
    {
        if (_telemetryCard == null)
            return;

        _telemetryCard.Title = "Telemetría de transferencia · " + status;
        _telemetryCard.Accent = color;
    }
}
