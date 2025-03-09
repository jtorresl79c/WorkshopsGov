using WorkshopsGov.Data;

namespace WorkshopsGov.Seeders
{
    public static class DiagnosticVehicleFailureSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.VehicleFailures.Any()) return;
            var diagnostic = context.Diagnostics.FirstOrDefault();
            var vehicleFailure = context.VehicleFailures.FirstOrDefault();
            
            if (diagnostic != null && vehicleFailure != null)
            {
                diagnostic.VehicleFailures.Add(vehicleFailure);
                context.SaveChanges();
            }
        }
    }
}