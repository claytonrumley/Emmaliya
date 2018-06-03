Namespace Database
    <AttributeUsage(AttributeTargets.Class)>
    Public Class TableAttribute
        Inherits Attribute

        Public ReadOnly Property Name As String

        Public Sub New(name As String)
            _Name = name
        End Sub
    End Class
End Namespace