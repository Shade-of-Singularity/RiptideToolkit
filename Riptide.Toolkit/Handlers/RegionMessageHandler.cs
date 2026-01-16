using Riptide.Toolkit.Extensions;
using Riptide.Toolkit.Settings;
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
    public sealed class RegionHandlerCollection<THandler> : MessageHandlerCollection<THandler>
        where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private readonly THandler[][] m_Regions;
        private readonly int m_RegionSize;
        private readonly int m_RegionMask;
        private readonly int m_RegionOffset;
        private int m_HeadIndex = (int)SystemMessageID.Amount; // Avoids ID range, allocated for system messages.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Simple constructor for <see cref="RegionHandlerCollection{THandler}"/>.
        /// Uses <see cref="Performance.RegionSize"/> as default region size for this collection.
        /// </summary>
        public RegionHandlerCollection() : this(Performance.RegionSize) { }
        
        /// <summary>
        /// Constructor for <see cref="RegionHandlerCollection{THandler}"/>
        /// </summary>
        /// <remarks>
        /// <paramref name="size"/> MUST be "Power of Two"! (i.e. 2, 4, 8, 16, 32, ...)
        /// </remarks>
        public RegionHandlerCollection(int size)
        {
            size = Math.Max(2, size);
            m_RegionSize = size;
            m_RegionMask = size - 1;
            m_RegionOffset = CountSetBits((uint)m_RegionMask);

            int regions = (ushort.MaxValue + 1) / size;
            m_Regions = new THandler[regions][];
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">
        /// Means that region for handlers under <paramref name="messageID"/> is not defined.
        /// Implies that message under this ID was not registered.
        /// </exception>
        public override THandler Get(ushort messageID)
        {
            return m_Regions[messageID >> m_RegionOffset][messageID & m_RegionMask];
        }

        /// <inheritdoc/>
        public override bool Has(ushort messageID)
        {
            var region = m_Regions[messageID >> m_RegionOffset];
            return !(region is null) && !region[messageID & m_RegionMask].IsDefault;
        }

        /// <inheritdoc/>
        public override bool TryGet(ushort messageID, out THandler hander)
        {
            var region = m_Regions[messageID >> m_RegionOffset];
            if (region is null)
            {
                hander = default;
                return false;
            }

            hander = region[messageID & m_RegionMask];
            return !hander.IsDefault;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        protected override void Clear()
        {
            int size = m_RegionSize;
            var array = m_Regions;
            for (int i = 0; i < array.Length; i++)
            {
                Array.Clear(array[i], 0, size);
            }
        }

        /// <inheritdoc/>
        protected override void Reset()
        {
            var array = m_Regions;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = null;
            }
        }

        /// <inheritdoc/>
        protected override ushort Put(THandler handler)
        {
            // Allocates local array variable to reduce instruction amount.
            var regions = m_Regions;
            var references = null;
            int length = regions.Length;

            // We use 'for' loop here instead of 'while', in case compiler is more familiar with this format.
            for (; m_HeadIndex < length; m_HeadIndex++)
            {
                // TODO: Optimize by updating region index only after passing to the second region (i.e. each 16).
                var references = regions[m_HeadIndex >> m_RegionOffset];
                if (references[m_HeadIndex & m_RegionMask].IsDefault) goto Finish;
            }

            // This point can only be reached after iterating over whole array.
            int size = DynamicArrays.NextArraySize(m_HeadIndex);
            if (size > ushort.MaxValue) throw new Exception($"{nameof(RegionHandlerCollection<THandler>)} Reached array size limit of ({ushort.MaxValue + 1}).");
            Array.Resize(ref regions, size);

            Finish:
            regions[m_HeadIndex] = handler;
            m_HeadIndex++; // Picks (potentially free) next ID as head. 
            return (int)m_HeadIndex;
        }

        /// <inheritdoc/>
        protected override void Remove(ushort messageID)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void Set(ushort messageID, THandler handler)
        {
            throw new NotImplementedException();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static int CountSetBits(int i)
        {
            i -= (i >> 1) & 0x55555555;
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
    }
}
