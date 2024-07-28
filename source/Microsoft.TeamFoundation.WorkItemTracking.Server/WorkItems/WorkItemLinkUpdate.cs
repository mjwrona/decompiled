// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  [System.Diagnostics.DebuggerDisplay("DebuggerDisplay,nq")]
  public class WorkItemLinkUpdate : LinkUpdate
  {
    public int TargetWorkItemId { get; set; }

    public int LinkType { get; set; }

    public bool? Locked { get; set; }

    public Guid? RemoteHostId { get; set; }

    public Guid? RemoteProjectId { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus? RemoteStatus { get; set; }

    public string RemoteStatusMessage { get; set; }

    public long? RemoteWatermark { get; set; }

    public string RemoteUrl { get; set; }

    public string RemoteWorkItemTitle { get; set; }

    public string RemoteWorkItemType { get; set; }

    public long TimeStamp { get; set; }

    protected override string DebuggerDisplay => string.Format("{0}, TargetWorkItemId = {1}, LinkType = {2}, Locked = {3}, ", (object) base.DebuggerDisplay, (object) this.TargetWorkItemId, (object) this.LinkType, (object) this.Locked) + string.Format("RemoteHostId = {0}, RemoteProjectId = {1}, RemoteStatus = {2}, RemoteWatermark = {3},  RemoteUrl = {4}, RemoteWorkItemTitle = {5}, RemoteWorkItemType = {6}, TimeStamp = {7}", (object) this.RemoteHostId, (object) this.RemoteProjectId, (object) this.RemoteStatus, (object) this.RemoteWatermark, (object) this.RemoteUrl, (object) this.RemoteWorkItemTitle, (object) this.RemoteWorkItemType, (object) this.TimeStamp);
  }
}
