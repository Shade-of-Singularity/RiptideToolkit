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
    /// Custom <see cref="Riptide"/> <see cref="Client"/> which supports runtime Message ID identification.
    /// </summary>
    /// Note: "Advanced" because of a lack of a better name. Might be changed before final release.
    public sealed class AdvancedClient : Client
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private ClientHandlers m_MessageHandlers;
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
        public AdvancedClient(string logName = "CLIENT") : this(new UdpClient(), logName) { }

        /// <summary>
        /// Handles initial setup.
        /// </summary>
        /// <param name="transport">The transport to use for sending and receiving data.</param>
        /// <param name="logName">The name to use when logging messages via <see cref="RiptideLogger"/>.</param>
        public AdvancedClient(IClient transport, string logName = "CLIENT") : base(transport, logName)
        {
            // We also cannot override built-in handling method without breaking the message, so we just route message manually via callback.
            MessageReceived += ClientBroadcastMessage;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected override void CreateMessageHandlersDictionary(byte groupID) => m_MessageHandlers = NetworkIndex.ClientHandlers(groupID);
        protected override void OnMessageReceived(Message message)
        {
            bool flag = useMessageHandlers;

            // We use custom handling method, so we need silence built-in handling as a result.
            useMessageHandlers = false;
            m_BroadcastToHandlers = flag;

            // This will fire MessageReceived callback, no matter the handler flag.
            base.OnMessageReceived(message);

            // Restores previous state.
            m_BroadcastToHandlers = false;
            useMessageHandlers = flag;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void ClientBroadcastMessage(object sender, MessageReceivedEventArgs args)
        {
            if (!m_BroadcastToHandlers) return;
            NetMessage.ReadHeaders(args.Message, out SystemMessageID systemMessageID);
            switch (systemMessageID)
            {
                // Allows regular execution.
                case SystemMessageID.Regular: break;

                // Sends message data to a requesting side. Timeouts if response is not satisfied in time.
                case SystemMessageID.ToSingle:
                case SystemMessageID.ToAll:
                case SystemMessageID.Request:
                case SystemMessageID.Response: throw new NotImplementedException();

                // Fires custom handlers.
                default: NetworkIndex.HandleClient((byte)systemMessageID, this, args); return;
            }

            // Handles regular messages:
            if (m_MessageHandlers.TryFire(this, args.MessageId, args.Message) != true)
            {
                RiptideLogger.Log(LogType.Warning, $"No Client-side advanced message handler found for MessageID ({args.MessageId})!");
            }
        }
    }
}
