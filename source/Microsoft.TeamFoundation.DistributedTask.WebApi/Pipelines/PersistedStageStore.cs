// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PersistedStageStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public class PersistedStageStore : IPersistedStageStore
  {
    private readonly IPersistedStageResolver m_resolver;
    private readonly IDictionary<string, PersistedStage> m_stageByName;

    public PersistedStageStore(IList<PersistedStage> stages, IPersistedStageResolver resolver = null)
    {
      this.m_resolver = resolver;
      this.m_stageByName = (IDictionary<string, PersistedStage>) new Dictionary<string, PersistedStage>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.AddStages(stages);
    }

    public IList<PersistedStage> GetAuthorizedReferences() => throw new NotImplementedException();

    public PersistedStage ResolvePersistedStage(PersistedStageReference stageRef)
    {
      PersistedStage stage;
      if (!this.m_stageByName.TryGetValue(stageRef.Name.Literal, out stage) && this.m_resolver != null)
      {
        stage = this.m_resolver.Resolve((ICollection<PersistedStageReference>) new List<PersistedStageReference>()
        {
          stageRef
        }).FirstOrDefault<PersistedStage>();
        this.AddStage(stage);
      }
      return stage;
    }

    public PersistedStage Get(PersistedStageReference reference) => reference == null ? (PersistedStage) null : this.ResolvePersistedStage(reference);

    public IList<PersistedStageReference> GetAll() => (IList<PersistedStageReference>) this.m_stageByName.Values.Select<PersistedStage, PersistedStageReference>((Func<PersistedStage, PersistedStageReference>) (stage => stage.GetPersistedStageReference())).ToList<PersistedStageReference>();

    private void AddStage(PersistedStage stage)
    {
      if (stage == null || string.IsNullOrWhiteSpace(stage.Name))
        return;
      this.m_stageByName[stage.Name] = stage;
    }

    private void AddStages(IList<PersistedStage> stages)
    {
      if (stages == null)
        return;
      foreach (PersistedStage stage in (IEnumerable<PersistedStage>) stages)
        this.AddStage(stage);
    }
  }
}
