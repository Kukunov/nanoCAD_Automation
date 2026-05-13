using HostMgd.Windows.ToolPalette;
using NanoCAD.API.Commands;
using NanoCAD.API.Data;
using NanoCAD.API.Models;
using NanoCAD.API.Services;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Teigha.DatabaseServices;

namespace NanoCAD.API.Forms
{
    public partial class MainForm : Form
    {
        private static MainForm? _instance;
        private readonly BlockPreviewService _previewService = new();

        public MainForm()
        {
            InitializeComponent();
            InitializeBlockTypeComboBox();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Size = new Size(422, 463);
            this.MouseEnter += (s, e) => this.Activate();

            txtHelp.Text = string.Join("\r\n", HelpText.Content);
        }

        private void InitializeBlockTypeComboBox()
        {
            cmbBlockType.Items.Add(new { Text = "ПНЩ (Прибор на щите)", Value = "PNSH" });
            cmbBlockType.Items.Add(new { Text = "ПВЩ (Прибор вне щита)", Value = "PVSH" });
            cmbBlockType.DisplayMember = "Text";
            cmbBlockType.ValueMember = "Value";
            cmbBlockType.SelectedIndex = 0;

            cmbBlockType.SelectedIndexChanged += (s, e) =>
            {
                dynamic? item = cmbBlockType.SelectedItem;
                if (item != null)
                    _previewService.LoadPreview(pbBlockPreview, item.Value);
            };

            _previewService.LoadPreview(pbBlockPreview, "PNSH");
        }

        public static void ShowOrActivate()
        {
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new MainForm();
                _instance.TopMost = true;
                _instance.Show();
            }
            else
            {
                if (_instance.WindowState == FormWindowState.Minimized)
                    _instance.WindowState = FormWindowState.Normal;
                _instance.TopMost = true;
                _instance.Show();
                _instance.Activate();
            }
        }

        public static void CloseIfOpen()
        {
            _instance?.Close();
            _instance?.Dispose();
            _instance = null;
        }

        public static void UpdateDisplayIfOpen()
        {
            if (_instance == null || _instance.IsDisposed) return;
            _instance.UpdateDisplay();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            UpdateDisplay();
        }

        // Единый метод обновления дисплея
        private void UpdateDisplay()
        {
            try
            {
                var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (doc != null)
                {
                    var contourService = new ContourService(doc.Database);
                    lblCurrentContourValue.Text = contourService.GetCurrentContour().ToString();
                    lblNextElementValue.Text = contourService.GetNextElementPreview().ToString();
                    lblPositionValue.Text = contourService.GetNextPositionPreview().ToString();
                    nudContour.Value = contourService.GetCurrentContour();
                    nudElement.Value = contourService.GetNextElementPreview();

                    // ТИП обновляем только если поле пустое
                    if (string.IsNullOrEmpty(txtTypeDesignation.Text))
                        txtTypeDesignation.Text = contourService.GetLastTypeDesignation();

                    // Восстанавливаем состояние чекбокса
                    var colorService = new ContourColorService();
                    chbxContourColors.Checked = colorService.IsColorSchemeActive(doc.Database);
                }
                else
                {
                    lblCurrentContourValue.Text = "—";
                    lblNextElementValue.Text = "—";
                }
            }
            catch
            {
                lblCurrentContourValue.Text = "!";
                lblNextElementValue.Text = "!";
            }
        }

        private void StartInsertionLoop()
        {
            this.WindowState = FormWindowState.Minimized;

            string blockCode = ((dynamic)cmbBlockType.SelectedItem).Value;

            var options = new BlockInsertOptions
            {
                BlockName = blockCode == "PNSH" ? "ПриборНаЩите" : "ПриборВнеЩита",
                TypeDesignation = txtTypeDesignation.Text.ToUpperInvariant()
            };

            var blockCommands = new BlockCommands();
            blockCommands.InsertBlockLoop(options);

            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            UpdateDisplay();
        }

