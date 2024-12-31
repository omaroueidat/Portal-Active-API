using MailerSendNetCore.Common.Interfaces;
using MailerSendNetCore.Emails;
using MailerSendNetCore.Emails.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Email
{
    public class EmailSender
    {
        private readonly IConfiguration _config;
        private readonly IMailerSendEmailClient _mailerSendEmailClient;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, IMailerSendEmailClient mailerSendEmailClient, ILogger<EmailSender> logger)
        {
            _config = config;
            _mailerSendEmailClient = mailerSendEmailClient;
            _logger = logger;
        }

        public async Task<string> SendEmail(string templateId, string senderName, string senderEmail, string[] to, string subject, IDictionary<string, string>? variables, CancellationToken cancellationToken = default)
        {
            var parameters = new MailerSendEmailParameters();
            parameters
                .WithTemplateId(templateId)
                .WithFrom(senderEmail, senderName)
                .WithTo(to)
                .WithSubject(subject);

            if (variables is { Count: > 0 })
            {
                foreach (var recipient in to)
                {
                    parameters.WithPersonalization(recipient, variables);
                }
            }

            var response = await _mailerSendEmailClient.SendEmailAsync(parameters, cancellationToken);
            if (response is { Errors.Count: > 0 })
            {
                // Do nothing for now
                _logger.LogError(response.Message);
            }

            return response.MessageId;
        }
    }
}
