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
    /// I doublt developers will define multiple base mod types.
    public static class Reflections
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether to check for <see cref="GroupIDAttribute"/>
        /// on fields first, then properties (when true) or the other way around (when false)
        /// </summary>
        /// True by default, because <see cref="Toolkit.Messages.NetworkGroup{TGroup}.GroupID"/> is a field.
        public static bool GroupAttributeAnalysis_PrioritizeFields { get; set; } = true;

        /// <summary>
        /// Whether to check for <see cref="MessageIDAttribute"/>
        /// on fields first, then properties (when true) or the other way around (when false)
        /// </summary>
        /// True by default, because <see cref="Toolkit.Messages.NetworkMessage{TMessage, TGroup, TProfile}.MessageID"/> is a field.
        public static bool MessageAttributeAnalysis_PrioritizeFields { get; set; } = true;

        /// <summary>
        /// Whether to check for any attributes
        /// (<see cref="MessageIDAttribute"/>, <see cref="GroupIDAttribute"/> or <see cref="ModIDAttribute"/>)
        /// inside other handlers
        /// (<see cref="Toolkit.Messages.NetworkMessage"/>)
        /// on fields first, then properties (when true) or the other way around (when false)
        /// </summary>
        /// False by default, because <see cref="Toolkit.Messages.NetworkMessage{TMessage, TGroup, TProfile}.GroupID"/> is a property.
        public static bool ImbeddedAttributeAnalysis_PrioritizeFields { get; set; } = false;

        /// <summary>
        /// This value controls how <see cref="AdvancedMessageAttribute"/> constructor with only one multicast type will treat multicast type.
        /// Be default, we expect first multicast type to be <see cref="Toolkit.Messages.NetworkMessage"/><>,
        /// and second <see cref="Toolkit.Messages.NetworkGroup"/>.
        /// <para>
        /// If you prefer using <see cref="Toolkit.Messages.NetworkGroup"/> as a first multicast and <see cref="Toolkit.Messages.NetworkMessage"/>
        /// as second, you can set this value to false. It will flip order around.
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
