namespace Compartimoss.Example.Throttling.Model
{
	public class QuotaExceededResponse
	{
		public string ContentType { get; set; }

		public string Content { get; set; }

		public int? StatusCode { get; set; } = 429;
	}
}
