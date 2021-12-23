using System;
using System.IO;

namespace BinarySerializer.Image
{
    public class DDS_Encoder : IStreamEncoder
    {
        public DDS_Encoder(DDS_Header header, uint width, uint height)
        {
            Header = header;
            Width = width;
            Height = height;
        }

        public string Name => $"{Header.PixelFormat.FourCC}";
        public DDS_Header Header { get; }
        public uint Width { get; }
        public uint Height { get; }

        public Stream DecodeStream(Stream s)
        {
            return new MemoryStream(DDS_Parser.DecompressData(new Reader(s), Header, Width, Height));
        }

        public Stream EncodeStream(Stream s)
        {
            throw new NotImplementedException();
        }
    }
}