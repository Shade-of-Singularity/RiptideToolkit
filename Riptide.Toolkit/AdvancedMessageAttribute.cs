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
    /// TODO: Use MemberInfo instead of Type, or box FieldInfo/PropertyInfo instead.
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
            ResolveMultitypePrioritizeMessage(multicast);
        }

        public AdvancedMessageAttribute(Type multicast, byte groupID)
        {
            GroupID = groupID;
            ResolveMultitypePrioritizeMessage(multicast);
        }

        public AdvancedMessageAttribute(Type multicast, ushort messageID)
        {
            MessageID = messageID;
            ResolveMultitypePrioritizeGroup(multicast);
        }

        public AdvancedMessageAttribute(Type multicast, byte groupID, ushort messageID)
        {
            GroupID = groupID;
            MessageID = messageID;
            ResolveMultitypePrioritizeMod(multicast);
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
        }

        public AdvancedMessageAttribute(Type multicast1, Type multicast2, byte groupID)
        {
            GroupID = groupID;
            ResolveMultitypePrioritizeMod(multicast1);
            ResolveMultitypePrioritizeMessage(multicast2);
        }

        public AdvancedMessageAttribute(Type multicast1, Type multicast2, ushort messageID)
        {
            MessageID = messageID;
            ResolveMultitypePrioritizeMod(multicast1);
            ResolveMultitypePrioritizeGroup(multicast2);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                3 Multi-types
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AdvancedMessageAttribute(Type multicast1, Type multicast2, Type multicast3)
        {
            ResolveMultitypePrioritizeMod(multicast1);
            ResolveMultitypePrioritizeGroup(multicast2);
            ResolveMultitypePrioritizeMessage(multicast3);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private const BindingFlags MemberBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static Type LastModType = typeof(DefaultGroup); // Not a mistake. Uses random type to not use null, and avoid 1 branch.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void ResolveMultitypePrioritizeMod(Type value)
        {
            if (!ResolveMod(value) && !ResolveGroup(value) && !ResolveMessage(value))
            {
                throw GetResolvingFailedException(value);
            }
        }

        private void ResolveMultitypePrioritizeGroup(Type value)
        {
            if (!ResolveGroup(value) && !ResolveMod(value) && !ResolveMessage(value))
            {
                throw GetResolvingFailedException(value);
            }
        }

        private void ResolveMultitypePrioritizeMessage(Type value)
        {
            if (!ResolveMessage(value) && !ResolveGroup(value) && !ResolveMod(value))
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

        private bool ResolveMod(Type mod)
        {
            if (Mod is null)
            {
                // Starts from a fast cache lookup to avoid reflections.
                if (LastModType.IsAssignableFrom(mod))
                {
                    Mod = mod;
                }
                else if (LookupAttribute<ModIDAttribute>(mod, out mod, Reflections.ModAttributeAnalysis_PrioritizeFields))
                {
                    LastModType = Mod = mod;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ResolveGroup(Type group)
        {
            if (GroupID is null && Group is null && typeof(NetworkGroup).IsAssignableFrom(group))
            {
                Group = group;
                return true;
            }

            return false;
        }

        private bool ResolveMessage(Type message)
        {
            if (MessageID is null && Message is null && typeof(NetworkMessage).IsAssignableFrom(message))
            {
                Message = message;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LookupAttribute<T>(Type target, out Type result, bool fieldsFirst) where T : Attribute
        {
            return LookupAttribute(typeof(T), target, out result, fieldsFirst);
        }

        private static bool LookupAttribute(Type attribute, Type target, out Type result, bool fieldsFirst)
        {
            if (fieldsFirst)
            {
                if (CheckFields(out result)) return true;
                if (CheckProperties(out result)) return true;
            }
            else
            {
                if (CheckProperties(out result)) return true;
                if (CheckFields(out result)) return true;
            }

            return false;

            // Simplifications:
            bool CheckFields(out Type ret)
            {
                ret = target.BaseType;
                while (ret != null)
                {
                    foreach (var field in ret.GetFields(MemberBindingFlags))
                    {
                        if (field.IsDefined(attribute)) return true;
                    }

                    ret = ret.BaseType;
                }

                foreach (var field in (ret = target).GetFields(MemberBindingFlags))
                {
                    if (field.IsDefined(attribute)) return true;
                }

                return false;
            }

            bool CheckProperties(out Type ret)
            {
                ret = target.BaseType;
                while (ret != null)
                {
                    foreach (var field in ret.GetFields(MemberBindingFlags))
                    {
                        if (field.IsDefined(attribute)) return true;
                    }

                    ret = ret.BaseType;
                }

                foreach (var field in (ret = target).GetFields(MemberBindingFlags))
                {
                    if (field.IsDefined(attribute)) return true;
                }

                return false;
            }
        }
    }
}
