'所有型材的基类
Public Class _BaseProfile
    Public Overridable Function Get_Resault(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        If Calculation_Type And TYPE_AREA Then
            Return Area(Calculation_Type, Calculation_Method)
        Else
            Return Weight(Calculation_Method)
        End If
    End Function
    Protected Overridable Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Return ""
    End Function
    Protected Overridable Function Weight(ByRef Calculation_Method As Integer) As String
        Return ""
    End Function
End Class
'H型钢(基类，不直接使用，不支持不等宽翼缘)
Public Class _H : Inherits _BaseProfile
    Public ShortHigh As Double
    Public ShortWidth As Double
    Public H As Double
    Public B As Double
    Public tH As Double
    Public tB As Double
    'Protected Overridable Function DataSheetName() As String
    '    Return "H"
    'End Function
    Private Protected Overridable Function DataRef() As GBDataStru()
        Return GBData_H
    End Function
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 2
                ShortHigh = Val(strArr(0)) : ShortWidth = Val(strArr(1))
                Expand()
            Case 4
                H = StrAverage(strArr(0), "～")
                B = StrAverage(strArr(1), "～")
                tH = Val(strArr(2)) : tB = Val(strArr(3))
        End Select
    End Sub
    Protected Sub Expand()
        '根据简写进行扩展
        'If PrepareForReadingData(DataSheetName, "$B:$J") Then
        '    Release()
        '    Exit Sub
        'End If
        'On Error Resume Next
        'H = DataFunc.ifna(DataFunc.vlookup(ShortHigh & "×" & ShortWidth, DataRange, 2, False), 0)
        'B = DataFunc.ifna(DataFunc.vlookup(ShortHigh & "×" & ShortWidth, DataRange, 3, False), 0)
        'tH = DataFunc.ifna(DataFunc.vlookup(ShortHigh & "×" & ShortWidth, DataRange, 4, False), 0)
        'tB = DataFunc.ifna(DataFunc.vlookup(ShortHigh & "×" & ShortWidth, DataRange, 5, False), 0)
        'Release()
        Dim arr As Double()
        arr = Search_Specification(DataRef, ShortHigh & "×" & ShortWidth)
        If arr Is Nothing Then
            arr = Search_Specification_ByPartData(DataRef, {ShortHigh, ShortWidth})
        End If
        If arr IsNot Nothing Then
            H = arr(0) : B = arr(1) : tH = arr(2) : tB = arr(3)
        End If
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleOfWidth As Integer

        Area = ""
        If tB = 0 Then Return Area
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            'If PrepareForReadingData(DataSheetName, "$B:$J") Then
            '    Release()
            '    Return Area
            'End If
            ''On Error Resume Next
            'Area = DataFunc.sumifs(DataSheet.range("$J:$J"), DataSheet.range("$C:$C"), H, DataSheet.range("$D:$D"), B, DataSheet.range("$E:$E"), tH, DataSheet.range("$F:$F"), tB)
            'Release()
            Area = Search_AorW(DataRef, {H, B, tH, tB}, 0)
            If Area = 0 Then Return ""
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then Area = Area & "-" & B * 0.001
        Else
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                MultipleOfWidth = 3
            Else
                MultipleOfWidth = 4
            End If
            Area = H * 0.001 & "*2+" & B * 0.001 & "*" & MultipleOfWidth
            If Calculation_Method And METHOD_PRECISELY Then Area = Area & "-" & tH * 0.001 & "*2"
        End If
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If tB = 0 Then Return Weight
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            'If PrepareForReadingData(DataSheetName, "$B:$J") Then
            '    Release()
            '    Return Weight
            'End If
            ''On Error Resume Next
            'Weight = DataFunc.sumifs(DataSheet.range("$I:$I"), DataSheet.range("$C:$C"), H, DataSheet.range("$D:$D"), B, DataSheet.range("$E:$E"), tH, DataSheet.range("$F:$F"), tB)
            'Release()
            Weight = Search_AorW(DataRef, {H, B, tH, tB}, 1)
            If Weight = 0 Then Return ""
        Else
            '粗略和精细方式相同
            Weight = "((" & H * 0.001 & "-" & tB * 0.001 & "*2)*" & tH * 0.001 & "+" & B * 0.001 & "*" & tB * 0.001 & "*2)*7850"
        End If
    End Function
End Class
'H型钢(实际使用的类，支持不等宽翼缘）
Public Class Profiles_H : Inherits _H
    Public B2 As Double
    Public tB2 As Double
    Public Overrides Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 2
                ShortHigh = Val(strArr(0)) : ShortWidth = Val(strArr(1))
                Expand()
                B2 = B : tB2 = tB
            Case 4
                H = StrAverage(strArr(0), "～")
                If strArr(1).IndexOf("/") < 0 Then
                    B = StrAverage(strArr(1), "～") : B2 = B
                Else
                    B = StrAverage(strArr(1).Split("/")(0), "～") : B2 = B = StrAverage(strArr(1).Split("/")(1), "～")
                End If
                tH = Val(strArr(2))
                If strArr(3).IndexOf("/") < 0 Then
                    tB = Val(strArr(3)) : tB2 = tB
                Else
                    tB = Val(strArr(3).Split("/")(0)) : tB2 = Val(strArr(3).Split("/")(1))
                End If
        End Select
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleOfWidth As Integer

        Area = ""
        If tB = 0 Then Return Area
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            Area = MyBase.Area(Calculation_Type, Calculation_Method)
        Else
            Area = H * 0.001 & "*2+"
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                MultipleOfWidth = 3
            Else
                MultipleOfWidth = 4
            End If
            If B = B2 Then
                Area = Area & B * 0.001 & "*" & MultipleOfWidth
            Else
                Area = Area & B * 0.001 & "*" & (MultipleOfWidth - 2) & "+" & B2 * 0.001 & "*2"
            End If
            If Calculation_Method And METHOD_PRECISELY Then Area = Area & "-" & tH * 0.001 & "*2"
        End If
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If tB = 0 Then Return Weight
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            Weight = MyBase.Weight(Calculation_Method)
        Else
            '粗略和精细方式相同
            If B = B2 Then
                Weight = "((" & H * 0.001 & "-" & tB * 0.001 & "*2)*" & tH * 0.001 & "+" & B * 0.001 & "*" & tB * 0.001 & "*2)*7850"
            Else
                Weight = "((" & H * 0.001 & "-" & tB * 0.001 & "-" & tB2 * 0.001 & ")*" & tH * 0.001 &
                                                        "+" & B * 0.001 & "*" & tB * 0.001 & "+" & B2 * 0.001 & "*" & tB2 * 0.001 & ")*7850"
            End If
        End If
    End Function
End Class
'HT型钢(采用简写形式可能与其他种类H型钢冲突，故独立出来)
Public Class Profiles_HT : Inherits _H
    'Protected Overrides Function DataSheetName() As String
    '    Return "HT"
    'End Function
    Private Protected Overrides Function DataRef() As GBDataStru()
        Return GBData_HT
    End Function
End Class
'双拼H型钢，十字交叉
Public Class Profiles_HI : Inherits _BaseProfile
    Public H1 As Double
    Public B1 As Double
    Public tH1 As Double
    Public tB1 As Double
    Public H2 As Double
    Public B2 As Double
    Public tH2 As Double
    Public tB2 As Double
    'Protected Overridable Function DataSheetName() As String
    '    Return "H"
    'End Function
    Private Protected Overridable Function DataRef() As GBDataStru()
        Return GBData_H
    End Function
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 3
                'to do：简写形式H1*B1+H2*B2
            Case 4
                H1 = StrAverage(strArr(0), "～") : B1 = StrAverage(strArr(1), "～") : tH1 = strArr(2) : tB1 = strArr(3)
                H2 = H1 : B2 = B1 : tH2 = tH1 : tB2 = tB1
            Case 7
                H1 = StrAverage(strArr(0), "～")
                B1 = StrAverage(strArr(1), "～")
                tH1 = Val(strArr(2))
                tB1 = Val(strArr(3).Split("+")(0))
                H2 = StrAverage(strArr(3).Split("+")(1), "～")
                B2 = StrAverage(strArr(4), "～")
                tH2 = Val(strArr(5))
                tB2 = Val(strArr(6))
        End Select
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleOfWidth As Integer
        Dim area1, area2 As String

        Area = ""
        If tB1 = 0 Then Return Area
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            'If PrepareForReadingData(DataSheetName, "$B:$J") Then
            '    Release()
            '    Return Area
            'End If
            ''On Error Resume Next
            'area1 = DataFunc.sumifs(DataSheet.range("$J:$J"), DataSheet.range("$C:$C"), H1, DataSheet.range("$D:$D"), B1, DataSheet.range("$E:$E"), tH1, DataSheet.range("$F:$F"), tB1)
            'area2 = DataFunc.sumifs(DataSheet.range("$J:$J"), DataSheet.range("$C:$C"), H2, DataSheet.range("$D:$D"), B2, DataSheet.range("$E:$E"), tH2, DataSheet.range("$F:$F"), tB2)
            'Release()
            area1 = Search_AorW(DataRef, {H1, B1, tH1, tB1}, 0)
            area2 = Search_AorW(DataRef, {H2, B2, tH2, tB2}, 0)
            If area1 = 0 Or area2 = 0 Then
                Return ""
            Else
                Area = area1 & "+" & area2
            End If
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then Area = Area & "-" & B1 * 0.001
        Else
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                MultipleOfWidth = 3
            Else
                MultipleOfWidth = 4
            End If
            If H1 = H2 And B1 = B2 And tH1 = tH2 And tB1 = tB2 Then
                Area = H1 * 0.001 & "*4+" & B1 * 0.001 & "*" & MultipleOfWidth + 4
            Else
                Area = H1 * 0.001 & "*2+" & B1 * 0.001 & "*" & MultipleOfWidth & "+" & H2 * 0.001 & "*2+" & B2 * 0.001 & "*4"
            End If
            If Calculation_Method And METHOD_PRECISELY Then
                If H1 = H2 And B1 = B2 And tH1 = tH2 And tB1 = tB2 Then
                    Area = Area & "-" & tH1 * 0.001 & "*8"
                Else
                    Area = Area & "-" & tH1 * 0.001 & "*4-" & tH2 * 0.001 & "*4"
                End If
            End If
        End If
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Dim weight1, weight2 As String

        Weight = ""
        If tB1 = 0 Then Return Weight
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            'If PrepareForReadingData(DataSheetName, "$B:$J") Then
            '    Release()
            '    Return Weight
            'End If
            ''On Error Resume Next
            'weight1 = DataFunc.sumifs(DataSheet.range("$I:$I"), DataSheet.range("$C:$C"), H1, DataSheet.range("$D:$D"), B1, DataSheet.range("$E:$E"), tH1, DataSheet.range("$F:$F"), tB1)
            'weight2 = DataFunc.sumifs(DataSheet.range("$I:$I"), DataSheet.range("$C:$C"), H2, DataSheet.range("$D:$D"), B2, DataSheet.range("$E:$E"), tH2, DataSheet.range("$F:$F"), tB2)
            'Release()
            weight1 = Search_AorW(DataRef, {H1, B1, tH1, tB1}, 1)
            weight2 = Search_AorW(DataRef, {H2, B2, tH2, tB2}, 1)
            If weight1 = 0 Or weight2 = 0 Then
                Return ""
            Else
                Weight = weight1 & "+" & weight2
            End If
        Else
            '粗略和精细方式相同
            If H1 = H2 And B1 = B2 And tH1 = tH2 And tB1 = tB2 Then
                Weight = "((" & H1 * 0.001 & "-" & tB1 * 0.001 & "*2)*" & tH1 * 0.001 & "+" & B1 * 0.001 & "*" & tB1 * 0.001 & "*2)*2*7850"
            Else
                Weight = "((" & H1 * 0.001 & "-" & tB1 * 0.001 & "*2)*" & tH1 * 0.001 & "+" & B1 * 0.001 & "*" & tB1 * 0.001 & "*2+(" &
                                        H2 * 0.001 & "-" & tB2 * 0.001 & "*2)*" & tH2 * 0.001 & "+" & B2 * 0.001 & "*" & tB2 * 0.001 & "*2)*7850"
            End If
        End If
    End Function
End Class
'T型钢
Public Class Profiles_T : Inherits _H
    'Protected Overrides Function DataSheetName() As String
    '    Return "T"
    'End Function
    Private Protected Overrides Function DataRef() As GBDataStru()
        Return GBData_T
    End Function
    Public Overrides Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 2
                ShortHigh = Val(strArr(0)) : ShortWidth = Val(strArr(1))
                Expand()
            Case 4
                H = StrAverage(strArr(0), "～")
                B = StrAverage(strArr(1), "～")
                tH = Val(strArr(2)) : tB = Val(strArr(3))
        End Select
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleOfWidth As Integer

        Area = ""
        If tB = 0 Then Return Area
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            Area = MyBase.Area(Calculation_Type, Calculation_Method)
        Else
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                MultipleOfWidth = 1
            Else
                MultipleOfWidth = 2
            End If
            Area = H * 0.001 & "*2+" & B * 0.001 & "*" & MultipleOfWidth
            If Calculation_Method And METHOD_PRECISELY Then Area = Area & "-" & tH * 0.001
        End If
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If tB = 0 Then Return Weight
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            Weight = MyBase.Weight(Calculation_Method)
        Else
            '粗略和精细方式相同
            Weight = "((" & H * 0.001 & "-" & tB * 0.001 & ")*" & tH * 0.001 & "+" & B * 0.001 & "*" & tB * 0.001 & ")*7850"
        End If
    End Function
End Class
'矩形管
Public Class Profiles_Rect : Inherits _BaseProfile
    Public H As Double
    Public B As Double
    Public tH As Double
    Public tB As Double
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 2
                H = Val(strArr(0)) : tH = Val(strArr(1))
                B = H : tB = tH
            Case 3, 4
                H = StrAverage(strArr(0), "～")
                B = StrAverage(strArr(1), "～")
                tH = Val(strArr(2))
                If strArr.Length = 3 Then
                    tB = tH
                Else
                    tB = Val(strArr(3))
                End If
        End Select
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleOfWidth As Integer

        Area = ""
        If tB = 0 Then Return Area
        '三种计算方式相同
        If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
            MultipleOfWidth = 1
        Else
            MultipleOfWidth = 2
        End If
        Area = H * 0.001 & "*2+" & B * 0.001 & "*" & MultipleOfWidth
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If tB = 0 Then Return Weight
        '三种计算方式相同
        Weight = "(" & H * 0.001 & "*" & tH * 0.001 & "+" & B * 0.001 & "*" & tB * 0.001 & ")*2*7850"
    End Function
End Class
'圆管
Public Class Profiles_Cir : Inherits _BaseProfile
    Public D As Double
    Public t As Double
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 1
                D = Val(strArr(0)) : t = 0
            Case 2
                D = Val(strArr(0)) : t = Val(strArr(1))
        End Select
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        If D = 0 Then Return ""
        '三种计算方式相同
        Area = "PI()*" & D * 0.001
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        If D = 0 Then Return ""
        '三种计算方式相同
        If t = 0 Then
            Weight = "PI()*" & D * 0.001 * 0.5 & "^2*7850"
        Else
            Weight = "PI()*(" & D * 0.001 * 0.5 & "^2-" & (D - t * 2) * 0.001 * 0.5 & "^2)*7850"
        End If
    End Function
End Class
'工字钢
Public Class Profiles_I : Inherits _BaseProfile
    Public ShortName As String
    Public H As Double
    Public B As Double
    Public tH As Double
    Public tB As Double
    'Protected Overridable Function DataSheetName() As String
    '    Return "I"
    'End Function
    Private Protected Overridable Function DataRef() As GBDataStru()
        Return GBData_I
    End Function
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 1
                ShortName = strArr(0)
                Expand()
            Case 2
                H = Val(strArr(0)) : B = Val(strArr(1))
                Expand()
            Case 4
                H = Val(strArr(0)) : B = Val(strArr(1)) : tH = Val(strArr(2)) : tB = Val(strArr(3))
        End Select
    End Sub
    Protected Sub Expand()
        '根据简写进行扩展
        'If PrepareForReadingData(DataSheetName, "$B:$K") Then
        '    Release()
        '    Exit Sub
        'End If
        'If ShortName <> "" Then
        '    On Error Resume Next
        '    H = DataFunc.ifna(DataFunc.vlookup(ShortName, DataRange, 2, False), 0)
        '    B = DataFunc.ifna(DataFunc.vlookup(ShortName, DataRange, 3, False), 0)
        '    tH = DataFunc.ifna(DataFunc.vlookup(ShortName, DataRange, 4, False), 0)
        '    tB = DataFunc.ifna(DataFunc.vlookup(ShortName, DataRange, 5, False), 0)
        'Else
        '    tH = DataFunc.sumifs(DataSheet.range("$E:$E"), DataSheet.range("$C:$C"), H, DataSheet.range("$D:$D"), B)
        '    tB = DataFunc.sumifs(DataSheet.range("$F:$F"), DataSheet.range("$C:$C"), H, DataSheet.range("$D:$D"), B)
        'End If
        'Release()
        Dim arr As Double()
        If ShortName <> "" Then
            arr = Search_Specification(DataRef, ShortName)
        Else
            arr = Search_Specification_ByPartData(DataRef, {H, B})
        End If
        If arr IsNot Nothing Then
            H = arr(0) : B = arr(1) : tH = arr(2) : tB = arr(3)
        End If
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleOfWidth As Integer

        Area = ""
        If tB = 0 Then Return Area
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            'If PrepareForReadingData(DataSheetName, "$B:$K") Then
            '    Release()
            '    Return Area
            'End If
            ''On Error Resume Next
            'Area = DataFunc.sumifs(DataSheet.range("$K:$K"), DataSheet.range("$C:$C"), H, DataSheet.range("$D:$D"), B, DataSheet.range("$E:$E"), tH, DataSheet.range("$F:$F"), tB)
            'Release()
            Area = Search_AorW(DataRef, {H, B, tH, tB}, 0)
            If Area = 0 Then Return ""
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then Area = Area & "-" & B * 0.001
        Else
            Area = H * 0.001 & "*2+"
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                MultipleOfWidth = 3
            Else
                MultipleOfWidth = 4
            End If
            Area = Area & B * 0.001 & "*" & MultipleOfWidth
            If Calculation_Method And METHOD_PRECISELY Then Area = Area & "-" & tH * 0.001 & "*2"
        End If
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If tB = 0 Then Return Weight
        '无论选择何种方式都按查表方式计算 
        'If PrepareForReadingData(DataSheetName, "$B:$K") Then
        '    Release()
        '    Return Weight
        'End If
        ''On Error Resume Next
        'Weight = DataFunc.sumifs(DataSheet.range("$J:$J"), DataSheet.range("$C:$C"), H, DataSheet.range("$D:$D"), B, DataSheet.range("$E:$E"), tH, DataSheet.range("$F:$F"), tB)
        'Release()
        Weight = Search_AorW(DataRef, {H, B, tH, tB}, 1)
        If Weight = 0 Then Return ""
    End Function
End Class
'槽钢
Public Class Profiles_Chan : Inherits Profiles_I
    'Protected Overrides Function DataSheetName() As String
    '    Return "Channel"
    'End Function
    Private Protected Overrides Function DataRef() As GBDataStru()
        Return GBData_Chan
    End Function
End Class
'双拼槽钢，口对口
Public Class Profiles_Chan_MtM : Inherits Profiles_Chan
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleofWidth As Integer

        Area = ""
        If tB = 0 Then Return Area
        '三种计算方式相同
        If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
            MultipleofWidth = 2
        Else
            MultipleofWidth = 4
        End If
        Area = H * 0.001 & "*2+" & B * 0.001 & "*" & MultipleofWidth
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If tB = 0 Then Return Weight
        Weight = MyBase.Weight(Calculation_Method)
        If Weight = "" Then Return Weight
        Weight &= "*2"
    End Function
End Class
'双拼槽钢，背对背
Public Class Profiles_Chan_BtB : Inherits Profiles_Chan
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleofWidth As Integer

        Area = ""
        If tB = 0 Then Return Area
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            Area = MyBase.Area(Calculation_Type, Calculation_Method)
            If Area = "" Then Return Area
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                Area = "(" & Area & ")*2-" & H * 0.001 & "*2"
            Else
                Area = Area & "*2-" & H * 0.001 & "*2"
            End If
        Else
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                MultipleofWidth = 6
            Else
                MultipleofWidth = 8
            End If
            Area = H * 0.001 & "*2+" & B * 0.001 & "*" & MultipleofWidth
            If Calculation_Method And METHOD_PRECISELY Then Area = Area & "-" & tH * 0.001 & "*4"
        End If
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If tB = 0 Then Return Weight
        Weight = MyBase.Weight(Calculation_Method)
        If Weight = "" Then Return Weight
        Weight &= "*2"
    End Function
End Class
'角钢
Public Class Profiles_L : Inherits _BaseProfile
    Public ShortName As String
    Public B1 As Double
    Public B2 As Double
    Public t As Double
    'Protected Overridable Function DataSheetName() As String
    '    Return "L"
    'End Function
    Private Protected Overridable Function DataRef() As GBDataStru()
        Return GBData_L
    End Function
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 1
                ShortName = strArr(0)
                Expand()
            Case 2
                B1 = Val(strArr(0)) : B2 = B1 : t = Val(strArr(1))
            Case 3
                B1 = Val(strArr(0)) : B2 = Val(strArr(1)) : t = Val(strArr(2))
        End Select
    End Sub
    Protected Sub Expand()
        '根据简写进行扩展
        'If PrepareForReadingData(DataSheetName, "$B:$I") Then
        '    Release()
        '    Exit Sub
        'End If
        'On Error Resume Next
        'B1 = DataFunc.ifna(DataFunc.vlookup(ShortName, DataRange, 2, False), 0)
        'B2 = DataFunc.ifna(DataFunc.vlookup(ShortName, DataRange, 3, False), 0)
        't = DataFunc.ifna(DataFunc.vlookup(ShortName, DataRange, 4, False), 0)
        'Release()
        Dim arr As Double()
        arr = Search_Specification(DataRef, ShortName)
        If arr IsNot Nothing Then
            B1 = arr(0) : B2 = arr(1) : t = arr(2)
        End If
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipleofWidth As Integer

        Area = ""
        If t = 0 Then Return Area
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            'If PrepareForReadingData(DataSheetName, "$B:$I") Then
            '    Release()
            '    Return Area
            'End If
            ''On Error Resume Next
            'Area = DataFunc.sumifs(DataSheet.range("$I:$I"), DataSheet.range("$C:$C"), B1, DataSheet.range("$D:$D"), B2, DataSheet.range("$E:$E"), t)
            'Release()
            Area = Search_AorW(DataRef, {B1, B2, t}, 0)
            If Area = 0 Then Return ""
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then Area = Area & "-" & B2 * 0.001
        Else
            '粗略和精细方式相同
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                MultipleofWidth = 1
            Else
                MultipleofWidth = 2
            End If
            If B1 = B2 Then
                Area = B1 * 0.001 & "*" & MultipleofWidth + 2
            Else
                Area = B1 * 0.001 & "*2+" & B2 * 0.001 & "*" & MultipleofWidth
            End If
        End If
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If t = 0 Then Return Weight
        '无论选择何种方式都按查表方式计算
        'If PrepareForReadingData(DataSheetName, "$B:$I") Then
        '    Release()
        '    Return Weight
        'End If
        ''On Error Resume Next
        'Weight = DataFunc.sumifs(DataSheet.range("$H:$H"), DataSheet.range("$C:$C"), B1, DataSheet.range("$D:$D"), B2, DataSheet.range("$E:$E"), t)
        'Release()
        Weight = Search_AorW(DataRef, {B1, B2, t}, 1)
        If Weight = 0 Then Return ""
    End Function
End Class
'双拼角钢，背对背
Public Class Profiles_2L : Inherits Profiles_L

    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipeofWidth As Integer

        Area = ""
        If t = 0 Then Return Area
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            Area = MyBase.Area(Calculation_Type, Calculation_Method)
            If Area = "" Then Return Area
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                Area = "(" & Area & "-" & B1 * 0.001 & ")*2"
            Else
                Area = Area & "-" & B1 * 0.001 & "*2"
            End If
        Else
            '粗略和精细方式相同
            If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
                MultipeofWidth = 2
            Else
                MultipeofWidth = 4
            End If
            If B1 = B2 Then
                Area = B1 * 0.001 & "*" & MultipeofWidth + 2
            Else
                Area = B1 * 0.001 & "*2+" & B2 * 0.001 & "*" & MultipeofWidth
            End If
        End If
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If t = 0 Then Return Weight
        Weight = MyBase.Weight(Calculation_Method)
        If Weight = "" Then Return Weight
        Weight &= "*2"
    End Function
End Class
'C型钢
Public Class Profiles_C : Inherits _BaseProfile
    Public H As Double
    Public B As Double
    Public C As Double
    Public t As Double
    'Protected Overridable Function DataSheetName() As String
    '    Return "C"
    'End Function
    Private Protected Overridable Function DataRef() As GBDataStru()
        Return GBData_C
    End Function
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, TypeStr.Length)
        strArr = DataStr.Split("×")
        If strArr.Length = 4 Then
            H = Val(strArr(0)) : B = Val(strArr(1)) : C = Val(strArr(2)) : t = Val(strArr(3))
        End If
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipeofWidth As Integer

        Area = ""
        If t = 0 Then Return Area
        '不支持查表方式（查表方式按精细方式计算）
        If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
            MultipeofWidth = 3
        Else
            MultipeofWidth = 4
        End If
        Area = H * 0.001 & "*2+" & B * 0.001 & "*" & MultipeofWidth & "+" & C * 0.001 & "*4"
        If (Calculation_Method And METHOD_ROUGHLY) = 0 Then Area = Area & "-" & t * 0.001 & "*6"
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If t = 0 Then Return Weight
        If Calculation_Method And METHOD_LOOKUPINTABLE Then
            'If PrepareForReadingData(DataSheetName, "$B:$H") Then
            '    Release()
            '    Return Weight
            'End If
            ''On Error Resume Next
            'Weight = DataFunc.sumifs(DataSheet.range("$G:$G"), DataSheet.range("$C:$C"), H, DataSheet.range("$D:$D"), B, DataSheet.range("$E:$E"), C, DataSheet.range("$F:$F"), t)
            'Release()
            Weight = Search_AorW(DataRef, {H, B, C, t}, 1)
            If Weight = 0 Then Return ""
        Else
            Weight = "(" & H * 0.001 & "+(" & B * 0.001 & "-" & t * 0.001 & "*2)*2+" & C * 0.001 & "*2)*" & t * 0.001 & "*7850"
        End If
    End Function
