// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemLinkChangeBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemLinkChangeBinder : ObjectBinder<WorkItemLinkChange>
  {
    private SqlColumnBinder sourceID = new SqlColumnBinder("SourceID");
    private SqlColumnBinder targetID = new SqlColumnBinder("TargetID");
    private SqlColumnBinder linkType = new SqlColumnBinder("LinkType");
    private SqlColumnBinder fDeleted = new SqlColumnBinder(nameof (fDeleted));
    private SqlColumnBinder changedDate = new SqlColumnBinder("ChangedDate");
    private SqlColumnBinder rowVersion = new SqlColumnBinder("RowVersion");
    private SqlColumnBinder sourceAreaId = new SqlColumnBinder("SourceAreaId");
    private SqlColumnBinder targetAreaId = new SqlColumnBinder("TargetAreaId");
    private SqlColumnBinder linkTypeId = new SqlColumnBinder("LinkTypeId");
    private SqlColumnBinder changedBy_Name = new SqlColumnBinder("ChangedBy_DisplayPart");
    private SqlColumnBinder changedBy_TfId = new SqlColumnBinder("ChangedBy_TeamFoundationId");
    private SqlColumnBinder comment = new SqlColumnBinder("Comment");
    private SqlColumnBinder sourceDataspaceId = new SqlColumnBinder("SourceDataspaceId");
    private SqlColumnBinder targetDataspaceId = new SqlColumnBinder("TargetDataspaceId");
    private IPermissionCheckHelper m_permissionCheckHelper;

    internal WorkItemLinkChangeBinder(IPermissionCheckHelper permissionCheckHelper) => this.m_permissionCheckHelper = permissionCheckHelper;

    protected override WorkItemLinkChange Bind()
    {
      WorkItemLinkChange workItemLinkChange = new WorkItemLinkChange();
      if (this.m_permissionCheckHelper != null && !this.m_permissionCheckHelper.HasWorkItemPermission(this.sourceAreaId.GetInt32((IDataReader) this.Reader), 16) && !this.m_permissionCheckHelper.HasWorkItemPermission(this.targetAreaId.GetInt32((IDataReader) this.Reader), 16))
      {
        workItemLinkChange.PassedPermissionCheck = false;
        workItemLinkChange.RowVersion = this.rowVersion.GetInt64((IDataReader) this.Reader);
        workItemLinkChange.IsActive = !this.fDeleted.GetBoolean((IDataReader) this.Reader);
        workItemLinkChange.ChangedDate = this.changedDate.GetDateTime((IDataReader) this.Reader);
        return workItemLinkChange;
      }
      workItemLinkChange.PassedPermissionCheck = true;
      workItemLinkChange.SourceID = this.sourceID.GetInt32((IDataReader) this.Reader);
      workItemLinkChange.TargetID = this.targetID.GetInt32((IDataReader) this.Reader);
      workItemLinkChange.LinkType = this.linkType.GetString((IDataReader) this.Reader, false);
      workItemLinkChange.LinkTypeId = (int) this.linkTypeId.GetInt16((IDataReader) this.Reader, (short) 0, (short) 0);
      workItemLinkChange.IsActive = !this.fDeleted.GetBoolean((IDataReader) this.Reader);
      workItemLinkChange.ChangedDate = this.changedDate.GetDateTime((IDataReader) this.Reader);
      workItemLinkChange.ChangedBy_Name = this.changedBy_Name.GetString((IDataReader) this.Reader, (string) null);
      workItemLinkChange.ChangedBy_TfId = this.changedBy_TfId.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      workItemLinkChange.Comment = this.comment.GetString((IDataReader) this.Reader, (string) null);
      workItemLinkChange.RowVersion = this.rowVersion.GetInt64((IDataReader) this.Reader);
      workItemLinkChange.RowVersion = this.rowVersion.GetInt64((IDataReader) this.Reader);
      workItemLinkChange.SourceDataspaceId = this.sourceDataspaceId.GetInt32((IDataReader) this.Reader, 0, 0);
      workItemLinkChange.TargetDataspaceId = this.targetDataspaceId.GetInt32((IDataReader) this.Reader, 0, 0);
      return workItemLinkChange;
    }
  }
}
