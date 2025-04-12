using Microsoft.AspNetCore.Mvc;
using SCG_ReportAPI.Models.Common;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace SCG_ReportAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new IndexModel
            {
                IP = GetLocalIPAddress(),
                AssemblyName = Assembly.GetEntryAssembly()?.GetName().Name,
                AssemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString(),
                AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                DateModified = GetDateModified()
            };

            return View(model);
        }

        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                return ip?.ToString() ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        private string GetDateModified()
        {
            try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                var fileInfo = new FileInfo(location);
                return fileInfo.LastWriteTime.ToString("dd/MM/yyyy : HH:mm:ss");
            }
            catch
            {
                return "N/A";
            }
        }
    }
}
