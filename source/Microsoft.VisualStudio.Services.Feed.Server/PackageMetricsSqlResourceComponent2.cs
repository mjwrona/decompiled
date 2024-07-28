// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageMetricsSqlResourceComponent2
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageMetricsSqlResourceComponent2 : PackageMetricsSqlResourceComponent
  {
    protected virtual SqlMetaData[] Typ_UniqueIdentifierList => new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
    };

    public override int AggregateUserDownloadMetrics(int batchSize)
    {
      this.PrepareStoredProcedure("Feed.prc_TransferRawDownloadMetricsIntoStats", false);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
      return 0;
    }

    public override IEnumerable<PackageVersionMetrics> QueryPackageVersionMetrics(
      FeedIdentity feedIdentity,
      Guid packageId,
      IEnumerable<Guid> versionIds)
    {
      this.PrepareStoredProcedure("Feed.prc_QueryPackageVersionMetrics");
      this.BindFeedIdentity(feedIdentity);
      this.BindGuid("@packageId", packageId);
      this.BindUniqueIdentifierList("@packageVersionIds", versionIds);
      return this.ReadPackageVersionMetrics();
    }

    public override IEnumerable<PackageMetrics> QueryPackageMetrics(
      FeedIdentity feedIdentity,
      IEnumerable<Guid> packageIds)
    {
      this.PrepareStoredProcedure("Feed.prc_QueryPackageMetrics");
      this.BindFeedIdentity(feedIdentity);
      this.BindUniqueIdentifierList("@packageIds", packageIds);
      return this.ReadPackageMetrics();
    }

    protected virtual SqlParameter BindUniqueIdentifierList(
      string parameterName,
      IEnumerable<Guid> ids)
    {
      ids = ids ?? Enumerable.Empty<Guid>();
      System.Func<Guid, SqlDataRecord> selector = (System.Func<Guid, SqlDataRecord>) (id =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.Typ_UniqueIdentifierList);
        sqlDataRecord.SetGuid(0, id);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Feed.Typ_UniqueIdentifierList", ids.Select<Guid, SqlDataRecord>(selector));
    }

    protected IEnumerable<PackageVersionMetrics> ReadPackageVersionMetrics()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<QueryPackageVersionMetricsRow>((ObjectBinder<QueryPackageVersionMetricsRow>) new QueryPackageVersionMetricsBinder());
        return (IEnumerable<PackageVersionMetrics>) resultCollection.GetCurrent<QueryPackageVersionMetricsRow>().Select<QueryPackageVersionMetricsRow, PackageVersionMetrics>((System.Func<QueryPackageVersionMetricsRow, PackageVersionMetrics>) (packageMetric =>
        {
          PackageVersionMetrics packageVersionMetrics = JsonConvert.DeserializeObject<PackageVersionMetrics>(packageMetric.Metrics);
          packageVersionMetrics.PackageId = packageMetric.PackageId;
          packageVersionMetrics.PackageVersionId = packageMetric.PackageVersionId;
          return packageVersionMetrics;
        })).ToList<PackageVersionMetrics>();
      }
    }

    protected IEnumerable<PackageMetrics> ReadPackageMetrics()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<QueryPackageMetricsRow>((ObjectBinder<QueryPackageMetricsRow>) new QueryPackageMetricsBinder());
        return (IEnumerable<PackageMetrics>) resultCollection.GetCurrent<QueryPackageMetricsRow>().Select<QueryPackageMetricsRow, PackageMetrics>((System.Func<QueryPackageMetricsRow, PackageMetrics>) (packageMetric =>
        {
          PackageMetrics packageMetrics = JsonConvert.DeserializeObject<PackageMetrics>(packageMetric.Metrics);
          packageMetrics.PackageId = packageMetric.PackageId;
          return packageMetrics;
        })).ToList<PackageMetrics>();
      }
    }
  }
}
