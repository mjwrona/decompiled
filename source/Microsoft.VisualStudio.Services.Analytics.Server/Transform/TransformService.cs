// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.DataQuality;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  public class TransformService : ITransformService, IVssFrameworkService
  {
    private const int transformTimeoutSecondsDefault = 300;
    private static Guid[] transformJobIds = new Guid[5]
    {
      new Guid("63B34E3D-DD96-42B1-A262-8C2A6E3F127D"),
      new Guid("237EFE54-206B-46F8-8667-B039538883FC"),
      new Guid("E4ACE4FC-D84E-4AD4-BE82-CB21A63CEDF8"),
      new Guid("1785E032-4C1C-4E82-99A7-F64FAF3D36DE"),
      new Guid("278A4015-40D6-40A3-BF83-60C8C9666AA4")
    };
    private const string RetainHistoryDaysRegistryPath = "/Service/Analytics/Settings/Transform/RetainHistoryDays";
    private const string MaxConcurrentTransformJobsQueryPath = "/Service/Analytics/Settings/Transform/MaxConcurrentJobs";
    private static readonly RegistryQuery s_TransformSettingsQuery = new RegistryQuery("/Service/Analytics/Settings/Transform/**");
    private static readonly RegistryQuery s_HoldQuery = new RegistryQuery("/Service/Analytics/Settings/Transform/Hold");
    private static readonly RegistryQuery s_RetainHistoryDaysQuery = new RegistryQuery("/Service/Analytics/Settings/Transform/RetainHistoryDays");
    private static readonly RegistryQuery s_ThrottlePriorityThreshold = new RegistryQuery("/Service/Analytics/Settings/Transform/ThrottlePriorityThreshold");
    private static readonly RegistryQuery s_MaxConcurrentTransformJobsQuery = new RegistryQuery("/Service/Analytics/Settings/Transform/MaxConcurrentJobs");
    private static readonly string s_traceLayer = nameof (TransformService);
    internal const int retainHistoryDaysDefault = 180;

    public int TransformTimeoutSeconds { get; set; } = 300;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.TransformTimeoutSeconds = systemRequestContext.GetService<IVssRegistryService>().GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Settings/Transform/Timeout", 300);

    public bool TransformNext(
      IVssRequestContext requestContext,
      ICollection<TransformResult> results = null,
      int? partitionDBConcurrencyCounter = null,
      PreTransformAction preTransformAction = null,
      int selectionOffset = 0)
    {
      bool flag1 = true;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      Dictionary<string, string> dictionary = service.ReadEntriesFallThru(requestContext, in TransformService.s_TransformSettingsQuery).ToDictionary<RegistryEntry, string, string>((Func<RegistryEntry, string>) (ri => ri.Path), (Func<RegistryEntry, string>) (ri => ri.Value));
      foreach (string enableWithFeatureName in TransformDefinitions.EnableWithFeatureNames())
      {
        string featureName = enableWithFeatureName;
        if (!requestContext.IsFeatureEnabled(featureName))
        {
          foreach (string str in ((IEnumerable<TransformDefinition>) TransformDefinitions.All).Where<TransformDefinition>((Func<TransformDefinition, bool>) (d =>
          {
            List<string> withFeatureNames = d.EnableWithFeatureNames;
            // ISSUE: explicit non-virtual call
            return withFeatureNames != null && __nonvirtual (withFeatureNames.Contains(featureName));
          })).Select<TransformDefinition, string>((Func<TransformDefinition, string>) (d => d.SprocName)).Distinct<string>())
          {
            string key = "/Service/Analytics/Settings/Transform/" + str + "/FeatureDisabled";
            if (!dictionary.ContainsKey(key))
              dictionary.Add(key, "1");
          }
        }
      }
      foreach (string disableWithFeatureName in TransformDefinitions.DisableWithFeatureNames())
      {
        string featureName = disableWithFeatureName;
        if (requestContext.IsFeatureEnabled(featureName))
        {
          foreach (string str in ((IEnumerable<TransformDefinition>) TransformDefinitions.All).Where<TransformDefinition>((Func<TransformDefinition, bool>) (d =>
          {
            List<string> withFeatureNames = d.DisableWithFeatureNames;
            // ISSUE: explicit non-virtual call
            return withFeatureNames != null && __nonvirtual (withFeatureNames.Contains(featureName));
          })).Select<TransformDefinition, string>((Func<TransformDefinition, string>) (d => d.SprocName)).Distinct<string>())
          {
            string key = "/Service/Analytics/Settings/Transform/" + str + "/FeatureDisabled";
            if (!dictionary.ContainsKey(key))
              dictionary.Add(key, "1");
          }
        }
      }
      bool flag2 = service.GetValue<bool>(requestContext, in TransformService.s_HoldQuery, false);
      int num1 = service.GetValue<int>(requestContext, in TransformService.s_ThrottlePriorityThreshold, true);
      Stopwatch stopwatch = new Stopwatch();
      long? nullable1 = new long?();
      TransformResult result = (TransformResult) null;
      int sqlExecutionTime1 = PerformanceTimer.GetWellKnownParsedPerformanceTimings(requestContext).SqlExecutionTime;
      try
      {
        if (flag2)
        {
          result = new TransformResult()
          {
            AllProcessingComplete = true,
            Held = new bool?(true)
          };
        }
        else
        {
          TransformBatch batch = (TransformBatch) null;
          using (TransformComponent component = requestContext.CreateComponent<TransformComponent>())
            batch = component.GetNextTransformBatch((string) null, selectionOffset, (IDictionary<string, string>) dictionary);
          if (batch != null)
          {
            nullable1 = new long?(batch.BatchId);
            if (batch.Held)
              result = new TransformResult()
              {
                AllProcessingComplete = true,
                Held = new bool?(true)
              };
            else if (batch.LowPriorityDeferred)
              result = new TransformResult()
              {
                AllProcessingComplete = false,
                Deferred = true,
                DeferredReason = "Concurrent rework limit for table: " + batch.TableName
              };
            else if (batch.TargetActiveBlocked)
            {
              result = new TransformResult()
              {
                AllProcessingComplete = false,
                Deferred = true,
                DeferredReason = "Concurrent work blocking on table: " + batch.TableName
              };
            }
            else
            {
              string reason;
              if (batch.TransformPriority <= num1 && this.ShouldThrottleDatabaseAccess(requestContext, out reason))
              {
                result = new TransformResult()
                {
                  AllProcessingComplete = false,
                  Deferred = true,
                  DeferredReason = "SQL Resource Throttling: " + reason
                };
              }
              else
              {
                this.QueueTransform(requestContext, true);
                if (preTransformAction != null)
                  preTransformAction(batch);
                using (TransformComponent component = requestContext.CreateComponent<TransformComponent>())
                {
                  try
                  {
                    stopwatch.Start();
                    result = component.TransformBatch(batch.BatchId, (IDictionary<string, string>) dictionary, this.TransformTimeoutSeconds);
                    stopwatch.Stop();
                  }
                  catch (Exception ex)
                  {
                    stopwatch.Stop();
                    throw new AnalyticsTransformFailedException(batch.Sproc, ex.Message, ex);
                  }
                }
              }
            }
          }
        }
        if (result != null)
        {
          bool? failedAttempt = result.FailedAttempt;
          bool flag3 = true;
          if (failedAttempt.GetValueOrDefault() == flag3 & failedAttempt.HasValue)
            throw new AnalyticsTransformFailedException(result.Sproc, result.FailedMessage);
        }
        if (result != null)
        {
          int valueOrDefault1 = result.DeletedCount.GetValueOrDefault();
          int? nullable2 = result.UpdatedCount;
          int valueOrDefault2 = nullable2.GetValueOrDefault();
          int num2 = valueOrDefault1 + valueOrDefault2;
          nullable2 = result.InsertedCount;
          int valueOrDefault3 = nullable2.GetValueOrDefault();
          if (num2 + valueOrDefault3 > 0)
          {
            requestContext.GetService<IDataQualityService>().NotifyDataChange(requestContext);
            requestContext.GetService<IArchivalService>().NotifyDataChange(requestContext, result.TableName);
          }
        }
      }
      catch (AnalyticsTransformFailedException ex1)
      {
        requestContext.TraceException(12010005, "AnalyticsTransform", TransformService.s_traceLayer, (Exception) ex1);
        if (result == null)
        {
          string failedMessage = ex1.InnerException?.Message ?? ex1.Message;
          try
          {
            using (TransformComponent component = requestContext.CreateComponent<TransformComponent>())
              result = component.FailTransformBatch(nullable1.Value, (IDictionary<string, string>) dictionary, failedMessage, (int) stopwatch.ElapsedMilliseconds);
          }
          catch (Exception ex2)
          {
            requestContext.TraceException(12010006, "AnalyticsTransform", TransformService.s_traceLayer, ex2);
          }
        }
        throw;
      }
      finally
      {
        if (result != null)
        {
          int sqlExecutionTime2 = PerformanceTimer.GetWellKnownParsedPerformanceTimings(requestContext).SqlExecutionTime;
          result.SqlExecutionTime = new int?(sqlExecutionTime2 - sqlExecutionTime1);
          result.ConcurrencyCounter = partitionDBConcurrencyCounter;
          results?.Add(result);
          TransformService.LogTransformResult(requestContext, result);
          bool? failed = result.Failed;
          bool flag4 = true;
          if (failed.GetValueOrDefault() == flag4 & failed.HasValue && result.BatchId.HasValue)
          {
            using (TransformComponent component = requestContext.CreateComponent<TransformComponent>())
              component.CreateTransformRework(result.BatchId.Value, (string) null, true, true, true, true);
          }
          flag1 = result.AllProcessingComplete;
        }
      }
      return flag1;
    }

    private bool ShouldThrottleDatabaseAccess(IVssRequestContext requestContext, out string reason)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationDatabaseManagementService service = vssRequestContext.GetService<ITeamFoundationDatabaseManagementService>();
      HostProperties hostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(requestContext, requestContext.ServiceHost.InstanceId);
      ITeamFoundationDatabaseProperties database = service.GetDatabase(vssRequestContext, hostProperties.DatabaseId);
      return service.ThrottleDatabaseAccess(vssRequestContext, database, out reason);
    }

    private static void LogTransformResult(
      IVssRequestContext requestContext,
      TransformResult result)
    {
      if (result == null)
        return;
      if (requestContext.IsTracing(12010004, TraceLevel.Info, "AnalyticsTransform", TransformService.s_traceLayer))
      {
        string message = JsonConvert.SerializeObject((object) result, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        requestContext.Trace(12010004, TraceLevel.Info, "AnalyticsTransform", TransformService.s_traceLayer, message);
      }
      bool? failed = result.Failed;
      bool flag = true;
      if (failed.GetValueOrDefault() == flag & failed.HasValue)
        TeamFoundationEventLog.Default.Log(requestContext, string.Format("Transform batch {0} failed", (object) result.BatchId), TeamFoundationEventId.AnalyticsTransformFailed, EventLogEntryType.Error);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "AnalyticsTransform", "TransformResult", TransformService.CreateCIDataFromTransformResult(result));
    }

    private static CustomerIntelligenceData CreateCIDataFromTransformResult(TransformResult result)
    {
      CustomerIntelligenceData fromTransformResult = new CustomerIntelligenceData();
      fromTransformResult.Add("AllProcessingComplete", result.AllProcessingComplete);
      if (result.AlreadyActive)
        fromTransformResult.Add("AlreadyActive", result.AlreadyActive);
      if (result.Deferred)
        fromTransformResult.Add("Deferred", result.Deferred);
      if (!string.IsNullOrEmpty(result.DeferredReason))
        fromTransformResult.Add("DeferredReason", result.DeferredReason);
      bool? nullable1 = result.Held;
      bool flag = true;
      if (nullable1.GetValueOrDefault() == flag & nullable1.HasValue)
        fromTransformResult.Add("Held", (object) result.Held);
      long? nullable2 = result.BatchId;
      if (nullable2.HasValue)
        fromTransformResult.Add("BatchId", (object) result.BatchId);
      int? nullable3 = result.SubBatchCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("SubBatchCount", (object) result.SubBatchCount);
      nullable3 = result.AttemptCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("AttemptCount", (object) result.AttemptCount);
      if (result.StartTime.HasValue)
        fromTransformResult.Add("StartTime", (object) result.StartTime);
      if (result.TableName != null)
        fromTransformResult.Add("TableName", result.TableName);
      if (result.Sproc != null)
        fromTransformResult.Add("Sproc", result.Sproc);
      nullable3 = result.SprocVersion;
      if (nullable3.HasValue)
        fromTransformResult.Add("SprocVersion", (object) result.SprocVersion);
      if (result.TriggerTableName != null)
        fromTransformResult.Add("TriggerTableName", result.TriggerTableName);
      nullable2 = result.TriggerBatchIdStart;
      if (nullable2.HasValue)
        fromTransformResult.Add("TriggerBatchIdStart", (object) result.TriggerBatchIdStart);
      nullable2 = result.TriggerBatchIdEnd;
      if (nullable2.HasValue)
        fromTransformResult.Add("TriggerBatchIdEnd", (object) result.TriggerBatchIdEnd);
      if (!string.IsNullOrEmpty(result.State))
        fromTransformResult.Add("State", result.State);
      nullable2 = result.StateData;
      if (nullable2.HasValue)
        fromTransformResult.Add("StateData", (object) result.StateData);
      if (!string.IsNullOrEmpty(result.EndState))
        fromTransformResult.Add("EndState", result.EndState);
      nullable2 = result.EndStateData;
      if (nullable2.HasValue)
        fromTransformResult.Add("EndStateData", (object) result.EndStateData);
      nullable1 = result.Ready;
      if (nullable1.HasValue)
        fromTransformResult.Add("Ready", (object) result.Ready);
      nullable1 = result.Failed;
      if (nullable1.HasValue)
        fromTransformResult.Add("Failed", (object) result.Failed);
      nullable1 = result.FailedAttempt;
      if (nullable1.HasValue)
        fromTransformResult.Add("FailedAttempt", (object) result.FailedAttempt);
      if (!string.IsNullOrEmpty(result.FailedMessage))
        fromTransformResult.Add("FailedMessage", result.FailedMessage);
      nullable3 = result.InsertedCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("InsertedCount", (object) result.InsertedCount);
      nullable3 = result.UpdatedCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("UpdatedCount", (object) result.UpdatedCount);
      nullable3 = result.DeletedCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("DeletedCount", (object) result.DeletedCount);
      nullable3 = result.DurationMS;
      if (nullable3.HasValue)
        fromTransformResult.Add("DurationMS", (object) result.DurationMS);
      nullable3 = result.TotalInsertedCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("TotalInsertedCount", (object) result.TotalInsertedCount);
      nullable3 = result.TotalUpdatedCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("TotalUpdatedCount", (object) result.TotalUpdatedCount);
      nullable3 = result.TotalDeletedCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("TotalDeletedCount", (object) result.TotalDeletedCount);
      nullable3 = result.TotalDurationMS;
      if (nullable3.HasValue)
        fromTransformResult.Add("TotalDurationMS", (object) result.TotalDurationMS);
      nullable3 = result.TotalFailedCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("TotalFailedCount", (object) result.TotalFailedCount);
      nullable3 = result.ReworkAttemptCount;
      if (nullable3.HasValue)
        fromTransformResult.Add("ReworkAttemptCount", (object) result.ReworkAttemptCount);
      nullable3 = result.Priority;
      if (nullable3.HasValue)
        fromTransformResult.Add("Priority", (object) result.Priority);
      nullable3 = result.SqlExecutionTime;
      if (nullable3.HasValue)
        fromTransformResult.Add("SqlExecutionTime", (object) result.SqlExecutionTime);
      nullable3 = result.ConcurrencyCounter;
      if (nullable3.HasValue)
        fromTransformResult.Add("ConcurrencyCounter", (object) result.ConcurrencyCounter);
      return fromTransformResult;
    }

    public void QueueTransform(IVssRequestContext requestContext, int maxDelaySeconds = 0) => this.QueueTransform(requestContext, false, maxDelaySeconds);

    private void QueueTransform(
      IVssRequestContext requestContext,
      bool onlyWhenConcurrencyAllowed,
      int maxDelaySeconds = 0)
    {
      int count = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in TransformService.s_MaxConcurrentTransformJobsQuery, 1);
      if (onlyWhenConcurrencyAllowed && count <= 1)
        return;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      Guid? nullable = new Guid?(TransformService.transformJobIds[0]);
      if (count > 1)
      {
        List<TeamFoundationJobQueueEntry> source = service.QueryJobQueue(requestContext, ((IEnumerable<Guid>) TransformService.transformJobIds).Take<Guid>(count));
        nullable = source.Where<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (q => q != null && q.State == TeamFoundationJobState.QueuedScheduled)).FirstOrDefault<TeamFoundationJobQueueEntry>()?.JobId;
        if (!nullable.HasValue)
        {
          int index = 0;
          foreach (TeamFoundationJobQueueEntry foundationJobQueueEntry in source)
          {
            if (foundationJobQueueEntry == null)
            {
              nullable = new Guid?(TransformService.transformJobIds[index]);
              break;
            }
            ++index;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      if (maxDelaySeconds == 0)
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          nullable.Value
        });
      else
        service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          nullable.Value
        }, maxDelaySeconds);
    }

    public void ReprocessFromTable(IVssRequestContext requestContext, string tableName)
    {
      using (StageComponent component = requestContext.CreateComponent<StageComponent>())
        component.RecordTableOperation(tableName, new int?(), new int?(), "reprocess");
      this.QueueTransform(requestContext, 0);
    }

    public void ReprocessFromProcessBatch(
      IVssRequestContext requestContext,
      long batchId,
      string triggerTableName,
      bool fromExistingState,
      bool createDependentWork,
      bool delayReworkPerAttemptHistory,
      bool ignoreWhenConsecutiveSprocFailures)
    {
      using (TransformComponent component = requestContext.CreateComponent<TransformComponent>())
        component.CreateTransformRework(batchId, triggerTableName, fromExistingState, createDependentWork, delayReworkPerAttemptHistory, ignoreWhenConsecutiveSprocFailures);
      this.QueueTransform(requestContext, 0);
    }

    public void ReprocessFromUncorrectedBatches(
      IVssRequestContext requestContext,
      string operationSproc,
      bool fromExistingState,
      bool createDependentWork,
      bool delayReworkPerAttemptHistory)
    {
      using (TransformComponent component = requestContext.CreateComponent<TransformComponent>())
        component.CreateTransformReworkFromUncorrectedBatches(operationSproc, fromExistingState, createDependentWork, delayReworkPerAttemptHistory);
      this.QueueTransform(requestContext, 0);
    }

    public void GenerateCalendar(IVssRequestContext requestContext)
    {
      CultureInfo culture = requestContext.ServiceHost.GetCulture(requestContext);
      using (TransformComponent component = requestContext.CreateComponent<TransformComponent>())
        component.GenerateCalendar(culture.Name, culture.LCID);
    }

    public CleanupDeletedTableResult CleanupDeletedTable(
      IVssRequestContext requestContext,
      string tableName,
      bool continueToNextTable)
    {
      int retainHistoryDays = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in TransformService.s_RetainHistoryDaysQuery, 180);
      using (TransformComponent component = requestContext.CreateComponent<TransformComponent>())
        return component.CleanupDeletedTable(tableName, continueToNextTable, retainHistoryDays);
    }
  }
}
