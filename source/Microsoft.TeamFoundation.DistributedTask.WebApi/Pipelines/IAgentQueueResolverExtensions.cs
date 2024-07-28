// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.IAgentQueueResolverExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IAgentQueueResolverExtensions
  {
    public static TaskAgentQueue Resolve(
      this IAgentQueueResolver resolver,
      AgentQueueReference reference,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      return resolver.Resolve((ICollection<AgentQueueReference>) new AgentQueueReference[1]
      {
        reference
      }, actionFilter).FirstOrDefault<TaskAgentQueue>();
    }
  }
}
