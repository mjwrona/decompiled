// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Controllers.PipelineTriggerController
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Deployment.Controllers
{
  [VersionedApiControllerCustomName(Area = "Deployment", ResourceName = "resourcetriggers")]
  [ClientIgnore]
  public class PipelineTriggerController : TfsProjectApiController
  {
    [ClientResponseType(typeof (IList<PipelineTriggerInfo>), null, null)]
    [HttpGet]
    public IList<PipelineTriggerInfo> GetPipelineTriggerInfo(int pipelineDefinitionId)
    {
      IList<PipelineDefinitionTrigger> pipelineTriggers = this.TfsRequestContext.GetService<IPipelineTriggerService>().GetPipelineTriggers(this.TfsRequestContext, this.ProjectId, pipelineDefinitionId);
      List<string> list = pipelineTriggers.Select<PipelineDefinitionTrigger, string>((Func<PipelineDefinitionTrigger, string>) (trigger => trigger.ArtifactDefinition.UniqueResourceIdentifier)).ToList<string>();
      IWebHookService service = this.TfsRequestContext.GetService<IWebHookService>();
      Dictionary<string, WebHook> dictionary = new Dictionary<string, WebHook>();
      foreach (string str in list)
      {
        WebHook webHook = service.GetWebHook(this.TfsRequestContext, str, true);
        if (webHook != null)
          dictionary[str] = webHook;
      }
      List<PipelineTriggerInfo> pipelineTriggerInfo1 = new List<PipelineTriggerInfo>();
      foreach (PipelineDefinitionTrigger definitionTrigger in (IEnumerable<PipelineDefinitionTrigger>) pipelineTriggers)
      {
        PipelineTriggerInfo pipelineTriggerInfo2 = new PipelineTriggerInfo();
        pipelineTriggerInfo2.PipelineDefinitionTrigger = definitionTrigger;
        if (dictionary.ContainsKey(definitionTrigger.ArtifactDefinition.UniqueResourceIdentifier))
          pipelineTriggerInfo2.WebHook = dictionary[definitionTrigger.ArtifactDefinition.UniqueResourceIdentifier];
        pipelineTriggerInfo1.Add(pipelineTriggerInfo2);
      }
      return (IList<PipelineTriggerInfo>) pipelineTriggerInfo1;
    }
  }
}
