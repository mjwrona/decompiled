// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactWorkItemIdsBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ArtifactWorkItemIdsBinder : ObjectBinder<ArtifactWorkItemIds>
  {
    private SqlColumnBinder uri = new SqlColumnBinder("UriId");
    private SqlColumnBinder workItemId = new SqlColumnBinder("WorkItemID");
    private SqlColumnBinder areaId = new SqlColumnBinder("System.AreaId");
    private PermissionCheckHelper m_permissionCheckHelper;

    internal ArtifactWorkItemIdsBinder(PermissionCheckHelper permissionCheckHelper) => this.m_permissionCheckHelper = permissionCheckHelper;

    protected override ArtifactWorkItemIds Bind()
    {
      ArtifactWorkItemIds artifactWorkItemIds = new ArtifactWorkItemIds();
      artifactWorkItemIds.WorkItemIds = new List<int>();
      artifactWorkItemIds.UriListOffset = this.uri.GetInt32((IDataReader) this.Reader);
      int int32 = this.workItemId.GetInt32((IDataReader) this.Reader);
      if (this.m_permissionCheckHelper == null || this.m_permissionCheckHelper.HasWorkItemPermission(this.areaId.GetInt32((IDataReader) this.Reader), 16))
        artifactWorkItemIds.WorkItemIds.Add(int32);
      return artifactWorkItemIds;
    }
  }
}
