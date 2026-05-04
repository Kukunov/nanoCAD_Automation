using System;
using System.Drawing;
using System.Windows.Forms;
using NanoCAD.UI.Models;
using NanoCAD.UI.Services;

namespace NanoCAD.UI.Forms
{
    public partial class MainForm : Form
    {
        // Модель состояния (хранит все данные)
        private readonly UIModel _model;

        // Сервис связи с nanoCAD (отправка команд)
        private readonly NanoCADConnector _connector;

        // Сервис превью блоков (загрузка и кэширование изображений)
        private readonly BlockPreviewService _previewService;

        // Всплывающие подсказки
        private readonly ToolTip _toolTip;

        public MainForm()
        {
            InitializeComponent();

            _model = new UIModel();
            _connector = new NanoCADConnector(_model);
            _previewService = new BlockPreviewService(new Size(127, 127));
            _toolTip = new ToolTip();

            // Подписываемся на изменения модели
            _model.StateChanged += UpdateUI;
            _model.PropertyChanged += OnModelPropertyChanged;

            // Инициализация элементов
            InitializeBlockTypeComboBox();
            InitializeNumericUpDowns();
            InitializeToolTips();
            BindEvents();

            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        #region Инициализация элементов

        // Настройка выпадающего списка типов блоков
        private void InitializeBlockTypeComboBox()
        {
            cmbBlockType.DisplayMember = "Text";
            cmbBlockType.ValueMember = "Value";

            // Добавляем доступные типы блоков
            cmbBlockType.Items.Add(new { Text = "ПНЩ (Прибор на щите)", Value = "PNSH" });
            cmbBlockType.Items.Add(new { Text = "ПВЩ (Прибор вне щита)", Value = "PVSH" });

            cmbBlockType.SelectedIndex = 0;

            // Обработчик выбора — обновляем превью
            cmbBlockType.SelectedIndexChanged += (s, e) =>
            {
                dynamic? item = cmbBlockType.SelectedItem;
                if (item != null)
                {
                    _model.SelectedBlockCode = item.Value;
                    _model.SelectedBlockName = item.Text;
                }
                UpdateBlockPreview();
            };
        }

        // Настройка полей ввода чисел (контур и элемент)
        private void InitializeNumericUpDowns()
        {
            nudContour.Minimum = 1;
            nudContour.Maximum = 999;
            nudContour.Value = 1;

            nudElement.Minimum = 1;
            nudElement.Maximum = 999;
            nudElement.Value = 1;

            // ТИП по умолчанию
            txtTypeDesignation.Text = "TE";
            txtTypeDesignation.MaxLength = 4;
            txtTypeDesignation.CharacterCasing = CharacterCasing.Upper;
        }

        // Добавление всплывающих подсказок
        private void InitializeToolTips()
        {
            _toolTip.SetToolTip(cmbBlockType,
                "Выберите тип графического обозначения по ГОСТ 21.208-2013.\n\n" +
                "ПНЩ — Прибор на щите\n" +
                "ПВЩ — Прибор вне щита");

            _toolTip.SetToolTip(txtTypeDesignation,
                "Обозначение типа элемента.\n" +
                "Только латинские буквы, от 1 до 4 символов.\n\n" +
                "Примеры: TE, PE, LT, PI");

            _toolTip.SetToolTip(btnChangeContour,
                "Сменить текущий контур автоматизации.\n" +
                "Все последующие вставки будут в новом контуре.");

            _toolTip.SetToolTip(btnResetCounter,
                "Сбросить счётчик элементов в текущем контуре.\n" +
                "Нумерация начнётся заново с 1.");

            _toolTip.SetToolTip(pbBlockPreview,
                "Предварительный просмотр выбранного блока.");
        }

        #endregion

        #region Привязка событий

        private void BindEvents()
        {
            // Кнопки управления контуром
            btnChangeContour.Click += BtnChangeContour_Click;
            btnResetCounter.Click += BtnResetCounter_Click;

            // Изменение чисел
            nudContour.ValueChanged += NudContour_ValueChanged;
            nudElement.ValueChanged += NudElement_ValueChanged;

            // Изменение и валидация ТИП
            txtTypeDesignation.TextChanged += TxtTypeDesignation_TextChanged;

            // Кнопки OK и Отмена
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            // При закрытии — скрываем, а не закрываем
            this.FormClosing += MainForm_FormClosing;
        }

        #endregion

        #region Обработчики событий формы

        // Загрузка формы
        private async void MainForm_Load(object? sender, EventArgs e)
        {
            try
            {
                // Получаем состояние из nanoCAD
                await _connector.RefreshStateAsync();

                // Обновляем отображение
                UpdateContourDisplay();
                UpdateBlockPreview();
                UpdatePositionDisplay();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
        }

        // Скрываем форму
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "Закрыть панель GOST Automation?\n\n" +
                "Вы сможете открыть её снова командой ГОСТПАНЕЛЬ.",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                // Освобождаем ресурсы
                _previewService.Dispose();
            }
        }

