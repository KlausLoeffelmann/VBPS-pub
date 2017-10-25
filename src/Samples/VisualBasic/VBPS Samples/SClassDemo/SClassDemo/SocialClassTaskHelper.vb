Imports System.Runtime.CompilerServices

Public Module SocialClassTaskHelper

    <Extension>
    Public Async Function AsyncResult(Of t)(task As Task(Of t)) As Task(Of t)
        Return Await task
    End Function
End Module
