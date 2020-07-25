using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DinkToPdf;

namespace PdfFactory
{
    public static class PdfFromUrl
    {
        [FunctionName("PdfFromUrl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string url = req.Query["url"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            url = url ?? data?.url;

            var client = new HttpClient();
            var html = await client.GetStringAsync(url);

            var converter = new BasicConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings =
                        {
                            DefaultEncoding = "utf-8"
                        },
                        HeaderSettings =
                        {
                            Left = "Top-Left",
                            Right = "Top-Right",
                            Center = "Top"
                        },
                        FooterSettings =
                        {
                            Left = "Bottom-Left",
                            Right = "Bottom-Right",
                            Center = "Bottom"
                        }
                    }
                }
            };

            log.LogInformation("Convert begin.");

            var buffer = converter.Convert(doc);

            log.LogInformation($"Convert end. {buffer.Length}");

            return new FileStreamResult(new MemoryStream(buffer), "application/pdf");
        }
    }
}
