using Raylib_cs;
using System.Numerics;
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;

namespace Raylib2DSwarm;

/// <summary>
/// 2D boids flocking simulation using triangular agents.
/// Implements Craig Reynolds' boids algorithm with separation, alignment, and cohesion.
/// </summary>
class Program
{
    // --- Window & Simulation Constants ---
    const int ScreenWidth = 1280;
    const int ScreenHeight = 720;
    const int BoidCount = 100;
    const float BoidSize = 12.0f; // triangle side length
    const float MaxSpeed = 200.0f;
    const float MaxForce = 100.0f;
    const float PerceptionRadius = 80.0f;
    const float SeparationRadius = 30.0f;

    // Boid rule weights
    const float SeparationWeight = 1.5f;
    const float AlignmentWeight = 1.0f;
    const float CohesionWeight = 1.0f;

    // Boundary behavior
    const float BoundaryMargin = 40.0f;
    const float TurnForce = 300.0f;

    struct Boid
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Color Color;
    }

    static Boid[] boids = new Boid[BoidCount];
    static Random rng = new Random();

    static void Main(string[] args)
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "Raylib C# 2D Boids Flocking Simulation");
        Raylib.SetTargetFPS(60);

        InitBoids();

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();
            if (dt > 0.033f) dt = 0.033f;

            HandleInput();

            // Update
            UpdateBoids(dt);

            // Render
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(10, 10, 20, 255));

            DrawBorders();
            DrawBoids();

            // UI
            int ls = 25;
            Raylib.DrawFPS(10, 10);
            Raylib.DrawText($"Boids: {BoidCount}", 10, 10 + ls, 20, Color.LightGray);
            Raylib.DrawText("R to reset  |  Click to attract  |  Right-click to repel", 10, 10 + 2 * ls, 20, Color.LightGray);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    static void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.R))
        {
            InitBoids();
        }
    }

    static void InitBoids()
    {
        float margin = 80.0f;
        for (int i = 0; i < BoidCount; i++)
        {
            float angle = (float)(rng.NextDouble() * Math.PI * 2);
            float speed = MaxSpeed * (0.5f + (float)rng.NextDouble() * 0.5f);

            // Generate a nice color palette
            float hue = (float)i / BoidCount;
            Color col = ColorFromHSV(hue * 360.0f, 0.8f, 0.9f);

            boids[i] = new Boid
            {
                Position = new Vector2(
                    margin + (float)rng.NextDouble() * (ScreenWidth - 2 * margin),
                    margin + (float)rng.NextDouble() * (ScreenHeight - 2 * margin)
                ),
                Velocity = new Vector2(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed),
                Acceleration = Vector2.Zero,
                Color = col
            };
        }
    }

    static Color ColorFromHSV(float hue, float saturation, float value)
    {
        // Convert HSV to RGB
        float c = value * saturation;
        float x = c * (1.0f - MathF.Abs((hue / 60.0f) % 2.0f - 1.0f));
        float m = value - c;

        float r, g, b;
        if (hue < 60) { r = c; g = x; b = 0; }
        else if (hue < 120) { r = x; g = c; b = 0; }
        else if (hue < 180) { r = 0; g = c; b = x; }
        else if (hue < 240) { r = 0; g = x; b = c; }
        else if (hue < 300) { r = x; g = 0; b = c; }
        else { r = c; g = 0; b = x; }

        return new Color(
            (int)((r + m) * 255),
            (int)((g + m) * 255),
            (int)((b + m) * 255),
            255
        );
    }

    static void DrawBorders()
    {
        Raylib.DrawRectangleLinesEx(
            new Rectangle(BoundaryMargin, BoundaryMargin, ScreenWidth - 2 * BoundaryMargin, ScreenHeight - 2 * BoundaryMargin),
            2,
            new Color(60, 80, 120, 255));
    }

    static void DrawBoids()
    {
        for (int i = 0; i < BoidCount; i++)
        {
            DrawTriangleBoid(boids[i]);
        }
    }

    static void DrawTriangleBoid(Boid boid)
    {
        Vector2 pos = boid.Position;
        Vector2 vel = boid.Velocity;

        // Calculate the direction angle
        float angle = MathF.Atan2(vel.Y, vel.X);

        // Triangle points: tip points in direction of movement
        float tipOffset = BoidSize * 0.6f;
        float backOffset = BoidSize * 0.3f;

        Vector2 tip = new Vector2(
            pos.X + MathF.Cos(angle) * tipOffset,
            pos.Y + MathF.Sin(angle) * tipOffset
        );

        Vector2 left = new Vector2(
            pos.X + MathF.Cos(angle + 2.5f) * backOffset,
            pos.Y + MathF.Sin(angle + 2.5f) * backOffset
        );

        Vector2 right = new Vector2(
            pos.X + MathF.Cos(angle - 2.5f) * backOffset,
            pos.Y + MathF.Sin(angle - 2.5f) * backOffset
        );

        Raylib.DrawTriangle(tip, left, right, boid.Color);

        // Draw a small outline for visibility
        Raylib.DrawTriangleLines(tip, left, right, Color.White);
    }

    static void UpdateBoids(float dt)
    {
        Vector2? mousePos = null;
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            mousePos = Raylib.GetMousePosition();
        }
        Vector2? rightClickPos = null;
        if (Raylib.IsMouseButtonDown(MouseButton.Right))
        {
            rightClickPos = Raylib.GetMousePosition();
        }

        for (int i = 0; i < BoidCount; i++)
        {
            Vector2 separation = Vector2.Zero;
            Vector2 alignment = Vector2.Zero;
            Vector2 cohesion = Vector2.Zero;

            int neighbors = 0;
            int separationNeighbors = 0;

            for (int j = 0; j < BoidCount; j++)
            {
                if (i == j) continue;

                Vector2 diff = boids[i].Position - boids[j].Position;
                float distSq = diff.X * diff.X + diff.Y * diff.Y;

                if (distSq < PerceptionRadius * PerceptionRadius)
                {
                    float dist = MathF.Sqrt(distSq);

                    // Separation: steer away from nearby boids
                    if (dist < SeparationRadius && dist > 0.001f)
                    {
                        Vector2 away = Vector2.Normalize(diff) / dist;
                        separation += away;
                        separationNeighbors++;
                    }

                    // Alignment: match velocity of nearby boids
                    alignment += boids[j].Velocity;
                    neighbors++;

                    // Cohesion: steer toward center of mass of nearby boids
                    cohesion += boids[j].Position;
                }
            }

            if (separationNeighbors > 0)
            {
                separation /= separationNeighbors;
                separation = Vector2.Normalize(separation) * MaxSpeed - boids[i].Velocity;
                separation = LimitMagnitude(separation, MaxForce);
            }

            if (neighbors > 0)
            {
                alignment /= neighbors;
                alignment = Vector2.Normalize(alignment) * MaxSpeed - boids[i].Velocity;
                alignment = LimitMagnitude(alignment, MaxForce);

                cohesion /= neighbors;
                cohesion -= boids[i].Position;
                cohesion = Vector2.Normalize(cohesion) * MaxSpeed - boids[i].Velocity;
                cohesion = LimitMagnitude(cohesion, MaxForce);
            }

            // Apply forces
            Vector2 acceleration = separation * SeparationWeight +
                                   alignment * AlignmentWeight +
                                   cohesion * CohesionWeight;

            // Boundary avoidance
            Vector2 pos = boids[i].Position;
            if (pos.X < BoundaryMargin)
                acceleration.X += TurnForce;
            else if (pos.X > ScreenWidth - BoundaryMargin)
                acceleration.X -= TurnForce;

            if (pos.Y < BoundaryMargin)
                acceleration.Y += TurnForce;
            else if (pos.Y > ScreenHeight - BoundaryMargin)
                acceleration.Y -= TurnForce;

            // Mouse interaction
            if (mousePos.HasValue)
            {
                Vector2 toMouse = mousePos.Value - pos;
                float distToMouse = toMouse.Length();
                if (distToMouse < 150.0f && distToMouse > 0.001f)
                {
                    acceleration += Vector2.Normalize(toMouse) * 200.0f;
                }
            }

            if (rightClickPos.HasValue)
            {
                Vector2 fromMouse = pos - rightClickPos.Value;
                float distFromMouse = fromMouse.Length();
                if (distFromMouse < 150.0f && distFromMouse > 0.001f)
                {
                    acceleration += Vector2.Normalize(fromMouse) * 300.0f;
                }
            }

            boids[i].Acceleration = acceleration;
        }

        // Integrate
        for (int i = 0; i < BoidCount; i++)
        {
            boids[i].Velocity += boids[i].Acceleration * dt;
            boids[i].Velocity = LimitMagnitude(boids[i].Velocity, MaxSpeed);
            boids[i].Position += boids[i].Velocity * dt;

            // Wrap around edges (soft boundary)
            Vector2 pos = boids[i].Position;
            if (pos.X < -BoidSize) pos.X = ScreenWidth + BoidSize;
            if (pos.X > ScreenWidth + BoidSize) pos.X = -BoidSize;
            if (pos.Y < -BoidSize) pos.Y = ScreenHeight + BoidSize;
            if (pos.Y > ScreenHeight + BoidSize) pos.Y = -BoidSize;
            boids[i].Position = pos;
        }
    }

    static Vector2 LimitMagnitude(Vector2 v, float maxMagnitude)
    {
        float magSq = v.X * v.X + v.Y * v.Y;
        if (magSq > maxMagnitude * maxMagnitude)
        {
            float mag = MathF.Sqrt(magSq);
            return new Vector2(v.X / mag * maxMagnitude, v.Y / mag * maxMagnitude);
        }
        return v;
    }
}
