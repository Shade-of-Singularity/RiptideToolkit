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

using System;

namespace Riptide.Toolkit.Settings
{
    /// <summary>
    /// Initialization settings for <see cref="Toolkit"/> systems.
    /// </summary>
    /// <remarks>
    /// Most of the settings here can only be set before starting any client or server.
    /// It is recommended to set those settings somewhere around application initialization.
    /// </remarks>
    public static class Initialization
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const string DoCollectionResetFlag = "-riptide.initialization.do-collection-reset";
        public const string ForceMessageHandlerRegistrationFlag = "-riptide.initialization.force-message-handler-registration";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether to do collection resets during <see cref="NetworkIndex.Initialize"/>.
        /// </summary>
        public static bool DoCollectionResetOnInitialization
        {
            get => m_DoCollectionResetOnInitialization;
            set
            {
                if (m_DoCollectionResetOnInitialization == value) return;
                if (NetworkIndex.IsEverInitialized)
                {
                    throw new Exception(
                        $"Cannot modify performance settings after {nameof(NetworkIndex)} initialization! Set performance options on app launch.");
                }

                m_DoCollectionResetOnInitialization = value;
            }
        }

        /// <summary>
        /// Whether to forcefully register <see cref="Toolkit.Messages.NetworkMessage"/>s even if their MessageID was set.
        /// Might help during reloading, but can break things if you don't know what you are doing.
        /// </summary>
        //public static bool ForceMessageHandlerRegistration
        //{
        //    get => m_ForceMessageHandlerRegistration;
        //    set
        //    {
        //        if (m_ForceMessageHandlerRegistration == value) return;
        //        if (NetworkIndex.IsEverInitialized)
        //        {
        //            throw new Exception(
        //                $"Cannot modify performance settings after {nameof(NetworkIndex)} initialization! Set performance options on app launch.");
        //        }

        //        m_ForceMessageHandlerRegistration = value;
        //    }
        //}




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static bool m_DoCollectionResetOnInitialization = ConsoleArgs.Has(DoCollectionResetFlag);
        //private static bool m_ForceMessageHandlerRegistration = ConsoleArgs.Has(ForceMessageHandlerRegistrationFlag);
    }
}
