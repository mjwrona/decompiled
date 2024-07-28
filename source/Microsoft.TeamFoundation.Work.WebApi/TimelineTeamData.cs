// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.TimelineTeamData
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class TimelineTeamData
  {
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "projectId", EmitDefaultValue = false)]
    public Guid ProjectId { get; set; }

    [DataMember(Name = "orderByField", EmitDefaultValue = false)]
    public string OrderByField { get; set; }

    [DataMember(Name = "teamFieldName", EmitDefaultValue = false)]
    public string TeamFieldName { get; set; }

    [DataMember(Name = "teamFieldDefaultValue", EmitDefaultValue = false)]
    public string TeamFieldDefaultValue { get; set; }

    [DataMember(Name = "teamFieldValues", EmitDefaultValue = false)]
    public IEnumerable<TeamFieldValue> TeamFieldValues { get; set; }

    [DataMember(Name = "fieldReferenceNames", EmitDefaultValue = false)]
    public IEnumerable<string> FieldReferenceNames { get; set; }

    [DataMember(Name = "partiallyPagedFieldReferenceNames", EmitDefaultValue = false)]
    public IEnumerable<string> PartiallyPagedFieldReferenceNames { get; set; }

    [DataMember(Name = "workItemTypeColors", EmitDefaultValue = false)]
    public IEnumerable<WorkItemColor> WorkItemTypeColors { get; set; }

    [DataMember(Name = "iterations", EmitDefaultValue = false)]
    public IEnumerable<TimelineTeamIteration> Iterations { get; set; }

    [DataMember(Name = "workItems", EmitDefaultValue = false)]
    public IEnumerable<object[]> WorkItems { get; set; }

    [DataMember(Name = "partiallyPagedWorkItems", EmitDefaultValue = false)]
    public IEnumerable<object[]> PartiallyPagedWorkItems { get; set; }

    [DataMember(Name = "isExpanded", EmitDefaultValue = false)]
    public bool IsExpanded { get; set; }

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public TimelineTeamStatus Status { get; set; }

    [DataMember(Name = "backlog", EmitDefaultValue = false)]
    public BacklogLevel Backlog { get; set; }

    [DataMember(Name = "rollupWorkItemTypes", EmitDefaultValue = false)]
    public IEnumerable<string> RollupWorkItemTypes { get; set; }
  }
}
