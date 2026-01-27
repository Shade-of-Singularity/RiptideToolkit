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
                message = NetMessage.Create(mode);
            }
            else
            {
                message.SkipBits(message.UnreadBits);
            }

            return message.AddUShort(messageID);
        }
    }
}
