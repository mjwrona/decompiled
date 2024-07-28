// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmTestLinkedWorkItemsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "workitems", ResourceVersion = 1)]
  public class TcmTestLinkedWorkItemsController : TcmControllerBase
  {
    private TestLinkedWorkItemsHelper _testLinkedWorkItemsHelper;
    private const int c_defaultWorkItemCount = 100;

    [HttpGet]
    [ClientLocationId("F7401A26-331B-44FE-A470-F7ED35138E4A")]
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
    [ClientLocationId("CBD50BD7-F7ED-4E35-B127-4408AE6BFA2C")]
    [PublicProjectRequestRestrictions]
    public TestToWorkItemLinks QueryTestMethodLinkedWorkItems([ClientQueryParameter] string testName) => this.TestLinkedWorkItemsHelper.QueryLinkedWorkItems(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), testName);

    [ValidateModel]
    [HttpPost]
    [ClientLocationId("4E3ABE63-CA46-4FE0-98B2-363F7EC7AA5F")]
    public WorkItemToTestLinks AddWorkItemToTestLinks(WorkItemToTestLinks workItemToTestLinks) => this.TestLinkedWorkItemsHelper.AddWorkItemLink(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), workItemToTestLinks);

    [HttpDelete]
    [ClientLocationId("CBD50BD7-F7ED-4E35-B127-4408AE6BFA2C")]
    public bool DeleteTestMethodToWorkItemLink([ClientQueryParameter] string testName, [ClientQueryParameter] int workItemId) => this.TestLinkedWorkItemsHelper.DeleteWorkItemLink(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), testName, workItemId);

    internal TestLinkedWorkItemsHelper TestLinkedWorkItemsHelper
    {
      get => this._testLinkedWorkItemsHelper ?? new TestLinkedWorkItemsHelper(this.TestManagementRequestContext);
      set => this._testLinkedWorkItemsHelper = value;
    }
  }
}
