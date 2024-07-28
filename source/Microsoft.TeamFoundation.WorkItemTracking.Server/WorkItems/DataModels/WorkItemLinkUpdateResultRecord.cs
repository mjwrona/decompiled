// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkUpdateResultRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal struct WorkItemLinkUpdateResultRecord
  {
    public int Order { get; set; }

    public LinkUpdateType UpdateType { get; set; }

    public LinkUpdateType? UpdateTypeExecuted { get; set; }

    public int Status { get; set; }

    public int SourceId { get; set; }

    public int TargetId { get; set; }

    public int DataspaceId { get; set; }

    public int TargetDataspaceId { get; set; }

    public int LinkType { get; set; }

    public long Timestamp { get; set; }

    public Guid AuthorizedByTfid { get; set; }

    public string Comment { get; set; }

    public Guid? RemoteHostId { get; set; }

    public Guid? RemoteProjectId { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus? RemoteStatus { get; set; }

    public string RemoteStatusMessage { get; set; }

    public long? RemoteWatermark { get; set; }

    public Guid LocalProjectId { get; set; }

    public DateTime AuthorizedDate { get; set; }
  }
}
