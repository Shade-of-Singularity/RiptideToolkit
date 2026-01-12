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

using Riptide;
using UnityEngine;

namespace Riptide.Toolkit.Examples
{
    public static class TestHandlers
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Client-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [AdvancedMessage]
        public static void SendChunkHandler(ChunkContainer chunk)
        {
            Debug.Log($"Client-side {nameof(SendChunkHandler)} was fired with data: {chunk}");
        }

        [AdvancedMessage]
        public static void ValidateChunkHandler(ValidateChunk chunk)
        {
            Debug.Log($"Client-side {nameof(ValidateChunkHandler)} was fired with data: {chunk}");
        }

        [AdvancedMessage(typeof(ReceiveInventory))] // We need to specify type if we put raw message in method parameters.
        public static void ReceiveInventoryHandler(Message message)
        {
            var container = new ReceiveInventory();
            container.Read(message);
            Debug.Log($"Client-side {nameof(ReceiveInventoryHandler)} was fired with data: {container}");
        }

        [AdvancedMessage]
        public static void HandleVFXSignal(VFXSignal _)
        {
            Debug.Log($"Client-side {nameof(VFXSignal)} received.");
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Server-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [AdvancedMessage]
        public static void SendChunkHandler(ushort clientID, ChunkContainer chunk)
        {
            Debug.Log($"Server-side {nameof(SendChunkHandler)} was fired with data from client ({clientID}): {chunk}");
        }

        [AdvancedMessage]
        public static void ValidateChunkHandler(ushort clientID, ValidateChunk chunk)
        {
            Debug.Log($"Server-side {nameof(ValidateChunkHandler)} was fired with data from client ({clientID}): {chunk}");
        }

        [AdvancedMessage(typeof(ReceiveInventory))] // We need to specify type if we put raw message in method parameters.
        public static void ReceiveInventoryHandler(ushort clientID, Message message)
        {
            var container = new ReceiveInventory();
            container.Read(message);
            Debug.Log($"Server-side {nameof(ReceiveInventoryHandler)} was fired with data from client ({clientID}): {container}");
        }

        [AdvancedMessage]
        public static void HandleVFXSignal(ushort clientID, VFXSignal _)
        {
            Debug.Log($"Server-side {nameof(VFXSignal)} received from client ({clientID})");
        }
    }
}