        private void btnChangeContour_Click(object sender, EventArgs e)
        {
            int newContour = (int)nudContour.Value;

            try
            {
                var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (doc == null) return;

                var service = new ContourService(doc.Database);
                service.SetCurrentContour(newContour);

                UpdateDisplay(); // обновить лейблы после смены
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResetCounter_Click(object sender, EventArgs e)
        {
            try
            {
                var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (doc == null) return;

                var contourService = new ContourService(doc.Database);
                DialogResult result = MessageBox.Show($"Сбросить нумерацию контура?\nВыбран контур: {contourService.GetCurrentContour()}", "Сбросить счет",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                contourService.ResetCurrentContour();
                UpdateDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (doc == null) return;

                string type = txtTypeDesignation.Text.ToUpperInvariant();

                // Используем валидацию
                var validation = ValidationService.ValidateTypeDesignation(type);
                if (!validation.IsValid)
                {
                    MessageBox.Show(validation.ErrorMessage, "Ошибка вставки",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTypeDesignation.Focus();
                    return;
                }

                var contourService = new ContourService(doc.Database);
                contourService.SetLastTypeDesignation(type);
                contourService.SetCurrentContour((int)nudContour.Value);

                // Запуск цикла вставки
                StartInsertionLoop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnContourInfo_Click(object sender, EventArgs e)
        {
            var contourCommands = new ContourCommands();
            string report = contourCommands.GetContourReport();
            MessageBox.Show(report, "Информация о контурах",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chbxContourColors_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (doc == null) return;

                var colorService = new ContourColorService();

                if (chbxContourColors.Checked)
                {
                    colorService.ApplyAutoColors(doc.Database);
                }
                else
                {
                    colorService.ClearContourColors(doc.Database);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Возвращаем чекбокс в предыдущее состояние
                chbxContourColors.CheckedChanged -= chbxContourColors_CheckedChanged;
                chbxContourColors.Checked = !chbxContourColors.Checked;
                chbxContourColors.CheckedChanged += chbxContourColors_CheckedChanged;
            }
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlMain.SelectedIndex)
            {
                case 0: // Вставка
                    this.Size = new Size(422, 463);
                    break;
                case 1: // Контуры
                    this.Size = new Size(422, 530);
                    RefreshContoursTab();
                    break;
                case 2: // Помощь
                    this.Size = new Size(422, 530);
                    break;
            }
        }

        private void tsbRefreshContours_Click(object sender, EventArgs e)
        {
            RefreshContoursTab();
        }

        private void RefreshContoursTab()
        {
            try
            {
                var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (doc == null) { tsslContourStats.Text = "Нет чертежа"; return; }

                var reportService = new ContourReportService(doc.Database);
                var rows = reportService.GetContourTableData();

                dgvContourSummary.Rows.Clear();
                foreach (var row in rows)
                {
                    dgvContourSummary.Rows.Add(row.ContourNumber, row.LastInsert,
                        row.ActualBlocks, row.Types, row.ColorName);
                }

                int total = rows.Sum(r => r.ActualBlocks);
                tsslContourStats.Text = $"Всего контуров: {rows.Count}, Всего приборов: {total}";
            }
            catch (Exception ex)
            {
                tsslContourStats.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void DgvContourSummary_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvContourSummary.SelectedRows.Count == 0) return;

            int contour = (int)dgvContourSummary.SelectedRows[0].Cells["Number"].Value;

            var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var reportService = new ContourReportService(doc.Database);
            var devices = reportService.GetDevicesForContour(contour);

            dgvDeviceList.Rows.Clear();
            foreach (var device in devices)
            {
                dgvDeviceList.Rows.Add(device.Position, device.BlockName, device.TypeDesignation);
            }
        }

        private void tsbExportTXT_Click(object sender, EventArgs e)
        {
            var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var reportService = new ContourReportService(doc.Database);
            string drawingName = doc?.Name ?? "Без чертежа";
            drawingName = Path.GetFileNameWithoutExtension(drawingName);

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                dialog.FileName = $"{drawingName}_Контуры_{DateTime.Now:ddMMyyyy_HHmmss}.txt";

                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var lines = reportService.GetContourReport();
                    File.WriteAllLines(dialog.FileName, lines, System.Text.Encoding.UTF8);
                    tsslContourStats.Text = $"Экспортировано: {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    tsslContourStats.Text = $"Ошибка экспорта: {ex.Message}";
                }
            }

            RefreshContoursTab();
        }

        private void tsbExportCSV_Click(object sender, EventArgs e)
        {
            var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var reportService = new ContourReportService(doc.Database);
            string drawingName = doc?.Name ?? "Без чертежа";
            drawingName = Path.GetFileNameWithoutExtension(drawingName);

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV файлы (*.csv)|*.csv";
                dialog.FileName = $"{drawingName}_Контуры_{DateTime.Now:ddMMyyyy_HHmmss}.csv";

                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var lines = reportService.GetContourCsv();
                    File.WriteAllLines(dialog.FileName, lines, System.Text.Encoding.UTF8);
                    tsslContourStats.Text = $"Экспортировано: {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    tsslContourStats.Text = $"Ошибка экспорта: {ex.Message}";
                }
            }

            RefreshContoursTab();
        }

        private void lnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = lnkGitHub.Text,
                UseShellExecute = true
            });
        }
    }
}