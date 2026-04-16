namespace NanoCAD.UI
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
            tabControlMain = new TabControl();
            tabInsert = new TabPage();
            btnCancel = new Button();
            btnOK = new Button();
            gbInsertAttributes = new GroupBox();
            lblPositionCaption = new Label();
            lblElementNumber = new Label();
            lblContourNumber = new Label();
            txtTypeDesignation = new TextBox();
            lblTypeDesignation = new Label();
            gbBlockSelection = new GroupBox();
            pbBlockPreview = new PictureBox();
            btnResetCounter = new Button();
            cmbBlockType = new ComboBox();
            lblCurrentContourCaption = new Label();
            lblCurrentContourValue = new Label();
            lblNextElementValue = new Label();
            btnChangeContour = new Button();
            lblNextElementCaption = new Label();
            tabParams = new TabPage();
            lblPositionValue = new Label();
            nudContour = new NumericUpDown();
            nudElement = new NumericUpDown();
            tabControlMain.SuspendLayout();
            tabInsert.SuspendLayout();
            gbInsertAttributes.SuspendLayout();
            gbBlockSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbBlockPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudContour).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudElement).BeginInit();
            SuspendLayout();
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabInsert);
            tabControlMain.Controls.Add(tabParams);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(406, 449);
            tabControlMain.TabIndex = 0;
            // 
            // tabInsert
            // 
            tabInsert.Controls.Add(btnCancel);
            tabInsert.Controls.Add(btnOK);
            tabInsert.Controls.Add(gbInsertAttributes);
            tabInsert.Controls.Add(gbBlockSelection);
            tabInsert.Location = new Point(4, 24);
            tabInsert.Name = "tabInsert";
            tabInsert.Padding = new Padding(3);
            tabInsert.Size = new Size(398, 421);
            tabInsert.TabIndex = 0;
            tabInsert.Text = "Вставка";
            tabInsert.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(286, 376);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 28);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(161, 376);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(106, 28);
            btnOK.TabIndex = 9;
            btnOK.Text = "ОК";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // gbInsertAttributes
            // 
            gbInsertAttributes.Controls.Add(nudElement);
            gbInsertAttributes.Controls.Add(nudContour);
            gbInsertAttributes.Controls.Add(lblPositionValue);
            gbInsertAttributes.Controls.Add(lblPositionCaption);
            gbInsertAttributes.Controls.Add(lblElementNumber);
            gbInsertAttributes.Controls.Add(lblContourNumber);
            gbInsertAttributes.Controls.Add(txtTypeDesignation);
            gbInsertAttributes.Controls.Add(lblTypeDesignation);
            gbInsertAttributes.Location = new Point(8, 196);
            gbInsertAttributes.Name = "gbInsertAttributes";
            gbInsertAttributes.Size = new Size(384, 165);
            gbInsertAttributes.TabIndex = 8;
            gbInsertAttributes.TabStop = false;
            gbInsertAttributes.Text = "Атрибуты вставки";
            // 
            // lblPositionCaption
            // 
            lblPositionCaption.AutoSize = true;
            lblPositionCaption.Location = new Point(6, 135);
            lblPositionCaption.Name = "lblPositionCaption";
            lblPositionCaption.Size = new Size(164, 15);
            lblPositionCaption.TabIndex = 4;
            lblPositionCaption.Text = "Позиционное обозначение: ";
            // 
            // lblElementNumber
            // 
            lblElementNumber.AutoSize = true;
            lblElementNumber.Location = new Point(6, 96);
            lblElementNumber.Name = "lblElementNumber";
            lblElementNumber.Size = new Size(103, 15);
            lblElementNumber.TabIndex = 3;
            lblElementNumber.Text = "Номер элемента:";
            // 
            // lblContourNumber
            // 
            lblContourNumber.AutoSize = true;
            lblContourNumber.Location = new Point(6, 65);
            lblContourNumber.Name = "lblContourNumber";
            lblContourNumber.Size = new Size(98, 15);
            lblContourNumber.TabIndex = 2;
            lblContourNumber.Text = "Номер контура: ";
            // 
            // txtTypeDesignation
            // 
            txtTypeDesignation.Location = new Point(198, 20);
            txtTypeDesignation.Name = "txtTypeDesignation";
            txtTypeDesignation.Size = new Size(180, 23);
            txtTypeDesignation.TabIndex = 1;
            // 
            // lblTypeDesignation
            // 
            lblTypeDesignation.AutoSize = true;
            lblTypeDesignation.Location = new Point(6, 28);
            lblTypeDesignation.Name = "lblTypeDesignation";
            lblTypeDesignation.Size = new Size(170, 15);
            lblTypeDesignation.TabIndex = 0;
            lblTypeDesignation.Text = "Обозначение типа элемента: ";
            // 
            // gbBlockSelection
            // 
            gbBlockSelection.Controls.Add(pbBlockPreview);
            gbBlockSelection.Controls.Add(btnResetCounter);
            gbBlockSelection.Controls.Add(cmbBlockType);
            gbBlockSelection.Controls.Add(lblCurrentContourCaption);
            gbBlockSelection.Controls.Add(lblCurrentContourValue);
            gbBlockSelection.Controls.Add(lblNextElementValue);
            gbBlockSelection.Controls.Add(btnChangeContour);
            gbBlockSelection.Controls.Add(lblNextElementCaption);
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
            pbBlockPreview.Size = new Size(127, 126);
            pbBlockPreview.TabIndex = 7;
            pbBlockPreview.TabStop = false;
            // 
            // btnResetCounter
            // 
            btnResetCounter.Location = new Point(278, 122);
            btnResetCounter.Name = "btnResetCounter";
            btnResetCounter.Size = new Size(100, 26);
            btnResetCounter.TabIndex = 6;
            btnResetCounter.Text = "Сбросить счет";
            btnResetCounter.UseVisualStyleBackColor = true;
            // 
            // cmbBlockType
            // 
            cmbBlockType.FormattingEnabled = true;
            cmbBlockType.Location = new Point(156, 22);
            cmbBlockType.Name = "cmbBlockType";
            cmbBlockType.Size = new Size(222, 23);
            cmbBlockType.TabIndex = 0;
            // 
            // lblCurrentContourCaption
            // 
            lblCurrentContourCaption.AutoSize = true;
            lblCurrentContourCaption.Location = new Point(156, 61);
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
            lblNextElementValue.Location = new Point(297, 88);
            lblNextElementValue.Name = "lblNextElementValue";
            lblNextElementValue.Size = new Size(12, 15);
            lblNextElementValue.TabIndex = 4;
            lblNextElementValue.Text = "-";
            // 
            // btnChangeContour
            // 
            btnChangeContour.Location = new Point(153, 122);
            btnChangeContour.Name = "btnChangeContour";
            btnChangeContour.Size = new Size(106, 26);
            btnChangeContour.TabIndex = 5;
            btnChangeContour.Text = "Сменить контур";
            btnChangeContour.UseVisualStyleBackColor = true;
            // 
            // lblNextElementCaption
            // 
            lblNextElementCaption.AutoSize = true;
            lblNextElementCaption.Location = new Point(156, 88);
            lblNextElementCaption.Name = "lblNextElementCaption";
            lblNextElementCaption.Size = new Size(128, 15);
            lblNextElementCaption.TabIndex = 2;
            lblNextElementCaption.Text = "Следующая позиция: ";
            // 
            // tabParams
            // 
            tabParams.Location = new Point(4, 24);
            tabParams.Name = "tabParams";
            tabParams.Padding = new Padding(3);
            tabParams.Size = new Size(398, 421);
            tabParams.TabIndex = 1;
            tabParams.Text = "Параметры";
            tabParams.UseVisualStyleBackColor = true;
            // 
            // lblPositionValue
            // 
            lblPositionValue.AutoSize = true;
            lblPositionValue.Location = new Point(198, 135);
            lblPositionValue.Name = "lblPositionValue";
            lblPositionValue.Size = new Size(12, 15);
            lblPositionValue.TabIndex = 5;
            lblPositionValue.Text = "-";
            // 
            // nudContour
            // 
            nudContour.Location = new Point(198, 57);
            nudContour.Name = "nudContour";
            nudContour.Size = new Size(61, 23);
            nudContour.TabIndex = 6;
            // 
            // nudElement
            // 
            nudElement.Location = new Point(198, 88);
            nudElement.Name = "nudElement";
            nudElement.Size = new Size(61, 23);
            nudElement.TabIndex = 7;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(406, 449);
            Controls.Add(tabControlMain);
            Name = "MainForm";
            Text = "Схемы по ГОСТ 21.208-2013";
            Load += Form1_Load;
            tabControlMain.ResumeLayout(false);
            tabInsert.ResumeLayout(false);
            gbInsertAttributes.ResumeLayout(false);
            gbInsertAttributes.PerformLayout();
            gbBlockSelection.ResumeLayout(false);
            gbBlockSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbBlockPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudContour).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudElement).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControlMain;
        private TabPage tabInsert;
        private TabPage tabParams;
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
    }
}
