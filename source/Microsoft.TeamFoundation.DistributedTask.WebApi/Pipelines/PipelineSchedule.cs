// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineSchedule
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineSchedule
  {
    [DataMember(Name = "BranchFilters", EmitDefaultValue = false)]
    private List<string> m_branchFilters;

    public PipelineSchedule()
    {
      this.ScheduleOnlyWithChanges = true;
      this.BatchSchedules = false;
    }

    [DataMember(EmitDefaultValue = false)]
    public string ScheduleType => PipelineConstants.ScheduleType.Cron;

    [DataMember(EmitDefaultValue = false)]
    public string ScheduleDetails { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    public IList<string> BranchFilters
    {
      get
      {
        if (this.m_branchFilters == null)
          this.m_branchFilters = new List<string>();
        return (IList<string>) this.m_branchFilters;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public bool BatchSchedules { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ScheduleJobId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ScheduleOnlyWithChanges { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<string> branchFilters = this.m_branchFilters;
      // ISSUE: explicit non-virtual call
      if ((branchFilters != null ? (__nonvirtual (branchFilters.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_branchFilters = (List<string>) null;
    }
  }
}
