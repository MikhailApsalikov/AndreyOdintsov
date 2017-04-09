namespace XmlEntities
{
	using System.Collections.Generic;
	using System.IO;
	using System.Xml.Serialization;

	[XmlRoot("CompetencyList")]
	public class CompetencyList
	{
		public CompetencyList()
		{
		}

		public CompetencyList(string xmlFileName)
		{
			var serializer = new XmlSerializer(typeof (CompetencyList));
			using (var fileStream = new FileStream(xmlFileName, FileMode.Open))
			{
				Competencies = ((CompetencyList) serializer.Deserialize(fileStream)).Competencies;
			}
		}

		[XmlElement("Сompetency")]
		public List<Competency> Competencies { get; set; }
	}
}