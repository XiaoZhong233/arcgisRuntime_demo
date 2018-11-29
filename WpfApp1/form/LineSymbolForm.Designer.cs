namespace WpfApp1.form
{
    partial class LineSymbolForm
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
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label line;
            this.okBtn = new System.Windows.Forms.Button();
            this.colorBtn = new System.Windows.Forms.Button();
            this.sizeUpDown = new System.Windows.Forms.NumericUpDown();
            this.previewBox = new System.Windows.Forms.PictureBox();
            this.StyleCombox = new System.Windows.Forms.ComboBox();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.lineTab = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.VerticsTab = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.transparencyControl = new WpfApp1.form.Controls.TransparencyControl();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            line = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
            this.TabControl.SuspendLayout();
            this.lineTab.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("宋体", 9F);
            label4.Location = new System.Drawing.Point(21, 96);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(52, 15);
            label4.TabIndex = 14;
            label4.Text = "颜色：";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("宋体", 9F);
            label3.Location = new System.Drawing.Point(21, 40);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(52, 15);
            label3.TabIndex = 13;
            label3.Text = "宽度：";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // line
            // 
            line.AutoSize = true;
            line.Font = new System.Drawing.Font("宋体", 9F);
            line.Location = new System.Drawing.Point(26, 48);
            line.Name = "line";
            line.Size = new System.Drawing.Size(67, 15);
            line.TabIndex = 10;
            line.Text = "线样式：";
            line.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(650, 370);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(90, 35);
            this.okBtn.TabIndex = 17;
            this.okBtn.Text = "确认";
            this.okBtn.UseVisualStyleBackColor = true;
            // 
            // colorBtn
            // 
            this.colorBtn.BackColor = System.Drawing.Color.Black;
            this.colorBtn.Location = new System.Drawing.Point(79, 91);
            this.colorBtn.Name = "colorBtn";
            this.colorBtn.Size = new System.Drawing.Size(165, 24);
            this.colorBtn.TabIndex = 16;
            this.colorBtn.UseVisualStyleBackColor = false;
            // 
            // sizeUpDown
            // 
            this.sizeUpDown.Location = new System.Drawing.Point(79, 38);
            this.sizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sizeUpDown.Name = "sizeUpDown";
            this.sizeUpDown.Size = new System.Drawing.Size(165, 25);
            this.sizeUpDown.TabIndex = 15;
            this.sizeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // previewBox
            // 
            this.previewBox.Location = new System.Drawing.Point(10, 24);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(197, 183);
            this.previewBox.TabIndex = 11;
            this.previewBox.TabStop = false;
            // 
            // StyleCombox
            // 
            this.StyleCombox.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.StyleCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StyleCombox.FormattingEnabled = true;
            this.StyleCombox.Location = new System.Drawing.Point(103, 45);
            this.StyleCombox.Name = "StyleCombox";
            this.StyleCombox.Size = new System.Drawing.Size(165, 23);
            this.StyleCombox.TabIndex = 9;
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.lineTab);
            this.TabControl.Controls.Add(this.VerticsTab);
            this.TabControl.Location = new System.Drawing.Point(240, 12);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(521, 352);
            this.TabControl.TabIndex = 19;
            // 
            // lineTab
            // 
            this.lineTab.BackColor = System.Drawing.Color.White;
            this.lineTab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lineTab.Controls.Add(this.groupBox2);
            this.lineTab.Controls.Add(line);
            this.lineTab.Controls.Add(this.StyleCombox);
            this.lineTab.Location = new System.Drawing.Point(4, 25);
            this.lineTab.Name = "lineTab";
            this.lineTab.Padding = new System.Windows.Forms.Padding(3);
            this.lineTab.Size = new System.Drawing.Size(513, 323);
            this.lineTab.TabIndex = 0;
            this.lineTab.Text = "线样式";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(label4);
            this.groupBox2.Controls.Add(this.colorBtn);
            this.groupBox2.Controls.Add(label3);
            this.groupBox2.Controls.Add(this.transparencyControl);
            this.groupBox2.Controls.Add(this.sizeUpDown);
            this.groupBox2.Location = new System.Drawing.Point(24, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(472, 204);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 19;
            this.label1.Text = "透明度:";
            // 
            // VerticsTab
            // 
            this.VerticsTab.BackColor = System.Drawing.Color.White;
            this.VerticsTab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VerticsTab.Location = new System.Drawing.Point(4, 25);
            this.VerticsTab.Name = "VerticsTab";
            this.VerticsTab.Padding = new System.Windows.Forms.Padding(3);
            this.VerticsTab.Size = new System.Drawing.Size(513, 323);
            this.VerticsTab.TabIndex = 1;
            this.VerticsTab.Text = "顶点样式";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.previewBox);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(221, 215);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "预览";
            // 
            // transparencyControl
            // 
            this.transparencyControl.BandColor = System.Drawing.Color.Empty;
            this.transparencyControl.Location = new System.Drawing.Point(76, 148);
            this.transparencyControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.transparencyControl.MaximumSize = new System.Drawing.Size(1365, 37);
            this.transparencyControl.MinimumSize = new System.Drawing.Size(171, 37);
            this.transparencyControl.Name = "transparencyControl";
            this.transparencyControl.Size = new System.Drawing.Size(371, 37);
            this.transparencyControl.TabIndex = 18;
            this.transparencyControl.Value = ((byte)(255));
            // 
            // LineSymbolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 417);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.okBtn);
            this.Name = "LineSymbolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "线符号";
            ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
            this.TabControl.ResumeLayout(false);
            this.lineTab.ResumeLayout(false);
            this.lineTab.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button colorBtn;
        private System.Windows.Forms.NumericUpDown sizeUpDown;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.ComboBox StyleCombox;
        private Controls.TransparencyControl transparencyControl;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage lineTab;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage VerticsTab;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ColorDialog colorDialog;
    }
}