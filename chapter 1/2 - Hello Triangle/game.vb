Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics
Imports OpenTK.Input
Imports OpenTK.Graphics.OpenGL
'Imports OpenTK.Graphics.OpenGL4

Public Class game
    Inherits GameWindow

    'Create the vertices for our triangle. These are listed in normalized device coordinates (NDC)
    'In NDC, (0, 0) Is the center of the screen.
    'Negative X coordinates move to the left, positive X move to the right.
    'Negative Y coordinates move to the bottom, positive Y move to the top.
    'OpenGL only supports rendering in 3D, so to create a flat triange, the Z coordinate will be kept as 0.

    Dim vertices As Single() = {-0.5F, -0.5F, 0.0F, 0.5F, -0.5F, 0.0F, 0.0F, 0.5F, 0.0F}
    Dim VertexBufferObject As Integer
    Dim VertexArrayObject As Integer
    Dim shader As shader

    Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal title As String)
        MyBase.New(width, height, GraphicsMode.[Default], title)
    End Sub

    'Now, we start initializing OpenGL.
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        'This will be the color of the background after we clear it, in normalized colors. This Is a deep green.
        GL.ClearColor(0.2F, 0.3F, 0.3F, 1.0F)

        'We need to send our vertices over to the graphics card so OpenGL can use them.
        'To do this, we need to create what's called a Vertex Buffer Object (VBO).
        'These allow you to upload a bunch of data to a buffer, And send the buffer to the graphics card.
        'This effectively sends all the vertices at the same time.
        'Keep in mind that this function returns an int, which Is a handle to the actual object.
        VertexBufferObject = GL.GenBuffer()

        'Now, bind the buffer. OpenGL uses one global state, so after calling this,
        '  all future calls that modify the VBO will be applied to this buffer until another buffer Is bound instead.
        'The first argument Is an enum, specifying what type of buffer it should be bound to.
        'There are multiple types of buffers, but for now, only the VBO Is necessary.
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject)


        'Finally, upload the vertices to the buffer.
        'Arguments:
        '  What type of buffer the data should be sent to
        '  How much data Is being sent, in bytes.
        '  The vertices themselves
        '  How the buffer will be used, so that OpenGL can write the data to the proper memory space on the GPU
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Len(vbSingle), vertices, BufferUsageHint.StaticDraw)


        'We've got the vertices done, but how exactly should this be converted to pixels for the final image?
        'To decide this, we must create what are known as shaders; these are small programs that live on the graphics card, And transform the vertices into pixels.
        'The file shader.vert has an example of what shader programming Is Like.
        shader = New shader("shader.vert", "shader.frag")

        'Now, enable the shader.
        'Just Like the VBO, this Is global, so every function that modifies a shader will modify this one until a New one Is bound instead.
        shader.Use()

        'Ignore this for now.
        VertexArrayObject = GL.GenVertexArray()
        GL.BindVertexArray(VertexArrayObject)

        'Now, we need to setup how the vertex shader will interpret the VBO data; you can send almost any C datatype (And a few non-C ones too) to it.
        'While this makes them incredibly flexible, it means we have to specify how that data will be mapped to the shader's input variables.

        'To do this, we use the GL.VertexAttribPointer function
        'Arguments:
        '  Location of the input variable in the shader. the layout(location = 0) line in the vertex shader explicitly sets it to 0.
        '  How many elements will be sent to the variable. In this case, 3 floats for every vertex.
        '  The data type of the elements set, in this case float.
        '  Whether Or Not the data should be converted to normalized device coordinates. In this case, false, because that's already done.
        '  The stride; this Is how many bytes are between the last element of one vertex And the first element of the next. 3 * sizeof(float) in this case.
        '  The offset; this Is how many bytes it should skip to find the first element of the first vertex. 0 as of right now.
        'Stride And Offset are just sort of glossed over for now, but when we get into texture coordinates they'll be shown in better detail.
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, False, 3 * Len(vbSingle), 0)

        'Enable variable 0 in the shader.
        GL.EnableVertexAttribArray(0)

        'For a simple project, this would probably be enough. However, if you have a bunch of objects with their own shaders being drawn, it would be incredibly
        'tedious to do this over And over again every time you need to switch what object Is being drawn. Because of this, OpenGL now *requires* that you create
        'what Is known as a Vertex Array Object (VAO). This stores the layout you create with VertexAttribPointer/EnableVertexAttribArray so that it can be
        'recreated with one simple function call.
        'By creating the VertexArrayObject, it has automatically saved this layout, so you can simply bind the VAO again to get everything back how it should be.

        'Finally, we bind the VBO again so that the VAO will bind that as well.
        'This means that, when you bind the VAO, it will automatically bind the VBO as well.
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject)


        'Setup Is now complete! Now we move to the OnRenderFrame function to finally draw the triangle.
        MyBase.OnLoad(e)
    End Sub

    'Now that initialization Is done, let's create our render loop.
    Protected Overrides Sub OnRenderFrame(ByVal e As FrameEventArgs)

        'This clears the image, using what you set as GL.ClearColor earlier.
        'OpenGL provides several different types of data that can be rendered, And all of them can be cleared here.
        'However, we only modify the color, so ColorBufferBit Is all we need to clear.
        GL.Clear(ClearBufferMask.ColorBufferBit)


        'To draw an object in OpenGL, it's typically as simple as binding your shader,
        'setting shader variables (Not done here, will be shown in a future tutorial)
        'binding the VAO,
        'And then calling GL.DrawArrays

        'Bind the shader
        shader.Use()

        'Bind the VAO
        GL.BindVertexArray(VertexArrayObject)

        'And then call GL.DrawArrays
        'Arguments:
        '  Primitive type; What sort of geometric primitive the vertices represent. We just want a triangle, so PrimitiveType.Triangles Is fine.
        '  Starting index; this Is just the start of the data you want to draw. 0 here.
        '  How many vertices you want to draw. 3 for a triangle.
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3)


        'OpenTK windows are what's known as "double-buffered". In essence, the window manages two images.
        'One Is rendered to while the other Is currently displayed by the window.
        'After drawing, call this function to swap the buffers. If you don't, it won't display what you've rendered.
        Context.SwapBuffers()


        'And that's all you have to do for rendering! You should now see a yellow triangle on a black screen.
        MyBase.OnRenderFrame(e)
    End Sub

    Protected Overrides Sub OnUpdateFrame(ByVal e As FrameEventArgs)
        Dim input As KeyboardState = Keyboard.GetState()

        If input.IsKeyDown(Key.Escape) Then
            End
        End If

        MyBase.OnUpdateFrame(e)
    End Sub

    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        GL.Viewport(0, 0, Width, Height)
        MyBase.OnResize(e)
    End Sub

    Protected Overrides Sub OnUnload(ByVal e As EventArgs)
        'Unbind all the resources by binding the targets to 0/null.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
        GL.BindVertexArray(0)
        GL.UseProgram(0)

        'Delete all the resources.
        GL.DeleteBuffer(VertexBufferObject)
        GL.DeleteVertexArray(VertexArrayObject)

        shader.Dispose()
        MyBase.OnUnload(e)
    End Sub

End Class
