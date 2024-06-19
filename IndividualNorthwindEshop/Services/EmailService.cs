using System;
using RestSharp;
using RestSharp.Authenticators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailService : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<RestResponse> SendSimpleMessageAsync(string toEmail, string subject, string text)
    {
        var client = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri("https://api.eu.mailgun.net/v3"),
            Authenticator = new HttpBasicAuthenticator("api", _configuration["Mailgun:ApiKey"])
        });
        _logger.LogInformation(_configuration["Mailgun:ApiKey"]);
        _logger.LogInformation(_configuration["Mailgun:Domain"]);
        var request = new RestRequest("{domain}/messages", Method.Post);
        request.AddParameter("domain", _configuration["Mailgun:Domain"], ParameterType.UrlSegment);
        request.AddParameter("from", $"Excited User <mailgun@{_configuration["Mailgun:Domain"]}>");
        request.AddParameter("to", toEmail);
        request.AddParameter("subject", subject);
        request.AddParameter("text", text);

        try
        {
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                _logger.LogInformation("Email successfully sent to {toEmail} with subject {subject}", toEmail, subject);
            }
            else
            {
                _logger.LogError("Failed to send email to {toEmail}. StatusCode: {StatusCode}, Content: {Content}", toEmail, response.StatusCode, response.Content);
                _logger.LogDebug("Response Headers: {Headers}", response.Headers);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending email to {toEmail}", toEmail);
            throw;
        }
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await SendSimpleMessageAsync(email, subject, htmlMessage);
    }
}

