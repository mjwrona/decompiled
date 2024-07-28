// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmRunLogsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "MessageLogs", ResourceVersion = 1)]
  public class TcmRunLogsController : TcmControllerBase
  {
    private RunLogsHelper m_runLogsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IList<TestMessageLogDetails>), null, null)]
    [ClientLocationId("E9AB0C6A-1984-418B-87C0-EE4202318BA3")]
    public HttpResponseMessage GetTestRunMessageLogs(int runId) => this.GenerateResponse<TestMessageLogDetails>((IEnumerable<TestMessageLogDetails>) this.RunLogsHelper.GetTestRunLogs(this.ProjectId.ToString(), runId));

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
