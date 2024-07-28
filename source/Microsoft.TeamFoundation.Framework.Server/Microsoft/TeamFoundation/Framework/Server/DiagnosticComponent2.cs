// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent2 : DiagnosticComponent
  {
    public override List<DatabaseManagementViewResult> QueryWhatsRunning()
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryWhatsRunning");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DatabaseManagementViewResult>((ObjectBinder<DatabaseManagementViewResult>) new DMVBinder());
        return resultCollection.GetCurrent<DatabaseManagementViewResult>().Items;
      }
    }

    public override DatabaseResourceStats QueryDbmsResourceStats(int samples)
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryDbmsResourceStats");
      this.BindDatabaseResourceStatsParameters(samples);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DatabaseResourceStats>((ObjectBinder<DatabaseResourceStats>) this.CreateDatabaseResourceStatsBinder());
        return resultCollection.GetCurrent<DatabaseResourceStats>().Items.FirstOrDefault<DatabaseResourceStats>();
      }
    }

    protected virtual DatabaseResourceStatsBinder CreateDatabaseResourceStatsBinder() => new DatabaseResourceStatsBinder();

    protected virtual void BindDatabaseResourceStatsParameters(int samples)
    {
    }
  }
}
