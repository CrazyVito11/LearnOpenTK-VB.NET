Imports System
Imports System.IO
Imports System.Text
Imports OpenTK
Imports OpenTK.Graphics.OpenGL4

Public Class shader

    Private disposedValue As Boolean = False

    Dim Handle As Integer

    'This Is how you create a simple shader.
    'Shaders are written in GLSL, which Is a language very similar to C in its semantics.
    'The GLSL source Is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
    'A commented example of GLSL can be found in shader.vert

    Public Sub New(ByRef vertPath As String, ByRef fragPath As String)
        'There are several different types of shaders, but the only two you need for basic rendering are the vertex And fragment shaders.
        'The vertex shader Is responsible for moving around vertices, And uploading that data to the fragment shader.
        '  The vertex shader won't be too important here, but they'll be more important later.
        'The fragment shader Is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
        '  The fragment shader Is what we'll be using the most here.

        'Create handles for both the vertex And fragment shaders.
        Dim VertexShader As Integer
        Dim FragmentShader As Integer

        'Load vertex shader And compile
        'LoadSource Is a simple function that just loads all text from the file whose path Is given.
        Dim VertexShaderSource As String = LoadSource(vertPath)

        'GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
        VertexShader = GL.CreateShader(ShaderType.VertexShader)

        'Now, bind the GLSL source code
        GL.ShaderSource(VertexShader, VertexShaderSource)

        'And then compile
        GL.CompileShader(VertexShader)

        'Check for compile errors
        Dim infoLogVert As String = GL.GetShaderInfoLog(VertexShader)

        If infoLogVert <> System.String.Empty Then
            System.Console.WriteLine(infoLogVert)
        End If

        'Do the same thing for the fragment shader
        Dim FragmentShaderSource As String = LoadSource(fragPath)
        FragmentShader = GL.CreateShader(ShaderType.FragmentShader)
        GL.ShaderSource(FragmentShader, FragmentShaderSource)
        GL.CompileShader(FragmentShader)

        'Check for compile errors
        Dim infoLogFrag As String = GL.GetShaderInfoLog(VertexShader)
        If infoLogFrag <> System.String.Empty Then
            System.Console.WriteLine(infoLogFrag)
        End If

        'These two shaders must then be merged into a shader program, which can then be used by OpenGL.
        'To do this, create a program...
        Handle = GL.CreateProgram()

        'Attach both shaders...
        GL.AttachShader(Handle, VertexShader)
        GL.AttachShader(Handle, FragmentShader)

        'And then link them together.
        GL.LinkProgram(Handle)

        'Check for linker errors
        Dim infoLogLink As String = GL.GetProgramInfoLog(Handle)

        If infoLogLink <> System.String.Empty Then
            System.Console.WriteLine(infoLogLink)
        End If


        'Now that it's done, clean up.
        'When the shader program Is linked, it no longer needs the individual shaders attacked to it; the compiled code Is copied into the shader program.
        'Detact them, And then delete them.
        GL.DetachShader(Handle, VertexShader)
        GL.DetachShader(Handle, FragmentShader)
        GL.DeleteShader(FragmentShader)
        GL.DeleteShader(VertexShader)

    End Sub

    'A wrapper function that enables the shader program.
    Public Sub Use()
        GL.UseProgram(Handle)
    End Sub

    'The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
    'you can omit the layout(location=X) lines in the vertex shader, And use this in VertexAttribPointer instead of the hardcoded values.
    Public Function GetAttribLocation(ByRef attribName As String) As Integer
        Return GL.GetAttribLocation(Handle, attribName)
    End Function

    'Just loads the entire file into a string.
    Private Function LoadSource(ByRef path As String) As String
        Dim readContents As String

        Using streamReader As StreamReader = New StreamReader(path, Encoding.UTF8)
            readContents = streamReader.ReadToEnd()
        End Using

        Return readContents
    End Function


    Public Sub SetInt(ByVal name As String, ByVal data As Integer)
        GL.UseProgram(Handle)
        Dim location = GL.GetUniformLocation(Handle, name)
        GL.Uniform1(location, data)
    End Sub


    Public Sub SetMatrix4(ByVal name As String, ByVal data As Matrix4)
        GL.UseProgram(Handle)
        Dim location = GL.GetUniformLocation(Handle, name)
        GL.UniformMatrix4(location, True, data)
    End Sub

    'This section Is dedicated to cleaning up the shader after it's finished.
    'Doing this solely in a finalizer results in a crash because of the Object-Oriented Language Problem

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not disposedValue Then

            If disposing Then

            End If

            GL.DeleteProgram(Handle)
            disposedValue = True
        End If
    End Sub

    Protected Overrides Sub Finalize()
        GL.DeleteProgram(Handle)
    End Sub

    Public Sub Dispose()
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
