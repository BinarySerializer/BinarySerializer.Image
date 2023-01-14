namespace BinarySerializer.Image
{
    public class TGA_Footer : BinarySerializable
    {
        public uint ExtensionOffset { get; set; }
        public uint DeveloperAreaOffset { get; set; }
        public string Signature { get; set; } = "TRUEVISION-XFILE.";

        public override void SerializeImpl(SerializerObject s)
        {
            ExtensionOffset = s.Serialize<uint>(ExtensionOffset, name: nameof(ExtensionOffset));
            DeveloperAreaOffset = s.Serialize<uint>(DeveloperAreaOffset, name: nameof(DeveloperAreaOffset));
            Signature = s.SerializeString(Signature, length: 18, name: nameof(Signature));
        }
    }
}