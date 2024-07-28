// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageMetricsSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageMetricsSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public const int PackageIdMaxLength = 255;
    public const int PackageVersionIdMaxLength = 127;
    public const int MetricDataMaxLength = -1;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<PackageMetricsSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<PackageMetricsSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<PackageMetricsSqlResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<PackageMetricsSqlResourceComponent4>(4)
    }, "PackageMetrics");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    protected virtual SqlMetaData[] Typ_RawMetrics => new SqlMetaData[6]
    {
      new SqlMetaData("Protocol", SqlDbType.NVarChar, 20L),
      new SqlMetaData("FeedId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NormalizedPackageName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("NormalizedPackageVersion", SqlDbType.NVarChar, (long) sbyte.MaxValue),
      new SqlMetaData("RawMetricType", SqlDbType.TinyInt),
      new SqlMetaData("MetricData", SqlDbType.NVarChar, -1L)
    };

    protected virtual SqlMetaData[] Typ_PackageVersionStatsQuery => new SqlMetaData[4]
    {
      new SqlMetaData("FeedId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("PackageId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("PackageVersionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("MetricType", SqlDbType.TinyInt)
    };

    public PackageMetricsSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PackageMetricsSqlResourceComponent.sqlExceptionFactories;

    public virtual void IngestRawMetrics(IEnumerable<PackageMetricsData> rawMetrics)
    {
      this.PrepareStoredProcedure("Feed.prc_InsertRawMetrics");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindRawMetricsTable("@rawMetrics", rawMetrics);
      this.ExecuteNonQuery();
    }

    public virtual int AggregateUserDownloadMetrics(int batchSize)
    {
      this.PrepareStoredProcedure("Feed.prc_AggregateDownloadMetrics", false);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
      return 0;
    }

    public virtual IEnumerable<PackageVersionMetrics> QueryPackageVersionMetrics(
      FeedIdentity feedIdentity,
      Guid packageId,
      IEnumerable<Guid> versionIds)
    {
      return (IEnumerable<PackageVersionMetrics>) new List<PackageVersionMetrics>();
    }

    public virtual IEnumerable<PackageMetrics> QueryPackageMetrics(
      FeedIdentity feedIdentity,
      IEnumerable<Guid> packageIds)
    {
      return (IEnumerable<PackageMetrics>) new List<PackageMetrics>();
    }

    protected virtual SqlParameter BindRawMetricsTable(
      string parameterName,
      IEnumerable<PackageMetricsData> rawMetrics)
    {
      rawMetrics = rawMetrics ?? Enumerable.Empty<PackageMetricsData>();
      System.Func<PackageMetricsData, SqlDataRecord> selector = (System.Func<PackageMetricsData, SqlDataRecord>) (metric =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.Typ_RawMetrics);
        sqlDataRecord.SetString(0, metric.Protocol);
        sqlDataRecord.SetGuid(1, metric.FeedId);
        sqlDataRecord.SetString(2, metric.NormalizedPackageName);
        sqlDataRecord.SetString(3, metric.NormalizedPackageVersion);
        sqlDataRecord.SetByte(4, (byte) metric.RawMetricType);
        sqlDataRecord.SetString(5, JsonConvert.SerializeObject((object) metric.RawMetricData));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Feed.typ_RawMetrics", rawMetrics.Select<PackageMetricsData, SqlDataRecord>(selector));
    }
  }
}
