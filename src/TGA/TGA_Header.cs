namespace BinarySerializer.Image
{
    public class TGA_Header : BinarySerializable
    {
        public bool Pre_ForceNoColorMap { get; set; } // Set before serializing

        public byte IdentificationFieldLength { get; set; }
        public bool HasColorMap { get; set; }
        public TGA_ImageType ImageType { get; set; }

        // Color Map
        public ushort ColorMapOrigin { get; set; }
        public ushort ColorMapLength { get; set; }
        public byte ColorMapEntrySize { get; set; } // Number of bits

        // Image
        public ushort OriginX { get; set; }
        public ushort OriginY { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte BitsPerPixel { get; set; }
        public byte AttributeBitsCount { get; set; } // Determines how many bits are used for alpha
        public byte Reserved { get; set; }
        public TGA_Origin OriginPoint { get; set; }
        public TGA_Interleaving InterleavingFlag { get; set; }

        public byte[] IdentificationField { get; set; }
        public byte[] ColorMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            IdentificationFieldLength = s.Serialize<byte>(IdentificationFieldLength, name: nameof(IdentificationFieldLength));
            HasColorMap = s.Serialize<bool>(HasColorMap, name: nameof(HasColorMap));
            ImageType = s.Serialize<TGA_ImageType>(ImageType, name: nameof(ImageType));

            ColorMapOrigin = s.Serialize<ushort>(ColorMapOrigin, name: nameof(ColorMapOrigin));
            ColorMapLength = s.Serialize<ushort>(ColorMapLength, name: nameof(ColorMapLength));
            ColorMapEntrySize = s.Serialize<byte>(ColorMapEntrySize, name: nameof(ColorMapEntrySize));

            OriginX = s.Serialize<ushort>(OriginX, name: nameof(OriginX));
            OriginY = s.Serialize<ushort>(OriginY, name: nameof(OriginY));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            BitsPerPixel = s.Serialize<byte>(BitsPerPixel, name: nameof(BitsPerPixel));
            s.DoBits<byte>(b =>
            {
                AttributeBitsCount = b.SerializeBits<byte>(AttributeBitsCount, 4, name: nameof(AttributeBitsCount));
                Reserved = b.SerializeBits<byte>(Reserved, 1, name: nameof(Reserved));
                OriginPoint = b.SerializeBits<TGA_Origin>(OriginPoint, 1, name: nameof(OriginPoint));
                InterleavingFlag = b.SerializeBits<TGA_Interleaving>(InterleavingFlag, 2, name: nameof(InterleavingFlag));
            });

            IdentificationField = s.SerializeArray<byte>(IdentificationField, IdentificationFieldLength, name: nameof(IdentificationField));
            ColorMap = s.SerializeArray<byte>(ColorMap, !Pre_ForceNoColorMap ? (ColorMapLength * (ColorMapEntrySize / 8)) : 0, name: nameof(ColorMap));
        }
    }
}