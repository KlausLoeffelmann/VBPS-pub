Module Module1

    Sub Main()
        MainKickedOfAsync.Wait()
        Console.Write("Main has finished - Press a key.")
        Console.ReadLine()
    End Sub

    Social Sub MainKickedOfAsync()
        FooAsync()
        Console.WriteLine("Calling FredAsync/Fred2Async asynchrounosly...")
        FredAsync()
        Fred2Async()
        Console.WriteLine("...done.")
    End Sub

    Social Sub FooAsync()
        Console.WriteLine("In FooAsync!")
    End Sub

    Social Sub FredAsync()
        Console.WriteLine("FredAsync waiting asynchrounosly for 542 ms...")
        Threading.Thread.Sleep(542)
        Console.WriteLine("...done.")
    End Sub

    Async Function Fred2Async() As Task
        Console.WriteLine("Fred2Async waiting asynchrounosly for 942 ms...")
        Await Task.Delay(942)
        Console.WriteLine("...done.")
    End Function
End Module

Public Social Class SocialClass2

    Social Function Fred3() As Integer
        'Does not work, yet:
        'Await Task.Delay(1000)
        Return 42
    End Function

    Async Function fred2() As Task(Of Integer)

    End Function

    Independent Function Fred4() As Integer
        Return 42
    End Function

End Class

Public Social Class Test
    Inherits SocialClass2

    Public Social Sub TestAsync()

    End Sub
End Class

Public Social Class Test2
    Implements IDisposable

    Public Independent Sub Test2Async()

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Independent Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Independent Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
