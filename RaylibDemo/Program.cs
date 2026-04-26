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
Raylib.DisableCursor();

while (!Raylib.WindowShouldClose())
{
    Raylib.UpdateCamera(ref camera, CameraMode.Free);

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    Raylib.BeginMode3D(camera);

    Raylib.DrawCube(new Vector3(0.0f, 0.0f, 0.0f), 2.0f, 2.0f, 2.0f, Color.Red);
    Raylib.DrawCubeWires(new Vector3(0.0f, 0.0f, 0.0f), 2.0f, 2.0f, 2.0f, Color.Maroon);

    Raylib.DrawGrid(10, 1.0f);

    Raylib.EndMode3D();

    Raylib.DrawText("Hello Raylib! 3D Cube", 10, 10, 20, Color.DarkGray);
    Raylib.DrawFPS(10, 40);

    Raylib.EndDrawing();
}

Raylib.CloseWindow();