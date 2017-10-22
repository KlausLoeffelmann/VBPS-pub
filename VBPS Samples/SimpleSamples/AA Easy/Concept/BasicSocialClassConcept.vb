<UIClass>
Public NonInheritable Async Class Foo

    Private Sub New
    End Sub

    Public Async Shared GetInstance() as Task(of Foo)
    End Sub

    Public Property BarPropAsync as Type With Value as Default
        Get
            Return DefaultValue
            FireAndForgetAsync PropertyGetterAsync
            RaiseEvent INotifyPropertyChange(ActualValue)

        End Get
    End Property

    Sub Bar
    End Sub

    Public Sub BarAsync
        Task.Delay(0)
    End Sub

    'Instead Of
    Public Function BarAsync as Task
        Await Task.Delay(0)
    End Function 
End Class
