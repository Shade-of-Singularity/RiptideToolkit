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

using Riptide;
using System;

namespace Eclipse.Riptide.Messages
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
