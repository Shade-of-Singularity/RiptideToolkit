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

using Riptide.Toolkit.Extensions;
using Riptide.Toolkit.Messages;
using Riptide.Toolkit.Settings;
using System;
using System.Reflection;

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Collection of all server-side message handlers for specific handler group ID.
    /// </summary>
    public sealed class ServerHandlers : IMessageHandlerCollection<ServerHandlers.HandlerInfo>
    {
        /// <summary>
        /// Client-side handler info.
        /// </summary>
        public readonly struct HandlerInfo : IStructValidator
        {
            /// <inheritdoc/>
            bool IStructValidator.IsDefault => Method is null;

            /// <summary>
            /// Method which have to be invoked.
            /// </summary>
            /// <remarks>
            /// Using it over <see cref="Delegate.DynamicInvoke(object[])"/> should be better for performance. -Gemini (TODO: actually benchmark it)
            /// </remarks>
            public readonly MethodInfo Method;

            /// <summary>
            /// <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> type.
            /// </summary>
            public readonly Type MessageType;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="method">Method info to use for invocation.</param>
            /// <param name="messageType">Type in the message data holder, and second parameter (after clientID) of the <paramref name="method"/>.</param>
            public HandlerInfo(MethodInfo method, Type messageType)
            {
                Method = method;
                MessageType = messageType;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public MessageHandlerCollection<HandlerInfo> Handlers { get; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public ServerHandlers()
        {
            if (Performance.MessageHandlerFocus == PerformanceType.OptimizeCPU)
            {
                Handlers = new RegionHandlerCollection<HandlerInfo>(Performance.RegionSize);
            }
            else
            {
                Handlers = new DictionaryHandlerCollection<HandlerInfo>();
            }
        }

        public ServerHandlers(MessageHandlerCollection<HandlerInfo> handlers)
        {
            Handlers = handlers;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="TryFire(AdvancedServer, ushort, ushort, ushort, Message)"/>
        public bool TryFire(AdvancedServer server, ushort messageID, ushort clientID, Message message)
        {
            if (Handlers.Has(messageID))
            {
                Fire(server, messageID, clientID, message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to fire message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <param name="modID">ModID under which <paramref name="messageID"/> is registered.</param>
        /// <param name="messageID">ID of a message handler.</param>
        /// <param name="clientID">Riptide ID of a client which sent the message.</param>
        /// <param name="message">Message to read.</param>
        /// <returns><c>false</c> if there is no handler under given <paramref name="messageID"/> registered. <c>true</c> otherwise.</returns>
        public bool TryFire(AdvancedServer server, ushort modID, ushort messageID, ushort clientID, Message message)
        {
            if (Handlers.Has(modID, messageID))
            {
                Fire(server, modID, messageID, clientID, message);
                return true;
            }

            return false;
        }

        /// <inheritdoc cref="Fire(AdvancedServer, ushort, ushort, ushort, Message)"/>
        public void Fire(AdvancedServer server, ushort messageID, ushort clientID, Message message)
        {
            HandlerInfo info = Handlers.Get(messageID);
            object[] args = new object[2];
            args[0] = clientID;

            if (info.MessageType == typeof(Message))
            {
                args[1] = message;
            }
            else
            {
                NetworkMessage container = (NetworkMessage)Activator.CreateInstance(info.MessageType);
                container.Read(message);
                args[1] = container;
            }

            switch (info.Method.Invoke(null, args))
            {
                // If you return regular message - it will immediately send its contents as a response.
                // TODO: Make sure that FlagMessages will also be supported.
                case Message msg: server.SendResponse(msg, clientID); break;

                // Automatically packs and sends response message.
                // TODO: Make sure that MessageID is read correctly.
                case NetworkMessage net: server.SendResponse(net.Write(message), clientID); break;
                default: break;
            }
        }

        /// <summary>
        /// Fires handler with specified ID client-side.
        /// </summary>
        /// <param name="modID">ModID under which <paramref name="messageID"/> is registered.</param>
        /// <param name="messageID">ID of a message handler.</param>
        /// <param name="clientID">Riptide ID of a client which sent the message.</param>
        /// <param name="message">Message to read.</param>
        public void Fire(AdvancedServer server, ushort modID, ushort messageID, ushort clientID, Message message)
        {
            HandlerInfo info = Handlers.Get(modID, messageID);
            object[] args = new object[2];
            args[0] = clientID;

            if (info.MessageType == typeof(Message))
            {
                args[1] = message;
            }
            else
            {
                NetworkMessage container = (NetworkMessage)Activator.CreateInstance(info.MessageType);
                container.Read(message);
                args[1] = container;
            }

            switch (info.Method.Invoke(null, args))
            {
                // If you return regular message - it will immediately send its contents as a response.
                // TODO: Make sure that FlagMessages will also be supported.
                case Message msg: server.SendResponse(msg, clientID); break;

                // Automatically packs and sends response message.
                // TODO: Make sure that MessageID is read correctly.
                case NetworkMessage net: server.SendResponse(net.Write(message), clientID); break;
                default: break;
            }
        }

        /// <inheritdoc/>
        public HandlerInfo Get(ushort messageID) => Handlers.Get(messageID);

        /// <inheritdoc/>
        public HandlerInfo Get(ushort modID, ushort messageID) => Handlers.Get(messageID);

        /// <inheritdoc/>
        public bool Has(ushort messageID) => Handlers.Has(messageID);

        /// <inheritdoc/>
        public bool Has(ushort modID, ushort messageID) => Handlers.Has(messageID);

        /// <inheritdoc/>
        public bool TryGet(ushort messageID, out HandlerInfo hander) => Handlers.TryGet(messageID, out hander);

        /// <inheritdoc/>
        public bool TryGet(ushort modID, ushort messageID, out HandlerInfo hander) => Handlers.TryGet(messageID, out hander);
    }
}
