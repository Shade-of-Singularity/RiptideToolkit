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
using System;
using System.Reflection;

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Collection of all server-side message handlers for specific handler group ID.
    /// </summary>
    public sealed class ServerHandlers : MessageHandlers<ServerHandlers.HandlerInfo>
    {
        /// <summary>
        /// Client-side handler info.
        /// </summary>
        public readonly struct HandlerInfo
        {
            /// <summary>
            /// Method which have to be invoked.
            /// </summary>
            /// <remarks>
            /// Using it over <see cref="Delegate.DynamicInvoke(object[])"/> should be better for performance. -Gemini (TODO: actually benchmark it)
            /// </remarks>
            public readonly MethodInfo Method;

            /// <summary>
            /// <see cref="INetworkMessage"/> type.
            /// </summary>
            public readonly Type MessageType;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="method">Method info to use for invocation.</param>
            /// <param name="dataType">Type in the message data holder, and second parameter (after clientID) of the <paramref name="method"/>.</param>
            public HandlerInfo(MethodInfo method, Type dataType)
            {
                Method = method;
                MessageType = dataType;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Attempts to fire message handler under given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">ID of a message handler.</param>
        /// <param name="clientID">Riptide ID of a client which sent the message.</param>
        /// <param name="message">Message to read.</param>
        /// <returns><c>false</c> if there is no handler under given <paramref name="id"/> registered. <c>true</c> otherwise.</returns>
        public bool TryFire(ushort id, ushort clientID, Message message)
        {
            if (Has(id))
            {
                Fire(id, clientID, message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fires handler with specified ID client-side.
        /// </summary>
        /// <param name="id">ID of a message handler.</param>
        /// <param name="clientID">Riptide ID of a client which sent the message.</param>
        /// <param name="message">Message to read.</param>
        public void Fire(ushort id, ushort clientID, Message message)
        {
            HandlerInfo info = Get(id);
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

            info.Method.Invoke(null, args);
        }
    }
}
