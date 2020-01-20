namespace ThaiNationalIDCard.NET.Models
{
    public class AddressInfo
    {
        public string HouseNo { get; set; }
        public string VillageNo { get; set; }
        public string Lane { get; set; }
        public string Road { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }

        public AddressInfo(string addressInfo)
        {
            string[] infos = addressInfo.Split('#');

            HouseNo = infos[0].Trim();
            VillageNo = infos[1].Trim();
            Lane = infos[2].Trim();
            Road = infos[3].Trim();
            SubDistrict = infos[5].Trim();
            District = infos[6].Trim();
            Province = infos[7].Trim();
        }

        public override string ToString()
        {
            string address = HouseNo;

            if (!string.IsNullOrEmpty(VillageNo))
                address += string.Format($" ม.{VillageNo}");

            if (!string.IsNullOrEmpty(Lane))
                address += string.Format($" ซ.{Lane}");

            if (!string.IsNullOrEmpty(Road))
                address += string.Format($" ถ.{Road}");

            if (!string.IsNullOrEmpty(SubDistrict))
                address += string.Format($" ต.{SubDistrict}");

            if (!string.IsNullOrEmpty(District))
                address += string.Format($" อ.{District}");

            if (!string.IsNullOrEmpty(Province))
                address += string.Format($" จ.{Province}");

            return address;
        }
    }
}
