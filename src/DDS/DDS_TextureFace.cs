namespace BinarySerializer.Image
{
    public class DDS_TextureFace : BinarySerializable
    {
        public DDS_Header Pre_Header { get; set; } // Set before serializing

        public DDS_MipSurface[] Surfaces { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Surfaces == null)
                Surfaces = new DDS_MipSurface[Pre_Header.GetMipMapCount];

            var w = Pre_Header.Width;
            var h = Pre_Header.Height;

            for (int i = 0; i < Surfaces.Length; i++)
            {
                Surfaces[i] = s.SerializeObject<DDS_MipSurface>(Surfaces[i], x =>
                {
                    x.Pre_Header = Pre_Header;
                    x.Pre_Width = w;
                    x.Pre_Height = h;

                    w /= 2;
                    h /= 2;
                }, name: $"{nameof(Surfaces)}[{i}]");
            }
        }
    }
}