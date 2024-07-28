// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LegacyLinkedWorkItemsToResultController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "bugs", ResourceVersion = 1)]
  public class LegacyLinkedWorkItemsToResultController : TcmControllerBase
  {
    private TestLinkedWorkItemsHelper m_testLinkedWorkItemsHelper;

    [HttpGet]
    [ClientLocationId("F337D599-48F4-41A9-A21D-02892899FAAF")]
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
