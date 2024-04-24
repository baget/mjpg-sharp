# Mjpg# (MJpg-Sharp) - A quick and dirty .NET MJPEG Streamer

[![Tech Stack](https://skillicons.dev/icons?i=cs,dotnet,opencv,visualstudio)](https://skillicons.dev)

This project implements an MJPEG streamer using C# and OpenCvSharp. It allows you to stream video frames from a camera over HTTP, making it accessible through a web browser or other clients.

## Features

- **Snapshot Endpoint**: Capture a single frame from the camera and retrieve it as a JPEG image.
- **Streaming Endpoint**: Continuously stream frames from the camera as an MJPEG stream.

## Prerequisites

- .NET Core SDK (or .NET Framework)
- OpenCvSharp4 (installed via NuGet)

## Usage

1. Clone this repository.
2. Build the project using Visual Studio 2022 or `dotnet` command line.
3. Run the compiled executable.

## HTTP Endpoints

- **Snapshot**: `/snapshot`
  - Captures a single frame from the camera and returns it as a JPEG image.
- **Streaming**: `/stream`
  - Provides a continuous MJPEG stream of frames from the camera.

## How to Run

1. Compile the project.
2. Execute the compiled binary as elevated admin (since `HttpListener` requried it)
3. Open a web browser and navigate to `http://localhost:8080` to view the MJPEG stream.

### Running as Non Elevated Admin
https://stackoverflow.com/questions/4019466/httplistener-access-denied

## License

This project is licensed under the MIT License. Feel free to use, modify, and distribute it as needed.
