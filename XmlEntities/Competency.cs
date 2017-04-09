namespace XmlEntities
{
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class Competency
	{
		[XmlAttribute("Id")]
		public int Id { get; set; }

		[XmlAttribute("Title")]
		public string Title { get; set; }

		[XmlAttribute("Enabled")]
		public bool Enabled { get; set; } = true;

		[XmlElement("Indicator")]
		public List<Indicator> Indicators { get; set; }
	}
}