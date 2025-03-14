using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class InspectionSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            var foundUser = context.Users.FirstOrDefault();
            var newInspection1 = new Inspection
            {
                Id = 1,
                MemoNumber = "MEMO-001",
                InspectionDate = DateTime.UtcNow,
                CheckInTime = new TimeSpan(8, 30, 0),
                OperatorName = "Juan Pérez",
                ApplicationUserId = foundUser.Id,
                InspectionServiceStatusId = 1,
                VehicleId = 1,
                DepartmentId = 1,
                ExternalWorkshopBranchId = 1,
                DistanceUnit = "km",
                DistanceValue = 15234.5f,
                FuelLevel = 75.5f,
                FailureReport = "El motor hace un ruido extraño.",
                VehicleFailureObservation = "Posible problema con la transmisión.",
                Repairs = "Revisión del sistema de transmisión.",
                InspectionStatusId = 1,
                MechanicName = "Carlos Gómez",
                InspectionPartId = 1,
                CompletionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow 
            };

            
            if (!context.Inspections.Any())
            {
                context.Inspections.AddRange(
                    newInspection1
                );
                context.SaveChanges();
            }
        }
    }
}