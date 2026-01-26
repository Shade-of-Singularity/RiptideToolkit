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
using System;
using System.Reflection;

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Describes methods with <see cref="AdvancedMessageAttribute"/> attached.
    /// </summary>
    public readonly struct MessageHandlerInfo : IStructValidator
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
        /// <see cref="Messages.NetworkMessage{TMessage, TProfile}"/> type or <see cref="Message"/>.
        /// </summary>
        public readonly Type MessageType;
        /// <summary>
        /// Whether to release <see cref="MessageType"/> after calling this handler.
        /// If set to <c>false</c> - you will have to call <see cref="Message.Release"/> or <see cref="Messages.NetworkMessage.Release"/> manually.
        /// </summary>
        public readonly bool Release;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="method">Method info to use for invocation.</param>
        /// <param name="messageType">Type in the message data holder, and first parameter of the <paramref name="method"/>.</param>
        public MessageHandlerInfo(MethodInfo method, Type messageType, bool release)
        {
            Method = method;
            MessageType = messageType;
            Release = release;
        }
    }
}
