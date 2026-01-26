using System;
using System.Collections.Generic;

namespace Riptide.Toolkit.Relaying
{
    public sealed class DictionaryAdvancedRelayFilter : AdvancedRelayFilter
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Size (in bits) of one flag in a filter array.
        /// </summary>
        private const uint FlagSize = sizeof(uint) * 8;
        /// <summary>
        /// Offset (in bits) from bit #0, covering <see cref="FlagMask"/>.
        /// </summary>
        private const int FlagOffset = 5;
        /// <summary>
        /// Mask covering all bits for one filter flag.
        /// </summary>
        private const uint FlagMask = FlagSize - 1;



        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private readonly Dictionary<uint, uint> m_Filters = new Dictionary<uint, uint>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override void EnableRelay(uint messageID)
        {
            uint region = messageID >> FlagOffset;
            uint pin = 1u << (int)(messageID & FlagMask);
            if (m_Filters.TryGetValue(region, out uint flag))
            {
                m_Filters[region] = flag | pin;
            }
            else
            {
                m_Filters[region] = pin;
            }
        }

        public override void DisableRelay(uint messageID)
        {
            uint region = messageID >> FlagOffset;
            uint pin = 1u << (int)(messageID & FlagMask);
            if (m_Filters.TryGetValue(region, out uint flag))
            {
                flag &= ~pin;
                if (flag == 0u) m_Filters.Remove(region);
                else m_Filters[region] = flag;
            }
        }

        public override bool ShouldRelay(uint messageID)
        {
            uint region = messageID >> FlagOffset;
            uint pin = 1u << (int)(messageID & FlagMask);
            if (m_Filters.TryGetValue(region, out uint flag))
            {
                return (flag & pin) != 0u;
            }

            return false;
        }

        public override void Clear() => m_Filters.Clear();
    }
}
