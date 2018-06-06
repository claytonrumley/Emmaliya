Namespace Web
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=True)>
    Public Class AddStylesheetAttribute
        Inherits Attribute

        Public ReadOnly Property StylesheetPath As String

        Public Sub New(stylesheetPath As String)
            _StylesheetPath = stylesheetPath
        End Sub
    End Class
End Namespace


