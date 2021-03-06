﻿Public Class Form1Org

    Const ELEMENTS = 10000000

    Sub SimpleConversions()

        Dim aDouble = 42.42
        Dim anInteger = aDouble As Integer

        Dim aSingle = 42.42R
        Dim aShort = aSingle As Short

        Dim aControl = New Button
        Dim aButton = aControl As Button

        'Conversion from char value to char was only possible with ChrW!
        Dim firstName As String = ""
        Dim charValues = {65, 100, 114, 105, 97, 110, 97}
        For Each cItem In charValues : firstName &= cItem As Char : Next

        'Converting to nullable
        Dim iAsObject = "Klaus"
        Dim iNullable = iAsObject As Integer?

        'Boxing:
        Dim boxedInteger As Object = 42

        'Exception, Short cannot unboxed directly into integer
        aShort = CShort(boxedInteger)

        'That's the way to do it:
        aShort = boxedInteger As Integer As Short

    End Sub

    Sub ConvertingNumbers()

        Dim random As New Random(Date.Now.Millisecond)

        Dim doubleArray As Double() = New Double(ELEMENTS - 1) {}
        Dim intArray As Integer() = New Integer(ELEMENTS - 1) {}

        'Generating random double elements.
        For count = 0 To ELEMENTS - 1
            doubleArray(count) = random.NextDouble()
        Next

        Dim sw = Stopwatch.StartNew

        For count = 0 To ELEMENTS - 1
            intArray(count) = CInt(Math.Truncate(doubleArray(count)))
        Next

        sw.Stop()
        Console.WriteLine($"Converted {ELEMENTS} double elements with 'CInt' to integer {sw.ElapsedMilliseconds:#,##0} ms.")
    End Sub
End Class
