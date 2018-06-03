Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Reflection
Imports System.Xml

Namespace Database.Sql

    ''' <summary>
    ''' A powerful wrapper class for Sql Server database querying
    ''' </summary>
    Public Class DatabaseQuery
        Implements IDatabaseQuery

#Region "Private Members"
        Private _Connection As SqlConnection
        Private _Parameters As New Dictionary(Of String, SqlParameter)
#End Region

#Region "Properties"
        Public ReadOnly Property CurrentReader As SqlDataReader

        Private ReadOnly Property IDatabaseQuery_CurrentReader As IDataReader Implements IDatabaseQuery.CurrentReader
            Get
                Return CurrentReader
            End Get
        End Property

        Public Property IsPaged As Boolean = False Implements IDatabaseQuery.IsPaged

        Default Public ReadOnly Property Item(i As Integer) As Object Implements IDatabaseQuery.Item
            Get
                If CurrentReader Is Nothing OrElse CurrentReader.IsClosed Then Return Nothing
                Return CurrentReader(i)
            End Get
        End Property

        Default Public ReadOnly Property Item(name As String) As Object Implements IDatabaseQuery.Item
            Get
                If CurrentReader Is Nothing OrElse CurrentReader.IsClosed Then Return Nothing
                Return CurrentReader(name)
            End Get
        End Property

        Public Property PageSize As Integer Implements IDatabaseQuery.PageSize

        Public Property PageStart As Integer Implements IDatabaseQuery.PageStart

        Public ReadOnly Property Parameter(name As String) As SqlParameter
            Get
                Return _Parameters(name)
            End Get
        End Property

        Private ReadOnly Property IDatabaseQuery_Parameter(name As String) As IDbDataParameter Implements IDatabaseQuery.Parameter
            Get
                Return Parameter(name)
            End Get
        End Property

        Public Property Sql As String Implements IDatabaseQuery.Sql

        Public ReadOnly Property TotalRecords As Integer Implements IDatabaseQuery.TotalRecords

        Public ReadOnly Property TotalPages As Integer Implements IDatabaseQuery.TotalPages

        Public ReadOnly Property Transaction As SqlTransaction = Nothing
        Private ReadOnly Property IDatabaseQuery_Transaction As IDbTransaction Implements IDatabaseQuery.Transaction
            Get
                Return Transaction
            End Get
        End Property
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructs a new instance of DatabaseQuery, using the ConnectionString
        ''' </summary>
        Public Sub New()
            Me.New(ConnectionStrings.Item("DefaultConnection").ConnectionString)
        End Sub

        Public Sub New(connectionString As String)
            _Connection = New SqlConnection(connectionString)
        End Sub
#End Region

#Region "AddParameter"

        ''' <summary>
        ''' Adds the given name and value as parameter objects to this DatabaseQuery's parameter collection.
        ''' </summary>
        ''' <param name="name">The name of this parameter</param>
        ''' <param name="value">The value of this parameter</param>
        Public Sub AddParameter(name As String, value As Object) Implements IDatabaseQuery.AddParameter
            If value Is Nothing Then value = DBNull.Value

            AddParameter(New SqlParameter(name, value))
        End Sub

        ''' <summary>
        ''' Adds the given SqlParameter to this DatabaseQuery's parameter collection.
        ''' </summary>
        ''' <param name="parameter"></param>
        Public Sub AddParameter(parameter As SqlParameter)
            _Parameters.Add(parameter.ParameterName, parameter)
        End Sub

        Private Sub AddParameter(parameter As IDbDataParameter) Implements IDatabaseQuery.AddParameter
            _Parameters.Add(parameter.ParameterName, parameter)
        End Sub
#End Region

#Region "BeginTransaction"
        ''' <summary>
        ''' Begins a transaction for this DatabaseQuery
        ''' </summary>
        Public Sub BeginTransaction() Implements IDatabaseQuery.BeginTransaction
            If Transaction IsNot Nothing Then Return

            EnsureConnected()

            _Transaction = _Connection.BeginTransaction()
        End Sub
#End Region

