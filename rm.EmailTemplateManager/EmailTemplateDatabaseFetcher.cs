using System;

namespace rm.EmailTemplateManager
{
    /// <summary>
    /// Fetches templates from database. 
    /// </summary>
    /// <remarks>example.</remarks>
    public class EmailTemplateDatabaseFetcher : IEmailTemplateFetcher
    {
        /// <summary>
        /// Get <paramref name="EmailTemplate"/> as per <paramref name="EmailTemplateType"/>.
        /// </summary>
        public EmailTemplate GetBy(EmailTemplateType emailTemplateType)
        {
            // implementation goes here.
            throw new NotImplementedException();
        }
    }
}
