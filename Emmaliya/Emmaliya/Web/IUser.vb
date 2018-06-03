Namespace Web
    Public Interface IUser

        ''' <summary>
        ''' Returns true if this user is a member of the given role
        ''' </summary>
        ''' <param name="roleName">The name of the role to test</param>
        ''' <returns></returns>
        Function MemberOf(roleName As String) As Boolean

        ''' <summary>
        ''' Returns true if this user is a member of any of the given roles
        ''' </summary>
        ''' <param name="roleNames">The names of the roles to test</param>
        ''' <returns></returns>
        Function MemberOf(ParamArray roleNames() As String) As Boolean


    End Interface
End Namespace