public partial class MainForm
{
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
