namespace BinarySerializer.Image
{
    public class DDS_MipSurface : BinarySerializable
    {
        // Set before serializing
        public DDS_Header Pre_Header { get; set; }
        public uint Pre_Width { get; set; }
        public uint Pre_Height { get; set; }

        public byte[] ImageData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoEncoded(new DDS_Encoder(Pre_Header, Pre_Width, Pre_Height), () =>
            {
                ImageData = s.SerializeArray<byte>(ImageData, s.CurrentLength, name: nameof(ImageData));
            });
        }
    }
}