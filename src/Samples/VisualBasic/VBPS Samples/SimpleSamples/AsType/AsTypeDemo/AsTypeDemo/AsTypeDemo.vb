Imports System.Diagnostics

Module AsTypeDemo

    Private Const ELEMENTS = 10000000

    Sub Main()

        Dim random As New Random(Date.Now.Millisecond)

        Dim doubleArray As Double() = New Double(ELEMENTS - 1) {}
        Dim intArray As Integer() = New Integer(ELEMENTS - 1) {}

        'Generating random double elements.
        Dim sw = Stopwatch.StartNew
        For count = 0 To ELEMENTS - 1
            doubleArray(count) = random.NextDouble()
        Next
        sw.Stop()

        Console.WriteLine($"Generated {ELEMENTS} random double elements in {sw.ElapsedMilliseconds:#,##0} ms.")

        sw = Stopwatch.StartNew
        For count = 0 To ELEMENTS - 1
            intArray(count) = CInt(Math.Truncate(doubleArray(count)))
        Next
        sw.Stop()
        Console.WriteLine($"Converted {ELEMENTS} double elements with 'CInt' to integer {sw.ElapsedMilliseconds:#,##0} ms.")

        sw = Stopwatch.StartNew
        For count = 0 To ELEMENTS - 1
            intArray(count) = doubleArray(count) As Integer
        Next
        sw.Stop()
        Console.WriteLine($"Converted {ELEMENTS} double elements with 'As Integer' to integer {sw.ElapsedMilliseconds:#,##0} ms.")

        Console.WriteLine()
        Console.WriteLine("Press a key...")
        Console.ReadKey()
    End Sub

End Module

