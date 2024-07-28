// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionsReconciliationTelemetryParams
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeExtensionsReconciliationTelemetryParams
  {
    public WorkItemTypeExtension Extension { get; set; }

    public List<FieldEntry> PageFields { get; set; }

    public HashSet<int> ExtensionFields { get; set; }

    public string Wiql { get; set; }

    public bool QueryLimitExceededFallback { get; set; }

    public int InitialActivateCount { get; set; }

    public int ActualActivateCount { get; set; }

    public int InitialDeactivateCount { get; set; }

    public int ActualDeactivateCount { get; set; }

    public int AlreadyActiveCount { get; set; }

    public int ActivateRetryTimes { get; set; }

    public int ActivateRetryCount { get; set; }

    public int DeactivateRetryTimes { get; set; }

    public int DeactivateRetryCount { get; set; }

    public int TotalFetchCount { get; set; }

    public DateTime NewestWorkItemDateTime { get; set; }

    public DateTime OldestWorkItemDateTime { get; set; }

    public int PageWorkItemsDbCount { get; set; }

    public int UpdateWorkItemsDbCount { get; set; }

    public List<int> FieldsToEvaluate { get; set; }

    public long QueryWorkItemsTimeMs { get; set; }

    public long GetActiveWorkItemsTimeMs { get; set; }

    public long PageWorkItemsTimeMs { get; set; }

    public long PageWorkItemsMaxTimeMs { get; set; }

    public long UpdateWorkItemsTimeMs { get; set; }

    public long UpdateWorkItemsMaxTimeMs { get; set; }

    public string LastStepName { get; set; }

    public bool ReconcileResult { get; set; }

    public int LeaseRenewCount { get; set; }

    public void RecordPageWorkItemsTime(long elapsedMilliseconds)
    {
      this.PageWorkItemsTimeMs += elapsedMilliseconds;
      if (elapsedMilliseconds <= this.PageWorkItemsMaxTimeMs)
        return;
      this.PageWorkItemsMaxTimeMs = elapsedMilliseconds;
    }

    public void RecordUpdateWorkItemsTime(long elapsedMilliseconds)
    {
      this.UpdateWorkItemsTimeMs += elapsedMilliseconds;
      if (elapsedMilliseconds <= this.UpdateWorkItemsMaxTimeMs)
        return;
      this.UpdateWorkItemsMaxTimeMs = elapsedMilliseconds;
    }
  }
}
