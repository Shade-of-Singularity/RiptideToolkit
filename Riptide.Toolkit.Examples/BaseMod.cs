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
    /// Custom base mod implementation.
    /// </summary>
    /// <remarks>
    /// From Dark: I understand that it's inconvenient that you have to manually create your own modding class,
    ///  but we cannot standardize how base class for mods looks like, nor should we.
    ///  This is up to a game developer, and as such - up to you.
    ///  You can copy this class as a base class for your mods, or you can create your own, but:
    /// <para>
    /// All mod classes should contain one static field or static property of a type <see cref="ushort"/> with <see cref="ModIDAttribute"/>.
    /// </para>
    /// <para>
    /// Generic base class is not mandatory, BUT as long as you can guarantee that all mods define ModID themselves, manually.
    /// </para>
    /// </remarks>
    public abstract class BaseMod<T> : IDirectModAccess where T : BaseMod<T>, new()
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static readonly T Instance = new();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public bool IsRegisteredMessages { get; private set; }
        public bool IsInitialized { get; private set; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [ModID] public static readonly ushort ModID = NetworkIndex.NextModID();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Should register all message handlers you want.
        /// </summary>
        /// <remarks>
        /// Runs before <see cref="Initialize"/> method,
        /// because I recommend that all handlers should be initialized right after Assemblies are loaded.
        /// </remarks>
        protected abstract void RegisterMessages();

        /// <summary>
        /// Initializes a mod itself - loads-in textures, constructs classes, etc.
        /// Should run after <see cref="RegisterMessages"/>.
        /// </summary>
        /// <remarks>
        /// In this example project, this method is not used.
        /// It just serves as an example of how base mod can be implemented.
        /// </remarks>
        protected abstract void Initialize();

        /// <summary>
        /// Can unload any resources used by mod, serialize/save files and settings, etc.
        /// </summary>
        /// <remarks>
        /// In this example project, this method is not used.
        /// It just serves as an example of how base mod can be implemented.
        /// </remarks>
        protected abstract void Unload();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        void IDirectModAccess.InvokeRegisterMessages()
        {
            if (!IsRegisteredMessages)
            {
                try
                {
                    RegisterMessages();
                }
                catch (Exception ex)
                {
                    RiptideLogger.Log(LogType.Error, $"Method '{nameof(RegisterMessages)}' of type {GetType().Name} exited with exception:\n{ex}");
                }

                IsRegisteredMessages = true;
            }
        }

        void IDirectModAccess.InvokeInitialize()
        {
            if (!IsInitialized)
            {
                try
                {
                    Initialize();
                }
                catch (Exception ex)
                {
                    RiptideLogger.Log(LogType.Error, $"Method '{nameof(Initialize)}' of type {GetType().Name} exited with exception:\n{ex}");
                }

                IsInitialized = true;
            }
        }

        void IDirectModAccess.InvokeUnload()
        {
            if (IsInitialized)
            {
                try
                {
                    Unload();
                }
                catch (Exception ex)
                {
                    RiptideLogger.Log(LogType.Error, $"Method '{nameof(Unload)}' of type {GetType().Name} exited with exception:\n{ex}");
                }

                IsInitialized = false;
            }
        }
    }

    /// <summary>
    /// Provides access to starting major mod events, such as loading and unloading.
    /// </summary>
    /// <remarks>
    /// Has internal keyword, to make sure mod developers cannot access methods, which should only be invoked by the core of the game.
    /// </remarks>
    internal interface IDirectModAccess
    {

        /// <inheritdoc cref="BaseMod{T}.RegisterMessages"/>
        void InvokeRegisterMessages();

        /// <inheritdoc cref="BaseMod{T}.Initialize"/>
        void InvokeInitialize();

        /// <inheritdoc cref="BaseMod{T}.Unload"/>
        void InvokeUnload();
    }
}
