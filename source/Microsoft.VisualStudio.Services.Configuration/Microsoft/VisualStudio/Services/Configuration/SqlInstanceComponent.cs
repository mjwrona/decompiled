// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlInstanceComponent
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SqlInstanceComponent : TeamFoundationSqlResourceComponent
  {
    public SqlInstanceProperties GetSqlInstanceProperties()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetSqlInstanceProperties.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "stmt_GetSqlInstanceProperties.sql", (IVssRequestContext) null);
      resultCollection.AddBinder<SqlInstanceProperties>((ObjectBinder<SqlInstanceProperties>) new SqlInstancePropertiesColumns());
      return resultCollection.GetCurrent<SqlInstanceProperties>().Single<SqlInstanceProperties>();
    }

    public List<string> GetFiles(string path) => this.GetDirectoryContents(path, 1);

    public List<string> GetFolders(string path) => this.GetDirectoryContents(path, 0);

    private List<string> GetDirectoryContents(string path, int includeFiles)
    {
      this.PrepareStoredProcedure("xp_dirtree");
      this.BindString("@path", path, path.Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@maxDepth", 1);
      this.BindInt("@includeFiles", includeFiles);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "xp_dirtree", (IVssRequestContext) null);
      resultCollection.AddBinder<DirTreeInfo>((ObjectBinder<DirTreeInfo>) new DirTreeInfoColumns());
      return resultCollection.GetCurrent<DirTreeInfo>().Where<DirTreeInfo>((System.Func<DirTreeInfo, bool>) (info => info.File == includeFiles)).Select<DirTreeInfo, string>((System.Func<DirTreeInfo, string>) (info => info.SubDirectory)).ToList<string>();
    }
  }
}
