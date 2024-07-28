// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildResourceTrigger
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BuildResourceTrigger : PipelineTrigger, IEquatable<BuildResourceTrigger>
  {
    public BuildResourceTrigger()
      : base(PipelineTriggerType.BuildResourceCompletion)
    {
    }

    private BuildResourceTrigger(BuildResourceTrigger triggerToClone)
      : base(PipelineTriggerType.BuildResourceCompletion)
    {
    }

    public BuildResourceTrigger Clone() => new BuildResourceTrigger(this);

    public bool Equals(BuildResourceTrigger other) => other != null && this.TriggerType == other.TriggerType && this.Enabled == other.Enabled;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((BuildResourceTrigger) obj);
    }

    public override int GetHashCode() => this.Enabled.GetHashCode() * 397 ^ this.TriggerType.GetHashCode();
  }
}
