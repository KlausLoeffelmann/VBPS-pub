Public Class Form1

    Sub ButtonHandler(sender As Object, e As EventArgs) Handles _
        FirstButton.Click, SecondButton.Click, ThirdButton.Click

        Dim clickedButton = sender As Button
        If clickedButton IsNot Nothing Then
            MessageBox.Show($"You clicked {clickedButton.Name}!")
        End If

        Dim clickedText = sender As TextBox
        If clickedText Is Nothing Then

        End If

    End Sub

End Class
