namespace rm.EmailTemplateManager
{
    /// <summary>
    /// Holds email template values.
    /// </summary>
    public class EmailTemplate
    {
        public string Id { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public EmailTemplate(string id, string from, string to, string subject, string body)
        {
            Id = id;
            From = from;
            To = to;
            Subject = subject;
            Body = body;
        }
    }
}
