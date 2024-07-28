// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LinkedWorkItemsToResultController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(2.0)]
  [DemandFeature("402E4502-9389-420C-BA11-796CDA2E4867", true)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Bugs", ResourceVersion = 1)]
  public class LinkedWorkItemsToResultController : TestResultsControllerBase
  {
    private TestLinkedWorkItemsHelper m_testLinkedWorkItemsHelper;

    [HttpGet]
    [ClientLocationId("6DE20CA2-67DE-4FAF-97FA-38C5D585EB00")]
    [PublicProjectRequestRestrictions]
    public IEnumerable<WorkItemReference> GetBugsLinkedToTestResult(int runId, int testCaseResultId)
    {
      this.TestManagementRequestContext.SecurityManager.CheckViewTestResultsPermission(this.TestManagementRequestContext, this.ProjectInfo.Uri);
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return (IEnumerable<WorkItemReference>) this.TestLinkedWorkItemsHelper.GetAssociatedBugsForResult(this.ProjectId, runId, testCaseResultId);
      List<WorkItemReference> references = TestManagementController.InvokeAction<List<WorkItemReference>>((Func<List<WorkItemReference>>) (() => this.TestResultsHttpClient.GetBugsLinkedToTestResultAsync(this.ProjectId, runId, testCaseResultId)?.Result));
      this.TestLinkedWorkItemsHelper.SecureWorkItemReferences(this.ProjectId, references);
      return (IEnumerable<WorkItemReference>) references;
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
