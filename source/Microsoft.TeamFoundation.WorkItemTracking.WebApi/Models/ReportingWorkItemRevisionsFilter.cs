// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ReportingWorkItemRevisionsFilter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class ReportingWorkItemRevisionsFilter
  {
    [DataMember]
    public IEnumerable<string> Fields { get; set; }

    [DataMember]
    public IEnumerable<string> Types { get; set; }

    [DataMember]
    public bool IncludeIdentityRef { get; set; }

    [DataMember]
    public bool IncludeDeleted { get; set; }

    [DataMember]
    public bool IncludeTagRef { get; set; }

    [DataMember]
    public bool IncludeLatestOnly { get; set; }
  }
}
