using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace RockyUtility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public MailJetSettings MailJetSettings { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            MailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(MailJetSettings.ApiKey, MailJetSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.Messages, new JArray
            {
                new JObject
                {
                    {
                        "From",
                        new JObject
                        {
                            {"Email", "vedran@milkovic.dev"},
                            {"Name", "Vedran"}
                        }
                    },
                    {
                        "To",
                        new JArray
                        {
                            new JObject
                            {
                                {"Email", email},
                                {"Name", "Vedran"}
                            }
                        }
                    },
                    {"Subject", subject},
                    {"HTMLPart", body }
                }
            });

            await client.PostAsync(request);
        }
    }
}