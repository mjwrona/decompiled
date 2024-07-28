// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.DataspaceMigrationComponent
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class DataspaceMigrationComponent : TeamFoundationSqlResourceComponent
  {
    public List<TableSizeDataspaceInfoProperties> GetTableSizesWithDataspaceInfo()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetTableSizesAndDataspaceInfo.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TableSizeDataspaceInfoProperties>((ObjectBinder<TableSizeDataspaceInfoProperties>) new TableSizeDataspaceInfoColumns());
      return resultCollection.GetCurrent<TableSizeDataspaceInfoProperties>().Items;
    }
  }
}
