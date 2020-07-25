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
    public static class PdfHelloWorld
    {
        [FunctionName("PdfHelloWorld")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var html = @"
            <html>
                <head>
                    <style>
                        body {
                            background-image: url(https://rotativa.io/Content/assets/img/machine.svg);
                            background-repeat: repeat-y;
                            background-position: center;
                            background-attachment: fixed;
                            background-size: 100%;
                        }
                    </style>
                </head>
                <body>
                    Hello World
                </body>
            </html>";

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
