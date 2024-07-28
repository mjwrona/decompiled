// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemBatchGetRequest
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemBatchGetRequest
  {
    [DataMember]
    public IEnumerable<int> Ids { get; set; }

    [DataMember]
    public IEnumerable<string> Fields { get; set; }

    [DataMember]
    public DateTime? AsOf { get; set; }

    [DataMember(Name = "$expand")]
    public WorkItemExpand Expand { get; set; }

    [DataMember]
    public WorkItemErrorPolicy ErrorPolicy { get; set; } = WorkItemErrorPolicy.Fail;
  }
}
