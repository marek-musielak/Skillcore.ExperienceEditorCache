using System;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.FXM.Abstractions;
using Sitecore.Pipelines.GetPlaceholderRenderings;

namespace Skillcore.ExperienceEditorCache.Processors
{
	public class EnableExternalPagePlaceholdersProcessor
	{
		private readonly ISitecoreContext _sitecoreContext;
		public EnableExternalPagePlaceholdersProcessor() : this(new SitecoreContextWrapper())
		{
		}
		public EnableExternalPagePlaceholdersProcessor(ISitecoreContext context)
		{
			_sitecoreContext = context;
		}
		public void Process(GetPlaceholderRenderingsArgs args)
		{
			Item item = _sitecoreContext.Item;
			if (item != null)
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["HasPlaceholderSettings-" + item.Uri.ToString(true)] != null)
					{
						if ((bool)HttpContext.Current.Items["HasPlaceholderSettings-" + item.Uri.ToString(true)])
						{
							args.HasPlaceholderSettings = true;
							return;
						}
					}
				}
				if (item.TemplateID.Guid == new Guid("{10E23679-55DB-4059-B8F2-E417A2F78FCB}"))
				{
					args.HasPlaceholderSettings = true;
				}
				if (item.Visualization.Layout == null || !(item.Visualization.Layout.ID.Guid == new Guid("{C72C238F-018E-469E-9440-3EDD7420A8C8}")))
				{
					if (HttpContext.Current != null)
					{
						HttpContext.Current.Items["HasPlaceholderSettings-" + item.Uri.ToString(true)] = args.HasPlaceholderSettings;
					}
				}
				else
				{
					args.HasPlaceholderSettings = true;
					if (HttpContext.Current != null)
					{
						HttpContext.Current.Items["HasPlaceholderSettings-" + item.Uri.ToString(true)] = args.HasPlaceholderSettings;
					}
				}
			}
		}
	}
}
