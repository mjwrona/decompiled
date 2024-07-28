// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetWorkItemElementBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal abstract class DalGetWorkItemElementBase : DalSqlElement
  {
    private bool? m_hasPermission;
    private int m_areaId;
    protected int m_areaIdTableCount;

    protected void InitializePermissionCheck(bool skipPermissionCheck)
    {
      if (this.Version < 8 | skipPermissionCheck)
      {
        this.m_hasPermission = new bool?(true);
      }
      else
      {
        if (this.m_areaIdTableCount <= 0)
          return;
        PayloadTable table = this.SqlBatch.ResultPayload.Tables[this.m_index];
        this.SqlBatch.ResultPayload.Tables.Remove(table);
        if (table.RowCount > 0)
          this.m_areaId = (int) table.Rows[0]["LatestAreaId"];
        else
          this.m_hasPermission = new bool?(false);
      }
    }

    protected bool FilterResultTable(IVssRequestContext requestContext, PayloadTable table)
    {
      if (!this.m_hasPermission.HasValue)
      {
        if (this.m_areaIdTableCount == 0 && table.RowCount > 0)
          this.m_areaId = (int) table.Rows[0]["System.AreaId"];
        this.m_hasPermission = this.m_areaId <= 0 ? new bool?(false) : new bool?(new PermissionCheckHelper(requestContext).HasWorkItemPermission(this.m_areaId, 16));
      }
      if (!this.m_hasPermission.Value)
        table.ClearRows();
      return this.m_hasPermission.Value;
    }
  }
}
