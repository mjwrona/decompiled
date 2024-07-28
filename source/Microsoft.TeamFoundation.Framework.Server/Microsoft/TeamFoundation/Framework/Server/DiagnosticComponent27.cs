// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent27
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent27 : DiagnosticComponent26
  {
    public override List<SqlRowLockInfo> QueryRowLockInfo(List<string> hobtIds)
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryRowLockInfo");
      this.BindStringTable("@hobtIdTable", (IEnumerable<string>) hobtIds, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SqlRowLockInfo>((ObjectBinder<SqlRowLockInfo>) new SqlRowLockInfoBinder());
        return resultCollection.GetCurrent<SqlRowLockInfo>().Items;
      }
    }
  }
}
