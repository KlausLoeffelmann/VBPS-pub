Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Module MainModule

    Sub Main()
        Dim overClassInstance As New OverClass
        AddHandler overClassInstance.PropertyChanged,
            Sub(s As Object, e As PropertyChangedEventArgs)
                Console.WriteLine($"{s.GetType.ToString()} changed property {e.PropertyName}.")
            End Sub

        overClassInstance.BarProp = "Value1"
        overClassInstance.FooProp = "Value2"
        overClassInstance.ManualProp = "Value3"

        Dim inheritedFromOverClassInstance As New InheritedFromOverClass
        AddHandler inheritedFromOverClassInstance.PropertyChanged,
            Sub(s As Object, e As PropertyChangedEventArgs)
                Console.WriteLine($"{s.GetType.ToString()} changed property {e.PropertyName}.")
            End Sub

        inheritedFromOverClassInstance.FooPropInherited = "Value4"

        Console.ReadLine()
    End Sub

End Module

Public Class OverProperty
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Should raise Event directly.
    Public UserInterface Property FooProp As String
End Class

Public Class OverPropertyWithOn
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Should raise Event over OnNotifyPropertyChanged.
    Public UserInterface Property FooProp As String

    Protected Overridable Sub OnPropertyChanged(eArgs As PropertyChangedEventArgs)
        RaiseEvent PropertyChanged(Me, eArgs)
    End Sub

End Class

Public UserInterface Class HavingCustomOnPropertyChanged
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property FooProp As String

    Protected Overridable Sub OnPropertyChanged(eArgs As PropertyChangedEventArgs)
        RaiseEvent PropertyChanged(Me, eArgs)
    End Sub

End Class

Public UserInterface Class OverClass
    Implements INotifyPropertyChanged

    Private _manualProp As String

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Should generate and use OnPropertyChanged
    Public Property BarProp As String

'Should ignore.
Public Independent Property FooProp As String

    Public Property ManualProp As String
        Get
            Return _manualProp
        End Get
        Set(value As String)
            If Not Object.Equals(value, _manualProp) Then
                'This method is present, because we use the UserClassAttribute on Class level.
                OnPropertyChanged(New PropertyChangedEventArgs(NameOf(ManualProp)))
            End If
        End Set
    End Property

End Class

Public UserInterface Class InheritedFromOverClass
    Inherits OverClass

    'Should call Mybase.OnPropertyChanged
    Public Property FooPropInherited As String
End Class

''Should report 'Does not implement INotifyPropertyChange.'
'Public UserInterface Class OverClassMissingInterface

'    'Shouldn't generate.
'    Public Property BarProp As String

'End Class

Public Class BaseClassImplementsNotifyChangedProperly
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Overridable Sub OnPropertyChanged(eArgs As PropertyChangedEventArgs)
        RaiseEvent PropertyChanged(Me, eArgs)
    End Sub
End Class

Public UserInterface Class InheritedFromProperly
    Inherits BaseClassImplementsNotifyChangedProperly

    'Should call Mybase.OnNotifyPropertyChanged.
    Public Property FooProp As String
End Class

Public UserInterface Class Bar
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

    Public Independent Property FooPropIgnored As String

End Class
