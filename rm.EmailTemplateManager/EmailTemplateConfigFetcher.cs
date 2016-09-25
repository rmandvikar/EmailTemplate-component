using System;
using System.Linq;
using System.Xml.Linq;

namespace rm.EmailTemplateManager
{
    /// <summary>
    /// Fetches email templates from config file.
    /// </summary>
    public class EmailTemplateConfigFetcher : IEmailTemplateFetcher
    {
        /// <summary>
        /// Get <paramref name="EmailTemplate"/> as per <paramref name="EmailTemplateType"/>.
        /// </summary>
        public EmailTemplate GetBy(EmailTemplateType emailTemplateType)
        {
            var root = XElement.Load("EmailTemplates.config");
            var template = root.Elements("emailTemplate")
                .Where(x => x.Attribute("id").Value == emailTemplateType.ToString())
                .SingleOrDefault();
            if (template == null)
            {
                throw new ApplicationException($"{emailTemplateType} missing in EmailTemplates.config.");
            }
            return Convert(template);
        }
        private EmailTemplate Convert(XElement template)
        {
            return new EmailTemplate
            (
                template.Attribute("id").Value,
                template.Element("from").Value,
                template.Element("to").Value,
                template.Element("subject").Value,
                template.Element("body").Value
            );
        }
    }
}
