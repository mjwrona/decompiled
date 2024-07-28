// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemRestoreTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemRestoreTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_workItemRestore = "WorkItemRestore";

    public WorkItemRestoreTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, string.Empty, 0)
    {
    }

    public static string Feature => "WorkItemRestore";

    public override void AddData(params object[] param)
    {
      if (param == null || param.Length > 2)
        return;
      IEnumerable<WorkItemUpdateResult> source = param[0] as IEnumerable<WorkItemUpdateResult>;
      bool? nullable = param[1] as bool?;
      if (source != null)
      {
        this.ClientTraceData.Add("Count", (object) source.Count<WorkItemUpdateResult>());
        this.ClientTraceData.Add("WorkItemIds", (object) string.Join<int>(", ", source.Select<WorkItemUpdateResult, int>((Func<WorkItemUpdateResult, int>) (r => r.Id))));
        this.ClientTraceData.Add("RestoreErrorCodes", (object) string.Join<int>(", ", source.Select<WorkItemUpdateResult, int>((Func<WorkItemUpdateResult, int>) (r => r.Exception.ErrorCode))));
      }
      if (!nullable.HasValue)
        return;
      this.ClientTraceData.Add("IsRetry", (object) nullable);
    }
  }
}
