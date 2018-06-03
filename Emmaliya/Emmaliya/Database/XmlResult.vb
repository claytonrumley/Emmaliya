
Namespace Database
    Public Class XmlResult

        Public Sub New(el As XElement)

        End Sub

        Public Sub New(xml As String)
            Me.New(XElement.Parse(xml))
        End Sub

    End Class
End Namespace