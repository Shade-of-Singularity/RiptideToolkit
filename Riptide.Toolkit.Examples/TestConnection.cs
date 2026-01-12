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

using System;
using System.Collections;
using UnityEngine;

namespace Riptide.Toolkit.Examples
{
    // TODO: Remove UnityEngine reference.
    // TODO: Move to Basics example class.
    public static class TestConnection
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static AdvancedServer Server { get; } = new AdvancedServer();
        public static AdvancedClient Client { get; } = new AdvancedClient();
        public static ushort ServerPort { get; } = 52323;
        public static bool Enabled
        {
            get => m_Enabled;
            set
            {
                if (m_Enabled == value) return;
                if (m_Enabled = value)
                {
                    OnEnabled();
                }
                else
                {
                    OnDisabled();
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private sealed class Runner : MonoBehaviour { }
        private static readonly Runner m_Runner = new GameObject("Runner").AddComponent<Runner>();
        private static bool m_Enabled = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static void Start() => Enabled = true;
        public static void End() => Enabled = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void OnDisabled()
        {
            m_Runner.StopAllCoroutines();
            Client.Disconnect();
            Server.Stop();
            Debug.Log("Test connection was closed.");
        }

        private static void OnEnabled()
        {
            m_Runner.StopAllCoroutines();
            m_Runner.StartCoroutine(StartSequence());
        }

        private static IEnumerator StartSequence()
        {
            Debug.Log("Starting test connection.");
            int payload = Message.MaxPayloadSize;
            Message.MaxPayloadSize = ChunkContainer.ChunkVolume * 4 + ushort.MaxValue;
            Server.Start(ServerPort, 1, messageHandlerGroupId: ExampleGroup.GroupID);
            if (!Client.Connect($"127.0.0.1:{ServerPort}", messageHandlerGroupId: ExampleGroup.GroupID))
            {
                Debug.LogWarning("Cannot connect to the server.");
                Enabled = false;
                Message.MaxPayloadSize = payload;
                yield break;
            }

            uint repeats = 64;
            while (Client.IsConnecting && repeats > 0)
            {
                yield return Update();
                repeats--;
            }

            if (Client.IsConnecting && repeats == 0)
            {
                Debug.LogWarning("Client connection timeout.");
                Enabled = false;
                Message.MaxPayloadSize = payload;
                yield break;
            }

            Debug.Log("Sending example messages.");
            yield return SendChunkContainerMessage();
            yield return SendValidateChunkMessage();
            yield return SendReceiveInventoryMessage();
            yield return SendVFXSignalMessage();
            Debug.Log("All messages was sent! Test concluded.");
            Enabled = false;
            Message.MaxPayloadSize = payload;

            IEnumerator SendChunkContainerMessage()
            {
                // Sending chunk data.
                ChunkContainer chunk = ChunkContainer.Get();
                chunk.x = 4; chunk.y = 4;
                chunk.blocks = new uint[ChunkContainer.ChunkVolume];
                Array.Fill<uint>(chunk.blocks, 1);

                yield return Update();
                Client.Send(chunk.Pack(mode: MessageSendMode.Reliable));
                yield return Update();
                Server.SendToAll(chunk.Pack(mode: MessageSendMode.Reliable));
                yield return Update();

                chunk.Release();
            }

            IEnumerator SendValidateChunkMessage()
            {
                // Sending validation data.
                ValidateChunk validate = ValidateChunk.Get();
                validate.x = 4; validate.y = 4;
                validate.hash = (ulong)new System.Random().Next();

                yield return Update();
                Client.Send(validate.Pack(mode: MessageSendMode.Reliable));
                yield return Update();

                // Notice that we use PackRelease here, to immediately release container.
                Server.SendToAll(validate.PackRelease(mode: MessageSendMode.Reliable));
                yield return Update();
            }

            IEnumerator SendReceiveInventoryMessage()
            {
                const int InventorySize = 46;

                // Sending inventory data.
                ReceiveInventory inventory = ReceiveInventory.Get();
                inventory.ids = new uint[InventorySize];
                inventory.amounts = new uint[InventorySize];
                Array.Fill<uint>(inventory.ids, 1);
                Array.Fill<uint>(inventory.amounts, 1);
                inventory.ids[0] = 12;
                inventory.ids[1] = 4;
                inventory.ids[2] = 4;
                inventory.amounts[0] = 2;
                inventory.amounts[0] = 64;
                inventory.amounts[0] = 64;

                yield return Update();
                Client.Send(inventory.Pack(mode: MessageSendMode.Reliable));
                yield return Update();
                Server.SendToAll(inventory.Pack(mode: MessageSendMode.Reliable));
                yield return Update();

                inventory.Release();
            }

            IEnumerator SendVFXSignalMessage()
            {
                // Sending validation data.
                VFXSignal signal = VFXSignal.Get();

                yield return Update();
                Client.Send(signal.Pack(mode: MessageSendMode.Reliable));
                yield return Update();
                Server.SendToAll(signal.PackRelease(mode: MessageSendMode.Reliable));
                yield return Update();
            }
        }

        static IEnumerator Update()
        {
            yield return null;
            Client.Update();
            yield return null;
            Server.Update();
            yield return null;
            Client.Update();
            yield return null;
            Server.Update();
            yield return null;
        }
    }
}
