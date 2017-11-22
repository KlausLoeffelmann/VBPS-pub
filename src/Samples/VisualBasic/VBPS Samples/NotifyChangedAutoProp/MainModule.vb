Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks

Module MainModule

    Sub Main()
        Dim overClassInstance As New UserInterfaceOnClass
        AddHandler overClassInstance.PropertyChanged,
            Sub(s As Object, e As PropertyChangedEventArgs)
                Console.WriteLine($"{s.GetType.ToString()} changed property {e.PropertyName}.")
            End Sub

        'overClassInstance.BarProp = "Value1"
        'overClassInstance.FooProp = "Value2"
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

Public Class MarshallingConventional

    Public Async Function TestConfigureAwaitTrue() As Task
        Dim currentThreadID = Task.CurrentId
        Await Task.Delay(100).ConfigureAwait(True)
        Dim expect = (currentThreadID = Task.CurrentId)
    End Function

    Public Async Function TestConfigureAwaitfalse() As Task
        Dim currentThreadID = Task.CurrentId
        Await Task.Delay(100).ConfigureAwait(False)
        Dim expect = (currentThreadID <> Task.CurrentId)
    End Function

End Class

Public Social Class MarshallingWithSocial

    Public Social UserInterface Sub TestConfigureAwaitTrue()
        Dim currentThreadID = Task.CurrentId
        Task.Delay(100)
        'UserInterface: Synthesizes ConfigureAwait(True)
        Dim expect = (currentThreadID = Task.CurrentId)
    End Sub

    Public Social Sub TestConfigureAwaitFalse()
        Dim currentThreadID = Task.CurrentId
        Task.Delay(100)
        'Just Social: Synthesizes ConfigureAwait(false)
        Dim expect = (currentThreadID <> Task.CurrentId)
    End Sub

End Class

Public Class UserInterfaceOnProperty
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Implements the code for raising NotifyPropertyChanged directly.
    Public UserInterface Property MvvmProperty As String

End Class

Public Class UserInterfaceOnPropertyOnPropertyChanged
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Implements the code for raising the Event by calling OnPropertyChanged
    Public UserInterface Property MvvmProperty As String


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

'Synthesizes OnPropertyChange and code for raising PropertyChanged
'for every property that doesn't have the Independent Modifier.
Public UserInterface Class UserInterfaceOnClass
    Implements INotifyPropertyChanged

    Private _manualProp As String

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Generates and use OnPropertyChanged
    Public Property MvvmProperty As String

    'Tells compiler to NOT to implement a call to NotifyPropertyChanged.
    Public Independent Property SimpleAutoImplementing As Integer

    'Manual Property implementation, but using synthesized infrastructure:
    Public Property ManualProp As String
        Get
            Return _manualProp
        End Get
        Set(value As String)
            If Not Object.Equals(value, _manualProp) Then
                'This of course works with IntelliSense!
                OnPropertyChanged(New PropertyChangedEventArgs(NameOf(ManualProp)))
            End If
        End Set
    End Property

End Class

Public UserInterface Class InheritedFromOverClass
    Inherits UserInterfaceOnClass

    'Uses of course MyBase.OnPropertyChange to Synthesize
    'RaiseEvent Code, since it is derived.
    Public Property FooPropInherited As String
End Class

''Should report 'Does not implement INotifyPropertyChange.'
'Public UserInterface Class OverClassMissingInterface

'    Shouldn't generate.
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
