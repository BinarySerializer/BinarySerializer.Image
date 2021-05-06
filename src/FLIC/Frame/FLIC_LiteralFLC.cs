namespace BinarySerializer.Image
{
    public class FLIC_LiteralFLC : BinarySerializable
    {
        public FLIC Pre_Flic { get; set; } // Set before serializing

        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ImgData = s.SerializeArray<byte>(ImgData, Pre_Flic.Width * Pre_Flic.Height, name: nameof(ImgData));
        }
    }
}