
using System.Runtime.CompilerServices;

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
namespace Riptide.Toolkit
{
    /// <summary>
    /// Base class for custom message headers.
    /// </summary>
    /// <remarks>
    /// Don't forget to register it via <see cref="NetHeaders.Register{T}()"/>.
    /// </remarks>
    public abstract class CustomHeader<T> where T : CustomHeader<T>, new()
    {
        public static readonly T Instance = new T();

        /// <summary>
        /// Position is updated by internal code after calling <see cref="NetHeaders.Register{T}()"/>.
        /// </summary>
        public static CustomHeaderPosition Position { get; set; } = CustomHeaderPosition.Default;

        /// <summary>
        /// Amount of bits used by the header.
        /// Those bits will be reserved for this specific header and can be filled-in a message.
        /// </summary>
        public abstract int GetBitsLength(SystemMessageID ID);

        /// <inheritdoc cref="NetHeaders.Register{T}()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register() => NetHeaders.Register<T>();
    }

    public readonly struct CustomHeaderPosition
    {
        public static readonly CustomHeaderPosition Default = new CustomHeaderPosition(0, 0);

        /// <summary>
        /// Position of a header, relative to <see cref="SystemMessaging.SystemMessageIDOffset"/>.
        /// </summary>
        public readonly int origin;
        /// <summary>
        /// Length of a header, in bits.
        /// </summary>
        public readonly int length;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="origin">Position of a header, relative to <see cref="SystemMessaging.SystemMessageIDOffset"/>.</param>
        /// <param name="length">Length of a header, in bits.</param>
        public CustomHeaderPosition(int origin, int length)
        {
            this.origin = origin;
            this.length = length;
        }
    }
}
