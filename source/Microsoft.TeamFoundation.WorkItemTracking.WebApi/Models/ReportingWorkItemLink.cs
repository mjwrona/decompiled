// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ReportingWorkItemLink
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class ReportingWorkItemLink
  {
    [DataMember]
    public string Rel { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string LinkType { get; set; }

    [DataMember]
    public int SourceId { get; set; }

    [DataMember]
    public int TargetId { get; set; }

    [DataMember]
    public DateTime ChangedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ChangedBy { get; set; }

    [DataMember]
    public bool IsActive { get; set; }

    [DataMember]
    public LinkChangeType ChangedOperation { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }
  }
}
