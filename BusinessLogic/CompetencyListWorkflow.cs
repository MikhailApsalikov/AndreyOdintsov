namespace BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Web;
	using System.Web.Caching;
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
			new XmlSerializer(typeof (CompetencyList)).Deserialize(new StringReader(data));
			File.WriteAllText(HostingEnvironment.MapPath(string.Format(ProfCompetencyListPath, name)), data);
		}

		public static IEnumerable<string> GetProfCompetencyLists()
		{
			var files = HttpContext.Current.Cache.Get("ProfCompetencyList") as IEnumerable<string>;
			if (files != null)
			{
				return files;
			}

			files =
				Directory.GetFiles(HostingEnvironment.MapPath(ProfCompetencyListDirectory), "*.xml")
					.Select(Path.GetFileNameWithoutExtension);
			HttpContext.Current.Cache.Add("ProfCompetencyList", files, null, DateTime.Now.AddMinutes(15),
				Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
			return files;
		}

		public static bool IsProfCompetencyExist(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			return GetProfCompetencyLists().Any(s => s.ToUpperInvariant() == name.ToUpperInvariant());
		}

		public void CreateNewProfCompetency(string name)
		{
			SetProfCompetencyListAsText(name, NewTemplate);
			HttpContext.Current.Cache.Remove("ProfCompetencyList");
		}

		const string NewTemplate = @"<CompetencyList xml:space='preserve'>
	<Сompetency Id='1' Title='Компетенция' Enabled='true' xml:space='preserve'>
		<Indicator Id = '1' Title='Индикатор' Enabled='true' xml:space='preserve'>
			<LessThanExpectations xml:space='preserve'>Ниже ожиданий!</LessThanExpectations>
			<EqualsToExpectations xml:space='preserve'>Соответствует ожиданиям!</EqualsToExpectations>
			<MoreThanExpectations xml:space='preserve'>Выше ожиданий!</MoreThanExpectations>
		</Indicator>
	</Сompetency>
</CompetencyList>";
	}
}