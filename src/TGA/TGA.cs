using System;

namespace BinarySerializer.Image
{
    public class TGA : BinarySerializable
    {
        public RGBColorOrder Pre_ColorOrder { get; set; } // Set before serializing
        public bool Pre_SkipHeader { get; set; } // Set before serializing

        public TGA_Header Header { get; set; }
        public BaseColor[] RGBImageData { get; set; }
        public TGA_Footer Footer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (!Pre_SkipHeader)
                Header = s.SerializeObject<TGA_Header>(Header, name: nameof(Header));
            
            RGBImageData = Header.ImageType switch
            {
                TGA_ImageType.UnmappedRGB => (Header.BitsPerPixel switch
                {
                    24 => Pre_ColorOrder == RGBColorOrder.RGB
                        ? (BaseColor[]) s.SerializeObjectArray<RGB888Color>((RGB888Color[]) RGBImageData,
                            Header.Width * Header.Height, name: nameof(RGBImageData))
                        : s.SerializeObjectArray<BGR888Color>((BGR888Color[]) RGBImageData,
                            Header.Width * Header.Height, name: nameof(RGBImageData)),
                    32 => Pre_ColorOrder == RGBColorOrder.RGB
                        ? (BaseColor[]) s.SerializeObjectArray<RGBA8888Color>((RGBA8888Color[]) RGBImageData,
                            Header.Width * Header.Height, name: nameof(RGBImageData))
                        : s.SerializeObjectArray<BGRA8888Color>((BGRA8888Color[]) RGBImageData,
                            Header.Width * Header.Height, name: nameof(RGBImageData)),
                    _ => throw new NotImplementedException(
                        $"Not implemented support for textures with type {Header.ImageType} with bpp {Header.BitsPerPixel}")
                }),
                _ => throw new NotImplementedException(
                    $"Not implemented support for textures with type {Header.ImageType}")
            };
        }

        public void SerializeFooter(SerializerObject s)
        {
            Footer = s.SerializeObject<TGA_Footer>(Footer, name: nameof(Footer));
        }

        public enum RGBColorOrder
        {
            RGB,
            BGR
        }
    }
}