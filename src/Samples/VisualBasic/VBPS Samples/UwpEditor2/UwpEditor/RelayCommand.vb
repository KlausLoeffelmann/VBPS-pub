Public Class RelayCommand
    Implements ICommand

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Private ReadOnly _Execute As Action(Of Object)
    Private ReadOnly _CanExecute As Func(Of Object, Boolean)

    Public Sub New(execute As Action(Of Object))
        Me.New(execute, Nothing)
    End Sub

    Public Sub New(execute As Action(Of Object), canExecute As Func(Of Object, Boolean))
        _Execute = execute
        _CanExecute = canExecute
    End Sub

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _Execute(parameter)
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        If _CanExecute IsNot Nothing Then
            Return _CanExecute(parameter)
        Else
            Return True
        End If
    End Function

    Public Shared Narrowing Operator CType(execute As Action(Of Object)) As RelayCommand
        Return New RelayCommand(execute)
    End Operator

End Class
