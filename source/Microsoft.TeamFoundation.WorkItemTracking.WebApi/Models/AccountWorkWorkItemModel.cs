// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.AccountWorkWorkItemModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class AccountWorkWorkItemModel
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string AssignedTo { get; set; }

    [DataMember]
    public string WorkItemType { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string State { get; set; }

    [DataMember]
    public DateTime ChangedDate { get; set; }

    [DataMember]
    public string TeamProject { get; set; }
  }
}
