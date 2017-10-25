Imports System.Threading
Imports Windows.Storage.Pickers

Public Class EditorViewModelTraditional
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private WithEvents _CurrentTimeUpdater As New DispatcherTimer With {.Interval = New TimeSpan(0, 0, 1)}
    Private _SyncContext As SynchronizationContext

    Private _SaveCommand As RelayCommand
    Private _DocumentName As String
    Private _CurrentTime As String
    Private _CurrentDate As String
    'Private _MainDocument As String

    Sub New()
        _SyncContext = SynchronizationContext.Current
        _CurrentTimeUpdater.Start()

        DocumentName = "New Document"
        SaveCommand = New RelayCommand(Async Sub() Await SaveFileAsync())
    End Sub

    Protected Overridable Sub OnPropertyChanged(eArgs As PropertyChangedEventArgs)
        RaiseEvent PropertyChanged(Me, eArgs)
    End Sub

    'This will not work: We need a compatible property!
    Public Property MainDocument As String = "SomeText"

    Public Property DocumentName As String
        Get
            Return _DocumentName
        End Get
        Set(value As String)
            If Not Object.Equals(value, _DocumentName) Then
                _DocumentName = value
                OnPropertyChanged(New PropertyChangedEventArgs(NameOf(DocumentName)))
            End If
        End Set
    End Property

    Public Property CurrentTime As String
        Get
            Return _CurrentTime
        End Get
        Set(value As String)
            If Not Object.Equals(value, _CurrentTime) Then
                _CurrentTime = value
                OnPropertyChanged(New PropertyChangedEventArgs(NameOf(CurrentTime)))
            End If
        End Set
    End Property

    Public Property CurrentDate As String
        Get
            Return _CurrentDate
        End Get
        Set(value As String)
            If Not Object.Equals(value, _CurrentDate) Then
                _CurrentDate = value
                OnPropertyChanged(New PropertyChangedEventArgs(NameOf(CurrentDate)))
            End If
        End Set
    End Property

    Public Property SaveCommand As RelayCommand
        Get
            Return _SaveCommand
        End Get
        Set(value As RelayCommand)
            If Not Object.Equals(value, _SaveCommand) Then
                _SaveCommand = value
                OnPropertyChanged(New PropertyChangedEventArgs(NameOf(SaveCommand)))
            End If
        End Set
    End Property

    Private Sub CurrentTimeUpdater(sender As Object, e As Object) Handles _CurrentTimeUpdater.Tick
        _SyncContext.Post(
            Sub()
                CurrentTime = $"{Date.Now:HH.mm.ss}"
                CurrentDate = $"{Date.Now:yyyy-MM-dd}"
            End Sub, Nothing)
    End Sub

    Public Async Function SaveFileAsync() As Task
        Dim fop = New FileSavePicker()
        fop.FileTypeChoices.Add("textfiles", {".txt"})
        Dim file = Await fop.PickSaveFileAsync.AsTask
        Await FileIoTasks.WriteTextAsync(file, MainDocument)
    End Function

End Class
