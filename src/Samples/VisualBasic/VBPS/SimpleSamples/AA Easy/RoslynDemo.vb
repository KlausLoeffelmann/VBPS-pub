Imports System
Imports System.Threading.Tasks

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

    Public Class Foo
        Public Property FooProp as String = "42"
    End Class

    Public Class Bar
    End Class
End Module
