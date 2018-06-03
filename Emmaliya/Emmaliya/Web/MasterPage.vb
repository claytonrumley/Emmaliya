Namespace Web

    ''' <summary>
    ''' 
    ''' </summary>
    Public MustInherit Class MasterPage(Of U As IUser)
        Inherits System.Web.UI.MasterPage

        Public Overridable ReadOnly Property AutoDetectScript As Boolean = True

        Public Overridable ReadOnly Property AutoDetectStylesheet As Boolean = True

        Public ReadOnly Property CurrentApplication As Application(Of U)
            Get
                Return Emmaliya.Web.Application(Of U).Current
            End Get
        End Property

        Public Shadows Property Page As Page(Of U)
            Get
                Return MyBase.Page
            End Get
            Set(value As Page(Of U))
                MyBase.Page = value
            End Set
        End Property

    End Class

End Namespace