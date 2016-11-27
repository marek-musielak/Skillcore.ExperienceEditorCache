using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Skillcore.ExperienceEditorCache.Processors
{
	public class ItemHasActiveTestRequest : PipelineProcessorRequest<ItemContext>
	{
		public override PipelineProcessorResponseValue ProcessRequest()
		{
			return new PipelineProcessorResponseValue
			{
				Value = false
			};
		}
	}
}
