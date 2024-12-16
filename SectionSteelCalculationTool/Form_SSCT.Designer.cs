/*==============================================================================
 *  SectionSteelCalculationTool - A tool that assists Excel in calculating 
 *  quantities of steel structures
 *
 *  Copyright © 2024 Huang YongXing.                 
 *
 *  This library is free software, licensed under the terms of the GNU 
 *  General Public License as published by the Free Software Foundation, 
 *  either version 3 of the License, or (at your option) any later version. 
 *  You should have received a copy of the GNU General Public License 
 *  along with this program. If not, see <http://www.gnu.org/licenses/>. 
 *==============================================================================
 *  Form_SSCT.Designer.cs: form designer for user interface
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
namespace SectionSteelCalculationTool {
    partial class Form_SSCT {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_SSCT));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_Unit_Area_Weight = new System.Windows.Forms.TabPage();
            this.gBox_PI_Style = new System.Windows.Forms.GroupBox();
            this.rButton_PI_NUM = new System.Windows.Forms.RadioButton();
            this.rButton_PI_FUNC = new System.Windows.Forms.RadioButton();
            this.gBox_Accuracy = new System.Windows.Forms.GroupBox();
            this.rButton_GB_Data = new System.Windows.Forms.RadioButton();
            this.rButton_Precisely = new System.Windows.Forms.RadioButton();
            this.rButton_Roughly = new System.Windows.Forms.RadioButton();
            this.gBox_AW_Target_Offsets = new System.Windows.Forms.GroupBox();
            this.cBox_AW_Overwrite = new System.Windows.Forms.CheckBox();
            this.numUD_AW_Columns = new System.Windows.Forms.NumericUpDown();
            this.numUD_AW_Rows = new System.Windows.Forms.NumericUpDown();
            this.label_AW_Columns = new System.Windows.Forms.Label();
            this.label_AW_Rows = new System.Windows.Forms.Label();
            this.gBox_GenerationType = new System.Windows.Forms.GroupBox();
            this.cBox_ExcludeTopSurface = new System.Windows.Forms.CheckBox();
            this.rButton_Unit_Weight = new System.Windows.Forms.RadioButton();
            this.rButton_Unit_Area = new System.Windows.Forms.RadioButton();
            this.button_AW_Generate = new System.Windows.Forms.Button();
            this.tabPage_Stiffener = new System.Windows.Forms.TabPage();
            this.button_STIF_Generate = new System.Windows.Forms.Button();
            this.gBox_STIF_Target_Offsets = new System.Windows.Forms.GroupBox();
            this.label_STIF_Rows = new System.Windows.Forms.Label();
            this.numUD_STIF_Columns = new System.Windows.Forms.NumericUpDown();
            this.cBox_STIF_Overwrite = new System.Windows.Forms.CheckBox();
            this.numUD_STIF_Rows = new System.Windows.Forms.NumericUpDown();
            this.label_STIF_Columns = new System.Windows.Forms.Label();
            this.cBox_Trunc = new System.Windows.Forms.CheckBox();
            this.tabPage_Option = new System.Windows.Forms.TabPage();
            this.cBox_AlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage_Unit_Area_Weight.SuspendLayout();
            this.gBox_PI_Style.SuspendLayout();
            this.gBox_Accuracy.SuspendLayout();
            this.gBox_AW_Target_Offsets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_AW_Columns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_AW_Rows)).BeginInit();
            this.gBox_GenerationType.SuspendLayout();
            this.tabPage_Stiffener.SuspendLayout();
            this.gBox_STIF_Target_Offsets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_STIF_Columns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_STIF_Rows)).BeginInit();
            this.tabPage_Option.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_Unit_Area_Weight);
            this.tabControl1.Controls.Add(this.tabPage_Stiffener);
            this.tabControl1.Controls.Add(this.tabPage_Option);
            this.tabControl1.Location = new System.Drawing.Point(6, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(316, 238);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage_Unit_Area_Weight
            // 
            this.tabPage_Unit_Area_Weight.Controls.Add(this.gBox_PI_Style);
            this.tabPage_Unit_Area_Weight.Controls.Add(this.gBox_Accuracy);
            this.tabPage_Unit_Area_Weight.Controls.Add(this.gBox_AW_Target_Offsets);
            this.tabPage_Unit_Area_Weight.Controls.Add(this.gBox_GenerationType);
            this.tabPage_Unit_Area_Weight.Controls.Add(this.button_AW_Generate);
            this.tabPage_Unit_Area_Weight.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Unit_Area_Weight.Name = "tabPage_Unit_Area_Weight";
            this.tabPage_Unit_Area_Weight.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Unit_Area_Weight.Size = new System.Drawing.Size(308, 212);
            this.tabPage_Unit_Area_Weight.TabIndex = 0;
            this.tabPage_Unit_Area_Weight.Text = "Unit Area/Weight";
            this.tabPage_Unit_Area_Weight.UseVisualStyleBackColor = true;
            // 
            // gBox_PI_Style
            // 
            this.gBox_PI_Style.Controls.Add(this.rButton_PI_NUM);
            this.gBox_PI_Style.Controls.Add(this.rButton_PI_FUNC);
            this.gBox_PI_Style.Location = new System.Drawing.Point(162, 6);
            this.gBox_PI_Style.Name = "gBox_PI_Style";
            this.gBox_PI_Style.Size = new System.Drawing.Size(138, 45);
            this.gBox_PI_Style.TabIndex = 0;
            this.gBox_PI_Style.TabStop = false;
            this.gBox_PI_Style.Text = "π Style";
            // 
            // rButton_PI_NUM
            // 
            this.rButton_PI_NUM.AutoSize = true;
            this.rButton_PI_NUM.Location = new System.Drawing.Point(80, 20);
            this.rButton_PI_NUM.Name = "rButton_PI_NUM";
            this.rButton_PI_NUM.Size = new System.Drawing.Size(47, 16);
            this.rButton_PI_NUM.TabIndex = 1;
            this.rButton_PI_NUM.Text = "&3.14";
            this.rButton_PI_NUM.UseVisualStyleBackColor = true;
            // 
            // rButton_PI_FUNC
            // 
            this.rButton_PI_FUNC.AutoSize = true;
            this.rButton_PI_FUNC.Checked = true;
            this.rButton_PI_FUNC.Location = new System.Drawing.Point(6, 20);
            this.rButton_PI_FUNC.Name = "rButton_PI_FUNC";
            this.rButton_PI_FUNC.Size = new System.Drawing.Size(47, 16);
            this.rButton_PI_FUNC.TabIndex = 0;
            this.rButton_PI_FUNC.TabStop = true;
            this.rButton_PI_FUNC.Text = "&PI()";
            this.rButton_PI_FUNC.UseVisualStyleBackColor = true;
            // 
            // gBox_Accuracy
            // 
            this.gBox_Accuracy.Controls.Add(this.rButton_GB_Data);
            this.gBox_Accuracy.Controls.Add(this.rButton_Precisely);
            this.gBox_Accuracy.Controls.Add(this.rButton_Roughly);
            this.gBox_Accuracy.Location = new System.Drawing.Point(6, 106);
            this.gBox_Accuracy.Name = "gBox_Accuracy";
            this.gBox_Accuracy.Size = new System.Drawing.Size(150, 100);
            this.gBox_Accuracy.TabIndex = 1;
            this.gBox_Accuracy.TabStop = false;
            this.gBox_Accuracy.Text = "Accuracy";
            // 
            // rButton_GB_Data
            // 
            this.rButton_GB_Data.AutoSize = true;
            this.rButton_GB_Data.Checked = true;
            this.rButton_GB_Data.Location = new System.Drawing.Point(7, 67);
            this.rButton_GB_Data.Name = "rButton_GB_Data";
            this.rButton_GB_Data.Size = new System.Drawing.Size(65, 16);
            this.rButton_GB_Data.TabIndex = 2;
            this.rButton_GB_Data.TabStop = true;
            this.rButton_GB_Data.Text = "GB &Data";
            this.rButton_GB_Data.UseVisualStyleBackColor = true;
            // 
            // rButton_Precisely
            // 
            this.rButton_Precisely.AutoSize = true;
            this.rButton_Precisely.Location = new System.Drawing.Point(7, 44);
            this.rButton_Precisely.Name = "rButton_Precisely";
            this.rButton_Precisely.Size = new System.Drawing.Size(77, 16);
            this.rButton_Precisely.TabIndex = 1;
            this.rButton_Precisely.Text = "&Precisely";
            this.rButton_Precisely.UseVisualStyleBackColor = true;
            // 
            // rButton_Roughly
            // 
            this.rButton_Roughly.AutoSize = true;
            this.rButton_Roughly.Location = new System.Drawing.Point(7, 21);
            this.rButton_Roughly.Name = "rButton_Roughly";
            this.rButton_Roughly.Size = new System.Drawing.Size(65, 16);
            this.rButton_Roughly.TabIndex = 0;
            this.rButton_Roughly.Text = "R&oughly";
            this.rButton_Roughly.UseVisualStyleBackColor = true;
            // 
            // gBox_AW_Target_Offsets
            // 
            this.gBox_AW_Target_Offsets.Controls.Add(this.cBox_AW_Overwrite);
            this.gBox_AW_Target_Offsets.Controls.Add(this.numUD_AW_Columns);
            this.gBox_AW_Target_Offsets.Controls.Add(this.numUD_AW_Rows);
            this.gBox_AW_Target_Offsets.Controls.Add(this.label_AW_Columns);
            this.gBox_AW_Target_Offsets.Controls.Add(this.label_AW_Rows);
            this.gBox_AW_Target_Offsets.Location = new System.Drawing.Point(162, 51);
            this.gBox_AW_Target_Offsets.Name = "gBox_AW_Target_Offsets";
            this.gBox_AW_Target_Offsets.Size = new System.Drawing.Size(138, 105);
            this.gBox_AW_Target_Offsets.TabIndex = 0;
            this.gBox_AW_Target_Offsets.TabStop = false;
            this.gBox_AW_Target_Offsets.Text = "Target Offsets";
            // 
            // cBox_AW_Overwrite
            // 
            this.cBox_AW_Overwrite.Location = new System.Drawing.Point(6, 71);
            this.cBox_AW_Overwrite.Name = "cBox_AW_Overwrite";
            this.cBox_AW_Overwrite.Size = new System.Drawing.Size(104, 30);
            this.cBox_AW_Overwrite.TabIndex = 4;
            this.cBox_AW_Overwrite.Text = "Overwrite &existing data";
            this.cBox_AW_Overwrite.UseVisualStyleBackColor = true;
            // 
            // numUD_AW_Columns
            // 
            this.numUD_AW_Columns.Location = new System.Drawing.Point(59, 44);
            this.numUD_AW_Columns.Name = "numUD_AW_Columns";
            this.numUD_AW_Columns.Size = new System.Drawing.Size(40, 21);
            this.numUD_AW_Columns.TabIndex = 3;
            this.numUD_AW_Columns.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numUD_AW_Columns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numUD_AW_Rows
            // 
            this.numUD_AW_Rows.Location = new System.Drawing.Point(59, 17);
            this.numUD_AW_Rows.Name = "numUD_AW_Rows";
            this.numUD_AW_Rows.Size = new System.Drawing.Size(40, 21);
            this.numUD_AW_Rows.TabIndex = 1;
            this.numUD_AW_Rows.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_AW_Columns
            // 
            this.label_AW_Columns.AutoSize = true;
            this.label_AW_Columns.Location = new System.Drawing.Point(6, 47);
            this.label_AW_Columns.Name = "label_AW_Columns";
            this.label_AW_Columns.Size = new System.Drawing.Size(47, 12);
            this.label_AW_Columns.TabIndex = 2;
            this.label_AW_Columns.Text = "&Columns";
            // 
            // label_AW_Rows
            // 
            this.label_AW_Rows.AutoSize = true;
            this.label_AW_Rows.Location = new System.Drawing.Point(6, 21);
            this.label_AW_Rows.Name = "label_AW_Rows";
            this.label_AW_Rows.Size = new System.Drawing.Size(29, 12);
            this.label_AW_Rows.TabIndex = 0;
            this.label_AW_Rows.Text = "&Rows";
            // 
            // gBox_GenerationType
            // 
            this.gBox_GenerationType.Controls.Add(this.cBox_ExcludeTopSurface);
            this.gBox_GenerationType.Controls.Add(this.rButton_Unit_Weight);
            this.gBox_GenerationType.Controls.Add(this.rButton_Unit_Area);
            this.gBox_GenerationType.Location = new System.Drawing.Point(6, 6);
            this.gBox_GenerationType.Name = "gBox_GenerationType";
            this.gBox_GenerationType.Size = new System.Drawing.Size(150, 100);
            this.gBox_GenerationType.TabIndex = 1;
            this.gBox_GenerationType.TabStop = false;
            this.gBox_GenerationType.Text = "Generation Type";
            // 
            // cBox_ExcludeTopSurface
            // 
            this.cBox_ExcludeTopSurface.AutoSize = true;
            this.cBox_ExcludeTopSurface.Location = new System.Drawing.Point(6, 42);
            this.cBox_ExcludeTopSurface.Name = "cBox_ExcludeTopSurface";
            this.cBox_ExcludeTopSurface.Size = new System.Drawing.Size(138, 16);
            this.cBox_ExcludeTopSurface.TabIndex = 2;
            this.cBox_ExcludeTopSurface.Text = "Exclude &Top Surface";
            this.cBox_ExcludeTopSurface.UseVisualStyleBackColor = true;
            // 
            // rButton_Unit_Weight
            // 
            this.rButton_Unit_Weight.AutoSize = true;
            this.rButton_Unit_Weight.Location = new System.Drawing.Point(6, 64);
            this.rButton_Unit_Weight.Name = "rButton_Unit_Weight";
            this.rButton_Unit_Weight.Size = new System.Drawing.Size(89, 16);
            this.rButton_Unit_Weight.TabIndex = 1;
            this.rButton_Unit_Weight.Text = "Unit &Weight";
            this.rButton_Unit_Weight.UseVisualStyleBackColor = true;
            // 
            // rButton_Unit_Area
            // 
            this.rButton_Unit_Area.AutoSize = true;
            this.rButton_Unit_Area.Checked = true;
            this.rButton_Unit_Area.Location = new System.Drawing.Point(6, 20);
            this.rButton_Unit_Area.Name = "rButton_Unit_Area";
            this.rButton_Unit_Area.Size = new System.Drawing.Size(77, 16);
            this.rButton_Unit_Area.TabIndex = 0;
            this.rButton_Unit_Area.TabStop = true;
            this.rButton_Unit_Area.Text = "Unit &Area";
            this.rButton_Unit_Area.UseVisualStyleBackColor = true;
            this.rButton_Unit_Area.CheckedChanged += new System.EventHandler(this.UnitArea_CheckedChanged);
            // 
            // button_AW_Generate
            // 
            this.button_AW_Generate.Location = new System.Drawing.Point(181, 166);
            this.button_AW_Generate.Name = "button_AW_Generate";
            this.button_AW_Generate.Size = new System.Drawing.Size(100, 35);
            this.button_AW_Generate.TabIndex = 0;
            this.button_AW_Generate.Text = "&Generate";
            this.button_AW_Generate.UseVisualStyleBackColor = true;
            this.button_AW_Generate.Click += new System.EventHandler(this.Button_AW_Generate_Click);
            // 
            // tabPage_Stiffener
            // 
            this.tabPage_Stiffener.Controls.Add(this.button_STIF_Generate);
            this.tabPage_Stiffener.Controls.Add(this.gBox_STIF_Target_Offsets);
            this.tabPage_Stiffener.Controls.Add(this.cBox_Trunc);
            this.tabPage_Stiffener.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Stiffener.Name = "tabPage_Stiffener";
            this.tabPage_Stiffener.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Stiffener.Size = new System.Drawing.Size(308, 212);
            this.tabPage_Stiffener.TabIndex = 1;
            this.tabPage_Stiffener.Text = "Stiffener";
            this.tabPage_Stiffener.UseVisualStyleBackColor = true;
            // 
            // button_STIF_Generate
            // 
            this.button_STIF_Generate.Location = new System.Drawing.Point(181, 166);
            this.button_STIF_Generate.Name = "button_STIF_Generate";
            this.button_STIF_Generate.Size = new System.Drawing.Size(100, 35);
            this.button_STIF_Generate.TabIndex = 7;
            this.button_STIF_Generate.Text = "&Generate";
            this.button_STIF_Generate.UseVisualStyleBackColor = true;
            this.button_STIF_Generate.Click += new System.EventHandler(this.Button_STIF_Generate_Click);
            // 
            // gBox_STIF_Target_Offsets
            // 
            this.gBox_STIF_Target_Offsets.Controls.Add(this.label_STIF_Rows);
            this.gBox_STIF_Target_Offsets.Controls.Add(this.numUD_STIF_Columns);
            this.gBox_STIF_Target_Offsets.Controls.Add(this.cBox_STIF_Overwrite);
            this.gBox_STIF_Target_Offsets.Controls.Add(this.numUD_STIF_Rows);
            this.gBox_STIF_Target_Offsets.Controls.Add(this.label_STIF_Columns);
            this.gBox_STIF_Target_Offsets.Location = new System.Drawing.Point(6, 42);
            this.gBox_STIF_Target_Offsets.Name = "gBox_STIF_Target_Offsets";
            this.gBox_STIF_Target_Offsets.Size = new System.Drawing.Size(138, 115);
            this.gBox_STIF_Target_Offsets.TabIndex = 6;
            this.gBox_STIF_Target_Offsets.TabStop = false;
            this.gBox_STIF_Target_Offsets.Text = "Target Offsets";
            // 
            // label_STIF_Rows
            // 
            this.label_STIF_Rows.AutoSize = true;
            this.label_STIF_Rows.Location = new System.Drawing.Point(6, 31);
            this.label_STIF_Rows.Name = "label_STIF_Rows";
            this.label_STIF_Rows.Size = new System.Drawing.Size(29, 12);
            this.label_STIF_Rows.TabIndex = 2;
            this.label_STIF_Rows.Text = "&Rows";
            // 
            // numUD_STIF_Columns
            // 
            this.numUD_STIF_Columns.Location = new System.Drawing.Point(59, 54);
            this.numUD_STIF_Columns.Name = "numUD_STIF_Columns";
            this.numUD_STIF_Columns.Size = new System.Drawing.Size(40, 21);
            this.numUD_STIF_Columns.TabIndex = 5;
            this.numUD_STIF_Columns.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cBox_STIF_Overwrite
            // 
            this.cBox_STIF_Overwrite.Location = new System.Drawing.Point(6, 81);
            this.cBox_STIF_Overwrite.Name = "cBox_STIF_Overwrite";
            this.cBox_STIF_Overwrite.Size = new System.Drawing.Size(104, 30);
            this.cBox_STIF_Overwrite.TabIndex = 1;
            this.cBox_STIF_Overwrite.Text = "Overwrite &existing data";
            this.cBox_STIF_Overwrite.UseVisualStyleBackColor = true;
            // 
            // numUD_STIF_Rows
            // 
            this.numUD_STIF_Rows.Location = new System.Drawing.Point(59, 27);
            this.numUD_STIF_Rows.Name = "numUD_STIF_Rows";
            this.numUD_STIF_Rows.Size = new System.Drawing.Size(40, 21);
            this.numUD_STIF_Rows.TabIndex = 3;
            this.numUD_STIF_Rows.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numUD_STIF_Rows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label_STIF_Columns
            // 
            this.label_STIF_Columns.AutoSize = true;
            this.label_STIF_Columns.Location = new System.Drawing.Point(6, 57);
            this.label_STIF_Columns.Name = "label_STIF_Columns";
            this.label_STIF_Columns.Size = new System.Drawing.Size(47, 12);
            this.label_STIF_Columns.TabIndex = 4;
            this.label_STIF_Columns.Text = "&Columns";
            // 
            // cBox_Trunc
            // 
            this.cBox_Trunc.Checked = true;
            this.cBox_Trunc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBox_Trunc.Location = new System.Drawing.Point(12, 6);
            this.cBox_Trunc.Name = "cBox_Trunc";
            this.cBox_Trunc.Size = new System.Drawing.Size(104, 30);
            this.cBox_Trunc.TabIndex = 0;
            this.cBox_Trunc.Text = "&Truncate the parameters";
            this.cBox_Trunc.UseVisualStyleBackColor = true;
            // 
            // tabPage_Option
            // 
            this.tabPage_Option.Controls.Add(this.cBox_AlwaysOnTop);
            this.tabPage_Option.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Option.Name = "tabPage_Option";
            this.tabPage_Option.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Option.Size = new System.Drawing.Size(308, 212);
            this.tabPage_Option.TabIndex = 2;
            this.tabPage_Option.Text = "Option";
            this.tabPage_Option.UseVisualStyleBackColor = true;
            // 
            // cBox_AlwaysOnTop
            // 
            this.cBox_AlwaysOnTop.AutoSize = true;
            this.cBox_AlwaysOnTop.Checked = true;
            this.cBox_AlwaysOnTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBox_AlwaysOnTop.Location = new System.Drawing.Point(6, 6);
            this.cBox_AlwaysOnTop.Name = "cBox_AlwaysOnTop";
            this.cBox_AlwaysOnTop.Size = new System.Drawing.Size(102, 16);
            this.cBox_AlwaysOnTop.TabIndex = 0;
            this.cBox_AlwaysOnTop.Text = "&Always on top";
            this.cBox_AlwaysOnTop.UseVisualStyleBackColor = true;
            this.cBox_AlwaysOnTop.CheckedChanged += new System.EventHandler(this.CBox_AlwaysOnTop_CheckedChanged);
            // 
            // Form_SSCT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 249);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_SSCT";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SSCT";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_SSCT_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage_Unit_Area_Weight.ResumeLayout(false);
            this.gBox_PI_Style.ResumeLayout(false);
            this.gBox_PI_Style.PerformLayout();
            this.gBox_Accuracy.ResumeLayout(false);
            this.gBox_Accuracy.PerformLayout();
            this.gBox_AW_Target_Offsets.ResumeLayout(false);
            this.gBox_AW_Target_Offsets.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_AW_Columns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_AW_Rows)).EndInit();
            this.gBox_GenerationType.ResumeLayout(false);
            this.gBox_GenerationType.PerformLayout();
            this.tabPage_Stiffener.ResumeLayout(false);
            this.gBox_STIF_Target_Offsets.ResumeLayout(false);
            this.gBox_STIF_Target_Offsets.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_STIF_Columns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_STIF_Rows)).EndInit();
            this.tabPage_Option.ResumeLayout(false);
            this.tabPage_Option.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_Unit_Area_Weight;
        private System.Windows.Forms.TabPage tabPage_Stiffener;
        private System.Windows.Forms.TabPage tabPage_Option;
        private System.Windows.Forms.Button button_AW_Generate;
        private System.Windows.Forms.GroupBox gBox_GenerationType;
        private System.Windows.Forms.GroupBox gBox_PI_Style;
        private System.Windows.Forms.GroupBox gBox_Accuracy;
        private System.Windows.Forms.GroupBox gBox_AW_Target_Offsets;
        private System.Windows.Forms.CheckBox cBox_ExcludeTopSurface;
        private System.Windows.Forms.RadioButton rButton_Unit_Weight;
        private System.Windows.Forms.RadioButton rButton_Unit_Area;
        private System.Windows.Forms.RadioButton rButton_PI_NUM;
        private System.Windows.Forms.RadioButton rButton_PI_FUNC;
        private System.Windows.Forms.RadioButton rButton_GB_Data;
        private System.Windows.Forms.RadioButton rButton_Precisely;
        private System.Windows.Forms.RadioButton rButton_Roughly;
        private System.Windows.Forms.Label label_AW_Columns;
        private System.Windows.Forms.Label label_AW_Rows;
        private System.Windows.Forms.NumericUpDown numUD_AW_Columns;
        private System.Windows.Forms.NumericUpDown numUD_AW_Rows;
        private System.Windows.Forms.CheckBox cBox_AW_Overwrite;
        private System.Windows.Forms.NumericUpDown numUD_STIF_Columns;
        private System.Windows.Forms.NumericUpDown numUD_STIF_Rows;
        private System.Windows.Forms.Label label_STIF_Columns;
        private System.Windows.Forms.Label label_STIF_Rows;
        private System.Windows.Forms.CheckBox cBox_STIF_Overwrite;
        private System.Windows.Forms.CheckBox cBox_Trunc;
        private System.Windows.Forms.GroupBox gBox_STIF_Target_Offsets;
        private System.Windows.Forms.Button button_STIF_Generate;
        private System.Windows.Forms.CheckBox cBox_AlwaysOnTop;
    }
}

