using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Odintsov.Accounts.Web.Models
{
	[XmlRoot("CompetencyList")]
	public class CompetencyList
	{
		[XmlElement("Сompetency")]
		public List<Competency> Competencies { get; set; }

        public CompetencyList()
		{
		}

		public CompetencyList(string xmlFileName)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(CompetencyList));
			using (FileStream fileStream = new FileStream(xmlFileName, FileMode.Open))
			{
				Competencies = ((CompetencyList)serializer.Deserialize(fileStream)).Competencies;
			}
		}
	}

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