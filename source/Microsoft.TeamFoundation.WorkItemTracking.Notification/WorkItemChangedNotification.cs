// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemChangedNotification
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Notification, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 45693437-E9C3-448A-85F4-93C2412459C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Notification.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Notification
{
  [DataContract]
  [ServiceEventObject]
  public class WorkItemChangedNotification
  {
    [DataMember]
    public List<WorkItemChangedProjectEvent> Events { get; set; }

    public override string ToString()
    {
      List<string> values = new List<string>();
      foreach (WorkItemChangedProjectEvent changedProjectEvent in this.Events)
        values.Add("workItemChangedProjectEvent = [" + changedProjectEvent.ToString() + "]");
      return string.Join(",", (IEnumerable<string>) values);
    }
  }
}
