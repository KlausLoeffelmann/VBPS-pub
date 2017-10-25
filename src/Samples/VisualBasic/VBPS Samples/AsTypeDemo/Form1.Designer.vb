<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.FirstButton = New System.Windows.Forms.Button()
        Me.SecondButton = New System.Windows.Forms.Button()
        Me.ThirdButton = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'FirstButton
        '
        Me.FirstButton.Location = New System.Drawing.Point(1086, 23)
        Me.FirstButton.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.FirstButton.Name = "FirstButton"
        Me.FirstButton.Size = New System.Drawing.Size(208, 67)
        Me.FirstButton.TabIndex = 0
        Me.FirstButton.Text = "First Button"
        Me.FirstButton.UseVisualStyleBackColor = True
        '
        'SecondButton
        '
        Me.SecondButton.Location = New System.Drawing.Point(1086, 102)
        Me.SecondButton.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.SecondButton.Name = "SecondButton"
        Me.SecondButton.Size = New System.Drawing.Size(208, 67)
        Me.SecondButton.TabIndex = 1
        Me.SecondButton.Text = "Second Button"
        Me.SecondButton.UseVisualStyleBackColor = True
        '
        'ThirdButton
        '
        Me.ThirdButton.Location = New System.Drawing.Point(1086, 181)
        Me.ThirdButton.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.ThirdButton.Name = "ThirdButton"
        Me.ThirdButton.Size = New System.Drawing.Size(208, 67)
        Me.ThirdButton.TabIndex = 2
        Me.ThirdButton.Text = "Third Button"
        Me.ThirdButton.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(24, 23)
        Me.TextBox1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(998, 31)
        Me.TextBox1.TabIndex = 3
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(487, 284)
        Me.Button1.Margin = New System.Windows.Forms.Padding(6)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(396, 67)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Some Test Button"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1318, 704)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.ThirdButton)
        Me.Controls.Add(Me.SecondButton)
        Me.Controls.Add(Me.FirstButton)
        Me.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FirstButton As Button
    Friend WithEvents SecondButton As Button
    Friend WithEvents ThirdButton As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Button1 As Button
End Class
