Imports System

Module Program

    Sub Main(args As String())

        Dim aString As String
        aString = "42"
        Dim anInt = Integer.Parse(aString)
        anInt = anInt + anInt + 2

        For z = 0 To 100
            Console.WriteLine(z.ToString)
        Next

        Console.WriteLine(anInt.ToString)

    End Sub

End Module
