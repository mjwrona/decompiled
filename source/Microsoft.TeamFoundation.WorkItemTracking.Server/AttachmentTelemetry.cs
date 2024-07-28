// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AttachmentTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class AttachmentTelemetry : WorkItemTrackingTelemetry
  {
    public AttachmentTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, string.Empty, 0)
    {
    }

    public static string Feature => "AttachmentService";

    public override void AddData(params object[] param)
    {
      if (param.Length < 3)
        return;
      string str1 = param[0] as string;
      string str2 = param[1] as string;
      string str3 = param[2] as string;
      if (param.Length == 4 && param[3] is string str4)
        this.ClientTraceData.Add("FileGuid", (object) str4);
      if (str1 != null)
        this.ClientTraceData.Add("Action", (object) str1);
      if (str2 != null)
        this.ClientTraceData.Add("Id", (object) str2);
      if (str3 == null)
        return;
      this.ClientTraceData.Add("FileSize", (object) str3);
    }
  }
}
