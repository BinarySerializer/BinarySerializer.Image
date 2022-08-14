using System.IO;
using System.Linq;

namespace BinarySerializer.Image
{
    public class DDS : BinarySerializable
    {
        public bool Pre_SkipHeader { get; set; } // Set before serializing

        public DDS_Header Header { get; set; }
        public DDS_TextureFace[] Faces { get; set; }

        public DDS_MipSurface PrimaryTexture => Faces?.FirstOrDefault()?.Surfaces?.FirstOrDefault();

        public override void SerializeImpl(SerializerObject s)
        {
            if (!Pre_SkipHeader)
                Header = s.SerializeObject<DDS_Header>(Header, name: nameof(Header));

            var texturesCount = 1;

            // TODO: Improve this - a cubemap doesn't require all surfaces to be available
            if (Header.Caps.HasFlag(DDS_Header.DDS_CapsFlags.DDS_SURFACE_FLAGS_CUBEMAP) && 
                Header.Caps2.HasFlag(DDS_Header.DDS_Caps2Flags.DDSCAPS2_CUBEMAP))
                texturesCount = 6;

            Faces = s.SerializeObjectArray(Faces, texturesCount, x => x.Pre_Header = Header, name: nameof(Faces));
        }

		public static DDS FromRawData(byte[] data, DDS_Parser.PixelFormat pixelFormat, uint width, uint height)
        {
			if (data == null)
				return null;

			var dds = new DDS()
			{
				Header = new DDS_Header
				{
					PixelFormat = DDS_PixelFormat.GetDefaultPixelFormat(pixelFormat),
					Height = height,
					Width = width
				}
			};
            if (pixelFormat == DDS_Parser.PixelFormat.DXT5n) {
                pixelFormat = DDS_Parser.PixelFormat.DXT5;
            }

            using var memStream = new MemoryStream(data);
            using var reader = new Reader(memStream);

            dds.Faces = new DDS_TextureFace[]
            {
                new DDS_TextureFace()
                {
                    Pre_Header = dds.Header,
                    Surfaces = new DDS_MipSurface[]
                    {
                        new DDS_MipSurface()
                        {
                            ImageData = DDS_Parser.DecompressData(dds.Header, reader, pixelFormat, width, height),
                            Pre_Header = dds.Header,
                            Pre_Width = width,
                            Pre_Height = height
                        }
                    }
                }
            };

            return dds;
        }
    }
}