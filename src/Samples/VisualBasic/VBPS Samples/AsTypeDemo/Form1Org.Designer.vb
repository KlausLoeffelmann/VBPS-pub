<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1Org
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.FirstButton = New System.Windows.Forms.Button()
        Me.SecondButton = New System.Windows.Forms.Button()
        Me.ThirdButton = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'FirstButton
        '
        Me.FirstButton.Location = New System.Drawing.Point(543, 12)
        Me.FirstButton.Name = "FirstButton"
        Me.FirstButton.Size = New System.Drawing.Size(104, 35)
        Me.FirstButton.TabIndex = 0
        Me.FirstButton.Text = "First Button"
        Me.FirstButton.UseVisualStyleBackColor = True
        '
        'SecondButton
        '
        Me.SecondButton.Location = New System.Drawing.Point(543, 53)
        Me.SecondButton.Name = "SecondButton"
        Me.SecondButton.Size = New System.Drawing.Size(104, 35)
        Me.SecondButton.TabIndex = 1
        Me.SecondButton.Text = "Second Button"
        Me.SecondButton.UseVisualStyleBackColor = True
        '
        'ThirdButton
        '
        Me.ThirdButton.Location = New System.Drawing.Point(543, 94)
        Me.ThirdButton.Name = "ThirdButton"
        Me.ThirdButton.Size = New System.Drawing.Size(104, 35)
        Me.ThirdButton.TabIndex = 2
        Me.ThirdButton.Text = "Third Button"
        Me.ThirdButton.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(12, 12)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(501, 20)
        Me.TextBox1.TabIndex = 3
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(659, 366)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.ThirdButton)
        Me.Controls.Add(Me.SecondButton)
        Me.Controls.Add(Me.FirstButton)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FirstButton As Button
    Friend WithEvents SecondButton As Button
    Friend WithEvents ThirdButton As Button
    Friend WithEvents TextBox1 As TextBox
End Class
