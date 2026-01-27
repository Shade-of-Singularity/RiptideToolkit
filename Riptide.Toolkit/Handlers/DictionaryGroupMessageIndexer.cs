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
/// 

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




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
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
            }
        }
    }
}
