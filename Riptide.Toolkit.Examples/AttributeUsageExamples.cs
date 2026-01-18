namespace Riptide.Toolkit.Examples
{
    public static class AttributeUsageExamples
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Client-side
        /// .                         Note: This section doesn't cover all possible combinations.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Important points:

        // Here:
        // - For method, Riptide.Message is automatically read to 'CustomNetworkMessage'.
        // - MessageID and GroupID is retrieved from 'CustomNetworkMessage' class in method parameters.
        // - MessageID is defined automatically.
        // - Defaults to ModID (0) internally.
        [AdvancedMessage] public static void ReceiveChunkData(CustomNetworkMessage message) { }


        // Here:
        // - For method, raw Riptide.Message is provided instead.
        // - MessageID and GroupID is retrieved from 'CustomNetworkMessage' class in method parameters.
        // - MessageID is defined automatically.
        // - Defaults to ModID (0) internally.
        [AdvancedMessage(typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(Message message) { }


        // - You can provide types in attribute in any order:
        // - But this is the most unoptimized ordering.
        [AdvancedMessage(typeof(CustomNetworkMessage), typeof(ExampleMod), typeof(ExampleGroup))]
        public static void ReceiveChunkData(Message message) { }


        // - But Riptide.Toolkit predicts (Mod) -> (Group) -> (Message).
        // - Any other ordering will make reflection slower.
        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup), typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(Message message) { }


        // - Even if one type is missing, Riptide.Toolkit predicts (Mod) -> (Group) -> (Message).
        [AdvancedMessage(typeof(ExampleMod), typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(Message message) { }


        // Here:
        // - ModID is defined by custom class.
        // - typeof(Mod) need to have ModIDAttribute attached to any static field / static property.
        // - GroupID & MessageID can be provided manually.
        // - MessageID is defined using '0u' (ushort) to differentiate it from GroupID '0' (byte)
        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup), 0u)]
        public static void ReceiveChunkData(Message message) { }





        // Most combinations:
        // Invalid handler - no way to identify it.
        [AdvancedMessage]
        public static void ReceiveChunkData(Message message) { }

        // Defines GroupID manually in attribute.
        // MessageID is retrieved from 'CustomNetworkMessage' class in method parameters.
        // Will automatically read Riptide.Message to 'CustomNetworkMessage'
        [AdvancedMessage(0)] // '0' -> (byte)0 -> [GroupID]
        public static void ReceiveChunkData(CustomNetworkMessage message) { }

        // Defines GroupID & MessageID manually.
        // Will automatically read Riptide.Message to 'CustomNetworkMessage'
        [AdvancedMessage(0, 0)]
        public static void ReceiveChunkData(Message message) { }

        // Prioritizes GroupID defined in 'ExampleGroup' in attribute.
        // MessageID is retrieved from 'CustomNetworkMessage' class in method parameters.
        // Will automatically read Riptide.Message to 'CustomNetworkMessage'
        [AdvancedMessage(typeof(ExampleGroup))]
        public static void ReceiveChunkData(CustomNetworkMessage message) { }

        // Prioritizes GroupID defined in 'ExampleGroup'.
        // MessageID is retrieved from 'CustomNetworkMessage' class.
        [AdvancedMessage(typeof(ExampleGroup), typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(Message message) { }



        // Attaches this handler to a specified 'ExampleMod'.
        // MessageID and GroupID is retrieved from 'CustomNetworkMessage' class in method parameters.
        [AdvancedMessage(typeof(ExampleMod))]
        public static void ReceiveChunkData(CustomNetworkMessage message) { }

        // Attaches this handler to a specified 'ExampleMod'.
        // Prioritizes GroupID defined in 'ExampleGroup'.
        // Defines MessageID in attribute (for prototyping)
        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup), 0u)] // '0u' -> (uint)0 -> (ushort)0 -> [MessageID]
        public static void ReceiveChunkData(Message message) { }

        // Attaches this handler to a specified 'ExampleMod'.
        // Prioritizes GroupID defined in 'ExampleGroup'.
        // MessageID is retrieved from 'CustomNetworkMessage' class.
        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup), typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(Message message) { }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Server-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Important points (same as above):

        // Here:
        // - For method, Riptide.Message is automatically read to 'CustomNetworkMessage'.
        // - MessageID and GroupID is retrieved from 'CustomNetworkMessage' class in method parameters.
        // - MessageID is defined automatically.
        // - Defaults to ModID (0) internally.
        [AdvancedMessage] public static void ReceiveChunkData(ushort clientID, CustomNetworkMessage message) { }


        // Here:
        // - For method, raw Riptide.Message is provided instead.
        // - MessageID and GroupID is retrieved from 'CustomNetworkMessage' class in method parameters.
        // - MessageID is defined automatically.
        // - Defaults to ModID (0) internally.
        [AdvancedMessage(typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(ushort clientID, Message message) { }


        // - You can provide types in attribute in any order:
        // - But this is the most unoptimized ordering.
        [AdvancedMessage(typeof(CustomNetworkMessage), typeof(ExampleMod), typeof(ExampleGroup))]
        public static void ReceiveChunkData(ushort clientID, Message message) { }


        // - But Riptide.Toolkit predicts (Mod) -> (Group) -> (Message).
        // - Any other ordering will make reflection slower.
        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup), typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(ushort clientID, Message message) { }


        // - Even if one type is missing, Riptide.Toolkit predicts (Mod) -> (Group) -> (Message).
        [AdvancedMessage(typeof(ExampleMod), typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(ushort clientID, Message message) { }


        // Here:
        // - ModID is defined by custom class.
        // - typeof(Mod) need to have ModIDAttribute attached to any static field / static property.
        // - GroupID & MessageID can be provided manually.
        // - MessageID is defined using '0u' (ushort) to differentiate it from GroupID '0' (byte)
        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup), 0u)]
        public static void ReceiveChunkData(ushort clientID, Message message) { }





        // Most combinations (same as above):
        // Invalid handler - no way to identify it.
        [AdvancedMessage]
        public static void ReceiveChunkData(ushort clientID, Message message) { }

        // Defines GroupID manually in attribute.
        // MessageID is retrieved from 'CustomNetworkMessage' class in method parameters.
        // Will automatically read Riptide.Message to 'CustomNetworkMessage'
        [AdvancedMessage(0)] // '0' -> (byte)0 -> [GroupID]
        public static void ReceiveChunkData(ushort clientID, CustomNetworkMessage message) { }

        // Defines GroupID & MessageID manually.
        // Will automatically read Riptide.Message to 'CustomNetworkMessage'
        [AdvancedMessage(0, 0)]
        public static void ReceiveChunkData(ushort clientID, Message message) { }

        // Prioritizes GroupID defined in 'ExampleGroup' in attribute.
        // MessageID is retrieved from 'CustomNetworkMessage' class in method parameters.
        // Will automatically read Riptide.Message to 'CustomNetworkMessage'
        [AdvancedMessage(typeof(ExampleGroup))]
        public static void ReceiveChunkData(ushort clientID, CustomNetworkMessage message) { }

        // Prioritizes GroupID defined in 'ExampleGroup'.
        // MessageID is retrieved from 'CustomNetworkMessage' class.
        [AdvancedMessage(typeof(ExampleGroup), typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(ushort clientID, Message message) { }



        // Attaches this handler to a specified 'ExampleMod'.
        // MessageID and GroupID is retrieved from 'CustomNetworkMessage' class in method parameters.
        [AdvancedMessage(typeof(ExampleMod))]
        public static void ReceiveChunkData(ushort clientID, CustomNetworkMessage message) { }

        // Attaches this handler to a specified 'ExampleMod'.
        // Prioritizes GroupID defined in 'ExampleGroup'.
        // Defines MessageID in attribute (for prototyping)
        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup), 0u)] // '0u' -> (uint)0 -> (ushort)0 -> [MessageID]
        public static void ReceiveChunkData(ushort clientID, Message message) { }

        // Attaches this handler to a specified 'ExampleMod'.
        // Prioritizes GroupID defined in 'ExampleGroup'.
        // MessageID is retrieved from 'CustomNetworkMessage' class.
        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup), typeof(CustomNetworkMessage))]
        public static void ReceiveChunkData(ushort clientID, Message message) { }
    }
}
