// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent7
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewComponent7 : ScaledAgileViewComponent6
  {
    internal override IReadOnlyList<ShallowScaledAgileViewRecord> GetViewDefinitions(
      IReadOnlyList<Guid> planIds)
    {
      this.PrepareStoredProcedure("prc_GetScaledAgileViewsById");
      this.BindScaledAgileViewIds("@ids", planIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowScaledAgileViewRecord>((ObjectBinder<ShallowScaledAgileViewRecord>) this.GetViewDefinitionsRecordBinder());
        return (IReadOnlyList<ShallowScaledAgileViewRecord>) resultCollection.GetCurrent<ShallowScaledAgileViewRecord>().Items;
      }
    }

    protected virtual SqlParameter BindScaledAgileViewIds(
      string parameterName,
      IReadOnlyList<Guid> planIds)
    {
      return this.BindGuidTable(parameterName, (IEnumerable<Guid>) planIds);
    }
  }
}
