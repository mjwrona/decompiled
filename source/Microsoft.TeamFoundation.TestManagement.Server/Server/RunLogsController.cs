// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RunLogsController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "MessageLogs", ResourceVersion = 1)]
  public class RunLogsController : TestResultsControllerBase
  {
    private RunLogsHelper m_runLogsHelper;

    [HttpGet]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientResponseType(typeof (IList<TestMessageLogDetails>), null, null)]
    [ClientLocationId("A1E55200-637E-42E9-A7C0-7E5BFDEDB1B3")]
    public HttpResponseMessage GetTestRunLogs(int runId) => !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.GenerateResponse<TestMessageLogDetails>((IEnumerable<TestMessageLogDetails>) this.RunLogsHelper.GetTestRunLogs(this.ProjectId.ToString(), runId)) : this.GenerateResponse<TestMessageLogDetails>((IEnumerable<TestMessageLogDetails>) TestManagementController.InvokeAction<List<TestMessageLogDetails>>((Func<List<TestMessageLogDetails>>) (() => this.TestResultsHttpClient.GetTestRunMessageLogsAsync(this.ProjectId.ToString(), runId)?.Result)));

    internal RunLogsHelper RunLogsHelper
    {
      get
      {
        if (this.m_runLogsHelper == null)
          this.m_runLogsHelper = new RunLogsHelper(this.TestManagementRequestContext);
        return this.m_runLogsHelper;
      }
    }
  }
}
