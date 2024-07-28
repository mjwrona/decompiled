// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemMetadataTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemMetadataTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_workItemMetadata = "WorkItemMetadata";
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemMetadataAboveThreshold";
    private const int c_defaultThresholdTime = 500;

    public WorkItemMetadataTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemMetadataAboveThreshold", 500)
    {
    }

    public static string Feature => "WorkItemMetadata";

    public override void AddData(params object[] param)
    {
      if (param.Length != 2)
        return;
      bool? nullable = param[0] as bool?;
      if (nullable.HasValue)
        this.ClientTraceData.Add("isIncremental", (object) nullable.Value);
      if (!(param[1] is IDictionary<string, int> dictionary))
        return;
      foreach (KeyValuePair<string, int> keyValuePair in (IEnumerable<KeyValuePair<string, int>>) dictionary)
        this.ClientTraceData.Add(keyValuePair.Key + "_rowCount", (object) keyValuePair.Value);
    }
  }
}
