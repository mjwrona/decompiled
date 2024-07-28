// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.DeliveryViewData
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class DeliveryViewData : PlanViewData
  {
    [DataMember(Name = "startDate", EmitDefaultValue = false)]
    public DateTime StartDate { get; set; }

    [DataMember(Name = "endDate", EmitDefaultValue = false)]
    public DateTime EndDate { get; set; }

    [DataMember(Name = "teams", EmitDefaultValue = false)]
    public IEnumerable<TimelineTeamData> Teams { get; set; }

    [DataMember(Name = "childIdToParentIdMap", EmitDefaultValue = false)]
    public IDictionary<int, int> ChildIdToParentIdMap { get; set; }

    [DataMember(Name = "criteriaStatus", EmitDefaultValue = false)]
    public TimelineCriteriaStatus CriteriaStatus { get; set; }

    [DataMember(Name = "maxExpandedTeams", EmitDefaultValue = false, IsRequired = false)]
    public int MaxExpandedTeams { get; set; }

    [DataMember(Name = "parentItemMaps", EmitDefaultValue = false, IsRequired = false)]
    public ICollection<ParentChildWIMap> ParentItemMaps { get; set; }

    [DataMember(Name = "workItemViolations", EmitDefaultValue = false, IsRequired = false)]
    public List<int> WorkItemViolations { get; set; }

    [DataMember(Name = "workItemDependencies", EmitDefaultValue = false, IsRequired = false)]
    public List<int> WorkItemDependencies { get; set; }
  }
}
