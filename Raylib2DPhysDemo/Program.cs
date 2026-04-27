using Raylib_cs;
using System.Numerics;
using System.Diagnostics;

namespace Raylib2DPhysDemo;

/// <summary>
/// 2D physics demo: spheres falling onto a plane.
/// Uses spatial grid for efficient sphere-sphere collision.
/// </summary>
class Program
{
    const int ScreenWidth = 960;
    const int ScreenHeight = 540;
    const int SphereCount = 1000;
    const float SphereRadius = 6.0f;
    static int numColumns = 20;
    const float SphereSpacingX = 2.1f;
    const float SphereSpacingY = 2.1f;    
    const float Gravity = 980.0f;
    const float GroundY = ScreenHeight - 50;
    static float Restitution = 0.9f;
    static float Friction = 0.0f;

    // Spatial grid
    const float CellSize = SphereRadius * 4;
    const int GridCols = (int)(ScreenWidth / CellSize) + 3;
    const int GridRows = (int)(ScreenHeight / CellSize) + 3;

    struct Ball
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public bool IsAlive;
    }

    static Ball[] balls = new Ball[SphereCount];
    static Random rng = new Random(42);

    // Spatial grid - pre-allocated arrays, no List<T> overhead
    static int[] gridHeads = new int[GridCols * GridRows]; // -1 = empty
    static int[] gridNext = new int[SphereCount];          // linked list per cell
    static int[] gridCellIdx = new int[SphereCount];       // which cell each ball is in

    // Pre-rendered circle textures
    static Texture2D circleTexture;
    static Texture2D highlightTexture;

    // Performance tracking
    static Stopwatch sw = new Stopwatch();
    static double physicsTime = 0;
    static double renderTime = 0;

    // Auto-restart timer
    const float RestartInterval = 5.0f;
    static float restartTimer = 0;

    static void Main(string[] args)
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "Raylib C# 2D Physics Simulation");
        Raylib.SetTargetFPS(60);

        // Initialize grid heads to -1
        Array.Fill(gridHeads, -1);

        InitTextures();
        InitBalls();

        sw.Start();

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();
            if (dt > 0.033f) dt = 0.033f;

            if (Raylib.IsKeyPressed(KeyboardKey.R))
                InitBalls();

            // Auto-restart every RestartInterval seconds
            restartTimer += dt;
            if (restartTimer >= RestartInterval)
            {
                restartTimer = 0;
                InitBalls();
            }

            // Physics
            sw.Restart();
            UpdatePhysics(dt);
            physicsTime = sw.Elapsed.TotalMilliseconds;

            // Render
            sw.Restart();
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(15, 15, 25, 255));

            Raylib.DrawRectangle(0, (int)GroundY, ScreenWidth, 4, Color.SkyBlue);

            DrawSpheres();

            // UI
            int ls = 25;
            int sy = 10;
            Raylib.DrawFPS(10, sy);
            Raylib.DrawText($"Spheres: {SphereCount}", 10, sy + ls, 20, Color.LightGray);
            Raylib.DrawText($"Physics: {physicsTime:F1}ms  Render: {renderTime:F1}ms", 10, sy + 2 * ls, 20, Color.LightGray);
            Raylib.DrawText($"Restitution: {Restitution:F2}  Friction: {Friction:F2}  Cols: {numColumns}", 10, sy + 3 * ls, 20, Color.LightGray);
            float timeLeft = RestartInterval - restartTimer;
            Raylib.DrawText($"Press R to reset  (auto: {timeLeft:F1}s)", 10, sy + 4 * ls, 20, Color.LightGray);

            Raylib.EndDrawing();
            renderTime = sw.Elapsed.TotalMilliseconds;
        }

        Raylib.UnloadTexture(circleTexture);
        Raylib.UnloadTexture(highlightTexture);
        Raylib.CloseWindow();
    }

    static void InitTextures()
    {
        int texSize = (int)(SphereRadius * 4);
        if (texSize < 4) texSize = 4;

        Image circleImg = Raylib.GenImageColor(texSize, texSize, new Color(0, 0, 0, 0));
        Vector2 center = new Vector2(texSize / 2.0f, texSize / 2.0f);
        for (int y = 0; y < texSize; y++)
            for (int x = 0; x < texSize; x++)
                if (Vector2.Distance(new Vector2(x, y), center) <= SphereRadius)
                    Raylib.ImageDrawPixel(ref circleImg, x, y, Color.White);
        circleTexture = Raylib.LoadTextureFromImage(circleImg);
        Raylib.UnloadImage(circleImg);

        Image highlightImg = Raylib.GenImageColor(texSize, texSize, new Color(0, 0, 0, 0));
        Vector2 hc = center - new Vector2(2, 2);
        float hr = SphereRadius * 0.3f;
        for (int y = 0; y < texSize; y++)
            for (int x = 0; x < texSize; x++)
                if (Vector2.Distance(new Vector2(x, y), hc) <= hr)
                    Raylib.ImageDrawPixel(ref highlightImg, x, y, Color.White);
        highlightTexture = Raylib.LoadTextureFromImage(highlightImg);
        Raylib.UnloadImage(highlightImg);
    }

    static void RandomizeParams()
    {
        numColumns = rng.Next(4, 31);
        Restitution = 0.1f + (float)rng.NextDouble() * 0.8f;
        Friction = (float)rng.NextDouble() * 0.5f;
    }

    static void InitBalls()
    {
        RandomizeParams();

        float spacingX = SphereRadius * SphereSpacingX;
        float startX = ScreenWidth / 2.0f - (numColumns - 1) * spacingX / 2.0f;
        float spacingY = SphereRadius * SphereSpacingY;

        for (int i = 0; i < SphereCount; i++)
        {
            int col = i % numColumns;
            int row = i / numColumns;

            float x = startX + col * spacingX + (float)(rng.NextDouble() - 0.5) * spacingX * 0.5f;
            float y = -SphereRadius * 2 - row * spacingY;

            balls[i] = new Ball
            {
                Position = new Vector2(x, y),
                Velocity = Vector2.Zero,
                Color = new Color(
                    (int)(rng.NextDouble() * 256),
                    (int)(rng.NextDouble() * 256),
                    (int)(rng.NextDouble() * 256),
                    255
                ),
                IsAlive = true
            };
        }
    }

    static void DrawSpheres()
    {
        float texRadius = SphereRadius * 2;
        Rectangle srcRect = new Rectangle(0, 0, circleTexture.Width, circleTexture.Height);

        for (int i = 0; i < SphereCount; i++)
        {
            if (!balls[i].IsAlive) continue;

            Vector2 pos = balls[i].Position;

            // Skip spheres that are entirely outside the screen
            if (pos.X + SphereRadius < 0 || pos.X - SphereRadius > ScreenWidth ||
                pos.Y + SphereRadius < 0 || pos.Y - SphereRadius > ScreenHeight)
                continue;

            Rectangle destRect = new Rectangle(
                pos.X - texRadius, pos.Y - texRadius,
                texRadius * 2, texRadius * 2
            );

            Raylib.DrawTexturePro(circleTexture, srcRect, destRect, Vector2.Zero, 0.0f, balls[i].Color);
            Raylib.DrawTexturePro(highlightTexture, srcRect, destRect, Vector2.Zero, 0.0f, Color.White);
        }
    }

    /// <summary>
    /// Build spatial grid using linked-list arrays (no allocations).
    /// </summary>
    static void BuildGrid()
    {
        Array.Fill(gridHeads, -1);

        for (int i = 0; i < SphereCount; i++)
        {
            if (!balls[i].IsAlive) continue;

            int col = (int)(balls[i].Position.X / CellSize);
            int row = (int)(balls[i].Position.Y / CellSize);
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
        int substeps = 4;
        float subDt = dt / substeps;

        for (int step = 0; step < substeps; step++)
        {
            // Phase 1: Apply forces (skip dead balls)
            for (int i = 0; i < SphereCount; i++)
            {
                if (!balls[i].IsAlive) continue;

                balls[i].Velocity.Y += Gravity * subDt;
                balls[i].Velocity *= MathF.Max(0, 1.0f - 0.5f * subDt);
                balls[i].Position += balls[i].Velocity * subDt;

                // Mark balls that fall far below the ground as dead
                if (balls[i].Position.Y > GroundY + ScreenHeight * 0.5f)
                    balls[i].IsAlive = false;
            }

            // Phase 2: Build spatial grid (only alive balls)
            BuildGrid();

            // Phase 3: Wall/ground collision (only alive balls)
            for (int i = 0; i < SphereCount; i++)
            {
                if (!balls[i].IsAlive) continue;

                if (balls[i].Position.Y + SphereRadius > GroundY)
                {
                    balls[i].Position.Y = GroundY - SphereRadius;
                    balls[i].Velocity.Y = -balls[i].Velocity.Y * Restitution;
                    balls[i].Velocity.X *= (1.0f - Friction);
                    if (MathF.Abs(balls[i].Velocity.Y) < 10.0f)
                        balls[i].Velocity.Y = 0;
                }

                /*
                if (balls[i].Position.X - SphereRadius < 0)
                {
                    balls[i].Position.X = SphereRadius;
                    balls[i].Velocity.X = -balls[i].Velocity.X * Restitution;
                }
                if (balls[i].Position.X + SphereRadius > ScreenWidth)
                {
                    balls[i].Position.X = ScreenWidth - SphereRadius;
                    balls[i].Velocity.X = -balls[i].Velocity.X * Restitution;
                }
                */
            }

            // Phase 4: Sphere-sphere collision via spatial grid
            // Check each alive ball against others in its cell and adjacent cells
            for (int i = 0; i < SphereCount; i++)
            {
                if (!balls[i].IsAlive) continue;

                int cell = gridCellIdx[i];
                int row = cell / GridCols;
                int col = cell % GridCols;

                // Check same cell (only higher indices to avoid double-checks)
                for (int j = gridHeads[cell]; j != -1; j = gridNext[j])
                {
                    if (j <= i) continue;
                    ResolveCollision(i, j);
                }

                // Check 4 adjacent cells (right, bottom-right, bottom, bottom-left)
                int[] neighborCols = { col + 1, col + 1, col, col - 1 };
                int[] neighborRows = { row, row + 1, row + 1, row + 1 };

                for (int n = 0; n < 4; n++)
                {
                    int nc = neighborCols[n];
                    int nr = neighborRows[n];
                    if (nc < 0 || nc >= GridCols || nr < 0 || nr >= GridRows) continue;

                    int neighborCell = nr * GridCols + nc;
                    for (int j = gridHeads[neighborCell]; j != -1; j = gridNext[j])
                    {
                        if (j <= i) continue;
                        ResolveCollision(i, j);
                    }
                }
            }
        }
    }

    static void ResolveCollision(int i, int j)
    {
        Vector2 delta = balls[j].Position - balls[i].Position;
        float distSq = delta.X * delta.X + delta.Y * delta.Y;
        float minDist = SphereRadius * 2;
        float minDistSq = minDist * minDist;

        if (distSq < minDistSq && distSq > 0.0001f)
        {
            float dist = MathF.Sqrt(distSq);
            float invDist = 1.0f / dist;
            Vector2 normal = new Vector2(delta.X * invDist, delta.Y * invDist);
            float overlap = minDist - dist;

            // Separate
            float halfOverlap = overlap * 0.5f;
            balls[i].Position.X -= normal.X * halfOverlap;
            balls[i].Position.Y -= normal.Y * halfOverlap;
            balls[j].Position.X += normal.X * halfOverlap;
            balls[j].Position.Y += normal.Y * halfOverlap;

            // Elastic collision
            float relVelX = balls[j].Velocity.X - balls[i].Velocity.X;
            float relVelY = balls[j].Velocity.Y - balls[i].Velocity.Y;
            float velAlongNormal = relVelX * normal.X + relVelY * normal.Y;

            if (velAlongNormal < 0)
            {
                float impulse = -(1.0f + Restitution) * velAlongNormal * 0.5f;
                float impX = normal.X * impulse;
                float impY = normal.Y * impulse;

                balls[i].Velocity.X -= impX;
                balls[i].Velocity.Y -= impY;
                balls[j].Velocity.X += impX;
                balls[j].Velocity.Y += impY;

                // Friction
                float tangentX = -normal.Y;
                float tangentY = normal.X;
                float velAlongTangent = relVelX * tangentX + relVelY * tangentY;
                float fricX = tangentX * (velAlongTangent * Friction * 0.5f);
                float fricY = tangentY * (velAlongTangent * Friction * 0.5f);
                balls[i].Velocity.X += fricX;
                balls[i].Velocity.Y += fricY;
                balls[j].Velocity.X -= fricX;
                balls[j].Velocity.Y -= fricY;
            }
        }
    }
}
