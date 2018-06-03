Imports System.Reflection

''' <summary>
''' Defines the roles that will have permission to access this resource
''' </summary>
<AttributeUsage(AttributeTargets.Class Or AttributeTargets.Property)>
    Public Class AccessAttribute
        Inherits Attribute

    Private _Roles As New List(Of String)
    Public ReadOnly Property Roles As String()
        Get
            Return _Roles.ToArray()
        End Get
    End Property

    Public Sub New(ParamArray roles() As String)
        _Roles.AddRange(roles)
    End Sub

    ''' <summary>
    ''' Checks the given object for Access attributes, returning true if there are none or if the given user is a member of one of the roles in the Access object
    ''' </summary>
    ''' <param name="o">The object to test for access</param>
    ''' <param name="u">The user whose access is being tested</param>
    ''' <returns>True if the user has access to this object, false otherwise.</returns>
    Public Shared Function HasAccessTo(o As Object, u As Web.IUser)
        Dim hasAccess As Boolean = True
        Dim accessList() As AccessAttribute = o.GetType().GetCustomAttributes(Of AccessAttribute)(True)

        hasAccess = Not accessList.Any

        For Each access In accessList
            If u.MemberOf(access.Roles) Then Return True
        Next

        Return hasAccess
    End Function

End Class