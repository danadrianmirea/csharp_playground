using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Graphical_OpenGL;

public class CubeDemo : GameWindow
{
    // Cube vertices: position (3) + color (3)
    private readonly float[] _vertices =
    {
        // Positions          // Colors          
        -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f, // 0: Front-bottom-left
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f, // 1: Front-bottom-right
         0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f, // 2: Front-top-right
        -0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f, // 3: Front-top-left
        -0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 1.0f, // 4: Back-bottom-left
         0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 1.0f, // 5: Back-bottom-right
         0.5f,  0.5f,  0.5f,  0.5f, 0.5f, 1.0f, // 6: Back-top-right
        -0.5f,  0.5f,  0.5f,  1.0f, 0.5f, 0.5f  // 7: Back-top-left
    };

    // Triangle indices for solid faces
    private readonly uint[] _indices =
    {
        // Front face
        0, 1, 2,
        2, 3, 0,
        // Back face
        4, 5, 6,
        6, 7, 4,
        // Left face
        0, 3, 7,
        7, 4, 0,
        // Right face
        1, 5, 6,
        6, 2, 1,
        // Top face
        3, 2, 6,
        6, 7, 3,
        // Bottom face
        0, 1, 5,
        5, 4, 0
    };

    // Edge vertices for thick black outline (12 edges × 2 vertices each)
    // Each vertex: position (3) + color (3, black)
    private readonly float[] _edgeVertices =
    {
        // Front face edges
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
        // Back face edges
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
        // Connecting edges (front to back)
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 0.0f,
    };

    private int _vertexArrayObject;
    private int _vertexBufferObject;
    private int _elementBufferObject;
    private int _shaderProgram;
    private int _modelLocation;
    private int _viewLocation;
    private int _projectionLocation;

    // Edge outline resources
    private int _edgeVAO;
    private int _edgeVBO;

    private double _time;


    public CubeDemo()
        : base(GameWindowSettings.Default,
               new NativeWindowSettings
               {
                   ClientSize = new Vector2i(800, 600),
                   Title = "Rotating 3D Cube - OpenTK",
                   // This is needed for OpenGL 4.x on Windows
                   APIVersion = new Version(3, 3),
                   Profile = ContextProfile.Core,
                   Flags = ContextFlags.ForwardCompatible
               })

    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.2f, 0.2f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        // Generate and bind VAO
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        // Generate and fill VBO
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float),
                      _vertices, BufferUsageHint.StaticDraw);

        // Generate and fill EBO
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint),
                      _indices, BufferUsageHint.StaticDraw);

        // Vertex position attribute (location = 0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Vertex color attribute (location = 1)
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Create shaders
        string vertexShaderSource = @"
            #version 330 core
            layout (location = 0) in vec3 aPosition;
            layout (location = 1) in vec3 aColor;

            out vec3 vColor;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * view * model * vec4(aPosition, 1.0);
                vColor = aColor;
            }
        ";

        string fragmentShaderSource = @"
            #version 330 core
            in vec3 vColor;
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(vColor, 1.0);
            }
        ";

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompilation(vertexShader, "VERTEX");

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompilation(fragmentShader, "FRAGMENT");

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);
        CheckProgramLink(_shaderProgram);

        GL.DetachShader(_shaderProgram, vertexShader);
        GL.DetachShader(_shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        // Get uniform locations
        _modelLocation = GL.GetUniformLocation(_shaderProgram, "model");
        _viewLocation = GL.GetUniformLocation(_shaderProgram, "view");
        _projectionLocation = GL.GetUniformLocation(_shaderProgram, "projection");

        // Set up view and projection (these won't change per frame)
        GL.UseProgram(_shaderProgram);

        // View matrix: camera at (2, 2, 2) looking at origin
        var viewMatrix = Matrix4.LookAt(
            new Vector3(2.5f, 2.5f, 2.5f),
            Vector3.Zero,
            Vector3.UnitY);
        GL.UniformMatrix4(_viewLocation, false, ref viewMatrix);

        // Projection matrix: perspective
        float aspectRatio = ClientSize.X / (float)ClientSize.Y;

        var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            aspectRatio,
            0.1f,
            100f);
        GL.UniformMatrix4(_projectionLocation, false, ref projectionMatrix);

        GL.UseProgram(0);

        // ---- Set up edge VAO ----
        _edgeVAO = GL.GenVertexArray();
        GL.BindVertexArray(_edgeVAO);

        _edgeVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _edgeVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _edgeVertices.Length * sizeof(float),
                      _edgeVertices, BufferUsageHint.StaticDraw);

        // Vertex position attribute (location = 0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Vertex color attribute (location = 1)
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);

        Console.WriteLine("OpenGL Cube Demo loaded. Close the window to exit.");

    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vertexArrayObject);

        // Update rotation
        _time += args.Time;

        // Model matrix: rotate around Y and X axes
        var modelMatrix = Matrix4.CreateRotationY((float)_time) *
                          Matrix4.CreateRotationX((float)_time * 0.5f);
        GL.UniformMatrix4(_modelLocation, false, ref modelMatrix);

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length,
                        DrawElementsType.UnsignedInt, 0);

        // Draw thick black edges on top
        GL.BindVertexArray(_edgeVAO);
        GL.LineWidth(3.0f);
        GL.DrawArrays(PrimitiveType.Lines, 0, _edgeVertices.Length / 6);
        GL.LineWidth(1.0f);

        SwapBuffers();

    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

        // Update projection matrix on resize
        if (_shaderProgram != 0)
        {
            GL.UseProgram(_shaderProgram);
            float aspectRatio = ClientSize.X / (float)ClientSize.Y;

            var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                aspectRatio,
                0.1f,
                100f);
            GL.UniformMatrix4(_projectionLocation, false, ref projectionMatrix);
            GL.UseProgram(0);
        }
    }

    protected override void OnUnload()
    {
        // Clean up resources
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteBuffer(_elementBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteBuffer(_edgeVBO);
        GL.DeleteVertexArray(_edgeVAO);
        GL.DeleteProgram(_shaderProgram);

        base.OnUnload();

    }

    private static void CheckShaderCompilation(int shader, string type)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Shader compilation error ({type}): {infoLog}");
        }
    }

    private static void CheckProgramLink(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            throw new Exception($"Program link error: {infoLog}");
        }
    }
}
