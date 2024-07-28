// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.PhaseInstance
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PhaseInstance : GraphNodeInstance<PhaseNode>
  {
    public PhaseInstance()
    {
    }

    public PhaseInstance(string name)
      : this(name, TaskResult.Succeeded)
    {
    }

    public PhaseInstance(string name, int attempt)
      : this(name, attempt, (PhaseNode) null, TaskResult.Succeeded)
    {
    }

    public PhaseInstance(PhaseNode phase)
      : this(phase, 1)
    {
    }

    public PhaseInstance(PhaseNode phase, int attempt)
      : this(phase.Name, attempt, phase, TaskResult.Succeeded)
    {
    }

    public PhaseInstance(string name, TaskResult result)
      : this(name, 1, (PhaseNode) null, result)
    {
    }

    public PhaseInstance(string name, int attempt, PhaseNode definition, TaskResult result)
      : base(name, attempt, definition, result)
    {
    }

    public static implicit operator PhaseInstance(string name) => new PhaseInstance(name);
  }
}
