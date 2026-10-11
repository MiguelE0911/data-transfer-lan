using System.Globalization;

public partial class MainForm
{
    private void AddResultRow(TransferResult result)
    {
        int index = dgvResultados.Rows.Add(
            result.FileName,
            result.SizeBytes.ToString(CultureInfo.InvariantCulture),
            result.ElapsedSeconds.ToString("F3", CultureInfo.InvariantCulture),
            result.MegaBytesPerSecond.ToString("F2", CultureInfo.InvariantCulture),
            result.MegabitsPerSecond.ToString("F2", CultureInfo.InvariantCulture),
            result.IdealSeconds.ToString("F3", CultureInfo.InvariantCulture),
            result.PercentOfNominal.ToString("F1", CultureInfo.InvariantCulture),
            chkHash.Checked ? (result.HashVerified ? "OK" : "NO") : "-");

        if (index < 0 || index >= dgvResultados.Rows.Count)
            return;

        var row = dgvResultados.Rows[index];
        if (!result.Success)
        {
            row.DefaultCellStyle.ForeColor = Theme.FailText;
            row.DefaultCellStyle.BackColor = Theme.RowFail;
        }
        else if (chkHash.Checked && !result.HashVerified)
        {
            row.DefaultCellStyle.ForeColor = Theme.Warning;
        }
    }

    private void ClearResults()
    {
        if (_cts != null)
            return;
        if (dgvResultados.Rows.Count == 0 && _resultados.Count == 0)
            return;

        var answer = MessageBox.Show(this, "¿Limpiar la tabla de resultados?", "Limpiar resultados",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer != DialogResult.Yes)
            return;

        dgvResultados.Rows.Clear();
        _resultados.Clear();
        foreach (ListViewItem item in lstArchivos.Items)
            SetItemState(item, "Pendiente", Theme.Text);
        AppendLog("Resultados limpiados.");
    }

    private void DgvResultados_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= dgvResultados.Rows.Count)
            return;

        var row = dgvResultados.Rows[e.RowIndex];
        var values = row.Cells.Cast<DataGridViewCell>().Select(c => c.Value?.ToString() ?? "");
        try
        {
            Clipboard.SetText(string.Join('\t', values));
            AppendLog($"Fila copiada al portapapeles: {row.Cells[0].Value}");
        }
        catch (Exception ex)
        {
            AppendLog("No se pudo copiar la fila: " + ex.Message);
        }
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
}
