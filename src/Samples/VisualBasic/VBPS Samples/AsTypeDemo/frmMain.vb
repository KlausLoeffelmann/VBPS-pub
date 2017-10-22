Public Class frmMain

    Const ELEMENTS = 10000000

    Sub SimpleConversions()
        Dim aDouble = 42.42
        Dim anInteger = aDouble As Integer

        Dim aSingle = 42.42R
        Dim aShort = aSingle As Short

        Dim aControl = New Button
        Dim aButton = aControl As Button
        aButton.Tag = 42.42R
        Dim aComponent As ComponentModel.Component = aControl
        aShort = (aComponent As Control).Tag As Short

        'Conversion from char value to char was only possible with ChrW!
        Dim firstName As String = ""
        Dim charValues = {65, 100, 114, 105, 97, 110, 97}
        For Each cItem In charValues : firstName &= cItem As Char : Next

        'Converting to nullable
        Dim iAsObject = 42
        Dim iNullable = iAsObject As Integer?

        'Boxing/Converting
        Dim boxedDate As Object = New DateTimeOffset(#1969-07-24#)
        'DateTimeOffset cannot unboxed directly into string:
        Dim aString = boxedDate As Date As String

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

        sw = Stopwatch.StartNew
        For count = 0 To ELEMENTS - 1
            intArray(count) = doubleArray(count) As Integer
        Next
        sw.Stop()
    End Sub

    Sub ButtonHandler(sender As Object, e As EventArgs) Handles _
        FirstButton.Click, SecondButton.Click, ThirdButton.Click

        Dim clickedButton = sender As Button
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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SimpleConversions()
    End Sub
End Class
