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

using Riptide.Toolkit.Extensions;
using System;
using System.Collections.Generic;

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Collection for network <see cref="Message"/> handlers.
    /// </summary>
    /// <remarks>
    /// Based on custom Region map implementation.
    /// Has higher memory usage, but 
    /// </remarks>
    /// Note: If in the future someone will create dictionary with ushort as a base - we will use it instead.
    public sealed class DictionaryMessageHandlerCollection<THandler> : MessageHandlerCollection<THandler>
        where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// RegionMap with all the handlers.
        /// </summary>
        private readonly Dictionary<uint, THandler> m_Handlers = new Dictionary<uint, THandler>();
        /// <summary>
        /// Index of the next (probably) free cell in region array.
        /// </summary>
        private uint m_HeadIndex;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        /// <exception cref="KeyNotFoundException">
        /// Means that handler under <paramref name="messageID"/> is not defined.
        /// </exception>
        public override THandler Get(uint messageID)
        {
            NetworkIndex.Initialize();
            return m_Handlers[messageID];
        }

        /// <inheritdoc/>
        public override bool Has(uint messageID)
        {
            NetworkIndex.Initialize();
            return m_Handlers.ContainsKey(messageID);
        }

        /// <inheritdoc/>
        public override bool TryGet(uint messageID, out THandler hander)
        {
            NetworkIndex.Initialize();
            return m_Handlers.TryGetValue(messageID, out hander);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override void Clear()
        {
            m_Handlers.Clear();
            m_HeadIndex = 0;
        }

        /// <inheritdoc/>
        public override void Set(uint messageID, THandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            m_Handlers[messageID] = handler;
        }

        /// <inheritdoc/>
        public override uint Put(THandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var regions = m_Handlers; // Allocates local array variable to reduce instruction amount.
            uint head = m_HeadIndex;
            while (head < NetworkIndex.InvalidMessageID)
            {
                if (!regions.ContainsKey(head))
                {
                    regions[head] = handler;

                    // Moves head to (potentially free) next ID. 
                    m_HeadIndex = head + 1;
                    return head;
                }

                head++;
            }

            throw new InvalidOperationException($"Exhausted all free MessageIDs (see also: {nameof(NetworkIndex)}.{NetworkIndex.InvalidMessageID})");
        }

        /// <inheritdoc/>
        public override void Remove(uint messageID)
        {
            if (m_Handlers.Remove(messageID) && m_HeadIndex > messageID)
            {
                m_HeadIndex = messageID;
            }
        }
    }
}