// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmLinkedWorkItemsToResultController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "bugs", ResourceVersion = 1)]
  public class TcmLinkedWorkItemsToResultController : TcmControllerBase
  {
    private TestLinkedWorkItemsHelper m_testLinkedWorkItemsHelper;

    [HttpGet]
    [ClientLocationId("D8DBF98F-EB34-4F8D-8365-47972AF34F29")]
    [PublicProjectRequestRestrictions]
    public IEnumerable<WorkItemReference> GetBugsLinkedToTestResult(int runId, int testCaseResultId)
    {
      this.TestManagementRequestContext.SecurityManager.CheckViewTestResultsPermission(this.TestManagementRequestContext, this.ProjectInfo.Uri);
      return (IEnumerable<WorkItemReference>) this.TestLinkedWorkItemsHelper.GetAssociatedBugsForResult(this.ProjectId, runId, testCaseResultId);
    }

    internal TestLinkedWorkItemsHelper TestLinkedWorkItemsHelper
    {
      get
      {
        if (this.m_testLinkedWorkItemsHelper == null)
          this.m_testLinkedWorkItemsHelper = new TestLinkedWorkItemsHelper(this.TestManagementRequestContext);
        return this.m_testLinkedWorkItemsHelper;
      }
    }
  }
}
