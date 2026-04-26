using PhysX;
using Raylib_cs;
using System.Numerics;

namespace RaylibPhysDemo;

class Program
{
    const int GridWidth = 1;
    const int GridLength = 1;
    const int GridHeight = 700;
    const int SphereCount = GridWidth * GridLength * GridHeight;
    const float SphereRadius = 0.4f;
    const float Spacing = 1.0f;
    const float StartY = 25.0f;
    const float WorldHalfExtent = 2.5f;
    const float SecondPlaneHalfExtent = 25.0f;
    const float WallHeight = 12.0f;
    const float WallThickness = 0.3f;
    const float PlaneThickness = 0.3f;

    // PhysX variables
    static Foundation foundation = null!;
    static Physics physics = null!;
    static Scene scene = null!;
    static PhysX.Material material = null!;
    static RigidDynamic[] spheres = new RigidDynamic[SphereCount];
    static RigidStatic wallPosX = null!;
    static RigidStatic wallNegX = null!;
    static RigidStatic wallPosZ = null!;
    static RigidStatic wallNegZ = null!;

    // Camera
    static Camera3D camera;
    static float yaw = -MathF.PI / 4;
    static float pitch = -0.3f;

    // Per-sphere colors 
    static Color[] sphereColors = new Color[SphereCount];

    // Pre-created sphere model: mesh is uploaded to GPU VRAM once at startup
    static Model sphereModel;

    static unsafe void Main(string[] args)
    {
        const int screenWidth = 1280;
        const int screenHeight = 720;
        Raylib.InitWindow(screenWidth, screenHeight, "PhysX.Net: 800 Spheres (GPU Mesh)");
        Raylib.SetTargetFPS(60);

        camera = new Camera3D();
        camera.Position = new Vector3(20, 25, 20);
        camera.Target = new Vector3(0, 6, 0);
        camera.Up = new Vector3(0, 1, 0);
        camera.FovY = 60.0f;
        camera.Projection = CameraProjection.Perspective;

        Vector3 dir = Vector3.Normalize(camera.Target - camera.Position);
        yaw = MathF.Atan2(dir.X, dir.Z);
        pitch = MathF.Asin(Math.Clamp(dir.Y, -1.0f, 1.0f));


        // Pre-create sphere model (GPU-resident mesh)
        // GenMeshSphere creates geometry once; LoadModelFromMesh uploads it to GPU VRAM
        Mesh sphereMesh = Raylib.GenMeshSphere(SphereRadius, 16, 16);
        sphereModel = Raylib.LoadModelFromMesh(sphereMesh);

        // Initialize PhysX
        InitPhysX();

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();
            if (dt > 0.05f) dt = 0.05f;

            bool freelookActive = Raylib.IsMouseButtonDown(MouseButton.Right);
            if (freelookActive)
            {
                Raylib.HideCursor();
                Vector2 mouseDelta = Raylib.GetMouseDelta();
                float sensitivity = 0.003f;
                yaw -= mouseDelta.X * sensitivity;
                pitch -= mouseDelta.Y * sensitivity;
                pitch = Math.Clamp(pitch, -1.5f, 1.5f);
            }
            else
            {
                Raylib.ShowCursor();
            }

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

            // Update physics
            scene.Simulate(dt);
            scene.FetchResults(true);

            // Render
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.BeginMode3D(camera);

            // Draw walls (semi-transparent glass)
            Color wallColor = new Color(150, 180, 220, 60);
            float wallDrawWidth = WallThickness * 2;
            float wallDrawLength = WorldHalfExtent * 2;
            Raylib.DrawCube(wallPosX.GlobalPosePosition, wallDrawWidth, WallHeight, wallDrawLength, wallColor);
            Raylib.DrawCube(wallNegX.GlobalPosePosition, wallDrawWidth, WallHeight, wallDrawLength, wallColor);
            Raylib.DrawCube(wallPosZ.GlobalPosePosition, wallDrawLength, WallHeight, wallDrawWidth, wallColor);
            Raylib.DrawCube(wallNegZ.GlobalPosePosition, wallDrawLength, WallHeight, wallDrawWidth, wallColor);
            Raylib.DrawCubeWires(wallPosX.GlobalPosePosition, wallDrawWidth, WallHeight, wallDrawLength, new Color(200, 220, 255, 120));
            Raylib.DrawCubeWires(wallNegX.GlobalPosePosition, wallDrawWidth, WallHeight, wallDrawLength, new Color(200, 220, 255, 120));
            Raylib.DrawCubeWires(wallPosZ.GlobalPosePosition, wallDrawLength, WallHeight, wallDrawWidth, new Color(200, 220, 255, 120));
            Raylib.DrawCubeWires(wallNegZ.GlobalPosePosition, wallDrawLength, WallHeight, wallDrawWidth, new Color(200, 220, 255, 120));

            // Draw ground
            float groundSize = WorldHalfExtent * 2;
            Raylib.DrawPlane(new Vector3(0, 0, 0), new Vector2(groundSize, groundSize), wallColor);

            // Draw ground 2 
            float groundSize2 = SecondPlaneHalfExtent * 2;
            Raylib.DrawPlane(new Vector3(0, 0, 0), new Vector2(groundSize2, groundSize2), Color.DarkGreen);

            // Draw spheres using the pre-created GPU model
            // DrawModel reuses the GPU-resident mesh and only updates the transform.
            // This is MUCH faster than DrawSphere which regenerates geometry every call.
            for (int i = 0; i < SphereCount; i++)
            {
                Vector3 pos = spheres[i].GlobalPosePosition;

                // Draw using the GPU-resident model at the sphere's position with per-sphere tint
                Raylib.DrawModelEx(sphereModel, pos, Vector3.UnitY, 0f, Vector3.One, sphereColors[i]);
            }

            //Raylib.DrawGrid(20, 2.0f);
            Raylib.EndMode3D();

            // ── UI ──
            Raylib.DrawFPS(10, 10);
            Raylib.DrawText($"Spheres: {SphereCount}", 10, 30, 20, Color.DarkGray);
            Raylib.DrawText($"Camera: ({camera.Position.X:F2}, {camera.Position.Y:F2}, {camera.Position.Z:F2})", 10, 55, 20, Color.DarkGray);
            Raylib.DrawText("Hold right mouse button for freelook (WASD + Space + C)", 10, 80, 20, Color.DarkGray);
            
            Raylib.EndDrawing();
        }

