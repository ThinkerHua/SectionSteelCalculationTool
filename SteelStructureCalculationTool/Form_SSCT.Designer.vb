<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form_SSCT
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form_SSCT))
        Me.TabControl_SteelStructureCalculationTool = New System.Windows.Forms.TabControl()
        Me.TabPage_Generate_Unit_Area_or_Weight = New System.Windows.Forms.TabPage()
        Me.GBox_PI_STYLE = New System.Windows.Forms.GroupBox()
        Me.RBut_PI_FUN = New System.Windows.Forms.RadioButton()
        Me.RBut_PI_NUM = New System.Windows.Forms.RadioButton()
        Me.Button_AW_Generate = New System.Windows.Forms.Button()
        Me.GBox_AW_Offset = New System.Windows.Forms.GroupBox()
        Me.CBox_AW_Overwrite = New System.Windows.Forms.CheckBox()
        Me.Label_AW_Columns = New System.Windows.Forms.Label()
        Me.NumUD_AW_Columns = New System.Windows.Forms.NumericUpDown()
        Me.Label_AW_Rows = New System.Windows.Forms.Label()
        Me.NumUD_AW_Rows = New System.Windows.Forms.NumericUpDown()
        Me.GBox_CalculationMethod = New System.Windows.Forms.GroupBox()
        Me.RBut_LookUpInTable = New System.Windows.Forms.RadioButton()
        Me.RBut_Precisely = New System.Windows.Forms.RadioButton()
        Me.RBut_Roughly = New System.Windows.Forms.RadioButton()
        Me.GBox_CalculationType = New System.Windows.Forms.GroupBox()
        Me.RBut_UnitWeight = New System.Windows.Forms.RadioButton()
        Me.CBox_ExcludeTopSurface = New System.Windows.Forms.CheckBox()
        Me.RBut_UnitArea = New System.Windows.Forms.RadioButton()
        Me.TabPage_Stiffener = New System.Windows.Forms.TabPage()
        Me.Button_Stif_Generate = New System.Windows.Forms.Button()
        Me.GBox_Stif_Offset = New System.Windows.Forms.GroupBox()
        Me.CBox_Stif_Overwrite = New System.Windows.Forms.CheckBox()
        Me.Label_Stif_Columns = New System.Windows.Forms.Label()
        Me.NumUD_Stif_Columns = New System.Windows.Forms.NumericUpDown()
        Me.Label_Stif_Rows = New System.Windows.Forms.Label()
        Me.NumUD_Stif_Rows = New System.Windows.Forms.NumericUpDown()
        Me.TabPage_Export_table = New System.Windows.Forms.TabPage()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CBox_Trunc = New System.Windows.Forms.CheckBox()
        Me.TabControl_SteelStructureCalculationTool.SuspendLayout()
        Me.TabPage_Generate_Unit_Area_or_Weight.SuspendLayout()
        Me.GBox_PI_STYLE.SuspendLayout()
        Me.GBox_AW_Offset.SuspendLayout()
        CType(Me.NumUD_AW_Columns, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumUD_AW_Rows, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GBox_CalculationMethod.SuspendLayout()
        Me.GBox_CalculationType.SuspendLayout()
        Me.TabPage_Stiffener.SuspendLayout()
        Me.GBox_Stif_Offset.SuspendLayout()
        CType(Me.NumUD_Stif_Columns, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumUD_Stif_Rows, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage_Export_table.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl_SteelStructureCalculationTool
        '
        Me.TabControl_SteelStructureCalculationTool.Controls.Add(Me.TabPage_Generate_Unit_Area_or_Weight)
        Me.TabControl_SteelStructureCalculationTool.Controls.Add(Me.TabPage_Stiffener)
        Me.TabControl_SteelStructureCalculationTool.Controls.Add(Me.TabPage_Export_table)
        Me.TabControl_SteelStructureCalculationTool.Location = New System.Drawing.Point(6, 6)
        Me.TabControl_SteelStructureCalculationTool.Name = "TabControl_SteelStructureCalculationTool"
        Me.TabControl_SteelStructureCalculationTool.SelectedIndex = 0
        Me.TabControl_SteelStructureCalculationTool.Size = New System.Drawing.Size(316, 238)
        Me.TabControl_SteelStructureCalculationTool.TabIndex = 0
        '
        'TabPage_Generate_Unit_Area_or_Weight
        '
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.GBox_PI_STYLE)
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.Button_AW_Generate)
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.GBox_AW_Offset)
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.GBox_CalculationMethod)
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.GBox_CalculationType)
        Me.TabPage_Generate_Unit_Area_or_Weight.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_Generate_Unit_Area_or_Weight.Name = "TabPage_Generate_Unit_Area_or_Weight"
        Me.TabPage_Generate_Unit_Area_or_Weight.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_Generate_Unit_Area_or_Weight.Size = New System.Drawing.Size(308, 212)
        Me.TabPage_Generate_Unit_Area_or_Weight.TabIndex = 0
        Me.TabPage_Generate_Unit_Area_or_Weight.Text = "Generate Unit Area/Weight"
        Me.TabPage_Generate_Unit_Area_or_Weight.UseVisualStyleBackColor = True
        '
        'GBox_PI_STYLE
        '
        Me.GBox_PI_STYLE.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GBox_PI_STYLE.Controls.Add(Me.RBut_PI_FUN)
        Me.GBox_PI_STYLE.Controls.Add(Me.RBut_PI_NUM)
        Me.GBox_PI_STYLE.Location = New System.Drawing.Point(162, 6)
        Me.GBox_PI_STYLE.Name = "GBox_PI_STYLE"
        Me.GBox_PI_STYLE.Size = New System.Drawing.Size(138, 45)
        Me.GBox_PI_STYLE.TabIndex = 2
        Me.GBox_PI_STYLE.TabStop = False
        Me.GBox_PI_STYLE.Text = "π Style"
        '
        'RBut_PI_FUN
        '
        Me.RBut_PI_FUN.AutoSize = True
        Me.RBut_PI_FUN.Checked = True
        Me.RBut_PI_FUN.Location = New System.Drawing.Point(6, 20)
        Me.RBut_PI_FUN.Name = "RBut_PI_FUN"
        Me.RBut_PI_FUN.Size = New System.Drawing.Size(47, 16)
        Me.RBut_PI_FUN.TabIndex = 0
        Me.RBut_PI_FUN.TabStop = True
        Me.RBut_PI_FUN.Text = "P&I()"
        Me.RBut_PI_FUN.UseVisualStyleBackColor = True
        '
        'RBut_PI_NUM
        '
        Me.RBut_PI_NUM.AutoSize = True
        Me.RBut_PI_NUM.Location = New System.Drawing.Point(80, 20)
        Me.RBut_PI_NUM.Name = "RBut_PI_NUM"
        Me.RBut_PI_NUM.Size = New System.Drawing.Size(47, 16)
        Me.RBut_PI_NUM.TabIndex = 1
        Me.RBut_PI_NUM.TabStop = True
        Me.RBut_PI_NUM.Text = "&3.14"
        Me.RBut_PI_NUM.UseVisualStyleBackColor = True
        '
        'Button_AW_Generate
        '
        Me.Button_AW_Generate.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button_AW_Generate.Location = New System.Drawing.Point(181, 171)
        Me.Button_AW_Generate.Name = "Button_AW_Generate"
        Me.Button_AW_Generate.Size = New System.Drawing.Size(100, 35)
        Me.Button_AW_Generate.TabIndex = 4
        Me.Button_AW_Generate.Text = "&Generate"
        Me.Button_AW_Generate.UseVisualStyleBackColor = True
        '
        'GBox_AW_Offset
        '
        Me.GBox_AW_Offset.Controls.Add(Me.CBox_AW_Overwrite)
        Me.GBox_AW_Offset.Controls.Add(Me.Label_AW_Columns)
        Me.GBox_AW_Offset.Controls.Add(Me.NumUD_AW_Columns)
        Me.GBox_AW_Offset.Controls.Add(Me.Label_AW_Rows)
        Me.GBox_AW_Offset.Controls.Add(Me.NumUD_AW_Rows)
        Me.GBox_AW_Offset.Location = New System.Drawing.Point(162, 51)
        Me.GBox_AW_Offset.Name = "GBox_AW_Offset"
        Me.GBox_AW_Offset.Size = New System.Drawing.Size(138, 115)
        Me.GBox_AW_Offset.TabIndex = 3
        Me.GBox_AW_Offset.TabStop = False
        Me.GBox_AW_Offset.Text = "Destination offset from source"
        '
        'CBox_AW_Overwrite
        '
        Me.CBox_AW_Overwrite.Location = New System.Drawing.Point(6, 81)
        Me.CBox_AW_Overwrite.Name = "CBox_AW_Overwrite"
        Me.CBox_AW_Overwrite.Size = New System.Drawing.Size(104, 30)
        Me.CBox_AW_Overwrite.TabIndex = 4
        Me.CBox_AW_Overwrite.Text = "Overwrite &existing data"
        Me.CBox_AW_Overwrite.UseVisualStyleBackColor = True
        '
        'Label_AW_Columns
        '
        Me.Label_AW_Columns.AutoSize = True
        Me.Label_AW_Columns.Location = New System.Drawing.Point(6, 57)
        Me.Label_AW_Columns.Name = "Label_AW_Columns"
        Me.Label_AW_Columns.Size = New System.Drawing.Size(47, 12)
        Me.Label_AW_Columns.TabIndex = 2
        Me.Label_AW_Columns.Text = "&Columns"
        '
        'NumUD_AW_Columns
        '
        Me.NumUD_AW_Columns.Location = New System.Drawing.Point(59, 54)
        Me.NumUD_AW_Columns.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.NumUD_AW_Columns.Name = "NumUD_AW_Columns"
        Me.NumUD_AW_Columns.Size = New System.Drawing.Size(40, 21)
        Me.NumUD_AW_Columns.TabIndex = 3
        Me.NumUD_AW_Columns.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.NumUD_AW_Columns.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label_AW_Rows
        '
        Me.Label_AW_Rows.AutoSize = True
        Me.Label_AW_Rows.Location = New System.Drawing.Point(6, 31)
        Me.Label_AW_Rows.Name = "Label_AW_Rows"
        Me.Label_AW_Rows.Size = New System.Drawing.Size(29, 12)
        Me.Label_AW_Rows.TabIndex = 0
        Me.Label_AW_Rows.Text = "&Rows"
        '
        'NumUD_AW_Rows
        '
        Me.NumUD_AW_Rows.Location = New System.Drawing.Point(59, 27)
        Me.NumUD_AW_Rows.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.NumUD_AW_Rows.Name = "NumUD_AW_Rows"
        Me.NumUD_AW_Rows.Size = New System.Drawing.Size(40, 21)
        Me.NumUD_AW_Rows.TabIndex = 1
        Me.NumUD_AW_Rows.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GBox_CalculationMethod
        '
        Me.GBox_CalculationMethod.Controls.Add(Me.RBut_LookUpInTable)
        Me.GBox_CalculationMethod.Controls.Add(Me.RBut_Precisely)
        Me.GBox_CalculationMethod.Controls.Add(Me.RBut_Roughly)
        Me.GBox_CalculationMethod.Location = New System.Drawing.Point(6, 106)
        Me.GBox_CalculationMethod.Name = "GBox_CalculationMethod"
        Me.GBox_CalculationMethod.Size = New System.Drawing.Size(150, 100)
        Me.GBox_CalculationMethod.TabIndex = 1
        Me.GBox_CalculationMethod.TabStop = False
        Me.GBox_CalculationMethod.Text = "Calculation Method"
        '
        'RBut_LookUpInTable
        '
        Me.RBut_LookUpInTable.AutoSize = True
        Me.RBut_LookUpInTable.Location = New System.Drawing.Point(7, 67)
        Me.RBut_LookUpInTable.Name = "RBut_LookUpInTable"
        Me.RBut_LookUpInTable.Size = New System.Drawing.Size(119, 16)
        Me.RBut_LookUpInTable.TabIndex = 2
        Me.RBut_LookUpInTable.TabStop = True
        Me.RBut_LookUpInTable.Text = "&Look up in table"
        Me.RBut_LookUpInTable.UseVisualStyleBackColor = True
        '
        'RBut_Precisely
        '
        Me.RBut_Precisely.AutoSize = True
        Me.RBut_Precisely.Location = New System.Drawing.Point(7, 44)
        Me.RBut_Precisely.Name = "RBut_Precisely"
        Me.RBut_Precisely.Size = New System.Drawing.Size(77, 16)
        Me.RBut_Precisely.TabIndex = 1
        Me.RBut_Precisely.TabStop = True
        Me.RBut_Precisely.Text = "&Precisely"
        Me.RBut_Precisely.UseVisualStyleBackColor = True
        '
        'RBut_Roughly
        '
        Me.RBut_Roughly.AutoSize = True
        Me.RBut_Roughly.Checked = True
        Me.RBut_Roughly.Location = New System.Drawing.Point(7, 21)
        Me.RBut_Roughly.Name = "RBut_Roughly"
        Me.RBut_Roughly.Size = New System.Drawing.Size(65, 16)
        Me.RBut_Roughly.TabIndex = 0
        Me.RBut_Roughly.TabStop = True
        Me.RBut_Roughly.Text = "R&oughly"
        Me.RBut_Roughly.UseVisualStyleBackColor = True
        '
        'GBox_CalculationType
        '
        Me.GBox_CalculationType.Controls.Add(Me.RBut_UnitWeight)
        Me.GBox_CalculationType.Controls.Add(Me.CBox_ExcludeTopSurface)
        Me.GBox_CalculationType.Controls.Add(Me.RBut_UnitArea)
        Me.GBox_CalculationType.Location = New System.Drawing.Point(6, 6)
        Me.GBox_CalculationType.Name = "GBox_CalculationType"
        Me.GBox_CalculationType.Size = New System.Drawing.Size(150, 100)
        Me.GBox_CalculationType.TabIndex = 0
        Me.GBox_CalculationType.TabStop = False
        Me.GBox_CalculationType.Text = "Calculation Type"
        '
        'RBut_UnitWeight
        '
        Me.RBut_UnitWeight.AutoSize = True
        Me.RBut_UnitWeight.Location = New System.Drawing.Point(6, 64)
        Me.RBut_UnitWeight.Name = "RBut_UnitWeight"
        Me.RBut_UnitWeight.Size = New System.Drawing.Size(89, 16)
        Me.RBut_UnitWeight.TabIndex = 2
        Me.RBut_UnitWeight.TabStop = True
        Me.RBut_UnitWeight.Text = "Unit &Weight"
        Me.RBut_UnitWeight.UseVisualStyleBackColor = True
        '
        'CBox_ExcludeTopSurface
        '
        Me.CBox_ExcludeTopSurface.AutoSize = True
        Me.CBox_ExcludeTopSurface.Location = New System.Drawing.Point(6, 42)
        Me.CBox_ExcludeTopSurface.Name = "CBox_ExcludeTopSurface"
        Me.CBox_ExcludeTopSurface.Size = New System.Drawing.Size(138, 16)
        Me.CBox_ExcludeTopSurface.TabIndex = 1
        Me.CBox_ExcludeTopSurface.Text = "Exclude &Top Surface"
        Me.CBox_ExcludeTopSurface.UseVisualStyleBackColor = True
        '
        'RBut_UnitArea
        '
        Me.RBut_UnitArea.AutoSize = True
        Me.RBut_UnitArea.Checked = True
        Me.RBut_UnitArea.Location = New System.Drawing.Point(6, 20)
        Me.RBut_UnitArea.Name = "RBut_UnitArea"
        Me.RBut_UnitArea.Size = New System.Drawing.Size(77, 16)
        Me.RBut_UnitArea.TabIndex = 0
        Me.RBut_UnitArea.TabStop = True
        Me.RBut_UnitArea.Text = "Unit &Area"
        Me.RBut_UnitArea.UseVisualStyleBackColor = True
        '
        'TabPage_Stiffener
        '
        Me.TabPage_Stiffener.Controls.Add(Me.CBox_Trunc)
        Me.TabPage_Stiffener.Controls.Add(Me.Button_Stif_Generate)
        Me.TabPage_Stiffener.Controls.Add(Me.GBox_Stif_Offset)
        Me.TabPage_Stiffener.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_Stiffener.Name = "TabPage_Stiffener"
        Me.TabPage_Stiffener.Size = New System.Drawing.Size(308, 212)
        Me.TabPage_Stiffener.TabIndex = 2
        Me.TabPage_Stiffener.Text = "Stiffener"
        Me.TabPage_Stiffener.UseVisualStyleBackColor = True
        '
        'Button_Stif_Generate
        '
        Me.Button_Stif_Generate.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button_Stif_Generate.Location = New System.Drawing.Point(184, 144)
        Me.Button_Stif_Generate.Name = "Button_Stif_Generate"
        Me.Button_Stif_Generate.Size = New System.Drawing.Size(100, 35)
        Me.Button_Stif_Generate.TabIndex = 5
        Me.Button_Stif_Generate.Text = "&Generate"
        Me.Button_Stif_Generate.UseVisualStyleBackColor = True
        '
        'GBox_Stif_Offset
        '
        Me.GBox_Stif_Offset.Controls.Add(Me.CBox_Stif_Overwrite)
        Me.GBox_Stif_Offset.Controls.Add(Me.Label_Stif_Columns)
        Me.GBox_Stif_Offset.Controls.Add(Me.NumUD_Stif_Columns)
        Me.GBox_Stif_Offset.Controls.Add(Me.Label_Stif_Rows)
        Me.GBox_Stif_Offset.Controls.Add(Me.NumUD_Stif_Rows)
        Me.GBox_Stif_Offset.Location = New System.Drawing.Point(24, 64)
        Me.GBox_Stif_Offset.Name = "GBox_Stif_Offset"
        Me.GBox_Stif_Offset.Size = New System.Drawing.Size(138, 115)
        Me.GBox_Stif_Offset.TabIndex = 4
        Me.GBox_Stif_Offset.TabStop = False
        Me.GBox_Stif_Offset.Text = "Destination offset from source"
        '
        'CBox_Stif_Overwrite
        '
        Me.CBox_Stif_Overwrite.Location = New System.Drawing.Point(6, 81)
        Me.CBox_Stif_Overwrite.Name = "CBox_Stif_Overwrite"
        Me.CBox_Stif_Overwrite.Size = New System.Drawing.Size(104, 30)
        Me.CBox_Stif_Overwrite.TabIndex = 4
        Me.CBox_Stif_Overwrite.Text = "Overwrite &existing data"
        Me.CBox_Stif_Overwrite.UseVisualStyleBackColor = True
        '
        'Label_Stif_Columns
        '
        Me.Label_Stif_Columns.AutoSize = True
        Me.Label_Stif_Columns.Location = New System.Drawing.Point(6, 57)
        Me.Label_Stif_Columns.Name = "Label_Stif_Columns"
        Me.Label_Stif_Columns.Size = New System.Drawing.Size(47, 12)
        Me.Label_Stif_Columns.TabIndex = 2
        Me.Label_Stif_Columns.Text = "&Columns"
        '
        'NumUD_Stif_Columns
        '
        Me.NumUD_Stif_Columns.Location = New System.Drawing.Point(59, 54)
        Me.NumUD_Stif_Columns.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.NumUD_Stif_Columns.Name = "NumUD_Stif_Columns"
        Me.NumUD_Stif_Columns.Size = New System.Drawing.Size(40, 21)
        Me.NumUD_Stif_Columns.TabIndex = 3
        Me.NumUD_Stif_Columns.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label_Stif_Rows
        '
        Me.Label_Stif_Rows.AutoSize = True
        Me.Label_Stif_Rows.Location = New System.Drawing.Point(6, 31)
        Me.Label_Stif_Rows.Name = "Label_Stif_Rows"
        Me.Label_Stif_Rows.Size = New System.Drawing.Size(29, 12)
        Me.Label_Stif_Rows.TabIndex = 0
        Me.Label_Stif_Rows.Text = "&Rows"
        '
        'NumUD_Stif_Rows
        '
        Me.NumUD_Stif_Rows.Location = New System.Drawing.Point(59, 27)
        Me.NumUD_Stif_Rows.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.NumUD_Stif_Rows.Name = "NumUD_Stif_Rows"
        Me.NumUD_Stif_Rows.Size = New System.Drawing.Size(40, 21)
        Me.NumUD_Stif_Rows.TabIndex = 1
        Me.NumUD_Stif_Rows.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.NumUD_Stif_Rows.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'TabPage_Export_table
        '
        Me.TabPage_Export_table.Controls.Add(Me.Label1)
        Me.TabPage_Export_table.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_Export_table.Name = "TabPage_Export_table"
        Me.TabPage_Export_table.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_Export_table.Size = New System.Drawing.Size(308, 212)
        Me.TabPage_Export_table.TabIndex = 1
        Me.TabPage_Export_table.Text = "Export table"
        Me.TabPage_Export_table.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(131, 83)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "To Do"
        '
        'CBox_Trunc
        '
        Me.CBox_Trunc.Location = New System.Drawing.Point(32, 16)
        Me.CBox_Trunc.Name = "CBox_Trunc"
        Me.CBox_Trunc.Size = New System.Drawing.Size(128, 40)
        Me.CBox_Trunc.TabIndex = 6
        Me.CBox_Trunc.Text = "&Truncate the parameters of specification"
        Me.CBox_Trunc.UseVisualStyleBackColor = True
        '
        'Form_SSCT
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
        Me.ClientSize = New System.Drawing.Size(326, 249)
        Me.Controls.Add(Me.TabControl_SteelStructureCalculationTool)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.Name = "Form_SSCT"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SSCT"
        Me.TabControl_SteelStructureCalculationTool.ResumeLayout(False)
        Me.TabPage_Generate_Unit_Area_or_Weight.ResumeLayout(False)
        Me.GBox_PI_STYLE.ResumeLayout(False)
        Me.GBox_PI_STYLE.PerformLayout()
        Me.GBox_AW_Offset.ResumeLayout(False)
        Me.GBox_AW_Offset.PerformLayout()
        CType(Me.NumUD_AW_Columns, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumUD_AW_Rows, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GBox_CalculationMethod.ResumeLayout(False)
        Me.GBox_CalculationMethod.PerformLayout()
        Me.GBox_CalculationType.ResumeLayout(False)
        Me.GBox_CalculationType.PerformLayout()
        Me.TabPage_Stiffener.ResumeLayout(False)
        Me.GBox_Stif_Offset.ResumeLayout(False)
        Me.GBox_Stif_Offset.PerformLayout()
        CType(Me.NumUD_Stif_Columns, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumUD_Stif_Rows, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage_Export_table.ResumeLayout(False)
        Me.TabPage_Export_table.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControl_SteelStructureCalculationTool As TabControl
    Friend WithEvents TabPage_Generate_Unit_Area_or_Weight As TabPage
    Friend WithEvents Button_AW_Generate As Button
    Friend WithEvents GBox_AW_Offset As GroupBox
    Friend WithEvents CBox_AW_Overwrite As CheckBox
    Friend WithEvents Label_AW_Columns As Label
    Friend WithEvents NumUD_AW_Columns As NumericUpDown
    Friend WithEvents Label_AW_Rows As Label
    Friend WithEvents NumUD_AW_Rows As NumericUpDown
    Friend WithEvents GBox_CalculationMethod As GroupBox
    Friend WithEvents RBut_LookUpInTable As RadioButton
    Friend WithEvents RBut_Precisely As RadioButton
    Friend WithEvents RBut_Roughly As RadioButton
    Friend WithEvents GBox_CalculationType As GroupBox
    Friend WithEvents RBut_UnitWeight As RadioButton
    Friend WithEvents CBox_ExcludeTopSurface As CheckBox
    Friend WithEvents RBut_UnitArea As RadioButton
    Friend WithEvents TabPage_Export_table As TabPage
    Friend WithEvents Label1 As Label
    Friend WithEvents RBut_PI_NUM As RadioButton
    Friend WithEvents RBut_PI_FUN As RadioButton
    Friend WithEvents GBox_PI_STYLE As GroupBox
    Friend WithEvents TabPage_Stiffener As TabPage
    Friend WithEvents Button_Stif_Generate As Button
    Friend WithEvents GBox_Stif_Offset As GroupBox
    Friend WithEvents CBox_Stif_Overwrite As CheckBox
    Friend WithEvents Label_Stif_Columns As Label
    Friend WithEvents NumUD_Stif_Columns As NumericUpDown
    Friend WithEvents Label_Stif_Rows As Label
    Friend WithEvents NumUD_Stif_Rows As NumericUpDown
    Friend WithEvents CBox_Trunc As CheckBox
End Class
