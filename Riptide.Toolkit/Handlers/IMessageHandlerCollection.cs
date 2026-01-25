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
    public interface IMessageHandlerCollection<THandler> : IReadOnlyMessageHandlerCollection<THandler>
        where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Clears internal handler array.
        /// </summary>
        /// <remarks>
        /// Clears even system messages.
        /// <para>
        /// Will NOT initialize <see cref="NetworkIndex"/> when called, 
        /// unlike <see cref="IReadOnlyMessageHandlerCollection{THandler}"/>,
        /// to avoid deadlocks.
        /// </para>
        /// </remarks>
        void Clear();

        /// <summary>
        /// Clears internal handler array and resizes buffers to default size.
        /// GC will be able to collect released resources, if there is any.
        /// </summary>
        /// <remarks>
        /// Clears even system messages.
        /// <para>
        /// Will NOT initialize <see cref="NetworkIndex"/> when called, 
        /// unlike <see cref="IReadOnlyMessageHandlerCollection{THandler}"/>,
        /// to avoid deadlocks.
        /// </para>
        /// </remarks>
        void Reset();

        /// <summary>
        /// Registers message handler on target <paramref name="messageID"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Will NOT initialize <see cref="NetworkIndex"/> when called, 
        /// unlike <see cref="IReadOnlyMessageHandlerCollection{THandler}"/>,
        /// to avoid deadlocks.
        /// </para>
        /// </remarks>
        /// <param name="messageID">Handler MessageID to associate with <paramref name="handler"/>.</param>
        /// <param name="handler">Message handler to register.</param>
        void Set(uint messageID, THandler handler);

        /// <summary>
        /// Puts <paramref name="handler"/> on the next free MessageID.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Will NOT initialize <see cref="NetworkIndex"/> when called, 
        /// unlike <see cref="IReadOnlyMessageHandlerCollection{THandler}"/>,
        /// to avoid deadlocks.
        /// </para>
        /// </remarks>
        /// <param name="handler">Message handler to store.</param>
        /// <returns>MessageID under which handler was registered.</returns>
        uint Put(THandler handler);

        /// <summary>
        /// Removes message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Will NOT initialize <see cref="NetworkIndex"/> when called, 
        /// unlike <see cref="IReadOnlyMessageHandlerCollection{THandler}"/>,
        /// to avoid deadlocks.
        /// </para>
        /// </remarks>
        /// <param name="messageID">Handler MessageID to check and remove.</param>
        void Remove(uint messageID);
    }
}
