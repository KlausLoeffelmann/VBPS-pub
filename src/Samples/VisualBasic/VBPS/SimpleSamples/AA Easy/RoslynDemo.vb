Imports System
Imports System.Threading.Tasks
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Module Program

    Sub Main(args As String())

        Dim aDouble = 42.6
        Dim anIntWithCType = CType(aDouble, Integer)
        Dim anIntWithAsType = aDouble as Integer

        Dim aString=anIntWithAsType as String
        Console.WriteLine($"aString: {aString}")

        Console.WriteLine($"Ctype Value:{anIntWithCType}; AsType Value:{anIntWithAsType}")

        Dim aChar = 65 as Char
        Console.WriteLine($"Char Conversion Value: '{aChar}'")

        Dim intChar=aChar as Integer
        Console.WriteLine($"Char Value:{intChar}")

        Dim testObject as Object = New Foo
        Dim testFoo = testObject as Foo
        Dim testBar as Bar = testObject as Bar

        Console.WriteLine($"testFoo has instance:{(testFoo isnot Nothing)}")
        Console.WriteLine($"testBar has instance:{(testBar isnot Nothing)}")

        KickOfAsync
        Console.ReadLine
    End Sub

    Public Async Sub KickOfAsync
        'Await SocialSubFooAsync
        Console.WriteLine($"Async Result: {Await Get42Async}")
        Console.WriteLine($"Async Result: {Await SocialFooAsync}")
    End Sub

    Public Async Function Get42Async() As Task(Of Integer)
        Return Await Task.FromResult(42)
    End Function

    'public Social Handles Event Sub SomeEventHandler
    '    Task.Delay(0)
    'End Sub

    public Social Sub SocialSubFooAsync
        Task.Delay(1000)
    End Sub

    Public Social Function SocialFooAsync As Integer
        Task.Delay(2000)
        Return Get42Async
    End Function

    Public Social Function SocialBarAsync As Integer
        return SocialFooAsync
    End Function

End Module

Public Class Foo
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    <UserInterface(use:=True)>
    Public Property FooProp As String
End Class

<UserInterface>
Public Class Bar
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _BarPropComplete As String

    Public Property FooProp As String

    Public Property BarPropComplete As String
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
