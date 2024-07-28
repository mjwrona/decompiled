// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageMetricsSqlResourceComponent4
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageMetricsSqlResourceComponent4 : PackageMetricsSqlResourceComponent3
  {
    public override int AggregateUserDownloadMetrics(int batchSize)
    {
      this.PrepareStoredProcedure("Feed.prc_TransferRawDownloadMetricsIntoStats", false);
      this.BindInt("@batchSize", batchSize);
      return this.ReadProcessedRows();
    }

    protected virtual int ReadProcessedRows()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SingleInt32ValueBinder("RowsProcessed"));
        return resultCollection.GetCurrent<int>().Items[0];
      }
    }
  }
}
