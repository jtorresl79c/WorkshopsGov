using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Hosting;
using WorkshopsGov.Services;

namespace WorkshopsGov.TagHelpers
{
    [HtmlTargetElement("vite-script", TagStructure = TagStructure.WithoutEndTag)]
    public class ViteScriptTagHelper : TagHelper
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ViteManifestService _manifestService;

        public ViteScriptTagHelper(
            IWebHostEnvironment environment,
            ViteManifestService manifestService)
        {
            _environment = environment;
            _manifestService = manifestService;
        }

        [HtmlAttributeName("entry")]
        public string Entry { get; set; } = string.Empty;

        [HtmlAttributeName("dev-server")]
        public string DevServer { get; set; } = "http://localhost:5173";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("type", "module");

            if (_environment.IsDevelopment())
            {
                // Modo desarrollo - apunta al servidor de desarrollo de Vite
                output.Attributes.SetAttribute("src", $"{DevServer}/src/pages/{Entry}/index.js");
            }
            else
            {
                // Modo producción - usa el manifest para obtener la ruta correcta
                var src = _manifestService.GetAssetPath(Entry);
                output.Attributes.SetAttribute("src", src);
            }
        }
    }

    [HtmlTargetElement("vite-style", TagStructure = TagStructure.WithoutEndTag)]
    public class ViteStyleTagHelper : TagHelper
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ViteManifestService _manifestService;

        public ViteStyleTagHelper(
            IWebHostEnvironment environment,
            ViteManifestService manifestService)
        {
            _environment = environment;
            _manifestService = manifestService;
        }

        [HtmlAttributeName("entry")]
        public string Entry { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Solo en producción necesitamos cargar CSS explícitamente
            if (_environment.IsDevelopment())
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "link";
            output.TagMode = TagMode.StartTagOnly;
            output.Attributes.SetAttribute("rel", "stylesheet");
            output.Attributes.SetAttribute("href", _manifestService.GetStylePath(Entry));
        }
    }
}