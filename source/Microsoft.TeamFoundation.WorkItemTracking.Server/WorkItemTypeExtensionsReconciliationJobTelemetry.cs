// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionsReconciliationJobTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeExtensionsReconciliationJobTelemetry : WorkItemTrackingTelemetry
  {
    private const string featureName = "WorkItemTypeExtensionsReconciliationJob";
    private const string thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemTypeExtensionsReconciliationJobAboveThreshold";
    private const int defaultThresholdTimeMilliseconds = 10000;

    public WorkItemTypeExtensionsReconciliationJobTelemetry(
      IVssRequestContext requestContext,
      string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemTypeExtensionsReconciliationJobAboveThreshold", 10000)
    {
    }

    public static string Feature => "WorkItemTypeExtensionsReconciliationJob";

    public override void AddData(params object[] param)
    {
      if (param.Length != 1 || !(param[0] is WorkItemTypeExtensionsReconciliationJobTelemetryParams jobTelemetryParams))
        return;
      this.ClientTraceData.Add("QueueDateTime", (object) jobTelemetryParams.QueueDateTime.ToString("u"));
      this.ClientTraceData.Add("JobStartDateTime", (object) jobTelemetryParams.JobStartDateTime.ToString("u"));
      ClientTraceData clientTraceData1 = this.ClientTraceData;
      TimeSpan timeSpan = jobTelemetryParams.JobStartDateTime - jobTelemetryParams.QueueDateTime;
      timeSpan = timeSpan.Duration();
      // ISSUE: variable of a boxed type
      __Boxed<int> milliseconds = (ValueType) timeSpan.Milliseconds;
      clientTraceData1.Add("TimeInQueueMs", (object) milliseconds);
      this.ClientTraceData.Add("ResultMessage", (object) jobTelemetryParams.ResultMessage);
      IList<ReconcileSingleExtensionResult> reconcileResults = jobTelemetryParams.ReconcileResults;
      this.ClientTraceData.Add("ReconcileCount", (object) reconcileResults.Count);
      this.ClientTraceData.Add("AcquireLockTimeMs", (object) reconcileResults.Sum<ReconcileSingleExtensionResult>((Func<ReconcileSingleExtensionResult, long>) (r => r.AcquireLockTimeMs)));
      this.ClientTraceData.Add("ReconcileTimeMs", (object) reconcileResults.Sum<ReconcileSingleExtensionResult>((Func<ReconcileSingleExtensionResult, long>) (r => r.ReconcileTimeMs)));
      List<WorkItemTypeExtensionsReconciliationTelemetryParams> list = reconcileResults.Select<ReconcileSingleExtensionResult, WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<ReconcileSingleExtensionResult, WorkItemTypeExtensionsReconciliationTelemetryParams>) (r => r.Telemetry)).Where<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, bool>) (t => t != null)).ToList<WorkItemTypeExtensionsReconciliationTelemetryParams>();
      this.ClientTraceData.Add("ActualReconcileCount", (object) list.Count);
      this.ClientTraceData.Add("SucceededReconcileCount", (object) list.Count<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, bool>) (t => t.ReconcileResult)));
      this.ClientTraceData.Add("InitialActivateCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.InitialActivateCount)));
      this.ClientTraceData.Add("ActualActivateCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.ActualActivateCount)));
      this.ClientTraceData.Add("InitialDeactivateCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.InitialDeactivateCount)));
      this.ClientTraceData.Add("ActualDeactivateCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.ActualDeactivateCount)));
      this.ClientTraceData.Add("AlreadyActiveCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.AlreadyActiveCount)));
      this.ClientTraceData.Add("ActivateRetryTimes", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.ActivateRetryTimes)));
      this.ClientTraceData.Add("ActivateRetryCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.ActivateRetryCount)));
      this.ClientTraceData.Add("DeactivateRetryTimes", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.DeactivateRetryTimes)));
      this.ClientTraceData.Add("DeactivateRetryCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.DeactivateRetryCount)));
      this.ClientTraceData.Add("TotalFetchCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.TotalFetchCount)));
      ClientTraceData clientTraceData2 = this.ClientTraceData;
      DateTime dateTime = list.Select<WorkItemTypeExtensionsReconciliationTelemetryParams, DateTime>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, DateTime>) (t => t.NewestWorkItemDateTime)).DefaultIfEmpty<DateTime>().Max<DateTime>();
      string str1 = dateTime.ToString("u");
      clientTraceData2.Add("NewestWorkItemDateTime", (object) str1);
      ClientTraceData clientTraceData3 = this.ClientTraceData;
      dateTime = list.Select<WorkItemTypeExtensionsReconciliationTelemetryParams, DateTime>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, DateTime>) (t => t.OldestWorkItemDateTime)).DefaultIfEmpty<DateTime>().Min<DateTime>();
      string str2 = dateTime.ToString("u");
      clientTraceData3.Add("OldestWorkItemDateTime", (object) str2);
      this.ClientTraceData.Add("PageWorkItemsDbCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.PageWorkItemsDbCount)));
      this.ClientTraceData.Add("UpdateWorkItemsDbCount", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.UpdateWorkItemsDbCount)));
      this.ClientTraceData.Add("QueryWorkItemsTimeMs", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, long>) (t => t.QueryWorkItemsTimeMs)));
      this.ClientTraceData.Add("GetActiveWorkItemsTimeMs", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, long>) (t => t.GetActiveWorkItemsTimeMs)));
      this.ClientTraceData.Add("PageWorkItemsTimeMs", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, long>) (t => t.PageWorkItemsTimeMs)));
      this.ClientTraceData.Add("PageWorkItemsMaxTimeMs", (object) list.Select<WorkItemTypeExtensionsReconciliationTelemetryParams, long>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, long>) (t => t.PageWorkItemsMaxTimeMs)).DefaultIfEmpty<long>().Max());
      this.ClientTraceData.Add("UpdateWorkItemsTimeMs", (object) list.Sum<WorkItemTypeExtensionsReconciliationTelemetryParams>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, long>) (t => t.UpdateWorkItemsTimeMs)));
      this.ClientTraceData.Add("UpdateWorkItemsMaxTimeMs", (object) list.Select<WorkItemTypeExtensionsReconciliationTelemetryParams, long>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, long>) (t => t.UpdateWorkItemsMaxTimeMs)).DefaultIfEmpty<long>().Max());
      this.ClientTraceData.Add("LeaseRenewCount", (object) list.Select<WorkItemTypeExtensionsReconciliationTelemetryParams, int>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.LeaseRenewCount)).Sum());
      this.ClientTraceData.Add("MaxLeaseRenewCount", (object) list.Select<WorkItemTypeExtensionsReconciliationTelemetryParams, int>((Func<WorkItemTypeExtensionsReconciliationTelemetryParams, int>) (t => t.LeaseRenewCount)).DefaultIfEmpty<int>().Max());
      this.ClientTraceData.Add("JobResult", (object) jobTelemetryParams.JobResult.ToString());
    }
  }
}
