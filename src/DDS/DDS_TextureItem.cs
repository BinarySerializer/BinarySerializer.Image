namespace BinarySerializer.Image
{
    public class DDS_TextureItem : BinarySerializable
    {
        // Set before serializing
        public DDS_Header Header { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }

        public byte[] ImageData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoEncoded(new DDS_Encoder(Header, Width, Height), () =>
            {
                ImageData = s.SerializeArray<byte>(ImageData, s.CurrentLength, name: nameof(ImageData));
            });
        }
    }
}