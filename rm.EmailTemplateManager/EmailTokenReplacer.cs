using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace rm.EmailTemplateManager
{
    /// <summary>
    /// Replaces tokens in template.
    /// </summary>
    public class EmailTokenReplacer : IEmailTokenReplacer
    {
        #region members

        /// <summary>
        /// Regex for <paramref name="EmailToken"/>.
        /// </summary>
        public const string TokenRegex = @"\{!(?<typeName>\w+)?\.(?<propertyName>\w+)\}";
        /// <summary>
        /// Invalid <paramref name="EmailToken"/> strings. A sensitive <paramref name="EmailToken"/> 
        /// (ex: {!User.CreditCard} can be included. If left empty, any <paramref name="EmailToken"/> 
        /// is valid.
        /// </summary>
        readonly string[] InvalidTokens;
        private ISet<EmailToken> invalidTokensSet = null;
        /// <summary>
        /// Set containing invalid <paramref name="EmailToken"/>s.
        /// </summary>
        private ISet<EmailToken> InvalidTokensSet
        {
            get
            {
                if (invalidTokensSet == null)
                {
                    invalidTokensSet = GetInvalidTokens(InvalidTokens);
                }
                return invalidTokensSet;
            }
        }
        /// <summary>
        /// ctor.
        /// </summary>
        public EmailTokenReplacer(string[] invalidTokens = null)
        {
            InvalidTokens = invalidTokens;
        }

        #endregion

        #region methods

        #region IEmailTokenReplacer methods

        /// <summary>
        /// Create mail message from <paramref name="EmailTemplate"/>.
        /// </summary>
        public MailMessage CreateMessage(EmailTemplate emailTemplate, params object[] args)
        {
            var typepropertyToTokensMap = GetTokenMap(emailTemplate);
            var tokenToValueMap = GetTokenToValueMap(typepropertyToTokensMap, args);
            var message = CreateMessage(emailTemplate, tokenToValueMap);
            return message;
        }

        #endregion

        /// <summary>
        /// Parse out <paramref name="EmailToken"/>s from <paramref name="EmailTemplate"/>
        /// and create a type,property->token map.
        /// </summary>
        private IDictionary<string, Dictionary<string, EmailToken>> GetTokenMap(EmailTemplate emailTemplate)
        {
            var toTokens = GetTokens(emailTemplate.To);
            var fromTokens = GetTokens(emailTemplate.From);
            var subjectTokens = GetTokens(emailTemplate.Subject);
            var bodyTokens = GetTokens(emailTemplate.Body);
            var tokens = new HashSet<EmailToken>();
            AddTokenIfValid(tokens, toTokens);
            AddTokenIfValid(tokens, fromTokens);
            AddTokenIfValid(tokens, subjectTokens);
            AddTokenIfValid(tokens, bodyTokens);
            var typepropertyToTokensMap = Convert(tokens);
            return typepropertyToTokensMap;
        }
        /// <summary>
        /// Convert a <paramref name="EmailToken"/> set to typeName,propertyName->token map.
        /// </summary>
        private IDictionary<string, Dictionary<string, EmailToken>> Convert(ISet<EmailToken> tokens)
        {
            var typepropertyToTokensMap = new Dictionary<string, Dictionary<string, EmailToken>>();
            foreach (var token in tokens)
            {
                Dictionary<string, EmailToken> propertyToTokensMap;
                typepropertyToTokensMap.TryGetValue(token.TypeName, out propertyToTokensMap);
                if (propertyToTokensMap == null)
                {
                    propertyToTokensMap = new Dictionary<string, EmailToken>();
                    typepropertyToTokensMap[token.TypeName] = propertyToTokensMap;
                }
                EmailToken emailtoken;
                propertyToTokensMap.TryGetValue(token.PropertyName, out emailtoken);
                if (emailtoken == null)
                {
                    propertyToTokensMap[token.PropertyName] = token;
                }
            }
            return typepropertyToTokensMap;
        }
        /// <summary>
        /// Create token->value map from <paramref name="EmailToken"/>s and arguments.
        /// </summary>
        /// <remarks>Replaces using reflection.</remarks>
        private IDictionary<string, string> GetTokenToValueMap(
            IDictionary<string, Dictionary<string, EmailToken>> typepropertyToTokensMap,
            params object[] args)
        {
            var tokenToValueMap = new Dictionary<string, string>();
            // first reflect on args and find a matching token for given type, property 
            // rather than using token to find matching type and property among args
            foreach (var arg in args)
            {
                // first, get propertyName->token map for typeName
                Dictionary<string, EmailToken> propertyToTokensMap;
                typepropertyToTokensMap.TryGetValue(GetTypeName(arg), out propertyToTokensMap);
                if (propertyToTokensMap != null)
                {
                    foreach (var property in arg.GetType().GetProperties())
                    {
                        // second, get token for propertyName
                        EmailToken token;
                        propertyToTokensMap.TryGetValue(property.Name, out token);
                        if (token != null)
                        {
                            // found arg's property corresponding to the token
                            var propertyValue = property.GetValue(arg);
                            tokenToValueMap.Add(token.ToString(), propertyValue.ToString());
                        }
                    }
                }
            }
            return tokenToValueMap;
        }
        /// <summary>
        /// Get arg type's name. Empty if arg is anonymous type.
        /// </summary>
        private string GetTypeName(object arg)
        {
            var type = arg.GetType();
            if (Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType
                && type.Name.StartsWith("<>") && type.Name.Contains("AnonymousType")
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic)
                return "";
            return type.Name;
        }
        /// <summary>
        /// Create mail message from <paramref name="EmailTemplate"/> and token->value map.
        /// </summary>
        private MailMessage CreateMessage(EmailTemplate emailTemplate,
            IDictionary<string, string> tokenValueMap)
        {
            var from = Replace(emailTemplate.From, tokenValueMap);
            var to = Replace(emailTemplate.To, tokenValueMap);
            var subject = Replace(emailTemplate.Subject, tokenValueMap);
            var body = Replace(emailTemplate.Body, tokenValueMap);
            var message = new MailMessage();
            message.From = new MailAddress(from);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            return message;
        }
        /// <summary>
        /// Add tokens from <param name="otherSet"></param> only if valid. 
        /// If invalid set is empty, then any <paramref name="EmailToken"/> is valid.
        /// </summary>
        private void AddTokenIfValid(ISet<EmailToken> tokens, ISet<EmailToken> otherSet)
        {
            foreach (var token in otherSet)
            {
                if (!InvalidTokensSet.Contains(token))
                {
                    tokens.Add(token);
                }
            }
        }
        /// <summary>
        /// Get <paramref name="EmailToken"/>s from input.
        /// </summary>
        internal ISet<EmailToken> GetTokens(string input)
        {
            var tokens = new HashSet<EmailToken>();
            var matches = Regex.Matches(input, TokenRegex);
            foreach (Match match in matches)
            {
                AddTokenIfMatched(tokens, match);
            }
            return tokens;
        }
        /// <summary>
        /// If match found, add <paramref name="EmailToken"/>s to set.
        /// </summary>
        private static void AddTokenIfMatched(ISet<EmailToken> tokens, Match match)
        {
            if (match.Success)
            {
                var typeName = match.Groups["typeName"].Value;
                var propertyName = match.Groups["propertyName"].Value;
                var token = new EmailToken(typeName, propertyName);
                tokens.Add(token);
            }
        }
        /// <summary>
        /// Replace <paramref name="EmailToken"/>s in input with values from token->value map.
        /// </summary>
        private string Replace(string input, IDictionary<string, string> tokenValueMap)
        {
            var buffer = new StringBuilder(input);
            foreach (var token in tokenValueMap.Keys)
            {
                buffer.Replace(token, tokenValueMap[token]);
            }
            var result = buffer.ToString();
            return Validate(result);
        }
        /// <summary>
        /// Validate text has no <paramref name="EmailToken"/>s. Throw if not valid.
        /// </summary>
        private string Validate(string text)
        {
            var tokens = GetTokens(text);
            if (tokens.Count > 0)
            {
                var nl = Environment.NewLine;
                var tokenBuffer = new StringBuilder();
                foreach (var token in tokens)
                {
                    tokenBuffer.Append(token.ToString() + nl);
                }
                throw new ApplicationException(
                    string.Format("text still has token(s).{0}{0}tokens:{0}{1}{0}{0}text:{0}{2}", nl, tokenBuffer, text)
                    );
            }
            return text;
        }
        /// <summary>
        /// Create a set containing invalid <paramref name="EmailToken"/>s.
        /// </summary>
        private ISet<EmailToken> GetInvalidTokens(string[] invalidTokens)
        {
            var tokens = new HashSet<EmailToken>();
            if (invalidTokens != null)
            {
                foreach (var invalidToken in invalidTokens)
                {
                    var match = Regex.Match(invalidToken, TokenRegex);
                    AddTokenIfMatched(tokens, match);
                }
            }
            return tokens;
        }

        #endregion
    }
}
