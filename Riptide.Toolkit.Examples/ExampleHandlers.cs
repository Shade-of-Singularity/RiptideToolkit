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

using Riptide.Toolkit.Messages;
using Riptide.Transports;
using Riptide.Utils;

namespace Riptide.Toolkit.Examples
{
    /// <summary>
    /// Contains client-side and server-side message handlers, in almost all possible variations.
    /// </summary>
    /// TODO: Fill with all possible variations.
    public static class ExampleHandlers
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Client-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [AdvancedMessage((uint)ToClientMessages.UpdateUsername)]
        public static void UpdateUsername(Message message)
        {
            RiptideLogger.Log(LogType.Warning, $"");
            RiptideLogger.Log(LogType.Warning, $"[Client] Received client's ({message.GetUShort()}) new username ({message.GetString()})");
            RiptideLogger.Log(LogType.Warning, $"");
        }

        // Defines message handler in both groups.
        public sealed class ExampleGroup : NetworkGroup<ExampleGroup> { }

        // DefaultGroup is defined by default and has GroupID of '0'.
        [AdvancedMessage((uint)ToClientMessages.UpdatePlayerPosition, typeof(DefaultGroup))]
        [AdvancedMessage((uint)ToClientMessages.UpdatePlayerPosition, typeof(ExampleGroup))]
        public static void UpdatePlayerPosition(Message message)
        {
            RiptideLogger.Log(LogType.Warning, $"");
            RiptideLogger.Log(LogType.Warning, $"[Client] Received player position for at ({message.GetFloat()}, {message.GetFloat()})");
            RiptideLogger.Log(LogType.Warning, $"");
        }

        // Defines new message.
        public sealed class VFXSignal : FlagMessage<VFXSignal> { }

        // Will evaluate MessageID from VFXSignal class.
        [AdvancedMessage(typeof(DefaultGroup))]
        [AdvancedMessage(typeof(ExampleGroup))]
        public static void HandleVFXSignal(VFXSignal _)
        {
            RiptideLogger.Log(LogType.Warning, $"");
            RiptideLogger.Log(LogType.Warning, $"[Client] VFX Signal received.");
            RiptideLogger.Log(LogType.Warning, $"");
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Server-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [AdvancedMessage((uint)ToServerMessages.RegisterUsername)]
        public static void RequestHandler(ushort client, Message message)
        {
            RiptideLogger.Log(LogType.Warning, $"");
            RiptideLogger.Log(LogType.Warning, $"[Server] Received new username ({message.GetString()}) for user ({client})");
            RiptideLogger.Log(LogType.Warning, $"");
        }


        [AdvancedMessage((uint)ToServerMessages.ReceivePlayerPosition, typeof(DefaultGroup))]
        [AdvancedMessage((uint)ToServerMessages.ReceivePlayerPosition, typeof(ExampleGroup))]
        public static void ReceivePlayerPosition(ushort client, Message message)
        {
            RiptideLogger.Log(LogType.Warning, $"");
            RiptideLogger.Log(LogType.Warning, $"[Server] Received player position for ({client}) at ({message.GetFloat()}, {message.GetFloat()})");
            RiptideLogger.Log(LogType.Warning, $"");
        }

        [AdvancedMessage(typeof(DefaultGroup))]
        [AdvancedMessage(typeof(ExampleGroup))]
        public static void ReceiveVFXSignal(ushort client, VFXSignal _)
        {
            RiptideLogger.Log(LogType.Warning, $"");
            RiptideLogger.Log(LogType.Warning, $"[Server] VFX Signal received.");
            RiptideLogger.Log(LogType.Warning, $"");
        }
    }
}
