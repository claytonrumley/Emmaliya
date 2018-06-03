Imports System.Web
Imports System.Web.Routing

Namespace Web

    ''' <summary>
    ''' A wrapper class for IHttpHandler that exposes several useful properties
    ''' </summary>
    Public MustInherit Class RouteHandler
        Implements IHttpHandler

        Private MyContext As HttpContext

        ''' <summary>
        ''' Override and set to False in derived classes that should not check the Securable tables in Core to determine access permissions
        ''' </summary>
        ''' <returns></returns>
        Protected Overridable ReadOnly Property CheckRouteSecurity As Boolean
            Get
                Return True
            End Get
        End Property

        Protected ReadOnly Property Context As HttpContext
            Get
                Return MyContext
            End Get
        End Property

        ''' <summary>
        ''' An object representing the currently logged-in user
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CurrentUser As IUser
            Get
                Return CurrentApplication.CurrentUser
            End Get
        End Property

        Public ReadOnly Property CurrentApplication As Application(Of IUser)
            Get
                Return Application(Of IUser).Current()
            End Get
        End Property

        Protected ReadOnly Property Request As HttpRequest
            Get
                Return Context.Request
            End Get
        End Property

        Protected ReadOnly Property Response As HttpResponse
            Get
                Return Context.Response
            End Get
        End Property


        Protected ReadOnly Property RouteData As RouteData
            Get
                Return Context.Request.RequestContext.RouteData
            End Get
        End Property

        Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
            Dim hasAccess As Boolean = True

            MyContext = context

            If hasAccess Then
                ProcessRequest()
            Else
                Response.StatusCode = 403
                Response.Write("Access Denied. Ensure you are logged in and have permission to access this function.")
            End If
        End Sub

        Public MustOverride Sub ProcessRequest()
    End Class

End Namespace