using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WorkshopsGov.Services
{
    public class ViteManifestService
    {
        private readonly Dictionary<string, ManifestEntry> _manifest;
        private readonly string _baseUrl;

        public ViteManifestService(IWebHostEnvironment environment)
        {
            // Actualización de la ruta para incluir la carpeta .vite
            var manifestPath = Path.Combine(environment.WebRootPath, "vue-apps", ".vite", "manifest.json");
            
            if (File.Exists(manifestPath))
            {
                var manifestJson = File.ReadAllText(manifestPath);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                _manifest = JsonSerializer.Deserialize<Dictionary<string, ManifestEntry>>(manifestJson, options)  
                            ?? new Dictionary<string, ManifestEntry>();
            }
            else
            {
                _manifest = new Dictionary<string, ManifestEntry>();
                Console.WriteLine($"Advertencia: No se encontró el archivo de manifest en {manifestPath}");
            }
            
            _baseUrl = "/vue-apps/";
        }

        public string GetAssetPath(string name)
        {
            // Busca la entrada directamente o con diferentes formatos de ruta
            if (_manifest.TryGetValue(name, out var entry))
            {
                return _baseUrl + entry.File;
            }
            
            // Busca con el formato que usa Vite para entradas múltiples
            string assetKey = $"src/pages/{name}/index.js";
            if (_manifest.TryGetValue(assetKey, out entry))
            {
                return _baseUrl + entry.File;
            }
            
            // Intenta otros patrones comunes
            foreach (var key in _manifest.Keys)
            {
                if (key.Contains(name) && _manifest[key].IsEntry)
                {
                    return _baseUrl + _manifest[key].File;
                }
            }
            
            // Fallback path si no se encuentra en el manifest
            return _baseUrl + $"assets/{name}/{name}.js";
        }
        
        public string GetStylePath(string name)
        {
            // Verifica si la clave existe exactamente como la esperas
            string assetKey = $"src/pages/{name}/index.js";
            
            if (_manifest.TryGetValue(assetKey, out var entry) && entry.Css.Count > 0)
            {
                return _baseUrl + entry.Css[0];
            }
    
            // Intenta buscar cualquier entrada que contenga el nombre y tenga CSS
            foreach (var pair in _manifest)
            {
                if (pair.Key.Contains(name) && pair.Value.Css.Count > 0)
                {
                    return _baseUrl + pair.Value.Css[0];
                }
            }
    
            // Para debugging, puedes imprimir las claves disponibles
            Console.WriteLine($"No se encontró CSS para {name}. Claves disponibles:");
            foreach (var key in _manifest.Keys)
            {
                Console.WriteLine($"- {key}");
            }
    
            // Fallback path si no se encuentra un CSS
            return _baseUrl + $"assets/{name}/{name}.css";
        }
    }

    public class ManifestEntry
    {
        public string File { get; set; } = string.Empty;
        public List<string> Css { get; set; } = new();
        public List<string> Assets { get; set; } = new();
        public bool IsEntry { get; set; }
        public string Src { get; set; } = string.Empty;
    }

    // Extensión para registrar el servicio
    public static class ViteManifestServiceExtensions
    {
        public static IServiceCollection AddViteManifest(this IServiceCollection services)
        {
            return services.AddSingleton<ViteManifestService>();
        }
    }
}