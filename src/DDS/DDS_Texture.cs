namespace BinarySerializer.Image
{
    public class DDS_Texture : BinarySerializable
    {
        public DDS_Header Pre_Header { get; set; } // Set before serializing

        public DDS_TextureItem[] Items { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Items == null)
                Items = new DDS_TextureItem[Pre_Header.GetMipMapCount];

            var w = Pre_Header.Width;
            var h = Pre_Header.Height;

            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = s.SerializeObject<DDS_TextureItem>(Items[i], x =>
                {
                    x.Pre_Header = Pre_Header;
                    x.Pre_Width = w;
                    x.Pre_Height = h;

                    w /= 2;
                    h /= 2;
                }, name: $"{nameof(Items)}[{i}]");
            }
        }
    }
}