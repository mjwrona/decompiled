// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdateResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  [System.Diagnostics.DebuggerDisplay("DebuggerDisplay,nq")]
  public class WorkItemLinkUpdateResult : LinkUpdateResult
  {
    public int TargetWorkItemId { get; set; }

    public int LinkType { get; set; }

    public LinkUpdateType? UpdateTypeExecuted { get; set; }

    public Guid? RemoteHostId { get; set; }

    public Guid? RemoteProjectId { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus? RemoteStatus { get; set; }

    public string RemoteStatusMessage { get; set; }

    public long? RemoteWatermark { get; set; }

    public long Timestamp { get; set; }

    public string RemoteWorkItemTitle { get; set; }

    public string RemoteWorkItemType { get; set; }

    protected override string DebuggerDisplay => string.Format("{0}, TargetWorkItemId = {1}, LinkType = {2}, UpdateTypeExecuted = {3}, ", (object) base.DebuggerDisplay, (object) this.TargetWorkItemId, (object) this.LinkType, (object) this.UpdateTypeExecuted) + string.Format("Timestamp = {0}, RemoteHostId = {1}, RemoteProjectId = {2}, RemoteStatus = {3}, RemoteWatermark = {4}", (object) this.Timestamp, (object) this.RemoteHostId, (object) this.RemoteProjectId, (object) this.RemoteStatus, (object) this.RemoteWatermark) + "RemoteWorkItemTitle = " + this.RemoteWorkItemTitle + ", RemoteWorkItemType = " + this.RemoteWorkItemType;
  }
}
