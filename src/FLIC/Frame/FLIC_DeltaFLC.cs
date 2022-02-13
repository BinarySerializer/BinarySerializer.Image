﻿using System;
using System.Linq;

namespace BinarySerializer.Image
{
    public class FLIC_DeltaFLC : BinarySerializable
    {
        public ushort LinesCount { get; set; }
        public FLIC_DeltaFLCLine[] Lines { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LinesCount = s.Serialize<ushort>(LinesCount, name: nameof(LinesCount));
            Lines = s.SerializeObjectArray<FLIC_DeltaFLCLine>(Lines, LinesCount, name: nameof(Lines));
        }

        public class FLIC_DeltaFLCLine : BinarySerializable
        {
            public FLIC_DeltaFLCLineCommand[] Commands { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Commands = s.SerializeObjectArrayUntil<FLIC_DeltaFLCLineCommand>(Commands, x => x.ValueType != 3, name: nameof(Commands));
            }

            public class FLIC_DeltaFLCLineCommand : BinarySerializable
            {
                public ushort Value { get; set; }
                public byte ValueType { get; set; }

                public byte LastValue { get; set; }
                public short Skip { get; set; }
                public ushort PacketsCount { get; set; }
                public FLIC_DeltaFLCPacket[] Packets { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    s.DoBits<ushort>(b =>
                    {
                        Value = b.SerializeBits<ushort>(Value, 14, name: nameof(Value));
                        ValueType = b.SerializeBits<byte>(ValueType, 2, name: nameof(ValueType));
                    });

                    if (ValueType == 0)
                    {
                        PacketsCount = Value;
                    }
                    else if (ValueType == 2)
                    {
                        LastValue = (byte)(Value & 0xff);
                        s.Log("{0}: {1}", nameof(LastValue), LastValue);
                        PacketsCount = s.Serialize<ushort>(PacketsCount, name: nameof(PacketsCount));
                    }
                    else if (ValueType == 3)
                    {
                        if ((Value & 0x2000) != 0)
                            Skip = (short)(Value | 0xC000);
                        else
                            Skip = (short)Value;
                        s.Log("{0}: {1}", nameof(Skip), Skip);
                        PacketsCount = 0;
                    }

                    Packets = s.SerializeObjectArray<FLIC_DeltaFLCPacket>(Packets, PacketsCount, name: nameof(Packets));
                }

                public class FLIC_DeltaFLCPacket : BinarySerializable
                {
                    public byte Skip { get; set; }
                    public sbyte Count { get; set; }
                    public byte[] ImageData { get; set; }
                    public byte RepeatValue_0 { get; set; }
                    public byte RepeatValue_1 { get; set; }

                    public override void SerializeImpl(SerializerObject s)
                    {
                        Skip = s.Serialize<byte>(Skip, name: nameof(Skip));
                        Count = s.Serialize<sbyte>(Count, name: nameof(Count));

                        if (Count >= 0)
                        {
                            ImageData = s.SerializeArray<byte>(ImageData, Count * 2, name: nameof(Count));
                        }
                        else
                        {
                            RepeatValue_0 = s.Serialize<byte>(RepeatValue_0, name: nameof(RepeatValue_0));
                            RepeatValue_1 = s.Serialize<byte>(RepeatValue_1, name: nameof(RepeatValue_1));
                            ImageData = Enumerable.Repeat(new byte[]
                            {
                                RepeatValue_0,
                                RepeatValue_1
                            }, Math.Abs(Count)).SelectMany(x => x).ToArray();
                        }
                    }
                }
            }
        }
    }
}