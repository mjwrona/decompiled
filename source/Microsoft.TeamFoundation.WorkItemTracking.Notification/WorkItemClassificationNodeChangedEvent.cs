// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemClassificationNodeChangedEvent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Notification, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 45693437-E9C3-448A-85F4-93C2412459C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Notification.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Notification
{
  [DataContract]
  public class WorkItemClassificationNodeChangedEvent
  {
    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public EventType EventType { get; set; }

    [DataMember]
    public int NodeId { get; set; }

    [DataMember]
    public ClassificationNodeType NodeType { get; set; }

    [DataMember]
    public string CurrentNodePath { get; set; }

    [DataMember]
    public string NewNodeName { get; set; }

    [DataMember]
    public string NewNodePath { get; set; }

    [DataMember]
    public DateTime? StartDate { get; set; }

    [DataMember]
    public DateTime? FinishDate { get; set; }
  }
}
