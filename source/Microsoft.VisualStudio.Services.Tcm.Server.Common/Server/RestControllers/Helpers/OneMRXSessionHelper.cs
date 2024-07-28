// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers.OneMRXSessionHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers
{
  [CLSCompliant(false)]
  public class OneMRXSessionHelper : RestApiHelper
  {
    public OneMRXSessionHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public long CreateTestSession(Guid projectId, TestResultsSession session)
    {
      ArgumentUtility.CheckForNull<TestResultsSession>(session, nameof (session), "Test Results");
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SessionsHelper.CreateTestSession projectId = {0}", (object) projectId);
      return this.TeamFoundationOneMRXSessionService.CreateSession(this.TestManagementRequestContext, new SessionDataContractConverter(this.TestManagementRequestContext).UpdateSession(session), projectReference);
    }

    public void CreateConfigAndEnvironment(
      Guid projectId,
      IList<TestSessionEnvironment> environments)
    {
      ArgumentUtility.CheckForNull<IList<TestSessionEnvironment>>(environments, nameof (environments), "Test Results");
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SessionsHelper.CreateConfigAndEnvironment projectId = {0}", (object) projectId);
      this.TeamFoundationOneMRXSessionService.CreateConfigAndEnvironment(this.TestManagementRequestContext, environments, projectReference);
    }

    public List<TestResultsSession> GetTestSessionByBuildId(Guid projectId, int buildId)
    {
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SessionsHelper.GetTestSessionByBuildId projectId = {0}", (object) projectId);
      return new SessionDataContractConverter(this.TestManagementRequestContext).GetSession(this.TeamFoundationOneMRXSessionService.GetSessionsByBuildId(this.TestManagementRequestContext, buildId, projectReference));
    }

    public List<Layout> GetTestSessionLayoutBySessionId(Guid projectId, Guid sessionId)
    {
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SessionsHelper.GetTestSessionLayoutBySessionId projectId = {0}", (object) projectId);
      return this.TeamFoundationOneMRXSessionService.GetSessionLayoutBySessionId(this.TestManagementRequestContext, sessionId, projectReference);
    }
  }
}
