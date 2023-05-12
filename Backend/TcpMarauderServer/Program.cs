// TCP server port
using System.Net;
using TcpMulticastServer;

int port = 1111;
if (args.Length > 0)
    port = int.Parse(args[0]);

Console.WriteLine($"TCP server port: {port}");

Console.WriteLine();

// Create a new TCP chat server
var server = new MulticastServer(IPAddress.Any, port);

// Start the server
Console.Write("Server starting...");
server.Start();
Console.WriteLine("Done!");

Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

// Perform text input
for (; ; )
{
    string line = Console.ReadLine();
    if (string.IsNullOrEmpty(line))
        break;

    // Restart the server
    if (line == "!")
    {
        Console.Write("Server restarting...");
        server.Restart();
        Console.WriteLine("Done!");
        continue;
    }

    // Multicast admin message to all sessions
    line = "(admin) " + line;
    server.Multicast(line);
}

// Stop the server
Console.Write("Server stopping...");
server.Stop();
Console.WriteLine("Done!");