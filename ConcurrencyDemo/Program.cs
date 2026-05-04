// ============================================================
// ConcurrencyDemo - Async/Await & Task Parallel Library (TPL)
// ============================================================
// This demo covers:
//   1. Basic async/await with Task.Delay
//   2. async/await with HttpClient (I/O-bound work)
//   3. Task<T> return types
//   4. Task.WhenAll / Task.WhenAny
//   5. Parallel.For / Parallel.ForEach (CPU-bound work)
//   6. Cancellation tokens
//   7. IAsyncEnumerable<T> (async streaming)
//   8. Progress reporting with IProgress<T>
//   9. Exception handling in async code
//  10. ConfigureAwait(false) explanation
//  11. Common pitfalls demo
// ============================================================

using System.Collections.Concurrent;
using System.Diagnostics;

Console.WriteLine("============================================");
Console.WriteLine("  C# Concurrency: Async/Await & TPL Demo");
Console.WriteLine("============================================");
Console.WriteLine();

// ============================================================
// 1. BASIC ASYNC/AWAIT WITH Task.Delay
// ============================================================
Console.WriteLine("--- 1. Basic async/await with Task.Delay ---");
Console.WriteLine($"  [{DateTime.Now:HH:mm:ss.fff}] Starting basic demo...");

// Call an async method and wait for it synchronously
BasicAsyncDemo().GetAwaiter().GetResult();

Console.WriteLine($"  [{DateTime.Now:HH:mm:ss.fff}] Basic demo completed.");
Console.WriteLine();

// ============================================================
// 2. ASYNC/AWAIT WITH HttpClient (I/O-bound work)
// ============================================================
Console.WriteLine("--- 2. Async HTTP request (I/O-bound) ---");
Console.WriteLine($"  [{DateTime.Now:HH:mm:ss.fff}] Fetching data from httpbin.org...");

string httpResult = await FetchDataAsync("https://httpbin.org/get");
Console.WriteLine($"  Response received ({httpResult.Length} chars)");
Console.WriteLine($"  First 120 chars: {httpResult[..Math.Min(120, httpResult.Length)]}...");
Console.WriteLine();

// ============================================================
// 3. Task<T> RETURN TYPES
// ============================================================
Console.WriteLine("--- 3. Task<T> return types ---");

int calculationResult = await CalculateAsync(42);
Console.WriteLine($"  CalculateAsync(42) returned: {calculationResult}");

string greeting = await GreetAsync("World");
Console.WriteLine($"  GreetAsync(\"World\") returned: \"{greeting}\"");
Console.WriteLine();

// ============================================================
// 4. Task.WhenAll / Task.WhenAny
// ============================================================
Console.WriteLine("--- 4. Task.WhenAll / Task.WhenAny ---");

// WhenAll: run multiple tasks concurrently and wait for all
var stopwatch = Stopwatch.StartNew();

Task<string> task1 = SimulateWorkAsync("Task A", 1500);
Task<string> task2 = SimulateWorkAsync("Task B", 1000);
Task<string> task3 = SimulateWorkAsync("Task C", 2000);

string[] allResults = await Task.WhenAll(task1, task2, task3);
stopwatch.Stop();

Console.WriteLine($"  Task.WhenAll completed in {stopwatch.ElapsedMilliseconds}ms (would be ~4500ms if sequential)");
foreach (string result in allResults)
{
    Console.WriteLine($"    {result}");
}

// WhenAny: react to the first task that completes
Task<string> fastTask = SimulateWorkAsync("Fast Task", 500);
Task<string> slowTask = SimulateWorkAsync("Slow Task", 3000);

string firstCompletedResult = await await Task.WhenAny(fastTask, slowTask);
Console.WriteLine($"  First to complete: {firstCompletedResult}");
Console.WriteLine();

// ============================================================
// 5. PARALLEL.For / Parallel.ForEach (CPU-bound work)
// ============================================================
Console.WriteLine("--- 5. Parallel.For / Parallel.ForEach (CPU-bound) ---");

const int arraySize = 50_000_000;
Console.WriteLine($"  Processing array of {arraySize:N0} elements...");

int[] data = new int[arraySize];

// Sequential loop
stopwatch.Restart();
for (int i = 0; i < data.Length; i++)
{
    data[i] = (int)(Math.Sin(i * 0.001) * 1000);
}
stopwatch.Stop();
Console.WriteLine($"  Sequential loop: {stopwatch.ElapsedMilliseconds}ms");

// Parallel.For
stopwatch.Restart();
Parallel.For(0, data.Length, i =>
{
    data[i] = (int)(Math.Sin(i * 0.001) * 1000);
});
stopwatch.Stop();
Console.WriteLine($"  Parallel.For:    {stopwatch.ElapsedMilliseconds}ms (speedup!)");

