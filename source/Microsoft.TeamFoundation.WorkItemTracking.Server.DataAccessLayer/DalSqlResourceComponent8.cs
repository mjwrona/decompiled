// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent8
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent8 : DalSqlResourceComponent7
  {
    public override void GetWorkItemLinkChanges(
      long rowVersion,
      bool bypassPermissions,
      int batchSize,
      out ResultCollection rc)
    {
      try
      {
        this.PrepareStoredProcedure(nameof (GetWorkItemLinkChanges), 3600);
        this.BindLong("@rowVersion", rowVersion);
        this.BindBoolean("@bypassPermissions", bypassPermissions);
        IDataReader reader = this.ExecuteReader();
        PermissionCheckHelper helper = (PermissionCheckHelper) null;
        if (!bypassPermissions)
          helper = new PermissionCheckHelper(this.RequestContext);
        rc = new ResultCollection(reader, this.ProcedureName, this.RequestContext);
        rc.AddBinder<WorkItemLinkChange>((ObjectBinder<WorkItemLinkChange>) this.GetWorkItemLinkChangeBinder(helper));
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }
  }
}
