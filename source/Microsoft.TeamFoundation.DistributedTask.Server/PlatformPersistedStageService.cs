// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformPersistedStageService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformPersistedStageService : 
    IDistributedTaskPersistedStageService,
    IVssFrameworkService
  {
    public PersistedStage AddPersistedStageGroupMapping(
      IVssRequestContext requestContext,
      Guid projectId,
      PersistedStageReference stageRef)
    {
      using (new MethodScope(requestContext, "PersistedStageService", nameof (AddPersistedStageGroupMapping)))
      {
        requestContext.TraceInfo(10015330, "PersistedStageService", "Add persisted stage with Id: {0} for pipeline run {1} of definition id: {2}.", (object) stageRef.Id, (object) stageRef.BuildId, (object) stageRef.DefinitionId);
        PersistedStage persistedStage;
        using (PersistedStageComponent component = requestContext.CreateComponent<PersistedStageComponent>())
          persistedStage = component.AddPersistedStageGroupMapping(projectId, stageRef);
        return persistedStage;
      }
    }

    public IList<PersistedStage> GetPersistedStagesByDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId)
    {
      using (new MethodScope(requestContext, "PersistedStageService", nameof (GetPersistedStagesByDefinition)))
      {
        requestContext.TraceInfo(10015332, "PersistedStageService", "Get persisted stage resources for pipeline definition ID {0}.", (object) pipelineDefinitionId);
        IList<PersistedStage> stagesByDefinition = (IList<PersistedStage>) null;
        using (PersistedStageComponent component = requestContext.CreateComponent<PersistedStageComponent>())
          stagesByDefinition = component.GetPersistedStagesByDefinition(projectId, pipelineDefinitionId);
        return stagesByDefinition;
      }
    }

    public IList<PersistedStage> GetPersistedStages(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<PersistedStageReference> stageRefs,
      bool mapToGroup = false)
    {
      using (new MethodScope(requestContext, "PersistedStageService", nameof (GetPersistedStages)))
      {
        requestContext.TraceInfo(10015332, "PersistedStageService", "Get {0} persisted stage resources.", (object) stageRefs.Count);
        IList<PersistedStage> persistedStages = (IList<PersistedStage>) null;
        using (PersistedStageComponent component = requestContext.CreateComponent<PersistedStageComponent>())
          persistedStages = component.GetPersistedStages(projectId, stageRefs, mapToGroup);
        return persistedStages;
      }
    }

    public IList<PersistedStage> ResolvePersistedStages(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<PersistedStageReference> stageRefs)
    {
      using (new MethodScope(requestContext, "PersistedStageService", nameof (ResolvePersistedStages)))
      {
        requestContext.TraceInfo(10015333, "PersistedStageService", "Resolve {0} persisted stage resources.", (object) stageRefs.Count);
        IDictionary<string, PersistedStage> dictionary = (IDictionary<string, PersistedStage>) this.GetPersistedStages(requestContext, projectId, stageRefs, true).ToDictionary<PersistedStage, string, PersistedStage>((Func<PersistedStage, string>) (x => x.Name), (Func<PersistedStage, PersistedStage>) (x => x));
        foreach (PersistedStageReference stageRef in (IEnumerable<PersistedStageReference>) stageRefs)
        {
          PersistedStage persistedStage;
          if (!dictionary.TryGetValue(stageRef.Name.Literal, out persistedStage) || persistedStage.Group == null)
            dictionary[stageRef.Name.Literal] = this.AddPersistedStageGroupMapping(requestContext, projectId, stageRef);
        }
        return dictionary != null ? (IList<PersistedStage>) dictionary.Values.ToList<PersistedStage>() : (IList<PersistedStage>) null;
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
