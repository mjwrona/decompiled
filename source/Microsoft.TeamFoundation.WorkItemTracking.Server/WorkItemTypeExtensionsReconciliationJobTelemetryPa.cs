// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionsReconciliationJobTelemetryParams
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeExtensionsReconciliationJobTelemetryParams
  {
    internal DateTime QueueDateTime;
    internal DateTime JobStartDateTime;
    internal WorkItemTypeExtensionReconciliationJobData JobData;
    internal string ResultMessage;
    internal IList<ReconcileSingleExtensionResult> ReconcileResults;
    internal TeamFoundationJobExecutionResult JobResult;

    internal WorkItemTypeExtensionsReconciliationJobTelemetryParams(
      DateTime queueDateTime,
      DateTime jobStartDateTime,
      WorkItemTypeExtensionReconciliationJobData jobData,
      string resultMessage,
      IList<ReconcileSingleExtensionResult> reconcileResults,
      TeamFoundationJobExecutionResult jobResult)
    {
      this.QueueDateTime = queueDateTime;
      this.JobStartDateTime = jobStartDateTime;
      this.JobData = jobData;
      this.ResultMessage = resultMessage;
      this.ReconcileResults = reconcileResults;
      this.JobResult = jobResult;
    }
  }
}
