Imports System.Threading
Imports Windows.Storage.Pickers

<UserInterface>
Public Social Class EditorViewModelTraditional
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
                 Implements INotifyPropertyChanged.PropertyChanged

    Private WithEvents _CurrentTimeUpdater As New DispatcherTimer With {.Interval = New TimeSpan(0, 0, 1)}
    Private _SyncContext As SynchronizationContext

    Public Social Sub SaveFileAsync()
        Dim fop = New FileSavePicker()
        fop.FileTypeChoices.Add("textfiles", {".txt"})
        Dim file = fop.PickSaveFileAsync.AsTask
        FileIoTasks.WriteTextAsync(file, MainDocument)
    End Sub

    <UserInterface>
    Public Social Sub LoadFileAsync()
        Dim fop = New FileOpenPicker()
        fop.FileTypeFilter.Add(".txt")
        fop.ViewMode = PickerViewMode.List
        Dim file = fop.PickSingleFileAsync().AsTask
        MainDocument = FileIoTasks.ReadTextAsync(file)
    End Sub

    Private Independent Sub CurrentTimeUpdater(sender As Object, e As Object) _
        Handles _CurrentTimeUpdater.Tick
        _SyncContext.Post(
            Sub()
                CurrentTime = $"{Date.Now:HH.mm.ss}"
                CurrentDate = $"{Date.Now:yyyy-MM-dd}"
            End Sub, Nothing)
    End Sub

    Sub New()
        SaveCommand = New RelayCommand(Async Sub() Await SaveFileAsync())
        _SyncContext = SynchronizationContext.Current
        _CurrentTimeUpdater.Start()
    End Sub

    Public Property MainDocument As String

    <UserInterface(Use:=False)>
    Public Property DocumentName As String

    Public Property CurrentTime As String

    Public Property CurrentDate As String

    Public Property SaveCommand As RelayCommand
End Class
