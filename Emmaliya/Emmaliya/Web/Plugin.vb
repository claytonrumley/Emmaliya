Imports System.Web.Routing

Namespace Web
    Public MustInherit Class Plugin(Of U As IUser)

        Protected Friend Overridable Sub OnAfterDetectRoutes(ByVal routes As RouteCollection)

        End Sub

        Protected Friend Overridable Sub OnBeforeDetectRoutes(ByVal routes As RouteCollection)

        End Sub

        Protected Friend Overridable Sub OnInitialize()

        End Sub

        Protected Friend Overridable Sub OnPageLoad(p As Page(Of U))

        End Sub
    End Class
End Namespace