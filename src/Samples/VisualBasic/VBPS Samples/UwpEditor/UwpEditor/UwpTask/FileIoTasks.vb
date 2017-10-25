Imports Windows.Storage

Public Module FileIoTasks
    Public Async Function ReadTextAsync(file As StorageFile) As Task(Of String)
        Return Await FileIO.ReadTextAsync(file)
    End Function

    Public Async Function WriteTextAsync(file As StorageFile, contents As String) As Task
        Await FileIO.WriteTextAsync(file, contents)
    End Function
End Module
