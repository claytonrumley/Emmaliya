
Namespace Database
    Public Interface ITable

        Sub Delete(u As Web.IUser)
        Sub Delete(q As IDatabaseQuery, u As Web.IUser)

        Sub Load(primaryKeyValue As Object, u As Web.IUser)
        Sub Load(primaryKeyValue As Object, q As IDatabaseQuery, u As Web.IUser)
        Sub Load(columnName As String, value As Object, u As Web.IUser)
        Sub Load(columnName As String, value As Object, q As IDatabaseQuery, u As Web.IUser)
        Sub Load(d As Dictionary(Of String, Object), u As Web.IUser)
        Sub Load(d As Dictionary(Of String, Object), q As IDatabaseQuery, u As Web.IUser)

        Sub OnAfterDelete(q As IDatabaseQuery, u As Web.IUser)
        Sub OnAfterLoad(q As IDatabaseQuery, u As Web.IUser)
        Sub OnAfterSave(q As IDatabaseQuery, u As Web.IUser)
        Sub OnBeforeDelete(q As IDatabaseQuery, u As Web.IUser)
        Sub OnBeforeLoad(q As IDatabaseQuery, u As Web.IUser)
        Sub OnBeforeSave(q As IDatabaseQuery, u As Web.IUser)
        Sub OnValidate(q As IDatabaseQuery, u As Web.IUser)

        Sub Save(u As Web.IUser)
        Sub Save(q As IDatabaseQuery, u As Web.IUser)

        Sub Validate(u As Web.IUser)
        Sub Validate(q As IDatabaseQuery, u As Web.IUser)

    End Interface
End Namespace