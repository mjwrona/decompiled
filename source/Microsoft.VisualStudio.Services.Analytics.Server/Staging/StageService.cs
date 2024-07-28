// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Staging.StageService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Analytics.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.DataQuality;
using Microsoft.VisualStudio.Services.Analytics.Telemetry;
using Microsoft.VisualStudio.Services.Analytics.Transform;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Staging
{
  public class StageService : IStageService, IVssFrameworkService
  {
    public static readonly Guid StreamCleanupJobId = new Guid("9B61A767-F78E-4106-8886-45C9F560EC3F");
    private Dictionary<string, string> m_cleanupPlaceholder;
    internal const string lastBatchCleanupRegistryPath = "/Service/Analytics/State/LastBatchCleanup";
    internal const string retainBatchHistoryDaysRegistryPath = "/Service/Analytics/Settings/RetainBatchHistoryDays";
    internal const string streamCleanupCommandTimeoutSecondsRegistryPath = "/Service/Analytics/Settings/CleanupStreamCommandTimeout";
    internal const int retainBatchHistoryDaysDefault = 90;
    internal const int streamCleanupCommandTimeoutSecondsDefault = 900;
    private const string s_traceLayer = "StageService";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_cleanupPlaceholder = new Dictionary<string, string>();

    public StageProviderShardInfo GetShard(
      IVssRequestContext requestContext,
      string table,
      int providerShardId)
    {
      this.ThrowIfSubStatusPreventsStaging(requestContext);
      try
      {
        this.VerifyTableName(table);
        using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        {
          List<StreamInfo> shardStreamInfos = component.GetProviderShardStreamInfos(table, providerShardId);
          if (!shardStreamInfos.Any<StreamInfo>())
          {
            component.CreateStream(table, providerShardId, true, 5);
            shardStreamInfos = component.GetProviderShardStreamInfos(table, providerShardId);
          }
          List<StreamInfo> list1 = shardStreamInfos.Where<StreamInfo>((System.Func<StreamInfo, bool>) (strm => strm.Enabled)).ToList<StreamInfo>();
          if (!list1.Any<StreamInfo>())
            throw new InvalidOperationException(AnalyticsResources.NO_ENABLED_STREAMS((object) table));
          List<StageStreamInfo> list2 = list1.OrderByDescending<StreamInfo, int>((System.Func<StreamInfo, int>) (strm => strm.Priority)).Select<StreamInfo, StageStreamInfo>((System.Func<StreamInfo, StageStreamInfo>) (strm => new StageStreamInfo()
          {
            StreamId = strm.StreamId,
            Priority = strm.Priority,
            Watermark = strm.Watermark,
            Current = strm.Current,
            CreatedTime = strm.CreateTime,
            InitialContentVersion = strm.InitialContentVersion,
            LatestContentVersion = strm.LatestContentVersion,
            KeysOnly = strm.KeysOnly
          })).ToList<StageStreamInfo>();
          return new StageProviderShardInfo()
          {
            ProviderShardId = providerShardId,
            Streams = (IList<StageStreamInfo>) list2
          };
        }
      }
      catch (Exception ex)
      {
        throw new StageFailedException(nameof (GetShard), ex);
      }
    }

    public IngestResult StageRecords(
      IVssRequestContext requestContext,
      string table,
      int providerShard,
      int stream,
      Stream content)
    {
      this.ThrowIfSubStatusPreventsStaging(requestContext);
      try
      {
        Lazy<string> jsonContent = new Lazy<string>((Func<string>) (() =>
        {
          if (!content.CanSeek)
            return (string) null;
          using (MemoryStream destination = new MemoryStream())
          {
            using (StreamReader streamReader = new StreamReader((Stream) destination))
            {
              content.Seek(0L, SeekOrigin.Begin);
              content.CopyTo((Stream) destination);
              content.Seek(0L, SeekOrigin.Begin);
              destination.Seek(0L, SeekOrigin.Begin);
              return streamReader.ReadToEnd();
            }
          }
        }));
        requestContext.TraceLongTextConditionally(12010001, TraceLevel.Verbose, "AnalyticsStaging", nameof (StageService), (Func<string>) (() => !string.IsNullOrWhiteSpace(jsonContent.Value) ? jsonContent.Value : "Can't seek for " + table + "."));
        IDictionary<int, SqlMetaData[]> schemas = (IDictionary<int, SqlMetaData[]>) null;
        SqlMetaData[] extendedSchema = (SqlMetaData[]) null;
        int num = 0;
        using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        {
          schemas = component.GetStagingSchemas(table);
          extendedSchema = component.GetStagingExtendedSchema();
          num = component.GetTableStageVersions(table).Max();
        }
        StageEnvelope envelope = new StageEnvelopeJsonDeserializer().Deserialize(requestContext, content, schemas, extendedSchema);
        this.WriteOnPremTelemetry(requestContext, envelope);
        bool flag1 = false;
        bool flag2 = false;
        if (!envelope.IsCurrent.HasValue)
          throw new InvalidOperationException(AnalyticsResources.MISSING_REQUIRED_PROPERTY((object) "isCurrent"));
        if (!envelope.Replace.HasValue)
          throw new InvalidOperationException(AnalyticsResources.MISSING_REQUIRED_PROPERTY((object) "replace"));
        if (envelope.Records == null)
          throw new InvalidOperationException(AnalyticsResources.NO_DATA_TO_STAGE());
        if (envelope.Replace.Value && !envelope.IsCurrent.Value)
          throw new InvalidOperationException(AnalyticsResources.PROPERTY_SHOULD_BE_TRUE_WHEN_OTHER_PROPERTY_IS_USED((object) "isCurrent", (object) "replace"));
        StreamInfo streamInfo = (StreamInfo) null;
        using (StageComponent component = requestContext.CreateComponent<StageComponent>())
          streamInfo = component.GetShardStreamInfos(table, new int?(providerShard), new int?(stream)).SingleOrDefault<StreamInfo>();
        if (streamInfo == null)
          throw new StageStreamNotFoundException(table, providerShard, stream);
        if (!streamInfo.Enabled)
          throw new StageStreamDisabledException(table, providerShard, stream);
        if (streamInfo.Maintenance)
          throw new StageTableInMaintenanceException(table, streamInfo.MaintenanceReason);
        string reason;
        if (streamInfo.Priority <= 2 && this.ShouldThrottle(requestContext, out reason))
          throw new StageStreamThrottledException(table, providerShard, stream, reason);
        string watermark = streamInfo.Watermark;
        if (!string.IsNullOrEmpty(watermark))
        {
          if (string.IsNullOrEmpty(envelope.FromWatermark))
            throw new InvalidOperationException(AnalyticsResources.MISSING_REQUIRED_PROPERTY((object) "fromWatermark"));
          if (string.Compare(watermark, envelope.FromWatermark, StringComparison.InvariantCultureIgnoreCase) != 0)
            throw new InvalidOperationException(AnalyticsResources.WATERMARK_MISMATCH((object) "fromWatermark", (object) envelope.FromWatermark, (object) watermark));
        }
        int? contentVersion = new int?(envelope.ContentVersion);
        if (envelope.ProviderStageVersion != num)
          contentVersion = new int?();
        bool terminate = false;
        using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        {
          try
          {
            try
            {
              component.StageRecords(table, providerShard, stream, (IEnumerable<SqlDataRecord>) envelope.Records, (IEnumerable<SqlDataRecord>) envelope.ExtendedFields, envelope.Replace.Value, envelope.KeysOnly.Value, !streamInfo.Current, envelope.ToWatermark, envelope.StageVersion, contentVersion, envelope.SyncDate, out terminate);
            }
            catch (CircuitBreakerException ex) when (streamInfo.Priority <= 2)
            {
              throw new StageStreamThrottledException(table, providerShard, stream, ex.Message);
            }
            catch (Exception ex)
            {
              if (ex is StageKeysOnlyNotSupportedException)
                component.UpdateStream(table, providerShard, streamInfo.StreamId, true, streamInfo.Current, new int?(), false, new bool?(false));
              throw;
            }
            flag2 = true;
            if (!streamInfo.Current && envelope.IsCurrent.Value)
            {
              component.BeginTransaction(IsolationLevel.ReadCommitted);
              List<StreamInfo> shardStreamInfos = component.GetProviderShardStreamInfos(table, providerShard);
              foreach (StreamInfo streamInfo1 in (IEnumerable<StreamInfo>) shardStreamInfos.Where<StreamInfo>((System.Func<StreamInfo, bool>) (strm => !strm.Disposed && strm.StreamId < streamInfo.StreamId)).OrderBy<StreamInfo, int>((System.Func<StreamInfo, int>) (strm => strm.StreamId)))
                component.UpdateStream(table, providerShard, streamInfo1.StreamId, false, false, new int?(), true, new bool?());
              component.UpdateStream(table, providerShard, streamInfo.StreamId, true, true, new int?(8), false, new bool?(false));
              component.CommitTransaction();
              if (shardStreamInfos.Where<StreamInfo>((System.Func<StreamInfo, bool>) (strm => strm.Current)).Any<StreamInfo>())
                flag1 = true;
              if (shardStreamInfos.Where<StreamInfo>((System.Func<StreamInfo, bool>) (strm => !strm.Enabled && strm.StreamId > streamInfo.StreamId)).Any<StreamInfo>())
                throw new InvalidOperationException(AnalyticsResources.DISABLE_STREAMS_LEFT());
            }
            int? latestContentVersion = streamInfo.LatestContentVersion;
            if (latestContentVersion.HasValue)
            {
              if (contentVersion.HasValue)
              {
                latestContentVersion = streamInfo.LatestContentVersion;
                if (latestContentVersion.Value != contentVersion.Value)
                  component.InvalidateProviderShard(table, providerShard);
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceLongTextConditionally(12010001, TraceLevel.Error, "AnalyticsStaging", nameof (StageService), (Func<string>) (() => !string.IsNullOrWhiteSpace(jsonContent.Value) ? jsonContent.Value : "Can't seek for " + table + "."));
            throw;
          }
        }
        TelemetryLogger.Instance.Publish(requestContext, nameof (StageService), new Dictionary<string, object>()
        {
          ["records"] = (object) envelope.Records.Count,
          [nameof (table)] = (object) table
        });
        IDataQualityService service = requestContext.GetService<IDataQualityService>();
        if (flag2)
          service.NotifyDataChange(requestContext);
        if (flag1)
          requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            StageService.StreamCleanupJobId
          });
        requestContext.GetService<ITransformService>()?.QueueTransform(requestContext);
        return new IngestResult()
        {
          Count = envelope.Records.Count,
          Terminate = terminate
        };
      }
      catch (Exception ex) when (!(ex is StageExceptionBase))
      {
        throw new StageFailedException(nameof (StageRecords), ex);
      }
    }

    private bool ShouldThrottle(IVssRequestContext requestContext, out string reason)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationDatabaseManagementService service = vssRequestContext.GetService<ITeamFoundationDatabaseManagementService>();
      HostProperties hostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(requestContext, requestContext.ServiceHost.InstanceId);
      ITeamFoundationDatabaseProperties database = service.GetDatabase(vssRequestContext, hostProperties.DatabaseId);
      int num = service.ThrottleDatabaseAccess(vssRequestContext, database, out reason) ? 1 : 0;
      requestContext.Trace(12010002, TraceLevel.Info, "AnalyticsStaging", nameof (StageService), "ThrottleDatabaseAccess call for database(Name : {0}, Id : {1}) took {2} milliseconds", (object) database.DatabaseName, (object) database.DatabaseId, (object) stopwatch.ElapsedMilliseconds);
      return num != 0;
    }

    public void InvalidateProviderShard(
      IVssRequestContext requestContext,
      string table,
      int providerShardId,
      IList<string> fieldNames = null,
      bool disableCurrentStream = false,
      bool keysOnly = false)
    {
      try
      {
        using (StageComponent component = requestContext.CreateComponent<StageComponent>())
          component.InvalidateProviderShard(table, providerShardId, (IEnumerable<string>) fieldNames, disableCurrentStream, keysOnly);
      }
      catch (Exception ex)
      {
        throw new StageFailedException(nameof (InvalidateProviderShard), ex);
      }
    }

    public void InvalidateTable(IVssRequestContext requestContext, string table)
    {
      using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        component.InvalidateTable(table);
    }

    public void InvalidateAllTables(IVssRequestContext requestContext)
    {
      using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        component.InvalidateAllTables();
    }

    public CleanupStreamResult CleanupStreams(
      IVssRequestContext requestContext,
      bool forceBatchCleanup = false)
    {
      this.ThrowIfSubStatusPreventsStaging(requestContext);
      CleanupStreamResult cleanupStreamResult = new CleanupStreamResult()
      {
        Unsupported = true
      };
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int retainBatchHistoryDays = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Analytics/Settings/RetainBatchHistoryDays", 90);
      int commandTimeoutSeconds = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Analytics/Settings/CleanupStreamCommandTimeout", 900);
      DateTime dateTime = service.GetValue<DateTime>(requestContext, (RegistryQuery) "/Service/Analytics/State/LastBatchCleanup", DateTime.MinValue);
      DateTime utcNow = DateTime.UtcNow;
      bool flag = false;
      using (StageComponent component = requestContext.CreateComponent<StageComponent>())
      {
        foreach (StreamInfo streamInfo in component.GetProviderShardStreamInfos((string) null).Where<StreamInfo>((System.Func<StreamInfo, bool>) (s => s.Disposed)))
        {
          CleanupStreamResult operand = component.CleanupStream(streamInfo.TableName, streamInfo.ProviderShardId, streamInfo.StreamId, commandTimeoutSeconds);
          cleanupStreamResult.Add(operand);
          if (operand.DeletedRows > 0)
            component.RecordTableOperation(streamInfo.TableName, new int?(), new int?(), "delete");
          if (!cleanupStreamResult.Complete)
            break;
        }
        if (cleanupStreamResult.Complete)
        {
          if (dateTime.AddHours(23.0) < utcNow | forceBatchCleanup)
          {
            CleanupStreamResult operand = component.CleanupBatches(retainBatchHistoryDays);
            cleanupStreamResult.Add(operand);
            if (cleanupStreamResult.Complete)
              flag = true;
          }
        }
      }
      if (flag)
        service.SetValue<DateTime>(requestContext, "/Service/Analytics/State/LastBatchCleanup", utcNow);
      return cleanupStreamResult;
    }

    public CleanupStreamResult CleanupOrphanedStreamData(
      IVssRequestContext requestContext,
      int commandTimeout,
      ITeamFoundationDatabaseProperties database = null)
    {
      this.ThrowIfSubStatusPreventsStaging(requestContext);
      CleanupStreamResult cleanupStreamResult = new CleanupStreamResult()
      {
        Unsupported = true
      };
      using (StageComponent stageComponent = database != null ? TeamFoundationResourceManagementService.CreateComponentRaw<StageComponent>(database) : requestContext.CreateComponent<StageComponent>())
      {
        foreach (string table in StageTypeDefinitions.Definitions.Select<KeyValuePair<string, StageTypeDefinitions.StageTypeDefinition>, string>((System.Func<KeyValuePair<string, StageTypeDefinitions.StageTypeDefinition>, string>) (def => def.Key)))
        {
          CleanupStreamResult operand = stageComponent.CleanupOrphanedStreamData(table, commandTimeout);
          if (operand != null)
            cleanupStreamResult.Add(operand);
        }
      }
      return cleanupStreamResult;
    }

    public void SetStagingTableMaintenance(
      IVssRequestContext requestContext,
      string tableName,
      string maintenanceReason,
      ITeamFoundationDatabaseProperties database = null)
    {
      using (StageComponent stageComponent = database != null ? TeamFoundationResourceManagementService.CreateComponentRaw<StageComponent>(database) : requestContext.CreateComponent<StageComponent>())
        stageComponent.SetStagingTableMaintenance(tableName, true, maintenanceReason);
    }

    public void ClearStagingTableMaintenance(
      IVssRequestContext requestContext,
      string tableName,
      ITeamFoundationDatabaseProperties database = null)
    {
      using (StageComponent stageComponent = database != null ? TeamFoundationResourceManagementService.CreateComponentRaw<StageComponent>(database) : requestContext.CreateComponent<StageComponent>())
        stageComponent.SetStagingTableMaintenance(tableName, false, (string) null);
    }

    public string ExportRecords(IVssRequestContext requestContext, string table, long? batchId = null)
    {
      using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        return component.ExportRecords(table, batchId);
    }

    public void DeleteShard(IVssRequestContext requestContext, string table, int providerShard)
    {
      this.ThrowIfSubStatusPreventsStaging(requestContext);
      try
      {
        using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        {
          foreach (StreamInfo streamInfo in component.GetProviderShardStreamInfos(table, providerShard).OrderBy<StreamInfo, int>((System.Func<StreamInfo, int>) (x => x.StreamId)).ToList<StreamInfo>())
            component.UpdateStream(table, providerShard, streamInfo.StreamId, false, false, new int?(), true, new bool?());
        }
        requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          StageService.StreamCleanupJobId
        });
      }
      catch (Exception ex)
      {
        throw new StageFailedException(nameof (DeleteShard), ex);
      }
    }

    private void VerifyTableName(string table)
    {
      if (!((IEnumerable<string>) StageTableNames.All).Contains<string>(table, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
        throw new ArgumentException(AnalyticsResources.UNKNOWN_TABLE_NAME((object) (table ?? "(null)")));
    }

    public StageTableInfo GetTable(IVssRequestContext requestContext, string table)
    {
      this.ThrowIfSubStatusPreventsStaging(requestContext);
      try
      {
        this.VerifyTableName(table);
        int num = 0;
        List<StreamInfo> shardStreamInfos;
        using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        {
          shardStreamInfos = component.GetProviderShardStreamInfos(table);
          num = component.GetTableStageVersions(table).Max();
        }
        List<StageProviderShardInfo> list = shardStreamInfos.Where<StreamInfo>((System.Func<StreamInfo, bool>) (strm => strm.Enabled)).GroupBy<StreamInfo, int, StageProviderShardInfo>((System.Func<StreamInfo, int>) (es => es.ProviderShardId), (Func<int, IEnumerable<StreamInfo>, StageProviderShardInfo>) ((shardId, es) => new StageProviderShardInfo()
        {
          ProviderShardId = shardId,
          Streams = (IList<StageStreamInfo>) es.OrderByDescending<StreamInfo, int>((System.Func<StreamInfo, int>) (strm => strm.Priority)).Select<StreamInfo, StageStreamInfo>((System.Func<StreamInfo, StageStreamInfo>) (strm => new StageStreamInfo()
          {
            StreamId = strm.StreamId,
            Priority = strm.Priority,
            Watermark = strm.Watermark,
            Current = strm.Current,
            CreatedTime = strm.CreateTime,
            InitialContentVersion = strm.InitialContentVersion,
            LatestContentVersion = strm.LatestContentVersion,
            KeysOnly = strm.KeysOnly
          })).ToList<StageStreamInfo>()
        })).ToList<StageProviderShardInfo>();
        return new StageTableInfo()
        {
          TableName = table,
          Shards = (IList<StageProviderShardInfo>) list,
          StageVersion = num
        };
      }
      catch (Exception ex)
      {
        throw new StageFailedException(nameof (GetTable), ex);
      }
    }

    public StageProviderShardInfo CreateShard(
      IVssRequestContext requestContext,
      string table,
      int providerShard)
    {
      this.ThrowIfSubStatusPreventsStaging(requestContext);
      try
      {
        this.VerifyTableName(table);
        using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        {
          if (component.GetProviderShardStreamInfos(table, providerShard).Any<StreamInfo>())
            throw new InvalidOperationException(AnalyticsResources.HAS_ENABLED_STREAMS_ALREADY((object) table, (object) providerShard));
          component.CreateStream(table, providerShard, true, 5);
          List<StreamInfo> shardStreamInfos = component.GetProviderShardStreamInfos(table, providerShard);
          return new StageProviderShardInfo()
          {
            Streams = (IList<StageStreamInfo>) shardStreamInfos.Where<StreamInfo>((System.Func<StreamInfo, bool>) (s => s.Enabled)).Select<StreamInfo, StageStreamInfo>((System.Func<StreamInfo, StageStreamInfo>) (strm => new StageStreamInfo()
            {
              StreamId = strm.StreamId,
              Priority = strm.Priority,
              Watermark = strm.Watermark,
              Current = strm.Current,
              CreatedTime = strm.CreateTime,
              InitialContentVersion = strm.InitialContentVersion,
              LatestContentVersion = strm.LatestContentVersion,
              KeysOnly = strm.KeysOnly
            })).ToList<StageStreamInfo>(),
            ProviderShardId = providerShard
          };
        }
      }
      catch (Exception ex)
      {
        throw new StageFailedException(nameof (CreateShard), ex);
      }
    }

    private void WriteOnPremTelemetry(IVssRequestContext requestContext, StageEnvelope envelope)
    {
      if (envelope.PropertyParseErrorCounts == null)
        return;
      IAnalyticsOnPremTelemetryService service = requestContext.GetService<IAnalyticsOnPremTelemetryService>();
      foreach (KeyValuePair<string, int> propertyParseErrorCount in envelope.PropertyParseErrorCounts)
        service.IncrementCounter(requestContext, "StagingParseFailures", propertyParseErrorCount.Key, propertyParseErrorCount.Value);
    }

    private void ThrowIfSubStatusPreventsStaging(IVssRequestContext requestContext)
    {
      HostProperties hostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(requestContext, requestContext.ServiceHost.InstanceId);
      if (AnalyticsFeatureService.SubStatusPreventsStaging(hostProperties.SubStatus))
        throw new AnalyticsStagingUnavailableException(string.Format("Staging is currently unavailable for host {0} because hostSubStatus is {1}", (object) requestContext.ServiceHost.InstanceId, (object) hostProperties.SubStatus));
    }
  }
}
