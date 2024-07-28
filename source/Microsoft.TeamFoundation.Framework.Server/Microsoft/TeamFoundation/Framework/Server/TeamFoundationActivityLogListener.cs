// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationActivityLogListener
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationActivityLogListener : ITeamFoundationLoggingListener
  {
    private bool m_logToDatabase;

    public TeamFoundationActivityLogListener(bool logToDatabase) => this.m_logToDatabase = logToDatabase;

    public void SubmitRequestDetails(
      IVssRequestContext systemRequestContext,
      IEnumerable<RequestDetails> requestList,
      int maxCompressedThresholdTime,
      TeamFoundationLoggingLevel level)
    {
      TeamFoundationTracingService service = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTracingService>();
      if (requestList == null)
        return;
      foreach (RequestDetails request in requestList)
        service.TraceActivityLog(request);
      systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<MdmService>().PublishSli(systemRequestContext, requestList);
      if (!this.m_logToDatabase)
        return;
      if (level != TeamFoundationLoggingLevel.All && systemRequestContext.ExecutionEnvironment.IsCloudDeployment)
        requestList = (IEnumerable<RequestDetails>) requestList.Where<RequestDetails>((Func<RequestDetails, bool>) (x => x.IncludeMethodDetails(level == TeamFoundationLoggingLevel.All, maxCompressedThresholdTime))).ToList<RequestDetails>();
      if (!requestList.Any<RequestDetails>())
        return;
      using (RecordListenerComponent component = systemRequestContext.CreateComponent<RecordListenerComponent>())
        component.SubmitRecords(requestList, level == TeamFoundationLoggingLevel.All, maxCompressedThresholdTime);
    }
  }
}
