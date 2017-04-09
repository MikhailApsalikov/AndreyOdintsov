namespace XmlEntities
{
	using System.Xml.Serialization;

	public class Indicator
	{
		[XmlAttribute("Id")]
		public int Id { get; set; }

		[XmlAttribute("Title")]
		public string Title { get; set; }

		[XmlAttribute("Enabled")]
		public bool Enabled { get; set; } = true;

		[XmlElement("LessThanExpectations")]
		public string LessThanExpectations { get; set; }

		[XmlElement("EqualsToExpectations")]
		public string EqualsToExpectations { get; set; }

		[XmlElement("MoreThanExpectations")]
		public string MoreThanExpectations { get; set; }
	}
}