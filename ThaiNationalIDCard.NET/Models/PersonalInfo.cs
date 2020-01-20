namespace ThaiNationalIDCard.NET.Models
{
    public class PersonalInfo
    {
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public PersonalInfo(string personalInfo)
        {
            string[] infos = personalInfo.Split('#');

            Prefix = infos[0].Trim();
            FirstName = infos[1].Trim();
            MiddleName = infos[2].Trim();
            LastName = infos[3].Trim();
        }

        public override string ToString()
        {
            return string.Format($"{Prefix}{FirstName} {LastName}");
        }
    }
}
