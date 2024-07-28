// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent3 : DatabaseManagementComponent2
  {
    public override List<InternalDatabaseProperties> QueryDatabases(string poolName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(poolName, nameof (poolName));
      poolName = this.GetLegacyPoolName(poolName);
      this.PrepareStoredProcedure("prc_QueryDatabasesByPoolName");
      this.BindString("@poolName", poolName, 256, false, SqlDbType.VarChar);
      return this.CreatePropertiesBinder(this.ExecuteReader(), this.ProcedureName).Items;
    }

    public override List<InternalDatabaseProperties> QueryDatabases(
      TeamFoundationDatabaseType databaseType)
    {
      int legacyDatabaseType = this.GetLegacyDatabaseType(databaseType);
      this.PrepareStoredProcedure("prc_QueryDatabasesByType");
      this.BindInt("@databaseType", legacyDatabaseType);
      return this.CreatePropertiesBinder(this.ExecuteReader(), this.ProcedureName).Items;
    }
  }
}
