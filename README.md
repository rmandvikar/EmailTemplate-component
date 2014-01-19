EmailTemplate-component
=======================

Replaces tokens in email template.

####Explanation:
Email Template contains tokens in format `{!TypeName.PropertyName}` for email fields as To, From, Subject, and Body. The email template replacer logic uses .Net Reflection to dynamically replace tokens with their appropriate values from the arguments. The advantage of using reflection is that the replacer logic need not change after adding additional tokens to the template; only the caller logic does.

#####Sample Template:

For a sample template below, the token `{!Data.Email}` can be added without changing email template replacer logic or caller logic. The email templates are fetched from a config file (xml based) and could be fetched from database instead. 

```xml
<emailTemplate id="Template1">
  <to>{!Data.Email}</to>
  <from>noreply@domain.com</from>
  <subject>Hello {!Data.Value1}{!Data.Value2}!</subject>
  <body>
    So many victimized {!Data.Value1} {!Data.Value2}s had to mean something. #{!.Author} #{!.Book}
  </body>
</emailTemplate>
```

#####Example:

Certain sensitive tokens (ex: `{!User.CreditCard}`) can be included by specifying a list of invalid tokens and those are not replaced. If the invalid tokens are not specified then any token is valid. 

```c#
// specifies invalid tokens
EmailTemplateManagerWrapper wrapper1 = new EmailTemplateManagerWrapper(
    new EmailTemplateConfigFetcher(), new EmailTokenReplacer(
        new string[] { "{!Data.Secret}" } 
    ));
// does NOT specify invalid tokens so any token is valid
EmailTemplateManagerWrapper wrapper2 = new EmailTemplateManagerWrapper(
    new EmailTemplateConfigFetcher(), new EmailTokenReplacer()
    );
// argument
var data = new Data()
{
	Email = "richard@parker.com",
	Secret = "What is the secret of island?",
	Value1 = "Richard",
	Value2 = "Parker",
};
// pass non-property objects as anonymous types
MailMessage message1 = wrapper1.Replace(EmailTemplateType.Template1, 
	data, new { Book = "lifeOfPi" }, new { Author = "martel" });
// message1.To: "richard@parker.com"
// message1.From: "noreply@domain.com"
// message1.Subject: "Hello RichardParker!"
// message1.Body: "So many victimized Richard Parkers had to mean something. #martel #lifeOfPi"
```
