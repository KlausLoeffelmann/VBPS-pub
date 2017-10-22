Imports System.ComponentModel
Imports System.Runtime.CompilerServices

<UserInterface>
Public Class Contact
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Sub New()
        AddHandler Me.PropertyChanged, Sub(o, e)
                                           Debug.WriteLine($"{o.ToString}: {e.PropertyName}")
                                       End Sub
    End Sub

    Property Test As String
    Property Foo As String


End Class
