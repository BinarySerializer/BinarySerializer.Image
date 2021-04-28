﻿using System;

namespace BinarySerializer.Image
{
    public abstract class FLIC_BaseChunk : BinarySerializable
    {
        public uint ChunkSize { get; set; }
        public ushort ChunkType { get; set; }

        // Chunk data
        public byte[] Chunk_Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ChunkSize = s.Serialize<uint>(ChunkSize, name: nameof(ChunkSize));
            ChunkType = s.Serialize<ushort>(ChunkType, name: nameof(ChunkType));

            SerializeChunkData(s);

            var endOffset = Offset + ChunkSize;
            if (s.CurrentPointer != endOffset)
            {
                if (Math.Abs(s.CurrentPointer - Offset - ChunkSize) >= 4)
                    s.LogWarning($"Chunk size doesn't match! Type: {ChunkType}, Size: {ChunkSize}, Parsed: {s.CurrentPointer - Offset}");
                s.Goto(endOffset);
            }
        }

        public abstract void SerializeChunkData(SerializerObject s);

        public void SerializeUnknownData(SerializerObject s)
        {
            s.LogWarning($"Unknown FLIC chunk type {ChunkType} of size {ChunkSize} at {Offset}");
            Chunk_Data = s.SerializeArray<byte>(Chunk_Data, ChunkSize - 6, name: nameof(Chunk_Data));
        }
    }
}