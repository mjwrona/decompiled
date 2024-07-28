// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemIdentityTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemIdentityTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_workItemIdentity = "WorkItemIdentity";
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemIdentityAboveThreshold";
    private const int c_defaultThresholdTime = 700;

    public WorkItemIdentityTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemIdentityAboveThreshold", 700)
    {
    }

    public static string Feature => "WorkItemIdentity";

    public override void AddData(params object[] param)
    {
      if (param.Length != 2)
        return;
      WorkItemIdentityTelemetrySource identityTelemetrySource = (WorkItemIdentityTelemetrySource) param[0];
      if (!(param[1] is ResolvedIdentityNamesInfo identityNamesInfo))
        return;
      int count1 = identityNamesInfo.AmbiguousNamesLookup.Count;
      int count2 = identityNamesInfo.NotFoundNames.Count;
      int count3 = identityNamesInfo.NamesLookup.Count;
      string str1 = string.Join<int?>(",", identityNamesInfo.AmbiguousNamesLookup.Values.Select<ConstantsSearchRecord[], int?>((Func<ConstantsSearchRecord[], int?>) (x => ((IEnumerable<ConstantsSearchRecord>) x).FirstOrDefault<ConstantsSearchRecord>()?.Id)));
      string str2 = string.Join<int>(",", identityNamesInfo.NamesLookup.Values.Select<ConstantsSearchRecord, int>((Func<ConstantsSearchRecord, int>) (x => x.Id)));
      this.ClientTraceData.Add("Source", (object) identityTelemetrySource.ToString());
      this.ClientTraceData.Add("Count-Found", (object) count3);
      this.ClientTraceData.Add("Count-Ambiguous", (object) count1);
      this.ClientTraceData.Add("Count-NotFound", (object) count2);
      this.ClientTraceData.Add("Names-Found-ConstIds", (object) str2);
      this.ClientTraceData.Add("Names-Ambiguous-FirstConstIds", (object) str1);
    }
  }
}
