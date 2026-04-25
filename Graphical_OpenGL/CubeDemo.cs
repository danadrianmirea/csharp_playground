using System.ComponentModel.Design.Serialization;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

    // Crosshair resources
    private int _crosshairVAO;
    private int _crosshairVBO;
    private int _crosshairShaderProgram;
    private int _crosshairProjectionLocation;

    private double _time;

    // ---- Camera state ----
    private Vector3 _cameraPosition = new(2.5f, 2.5f, 2.5f);
    private Vector3 _cameraFront = -Vector3.UnitZ;
    private Vector3 _cameraUp = Vector3.UnitY;

    private float _yaw = -135f;   // degrees, so initial front points toward origin
    private float _pitch = -35f;  // degrees, looking down slightly toward origin

    private float _moveSpeed = 3.0f;
    private float _mouseSensitivity = 0.2f;

    private bool _isRightMouseDown = false;
    private Vector2 _lastMousePosition;

    private bool _firstMove = true;

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

        // Set up projection (this won't change per frame)
        GL.UseProgram(_shaderProgram);

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

        // ---- Set up crosshair ----
        SetupCrosshair();

        Console.WriteLine("OpenGL Cube Demo loaded. Close the window to exit.");

    }

    private void SetupCrosshair()
    {
        // Crosshair: a small circle approximated by a line loop (32 segments)
        const int segments = 32;
        const float radius = 5f; // 5 pixels
        float[] crosshairVertices = new float[segments * 2]; // x, y only (screen-space 2D)

        for (int i = 0; i < segments; i++)
        {
            float angle = MathHelper.TwoPi * i / segments;
            crosshairVertices[i * 2] = MathF.Cos(angle) * radius;
            crosshairVertices[i * 2 + 1] = MathF.Sin(angle) * radius;
        }

        _crosshairVAO = GL.GenVertexArray();
        GL.BindVertexArray(_crosshairVAO);

        _crosshairVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _crosshairVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, crosshairVertices.Length * sizeof(float),
                      crosshairVertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Simple 2D shader for crosshair (no model/view/projection, just screen coords)
        string crosshairVertexSource = @"
            #version 330 core
            layout (location = 0) in vec2 aPos;

            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * vec4(aPos, 0.0, 1.0);
            }
        ";

        string crosshairFragmentSource = @"
            #version 330 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(1.0, 1.0, 1.0, 1.0); // white
            }
        ";

        int vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, crosshairVertexSource);
        GL.CompileShader(vs);
        CheckShaderCompilation(vs, "CROSSHAIR_VERTEX");

        int fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, crosshairFragmentSource);
        GL.CompileShader(fs);
        CheckShaderCompilation(fs, "CROSSHAIR_FRAGMENT");

        _crosshairShaderProgram = GL.CreateProgram();
        GL.AttachShader(_crosshairShaderProgram, vs);
        GL.AttachShader(_crosshairShaderProgram, fs);
        GL.LinkProgram(_crosshairShaderProgram);
        CheckProgramLink(_crosshairShaderProgram);

        GL.DetachShader(_crosshairShaderProgram, vs);
        GL.DetachShader(_crosshairShaderProgram, fs);
        GL.DeleteShader(vs);
        GL.DeleteShader(fs);

        _crosshairProjectionLocation = GL.GetUniformLocation(_crosshairShaderProgram, "projection");
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // ---- Draw 3D scene ----
        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vertexArrayObject);

        // Update rotation
        _time += args.Time;

        // Model matrix: rotate around Y and X axes
        var modelMatrix = Matrix4.Identity;
        GL.UniformMatrix4(_modelLocation, false, ref modelMatrix);

        // Update view matrix from camera state
        var viewMatrix = Matrix4.LookAt(_cameraPosition, _cameraPosition + _cameraFront, _cameraUp);
        GL.UniformMatrix4(_viewLocation, false, ref viewMatrix);

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length,
                        DrawElementsType.UnsignedInt, 0);

        // Draw thick black edges on top
        GL.BindVertexArray(_edgeVAO);
        GL.LineWidth(3.0f);
        GL.DrawArrays(PrimitiveType.Lines, 0, _edgeVertices.Length / 6);
        GL.LineWidth(1.0f);

        // ---- Draw crosshair overlay (no depth test, screen-space) ----
        GL.Disable(EnableCap.DepthTest);

        GL.UseProgram(_crosshairShaderProgram);

        // Orthographic projection matching screen size, centered at (0,0)
        float halfW = ClientSize.X / 2f;
        float halfH = ClientSize.Y / 2f;
        var ortho = Matrix4.CreateOrthographicOffCenter(-halfW, halfW, -halfH, halfH, -1f, 1f);
        GL.UniformMatrix4(_crosshairProjectionLocation, false, ref ortho);

        GL.BindVertexArray(_crosshairVAO);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, 32);

        GL.Enable(EnableCap.DepthTest);

        SwapBuffers();

    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        float deltaTime = (float)args.Time;

        // ---- Keyboard movement ----
        Vector3 right = Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp));

        if (KeyboardState.IsKeyDown(Keys.W))
            _cameraPosition += _cameraFront * _moveSpeed * deltaTime;
        if (KeyboardState.IsKeyDown(Keys.S))
            _cameraPosition -= _cameraFront * _moveSpeed * deltaTime;
        if (KeyboardState.IsKeyDown(Keys.A))
            _cameraPosition -= right * _moveSpeed * deltaTime;
        if (KeyboardState.IsKeyDown(Keys.D))
            _cameraPosition += right * _moveSpeed * deltaTime;
        if (KeyboardState.IsKeyDown(Keys.Space))
            _cameraPosition += _cameraUp * _moveSpeed * deltaTime;
        if (KeyboardState.IsKeyDown(Keys.C))
            _cameraPosition -= _cameraUp * _moveSpeed * deltaTime;

        // ---- Mouse freelook (right-click held) ----
        if (_isRightMouseDown)
        {
            if (_firstMove)
            {
                _firstMove = false;
            }
            else
            {
                // Use MouseState.Delta for cursor movement delta when grabbed
                Vector2 mouseDelta = MouseState.Delta;

                _yaw += mouseDelta.X * _mouseSensitivity;
                _pitch -= mouseDelta.Y * _mouseSensitivity;

                // Clamp pitch to avoid gimbal lock
                _pitch = MathHelper.Clamp(_pitch, -89f, 89f);
            }

            UpdateCameraVectors();
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButton.Right)
        {
            _isRightMouseDown = true;
            _firstMove = true;

            // Hide cursor and trap it (CursorState.Grabbed also hides the cursor)
            CursorState = CursorState.Grabbed;
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.Button == MouseButton.Right)
        {
            _isRightMouseDown = false;

            // Show cursor and release it
            CursorState = CursorState.Normal;
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
        GL.DeleteBuffer(_crosshairVBO);
        GL.DeleteVertexArray(_crosshairVAO);
        GL.DeleteProgram(_shaderProgram);
        GL.DeleteProgram(_crosshairShaderProgram);

        base.OnUnload();

    }

    private void UpdateCameraVectors()
    {
        // Spherical to Cartesian conversion
        float yawRad = MathHelper.DegreesToRadians(_yaw);
        float pitchRad = MathHelper.DegreesToRadians(_pitch);

        _cameraFront.X = MathF.Cos(pitchRad) * MathF.Cos(yawRad);
        _cameraFront.Y = MathF.Sin(pitchRad);
        _cameraFront.Z = MathF.Cos(pitchRad) * MathF.Sin(yawRad);
        _cameraFront = Vector3.Normalize(_cameraFront);
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