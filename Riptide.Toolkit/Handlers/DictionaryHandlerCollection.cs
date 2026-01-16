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
    public sealed class DictionaryHandlerCollection<THandler> : MessageHandlerCollection<THandler>
        where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Array of RegionMaps with all the handlers.
        /// </summary>
        private Dictionary<ushort, THandler>[] m_Handlers;

        /// <summary>
        /// Direct reference to RegionMap under core mod.
        /// Saves few instructions.
        /// </summary>
        private readonly Dictionary<ushort, THandler> m_MainHandlers;

        /// <summary>
        /// Index of the next (probably) free cell in region array.
        /// </summary>
        private int m_HeadIndex = (int)SystemMessageID.Amount; // Avoids ID range, allocated for system messages.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Constructor for <see cref="DictionaryHandlerCollection{THandler}"/>
        /// </summary>
        /// <remarks>
        /// <paramref name="size"/> MUST be "Power of Two"! (i.e. 2, 4, 8, 16, 32, ...)
        /// </remarks>
        public DictionaryHandlerCollection()
        {
            m_Handlers = new Dictionary<ushort, THandler>[1];
            m_Handlers[0] = m_MainHandlers = new Dictionary<ushort, THandler>();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        /// <exception cref="KeyNotFoundException">
        /// Means that handler under <paramref name="messageID"/> is not defined.
        /// </exception>
        public override THandler Get(ushort messageID)
        {
            return m_MainHandlers[messageID];
        }

        /// <inheritdoc/>
        /// <exception cref="KeyNotFoundException">
        /// Means that handler under <paramref name="messageID"/> is not defined.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// Mod under <paramref name="modID"/> is not defined.
        /// </exception>
        public override THandler Get(ushort modID, ushort messageID)
        {
            return m_Handlers[modID][messageID];
        }

        /// <inheritdoc/>
        public override bool Has(ushort messageID)
        {
            return m_MainHandlers.ContainsKey(messageID);
        }

        /// <inheritdoc/>
        public override bool Has(ushort modID, ushort messageID)
        {
            // Assumes that mod lists are filled-in one-by-one.
            if (modID >= m_Handlers.Length) return false;
            var handlers = m_Handlers[modID];
            if (handlers is null) return false;
            return handlers.ContainsKey(messageID);
        }

        /// <inheritdoc/>
        public override bool TryGet(ushort messageID, out THandler hander)
        {
            return m_MainHandlers.TryGetValue(messageID, out hander);
        }

        /// <inheritdoc/>
        public override bool TryGet(ushort modID, ushort messageID, out THandler hander)
        {
            if (modID >= m_Handlers.Length)
            {
                hander = default;
                return false;
            }

            return m_Handlers[modID].TryGetValue(messageID, out hander);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        protected override void Clear()
        {
            var mods = m_Handlers;
            for (int i = 0; i < mods.Length; i++)
            {
                mods[i]?.Clear();
            }
        }

        /// <inheritdoc/>
        protected override void Reset()
        {
            var mods = m_Handlers;
            for (int i = 0; i < mods.Length; i++)
            {
                if (!(mods[i] is null)) mods[i] = new Dictionary<ushort, THandler>();
            }
        }

        /// <inheritdoc/>
        protected override void Set(ushort modID, ushort messageID, THandler handler)
        {
            if (modID >= m_Handlers.Length)
            {
                Array.Resize(ref m_Handlers, modID + 1);
                m_Handlers[modID] = new Dictionary<ushort, THandler>();
            }
            else if (m_Handlers[modID] is null)
            {
                m_Handlers[modID] = new Dictionary<ushort, THandler>();
            }

            m_Handlers[modID][messageID] = handler;
        }

        /// <inheritdoc/>
        protected override ushort Put(ushort modID, THandler handler)
        {
            Dictionary<ushort, THandler> regions;
            if (modID >= m_Handlers.Length)
            {
                Array.Resize(ref m_Handlers, modID + 1);
                m_Handlers[modID] = regions = new Dictionary<ushort, THandler>();
            }
            else if (m_Handlers[modID] is null)
            {
                m_Handlers[modID] = regions = new Dictionary<ushort, THandler>();
            }
            else
            {
                regions = m_Handlers[modID];
            }

            // Allocates local array variable to reduce instruction amount.
            for (; m_HeadIndex <= ushort.MaxValue; m_HeadIndex++)
            {
                if (!regions.ContainsKey((ushort)m_HeadIndex))
                {
                    regions[(ushort)m_HeadIndex] = handler;
                }
            }

            // Moves head to (potentially free) next ID. 
            return (ushort)(++m_HeadIndex);
        }

        /// <inheritdoc/>
        protected override void Remove(ushort modID, ushort messageID)
        {
            if (modID >= m_Handlers.Length) return;
            var handlers = m_Handlers[modID];
            if (handlers is null) return;
            handlers.Remove(messageID);
        }
    }
}