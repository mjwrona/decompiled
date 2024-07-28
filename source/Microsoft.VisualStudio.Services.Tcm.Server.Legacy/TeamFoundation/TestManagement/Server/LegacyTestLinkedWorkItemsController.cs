// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LegacyTestLinkedWorkItemsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "workitems", ResourceVersion = 1)]
  public class LegacyTestLinkedWorkItemsController : TcmControllerBase
  {
    private TestLinkedWorkItemsHelper _testLinkedWorkItemsHelper;
    private const int c_defaultWorkItemCount = 100;

    [HttpGet]
    [ClientLocationId("C7D72725-0479-4C83-8DBC-2BF27010B78B")]
    [PublicProjectRequestRestrictions]
    public List<WorkItemReference> QueryTestResultWorkItems(
      string workItemCategory,
      string automatedTestName = "",
      int testCaseId = 0,
      DateTime? maxCompleteDate = null,
      int days = -2147483648,
      [FromUri(Name = "$workItemCount")] int workItemCount = 100)
    {
      days = days > int.MinValue ? days : TestManagementServiceUtility.GetMaxDaysForTestResultsWorkItems(this.TestManagementRequestContext.RequestContext);
      return this.TestLinkedWorkItemsHelper.QueryTestResultWorkItems(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), workItemCategory, automatedTestName, testCaseId, maxCompleteDate, days, workItemCount);
    }

    [HttpPost]
    [ClientLocationId("F961577C-5122-4651-ADFD-C9D500B15D41")]
    [PublicProjectRequestRestrictions]
    public TestToWorkItemLinks QueryTestMethodLinkedWorkItems([ClientQueryParameter] string testName) => this.TestLinkedWorkItemsHelper.QueryLinkedWorkItems(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), testName);

    [ValidateModel]
    [HttpPost]
    [ClientLocationId("84CCB084-255B-483E-B7EA-9D39A273B5FE")]
    public WorkItemToTestLinks AddWorkItemToTestLinks(WorkItemToTestLinks workItemToTestLinks) => this.TestLinkedWorkItemsHelper.AddWorkItemLink(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), workItemToTestLinks);

    [HttpDelete]
    [ClientLocationId("F961577C-5122-4651-ADFD-C9D500B15D41")]
    public bool DeleteTestMethodToWorkItemLink([ClientQueryParameter] string testName, [ClientQueryParameter] int workItemId) => this.TestLinkedWorkItemsHelper.DeleteWorkItemLink(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), testName, workItemId);

    internal TestLinkedWorkItemsHelper TestLinkedWorkItemsHelper
    {
      get => this._testLinkedWorkItemsHelper ?? new TestLinkedWorkItemsHelper(this.TestManagementRequestContext);
      set => this._testLinkedWorkItemsHelper = value;
    }
  }
}
