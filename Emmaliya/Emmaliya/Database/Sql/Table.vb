Imports Emmaliya.Web

Namespace Database.Sql
    Public Class Table
        Implements ITable

#Region "Delete"
        Public Sub Delete(u As IUser) Implements ITable.Delete
            Throw New NotImplementedException()
        End Sub

        Public Sub Delete(q As DatabaseQuery, u As IUser)
            Throw New NotImplementedException()
        End Sub

        Private Sub ITable_Delete(q As IDatabaseQuery, u As IUser) Implements ITable.Delete
            Delete(q, u)
        End Sub
#End Region

#Region "GetTableName"
        ''' <summary>
        ''' Returns the name of the table (either from the class name itself or the TableAttribute, if one exists)
        ''' </summary>
        ''' <returns></returns>
        Protected Function GetTableName() As String
            Dim name As String = Me.GetType().Name
            Dim attr() As TableAttribute = Me.GetType().GetCustomAttributes(GetType(TableAttribute), True)

            If attr IsNot Nothing AndAlso attr.Any Then name = attr.First.Name

            Return name
        End Function
#End Region

#Region "Load"
        Public Sub Load(primaryKeyValue As Object, u As IUser) Implements ITable.Load
            Throw New NotImplementedException()
        End Sub

        Public Sub Load(primaryKeyValue As Object, q As DatabaseQuery, u As IUser)
            Throw New NotImplementedException()
        End Sub

        Private Sub ITable_Load(primaryKeyValue As Object, q As IDatabaseQuery, u As IUser) Implements ITable.Load
            Load(primaryKeyValue, q, u)
        End Sub

        Public Sub Load(columnName As String, value As Object, u As IUser) Implements ITable.Load
            Throw New NotImplementedException()
        End Sub

        Public Sub Load(columnName As String, value As Object, q As DatabaseQuery, u As IUser)
            Throw New NotImplementedException()
        End Sub

        Private Sub ITable_Load(columnName As String, value As Object, q As IDatabaseQuery, u As IUser) Implements ITable.Load
            Load(columnName, value, q, u)
        End Sub

        Public Sub Load(d As Dictionary(Of String, Object), u As IUser) Implements ITable.Load
            Throw New NotImplementedException()
        End Sub

        Public Sub Load(d As Dictionary(Of String, Object), q As DatabaseQuery, u As IUser)

        End Sub

        Private Sub ITable_Load(d As Dictionary(Of String, Object), q As IDatabaseQuery, u As IUser) Implements ITable.Load
            Load(d, CType(q, DatabaseQuery), u)
        End Sub
#End Region

#Region "OnAfterDelete"
        Protected Overridable Sub OnAfterDelete(q As DatabaseQuery, u As IUser)
            'To be overridden in derived classes
        End Sub

        Private Sub ITable_OnAfterDelete(q As IDatabaseQuery, u As IUser) Implements ITable.OnAfterDelete
            OnAfterDelete(q, u)
        End Sub
#End Region

#Region "OnAfterLoad"
        Protected Overridable Sub OnAfterLoad(q As DatabaseQuery, u As IUser)
            'To be overridden in derived classes
        End Sub

        Private Sub ITable_OnAfterLoad(q As IDatabaseQuery, u As IUser) Implements ITable.OnAfterLoad
            OnAfterLoad(q, u)
        End Sub
#End Region

#Region "OnAfterSave"
        Protected Overridable Sub OnAfterSave(q As DatabaseQuery, u As IUser)
            'To be overridden in derived classes
        End Sub

        Private Sub ITable_OnAfterSave(q As IDatabaseQuery, u As IUser) Implements ITable.OnAfterSave
            OnAfterSave(q, u)
        End Sub
#End Region

#Region "OnBeforeDelete"
        Protected Overridable Sub OnBeforeDelete(q As DatabaseQuery, u As IUser)
            'To be overridden in derived classes
        End Sub

        Private Sub ITable_OnBeforeDelete(q As IDatabaseQuery, u As IUser) Implements ITable.OnBeforeDelete
            OnBeforeDelete(q, u)
        End Sub
#End Region

#Region "OnBeforeLoad"
        Protected Overridable Sub OnBeforeLoad(q As IDatabaseQuery, u As IUser) Implements ITable.OnBeforeLoad
            'To be overridden in derived classes
        End Sub
#End Region

#Region "OnBeforeSave"
        Protected Overridable Sub OnBeforeSave(q As DatabaseQuery, u As IUser)
            'To be overridden in derived classes
        End Sub

        Private Sub ITable_OnBeforeSave(q As IDatabaseQuery, u As IUser) Implements ITable.OnBeforeSave
            OnBeforeSave(q, u)
        End Sub
#End Region

#Region "OnValidate"
        Protected Overridable Sub OnValidate(q As DatabaseQuery, u As IUser)
            'To be overridden in derived classes
        End Sub

        Private Sub ITable_OnValidate(q As IDatabaseQuery, u As IUser) Implements ITable.OnValidate
            OnValidate(q, u)
        End Sub
#End Region

#Region "Save"
        Public Sub Save(u As IUser) Implements ITable.Save
            Throw New NotImplementedException()
        End Sub

        Public Sub Save(q As DatabaseQuery, u As IUser)
            Throw New NotImplementedException()
        End Sub

        Private Sub ITable_Save(q As IDatabaseQuery, u As IUser) Implements ITable.Save
            Save(q, u)
        End Sub
#End Region

#Region "Validate"
        Public Sub Validate(u As IUser) Implements ITable.Validate
            Throw New NotImplementedException()
        End Sub

        Public Sub Validate(q As DatabaseQuery, u As IUser)
            'Let derived classes handle this:
            OnValidate(q, u)
        End Sub

        Private Sub ITable_Validate(q As IDatabaseQuery, u As IUser) Implements ITable.Validate
            Validate(q, u)
        End Sub
#End Region

    End Class
End Namespace