// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.PersistedStageResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class PersistedStageResolver : IPersistedStageResolver
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;

    public PersistedStageResolver(IVssRequestContext requestContext, Guid projectId)
    {
      this.m_requestContext = requestContext;
      this.m_projectId = projectId;
    }

    public IList<PersistedStage> Resolve(ICollection<PersistedStageReference> references) => references == null || references.Count <= 0 ? (IList<PersistedStage>) new List<PersistedStage>() : this.m_requestContext.GetService<IDistributedTaskPersistedStageService>().ResolvePersistedStages(this.m_requestContext, this.m_projectId, references);
  }
}
