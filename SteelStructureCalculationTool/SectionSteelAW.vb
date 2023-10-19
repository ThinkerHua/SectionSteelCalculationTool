Imports System.Runtime.InteropServices
Module SectionSteelAreaWeight
    Public Const _TESTFLAG = 0                  '测试代码控制符

    '面积、重量功能相关
    Public Const TYPE_AREA = 1                  '计算类型：面积
    Public Const TYPE_EXCLUDE_TOPSURFACE = 2    '计算类型：面积子项，扣除顶面
    Public Const TYPE_WEIGHT = 4                '计算类型：重量
    Public Const METHOD_ROUGHLY = 8             '计算模式：粗略
    Public Const METHOD_PRECISELY = 16          '计算模式：精细
    Public Const METHOD_LOOKUPINTABLE = 32      '计算模式：查表
    Public Const PI_STYLE = 64                  'PI的写法（置0为PI()，置1为3.14）

    Public AW_CTRLCODE As Integer = 0           '控制码
    Public AW_Offset_Rows As Integer = 0        '目标行偏移参数
    Public AW_Offset_Columns As Integer = 1     '目标列偏移参数
    Public AW_Overwrite As Integer = 0          '目标已有数据是否覆盖参数

    '加劲肋功能相关
    Public Const TRUNCATE = 1                   '截尾取整

    Public STIF_CTRLCODE As Integer = 0         '控制码
    Public Stif_Offset_Rows As Integer = 1      '目标行偏移参数
    Public Stif_Offset_Columns As Integer = 0   '目标列偏移参数
    Public Stif_Overwrite As Integer = 0        '目标已有数据是否覆盖参数

    Public Declare Function SectionSteel_Area_Weight Lib "SectionSteelAreaWeight.dll" (ByVal RawText As IntPtr, ByVal CtrlCode As UInteger) As IntPtr
    Public Declare Function Stiffener_Specification Lib "SectionSteelAreaWeight.dll" (ByVal RawText As IntPtr, ByVal CtrlCode As UInteger) As IntPtr
    Public Declare Sub free_dallocstr Lib "SectionSteelAreaWeight.dll" (ByVal str As IntPtr)

    Public Sub Generate(func_option As Integer)
        'func_option = 0    SectionSteel_Area_Weight
        'func_option = 1    Stiffener_Specification

        Dim xlApp As Object = Nothing       'Excel对象
        Dim xlWorkbook As Object = Nothing  '工作薄对象
        'Dim xlSheet As Object = Nothing     '工作表对象
        Dim xlRange As Object = Nothing     '单元格区域
        Dim newRange As Object = Nothing    '数据筛选后的新单元格区域
        Dim xlCell As Object = Nothing      '单元格

        Dim offsetRow As Integer = 0        '目标偏移行数
        Dim offsetCol As Integer = 0        '目标偏移列数
        Dim overwrite As Integer = 0        '目标是否覆写
        Dim targetCell As Object = Nothing  '目标单元格

        Dim sRawText As IntPtr = Nothing
        Dim sResault As IntPtr = Nothing

        '测试用变量
        Dim starttime As Date
        Dim endtime As Date

        '目前只提供0或1两个选项，先校验以减少后面的判断及操作
        Select Case func_option
            Case 0
                offsetRow = AW_Offset_Rows
                offsetCol = AW_Offset_Columns
                overwrite = AW_Overwrite
            Case 1
                offsetRow = Stif_Offset_Rows
                offsetCol = Stif_Offset_Columns
                overwrite = Stif_Overwrite
            Case Else
                Exit Sub
        End Select

        '获取Excel程序、工作薄、工作表、选定区域
        On Error Resume Next
        xlApp = GetObject(, "Excel.Application")
        If Err.Number <> 0 Then
            Err.Clear()
            MsgBox("Please open an Excel application first!", vbOKOnly + vbExclamation, "Waring")
            GoTo clean
        End If
        xlWorkbook = xlApp.ActiveWorkbook
        If xlWorkbook Is Nothing Then
            MsgBox("Please open an Workbook first!", vbOKOnly + vbExclamation, "Waring")
            GoTo clean
        End If
        xlRange = xlApp.Selection
        '只处理可见的、常量且数据为文本的单元格
        '不能用xlRange.Count = xlRange.Cells.SpecialCells(12).SpecialCells(2,2).Count来判断
        If xlRange.count > 1 Then '当xlRange.Count = 1时，SpecialCells会在整个工作表中进行搜索
            newRange = xlRange.Cells.SpecialCells(12).SpecialCells(2, 2)
            If newRange Is Nothing Then GoTo clean
        Else
            newRange = xlRange
        End If
        On Error GoTo 0

        '测试用
        If _TESTFLAG Then MsgBox("AW_CTRLCODE = " & AW_CTRLCODE & vbCrLf &
                                 "STIF_CTRLCODE = " & STIF_CTRLCODE, vbOKOnly + vbExclamation, "Testing")
        If _TESTFLAG Then starttime = System.DateTime.Now

        '正式开始
        xlApp.ScreenUpdating = False
        For Each xlCell In newRange
            targetCell = xlCell.Offset(offsetRow, offsetCol)
            '不覆写且目标不为空时直接跳过
            If (overwrite = 0) And (targetCell.value IsNot Nothing) Then Continue For

            sRawText = Marshal.StringToHGlobalAnsi(xlCell.Value)
            'Debug.WriteLine("before:" & Marshal.PtrToStringAnsi(sRawText))
            Select Case func_option
                Case 0
                    sResault = SectionSteel_Area_Weight(sRawText, AW_CTRLCODE)
                    'Debug.WriteLine(" after:" & Marshal.PtrToStringAnsi(sRawText))
                Case 1
                    sResault = Stiffener_Specification(sRawText, STIF_CTRLCODE)
                    'Debug.WriteLine(" after:" & Marshal.PtrToStringAnsi(sRawText))
            End Select

            '输出到Excel
            If sResault <> Nothing Then
                Select Case func_option
                    Case 0
                        targetCell.Value = "=" & Marshal.PtrToStringAnsi(sResault)
                    Case 1
                        targetCell.value = Marshal.PtrToStringAnsi(sResault)
                End Select
                'Debug.WriteLine("before:" & Marshal.PtrToStringAnsi(sResault))
                free_dallocstr(sResault)
                'Debug.WriteLine(" after:" & Marshal.PtrToStringAnsi(sResault))
                'Debug.WriteLine("")
                sResault = Nothing
            Else
                targetCell.Value = Nothing
            End If
        Next
        xlApp.screenupdating = True

        '测试用
        If _TESTFLAG Then endtime = System.DateTime.Now : MsgBox("执行持续时间：" & (endtime - starttime).ToString)

clean:
        '释放对象内存
        targetCell = Nothing : xlCell = Nothing : newRange = Nothing : xlRange = Nothing : xlWorkbook = Nothing : xlApp = Nothing
        'GC.Collect()
    End Sub
End Module
