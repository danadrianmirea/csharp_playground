using Box2D;
using static SDL2.SDL;

namespace Box2DTest;

internal class Program
{
    private const int WindowWidth = 1280;
    private const int WindowHeight = 720;
    private const float PixelsPerMeter = 30f;
    private const int SubStepCount = 4;
    private const int BallCount = 30;

    private static IntPtr _window;
    private static IntPtr _renderer;
    private static World _world = null!;
    private static bool _running = true;

    static void Main(string[] args)
    {
        // Initialize SDL
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL_GetError()}");
            return;
        }

        // Create window
        _window = SDL_CreateWindow(
            "Box2D Falling Balls (Debug Draw)",
            SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
            WindowWidth, WindowHeight,
            SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (_window == IntPtr.Zero)
        {
            Console.WriteLine($"Window could not be created! SDL_Error: {SDL_GetError()}");
            SDL_Quit();
            return;
        }

        // Create renderer
        _renderer = SDL_CreateRenderer(
            _window, -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
            SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (_renderer == IntPtr.Zero)
        {
            Console.WriteLine($"Renderer could not be created! SDL_Error: {SDL_GetError()}");
            SDL_DestroyWindow(_window);
            SDL_Quit();
            return;
        }

        // Initialize Box2D world with gravity
        WorldDef worldDef = new WorldDef
        {
            Gravity = new System.Numerics.Vector2(0, 10f)
        };
        _world = new World(worldDef);

        // Create ground plane
        CreateGround();

        // Spawn balls
        SpawnBalls();

        Console.WriteLine("Box2D Falling Balls Demo (Debug Draw) started. Close the window or press ESC to exit.");

        // Main loop
        RunLoop();

        // Cleanup
        Cleanup();
    }

    private static void CreateGround()
    {
        BodyDef groundBodyDef = new BodyDef
        {
            Position = new System.Numerics.Vector2(
                WindowWidth / 2f / PixelsPerMeter,
                (WindowHeight - 20f) / PixelsPerMeter),
            Name = "Ground"
        };

        Body groundBody = _world.CreateBody(groundBodyDef);

        // Create ground shape (a wide rectangle)
        Polygon groundShape = Polygon.MakeBox(
            WindowWidth / 2f / PixelsPerMeter,  // half-width in meters
            10f / PixelsPerMeter                 // half-height in meters
        );

        ShapeDef shapeDef = new ShapeDef
        {
            Material = new SurfaceMaterial
            {
                Friction = 0.3f,
                Restitution = 0.1f
            }
        };

        groundBody.CreateShape(shapeDef, groundShape);
    }

    private static void SpawnBalls()
    {
        Random random = new Random();

        for (int i = 0; i < BallCount; i++)
        {
            BodyDef ballBodyDef = new BodyDef
            {
                Position = new System.Numerics.Vector2(
                    (float)(random.NextDouble() * (WindowWidth - 100) + 50) / PixelsPerMeter,
                    (float)(random.NextDouble() * 200 + 50) / PixelsPerMeter),
                Type = BodyType.Dynamic,
                Name = $"Ball_{i}"
            };

            Body ballBody = _world.CreateBody(ballBodyDef);

            // Random radius between 8 and 20 pixels
            float radiusPixels = (float)(random.NextDouble() * 12 + 8);
            float radiusMeters = radiusPixels / PixelsPerMeter;

            Circle circle = new Circle
            {
                Center = System.Numerics.Vector2.Zero,
                Radius = radiusMeters
            };

            ShapeDef shapeDef = new ShapeDef
            {
                Density = 1.0f,
                Material = new SurfaceMaterial
                {
                    Friction = 0.3f,
                    Restitution = 0.5f
                }
            };

            ballBody.CreateShape(shapeDef, circle);
        }
    }

    private static void RunLoop()
    {
        uint lastTime = SDL_GetTicks();

        // Create the debug draw instance
        SdlDebugDraw debugDraw = new SdlDebugDraw(_renderer, PixelsPerMeter);

        while (_running)
        {
            // Handle events
            SDL_Event e;
            while (SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL_EventType.SDL_QUIT)
                {
                    _running = false;
                }
                else if (e.type == SDL_EventType.SDL_KEYDOWN)
                {
                    if (e.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                    {
                        _running = false;
                    }
                }
            }

            // Fixed time step
            uint currentTime = SDL_GetTicks();
            float deltaTime = (currentTime - lastTime) / 1000f;
            lastTime = currentTime;

            // Clamp delta time to avoid spiral of death
            if (deltaTime > 0.05f)
                deltaTime = 0.05f;

            // Step the physics world
            _world.Step(deltaTime, SubStepCount);

            // Render using debug draw
            Render(debugDraw);
        }
    }

    private static void Render(SdlDebugDraw debugDraw)
    {
        // Clear screen with dark blue background
        SDL_SetRenderDrawColor(_renderer, 30, 30, 50, 255);
        SDL_RenderClear(_renderer);

        // Use Box2D debug draw to render all objects
        _world.Draw(debugDraw);

        // Present
        SDL_RenderPresent(_renderer);
    }

    private static void Cleanup()
    {
        _world.Destroy();
        SDL_DestroyRenderer(_renderer);
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }
}

