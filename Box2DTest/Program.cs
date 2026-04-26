using Box2DX.Common;
using Box2DX.Dynamics;
using Box2DX.Collision;
using static SDL2.SDL;

namespace Box2DTest;

internal class Program
{
    private const int WindowWidth = 1280;
    private const int WindowHeight = 720;
    private const float PixelsPerMeter = 30f;
    private const int VelocityIterations = 8;
    private const int PositionIterations = 3;
    private const int BallCount = 50;

    private static IntPtr _window;
    private static IntPtr _renderer;
    private static World _world;
    private static readonly List<Body> _balls = new();
    private static readonly List<float> _ballRadii = new();
    private static Body _groundBody;
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
            "Box2D Falling Balls",
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
        // World bounds (large enough to contain our scene)
        AABB worldAABB = new AABB();
        worldAABB.LowerBound = new Vec2(-100, -100);
        worldAABB.UpperBound = new Vec2(100, 100);
        _world = new World(worldAABB, new Vec2(0, 30f), true); // gravity pointing down, doSleep = true

        // Create ground plane
        CreateGround();

        // Spawn balls
        SpawnBalls();

        Console.WriteLine("Box2D Falling Balls Demo started. Close the window or press ESC to exit.");

        // Main loop
        RunLoop();

        // Cleanup
        Cleanup();
    }

    private static void CreateGround()
    {
        BodyDef groundBodyDef = new BodyDef();
        groundBodyDef.Position = new Vec2(WindowWidth / 2f / PixelsPerMeter, (WindowHeight - 20f) / PixelsPerMeter);

        _groundBody = _world.CreateBody(groundBodyDef);

        // Create ground shape (a wide rectangle)
        PolygonDef groundShape = new PolygonDef();
        groundShape.SetAsBox(
            WindowWidth / 2f / PixelsPerMeter,  // half-width in meters
            10f / PixelsPerMeter                 // half-height in meters
        );
        groundShape.Friction = 0.3f;
        groundShape.Restitution = 0.1f;
        groundShape.Density = 0f; // static bodies don't need density

        _groundBody.CreateFixture(groundShape);
    }

    private static void SpawnBalls()
    {
        Random random = new Random();

        for (int i = 0; i < BallCount; i++)
        {
            BodyDef ballBodyDef = new BodyDef();
            ballBodyDef.Position = new Vec2(
                (float)(random.NextDouble() * (WindowWidth - 100) + 50) / PixelsPerMeter,
                (float)(random.NextDouble() * 200 + 50) / PixelsPerMeter
            );

            Body ballBody = _world.CreateBody(ballBodyDef);

            // Random radius between 8 and 20 pixels
            float radiusPixels = (float)(random.NextDouble() * 12 + 8);
            float radiusMeters = radiusPixels / PixelsPerMeter;

            CircleDef circleDef = new CircleDef();
            circleDef.Radius = radiusMeters;
            circleDef.Density = 1.0f;
            circleDef.Friction = 0.3f;
            circleDef.Restitution = 0.5f;

            ballBody.CreateFixture(circleDef);
            ballBody.SetMassFromShapes();

            _balls.Add(ballBody);
            _ballRadii.Add(radiusPixels);
        }
    }

    private static void RunLoop()
    {
        uint lastTime = SDL_GetTicks();

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
            _world.Step(deltaTime, VelocityIterations, PositionIterations);

            // Render
            Render();
        }
    }

    private static void Render()
    {
        // Clear screen with dark blue background
        SDL_SetRenderDrawColor(_renderer, 30, 30, 50, 255);
        SDL_RenderClear(_renderer);

        // Draw ground
        DrawGround();

        // Draw balls
        for (int i = 0; i < _balls.Count; i++)
        {
            DrawBall(_balls[i], _ballRadii[i]);
        }

        // Present
        SDL_RenderPresent(_renderer);
    }

    private static void DrawGround()
    {
        Vec2 position = _groundBody.GetPosition();
        float halfWidth = WindowWidth / 2f / PixelsPerMeter;
        float halfHeight = 10f / PixelsPerMeter;

        // Ground rectangle in world coordinates
        float left = (position.X - halfWidth) * PixelsPerMeter;
        float top = (position.Y - halfHeight) * PixelsPerMeter;
        float right = (position.X + halfWidth) * PixelsPerMeter;
        float bottom = (position.Y + halfHeight) * PixelsPerMeter;

        SDL_Rect groundRect = new SDL_Rect
        {
            x = (int)left,
            y = (int)top,
            w = (int)(right - left),
            h = (int)(bottom - top)
        };

        SDL_SetRenderDrawColor(_renderer, 100, 180, 100, 255);
        SDL_RenderFillRect(_renderer, ref groundRect);

        // Draw a line on top of the ground
        SDL_SetRenderDrawColor(_renderer, 150, 220, 150, 255);
        SDL_RenderDrawLine(_renderer, (int)left, (int)top, (int)right, (int)top);
    }

    private static void DrawBall(Body ball, float radiusPixels)
    {
        Vec2 position = ball.GetPosition();
        int screenX = (int)(position.X * PixelsPerMeter);
        int screenY = (int)(position.Y * PixelsPerMeter);
        int radius = (int)radiusPixels;

        DrawFilledCircle(screenX, screenY, radius);
    }

    private static void DrawFilledCircle(int centerX, int centerY, int radius)
    {
        // Draw filled circle using horizontal scanlines
        for (int y = -radius; y <= radius; y++)
        {
            int xWidth = (int)System.Math.Sqrt(radius * radius - y * y);
            int x1 = centerX - xWidth;
            int x2 = centerX + xWidth;

            SDL_SetRenderDrawColor(_renderer, 200, 80, 80, 255);
            SDL_RenderDrawLine(_renderer, x1, centerY + y, x2, centerY + y);
        }

        // Draw circle outline
        SDL_SetRenderDrawColor(_renderer, 255, 150, 150, 255);
        for (int angle = 0; angle < 360; angle += 5)
        {
            double rad = angle * System.Math.PI / 180.0;
            int x1 = centerX + (int)((radius - 1) * System.Math.Cos(rad));
            int y1 = centerY + (int)((radius - 1) * System.Math.Sin(rad));
            int x2 = centerX + (int)(radius * System.Math.Cos(rad));
            int y2 = centerY + (int)(radius * System.Math.Sin(rad));
            SDL_RenderDrawLine(_renderer, x1, y1, x2, y2);
        }
    }

    private static void Cleanup()
    {
        _world.Dispose();
        SDL_DestroyRenderer(_renderer);
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }
}
