using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Pipelines.GetChromeData;
using Sitecore.Pipelines.GetPlaceholderRenderings;
using Sitecore.StringExtensions;
using Sitecore.Web.UI.PageModes;

namespace Skillcore.ExperienceEditorCache.Processors
{
    public class GetPlaceholderChromeData : GetChromeDataProcessor
    {
        public const string ChromeType = "placeholder";
        public const string PlaceholderKey = "placeHolderKey";
        private const string DefaultButtonsRoot = "/sitecore/content/Applications/WebEdit/Default Placeholder Buttons";

        public override void Process(GetChromeDataArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.ChromeData, "Chrome Data");
            if ("placeholder".Equals(args.ChromeType, StringComparison.OrdinalIgnoreCase))
            {
                string str = args.CustomData["placeHolderKey"] as string;
                Assert.ArgumentNotNull(str, "CustomData[\"{0}\"]".FormatWith("placeHolderKey"));
                string lastPart = StringUtil.GetLastPart(str, '/', str);
                args.ChromeData.DisplayName = lastPart;
                AddButtonsToChromeData(GetButtons(DefaultButtonsRoot), args);
                Item obj = null;
                bool hasPlaceholderSettings = false;
                if (args.Item != null)
                {
                    string layout = GetLayout(args);
                    GetPlaceholderRenderingsArgs placeholderRenderingsArgs = new GetPlaceholderRenderingsArgs(str,
                        layout, args.Item.Database)
                    {
                        OmitNonEditableRenderings = true
                    };
                    CorePipeline.Run("getPlaceholderRenderings", placeholderRenderingsArgs);
                    hasPlaceholderSettings = placeholderRenderingsArgs.HasPlaceholderSettings;
                    List<string> ids;
                    if (placeholderRenderingsArgs.PlaceholderRenderings == null)
                    {
                        ids = new List<string>();
                    }
                    else
                    {
                        ids =
                            (placeholderRenderingsArgs.PlaceholderRenderings.Select(i => i.ID.ToShortID().ToString()))
                                .ToList();
                    }
                    List<string> list = ids;
                    args.ChromeData.Custom.Add("allowedRenderings", list);
                    obj = Client.Page.GetPlaceholderItem(str, args.Item.Database, layout);
                    if (obj != null)
                    {
                        args.ChromeData.DisplayName = obj.DisplayName;
                    }
                    if (obj != null && !string.IsNullOrEmpty(obj.Appearance.ShortDescription))
                    {
                        args.ChromeData.ExpandedDisplayName = obj.Appearance.ShortDescription;
                    }
                }
                else
                {
                    args.ChromeData.Custom.Add("allowedRenderings", new List<string>());
                }
                bool editable = (obj == null || obj["Editable"] == "1") && (Settings.WebEdit.PlaceholdersEditableWithoutSettings || hasPlaceholderSettings);
                args.ChromeData.Custom.Add("editable", editable.ToString().ToLowerInvariant());
            }
        }

        private string GetLayout(GetChromeDataArgs args)
        {
            string result;
            if (HttpContext.Current != null && args.Item != null)
            {
                string cachedLayout = HttpContext.Current.Items["Layout-" + args.Item.Uri.ToString(true)] as string;
                if (cachedLayout != null)
                {
                    result = cachedLayout;
                    return result;
                }
            }
            string layout = ChromeContext.GetLayout(args.Item);
            if (HttpContext.Current != null && args.Item != null && layout != null)
            {
                HttpContext.Current.Items["Layout-" + args.Item.Uri.ToString(true)] = layout;
            }
            result = layout;
            return result;
        }
    }
}
