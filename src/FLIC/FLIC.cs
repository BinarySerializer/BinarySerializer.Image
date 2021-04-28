namespace BinarySerializer.Image
{
    // https://www.drdobbs.com/windows/the-flic-file-format/184408954
    // https://github.com/aseprite/flic/blob/main/decoder.cpp
    public class FLIC : BinarySerializable
    {
        public uint FileSize { get; set; }
        public FLIC_Format FormatType { get; set; }
        public ushort FramesCount { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ushort BitsPerPixel { get; set; }
        public ushort Flags { get; set; }
        public uint Speed { get; set; }
        public ushort Ushort_14 { get; set; }
        public uint CreationDate { get; set; }
        public uint CreationProgramSerialNum { get; set; }
        public uint UpdatedDate { get; set; }
        public uint UpdatedProgramSerialNum { get; set; }
        public ushort AspectX { get; set; }
        public ushort AspectY { get; set; }
        public byte[] Reserved_0 { get; set; }
        public Pointer FirstFramePointer { get; set; }
        public Pointer SecondFramePointer { get; set; }
        public byte[] Reserved_1 { get; set; }

        public FLIC_PrimaryChunk[] Chunks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
            FormatType = s.Serialize<FLIC_Format>(FormatType, name: nameof(FormatType));

            if (FormatType != FLIC_Format.FLC)
                throw new BinarySerializableException(this, $"The FLIC format {FormatType} is not supported");

            FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            if (FileSize != 12)
            {
                BitsPerPixel = s.Serialize<ushort>(BitsPerPixel, name: nameof(BitsPerPixel));
                Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
                Speed = s.Serialize<uint>(Speed, name: nameof(Speed));
                Ushort_14 = s.Serialize<ushort>(Ushort_14, name: nameof(Ushort_14));
                CreationDate = s.Serialize<uint>(CreationDate, name: nameof(CreationDate));
                CreationProgramSerialNum = s.Serialize<uint>(CreationProgramSerialNum, name: nameof(CreationProgramSerialNum));
                UpdatedDate = s.Serialize<uint>(UpdatedDate, name: nameof(UpdatedDate));
                UpdatedProgramSerialNum = s.Serialize<uint>(UpdatedProgramSerialNum, name: nameof(UpdatedProgramSerialNum));
                AspectX = s.Serialize<ushort>(AspectX, name: nameof(AspectX));
                AspectY = s.Serialize<ushort>(AspectY, name: nameof(AspectY));
                Reserved_0 = s.SerializeArray<byte>(Reserved_0, 38, name: nameof(Reserved_0));
                FirstFramePointer = s.SerializePointer(FirstFramePointer, name: nameof(FirstFramePointer), allowInvalid: true);
                SecondFramePointer = s.SerializePointer(SecondFramePointer, name: nameof(SecondFramePointer), allowInvalid: true);
                Reserved_1 = s.SerializeArray<byte>(Reserved_1, 40, name: nameof(Reserved_1));
            }
            else
            {
                Speed = 30;
                FileSize = s.CurrentLength;
            }

            Chunks = s.SerializeObjectArrayUntil<FLIC_PrimaryChunk>(Chunks, x => s.CurrentPointer.FileOffset >= Offset.FileOffset + FileSize, includeLastObj: true, onPreSerialize: x => x.Flic = this, name: nameof(Chunks));
        }
    }
}