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
using System.Text.RegularExpressions;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Replacement for <see cref="MessageHandlerAttribute"/>s with non-strict Message ID.
    /// All you need to do is attach it to a static method with a right <see cref="Riptide"/> message signature.
    /// After that, provide message type in attribute.
    /// It is mandatory that specified message inherits <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/>.
    /// <para>
    /// Alternatively, you can simply replace <see cref="Riptide.Message"/> with your <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> implementation.
    /// It will automatically read-out the message and send <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> as method parameter.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Non-strict implementation makes it impossible to play games with different networking mods installed.
    /// Mods also has to be initialized in one set order, but it should be handled by <see cref="Engine"/> automatically anyway.
    /// </remarks>
    /// Note: (TODO) Allows multiple so you can define same methods in multiple groups.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class AdvancedMessageAttribute : Attribute
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Class that declares mod data. Must implement <see cref="IMod{T}"/>.
        /// </summary>
        public Type Mod { get; set; } = null;
        /// <summary>
        /// Type of the message group. Must inherit <see cref="NetworkGroup{TGroup}"/>.
        /// </summary>
        public Type Group { get; set; } = null;
        /// <summary>
        /// Type of the message container. Must inherit <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/>.
        /// </summary>
        public Type Message { get; set; } = null;
        /// <summary>
        /// Static GroupID of this message handler. Automatic ID assignment will avoid static IDs.
        /// </summary>
        public byte? GroupID { get; set; } = null;
        /// <summary>
        /// Static MessageID of this message handler. Automatic ID assignment will avoid static IDs.
        /// </summary>
        public ushort? MessageID { get; set; } = null;




        /// <summary>
        /// Default constructor for the attribute.
        /// </summary>
        /// <param name="messageType">
        /// Type of the message this attribute employs. Target type must inherit <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/>.
        /// </param>
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AdvancedMessageAttribute() { }
        public AdvancedMessageAttribute(byte groupID) => GroupID = groupID;
        public AdvancedMessageAttribute(ushort messageID) => MessageID = messageID;
        public AdvancedMessageAttribute(byte groupID, ushort messageID)
        {
            GroupID = groupID;
            MessageID = messageID;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                1 Multi-type
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AdvancedMessageAttribute(Type multicast)
        {
            ResolveMultitype(multicast);
        }

        public AdvancedMessageAttribute(Type multicast, byte groupID)
        {
            ResolveMultitype(multicast);
            SetGroupID(groupID);
        }

        public AdvancedMessageAttribute(Type multicast, ushort messageID)
        {
            ResolveMultitype(multicast);
            SetMessageID(messageID);
        }

        public AdvancedMessageAttribute(Type multicast, byte groupID, ushort messageID)
        {
            ResolveMultitype(multicast);
            SetGroupID(groupID);
            SetMessageID(messageID);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                2 Multi-types
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AdvancedMessageAttribute(Type multicast1, Type multicast2)
        {
            ResolveMultitype(multicast1);
            ResolveMultitype(multicast2);
        }

        public AdvancedMessageAttribute(Type multicast1, Type multicast2, byte groupID)
        {
            ResolveMultitype(multicast1);
            ResolveMultitype(multicast2);
            SetGroupID(groupID);
        }

        public AdvancedMessageAttribute(Type multicast1, Type multicast2, ushort messageID)
        {
            ResolveMultitype(multicast1);
            ResolveMultitype(multicast2);
            SetMessageID(messageID);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                3 Multi-types
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AdvancedMessageAttribute(Type multicast1, Type multicast2, Type multicast3)
        {
            ResolveMultitype(multicast1);
            ResolveMultitype(multicast2);
            ResolveMultitype(multicast3);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void ResolveMultitype(in Type value)
        {
            // TODO: Add mod check.
            if (typeof(NetworkGroup).IsAssignableFrom(value))
            {
                if (Group is null) Group = value;
                else throw new ArgumentException($"Provided multiple {nameof(NetworkGroup)} types in an advanced message handler attribute! This is not allowed!");
                return;
            }
            else if (typeof(NetworkMessage).IsAssignableFrom(value))
            {
                if (Message is null) Message = value;
                else throw new ArgumentException($"Provided multiple {nameof(NetworkMessage)} types in an advanced message handler attribute! This is not allowed!");
                return;
            }
            else
            {
                throw new NotSupportedException($"Cannot resolve multitype ({value.Name}) in {nameof(AdvancedMessageAttribute)}!");
            }
        }

        private void SetGroupID(in byte groupID)
        {
            if (Group is null) GroupID = groupID;
            else throw new ArgumentException($"Specified {nameof(NetworkGroup)} in two different ways - explicitly and via auto-type. This is not allowed!");
        }

        private void SetMessageID(in ushort messageID)
        {
            if (Message is null) MessageID = messageID;
            else throw new ArgumentException($"Specified {nameof(NetworkMessage)} in two different ways - explicitly and via auto-type. This is not allowed!");
        }
    }
}
