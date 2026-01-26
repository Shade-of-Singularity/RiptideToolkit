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

using Riptide.Toolkit.Handlers;
using Riptide.Toolkit.Messages;
using Riptide.Toolkit.Settings;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Helps with message indexing.
    /// </summary>
    /// TODO: Add ways to register new handlers for new loaded-in assemblies, automatically or manually, without full networking reload.
    /// TODO: Add a way to reset the entire system (includes headers and internal pools) for proper runtime reloading.
    public static class NetworkIndex
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Prefix for all logs sent from this class.
        /// </summary>
        public const string LogPrefix = "[" + nameof(NetworkIndex) + "]";
        /// <summary>
        /// How many groups is there.
        /// </summary>
        public const byte MaxGroupIDAmount = 255;
        /// <summary>
        /// How many messages one group can hold.
        /// </summary>
        public const uint MaxMessageIDAmount = uint.MaxValue;
        /// <summary>
        /// GroupID which indicates that ID was not assigned yet.
        /// <see cref="byte.MaxValue"/> is an invalid ID, since '0' is commonly used everywhere by <see cref="Toolkit"/>.
        /// </summary>
        public const byte InvalidGroupID = byte.MaxValue;
        /// <summary>
        /// MessageID which indicates that ID was not assigned yet.
        /// <see cref="uint.MaxValue"/> is an invalid ID, since '0' is commonly used everywhere by developers.
        /// </summary>
        public const uint InvalidMessageID = uint.MaxValue;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal delegate void ClientSystemHandler(AdvancedClient sender, AdvancedMessageReceivedEventArgs args);
        internal delegate void ServerSystemHandler(AdvancedServer sender, AdvancedMessageReceivedEventArgs args);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether <see cref="NetworkIndex"/> was ever initialized.
        /// </summary>
        /// <remarks>
        /// You will not be able to change settings in <see cref="Performance"/> category if this property is true.
        /// </remarks>
        public static bool IsEverInitialized => m_IsInitialized;

        /// <summary>
        /// Whether message handlers was loaded-in successfully.
        /// </summary>
        public static bool IsValid => m_IsValid;

        /// <summary>
        /// Contains references to raw <see cref="MessageHandlerInfo"/> structs for clients.
        /// </summary>
        public static IReadOnlyMessageHandlerCollection<MessageHandlerInfo> RawClientHandlers => m_RawClientHandlers;

        /// <summary>
        /// Contains references to raw <see cref="MessageHandlerInfo"/> structs for clients.
        /// </summary>
        public static IReadOnlyMessageHandlerCollection<MessageHandlerInfo> RawServerHandlers => m_RawServerHandlers;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // System message handlers. Does not rely on GroupID - maps directly to MessageID.
        private static readonly ClientSystemHandler[] m_ClientSystemHandlers = new ClientSystemHandler[SystemMessaging.TotalIDs];
        private static readonly ServerSystemHandler[] m_ServerSystemHandlers = new ServerSystemHandler[SystemMessaging.TotalIDs];

        // GroupIDs:
        private static readonly object _groupLock = new object();
        private static readonly GroupMessageIndexer[] m_ClientGroups = new GroupMessageIndexer[MaxGroupIDAmount];
        private static readonly GroupMessageIndexer[] m_ServerGroups = new GroupMessageIndexer[MaxGroupIDAmount];
        private static ushort m_GroupIDHeadIndex; // We use ushort instead of byte, to gracefully handle ID exhaustion.

        // MessageIDs: (doesn't map to Groups, rather Groups map to it instead)
        /// Change handler type if <see cref="Performance.MessageHandlerFocus"/> and recommended type changes.
        private static readonly MessageHandlerCollection<MessageHandlerInfo> m_RawClientHandlers = MessageHandlerCollection<MessageHandlerInfo>.Create();
        private static readonly MessageHandlerCollection<MessageHandlerInfo> m_RawServerHandlers = MessageHandlerCollection<MessageHandlerInfo>.Create();

        // Whether Networking systems were ever initialized.
        private static volatile bool m_IsInitialized = false;

        // Whether all handlers are valid. If false - will rescan all assemblies to find all handlers.
        private static volatile bool m_IsValid = false;

        // Lock will be locked on rescan.
        private static readonly object _lock = new object();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static NetworkIndex()
        {
            // Initializes it so clients and servers can initialize easily.
            for (byte i = 0; i < MaxGroupIDAmount; i++)
            {
                m_ClientGroups[i] = GroupMessageIndexer.Create(i);
                m_ServerGroups[i] = GroupMessageIndexer.Create(i);
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Invalidates initialization of the network handlers, forcing game to reload all message handlers before the next network call.
        /// All message IDs should stay the same, as long as client and server had same Assemblies loaded in the same order upon reloading.
        /// You should run <see cref="Initialize"/> immediate after <see cref="Invalidate"/> to be certain when initialization happens.
        /// This is important, because re-initialization happens ONLY when someone uses <see cref="ClientHandlers"/>
        /// or <see cref="ServerHandlers"/>.
        /// </summary>
        /// <remarks>
        /// Using this method outside of initialization sequence is dangerous.
        /// In <see cref="Riptide"/>, it might be used only once after <see cref="Engine"/> loads-in mod assemblies.
        /// </remarks>
        [Obsolete("Not obsolete, but will throw if used. Method will function properly in one of the upcoming updates.", error: true)]
        public static void Invalidate()
        {
            throw new NotImplementedException("Invalidation is temporary not supported after renovation.");
            //m_IsValid = false;
        }

        public static void Initialize()
        {
            if (!m_IsValid) lock (_lock) UpdateHandlers();
        }

        // TODO: Add Assembly loading and unloading (rescan for all classes containing MessageID and reset them to InvalidMessageI)

        /// <summary>
        /// Activates all static fields in given class/type.
        /// Can be used to Forcefully register GroupIDs from <see cref="NetworkGroup{TGroup}"/>s.
        /// </summary>
        /// <typeparam name="T">Type to initialize.</typeparam>
        public static void Register<T>() => System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);

        /// <summary>
        /// Activates all static fields in given class/type.
        /// Can be used to Forcefully register GroupIDs from <see cref="NetworkGroup{TGroup}"/>s.
        /// </summary>
        public static void Register(Type type) => System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Client Handlers
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Checks if group under given <paramref name="groupID"/> (for client-side messages) is defined.
        /// </summary>
        /// <param name="groupID">GroupID to check.</param>
        /// <returns>Whether group was registered.</returns>
        public static bool HasClientGroup(byte groupID)
        {
            lock (_groupLock) return !(m_ClientGroups[groupID] is null);
        }

        /// <summary>
        /// Attempts to retrieve <see cref="IReadOnlyGroupMessageIndexer"/> under given <paramref name="groupID"/> (for client-side messages).
        /// </summary>
        /// <param name="groupID">GroupID to use.</param>
        /// <param name="group">Indexer under <paramref name="groupID"/>, or <c>null</c>.</param>
        /// <returns>Whether <paramref name="group"/> under <paramref name="groupID"/> was found.</returns>
        public static bool TryGetClientGroup(byte groupID, out IReadOnlyGroupMessageIndexer group)
        {
            lock (_groupLock)
            {
                group = m_ClientGroups[groupID];
                return !(group is null);
            }
        }

        /// <summary>
        /// Retrieves <see cref="IReadOnlyGroupMessageIndexer"/> under given <paramref name="groupID"/> (for client-side messages).
        /// </summary>
        /// <param name="groupID">GroupID to check.</param>
        /// <returns><see cref="IReadOnlyGroupMessageIndexer"/> or <c>null</c> if <paramref name="groupID"/> is not defined.</returns>
        public static IReadOnlyGroupMessageIndexer GetClientGroup(byte groupID)
        {
            lock (_groupLock)
            {
                var group = m_ClientGroups[groupID];
                if (group is null) return m_ClientGroups[groupID] = GroupMessageIndexer.Create(groupID);
                else return group;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Server Handlers
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Checks if group under given <paramref name="groupID"/> (for server-side messages) is defined.
        /// </summary>
        /// <param name="groupID">GroupID to check.</param>
        /// <returns>Whether group was registered.</returns>
        public static bool HasServerGroup(byte groupID)
        {
            lock (_groupLock) return !(m_ServerGroups[groupID] is null);
        }

        /// <summary>
        /// Attempts to retrieve <see cref="IReadOnlyGroupMessageIndexer"/> under given <paramref name="groupID"/> (for server-side messages).
        /// </summary>
        /// <param name="groupID">GroupID to use.</param>
        /// <param name="group">Indexer under <paramref name="groupID"/>, or <c>null</c>.</param>
        /// <returns>Whether <paramref name="group"/> under <paramref name="groupID"/> was found.</returns>
        public static bool TryGetServerGroup(byte groupID, out IReadOnlyGroupMessageIndexer group)
        {
            lock (_groupLock)
            {
                group = m_ServerGroups[groupID];
                return !(group is null);
            }
        }

        /// <summary>
        /// Retrieves <see cref="IReadOnlyGroupMessageIndexer"/> under given <paramref name="groupID"/> (for server-side messages).
        /// </summary>
        /// <param name="groupID">GroupID to check.</param>
        /// <returns><see cref="IReadOnlyGroupMessageIndexer"/> or <c>null</c> if <paramref name="groupID"/> is not defined.</returns>
        public static IReadOnlyGroupMessageIndexer GetServerGroup(byte groupID)
        {
            lock (_groupLock)
            {
                var group = m_ServerGroups[groupID];
                if (group is null) return m_ServerGroups[groupID] = GroupMessageIndexer.Create(groupID);
                else return group;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // TODO: Make some internal methods public, especially for those system handlers. Sometimes you want to overwrite them.
        //  It also implies that we need to make public ALL code which previously internal methods touch.
        internal static void Register(byte systemID, ClientSystemHandler handler) => m_ClientSystemHandlers[systemID] = handler;
        internal static void Register(byte systemID, ServerSystemHandler handler) => m_ServerSystemHandlers[systemID] = handler;
        internal static void HandleClient(byte systemID, AdvancedClient client, AdvancedMessageReceivedEventArgs args)
        {
            m_ClientSystemHandlers[systemID](client, args);
        }

        internal static void HandleServer(byte systemID, AdvancedServer server, AdvancedMessageReceivedEventArgs args)
        {
            m_ServerSystemHandlers[systemID](server, args);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void InternalInitializeService()
        {
            // Mandatory second check - needed when _lock unlocks.
            if (m_IsInitialized) return;
            m_IsInitialized = true;
            // Doesn't initialize anything, but might do it in the future.
        }

        /// <summary>
        /// Retrieves new GroupID to use, and don't allow others to use it.
        /// </summary>
        /// <returns>New GroupID.</returns>
        /// <exception cref="IndexOutOfRangeException">Provided GroupID was not taken and thus - invalid.</exception>
        /// <exception cref="Exception">Exhausted all network message IDs for Riptide networking.</exception>
        /// <summary>
        /// Retrieves next group ID for networking with <see cref="Toolkit"/>.
        /// </summary>
        /// TODO: Make sure that <see cref="DefaultGroup"/> is initialized first, to take 0th group ID.
        /// However, do not force it - only initialize it first if any message handlers in main assembly define it.
        /// This is to make it possible for people to define their own default groups. No matter how confusing it is.
        private static byte NextGroupID()
        {
            ushort head = m_GroupIDHeadIndex;
            if (head >= MaxGroupIDAmount)
            {
                throw new Exception("Exhausted all network Group IDs for Riptide networking.");
            }

            if (m_ClientGroups[head] is null)
            {
                m_ClientGroups[head] = GroupMessageIndexer.Create((byte)head);
            }
            else
            {
                m_ClientGroups[head].Clear();
            }

            m_GroupIDHeadIndex = (ushort)(head + 1);
            return (byte)head;
        }

        private static void UpdateHandlers()
        {
            if (!m_IsInitialized)
            {
                InternalInitializeService();
                m_IsInitialized = true;
            }

            // Mandatory second check - needed when _lock unlocks.
            // TODO: Implement monitor here - I suspect there might be a race condition here in some cases.
            // We need this to allow multithreading when no changing is happening, and restrict it when we update handlers.
            // Single-threaded implementation should work though, so Unity should be covered.
            if (m_IsValid) return;

            try
            {
                // Resets all handlers first.
                // Note: All managed handlers will be locked at this point, so mutation is safe, even in multi-threaded context (untested though)
                // TODO: Avoid clearing of the entire database on first Initialization. All handlers will be empty at this point anyway.
                m_RawClientHandlers.Clear();
                m_RawServerHandlers.Clear();
                Array.ForEach(m_ClientGroups, g => g.Clear());
                Array.ForEach(m_ServerGroups, g => g.Clear());
                m_GroupIDHeadIndex = 0;

                // Starts fetching message handlers:
                List<(MethodInfo method, AdvancedMessageAttribute attribute, bool isServerSide)> handlers
                    = new List<(MethodInfo, AdvancedMessageAttribute, bool isServerSide)>();

                // Fetches own assembly first.
                Action<Assembly> fetcher = Debugging.WarnAboutRiptideMessages
                    ? (Action<Assembly>)FullHandlerFetching
                    : SimpleHandlerFetching;
                Assembly current = Assembly.GetExecutingAssembly();
                fetcher(current);

                // Seeks other assemblies which rely on this one to function.
                string extension = current.FullName;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    // Ignores current assembly, as it is initialized first anyway.
                    if (assembly == current)
                    {
                        continue;
                    }

                    foreach (var reference in assembly.GetReferencedAssemblies())
                    {
                        // Checks if this extension assembly is referenced by any other assembly.
                        if (extension.Equals(reference.FullName, StringComparison.Ordinal))
                        {
                            fetcher(assembly);
                            break;
                        }
                    }
                }

                // Registers MessageIDs first:
                int length = handlers.Count;
                for (int i = 0; i < length; i++)
                {
                    var (method, attribute, isServerSide) = handlers[i];
                    if (!attribute.MessageID.HasValue) continue;
                    ParameterInfo[] parameters = method.GetParameters();
                    Type dataType;
                    switch (parameters.Length)
                    {
                        case 0: throw new Exception($"Message handler ({method.Name}) has no method parameters.");
                        case 1: dataType = parameters[0].ParameterType; isServerSide = false; break; // Client-side method.
                        case 2: dataType = parameters[1].ParameterType; isServerSide = true; break; // Server-side method.
                        default: throw new Exception($"Message handler ({method.Name}) has too many parameters.");
                    }

                    // TODO: Add toggleable type check.
                    (isServerSide ? m_RawServerHandlers : m_RawClientHandlers)
                        .Set(attribute.MessageID.Value, new MessageHandlerInfo(method, dataType, attribute.Release));
                    handlers[i] = (method, attribute, isServerSide);
                }

                for (int i = 0; i < length; i++)
                {
                    var (method, attribute, isServerSide) = handlers[i];
                    if (attribute.MessageID.HasValue) continue;
                    ParameterInfo[] parameters = method.GetParameters();
                    Type dataType;
                    switch (parameters.Length)
                    {
                        case 0: throw new Exception($"Message handler ({method.Name}) has no method parameters.");
                        case 1: dataType = parameters[0].ParameterType; isServerSide = false; break; // Client-side method.
                        case 2: dataType = parameters[1].ParameterType; isServerSide = true; break; // Server-side method.
                        default: throw new Exception($"Message handler ({method.Name}) has too many parameters.");
                    }

                    PropertyInfo member;
                    if (attribute.Message is null)
                    {
                        // TODO: Optimize method parameter checks.
                        // At this point, maybe iterating over the entire code database and creating a few dictionaries will be faster?
                        if (!AdvancedMessageAttribute.LookupMemberRootLast(typeof(MessageIDAttribute), dataType, out member))
                        {
                            throw new Exception($"MessageID was not provided in any way for message handler ({method.Name}).");
                        }
                    }
                    else member = attribute.Message;
                    uint messageID = (uint)member.GetValue(null);
                    if (messageID == InvalidMessageID)
                    {
                        continue;
                    }

                    messageID = (isServerSide ? m_RawServerHandlers : m_RawClientHandlers)
                        .Put(new MessageHandlerInfo(method, dataType, attribute.Release));
                    member.SetValue(null, messageID);
                    attribute.MessageID = messageID; // Needed for working with GroupIDs.
                    handlers[i] = (method, attribute, isServerSide);
                }

                // Registers GroupIDs:
                for (int i = 0; i < length; i++)
                {
                    var (method, attribute, isServerSide) = handlers[i];
                    if (!attribute.GroupID.HasValue) continue;
                    byte groupID = attribute.GroupID.Value;

                    GroupMessageIndexer group;
                    if (isServerSide)
                    {
                        group = m_ServerGroups[groupID];
                        if (group is null)
                        {
                            m_ServerGroups[groupID] = group = GroupMessageIndexer.Create(groupID);
                        }
                    }
                    else
                    {
                        group = m_ClientGroups[groupID];
                        if (group is null)
                        {
                            m_ClientGroups[groupID] = group = GroupMessageIndexer.Create(groupID);
                        }
                    }

                    group.Register(attribute.MessageID.Value);
                }

                for (int i = 0; i < length; i++)
                {
                    var (method, attribute, isServerSide) = handlers[i];
                    if (attribute.GroupID.HasValue) continue;

                    byte groupID;
                    if (attribute.Group is null)
                    {
                        groupID = 0;
                    }
                    else
                    {
                        groupID = (byte)attribute.Group.GetValue(null);
                        if (groupID == InvalidGroupID)
                        {
                            groupID = NextGroupID();
                            attribute.Group.SetValue(null, groupID);
                        }
                    }

                    GroupMessageIndexer group;
                    if (isServerSide)
                    {
                        group = m_ServerGroups[groupID];
                        if (group is null)
                        {
                            m_ServerGroups[groupID] = group = GroupMessageIndexer.Create(groupID);
                        }
                    }
                    else
                    {
                        group = m_ClientGroups[groupID];
                        if (group is null)
                        {
                            m_ClientGroups[groupID] = group = GroupMessageIndexer.Create(groupID);
                        }
                    }

                    group.Register(attribute.MessageID.Value);
                }

                // Simplifications:
                void SimpleHandlerFetching(Assembly assembly)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var method in type.GetMethods(AdvancedMessageAttribute.MethodBindingFlags))
                        {
                            if (method.IsDefined(typeof(AdvancedMessageAttribute)))
                            {
                                QueueAdvancedHandlers(method);
                            }
                        }
                    }
                }

                void FullHandlerFetching(Assembly assembly)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var method in type.GetMethods(AdvancedMessageAttribute.MethodBindingFlags))
                        {
                            if (method.IsDefined(typeof(MessageHandlerAttribute)))
                            {
                                RiptideLogger.Log(LogType.Warning, $"{LogPrefix} Found method ({method.Name}) with regular {nameof(MessageHandlerAttribute)}. Those methods are ignored with {nameof(AdvancedClient)}s and {nameof(AdvancedServer)}s. Consider using {nameof(AdvancedMessageAttribute)}s instead.");
                            }

                            if (method.IsDefined(typeof(AdvancedMessageAttribute)))
                            {
                                QueueAdvancedHandlers(method);
                            }
                        }
                    }
                }

                void QueueAdvancedHandlers(MethodInfo method)
                {
                    foreach (var attribute in method.GetCustomAttributes<AdvancedMessageAttribute>())
                    {
                        handlers.Add((method, attribute, isServerSide: default));
                    }
                }
            }
            catch (Exception ex)
            {
                RiptideLogger.Log(LogType.Error, $"{LogPrefix} Could not update message handlers gracefully! Networking is likely broken at this point.");
                RiptideLogger.Log(LogType.Error, $"{ex}\n{ex.StackTrace}");
            }

            m_IsValid = true;
        }
    }
}
