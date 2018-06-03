
Namespace Web
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=True)>
    Public Class AddScriptAttribute
        Inherits Attribute

        Public ReadOnly Property ScriptPath As String

        Public Sub New(scriptPath As String)
            _ScriptPath = scriptPath
        End Sub
    End Class
End Namespace

