// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemChangedProjectEvent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Notification, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 45693437-E9C3-448A-85F4-93C2412459C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Notification.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Notification
{
  [DataContract]
  public class WorkItemChangedProjectEvent
  {
    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public IEnumerable<string> WorkItemChangedList { get; set; }

    [DataMember]
    public IEnumerable<string> WorkItemUpdatedList { get; set; }

    [DataMember]
    public IEnumerable<string> WorkItemDestroyedList { get; set; }

    [DataMember]
    public DateTime EventTime { get; set; }

    public override string ToString()
    {
      string[] strArray = new string[8]
      {
        "ProjectId = ",
        this.ProjectId.ToString(),
        ", WorkItemUpdatedList = [",
        this.WorkItemUpdatedList == null ? "" : string.Join(",", this.WorkItemUpdatedList),
        "], WorkItemDestroyedList = [",
        this.WorkItemDestroyedList == null ? "" : string.Join(",", this.WorkItemDestroyedList),
        "], EventTime = ",
        null
      };
      DateTime eventTime = this.EventTime;
      strArray[7] = this.EventTime.ToString();
      return string.Concat(strArray);
    }
  }
}
