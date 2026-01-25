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
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Reference to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> with <see cref="GroupIDAttribute"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="GroupIDAttribute"/> is already defined in <see cref="NetworkGroup{TGroup}"/>
        /// and also <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> types.
        /// </remarks>
        public MemberInfo Group { get; set; }
        /// <summary>
        /// Reference to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> with <see cref="MessageIDAttribute"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="ModIDAttribute"/> is already declared in all <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> types.
        /// </remarks>
        public MemberInfo Message { get; set; }
        /// <summary>
        /// Static GroupID of this message handler. Automatic ID assignment will avoid static IDs.
        /// </summary>
        public byte? GroupID { get; set; }
        /// <summary>
        /// Static MessageID of this message handler. Automatic ID assignment will avoid static IDs.
        /// </summary>
        public uint? MessageID { get; set; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Constructs handler, <see cref="MessageID"/> and <see cref="GroupID"/> for which
        /// is evaluated from parameters of a method to which this attribute is attached to.
        /// </summary>
        public AdvancedMessageAttribute() { }

        /// <inheritdoc cref=" AdvancedMessageAttribute(uint)"/>
        public AdvancedMessageAttribute(Enum messageID) : this((uint)(object)messageID) { }

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

        /// <inheritdoc cref=" AdvancedMessageAttribute(byte, uint)"/>
        public AdvancedMessageAttribute(byte groupID, Enum messageID) : this(groupID, (uint)(object)messageID) { }

        /// <summary>
        /// Constructs handler with both <see cref="GroupID"/> and <see cref="MessageID"/> manually defined.
        /// </summary>
        /// <remarks>
        /// This constructor should NOT be used by mods - mods should inherit <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> instead.
        /// But as a game developer (not mod developer) you are free to use it. It's a lot easier to use it for prototyping.
        /// </remarks>
        /// <param name="groupID"></param>
        /// <param name="messageID"></param>
        public AdvancedMessageAttribute(byte groupID, uint messageID)
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
            ResolveMultitypePrioritizeMessage(multicast);
            SetDefaults();
        }

        /// <remarks>
        /// Write ID as '0u' instead of '0' to use constructor for MessageID instead. Examples:
        /// <para>- <c><![CDATA[AdvancedMessage(typeof(Message), 0)]]></c> - here '0' marks GroupID.</para>
        /// <para>- <c><![CDATA[AdvancedMessage(typeof(Group), 0u)]]></c> - here '0u' marks MessageID.</para>
        /// </remarks>
        public AdvancedMessageAttribute(Type message, byte groupID)
        {
            GroupID = groupID;
            ResolveMultitypePrioritizeMessage(message);
            SetDefaults();
        }

        /// <remarks>
        /// Write ID as '0' instead of '0u' to use constructor for GroupID instead. Examples:
        /// <para>- <c><![CDATA[AdvancedMessage(typeof(Message), 0)]]></c> - here '0' marks GroupID.</para>
        /// <para>- <c><![CDATA[AdvancedMessage(typeof(Group), 0u)]]></c> - here '0u' marks MessageID.</para>
        /// </remarks>
        public AdvancedMessageAttribute(Type group, uint messageID)
        {
            MessageID = messageID;
            ResolveMultitypePrioritizeGroup(group);
            SetDefaults();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                2 Multi-types
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AdvancedMessageAttribute(Type multicast1, Type multicast2)
        {
            ResolveMultitypePrioritizeGroup(multicast1);
            ResolveMultitypePrioritizeMessage(multicast2);
            SetDefaults();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static AdvancedMessageAttribute()
        {
            if (LookupMemberRootLast(typeof(GroupIDAttribute), typeof(DefaultGroup), out MemberInfo member,
                Reflections.GroupAttributeAnalysis_PrioritizeFields))
            {
                DefaultGroupIDSource = member;
                LastGroupIDSource = member;
            }
            else
            {
                throw new Exception($"[{nameof(AdvancedMessageAttribute)}] Cannot retrieve {nameof(GroupIDAttribute)} from a {nameof(DefaultGroup)}.");
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private const BindingFlags MemberBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly MemberInfo DefaultGroupIDSource;
        private static MemberInfo LastGroupIDSource; // Not a mistake. Avoids null-check to remove 1 branch.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void SetDefaults()
        {
            if (GroupID is null && Group is null)
            {
                if (Message is null)
                {
                    GroupID = 0;
                }
                else if (LookupMember<GroupIDAttribute>(Message.DeclaringType, out MemberInfo member,
                        Reflections.ImbeddedAttributeAnalysis_PrioritizeFields))
                {
                    // Avoids defining GroupID to prioritize member's GroupID in field/parameter.
                    Group = member;
                }
                else
                {
                    Group = DefaultGroupIDSource;
                    GroupID = 0;
                }
            }
        }

        private void ResolveMultitypePrioritizeGroup(Type value)
        {
            if (!ResolveGroup(value) && !ResolveMessage(value))
            {
                throw GetResolvingFailedException(value);
            }
        }

        private void ResolveMultitypePrioritizeMessage(Type value)
        {
            if (!ResolveMessage(value) && !ResolveGroup(value))
            {
                throw GetResolvingFailedException(value);
            }
        }

        private void SetGroupID(byte groupID)
        {
            if (Group is null) GroupID = groupID;
            else throw new ArgumentException($"Specified {nameof(NetworkGroup)} in two different ways - explicitly and via auto-type. This is not allowed!");
        }

        private void SetMessageID(ushort messageID)
        {
            if (Message is null) MessageID = messageID;
            else throw new ArgumentException($"Specified {nameof(NetworkMessage)} in two different ways - explicitly and via auto-type. This is not allowed!");
        }

        private Exception GetResolvingFailedException(Type value)
        {
            // TODO: If all resolving attempts fail - run another analysis, more expensive one, but the one which gather debug info and log it.
            return new NotSupportedException($"Cannot resolve multitype ({value.Name}) in {nameof(AdvancedMessageAttribute)}! Either invalid types are used, or IDs provided in multiple different ways.");
        }

        private bool ResolveGroup(Type group)
        {
            if (GroupID.HasValue) return false;
            if (Group is null || Group == DefaultGroupIDSource)
            {
                // Starts from a fast cache lookup to avoid reflections.
                if (LastGroupIDSource.DeclaringType.IsAssignableFrom(group))
                {
                    Group = LastGroupIDSource;
                }
                else if (LookupMemberRootLast<GroupIDAttribute>(group, out MemberInfo member, Reflections.GroupAttributeAnalysis_PrioritizeFields))
                {
                    LastGroupIDSource = Group = member;
                }

                return true;
            }

            return false;
        }

        private bool ResolveMessage(Type message)
        {
            if (MessageID.HasValue) return false;
            if (Message is null)
            {
                if (LookupMemberRootLast<MessageIDAttribute>(message, out MemberInfo member, Reflections.MessageAttributeAnalysis_PrioritizeFields))
                {
                    Message = member;
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc cref="LookupMemberRootLast(Type, Type, out MemberInfo, bool)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool LookupMemberRootLast<T>(Type target, out MemberInfo member, bool fieldsFirst) where T : Attribute
        {
            return LookupMemberRootLast(typeof(T), target, out member, fieldsFirst);
        }

        /// <remarks>
        /// Checks <paramref name="target"/> type last,
        /// as sometimes we can be fairly certain that it won't contain our <paramref name="attribute"/>.
        /// </remarks>
        internal static bool LookupMemberRootLast(Type attribute, Type target, out MemberInfo member, bool fieldsFirst)
        {
            if (fieldsFirst)
            {
                if (CheckFields(out member)) return true;
                if (CheckProperties(out member)) return true;
            }
            else
            {
                if (CheckProperties(out member)) return true;
                if (CheckFields(out member)) return true;
            }

            return false;

            // Simplifications:
            bool CheckFields(out MemberInfo ret)
            {
                Type temp = target.BaseType;
                while (temp != null)
                {
                    foreach (var field in temp.GetFields(MemberBindingFlags))
                    {
                        if (field.IsDefined(attribute))
                        {
                            ret = field;
                            return true;
                        }
                    }

                    temp = temp.BaseType;
                }

                foreach (var field in target.GetFields(MemberBindingFlags))
                {
                    if (field.IsDefined(attribute))
                    {
                        ret = field;
                        return true;
                    }
                }

                ret = null;
                return false;
            }

            bool CheckProperties(out MemberInfo ret)
            {
                Type temp = target.BaseType;
                while (temp != null)
                {
                    foreach (var field in temp.GetProperties(MemberBindingFlags))
                    {
                        if (field.IsDefined(attribute))
                        {
                            ret = field;
                            return true;
                        }
                    }

                    temp = temp.BaseType;
                }

                foreach (var field in target.GetProperties(MemberBindingFlags))
                {
                    if (field.IsDefined(attribute))
                    {
                        ret = field;
                        return true;
                    }
                }

                ret = null;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool LookupMember<T>(Type target, out MemberInfo member, bool fieldsFirst) where T : Attribute
        {
            return LookupMemberRootLast(typeof(T), target, out member, fieldsFirst);
        }

        internal static bool LookupMember(Type attribute, Type target, out MemberInfo member, bool fieldsFirst)
        {
            if (fieldsFirst)
            {
                if (CheckFields(out member)) return true;
                if (CheckProperties(out member)) return true;
            }
            else
            {
                if (CheckProperties(out member)) return true;
                if (CheckFields(out member)) return true;
            }

            return false;

            // Simplifications:
            bool CheckFields(out MemberInfo ret)
            {
                Type temp = target;
                do
                {
                    foreach (var field in temp.GetFields(MemberBindingFlags))
                    {
                        if (field.IsDefined(attribute))
                        {
                            ret = field;
                            return true;
                        }
                    }

                    temp = temp.BaseType;
                }
                while (!(temp is null));

                ret = null;
                return false;
            }

            bool CheckProperties(out MemberInfo ret)
            {
                Type temp = target;
                do
                {
                    foreach (var field in temp.GetProperties(MemberBindingFlags))
                    {
                        if (field.IsDefined(attribute))
                        {
                            ret = field;
                            return true;
                        }
                    }

                    temp = temp.BaseType;
                }
                while (!(temp is null));

                ret = null;
                return false;
            }
        }
    }
}
