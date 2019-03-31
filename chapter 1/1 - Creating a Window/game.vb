
Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics
Imports OpenTK.Input
Imports OpenTK.Graphics.OpenGL

Public Class game
    Inherits GameWindow

    'This Is where all OpenGL code will be written.
    'OpenTK allows for several functions to be overriden to extend functionality; this Is how we'll be writing code.

    'A simple constructor to let us set the width/height/title of the window.

    Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal title As String)
        MyBase.New(width, height, GraphicsMode.[Default], title)
    End Sub


    'This function runs on every update frame.
    Protected Overrides Sub OnUpdateFrame(ByVal e As FrameEventArgs)
        Dim input As KeyboardState = Keyboard.GetState()

        If input.IsKeyDown(Key.Escape) Then
            End
        End If

        MyBase.OnUpdateFrame(e)
    End Sub
End Class
