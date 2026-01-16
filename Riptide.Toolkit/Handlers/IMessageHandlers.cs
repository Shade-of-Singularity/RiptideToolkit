using System;
using System.Collections.Generic;
using System.Text;

namespace Riptide.Toolkit.Handlers
{
    public interface IMessageHandlersCollection<THandler>
    {
        /// <summary>
        /// Retrieves message handler from this collection.
        /// </summary>
        /// <param name="messageID">ID associated with an message handler.</param>
        /// <returns></returns>
        public THandler Get(ushort messageID)
        {
            NetworkIndex.Initialize();
            // We don't need checks here - it will throw anyway.
            // if (id > m_Handlers.Length) throw new ArgumentOutOfRangeException(nameof(id));
            return m_Handlers[messageID];
        }

        /// <summary>
        /// Checks if handler is defined.
        /// </summary>
        /// <param name="messageID">Handler MessageID to check.</param>
        /// <returns><c>true</c> if defined. <c>false</c> otherwise.</returns>
        public bool Has(ushort messageID)
        {
            NetworkIndex.Initialize();
            if (messageID > m_Handlers.Length) return false;
            return !m_Handlers[messageID].IsDefault;
        }

        public bool TryGet(ushort messageID, out THandler hander)
        {
            NetworkIndex.Initialize();
            if (messageID > m_Handlers.Length)
            {
                hander = default;
                return false;
            }

            hander = m_Handlers[messageID];
            return !hander.IsDefault;
        }
    }
}
