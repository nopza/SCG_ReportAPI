namespace SCG_ReportAPI.Models
{
    public class AnomalyDetectionReportModel
    {
        public string PartNumber { get; set; }
        public string Date { get; set; }
        public int Quantity { get; set; }
        public double AverageQuantity { get; set; }

    }
}
