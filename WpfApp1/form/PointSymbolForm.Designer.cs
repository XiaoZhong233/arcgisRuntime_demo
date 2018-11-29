namespace WpfApp1.form
{
    partial class PointSymbolForm
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            this.StyleCombox = new System.Windows.Forms.ComboBox();
            this.previewBox = new System.Windows.Forms.PictureBox();
            this.sizeUpDown = new System.Windows.Forms.NumericUpDown();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.okBtn = new System.Windows.Forms.Button();
            this.customPic_RB = new System.Windows.Forms.RadioButton();
            this.simple_RB = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.styleGroup = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.colorBtn = new System.Windows.Forms.Button();
            this.previewGroup = new System.Windows.Forms.GroupBox();
            this.cusPictureBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.transparencyControl = new WpfApp1.form.Controls.TransparencyControl();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).BeginInit();
            this.panel1.SuspendLayout();
            this.styleGroup.SuspendLayout();
            this.previewGroup.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("宋体", 9F);
            label1.Location = new System.Drawing.Point(32, 49);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(67, 15);
            label1.TabIndex = 1;
            label1.Text = "点样式：";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("宋体", 9F);
            label3.Location = new System.Drawing.Point(16, 44);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(52, 15);
            label3.TabIndex = 4;
            label3.Text = "大小：";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("宋体", 9F);
            label4.Location = new System.Drawing.Point(247, 42);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(52, 15);
            label4.TabIndex = 5;
            label4.Text = "颜色：";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StyleCombox
            // 
            this.StyleCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StyleCombox.FormattingEnabled = true;
            this.StyleCombox.Location = new System.Drawing.Point(105, 46);
            this.StyleCombox.Name = "StyleCombox";
            this.StyleCombox.Size = new System.Drawing.Size(133, 23);
            this.StyleCombox.TabIndex = 0;
            // 
            // previewBox
            // 
            this.previewBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.previewBox.Location = new System.Drawing.Point(6, 33);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(188, 172);
            this.previewBox.TabIndex = 2;
            this.previewBox.TabStop = false;
            // 
            // sizeUpDown
            // 
            this.sizeUpDown.Location = new System.Drawing.Point(91, 38);
            this.sizeUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.sizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sizeUpDown.Name = "sizeUpDown";
            this.sizeUpDown.Size = new System.Drawing.Size(131, 25);
            this.sizeUpDown.TabIndex = 6;
            this.sizeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(679, 359);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(90, 36);
            this.okBtn.TabIndex = 8;
            this.okBtn.Text = "确认";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // customPic_RB
            // 
            this.customPic_RB.AutoSize = true;
            this.customPic_RB.Location = new System.Drawing.Point(35, 62);
            this.customPic_RB.Name = "customPic_RB";
            this.customPic_RB.Size = new System.Drawing.Size(103, 19);
            this.customPic_RB.TabIndex = 9;
            this.customPic_RB.TabStop = true;
            this.customPic_RB.Text = "自定义图片";
            this.customPic_RB.UseVisualStyleBackColor = true;
            // 
            // simple_RB
            // 
            this.simple_RB.AutoSize = true;
            this.simple_RB.Location = new System.Drawing.Point(35, 25);
            this.simple_RB.Name = "simple_RB";
            this.simple_RB.Size = new System.Drawing.Size(103, 19);
            this.simple_RB.TabIndex = 10;
            this.simple_RB.TabStop = true;
            this.simple_RB.Text = "简单点样式";
            this.simple_RB.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.styleGroup);
            this.panel1.Controls.Add(label1);
            this.panel1.Controls.Add(this.StyleCombox);
            this.panel1.Location = new System.Drawing.Point(227, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(542, 329);
            this.panel1.TabIndex = 11;
            // 
            // styleGroup
            // 
            this.styleGroup.Controls.Add(this.transparencyControl);
            this.styleGroup.Controls.Add(this.label2);
            this.styleGroup.Controls.Add(this.colorBtn);
            this.styleGroup.Controls.Add(label3);
            this.styleGroup.Controls.Add(this.sizeUpDown);
            this.styleGroup.Controls.Add(label4);
            this.styleGroup.Location = new System.Drawing.Point(16, 111);
            this.styleGroup.Name = "styleGroup";
            this.styleGroup.Size = new System.Drawing.Size(512, 162);
            this.styleGroup.TabIndex = 2;
            this.styleGroup.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "透明度：";
            // 
            // colorBtn
            // 
            this.colorBtn.BackColor = System.Drawing.Color.Black;
            this.colorBtn.Location = new System.Drawing.Point(305, 36);
            this.colorBtn.Name = "colorBtn";
            this.colorBtn.Size = new System.Drawing.Size(136, 23);
            this.colorBtn.TabIndex = 7;
            this.colorBtn.UseVisualStyleBackColor = false;
            this.colorBtn.Click += new System.EventHandler(this.colorBtn_Click);
            // 
            // previewGroup
            // 
            this.previewGroup.Controls.Add(this.cusPictureBtn);
            this.previewGroup.Controls.Add(this.previewBox);
            this.previewGroup.Location = new System.Drawing.Point(12, 12);
            this.previewGroup.Name = "previewGroup";
            this.previewGroup.Size = new System.Drawing.Size(198, 227);
            this.previewGroup.TabIndex = 12;
            this.previewGroup.TabStop = false;
            this.previewGroup.Text = "预览";
            // 
            // cusPictureBtn
            // 
            this.cusPictureBtn.Enabled = false;
            this.cusPictureBtn.Location = new System.Drawing.Point(46, 89);
            this.cusPictureBtn.Name = "cusPictureBtn";
            this.cusPictureBtn.Size = new System.Drawing.Size(103, 48);
            this.cusPictureBtn.TabIndex = 3;
            this.cusPictureBtn.Text = "添加图片";
            this.cusPictureBtn.UseVisualStyleBackColor = true;
            this.cusPictureBtn.Visible = false;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.customPic_RB);
            this.panel2.Controls.Add(this.simple_RB);
            this.panel2.Location = new System.Drawing.Point(22, 253);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(188, 100);
            this.panel2.TabIndex = 13;
            // 
            // transparencyControl
            // 
            this.transparencyControl.BandColor = System.Drawing.Color.Empty;
            this.transparencyControl.Location = new System.Drawing.Point(93, 117);
            this.transparencyControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.transparencyControl.MaximumSize = new System.Drawing.Size(1365, 37);
            this.transparencyControl.MinimumSize = new System.Drawing.Size(171, 37);
            this.transparencyControl.Name = "transparencyControl";
            this.transparencyControl.Size = new System.Drawing.Size(371, 37);
            this.transparencyControl.TabIndex = 9;
            this.transparencyControl.Value = ((byte)(255));
            // 
            // PointSymbolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 418);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.previewGroup);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.okBtn);
            this.Name = "PointSymbolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "点符号";
            this.Load += new System.EventHandler(this.PointSymbolForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.styleGroup.ResumeLayout(false);
            this.styleGroup.PerformLayout();
            this.previewGroup.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox StyleCombox;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.NumericUpDown sizeUpDown;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.RadioButton customPic_RB;
        private System.Windows.Forms.RadioButton simple_RB;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox styleGroup;
        private System.Windows.Forms.GroupBox previewGroup;
        private System.Windows.Forms.Button colorBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button cusPictureBtn;
        private Controls.TransparencyControl transparencyControl;
        private System.Windows.Forms.Label label2;
    }
}