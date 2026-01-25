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
        /// How many groups can be generated.
        /// </summary>
        public const ushort MaxGroupIDAmount = 1 << (sizeof(byte) - 1);

        /// <summary>
        /// How many messages one group can hold.
        /// </summary>
        public const uint MaxMessageIDAmount = uint.MaxValue;

        /// <summary>
        /// <see cref="uint.MaxValue"/> is an invalid ID, since '0' are commonly used everywhere by developers.
        /// </summary>
        public const uint InvalidMessageID = uint.MaxValue;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal delegate void ClientSystemHandler(AdvancedClient sender, MessageReceivedEventArgs args);
        internal delegate void ServerSystemHandler(AdvancedServer sender, MessageReceivedEventArgs args);




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
        /// Contains references to raw <see cref="MessageHandlerInfo"/> structs.
        /// </summary>
        public static IReadOnlyMessageHandlerCollection<MessageHandlerInfo> Handlers => m_MessageHandlers;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // System message handlers. Does not rely on GroupID - maps directly to MessageID.
        private static readonly ClientSystemHandler[] m_ClientSystemHandlers = new ClientSystemHandler[SystemMessaging.TotalIDs];
        private static readonly ServerSystemHandler[] m_ServerSystemHandlers = new ServerSystemHandler[SystemMessaging.TotalIDs];

        // GroupIDs:
        // Pre-allocates one for GroupID '0'.
        private static readonly object _groupLock = new object();
        private static readonly GroupMessageIndexer[] m_Groups = new GroupMessageIndexer[1] { GroupMessageIndexer.Create() };

        // MessageIDs: (doesn't map to Groups, rather Groups map to it instead)
        private static readonly MessageHandlerCollection<MessageHandlerInfo> m_MessageHandlers = MessageHandlerCollection<MessageHandlerInfo>.Create();

        // MessageIDs: (maps to each GroupID)
        private static ulong[][] m_MessageIDFlags = new ulong[1][] { new ulong[MaxMessageIDAmount / 64] }; // Describes all 65536 MessageIDs that can be taken.
        private static uint[] m_MessageIDHeadIndex = new uint[1]; // We use uint instead of ushort, to gracefully handle ID exhaustion.

        // Whether Networking systems were ever initialized.
        private static volatile bool m_IsInitialized = false;

        // Whether all handlers are valid. If false - will rescan all assemblies to find all handlers.
        private static volatile bool m_IsValid = false;

        // Lock will be locked on rescan.
        private static readonly object _lock = new object();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Invalidates initialization of the network handlers, forcing game to reload all message handlers before the next network call.
        /// All message IDs should stay the same, as long as client and server had same Assemblies loaded in the same order upon reloading.
        /// You should run <see cref="Initialize"/> immediate after <see cref="Invalidate"/> to be certain when initialization happens.
        /// This is important, because re-initialization happens ONLY when someone uses <see cref="Handlers.ClientHandlers"/>
        /// or <see cref="Handlers.ServerHandlers"/>.
        /// </summary>
        /// <remarks>
        /// Using this method outside of initialization sequence is dangerous.
        /// In <see cref="Riptide"/>, it might be used only once after <see cref="Engine"/> loads-in mod assemblies.
        /// </remarks>
        public static void Invalidate() => m_IsValid = false;
        public static void Initialize()
        {
            if (!m_IsValid) lock (_lock) UpdateHandlers();
        }

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

        /// <summary>
        /// Retrieves collection of all message handlers - specifically for client-side message handlers.
        /// </summary>
        /// <param name="groupID">Group ID of a collection of client-side message handlers.</param>
        /// <returns>Collection of client-side message handlers.</returns>
        public static ClientHandlers ClientHandlers(byte groupID = 0)
        {
            if (groupID >= m_ClientHandlers.Length)
            {
                throw new Exception($"Client message handler group ({groupID}) is not defined.");
            }

            return m_ClientHandlers[groupID];
        }

        /// <summary>
        /// Retrieves collection of all message handlers - specifically for server-side message handlers.
        /// </summary>
        /// <param name="groupID">Group ID of a collection of server-side message handlers.</param>
        /// <returns>Collection of server-side message handlers.</returns>
        public static ServerHandlers ServerHandlers(byte groupID = 0)
        {
            if (groupID >= m_ServerHandlers.Length)
            {
                throw new Exception($"Server message handler group ({groupID}) is not defined");
            }

            return m_ServerHandlers[groupID];
        }

        /// <summary>
        /// Retrieves new GroupID to use, and doesn't allow others use it.
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
        public static byte NextGroupID()
        {
            ushort head = m_GroupIDHeadIndex;
            while (true)
            {
                if (head >= MaxGroupIDAmount)
                {
                    throw new Exception("Exhausted all network Group IDs for Riptide networking.");
                }

                int region = head >> 6; // Divides by 64, essentially.
                ulong mask = m_GroupIDFlags[region];
                int index = head & 0b111111; // Covers first [0-63] values.
                ulong pin = 1uL << index;
                if ((mask & pin) == 0)
                {
                    m_GroupIDFlags[region] = mask | pin;
                    break;
                }

                head++;
            }

            // We resize if group was not defined yet.
            int size = head + 1;
            int totalGroups = m_MessageIDHeadIndex.Length;
            if (size >= totalGroups)
            {
                ClientHandlers[] clientHandlers = new ClientHandlers[size];
                ServerHandlers[] serverHandlers = new ServerHandlers[size];
                ulong[][] messageIDFlags = new ulong[size][];
                uint[] messageIDHeadIndexes = new uint[size];

                lock (_resizeLock)
                {
                    Array.Copy(m_ClientHandlers, clientHandlers, totalGroups);
                    Array.Copy(m_ServerHandlers, serverHandlers, totalGroups);
                    Array.Copy(m_MessageIDFlags, messageIDFlags, totalGroups);
                    Array.Copy(m_MessageIDHeadIndex, messageIDHeadIndexes, totalGroups);
                    for (int i = totalGroups; i < size; i++)
                    {
                        // We use "max >> 6" here instead of "max / 64", like in a constructor,
                        // as this part only maintainers will read, and we can optimize it a bit.
                        messageIDFlags[i] = new ulong[MaxMessageIDAmount >> 6];
                    }

                    m_ClientHandlers = clientHandlers;
                    m_ServerHandlers = serverHandlers;
                    m_MessageIDFlags = messageIDFlags;
                    m_MessageIDHeadIndex = messageIDHeadIndexes;
                }
            }

            // Read "size" as "head + 1" - we just avoid one addition here (translates to 3-4 instructions, I believe)
            m_GroupIDHeadIndex = (ushort)size;
            return (byte)head;
        }

        /// <summary>
        /// Retrieves MessageID for a given <paramref name="groupID"/>.
        /// </summary>
        /// <param name="groupID">GroupID for which you request new MessageID.</param>
        /// <returns>New MessageID.</returns>
        /// <exception cref="IndexOutOfRangeException">Provided GroupID was not taken and thus - invalid.</exception>
        /// <exception cref="Exception">Exhausted all network message IDs for Riptide networking.</exception>
        public static ushort NextMessageID(byte groupID)
        {
            lock (_resizeLock)
            {
                uint head = m_MessageIDHeadIndex[groupID];
                ulong[] flags = m_MessageIDFlags[groupID];
                while (true)
                {
                    if (head >= MaxMessageIDAmount)
                    {
                        throw new Exception("Exhausted all network Message IDs for Riptide networking.");
                    }

                    uint region = head >> 6; // Divides by 64, essentially.
                    ulong mask = flags[region];
                    int index = (int)(head & 0b111111); // Covers first [0-63] values.
                    ulong pin = 1uL << index;
                    if ((mask & pin) == 0)
                    {
                        flags[region] = mask | pin;
                        break;
                    }

                    head++;
                }

                m_MessageIDHeadIndex[groupID] = head + 1;
                return (ushort)head;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal static void Register(byte systemID, ClientSystemHandler handler) => m_ClientSystemHandlers[systemID] = handler;
        internal static void Register(byte systemID, ServerSystemHandler handler) => m_ServerSystemHandlers[systemID] = handler;
        internal static void HandleClient(byte systemID, AdvancedClient client, MessageReceivedEventArgs args)
        {
            m_ClientSystemHandlers[systemID](client, args);
        }

        internal static void HandleServer(byte systemID, AdvancedServer server, MessageReceivedEventArgs args)
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
                Array.ForEach(m_ClientHandlers, (client) => MessageHandlerCollection<ClientHandlers.HandlerInfo>.Unsafe.Clear(client.Handlers));
                Array.ForEach(m_ServerHandlers, (server) => MessageHandlerCollection<ServerHandlers.HandlerInfo>.Unsafe.Clear(server.Handlers));

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
            }
            catch (Exception ex)
            {
                RiptideLogger.Log(LogType.Error, $"{LogPrefix} Could not update message handlers gracefully! Networking is likely broken at this point.");
                RiptideLogger.Log(LogType.Error, $"{ex}\n{ex.StackTrace}");
            }

            m_IsValid = true;
        }

        private static void SimpleHandlerFetching(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var method in type.GetMethods(AdvancedMessageAttribute.MethodBindingFlags))
                {
                    if (method.IsDefined(typeof(AdvancedMessageAttribute)))
                    {
                        ProcessAdvancedHandlers(method);
                    }
                }
            }
        }

        private static void FullHandlerFetching(Assembly assembly)
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
                        ProcessAdvancedHandlers(method);
                    }
                }
            }
        }

        private static void ProcessAdvancedHandlers(MethodInfo method)
        {
            foreach (var attribute in method.GetCustomAttributes<AdvancedMessageAttribute>())
            {
                RegisterHandler(method, attribute);
            }
        }

        private static void RegisterHandler(MethodInfo method, AdvancedMessageAttribute attribute)
        {
            // TODO: Update, so it works better.
            // Note: Differentiates client-side and server-side message handlers by the parameter types they specify.
            ParameterInfo[] parameters = method.GetParameters();
            Type dataType;
            byte? groupID = attribute.GroupID is null ? (byte)(GetMemberValue(attribute.Group) ?? null) : attribute.GroupID.Value;
            ushort modID = attribute.Mod is null ? (ushort)0 : (ushort)GetMemberValue(attribute.Mod);
            ushort messageID = 0;
            switch (parameters.Length)
            {
                // Likely client-side method - they only have the NetworkMessage in parameters.
                case 1:
                    SeekIDs(method, dataType = parameters[0].ParameterType, attribute, ref groupID, ref messageID);
                    ClientHandlers.HandlerInfo clientHandler = new ClientHandlers.HandlerInfo(method, dataType);
                    MessageHandlerCollection<ClientHandlers.HandlerInfo>.Unsafe.Set(m_ClientHandlers[groupID.Value].Handlers, modID, messageID, clientHandler);
                    break;

                // Likely server-side method - they have UserID and INetworkMessage in parameters.
                case 2:
                    if (parameters[0].ParameterType != typeof(ushort))
                    {
                        throw new Exception($"Message handler ({method.Name}) is likely server-side, but doesn't have ClientID (ushort) as first parameter.");
                    }

                    // Commentary: Oh, well... Supporting random parameter type order might be hell (but fun), if we want to make it optimized, ha-ha)
                    SeekIDs(method, dataType = parameters[1].ParameterType, attribute, ref groupID, ref messageID);
                    ServerHandlers.HandlerInfo serverHandler = new ServerHandlers.HandlerInfo(method, dataType);
                    MessageHandlerCollection<ServerHandlers.HandlerInfo>.Unsafe.Set(m_ServerHandlers[groupID.Value].Handlers, modID, messageID, serverHandler);
                    break;

                // Any other kind of signature is not supported at the moment.
                default:
                case 0: throw new Exception($"Message handler ({method.Name}) doesn't have client-side nor server-side Riptide message signature.");
            }
        }

        private static void SeekIDs(MethodInfo method, Type dataType, AdvancedMessageAttribute attribute, ref byte? groupID, ref ushort messageID)
        {
            // Retrieves IDs in all possible ways.
            if (typeof(NetworkMessage).IsAssignableFrom(dataType))
            {
                if (AdvancedMessageAttribute.LookupMemberRootLast<MessageIDAttribute>(dataType, out MemberInfo member,
                    fieldsFirst: Reflections.MessageAttributeAnalysis_PrioritizeFields))
                {
                    messageID = (ushort)GetMemberValue(member);
                }
                else
                {
                    throw new Exception($"{LogPrefix} Cannot find {nameof(MessageIDAttribute)} in {nameof(NetworkMessage)} inside {method.Name} body.");
                }

                // Additional group check in case it was provided in network message itself.
                // This check will pass by default for all generic NetworkMessages, since we have this attribute,
                // however, it might be unclear as to why this succeeds, so we might remove this feature later.
                if (groupID is null && AdvancedMessageAttribute.LookupMemberRootLast<GroupIDAttribute>(dataType, out member,
                    fieldsFirst: Reflections.ImbeddedAttributeAnalysis_PrioritizeFields))
                {
                    groupID = (byte)GetMemberValue(member);
                }
                else
                {
                    groupID = 0; // Defaults to default group.
                }
            }
            else if (attribute.MessageID.HasValue)
            {
                messageID = attribute.MessageID.Value;
                if (groupID is null) groupID = 0;
            }
            else if (attribute.Message is null)
            {
                throw new Exception($"{LogPrefix} MessageID for {method.Name} was not provided by any of 3 known methods" +
                    $"({nameof(NetworkMessage)} as parameter in method params, MessageID in attribute as ushort/uint, or {nameof(NetworkMessage)} in attribute)");
            }
            else
            {
                messageID = (ushort)GetMemberValue(attribute.Message);
                if (groupID is null) groupID = 0;
            }

            
        }

        private static object GetMemberValue(MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo field: return field.GetValue(null);
                case PropertyInfo property: return property.GetValue(null);
                default: return null;
            }
        }
    }
}
