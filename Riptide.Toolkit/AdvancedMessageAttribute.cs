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
using Riptide.Toolkit.Settings;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

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
    /// TODO: Support methods without method parameters (for flag messages) and with 1 parameter (for server-side flag messages)
    /// TODO: Support <see cref="Enum"/> values, and use pattern matching for deciding between <see cref="bool"/> and <see cref="ushort"/>.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class AdvancedMessageAttribute : Attribute
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal const BindingFlags MethodBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Reference to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> with <see cref="GroupIDAttribute"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="GroupIDAttribute"/> is already defined in <see cref="NetworkGroup{TGroup}"/>
        /// and also <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> types.
        /// </remarks>
        public readonly PropertyInfo Group;
        /// <summary>
        /// Reference to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> with <see cref="MessageIDAttribute"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="ModIDAttribute"/> is already declared in all <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> types.
        /// </remarks>
        public readonly PropertyInfo Message;
        /// <summary>
        /// Static GroupID of this message handler. Automatic ID assignment will avoid static IDs.
        /// </summary>
        public readonly byte? GroupID; // Not read-only, to allow NetworkIndex to set ID to it, to speed-up analysis.
        /// <summary>
        /// Static MessageID of this message handler. Automatic ID assignment will avoid static IDs.
        /// </summary>
        public uint? MessageID; // Not read-only, to allow NetworkIndex to set ID to it, to speed-up analysis.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Constructs handler, <see cref="MessageID"/> for which is evaluated from parameters of a method to which this attribute is attached to.
        /// Uses default <see cref="GroupID"/> of '0'.
        /// </summary>
        public AdvancedMessageAttribute() => GroupID = 0;

        /// <summary>
        /// <para>Constructs handler attribute with given <paramref name="messageID"/>.</para>
        /// <para><see cref="GroupID"/> will be set to 0 (see also: <seealso cref="DefaultGroup"/>)</para>
        /// </summary>
        /// <param name="messageID">MessageID to associate a handler with.</param>
        public AdvancedMessageAttribute(uint messageID)
        {
            GroupID = 0;
            MessageID = messageID;
        }

        /// <summary>
        /// Constructs handler with both <see cref="GroupID"/> and <see cref="MessageID"/> manually defined.
        /// </summary>
        /// <remarks>
        /// This constructor should NOT be used by mods - mods should inherit <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> instead.
        /// But as a game developer (not mod developer) you are free to use it. It's a lot easier to use it for prototyping.
        /// </remarks>
        /// <param name="groupID"></param>
        /// <param name="messageID"></param>
        public AdvancedMessageAttribute(uint messageID, byte groupID)
        {
            GroupID = groupID;
            MessageID = messageID;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                1 Multi-type
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AdvancedMessageAttribute(Type multicast, [CallerFilePath] string source = "", [CallerLineNumber] int line = -1)
        {
            //if (typeof(NetworkGroup).IsAssignableFrom(multicast))
            //{
            //    if (!ResolveGroup(multicast))
            //    {
            //        throw new Exception($"Provided multicast type is not {nameof(NetworkMessage)}<> nor {nameof(NetworkGroup)}<> class type. Caused by Advanced message handler with multicast type ({multicast.Name}) at: ({source}) line: (#{line})");
            //    }
            //}

            if (ResolveMessage(multicast, ref Message))
            {
                // Two checks allows multicast type to have both MessageID and GroupID fields/parameters.
                if (ResolveGroup(multicast, ref Group))
                {
                    // Multicast contains has MessageID and GroupID fields/parameters.
                }
                else
                {
                    // Multicast is a NetworkMessage with MessageID only, but no GroupID.
                    GroupID = 0;
                }
            }
            else if (ResolveGroup(multicast, ref Group))
            {
                // Both MessageID and GroupID can be defined from one class.
            }
            else throw new Exception($"Provided multicast type is not {nameof(NetworkMessage)}<> nor {nameof(NetworkGroup)}<> class type. Caused by Advanced message handler with multicast type ({multicast.Name}) at: ({source}) line: (#{line})");
        }

        /// <remarks>
        /// Write ID as '0u' instead of '0' to use constructor for MessageID instead. Examples:
        /// <para>- <c><![CDATA[AdvancedMessage(typeof(Message), 0)]]></c> - here '0' marks GroupID.</para>
        /// <para>- <c><![CDATA[AdvancedMessage(typeof(Group), 0u)]]></c> - here '0u' marks MessageID.</para>
        /// </remarks>
        public AdvancedMessageAttribute(Type message, byte groupID, [CallerFilePath] string source = "", [CallerLineNumber] int line = -1)
        {
            GroupID = groupID;
            if (!ResolveMessage(message, ref Message))
            {
                PropertyInfo _ = null;
                if (ResolveGroup(message, ref _)) throw new Exception($"GroupID was provided twice in an Advanced message handler with multicast type ({message.Name}) at: ({source}) line: (#{line})");
                else throw new Exception($"Expected {nameof(NetworkMessage)}<> class type to be provided in Advanced message handler with multicast type ({message.Name}) at: ({source}) line: (#{line})");
            }
        }

        /// <remarks>
        /// Write ID as '0' instead of '0u' to use constructor for GroupID instead. Examples:
        /// <para>- <c><![CDATA[AdvancedMessage(typeof(Message), 0)]]></c> - here '0' marks GroupID.</para>
        /// <para>- <c><![CDATA[AdvancedMessage(typeof(Group), 0u)]]></c> - here '0u' marks MessageID.</para>
        /// </remarks>
        public AdvancedMessageAttribute(uint messageID, Type group, [CallerFilePath] string source = "", [CallerLineNumber] int line = -1)
        {
            MessageID = messageID;
            if (!ResolveGroup(group, ref Group))
            {
                PropertyInfo _ = null;
                if (ResolveMessage(group, ref _)) throw new Exception($"MessageID was provided twice in an Advanced message handler with multicast type ({group.Name}) at: ({source}) line: (#{line})");
                else throw new Exception($"Expected {nameof(NetworkGroup)}<> class type to be provided in Advanced message handler with multicast type ({group.Name}) at: ({source}) line: (#{line})");
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                2 Multi-types
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <remarks>
        /// Recommended to be used on methods which use <see cref="Riptide.Message"/> in method parameters.
        /// Allows specifying both <see cref="NetworkMessage"/> and <see cref="NetworkGroup"/> as types.
        /// <para>
        /// It will check if <paramref name="multicast1"/> is <see cref="NetworkMessage"/> before checking if it is <see cref="NetworkGroup"/>.
        /// If you prefer having <paramref name="multicast1"/> as <see cref="NetworkGroup"/> instead, and <paramref name="multicast2"/>
        /// as <see cref="NetworkMessage"/> - set <see cref="Reflections.AdvancedMessageHandlerConstructor_ExpectFirstMulticastToBeAMessage"/> to 'false'.
        /// Changing this might improve performance a bit, but you need to be consistent with how you use message handlers.
        /// </para>
        /// </remarks>
        /// <param name="multicast1"><see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> or <see cref="NetworkGroup{TGroup}"/>.</param>
        /// <param name="multicast2"><see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> or <see cref="NetworkGroup{TGroup}"/>.</param>
        public AdvancedMessageAttribute(Type multicast1, Type multicast2, [CallerFilePath] string source = "", [CallerLineNumber] int line = -1)
        {
            throw new NotImplementedException("WIP");
            //if (Reflections.AdvancedMessageHandlerConstructor_ExpectFirstMulticastToBeAMessage)
            //{
            //    if (ResolveMessage(multicast1))
            //    {
            //        if (ResolveGroup(multicast2))
            //        {
            //            // Prediction succeeded.
            //        }
            //        else
            //        {
            //            throw new Exception($"Since Advanced message handler got a {nameof(NetworkMessage)} as parameter '{nameof(multicast1)}' - expected {nameof(multicast2)} to be a {nameof(NetworkGroup)}, but it wasn't. Caused by Advanced message handler with multicast types ({multicast1.Name} and {multicast2.Name}) at: ({source}) line: (#{line})");
            //        }
            //    }
            //    else if (ResolveGroup(multicast1))
            //    {
            //        if (ResolveMessage(multicast2))
            //        {

            //        }
            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{

            //}

            //throw new Exception($"One or all provided multicast types is not {nameof(NetworkMessage)}<> nor {nameof(NetworkGroup)}<> class type. Caused by Advanced message handler with multicast types ({multicast1.Name} and {multicast2.Name}) at: ({source}) line: (#{line})");



            //if (!ResolveMessage(multicast1))
            //{
            //    if (!ResolveMessage(multicast2))
            //    {

            //    }
            //}

            //if (!ResolveMessage(multicast1) & !ResolveGroup(multicast1))
            //{

            //}

            //if (Message is null && ResolveMessage(multicast2))
            //{
            //    // Multicast #1 is Group and Multicast #2 is Message here.
            //}
            //else if (Group is null && ResolveGroup(multicast2))
            //{

            //}

            //if (ResolveGroup(multicast1))
            //{
            //    if (ResolveMessage(multicast2))
            //    {
            //        // Prediction succeeded.
            //        // No default values have to be provided.
            //    }
            //    else
            //    {
            //        if (ResolveGroup(multicast2))
            //            throw new Exception($"Provided two {nameof(NetworkGroup)}<> types for Advanced message handler with multicast types ({multicast1.Name} and {multicast2.Name}) at: ({source}) line: (#{line})");
            //        else throw new Exception($"{nameof(NetworkMessage)}<> was not provided for Advanced message handler with multicast types ({multicast1.Name} and {multicast2.Name}) at: ({source}) line: (#{line})");
            //    }
            //}
            //else if (ResolveMessage(multicast1))
            //{
            //    if (ResolveGroup(multicast2))
            //    {
            //        // Second prediction succeeded.
            //        // No default values have to be provided.
            //    }
            //    else
            //    {
            //        if (ResolveMessage(multicast2))
            //            throw new Exception($"Provided two {nameof(NetworkMessage)}<> types for Advanced message handler with multicast types ({multicast1.Name} and {multicast2.Name}) at: ({source}) line: (#{line})");
            //        else throw new Exception($"{nameof(NetworkGroup)}<> was not provided for Advanced message handler with multicast types ({multicast1.Name} and {multicast2.Name}) at: ({source}) line: (#{line})");
            //    }
            //}

        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static AdvancedMessageAttribute()
        {
            LastGroupIDSource = typeof(DefaultGroup).GetProperty(nameof(DefaultGroup.GroupID), MemberBindingFlags | BindingFlags.FlattenHierarchy);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private const BindingFlags MemberBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static PropertyInfo LastGroupIDSource;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private bool ResolveGroup(Type multicast, ref PropertyInfo groupID)
        {
            if (!(groupID is null)) return false;

            // Starts from a fast cache lookup to avoid reflections if possible.
            // TODO: Replace with dictionaries if it makes sense to do so. Don't forget to clear-out said array on global reload.
            if (LastGroupIDSource.DeclaringType.IsAssignableFrom(multicast))
            {
                groupID = LastGroupIDSource;
                return true;
            }
            else if (LookupMemberRootLast<GroupIDAttribute>(multicast, out PropertyInfo member))
            {
                LastGroupIDSource = groupID = member;
                return true;
            }

            return false;
        }

        private bool ResolveMessage(Type multicast, ref PropertyInfo messageID)
        {
            if (!(messageID is null)) return false;

            if (LookupMemberRootLast<MessageIDAttribute>(multicast, out PropertyInfo member))
            {
                messageID = member;
                return true;
            }

            return false;
        }

        /// <inheritdoc cref="LookupMemberRootLast(Type, Type, out PropertyInfo, bool)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool LookupMemberRootLast<T>(Type target, out PropertyInfo member) where T : Attribute
        {
            return LookupMemberRootLast(typeof(T), target, out member);
        }

        /// <remarks>
        /// Checks <paramref name="target"/> type last,
        /// as sometimes we can be fairly certain that it won't contain our <paramref name="attribute"/>.
        /// </remarks>
        internal static bool LookupMemberRootLast(Type attribute, Type target, out PropertyInfo member)
        {
            Type temp = target.BaseType;
            while (temp != null)
            {
                foreach (var field in temp.GetProperties(MemberBindingFlags))
                {
                    if (field.IsDefined(attribute))
                    {
                        member = field;
                        return true;
                    }
                }

                temp = temp.BaseType;
            }

            foreach (var field in target.GetProperties(MemberBindingFlags))
            {
                if (field.IsDefined(attribute))
                {
                    member = field;
                    return true;
                }
            }

            member = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool LookupMember<T>(Type target, out PropertyInfo member) where T : Attribute
        {
            return LookupMember(typeof(T), target, out member);
        }

        internal static bool LookupMember(Type attribute, Type target, out PropertyInfo member)
        {
            Type temp = target;
            do
            {
                foreach (var field in temp.GetProperties(MemberBindingFlags))
                {
                    if (field.IsDefined(attribute))
                    {
                        member = field;
                        return true;
                    }
                }

                temp = temp.BaseType;
            }
            while (!(temp is null));

            member = null;
            return false;
        }
    }
}