#Region "Close"
        ''' <summary>
        ''' Closes any open connection and data reader on this DatabaseQuery object
        ''' </summary>
        Public Sub Close() Implements IDatabaseQuery.Close
            'Close the open reader:
            If CurrentReader IsNot Nothing AndAlso Not CurrentReader.IsClosed Then CurrentReader.Close()

            'Close the database connection:
            _Connection.Close()
        End Sub
#End Region

#Region "CloseReader"
        ''' <summary>
        ''' Closes the DataReader object associated with this DatabaseQuery
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CloseReader() Implements IDatabaseQuery.CloseReader
            'Close the open reader:
            If CurrentReader IsNot Nothing AndAlso Not CurrentReader.IsClosed Then
                CurrentReader.Close()
                _CurrentReader = Nothing
            End If
        End Sub
#End Region

#Region "Commit"
        ''' <summary>
        ''' Commits the transaction started by <code>BeginTransaction</code>
        ''' </summary>
        Public Sub Commit() Implements IDatabaseQuery.Commit
            If Transaction Is Nothing Then Return

            Transaction.Commit()
        End Sub
#End Region

#Region "EnsureConnected"
        ''' <summary>
        ''' Ensures that the SqlConnection object is connected to the client
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub EnsureConnected()
            If _Connection.State <> Data.ConnectionState.Closed AndAlso _Connection.State <> Data.ConnectionState.Broken Then Return
            _Connection.Open()

            'Close the reader if it's open:
            If CurrentReader IsNot Nothing AndAlso Not CurrentReader.IsClosed Then
                CurrentReader.Close()
            End If
        End Sub
#End Region

#Region "ExecuteDataSet"
        ''' <summary>
        ''' Executes the query returning the results in a DataSet.
        ''' </summary>
        ''' <returns></returns>
        Public Function ExecuteDataSet() As DataSet Implements IDatabaseQuery.ExecuteDataSet
            EnsureConnected()

            Dim ds As New DataSet
            Dim cmd As SqlCommand = GetCommand()
            Dim adapter As New SqlDataAdapter(cmd)

            adapter.Fill(ds)

            If IsPaged Then
                _TotalRecords = cmd.Parameters("@__total_records").Value
            End If

            Return ds
        End Function
#End Region

#Region "ExecuteJSON"
        ''' <summary>
        ''' Executes the query returning a JSON string of the results
        ''' </summary>
        ''' <returns></returns>
        Public Function ExecuteJSON() As String Implements IDatabaseQuery.ExecuteJSON
            Dim json As New System.Web.Script.Serialization.JavaScriptSerializer
            Dim items As New List(Of Dictionary(Of String, Object))

            ExecuteReader()

            While CurrentReader.Read()
                Dim dict As New Dictionary(Of String, Object)

                For f As Integer = 0 To CurrentReader.FieldCount - 1
                    Dim val As Object = CurrentReader(f)

                    If IsDBNull(val) Then val = Nothing

                    'If val IsNot Nothing AndAlso JSONFormatting IsNot Nothing AndAlso JSONFormatting.ContainsKey(CurrentReader.GetName(f)) Then
                    '    val = String.Format("{0:" & JSONFormatting(CurrentReader.GetName(f)) & "}", val)
                    'End If

                    dict.Add(CurrentReader.GetName(f), val)
                Next

                items.Add(dict)
            End While

            CloseReader()

            If items.Count = 1 Then
                Return json.Serialize(items(0))
            End If

            Return json.Serialize(items)
        End Function
#End Region

#Region "ExecuteList"
        ''' <summary>
        ''' Executes the query and returns result set as a list of the given type X. If X is a ValueType, only the first column is returned, otherwise a new instance of X is created and its members are set to the columns with matching names.
        ''' </summary>
        ''' <typeparam name="X"></typeparam>
        ''' <returns></returns>
        Public Function ExecuteList(Of X)() As List(Of X) Implements IDatabaseQuery.ExecuteList
            Dim l As New List(Of X)

            ExecuteReader()

            While CurrentReader.Read
                If GetType(X).IsValueType Then
                    l.Add(CurrentReader(0))
                Else
                    Dim item As X = Activator.CreateInstance(Of X)()

                    For f As Integer = 0 To CurrentReader.FieldCount - 1
                        Dim members As MemberInfo() = GetType(X).GetMember(CurrentReader.GetName(f))
                        Dim val As Object = CurrentReader(f)

                        If IsDBNull(val) Then val = Nothing

                        For Each member As MemberInfo In members
                            Select Case member.MemberType
                                Case MemberTypes.Field
                                    CType(member, FieldInfo).SetValue(item, val)

                                Case MemberTypes.Property
                                    CType(member, PropertyInfo).SetValue(item, val)
                            End Select
                        Next

                    Next

                    l.Add(item)
                End If
            End While

            CurrentReader.Close()

            Return l
        End Function
#End Region

#Region "ExecuteNonQuery"
        ''' <summary>
        ''' Executes a non-query SQL statement, returning the number of rows affected.
        ''' </summary>
        ''' <returns>The number of rows affected by the query.</returns>
        ''' <remarks></remarks>
        Public Function ExecuteNonQuery() As Integer Implements IDatabaseQuery.ExecuteNonQuery
            EnsureConnected()

            Dim cmd As SqlCommand = GetCommand()
            Dim affectedRows As Integer

            affectedRows = cmd.ExecuteNonQuery

            If IsPaged Then
                _TotalRecords = cmd.Parameters("@__total_records").Value
            End If

            Return affectedRows
        End Function
