// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CollationComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CollationComponent : TeamFoundationSqlResourceComponent
  {
    public bool IsValidCollation(string collation)
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_CheckCollation.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@collation", collation, 128, false, SqlDbType.NVarChar);
      return this.ExecuteScalar() != DBNull.Value;
    }

    public virtual string GetDatabaseCollation()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseCollation.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      return (string) this.ExecuteScalar();
    }

    public int GetDatabaseLcid()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseLcid.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      return (int) this.ExecuteScalar();
    }

    public ResultCollection GetCollations()
    {
      string str = "stmt_GetValidCollations.sql";
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(str);
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection collations = new ResultCollection((IDataReader) this.ExecuteReader(), str, (IVssRequestContext) null);
      collations.AddBinder<string>((ObjectBinder<string>) new CollationName());
      return collations;
    }
  }
}
