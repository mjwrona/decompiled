// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemUpdate : WorkItemTrackingResource
  {
    public WorkItemUpdate()
    {
    }

    public WorkItemUpdate(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public int WorkItemId { get; set; }

    [DataMember]
    public int Rev { get; set; }

    [DataMember]
    public IdentityReference RevisedBy { get; set; }

    [DataMember]
    public DateTime RevisedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, WorkItemFieldUpdate> Fields { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkItemRelationUpdates Relations { get; set; }
  }
}
