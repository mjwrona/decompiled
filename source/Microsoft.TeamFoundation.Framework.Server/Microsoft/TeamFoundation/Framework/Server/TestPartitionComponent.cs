// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TestPartitionComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TestPartitionComponent : TeamFoundationSqlResourceComponent
  {
    public void SetupTestPartition()
    {
      if (this.IsSqlAzure)
        return;
      this.PrepareStoredProcedure("prc_SetupTestPartition");
      this.ExecuteNonQuery();
    }

    public virtual string GetOfflinePartitionFilePath()
    {
      if (!this.IsSqlAzure)
      {
        string sqlStatement = "SELECT  STUFF(physical_name, LEN(physical_name) - 2, 3, 'bak')\r\n                                      FROM    sys.database_files \r\n                                      WHERE   name = 'LeadingKey'\r\n                                               AND state_desc = 'OFFLINE'";
        this.PrepareSqlBatch(sqlStatement.Length);
        this.AddStatement(sqlStatement);
        using (SqlDataReader sqlDataReader = this.ExecuteReader())
        {
          if (sqlDataReader.Read())
            return sqlDataReader.GetString(0);
        }
      }
      return (string) null;
    }

    public void DisableTestPartition(List<string> tablesToDisable, bool restoreFileGroup = true)
    {
      if (this.IsSqlAzure)
        return;
      if (restoreFileGroup)
      {
        string partitionFilePath = this.GetOfflinePartitionFilePath();
        if (!string.IsNullOrEmpty(partitionFilePath))
        {
          using (RestoreFileComponent componentRaw = this.ConnectionInfo.CloneReplaceInitialCatalog(TeamFoundationSqlResourceComponent.Master).CreateComponentRaw<RestoreFileComponent>())
            componentRaw.RestoreDatabase(this.ConnectionInfo.InitialCatalog, "LeadingKey", partitionFilePath);
        }
      }
      this.PrepareStoredProcedure("prc_DisableTestPartition");
      if (tablesToDisable != null)
        this.BindStringTable("@tables", (IEnumerable<string>) tablesToDisable);
      this.ExecuteNonQuery();
    }
  }
}
