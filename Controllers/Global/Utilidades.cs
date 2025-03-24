using Microsoft.Extensions.Configuration;
using System.IO;
using System.Security.Claims;

namespace WorkshopsGov.Controllers.Global
{
    public static class Utilidades
    {
        private static IConfiguration _configuration;
        private static IHttpContextAccessor _httpContextAccessor;


        // Inicializar la configuración al inicio de la aplicación
        public static void Initialize(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // Acceder al valor de "PATH_MASTER" en appsettings.json
        public static string PATH_MASTER => _configuration["PATH_MASTER"] ?? "wwwroot/Formats/";
        public static int DB_ARCHIVOTIPOS_ENTREGA_RECEPCION => int.TryParse(_configuration["DB_Archivotypes:ENTREGA_RECEPCION_GENERADA"], out int value) ? value : 0;
        public static int ELIMINADO => int.TryParse(_configuration["DB_ESTADOS:ACTIVO"], out int value) ? value : 0;
        public static int DB_ARCHIVOTIPOS_ENTREGA_RECEPCION_DIGITALIZADA => int.TryParse(_configuration["DB_Archivotypes:ENTREGA_RECEPCION"], out int value) ? value : 0;
        public static int DB_ARCHIVOTIPOS_MEMO_GENERADA => int.TryParse(_configuration["DB_Archivotypes:MEMO_GENERADA"], out int value) ? value : 0;
        public static int DB_ARCHIVOTIPOS_MEMO_DIGITALIZADO => int.TryParse(_configuration["DB_Archivotypes:MEMO_DIGITALIZADO"], out int value) ? value : 0;
        public static char SEPARATOR = Path.DirectorySeparatorChar;

        public static string GetFullPathMaster()
        {
            return PATH_MASTER;
        }
        public static string GetFolderNameByFileTypeId(int fileTypeId)
        {
            return _configuration[$"FileStorage:TypeFolders:{fileTypeId}"] ?? "OTROS";
        }
        public static string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
        public static string GetFullPathInspection(int inspectionId) // \INSPECTIONS\_ID\
        {
            return PATH_MASTER + "INSPECTIONS" + SEPARATOR + "ER_" + inspectionId + SEPARATOR;
        }

        // 🔹 Método para crear directorios de inspección
        public static string CreateOrGetDirectoryInsideInspectionDirectory(string InspectionPath, string DirectoryName)
        {
            return Directory.CreateDirectory(Path.Combine(InspectionPath, DirectoryName)).FullName + Path.DirectorySeparatorChar;
        }

        public static string GetFileTypeDescription(int fileTypeId)
        {
            if (fileTypeId == DB_ARCHIVOTIPOS_ENTREGA_RECEPCION_DIGITALIZADA)
                return "Entrega-Recepción Digitalizada";

            if (fileTypeId == DB_ARCHIVOTIPOS_MEMO_DIGITALIZADO)
                return "Memo Digitalizado";

            return "Archivo";
        }

    }
}