using PhysX;
using Raylib_cs;
using System.Numerics;

namespace RaylibPhysDemo;

class Program
{
    // ── Configuration ──
    const int GridWidth = 1;
    const int GridLength = 1;
    const int GridHeight = 2000;
    const int SphereCount = GridWidth * GridLength * GridHeight;
    const float SphereRadius = 0.4f;
    const float Spacing = 1.0f;
    const float StartY = 25.0f;

    // ── PhysX state ──
    static Foundation foundation = null!;
    static Physics physics = null!;
    static Scene scene = null!;
    static PhysX.Material material = null!;
    static RigidDynamic[] spheres = new RigidDynamic[SphereCount];

    // ── Camera state ──
    static Camera3D camera;
    static float yaw = -MathF.PI / 4;
    static float pitch = -0.3f;
    static bool wasFreelookActive;

    // ── GPU-optimized rendering state ──
    // Pre-created sphere model: mesh is uploaded to GPU VRAM once at startup
    static Model sphereModel;

    static unsafe void Main(string[] args)
    {
        // ── Window setup ──
        const int screenWidth = 1280;
        const int screenHeight = 720;
        Raylib.InitWindow(screenWidth, screenHeight, "PhysX.Net: 800 Spheres (GPU Mesh)");
        Raylib.SetTargetFPS(60);

        // ── Camera setup ──
        camera = new Camera3D();
        camera.Position = new Vector3(25, 15, 25);
        camera.Target = new Vector3(0, 5, 0);
        camera.Up = new Vector3(0, 1, 0);
        camera.FovY = 60.0f;
        camera.Projection = CameraProjection.Perspective;

        // ── Pre-create sphere model (GPU-resident mesh) ──
        // GenMeshSphere creates geometry once; LoadModelFromMesh uploads it to GPU VRAM
        // This is the key optimization: the mesh data stays on the GPU, not regenerated per frame
        Mesh sphereMesh = Raylib.GenMeshSphere(SphereRadius, 16, 16);
        sphereModel = Raylib.LoadModelFromMesh(sphereMesh);

        // ── Initialize PhysX ──
        InitPhysX();

        // ── Main loop ──
        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();
            if (dt > 0.05f) dt = 0.05f;

            // ── Camera freelook ──
            bool freelookActive = Raylib.IsMouseButtonDown(MouseButton.Right);
            if (freelookActive)
            {
                if (!wasFreelookActive)
                {
                    // Derive yaw/pitch from current camera view direction
                    Vector3 dir = Vector3.Normalize(camera.Target - camera.Position);
                    yaw = MathF.Atan2(dir.X, dir.Z);
                    pitch = MathF.Asin(Math.Clamp(dir.Y, -1.0f, 1.0f));
                    Raylib.HideCursor();
                    wasFreelookActive = true;
                }

                Vector2 mouseDelta = Raylib.GetMouseDelta();
                float sensitivity = 0.003f;
                yaw -= mouseDelta.X * sensitivity;
                pitch -= mouseDelta.Y * sensitivity;
                pitch = Math.Clamp(pitch, -1.5f, 1.5f);

                float cp = MathF.Cos(pitch);
                Vector3 forward = new(cp * MathF.Sin(yaw), MathF.Sin(pitch), cp * MathF.Cos(yaw));
                forward = Vector3.Normalize(forward);

                Vector3 worldUp = new(0, 1, 0);
                Vector3 right = Vector3.Normalize(Vector3.Cross(forward, worldUp));
                Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

                float moveSpeed = 8.0f * dt;
                float speedBoost = 2.0f;

                if (Raylib.IsKeyDown(KeyboardKey.LeftShift)) moveSpeed *= speedBoost;
                if (Raylib.IsKeyDown(KeyboardKey.W))     camera.Position += forward * moveSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.A))     camera.Position -= right * moveSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.S))     camera.Position -= forward * moveSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.D))     camera.Position += right * moveSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.Space)) camera.Position += up * moveSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.C))     camera.Position -= up * moveSpeed;

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
            scene.Simulate(dt);
            scene.FetchResults(true);

            // ── Render ──
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.SkyBlue);

            Raylib.BeginMode3D(camera);

            // Draw ground
            Raylib.DrawPlane(new Vector3(0, 0, 0), new Vector2(50, 50), Color.Gray);

            // ── Draw spheres using the pre-created GPU model ──
            // DrawModel reuses the GPU-resident mesh and only updates the transform.
            // This is MUCH faster than DrawSphere which regenerates geometry every call.
            for (int i = 0; i < SphereCount; i++)
            {
                Vector3 pos = spheres[i].GlobalPosePosition;

                // Color based on height: blue (low) -> red (high)
                float t = Math.Clamp((pos.Y + 2.0f) / 30.0f, 0.0f, 1.0f);
                Color color = new Color(
                    (int)(t * 255),
                    (int)((1.0f - MathF.Abs(t - 0.5f) * 2.0f) * 50),
                    (int)((1.0f - t) * 255),
                    255
                );

                // Draw using the GPU-resident model at the sphere's position with per-sphere tint
                Raylib.DrawModelEx(sphereModel, pos, Vector3.UnitY, 0f, Vector3.One, color);
            }

            Raylib.DrawGrid(20, 2.0f);

            Raylib.EndMode3D();

            // ── UI ──
            Raylib.DrawFPS(10, 10);
            Raylib.DrawText($"Spheres: {SphereCount}", 10, 30, 20, Color.DarkGray);
            if (!freelookActive)
            {
                Raylib.DrawText("Hold right mouse button for freelook (WASD + Space + Shift)", 10, screenHeight - 30, 15, Color.Gray);
            }

            Raylib.EndDrawing();
        }

        // ── Cleanup ──
        Raylib.UnloadModel(sphereModel);
        CleanupPhysX();
        Raylib.CloseWindow();
    }

    static void InitPhysX()
    {
        Random rng = new Random();

        foundation = new Foundation(new DefaultErrorCallback());
        physics = new Physics(foundation, false, null!);

        SceneDesc sceneDesc = new SceneDesc();
        sceneDesc.Gravity = new Vector3(0, -9.81f, 0);
        scene = physics.CreateScene(sceneDesc);

        material = physics.CreateMaterial(0.5f, 0.3f, 0.15f);

        // ── Create ground plane ──
        var ground = physics.CreateRigidStatic(Matrix4x4.CreateTranslation(0, -0.5f, 0));
        var groundShape = RigidActorExt.CreateExclusiveShape(
            ground,
            new BoxGeometry(25, 0.5f, 25),
            material
        );
        scene.AddActor(ground);

        // ── Create spheres ──
        int idx = 0;
        for (int ix = 0; ix < GridWidth; ix++)
        {
            for (int iy = 0; iy < GridHeight; iy++)
            {
                for (int iz = 0; iz < GridLength; iz++)
                {
                    float x = (ix - (GridWidth - 1) * 0.5f) * Spacing + (float)(rng.NextDouble() - 0.5) * 1.6f;
                    float y = StartY + iy * Spacing + (float)(rng.NextDouble() - 0.5) * 1.6f;
                    float z = (iz - (GridLength - 1) * 0.5f) * Spacing + (float)(rng.NextDouble() - 0.5) * 1.6f;

                    var body = physics.CreateRigidDynamic(Matrix4x4.CreateTranslation(x, y, z));
                    var shape = RigidActorExt.CreateExclusiveShape(
                        body,
                        new SphereGeometry(SphereRadius),
                        material
                    );

                    body.SetMassAndUpdateInertia(1.0f);
                    body.LinearDamping = 0.1f;
                    body.AngularDamping = 0.1f;

                    scene.AddActor(body);
                    spheres[idx++] = body;
                }
            }
        }
    }

    static void CleanupPhysX()
    {
        for (int i = 0; i < SphereCount; i++)
        {
            if (spheres[i] != null)
            {
                scene.RemoveActor(spheres[i]);
                spheres[i].Dispose();
            }
        }

        scene?.Dispose();
        physics?.Dispose();
        foundation?.Dispose();
    }
}