// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FeedbackHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class FeedbackHelper : IFeedbackHelper
  {
    public bool IsFeedBackSession(
      TestManagementRequestContext context,
      int sessionId,
      string projectName)
    {
      bool flag = false;
      List<Session> sessionList1 = new List<Session>();
      if (sessionId != 0)
      {
        List<Session> sessionList2 = Session.Query((TestManagementRequestContext) new TfsTestManagementRequestContext(context.RequestContext.Elevate()), sessionId, Guid.Empty, (string) null, projectName);
        if (sessionList2 != null && sessionList2.Count == 1 && sessionList2[0] != null)
          flag = sessionList2[0].FeedbackId != 0 || sessionList2[0].TestPlanId == 0;
      }
      return flag;
    }
  }
}
