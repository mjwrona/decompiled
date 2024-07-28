// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ImsSyncTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ImsSyncTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/ImsSyncThreshold";
    private const int c_defaultThresholdTime = 0;

    public ImsSyncTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/ImsSyncThreshold", 0)
    {
    }

    public static string Feature => "ImsSync";

    public override void AddData(params object[] param)
    {
      if (param.Length != 2)
        return;
      object obj1 = param[0];
      object obj2 = param[1];
      this.ClientTraceData.Add("TotalMissingIdentities", obj1);
      this.ClientTraceData.Add("FoundIdentities", obj2);
    }
  }
}
