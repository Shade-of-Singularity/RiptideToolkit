/// - - -    Copyright (c) 2026     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the MIT License. Permission is hereby granted, free of charge,
/// to any person obtaining a copy of this software and associated documentation
/// files to deal in the Software without restriction. Full license terms are
/// available in the LICENSE.md file located at the following repository path:
///   
///                 "Eclipse/Eclipse.Riptide/LICENSE.md"
/// 
/// Note: Eclipse.Riptide and Eclipse are licensed under different licenses.
/// See "Eclipse/LICENSE.md" for details.
/// 
/// ]]>

using Eclipse.Riptide.Handlers;
using Riptide;
using Riptide.Transports;
using Riptide.Transports.Udp;
using Riptide.Utils;
using UnityEngine;

namespace Eclipse.Riptide
{
    /// <summary>
    /// Custom <see cref="global::Riptide"/> <see cref="Client"/> which supports runtime Message ID identification.
    /// </summary>
    public sealed class EclipseClient : Client
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private ClientHandlers? m_MessageHandlers;
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
        public EclipseClient(string logName = "CLIENT") : this(new UdpClient(), logName) { }

        /// <summary>
        /// Handles initial setup.
        /// </summary>
        /// <param name="transport">The transport to use for sending and receiving data.</param>
        /// <param name="logName">The name to use when logging messages via <see cref="RiptideLogger"/>.</param>
        public EclipseClient(IClient transport, string logName = "CLIENT") : base(transport, logName)
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
            if (m_MessageHandlers?.TryFire(args.MessageId, args.Message) != true)
            {
                Debug.LogWarning($"No Client-side Eclipse message handler method found for message ID ({args.MessageId})!");
            }
        }
    }
}
