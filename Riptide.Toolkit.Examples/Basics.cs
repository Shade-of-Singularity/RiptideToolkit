/// - - Shade of Singularity Community - - - Tom Weiland & Riptide Community, 2026 - - <![CDATA[
/// 
/// Licensed under the MIT License. Permission is hereby granted, free of charge,
/// to any person obtaining a copy of this software and associated documentation
/// files to deal in the Software without restriction. Full license terms are
/// available in the LICENSE.md file located at the following repository path:
///   
///                        "RiptideToolkit/LICENSE.md"
/// 
/// ]]>

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
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether testing is currently running or not.
        /// </summary>
        /// Note: Used by Riptide.Toolkit.Testing to indicate when to quit waiting for example method to finish.
        public static bool IsRunning { get; private set; }




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
            ShutdownToken = new CancellationTokenSource();

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
        private static CancellationTokenSource ShutdownToken = new CancellationTokenSource(); // Used to shutdown example.
        private static AdvancedServer Server;
        private static AdvancedClient Client;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void Initialize()
        {
            IsRunning = true;
            RiptideLogger.Log(LogType.Info, "Initializing Riptide.Toolkit Basics test.");
            Message.MaxPayloadSize = Message.MaxHeaderSize + ushort.MaxValue + 1;

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
                await Task.Delay(2);
                Client?.Update();
                await Task.Delay(2);
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
            RiptideLogger.Log(LogType.Info, $"");
            RiptideLogger.Log(LogType.Info, "Testing client...");

            Message message = NetMessage.Create(0, MessageSendMode.Reliable, ToServerMessages.RegisterUsername)
                .AddString("New User");
            Client.Send(message);
            await WaitForMessageToDeliver();

            // TODO: Add more client-side testing methods.

            RiptideLogger.Log(LogType.Info, "Client test finished.");
            RiptideLogger.Log(LogType.Info, $"");
            await Task.Delay(50);
        }

        private static async Task TestServer()
        {
            RiptideLogger.Log(LogType.Info, $"");
            RiptideLogger.Log(LogType.Info, "Testing server...");

            const ushort ClientID = 1;
            Message message = NetMessage.Create(0, MessageSendMode.Reliable, ToClientMessages.UpdateUsername)
                .AddUShort(ClientID).AddString("New User");
            Server.SendToAll(message);
            await WaitForMessageToDeliver();

            // TODO: Add more server-side testing methods.

            RiptideLogger.Log(LogType.Info, "Server test finished.");
            RiptideLogger.Log(LogType.Info, $"");
            await Task.Delay(50);
        }

        private static async Task ConcludeTest()
        {
            await Task.Delay(10);
            Stop(); // Automatically stops everything.

            RiptideLogger.Log(LogType.Info, "Basics test concluded!");
            await Task.Delay(50);
            IsRunning = false;
        }
    }
}
