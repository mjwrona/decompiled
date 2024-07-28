// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent4 : DiagnosticComponent3
  {
    internal virtual List<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> ReadResults(
      IDataReader reader)
    {
      using (ResultCollection resultCollection = new ResultCollection(reader, this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new DiagnosticComponent4.ServiceObjectiveBinder());
        string serviceObjective = resultCollection.GetCurrent<string>().FirstOrDefault<string>();
        resultCollection.NextResult();
        resultCollection.AddBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>((ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>) new DiagnosticComponent.DatabasePerformanceStatisticsViewBinder(serviceObjective));
        return resultCollection.GetCurrent<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>().Items;
      }
    }

    public override List<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> QueryDatabasePerformanceStatistics(
      int databaseId,
      DateTime? lastProcessedTimeStamp,
      int maxRecordCount,
      int periodInMinutes)
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryDatabasePerformanceStatistics");
      this.BindNullableDateTime2("@lastProcessedTimestamp", lastProcessedTimeStamp);
      this.BindInt("@maxRecordCount", maxRecordCount);
      this.BindInt("@periodInMinutes", periodInMinutes);
      return this.ReadResults((IDataReader) this.ExecuteReader());
    }

    protected class ServiceObjectiveBinder : ObjectBinder<string>
    {
      private SqlColumnBinder ServiceObjective = new SqlColumnBinder(nameof (ServiceObjective));

      protected override string Bind()
      {
        string str = this.ServiceObjective.GetString((IDataReader) this.Reader, true);
        return str != null ? string.Intern(str) : string.Empty;
      }
    }
  }
}
