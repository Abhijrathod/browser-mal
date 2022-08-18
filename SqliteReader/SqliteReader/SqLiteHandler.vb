Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CompilerServices

Public Class SqLiteHandler

    Private Structure sqlite_master_entry
        Public row_id As Long
        Public item_type As String
        Public item_name As String
        Public astable_name As String
        Public root_num As Long
        Public sql_statement As String
    End Structure

    Private Structure record_header_field
        Public size As Long
        Public type As Long
    End Structure

    Private Structure table_entry
        Public row_id As Long
        Public content As String()
    End Structure

    Private db_bytes As Byte()
    Private mEncoding As ULong
    Private field_names As String()
    Private master_table_entries As SqLiteHandler.sqlite_master_entry()
    Private page_size As UShort
    Private SQLDataTypeSize As Byte()
    Private table_entries As SqLiteHandler.table_entry()

    <MethodImpl(MethodImplOptions.NoInlining Or MethodImplOptions.NoOptimization)>
    Public Sub New(baseName As String)
        Me.SQLDataTypeSize = New Byte() {0, 1, 2, 3, 4, 6, 8, 8, 0, 0}
        If File.Exists(baseName) Then
            FileSystem.FileOpen(1, baseName, OpenMode.Binary, OpenAccess.Read, OpenShare.[Shared], -1)
            Dim s As String = Strings.Space(CInt(FileSystem.LOF(1)))
            FileSystem.FileGet(1, s, -1L, False)
            FileSystem.FileClose(New Integer() {1})
            Me.db_bytes = Encoding.[Default].GetBytes(s)
            If String.Compare(Encoding.[Default].GetString(Me.db_bytes, 0, 15), "SQLite format 3", StringComparison.Ordinal) <> 0 Then
                Throw New Exception("Not a valid SQLite 3 Database File")
            End If
            If Me.db_bytes(52) <> 0 Then
                Throw New Exception("Auto-vacuum capable database is not supported")
            End If
            Me.page_size = CUShort(Me.ConvertToInteger(16, 2))
            Me.mEncoding = Me.ConvertToInteger(56, 4)
            Dim d As Decimal = New Decimal(Me.mEncoding)
            If Decimal.Compare(d, 0D) = 0 Then
                Me.mEncoding = 1UL
            End If
            Me.ReadMasterTable(100UL)
        End If
    End Sub

    Private Function ConvertToInteger(startIndex As Integer, Size As Integer) As ULong
        If Size > 8 Or Size = 0 Then
            Return 0UL
        End If
        Dim num As ULong = 0UL
        Dim num2 As Integer = Size - 1
        Dim num3 As Integer = 0
        Dim num4 As Integer = num2
        For i As Integer = num3 To num4
            num = (num << 8 Or CULng(Me.db_bytes(startIndex + i)))
        Next
        Return num
    End Function

    Private Function CVL(startIndex As Integer, endIndex As Integer) As Long
        endIndex += 1
        Dim array As Byte() = New Byte(7) {}
        Dim num As Integer = endIndex - startIndex
        Dim flag As Boolean = False
        If num = 0 Or num > 9 Then
            Return 0L
        End If
        If num = 1 Then
            array(0) = (Me.db_bytes(startIndex) And 127)
            Return BitConverter.ToInt64(array, 0)
        End If
        If num = 9 Then
            flag = True
        End If
        Dim num2 As Integer = 1
        Dim num3 As Integer = 7
        Dim num4 As Integer = 0
        If flag Then
            array(0) = Me.db_bytes(endIndex - 1)
            endIndex -= 1
            num4 = 1
        End If
        For i As Integer = endIndex - 1 To startIndex Step -1
            If i - 1 >= startIndex Then
                array(num4) = CByte(((CInt(CByte((CUInt(Me.db_bytes(i)) >> (num2 - 1 And 7 And 7)))) And 255 >> num2) Or CInt(CByte((Me.db_bytes(i - 1) << (num3 And 7 And 7))))))
                num2 += 1
                num4 += 1
                num3 -= 1
            ElseIf Not flag Then
                array(num4) = CByte((CInt((CByte((CUInt(Me.db_bytes(i)) >> (num2 - 1 And 7 And 7))))) And 255 >> num2))
            End If
        Next
        Return BitConverter.ToInt64(array, 0)
    End Function

    Public Function GetRowCount() As Integer
        Return Me.table_entries.Length
    End Function

    Public Function GetTableNames() As String()
        Dim array As String() = Nothing
        Dim num As Integer = 0
        Dim num2 As Integer = Me.master_table_entries.Length - 1
        Dim num3 As Integer = 0
        Dim num4 As Integer = num2
        For i As Integer = num3 To num4
            If Operators.CompareString(Me.master_table_entries(i).item_type, "table", False) = 0 Then
                array = CType(Utils.CopyArray(array, New String(num + 1 - 1) {}), String())
                array(num) = Me.master_table_entries(i).item_name
                num += 1
            End If
        Next
        Return array
    End Function

    Public Function GetValue(row_num As Integer, field As Integer) As String
        If row_num >= Me.table_entries.Length Then
            Return Nothing
        End If
        If field >= Me.table_entries(row_num).content.Length Then
            Return Nothing
        End If
        Return Me.table_entries(row_num).content(field)
    End Function

    Public Function GetValue(row_num As Integer, field As String) As String
        Dim num As Integer = -1
        Dim num2 As Integer = Me.field_names.Length - 1
        Dim num3 As Integer = 0
        Dim num4 As Integer = num2
        For i As Integer = num3 To num4
            If Me.field_names(i).ToLower().CompareTo(field.ToLower()) = 0 Then
                num = i
                Exit For
            End If
        Next
        If num = -1 Then
            Return Nothing
        End If
        Return Me.GetValue(row_num, num)
    End Function

    Private Function GVL(startIndex As Integer) As Integer
        If startIndex > Me.db_bytes.Length Then
            Return 0
        End If
        Dim num As Integer = startIndex + 8
        Dim num2 As Integer = num
        For i As Integer = startIndex To num2
            If i > Me.db_bytes.Length - 1 Then
                Return 0
            End If
            If (Me.db_bytes(i) And 128) <> 128 Then
                Return i
            End If
        Next
        Return startIndex + 8
    End Function

    Private Function IsOdd(value As Long) As Boolean
        Return (value And 1L) = 1L
    End Function

    Private Sub ReadMasterTable(Offset As ULong)
        If Me.db_bytes(CInt(Offset)) = 13 Then
            Dim num As Decimal = New Decimal(Offset)
            Dim num2 As Decimal = New Decimal(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(num, 3D)), 2))
            Dim num3 As UShort = Convert.ToUInt16(Decimal.Subtract(num2, 1D))
            Dim num4 As Integer = 0
            If Me.master_table_entries IsNot Nothing Then
                num4 = Me.master_table_entries.Length
                Me.master_table_entries = CType(Utils.CopyArray(Me.master_table_entries, New SqLiteHandler.sqlite_master_entry(Me.master_table_entries.Length + CInt(num3) + 1 - 1) {}), SqLiteHandler.sqlite_master_entry())
            Else
                Me.master_table_entries = New SqLiteHandler.sqlite_master_entry(CInt((num3 + 1US)) - 1) {}
            End If
            Dim num5 As Integer = CInt(num3)
            Dim num6 As Integer = 0
            Dim num7 As Integer = num5
            For i As Integer = num6 To num7
                num2 = New Decimal(Offset)
                Dim d As Decimal = Decimal.Add(num2, 8D)
                num = New Decimal(i * 2)
                Dim num8 As ULong = Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(d, num)), 2)
                num2 = New Decimal(Offset)
                If Decimal.Compare(num2, 100D) <> 0 Then
                    num8 += Offset
                End If
                Dim num9 As Integer = Me.GVL(CInt(num8))
                Dim num10 As Long = Me.CVL(CInt(num8), num9)
                num2 = New Decimal(num8)
                Dim d2 As Decimal = num2
                num = New Decimal(num9)
                Dim d3 As Decimal = num
                Dim num11 As Decimal = New Decimal(num8)
                Dim num12 As Integer = Me.GVL(Convert.ToInt32(Decimal.Add(Decimal.Add(d2, Decimal.Subtract(d3, num11)), 1D)))
                Dim array As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                Dim num13 As Integer = num4 + i
                num11 = New Decimal(num8)
                Dim d4 As Decimal = num11
                num2 = New Decimal(num9)
                Dim d5 As Decimal = num2
                num = New Decimal(num8)
                array(num13).row_id = Me.CVL(Convert.ToInt32(Decimal.Add(Decimal.Add(d4, Decimal.Subtract(d5, num)), 1D)), num12)
                num11 = New Decimal(num8)
                Dim d6 As Decimal = num11
                num2 = New Decimal(num12)
                Dim d7 As Decimal = num2
                num = New Decimal(num8)
                num8 = Convert.ToUInt64(Decimal.Add(Decimal.Add(d6, Decimal.Subtract(d7, num)), 1D))
                num9 = Me.GVL(CInt(num8))
                num12 = num9
                Dim value As Long = Me.CVL(CInt(num8), num9)
                Dim array2 As Long() = New Long(4) {}
                Dim num14 As Integer = 0
                Do
                    num9 = num12 + 1
                    num12 = Me.GVL(num9)
                    array2(num14) = Me.CVL(num9, num12)
                    If array2(num14) > 9L Then
                        If Me.IsOdd(array2(num14)) Then
                            array2(num14) = CLng(Math.Round(Math.Round(CDbl((array2(num14) - 13L)) / 2.0)))
                        Else
                            array2(num14) = CLng(Math.Round(Math.Round(CDbl((array2(num14) - 12L)) / 2.0)))
                        End If
                    Else
                        array2(num14) = CLng((CULng(Me.SQLDataTypeSize(CInt(array2(num14))))))
                    End If
                    num14 += 1
                Loop While num14 <= 4
                num11 = New Decimal(Me.mEncoding)
                If Decimal.Compare(num11, 1D) = 0 Then
                    Dim array3 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                    Dim num15 As Integer = num4 + i
                    Dim [default] As Encoding = Encoding.[Default]
                    Dim bytes As Byte() = Me.db_bytes
                    num2 = New Decimal(num8)
                    Dim d8 As Decimal = num2
                    num = New Decimal(value)
                    array3(num15).item_type = [default].GetString(bytes, Convert.ToInt32(Decimal.Add(d8, num)), CInt(array2(0)))
                Else
                    num11 = New Decimal(Me.mEncoding)
                    If Decimal.Compare(num11, 2D) = 0 Then
                        Dim array4 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                        Dim num16 As Integer = num4 + i
                        Dim unicode As Encoding = Encoding.Unicode
                        Dim bytes2 As Byte() = Me.db_bytes
                        num2 = New Decimal(num8)
                        Dim d9 As Decimal = num2
                        num = New Decimal(value)
                        array4(num16).item_type = unicode.GetString(bytes2, Convert.ToInt32(Decimal.Add(d9, num)), CInt(array2(0)))
                    Else
                        num11 = New Decimal(Me.mEncoding)
                        If Decimal.Compare(num11, 3D) = 0 Then
                            Dim array5 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                            Dim num17 As Integer = num4 + i
                            Dim bigEndianUnicode As Encoding = Encoding.BigEndianUnicode
                            Dim bytes3 As Byte() = Me.db_bytes
                            num2 = New Decimal(num8)
                            Dim d10 As Decimal = num2
                            num = New Decimal(value)
                            array5(num17).item_type = bigEndianUnicode.GetString(bytes3, Convert.ToInt32(Decimal.Add(d10, num)), CInt(array2(0)))
                        End If
                    End If
                End If
                num11 = New Decimal(Me.mEncoding)
                Dim num19 As Decimal
                If Decimal.Compare(num11, 1D) = 0 Then
                    Dim array6 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                    Dim num18 As Integer = num4 + i
                    Dim default2 As Encoding = Encoding.[Default]
                    Dim bytes4 As Byte() = Me.db_bytes
                    num2 = New Decimal(num8)
                    Dim d11 As Decimal = num2
                    num = New Decimal(value)
                    Dim d12 As Decimal = Decimal.Add(d11, num)
                    num19 = New Decimal(array2(0))
                    array6(num18).item_name = default2.GetString(bytes4, Convert.ToInt32(Decimal.Add(d12, num19)), CInt(array2(1)))
                Else
                    num19 = New Decimal(Me.mEncoding)
                    If Decimal.Compare(num19, 2D) = 0 Then
                        Dim array7 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                        Dim num20 As Integer = num4 + i
                        Dim unicode2 As Encoding = Encoding.Unicode
                        Dim bytes5 As Byte() = Me.db_bytes
                        num11 = New Decimal(num8)
                        Dim d13 As Decimal = num11
                        num2 = New Decimal(value)
                        Dim d14 As Decimal = Decimal.Add(d13, num2)
                        num = New Decimal(array2(0))
                        array7(num20).item_name = unicode2.GetString(bytes5, Convert.ToInt32(Decimal.Add(d14, num)), CInt(array2(1)))
                    Else
                        num19 = New Decimal(Me.mEncoding)
                        If Decimal.Compare(num19, 3D) = 0 Then
                            Dim array8 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                            Dim num21 As Integer = num4 + i
                            Dim bigEndianUnicode2 As Encoding = Encoding.BigEndianUnicode
                            Dim bytes6 As Byte() = Me.db_bytes
                            num11 = New Decimal(num8)
                            Dim d15 As Decimal = num11
                            num2 = New Decimal(value)
                            Dim d16 As Decimal = Decimal.Add(d15, num2)
                            num = New Decimal(array2(0))
                            array8(num21).item_name = bigEndianUnicode2.GetString(bytes6, Convert.ToInt32(Decimal.Add(d16, num)), CInt(array2(1)))
                        End If
                    End If
                End If
                Dim array9 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                Dim num22 As Integer = num4 + i
                num19 = New Decimal(num8)
                Dim d17 As Decimal = num19
                num11 = New Decimal(value)
                Dim d18 As Decimal = Decimal.Add(d17, num11)
                num2 = New Decimal(array2(0))
                Dim d19 As Decimal = Decimal.Add(d18, num2)
                num = New Decimal(array2(1))
                Dim d20 As Decimal = Decimal.Add(d19, num)
                Dim num23 As Decimal = New Decimal(array2(2))
                array9(num22).root_num = CLng(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(d20, num23)), CInt(array2(3))))
                num23 = New Decimal(Me.mEncoding)
                If Decimal.Compare(num23, 1D) = 0 Then
                    Dim array10 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                    Dim num24 As Integer = num4 + i
                    Dim default3 As Encoding = Encoding.[Default]
                    Dim bytes7 As Byte() = Me.db_bytes
                    num19 = New Decimal(num8)
                    Dim d21 As Decimal = num19
                    num11 = New Decimal(value)
                    Dim d22 As Decimal = Decimal.Add(d21, num11)
                    num2 = New Decimal(array2(0))
                    Dim d23 As Decimal = Decimal.Add(d22, num2)
                    num = New Decimal(array2(1))
                    Dim d24 As Decimal = Decimal.Add(d23, num)
                    Dim num25 As Decimal = New Decimal(array2(2))
                    Dim d25 As Decimal = Decimal.Add(d24, num25)
                    Dim num26 As Decimal = New Decimal(array2(3))
                    array10(num24).sql_statement = default3.GetString(bytes7, Convert.ToInt32(Decimal.Add(d25, num26)), CInt(array2(4)))
                Else
                    Dim num26 As Decimal = New Decimal(Me.mEncoding)
                    If Decimal.Compare(num26, 2D) = 0 Then
                        Dim array11 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                        Dim num27 As Integer = num4 + i
                        Dim unicode3 As Encoding = Encoding.Unicode
                        Dim bytes8 As Byte() = Me.db_bytes
                        Dim num25 As Decimal = New Decimal(num8)
                        Dim d26 As Decimal = num25
                        num23 = New Decimal(value)
                        Dim d27 As Decimal = Decimal.Add(d26, num23)
                        num19 = New Decimal(array2(0))
                        Dim d28 As Decimal = Decimal.Add(d27, num19)
                        num11 = New Decimal(array2(1))
                        Dim d29 As Decimal = Decimal.Add(d28, num11)
                        num2 = New Decimal(array2(2))
                        Dim d30 As Decimal = Decimal.Add(d29, num2)
                        num = New Decimal(array2(3))
                        array11(num27).sql_statement = unicode3.GetString(bytes8, Convert.ToInt32(Decimal.Add(d30, num)), CInt(array2(4)))
                    Else
                        num26 = New Decimal(Me.mEncoding)
                        If Decimal.Compare(num26, 3D) = 0 Then
                            Dim array12 As SqLiteHandler.sqlite_master_entry() = Me.master_table_entries
                            Dim num28 As Integer = num4 + i
                            Dim bigEndianUnicode3 As Encoding = Encoding.BigEndianUnicode
                            Dim bytes9 As Byte() = Me.db_bytes
                            Dim num25 As Decimal = New Decimal(num8)
                            Dim d31 As Decimal = num25
                            num23 = New Decimal(value)
                            Dim d32 As Decimal = Decimal.Add(d31, num23)
                            num19 = New Decimal(array2(0))
                            Dim d33 As Decimal = Decimal.Add(d32, num19)
                            num11 = New Decimal(array2(1))
                            Dim d34 As Decimal = Decimal.Add(d33, num11)
                            num2 = New Decimal(array2(2))
                            Dim d35 As Decimal = Decimal.Add(d34, num2)
                            num = New Decimal(array2(3))
                            array12(num28).sql_statement = bigEndianUnicode3.GetString(bytes9, Convert.ToInt32(Decimal.Add(d35, num)), CInt(array2(4)))
                        End If
                    End If
                End If
            Next
        ElseIf Me.db_bytes(CInt(Offset)) = 5 Then
            Dim num26 As Decimal = New Decimal(Offset)
            Dim num25 As Decimal = New Decimal(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(num26, 3D)), 2))
            Dim num29 As UShort = Convert.ToUInt16(Decimal.Subtract(num25, 1D))
            Dim num30 As Integer = CInt(num29)
            Dim num31 As Integer = 0
            Dim num32 As Integer = num30
            Dim num23 As Decimal
            For j As Integer = num31 To num32
                num26 = New Decimal(Offset)
                Dim d36 As Decimal = Decimal.Add(num26, 12D)
                num25 = New Decimal(j * 2)
                Dim num33 As UShort = CUShort(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(d36, num25)), 2))
                num26 = New Decimal(Offset)
                If Decimal.Compare(num26, 100D) = 0 Then
                    num25 = New Decimal(Me.ConvertToInteger(CInt(num33), 4))
                    Dim d37 As Decimal = Decimal.Subtract(num25, 1D)
                    num23 = New Decimal(CInt(Me.page_size))
                    Me.ReadMasterTable(Convert.ToUInt64(Decimal.Multiply(d37, num23)))
                Else
                    num26 = New Decimal(Me.ConvertToInteger(CInt((Offset + CULng(num33))), 4))
                    Dim d38 As Decimal = Decimal.Subtract(num26, 1D)
                    num25 = New Decimal(CInt(Me.page_size))
                    Me.ReadMasterTable(Convert.ToUInt64(Decimal.Multiply(d38, num25)))
                End If
            Next
            num26 = New Decimal(Offset)
            num25 = New Decimal(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(num26, 8D)), 4))
            Dim d39 As Decimal = Decimal.Subtract(num25, 1D)
            num23 = New Decimal(CInt(Me.page_size))
            Me.ReadMasterTable(Convert.ToUInt64(Decimal.Multiply(d39, num23)))
        End If
    End Sub

    Public Function ReadTable(TableName As String) As Boolean
        Dim num As Integer = -1
        Dim num2 As Integer = Me.master_table_entries.Length - 1
        Dim num3 As Integer = 0
        Dim num4 As Integer = num2
        For i As Integer = num3 To num4
            If String.Compare(Me.master_table_entries(i).item_name.ToLower(), TableName.ToLower(), StringComparison.Ordinal) = 0 Then
                num = i
                Exit For
            End If
        Next
        If num = -1 Then
            Return False
        End If
        Dim array As String() = Me.master_table_entries(num).sql_statement.Substring(Me.master_table_entries(num).sql_statement.IndexOf("(", StringComparison.Ordinal) + 1).Split(New Char() {","c})
        Dim num5 As Integer = array.Length - 1
        Dim num6 As Integer = 0
        Dim num7 As Integer = num5
        For j As Integer = num6 To num7
            array(j) = array(j).TrimStart(New Char(-1) {})
            Dim num8 As Integer = array(j).IndexOf(" ", StringComparison.Ordinal)
            If num8 > 0 Then
                array(j) = array(j).Substring(0, num8)
            End If
            If array(j).IndexOf("UNIQUE", StringComparison.Ordinal) = 0 Then
                Exit For
            End If
            Me.field_names = CType(Utils.CopyArray(Me.field_names, New String(j + 1 - 1) {}), String())
            Me.field_names(j) = array(j)
        Next
        Return Me.ReadTableFromOffset(CULng(((Me.master_table_entries(num).root_num - 1L) * CLng((CULng(Me.page_size))))))
    End Function

    Private Function ReadTableFromOffset(offset As ULong) As Boolean
        If Me.db_bytes(CInt(offset)) = 13 Then
            Dim num As Decimal = New Decimal(offset)
            Dim num2 As Decimal = New Decimal(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(num, 3D)), 2))
            Dim num3 As Integer = Convert.ToInt32(Decimal.Subtract(num2, 1D))
            Dim num4 As Integer = 0
            If Me.table_entries IsNot Nothing Then
                num4 = Me.table_entries.Length
                Me.table_entries = CType(Utils.CopyArray(Me.table_entries, New SqLiteHandler.table_entry(Me.table_entries.Length + num3 + 1 - 1) {}), SqLiteHandler.table_entry())
            Else
                Me.table_entries = New SqLiteHandler.table_entry(num3 + 1 - 1) {}
            End If
            Dim num5 As Integer = num3
            Dim num6 As Integer = 0
            Dim num7 As Integer = num5
            For i As Integer = num6 To num7
                Dim array As SqLiteHandler.record_header_field() = Nothing
                num2 = New Decimal(offset)
                Dim d As Decimal = Decimal.Add(num2, 8D)
                num = New Decimal(i * 2)
                Dim num8 As ULong = Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(d, num)), 2)
                num2 = New Decimal(offset)
                If Decimal.Compare(num2, 100D) <> 0 Then
                    num8 += offset
                End If
                Dim num9 As Integer = Me.GVL(CInt(num8))
                Dim num10 As Long = Me.CVL(CInt(num8), num9)
                num2 = New Decimal(num8)
                Dim d2 As Decimal = num2
                num = New Decimal(num9)
                Dim d3 As Decimal = num
                Dim num11 As Decimal = New Decimal(num8)
                Dim num12 As Integer = Me.GVL(Convert.ToInt32(Decimal.Add(Decimal.Add(d2, Decimal.Subtract(d3, num11)), 1D)))
                Dim array2 As SqLiteHandler.table_entry() = Me.table_entries
                Dim num13 As Integer = num4 + i
                num11 = New Decimal(num8)
                Dim d4 As Decimal = num11
                num2 = New Decimal(num9)
                Dim d5 As Decimal = num2
                num = New Decimal(num8)
                array2(num13).row_id = Me.CVL(Convert.ToInt32(Decimal.Add(Decimal.Add(d4, Decimal.Subtract(d5, num)), 1D)), num12)
                num11 = New Decimal(num8)
                Dim d6 As Decimal = num11
                num2 = New Decimal(num12)
                Dim d7 As Decimal = num2
                num = New Decimal(num8)
                num8 = Convert.ToUInt64(Decimal.Add(Decimal.Add(d6, Decimal.Subtract(d7, num)), 1D))
                num9 = Me.GVL(CInt(num8))
                num12 = num9
                Dim num14 As Long = Me.CVL(CInt(num8), num9)
                num11 = New Decimal(num8)
                Dim d8 As Decimal = num11
                num2 = New Decimal(num9)
                Dim num15 As Long = Convert.ToInt64(Decimal.Add(Decimal.Subtract(d8, num2), 1D))
                Dim num16 As Integer = 0
                While num15 < num14
                    array = CType(Utils.CopyArray(array, New SqLiteHandler.record_header_field(num16 + 1 - 1) {}), SqLiteHandler.record_header_field())
                    num9 = num12 + 1
                    num12 = Me.GVL(num9)
                    array(num16).type = Me.CVL(num9, num12)
                    If array(num16).type > 9L Then
                        array(num16).size = If(Me.IsOdd(array(num16).type), CLng(Math.Round(Math.Round(CDbl((array(num16).type - 13L)) / 2.0))), CLng(Math.Round(Math.Round(CDbl((array(num16).type - 12L)) / 2.0))))
                    Else
                        array(num16).size = CLng((CULng(Me.SQLDataTypeSize(CInt(array(num16).type)))))
                    End If
                    num15 = num15 + CLng((num12 - num9)) + 1L
                    num16 += 1
                End While
                Me.table_entries(num4 + i).content = New String(array.Length - 1 + 1 - 1) {}
                Dim num17 As Integer = 0
                Dim num18 As Integer = array.Length - 1
                Dim num19 As Integer = 0
                Dim num20 As Integer = num18
                For j As Integer = num19 To num20
                    If array(j).type > 9L Then
                        If Not Me.IsOdd(array(j).type) Then
                            num11 = New Decimal(Me.mEncoding)
                            If Decimal.Compare(num11, 1D) = 0 Then
                                Dim content As String() = Me.table_entries(num4 + i).content
                                Dim num21 As Integer = j
                                Dim [default] As Encoding = Encoding.[Default]
                                Dim bytes As Byte() = Me.db_bytes
                                num2 = New Decimal(num8)
                                Dim d9 As Decimal = num2
                                num = New Decimal(num14)
                                Dim d10 As Decimal = Decimal.Add(d9, num)
                                Dim num22 As Decimal = New Decimal(num17)
                                content(num21) = [default].GetString(bytes, Convert.ToInt32(Decimal.Add(d10, num22)), CInt(array(j).size))
                            Else
                                Dim num22 As Decimal = New Decimal(Me.mEncoding)
                                If Decimal.Compare(num22, 2D) = 0 Then
                                    Dim content2 As String() = Me.table_entries(num4 + i).content
                                    Dim num23 As Integer = j
                                    Dim unicode As Encoding = Encoding.Unicode
                                    Dim bytes2 As Byte() = Me.db_bytes
                                    num11 = New Decimal(num8)
                                    Dim d11 As Decimal = num11
                                    num2 = New Decimal(num14)
                                    Dim d12 As Decimal = Decimal.Add(d11, num2)
                                    num = New Decimal(num17)
                                    content2(num23) = unicode.GetString(bytes2, Convert.ToInt32(Decimal.Add(d12, num)), CInt(array(j).size))
                                Else
                                    num22 = New Decimal(Me.mEncoding)
                                    If Decimal.Compare(num22, 3D) = 0 Then
                                        Dim content3 As String() = Me.table_entries(num4 + i).content
                                        Dim num24 As Integer = j
                                        Dim bigEndianUnicode As Encoding = Encoding.BigEndianUnicode
                                        Dim bytes3 As Byte() = Me.db_bytes
                                        num11 = New Decimal(num8)
                                        Dim d13 As Decimal = num11
                                        num2 = New Decimal(num14)
                                        Dim d14 As Decimal = Decimal.Add(d13, num2)
                                        num = New Decimal(num17)
                                        content3(num24) = bigEndianUnicode.GetString(bytes3, Convert.ToInt32(Decimal.Add(d14, num)), CInt(array(j).size))
                                    End If
                                End If
                            End If
                        Else
                            Dim content4 As String() = Me.table_entries(num4 + i).content
                            Dim num25 As Integer = j
                            Dim default2 As Encoding = Encoding.[Default]
                            Dim bytes4 As Byte() = Me.db_bytes
                            Dim num22 As Decimal = New Decimal(num8)
                            Dim d15 As Decimal = num22
                            num11 = New Decimal(num14)
                            Dim d16 As Decimal = Decimal.Add(d15, num11)
                            num2 = New Decimal(num17)
                            content4(num25) = default2.GetString(bytes4, Convert.ToInt32(Decimal.Add(d16, num2)), CInt(array(j).size))
                        End If
                    Else
                        Dim content5 As String() = Me.table_entries(num4 + i).content
                        Dim num26 As Integer = j
                        Dim num22 As Decimal = New Decimal(num8)
                        Dim d17 As Decimal = num22
                        num11 = New Decimal(num14)
                        Dim d18 As Decimal = Decimal.Add(d17, num11)
                        num2 = New Decimal(num17)
                        content5(num26) = Conversions.ToString(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(d18, num2)), CInt(array(j).size)))
                    End If
                    num17 += CInt(array(j).size)
                Next
            Next
        ElseIf Me.db_bytes(CInt(offset)) = 5 Then
            Dim num22 As Decimal = New Decimal(offset)
            Dim num11 As Decimal = New Decimal(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(num22, 3D)), 2))
            Dim num27 As UShort = Convert.ToUInt16(Decimal.Subtract(num11, 1D))
            Dim num28 As Integer = CInt(num27)
            Dim num29 As Integer = 0
            Dim num30 As Integer = num28
            For k As Integer = num29 To num30
                num22 = New Decimal(offset)
                Dim d19 As Decimal = Decimal.Add(num22, 12D)
                num11 = New Decimal(k * 2)
                Dim num31 As UShort = CUShort(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(d19, num11)), 2))
                num22 = New Decimal(Me.ConvertToInteger(CInt((offset + CULng(num31))), 4))
                Dim d20 As Decimal = Decimal.Subtract(num22, 1D)
                num11 = New Decimal(CInt(Me.page_size))
                Me.ReadTableFromOffset(Convert.ToUInt64(Decimal.Multiply(d20, num11)))
            Next
            num22 = New Decimal(offset)
            num11 = New Decimal(Me.ConvertToInteger(Convert.ToInt32(Decimal.Add(num22, 8D)), 4))
            Dim d21 As Decimal = Decimal.Subtract(num11, 1D)
            Dim num2 As Decimal = New Decimal(CInt(Me.page_size))
            Me.ReadTableFromOffset(Convert.ToUInt64(Decimal.Multiply(d21, num2)))
        End If
        Return True
    End Function

End Class
