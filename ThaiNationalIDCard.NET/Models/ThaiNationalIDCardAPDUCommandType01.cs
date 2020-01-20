namespace ThaiNationalIDCard.NET.Models
{
    public class ThaiNationalIDCardAPDUCommandType01 : ThaiNationalIDCardAPDUCommand
    {
        public override byte[] GetResponse()
        {
            return new byte[] { 0x00, 0xc0, 0x00, 0x01 };
        }
    }
}
