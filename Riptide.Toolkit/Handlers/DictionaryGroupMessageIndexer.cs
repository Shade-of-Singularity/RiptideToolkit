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

using System.Collections.Generic;

namespace Riptide.Toolkit.Handlers
{
    public sealed class DictionaryGroupMessageIndexer : GroupMessageIndexer
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// By how much bits we need to move an input value to the right to rebase its value around bit #0.
        /// </summary>
        private const int FlagOffset = 4;
        /// <summary>
        /// Mask which describes bits that one flag covers.
        /// </summary>
        private const uint FlagMask = 0b1111;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private readonly Dictionary<uint, uint> m_Flags = new Dictionary<uint, uint>(0);
        private readonly object _lock = new object();
        private uint m_MessageIDHeadIndex;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public DictionaryGroupMessageIndexer(byte groupID) : base(groupID) { }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override void Clear()
        {
            lock (_lock) m_Flags.Clear();
        }

        /// <inheritdoc/>
        public override IndexDefinition Get(uint messageID)
        {
            NetworkIndex.Initialize();
            return GetInternal(messageID);
        }

        /// <inheritdoc/>
        public override void Set(uint messageID, IndexDefinition definition) => SetInternal(messageID, definition, clear: IndexDefinition.Both);

        /// <inheritdoc/>
        public override void Add(uint messageID, IndexDefinition definition)
        {
            switch (definition)
            {
                case IndexDefinition.Both:
                case IndexDefinition.Client:
                case IndexDefinition.Server: SetInternal(messageID, definition, clear: definition); return;

                default:
                case IndexDefinition.None: return;
            }
        }

        /// <inheritdoc/>
        public override uint Put(IndexDefinition definition)
        {
            uint head = m_MessageIDHeadIndex;
            while (true)
            {
                // TODO: Optimize.
                if (GetInternal(head) == IndexDefinition.None)
                {
                    if (definition == IndexDefinition.None) return m_MessageIDHeadIndex = head;
                    else
                    {
                        SetInternal(head, definition, clear: IndexDefinition.Both);
                        m_MessageIDHeadIndex = head + 1;
                        return head;
                    }
                }

                checked { head++; }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private IndexDefinition GetInternal(uint messageID)
        {
            uint location = messageID >> FlagOffset;
            int offset = (int)(messageID & FlagMask);
            lock (_lock)
            {
                if (m_Flags.TryGetValue(location, out uint flag))
                {
                    return (IndexDefinition)(flag >> offset) & IndexDefinition.Both;
                }
            }

            return IndexDefinition.None;
        }

        /// <param name="clear">Describes bits in flag, that have to be cleared before updating value.</param>
        private void SetInternal(uint messageID, IndexDefinition definition, IndexDefinition clear)
        {
            uint location = messageID >> FlagOffset;
            int offset = (int)(messageID & FlagMask);
            if (definition == IndexDefinition.None)
            {
                // Removes flag.
                lock (_lock)
                {
                    if (m_Flags.TryGetValue(location, out uint flag))
                    {
                        flag &= ~((uint)clear << offset);
                        if (flag == 0u) m_Flags.Remove(location);
                        else m_Flags[location] = flag;
                    }
                }

                // TODO: Benchmark if Math.Min() will be more optimized.
                if (m_MessageIDHeadIndex > messageID)
                {
                    m_MessageIDHeadIndex = messageID;
                }
            }
            else
            {
                // Adds flag
                lock (_lock)
                {
                    if (m_Flags.TryGetValue(location, out uint flag))
                    {
                        uint frame = flag & ~((uint)clear << offset);
                        uint value = (uint)(definition & clear) << offset;
                        m_Flags[location] = frame | value;
                    }
                    else
                    {
                        m_Flags[location] = (uint)definition << offset;
                    }
                }

                if (m_MessageIDHeadIndex == messageID)
                {
                    m_MessageIDHeadIndex = messageID + 1;
                }
            }
        }
    }
}
