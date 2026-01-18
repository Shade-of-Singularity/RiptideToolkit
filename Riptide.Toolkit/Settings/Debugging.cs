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
