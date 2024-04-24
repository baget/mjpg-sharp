/*
    MIT License

    Copyright (c) 2024 Oren Weil

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

// Listing Port 
const int PORT = 8080;

// Binding Address
string ALL_ADDR = $"http://*:{PORT}/";

// Initialize camera
using var capture = new VideoCapture(0); // Use the appropriate camera index
var running = true;

// Set up HTTP server
var listener = new HttpListener();
listener.Prefixes.Add(ALL_ADDR);
listener.Start();

Console.WriteLine($"MJPG# - .NET MJPEG Streamer - Server started. Listening on Port {PORT}");

Console.CancelKeyPress += Console_CancelKeyPress;

while (running)
{
    var context = await listener.GetContextAsync();

    if (context == null)
    { 
        break; 
    }

    var thread = new Thread(() =>
    {
        switch (context.Request.Url.LocalPath)
        {
            case "/snapshot":
                {
                    // Read a frame from the camera
                    using var frame = new Mat();
                    capture.Read(frame);

                    // Encode frame to JPEG
                    var jpegBytes = frame.ToBytes(".jpg");

                    // Send response
                    context.Response.ContentType = "image/jpeg";
                    context.Response.OutputStream.Write(jpegBytes);
                    break;
                }

            case "/stream":
                {
                    try
                    {
                        // Set the content type for the MJPEG stream (Multipart)
                        context.Response.ContentType = "multipart/x-mixed-replace; boundary=image-boundary";


                        // Continuously send frames
                        while (true)
                        {
                            // Read a frame from the camera
                            using var frame = new Mat();
                            capture.Read(frame);

                            // Encode frame to JPEG
                            var jpegBytes = frame.ToBytes(".jpg");

                            // Send the a frame
                            var boundary = Encoding.ASCII.GetBytes("\r\n--image-boundary\r\n");
                            context.Response.OutputStream.Write(boundary);
                            context.Response.OutputStream.Write(Encoding.ASCII.GetBytes("Content-Type: image/jpeg\r\n\r\n"));
                            context.Response.OutputStream.Write(jpegBytes);
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        context.Response.Close();
                    }

                    break;
                }

            case "/":
                {
                    var html = @"
                    <!DOCTYPE html>
                    <html lang=""en"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>MJPEG Stream</title>
                    </head>
                    <body>
                        <h1>MJPEG Stream</h1>
                        <img src=""/stream"" alt=""MJPEG Stream"">
                    </body>
                    </html>";

                    var htmlBytes = Encoding.UTF8.GetBytes(html);
                    context.Response.ContentType = "text/html";
                    context.Response.OutputStream.Write(htmlBytes);
                    context.Response.Close();
                    break;
                }

            default:
                context.Response.StatusCode = 404;
                context.Response.Close();
                break;
        }
    });

    thread.Start();
}

Console.WriteLine("Bye");

void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
{
    running = false;
}