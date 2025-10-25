namespace ChoreQuest.API.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string username, string resetToken);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string username, string resetToken)
    {
        // In development, log the reset link to console
        // In production, this would use an actual email service (SendGrid, AWS SES, etc.)
        
        var resetUrl = $"http://localhost:4200/reset-password?token={resetToken}";
        
        _logger.LogInformation(
            "Password reset requested for user: {Username} ({Email})\n" +
            "Reset link: {ResetUrl}\n" +
            "Token: {Token}",
            username, toEmail, resetUrl, resetToken);
        
        // Simulate async email sending
        await Task.Delay(100);
        
        // TODO: In production, implement actual email sending:
        // Example with SendGrid:
        // var client = new SendGridClient(_configuration["SendGrid:ApiKey"]);
        // var msg = MailHelper.CreateSingleEmail(
        //     new EmailAddress(_configuration["SendGrid:FromEmail"], "ChoreQuest"),
        //     new EmailAddress(toEmail, username),
        //     "Reset Your ChoreQuest Password",
        //     $"Click here to reset your password: {resetUrl}",
        //     $"<p>Click <a href='{resetUrl}'>here</a> to reset your password.</p>");
        // await client.SendEmailAsync(msg);
    }
}
