// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RunLogsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class RunLogsHelper : RestApiHelper
  {
    internal RunLogsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<TestMessageLogDetails> GetTestRunLogs(string projectId, int runId)
    {
      this.RequestContext.TraceInfo("RestLayer", "RunLogsHelper.GetTestRunLogs projectId = {0}, runId = {1}", (object) projectId, (object) runId);
      return this.ExecuteAction<List<TestMessageLogDetails>>("RunLogsHelper.GetTestRunLogs", (Func<List<TestMessageLogDetails>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          List<TestMessageLogDetails> testMessageLogs;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestRunLogs(this.RequestContext, projectReference.Id, runId, out testMessageLogs))
            return testMessageLogs;
        }
        else if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        return this.ConvertTestRunLogsToDataContract(this.TestManagementRunService.QueryTestRunLogs(this.TestManagementRequestContext, runId, projectReference));
      }), 1015062, "TestManagement");
    }

    private List<TestMessageLogDetails> ConvertTestRunLogsToDataContract(
      List<TestMessageLogEntry> runLogs)
    {
      List<TestMessageLogDetails> dataContract = new List<TestMessageLogDetails>();
      foreach (TestMessageLogEntry runLog in runLogs)
        dataContract.Add(new TestMessageLogDetails()
        {
          EntryId = runLog.EntryId,
          DateCreated = runLog.DateCreated,
          Message = runLog.Message
        });
      return dataContract;
    }
  }
}
