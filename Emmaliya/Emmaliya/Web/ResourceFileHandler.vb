Namespace Web
    ''' <summary>
    ''' Internal class for service resources as files to the client
    ''' </summary>
    Friend Class ResourceFileHandler
        Inherits RouteHandler

        Public ReadOnly Property ContentType As String
        Public ReadOnly Property ResourceName As String

        Public Sub New(contentType As String, resourceName As String)
            _ContentType = contentType
            _ResourceName = resourceName
        End Sub

        Public Overrides Sub ProcessRequest()
            Response.ContentType = ContentType

            Response.Write(My.Resources.ResourceManager.GetObject(ResourceName))
        End Sub
    End Class
End Namespace