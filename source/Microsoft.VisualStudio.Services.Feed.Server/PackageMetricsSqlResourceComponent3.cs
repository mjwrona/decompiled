// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageMetricsSqlResourceComponent3
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageMetricsSqlResourceComponent3 : PackageMetricsSqlResourceComponent2
  {
    protected virtual SqlMetaData[] Typ_RawMetrics2 => new SqlMetaData[7]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Protocol", SqlDbType.NVarChar, 20L),
      new SqlMetaData("FeedId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NormalizedPackageName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("NormalizedPackageVersion", SqlDbType.NVarChar, (long) sbyte.MaxValue),
      new SqlMetaData("RawMetricType", SqlDbType.TinyInt),
      new SqlMetaData("MetricData", SqlDbType.NVarChar, -1L)
    };

    public override void IngestRawMetrics(IEnumerable<PackageMetricsData> rawMetrics)
    {
      this.PrepareStoredProcedure("Feed.prc_InsertRawMetrics");
      this.BindRawMetricsTable("@rawMetrics", rawMetrics);
      this.ExecuteNonQuery();
    }

    protected override SqlParameter BindRawMetricsTable(
      string parameterName,
      IEnumerable<PackageMetricsData> rawMetrics)
    {
      rawMetrics = rawMetrics ?? Enumerable.Empty<PackageMetricsData>();
      System.Func<PackageMetricsData, SqlDataRecord> selector = (System.Func<PackageMetricsData, SqlDataRecord>) (metric =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.Typ_RawMetrics2);
        sqlDataRecord.SetInt32(0, this.GetFeedDataspaceId(metric.ProjectId));
        sqlDataRecord.SetString(1, metric.Protocol);
        sqlDataRecord.SetGuid(2, metric.FeedId);
        sqlDataRecord.SetString(3, metric.NormalizedPackageName);
        sqlDataRecord.SetString(4, metric.NormalizedPackageVersion);
        sqlDataRecord.SetByte(5, (byte) metric.RawMetricType);
        sqlDataRecord.SetString(6, JsonConvert.SerializeObject((object) metric.RawMetricData));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Feed.typ_RawMetrics2", rawMetrics.Select<PackageMetricsData, SqlDataRecord>(selector));
    }

    protected override void BindFeedIdentity(
      FeedIdentity feedId,
      bool createMissingDataspace = false,
      string dataspaceName = "@dataspaceId",
      string feedIdName = "@feedId")
    {
      this.BindInt(dataspaceName, this.GetFeedDataspaceId(feedId.ProjectId));
      this.BindGuid(feedIdName, feedId.Id);
    }
  }
}
