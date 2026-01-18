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

using Riptide.Toolkit.Handlers;
using Riptide.Transports;
using Riptide.Transports.Udp;
using Riptide.Utils;
using System;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Custom <see cref="Riptide"/> <see cref="Server"/> which supports runtime Message ID identification.
    /// </summary>
    /// Note: "Advanced" because of a lack of a better name. Might be changed before final release.
    public sealed class AdvancedServer : Server
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private ServerHandlers m_MessageHandlers;
        private bool m_BroadcastToHandlers;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Handles initial setup using the built-in UDP transport.
        /// </summary>
        /// <param name="transport">The transport to use for sending and receiving data.</param>
        /// <param name="logName">The name to use when logging messages via <see cref="RiptideLogger"/>.</param>
        public AdvancedServer(string logName = "SERVER") : this(new UdpServer(), logName) { }

        /// <summary>
        /// Handles initial setup.
        /// </summary>
        /// <param name="transport">The transport to use for sending and receiving data.</param>
        /// <param name="logName">The name to use when logging messages via <see cref="RiptideLogger"/>.</param>
        public AdvancedServer(IServer transport, string logName = "SERVER") : base(transport, logName)
        {
            // We also cannot override built-in handling method without breaking the message, so we just route message manually via callback.
            MessageReceived += ServerBroadcastMessage;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected override void CreateMessageHandlersDictionary(byte groupID) => m_MessageHandlers = NetworkIndex.ServerHandlers(groupID);
        protected override void OnMessageReceived(Message message, Connection fromConnection)
        {
            bool flag = useMessageHandlers;

            // We use custom handling method, so we need silence built-in handling as a result.
            useMessageHandlers = false;
            m_BroadcastToHandlers = flag;

            // This will fire MessageReceived callback, no matter the handler flag.
            base.OnMessageReceived(message, fromConnection);

            // Restores previous state.
            m_BroadcastToHandlers = false;
            useMessageHandlers = flag;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void ServerBroadcastMessage(object sender, MessageReceivedEventArgs args)
        {
            if (!m_BroadcastToHandlers) return;
            NetMessage.ReadHeaders(args.Message, out bool isModded, out SystemMessageID systemMessageID, out ushort modID);
            switch (systemMessageID)
            {
                // Allows regular execution.
                case SystemMessageID.Regular: break;

                // Sends message data to a requesting side. Timeouts if response is not satisfied in time.
                case SystemMessageID.Request:
                case SystemMessageID.Response: throw new NotImplementedException();

                // Fires custom handlers.
                default: NetworkIndex.HandleServer((byte)systemMessageID, this, args); return;
            }

            // Handles regular messages:
            if (isModded)
            {
                if (m_MessageHandlers?.TryFire(this, modID, args.MessageId, args.FromConnection.Id, args.Message) != true)
                {
                    RiptideLogger.Log(LogType.Warning, $"No Server-side advanced message handler found in mod ({modID}) for MessageID ({args.MessageId})!");
                }
            }
            else
            {
                if (m_MessageHandlers?.TryFire(this, args.MessageId, args.FromConnection.Id, args.Message) != true)
                {
                    RiptideLogger.Log(LogType.Warning, $"No Server-side advanced message handler found for MessageID ({args.MessageId})!");
                }
            }
        }
    }
}