        #endregion

        #region Обработчики кнопок

        // Смена контура
        private async void BtnChangeContour_Click(object? sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите номер нового контура (1-999):",
                "Смена контура",
                _model.CurrentContour.ToString(),
                -1, -1
            );

            if (string.IsNullOrEmpty(input)) return;

            if (int.TryParse(input, out int newContour) && newContour >= 1 && newContour <= 999)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    btnChangeContour.Enabled = false;

                    await _connector.SetCurrentContourAsync(newContour);
                    await _connector.RefreshStateAsync();

                    _model.CurrentContour = newContour;
                    UpdateContourDisplay();
                    nudContour.Value = newContour;
                    nudElement.Value = _model.NextElementNumber;
                    UpdatePositionDisplay();

                    _toolTip.Show($"Контур изменён на {newContour}", btnChangeContour, 0, -30, 2000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                    btnChangeContour.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Номер контура должен быть от 1 до 999.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Сброс счётчика контура
        private async void BtnResetCounter_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                $"Сбросить счётчик элементов контура {_model.CurrentContour}?\n\n" +
                "Нумерация в этом контуре начнётся с 1.",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            );

            if (result != DialogResult.Yes) return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnResetCounter.Enabled = false;

                await _connector.ResetContourAsync(_model.CurrentContour);
                await _connector.RefreshStateAsync();

                UpdateContourDisplay();
                nudElement.Value = 1;
                UpdatePositionDisplay();

