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

    Independent Sub Fred3Async()
        'Does not work, yet:
        'Await Task.Delay(1000)
    End Sub

End Class
