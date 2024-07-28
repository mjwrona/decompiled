// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent18
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent18 : DiagnosticComponent17
  {
    public override List<TableName> QueryEmptyTables()
    {
      string str = "DIAGNOSTIC.prc_QueryEmptyTables";
      this.PrepareStoredProcedure(str);
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, str, this.RequestContext))
        {
          resultCollection.AddBinder<TableName>((ObjectBinder<TableName>) new TableNameBinder());
          return resultCollection.GetCurrent<TableName>().Items;
        }
      }
    }
  }
}
