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
        Me.Button_Generate = New System.Windows.Forms.Button()
        Me.GBox_Offset = New System.Windows.Forms.GroupBox()
        Me.CBox_Overwrite = New System.Windows.Forms.CheckBox()
        Me.Label_Columns = New System.Windows.Forms.Label()
        Me.NumUD_Columns = New System.Windows.Forms.NumericUpDown()
        Me.Label_Rows = New System.Windows.Forms.Label()
        Me.NumUD_Rows = New System.Windows.Forms.NumericUpDown()
        Me.GBox_CalculationMethod = New System.Windows.Forms.GroupBox()
        Me.RBut_LookUpInTable = New System.Windows.Forms.RadioButton()
        Me.RBut_Precisely = New System.Windows.Forms.RadioButton()
        Me.RBut_Roughly = New System.Windows.Forms.RadioButton()
        Me.GBox_CalculationType = New System.Windows.Forms.GroupBox()
        Me.RBut_UnitWeight = New System.Windows.Forms.RadioButton()
        Me.CBox_DeductTopSurface = New System.Windows.Forms.CheckBox()
        Me.RBut_UnitArea = New System.Windows.Forms.RadioButton()
        Me.TabPage_ToDo = New System.Windows.Forms.TabPage()
        Me.TabControl_SteelStructureCalculationTool.SuspendLayout()
        Me.TabPage_Generate_Unit_Area_or_Weight.SuspendLayout()
        Me.GBox_Offset.SuspendLayout()
        CType(Me.NumUD_Columns, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumUD_Rows, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GBox_CalculationMethod.SuspendLayout()
        Me.GBox_CalculationType.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl_SteelStructureCalculationTool
        '
        Me.TabControl_SteelStructureCalculationTool.Controls.Add(Me.TabPage_Generate_Unit_Area_or_Weight)
        Me.TabControl_SteelStructureCalculationTool.Controls.Add(Me.TabPage_ToDo)
        Me.TabControl_SteelStructureCalculationTool.Location = New System.Drawing.Point(6, 6)
        Me.TabControl_SteelStructureCalculationTool.Name = "TabControl_SteelStructureCalculationTool"
        Me.TabControl_SteelStructureCalculationTool.SelectedIndex = 0
        Me.TabControl_SteelStructureCalculationTool.Size = New System.Drawing.Size(316, 244)
        Me.TabControl_SteelStructureCalculationTool.TabIndex = 0
        '
        'TabPage_Generate_Unit_Area_or_Weight
        '
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.Button_Generate)
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.GBox_Offset)
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.GBox_CalculationMethod)
        Me.TabPage_Generate_Unit_Area_or_Weight.Controls.Add(Me.GBox_CalculationType)
        Me.TabPage_Generate_Unit_Area_or_Weight.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_Generate_Unit_Area_or_Weight.Name = "TabPage_Generate_Unit_Area_or_Weight"
        Me.TabPage_Generate_Unit_Area_or_Weight.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_Generate_Unit_Area_or_Weight.Size = New System.Drawing.Size(308, 218)
        Me.TabPage_Generate_Unit_Area_or_Weight.TabIndex = 0
        Me.TabPage_Generate_Unit_Area_or_Weight.Text = "Generate Unit Area/Weight"
        Me.TabPage_Generate_Unit_Area_or_Weight.UseVisualStyleBackColor = True
        '
        'Button_Generate
        '
        Me.Button_Generate.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button_Generate.Location = New System.Drawing.Point(189, 155)
        Me.Button_Generate.Name = "Button_Generate"
        Me.Button_Generate.Size = New System.Drawing.Size(100, 50)
        Me.Button_Generate.TabIndex = 11
        Me.Button_Generate.Text = "&Generate"
        Me.Button_Generate.UseVisualStyleBackColor = True
        '
        'GBox_Offset
        '
        Me.GBox_Offset.Controls.Add(Me.CBox_Overwrite)
        Me.GBox_Offset.Controls.Add(Me.Label_Columns)
        Me.GBox_Offset.Controls.Add(Me.NumUD_Columns)
        Me.GBox_Offset.Controls.Add(Me.Label_Rows)
        Me.GBox_Offset.Controls.Add(Me.NumUD_Rows)
        Me.GBox_Offset.Location = New System.Drawing.Point(181, 6)
        Me.GBox_Offset.Name = "GBox_Offset"
        Me.GBox_Offset.Size = New System.Drawing.Size(120, 143)
        Me.GBox_Offset.TabIndex = 2
        Me.GBox_Offset.TabStop = False
        Me.GBox_Offset.Text = "Destination offset from source"
        '
        'CBox_Overwrite
        '
        Me.CBox_Overwrite.Location = New System.Drawing.Point(6, 92)
        Me.CBox_Overwrite.Name = "CBox_Overwrite"
        Me.CBox_Overwrite.Size = New System.Drawing.Size(104, 39)
        Me.CBox_Overwrite.TabIndex = 10
        Me.CBox_Overwrite.Text = "Overwrite &existing data"
        Me.CBox_Overwrite.UseVisualStyleBackColor = True
        '
        'Label_Columns
        '
        Me.Label_Columns.AutoSize = True
        Me.Label_Columns.Location = New System.Drawing.Point(6, 68)
        Me.Label_Columns.Name = "Label_Columns"
        Me.Label_Columns.Size = New System.Drawing.Size(47, 12)
        Me.Label_Columns.TabIndex = 8
        Me.Label_Columns.Text = "&Columns"
        '
        'NumUD_Columns
        '
        Me.NumUD_Columns.Location = New System.Drawing.Point(59, 65)
        Me.NumUD_Columns.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.NumUD_Columns.Name = "NumUD_Columns"
        Me.NumUD_Columns.Size = New System.Drawing.Size(40, 21)
        Me.NumUD_Columns.TabIndex = 9
        Me.NumUD_Columns.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.NumUD_Columns.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label_Rows
        '
        Me.Label_Rows.AutoSize = True
        Me.Label_Rows.Location = New System.Drawing.Point(6, 42)
        Me.Label_Rows.Name = "Label_Rows"
        Me.Label_Rows.Size = New System.Drawing.Size(29, 12)
        Me.Label_Rows.TabIndex = 6
        Me.Label_Rows.Text = "&Rows"
        '
        'NumUD_Rows
        '
        Me.NumUD_Rows.Location = New System.Drawing.Point(59, 38)
        Me.NumUD_Rows.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.NumUD_Rows.Name = "NumUD_Rows"
        Me.NumUD_Rows.Size = New System.Drawing.Size(40, 21)
        Me.NumUD_Rows.TabIndex = 7
        Me.NumUD_Rows.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GBox_CalculationMethod
        '
        Me.GBox_CalculationMethod.Controls.Add(Me.RBut_LookUpInTable)
        Me.GBox_CalculationMethod.Controls.Add(Me.RBut_Precisely)
        Me.GBox_CalculationMethod.Controls.Add(Me.RBut_Roughly)
        Me.GBox_CalculationMethod.Location = New System.Drawing.Point(6, 112)
        Me.GBox_CalculationMethod.Name = "GBox_CalculationMethod"
        Me.GBox_CalculationMethod.Size = New System.Drawing.Size(168, 100)
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
        Me.RBut_LookUpInTable.TabIndex = 5
        Me.RBut_LookUpInTable.Text = "&Look up in table"
        Me.RBut_LookUpInTable.UseVisualStyleBackColor = True
        '
        'RBut_Precisely
        '
        Me.RBut_Precisely.AutoSize = True
        Me.RBut_Precisely.Location = New System.Drawing.Point(7, 44)
        Me.RBut_Precisely.Name = "RBut_Precisely"
        Me.RBut_Precisely.Size = New System.Drawing.Size(77, 16)
        Me.RBut_Precisely.TabIndex = 4
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
        Me.RBut_Roughly.TabIndex = 3
        Me.RBut_Roughly.TabStop = True
        Me.RBut_Roughly.Text = "R&oughly"
        Me.RBut_Roughly.UseVisualStyleBackColor = True
        '
        'GBox_CalculationType
        '
        Me.GBox_CalculationType.Controls.Add(Me.RBut_UnitWeight)
        Me.GBox_CalculationType.Controls.Add(Me.CBox_DeductTopSurface)
        Me.GBox_CalculationType.Controls.Add(Me.RBut_UnitArea)
        Me.GBox_CalculationType.Location = New System.Drawing.Point(6, 6)
        Me.GBox_CalculationType.Name = "GBox_CalculationType"
        Me.GBox_CalculationType.Size = New System.Drawing.Size(168, 100)
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
        Me.RBut_UnitWeight.Text = "Unit &Weight"
        Me.RBut_UnitWeight.UseVisualStyleBackColor = True
        '
        'CBox_DeductTopSurface
        '
        Me.CBox_DeductTopSurface.AutoSize = True
        Me.CBox_DeductTopSurface.Location = New System.Drawing.Point(6, 42)
        Me.CBox_DeductTopSurface.Name = "CBox_DeductTopSurface"
        Me.CBox_DeductTopSurface.Size = New System.Drawing.Size(156, 16)
        Me.CBox_DeductTopSurface.TabIndex = 1
        Me.CBox_DeductTopSurface.Text = "&Deduct The Top Surface"
        Me.CBox_DeductTopSurface.UseVisualStyleBackColor = True
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
        'TabPage_ToDo
        '
        Me.TabPage_ToDo.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_ToDo.Name = "TabPage_ToDo"
        Me.TabPage_ToDo.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_ToDo.Size = New System.Drawing.Size(308, 218)
        Me.TabPage_ToDo.TabIndex = 1
        Me.TabPage_ToDo.Text = "ToDo"
        Me.TabPage_ToDo.UseVisualStyleBackColor = True
        '
        'Form_SSCT
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
        Me.ClientSize = New System.Drawing.Size(326, 255)
        Me.Controls.Add(Me.TabControl_SteelStructureCalculationTool)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.Name = "Form_SSCT"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SSCT - Ver1.2"
        Me.TabControl_SteelStructureCalculationTool.ResumeLayout(False)
        Me.TabPage_Generate_Unit_Area_or_Weight.ResumeLayout(False)
        Me.GBox_Offset.ResumeLayout(False)
        Me.GBox_Offset.PerformLayout()
        CType(Me.NumUD_Columns, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumUD_Rows, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GBox_CalculationMethod.ResumeLayout(False)
        Me.GBox_CalculationMethod.PerformLayout()
        Me.GBox_CalculationType.ResumeLayout(False)
        Me.GBox_CalculationType.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControl_SteelStructureCalculationTool As TabControl
    Friend WithEvents TabPage_Generate_Unit_Area_or_Weight As TabPage
    Friend WithEvents Button_Generate As Button
    Friend WithEvents GBox_Offset As GroupBox
    Friend WithEvents CBox_Overwrite As CheckBox
    Friend WithEvents Label_Columns As Label
    Friend WithEvents NumUD_Columns As NumericUpDown
    Friend WithEvents Label_Rows As Label
    Friend WithEvents NumUD_Rows As NumericUpDown
    Friend WithEvents GBox_CalculationMethod As GroupBox
    Friend WithEvents RBut_LookUpInTable As RadioButton
    Friend WithEvents RBut_Precisely As RadioButton
    Friend WithEvents RBut_Roughly As RadioButton
    Friend WithEvents GBox_CalculationType As GroupBox
    Friend WithEvents RBut_UnitWeight As RadioButton
    Friend WithEvents CBox_DeductTopSurface As CheckBox
    Friend WithEvents RBut_UnitArea As RadioButton
    Friend WithEvents TabPage_ToDo As TabPage
End Class
