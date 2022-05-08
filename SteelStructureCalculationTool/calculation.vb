Module calculation
    Public Const _TESTFLAG = 0              '测试代码控制符

    Public Const TYPE_AREA = 1              '计算类型：面积
    Public Const TYPE_DEDUCTTOPSURFACE = 2  '计算类型：面积子项，扣除顶面
    Public Const TYPE_WEIGHT = 4            '计算类型：重量
    Public Const METHOD_ROUGHLY = 1         '计算模式：粗略
    Public Const METHOD_PRECISELY = 2       '计算模式：精细
    Public Const METHOD_LOOKUPINTABLE = 4   '计算模式：查表

    Public Calculation_Type As Integer      '计算类型参数
    Public Calculation_Method As Integer    '计算模式参数
    Public Offset_Rows As Integer           '目标行偏移参数
    Public Offset_Columns As Integer        '目标列偏移参数
    Public Overwrite As Integer             '目标已有数据是否覆盖参数

    '工作对象
    Private xlApp As Object                 'Excel对象
    Private xlWorkBook As Object            '工作薄对象
    'Private xlSheet As Object               '工作表对象
    Private xlRange As Object               '单元格区域
    Private xlCell As Object                '单元格

    ''源数据对象
    'Public DataApp As Object
    'Public DataWorkBook As Object
    'Public DataSheet As Object
    'Public DataRange As Object
    'Public DataFunc As Object
    'ReadOnly Property DataPath As String = Application.StartupPath & "\SteelData.xls"

    '受支持的型材类型列表
    Private ReadOnly SpeTypeArr() As String = {"H", "HT", "HI", "T", "J", "D", "I", "[", "[]", "2[", "L", "2L", "C", "2C", "Z", "PL", "PLT", "PLD"}
    ''对应型材类型所需参数数量
    'Private ReadOnly NumOfData() As Integer = {4, 4, 8, 4, 4, 2, 4, 4, 4, 4, 3, 3, 4, 4, 4, 3, 3, 1}
    'Public Function PrepareForReadingData(ByVal SheetName As String, ByVal Range As String) As Integer                  '为查表做准备
    '    On Error Resume Next
    '    If DataApp Is Nothing Then
    '        DataApp = CreateObject("Excel.Application")
    '        If Err.Number Then
    '            Err.Clear()
    '            MsgBox("There is no ""Excel Application"" exists", vbOKOnly + vbExclamation, "Waring")
    '            Return 1
    '        End If
    '    End If
    '    If DataWorkBook Is Nothing Then
    '        DataWorkBook = DataApp.workbooks.open(DataPath, vbReadOnly)
    '        If Err.Number Then
    '            Err.Clear()
    '            MsgBox("The data file is not found!" & vbCrLf &
    '                    "Please make sure that there is an ""SteelData.xls"" file exists in this program directory.",
    '                    vbOKOnly + vbExclamation, "Waring")
    '            Return 1
    '        End If
    '    End If
    '    If SheetName <> "" Then
    '        DataSheet = DataWorkBook.worksheets(SheetName)
    '        If Err.Number Then
    '            Err.Clear()
    '            MsgBox("The sheet """ & SheetName & """ is not found!", vbOKOnly + vbExclamation, "Waring")
    '            Return 1
    '        End If
    '    End If
    '    If Range <> "" Then
    '        DataRange = DataSheet.range(Range)
    '        If Err.Number Then
    '            Err.Clear()
    '            MsgBox("Wrong range!", vbOKOnly + vbExclamation, "Waring")
    '            Return 1
    '        End If
    '    End If
    '    DataFunc = DataApp.worksheetfunction
    '    Return 0
    'End Function
    'Public Sub Release()                                                                                                '查表完成释放空间
    '    DataFunc = Nothing : DataRange = Nothing : DataSheet = Nothing
    'End Sub
    'Public Sub CloseDataFile()                                                                                          '关闭数据文件
    '    If DataWorkBook IsNot Nothing Then DataWorkBook.close() : DataWorkBook = Nothing
    '    If DataApp IsNot Nothing Then DataApp.quit() : DataApp = Nothing
    '    '经测试，WorkBooks.close()和Application.quit()后，对象并没有置空，仍然存在，所以添加了赋空值操作
    '    'If _TESTFLAG Then
    '    '    If DataWorkBook Is Nothing Then
    '    '        MsgBox("Data Workbook has been closed.")
    '    '    Else
    '    '        MsgBox("Data Workbook has not been closed yet.")
    '    '    End If
    '    '    If DataApp Is Nothing Then
    '    '        MsgBox("Excel Application has been closed.")
    '    '    Else
    '    '        MsgBox("Excel Application has not been closed yet.")
    '    '    End If
    '    'End If
    'End Sub

    Public Function _Test() As Double                                                                                '测试用
        'Dim t As New TestClass
        Dim starttime, endtime As System.DateTime
        'Return MsgBox("Calculation Type =" & Calculation_Type & vbCrLf &
        '                "Method = " & Calculation_Method & vbCrLf &
        '                "Offset_Rows = " & Offset_Rows & vbCrLf &
        '                "Offset_Columns = " & Offset_Columns & vbCrLf &
        '                "Overwrite = " & Overwrite & vbCrLf)
        'Return MsgBox(t.resault(1))
        'Dim d As Double = Nothing
        'If String.Compare(Str(d), "0") Then MsgBox("Equals")
        If xlWorkBook IsNot Nothing Then
            starttime = System.DateTime.Now
            xlApp.screenupdating = False
            xlRange = xlApp.range("$O2:$O447")
            For Each xlCell In xlRange
                xlCell.value = "test"
            Next
            xlApp.screenupdating = True
            endtime = System.DateTime.Now
            MsgBox("测试持续时间：" & (endtime - starttime).ToString)
        End If
        Return 0
    End Function
    Public Sub Generate()                                                                                           '执行主体函数
        Dim Specification_Text As String
        Dim Specification_Type As String
        'Dim Specification_Data() As Double
        Dim sResault As String
        Dim Profiles As Object
        Dim starttime, endtime As System.DateTime

        '获取Excel程序、工作薄、工作表、选定区域
        On Error Resume Next
        xlApp = GetObject(, "Excel.Application")
        '2022-05-05
        'If _TESTFLAG Then If xlApp.Equals(DataApp) Then MsgBox("所有Excel.Application都是同一个对象吗？")
        '答案是GetObject(, "Excel.Application")获取的对象始终是第一个打开的那个
        If Err.Number <> 0 Then
            Err.Clear()
            MsgBox("Please open an Excel application first!", vbOKOnly + vbExclamation, "Waring")
            Exit Sub
        End If
        xlWorkBook = xlApp.activeworkbook
        If xlWorkBook Is Nothing Then
            MsgBox("Please open an Workbook first!", vbOKOnly + vbExclamation, "Waring")
            Exit Sub
        End If
        'xlSheet = xlApp.activesheet
        xlRange = xlApp.selection
        On Error GoTo 0

        If _TESTFLAG Then starttime = System.DateTime.Now
        xlApp.screenupdating = False
        For Each xlCell In xlRange
            '不覆写且目标不为空时直接跳过
            If (Overwrite = 0) And (xlCell.offset(Offset_Rows, Offset_Columns).value IsNot Nothing) Then Continue For
            '获取计算所需数据参数
            Specification_Text = Get_Specification_Text(xlCell)
            If Specification_Text = "" Then '单元格内容为空，直接跳过
                Continue For
            Else
                Specification_Type = Get_Specification_Type(Specification_Text)
                Select Case Specification_Type
                    Case "H"
                        Profiles = New Profiles_H
                    Case "HT"
                        Profiles = New Profiles_HT
                    Case "HI"
                        Profiles = New Profiles_HI
                    Case "T"
                        Profiles = New Profiles_T
                    Case "J"
                        Profiles = New Profiles_Rect
                    Case "D"
                        Profiles = New Profiles_Cir
                    Case "I"
                        Profiles = New Profiles_I
                    Case "["
                        Profiles = New Profiles_Chan
                    Case "[]"
                        Profiles = New Profiles_Chan_MtM
                    Case "2["
                        Profiles = New Profiles_Chan_BtB
                    Case "L"
                        Profiles = New Profiles_L
                    Case "2L"
                        Profiles = New Profiles_2L
                    Case "C"
                        Profiles = New Profiles_C
                    Case "2C"
                        Profiles = New Profiles_2C
                    Case "Z"
                        Profiles = New Profiles_Z
                    Case "PL"
                        Profiles = New Profiles_PL
                    Case "PLT"
                        Profiles = New Profiles_PLT
                    Case "PLD"
                        Profiles = New Profiles_PLD
                    Case Else
                        '不受支持的型材规格，直接跳过
                        Continue For
                End Select
                'Specification_Data = Get_Specification_Data(Specification_Type, Specification_Text)
                'If Specification_Data Is Nothing Then '型材参数不符合预设规则，直接跳过
                '    Continue For
                'End If
            End If

            '执行计算
            '----------2022-04-26----------
            'If Calculation_Type And TYPE_AREA Then
            '    sResault = Get_Area(Specification_Type, Specification_Data)
            'ElseIf Calculation_Type And TYPE_WEIGHT Then
            '    sResault = Get_Weight(Specification_Type, Specification_Data)
            'Else
            '    MsgBox("Error: Initialization has not yet completed.", vbOKOnly + vbCritical, "Waring")
            '    Exit Sub
            'End If
            '------------------------------

            Profiles.GetData(Specification_Text, Specification_Type)
            sResault = Profiles.Get_Resault(Calculation_Type, Calculation_Method)
            '输出到Excel
            If sResault <> "" Then
                xlCell.offset(Offset_Rows, Offset_Columns).value = "=" & sResault
            Else
                xlCell.offset(Offset_Rows, Offset_Columns).value = Nothing
            End If
        Next
        xlApp.screenupdating = True
        If _TESTFLAG Then endtime = System.DateTime.Now : MsgBox("执行持续时间：" & (endtime - starttime).ToString)

        '释放对象内存
        xlCell = Nothing : xlRange = Nothing : xlWorkBook = Nothing : xlApp = Nothing
    End Sub
    Private Function Get_Specification_Text(ByVal xlCell As Object) As String                                       '获取截面规格文本（单元格内容）
        Dim sText As String
        sText = xlCell.value
        If sText = "" Then Return ""
        sText = Text_Formatting(sText)
        Return sText
    End Function
    Private Function Text_Formatting(ByVal sText As String) As String                                               '文本格式化
        sText = sText.Trim
        sText = sText.Replace(" ", "")
        sText = sText.Replace("(", "")
        sText = sText.Replace(")", "")
        sText = sText.Replace("（", "")
        sText = sText.Replace("）", "")
        sText = sText.Replace("~", "～")
        sText = sText.Replace("*", "×")
        sText = sText.Replace("＊", "×")
        sText = sText.Replace("x", "×")
        sText = sText.Replace("X", "×")
        sText = sText.Replace("—", "-")
        sText = sText.Replace("HW", "H")
        sText = sText.Replace("HM", "H")
        sText = sText.Replace("HN", "H")
        'sText = sText.Replace("HT", "H")
        sText = sText.Replace("TW", "T")
        sText = sText.Replace("TM", "T")
        sText = sText.Replace("TN", "T")
        sText = sText.Replace("工", "I")
        sText = sText.Replace("F", "J")
        sText = sText.Replace("][", "2[")

        Return sText
    End Function
    Private Function Get_Specification_Type(ByVal text As String) As String                                         '获取截面规格类型
        Dim _SpeTypeArr() As String
        Dim i, iLen As Integer

        '创建受支持的型材类型列表的副本并重新排序，原始副本适用于人类阅读编辑，排序后的副本适用于程序查找使用
        iLen = SpeTypeArr.Length
        ReDim _SpeTypeArr(iLen - 1)
        For i = 0 To iLen - 1
            _SpeTypeArr(i) = SpeTypeArr(i)
        Next
        QSort(_SpeTypeArr, 0, iLen - 1)

        '查找受支持类型是否在文本开头出现
        For i = 0 To iLen - 1
            If text.StartsWith(_SpeTypeArr(i)) Then Return _SpeTypeArr(i)
        Next

        '不受支持的型材类型，返回空字符串
        Return ""
    End Function
    Private Sub QSort(ByRef sArray As String(), ByVal Left As Integer, ByVal Right As Integer)                      '数组排序
        Dim i, Mid, Last As Integer

        If Left >= Right Then Exit Sub
        Mid = (Left + Right) \ 2
        Swap(sArray, Left, Mid)
        Last = Left
        For i = Left + 1 To Right
            If _StrCompare(sArray(i), sArray(Left)) > 0 Then
                Last += 1
                Swap(sArray, i, Last)
            End If
        Next
        Swap(sArray, Left, Last)
        QSort(sArray, Left, Last - 1)
        QSort(sArray, Last + 1, Right)

    End Sub
    Private Function _StrCompare(ByVal s1 As String, ByVal s2 As String) As Integer                                   '字符串大小比较
        Dim i, len1, len2 As Integer
        len1 = s1.Length : len2 = s2.Length
        If len1 > len2 Then
            Return 1
        ElseIf len1 < len2 Then
            Return -1
        ElseIf len1 <> 0 Then
            For i = 0 To len1 - 1
                If s1.Chars(i) > s2.Chars(i) Then
                    Return 1
                ElseIf s1.Chars(i) < s2.Chars(i) Then
                    Return -1
                End If
            Next
        End If

        Return 0
    End Function
    Private Sub Swap(ByRef sArray As String(), ByVal Index1 As Integer, ByVal Index2 As Integer)                    '交换数组元素
        Dim sTemp As String
        sTemp = sArray(Index1)
        sArray(Index1) = sArray(Index2)
        sArray(Index2) = sTemp
    End Sub

    '----------2022-04-26----------
    'Private Function Get_Specification_Data(ByVal type As String, ByVal text As String) As Double()                 '获取截面规格数据
    '    Dim Data(), valStack As Double
    '    Dim i, j, nItem, iVariable As Integer
    '    Dim c As Char, str As String

    '    '确定所需获取数据数量
    '    For i = 0 To SpeTypeArr.Length - 1
    '        If type.Equals(SpeTypeArr(i)) Then Exit For
    '    Next
    '    If i = SpeTypeArr.Length Then Return Nothing
    '    nItem = NumOfData(i)
    '    ReDim Data(nItem - 1)

    '    '支持的书写形式
    '    'H400×200             H244×175×7×11     H300～500×250×10×14  H400×300～200×8×10   H400×300/200×8×10/12
    '    'HI600×300×14×18    T400×200×8×12
    '    'J500×250×10         J500×250×10×12    J300～500×250×10×12  J500×250～300×10×12  F80×4   F80×80×4×4
    '    'D32×2.5              I20a                 [16a                    []12.6                  2[14     ][14
    '    'L50                   L100×8              L100×63×6             2L75×5
    '    'C160×60×20×2.2     2C160×60×20×2.2   Z160×60×20×2.0
    '    'PL300×300×20        PL300×300×20-PLT150×150×20               PLT150×150×20         PLD300×20
    '    i = type.Length
    '    If type = "I" Or type = "[" Or type = "[]" Or type = "2[" Or type = "][" Then
    '        '工字钢、槽钢

    '    Else
    '        While i < text.Length
    '            c = text.Chars(i)
    '            If c = "×" Then
    '                If iVariable Then
    '                    j += 1 : Data(j) = (Val(str) + valStack) / 2 : iVariable = 0
    '                Else
    '                    j += 1 : Data(j) = Val(str)
    '                End If
    '            ElseIf c = "～" Then
    '                iVariable = 1 : valStack = Val(str)
    '            ElseIf c = "+" Then
    '                ReDim Data(nItem)
    '                j += 1 : Data(j) = Val(str)
    '            ElseIf c = "-" Then
    '                '异形板件

    '            Else
    '                str &= c
    '            End If
    '            i += 1
    '        End While
    '        '对简写形式进行扩充
    '        If j < nItem Then

    '        End If
    '    End If

    '    Return Data
    'End Function
    '------------------------------
    Public Function StrAverage(ByVal str As String, ByVal c As Char) As Double
        If str.IndexOf(c) < 0 Then
            Return Val(str)
        Else
            Return (Val(str.Split(c)(0)) + Val(str.Split(c)(1))) / 2
        End If
    End Function
    'Private Function Get_Area(ByVal type As String, ByVal data As Double()) As String                               '计算面积
    '    Dim Area As String = "Area"

    '    Return Area
    'End Function
    'Private Function Get_Weight(ByVal type As String, ByVal data As Double()) As String                             '计算重量
    '    Dim Weight As String = "Weight"

    '    Return Weight
    'End Function
    'Private Function LookupInTable(ByVal Profiles_Type As String, ByVal data As Double()) As String                          '在表格中查找
    '    Dim Val As String = ""

    '    Return Val
    'End Function
    'Private Sub ExportToExcel(ByVal sResault As String, ByVal BaseCell As Object)                                   '结果导出到Excel

    'End Sub
End Module
