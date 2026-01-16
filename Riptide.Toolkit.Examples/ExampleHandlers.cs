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

using Riptide.Utils;
using System;

namespace Riptide.Toolkit.Examples
{
    /// <summary>
    /// Contains client-side and server-side message handlers, in almost all possible variations.
    /// </summary>
    public static class ExampleHandlers
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Client-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// 
        /// </summary>
        [AdvancedMessage(0, 0)]
        public static void RequestHandler(Message message)
        {
            RiptideLogger.Log(LogType.Warning, $"Fired client-side {nameof(RequestHandler)}");
        }

        [AdvancedMessage(typeof(ExampleMod), typeof(ExampleGroup))]
        public static void ReceiveInventory(ReceiveInventory inventory)
        {
            RiptideLogger.Log(LogType.Warning, $"Inventory received!");
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Server-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// 
        /// </summary>
        [AdvancedMessage(0, 0)]
        public static void RequestHandler(ushort client, Message message)
        {
            RiptideLogger.Log(LogType.Warning, $"Fired server-side {nameof(RequestHandler)} from client ({client})");
        }
    }
}
