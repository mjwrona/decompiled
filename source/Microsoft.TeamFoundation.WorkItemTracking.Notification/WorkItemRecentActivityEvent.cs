// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemRecentActivityEvent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Notification, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 45693437-E9C3-448A-85F4-93C2412459C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Notification.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Notification
{
  [DataContract]
  public class WorkItemRecentActivityEvent
  {
    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public Guid UserId { get; set; }

    [DataMember]
    public Dictionary<int, int> WorkItemIdsAndAreaIds { get; set; }

    public override string ToString() => "ProjectId = " + this.ProjectId.ToString() + ", UserId = " + this.UserId.ToString() + ", WorkItemIdsAndAreaIds = [" + (this.WorkItemIdsAndAreaIds == null ? "" : string.Join<KeyValuePair<int, int>>(",", (IEnumerable<KeyValuePair<int, int>>) this.WorkItemIdsAndAreaIds)) + "], ";
  }
}
