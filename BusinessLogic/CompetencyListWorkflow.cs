namespace BusinessLogic
{
	using System;
	using System.IO;
	using System.Xml.Serialization;
	using XmlEntities;

	public class CompetencyListWorkflow
	{
		private const string DefaultPath = "~/App_Data/CompetencyList.xml";

		public CompetencyListWorkflow(Func<string, string> mapPath)
		{
			MapPath = mapPath;
		}

		public Func<string, string> MapPath { get; set; }

		public CompetencyList GetDefault()
		{
			return new CompetencyList(MapPath(DefaultPath));
		}

		public string GetDefaultAsText()
		{
			return File.ReadAllText(MapPath(DefaultPath));
		}

		public void SetDefaultAsText(string data)
		{
			new XmlSerializer(typeof (CompetencyList)).Deserialize(new StringReader(data));
			File.WriteAllText(MapPath(DefaultPath), data);
		}
	}
}