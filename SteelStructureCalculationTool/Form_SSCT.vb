'Imports Microsoft.Office.Interop.Excel
'Imports System.ComponentModel   '窗体关闭事件需引用的命名空间

Public Class Form_SSCT
    Private Sub RBut_UnitArea_Click(sender As Object, e As EventArgs) Handles RBut_UnitArea.Click
        CBox_DeductTopSurface.Enabled = True
        Set_Calculation_Type()
    End Sub

    Private Sub CBox_DeductTopSurface_Click(sender As Object, e As EventArgs) Handles CBox_DeductTopSurface.Click
        Set_Calculation_Type()
    End Sub

    Private Sub RBut_UnitWeight_Click(sender As Object, e As EventArgs) Handles RBut_UnitWeight.Click
        CBox_DeductTopSurface.Enabled = False
        Set_Calculation_Type()
    End Sub

    Private Sub RBut_Roughly_Click(sender As Object, e As EventArgs) Handles RBut_Roughly.Click
        Set_Calculation_Method()
    End Sub

    Private Sub RBut_Precisely_Click(sender As Object, e As EventArgs) Handles RBut_Precisely.Click
        Set_Calculation_Method()
    End Sub

    Private Sub RBut_LookUpInTable_Click(sender As Object, e As EventArgs) Handles RBut_LookUpInTable.Click
        Set_Calculation_Method()
    End Sub

    Private Sub NumUD_Rows_ValueChanged(sender As Object, e As EventArgs) Handles NumUD_Rows.ValueChanged
        Set_Offset_Row()
    End Sub

    Private Sub NumUD_Columns_ValueChanged(sender As Object, e As EventArgs) Handles NumUD_Columns.ValueChanged
        Set_Offset_Column()
    End Sub

    Private Sub CBox_Overwrite_Click(sender As Object, e As EventArgs) Handles CBox_Overwrite.Click
        Set_Overwrite()
    End Sub
    Private Sub Button_Generate_Click(sender As Object, e As EventArgs) Handles Button_Generate.Click   '执行按钮
        Generate()
        If _TESTFLAG Then _Test()
    End Sub
    Private Sub Form_SSCT_Load(sender As Object, e As EventArgs) Handles Me.Load                        '初始化
        Me.Text = Application.ProductName & " - Ver" & Application.ProductVersion
        Set_Public_Arguments()
        '程序开始运行就直接打开数据文件，不再等第一次查表操作才打开，加快第一次查表时的执行速度
        '2022-05-05 由于GetObject函数的特性，数据文件必须晚于工作文件打开
        'PrepareForReadingData("", "")
    End Sub

    'Private Sub Form_SSCT_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing            '关闭前解除占用、释放内存
    '    'Release()
    '    'CloseDataFile()
    'End Sub
    Private Sub Set_Public_Arguments()                                                                  '设置公用参数
        Set_Calculation_Type()
        Set_Calculation_Method()
        Set_Offset_Row()
        Set_Offset_Column()
        Set_Overwrite()
    End Sub
    Private Sub Set_Calculation_Type()                                                                  '设置公用参数：计算类型
        If RBut_UnitArea.Checked = True Then
            If CBox_DeductTopSurface.Checked = True Then
                Calculation_Type = TYPE_AREA + TYPE_DEDUCTTOPSURFACE
            Else
                Calculation_Type = TYPE_AREA
            End If
        ElseIf RBut_UnitWeight.Checked = True Then
            Calculation_Type = TYPE_WEIGHT
        Else

        End If
    End Sub
    Private Sub Set_Calculation_Method()                                                                '设置公用参数：计算模式
        If RBut_Roughly.Checked = True Then
            Calculation_Method = METHOD_ROUGHLY
        End If
        If RBut_Precisely.Checked = True Then
            Calculation_Method = METHOD_PRECISELY
        End If
        If RBut_LookUpInTable.Checked = True Then
            Calculation_Method = METHOD_LOOKUPINTABLE
        End If
    End Sub
    Private Sub Set_Offset_Row()                                                                        '设置公用参数：目标行偏移
        Offset_Rows = NumUD_Rows.Value
    End Sub
    Private Sub Set_Offset_Column()                                                                     '设置公用参数：目标列偏移
        Offset_Columns = NumUD_Columns.Value
    End Sub
    Private Sub Set_Overwrite()                                                                         '设置公用参数：目标已有数据是否覆盖
        If CBox_Overwrite.Checked = True Then
            Overwrite = 1
        Else
            Overwrite = 0
        End If
    End Sub
End Class
