namespace Paint
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuUngroup = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbKieuVe = new System.Windows.Forms.ComboBox();
            this.btnMauNen = new System.Windows.Forms.Button();
            this.numDoDay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMauDuong = new System.Windows.Forms.Button();
            this.btnDaGiac = new System.Windows.Forms.Button();
            this.btnDuongCong = new System.Windows.Forms.Button();
            this.btnHinhTron = new System.Windows.Forms.Button();
            this.btnHV = new System.Windows.Forms.Button();
            this.btnHcn = new System.Windows.Forms.Button();
            this.btnEllipse = new System.Windows.Forms.Button();
            this.btnDuongThang = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDoDay)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuSelect,
            this.MenuGroup,
            this.MenuUngroup,
            this.MenuZoom});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(46, 24);
            this.MenuFile.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.newToolStripMenuItem.Text = "New";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // MenuSelect
            // 
            this.MenuSelect.Name = "MenuSelect";
            this.MenuSelect.Size = new System.Drawing.Size(63, 24);
            this.MenuSelect.Text = "Select";
            // 
            // MenuGroup
            // 
            this.MenuGroup.Name = "MenuGroup";
            this.MenuGroup.Size = new System.Drawing.Size(64, 24);
            this.MenuGroup.Text = "Group";
            // 
            // MenuUngroup
            // 
            this.MenuUngroup.Name = "MenuUngroup";
            this.MenuUngroup.Size = new System.Drawing.Size(81, 24);
            this.MenuUngroup.Text = "Ungroup";
            // 
            // MenuZoom
            // 
            this.MenuZoom.Name = "MenuZoom";
            this.MenuZoom.Size = new System.Drawing.Size(63, 24);
            this.MenuZoom.Text = "Zoom";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbKieuVe);
            this.panel1.Controls.Add(this.btnMauNen);
            this.panel1.Controls.Add(this.numDoDay);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnMauDuong);
            this.panel1.Controls.Add(this.btnDaGiac);
            this.panel1.Controls.Add(this.btnDuongCong);
            this.panel1.Controls.Add(this.btnHinhTron);
            this.panel1.Controls.Add(this.btnHV);
            this.panel1.Controls.Add(this.btnHcn);
            this.panel1.Controls.Add(this.btnEllipse);
            this.panel1.Controls.Add(this.btnDuongThang);
            this.panel1.Location = new System.Drawing.Point(0, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(142, 419);
            this.panel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 368);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "Kiểu đường vẽ";
            // 
            // cmbKieuVe
            // 
            this.cmbKieuVe.FormattingEnabled = true;
            this.cmbKieuVe.Items.AddRange(new object[] {
            "Nét liền",
            "Nét đứt"});
            this.cmbKieuVe.Location = new System.Drawing.Point(12, 390);
            this.cmbKieuVe.Name = "cmbKieuVe";
            this.cmbKieuVe.Size = new System.Drawing.Size(121, 24);
            this.cmbKieuVe.TabIndex = 11;
            // 
            // btnMauNen
            // 
            this.btnMauNen.Location = new System.Drawing.Point(12, 336);
            this.btnMauNen.Name = "btnMauNen";
            this.btnMauNen.Size = new System.Drawing.Size(116, 29);
            this.btnMauNen.TabIndex = 10;
            this.btnMauNen.Text = "Màu nền tô";
            this.btnMauNen.UseVisualStyleBackColor = true;
            // 
            // numDoDay
            // 
            this.numDoDay.Location = new System.Drawing.Point(16, 309);
            this.numDoDay.Name = "numDoDay";
            this.numDoDay.Size = new System.Drawing.Size(120, 22);
            this.numDoDay.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 285);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Độ dày";
            // 
            // btnMauDuong
            // 
            this.btnMauDuong.Location = new System.Drawing.Point(12, 252);
            this.btnMauDuong.Name = "btnMauDuong";
            this.btnMauDuong.Size = new System.Drawing.Size(116, 29);
            this.btnMauDuong.TabIndex = 7;
            this.btnMauDuong.Text = "Màu đường tô";
            this.btnMauDuong.UseVisualStyleBackColor = true;
            // 
            // btnDaGiac
            // 
            this.btnDaGiac.Location = new System.Drawing.Point(13, 217);
            this.btnDaGiac.Name = "btnDaGiac";
            this.btnDaGiac.Size = new System.Drawing.Size(117, 29);
            this.btnDaGiac.TabIndex = 6;
            this.btnDaGiac.Text = "Đa giác";
            this.btnDaGiac.UseVisualStyleBackColor = true;
            // 
            // btnDuongCong
            // 
            this.btnDuongCong.Location = new System.Drawing.Point(13, 182);
            this.btnDuongCong.Name = "btnDuongCong";
            this.btnDuongCong.Size = new System.Drawing.Size(117, 29);
            this.btnDuongCong.TabIndex = 5;
            this.btnDuongCong.Text = "Đường cong";
            this.btnDuongCong.UseVisualStyleBackColor = true;
            // 
            // btnHinhTron
            // 
            this.btnHinhTron.Location = new System.Drawing.Point(13, 147);
            this.btnHinhTron.Name = "btnHinhTron";
            this.btnHinhTron.Size = new System.Drawing.Size(117, 29);
            this.btnHinhTron.TabIndex = 4;
            this.btnHinhTron.Text = "Hình tròn";
            this.btnHinhTron.UseVisualStyleBackColor = true;
            // 
            // btnHV
            // 
            this.btnHV.Location = new System.Drawing.Point(13, 112);
            this.btnHV.Name = "btnHV";
            this.btnHV.Size = new System.Drawing.Size(117, 29);
            this.btnHV.TabIndex = 3;
            this.btnHV.Text = "Hình vuông";
            this.btnHV.UseVisualStyleBackColor = true;
            // 
            // btnHcn
            // 
            this.btnHcn.Location = new System.Drawing.Point(13, 77);
            this.btnHcn.Name = "btnHcn";
            this.btnHcn.Size = new System.Drawing.Size(117, 29);
            this.btnHcn.TabIndex = 2;
            this.btnHcn.Text = "Hình chữ nhật";
            this.btnHcn.UseVisualStyleBackColor = true;
            // 
            // btnEllipse
            // 
            this.btnEllipse.Location = new System.Drawing.Point(12, 42);
            this.btnEllipse.Name = "btnEllipse";
            this.btnEllipse.Size = new System.Drawing.Size(117, 29);
            this.btnEllipse.TabIndex = 1;
            this.btnEllipse.Text = "Hình Ellipse";
            this.btnEllipse.UseVisualStyleBackColor = true;
            // 
            // btnDuongThang
            // 
            this.btnDuongThang.Location = new System.Drawing.Point(13, 7);
            this.btnDuongThang.Name = "btnDuongThang";
            this.btnDuongThang.Size = new System.Drawing.Size(117, 29);
            this.btnDuongThang.TabIndex = 0;
            this.btnDuongThang.Text = "Đoạn thẳng";
            this.btnDuongThang.UseVisualStyleBackColor = true;
            this.btnDuongThang.Click += new System.EventHandler(this.btnDuongThang_Click);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(148, 32);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(652, 419);
            this.panel2.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDoDay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDaGiac;
        private System.Windows.Forms.Button btnDuongCong;
        private System.Windows.Forms.Button btnHinhTron;
        private System.Windows.Forms.Button btnHV;
        private System.Windows.Forms.Button btnHcn;
        private System.Windows.Forms.Button btnEllipse;
        private System.Windows.Forms.Button btnDuongThang;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem MenuSelect;
        private System.Windows.Forms.ToolStripMenuItem MenuGroup;
        private System.Windows.Forms.ToolStripMenuItem MenuUngroup;
        private System.Windows.Forms.ToolStripMenuItem MenuZoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMauDuong;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbKieuVe;
        private System.Windows.Forms.Button btnMauNen;
        private System.Windows.Forms.NumericUpDown numDoDay;
    }
}

