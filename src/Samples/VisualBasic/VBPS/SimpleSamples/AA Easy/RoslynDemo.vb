Imports System

Module Program

    Sub Main(args As String())

        Dim aDouble = 42.6
        Dim anIntWithCType = CType(aDouble, Integer)
        Dim anIntWithAsType = aDouble as Integer

        Console.WriteLine($"Ctype Value:{anIntWithCType}; AsType Value:{anIntWithAsType}")

        Dim aChar = 65 as Char
        Console.WriteLine($"Char Conversion Value: '{aChar}'")

        Dim intChar=aChar as Integer
        Console.WriteLine($"Char Value:{intChar}")
    End Sub
End Module
