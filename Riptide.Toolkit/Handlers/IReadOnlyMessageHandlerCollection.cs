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
    public interface IReadOnlyMessageHandlerCollection<THandler>
        where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves message handler under given <paramref name="messageID"/>. Throws if handler wasn't found.
        /// </summary>
        /// <param name="messageID">ID associated with an message handler.</param>
        /// <returns>Message handler under given ID, or throws.</returns>
        THandler Get(uint messageID);

        /// <summary>
        /// Checks if handler is defined.
        /// </summary>
        /// <param name="messageID">Handler MessageID to check.</param>
        /// <returns><c>true</c> if defined. <c>false</c> otherwise.</returns>
        bool Has(uint messageID);

        /// <summary>
        /// Tries to find message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <param name="messageID">ID associated with an message handler.</param>
        /// <param name="hander">Message handler under given ID or default value.</param>
        /// <returns><c>true</c> if handler was found. <c>false</c> otherwise.</returns>
        bool TryGet(uint messageID, out THandler hander);
    }
}
