using NanoCAD.API.Services;

namespace NanoCAD.API.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tabControlMain = new TabControl();
            tabInsert = new TabPage();
            chbxContourColors = new CheckBox();
            btnCancel = new Button();
            btnOK = new Button();
            gbInsertAttributes = new GroupBox();
            btnResetCounter = new Button();
            nudElement = new NumericUpDown();
            nudContour = new NumericUpDown();
            lblPositionValue = new Label();
            lblPositionCaption = new Label();
            lblElementNumber = new Label();
            lblContourNumber = new Label();
            txtTypeDesignation = new TextBox();
            lblTypeDesignation = new Label();
            gbBlockSelection = new GroupBox();
            pbBlockPreview = new PictureBox();
            cmbBlockType = new ComboBox();
            btnContourInfo = new Button();
            lblCurrentContourCaption = new Label();
            lblCurrentContourValue = new Label();
            lblNextElementValue = new Label();
            lblNextElementCaption = new Label();
            btnChangeContour = new Button();
            tabContours = new TabPage();
            statusStripContours = new StatusStrip();
            tsslContourStats = new ToolStripStatusLabel();
            panelContoursBottom = new Panel();
            dgvDeviceList = new DataGridView();
            Position = new DataGridViewTextBoxColumn();
            Block = new DataGridViewTextBoxColumn();
            Type = new DataGridViewTextBoxColumn();
            lblDeviceListTitle = new Label();
            panelContoursTop = new Panel();
            dgvContourSummary = new DataGridView();
            Number = new DataGridViewTextBoxColumn();
            Counter = new DataGridViewTextBoxColumn();
            Actual = new DataGridViewTextBoxColumn();
            Types = new DataGridViewTextBoxColumn();
            Colors = new DataGridViewTextBoxColumn();
            lblContourSummaryTitle = new Label();
            toolStripContours = new ToolStrip();
            tsbRefreshContours = new ToolStripButton();
            tsbExportCSV = new ToolStripButton();
            tsbExportTXT = new ToolStripButton();
            tabHelp = new TabPage();
            pictureBox1 = new PictureBox();
            lnkGitHub = new LinkLabel();
            lblVersion = new Label();
            txtHelp = new TextBox();
            tabControlMain.SuspendLayout();
            tabInsert.SuspendLayout();
            gbInsertAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudElement).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudContour).BeginInit();
            gbBlockSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbBlockPreview).BeginInit();
            tabContours.SuspendLayout();
            statusStripContours.SuspendLayout();
            panelContoursBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDeviceList).BeginInit();
            panelContoursTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvContourSummary).BeginInit();
            toolStripContours.SuspendLayout();
            tabHelp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabInsert);
            tabControlMain.Controls.Add(tabContours);
            tabControlMain.Controls.Add(tabHelp);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(406, 491);
            tabControlMain.TabIndex = 0;
            tabControlMain.SelectedIndexChanged += tabControlMain_SelectedIndexChanged;
            // 
            // tabInsert
            // 
            tabInsert.Controls.Add(chbxContourColors);
            tabInsert.Controls.Add(btnCancel);
            tabInsert.Controls.Add(btnOK);
            tabInsert.Controls.Add(gbInsertAttributes);
            tabInsert.Controls.Add(gbBlockSelection);
            tabInsert.Location = new Point(4, 24);
            tabInsert.Name = "tabInsert";
            tabInsert.Padding = new Padding(3);
            tabInsert.Size = new Size(398, 463);
            tabInsert.TabIndex = 0;
            tabInsert.Text = "Вставка";
            tabInsert.UseVisualStyleBackColor = true;
            // 
            // chbxContourColors
            // 
            chbxContourColors.AutoSize = true;
            chbxContourColors.Location = new Point(14, 360);
            chbxContourColors.Name = "chbxContourColors";
            chbxContourColors.RightToLeft = RightToLeft.No;
            chbxContourColors.Size = new Size(112, 19);
            chbxContourColors.TabIndex = 8;
            chbxContourColors.Text = "Цвета контуров";
            chbxContourColors.UseVisualStyleBackColor = true;
            chbxContourColors.CheckedChanged += chbxContourColors_CheckedChanged;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(286, 354);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 28);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(161, 354);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(115, 28);
            btnOK.TabIndex = 9;
            btnOK.Text = "ОК";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // gbInsertAttributes
            // 
            gbInsertAttributes.Controls.Add(btnResetCounter);
            gbInsertAttributes.Controls.Add(nudElement);
            gbInsertAttributes.Controls.Add(nudContour);
            gbInsertAttributes.Controls.Add(lblPositionValue);
            gbInsertAttributes.Controls.Add(lblPositionCaption);
            gbInsertAttributes.Controls.Add(lblElementNumber);
            gbInsertAttributes.Controls.Add(lblContourNumber);
            gbInsertAttributes.Controls.Add(txtTypeDesignation);
            gbInsertAttributes.Controls.Add(lblTypeDesignation);
            gbInsertAttributes.Location = new Point(8, 178);
            gbInsertAttributes.Name = "gbInsertAttributes";
            gbInsertAttributes.Size = new Size(384, 165);
            gbInsertAttributes.TabIndex = 8;
            gbInsertAttributes.TabStop = false;
            gbInsertAttributes.Text = "Атрибуты вставки";
            // 
            // btnResetCounter
            // 
            btnResetCounter.Location = new Point(278, 24);
            btnResetCounter.Name = "btnResetCounter";
            btnResetCounter.Size = new Size(100, 26);
            btnResetCounter.TabIndex = 6;
            btnResetCounter.Text = "Сбросить счет";
            btnResetCounter.UseVisualStyleBackColor = true;
            btnResetCounter.Click += btnResetCounter_Click;
            // 
            // nudElement
            // 
            nudElement.Location = new Point(153, 90);
            nudElement.Name = "nudElement";
            nudElement.Size = new Size(76, 23);
            nudElement.TabIndex = 7;
            // 
            // nudContour
            // 
            nudContour.Location = new Point(153, 59);
            nudContour.Name = "nudContour";
            nudContour.Size = new Size(76, 23);
            nudContour.TabIndex = 6;
            // 
            // lblPositionValue
            // 
            lblPositionValue.AutoSize = true;
            lblPositionValue.Location = new Point(181, 130);
            lblPositionValue.Name = "lblPositionValue";
            lblPositionValue.Size = new Size(12, 15);
            lblPositionValue.TabIndex = 5;
            lblPositionValue.Text = "-";
            // 
            // lblPositionCaption
            // 
            lblPositionCaption.AutoSize = true;
            lblPositionCaption.Location = new Point(11, 130);
            lblPositionCaption.Name = "lblPositionCaption";
            lblPositionCaption.Size = new Size(164, 15);
            lblPositionCaption.TabIndex = 4;
            lblPositionCaption.Text = "Позиционное обозначение: ";
            // 
            // lblElementNumber
            // 
            lblElementNumber.AutoSize = true;
            lblElementNumber.Location = new Point(21, 92);
            lblElementNumber.Name = "lblElementNumber";
            lblElementNumber.Size = new Size(103, 15);
            lblElementNumber.TabIndex = 3;
            lblElementNumber.Text = "Номер элемента:";
            // 
            // lblContourNumber
            // 
            lblContourNumber.AutoSize = true;
            lblContourNumber.Location = new Point(21, 61);
            lblContourNumber.Name = "lblContourNumber";
            lblContourNumber.Size = new Size(98, 15);
            lblContourNumber.TabIndex = 2;
            lblContourNumber.Text = "Номер контура: ";
            // 
            // txtTypeDesignation
            // 
            txtTypeDesignation.Location = new Point(153, 27);
            txtTypeDesignation.Name = "txtTypeDesignation";
            txtTypeDesignation.Size = new Size(76, 23);
            txtTypeDesignation.TabIndex = 1;
            // 
            // lblTypeDesignation
            // 
            lblTypeDesignation.AutoSize = true;
            lblTypeDesignation.Location = new Point(21, 30);
            lblTypeDesignation.Name = "lblTypeDesignation";
            lblTypeDesignation.Size = new Size(88, 15);
            lblTypeDesignation.TabIndex = 0;
            lblTypeDesignation.Text = "Тип элемента: ";
            // 
            // gbBlockSelection
            // 
            gbBlockSelection.Controls.Add(pbBlockPreview);
            gbBlockSelection.Controls.Add(cmbBlockType);
            gbBlockSelection.Controls.Add(btnContourInfo);
            gbBlockSelection.Controls.Add(lblCurrentContourCaption);
            gbBlockSelection.Controls.Add(lblCurrentContourValue);
            gbBlockSelection.Controls.Add(lblNextElementValue);
            gbBlockSelection.Controls.Add(lblNextElementCaption);
            gbBlockSelection.Controls.Add(btnChangeContour);
            gbBlockSelection.Location = new Point(8, 6);
            gbBlockSelection.Name = "gbBlockSelection";
            gbBlockSelection.Size = new Size(384, 166);
            gbBlockSelection.TabIndex = 7;
            gbBlockSelection.TabStop = false;
            gbBlockSelection.Text = "Выбор блока";
            // 
            // pbBlockPreview
            // 
            pbBlockPreview.Location = new Point(6, 22);
            pbBlockPreview.Name = "pbBlockPreview";
            pbBlockPreview.Size = new Size(127, 127);
            pbBlockPreview.TabIndex = 7;
            pbBlockPreview.TabStop = false;
            // 
            // cmbBlockType
            // 
            cmbBlockType.FormattingEnabled = true;
            cmbBlockType.Location = new Point(153, 22);
            cmbBlockType.Name = "cmbBlockType";
            cmbBlockType.Size = new Size(225, 23);
            cmbBlockType.TabIndex = 0;
            // 
            // btnContourInfo
            // 
            btnContourInfo.Location = new Point(278, 123);
            btnContourInfo.Name = "btnContourInfo";
            btnContourInfo.Size = new Size(100, 26);
            btnContourInfo.TabIndex = 8;
            btnContourInfo.Text = "Контур инфо";
            btnContourInfo.UseVisualStyleBackColor = true;
            btnContourInfo.Click += btnContourInfo_Click;
            // 
            // lblCurrentContourCaption
            // 
            lblCurrentContourCaption.AutoSize = true;
            lblCurrentContourCaption.Location = new Point(153, 61);
            lblCurrentContourCaption.Name = "lblCurrentContourCaption";
            lblCurrentContourCaption.Size = new Size(103, 15);
            lblCurrentContourCaption.TabIndex = 1;
            lblCurrentContourCaption.Text = "Текущий контур: ";
            // 
            // lblCurrentContourValue
            // 
            lblCurrentContourValue.AutoSize = true;
            lblCurrentContourValue.Location = new Point(297, 61);
            lblCurrentContourValue.Name = "lblCurrentContourValue";
            lblCurrentContourValue.Size = new Size(12, 15);
            lblCurrentContourValue.TabIndex = 3;
            lblCurrentContourValue.Text = "-";
            // 
            // lblNextElementValue
            // 
            lblNextElementValue.Anchor = AnchorStyles.None;
            lblNextElementValue.AutoSize = true;
            lblNextElementValue.Location = new Point(297, 87);
            lblNextElementValue.Name = "lblNextElementValue";
            lblNextElementValue.Size = new Size(12, 15);
            lblNextElementValue.TabIndex = 4;
            lblNextElementValue.Text = "-";
            // 
            // lblNextElementCaption
            // 
            lblNextElementCaption.AutoSize = true;
            lblNextElementCaption.Location = new Point(153, 87);
            lblNextElementCaption.Name = "lblNextElementCaption";
            lblNextElementCaption.Size = new Size(128, 15);
            lblNextElementCaption.TabIndex = 2;
            lblNextElementCaption.Text = "Следующая позиция: ";
            // 
            // btnChangeContour
            // 
            btnChangeContour.Location = new Point(153, 123);
            btnChangeContour.Name = "btnChangeContour";
            btnChangeContour.Size = new Size(115, 26);
            btnChangeContour.TabIndex = 5;
            btnChangeContour.Text = "Сменить контур";
            btnChangeContour.UseVisualStyleBackColor = true;
            btnChangeContour.Click += btnChangeContour_Click;
            // 
            // tabContours
            // 
            tabContours.Controls.Add(statusStripContours);
            tabContours.Controls.Add(panelContoursBottom);
            tabContours.Controls.Add(panelContoursTop);
            tabContours.Controls.Add(toolStripContours);
            tabContours.Location = new Point(4, 24);
            tabContours.Name = "tabContours";
            tabContours.Padding = new Padding(3);
            tabContours.Size = new Size(398, 463);
            tabContours.TabIndex = 1;
            tabContours.Text = "Контуры";
            tabContours.UseVisualStyleBackColor = true;
            // 
            // statusStripContours
            // 
            statusStripContours.Items.AddRange(new ToolStripItem[] { tsslContourStats });
            statusStripContours.Location = new Point(3, 438);
            statusStripContours.Name = "statusStripContours";
            statusStripContours.Size = new Size(392, 22);
            statusStripContours.TabIndex = 3;
            // 
            // tsslContourStats
            // 
            tsslContourStats.Name = "tsslContourStats";
            tsslContourStats.Size = new Size(217, 17);
            tsslContourStats.Text = "Всего контуров: ..., Всего приборов: ...";
            // 
            // panelContoursBottom
            // 
            panelContoursBottom.Controls.Add(dgvDeviceList);
            panelContoursBottom.Controls.Add(lblDeviceListTitle);
            panelContoursBottom.Location = new Point(3, 225);
            panelContoursBottom.Name = "panelContoursBottom";
            panelContoursBottom.Size = new Size(268, 210);
            panelContoursBottom.TabIndex = 2;
            // 
            // dgvDeviceList
            // 
            dgvDeviceList.AllowUserToAddRows = false;
            dgvDeviceList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDeviceList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDeviceList.Columns.AddRange(new DataGridViewColumn[] { Position, Block, Type });
            dgvDeviceList.Location = new Point(5, 22);
            dgvDeviceList.Name = "dgvDeviceList";
            dgvDeviceList.ReadOnly = true;
            dgvDeviceList.RowTemplate.Height = 25;
            dgvDeviceList.Size = new Size(260, 185);
            dgvDeviceList.TabIndex = 1;
            // 
            // Position
            // 
            Position.HeaderText = "ПОЗ";
            Position.Name = "Position";
            Position.ReadOnly = true;
            // 
            // Block
            // 
            Block.HeaderText = "Блок";
            Block.Name = "Block";
            Block.ReadOnly = true;
            // 
            // Type
            // 
            Type.HeaderText = "ТИП";
            Type.Name = "Type";
            Type.ReadOnly = true;
            // 
            // lblDeviceListTitle
            // 
            lblDeviceListTitle.AutoSize = true;
            lblDeviceListTitle.Location = new Point(3, 4);
            lblDeviceListTitle.Name = "lblDeviceListTitle";
            lblDeviceListTitle.Size = new Size(187, 15);
            lblDeviceListTitle.TabIndex = 0;
            lblDeviceListTitle.Text = "Приборы в выбранном контуре:";
            // 
            // panelContoursTop
            // 
            panelContoursTop.Controls.Add(dgvContourSummary);
            panelContoursTop.Controls.Add(lblContourSummaryTitle);
            panelContoursTop.Location = new Point(3, 31);
            panelContoursTop.Name = "panelContoursTop";
            panelContoursTop.Size = new Size(392, 188);
            panelContoursTop.TabIndex = 1;
            // 
            // dgvContourSummary
            // 
            dgvContourSummary.AllowUserToAddRows = false;
            dgvContourSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvContourSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvContourSummary.Columns.AddRange(new DataGridViewColumn[] { Number, Counter, Actual, Types, Colors });
            dgvContourSummary.Location = new Point(5, 22);
            dgvContourSummary.Name = "dgvContourSummary";
            dgvContourSummary.ReadOnly = true;
            dgvContourSummary.RowTemplate.Height = 25;
            dgvContourSummary.Size = new Size(382, 163);
            dgvContourSummary.TabIndex = 1;
            dgvContourSummary.SelectionChanged += DgvContourSummary_SelectionChanged;
            // 
            // Number
            // 
            Number.HeaderText = "Контур";
            Number.Name = "Number";
            Number.ReadOnly = true;
            // 
            // Counter
            // 
            Counter.HeaderText = "Учтено";
            Counter.Name = "Counter";
            Counter.ReadOnly = true;
            // 
            // Actual
            // 
            Actual.HeaderText = "На чертеже";
            Actual.Name = "Actual";
            Actual.ReadOnly = true;
            // 
            // Types
            // 
            Types.HeaderText = "Типы (ТИП)";
            Types.Name = "Types";
            Types.ReadOnly = true;
            // 
            // Colors
            // 
            Colors.HeaderText = "Цвет";
            Colors.Name = "Colors";
            Colors.ReadOnly = true;
            // 
            // lblContourSummaryTitle
            // 
            lblContourSummaryTitle.AutoSize = true;
            lblContourSummaryTitle.Location = new Point(3, 4);
            lblContourSummaryTitle.Name = "lblContourSummaryTitle";
            lblContourSummaryTitle.Size = new Size(122, 15);
            lblContourSummaryTitle.TabIndex = 0;
            lblContourSummaryTitle.Text = "Сводка по контурам:";
            // 
            // toolStripContours
            // 
            toolStripContours.Items.AddRange(new ToolStripItem[] { tsbRefreshContours, tsbExportCSV, tsbExportTXT });
            toolStripContours.Location = new Point(3, 3);
            toolStripContours.Name = "toolStripContours";
            toolStripContours.Size = new Size(392, 25);
            toolStripContours.TabIndex = 0;
            toolStripContours.Text = "toolStrip1";
            // 
            // tsbRefreshContours
            // 
            tsbRefreshContours.Image = (Image)resources.GetObject("tsbRefreshContours.Image");
            tsbRefreshContours.ImageTransparentColor = Color.Magenta;
            tsbRefreshContours.Name = "tsbRefreshContours";
            tsbRefreshContours.Size = new Size(81, 22);
            tsbRefreshContours.Text = "Обновить";
            tsbRefreshContours.TextAlign = ContentAlignment.MiddleRight;
            tsbRefreshContours.Click += tsbRefreshContours_Click;
            // 
            // tsbExportCSV
            // 
            tsbExportCSV.Image = (Image)resources.GetObject("tsbExportCSV.Image");
            tsbExportCSV.ImageTransparentColor = Color.Magenta;
            tsbExportCSV.Name = "tsbExportCSV";
            tsbExportCSV.Size = new Size(96, 22);
            tsbExportCSV.Text = "Экспорт CSV";
            tsbExportCSV.Click += tsbExportCSV_Click;
            // 
            // tsbExportTXT
            // 
            tsbExportTXT.Image = (Image)resources.GetObject("tsbExportTXT.Image");
            tsbExportTXT.ImageTransparentColor = Color.Magenta;
            tsbExportTXT.Name = "tsbExportTXT";
            tsbExportTXT.Size = new Size(94, 22);
            tsbExportTXT.Text = "Экспорт TXT";
            tsbExportTXT.Click += tsbExportTXT_Click;
            // 
            // tabHelp
            // 
            tabHelp.Controls.Add(pictureBox1);
            tabHelp.Controls.Add(lnkGitHub);
            tabHelp.Controls.Add(lblVersion);
            tabHelp.Controls.Add(txtHelp);
            tabHelp.Location = new Point(4, 24);
            tabHelp.Name = "tabHelp";
            tabHelp.RightToLeft = RightToLeft.No;
            tabHelp.Size = new Size(398, 463);
            tabHelp.TabIndex = 2;
            tabHelp.Text = "Помощь";
            tabHelp.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(7, 378);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(90, 72);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // lnkGitHub
            // 
            lnkGitHub.AutoSize = true;
            lnkGitHub.Location = new Point(103, 431);
            lnkGitHub.Name = "lnkGitHub";
            lnkGitHub.Size = new Size(286, 15);
            lnkGitHub.TabIndex = 2;
            lnkGitHub.TabStop = true;
            lnkGitHub.Text = "https://github.com/Kukunov/nanoCAD_Automation";
            lnkGitHub.LinkClicked += lnkGitHub_LinkClicked;
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.ForeColor = SystemColors.ControlText;
            lblVersion.Location = new Point(103, 380);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(193, 45);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "GOST 21.208 nanoCAD Automation\r\nВерсия 0.0.5-alpha\r\nКукунов Константин | 2026";
            // 
            // txtHelp
            // 
            txtHelp.BackColor = SystemColors.Window;
            txtHelp.Dock = DockStyle.Top;
            txtHelp.Location = new Point(0, 0);
            txtHelp.Multiline = true;
            txtHelp.Name = "txtHelp";
            txtHelp.ReadOnly = true;
            txtHelp.ScrollBars = ScrollBars.Vertical;
            txtHelp.Size = new Size(398, 350);
            txtHelp.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(406, 491);
            Controls.Add(tabControlMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Схемы по ГОСТ 21.208-2013";
            tabControlMain.ResumeLayout(false);
            tabInsert.ResumeLayout(false);
            tabInsert.PerformLayout();
            gbInsertAttributes.ResumeLayout(false);
            gbInsertAttributes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudElement).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudContour).EndInit();
            gbBlockSelection.ResumeLayout(false);
            gbBlockSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbBlockPreview).EndInit();
            tabContours.ResumeLayout(false);
            tabContours.PerformLayout();
            statusStripContours.ResumeLayout(false);
            statusStripContours.PerformLayout();
            panelContoursBottom.ResumeLayout(false);
            panelContoursBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDeviceList).EndInit();
            panelContoursTop.ResumeLayout(false);
            panelContoursTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvContourSummary).EndInit();
            toolStripContours.ResumeLayout(false);
            toolStripContours.PerformLayout();
            tabHelp.ResumeLayout(false);
            tabHelp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControlMain;
        private TabPage tabInsert;
        private TabPage tabContours;
        private Label lblNextElementCaption;
        private Label lblCurrentContourCaption;
        private ComboBox cmbBlockType;
        private GroupBox gbBlockSelection;
        private Button btnResetCounter;
        private Label lblCurrentContourValue;
        private Label lblNextElementValue;
        private Button btnChangeContour;
        private PictureBox pbBlockPreview;
        private GroupBox gbInsertAttributes;
        private Label lblTypeDesignation;
        private Button btnCancel;
        private Button btnOK;
        private Label lblPositionCaption;
        private Label lblElementNumber;
        private Label lblContourNumber;
        private TextBox txtTypeDesignation;
        private NumericUpDown nudElement;
        private NumericUpDown nudContour;
        private Label lblPositionValue;
        private Button btnContourInfo;
        private CheckBox chbxContourColors;
        private Panel panelContoursTop;
        private ToolStrip toolStripContours;
        private ToolStripButton tsbRefreshContours;
        private ToolStripButton tsbExportCSV;
        private ToolStripButton tsbExportTXT;
        private Panel panelContoursBottom;
        private DataGridView dgvContourSummary;
        private Label lblContourSummaryTitle;
        private DataGridView dgvDeviceList;
        private Label lblDeviceListTitle;
        private StatusStrip statusStripContours;
        private ToolStripStatusLabel tsslContourStats;
        private DataGridViewTextBoxColumn Number;
        private DataGridViewTextBoxColumn Counter;
        private DataGridViewTextBoxColumn Actual;
        private DataGridViewTextBoxColumn Types;
        private DataGridViewTextBoxColumn Colors;
        private DataGridViewTextBoxColumn Position;
        private DataGridViewTextBoxColumn Block;
        private DataGridViewTextBoxColumn Type;
        private TabPage tabHelp;
        private Label lblVersion;
        private TextBox txtHelp;
        private LinkLabel lnkGitHub;
        private PictureBox pictureBox1;
    }
}