#End Region

#Region "ExecuteReader"
        ''' <summary>
        ''' Executes the query and initalizes the CurrentReader
        ''' </summary>
        Public Sub ExecuteReader() Implements IDatabaseQuery.ExecuteReader
            EnsureConnected()

            Dim cmd As SqlCommand = GetCommand()

            If CurrentReader IsNot Nothing AndAlso Not CurrentReader.IsClosed Then
                CloseReader()

            End If

            _CurrentReader = cmd.ExecuteReader

            If IsPaged Then
                _TotalRecords = cmd.Parameters("@__total_records").Value
            End If
        End Sub
#End Region

#Region "ExecuteScalar"
        Public Function ExecuteScalar() As Object Implements IDatabaseQuery.ExecuteScalar
            Return ExecuteScalar(Nothing)
        End Function

        Public Function ExecuteScalar(defaultValue As Object) As Object Implements IDatabaseQuery.ExecuteScalar
            EnsureConnected()

            Dim cmd As SqlCommand = GetCommand()
            Dim result As Object

            EnsureConnected()

            result = cmd.ExecuteScalar

            If IsPaged Then
                _TotalRecords = cmd.Parameters("@__total_records").Value
            End If

            'If the result was NULL, set the return variable to the default value that was passed in
            If IsDBNull(result) Then
                result = defaultValue
            End If

            Return result
        End Function

        Public Function ExecuteScalar(Of X)() As X Implements IDatabaseQuery.ExecuteScalar
            ExecuteScalar(Of X)(Nothing)
        End Function

        Public Function ExecuteScalar(Of X)(defaultValue As X) As X Implements IDatabaseQuery.ExecuteScalar
            Dim obj As Object = ExecuteScalar()

            If IsDBNull(obj) Then Return Nothing

            Return CType(obj, X)
        End Function
#End Region

#Region "ExecuteXml"
        Public Function ExecuteXml() As XmlResult Implements IDatabaseQuery.ExecuteXml
            EnsureConnected()

            Dim command As SqlCommand = GetCommand()
            Dim results As XElement = Nothing
            Dim reader As XmlReader

            Try
                reader = command.ExecuteXmlReader()

                results = XElement.Load(reader)

                reader.Close()

                If IsPaged Then
                    _TotalRecords = command.Parameters("@__total_records").Value
                End If

                'XmlFormatting.Apply(results)
            Catch ex As InvalidOperationException
                'Do nothing (no result from server)
                ' Throw New Exception(ex.GetType().ToString() & " : " & ex.Message, ex)
            Catch ex As Exception
                'No result (or Bad XML) sent by the server
                Throw New Exception(ex.Message, ex)
            End Try

            Return New XmlResult(results)
        End Function
#End Region

#Region "GetCommand"
        ''' <summary>
        ''' Returns a command object initialized with all parameters and sql
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetCommand() As SqlCommand
            Dim cmd As SqlCommand = _Connection.CreateCommand()

            For Each value As SqlParameter In _Parameters.Values
                cmd.Parameters.Add(value)
            Next

            If IsPaged Then
                Dim p As New SqlParameter("@__total_records", SqlDbType.Int)

                p.Direction = ParameterDirection.Output

                cmd.Parameters.Add(p)
            End If

            cmd.Transaction = Transaction
            cmd.CommandText = GetFormattedSql()

            Return cmd
        End Function
#End Region

#Region "GetFormattedSql"
        Private Function GetFormattedSql() As String
            'If we don't have paging turned on, just return the SQL:
            If Not IsPaged Or (Sql.ToUpper.IndexOf("SELECT ") = -1 And Sql.ToUpper.IndexOf("FROM ") = -1) Then
                Return Sql
            End If

            'Find the from:
            Dim i As Integer = 0
            Dim bracketCount As Integer = 0
            Dim fromIndex As Integer = 0
            Dim foundFrom As Boolean = False
            Dim searchIndex As Integer = 0

            If Sql.ToLower().Contains("-- paged query") Then
                searchIndex = Sql.ToLower().IndexOf("-- paged query") + "-- paged query".Length
                i = searchIndex
            End If

            For ci As Integer = searchIndex To Sql.Length - 1 'c As Char In Sql
                Dim c As Char = Sql(ci)

                If c = "(" Then
                    fromIndex = 0
                    bracketCount += 1
                ElseIf c = ")" Then
                    fromIndex = 0
                    bracketCount -= 1
                ElseIf bracketCount = 0 Then
                    Select Case fromIndex
                        Case 0
                            If Char.ToUpper(c) = "F" Then fromIndex += 1
                        Case 1
                            If Char.ToUpper(c) = "R" Then
                                fromIndex += 1
                            ElseIf Char.ToUpper(c) = "F" Then
                                fromIndex = 1
                            Else
                                fromIndex = 0
                            End If
                        Case 2
                            If Char.ToUpper(c) = "O" Then
                                fromIndex += 1
                            ElseIf Char.ToUpper(c) = "F" Then
                                fromIndex = 1
                            Else
                                fromIndex = 0
                            End If
                        Case 3
                            If Char.ToUpper(c) = "M" Then
                                fromIndex += 1
                                foundFrom = True
                                Exit For
                            ElseIf Char.ToUpper(c) = "F" Then
                                fromIndex = 1
                            Else
                                fromIndex = 0
                            End If
                    End Select
                End If

                i += 1
            Next

            If Not foundFrom Then Throw New Exception("Could not find FROM -> possibly invalid SQL statement")

            Dim from As Integer = i - 3

            Dim selectIndex As Integer = Sql.ToLower().IndexOf("select", searchIndex)
            Dim orderIndex As Integer = Sql.ToLower().LastIndexOf("order by ")
            Dim forXmlIndex As Integer = Sql.ToLower().LastIndexOf("for xml ")

            If orderIndex < 0 Then Throw New Exception("You must provide an order-by statement for paging (" & searchIndex & ", " & from & ", " & selectIndex & ")")

            Dim orderLength As Integer = Sql.Length - orderIndex

            If forXmlIndex >= 0 AndAlso forXmlIndex > orderIndex Then
                orderLength = forXmlIndex - orderIndex
            End If

            Dim newSql As String = ""

            If searchIndex > 0 Then
                newSql = Sql.Substring(0, searchIndex - "-- paged query".Length) & " "
            End If

            newSql &= Sql.Substring(selectIndex, from - selectIndex) &
                      " INTO #Pager " &
                      vbNewLine &
            Sql.Substring(from, orderIndex - from) &
                      " select @__total_records = @@ROWCOUNT" &
                      " select * from #Pager " &
                      "  " &
            Sql.Substring(orderIndex, orderLength) &
                      " OFFSET(" & PageStart & " - 1) * " & PageSize & " ROWS FETCH NEXT " & PageSize & " ROWS ONLY "


            If forXmlIndex >= 0 AndAlso forXmlIndex > orderIndex Then
                newSql &= " " & Sql.Substring(forXmlIndex)
            End If

            Return newSql
        End Function
#End Region

#Region "LatestId"
        ''' <summary>
        ''' Returns the value of @@IDENTITY
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LatestId() As Integer Implements IDatabaseQuery.LatestId
            Dim command As SqlCommand = _Connection.CreateCommand()

            command.Transaction = Transaction
            command.CommandText = "SELECT @@IDENTITY"

            Return command.ExecuteScalar()
        End Function
#End Region

#Region "Rollback"
        ''' <summary>
        ''' Rolls back the transaction
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Rollback() Implements IDatabaseQuery.Rollback
            If Transaction Is Nothing Then Return

            Transaction.Rollback()
            _Transaction = Nothing
        End Sub
#End Region

    End Class

End Namespace