namespace SCG_ReportAPI.Models
{
    public class InvalidUsageTypeModel
    {
        public string PartNumber { get; set; }
        public List<string> AllowedUsages { get; set; }
        public List<string> ActualUsages { get; set; }
        public List<string> InvalidUsages { get; set; }
        public int InvalidUsageCount { get; set; }
        public double InvalidUsagePercent { get; set; }

    }
}
