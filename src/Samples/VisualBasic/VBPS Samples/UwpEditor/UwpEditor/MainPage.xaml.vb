' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public Class MainPage
    Inherits Page

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = New EditorViewModel

        '' Add any initialization after the InitializeComponent() call.
        'Me.DataContext = New EditorViewModelTraditional

    End Sub

End Class
