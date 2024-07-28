// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RestoreFileComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RestoreFileComponent : TeamFoundationSqlResourceComponent
  {
    public void RestoreDatabase(string databaseName, string fileName, string pathOnDisk)
    {
      if (!string.Equals(this.InitialCatalog, TeamFoundationSqlResourceComponent.Master, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(this.InitialCatalog))
        throw new InvalidOperationException(FrameworkResources.MethodCanOnlyBeExecutedOnMaster());
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_RestoreFile.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@dbName", databaseName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@fileName", fileName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@path", pathOnDisk, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
