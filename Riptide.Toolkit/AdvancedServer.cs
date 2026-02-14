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
using Riptide.Toolkit.Relaying;
using Riptide.Transports;
using Riptide.Transports.Udp;
using Riptide.Utils;
using System;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Custom <see cref="Riptide"/> <see cref="Server"/> which supports runtime Message ID identification.
    /// </summary>
    /// <remarks>
    /// HEADS-UP! <see cref="Server.RelayFilter"/> is NOT supported at the moment!
    /// </remarks>
    /// Note: "Advanced" because of a lack of a better name. Might be changed before final release.
    public class AdvancedServer : Server
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="Server.MessageReceived"/>
        /// <remarks>
        /// Original <see cref="Server.MessageReceived"/> will not fire!
        /// </remarks>
        public new event EventHandler<AdvancedMessageReceivedEventArgs> MessageReceived;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="Server.RelayFilter"/>
        /// <remarks>
        /// Original <see cref="Server.RelayFilter"/> will not fire!
        /// </remarks>
        public new AdvancedRelayFilter RelayFilter;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private ServerHandlers m_MessageHandlers;




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
        public AdvancedServer(IServer transport, string logName = "SERVER") : base(transport, logName) { }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected override void CreateMessageHandlersDictionary(byte groupID) => m_MessageHandlers = ServerHandlers.Create(groupID);
        protected override void OnMessageReceived(Message message, Connection fromConnection)
        {
            message.GetSystemMessageID(out SystemMessageID systemMessageID);
            message.SkipHeaders(systemMessageID);

            uint messageID = (uint)message.GetVarULong();
            if (RelayFilter != null && RelayFilter.ShouldRelay(messageID))
            {
                // The message should be automatically relayed to clients instead of being handled on the server
                // TODO: Test this part.
                SendToAll(message, fromConnection.Id);
                return;
            }

            var args = new AdvancedMessageReceivedEventArgs(fromConnection, messageID, message);
            MessageReceived?.Invoke(this, args);
            if (useMessageHandlers)
            {
                switch (systemMessageID)
                {
                    // Allows regular execution.
                    case SystemMessageID.Private:
                    case SystemMessageID.Regular: break;

                    // Fires custom handlers.
                    default: NetworkIndex.HandleServer((byte)systemMessageID, this, args); return;
                }

                // Handles regular messages:
                if (!m_MessageHandlers.TryFire(this, messageID, fromConnection.Id, message))
                {
                    RiptideLogger.Log(LogType.Warning, $"No Server-side advanced message handler found for MessageID ({messageID})!");
                }
            }
        }
    }
}
