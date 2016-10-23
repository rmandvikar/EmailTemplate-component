using System.Net.Mail;

namespace rm.EmailTemplateManager
{
	/// <summary>
	/// Wrapper class.
	/// </summary>
	public class EmailTemplateManagerWrapper
	{
		public IEmailTemplateFetcher EmailTemplateFetcher { get; private set; }
		public IEmailTokenReplacer EmailTokenReplacer { get; private set; }
		public EmailTemplateManagerWrapper(
			IEmailTemplateFetcher emailTemplateFetcher,
			IEmailTokenReplacer emailTokenReplacer
			)
		{
			this.EmailTemplateFetcher = emailTemplateFetcher;
			this.EmailTokenReplacer = emailTokenReplacer;
		}

		/// <summary>
		/// Replace tokens in <paramref name="EmailTemplateType"/> template using 
		/// values from arguments.
		/// </summary>
		public MailMessage Replace(EmailTemplateType emailTemplateType, params object[] args)
		{
			var message = EmailTokenReplacer.CreateMessage(
				EmailTemplateFetcher.GetBy(emailTemplateType), args
				);
			return message;
		}
	}
}
