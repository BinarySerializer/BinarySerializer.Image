using System.IO;
using System.Linq;

namespace BinarySerializer.Image
{
    public class DDS : BinarySerializable
    {
        public bool Pre_SkipHeader { get; set; } // Set before serializing

        public DDS_Header Header { get; set; }
        public DDS_Texture[] Textures { get; set; }

        public DDS_TextureItem PrimaryTexture => Textures?.FirstOrDefault()?.Items?.FirstOrDefault();

        public override void SerializeImpl(SerializerObject s)
        {
            if (!Pre_SkipHeader)
                Header = s.SerializeObject<DDS_Header>(Header, name: nameof(Header));

            var texturesCount = 1;

            // TODO: Improve this - a cubemap doesn't require all surfaces to be available
            if (Header.Caps.HasFlag(DDS_Header.DDS_CapsFlags.DDS_SURFACE_FLAGS_CUBEMAP) && 
                Header.Caps2.HasFlag(DDS_Header.DDS_Caps2Flags.DDSCAPS2_CUBEMAP))
                texturesCount = 6;

            Textures = s.SerializeObjectArray(Textures, texturesCount, x => x.Pre_Header = Header, name: nameof(Textures));
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

            dds.Textures = new DDS_Texture[]
            {
                new DDS_Texture()
                {
                    Pre_Header = dds.Header,
                    Items = new DDS_TextureItem[]
                    {
                        new DDS_TextureItem()
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