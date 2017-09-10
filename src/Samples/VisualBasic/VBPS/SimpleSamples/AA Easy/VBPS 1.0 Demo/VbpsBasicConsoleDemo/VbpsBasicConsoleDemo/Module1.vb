Module Module1

    Sub Main()
        MainKickedOfAsync.Wait()
        Console.Write("Main has finished - Press a key.")
        Console.ReadLine()
    End Sub

    Social Sub MainKickedOfAsync()
        FooAsync()
        Console.WriteLine("Calling FredAsync asynchrounosly...")
        FredAsync()
        Console.WriteLine("...done.")
    End Sub

    Social Sub FooAsync()
        Console.WriteLine("In FooAsync!")
    End Sub

    Social Sub FredAsync()
        Console.WriteLine("FredAsync waiting asynchrounosly for 942 ms...")
        Task.Delay(942)
        Console.WriteLine("...done.")
    End Sub

    Async Function Fred2Async() As Task
        Console.WriteLine("FredAsync waiting asynchrounosly for 942 ms...")
        Await Task.Delay(942)
        Console.WriteLine("...done.")
    End Function

    Independent Function Fred3() As Task(Of Integer)
        'Does not work, yet:
        'Await Task.Delay(1000)
        Return 5
    End Function

End Module
