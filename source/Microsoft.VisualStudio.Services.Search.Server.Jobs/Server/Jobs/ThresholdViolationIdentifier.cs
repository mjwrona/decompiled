// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ThresholdViolationIdentifier
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class ThresholdViolationIdentifier
  {
    internal virtual bool IdentifyChangeEventWaitTimeThresholdViolation(
      ExecutionContext executionContext,
      IIndexingUnitChangeEventDataAccess indexingUnitChangeEventDataAccess)
    {
      Dictionary<string, TimeSpan> eventThresholdValues = this.GetChangeEventThresholdValues(executionContext);
      if (executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/ThresholdViolationsV2Enabled", true))
      {
        IDictionary<string, int> breachingThreshold = indexingUnitChangeEventDataAccess.GetCountOfEventsBreachingThreshold(executionContext.RequestContext, (int) eventThresholdValues["BeginBulkIndex"].TotalMinutes, (int) eventThresholdValues["UpdateIndex"].TotalMinutes);
        int num1;
        breachingThreshold.TryGetValue("BeginBulkIndex", out num1);
        int num2;
        breachingThreshold.TryGetValue("UpdateIndex", out num2);
        IDictionary<string, object> properties = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
        properties.Add("BulkIndexThresholdViolations", (object) num1);
        properties.Add("UpdateIndexThresholdViolations", (object) num2);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties);
        return num1 > 0 || num2 > 0;
      }
      IDictionary<string, TimeSpan> maxTimeForEachChangeType = indexingUnitChangeEventDataAccess.GetMaxTimeForEachChangeType(executionContext.RequestContext);
      int num = maxTimeForEachChangeType == null ? 0 : (eventThresholdValues.SelectMany((Func<KeyValuePair<string, TimeSpan>, IEnumerable<KeyValuePair<string, TimeSpan>>>) (kvp => (IEnumerable<KeyValuePair<string, TimeSpan>>) maxTimeForEachChangeType), (kvp, kvp2) => new
      {
        kvp = kvp,
        kvp2 = kvp2
      }).Where(_param1 =>
      {
        KeyValuePair<string, TimeSpan> keyValuePair1 = _param1.kvp;
        string key1 = keyValuePair1.Key;
        keyValuePair1 = _param1.kvp2;
        string key2 = keyValuePair1.Key;
        if (!(key1 == key2))
          return false;
        KeyValuePair<string, TimeSpan> keyValuePair2 = _param1.kvp;
        TimeSpan timeSpan1 = keyValuePair2.Value;
        keyValuePair2 = _param1.kvp2;
        TimeSpan timeSpan2 = keyValuePair2.Value;
        return timeSpan1 < timeSpan2;
      }).Select(_param1 => _param1.kvp).Any<KeyValuePair<string, TimeSpan>>() ? 1 : 0);
      if (num == 0)
        return num != 0;
      executionContext.ExecutionTracerContext.PublishKpi("ThresholdViolation", "ThresholdViolation", 1.0);
      return num != 0;
    }

    internal virtual bool IdentifyIndexingStuckForCollection(
      ExecutionContext executionContext,
      IIndexingUnitChangeEventDataAccess changeEventDataAccess)
    {
      int unitChangeEvents1 = changeEventDataAccess.GetCountOfIndexingUnitChangeEvents(executionContext.RequestContext);
      int unitChangeEvents2 = changeEventDataAccess.GetCountOfIndexingUnitChangeEvents(executionContext.RequestContext, IndexingUnitChangeEventState.Succeeded);
      int unitChangeEvents3 = changeEventDataAccess.GetCountOfIndexingUnitChangeEvents(executionContext.RequestContext, IndexingUnitChangeEventState.Failed);
      int unitChangeEvents4 = changeEventDataAccess.GetCountOfIndexingUnitChangeEvents(executionContext.RequestContext, IndexingUnitChangeEventState.Pending);
      int num = unitChangeEvents1 == 0 ? 0 : (unitChangeEvents1 == unitChangeEvents4 ? 1 : 0);
      if (num == 0)
        return num != 0;
      executionContext.ExecutionTracerContext.PublishKpi("IndexingStuckForCollection", "ThresholdViolation", 1.0);
      executionContext.ExecutionTracerContext.PublishKpi("TotalNumberOfEvents", "ThresholdViolation", (double) unitChangeEvents1);
      executionContext.ExecutionTracerContext.PublishKpi("TotalNumberOfSucceededEvents", "ThresholdViolation", (double) unitChangeEvents2);
      executionContext.ExecutionTracerContext.PublishKpi("TotalNumberOfFailedEvents", "ThresholdViolation", (double) unitChangeEvents3);
      return num != 0;
    }

    internal virtual bool IdentifyItemLevelFailureViolationsForIndexingUnit(
      ExecutionContext executionContext,
      IItemLevelFailureDataAccess ilfDataAccess,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      try
      {
        if (!this.IsItemLevelFailureThresholdViolationCheckEnabled(executionContext, "/Service/ALMSearch/Settings/IsFileLevelFailureAlertEnabled"))
          return false;
        int failureThresholdTime = this.GetItemLevelFailureThresholdTime(executionContext);
        int forIndexingUnitId = ilfDataAccess.GetCountOfRecordsCreatedBeforeGivenHoursForIndexingUnitId(executionContext.RequestContext, indexingUnit.IndexingUnitId, RejectionCodeExtension.GetNonRetriableCodes(), failureThresholdTime);
        if (forIndexingUnitId > 0)
        {
          executionContext.ExecutionTracerContext.PublishKpi("ItemLevelFailureThresholdViolation", "ThresholdViolation", (double) forIndexingUnitId);
          executionContext.ExecutionTracerContext.PublishClientTrace("ThresholdViolation", "ItemLevelFailureThresholdViolation", "ItemLevelFailurePerEntityPerIndexingUnit", (object) FormattableString.Invariant(FormattableStringFactory.Create("EntityType:IndexingUnitId:FailureCount,{0}:{1}:{2}", (object) indexingUnit.EntityType.Name, (object) indexingUnit.IndexingUnitId, (object) forIndexingUnitId)));
        }
        return forIndexingUnitId > 0;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083114, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("exeception while identifying ItemLevelThresholdViolatins. Exception: {0}", (object) ex)));
      }
      return false;
    }

    internal virtual bool IdentifyItemLevelFailureViolationsForAllEntities(
      ExecutionContext executionContext,
      IItemLevelFailureDataAccess ilfDataAccess)
    {
      try
      {
        if (!this.IsItemLevelFailureThresholdViolationCheckEnabled(executionContext, "/Service/ALMSearch/Settings/IsFileLevelFailureAlertEnabledAtCollectionLevel"))
          return false;
        int failureThresholdTime = this.GetItemLevelFailureThresholdTime(executionContext);
        IDictionary<string, IDictionary<int, int>> andIndexingUnitId = ilfDataAccess.GetCountOfRecordsCreatedBeforeGivenHoursGroupedByEntityAndIndexingUnitId(executionContext.RequestContext, RejectionCodeExtension.GetNonRetriableCodes(), failureThresholdTime);
        if (andIndexingUnitId.Count > 0)
        {
          int num = 0;
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("EntityType:IndexingUnitId:FailureCount");
          foreach (KeyValuePair<string, IDictionary<int, int>> keyValuePair1 in (IEnumerable<KeyValuePair<string, IDictionary<int, int>>>) andIndexingUnitId)
          {
            foreach (KeyValuePair<int, int> keyValuePair2 in (IEnumerable<KeyValuePair<int, int>>) keyValuePair1.Value)
            {
              num += keyValuePair2.Value;
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create(",{0}:{1}:{2}", (object) keyValuePair1.Key, (object) keyValuePair2.Key, (object) keyValuePair2.Value)));
            }
          }
          executionContext.ExecutionTracerContext.PublishKpi("ItemLevelFailureThresholdViolation", "ThresholdViolation", (double) num);
          executionContext.ExecutionTracerContext.PublishClientTrace("ThresholdViolation", "ItemLevelFailureThresholdViolation", "ItemLevelFailurePerEntityPerIndexingUnit", (object) stringBuilder.ToString());
        }
        return andIndexingUnitId.Count > 0;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083114, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("exeception while identifying ItemLevelThresholdViolatins. Exception: {0}", (object) ex)));
      }
      return false;
    }

    internal virtual bool IdentifyLongWaitingChangeEvents(
      ExecutionContext executionContext,
      IIndexingUnitChangeEventDataAccess indexingUnitChangeEventDataAccess)
    {
      int longWaitingEvents = this.GetCountOfLongWaitingEvents(executionContext, indexingUnitChangeEventDataAccess, IndexingUnitChangeEventState.Pending, executionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/ChangeEventPendingDays", TeamFoundationHostType.ProjectCollection, false, 7));
      if (longWaitingEvents > 0)
        executionContext.ExecutionTracerContext.PublishKpi("TotalNumberOfLongPendingEvents", "ThresholdViolation", (double) longWaitingEvents);
      return longWaitingEvents > 0;
    }

    private Dictionary<string, TimeSpan> GetChangeEventThresholdValues(
      ExecutionContext executionContext)
    {
      int configValue1 = executionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BulkIndexOperationCompletionThresholdInHours", TeamFoundationHostType.ProjectCollection, false, 10);
      int configValue2 = executionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/UpdateIndexOperationCompletionThresholdInHours", TeamFoundationHostType.ProjectCollection, false, 3);
      return new Dictionary<string, TimeSpan>()
      {
        {
          "BeginBulkIndex",
          TimeSpan.FromHours((double) configValue1)
        },
        {
          "UpdateIndex",
          TimeSpan.FromHours((double) configValue2)
        }
      };
    }

    private int GetItemLevelFailureThresholdTime(ExecutionContext executionContext) => executionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FileLevelFailureAlertThreshold", TeamFoundationHostType.ProjectCollection, true, 600) / 60;

    private int GetCountOfLongWaitingEvents(
      ExecutionContext executionContext,
      IIndexingUnitChangeEventDataAccess changeEventDataAccess,
      IndexingUnitChangeEventState indexingUnitChangeEventState,
      int days)
    {
      return changeEventDataAccess.GetCountOfIndexingUnitChangeEvents(executionContext.RequestContext, indexingUnitChangeEventState, days);
    }

    private bool IsItemLevelFailureThresholdViolationCheckEnabled(
      ExecutionContext executionContext,
      string regKey)
    {
      return !executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment && executionContext.RequestContext.GetConfigValue<bool>(regKey, TeamFoundationHostType.ProjectCollection, true, false);
    }
  }
}
