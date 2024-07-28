// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLinkedWorkItemsController
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
  [ControllerApiVersion(3.0)]
  [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "WorkItems", ResourceVersion = 1)]
  public class TestLinkedWorkItemsController : TestResultsControllerBase
  {
    private TestLinkedWorkItemsHelper _testLinkedWorkItemsHelper;
    private const int c_defaultWorkItemCount = 100;

    [HttpGet]
    [ClientLocationId("926FF5DC-137F-45F0-BD51-9412FA9810CE")]
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
      GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.TestLinkedWorkItemsHelper.QueryTestResultWorkItems(projectId, workItemCategory, automatedTestName, testCaseId, maxCompleteDate, days, workItemCount);
      List<WorkItemReference> references = TestManagementController.InvokeAction<List<WorkItemReference>>((Func<List<WorkItemReference>>) (() => this.TestResultsHttpClient.QueryTestResultWorkItemsAsync(projectId.GuidId, workItemCategory, automatedTestName, new int?(testCaseId), maxCompleteDate, new int?(days), new int?(workItemCount))?.Result));
      this.TestLinkedWorkItemsHelper.SecureWorkItemReferences(projectId.GuidId, references);
      return references;
    }

    [HttpPost]
    [ClientLocationId("7B0BDEE3-A354-47F9-A42C-89018D7808D5")]
    [PublicProjectRequestRestrictions]
    public TestToWorkItemLinks QueryTestMethodLinkedWorkItems([ClientQueryParameter] string testName)
    {
      GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.TestLinkedWorkItemsHelper.QueryLinkedWorkItems(projectId, testName);
      TestToWorkItemLinks linkedRequirementsForTest = TestManagementController.InvokeAction<TestToWorkItemLinks>((Func<TestToWorkItemLinks>) (() => this.TestResultsHttpClient.QueryTestMethodLinkedWorkItemsAsync(projectId.GuidId, testName)?.Result));
      this.TestLinkedWorkItemsHelper.SecurifyLinkedRequirementsForTest(linkedRequirementsForTest, projectId.GuidId);
      return linkedRequirementsForTest;
    }

    [HttpPost]
    [ClientLocationId("371B1655-CE05-412E-A113-64CC77BB78D2")]
    public WorkItemToTestLinks AddWorkItemToTestLinks(WorkItemToTestLinks workItemToTestLinks)
    {
      GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.TestLinkedWorkItemsHelper.AddWorkItemLink(projectId, workItemToTestLinks) : TestManagementController.InvokeAction<WorkItemToTestLinks>((Func<WorkItemToTestLinks>) (() => this.TestResultsHttpClient.AddWorkItemToTestLinksAsync(workItemToTestLinks, projectId.GuidId)?.Result));
    }

    [HttpDelete]
    [ClientLocationId("7B0BDEE3-A354-47F9-A42C-89018D7808D5")]
    public bool DeleteTestMethodToWorkItemLink([ClientQueryParameter] string testName, [ClientQueryParameter] int workItemId)
    {
      GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.TestLinkedWorkItemsHelper.DeleteWorkItemLink(projectId, testName, workItemId) : TestManagementController.InvokeAction<bool>((Func<bool>) (() => this.TestResultsHttpClient.DeleteTestMethodToWorkItemLinkAsync(projectId.GuidId, testName, workItemId).Result));
    }

    internal TestLinkedWorkItemsHelper TestLinkedWorkItemsHelper
    {
      get => this._testLinkedWorkItemsHelper ?? new TestLinkedWorkItemsHelper(this.TestManagementRequestContext);
      set => this._testLinkedWorkItemsHelper = value;
    }
  }
}