End Class
'C型钢，口对口
Public Class Profiles_2C : Inherits Profiles_C
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim MultipeofWidth As Integer

        Area = ""
        If t = 0 Then Return Area
        '三种计算方式相同
        If Calculation_Type And TYPE_DEDUCTTOPSURFACE Then
            MultipeofWidth = 2
        Else
            MultipeofWidth = 4
        End If
        Area = H * 0.001 & "*2+" & B * 0.001 & "*" & MultipeofWidth
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = MyBase.Weight(Calculation_Method)
        If Weight = "" Then Return Weight
        Weight &= "*2"
    End Function
End Class
'Z型钢
Public Class Profiles_Z : Inherits Profiles_C
    'Protected Overrides Function DataSheetName() As String
    '    Return "Z"
    'End Function
    Private Protected Overrides Function DataRef() As GBDataStru()
        Return GBData_Z
    End Function
End Class
'矩形板件(基类，不直接使用)
Public Class _Profiles_PL : Inherits _BaseProfile
    Public L As Double
    Public B As Double
    Public t As Double
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, DataStr.IndexOf(TypeStr) + TypeStr.Length)
        strArr = DataStr.Split("×")
        Select Case strArr.Length
            Case 2
                L = StrAverage(strArr(0), "～")
                t = Val(strArr(1))
            Case 3
                L = StrAverage(strArr(0), "～")
                B = StrAverage(strArr(1), "～")
                t = Val(strArr(2))
        End Select
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Area = ""
        If L = 0 Then Return Area
        '三种计算方式相同
        Area = L * 0.001
        If (Calculation_Type And TYPE_DEDUCTTOPSURFACE) = 0 Then Area &= "*2"
        If B <> 0 Then Area = Area & "*" & B * 0.001
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If L = 0 Then Return Weight
        '三种计算方式相同
        Weight = L * 0.001
        If B = 0 Then
            Weight = Weight & "*" & t * 0.001 & "*7850"
        Else
            Weight = Weight & "*" & B * 0.001 & "*" & t * 0.001 & "*7850"
        End If
    End Function
