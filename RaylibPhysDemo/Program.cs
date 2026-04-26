using Jitter2;
using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Jitter2.LinearMath;
using Raylib_cs;
using System.Numerics;

const int screenWidth = 1280;
const int screenHeight = 720;

Raylib.InitWindow(screenWidth, screenHeight, "Raylib C# - Jitter Physics: 1000 Spheres");
Raylib.SetTargetFPS(60);

// ── Camera setup ──
Camera3D camera = new()
{
    Position = new Vector3(25.0f, 20.0f, 25.0f),
    Target = new Vector3(0.0f, 5.0f, 0.0f),
    Up = new Vector3(0.0f, 1.0f, 0.0f),
    FovY = 60.0f,
    Projection = CameraProjection.Perspective
};

Vector3 lookDir = Vector3.Normalize(camera.Target - camera.Position);
float yaw = MathF.Atan2(lookDir.X, lookDir.Z);
float pitch = MathF.Asin(Math.Clamp(lookDir.Y, -1.0f, 1.0f));
bool wasFreelookActive = false;

// ── Jitter Physics World ──
World physicsWorld = new();

// Ground plane: a large static box
RigidBody ground = physicsWorld.CreateRigidBody();
ground.AddShape(new BoxShape(50, 1, 50));
ground.Position = new JVector(0, -0.5f, 0);
ground.MotionType = MotionType.Static;

// ── Create 1000 spheres in a 10×10×10 grid ──
const int gridSize = 10;
const float spacing = 2.5f;
const float sphereRadius = 0.4f;
const float startY = 25.0f;

RigidBody[] spheres = new RigidBody[gridSize * gridSize * gridSize];
int idx = 0;

for (int ix = 0; ix < gridSize; ix++)
{
    for (int iy = 0; iy < gridSize; iy++)
    {
        for (int iz = 0; iz < gridSize; iz++)
        {
            float x = (ix - (gridSize - 1) * 0.5f) * spacing;
            float y = startY + iy * spacing;
            float z = (iz - (gridSize - 1) * 0.5f) * spacing;

            RigidBody sphere = physicsWorld.CreateRigidBody();
            sphere.AddShape(new SphereShape(sphereRadius));
            sphere.Position = new JVector(x, y, z);
            sphere.Damping = (0.01f, 0.01f); // slight damping for stability
            sphere.Restitution = 0.3f;
            sphere.Friction = 0.5f;

            spheres[idx++] = sphere;
        }
    }
}

// ── Main loop ──
while (!Raylib.WindowShouldClose())
{
    // ── Freelook camera ──
    bool freelookActive = Raylib.IsMouseButtonDown(MouseButton.Right);

    if (freelookActive)
    {
        if (!wasFreelookActive)
        {
            Raylib.HideCursor();
            wasFreelookActive = true;
        }

        Vector2 mouseDelta = Raylib.GetMouseDelta();
        float sensitivity = 0.003f;
        yaw -= mouseDelta.X * sensitivity;
        pitch -= mouseDelta.Y * sensitivity;
        pitch = Math.Clamp(pitch, -1.5f, 1.5f);

        float cp = MathF.Cos(pitch);
        Vector3 forward = new(
            cp * MathF.Sin(yaw),
            MathF.Sin(pitch),
            cp * MathF.Cos(yaw)
        );
        forward = Vector3.Normalize(forward);

        Vector3 worldUp = new(0, 1, 0);
        Vector3 right = Vector3.Normalize(Vector3.Cross(forward, worldUp));
        Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

        float moveSpeed = 0.1f;

        if (Raylib.IsKeyDown(KeyboardKey.W))
            camera.Position += forward * moveSpeed;
        if (Raylib.IsKeyDown(KeyboardKey.S))
            camera.Position -= forward * moveSpeed;
        if (Raylib.IsKeyDown(KeyboardKey.A))
            camera.Position -= right * moveSpeed;
        if (Raylib.IsKeyDown(KeyboardKey.D))
            camera.Position += right * moveSpeed;
        if (Raylib.IsKeyDown(KeyboardKey.Space))
            camera.Position += up * moveSpeed;
        if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
            camera.Position -= up * moveSpeed;

        camera.Target = camera.Position + forward;
        camera.Up = up;
    }
    else
    {
        if (wasFreelookActive)
        {
            Raylib.ShowCursor();
            wasFreelookActive = false;
        }
    }

    // ── Step physics ──
    physicsWorld.Step(1.0f / 60.0f, true);

    // ── Draw ──
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.SkyBlue);

    Raylib.BeginMode3D(camera);

    // Draw ground plane
    Raylib.DrawCube(new Vector3(0, -0.5f, 0), 50, 1, 50, Color.DarkGray);
    Raylib.DrawCubeWires(new Vector3(0, -0.5f, 0), 50, 1, 50, Color.Black);

    // Draw all spheres
    foreach (RigidBody sphere in spheres)
    {
        JVector pos = sphere.Position;
        // Color based on height: blue (low) → red (high)
        float t = Math.Clamp((pos.Y + 2.0f) / 30.0f, 0.0f, 1.0f);
        Color color = new(
            (int)(t * 255),
            (int)((1.0f - MathF.Abs(t - 0.5f) * 2.0f) * 200),
            (int)((1.0f - t) * 255),
            255
        );
        Raylib.DrawSphere(new Vector3(pos.X, pos.Y, pos.Z), sphereRadius, color);
    }

    Raylib.DrawGrid(20, 2.0f);

    Raylib.EndMode3D();

    // ── UI overlay ──
    Raylib.DrawText("Jitter Physics: 1000 Spheres", 10, 10, 24, Color.Black);
    Raylib.DrawFPS(10, 40);
    Raylib.DrawText("Hold right mouse button for freelook (WASD + Space/Shift)", 10, screenHeight - 30, 15, Color.Gray);

    Raylib.EndDrawing();
}

Raylib.CloseWindow();