namespace ThaiNationalIDCard.NET.Models
{
    class ThaiNationalIDCardAPDUCommandType02 : ThaiNationalIDCardAPDUCommand
    {
        public override byte[] GetResponse()
        {
            return new byte[] { 0x00, 0xc0, 0x00, 0x00 };
        }
    }
}
