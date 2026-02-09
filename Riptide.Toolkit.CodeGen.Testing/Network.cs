using Riptide.Toolkit.Messages;
using Riptide.Toolkit.Storage;

namespace Riptide.Toolkit.CodeGen.Testing
{
    [S1]
    public sealed partial class TestMessage : NetworkMessage<TestMessage>
    {
        // TODO: Introduce static identification.
        public ushort ClientID;
        public uint RoomID;
        public float PositionX;
        public float PositionY;
    }

    public sealed partial class FlagBlock : NetworkMessage<FlagBlock>
    {
        public uint FlagID;
    }
}
