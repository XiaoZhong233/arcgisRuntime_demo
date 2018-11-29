namespace WpfApp1.form
{
    partial class ProsperityForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstFields1 = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.chkUniqueValues = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkRandomColors = new System.Windows.Forms.CheckBox();
            this.chkSetGradient = new System.Windows.Forms.CheckBox();
            this.btnChangeColorScheme = new System.Windows.Forms.Button();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.dgvCategories = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Visible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cmnStyle = new System.Windows.Forms.DataGridViewImageColumn();
            this.cmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmnExpression = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmnCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCategoryGenerate = new System.Windows.Forms.Button();
            this.btnCategoryAppearance = new System.Windows.Forms.Button();
            this.btnCategoryRemove = new System.Windows.Forms.Button();
            this.btnCategoryClear = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategories)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(882, 455);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnCategoryClear);
            this.tabPage1.Controls.Add(this.btnCategoryRemove);
            this.tabPage1.Controls.Add(this.btnCategoryAppearance);
            this.tabPage1.Controls.Add(this.btnCategoryGenerate);
            this.tabPage1.Controls.Add(this.groupBox12);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(874, 426);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 71);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstFields1);
            this.groupBox1.Location = new System.Drawing.Point(6, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 403);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "字段";
            // 
            // lstFields1
            // 
            this.lstFields1.FormattingEnabled = true;
            this.lstFields1.ItemHeight = 15;
            this.lstFields1.Location = new System.Drawing.Point(7, 25);
            this.lstFields1.Name = "lstFields1";
            this.lstFields1.Size = new System.Drawing.Size(187, 364);
            this.lstFields1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.chkUniqueValues);
            this.groupBox2.Location = new System.Drawing.Point(226, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(204, 115);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "分类";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(20, 40);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(87, 15);
            this.label19.TabIndex = 125;
            this.label19.Text = "Categories";
            // 
            // chkUniqueValues
            // 
            this.chkUniqueValues.AutoSize = true;
            this.chkUniqueValues.Location = new System.Drawing.Point(23, 73);
            this.chkUniqueValues.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkUniqueValues.Name = "chkUniqueValues";
            this.chkUniqueValues.Size = new System.Drawing.Size(74, 19);
            this.chkUniqueValues.TabIndex = 124;
            this.chkUniqueValues.Text = "唯一值";
            this.chkUniqueValues.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkRandomColors);
            this.groupBox6.Controls.Add(this.chkSetGradient);
            this.groupBox6.Controls.Add(this.btnChangeColorScheme);
            this.groupBox6.Location = new System.Drawing.Point(452, 15);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox6.Size = new System.Drawing.Size(224, 115);
            this.groupBox6.TabIndex = 129;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "颜色";
            // 
            // chkRandomColors
            // 
            this.chkRandomColors.AutoSize = true;
            this.chkRandomColors.Location = new System.Drawing.Point(102, 73);
            this.chkRandomColors.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkRandomColors.Name = "chkRandomColors";
            this.chkRandomColors.Size = new System.Drawing.Size(74, 19);
            this.chkRandomColors.TabIndex = 108;
            this.chkRandomColors.Text = "随机色";
            this.chkRandomColors.UseVisualStyleBackColor = true;
            // 
            // chkSetGradient
            // 
            this.chkSetGradient.Checked = true;
            this.chkSetGradient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSetGradient.Location = new System.Drawing.Point(23, 71);
            this.chkSetGradient.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkSetGradient.Name = "chkSetGradient";
            this.chkSetGradient.Size = new System.Drawing.Size(91, 23);
            this.chkSetGradient.TabIndex = 116;
            this.chkSetGradient.Text = "渐变";
            this.chkSetGradient.UseVisualStyleBackColor = true;
            // 
            // btnChangeColorScheme
            // 
            this.btnChangeColorScheme.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.btnChangeColorScheme.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnChangeColorScheme.Location = new System.Drawing.Point(156, 30);
            this.btnChangeColorScheme.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnChangeColorScheme.Name = "btnChangeColorScheme";
            this.btnChangeColorScheme.Size = new System.Drawing.Size(37, 24);
            this.btnChangeColorScheme.TabIndex = 107;
            this.btnChangeColorScheme.Text = "...";
            this.btnChangeColorScheme.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.dgvCategories);
            this.groupBox12.Location = new System.Drawing.Point(226, 151);
            this.groupBox12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox12.Size = new System.Drawing.Size(456, 253);
            this.groupBox12.TabIndex = 131;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "类别";
            // 
            // dgvCategories
            // 
            this.dgvCategories.AllowUserToAddRows = false;
            this.dgvCategories.AllowUserToDeleteRows = false;
            this.dgvCategories.AllowUserToResizeRows = false;
            this.dgvCategories.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvCategories.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCategories.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCategories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCategories.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Visible,
            this.cmnStyle,
            this.cmnName,
            this.cmnExpression,
            this.cmnCount});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCategories.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCategories.GridColor = System.Drawing.SystemColors.ButtonFace;
            this.dgvCategories.Location = new System.Drawing.Point(4, 21);
            this.dgvCategories.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvCategories.Name = "dgvCategories";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCategories.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCategories.RowHeadersVisible = false;
            this.dgvCategories.RowHeadersWidth = 15;
            this.dgvCategories.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvCategories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvCategories.ShowCellErrors = false;
            this.dgvCategories.Size = new System.Drawing.Size(448, 229);
            this.dgvCategories.TabIndex = 93;
            // 
            // ID
            // 
            this.ID.Frozen = true;
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ID.Visible = false;
            this.ID.Width = 30;
            // 
            // Visible
            // 
            this.Visible.HeaderText = "";
            this.Visible.Name = "Visible";
            this.Visible.Width = 30;
            // 
            // cmnStyle
            // 
            this.cmnStyle.HeaderText = "样式";
            this.cmnStyle.Name = "cmnStyle";
            this.cmnStyle.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cmnStyle.Width = 50;
            // 
            // cmnName
            // 
            this.cmnName.HeaderText = "名字";
            this.cmnName.Name = "cmnName";
            this.cmnName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cmnName.Width = 120;
            // 
            // cmnExpression
            // 
            this.cmnExpression.HeaderText = "Expression";
            this.cmnExpression.Name = "cmnExpression";
            this.cmnExpression.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cmnExpression.Visible = false;
            this.cmnExpression.Width = 5;
            // 
            // cmnCount
            // 
            this.cmnCount.HeaderText = "数量";
            this.cmnCount.Name = "cmnCount";
            this.cmnCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cmnCount.Width = 40;
            // 
            // btnCategoryGenerate
            // 
            this.btnCategoryGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCategoryGenerate.Location = new System.Drawing.Point(704, 25);
            this.btnCategoryGenerate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCategoryGenerate.Name = "btnCategoryGenerate";
            this.btnCategoryGenerate.Size = new System.Drawing.Size(124, 30);
            this.btnCategoryGenerate.TabIndex = 132;
            this.btnCategoryGenerate.Text = "生成";
            this.btnCategoryGenerate.UseVisualStyleBackColor = true;
            // 
            // btnCategoryAppearance
            // 
            this.btnCategoryAppearance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCategoryAppearance.Location = new System.Drawing.Point(704, 77);
            this.btnCategoryAppearance.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCategoryAppearance.Name = "btnCategoryAppearance";
            this.btnCategoryAppearance.Size = new System.Drawing.Size(124, 30);
            this.btnCategoryAppearance.TabIndex = 133;
            this.btnCategoryAppearance.Text = "外观...";
            this.btnCategoryAppearance.UseVisualStyleBackColor = true;
            // 
            // btnCategoryRemove
            // 
            this.btnCategoryRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCategoryRemove.Location = new System.Drawing.Point(704, 135);
            this.btnCategoryRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCategoryRemove.Name = "btnCategoryRemove";
            this.btnCategoryRemove.Size = new System.Drawing.Size(124, 30);
            this.btnCategoryRemove.TabIndex = 134;
            this.btnCategoryRemove.Text = "移除";
            this.btnCategoryRemove.UseVisualStyleBackColor = true;
            // 
            // btnCategoryClear
            // 
            this.btnCategoryClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCategoryClear.Location = new System.Drawing.Point(704, 190);
            this.btnCategoryClear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCategoryClear.Name = "btnCategoryClear";
            this.btnCategoryClear.Size = new System.Drawing.Size(124, 30);
            this.btnCategoryClear.TabIndex = 135;
            this.btnCategoryClear.Text = "清除";
            this.btnCategoryClear.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(624, 469);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(124, 30);
            this.btnOk.TabIndex = 137;
            this.btnOk.Text = "确认";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(760, 469);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(124, 30);
            this.btnCancel.TabIndex = 138;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ProsperityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 503);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tabControl1);
            this.Name = "ProsperityForm";
            this.Text = "ProsperityForm";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategories)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox chkUniqueValues;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstFields1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnCategoryClear;
        private System.Windows.Forms.Button btnCategoryRemove;
        private System.Windows.Forms.Button btnCategoryAppearance;
        private System.Windows.Forms.Button btnCategoryGenerate;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.DataGridView dgvCategories;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Visible;
        private System.Windows.Forms.DataGridViewImageColumn cmnStyle;
        private System.Windows.Forms.DataGridViewTextBoxColumn cmnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cmnExpression;
        private System.Windows.Forms.DataGridViewTextBoxColumn cmnCount;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox chkRandomColors;
        private System.Windows.Forms.CheckBox chkSetGradient;
        private System.Windows.Forms.Button btnChangeColorScheme;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}