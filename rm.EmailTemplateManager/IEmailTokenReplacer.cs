using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace rm.EmailTemplateManager
{
	/// <summary>
	/// Defines methods to replace tokens.
	/// </summary>
	public interface IEmailTokenReplacer
	{
		/// <summary>
		/// Create mail message from <paramref name="EmailTemplate"/>.
		/// </summary>
		MailMessage CreateMessage(EmailTemplate emailTemplate, params object[] args);
	}
}
