// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemLinkChangeBinder2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemLinkChangeBinder2 : WorkItemLinkChangeBinder
  {
    private SqlColumnBinder RemoteHostId = new SqlColumnBinder(nameof (RemoteHostId));
    private SqlColumnBinder RemoteProjectId = new SqlColumnBinder(nameof (RemoteProjectId));
    private SqlColumnBinder RemoteStatus = new SqlColumnBinder(nameof (RemoteStatus));
    private SqlColumnBinder RemoteStatusMessage = new SqlColumnBinder(nameof (RemoteStatusMessage));

    internal WorkItemLinkChangeBinder2(IPermissionCheckHelper permissionCheckHelper)
      : base(permissionCheckHelper)
    {
    }

    protected override WorkItemLinkChange Bind()
    {
      WorkItemLinkChange workItemLinkChange = base.Bind();
      workItemLinkChange.RemoteHostId = new Guid?(this.RemoteHostId.GetGuid((IDataReader) this.Reader, true));
      workItemLinkChange.RemoteProjectId = new Guid?(this.RemoteProjectId.GetGuid((IDataReader) this.Reader, true));
      byte? nullableByte = this.RemoteStatus.GetNullableByte((IDataReader) this.Reader);
      workItemLinkChange.RemoteStatus = nullableByte.HasValue ? new Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus?((Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus) nullableByte.GetValueOrDefault()) : new Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus?();
      workItemLinkChange.RemoteStatusMessage = this.RemoteStatusMessage.GetString((IDataReader) this.Reader, true);
      return workItemLinkChange;
    }
  }
}