                _toolTip.Show($"Счётчик контура {_model.CurrentContour} сброшен",
                    btnResetCounter, 0, -30, 2000);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnResetCounter.Enabled = true;
            }
        }

        // Кнопка OK — вставка блока
        private async void BtnOK_Click(object? sender, EventArgs e)
        {
            if (!ValidateAllInputs()) return;

            // Сохраняем данные в модель
            _model.TypeDesignation = txtTypeDesignation.Text.ToUpperInvariant();
            _model.CurrentContour = (int)nudContour.Value;
            _model.ElementNumber = (int)nudElement.Value;
            _model.Position = $"{nudContour.Value}-{nudElement.Value}";

            try
            {
                // Скрываем окно, чтобы не мешало указывать точку в nanoCAD
                this.WindowState = FormWindowState.Minimized;
                await System.Threading.Tasks.Task.Delay(200);

                // Отправляем команду вставки (nanoCAD запросит точку)
                await _connector.InsertBlockAsync();

                // Ждём завершения
                await System.Threading.Tasks.Task.Delay(500);

                // Обновляем состояние
                await _connector.RefreshStateAsync();

                // Показываем окно снова
                this.Show();
                this.Activate();

                // Обновляем дисплей
                UpdateContourDisplay();
                nudElement.Value = _model.NextElementNumber;
                UpdatePositionDisplay();

                _toolTip.Show("Блок успешно вставлен", btnOK, 0, -30, 2000);
            }
            catch (Exception ex)
            {
                this.Show();
                MessageBox.Show($"Ошибка при вставке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка Отмена — скрытие формы
        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Обработчики полей ввода

        // Изменение номера контура
        private void NudContour_ValueChanged(object? sender, EventArgs e)
        {
            if (_model.CurrentContour != (int)nudContour.Value)
            {
                _model.CurrentContour = (int)nudContour.Value;
                UpdatePositionDisplay();
            }
        }

        // Изменение номера элемента
        private void NudElement_ValueChanged(object? sender, EventArgs e)
        {
            if (_model.ElementNumber != (int)nudElement.Value)
            {
                _model.ElementNumber = (int)nudElement.Value;
                UpdatePositionDisplay();
            }
        }

        // Изменение текста ТИП
        private void TxtTypeDesignation_TextChanged(object? sender, EventArgs e)
        {
            ValidateTypeDesignationVisual();
        }

        #endregion

        #region Обработчики изменений модели

        // Реакция на изменение конкретного свойства модели
        private void OnModelPropertyChanged(string propertyName)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(OnModelPropertyChanged), propertyName);
                return;
            }

            switch (propertyName)
            {
                case nameof(UIModel.CurrentContour):
                case nameof(UIModel.NextElementNumber):
                    UpdateContourDisplay();
                    break;
                case nameof(UIModel.ElementNumber):
                    UpdatePositionDisplay();
                    break;
                case nameof(UIModel.SelectedBlockCode):
                    UpdateBlockPreview();
                    break;
            }
        }

        // Реакция на любое изменение модели
        private void UpdateUI()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateUI));
                return;
            }

            UpdateContourDisplay();
            UpdatePositionDisplay();
        }

        #endregion

        #region Валидация

        // Визуальная индикация правильности ТИП
        private bool ValidateTypeDesignationVisual()
        {
            string input = txtTypeDesignation.Text;
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(input, @"^[A-Za-z]{1,4}$");

            txtTypeDesignation.BackColor = isValid || string.IsNullOrEmpty(input)
                ? SystemColors.Window
                : Color.LightPink;

            return isValid;
        }

        // Проверка всех полей перед вставкой
        private bool ValidateAllInputs()
        {
            if (!ValidateTypeDesignationVisual())
            {
                MessageBox.Show(
                    "Обозначение типа (ТИП) должно содержать только латинские буквы (1-4 символа).\n\n" +
                    "Примеры: TE, PE, PI, LT",
                    "Ошибка валидации",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTypeDesignation.Focus();
                return false;
            }

            if (nudContour.Value < 1 || nudContour.Value > 999)
            {
                MessageBox.Show("Номер контура должен быть от 1 до 999.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudContour.Focus();
                return false;
            }

            if (nudElement.Value < 1 || nudElement.Value > 999)
            {
                MessageBox.Show("Номер элемента должен быть от 1 до 999.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudElement.Focus();
                return false;
            }

            return true;
        }

        #endregion

        #region Обновление отображения

        // Обновить информацию о контуре (текущий и следующий)
        private void UpdateContourDisplay()
        {
            lblCurrentContourValue.Text = _model.CurrentContour.ToString();
            lblNextElementValue.Text = $"{_model.CurrentContour}-{_model.NextElementNumber}";
        }

        // Обновить позиционное обозначение
        private void UpdatePositionDisplay()
        {
            lblPositionValue.Text = $"{nudContour.Value}-{nudElement.Value}";
        }

        // Обновить превью блока
        private void UpdateBlockPreview()
        {
            dynamic? item = cmbBlockType.SelectedItem;
            if (item == null) return;

            string blockCode = item.Value;
            Bitmap? preview = _previewService.GetBlockPreview(blockCode);

            if (preview != null)
            {
                pbBlockPreview.Image = preview;
            }
        }

        #endregion
    }
}
