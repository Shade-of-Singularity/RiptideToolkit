#nullable disable // Disabled to not confuse readers with compiler warnings.
using Riptide.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace Riptide.Toolkit.Examples
{
    /// <summary>
    /// Shows some ways you can use Riptide now.
    /// </summary>
    /// TODO: Make message handling dynamically adjust delays,
    ///  as with ping messages will take more time to deliver.
    public static class Basics
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Default IP to use by <see cref="Client"/> to connect to the <see cref="Server"/>.
        /// "127.0.0.1" will sent the data back to your PC.
        /// </summary>
        public const string DefaultServerIP = "127.0.0.1";

        /// <summary>
        /// Port on which <see cref="Server"/> will be hosted.
        /// </summary>
        /// TODO: Automatically change if occupied.
        public const ushort DefaultServerPort = 45840;

        /// <summary>
        /// Lists all the example mods.
        /// </summary>
        /// <remarks>
        /// Ignore that it is not <see cref="BaseMod{T}"/> but <see cref="IDirectModAccess"/>.
        /// <see cref="IDirectModAccess"/> - is an interface, which hides methods mod developers should not access.
        /// </remarks>
        internal static readonly IDirectModAccess[] Mods = new IDirectModAccess[]
        {
            ExampleMod.Instance,
        };




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Entry Point
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static async void Run(string ip = DefaultServerIP, ushort port = DefaultServerPort)
        {
            // Some systems might require initialization.
            // This method will initialize all the systems needed during development.
            // Some things are not mandatory to run.
            Initialize();

            // Starts our new custom server.
            StartServer(port);

            // Starts our new custom client.
            StartClient(ip, port);

            // Starts constant server and client updates, so they can send each other messages.
            StartUpdates();

            // Gives Server and Client some time to establish connection.
            await LetServerAndClientEstablishConnection();

            // Tests common and advanced client usage.
            await TestClient();

            // Tests common and advanced server usage.
            await TestServer();

            // Unloads everything and finishes testing.
            await ConcludeTest();
        }

        /// <summary>
        /// Stops all running systems.
        /// </summary>
        public static void Stop()
        {
            Client?.Disconnect();
            Server?.Stop();

            ShutdownToken.Cancel();
            ShutdownToken.Dispose();
            ShutdownToken = new();

            // As an example - unloads all mods as well.
            foreach (var mod in Mods)
            {
                mod.InvokeUnload();
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static CancellationTokenSource ShutdownToken = new(); // Used to shutdown example.
        private static AdvancedServer Server;
        private static AdvancedClient Client;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void Initialize()
        {
            RiptideLogger.Log(LogType.Info, "Initializing Riptide.Toolkit Basics test.");

            // Here mods can register custom message handlers.
            foreach (var mod in Mods)
            {
                mod.InvokeRegisterMessages();
            }

            // Initializes mod as an example.
            // Note: It is recommended that you first run message registration on all mods, before initialization.
            foreach (var mod in Mods)
            {
                mod.InvokeInitialize();
            }
        }

        private static void StartServer(ushort port)
        {
            RiptideLogger.Log(LogType.Info, $"Starting testing server on port ({port})...");

            Server = new AdvancedServer();
            Server.ClientConnected += (s, e) => RiptideLogger.Log(LogType.Info, $"Client connected! ClientID: {e.Client.Id}");
            Server.Start(port, maxClientCount: 8);

            RiptideLogger.Log(LogType.Info, $"Server is started successfully.");
        }

        private static void StartClient(string ip, ushort port)
        {
            RiptideLogger.Log(LogType.Info, $"Connecting testing client to server at ({ip}:{port})...");
            Client = new AdvancedClient();
            Client.Connect($"{ip}:{port}");
        }

        private static async void StartUpdates()
        {
            CancellationToken token = ShutdownToken.Token;
            while (!token.IsCancellationRequested)
            {
                Server?.Update();
                await Task.Delay(3);
                Client?.Update();
                await Task.Delay(3);
            }
        }

        private static async Task LetServerAndClientEstablishConnection()
        {
            RiptideLogger.Log(LogType.Info, "Letting server and client settle...");
            await Task.Delay(100);

            RiptideLogger.Log(LogType.Info, "Waiting is done.");
            await Task.Delay(50); // Additional delay, to make logs look better in console.
        }

        private static async Task WaitForMessageToDeliver() => await Task.Delay(25);
        private static async Task TestClient()
        {
            RiptideLogger.Log(LogType.Info, "Testing client...");

            // TODO: Add client-side testing methods.
            await WaitForMessageToDeliver();

            RiptideLogger.Log(LogType.Info, "Client test finished.");
            await Task.Delay(50);
        }

        private static async Task TestServer()
        {
            RiptideLogger.Log(LogType.Info, "Testing server...");

            // TODO: Add server-side testing methods.
            await WaitForMessageToDeliver();

            RiptideLogger.Log(LogType.Info, "Server test finished.");
            await Task.Delay(50);
        }

        private static async Task ConcludeTest()
        {
            await Task.Delay(10);
            Stop(); // Automatically stops everything.

            RiptideLogger.Log(LogType.Info, "Basics test concluded!");
            await Task.Delay(50);
        }
    }
}
