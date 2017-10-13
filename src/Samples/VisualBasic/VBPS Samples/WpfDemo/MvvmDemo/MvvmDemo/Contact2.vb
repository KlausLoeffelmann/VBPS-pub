Imports System.ComponentModel
Imports System.Runtime.CompilerServices

<UserInterface>
Public Class Contact
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Sub New()
        AddHandler Me.PropertyChanged, AddressOf HandleSomething
    End Sub

    Private Sub HandleSomething(o As Object, e As PropertyChangedEventArgs)
        Debug.WriteLine($"{o.ToString}: {e.PropertyName}")

        Dim t = o As Contact
        _Foo = "Test"

    End Sub

    Property Foo As String
    Property Test As String

End Class
