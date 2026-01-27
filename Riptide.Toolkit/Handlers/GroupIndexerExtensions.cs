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

using System.Runtime.CompilerServices;

namespace Riptide.Toolkit.Handlers
{
    public static class GroupIndexerExtensions
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Get Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasClient(this IReadOnlyGroupMessageIndexer indexer, uint messageID)
        {
            return (indexer.Get(messageID) & IndexDefinition.Client) == IndexDefinition.Client;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasServer(this IReadOnlyGroupMessageIndexer indexer, uint messageID)
        {
            return (indexer.Get(messageID) & IndexDefinition.Server) == IndexDefinition.Server;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAny(this IReadOnlyGroupMessageIndexer indexer, uint messageID)
        {
            return indexer.Get(messageID) == IndexDefinition.Both;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNone(this IReadOnlyGroupMessageIndexer indexer, uint messageID)
        {
            return indexer.Get(messageID) == IndexDefinition.None;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Set Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove(this IGroupMessageIndexer indexer, uint messageID)
        {
            indexer.Set(messageID, IndexDefinition.None);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetClient(this IGroupMessageIndexer indexer, uint messageID)
        {
            indexer.Set(messageID, IndexDefinition.Client);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetServer(this IGroupMessageIndexer indexer, uint messageID)
        {
            indexer.Set(messageID, IndexDefinition.Server);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBoth(this IGroupMessageIndexer indexer, uint messageID)
        {
            indexer.Set(messageID, IndexDefinition.Both);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddClient(this IGroupMessageIndexer indexer, uint messageID)
        {
            indexer.Add(messageID, IndexDefinition.Client);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddServer(this IGroupMessageIndexer indexer, uint messageID)
        {
            indexer.Add(messageID, IndexDefinition.Server);
        }
    }
}
