Namespace Web
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=True)>
    Public Class RouteAttribute
        Inherits Attribute

        Public ReadOnly Property InternalPath As String
        Public ReadOnly Property Url As String

        Public Sub New(url As String, internalPath As String)
            _Url = url
            _InternalPath = internalPath
        End Sub
    End Class
End Namespace