/// <summary>
/// Custom debug draw implementation that renders Box2D shapes using SDL2.
/// </summary>
internal class SdlDebugDraw : DebugDrawSimpleBase
{
    private readonly IntPtr _renderer;
    private readonly float _pixelsPerMeter;

    public SdlDebugDraw(IntPtr renderer, float pixelsPerMeter)
    {
        _renderer = renderer;
        _pixelsPerMeter = pixelsPerMeter;

        // Enable drawing shapes
        DrawShapes = true;
        DrawJoints = false;
        DrawBounds = false;
        DrawMass = false;
        DrawBodyNames = false;
        DrawContacts = false;
        DrawContactNormals = false;
        DrawContactImpulses = false;
        DrawFrictionImpulses = false;
        DrawContactFeatures = false;
        DrawIslands = false;
        DrawGraphColors = false;
        DrawJointExtras = false;
    }

    private int ToScreenX(float worldX) => (int)(worldX * _pixelsPerMeter);
    private int ToScreenY(float worldY) => (int)(worldY * _pixelsPerMeter);

    private void SetColor(HexColor color)
    {
        byte r = color.Red();
        byte g = color.Green();
        byte b = color.Blue();
        SDL_SetRenderDrawColor(_renderer, r, g, b, 255);
    }

    protected override void DrawPolygon(ReadOnlySpan<System.Numerics.Vector2> vertices, HexColor color)
    {
        if (vertices.Length < 2)
            return;

        SetColor(color);

        for (int i = 0; i < vertices.Length; i++)
        {
            int next = (i + 1) % vertices.Length;
            int x1 = ToScreenX(vertices[i].X);
            int y1 = ToScreenY(vertices[i].Y);
            int x2 = ToScreenX(vertices[next].X);
            int y2 = ToScreenY(vertices[next].Y);
            SDL_RenderDrawLine(_renderer, x1, y1, x2, y2);
        }
    }

    protected override void DrawSolidPolygon(Transform transform, ReadOnlySpan<System.Numerics.Vector2> vertices, float radius, HexColor color)
    {
        if (vertices.Length < 2)
            return;

        // Transform all vertices
        System.Numerics.Vector2[] transformed = new System.Numerics.Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            transformed[i] = Core.TransformPoint(transform, vertices[i]);
        }

        // Draw filled polygon
        DrawFilledPolygon(transformed, color);

