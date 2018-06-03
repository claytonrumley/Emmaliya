Namespace Database

    ''' <summary>
    ''' An interface for providing generic database connectivity functions
    ''' </summary>
    Public Interface IDatabaseQuery

#Region "Properties"
        ReadOnly Property CurrentReader As IDataReader

        Property IsPaged As Boolean

        ''' <summary>
        ''' Returns the value for the ordinal column in the current row of the SqlDataReader
        ''' </summary>
        ''' <remarks>
        ''' Will return the value of the referenced column in the current row of the SqlDataReader.
        ''' Can be used as q.Item(1) will return the value of the first column in the current reader.
        ''' Alternatively q(1) can be used because this is the default property for the class.
        ''' </remarks>
        ''' <param name="i">The index of the column whose value to return</param>
        Default ReadOnly Property Item(i As Integer) As Object

        ''' <summary>
        ''' Returns the value for the named column in the current row of the SqlDataReader
        ''' </summary>
        ''' <remarks>
        ''' Will return the value of the referenced column in the current row of the SqlDataReader.
        ''' Can be used as q.Item("LastModifiedDate") will return the value of the column named.
        ''' LastModifiedDate in the current reader. Alternatively q(1) can be used because this
        ''' is the default property for the class.
        ''' </remarks>
        ''' <param name="name">The name of the column whose value to return</param>
        Default ReadOnly Property Item(name As String) As Object

        Property PageSize As Integer

        Property PageStart As Integer

        ReadOnly Property Parameter(name As String) As IDbDataParameter

        Property Sql As String

        ReadOnly Property TotalRecords As Integer

        ReadOnly Property TotalPages As Integer

        ReadOnly Property Transaction As IDbTransaction

#End Region

        Sub AddParameter(name As String, value As Object)
        Sub AddParameter(parameter As IDbDataParameter)

        Sub BeginTransaction()

        Sub Close()

        Sub Commit()

        ''' <summary>
        ''' Closes the current reader for this query, if it is open.
        ''' </summary>
        Sub CloseReader()

        ''' <summary>
        ''' Executes the query returning the results in a DataSet
        ''' </summary>
        ''' <returns></returns>
        Function ExecuteDataSet() As DataSet

        ''' <summary>
        ''' Executes the query returning a JSON string of the results
        ''' </summary>
        ''' <returns></returns>
        Function ExecuteJSON() As String

        ''' <summary>
        ''' Executes the query and returns the first column of the result set as a list of the given type X
        ''' </summary>
        ''' <typeparam name="X"></typeparam>
        ''' <returns></returns>
        Function ExecuteList(Of X)() As List(Of X)

        ''' <summary>
        ''' Executes a non-query SQL statement, returning the number of rows affected.
        ''' </summary>
        ''' <returns>The number of rows affected by the query.</returns>
        Function ExecuteNonQuery() As Integer

        ''' <summary>
        ''' Executes the query and initalizes the CurrentReader
        ''' </summary>
        Sub ExecuteReader()

        Function ExecuteScalar() As Object

        Function ExecuteScalar(defaultValue As Object) As Object

        Function ExecuteScalar(Of X)() As X

        Function ExecuteScalar(Of X)(defaultValue As X) As X

        Function ExecuteXml() As XmlResult

        ''' <summary>
        ''' Returns the last identity value inserted by this query.
        ''' </summary>
        ''' <returns></returns>
        Function LatestId() As Integer

        ''' <summary>
        ''' Rolls back the current transaction, if there is one.
        ''' </summary>
        Sub Rollback()

    End Interface

End Namespace