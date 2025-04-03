using WorkshopsGov.Data;
using WorkshopsGov.Models;

namespace WorkshopsGov.Seeders;

public static class RequestServiceSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        var foundUser = context.Users.FirstOrDefault();
        if (foundUser == null) return;

        var vehicleIds = context.Vehicles.Select(v => v.Id).ToList();
        var inspectionIds = context.Inspections.Select(i => i.Id).ToList();

        if (!context.RequestServices.Any() && vehicleIds.Any() && inspectionIds.Count >= 10)
        {
            var rng = new Random();
            var dateBase = DateTime.UtcNow;
            var services = new List<RequestService>();

            for (int i = 0; i < 10; i++)
            {
                var selectedInspections = inspectionIds
                    .OrderBy(_ => rng.Next())
                    .Take(3)
                    .Select(id => context.Inspections.Find(id)!)
                    .ToList();

                var requestService = new RequestService
                {
                    Folio = $"RS-{i + 1:000}",
                    ApplicationUserId = foundUser.Id,
                    VehicleId = vehicleIds[rng.Next(vehicleIds.Count)],
                    DepartmentId = ((i) % 3) + 1,
                    Description = "Solicitud de servicio generada automáticamente.",
                    ReceptionDate = DateOnly.FromDateTime(dateBase.AddDays(-i)),
                    Active = true,
                    CreatedAt = dateBase.AddDays(-i),
                    UpdatedAt = dateBase.AddDays(-i),
                    Inspections = selectedInspections,
                    Files = context.Files.Where(f => f.Id == 1 || f.Id == 2).ToList()
                };

                services.Add(requestService);
            }

            context.RequestServices.AddRange(services);
            context.SaveChanges();
        }
    }
}