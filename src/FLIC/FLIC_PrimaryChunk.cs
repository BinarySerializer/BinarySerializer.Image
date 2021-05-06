namespace BinarySerializer.Image
{
    public class FLIC_PrimaryChunk : FLIC_BaseChunk
    {
        public FLIC Pre_Flic { get; set; } // Set before serializing

        // Chunk data
        public FLIC_Frame Chunk_Frame { get; set; }

        public override void SerializeChunkData(SerializerObject s)
        {
            // Prefix = 0xF100, Frame = 0xF1FA
            if (ChunkType == 0xF1FA)
                Chunk_Frame = s.SerializeObject<FLIC_Frame>(Chunk_Frame, x => x.Pre_Flic = Pre_Flic, name: nameof(Chunk_Frame));
            else
                SerializeUnknownData(s);
        }
    }
}