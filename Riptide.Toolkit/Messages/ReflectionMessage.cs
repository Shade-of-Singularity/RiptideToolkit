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

using System;

namespace Riptide.Toolkit.Messages
{
    /// <summary>
    /// Exist with a sole purpose of simplifying reflections.
    /// Never used in networking.
    /// </summary>
    public sealed class ReflectionMessage : NetworkMessage<ReflectionMessage>
    {
        /// <inheritdoc/>
        /// <remarks>
        /// With <see cref="ReflectionMessage"/> - throws immediately when this method is called.
        /// </remarks>
        public override Message Read(Message message) => throw new InvalidOperationException($"Cannot read message with {nameof(ReflectionMessage)}, by design.");

        /// <inheritdoc/>
        /// <remarks>
        /// With <see cref="ReflectionMessage"/> - throws immediately when this method is called.
        /// </remarks>
        public override Message Write(Message message) => throw new InvalidOperationException($"Cannot write message with {nameof(ReflectionMessage)}, by design.");

        /// <inheritdoc/>
        /// <remarks>
        /// With <see cref="ReflectionMessage"/> - throws immediately when this method is called.
        /// </remarks>
        protected override void Dispose() => throw new InvalidOperationException($"Cannot dispose {nameof(ReflectionMessage)}, by design.");
    }
}
