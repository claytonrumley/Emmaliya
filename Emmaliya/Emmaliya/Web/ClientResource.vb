Namespace Web
    ''' <summary>
    ''' Used to populate the list of client-side scripts and stylesheets (or inline scripts and styles) to be added to the page.
    ''' </summary>
    Public Class ClientResource

#Region "Properties"
        Protected Friend Property AddedIndex As Integer

        Public ReadOnly Property Priority As Integer

        Public ReadOnly Property ValueIsUrl As Boolean

        Public ReadOnly Property Value As String
#End Region

#Region "Constructor"
        Private Sub New()

        End Sub
#End Region

#Region "FromUrl"
        ''' <summary>
        ''' Adds the resource as a link to an external url
        ''' </summary>
        ''' <param name="url">The url of the resource</param>
        ''' <param name="priority">An optional priority to ensure load order is followed</param>
        ''' <returns></returns>
        Public Shared Function FromUrl(url As String, Optional priority As Boolean = 100) As ClientResource
            Dim cr As New ClientResource

            cr._Priority = priority
            cr._Value = url
            cr._ValueIsUrl = True

            Return cr
        End Function
#End Region

#Region "FromContents"
        ''' <summary>
        ''' Adds the resource as inline content
        ''' </summary>
        ''' <param name="value">The content to be included inline</param>
        ''' <param name="priority">An optional priority to ensure load order is followed</param>
        ''' <returns></returns>
        Public Shared Function FromContents(value As String, Optional priority As Boolean = 100) As ClientResource
            Dim cr As New ClientResource

            cr._Priority = priority
            cr._Value = value
            cr._ValueIsUrl = False

            Return cr
        End Function
#End Region

    End Class
End Namespace