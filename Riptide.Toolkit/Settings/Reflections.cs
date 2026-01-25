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
    /// Holds Toolkit advanced performance options you can configure to improve performance a bit.
    /// </summary>
    /// TODO: Field/Property prioritization can probably be automated, by using last successful priority.
    /// I doubt developers will define multiple base mod types.
    public static class Reflections
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// This value controls how <see cref="AdvancedMessageAttribute"/> constructor with only one multicast type will treat multicast type.
        /// By default check if it's <see cref="Toolkit.Messages.NetworkMessage"/> first,
        /// and then if it's <see cref="Toolkit.Messages.NetworkGroup"/>.
        /// <para>
        /// If you prefer using <see cref="Toolkit.Messages.NetworkGroup"/> more often - you can set this value to <c>false</c>.
        /// With a lot of message handlers, might same a few parts of a second during initialization.
        /// </para>
        /// </summary>
        public static bool AdvancedMessageHandlerConstructor_ExpectSingleMulticastToBeAMessage { get; set; } = true;

        /// <summary>
        /// In <see cref="AdvancedMessageAttribute"/>, there is a constructor with two multicast types.
        /// Be default, we expect first multicast type to be <see cref="Toolkit.Messages.NetworkMessage"/><>,
        /// and second <see cref="Toolkit.Messages.NetworkGroup"/>.
        /// <para>
        /// If you prefer using <see cref="Toolkit.Messages.NetworkGroup"/> as a first multicast and <see cref="Toolkit.Messages.NetworkMessage"/>
        /// as second, you can set this value to false. It will flip order around.
        /// </para>
        /// </summary>
        public static bool AdvancedMessageHandlerConstructor_ExpectFirstMulticastToBeAMessage { get; set; } = true;
    }
}
