// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsFilter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class ReportingWorkItemRevisionsFilter
  {
    [DataMember(Name = "fields")]
    public IEnumerable<string> Fields { get; set; }

    [DataMember(Name = "types")]
    public IEnumerable<string> Types { get; set; }

    [DataMember(Name = "includeIdentityRef")]
    public bool? IncludeIdentityRef { get; set; }

    [DataMember(Name = "includeDeleted")]
    public bool? IncludeDeleted { get; set; }

    [DataMember(Name = "includeTagRef")]
    public bool? IncludeTagRef { get; set; }

    [DataMember(Name = "includeLatestOnly")]
    public bool? IncludeLatestOnly { get; set; }

    [DataMember(Name = "includeDiscussionChangesOnly")]
    public bool? IncludeDiscussionChangesOnly { get; set; }
  }
}
