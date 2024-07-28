// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess.SocialActivityAggregationComponent
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess
{
  internal class SocialActivityAggregationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<SocialActivityAggregationComponent>(1340),
      (IComponentCreator) new ComponentCreator<SocialActivityAggregationComponent>(1360),
      (IComponentCreator) new ComponentCreator<SocialActivityAggregationComponent>(1370),
      (IComponentCreator) new ComponentCreator<SocialActivityAggregationComponent>(1480),
      (IComponentCreator) new ComponentCreator<SocialActivityAggregationComponent>(1590),
      (IComponentCreator) new ComponentCreator<SocialActivityAggregationComponent>(1600),
      (IComponentCreator) new ComponentCreator<SocialActivityAggregationComponent>(1610)
    }, "SocialActivityAggregation");
    private static readonly SqlMetaData[] Social_typ_AggregatedArtifacts = new SqlMetaData[3]
    {
      new SqlMetaData("ProviderId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ArtifactType", SqlDbType.TinyInt),
      new SqlMetaData("Artifacts", SqlDbType.NVarChar, SqlMetaData.Max)
    };
    private static readonly SqlMetaData[] Social_typ_SocialActivity = new SqlMetaData[6]
    {
      new SqlMetaData("ActivityType", SqlDbType.VarChar, 24L),
      new SqlMetaData("ActivityId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ActivityTimeStamp", SqlDbType.DateTime2),
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Data", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("ExtendedData", SqlDbType.NVarChar, 4000L)
    };
    private static readonly SqlMetaData[] Social_typ_SocialActivityAggregatedMetrics = new SqlMetaData[6]
    {
      new SqlMetaData("DataSpaceId", SqlDbType.Int),
      new SqlMetaData("ProviderId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ArtifactType", SqlDbType.TinyInt),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 100L),
      new SqlMetaData("AggregateDateTime", SqlDbType.DateTime2),
      new SqlMetaData("MetaData", SqlDbType.NVarChar, 1024L)
    };
    private static readonly SqlMetaData[] Social_typ_SocialActivityAggregatedArtifact = new SqlMetaData[4]
    {
      new SqlMetaData("ProviderId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ArtifactType", SqlDbType.TinyInt),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 100L),
      new SqlMetaData("MetaData", SqlDbType.NVarChar, 1024L)
    };

    public SocialActivityAggregationComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    internal void AddOrUpdateSocialActivityAggregatedArtifactRecords(
      Guid scopeId,
      IList<SocialActivityAggregatedArtifact> aggregatedArtifacts)
    {
      if (this.Version < 1590)
        return;
      this.PrepareStoredProcedure("Social.prc_CreateOrUpdateSocialActivityAggregatedArtifact");
      this.BindInt("@dataSpaceId", this.GetDataspaceId(scopeId));
      this.BindSocialActivityActivityAggregatedArtifactTable("@socialActivityAggregatedArtifacts", (IEnumerable<SocialActivityAggregatedArtifact>) aggregatedArtifacts);
      this.ExecuteNonQuery();
    }

    internal void AddOrUpdateActivityAggregatedMetricRecords(
      IList<SocialActivityAggregatedMetric> aggregatedMetrics)
    {
      if (this.Version < 1360)
        return;
      this.PrepareStoredProcedure("Social.prc_CreateOrUpdateActivityAggregatedMetrics");
      this.BindSocialActivityAggregatedMetricTable("@aggregatedMetrics", (IEnumerable<SocialActivityAggregatedMetric>) aggregatedMetrics);
      this.ExecuteNonQuery();
    }

    internal void AddOrUpdateAggregatedArtifactsRecord(
      Guid scopeId,
      IList<AggregatedArtifactsRecord> aggregatedArtifactsRecords)
    {
      if (this.Version < 1360)
        return;
      this.PrepareStoredProcedure("Social.prc_CreateOrUpdateAggregatedArtifacts");
      this.BindInt("@dataSpaceId", this.GetDataspaceId(scopeId));
      this.BindAggregatedArtifactsTable("@aggregatedArtifacts", (IEnumerable<AggregatedArtifactsRecord>) aggregatedArtifactsRecords);
      this.ExecuteNonQuery();
    }

    internal void AddOrUpdateSocialPluginWatermark(
      Guid jobId,
      Guid providerId,
      DateTime processedTillTime)
    {
      if (this.Version < 1360)
        return;
      this.PrepareStoredProcedure("Social.prc_CreateOrUpdateSocialPluginWatermark");
      this.BindDateTime2("@processedTillTime", processedTillTime);
      this.BindGuid("@jobId", jobId);
      this.BindGuid("@providerId", providerId);
      this.ExecuteNonQuery();
    }

    internal virtual int AddSocialActivityRecords(IList<SocialActivityRecord> socialActivities)
    {
      this.PrepareStoredProcedure("Social.prc_CreateSocialActivity");
      this.BindSocialActivityTable("@socialActivities", (IEnumerable<SocialActivityRecord>) socialActivities);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder createdRecordsCount = new SqlColumnBinder("CreatedRecordsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => createdRecordsCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    internal virtual int DeleteSocialActivityAggregatedArtifactRecordBatch(
      Guid scopeId,
      Guid providerId,
      byte artifactType,
      IList<string> artifactIds)
    {
      if (this.Version < 1600)
        return 0;
      this.PrepareStoredProcedure("Social.prc_DeleteSocialActivityAggregatedArtifactBatch");
      this.BindInt("@dataSpaceId", this.GetDataspaceId(scopeId));
      this.BindGuid("@providerId", providerId);
      this.BindByte("@artifactType", artifactType);
      this.BindStringTable("@artifactIds", (IEnumerable<string>) new HashSet<string>((IEnumerable<string>) artifactIds));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder deletedRecordCount = new SqlColumnBinder("DeletedRecordsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => deletedRecordCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    internal virtual int DeleteActivityAggregatedMetricRecords(Guid providerId, DateTime till)
    {
      if (this.Version < 1610)
        return 0;
      this.PrepareStoredProcedure("Social.prc_DeleteActivityAggregatedMetrics2");
      this.BindGuid("@providerId", providerId);
      this.BindDateTime2("@till", till);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder deletedRecordCount = new SqlColumnBinder("DeletedRecordsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => deletedRecordCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    internal int DeleteAggregatedArtifactsRecords(DateTime till)
    {
      if (this.Version < 1360)
        return 0;
      this.PrepareStoredProcedure("Social.prc_DeleteAggregatedArtifactsRecords");
      this.BindDateTime2("@till", till);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder deletedRecordCount = new SqlColumnBinder("DeletedRecordsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => deletedRecordCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    internal int DeleteSocialActivityRecords(DateTime tillDate)
    {
      this.PrepareStoredProcedure("Social.prc_DeleteSocialActivity");
      this.BindDateTime2("@tillDate", tillDate);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder deletedRecordCount = new SqlColumnBinder("DeletedRecordsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => deletedRecordCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    internal int DeleteSocialPluginWatermarks(DateTime till)
    {
      if (this.Version < 1360)
        return 0;
      this.PrepareStoredProcedure("Social.prc_DeleteSocialPluginWatermarks");
      this.BindDateTime2("@till", till);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder deletedRecordCount = new SqlColumnBinder("DeletedRecordsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => deletedRecordCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    internal virtual IEnumerable<SocialActivityAggregatedArtifact> GetSocialActivityAggregatedArtifactRecordBatch(
      Guid scopeId,
      Guid providerId,
      byte artifactType,
      IList<string> artifactIds)
    {
      if (this.Version < 1590)
        return Enumerable.Empty<SocialActivityAggregatedArtifact>();
      this.PrepareStoredProcedure("Social.prc_GetSocialActivityAggregatedArtifactBatch");
      this.BindInt("@dataSpaceId", this.GetDataspaceId(scopeId));
      this.BindGuid("@providerId", providerId);
      this.BindByte("@artifactType", artifactType);
      this.BindStringTable("@artifactIds", (IEnumerable<string>) new HashSet<string>((IEnumerable<string>) artifactIds));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialActivityAggregatedArtifact>((ObjectBinder<SocialActivityAggregatedArtifact>) new SocialActivityAggregatedArtifactBinder());
        return (IEnumerable<SocialActivityAggregatedArtifact>) resultCollection.GetCurrent<SocialActivityAggregatedArtifact>().Items ?? Enumerable.Empty<SocialActivityAggregatedArtifact>();
      }
    }

    internal virtual IEnumerable<SocialActivityAggregatedMetric> GetActivityAggregatedMetricRecords(
      Guid dataSpaceIdentifier,
      Guid providerId,
      DateTime from,
      DateTime till,
      int skip,
      int take)
    {
      if (this.Version < 1360)
        return Enumerable.Empty<SocialActivityAggregatedMetric>();
      this.PrepareStoredProcedure("Social.prc_GetActivityAggregatedMetrics");
      this.BindInt("@dataSpaceId", this.GetDataspaceId(dataSpaceIdentifier));
      this.BindGuid("@providerId", providerId);
      this.BindDateTime2("@from", from);
      this.BindDateTime2("@till", till);
      this.BindInt("@skip", skip);
      this.BindInt("@take", take);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialActivityAggregatedMetric>((ObjectBinder<SocialActivityAggregatedMetric>) new SocialActivityAggregatedMetricBinder());
        return (IEnumerable<SocialActivityAggregatedMetric>) resultCollection.GetCurrent<SocialActivityAggregatedMetric>().Items;
      }
    }

    internal virtual AggregatedArtifactsRecord GetAggregatedArtifactsRecord(
      Guid scopeId,
      Guid providerId,
      byte artifactType)
    {
      if (this.Version < 1360)
        return (AggregatedArtifactsRecord) null;
      this.PrepareStoredProcedure("Social.prc_GetAggregatedArtifacts");
      this.BindInt("@dataSpaceId", this.GetDataspaceId(scopeId));
      this.BindGuid("@providerId", providerId);
      this.BindByte("@artifactType", artifactType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AggregatedArtifactsRecord>((ObjectBinder<AggregatedArtifactsRecord>) new AggregatedArtifactsRecordBinder());
        return resultCollection.GetCurrent<AggregatedArtifactsRecord>().FirstOrDefault<AggregatedArtifactsRecord>();
      }
    }

    internal virtual IEnumerable<AggregatedArtifactsRecord> GetAllAggregatedArtifactsRecords(
      Guid scopeId,
      Guid providerId)
    {
      if (this.Version < 1370)
        return Enumerable.Empty<AggregatedArtifactsRecord>();
      this.PrepareStoredProcedure("Social.prc_GetAllAggregatedArtifactsRecords");
      this.BindInt("@dataSpaceId", this.GetDataspaceId(scopeId));
      this.BindGuid("@providerId", providerId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AggregatedArtifactsRecord>((ObjectBinder<AggregatedArtifactsRecord>) new AggregatedArtifactsRecordBinder());
        return (IEnumerable<AggregatedArtifactsRecord>) resultCollection.GetCurrent<AggregatedArtifactsRecord>().Items;
      }
    }

    internal virtual IEnumerable<SocialActivityRecord> GetSocialActivityRecords(
      string activityType,
      DateTime fromDate,
      DateTime tillDate,
      bool fetchExtendedData,
      int skip,
      int take)
    {
      this.PrepareStoredProcedure("Social.prc_GetActivityRecordsForTimeRange");
      this.BindString("@activityType", activityType, 24, false, SqlDbType.VarChar);
      this.BindDateTime2("@fromDate", fromDate);
      this.BindDateTime2("@tillDate", tillDate);
      if (this.Version >= 1360)
      {
        this.BindInt("@skip", skip);
        this.BindInt("@take", take);
      }
      if (this.Version >= 1480)
        this.BindBoolean("@fetchExtendedData", fetchExtendedData);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialActivityRecord>((ObjectBinder<SocialActivityRecord>) new SocialActivityRecordBinder());
        return (IEnumerable<SocialActivityRecord>) resultCollection.GetCurrent<SocialActivityRecord>().Items;
      }
    }

    internal virtual DateTime GetSocialPluginProcessedTillTime(Guid jobId, Guid providerId)
    {
      if (this.Version < 1360)
        return DateTime.MinValue;
      this.PrepareStoredProcedure("Social.prc_GetSocialPluginWatermark");
      this.BindGuid("@providerId", providerId);
      this.BindGuid("@jobId", jobId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder processedTillTime = new SqlColumnBinder("ProcessedTillTime");
        resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => processedTillTime.GetDateTime(reader, DateTime.MinValue))));
        return resultCollection.GetCurrent<DateTime>().FirstOrDefault<DateTime>();
      }
    }

    private SqlParameter BindAggregatedArtifactsTable(
      string parameterName,
      IEnumerable<AggregatedArtifactsRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<AggregatedArtifactsRecord>();
      System.Func<AggregatedArtifactsRecord, SqlDataRecord> selector = (System.Func<AggregatedArtifactsRecord, SqlDataRecord>) (aggregatedArtifactsRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SocialActivityAggregationComponent.Social_typ_AggregatedArtifacts);
        sqlDataRecord.SetGuid(0, aggregatedArtifactsRecord.ProviderId);
        sqlDataRecord.SetByte(1, aggregatedArtifactsRecord.ArtifactType);
        sqlDataRecord.SetString(2, aggregatedArtifactsRecord.Artifacts);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Social.typ_AggregatedArtifacts", rows.Select<AggregatedArtifactsRecord, SqlDataRecord>(selector));
    }

    private SqlParameter BindSocialActivityTable(
      string parameterName,
      IEnumerable<SocialActivityRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<SocialActivityRecord>();
      System.Func<SocialActivityRecord, SqlDataRecord> selector = (System.Func<SocialActivityRecord, SqlDataRecord>) (socialActivity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SocialActivityAggregationComponent.Social_typ_SocialActivity);
        sqlDataRecord.SetString(0, socialActivity.ActivityType);
        sqlDataRecord.SetGuid(1, socialActivity.ActivityId);
        sqlDataRecord.SetDateTime(2, socialActivity.ActivityTimeStamp);
        sqlDataRecord.SetGuid(3, socialActivity.UserId);
        sqlDataRecord.SetString(4, socialActivity.Data);
        sqlDataRecord.SetString(5, socialActivity.ExtendedData);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Social.typ_SocialActivity", rows.Select<SocialActivityRecord, SqlDataRecord>(selector));
    }

    private SqlParameter BindSocialActivityActivityAggregatedArtifactTable(
      string parameterName,
      IEnumerable<SocialActivityAggregatedArtifact> rows)
    {
      rows = rows ?? Enumerable.Empty<SocialActivityAggregatedArtifact>();
      System.Func<SocialActivityAggregatedArtifact, SqlDataRecord> selector = (System.Func<SocialActivityAggregatedArtifact, SqlDataRecord>) (aggregatedArtifact =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SocialActivityAggregationComponent.Social_typ_SocialActivityAggregatedArtifact);
        sqlDataRecord.SetGuid(0, aggregatedArtifact.ProviderId);
        sqlDataRecord.SetByte(1, aggregatedArtifact.ArtifactType);
        sqlDataRecord.SetString(2, aggregatedArtifact.ArtifactId);
        sqlDataRecord.SetString(3, aggregatedArtifact.MetaData);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Social.typ_SocialActivityAggregatedArtifact", rows.Select<SocialActivityAggregatedArtifact, SqlDataRecord>(selector));
    }

    private SqlParameter BindSocialActivityAggregatedMetricTable(
      string parameterName,
      IEnumerable<SocialActivityAggregatedMetric> rows)
    {
      rows = rows ?? Enumerable.Empty<SocialActivityAggregatedMetric>();
      System.Func<SocialActivityAggregatedMetric, SqlDataRecord> selector = (System.Func<SocialActivityAggregatedMetric, SqlDataRecord>) (aggregatedMetric =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SocialActivityAggregationComponent.Social_typ_SocialActivityAggregatedMetrics);
        sqlDataRecord.SetInt32(0, this.GetDataspaceId(aggregatedMetric.DataSpaceIdentifier));
        sqlDataRecord.SetGuid(1, aggregatedMetric.ProviderId);
        sqlDataRecord.SetByte(2, aggregatedMetric.ArtifactType);
        sqlDataRecord.SetString(3, aggregatedMetric.ArtifactId);
        sqlDataRecord.SetDateTime(4, aggregatedMetric.AggregateDateTime);
        sqlDataRecord.SetString(5, aggregatedMetric.MetaData);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Social.typ_SocialActivityAggregatedMetric", rows.Select<SocialActivityAggregatedMetric, SqlDataRecord>(selector));
    }
  }
}
