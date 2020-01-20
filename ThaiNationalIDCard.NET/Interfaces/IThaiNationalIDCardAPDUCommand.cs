namespace ThaiNationalIDCard.NET.Interfaces
{
    public interface IThaiNationalIDCardAPDUCommand
    {
        byte[] GetResponse();
        byte[] Select(byte[] command);
        byte[] MinistryOfInteriorAppletCommand { get; }
        byte[] CitizenIDCommand { get; }
        byte[] PersonalInfoCommand { get; }
        byte[] AddressInfoCommand { get; }
        byte[] CardIssueExpireCommand { get; }
        byte[] CardIssuerCommand { get; }
        byte[][] PhotoCommand { get; }
    }
}
