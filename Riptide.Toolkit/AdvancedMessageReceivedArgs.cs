using System;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Replica of <see cref="MessageReceivedEventArgs"/> with <see cref="uint"/> for <see cref="MessageId"/>.
    /// </summary>
    public sealed class AdvancedMessageReceivedEventArgs : EventArgs
    {
        /// <inheritdoc cref="MessageReceivedEventArgs.FromConnection"/>
        public readonly Connection FromConnection;

        /// <inheritdoc cref="MessageReceivedEventArgs.MessageId"/>
        public readonly uint MessageId;

        /// <inheritdoc cref="MessageReceivedEventArgs.Message"/>
        public readonly Message Message;

        /// <inheritdoc cref="MessageReceivedEventArgs(Connection, ushort, Message)"/>
        public AdvancedMessageReceivedEventArgs(Connection fromConnection, uint messageId, Message message)
        {
            FromConnection = fromConnection;
            MessageId = messageId;
            Message = message;
        }
    }
}
