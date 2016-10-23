namespace rm.EmailTemplateManager
{
	/// <summary>
	/// Encapsulates an email token of format {!TypeName.PropertyName}.
	/// </summary>
	/// <example>{!Data.Value1}</example>
	public class EmailToken
	{
		/// <summary>
		/// Type name.
		/// </summary>
		public string TypeName { get; private set; }
		/// <summary>
		/// Property name.
		/// </summary>
		public string PropertyName { get; private set; }
		public EmailToken(string typeName, string propertyName)
		{
			this.TypeName = typeName;
			this.PropertyName = propertyName;
		}
		public override bool Equals(object obj)
		{
			EmailToken that;
			return
				obj != null
				&& (that = obj as EmailToken) != null
				&& that.ToString() == this.ToString();
		}
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
		public override string ToString()
		{
			return EmailTokenReplacer.GetEmailTokenString(TypeName, PropertyName);
		}
	}
}
