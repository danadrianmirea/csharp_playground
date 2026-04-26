using Raylib_cs;
using System.Numerics;

const int screenWidth = 800;
const int screenHeight = 600;

Raylib.InitWindow(screenWidth, screenHeight, "Raylib C# - 3D Cube Demo");

Camera3D camera = new Camera3D();
camera.Position = new Vector3(10.0f, 10.0f, 10.0f);
camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
camera.FovY = 45.0f;
camera.Projection = CameraProjection.Perspective;

Raylib.SetTargetFPS(60);

// Compute initial yaw/pitch from the camera's look direction
// lookDir = normalize(Target - Position)
Vector3 lookDir = Vector3.Normalize(camera.Target - camera.Position);
float yaw = MathF.Atan2(lookDir.X, lookDir.Z);
float pitch = MathF.Asin(Math.Clamp(lookDir.Y, -1.0f, 1.0f));

bool wasFreelookActive = false;

while (!Raylib.WindowShouldClose())
{
    bool freelookActive = Raylib.IsMouseButtonDown(MouseButton.Right);

    if (freelookActive)
    {
        if (!wasFreelookActive)
        {
            Raylib.HideCursor();
            wasFreelookActive = true;
        }

        // Mouse look
        Vector2 mouseDelta = Raylib.GetMouseDelta();
        float sensitivity = 0.003f;
        yaw -= mouseDelta.X * sensitivity;
        pitch -= mouseDelta.Y * sensitivity;
        pitch = Math.Clamp(pitch, -1.5f, 1.5f);

        // Compute forward direction from yaw/pitch
        float cp = MathF.Cos(pitch);
        Vector3 forward = new Vector3(
            cp * MathF.Sin(yaw),
            MathF.Sin(pitch),
            cp * MathF.Cos(yaw)
        );
        forward = Vector3.Normalize(forward);

        // Compute right and up vectors
        Vector3 worldUp = new Vector3(0, 1, 0);
        Vector3 right = Vector3.Normalize(Vector3.Cross(forward, worldUp));
        Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

        // WASD movement in local camera space
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
        if (Raylib.IsKeyDown(KeyboardKey.C))
            camera.Position -= up * moveSpeed;

        // Set target to look in the forward direction
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

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    Raylib.BeginMode3D(camera);

    Raylib.DrawCube(new Vector3(0.0f, 0.0f, 0.0f), 2.0f, 2.0f, 2.0f, Color.Red);
    Raylib.DrawCubeWires(new Vector3(0.0f, 0.0f, 0.0f), 2.0f, 2.0f, 2.0f, Color.Maroon);

    Raylib.DrawGrid(10, 1.0f);

    Raylib.EndMode3D();

    Raylib.DrawText("Hello Raylib! 3D Cube", 10, 10, 20, Color.DarkGray);
    Raylib.DrawFPS(10, 40);

    if (!freelookActive)
    {
        Raylib.DrawText("Hold right mouse button for freelook (mouse + WASD)", 10, screenHeight - 30, 15, Color.Gray);
    }

    Raylib.EndDrawing();
}

Raylib.CloseWindow();