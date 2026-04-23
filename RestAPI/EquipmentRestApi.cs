using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestAPI
{
    public enum OperationMode
    {
        OPERATIONAL,
        STANDBY,
        FAILURE
    }

    public class EquipmentRestApi
    {
        private readonly HttpListener _listener;
        private readonly int _port;
        private OperationMode _currentOperationMode;
        private readonly Dictionary<string, object> _parameters;

        public EquipmentRestApi(int port = 8080)
        {
            _port = port;
            _currentOperationMode = OperationMode.STANDBY;
            _parameters = new Dictionary<string, object>
            {
                { "operationMode", _currentOperationMode },
                { "lastUpdated", DateTime.UtcNow },
                { "equipmentName", "testHardware" },
                { "version", "1.0.0" }
            };

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{port}/");
            _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        }

        public async Task StartAsync()
        {
            try
            {
                _listener.Start();
                Console.WriteLine($"Equipment REST API Server started on port {_port}");
                Console.WriteLine($"API Endpoints:");
                Console.WriteLine($"  GET  /api/equipment/testHardware - Get all parameters");
                Console.WriteLine($"  GET  /api/equipment/testHardware?operationalStatus=OPERATIONAL|STANDBY|FAILURE - Set operation mode");
                Console.WriteLine($"Press Ctrl+C to stop the server...\n");

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

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} - {request.HttpMethod} {request.Url.PathAndQuery}");

            try
            {
                string path = request.Url.LocalPath;
                string operationalStatus = request.QueryString["operationalStatus"];

                // Handle API routes
                if (path.Equals("/api/equipment/testHardware", StringComparison.OrdinalIgnoreCase))
                {
                    if (request.HttpMethod == "GET")
                    {
                        if (string.IsNullOrEmpty(operationalStatus) == false)
                        {
                            await HandleSetOperationModeAsync(request, response);
                        }
                        else 
                        {
                            await HandleGetParametersAsync(response);
                        }
                    }
                    else
                    {
                        SendError(response, HttpStatusCode.MethodNotAllowed, "Method not allowed. Use GET or PUT.");
                    }
                }
                else
                {
                    SendError(response, HttpStatusCode.NotFound, "Endpoint not found. Available endpoint: /api/equipment/testHardware");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing API request: {ex.Message}");
                SendError(response, HttpStatusCode.InternalServerError, "Internal server error");
            }
            finally
            {
                response.OutputStream.Close();
            }
        }

        private async Task HandleGetParametersAsync(HttpListenerResponse response)
        {
            // Update timestamp
            _parameters["lastUpdated"] = DateTime.UtcNow;
            _parameters["operationMode"] = _currentOperationMode;

            var responseData = new
            {
                equipment = "testHardware",
                parameters = _parameters,
                timestamp = DateTime.UtcNow
            };

            string jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseData, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentType = "application/json";
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task HandleSetOperationModeAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            string operationalStatus = request.QueryString["operationalStatus"];

            if (string.IsNullOrEmpty(operationalStatus))
            {
                SendError(response, HttpStatusCode.BadRequest, "Missing required query parameter: operationalStatus");
                return;
            }

            if (Enum.TryParse<OperationMode>(operationalStatus, true, out OperationMode newMode))
            {
                OperationMode oldMode = _currentOperationMode;
                _currentOperationMode = newMode;
                _parameters["operationMode"] = _currentOperationMode;
                _parameters["lastUpdated"] = DateTime.UtcNow;

                Console.WriteLine($"Operation mode changed from {oldMode} to {newMode}");

                var responseData = new
                {
                    message = $"Operation mode set to {newMode}",
                    previousMode = oldMode.ToString(),
                    currentMode = newMode.ToString(),
                    timestamp = DateTime.UtcNow
                };

                string jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseData, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                SendError(response, HttpStatusCode.BadRequest, $"Invalid operationalStatus value: {operationalStatus}. Valid values: OPERATIONAL, STANDBY, FAILURE");
            }
        }

        private void SendError(HttpListenerResponse response, HttpStatusCode statusCode, string message)
        {
            response.StatusCode = (int)statusCode;
            var errorResponse = new
            {
                error = statusCode.ToString(),
                message = message,
                timestamp = DateTime.UtcNow
            };

            string jsonError = System.Text.Json.JsonSerializer.Serialize(errorResponse);
            byte[] buffer = Encoding.UTF8.GetBytes(jsonError);
            response.ContentType = "application/json";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Stop()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
                Console.WriteLine("\nREST API Server stopped.");
            }
        }

        public void Run()
        {
            try
            {
                StartAsync().Wait();
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

        public static async Task RunServerAsync(int port = 8080)
        {
            var server = new EquipmentRestApi(port);

            // Handle Ctrl+C gracefully
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                server.Stop();
            };

            await server.StartAsync();
        }
    }
}