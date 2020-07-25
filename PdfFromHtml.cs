using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DinkToPdf;

namespace PdfFactory
{
    public static class PdfFromHtml
    {
        [FunctionName("PdfFromHtml")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

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
                        HtmlContent = requestBody,
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
