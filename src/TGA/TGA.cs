using System;

namespace BinarySerializer.Image
{
    public class TGA : BinarySerializable
    {
        public RGBColorOrder Pre_ColorOrder { get; set; } // Set before serializing
        public bool Pre_SkipHeader { get; set; } // Set before serializing

        public TGA_Header Header { get; set; }
        public SerializableColor[] RGBImageData { get; set; }
        public TGA_Footer Footer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (!Pre_SkipHeader)
                Header = s.SerializeObject<TGA_Header>(Header, name: nameof(Header));
            
            RGBImageData = Header.ImageType switch
            {
                TGA_ImageType.UnmappedRGB => s.SerializeIntoArray<SerializableColor>(
                    obj: RGBImageData,
                    count: Header.Width * Header.Height, 
                    serializeFunc: Header.BitsPerPixel switch
                    {
                        24 when Pre_ColorOrder is RGBColorOrder.RGB => BytewiseColor.RGB888,
                        24 when Pre_ColorOrder is RGBColorOrder.BGR => BytewiseColor.BGR888,
                        32 when Pre_ColorOrder is RGBColorOrder.RGB => BytewiseColor.RGBA8888,
                        32 when Pre_ColorOrder is RGBColorOrder.BGR => BytewiseColor.BGRA8888,
                        _ => throw new NotImplementedException($"Not implemented support for textures with type {Header.ImageType} with bpp {Header.BitsPerPixel}")
                    }, 
                    name: nameof(RGBImageData)),
                _ => throw new NotImplementedException($"Not implemented support for textures with type {Header.ImageType}")
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