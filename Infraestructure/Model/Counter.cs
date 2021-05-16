using System;

namespace Compartimoss.Example.Throotling.Model
{
	/// <summary>
	/// Stores the initial access time and the numbers of calls made from that point
	/// </summary>
	public class Counter
	{
		public DateTime Timestamp { get; set; }

		public double Count { get; set; }
	}
}