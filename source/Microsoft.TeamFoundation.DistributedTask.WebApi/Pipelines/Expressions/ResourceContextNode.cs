// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ResourceContextNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ResourceContextNode : NamedValueNode
  {
    protected override object EvaluateCore(EvaluationContext context)
    {
      IPipelineContext state = context.State as IPipelineContext;
      Dictionary<string, object> core = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, RepositoryNode> dictionary1 = state.ResourceStore.Repositories.GetAll().ToDictionary<RepositoryResource, string, RepositoryNode>((Func<RepositoryResource, string>) (k => k.Alias), (Func<RepositoryResource, RepositoryNode>) (v => new RepositoryNode(v)));
      core.Add(ExpressionConstants.Repositories, (object) dictionary1);
      Dictionary<string, ContainerNode> dictionary2 = state.ResourceStore.Containers.GetAll().ToDictionary<ContainerResource, string, ContainerNode>((Func<ContainerResource, string>) (k => k.Alias), (Func<ContainerResource, ContainerNode>) (v => new ContainerNode(v)));
      core.Add(ExpressionConstants.Containers, (object) dictionary2);
      return (object) core;
    }
  }
}
