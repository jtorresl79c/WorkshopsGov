using WorkshopsGov.Data;

namespace WorkshopsGov.Seeders
{
    public static class InspectionVehicleFailureSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.VehicleFailures.Any()) return;
            var inspection = context.Inspections.FirstOrDefault();
            var vehicleFailure = context.VehicleFailures.FirstOrDefault();
            
            if (inspection != null && vehicleFailure != null)
            {
                inspection.VehicleFailures.Add(vehicleFailure);
                context.SaveChanges();
            }
        }
    }
}