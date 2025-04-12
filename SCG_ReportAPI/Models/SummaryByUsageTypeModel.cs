namespace SCG_ReportAPI.Models
{
    public class SummaryByUsageTypeModel
    {
        public string PartNumber { get; set; }

        public int PM { get; set; }
        public int CM { get; set; }
        public int Project { get; set; }

        public double PercentPM { get; set; }
        public double PercentCM { get; set; }
        public double PercentProject { get; set; }

        public int Total => PM + CM + Project;
    }
}
