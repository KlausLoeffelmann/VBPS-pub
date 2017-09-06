Imports System

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

        testObject = New Foo
        testFoo = CType(testObject, Foo)
        testBar = CType(testObject, Bar)

    End Sub

    'public Social Handles Event Sub SomeEventHandler
    '    Await System.Threading.Tasks.Task.Delay(0)
    'End Sub

    Public Social Function SocialFoo As Integer
        Return 42
    End Function

    Public Social Function SocialBar As Integer
        Dim task = SocialFoo
        return 42
    End Function

    Public Class Foo
    End Class

    Public Class Bar
    End Class
End Module
