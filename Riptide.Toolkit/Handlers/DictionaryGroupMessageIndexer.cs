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
        private const int FlagOffset = 5;

        /// <summary>
        /// Mask which describes bits that one flag covers.
        /// </summary>
        private const uint FlagMask = 31;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private Dictionary<uint, uint> m_Flags = new Dictionary<uint, uint>();
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
        public override bool Has(uint messageID)
        {
            // TODO: Consider using concurrent dictionary instead.
            uint location = messageID >> FlagOffset;
            uint bit = 1u << (int)(messageID & FlagMask);
            lock (_lock)
            {
                return m_Flags.TryGetValue(location, out uint flag) && (flag & bit) != 0;
            }
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            lock (_lock) m_Flags.Clear();
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            lock (_lock) m_Flags = new Dictionary<uint, uint>();
        }

        /// <inheritdoc/>
        public override void Register(uint messageID)
        {
            uint location = messageID >> FlagOffset;
            lock (_lock)
            {
                if (m_Flags.TryGetValue(location, out uint flag))
                {
                    m_Flags[location] = flag | (1u << (int)(messageID & FlagMask));
                }
                else
                {
                    m_Flags[location] = 1u << (int)(messageID & FlagMask);
                }
            }
        }

        /// <inheritdoc/>
        public override void Remove(uint messageID)
        {
            uint location = messageID >> FlagOffset;
            lock (_lock)
            {
                if (m_Flags.TryGetValue(location, out uint flag))
                {
                    flag &= ~(1u << (int)(messageID & FlagMask));
                    if (flag == 0) m_Flags.Remove(location);
                    else m_Flags[location] = flag;
                }
            }
        }
    }
}
