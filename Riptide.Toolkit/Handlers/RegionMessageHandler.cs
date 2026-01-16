using Riptide.Toolkit.Extensions;
using Riptide.Toolkit.Settings;
using System;
using System.Runtime.InteropServices;

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
            m_RegionOffset = CountSetBits(m_RegionMask);

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
        protected override void Set(ushort messageID, THandler handler)
        {
            int regionIndex = messageID >> m_RegionOffset;
            int referenceIndex = messageID & m_RegionMask;

            var region = m_Regions[regionIndex];
            if (region is null)
            {
                m_Regions[regionIndex] = region = new THandler[m_RegionSize];
                region[referenceIndex] = handler;
            }
            else
            {
                region[referenceIndex] = handler;
            }
        }

        /// <inheritdoc/>
        protected override ushort Put(THandler handler)
        {
            // Allocates local array variable to reduce instruction amount.
            var regions = m_Regions;
            for (; m_HeadIndex <= ushort.MaxValue; m_HeadIndex++)
            {
                int regionIndex = m_HeadIndex >> m_RegionOffset;
                int referenceIndex = m_HeadIndex & m_RegionMask;
                THandler[] region = regions[regionIndex];
                if (region is null)
                {
                    m_Regions[regionIndex] = region = new THandler[m_RegionSize];
                    region[referenceIndex] = handler;
                    break;
                }
                else if (region[referenceIndex].IsDefault)
                {
                    region[referenceIndex] = handler;
                }
            }

            // Moves head to (potentially free) next ID. 
            return (ushort)(++m_HeadIndex);
        }

        /// <inheritdoc/>
        protected override void Remove(ushort messageID)
        {
            var region = m_Regions[messageID >> m_RegionOffset];
            if (!(region is null))
            {
                region[messageID & m_RegionMask] = default;
            }
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