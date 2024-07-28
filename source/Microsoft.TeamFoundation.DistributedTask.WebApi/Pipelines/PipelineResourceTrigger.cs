// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineResourceTrigger
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
  public class PipelineResourceTrigger : PipelineTrigger, IEquatable<PipelineResourceTrigger>
  {
    [DataMember(Name = "BranchFilters", EmitDefaultValue = false)]
    private List<string> m_branchFilters;
    [DataMember(Name = "StageFilters", EmitDefaultValue = false)]
    private List<string> m_stageFilters;
    [DataMember(Name = "TagFilters", EmitDefaultValue = false)]
    private List<string> m_tagFilters;

    public PipelineResourceTrigger()
      : base(PipelineTriggerType.PipelineCompletion)
    {
    }

    private PipelineResourceTrigger(PipelineResourceTrigger triggerToClone)
      : base(PipelineTriggerType.PipelineCompletion)
    {
      this.BranchFilters.AddRange<string, IList<string>>((IEnumerable<string>) new List<string>((IEnumerable<string>) triggerToClone.BranchFilters));
      this.StageFilters.AddRange<string, IList<string>>((IEnumerable<string>) new List<string>((IEnumerable<string>) triggerToClone.StageFilters));
      this.TagFilters.AddRange<string, IList<string>>((IEnumerable<string>) new List<string>((IEnumerable<string>) triggerToClone.TagFilters));
    }

    public IList<string> BranchFilters
    {
      get
      {
        if (this.m_branchFilters == null)
          this.m_branchFilters = new List<string>();
        return (IList<string>) this.m_branchFilters;
      }
    }

    public IList<string> StageFilters
    {
      get
      {
        if (this.m_stageFilters == null)
          this.m_stageFilters = new List<string>();
        return (IList<string>) this.m_stageFilters;
      }
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

    public PipelineResourceTrigger Clone() => new PipelineResourceTrigger(this);

    public bool Equals(PipelineResourceTrigger other) => other != null && this.TriggerType == other.TriggerType && this.BranchFilters.Except<string>((IEnumerable<string>) other.BranchFilters).Count<string>() == 0 && other.BranchFilters.Except<string>((IEnumerable<string>) this.BranchFilters).Count<string>() == 0 && this.StageFilters.Except<string>((IEnumerable<string>) other.StageFilters).Count<string>() == 0 && other.StageFilters.Except<string>((IEnumerable<string>) this.StageFilters).Count<string>() == 0 && this.TagFilters.Except<string>((IEnumerable<string>) other.TagFilters).Count<string>() == 0 && other.TagFilters.Except<string>((IEnumerable<string>) this.TagFilters).Count<string>() == 0;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((PipelineResourceTrigger) obj);
    }

    public override int GetHashCode()
    {
      int hashCode = 0;
      if (this.m_branchFilters != null && this.m_branchFilters.Any<string>())
        hashCode = this.GetHashCode((IList<string>) this.m_branchFilters);
      if (this.m_stageFilters != null && this.m_stageFilters.Any<string>())
        hashCode = hashCode * 397 ^ this.GetHashCode((IList<string>) this.m_stageFilters);
      if (this.m_tagFilters != null && this.m_tagFilters.Any<string>())
        hashCode = hashCode * 397 ^ this.GetHashCode((IList<string>) this.m_tagFilters);
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
