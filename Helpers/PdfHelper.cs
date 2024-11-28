using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RazorEngine;
using RazorEngine.Templating;


namespace CoreSystem.Helpers
{
    public class PdfHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PdfHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public enum Template
        {
            OrderConfirmation
        }

        public enum PageSizes
        {
            A4,
            A0,
            A1,
            A2,
            A3,
            A5,
            Legal
        }

        public enum OutputFormats
        {
            pdf,
            jpg,
            png,
            bmp,
            svg
        }


        public string ApiKey { get; set; }
        public string Value { get; set; }
        public string FooterHtml { get; set; }

        public string ApiUrl { get; set; } // http://api.html2pdfrocket.com/pdf

        public int? MarginLeft { get; set; }
        public int? MarginRight { get; set; }
        public int? MarginTop { get; set; }
        public int? MarginBottom { get; set; }

        public bool? UseGrayscale { get; set; }
        public bool? UseLandscape { get; set; } 
        public bool? DisableJavascript { get; set; }
        public bool? JavascriptDelay { get; set; }
        public int? Dpi { get; set; }

        public PageSizes PageSize { get; set; }

        public PdfHelper()
        {
            ApiKey = ConfigurationManager.AppSettings["html2PdfApiKey"];
            ApiUrl = ConfigurationManager.AppSettings["html2PdfApiUrl"];
        }

        public byte[] Print<T>(T model, Template template)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            var templatePath = Path.Combine(contentRootPath + "\"~/Views/Pdf/\"" + template + ".cshtml");
            var footerPath = Path.Combine(contentRootPath + "\"~/Views/Pdf/\"" + "Footer.cshtml");

            string templateString = File.ReadAllText(templatePath);
            string footerString = File.ReadAllText(footerPath);


            var parsedTemplate = Engine.Razor.IsTemplateCached("__pdfOrderConfirmation___", null) 
                ? Engine.Razor.Run("__pdfOrderConfirmation___", null, model) 
                : Engine.Razor.RunCompile(templateString, "__pdfOrderConfirmation___", null, model);


            //var parsedFooter = Engine.Razor.IsTemplateCached("__pdfFooter_", null) 
            //    ? Engine.Razor.Run("__pdfFooter_", null, model) 
            //    : Engine.Razor.RunCompile(footerString, "__pdfFooter_", null, model);
            
            Value = parsedTemplate;
           // FooterHtml = parsedFooter;

            var pdfBytes = GetBytes();

            return pdfBytes;
        }

        private byte[] GetBytes()
        {
            if (string.IsNullOrWhiteSpace(ApiKey)
                || string.IsNullOrWhiteSpace(Value)
                || string.IsNullOrWhiteSpace(ApiUrl))
                throw new ArgumentNullException();

            using (var client = new WebClient())
            {
                NameValueCollection options = new NameValueCollection();

                options.Add("apikey", ApiKey);
                options.Add("value", Value);
                options.Add("PageSize", PageSize.ToString());
                options.Add("LowQuality", "false");
                //options.Add("DisableShrinking", "true");

                if (MarginLeft != null)
                    options.Add("MarginLeft", MarginLeft.ToString());

                if (MarginRight != null)
                    options.Add("MarginRight", MarginRight.ToString());

                if (MarginTop != null)
                    options.Add("MarginTop", MarginTop.ToString());

                if (MarginBottom != null)
                    options.Add("MarginBottom", MarginBottom.ToString());

                if (UseGrayscale != null)
                    options.Add("UseGrayscale", UseGrayscale.ToString());

                if (UseLandscape != null)
                    options.Add("UseLandscape", UseLandscape.ToString());

                if (DisableJavascript != null)
                    options.Add("DisableJavascript", DisableJavascript.ToString());

                if (JavascriptDelay != null)
                    options.Add("JavascriptDelay", JavascriptDelay.ToString());

                if (Dpi != null)
                    options.Add("Dpi", Dpi.ToString());

                if (FooterHtml != null)
                    options.Add("FooterHtml", FooterHtml);



                byte[] result = client.UploadValues(ApiUrl, options);

                return result;
            }
        }


        
    }
}