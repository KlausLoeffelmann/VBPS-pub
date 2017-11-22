Imports Windows.Storage

Public Module StorageFileTasks

    Public Async Function CreateStreamedFileAsync(displayNameWithExtension As String,
                                                  dataRequested As StreamedFileDataRequestedHandler,
                                                  thumbnail As Streams.IRandomAccessStreamReference) As Task(Of StorageFile)
        Return Await StorageFile.CreateStreamedFileAsync(displayNameWithExtension,
                                                         dataRequested,
                                                         thumbnail)
    End Function

    Public Async Function GetFileFromPathAsync(path As String) As Task(Of StorageFile)
        Return Await StorageFile.GetFileFromPathAsync(path)
    End Function

End Module
