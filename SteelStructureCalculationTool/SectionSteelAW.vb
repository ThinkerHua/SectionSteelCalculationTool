Imports System.Runtime.InteropServices
Module SectionSteelAW
    Public Const _TESTFLAG = 0                  '测试代码控制符

    Public Const TYPE_AREA = 1                  '计算类型：面积
    Public Const TYPE_EXCLUDE_TOPSURFACE = 2    '计算类型：面积子项，扣除顶面
    Public Const TYPE_WEIGHT = 4                '计算类型：重量
    Public Const METHOD_ROUGHLY = 8             '计算模式：粗略
    Public Const METHOD_PRECISELY = 16          '计算模式：精细
    Public Const METHOD_LOOKUPINTABLE = 32      '计算模式：查表
    Public Const PI_STYLE = 64                  'PI的写法（置0为PI()，置1为3.14）

    Public CTRLCODE As Integer = 0              '控制码
    Public Offset_Rows As Integer = 0           '目标行偏移参数
    Public Offset_Columns As Integer = 1        '目标列偏移参数
    Public Overwrite As Integer = 0             '目标已有数据是否覆盖参数

    Public Declare Function SectionSteelAW Lib "SectionSteelAW.dll" (ByVal RawText As IntPtr, ByVal CtrlCode As UInteger) As IntPtr
    Public Declare Sub free_dallocstr Lib "SectionSteelAW.dll" (ByVal str As IntPtr)

    Public Sub Generate()
        Dim xlApp As Object = Nothing       'Excel对象
        Dim xlWorkbook As Object = Nothing  '工作薄对象
        'Dim xlSheet As Object = Nothing     '工作表对象
        Dim xlRange As Object = Nothing     '单元格区域
        Dim xlCell As Object = Nothing      '单元格

        Dim sRawText As IntPtr = Nothing
        Dim sResault As IntPtr = Nothing

        Dim starttime As Date
        Dim endtime As Date

        '获取Excel程序、工作薄、工作表、选定区域
        On Error Resume Next
        xlApp = GetObject(, "Excel.Application")
        If Err.Number <> 0 Then
            Err.Clear()
            MsgBox("Please open an Excel application first!", vbOKOnly + vbExclamation, "Waring")
            Exit Sub
        End If
        xlWorkbook = xlApp.ActiveWorkbook
        If xlWorkbook Is Nothing Then
            MsgBox("Please open an Workbook first!", vbOKOnly + vbExclamation, "Waring")
            Exit Sub
        End If
        xlRange = xlApp.Selection
        On Error GoTo 0

        If _TESTFLAG Then starttime = System.DateTime.Now
        xlApp.screenupdating = False
        For Each xlCell In xlRange
            '不覆写且目标不为空时直接跳过
            If (Overwrite = 0) And (xlCell.offset(Offset_Rows, Offset_Columns).value IsNot Nothing) Then Continue For

            sRawText = Marshal.StringToHGlobalAnsi(xlCell.Value)
            'Debug.WriteLine("before:" & Marshal.PtrToStringAnsi(sRawText))
            sResault = SectionSteelAW(sRawText, CTRLCODE)
            'Debug.WriteLine(" after:" & Marshal.PtrToStringAnsi(sRawText))
            '输出到Excel
            If sResault <> Nothing Then
                xlCell.offset(Offset_Rows, Offset_Columns).Value = "=" & Marshal.PtrToStringAnsi(sResault)
                'Debug.WriteLine("before:" & Marshal.PtrToStringAnsi(sResault))
                free_dallocstr(sResault)
                'Debug.WriteLine(" after:" & Marshal.PtrToStringAnsi(sResault))
                'Debug.WriteLine("")
                sResault = Nothing
            Else
                xlCell.offset(Offset_Rows, Offset_Columns).Value = Nothing
            End If
        Next
        xlApp.screenupdating = True
        If _TESTFLAG Then endtime = System.DateTime.Now : MsgBox("执行持续时间：" & (endtime - starttime).ToString)

        '释放对象内存
        xlCell = Nothing : xlRange = Nothing : xlWorkbook = Nothing : xlApp = Nothing
    End Sub
End Module
