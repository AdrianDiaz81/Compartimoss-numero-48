namespace Compartimoss.Example.Throttling.Model
{
	public class RequestIdentity
	{
		public string Ip { get; set; }

		public string Path { get; set; }

		public string HttpMethod { get; set; }
	}
}