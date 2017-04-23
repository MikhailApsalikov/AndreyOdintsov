namespace BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Web.Hosting;
	using System.Xml.Serialization;
	using XmlEntities;

	public class CompetencyListWorkflow
	{
		private const string CompetencyListPath = "~/App_Data/CompetencyList.xml";
		private const string ProfCompetencyListDirectory = "~/App_Data/ProfCompetencies/";
		private const string ProfCompetencyListPath = "~/App_Data/ProfCompetencies/{0}.xml";

		public CompetencyList GetCompetencyList()
		{
			return new CompetencyList(HostingEnvironment.MapPath(CompetencyListPath));
		}

		public string GetCompetencyListAsText()
		{
			return File.ReadAllText(HostingEnvironment.MapPath(CompetencyListPath));
		}

		public void SetCompetencyListAsText(string data)
		{
			new XmlSerializer(typeof (CompetencyList)).Deserialize(new StringReader(data));
			File.WriteAllText(HostingEnvironment.MapPath(CompetencyListPath), data);
		}


		public CompetencyList GetProfCompetencyList(string name)
		{
			return new CompetencyList(HostingEnvironment.MapPath(string.Format(ProfCompetencyListPath, name)));
		}

		public string GetProfCompetencyListAsText(string name)
		{
			return File.ReadAllText(HostingEnvironment.MapPath(string.Format(ProfCompetencyListPath, name)));
		}

		public void SetProfCompetencyListAsText(string name, string data)
		{
			new XmlSerializer(typeof(CompetencyList)).Deserialize(new StringReader(data));
			File.WriteAllText(HostingEnvironment.MapPath(string.Format(ProfCompetencyListPath, name)), data);
		}

		public IEnumerable<string> GetProfCompetencyLists()
		{
			return Directory.GetFiles(HostingEnvironment.MapPath(ProfCompetencyListDirectory), "*.xml").Select(Path.GetFileNameWithoutExtension);
		}
	}
}