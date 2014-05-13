using System;
using System.Net.Mail;
using NUnit.Framework;
using rm.EmailTemplateManager;

namespace rm.EmailTemplateManagerTest
{
    [TestFixture]
    public class EmailTemplateManagerWrapperTest
    {
        EmailTemplateManagerWrapper wrapper1 = new EmailTemplateManagerWrapper(
            new EmailTemplateConfigFetcher(), new EmailTokenReplacer(
                new string[] { "{!Data.Secret}" }
            ));
        EmailTemplateManagerWrapper wrapper2 = new EmailTemplateManagerWrapper(
            new EmailTemplateConfigFetcher(), new EmailTokenReplacer()
            );
        Data data = new Data()
        {
            Email = "richard@parker.com",
            Secret = "What is the secret of island?",
            Value1 = "Richard",
            Value2 = "Parker",
        };
        User user = new User()
        {
            Value = "Pi"
        };

        [Test(Description = "Replace multiple tokens of one type.")]
        public void Replace_validtokens_01()
        {
            var message1 = wrapper1.Replace(EmailTemplateType.Template1, data);
            Assert.NotNull(message1);
            Assert.AreEqual("So many victimized Richard Parkers had to mean something.", message1.Body);
        }

        [Test(Description = "Fail on replacing invalid tokens.")]
        public void Replace_invalidtokens_01()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                var message2 = wrapper1.Replace(EmailTemplateType.Template2, data);
            });
        }

        [Test(Description = "Does not fail on replacing repeated args.")]
        public void Replace_args_repeated_01()
        {
            Assert.DoesNotThrow(() =>
            {
                var message1 = wrapper1.Replace(EmailTemplateType.Template1, data, data);
            });
        }

        [Test(Description = "Replace by not specifying invalid tokens.")]
        public void Replace_ALLvalidtokens_01()
        {
            var message2 = wrapper2.Replace(EmailTemplateType.Template2, data);
            Assert.AreEqual("So many victimized RichardParkerParkers had to mean something. What is the secret of island?", message2.Body);
            Assert.NotNull(message2);
        }

        [Test(Description = "Fail on replacing with argument missing a property.")]
        public void Replace_ALLvalidtokens_02()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                var message2 = wrapper2.Replace(EmailTemplateType.Template4, data, user);
            });
        }

        [Test(Description = "Replace multiple types.")]
        public void Replace_ALLvalidtokens_03()
        {
            var message2 = wrapper2.Replace(EmailTemplateType.Template3, data, user);
            Assert.AreEqual("So many victimized RichardParkerParkers had to mean something. What is the secret of island? Pi.", message2.Body);
            Assert.NotNull(message2);
        }

        [Test(Description = "Replace with primitive types.")]
        public void Replace_primitivetype_01()
        {
            var message2 = wrapper2.Replace(EmailTemplateType.Template4, data, user, new { Book = "lifeOfPi" }, new { Author = "martel" });
            Assert.AreEqual("So many victimized Richard Parkers had to mean something. #martel #lifeOfPi", message2.Body);
            Assert.NotNull(message2);
        }
    }
}
