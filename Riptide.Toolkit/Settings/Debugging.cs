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

namespace Riptide.Toolkit.Settings
{
    /// <summary>
    /// Debugging options. Can be disabled to improve performance a bit.
    /// </summary>
    public static class Debugging
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const string OptimizeDebuggingFlag = "-riptide.toolkit.debug.optimize";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether to log in console that <see cref="MessageHandlerAttribute"/>s are not supported
        /// and <see cref="AdvancedMessageAttribute"/> should be used instead, if any were found.
        /// </summary>
        /// <remarks>
        /// Disabling this feature might remove quite a few attribute checks,
        /// but will make it harder to understand why old handlers don't work.
        /// We expect everyone to eventually set this value to <c>false</c>, after they learn about <see cref="AdvancedMessageAttribute"/>s.
        /// </remarks>
        /// Note: Should be 'true' by default (unless overwritten), to incentivize natural learning via trial-and-error.
        public static bool WarnAboutRiptideMessages { get; set; } = !ConsoleArgs.Has(OptimizeDebuggingFlag);
    }
}
