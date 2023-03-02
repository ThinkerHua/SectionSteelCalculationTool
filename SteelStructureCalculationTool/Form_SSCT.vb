Public Class Form_SSCT
    Private Sub Form_SSCT_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Text = Application.ProductName & " - Ver" & Application.ProductVersion
        Me.ActiveControl = Button_AW_Generate
    End Sub
    Private Sub TabControl_SteelStructureCalculationTool_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl_SteelStructureCalculationTool.SelectedIndexChanged
        If TabControl_SteelStructureCalculationTool.SelectedIndex = 0 Then
            Button_AW_Generate.Focus()
        ElseIf TabControl_SteelStructureCalculationTool.SelectedIndex = 1 Then
            Button_Stif_Generate.Focus()
        End If
    End Sub
    Private Sub RBut_UnitArea_Click(sender As Object, e As EventArgs) Handles RBut_UnitArea.Click
        CBox_ExcludeTopSurface.Enabled = True
    End Sub

    Private Sub RBut_UnitWeight_Click(sender As Object, e As EventArgs) Handles RBut_UnitWeight.Click
        CBox_ExcludeTopSurface.Enabled = False
    End Sub
    Private Sub Button_AW_Generate_Click(sender As Object, e As EventArgs) Handles Button_AW_Generate.Click
        Gather_Information()
        Generate(0)
    End Sub
    Private Sub Button_Stif_Generate_Click(sender As Object, e As EventArgs) Handles Button_Stif_Generate.Click
        Gather_Information()
        Generate(1)
    End Sub
    Private Sub Gather_Information()
        '当 Visual Basic 将数值数据类型值转换为 Boolean 时，0 变为 False，所有其他值变为 True
        '当 Visual Basic 将 Boolean 值转换为数值类型时，False 变为 0，True 变为 -1
        AW_CTRLCODE = -RBut_UnitArea.Checked * TYPE_AREA _
                        - CBox_ExcludeTopSurface.Checked * TYPE_EXCLUDE_TOPSURFACE _
                        - RBut_UnitWeight.Checked * TYPE_WEIGHT _
                        - RBut_Roughly.Checked * METHOD_ROUGHLY _
                        - RBut_Precisely.Checked * METHOD_PRECISELY _
                        - RBut_LookUpInTable.Checked * METHOD_LOOKUPINTABLE _
                        - RBut_PI_NUM.Checked * PI_STYLE
        AW_Offset_Rows = NumUD_AW_Rows.Value
        AW_Offset_Columns = NumUD_AW_Columns.Value
        AW_Overwrite = CBox_AW_Overwrite.Checked

        STIF_CTRLCODE = -CInt(CBox_Trunc.Checked) * TRUNCATE
        Stif_Offset_Rows = NumUD_Stif_Rows.Value
        Stif_Offset_Columns = NumUD_Stif_Columns.Value
        Stif_Overwrite = CBox_Stif_Overwrite.Checked
    End Sub
End Class
