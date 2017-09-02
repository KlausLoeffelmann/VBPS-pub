Imports System

Module Program

    Sub Main(args As String())

        Dim aString As String
        aString = "42.42"
        Dim aDouble = Double.Parse(aString)
        Dim aLong = CType(aDouble + aDouble, Long)

        For z = 0 To aLong
            Console.WriteLine(z.ToString)
        Next
    End Sub
End Module
