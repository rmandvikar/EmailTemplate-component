using System.Collections.Generic;
using NUnit.Framework;
using rm.EmailTemplateManager;

namespace rm.EmailTemplateManagerTest
{
    [TestFixture]
    public class EmailTokenReplacerTest
    {
        EmailTokenReplacer emailTokenReplacer1 = null;

        [Test]
        [TestCase("{!atype.aproperty}", 1, "atype", "aproperty", "", "")]
        [TestCase("{!.aproperty}", 1, "", "aproperty", "", "")]
        [TestCase("{!at.}", 0, "", "", "", "")]
        [TestCase("{!atype.aproperty} {!a.aproperty}", 2, "atype", "aproperty", "a", "aproperty")]
        [TestCase("{!aa.bb}{!yy.xx}", 2, "aa", "bb", "yy", "xx")]
        [TestCase("me@domain.com", 0, "", "", "", "")]
        [TestCase("This is a test email body for {!Data.Secret}{!Data.Value1}.", 2, "Data", "Secret", "Data", "Value1")]
        public void Parse(string input, int count,
            string type1, string prop1, string type2, string prop2)
        {
            emailTokenReplacer1 = new EmailTokenReplacer(
                new string[] { "{!Data.Secret}" }
                );
            var tokens1 = emailTokenReplacer1.GetTokens(input);
            Assert.AreEqual(count, tokens1.Count);
            Verify(tokens1, type1, prop1, type2, prop2);
        }

        private void Verify(ISet<EmailToken> tokens,
            string type1, string prop1, string type2, string prop2)
        {
            if (tokens.Count >= 0)
            {
                int i = 0;
                foreach (var token in tokens)
                {
                    switch (i)
                    {
                        case 0:
                            Assert.AreEqual(type1, token.TypeName);
                            Assert.AreEqual(prop1, token.PropertyName);
                            break;
                        case 1:
                            Assert.AreEqual(type2, token.TypeName);
                            Assert.AreEqual(prop2, token.PropertyName);
                            break;
                        default:
                            break;
                    }
                    i++;
                }
            }
        }
    }
}
