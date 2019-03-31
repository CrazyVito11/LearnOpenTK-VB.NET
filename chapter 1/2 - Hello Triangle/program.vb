Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class program

    Public Shared Sub Main()
        Using Game As GameWindow = New game(800, 600, "LearnOpenTK - Hello Triangle!")

            'To create a New window, create a class that extends GameWindow, then call Run() on it.
            'Run takes a double, which Is how many frames per second it should strive to reach.
            'You can leave that out And it'll just update as fast as the hardware will allow it.
            Game.Run(60.0)
        End Using

        'And that's it! That's all it takes to create a window with OpenTK.
    End Sub

End Class
