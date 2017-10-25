Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices

Public Social Class AsyncTextFileManager
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private myTextDocument As String

    Social Sub SaveTextFileAsync(filename As String)
        Using stream As New FileStream(filename, FileMode.Create)
            Using writer As New StreamWriter(stream)
                writer.WriteAsync(TextDocument)
                writer.FlushAsync()
                stream.FlushAsync()
                writer.Close()
                stream.Close()
            End Using
        End Using
    End Sub

    <UserInterface>
    Social Sub LoadTextFileAsync(filename As String)
        Using stream As New FileStream(filename, FileMode.Open)
            Using reader As New StreamReader(stream)
                TextDocument = reader.ReadToEndAsync
                reader.Close()
                stream.Close()
            End Using
        End Using
    End Sub

    Social Function GetFilenameAsync(Optional owner As IWin32Window = Nothing) As String
        Dim tcs As New TaskCompletionSource(Of String)
        Dim fod = New OpenFileDialog()
        Dim diagResult As DialogResult
        fod.Title = "Open Textfile"

        If owner Is Nothing Then
            diagResult = fod.ShowDialog
        Else
            diagResult = fod.ShowDialog(owner)
        End If

        If diagResult = DialogResult.Cancel Then
            tcs.SetResult(Nothing)
        Else
            tcs.SetResult(fod.FileName)
        End If
        Return tcs.Task.AsyncResult
    End Function

    <UserInterface>
    Public Property TextDocument As String

End Class
