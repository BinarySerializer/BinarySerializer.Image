using System;

namespace BinarySerializer.Image
{
    public class TGA : BinarySerializable
    {
        public RGBColorOrder ColorOrder { get; set; } // Set before serializing
        public bool SkipHeader { get; set; } // Set before serializing

        public TGA_Header Header { get; set; }
        public BaseColor[] RGBImageData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (!SkipHeader)
                Header = s.SerializeObject<TGA_Header>(Header, name: nameof(Header));
            
            RGBImageData = Header.ImageType switch
            {
                TGA_ImageType.UnmappedRGB => (Header.BitsPerPixel switch
                {
                    24 => ColorOrder == RGBColorOrder.RGB
                        ? (BaseColor[]) s.SerializeObjectArray<RGB888Color>((RGB888Color[]) RGBImageData,
                            Header.Width * Header.Height, name: nameof(RGBImageData))
                        : s.SerializeObjectArray<BGR888Color>((BGR888Color[]) RGBImageData,
                            Header.Width * Header.Height, name: nameof(RGBImageData)),
                    32 => ColorOrder == RGBColorOrder.RGB
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

        public enum RGBColorOrder
        {
            RGB,
            BGR
        }
    }
}