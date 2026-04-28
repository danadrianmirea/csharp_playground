using Raylib_cs;
using System.Numerics;
using System.Diagnostics;
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;

namespace Raylib2DWater;

/// <summary>
/// 2D water simulation using a particle system with pressure-based physics.
/// Particles repel each other when too close, simulating incompressible fluid behavior.
/// </summary>
class Program
{
    // --- Window & Simulation Constants ---
    const int ScreenWidth = 960;
    const int ScreenHeight = 640;
    const int ParticleCount = 3000;
    const float ParticleRadius = 6.0f;
    const float Gravity = 300.0f;
    const float RestDensity = 1.0f;
    const float GasConstant = 12000.0f;
    const float Viscosity = 0.2f;
    const float H = 40.0f; // interaction radius
    const float Hsq = H * H;
    const float Mass = 1.0f;

    // --- Container boundaries ---
    const float ContainerLeft = 40;
    const float ContainerRight = ScreenWidth - 40;
    const float ContainerTop = 40;
    const float ContainerBottom = ScreenHeight - 40;

    // --- Spatial grid for neighbor search ---
    const int GridCols = (int)(ScreenWidth / H) + 3;
    const int GridRows = (int)(ScreenHeight / H) + 3;

    struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Force;
        public float Density;
        public float Pressure;
        public Color Color;
    }

    static Particle[] particles = new Particle[ParticleCount];
    static Random rng = new Random();

    // Spatial grid
    static int[] gridHeads = new int[GridCols * GridRows];
    static int[] gridNext = new int[ParticleCount];
    static int[] gridCellIdx = new int[ParticleCount];

    // Mouse interaction
    static Vector2 mousePos;
    static Vector2 mousePrev;
    static bool mouseDown = false;
    static float mouseRadius = 50.0f;
    static float mouseStrength = 10000.0f;

    // Performance tracking
    static Stopwatch sw = new Stopwatch();
    static double physicsTime = 0;
    static double renderTime = 0;
    static double frameTime = 0;

    // Water level control
    static float waterFillLevel = 0.6f;

    static void Main(string[] args)
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "Raylib C# 2D Water Simulation");
        Raylib.SetTargetFPS(60);

        Array.Fill(gridHeads, -1);

        InitParticles();

        sw.Start();

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();
            if (dt > 0.033f) dt = 0.033f;

            HandleInput();

            // Physics
            sw.Restart();
            UpdatePhysics(dt);
            physicsTime = sw.Elapsed.TotalMilliseconds;

            // Render
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(10, 10, 20, 255));

            DrawContainer();

            sw.Restart();
            DrawParticles();
            renderTime = sw.Elapsed.TotalMilliseconds;

            if (mouseDown)
            {
                Raylib.DrawCircleLines((int)mousePos.X, (int)mousePos.Y, mouseRadius, new Color(255, 255, 255, 60));
            }

            // UI
            int ls = 25;
            int sy = 10;
            Raylib.DrawFPS(10, sy);
            Raylib.DrawText($"Particles: {ParticleCount}", 10, sy + ls, 20, Color.LightGray);
            Raylib.DrawText($"Physics: {physicsTime:F2}ms  Draw: {renderTime:F2}ms  Frame: {frameTime:F2}ms", 10, sy + 2 * ls, 20, Color.LightGray);
            Raylib.DrawText("Left-click+drag to push water  |  R to reset  |  +/- fill  |  [/] mouse strength", 10, sy + 3 * ls, 20, Color.LightGray);
            Raylib.DrawText($"Fill level: {waterFillLevel * 100:F0}%  |  Mouse strength: {mouseStrength:F0}", 10, sy + 4 * ls, 20, Color.LightGray);

            Raylib.EndDrawing();
            frameTime = sw.Elapsed.TotalMilliseconds;
        }

        Raylib.CloseWindow();
    }

    static void HandleInput()
    {
        mousePos = Raylib.GetMousePosition();
        mouseDown = Raylib.IsMouseButtonDown(MouseButton.Left);

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            mousePrev = mousePos;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.R))
        {
            InitParticles();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Equal) || Raylib.IsKeyPressed(KeyboardKey.KpAdd))
        {
            waterFillLevel = Math.Clamp(waterFillLevel + 0.05f, 0.1f, 0.95f);
            InitParticles();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Minus) || Raylib.IsKeyPressed(KeyboardKey.KpSubtract))
        {
            waterFillLevel = Math.Clamp(waterFillLevel - 0.05f, 0.1f, 0.95f);
            InitParticles();
        }

        // Adjust mouse strength with [ and ] keys
        if (Raylib.IsKeyPressed(KeyboardKey.LeftBracket))
        {
            mouseStrength = Math.Max(500, mouseStrength - 500);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.RightBracket))
        {
            mouseStrength = Math.Min(50000, mouseStrength + 500);
        }
    }

    static void InitParticles()
    {
        float containerW = ContainerRight - ContainerLeft;
        float containerH = ContainerBottom - ContainerTop;
        float fillH = containerH * waterFillLevel;

        float spacing = ParticleRadius * 2.2f;
        int cols = (int)(containerW / spacing);
        int rows = (int)(fillH / spacing);

        int idx = 0;
        for (int r = 0; r < rows && idx < ParticleCount; r++)
        {
            for (int c = 0; c < cols && idx < ParticleCount; c++)
            {
                float x = ContainerLeft + spacing * 0.5f + c * spacing + (float)(rng.NextDouble() - 0.5) * spacing * 0.2f;
                float y = ContainerBottom - spacing * 0.5f - r * spacing + (float)(rng.NextDouble() - 0.5) * spacing * 0.2f;

                // Color gradient: lighter at top, darker at bottom
                float t = (float)r / Math.Max(rows - 1, 1);
                byte rCol = (byte)(20 + t * 30);
                byte gCol = (byte)(80 + (1 - t) * 80);
                byte bCol = (byte)(160 + (1 - t) * 95);

                particles[idx] = new Particle
                {
                    Position = new Vector2(x, y),
                    Velocity = Vector2.Zero,
                    Force = Vector2.Zero,
                    Density = 0,
                    Pressure = 0,
                    Color = new Color((int)rCol, (int)gCol, (int)bCol, 220)
                };
                idx++;
            }
        }

        // Mark remaining particles as inactive
        for (; idx < ParticleCount; idx++)
        {
            particles[idx] = new Particle
            {
                Position = new Vector2(-1000, -1000),
                Velocity = Vector2.Zero,
                Force = Vector2.Zero,
                Density = 0,
                Pressure = 0,
                Color = Color.Blue
            };
        }
    }

    static void DrawContainer()
    {
        Raylib.DrawRectangleLinesEx(
            new Rectangle(ContainerLeft, ContainerTop, ContainerRight - ContainerLeft, ContainerBottom - ContainerTop),
            3,
            new Color(60, 80, 120, 255));

        float containerH = ContainerBottom - ContainerTop;
        float fillH = containerH * waterFillLevel;
        float waterSurfaceY = ContainerBottom - fillH;
        Raylib.DrawLine((int)ContainerLeft, (int)waterSurfaceY, (int)ContainerRight, (int)waterSurfaceY, new Color(60, 120, 200, 40));
    }

    static void DrawParticles()
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            Vector2 pos = particles[i].Position;

            if (pos.X < ContainerLeft - 10 || pos.X > ContainerRight + 10 ||
                pos.Y < ContainerTop - 10 || pos.Y > ContainerBottom + 10)
                continue;

            Raylib.DrawCircleV(pos, ParticleRadius, particles[i].Color);
        }
    }

    static void BuildGrid()
    {
        Array.Fill(gridHeads, -1);

        for (int i = 0; i < ParticleCount; i++)
        {
            Vector2 pos = particles[i].Position;

            if (pos.X < ContainerLeft - 10 || pos.X > ContainerRight + 10 ||
                pos.Y < ContainerTop - 10 || pos.Y > ContainerBottom + 10)
            {
                gridCellIdx[i] = -1;
                continue;
            }

            int col = (int)(pos.X / H);
            int row = (int)(pos.Y / H);
            col = Math.Clamp(col, 0, GridCols - 1);
            row = Math.Clamp(row, 0, GridRows - 1);

            int cell = row * GridCols + col;
            gridNext[i] = gridHeads[cell];
            gridHeads[cell] = i;
            gridCellIdx[i] = cell;
        }
    }

    static void UpdatePhysics(float dt)
    {
        int substeps = 6;
        float subDt = dt / substeps;

        for (int step = 0; step < substeps; step++)
        {
            BuildGrid();
            ComputeDensityPressure();
            ComputeForces();
            ApplyMouseForce();
            Integrate(subDt);
            EnforceBoundaries();
        }
    }

    static void ComputeDensityPressure()
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            Vector2 pos = particles[i].Position;
            if (pos.X < ContainerLeft - 10 || pos.X > ContainerRight + 10 ||
                pos.Y < ContainerTop - 10 || pos.Y > ContainerBottom + 10)
                continue;

            float density = 0;

            int cell = gridCellIdx[i];
            if (cell < 0) continue;

            int row = cell / GridCols;
            int col = cell % GridCols;

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int nc = col + dc;
                    int nr = row + dr;
                    if (nc < 0 || nc >= GridCols || nr < 0 || nr >= GridRows) continue;

                    int neighborCell = nr * GridCols + nc;
                    for (int j = gridHeads[neighborCell]; j != -1; j = gridNext[j])
                    {
                        Vector2 delta = particles[j].Position - pos;
                        float distSq = delta.X * delta.X + delta.Y * delta.Y;

                        if (distSq < Hsq)
                        {
                            float dist = MathF.Sqrt(distSq);
                            // Simple distance-based density: closer = denser
                            float influence = MathF.Max(0, 1.0f - dist / H);
                            density += Mass * influence * influence;
                        }
                    }
                }
            }

            particles[i].Density = Math.Max(density, 0.001f);
            particles[i].Pressure = GasConstant * (particles[i].Density - RestDensity);
        }
    }

    static void ComputeForces()
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            Vector2 pos = particles[i].Position;
            if (pos.X < ContainerLeft - 10 || pos.X > ContainerRight + 10 ||
                pos.Y < ContainerTop - 10 || pos.Y > ContainerBottom + 10)
                continue;

            Vector2 pressureForce = Vector2.Zero;
            Vector2 viscosityForce = Vector2.Zero;

            int cell = gridCellIdx[i];
            if (cell < 0) continue;

            int row = cell / GridCols;
            int col = cell % GridCols;

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int nc = col + dc;
                    int nr = row + dr;
                    if (nc < 0 || nc >= GridCols || nr < 0 || nr >= GridRows) continue;

                    int neighborCell = nr * GridCols + nc;
                    for (int j = gridHeads[neighborCell]; j != -1; j = gridNext[j])
                    {
                        if (i == j) continue;

                        Vector2 delta = particles[j].Position - pos;
                        float distSq = delta.X * delta.X + delta.Y * delta.Y;

                        if (distSq < Hsq && distSq > 0.0001f)
                        {
                            float dist = MathF.Sqrt(distSq);
                            float invDist = 1.0f / dist;
                            Vector2 dir = new Vector2(delta.X * invDist, delta.Y * invDist);

                            // Pressure force: push away from high pressure areas
                            float avgPressure = (particles[i].Pressure + particles[j].Pressure) * 0.5f;
                            float weight = MathF.Max(0, 1.0f - dist / H);
                            float pForce = Mass * avgPressure * weight * weight / (particles[j].Density * dist);
                            pressureForce -= dir * pForce;

                            // Viscosity force: dampen relative velocity
                            float viscWeight = MathF.Max(0, 1.0f - dist / H);
                            Vector2 velDiff = particles[j].Velocity - particles[i].Velocity;
                            viscosityForce += velDiff * (Viscosity * Mass * viscWeight / particles[j].Density);
                        }
                    }
                }
            }

            // Gravity
            Vector2 gravityForce = new Vector2(0, Gravity * Mass);

            particles[i].Force = pressureForce + viscosityForce + gravityForce;
        }
    }

    static void ApplyMouseForce()
    {
        if (!mouseDown) return;

        Vector2 mouseVel = mousePos - mousePrev;
        mousePrev = mousePos;

        for (int i = 0; i < ParticleCount; i++)
        {
            Vector2 pos = particles[i].Position;
            if (pos.X < ContainerLeft - 10 || pos.X > ContainerRight + 10 ||
                pos.Y < ContainerTop - 10 || pos.Y > ContainerBottom + 10)
                continue;

            Vector2 toMouse = mousePos - pos;
            float distSq = toMouse.X * toMouse.X + toMouse.Y * toMouse.Y;

            if (distSq < mouseRadius * mouseRadius && distSq > 0.0001f)
            {
                float dist = MathF.Sqrt(distSq);
                float strength = (1.0f - dist / mouseRadius) * mouseStrength;

                Vector2 pushDir = -Vector2.Normalize(toMouse);
                particles[i].Force += pushDir * strength;
                particles[i].Force += mouseVel * 8.0f;
            }
        }
    }

    static void Integrate(float dt)
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            Vector2 pos = particles[i].Position;
            if (pos.X < ContainerLeft - 10 || pos.X > ContainerRight + 10 ||
                pos.Y < ContainerTop - 10 || pos.Y > ContainerBottom + 10)
                continue;

            Vector2 acceleration = particles[i].Force / particles[i].Density;

            particles[i].Velocity += acceleration * dt;
            particles[i].Position += particles[i].Velocity * dt;

            // Damping
            particles[i].Velocity *= 0.995f;
        }
    }

    static void EnforceBoundaries()
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            Vector2 pos = particles[i].Position;
            if (pos.X < ContainerLeft - 10 || pos.X > ContainerRight + 10 ||
                pos.Y < ContainerTop - 10 || pos.Y > ContainerBottom + 10)
                continue;

            float restitution = 0.4f;

            if (pos.X - ParticleRadius < ContainerLeft)
            {
                particles[i].Position.X = ContainerLeft + ParticleRadius;
                particles[i].Velocity.X = -particles[i].Velocity.X * restitution;
            }

            if (pos.X + ParticleRadius > ContainerRight)
            {
                particles[i].Position.X = ContainerRight - ParticleRadius;
                particles[i].Velocity.X = -particles[i].Velocity.X * restitution;
            }

            if (pos.Y - ParticleRadius < ContainerTop)
            {
                particles[i].Position.Y = ContainerTop + ParticleRadius;
                particles[i].Velocity.Y = -particles[i].Velocity.Y * restitution;
            }

            if (pos.Y + ParticleRadius > ContainerBottom)
            {
                particles[i].Position.Y = ContainerBottom - ParticleRadius;
                particles[i].Velocity.Y = -particles[i].Velocity.Y * restitution;
                particles[i].Velocity.X *= 0.95f;
            }
        }
    }
}
