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
        /// How many mods toolkit supports.
        /// You can bypass this limit by not registering mods which doesn't use networking.
        /// (Though, I have no idea how you can genuinely exhaust all 65536 available mod IDs)
        /// </summary>
        public const uint MaxModIDAmount = ushort.MaxValue + 1;

        /// <summary>
        /// How many groups can be generated.
        /// </summary>
        public const ushort MaxGroupIDAmount = byte.MaxValue + 1;

        /// <summary>
        /// How many messages one group can hold.
        /// </summary>
        public const uint MaxMessageIDAmount = ushort.MaxValue + 1;

        /// <summary>
        /// How many handler array cells should be allocated for <see cref="SystemMessageID"/> enum.
        /// </summary>
        /// <remarks>
        /// System messages work with all groups - they are used to determine if systems are compatible or not, so kind of important.
        /// </remarks>
        internal const ushort SystemMessagesCount = 4;




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
        /// You will not be able to change settings in <see cref="Settings.Performance"/> category if this property is true.
        /// </remarks>
        public static bool IsEverInitialized => m_IsInitialized;

        /// <summary>
        /// Whether message handlers was loaded-in successfully.
        /// </summary>
        public static bool IsValid => m_IsValid;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly ClientSystemHandler[] m_ClientSystemHandlers = new ClientSystemHandler[SystemMessagesCount];
        private static readonly ServerSystemHandler[] m_ServerSystemHandlers = new ServerSystemHandler[SystemMessagesCount];
        private static ClientHandlers[] m_ClientHandlers = new ClientHandlers[1] { new ClientHandlers() };
        private static ServerHandlers[] m_ServerHandlers = new ServerHandlers[1] { new ServerHandlers() };
        private static uint[] m_NextMessageIDs = new uint[1];
        private static volatile bool m_IsInitialized = false;
        private static volatile bool m_IsValid = false;
        private static readonly object _lock = new object();
        private static ushort m_NextGroupID; // We use ushort instead of byte, to gracefully handle ID exhaustion.
        private static uint m_NextModID; // We use uint instead of ushort, to gracefully handle ID exhaustion.




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
        /// Retrieves next mod id for internal usage.
        /// </summary>
        public static ushort NextModID()
        {
            if (m_NextModID >= MaxModIDAmount)
            {
                throw new Exception("Exhausted all Mod IDs for Riptide networking.");
            }

            return (ushort)++m_NextModID;
        }

        /// <summary>
        /// Retrieves next group ID for networking with <see cref="Toolkit"/>.
        /// </summary>
        /// TODO: Make sure that <see cref="DefaultGroup"/> is initialized first, to take 0th group ID.
        /// However, do not force it - only initialize it first if any message handlers in main assembly define it.
        /// This is to make it possible for people to define their own default groups. No matter how confusing it is.
        public static byte NextGroupID()
        {
            if (m_NextGroupID >= MaxGroupIDAmount)
            {
                throw new Exception("Exhausted all network Group IDs for Riptide networking.");
            }

            ushort index = m_NextGroupID;
            if (index >= m_NextMessageIDs.Length)
            {
                Array.Resize(ref m_ClientHandlers, m_NextGroupID);
                m_ClientHandlers[index] = new ClientHandlers();

                Array.Resize(ref m_ServerHandlers, m_NextGroupID);
                m_ServerHandlers[index] = new ServerHandlers();

                Array.Resize(ref m_NextMessageIDs, m_NextGroupID);
            }

            m_NextGroupID = (ushort)(index + 1);
            return (byte)index;
        }

        public static ushort NextMessageID(byte groupID)
        {
            // We use '>=' because ID is 0-based value, and Limit is 1-based value.
            if (m_NextMessageIDs[groupID] >= MaxMessageIDAmount)
            {
                throw new Exception("Exhausted all network Message IDs for Riptide networking.");
            }

            return (ushort)++m_NextMessageIDs[groupID];
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal static void Register(byte systemID, ClientSystemHandler handler) => m_ClientSystemHandlers[systemID] = handler;
        internal static void Register(byte systemID, ServerSystemHandler handler) => m_ServerSystemHandlers[systemID] = handler;
        internal static void HandleClient(byte systemID, AdvancedClient client, MessageReceivedEventArgs args) => m_ClientSystemHandlers[systemID](client, args);
        internal static void HandleServer(byte systemID, AdvancedServer server, MessageReceivedEventArgs args) => m_ServerSystemHandlers[systemID](server, args);




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
            if (m_IsValid) return;
            m_IsValid = true;

            try
            {
                // Resets all handlers first.
                // Note: All managed handlers will be locked at this point, so mutation is safe, even in multi-threaded context (untested though)
                // TODO: Avoid clearing of the entire database on first Initialization. All handlers will be empty at this point anyway.
                Array.ForEach(m_ClientHandlers, (client) => MessageHandlerCollection<ClientHandlers.HandlerInfo>.Unsafe.Clear(client.Handlers));
                Array.ForEach(m_ServerHandlers, (server) => MessageHandlerCollection<ServerHandlers.HandlerInfo>.Unsafe.Clear(server.Handlers));

                // Fetches own assembly first.
                Assembly current = Assembly.GetExecutingAssembly();
                FetchHandlers(current);

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
                        if (extension.Equals(reference.FullName))
                        {
                            FetchHandlers(assembly);
                            break;
                        }
                    }
                }


                // Simplifications:
                void FetchHandlers(Assembly assembly)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                        {
                            RegisterHandlers(method);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RiptideLogger.Log(LogType.Error, $"{LogPrefix} Could not update message handlers gracefully! Networking is likely broken at this point.");
                RiptideLogger.Log(LogType.Error, $"{ex}\n{ex.StackTrace}");
            }
        }

        private static void RegisterHandlers(MethodInfo method)
        {
            // TODO: Update, so it works better.
            if (!method.IsDefined(typeof(AdvancedMessageAttribute))) return;

            RiptideLogger.Log(LogType.Warning, method.Name);

            // TODO: Analyze multiple attributes.
            AdvancedMessageAttribute attribute = method.GetCustomAttribute<AdvancedMessageAttribute>();

            // Differentiates client-side and server-side message handlers by the parameter types they specify.
            ParameterInfo[] parameters = method.GetParameters();
            Type dataType;
            byte groupID = attribute.GroupID is null ? (byte)(GetValue(attribute.Group) ?? 0) : attribute.GroupID.Value;
            ushort modID = attribute.Mod is null ? (ushort)0 : (ushort)GetValue(attribute.Mod);
            ushort messageID;
            switch (parameters.Length)
            {
                // Likely client-side method - they only have the NetworkMessage in parameters.
                case 1:
                    dataType = parameters[0].ParameterType;
                    if (typeof(NetworkMessage).IsAssignableFrom(dataType))
                    {
                        if (AdvancedMessageAttribute.LookupMember<MessageIDAttribute>(
                            dataType, out MemberInfo member,
                            Reflections.MessageAttributeAnalysis_PrioritizeFields))
                        {
                            messageID = (ushort)GetValue(member);
                        }
                        else
                        {
                            throw new Exception($"{LogPrefix} Cannot find {nameof(MessageIDAttribute)} in {nameof(NetworkMessage)} inside {method.Name} body.");
                        }
                    }
                    else if (attribute.MessageID.HasValue)
                    {
                        messageID = attribute.MessageID.Value;
                    }
                    else if (attribute.Message is null)
                    {
                        throw new Exception($"{LogPrefix} MessageID for {method.Name} was not provided by any of 3 known methods" +
                            $"(method body, ID in attribute, type in attribute)");
                    }
                    else
                    {
                        messageID = (ushort)GetValue(attribute.Message);
                    }

                    ClientHandlers.HandlerInfo clientHandler = new ClientHandlers.HandlerInfo(method, dataType);
                    MessageHandlerCollection<ClientHandlers.HandlerInfo>.Unsafe.Set(m_ClientHandlers[groupID].Handlers, modID, messageID, clientHandler);
                    break;

                // Likely server-side method - they have UserID and INetworkMessage in parameters.
                case 2:
                    if (parameters[0].ParameterType != typeof(ushort))
                    {
                        throw new Exception($"Message handler ({method.Name}) is likely server-side, but doesn't have ClientID (ushort) as first parameter.");
                    }

                    dataType = parameters[1].ParameterType;
                    if (typeof(NetworkMessage).IsAssignableFrom(dataType))
                    {
                        if (AdvancedMessageAttribute.LookupMember<MessageIDAttribute>(
                            dataType, out MemberInfo member,
                            Reflections.MessageAttributeAnalysis_PrioritizeFields))
                        {
                            messageID = (ushort)GetValue(member);
                        }
                        else
                        {
                            throw new Exception($"{LogPrefix} Cannot find {nameof(MessageIDAttribute)} in {nameof(NetworkMessage)} inside {method.Name} body.");
                        }
                    }
                    else if (attribute.MessageID.HasValue)
                    {
                        messageID = attribute.MessageID.Value;
                    }
                    else if (attribute.Message is null)
                    {
                        throw new Exception($"{LogPrefix} MessageID for {method.Name} was not provided by any of 3 known methods" +
                            $"(method body, ID in attribute, type in attribute)");
                    }
                    else
                    {
                        messageID = (ushort)GetValue(attribute.Message);
                    }

                    ServerHandlers.HandlerInfo serverHandler = new ServerHandlers.HandlerInfo(method, dataType);
                    MessageHandlerCollection<ServerHandlers.HandlerInfo>.Unsafe.Set(m_ServerHandlers[groupID].Handlers, modID, messageID, serverHandler);
                    break;

                // Any other kind of signature is not supported at the moment.
                default:
                case 0: throw new Exception($"Message handler ({method.Name}) doesn't have client-side nor server-side Riptide message signature.");
            }

            // Simplifications:
            object GetValue(MemberInfo member)
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
}
