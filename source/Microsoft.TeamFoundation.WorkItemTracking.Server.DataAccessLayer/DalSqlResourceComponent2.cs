// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent2 : DalSqlResourceComponent
  {
    public override void AddNewBuild(
      string buildName,
      string project,
      string buildDefinitionName,
      int maxBuildListSize)
    {
      this.PrepareStoredProcedure(nameof (AddNewBuild));
      this.BindString("@BuildName", buildName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@Project", project, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@BuildDefinitionName", buildDefinitionName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindInt("@MaxBuildListCount", maxBuildListSize);
      this.ExecuteNonQuery();
    }

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
        this.BindString("@userSID", this.RequestContext.UserContext.Identifier, 256, false, SqlDbType.NVarChar);
        IDataReader reader = this.ExecuteReader();
        rc = new ResultCollection(reader, this.ProcedureName, this.RequestContext);
        rc.AddBinder<WorkItemLinkChange>((ObjectBinder<WorkItemLinkChange>) this.GetWorkItemLinkChangeBinder((PermissionCheckHelper) null));
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }
  }
}