        // Cleanup
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

        // Create ground plane
        var ground = physics.CreateRigidStatic(Matrix4x4.CreateTranslation(0, -0.5f, 0));
        var groundShape = RigidActorExt.CreateExclusiveShape(
            ground,
            new BoxGeometry(WorldHalfExtent, PlaneThickness, WorldHalfExtent),
            material
        );
        scene.AddActor(ground);

        // Create second ground plane
        var ground2 = physics.CreateRigidStatic(Matrix4x4.CreateTranslation(0, -0.5f, 0));
        var groundShape2 = RigidActorExt.CreateExclusiveShape(
            ground2,
            new BoxGeometry(SecondPlaneHalfExtent, PlaneThickness, SecondPlaneHalfExtent),
            material
        );
        scene.AddActor(ground2);

        // Create lateral walls
        float wallHalfHeight = WallHeight * 0.5f;
        float wallY = wallHalfHeight - 0.5f;

        // +X wall
        wallPosX = physics.CreateRigidStatic(Matrix4x4.CreateTranslation(WorldHalfExtent + WallThickness, wallY, 0));
        RigidActorExt.CreateExclusiveShape(wallPosX, new BoxGeometry(WallThickness, wallHalfHeight, WorldHalfExtent), material);
        scene.AddActor(wallPosX);

        // -X wall
        wallNegX = physics.CreateRigidStatic(Matrix4x4.CreateTranslation(-(WorldHalfExtent + WallThickness), wallY, 0));
        RigidActorExt.CreateExclusiveShape(wallNegX, new BoxGeometry(WallThickness, wallHalfHeight, WorldHalfExtent), material);
        scene.AddActor(wallNegX);

        // +Z wall
        wallPosZ = physics.CreateRigidStatic(Matrix4x4.CreateTranslation(0, wallY, WorldHalfExtent + WallThickness));
        RigidActorExt.CreateExclusiveShape(wallPosZ, new BoxGeometry(WorldHalfExtent, wallHalfHeight, WallThickness), material);
        scene.AddActor(wallPosZ);

        // -Z wall
        wallNegZ = physics.CreateRigidStatic(Matrix4x4.CreateTranslation(0, wallY, -(WorldHalfExtent + WallThickness)));
        RigidActorExt.CreateExclusiveShape(wallNegZ, new BoxGeometry(WorldHalfExtent, wallHalfHeight, WallThickness), material);
        scene.AddActor(wallNegZ);

        // Create spheres
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
                    spheres[idx] = body;

                    // Assign a random unique color to each sphere
                    sphereColors[idx] = new Color(
                        (int)(rng.NextDouble() * 256),
                        (int)(rng.NextDouble() * 256),
                        (int)(rng.NextDouble() * 256),
                        255
                    );
                    idx++;
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