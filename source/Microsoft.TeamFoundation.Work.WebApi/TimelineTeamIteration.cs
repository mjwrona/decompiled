// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.TimelineTeamIteration
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class TimelineTeamIteration
  {
    [DataMember(Name = "cssNodeId", EmitDefaultValue = false)]
    public string CssNodeId { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "path", EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(Name = "startDate", EmitDefaultValue = false)]
    public DateTime StartDate { get; set; }

    [DataMember(Name = "finishDate", EmitDefaultValue = false)]
    public DateTime FinishDate { get; set; }

    [DataMember(Name = "workItems", EmitDefaultValue = false)]
    public IList<object[]> WorkItems { get; set; } = (IList<object[]>) new List<object[]>();

    [DataMember(Name = "partiallyPagedWorkItems", EmitDefaultValue = false)]
    public IEnumerable<object[]> PartiallyPagedWorkItems { get; set; } = (IEnumerable<object[]>) new List<object[]>();

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public TimelineIterationStatus Status { get; set; }
  }
}
