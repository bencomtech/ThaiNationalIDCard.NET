namespace ThaiNationalIDCard.NET.Models
{
    public class PersonalPhoto : Personal
    {
        public string Photo { get; set; }

        public PersonalPhoto(Personal personal)
        {
            CitizenID = personal.CitizenID;
            ThaiPersonalInfo = personal.ThaiPersonalInfo;
            EnglishPersonalInfo = personal.EnglishPersonalInfo;
            DateOfBirth = personal.DateOfBirth;
            Sex = personal.Sex;
            AddressInfo = personal.AddressInfo;
            IssueDate = personal.IssueDate;
            ExpireDate = personal.ExpireDate;
            Issuer = personal.Issuer;
        }
    }
}
