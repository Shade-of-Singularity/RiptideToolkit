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

namespace Eclipse.Riptide
{
    /// <summary>
    /// Replacement for <see cref="MessageHandlerAttribute"/>s with non-strict Message ID.
    /// All you need to do is attach it to a static method with a right <see cref="global::Riptide"/> message signature.
    /// After that, provide message type in attribute.
    /// It is mandatory that specified message inherits <see cref="Messages.NetworkMessage{TMessage, TGroup, TProfile}"/>.
    /// <para>
    /// Alternatively, you can simply replace <see cref="Message"/> with your <see cref="Messages.NetworkMessage{TMessage, TGroup, TProfile}"/> implementation.
    /// It will automatically read-out the message and send <see cref="Messages.NetworkMessage{TMessage, TGroup, TProfile}"/> as method parameter.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Non-strict implementation makes it impossible to play games with different networking mods installed.
    /// Mods also has to be initialized in one set order, but it should be handled by <see cref="Engine"/> automatically anyway.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class EclipseMessageAttribute : Attribute
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Type of the message. Must inherit <see cref="Messages.NetworkMessage{TMessage, TGroup, TProfile}"/>.
        /// </summary>
        public readonly Type? MessageType;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Default constructor for the attribute.
        /// </summary>
        /// <param name="messageType">
        /// Type of the message this attribute employs. Target type must inherit <see cref="Messages.NetworkMessage{TMessage, TGroup, TProfile}"/>.
        /// </param>
        public EclipseMessageAttribute(Type? messageType = null)
        {
            MessageType = messageType;
        }
    }
}
