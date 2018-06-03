Imports System.IO
Imports System.Web
Imports System.Web.Routing
Imports Emmaliya.Utilities.Extensions

Namespace Web
    ''' <summary>
    ''' The base class for your applications
    ''' </summary>
    Public MustInherit Class Application(Of U As IUser)
        Inherits HttpApplication

        Public Overridable ReadOnly Property AutoDetectScripts As Boolean = True

        Public Overridable ReadOnly Property AutoDetectStylesheets As Boolean = True

        ''' <summary>
        ''' Provides the client-side link to the emmaliya javascript object
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property EmmaliyaScriptUrl As String = "emmaliya.js"

        Public Shared ReadOnly Property Current As Application(Of U) = Nothing

        Public ReadOnly Property CurrentUser As U

        ''' <summary>
        ''' Returns a list of paths to search for Javascript files when AutoDetectScripts is set to true
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property ScriptPaths As String()
            Get
                Return New String() {"~/Script", "~/Javascript", "~/js", "~/Scripts"}
            End Get
        End Property

        ''' <summary>
        ''' Returns a list of paths to search for CSS files when AutoDetectStylesheets is set to true
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property StylePaths As String()
            Get
                Return New String() {"~/Style", "~/css", "~/Styles", "~/Content"}
            End Get
        End Property

        Private Sub DetectRoutes()
            Dim applicationType As Type = Me.GetType().BaseType.BaseType

            'Add handler to generate emmaliya.js script:
            RouteTable.Routes.MapHttpHandler(Of EmmaliyaScriptHandler)(EmmaliyaScriptUrl)

            For Each t As Type In applicationType.Assembly.GetTypes()
                If t.IsSubclassOfRawGeneric(GetType(Page(Of IUser))) Then
                    Dim routes() As RouteAttribute = t.GetCustomAttributes(GetType(RouteAttribute), True)

                    For Each route As RouteAttribute In routes
                        RouteTable.Routes.MapPageRoute("", route.Url, route.InternalPath)
                    Next

                End If
            Next

        End Sub

        Public Sub Initialize()

            If Current IsNot Nothing Then Return

            _Current = Me

            OnBeforeDetectRoutes(RouteTable.Routes)

            DetectRoutes()

            OnAfterDetectRoutes(RouteTable.Routes)

            _CurrentUser = OnInitializeUser()

            OnInitialize()
        End Sub

        Protected Overridable Sub OnAfterDetectRoutes(ByVal routes As RouteCollection)

        End Sub

        Protected Overridable Sub OnBeforeDetectRoutes(ByVal routes As RouteCollection)

        End Sub

        Protected Overridable Sub OnInitialize()
            'To be handled by children
        End Sub

        Protected MustOverride Function OnInitializeUser() As U

    End Class

End Namespace