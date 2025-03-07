using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class DiagnosticSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            var foundUser = context.Users.FirstOrDefault();
            var newDiagnostic1 = new Diagnostic
            {
                Id = 1,
                MemoNumber = "MEMO-001",
                DiagnosticDate = DateTime.UtcNow,
                CheckInTime = new TimeSpan(8, 30, 0),
                OperatorName = "Juan Pérez",
                ApplicationUserId = foundUser.Id,
                DiagnosticServiceStatusId = 1,
                VehicleId = 1,
                DepartmentId = 1,
                ExternalWorkshopBranchId = 1,
                DistanceUnit = "km",
                DistanceValue = 15234.5f,
                FuelLevel = 75.5f,
                FailureReport = "El motor hace un ruido extraño.",
                VehicleFailureObservation = "Posible problema con la transmisión.",
                Repairs = "Revisión del sistema de transmisión.",
                DiagnosticStatusId = 1,
                MechanicName = "Carlos Gómez",
                DiagnosticPartId = 1,
                CompletionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow 
            };

            
            if (!context.Diagnostics.Any())
            {
                context.Diagnostics.AddRange(
                    newDiagnostic1
                );
                context.SaveChanges();
            }
        }
    }
}