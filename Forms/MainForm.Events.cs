public partial class MainForm
{
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
}
