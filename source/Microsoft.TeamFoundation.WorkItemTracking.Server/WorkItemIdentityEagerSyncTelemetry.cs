// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemIdentityEagerSyncTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemIdentityEagerSyncTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_workItemIdentityForceSync = "WorkItemIdentityEagerSync";
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemIdentityEagerSyncAboveThreshold";
    private const int c_defaultThresholdTime = 0;

    public WorkItemIdentityEagerSyncTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemIdentityEagerSyncAboveThreshold", 0)
    {
    }

    public static string Feature => "WorkItemIdentityEagerSync";

    public override void AddData(params object[] param)
    {
      if (param.Length != 3)
        return;
      Guid[] values = param[0] as Guid[];
      bool? nullable1 = param[1] as bool?;
      bool? nullable2 = param[1] as bool?;
      if (values == null || !nullable1.HasValue || !nullable2.HasValue)
        return;
      int length = values.Length;
      string str = string.Join<Guid>(",", (IEnumerable<Guid>) values);
      this.ClientTraceData.Add("Count-NotFound", (object) length);
      this.ClientTraceData.Add("VSIDs-NotFound", (object) str);
      this.ClientTraceData.Add("AllResolvedInIMS", (object) nullable1.Value);
      this.ClientTraceData.Add("AllSyncedToConstants", (object) nullable2.Value);
    }
  }
}
