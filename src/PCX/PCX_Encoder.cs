using System.IO;

namespace BinarySerializer.Image
{
    public class PCX_Encoder : IStreamEncoder
    {
        public PCX_Encoder(int decodedLength)
        {
            DecodedLength = decodedLength;
        }

        public int DecodedLength { get; }
        public string Name => "PCX_RLE";

        public void DecodeStream(Stream input, Stream output)
        {
            using Reader reader = new Reader(input, leaveOpen: true);

            // Keep track of the index
            int index = 0;

            // Create the buffer for the decoded data
            byte[] decodedBuffer = new byte[DecodedLength];

            do
            {
                // Read the next byte
                byte b = reader.ReadByte();

                int repeatCount;
                byte runValue;

                // Check if it should be repeated
                if ((b & 0xC0) == 0xC0)
                {
                    repeatCount = b & 0x3F;
                    runValue = reader.ReadByte();
                }
                else
                {
                    repeatCount = 1;
                    runValue = b;
                }

                // Write the specified number of bytes
                while (index < decodedBuffer.Length && repeatCount > 0)
                {
                    decodedBuffer[index] = runValue;
                    repeatCount--;
                    index++;
                }

            } while (index < decodedBuffer.Length);

            output.Write(decodedBuffer, 0, decodedBuffer.Length);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            // Read the bytes to be encoded
            byte[] buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length); 
            
            using Writer writer = new Writer(output, leaveOpen: true);

            for (int i = 0; i < buffer.Length; i++)
            {
                byte b = buffer[i];

                // Repeat if the byte has the top two bits set or else we can't write it normally
                bool shouldRepeat = (b & 0xC0) == 0xC0;

                // Also repeat if the next 2 bytes matches this one
                if (!shouldRepeat &&
                    i + 2 < buffer.Length &&
                    b == buffer[i + 1] &&
                    b == buffer[i + 2])
                    shouldRepeat = true;

                if (shouldRepeat)
                {
                    // Get the value to repeat
                    byte runValue = b;

                    // Keep track of how many times we repeat
                    int repeatCount = 0;

                    // Check each value until we break
                    while (i < buffer.Length)
                    {
                        // Get the byte
                        b = buffer[i];

                        // Make sure it's still equal to the value and we haven't reached the maximum value
                        if (b != runValue || repeatCount >= 0x3F)
                            break;

                        // Increment the index and count
                        i++;
                        repeatCount++;
                    }

                    // Decrement the index since it gets incremented after
                    i--;

                    // Write the repeat byte
                    output.WriteByte((byte)(repeatCount | 0xC0));

                    // Write the value to repeat
                    output.WriteByte(runValue);
                }
                else
                {
                    // Write the byte
                    output.WriteByte(b);
                }
            }
        }
    }
}