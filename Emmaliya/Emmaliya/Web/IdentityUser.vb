Imports System.Web

Namespace Web
    ''' <summary>
    ''' A simple wrapper class around the HttpContext.Current.User object for integration with Emmaliya sites
    ''' </summary>
    Public Class IdentityUser
        Implements IUser

        ''' <summary>
        ''' Gets a value that indicates whether the user has been authenticated
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsAuthenticated As Boolean
            Get
                Return HttpContext.Current.User.Identity.IsAuthenticated
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the current user
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Name As String
            Get
                Return HttpContext.Current.User.Identity.Name
            End Get
        End Property

        ''' <summary>
        ''' Returns true if this user is a member of the given role name.
        ''' </summary>
        ''' <param name="roleName"></param>
        ''' <returns></returns>
        Public Function MemberOf(roleName As String) As Boolean Implements IUser.MemberOf
            Return HttpContext.Current.User.IsInRole(roleName)
        End Function

        ''' <summary>
        ''' Returns true if this user is a member of any of the given role names.
        ''' </summary>
        ''' <param name="roleNames"></param>
        ''' <returns></returns>
        Public Function MemberOf(ParamArray roleNames() As String) As Boolean Implements IUser.MemberOf
            For Each roleName As String In roleNames
                If HttpContext.Current.User.IsInRole(roleName) Then Return True
            Next

            Return False
        End Function
    End Class
End Namespace