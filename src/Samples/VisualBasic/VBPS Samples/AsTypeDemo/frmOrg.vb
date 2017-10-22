Public Class frmMainOriginal

    Const ELEMENTS = 10000000

    Sub SimpleConversions()
        Dim aDouble = 42.42
        Dim anInteger = CInt(aDouble)

        Dim aSingle = 42.42R
        Dim aShort = CShort(aSingle)

        Dim aControl = New Button
        Dim aButton = TryCast(aControl, Button)

        'Conversion from char value to char was only possible with ChrW!
        Dim firstName As String = ""
        Dim charValues = {65, 100, 114, 105, 97, 110, 97}
        For Each cItem In charValues : firstName &= ChrW(cItem) : Next

        'Converting to nullable
        Dim iAsObject = "Klaus"
        Dim iNullable = CType(iAsObject, Integer?)

        'Boxing:
        Dim boxedInteger As Object = 42
        'Exception, Short cannot unboxed directly into integer
        aShort = CShort(boxedInteger)
        'That's the way to do it:
        aShort = CShort(CShort(boxedInteger))

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

    Sub ButtonHandler(sender As Object, e As EventArgs) Handles _
        FirstButton.Click, SecondButton.Click, ThirdButton.Click

        Dim clickedButton = DirectCast(sender, Button)
        If clickedButton IsNot Nothing Then
            MessageBox.Show($"You clicked {clickedButton.Name}!")
        End If
    End Sub

    Sub ClickHandler(sender As Object, e As EventArgs) Handles _
        FirstButton.Click, SecondButton.Click, ThirdButton.Click

        If sender As Button IsNot Nothing Then
            MessageBox.Show($"You clicked Button {(sender As Button).Name}!")
        ElseIf sender As TextBox IsNot Nothing Then
            MessageBox.Show($"You clicked TextBox {(sender As Button).Name}!")
        End If
    End Sub
End Class
