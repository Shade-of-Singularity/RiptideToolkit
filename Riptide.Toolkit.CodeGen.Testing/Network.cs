using Riptide.Toolkit.Messages;

namespace Riptide.Toolkit.CodeGen.Testing
{
    // TODO: Define S1 by default when CodeGen with disposal is introduced.
    public sealed partial class TestMessage : NetworkMessage<TestMessage>
    {
        // TODO: Introduce static identification.
        // TODO: Auto-implement disposal.
        public ushort ClientID;
        public uint RoomID;
        public float PositionX;
        public float PositionY;
    }

    // TODO: Define S1 by default when CodeGen with disposal is introduced.
    public sealed partial class FlagBlock : NetworkMessage<FlagBlock>
    {
        // TODO: Auto-implement disposal.
        public uint FlagID;
    }
}
