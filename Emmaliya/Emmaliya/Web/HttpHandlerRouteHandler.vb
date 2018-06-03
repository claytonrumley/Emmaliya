Imports Microsoft.VisualBasic
Imports System.Web.Routing
Imports System.Runtime.CompilerServices

Namespace Web
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>This code was taken from http://haacked.com/archive/2009/11/04/routehandler-for-http-handlers.aspx. </remarks>
    Public Class HttpHandlerRouteHandler(Of THandler As New)
        Implements IRouteHandler

        Public Function GetHttpHandler(ByVal requestContext As System.Web.Routing.RequestContext) As System.Web.IHttpHandler Implements System.Web.Routing.IRouteHandler.GetHttpHandler
            Return New THandler
        End Function
    End Class

    Public Module HttpHandlerExtensions
        <Extension()>
        Public Sub MapHttpHandler(Of THandler As New)(ByVal routes As RouteCollection, ByVal url As String)
            routes.MapHttpHandler(Of THandler)(Nothing, url, Nothing, Nothing)
        End Sub
        '...
        <Extension()>
        Public Sub MapHttpHandler(Of THandler As New)(ByVal routes As RouteCollection, ByVal name As String, ByVal url As String, ByVal defaults As Object, ByVal constraints As Object)
            Dim route = New Route(url, New HttpHandlerRouteHandler(Of THandler)())
            route.Defaults = New RouteValueDictionary(defaults)
            route.Constraints = New RouteValueDictionary(constraints)
            routes.Add(name, route)
        End Sub
    End Module
End Namespace