End Class
'矩形板件(实际使用的类)
Public Class Profiles_PL : Inherits _Profiles_PL
    Public PL_Arr() As _Profiles_PL
    Public PLT_Arr() As Profiles_PLT
    Public PLD_Arr() As Profiles_PLD
    Public Overrides Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim str, strArr() As String
        Dim i, j, k, n As Integer
        i = 0 : j = 0 : k = 0
        strArr = DataStr.Split("-")
        For Each str In strArr
            If str.IndexOf("PLD") >= 0 Then
                n = Val(str) : If n = 0 Then n = 1
                Do While (n > 0)
                    ReDim Preserve PLD_Arr(k)
                    PLD_Arr(k) = New Profiles_PLD
                    PLD_Arr(k).GetData(str, "PLD")
                    k += 1
                    n -= 1
                Loop
            ElseIf str.IndexOf("PLT") >= 0 Then
                n = Val(str) : If n = 0 Then n = 1
                Do While (n > 0)
                    ReDim Preserve PLT_Arr(j)
                    PLT_Arr(j) = New Profiles_PLT
                    PLT_Arr(j).GetData(str, "PLT")
                    j += 1
                    n -= 1
                Loop
            ElseIf str.IndexOf("PL") >= 0 Then
                n = Val(str) : If n = 0 Then n = 1
                Do While (n > 0)
                    ReDim Preserve PL_Arr(i)
                    PL_Arr(i) = New _Profiles_PL
                    PL_Arr(i).GetData(str, "PL")
                    i += 1
                    n -= 1
                Loop
            End If
        Next
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Dim _area As String
        Dim i, j, k As Integer
        Dim obj As Object
        Area = ""
        If PL_Arr IsNot Nothing Then i = PL_Arr.Length
        If PLT_Arr IsNot Nothing Then j = PLT_Arr.Length
        If PLD_Arr IsNot Nothing Then k = PLD_Arr.Length
        If i = 0 And j = 0 And k = 0 Then Return Area
        If i = 1 And j = 0 And k = 0 Then
            Area = PL_Arr(0).Get_Resault(Calculation_Type, Calculation_Method)
        Else
            If i > 0 Then
                For Each obj In PL_Arr
                    _area = obj.get_resault(Calculation_Type, Calculation_Method)
                    If _area <> "" Then Area = Area & "(" & _area & ")-"
                Next
            End If
            If j > 0 Then
                For Each obj In PLT_Arr
                    _area = obj.get_resault(Calculation_Type, Calculation_Method)
                    If _area <> "" Then Area = Area & "(" & _area & ")-"
                Next
            End If
            If k > 0 Then
                For Each obj In PLD_Arr
                    _area = obj.get_resault(Calculation_Type, Calculation_Method)
                    If _area <> "" Then Area = Area & "(" & _area & ")-"
                Next
            End If
        End If
        If Area.EndsWith("-") Then Area = Area.Remove(Area.Length - 1)
        'obj = Nothing
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Dim _weight As String
        Dim i, j, k As Integer
        Dim obj As Object
        Weight = ""
        If PL_Arr IsNot Nothing Then i = PL_Arr.Length
        If PLT_Arr IsNot Nothing Then j = PLT_Arr.Length
        If PLD_Arr IsNot Nothing Then k = PLD_Arr.Length
        If i = 0 And j = 0 And k = 0 Then Return Weight
        If i = 1 And j = 0 And k = 0 Then
            Weight = PL_Arr(0).Get_Resault(Calculation_Type, Calculation_Method)
        Else
            If i > 0 Then
                For Each obj In PL_Arr
                    _weight = obj.Get_Resault(Calculation_Type, Calculation_Method)
                    If _weight <> "" Then Weight = Weight & "(" & _weight & ")-"
                Next
            End If
            If j > 0 Then
                For Each obj In PLT_Arr
                    _weight = obj.Get_Resault(Calculation_Type, Calculation_Method)
                    If _weight <> "" Then Weight = Weight & "(" & _weight & ")-"
                Next
            End If
            If k > 0 Then
                For Each obj In PLD_Arr
                    _weight = obj.Get_Resault(Calculation_Type, Calculation_Method)
                    If _weight <> "" Then Weight = Weight & "(" & _weight & ")-"
                Next
            End If
        End If
        If Weight.EndsWith("-") Then Weight = Weight.Remove(Weight.Length - 1)
        'obj = Nothing
    End Function
