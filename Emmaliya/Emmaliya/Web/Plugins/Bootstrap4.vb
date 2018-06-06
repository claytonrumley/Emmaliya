Imports System.Web.Routing
Imports System.Web.UI

Namespace Web.Plugins
    Public Class Bootstrap4(Of U As IUser)
        Inherits Plugin(Of U)

        ' need to add js and css files
        ' need to be able to specify route and handler

        Protected Friend Overrides Sub OnBeforeDetectRoutes(routes As RouteCollection)
            routes.MapHttpHandler("bootstrap4.emmaliya.js", New ResourceFileHandler("application/javascript", "bootstrap4_emmaliya"))
            routes.MapHttpHandler("bootstrap4.emmaliya.css", New ResourceFileHandler("text/css", "bootstrap4_emmaliya_css"))

            MyBase.OnBeforeDetectRoutes(routes)
        End Sub

        Protected Friend Overrides Sub OnPageLoad(p As Page(Of U))
            p.AddScript(ClientResource.FromUrl("bootstrap4.emmaliya.js"))
            p.AddStyle(ClientResource.FromUrl("bootstrap4.emmaliya.css"))

        End Sub
    End Class
End Namespace
