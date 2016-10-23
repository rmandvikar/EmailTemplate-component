namespace rm.EmailTemplateManager
{
	/// <summary>
	/// A sample data class.
	/// </summary>
	public class Data
	{
		public string Value1 { get; set; }
		public string Value2 { get; set; }
		public string Email { get; set; }
		/// <summary>
		/// This property is sensitive and could be specified to be not replaced in template.
		/// </summary>
		public string Secret { get; set; }
	}

	/// <summary>
	/// A sample data class.
	/// </summary>
	public class User
	{
		public string Value { get; set; }
	}
}
