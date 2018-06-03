Imports System.Reflection

Namespace Web

    <AttributeUsage(AttributeTargets.Class)>
    Public Class AsyncNameAttribute
        Inherits Attribute

        Public ReadOnly Property Name As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="name"></param>
        Public Sub New(name As String)
            _Name = name
        End Sub

        Public Shared Function GetName(o As Object) As String
            Dim ana As AsyncNameAttribute = o.GetType().GetCustomAttribute(Of AsyncNameAttribute)(True)

            If ana IsNot Nothing Then Return ana.Name

            Return ""
        End Function

        Public Shared Function IsAsync(o As Object) As Boolean
            Dim ana As AsyncNameAttribute = o.GetType().GetCustomAttribute(Of AsyncNameAttribute)(True)

            Return ana IsNot Nothing
        End Function
    End Class
End Namespace