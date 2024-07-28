// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.RemoteLinkingTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class RemoteLinkingTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_remoteLinking = "RemoteLinkingJob";

    public RemoteLinkingTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, string.Empty, 0)
    {
    }

    public static string Feature => "RemoteLinkingJob";

    public override void AddData(params object[] param)
    {
      if (param.Length != 1 || !(param[0] is RemoteLinkingJobTelemetryParams jobTelemetryParams))
        return;
      this.ClientTraceData.Add("QueueDateTime", (object) jobTelemetryParams.QueueDateTime.ToString("u"));
      this.ClientTraceData.Add("PendingWorkItemRemoteLinks", (object) jobTelemetryParams.PendingWorkItemRemoteLinks);
      this.ClientTraceData.Add("LocalWorkItemLinkUpdates", (object) jobTelemetryParams.LocalWorkItemLinkUpdates);
      this.ClientTraceData.Add("RemoteWorkItemLinkE2EData", (object) jobTelemetryParams.RemoteWorkItemLinkE2EData);
      this.ClientTraceData.Add("ResultMessage", (object) jobTelemetryParams.ResultMessage);
      this.ClientTraceData.Add("JobResult", (object) jobTelemetryParams.JobResult);
      this.ClientTraceData.Add("AuthorizeById", (object) jobTelemetryParams.AuthorizeById);
    }
  }
}
