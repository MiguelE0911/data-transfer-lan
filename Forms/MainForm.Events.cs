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
        if (sender is not Button button || button.Tag is not string round)
            return;

        _subcarpeta = round;
        UpdateRoundButtons();
        RefreshResolvedPath();
        RefreshSummary();
        AppendLog($"Subcarpeta activa: {_subcarpeta}");
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

    private void BtnDesconectar_Click(object? sender, EventArgs e)
    {
        var unc = txtDestino.Text.Trim();
        if (string.IsNullOrWhiteSpace(unc))
        {
            AppendLog("La ruta de destino está vacía.");
            return;
        }

        bool ok = SmbConnection.Disconnect(unc, out var message);
        AppendLog($"Desconexión de {unc}: {(ok ? "correcta" : "con problemas")}. {message}");
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
                RefreshSummary();
                RefreshGaugeLimit();
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

        AddPaths(dialog.FileNames);
    }

    private void BtnAgregarCarpeta_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Seleccione la carpeta con los archivos a copiar",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = false
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            AddPaths(new[] { dialog.SelectedPath });
    }

    private void BtnQuitar_Click(object? sender, EventArgs e)
    {
        foreach (ListViewItem item in lstArchivos.SelectedItems.Cast<ListViewItem>().ToList())
            lstArchivos.Items.Remove(item);
        RefreshSummary();
    }

    private void LstArchivos_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            e.Effect = DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
    }

    private void LstArchivos_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] paths)
            AddPaths(paths);
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        if (_cts == null || _cts.IsCancellationRequested)
            return;

        var answer = MessageBox.Show(this,
            "¿Cancelar la copia en curso? El archivo parcial se eliminará.",
            "Cancelar transferencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer == DialogResult.Yes)
        {
            _cts.Cancel();
            AppendLog("Cancelación solicitada por el usuario...");
        }
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
        SetTelemetryStatus("en curso", Theme.Accent);
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        bool anyFailure = false;

        try
        {
            double nominalBitsPerSecond = (double)numLimiteMbps.Value * 1_000_000.0;
            double nominalMegaBytes = nominalBitsPerSecond / 8.0 / 1_000_000.0;
            int bufferSize = (int)numBufferMb.Value * 1024 * 1024;
            int maxRetries = (int)numReintentos.Value;

            var items = lstArchivos.Items.Cast<ListViewItem>()
                .Where(i => i.Tag is FileEntry)
                .ToList();
            _runTotal = items.Count;
            _runDone = 0;
            progressTanda.Maximum = 100;
            progressTanda.Value = 0;
            lblTanda.Text = $"0/{_runTotal} archivos";

            foreach (var item in items)
            {
                if (token.IsCancellationRequested)
                    break;
                if (item.Tag is not FileEntry entry)
                    continue;

                SetItemState(item, "Copiando...", Theme.Warning);

                var destinationDirectory = TransferPaths.BuildDestinationDirectory(
                    destinoBase, txtEstudiante.Text, _subcarpeta, chkCrearCarpetas.Checked);
                var destinationFile = Path.Combine(destinationDirectory, Path.GetFileName(entry.Path));

                var engine = new TransferEngine
                {
                    BufferSize = bufferSize,
                    VerifyHash = chkHash.Checked,
                    NominalBitsPerSecond = nominalBitsPerSecond
                };

                speedChart.Reset();
                speedChart.Subtitle = entry.Path;
                speedChart.NominalMegaBytesPerSecond = nominalMegaBytes;
                speedGauge.Maximum = Math.Max(1, nominalMegaBytes);
                speedGauge.Value = 0;
                speedGauge.Detail = $"0.0 Mb/s · 0.0 % del límite";

                var progress = new Progress<ProgressInfo>(info =>
                {
                    progressArchivo.Maximum = 100;
                    progressArchivo.Value = Math.Clamp(info.PercentComplete, 0, 100);

                    double average = info.ElapsedSeconds > 0
                        ? info.BytesCopied / (info.ElapsedSeconds * 1_000_000.0)
                        : 0;
                    double eta = average > 0 && info.TotalBytes > info.BytesCopied
                        ? (info.TotalBytes - info.BytesCopied) / (average * 1_000_000.0)
                        : 0;
                    lblVelocidad.Text = $"Instantánea: {info.InstantMegaBytesPerSecond:F2} MB/s · ETA {eta:F1} s";

                    speedChart.AddSample(info.ElapsedSeconds, info.InstantMegaBytesPerSecond);
                    speedChart.AverageMegaBytesPerSecond = average;
                    speedGauge.Value = average;
                    double percent = nominalBitsPerSecond > 0 ? average * 8.0 * 1_000.0 / nominalBitsPerSecond * 100.0 : 0;
                    speedGauge.Detail = $"{average * 8.0:F1} Mb/s · {percent:F1} % del límite";

                    double fraction = info.PercentComplete / 100.0;
                    progressTanda.Value = (int)Math.Round(100.0 * (_runDone + fraction) / Math.Max(1, _runTotal));
                });

                AppendLog($"Copiando \"{entry.Path}\" -> \"{destinationFile}\"");

                TransferResult result;
                int attempt = 0;
                while (true)
                {
                    result = await engine.CopyAsync(entry.Path, destinationFile, progress, token);
                    if (result.Success || token.IsCancellationRequested || attempt >= maxRetries)
                        break;

                    attempt++;
                    AppendLog($"Reintento {attempt}/{maxRetries} de \"{result.FileName}\": {result.Error}");
                    TryDelete(destinationFile);
                    try
                    {
                        await Task.Delay(400, token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                if (!result.Success)
                    TryDelete(destinationFile);

                _resultados.Add(result);
                AddResultRow(result);
                LogResult(result);

                if (result.Success)
                {
                    _runDone++;
                    SetItemState(item, "Listo", Theme.OkText);
                }
                else
                {
                    anyFailure = true;
                    SetItemState(item, "Fallo", Theme.FailText);
                }

                speedGauge.Value = result.MegaBytesPerSecond;
                speedChart.AverageMegaBytesPerSecond = result.MegaBytesPerSecond;
                progressArchivo.Value = result.Success ? 100 : 0;
                double finalPercent = nominalBitsPerSecond > 0
                    ? result.MegabitsPerSecond * 1_000_000.0 / nominalBitsPerSecond * 100.0
                    : 0;
                lblVelocidad.Text = $"{result.MegaBytesPerSecond:F2} MB/s · {result.MegabitsPerSecond:F1} Mb/s";
                speedGauge.Detail = $"{result.MegabitsPerSecond:F1} Mb/s · {finalPercent:F1} % del límite";
                progressTanda.Value = (int)Math.Round(100.0 * _runDone / Math.Max(1, _runTotal));
                lblTanda.Text = $"{_runDone}/{_runTotal} archivos";

                if (chkMostrarResultado.Checked && !token.IsCancellationRequested)
                    ShowResult(result);
            }

            if (token.IsCancellationRequested)
            {
                AppendLog("Copia cancelada por el usuario.");
                SetTelemetryStatus("cancelada", Theme.Warning);
            }
            else if (anyFailure)
            {
                AppendLog("Copia finalizada con errores.");
                SetTelemetryStatus("con errores", Theme.Danger);
            }
            else
            {
                AppendLog("Copia finalizada.");
                SetTelemetryStatus("completada", Theme.Success);
            }
        }
        catch (Exception ex)
        {
            AppendLog("ERROR durante la copia: " + ex.Message);
            SetTelemetryStatus("con errores", Theme.Danger);
        }
        finally
        {
            SetRunning(false);
            _cts?.Dispose();
            _cts = null;
        }
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

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Escape)
            return;

        if (_telemetryExpanded)
        {
            ToggleTelemetryExpanded();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        else if (_cts != null)
        {
            BtnCancelar_Click(this, EventArgs.Empty);
            e.Handled = true;
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_cts != null)
        {
            var answer = MessageBox.Show(this,
                "Hay una transferencia en curso. ¿Desea salir de todos modos?",
                "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            _cts.Cancel();
        }

        CaptureSettings().Save();
    }

    private void AddPaths(IEnumerable<string> paths)
    {
        int added = 0;
        foreach (var path in paths)
        {
            if (File.Exists(path))
            {
                if (AddFile(path))
                    added++;
            }
            else if (Directory.Exists(path))
            {
                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
                }
                catch
                {
                    continue;
                }

                foreach (var file in files)
                {
                    if (AddFile(file))
                        added++;
                }
            }
        }

        if (added > 0)
            AppendLog($"{added} archivo(s) agregado(s) a la lista.");
        RefreshSummary();
    }

    private bool AddFile(string path)
    {
        if (lstArchivos.Items.Cast<ListViewItem>()
            .Any(i => i.Tag is FileEntry f && string.Equals(f.Path, path, StringComparison.OrdinalIgnoreCase)))
            return false;

        long bytes = 0;
        try { bytes = new FileInfo(path).Length; }
        catch { }

        var item = new ListViewItem(Path.GetFileName(path))
        {
            Tag = new FileEntry { Path = path, Bytes = bytes },
            ForeColor = Theme.Text
        };
        item.SubItems.Add(Metrics.FormatBytes(bytes));
        item.SubItems.Add("Pendiente");
        lstArchivos.Items.Add(item);
        return true;
    }

    private void SetItemState(ListViewItem item, string state, Color color)
    {
        if (item.SubItems.Count > 2)
            item.SubItems[2].Text = state;
        item.ForeColor = color;
        lstArchivos.Invalidate();
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // El archivo parcial puede seguir bloqueado; se ignora.
        }
    }

    private void ShowResult(TransferResult result)
    {
        using var dialog = new ResultDialog(result, chkHash.Checked, _subcarpeta);
        dialog.ShowDialog(this);
    }

    private void SetRunning(bool running)
    {
        btnCopiar.Enabled = !running;
        btnCancelar.Enabled = running;
        btnDetectar.Enabled = !running;
        btnConectar.Enabled = !running;
        btnDesconectar.Enabled = !running;
        btnVerificar.Enabled = !running;
        btnAgregarArchivos.Enabled = !running;
        btnAgregarCarpeta.Enabled = !running;
        btnQuitar.Enabled = !running;
        numLimiteMbps.Enabled = !running;
        numBufferMb.Enabled = !running;
        numReintentos.Enabled = !running;
        chkHash.Enabled = !running;
        foreach (var button in _roundButtons)
            button.Enabled = !running;
    }
}
