namespace Riptide.Toolkit.Extensions
{
    public static class Messaging
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Responds to a given <paramref name="message"/> by firing a <paramref name="messageID"/> handler on it.
        /// </summary>
        /// <remarks>
        /// If <paramref name="message"/> <see cref="MessageSendMode"/> is different than a specified one -
        /// <paramref name="message"/> will be released, and new one will be created.
        /// </remarks>
        public static Message Respond(this Message message, ushort messageID, MessageSendMode mode = MessageSendMode.Reliable)
        {
            if (message.SendMode != mode)
            {
                message.Release();
                message = Message.Create(mode);
            }
            else
            {
                message.SkipBits(message.UnreadBits);
            }

            return message.AddUShort(messageID);
        }
    }
}
