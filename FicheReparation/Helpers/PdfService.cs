using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FicheReparation.Helpers
{
    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdfFromView(string viewName, object model, ControllerContext controllerContext)
        {
            var html = RenderViewToString(viewName, model, controllerContext);
            var pdf = ConvertHtmlToPdf(html);
            return pdf;
        }

        private string RenderViewToString(string viewName, object model, ControllerContext controllerContext)
        {
            var viewEngine = controllerContext.HttpContext.RequestServices.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            var tempDataProvider = controllerContext.HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            var actionContext = new ActionContext(controllerContext.HttpContext, controllerContext.RouteData, controllerContext.ActionDescriptor);
            var viewResult = viewEngine.FindView(actionContext, viewName, false);

            // Créez un ViewDataDictionary à partir du ViewContext
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), controllerContext.ModelState);
            viewDataDictionary.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewContext = new ViewContext(actionContext, viewResult.View, viewDataDictionary, new TempDataDictionary(controllerContext.HttpContext, tempDataProvider), stringWriter, new HtmlHelperOptions());
                viewResult.View.RenderAsync(viewContext).Wait();
                return stringWriter.ToString();
            }
        }

        private byte[] ConvertHtmlToPdf(string html)
        {
            var pdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = { ColorMode = ColorMode.Color, Orientation = Orientation.Portrait, PaperSize = PaperKind.A4 },
                Objects = { new ObjectSettings { HtmlContent = html, WebSettings = new WebSettings { DefaultEncoding = "utf-8" } } }
            };
            return _converter.Convert(pdfDocument);
        }
    }
}
