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
            this.MofileMoi = new System.Windows.Forms.ToolStripMenuItem();
            this.Luufiledave = new System.Windows.Forms.ToolStripMenuItem();
            this.Chonvungdave = new System.Windows.Forms.ToolStripMenuItem();
            this.NhomCacHinh = new System.Windows.Forms.ToolStripMenuItem();
            this.GoCacHinh = new System.Windows.Forms.ToolStripMenuItem();
            this.PhongToHinh = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_chucnang = new System.Windows.Forms.Panel();
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
            this.panel_khungve = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.panel_chucnang.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDoDay)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.Chonvungdave,
            this.NhomCacHinh,
            this.GoCacHinh,
            this.PhongToHinh});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1388, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MofileMoi,
            this.Luufiledave});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "File";
            // 
            // MofileMoi
            // 
            this.MofileMoi.Name = "MofileMoi";
            this.MofileMoi.Size = new System.Drawing.Size(180, 22);
            this.MofileMoi.Text = "New";
            // 
            // Luufiledave
            // 
            this.Luufiledave.Name = "Luufiledave";
            this.Luufiledave.Size = new System.Drawing.Size(180, 22);
            this.Luufiledave.Text = "Save";
            // 
            // Chonvungdave
            // 
            this.Chonvungdave.Name = "Chonvungdave";
            this.Chonvungdave.Size = new System.Drawing.Size(50, 20);
            this.Chonvungdave.Text = "Select";
            // 
            // NhomCacHinh
            // 
            this.NhomCacHinh.Name = "NhomCacHinh";
            this.NhomCacHinh.Size = new System.Drawing.Size(52, 20);
            this.NhomCacHinh.Text = "Group";
            // 
            // GoCacHinh
            // 
            this.GoCacHinh.Name = "GoCacHinh";
            this.GoCacHinh.Size = new System.Drawing.Size(66, 20);
            this.GoCacHinh.Text = "Ungroup";
            // 
            // PhongToHinh
            // 
            this.PhongToHinh.Name = "PhongToHinh";
            this.PhongToHinh.Size = new System.Drawing.Size(51, 20);
            this.PhongToHinh.Text = "Zoom";
            // 
            // panel_chucnang
            // 
            this.panel_chucnang.Controls.Add(this.cmbKieuVe);
            this.panel_chucnang.Controls.Add(this.label2);
            this.panel_chucnang.Controls.Add(this.btnMauNen);
            this.panel_chucnang.Controls.Add(this.numDoDay);
            this.panel_chucnang.Controls.Add(this.label1);
            this.panel_chucnang.Controls.Add(this.btnMauDuong);
            this.panel_chucnang.Controls.Add(this.btnDaGiac);
            this.panel_chucnang.Controls.Add(this.btnDuongCong);
            this.panel_chucnang.Controls.Add(this.btnHinhTron);
            this.panel_chucnang.Controls.Add(this.btnHV);
            this.panel_chucnang.Controls.Add(this.btnHcn);
            this.panel_chucnang.Controls.Add(this.btnEllipse);
            this.panel_chucnang.Controls.Add(this.btnDuongThang);
            this.panel_chucnang.Location = new System.Drawing.Point(0, 26);
            this.panel_chucnang.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel_chucnang.Name = "panel_chucnang";
            this.panel_chucnang.Size = new System.Drawing.Size(202, 624);
            this.panel_chucnang.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(-3, 477);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "Kiểu đường vẽ";
            // 
            // cmbKieuVe
            // 
            this.cmbKieuVe.FormattingEnabled = true;
            this.cmbKieuVe.Items.AddRange(new object[] {
            "Nét liền",
            "Nét đứt"});
            this.cmbKieuVe.Location = new System.Drawing.Point(100, 473);
            this.cmbKieuVe.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbKieuVe.Name = "cmbKieuVe";
            this.cmbKieuVe.Size = new System.Drawing.Size(92, 21);
            this.cmbKieuVe.TabIndex = 11;
            // 
            // btnMauNen
            // 
            this.btnMauNen.Location = new System.Drawing.Point(108, 381);
            this.btnMauNen.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnMauNen.Name = "btnMauNen";
            this.btnMauNen.Size = new System.Drawing.Size(92, 34);
            this.btnMauNen.TabIndex = 10;
            this.btnMauNen.Text = "Màu nền tô";
            this.btnMauNen.UseVisualStyleBackColor = true;
            // 
            // numDoDay
            // 
            this.numDoDay.Location = new System.Drawing.Point(100, 438);
            this.numDoDay.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numDoDay.Name = "numDoDay";
            this.numDoDay.Size = new System.Drawing.Size(90, 20);
            this.numDoDay.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 437);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Độ dày";
            // 
            // btnMauDuong
            // 
            this.btnMauDuong.Location = new System.Drawing.Point(2, 381);
            this.btnMauDuong.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnMauDuong.Name = "btnMauDuong";
            this.btnMauDuong.Size = new System.Drawing.Size(92, 34);
            this.btnMauDuong.TabIndex = 7;
            this.btnMauDuong.Text = "Màu đường tô";
            this.btnMauDuong.UseVisualStyleBackColor = true;
            // 
            // btnDaGiac
            // 
            this.btnDaGiac.Location = new System.Drawing.Point(51, 312);
            this.btnDaGiac.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDaGiac.Name = "btnDaGiac";
            this.btnDaGiac.Size = new System.Drawing.Size(103, 31);
            this.btnDaGiac.TabIndex = 6;
            this.btnDaGiac.Text = "Đa giác";
            this.btnDaGiac.UseVisualStyleBackColor = true;
            // 
            // btnDuongCong
            // 
            this.btnDuongCong.Location = new System.Drawing.Point(51, 260);
            this.btnDuongCong.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDuongCong.Name = "btnDuongCong";
            this.btnDuongCong.Size = new System.Drawing.Size(103, 31);
            this.btnDuongCong.TabIndex = 5;
            this.btnDuongCong.Text = "Đường cong";
            this.btnDuongCong.UseVisualStyleBackColor = true;
            // 
            // btnHinhTron
            // 
            this.btnHinhTron.Location = new System.Drawing.Point(51, 212);
            this.btnHinhTron.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHinhTron.Name = "btnHinhTron";
            this.btnHinhTron.Size = new System.Drawing.Size(103, 31);
            this.btnHinhTron.TabIndex = 4;
            this.btnHinhTron.Text = "Hình tròn";
            this.btnHinhTron.UseVisualStyleBackColor = true;
            // 
            // btnHV
            // 
            this.btnHV.Location = new System.Drawing.Point(51, 160);
            this.btnHV.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHV.Name = "btnHV";
            this.btnHV.Size = new System.Drawing.Size(103, 31);
            this.btnHV.TabIndex = 3;
            this.btnHV.Text = "Hình vuông";
            this.btnHV.UseVisualStyleBackColor = true;
            // 
            // btnHcn
            // 
            this.btnHcn.Location = new System.Drawing.Point(51, 108);
            this.btnHcn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHcn.Name = "btnHcn";
            this.btnHcn.Size = new System.Drawing.Size(103, 31);
            this.btnHcn.TabIndex = 2;
            this.btnHcn.Text = "Hình chữ nhật";
            this.btnHcn.UseVisualStyleBackColor = true;
            // 
            // btnEllipse
            // 
            this.btnEllipse.Location = new System.Drawing.Point(51, 61);
            this.btnEllipse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnEllipse.Name = "btnEllipse";
            this.btnEllipse.Size = new System.Drawing.Size(103, 31);
            this.btnEllipse.TabIndex = 1;
            this.btnEllipse.Text = "Hình Ellipse";
            this.btnEllipse.UseVisualStyleBackColor = true;
            // 
            // btnDuongThang
            // 
            this.btnDuongThang.Location = new System.Drawing.Point(51, 13);
            this.btnDuongThang.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDuongThang.Name = "btnDuongThang";
            this.btnDuongThang.Size = new System.Drawing.Size(103, 31);
            this.btnDuongThang.TabIndex = 0;
            this.btnDuongThang.Text = "Đoạn thẳng";
            this.btnDuongThang.UseVisualStyleBackColor = true;
            this.btnDuongThang.Click += new System.EventHandler(this.btnDuongThang_Click);
            // 
            // panel_khungve
            // 
            this.panel_khungve.Location = new System.Drawing.Point(206, 26);
            this.panel_khungve.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel_khungve.Name = "panel_khungve";
            this.panel_khungve.Size = new System.Drawing.Size(1182, 624);
            this.panel_khungve.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1388, 649);
            this.Controls.Add(this.panel_khungve);
            this.Controls.Add(this.panel_chucnang);
            this.Controls.Add(this.menuStrip1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel_chucnang.ResumeLayout(false);
            this.panel_chucnang.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDoDay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MofileMoi;
        private System.Windows.Forms.ToolStripMenuItem Luufiledave;
        private System.Windows.Forms.Panel panel_chucnang;
        private System.Windows.Forms.Button btnDaGiac;
        private System.Windows.Forms.Button btnDuongCong;
        private System.Windows.Forms.Button btnHinhTron;
        private System.Windows.Forms.Button btnHV;
        private System.Windows.Forms.Button btnHcn;
        private System.Windows.Forms.Button btnEllipse;
        private System.Windows.Forms.Button btnDuongThang;
        private System.Windows.Forms.Panel panel_khungve;
        private System.Windows.Forms.ToolStripMenuItem Chonvungdave;
        private System.Windows.Forms.ToolStripMenuItem NhomCacHinh;
        private System.Windows.Forms.ToolStripMenuItem GoCacHinh;
        private System.Windows.Forms.ToolStripMenuItem PhongToHinh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMauDuong;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbKieuVe;
        private System.Windows.Forms.Button btnMauNen;
        private System.Windows.Forms.NumericUpDown numDoDay;
    }
}

