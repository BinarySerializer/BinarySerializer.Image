namespace BinarySerializer.Image
{
    public class FLIC_Color256 : BinarySerializable
    {
        public ushort PacketsCount { get; set; }
        public FLIC_ColorPacket[] Packets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            PacketsCount = s.Serialize<ushort>(PacketsCount, name: nameof(PacketsCount));
            Packets = s.SerializeObjectArray<FLIC_ColorPacket>(Packets, PacketsCount, name: nameof(Packets));
        }

        public class FLIC_ColorPacket : BinarySerializable
        {
            public byte Skip { get; set; }
            public byte Count { get; set; }
            public SerializableColor[] Colors { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Skip = s.Serialize<byte>(Skip, name: nameof(Skip));
                Count = s.Serialize<byte>(Count, name: nameof(Count));
                Colors = s.SerializeIntoArray<SerializableColor>(Colors, Count == 0 ? 256 : Count, BytewiseColor.RGB888, name: nameof(Colors));
            }
        }
    }
}