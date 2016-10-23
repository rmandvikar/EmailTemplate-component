using System;
using NUnit.Framework;
using rm.EmailTemplateManager;

namespace rm.EmailTemplateManagerTest
{
	[TestFixture]
	public class EmailTemplateFetcherTest
	{
		IEmailTemplateFetcher emailTemplateFetcher = new EmailTemplateConfigFetcher();

		[Test]
		public void Fetch01()
		{
			var emailTemplate = emailTemplateFetcher.GetBy(EmailTemplateType.Template2);
			Assert.NotNull(emailTemplate);
			Assert.AreEqual("Template2", emailTemplate.Id);
			Assert.AreEqual("{!Data.Email}", emailTemplate.To);
			Assert.AreEqual("noreply@domain.com", emailTemplate.From);
			Assert.AreEqual("Hello {!Data.Value2}!", emailTemplate.Subject);
			Assert.AreEqual("So many victimized {!Data.Value1}{!Data.Value2}{!Data.Value2}s had to mean something. {!Data.Secret}", emailTemplate.Body);
		}
		[Test]
		public void Fetch02()
		{
			Assert.Throws<ApplicationException>(() =>
			{
				var emailTemplate = emailTemplateFetcher.GetBy(EmailTemplateType.Template5);
			});
		}
	}
}
