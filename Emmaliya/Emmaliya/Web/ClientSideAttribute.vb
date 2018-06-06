Namespace Web
    ''' <summary>
    ''' Identifies properties whose value will be exposed on the client side in the currentUser, currentPage, and currentApplication objects
    ''' </summary>
    <AttributeUsage(AttributeTargets.Property)>
    Public Class ClientSideAttribute
        Inherits Attribute


    End Class
End Namespace