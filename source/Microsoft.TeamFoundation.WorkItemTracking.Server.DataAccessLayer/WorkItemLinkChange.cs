// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemLinkChange
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemLinkChange : ICacheable
  {
    internal WorkItemLinkChange()
    {
    }

    public int SourceID { get; set; }

    public int TargetID { get; set; }

    public string LinkType { get; set; }

    public int LinkTypeId { get; set; }

    public string LinkTypeString { get; set; }

    public bool IsActive { get; set; }

    public DateTime ChangedDate { get; set; }

    public Guid ChangedBy_TfId { get; set; }

    public string ChangedBy_Name { get; set; }

    public string Comment { get; set; }

    public long RowVersion { get; set; }

    public int SourceDataspaceId { get; set; }

    public int TargetDataspaceId { get; set; }

    public Guid SourceProjectId { get; set; }

    public Guid TargetProjectId { get; set; }

    public Guid? RemoteHostId { get; set; }

    public Guid? RemoteProjectId { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus? RemoteStatus { get; set; }

    public string RemoteStatusMessage { get; set; }

    internal bool PassedPermissionCheck { get; set; }

    public int GetCachedSize() => 200;
  }
}
