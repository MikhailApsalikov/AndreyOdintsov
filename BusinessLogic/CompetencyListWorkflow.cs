namespace BusinessLogic
{
	using System;
	using System.IO;
	using System.Web.Hosting;
	using System.Xml.Serialization;
	using XmlEntities;

	public class CompetencyListWorkflow
	{
		private const string DefaultPath = "~/App_Data/CompetencyList.xml";

		public CompetencyList GetDefault()
		{
			return new CompetencyList(HostingEnvironment.MapPath(DefaultPath));
		}

		public string GetDefaultAsText()
		{
			return File.ReadAllText(HostingEnvironment.MapPath(DefaultPath));
		}

		public void SetDefaultAsText(string data)
		{
			new XmlSerializer(typeof (CompetencyList)).Deserialize(new StringReader(data));
			File.WriteAllText(HostingEnvironment.MapPath(DefaultPath), data);
		}
	}
}