Public Class Form_SSCT
    Private Sub RBut_UnitArea_Click(sender As Object, e As EventArgs) Handles RBut_UnitArea.Click
        CBox_ExcludeTopSurface.Enabled = True
        Set_Public_Arguments()
    End Sub

    Private Sub CBox_DeductTopSurface_Click(sender As Object, e As EventArgs) Handles CBox_ExcludeTopSurface.Click
        Set_Public_Arguments()
    End Sub

    Private Sub RBut_UnitWeight_Click(sender As Object, e As EventArgs) Handles RBut_UnitWeight.Click
        CBox_ExcludeTopSurface.Enabled = False
        Set_Public_Arguments()
    End Sub

    Private Sub RBut_Roughly_Click(sender As Object, e As EventArgs) Handles RBut_Roughly.Click
        Set_Public_Arguments()
    End Sub

    Private Sub RBut_Precisely_Click(sender As Object, e As EventArgs) Handles RBut_Precisely.Click
        Set_Public_Arguments()
    End Sub

    Private Sub RBut_LookUpInTable_Click(sender As Object, e As EventArgs) Handles RBut_LookUpInTable.Click
        Set_Public_Arguments()
    End Sub

    Private Sub NumUD_Rows_ValueChanged(sender As Object, e As EventArgs) Handles NumUD_Rows.ValueChanged
        Set_Public_Arguments()
    End Sub

    Private Sub NumUD_Columns_ValueChanged(sender As Object, e As EventArgs) Handles NumUD_Columns.ValueChanged
        Set_Public_Arguments()
    End Sub

    Private Sub CBox_Overwrite_Click(sender As Object, e As EventArgs) Handles CBox_Overwrite.Click
        Set_Public_Arguments()
    End Sub

    Private Sub RBut_PI_FUN_Click(sender As Object, e As EventArgs) Handles RBut_PI_FUN.Click
        Set_Public_Arguments()
    End Sub

    Private Sub RBut_PI_NUM_Click(sender As Object, e As EventArgs) Handles RBut_PI_NUM.Click
        Set_Public_Arguments()
    End Sub
    Private Sub Button_Generate_Click(sender As Object, e As EventArgs) Handles Button_Generate.Click
        Generate()
    End Sub
    Private Sub Form_SSCT_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Text = Application.ProductName & " - Ver" & Application.ProductVersion
        Set_Public_Arguments()
        Me.ActiveControl = Button_Generate
    End Sub
    Private Sub Set_Public_Arguments()
        If RBut_UnitArea.Checked Then
            CTRLCODE = CTRLCODE Or TYPE_AREA
        Else
            CTRLCODE = CTRLCODE And Not TYPE_AREA
        End If
        If CBox_ExcludeTopSurface.Checked Then
            CTRLCODE = CTRLCODE Or TYPE_EXCLUDE_TOPSURFACE
        Else
            CTRLCODE = CTRLCODE And Not TYPE_EXCLUDE_TOPSURFACE
        End If
        If RBut_UnitWeight.Checked Then
            CTRLCODE = CTRLCODE Or TYPE_WEIGHT
        Else
            CTRLCODE = CTRLCODE And Not TYPE_WEIGHT
        End If
        If RBut_Roughly.Checked Then
            CTRLCODE = CTRLCODE Or METHOD_ROUGHLY
        Else
            CTRLCODE = CTRLCODE And Not METHOD_ROUGHLY
        End If
        If RBut_Precisely.Checked Then
            CTRLCODE = CTRLCODE Or METHOD_PRECISELY
        Else
            CTRLCODE = CTRLCODE And Not METHOD_PRECISELY
        End If
        If RBut_LookUpInTable.Checked Then
            CTRLCODE = CTRLCODE Or METHOD_LOOKUPINTABLE
        Else
            CTRLCODE = CTRLCODE And Not METHOD_LOOKUPINTABLE
        End If
        If RBut_PI_FUN.Checked Then
            CTRLCODE = CTRLCODE And Not PI_STYLE
        Else
            CTRLCODE = CTRLCODE Or PI_STYLE
        End If
        Offset_Rows = NumUD_Rows.Value
        Offset_Columns = NumUD_Columns.Value
        Overwrite = CBox_Overwrite.Checked
    End Sub

    Private Sub TabControl_SteelStructureCalculationTool_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl_SteelStructureCalculationTool.SelectedIndexChanged
        If TabControl_SteelStructureCalculationTool.SelectedIndex = 0 Then
            Button_Generate.Focus()
        End If
    End Sub
End Class
