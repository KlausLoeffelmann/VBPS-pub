Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Module MainModule

    Sub Main()

    End Sub

End Module

Public Class OverProperty
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Should raise Event directly.
    <UserInterface>
    Public Property FooProp As String
End Class

Public Class OverPropertyWithOn
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Should raise Event over OnNotifyPropertyChanged.
    <UserInterface>
    Public Property FooProp As String

    Protected Overridable Sub OnNotifyPropertyChanged(eArgs As PropertyChangedEventArgs)
        RaiseEvent PropertyChanged(Me, eArgs)
    End Sub

End Class

<UserInterface>
Public Class OverClass
    Implements INotifyPropertyChanged

    Sub Test()

    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Should generate and use OnPropertyChanged
    Public Property BarProp As String

    'Should ignore.
    <UserInterface(False)>
    Public Property FooProp As String

End Class

<UserInterface>
Public Class InheritedFromOverClass
    Inherits OverClass

    'Should call Mybase.OnPropertyChanged
    Public Property FooPropInherited As String
End Class

'Should report 'Does not implement INotifyPropertyChange.'
<UserInterface>
Public Class OverClassMissingInterface

    'Shouldn't generate.
    Public Property BarProp As String

End Class

Public Class BaseClassImplementsNotifyChangedProperly
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Overridable Sub OnNotifyPropertyChanged(eArgs As PropertyChangedEventArgs)
        RaiseEvent PropertyChanged(Me, eArgs)
    End Sub
End Class

<UserInterface>
Public Class InheritedFromProperly
    Inherits BaseClassImplementsNotifyChangedProperly

    'Should call Mybase.OnNotifyPropertyChanged.
    Public Property FooProp As String
End Class

<UserInterface>
Public Class Bar
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _BarPropComplete As String

    'Should use OnPropertyChanged
    Public Property FooProp As String

    Public Property BarPropManual As String
        Get
            Return _BarPropComplete
        End Get
        Set(value As String)
            If Not Object.Equals(value, _BarPropComplete) Then
                _BarPropComplete = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("BarPropComplete"))
            End If
        End Set
    End Property

    <UserInterface(Use:=False)>
    Public Property FooPropIgnored As String

End Class
