using Microsoft.AspNetCore.Identity.UI.Services;

namespace WorkshopsGov.Services;

public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // No hacer nada, solo para cumplir con la dependencia
        return Task.CompletedTask;
    }
}