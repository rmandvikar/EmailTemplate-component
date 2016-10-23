namespace rm.EmailTemplateManager
{
	/// <summary>
	/// Defines methods to fetch email templates.
	/// </summary>
	public interface IEmailTemplateFetcher
	{
		/// <summary>
		/// Get <paramref name="EmailTemplate"/> by <paramref name="EmailTemplateType"/>.
		/// </summary>
		EmailTemplate GetBy(EmailTemplateType emailTemplateType);
	}
}
