// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ContainerResourceTrigger
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ContainerResourceTrigger : PipelineTrigger, IEquatable<ContainerResourceTrigger>
  {
    [DataMember(Name = "TagFilters", EmitDefaultValue = false)]
    private List<string> m_tagFilters;

    public ContainerResourceTrigger()
      : base(PipelineTriggerType.ContainerImage)
    {
    }

    private ContainerResourceTrigger(ContainerResourceTrigger triggerToClone)
      : base(PipelineTriggerType.ContainerImage)
    {
      this.TagFilters.AddRange<string, IList<string>>((IEnumerable<string>) new List<string>((IEnumerable<string>) triggerToClone.TagFilters));
    }

    public IList<string> TagFilters
    {
      get
      {
        if (this.m_tagFilters == null)
          this.m_tagFilters = new List<string>();
        return (IList<string>) this.m_tagFilters;
      }
    }

    public ContainerResourceTrigger Clone() => new ContainerResourceTrigger(this);

    public bool Equals(ContainerResourceTrigger other) => other != null && this.TriggerType == other.TriggerType && this.TagFilters.Except<string>((IEnumerable<string>) other.TagFilters).Count<string>() == 0 && other.TagFilters.Except<string>((IEnumerable<string>) this.TagFilters).Count<string>() == 0;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((ContainerResourceTrigger) obj);
    }

    public override int GetHashCode()
    {
      int hashCode = 0;
      if (this.m_tagFilters != null && this.m_tagFilters.Any<string>())
        hashCode = this.GetHashCode((IList<string>) this.m_tagFilters);
      return hashCode;
    }

    private int GetHashCode(IList<string> items)
    {
      int hashCode = 0;
      foreach (string str in (IEnumerable<string>) (items ?? (IList<string>) new List<string>()))
        hashCode = hashCode * 397 ^ str.GetHashCode();
      return hashCode;
    }
  }
}
