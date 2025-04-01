using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders
{
    public static class InspectionSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            var foundUser = context.Users.FirstOrDefault();
            if (foundUser == null) return;

            if (!context.Inspections.Any())
            {
                var rng = new Random();
                var dateBase = DateTime.UtcNow;

                // Distribución personalizada de inspecciones por vehículo
                var vehicleInspectionMap = new Dictionary<int, int>
                {
                    { 1, 6 },
                    { 2, 5 },
                    { 3, 5 },
                    { 4, 4 },
                    { 5, 3 },
                    { 6, 2 },
                    { 7, 2 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 }
                };

                var inspections = new List<Inspection>();
                int memoCounter = 1;

                foreach (var entry in vehicleInspectionMap)
                {
                    int vehicleId = entry.Key;
                    int count = entry.Value;

                    for (int i = 0; i < count; i++)
                    {
                        inspections.Add(new Inspection
                        {
                            MemoNumber = $"MEMO-{memoCounter:000}",
                            InspectionDate = dateBase.AddDays(-memoCounter),
                            CheckInTime = new TimeSpan(8 + rng.Next(0, 2), rng.Next(0, 60), 0),
                            OperatorName = $"Operador {memoCounter}",
                            ApplicationUserId = foundUser.Id,
                            InspectionServiceId = 1,
                            VehicleId = vehicleId,
                            DepartmentId = ((vehicleId - 1) % 3) + 1,
                            ExternalWorkshopBranchId = 1,
                            DistanceUnit = "km",
                            DistanceValue = 1000 + (float)(rng.NextDouble() * 9000),
                            FuelLevel = (float)(rng.NextDouble() * 100),
                            FailureReport = "Falla aleatoria generada.",
                            VehicleFailureObservation = "Observación generada.",
                            TowRequired = rng.Next(0, 2) == 1,
                            InspectionStatusId = 1,
                            Diagnostic = "Diagnóstico automático.",
                            Active = true,
                            CreatedAt = dateBase.AddDays(-memoCounter),
                            UpdatedAt = dateBase.AddDays(-memoCounter)
                        });

                        memoCounter++;
                    }
                }

                context.Inspections.AddRange(inspections);
                context.SaveChanges();
            }
        }
    }
}