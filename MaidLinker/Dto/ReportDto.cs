namespace MaidLinker.Dto
{
    public class ReportDto
    {
        public MasterDetailsDto MasterDetails { get; set; }
        public List<DataTableDto> DataTable { get; set; }
    }

    public class ServiceProviderReportDto
    {
        public MasterDetailsDto MasterDetails { get; set; }
        public List<ServiceProviderDataTableDto> DataTable { get; set; }
    }


    public class BeneficiaryReportDto
    {
        public MasterDetailsDto MasterDetails { get; set; }
        public List<BeneficiaryDataTableDto> DataTable { get; set; }
    }

    public enum ReportTypeEnum
    {
        None,
        ServiceProviderReportDto,
        BeneficiaryReportDto
    }
}
