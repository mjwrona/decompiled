// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.AccountRecentMentionWorkItemModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class AccountRecentMentionWorkItemModel
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
    public string TeamProject { get; set; }

    [DataMember]
    public DateTime MentionedDateField { get; set; }
  }
}
