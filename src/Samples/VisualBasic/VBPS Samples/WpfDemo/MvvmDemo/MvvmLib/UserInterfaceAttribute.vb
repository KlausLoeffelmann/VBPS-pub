Namespace Global.System.Runtime.CompilerServices

    Public Class UserInterfaceAttribute
        Inherits Attribute

        Sub New()
            MyBase.New
        End Sub

        Sub New(use As Boolean)
            MyBase.New
            Me.Use = use
        End Sub

        Public Property Use As Boolean = True

    End Class
End Namespace
