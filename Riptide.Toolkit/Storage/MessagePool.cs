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

namespace Riptide.Toolkit.Storage
{
    /// <summary>
    /// Stores reference to a <see cref="StructPool{TValue}"/>
    /// - <see cref="IPool"/> implementation for <see cref="NetworkMessage"/>s.
    /// </summary>
    /// <typeparam name="TMessage">Message type for which pool is needed.</typeparam>
    public static class MessagePool<TMessage> where TMessage : NetworkMessage<TMessage>, new()
    {
        public static readonly IPool<TMessage> Instance = new StructPool<TMessage>();
        static MessagePool() => MessagePools.Define(typeof(TMessage), Instance);
    }
}
