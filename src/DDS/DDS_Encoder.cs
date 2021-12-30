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

        public void DecodeStream(Stream input, Stream output)
        {
            using var r = new Reader(input, leaveOpen: true);
            var buffer = DDS_Parser.DecompressData(r, Header, Width, Height);
            output.Write(buffer, 0, buffer.Length);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            throw new NotImplementedException();
        }
    }
}