        // Draw outline (slightly brighter)
        DrawPolygon(transformed, color);
    }

    private void DrawFilledPolygon(System.Numerics.Vector2[] vertices, HexColor color)
    {
        if (vertices.Length < 3)
            return;

        // Find min/max y in screen coordinates
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        int[] screenX = new int[vertices.Length];
        int[] screenY = new int[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            screenX[i] = ToScreenX(vertices[i].X);
            screenY[i] = ToScreenY(vertices[i].Y);
            if (screenY[i] < minY) minY = screenY[i];
            if (screenY[i] > maxY) maxY = screenY[i];
        }

        SetColor(color);

        // Scanline fill
        for (int y = minY; y <= maxY; y++)
        {
            var intersections = new System.Collections.Generic.List<int>();

            for (int i = 0; i < vertices.Length; i++)
            {
                int j = (i + 1) % vertices.Length;
                int y1 = screenY[i];
                int y2 = screenY[j];

                if ((y1 <= y && y2 > y) || (y2 <= y && y1 > y))
                {
                    float t = (float)(y - y1) / (y2 - y1);
                    int x = (int)(screenX[i] + t * (screenX[j] - screenX[i]));
                    intersections.Add(x);
                }
            }

            intersections.Sort();
            for (int i = 0; i + 1 < intersections.Count; i += 2)
            {
                int x1 = intersections[i];
                int x2 = intersections[i + 1];
                SDL_RenderDrawLine(_renderer, x1, y, x2, y);
            }
        }
    }

    protected override void DrawCircle(System.Numerics.Vector2 center, float radius, HexColor color)
    {
        SetColor(color);
        int cx = ToScreenX(center.X);
        int cy = ToScreenY(center.Y);
        int r = (int)(radius * _pixelsPerMeter);

        DrawCircleOutline(cx, cy, r);
    }

    protected override void DrawSolidCircle(Transform transform, float radius, HexColor color)
    {
        System.Numerics.Vector2 center = transform.Position;
        int cx = ToScreenX(center.X);
        int cy = ToScreenY(center.Y);
        int r = (int)(radius * _pixelsPerMeter);

        // Draw filled circle
        SetColor(color);
        DrawFilledCircle(cx, cy, r);

        // Draw outline
        DrawCircleOutline(cx, cy, r);
    }

    protected override void DrawSolidCapsule(System.Numerics.Vector2 start, System.Numerics.Vector2 end, float radius, HexColor color)
    {
        SetColor(color);

        int x1 = ToScreenX(start.X);
        int y1 = ToScreenY(start.Y);
        int x2 = ToScreenX(end.X);
        int y2 = ToScreenY(end.Y);
        int r = (int)(radius * _pixelsPerMeter);

        // Draw the rectangle body
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float len = (float)System.Math.Sqrt(dx * dx + dy * dy);
        if (len > 0.001f)
        {
            float nx = -dy / len;
            float ny = dx / len;

            int ox = (int)(nx * radius * _pixelsPerMeter);
            int oy = (int)(ny * radius * _pixelsPerMeter);

            // Draw filled quad
            SDL_Point[] points = new SDL_Point[]
            {
                new SDL_Point { x = x1 + ox, y = y1 + oy },
                new SDL_Point { x = x1 - ox, y = y1 - oy },
                new SDL_Point { x = x2 - ox, y = y2 - oy },
                new SDL_Point { x = x2 + ox, y = y2 + oy }
            };

            // Simple filled quad via scanlines
            int minY = System.Math.Min(System.Math.Min(points[0].y, points[1].y), System.Math.Min(points[2].y, points[3].y));
            int maxY = System.Math.Max(System.Math.Max(points[0].y, points[1].y), System.Math.Max(points[2].y, points[3].y));

            for (int y = minY; y <= maxY; y++)
            {
                var intersections = new System.Collections.Generic.List<int>();
                for (int i = 0; i < 4; i++)
                {
                    int j = (i + 1) % 4;
                    int y1p = points[i].y;
                    int y2p = points[j].y;
                    if ((y1p <= y && y2p > y) || (y2p <= y && y1p > y))
                    {
                        float t = (float)(y - y1p) / (y2p - y1p);
                        int x = (int)(points[i].x + t * (points[j].x - points[i].x));
                        intersections.Add(x);
                    }
                }
                intersections.Sort();
                for (int i = 0; i + 1 < intersections.Count; i += 2)
                {
                    SDL_RenderDrawLine(_renderer, intersections[i], y, intersections[i + 1], y);
                }
            }
        }

        // Draw end circles
        DrawFilledCircle(x1, y1, r);
        DrawFilledCircle(x2, y2, r);
    }

    protected override void DrawSegment(System.Numerics.Vector2 start, System.Numerics.Vector2 end, HexColor color)
    {
        SetColor(color);
        int x1 = ToScreenX(start.X);
        int y1 = ToScreenY(start.Y);
        int x2 = ToScreenX(end.X);
        int y2 = ToScreenY(end.Y);
        SDL_RenderDrawLine(_renderer, x1, y1, x2, y2);
    }

    protected override void DrawTransform(Transform transform)
    {
        // Draw the transform axes
        System.Numerics.Vector2 p = transform.Position;
        float axisLength = 0.4f;

        // X axis (red)
        System.Numerics.Vector2 right = p + new System.Numerics.Vector2(transform.Rotation.Cos, transform.Rotation.Sin) * axisLength;
        SDL_SetRenderDrawColor(_renderer, 255, 0, 0, 255);
        SDL_RenderDrawLine(_renderer, ToScreenX(p.X), ToScreenY(p.Y), ToScreenX(right.X), ToScreenY(right.Y));

        // Y axis (green)
        System.Numerics.Vector2 up = p + new System.Numerics.Vector2(-transform.Rotation.Sin, transform.Rotation.Cos) * axisLength;
        SDL_SetRenderDrawColor(_renderer, 0, 255, 0, 255);
        SDL_RenderDrawLine(_renderer, ToScreenX(p.X), ToScreenY(p.Y), ToScreenX(up.X), ToScreenY(up.Y));
    }

    protected override void DrawPoint(System.Numerics.Vector2 point, float size, HexColor color)
    {
        SetColor(color);
        int px = ToScreenX(point.X);
        int py = ToScreenY(point.Y);
        int s = (int)(size * _pixelsPerMeter);
        if (s < 1) s = 1;

        SDL_Rect rect = new SDL_Rect
        {
            x = px - s / 2,
            y = py - s / 2,
            w = s,
            h = s
        };
        SDL_RenderFillRect(_renderer, ref rect);
    }

    protected override void DrawString(System.Numerics.Vector2 point, string? text, HexColor color)
    {
        // SDL2 doesn't have built-in text rendering without SDL_ttf
        // Skip string drawing for now
    }

    private void DrawCircleOutline(int cx, int cy, int r)
    {
        if (r <= 0) return;

        for (int angle = 0; angle < 360; angle += 5)
        {
            double rad = angle * System.Math.PI / 180.0;
            int x1 = cx + (int)((r - 1) * System.Math.Cos(rad));
            int y1 = cy + (int)((r - 1) * System.Math.Sin(rad));
            int x2 = cx + (int)(r * System.Math.Cos(rad));
            int y2 = cy + (int)(r * System.Math.Sin(rad));
            SDL_RenderDrawLine(_renderer, x1, y1, x2, y2);
        }
    }

    private void DrawFilledCircle(int cx, int cy, int r)
    {
        if (r <= 0) return;

        for (int y = -r; y <= r; y++)
        {
            int xWidth = (int)System.Math.Sqrt(r * r - y * y);
            int x1 = cx - xWidth;
            int x2 = cx + xWidth;
            SDL_RenderDrawLine(_renderer, x1, cy + y, x2, cy + y);
        }
    }
}