End Class
'直角三角形板件
Public Class Profiles_PLT : Inherits _Profiles_PL
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Area = ""
        If L = 0 Then Return Area
        '三种计算方式相同
        Area = L * 0.001 & "*0.5"
        If (Calculation_Type And TYPE_DEDUCTTOPSURFACE) = 0 Then Area &= "*2"
        If B <> 0 Then Area = Area & "*" & B * 0.001
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If L = 0 Then Return Weight
        '三种计算方式相同
        Weight = L * 0.001 & "*0.5"
        If B = 0 Then
            Weight = Weight & "*" & t * 0.001 & "*7850"
        Else
            Weight = Weight & "*" & B * 0.001 & "*" & t * 0.001 & "*7850"
        End If
    End Function
End Class
'圆形板件
Public Class Profiles_PLD : Inherits _BaseProfile
    Public D As Double
    Public t As Double
    Public Overridable Sub GetData(ByRef DataStr As String, ByRef TypeStr As String)
        Dim strArr() As String
        DataStr = DataStr.Remove(0, DataStr.IndexOf(TypeStr) + TypeStr.Length)
        strArr = DataStr.Split("×")
        If strArr.Length = 2 Then
            D = Val(strArr(0)) : t = Val(strArr(1))
        End If
    End Sub
    Protected Overrides Function Area(ByRef Calculation_Type As Integer, ByRef Calculation_Method As Integer) As String
        Area = ""
        If D = 0 Then Return Area
        '三种计算方式相同
        Area = "PI()*" & D * 0.001 * 0.5 & "^2"
        If (Calculation_Type And TYPE_DEDUCTTOPSURFACE) = 0 Then Area &= "*2"
    End Function
    Protected Overrides Function Weight(ByRef Calculation_Method As Integer) As String
        Weight = ""
        If D = 0 Then Return Weight
        '三种计算方式相同
        Weight = "PI()*" & D * 0.001 * 0.5 & "^2" & "*" & t * 0.001 & "*7850"
    End Function
End Class
'测试用
'Public Class _TestClass
'    Public Function resault(ByVal Flag As Boolean) As String
'        If Flag Then
'            Return A()
'        Else
'            Return B()
'        End If
'    End Function
'    Protected Overridable Function A() As String
'        Return "A"
'    End Function
'    Protected Overridable Function B() As String
'        Return "B"
'    End Function
'End Class
'Public Class TestClass : Inherits _TestClass
'    Protected Overrides Function A() As String
'        Return "A1"
'    End Function
'    Protected Overrides Function B() As String
'        Return "B1"
'    End Function
'End Class
Module Profiles_Func
    Public Function StrAverage(ByRef str As String, ByRef c As Char) As Double
        If str.IndexOf(c) < 0 Then
            Return Val(str)
        Else
            Return (Val(str.Split(c)(0)) + Val(str.Split(c)(1))) / 2
        End If
    End Function
End Module