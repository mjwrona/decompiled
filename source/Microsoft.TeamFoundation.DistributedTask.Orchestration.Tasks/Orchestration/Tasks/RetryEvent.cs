// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RetryEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  [DataContract]
  public sealed class RetryEvent
  {
    [DataMember]
    public IEnumerable<IGraphNode> Children { get; set; }

    [DataMember]
    public IList<string> TargetedChildNames { get; set; }

    [DataMember]
    public bool ForceRetrySubNodes { get; set; }

    [DataMember]
    public Guid RequestedBy { get; set; }
  }
}
