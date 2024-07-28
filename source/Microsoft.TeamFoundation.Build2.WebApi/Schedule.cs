// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Schedule
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class Schedule : BaseSecuredObject
  {
    [DataMember(Name = "BranchFilters", EmitDefaultValue = false)]
    private List<string> m_branchFilters;

    public Schedule()
    {
    }

    internal Schedule(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TimeZoneId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int StartHours { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int StartMinutes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ScheduleDays DaysToBuild { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid ScheduleJobId { get; set; }

    public List<string> BranchFilters
    {
      get
      {
        if (this.m_branchFilters == null)
          this.m_branchFilters = new List<string>();
        return this.m_branchFilters;
      }
      internal set => this.m_branchFilters = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool ScheduleOnlyWithChanges { get; set; }
  }
}
