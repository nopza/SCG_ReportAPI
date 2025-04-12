using Microsoft.AspNetCore.Mvc;
using SCG_ReportAPI.Models;

namespace SCG_ReportAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        // Ex. 1
        [HttpPost("SummaryByUsageType", Name = "SummaryByUsageType")]
        public IActionResult SummaryByUsageType([FromBody] List<UsageDataModel> request)
        {
            List<SummaryByUsageTypeModel> reportData = request
                                          .GroupBy(x => x.PartNumber)
                                          .Select(g => new SummaryByUsageTypeModel
                                          {
                                              PartNumber = g.Key,
                                              PM = g.Where(x => x.UsageType == "PM").Sum(x => x.Quantity),
                                              CM = g.Where(x => x.UsageType == "CM").Sum(x => x.Quantity),
                                              Project = g.Where(x => x.UsageType == "Project").Sum(x => x.Quantity),
                                          })
                                          .Select(x =>
                                          {
                                              int total = x.PM + x.CM + x.Project;
                                              x.PercentPM = total > 0 ? Math.Round((double)x.PM / total * 100, 2) : 0;
                                              x.PercentCM = total > 0 ? Math.Round((double)x.CM / total * 100, 2) : 0;
                                              x.PercentProject = total > 0 ? Math.Round((double)x.Project / total * 100, 2) : 0;
                                              return x;
                                          })
                                          .ToList();

            return Json(reportData);
        }

        // Ex. 2
        [HttpPost("InvalidUsageType", Name = "InvalidUsageType")]
        public IActionResult InvalidUsageType([FromBody] List<UsageDataModel> request)
        {
            List<PartTypeModel> allowedUsageList = GetPartTypeAllowedUsages();

            List<InvalidUsageTypeModel> reportData = request
                                        .GroupBy(x => x.PartNumber)
                                        .Select(g => { 
                                            string partNumber = g.Key;
                                            List<string> actualUsages = g.Select(x => x.UsageType.Trim()).Distinct().ToList();
                                        
                                            List<string> allowedUsages = allowedUsageList
                                                         .FirstOrDefault(p => p.PartNumber == partNumber)?
                                                         .AllowedUsages
                                                         .Select(x => x.Trim())
                                                         .Distinct()
                                                         .ToList() ?? new List<string>();
                                        
                                            List<string> invalidUsages = actualUsages.Where(u => !allowedUsages.Contains(u)).ToList();
                                        
                                            int totalQuantity = g.Sum(x => x.Quantity);
                                            int invalidUsageCount = g.Where(x => !allowedUsages.Contains(x.UsageType.Trim())).Sum(x => x.Quantity);
                                        
                                            double invalidUsagePercent = totalQuantity > 0 ? Math.Round((invalidUsageCount * 100.0) / totalQuantity, 2) : 0;
                                        
                                            return new InvalidUsageTypeModel
                                            {
                                                PartNumber = partNumber,
                                                AllowedUsages = allowedUsages,
                                                ActualUsages = actualUsages,
                                                InvalidUsages = invalidUsages,
                                                InvalidUsageCount = invalidUsageCount,
                                                InvalidUsagePercent = invalidUsagePercent
                                            };
                                        })
                                        .ToList();

            return Json(reportData);
        }

        // Ex. 3
        [HttpPost("AnomalyDetectionReport", Name = "AnomalyDetectionReport")]
        public IActionResult AnomalyDetectionReport([FromBody] List<UsageDataModel> request)
        {
            DateTime latestDate = DateTime.Now.Date;
            List<DateTime> last7DaysDateList = Enumerable.Range(0, 7).Select(i => latestDate.AddDays(-i)).ToList();
            List<PartTypeModel> allowedUsageList = GetPartTypeAllowedUsages();
            List<UsageDataModel> last7DayDataList = request.Where(x => last7DaysDateList.Contains(DateTime.Parse(x.Date))).ToList();

            List<AnomalyDetectionReportModel> reportData = last7DayDataList
                                              .GroupBy(x => new { x.PartNumber, x.Date })
                                              .Select(g =>
                                              {
                                                  string partNumber = g.Key.PartNumber;
                                                  string date = g.Key.Date;

                                                  List<string> allowedUsages = allowedUsageList
                                                               .FirstOrDefault(p => p.PartNumber == partNumber)?
                                                               .AllowedUsages ?? new List<string>();

                                                  int totalQuantity = g.Sum(x => x.Quantity);

                                                  int invalidQuantity = g
                                                      .Where(x => !allowedUsages.Contains(x.UsageType))
                                                      .Sum(x => x.Quantity);

                                                  double avgInvalidPerTotal = totalQuantity == 0 ? 0 : (double)invalidQuantity / totalQuantity;

                                                  return new AnomalyDetectionReportModel
                                                  {
                                                      PartNumber = partNumber,
                                                      Date = date,
                                                      Quantity = totalQuantity,
                                                      AverageQuantity = (int)Math.Round(avgInvalidPerTotal)
                                                  };
                                              })
                                              .ToList();

            return Json(reportData);
        }

        #region Private function

        private List<PartTypeModel> GetPartTypeAllowedUsages()
        {
            return new List<PartTypeModel>
            {
                new() { PartNumber = "PRT001", AllowedUsages = new List<string> { "PM" } },
                new() { PartNumber = "PRT002", AllowedUsages = new List<string> { "PM", "Project" } },
                new() { PartNumber = "PRT003", AllowedUsages = new List<string> { "CM" } },
                new() { PartNumber = "PRT004", AllowedUsages = new List<string> { "CM", "PM" } },
                new() { PartNumber = "PRT005", AllowedUsages = new List<string> { "CM" } },
                new() { PartNumber = "PRT006", AllowedUsages = new List<string> { "Project" } },
                new() { PartNumber = "PRT007", AllowedUsages = new List<string> { "PM" } },
                new() { PartNumber = "PRT008", AllowedUsages = new List<string> { "CM", "PM" } },
                new() { PartNumber = "PRT009", AllowedUsages = new List<string> { "Project" } },
                new() { PartNumber = "PRT010", AllowedUsages = new List<string> { "PM" } }
            };
        }

        #endregion
    }
}
