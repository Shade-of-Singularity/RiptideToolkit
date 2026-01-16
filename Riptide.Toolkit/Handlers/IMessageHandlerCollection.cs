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

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Interface for <see cref="ClientHandlers"/> and <see cref="ServerHandlers"/> to implement.
    /// Specifies API for <see cref="MessageHandlerCollection{THandler}"/>.
    /// </summary>
    public interface IMessageHandlerCollection<THandler> where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <param name="messageID">ID associated with an message handler.</param>
        /// <returns>Message handler under given ID, or throws.</returns>
        THandler Get(ushort messageID);

        /// <summary>
        /// Retrieves message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <param name="modID">ModID under which <paramref name="messageID"/> is registered.</param>
        /// <param name="messageID">ID associated with an message handler.</param>
        /// <returns>Message handler under given ID, or throws.</returns>
        THandler Get(ushort modID, ushort messageID);

        /// <summary>
        /// Checks if handler is defined.
        /// </summary>
        /// <param name="messageID">Handler MessageID to check.</param>
        /// <returns><c>true</c> if defined. <c>false</c> otherwise.</returns>
        bool Has(ushort messageID);

        /// <summary>
        /// Checks if handler for a specific <paramref name="modID"/> is defined.
        /// </summary>
        /// <param name="modID">ModID under which <paramref name="messageID"/> is registered.</param>
        /// <param name="messageID">Handler MessageID to check.</param>
        /// <returns><c>true</c> if defined. <c>false</c> otherwise.</returns>
        bool Has(ushort modID, ushort messageID);

        /// <summary>
        /// Tries to find message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <param name="messageID">ID associated with an message handler.</param>
        /// <param name="hander">Message handler under given ID or default value.</param>
        /// <returns><c>true</c> if handler was found. <c>false</c> otherwise.</returns>
        bool TryGet(ushort messageID, out THandler hander);

        /// <summary>
        /// Tries to find message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <param name="modID">ModID under which <paramref name="messageID"/> is registered.</param>
        /// <param name="messageID">ID associated with an message handler.</param>
        /// <param name="hander">Message handler under given ID or default value.</param>
        /// <returns><c>true</c> if handler was found. <c>false</c> otherwise.</returns>
        bool TryGet(ushort modID, ushort messageID, out THandler hander);
    }
}
