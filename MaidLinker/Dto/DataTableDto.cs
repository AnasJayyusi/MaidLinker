namespace MaidLinker.Dto
{
    public class DataTableDto
    {
        public string ServiceCode { get; set; }
        public string ServiceDesc { get; set; }
        public int Qty { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
        public double VatPercentage { get; set; }
        public double VatValue { get; set; }
        public double NetWithVat { get; set; }
    }

    public class ServiceProviderDataTableDto
    {
        public string ServiceCode { get; set; }
        public string ServiceDesc { get; set; }
        public int Qty { get; set; }
        public double ServiceFee { get; set; }
        public double Total { get; set; }
    }


    public class BeneficiaryDataTableDto
    {
        public string ServiceCode { get; set; }
        public string ServiceDesc { get; set; }
        public int Qty { get; set; }
        public double BeneficiaryPart { get; set; }
        public double Total { get; set; }
        public double PlatformFee { get; set; }
        public double VatPercentage { get; set; }
        public double TotalPlatformFee { get; set; }
        public double NetDrPart { get; set; }
    }
}
