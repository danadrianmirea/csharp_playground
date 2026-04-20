using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class SimpleWebServer
{
    private readonly HttpListener _listener;
    private readonly string _basePath;
    private readonly int _port;
    
    public SimpleWebServer(int port = 8080, string basePath = null)
    {
        _port = port;
        
        // If basePath is provided, use it
        if (!string.IsNullOrEmpty(basePath))
        {
            _basePath = Path.GetFullPath(basePath);
        }
        else
        {
            _basePath = Directory.GetCurrentDirectory();
        }
        
        Console.WriteLine($"Server base path set to: {_basePath}");
        
        // Validate that index.html exists in the base path
        string indexHtmlPath = Path.Combine(_basePath, "index.html");
        if (!File.Exists(indexHtmlPath))
        {
            Console.WriteLine($"Warning: index.html not found at {indexHtmlPath}");
            Console.WriteLine("Server will start but may return 404 for root requests.");
        }
        else
        {
            Console.WriteLine($"✓ Found index.html at: {indexHtmlPath}");
        }
        
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{port}/");
        _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
    }       
    
    public async Task StartAsync()
    {
        try
        {
            _listener.Start();
            Console.WriteLine($"Simple Web Server started on port {_port}");
            Console.WriteLine($"Serving files from: {_basePath}");
            Console.WriteLine($"Open your browser and navigate to: http://localhost:{_port}/");
            Console.WriteLine("Press Ctrl+C to stop the server...\n");
            
            while (_listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    await ProcessRequestAsync(context);
                }
                catch (HttpListenerException)
                {
                    // Listener was stopped
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing request: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start server: {ex.Message}");
            throw;
        }
    }
    
    private async Task ProcessRequestAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - {request.HttpMethod} {request.Url.LocalPath}");
        
        try
        {
            string path = request.Url.LocalPath;
            
            // Default to index.html if root path
            if (path == "/" || path == "")
            {
                path = "/index.html";
            }
            
            // Remove leading slash and get full file path
            string relativePath = path.TrimStart('/');
            string fullPath = Path.Combine(_basePath, relativePath);
            
            // Security check: ensure path is within base directory
            if (!fullPath.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
            {
                SendError(response, HttpStatusCode.Forbidden, "Access denied");
                return;
            }
            
            if (File.Exists(fullPath))
            {
                string contentType = GetContentType(fullPath);
                byte[] buffer = File.ReadAllBytes(fullPath);
                
                response.ContentType = contentType;
                response.ContentLength64 = buffer.Length;
                
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                SendError(response, HttpStatusCode.NotFound, "File not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serving file: {ex.Message}");
            SendError(response, HttpStatusCode.InternalServerError, "Internal server error");
        }
        finally
        {
            response.OutputStream.Close();
        }
    }
    
    private void SendError(HttpListenerResponse response, HttpStatusCode statusCode, string message)
    {
        response.StatusCode = (int)statusCode;
        string errorPage = $@"
<!DOCTYPE html>
<html>
<head>
<title>Error {(int)statusCode} - {statusCode}</title>
<style>
    body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
    h1 {{ color: #333; }}
    p {{ color: #666; }}
</style>
</head>
<body>
<h1>Error {(int)statusCode}: {statusCode}</h1>
<p>{message}</p>
<p><a href='/'>Go back to homepage</a></p>
</body>
</html>";
        
        byte[] buffer = Encoding.UTF8.GetBytes(errorPage);
        response.ContentType = "text/html";
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
    }
    
    private string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        return extension switch
        {
            ".html" => "text/html",
            ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            ".txt" => "text/plain",
            ".pdf" => "application/pdf",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
    
    public void Stop()
    {
        if (_listener.IsListening)
        {
            _listener.Stop();
            Console.WriteLine("\nServer stopped.");
        }
    }
    
    public static async Task RunServerAsync(int port = 8080)
    {
        var server = new SimpleWebServer(port);
        
        // Handle Ctrl+C gracefully
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            server.Stop();
        };
        
        await server.StartAsync();
    }
    
    public void run()
    {        
        try
        {
            RunServerAsync(_port).Wait();
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"Server error: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
