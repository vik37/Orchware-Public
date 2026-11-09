using MimeKit;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Models;
using MailKit.Net.Smtp;

namespace Orchware.Notification.API.Services;

public class EmailSender : INotifySender
{
	private readonly IConfiguration _config;

	public EmailSender(IConfiguration config)
	{
		_config = config;
	}

	public async Task SendEmailAsync(EmailModel model)
	{
		var emailConfig = _config.GetSection("EmailConfig");

		var message = new MimeMessage();

		var fromAddress = !string.IsNullOrEmpty(model.From) ? model.From : emailConfig["From"];
		message.From.Add(MailboxAddress.Parse(fromAddress));

		message.To.Add(MailboxAddress.Parse(model.To));

		message.Subject = model.Subject;

		var bodyBuilder = new BodyBuilder
		{
			HtmlBody = model.Body,
			TextBody = string.IsNullOrEmpty(model.TextBody) ? StripHtml(model.Body) : model.TextBody
		};
		message.Body = bodyBuilder.ToMessageBody();

		if (model.CC != null && model.CC.Count > 0)
		{
			foreach (var address in model.CC)
				message.Cc.Add(MailboxAddress.Parse(address));
		}

		if (model.BCC != null && model.BCC.Count > 0)
		{
			foreach (var address in model.BCC)
				message.Bcc.Add(MailboxAddress.Parse(address));
		}

		if (model.AttachmentData != null)
		{
			bodyBuilder.Attachments.Add(model.AttachmentFileName, model.AttachmentData);
		}

		using var client = new SmtpClient();

		// DEV ONLY: skip SSL validation if needed
		client.ServerCertificateValidationCallback = (s, c, h, e) => true;

		await client.ConnectAsync(emailConfig["Host"], int.Parse(emailConfig["Port"]),  MailKit.Security.SecureSocketOptions.StartTls);

		// Authenticate
		await client.AuthenticateAsync(emailConfig["Username"], emailConfig["Password"]);

		// Send
		await client.SendAsync(message);

		await client.DisconnectAsync(true);
	}

	// Simple HTML stripper for text fallback
	private string StripHtml(string html)
	{
		var array = new char[html.Length];
		int arrayIndex = 0;
		bool inside = false;

		foreach (char let in html)
		{
			if (let == '<')
			{
				inside = true;
				continue;
			}
			if (let == '>')
			{
				inside = false;
				continue;
			}
			if (!inside)
			{
				array[arrayIndex] = let;
				arrayIndex++;
			}
		}
		return new string(array, 0, arrayIndex);
	}
}
