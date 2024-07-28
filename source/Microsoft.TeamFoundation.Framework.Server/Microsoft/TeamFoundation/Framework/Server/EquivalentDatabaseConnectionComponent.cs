// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.EquivalentDatabaseConnectionComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class EquivalentDatabaseConnectionComponent : TeamFoundationSqlResourceComponent
  {
    public bool GetAppLock(Guid uniqueGuid)
    {
      if (this.Connection.State != ConnectionState.Open)
        this.Connection.Open();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_CheckEquivalentConnection.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@resource", string.Format("EquivalentDatabaseCheck:{0}", (object) uniqueGuid), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return (int) this.ExecuteScalar() > -1;
    }

    protected override ISqlConnectionInfo PrepareConnectionString(ISqlConnectionInfo connectionInfo)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionInfo.ConnectionString)
      {
        Pooling = false
      };
      return connectionInfo.Create(connectionStringBuilder.ConnectionString);
    }
  }
}