// Parallel.ForEach with partitioner
stopwatch.Restart();
Parallel.ForEach(
    Partitioner.Create(0, data.Length),
    range =>
    {
        for (int i = range.Item1; i < range.Item2; i++)
        {
            data[i] = (int)(Math.Sin(i * 0.001) * 1000);
        }
    });
stopwatch.Stop();
Console.WriteLine($"  Parallel.ForEach (partitioned): {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine();

// ============================================================
// 6. CANCELLATION TOKENS
// ============================================================
Console.WriteLine("--- 6. Cancellation Tokens ---");

using var cts = new CancellationTokenSource();
CancellationToken token = cts.Token;

// Start a cancellable operation
Task cancellableTask = CancellableWorkAsync(token, cts);

// Cancel after 1 second
await Task.Delay(1000);
Console.WriteLine("  Requesting cancellation...");
await cts.CancelAsync();

try
{
    await cancellableTask;
    Console.WriteLine("  Task completed normally (unexpected)");
}
catch (OperationCanceledException)
{
    Console.WriteLine("  Task was cancelled successfully (OperationCanceledException caught)");
}
Console.WriteLine();

// ============================================================
// 7. IAsyncEnumerable<T> (ASYNC STREAMING)
// ============================================================
Console.WriteLine("--- 7. IAsyncEnumerable<T> (Async Streaming) ---");
Console.WriteLine("  Streaming numbers with delay...");

await foreach (int number in GenerateNumbersAsync(5, 400))
{
    Console.WriteLine($"    Received: {number} at [{DateTime.Now:HH:mm:ss.fff}]");
}
Console.WriteLine();

// ============================================================
// 8. PROGRESS REPORTING WITH IProgress<T>
// ============================================================
Console.WriteLine("--- 8. Progress Reporting (IProgress<T>) ---");

var progress = new Progress<int>(percent =>
{
    // Clear the current line and write progress
    Console.Write($"\r    Progress: {percent}% [{new string('#', percent / 10)}{new string('-', 10 - percent / 10)}]");
});

await LongRunningWorkAsync(progress);
Console.WriteLine("\r    Progress: 100% [##########] Done!");
Console.WriteLine();

// ============================================================
// 9. EXCEPTION HANDLING IN ASYNC CODE
// ============================================================
Console.WriteLine("--- 9. Exception Handling in Async Code ---");

// Single exception
try
{
    await ThrowAsync("Something went wrong");
}
catch (Exception ex)
{
    Console.WriteLine($"  Caught single exception: {ex.GetType().Name}: {ex.Message}");
}

// Multiple exceptions with Task.WhenAll
try
{
    Task ex1 = ThrowAsync("Error 1");
    Task ex2 = ThrowAsync("Error 2");
    Task ex3 = ThrowAsync("Error 3");
    await Task.WhenAll(ex1, ex2, ex3);
}
catch (Exception ex)
{
    // WhenAll wraps exceptions in AggregateException only in some contexts
    // In async/await, the first exception is re-thrown
    Console.WriteLine($"  Caught from WhenAll: {ex.GetType().Name}: {ex.Message}");
}

// To see ALL exceptions, use Task.WhenAll and inspect .Exception
Task t1 = ThrowAsync("Failure A");
Task t2 = ThrowAsync("Failure B");
Task allTasks = Task.WhenAll(t1, t2);

try
{
    await allTasks;
}
catch
{
    if (allTasks.Exception is AggregateException ae)
    {
        Console.WriteLine($"  All exceptions ({ae.InnerExceptions.Count}):");
        foreach (var inner in ae.InnerExceptions)
        {
            Console.WriteLine($"    - {inner.Message}");
        }
    }
}
Console.WriteLine();

// ============================================================
// 10. ConfigureAwait(false) EXPLANATION
// ============================================================
Console.WriteLine("--- 10. ConfigureAwait(false) Demo ---");
Console.WriteLine("  (In a console app, ConfigureAwait(false) has no visible effect,");
Console.WriteLine("   but in UI/ASP.NET apps it prevents deadlocks by not capturing");
Console.WriteLine("   the SynchronizationContext.)");

await ConfigureAwaitDemoAsync();
Console.WriteLine();

// ============================================================
// 11. COMMON PITFALLS DEMO
// ============================================================
Console.WriteLine("--- 11. Common Pitfalls ---");

// Pitfall 1: Fire-and-forget without exception handling
Console.WriteLine("  Pitfall 1: Fire-and-forget (unobserved exception)");
_ = FireAndForgetWithExceptionAsync();
await Task.Delay(500); // Wait for the task to fail

// Pitfall 2: Blocking on async code with .Result or .Wait() (can deadlock)
Console.WriteLine("  Pitfall 2: Blocking with .Result (risks deadlock in UI/ASP.NET)");
try
{
    // Simulate blocking on async code - this works in console apps but deadlocks in UI/ASP.NET
    Task<string> blockingTask = Task.Run(async () =>
    {
        await Task.Delay(200);
        return "Done";
    });
    string result = blockingTask.Result;
    Console.WriteLine($"    Blocking call succeeded: {result}");
}
catch (AggregateException ae)
{
    Console.WriteLine($"    Blocking call failed: {ae.InnerException?.Message}");
}

Console.WriteLine();
Console.WriteLine("============================================");
Console.WriteLine("  Demo complete! Press any key to exit...");
Console.WriteLine("============================================");
// Use Console.Read() instead of Console.ReadKey() for compatibility with redirected terminals
try { Console.ReadKey(); } catch (InvalidOperationException) { Console.ReadLine(); }

// ============================================================
// METHOD DEFINITIONS
// ============================================================

/// <summary>
/// Basic async method demonstrating the async/await pattern with Task.Delay.
/// </summary>
static async Task BasicAsyncDemo()
{
    Console.WriteLine($"    [{DateTime.Now:HH:mm:ss.fff}] Inside BasicAsyncDemo - step 1");
    await Task.Delay(1000);
    Console.WriteLine($"    [{DateTime.Now:HH:mm:ss.fff}] Inside BasicAsyncDemo - step 2 (after 1s delay)");
    await Task.Delay(500);
    Console.WriteLine($"    [{DateTime.Now:HH:mm:ss.fff}] Inside BasicAsyncDemo - step 3 (after another 500ms)");
}

/// <summary>
/// Demonstrates async I/O with HttpClient.
/// </summary>
static async Task<string> FetchDataAsync(string url)
{
    using var client = new HttpClient();
    client.Timeout = TimeSpan.FromSeconds(10);
    string response = await client.GetStringAsync(url);
    return response;
}

/// <summary>
/// Returns a Task{T} with a computed value.
/// </summary>
static async Task<int> CalculateAsync(int input)
{
    await Task.Delay(500); // Simulate computation
    return input * 2 + 1;
}

/// <summary>
/// Returns a Task{string} with a formatted greeting.
/// </summary>
static async Task<string> GreetAsync(string name)
{
    await Task.Delay(300);
    return $"Hello, {name}!";
}

/// <summary>
/// Simulates async work with a configurable delay.
/// </summary>
static async Task<string> SimulateWorkAsync(string name, int delayMs)
{
    await Task.Delay(delayMs);
    return $"{name} completed in {delayMs}ms";
}

/// <summary>
/// Demonstrates cooperative cancellation with CancellationToken.
/// </summary>
static async Task CancellableWorkAsync(CancellationToken token, CancellationTokenSource cts)
{
    Console.WriteLine("  Cancellable work started...");
    try
    {
        for (int i = 0; i < 10; i++)
        {
            token.ThrowIfCancellationRequested();
            Console.WriteLine($"    Working... iteration {i + 1}/10");
            await Task.Delay(500, token);
        }
        Console.WriteLine("  Cancellable work completed normally.");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("  Cancellable work was cancelled.");
        throw; // Re-throw to let the caller know
    }
}

/// <summary>
/// Generates an async stream of numbers using IAsyncEnumerable.
/// </summary>
static async IAsyncEnumerable<int> GenerateNumbersAsync(int count, int delayMs)
{
    for (int i = 1; i <= count; i++)
    {
        await Task.Delay(delayMs);
        yield return i;
    }
}

/// <summary>
/// Demonstrates progress reporting with IProgress{T}.
/// </summary>
static async Task LongRunningWorkAsync(IProgress<int> progress)
{
    for (int i = 0; i <= 100; i += 10)
    {
        await Task.Delay(200);
        progress.Report(i);
    }
}

/// <summary>
/// Throws an exception after a short delay.
/// </summary>
static async Task ThrowAsync(string message)
{
    await Task.Delay(100);
    throw new InvalidOperationException(message);
}

/// <summary>
/// Demonstrates ConfigureAwait(false) usage.
/// </summary>
static async Task ConfigureAwaitDemoAsync()
{
    // In a console app, SynchronizationContext.Current is null,
    // so ConfigureAwait(false) has no visible effect.
    // In UI apps (WinForms, WPF, MAUI) or ASP.NET Core with legacy mode,
    // it prevents the continuation from being marshalled back to the original context,
    // which avoids deadlocks when blocking on async code.

    await Task.Delay(100).ConfigureAwait(false);
    Console.WriteLine("  After ConfigureAwait(false), we're on a thread-pool thread.");
    Console.WriteLine($"  Current SynchronizationContext: {(SynchronizationContext.Current?.GetType().Name ?? "null")}");
}

/// <summary>
/// Fire-and-forget method that throws (unobserved exception).
/// </summary>
static async Task FireAndForgetWithExceptionAsync()
{
    await Task.Delay(200);
    throw new InvalidOperationException("This exception is unobserved (fire-and-forget)");
}

