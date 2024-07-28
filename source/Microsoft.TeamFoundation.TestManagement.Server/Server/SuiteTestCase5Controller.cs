// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteTestCase5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "SuiteTestCase", ResourceVersion = 2)]
  public class SuiteTestCase5Controller : TestManagementController
  {
    [HttpGet]
    [ClientLocationId("a9bd61ac-45cf-4d13-9441-43dcd01edf8d")]
    public List<TestCase> GetTestCase(
      int planId,
      int suiteId,
      string testCaseId,
      string witFields = "",
      bool returnIdentityRef = false)
    {
      ArgumentUtility.CheckForNull<string>(testCaseId, nameof (testCaseId));
      if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.TestManagement.ApiDisallowMultipleIds"))
      {
        if (testCaseId.Split(',').Length > 1)
          throw new ArgumentException(ServerResources.ApiDisallowMultipleTestCaseIdsMessage);
      }
      return this.RevisedTestSuitesHelper.GetTestCases(this.ProjectInfo, planId, suiteId, testCaseId, witFields, returnIdentityRef);
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<TestCase>), null, null)]
    [ClientLocationId("a9bd61ac-45cf-4d13-9441-43dcd01edf8d")]
    public HttpResponseMessage GetTestCaseList(
      int planId,
      int suiteId,
      string testIds = "",
      string configurationIds = "",
      string witFields = "",
      string continuationToken = null,
      bool returnIdentityRef = false,
      bool expand = true,
      ExcludeFlags excludeFlags = ExcludeFlags.None,
      bool isRecursive = false)
    {
      int skipRows;
      int topToFetch;
      int watermark;
      int topRemaining;
      Utils.SetParametersForPaging(0, 0, continuationToken, out skipRows, out topToFetch, out watermark, out topRemaining);
      if (!expand && !excludeFlags.HasFlag((Enum) ExcludeFlags.ExtraInformation))
        excludeFlags |= ExcludeFlags.ExtraInformation;
      List<TestCase> testCaseList = this.RevisedTestSuitesHelper.GetTestCaseList(this.ProjectInfo, planId, suiteId, watermark, skipRows, topToFetch, testIds, configurationIds, witFields, returnIdentityRef, excludeFlags, isRecursive);
      continuationToken = (string) null;
      if (!testCaseList.IsNullOrEmpty<TestCase>() && testCaseList != null && testCaseList.Count >= topToFetch && testCaseList[topToFetch - 1] != null)
      {
        continuationToken = isRecursive ? Utils.GenerateContinuationToken(Convert.ToInt32(testCaseList[topToFetch - 1].workItem.Id), topRemaining) : Utils.GenerateContinuationToken(Convert.ToInt32(testCaseList[topToFetch - 1].Order), topRemaining);
        testCaseList.RemoveAt(topToFetch - 1);
      }
      HttpResponseMessage response = this.GenerateResponse<TestCase>((IEnumerable<TestCase>) testCaseList);
      if (continuationToken != null)
        Utils.SetContinuationToken(response, continuationToken);
      return response;
    }

    [HttpDelete]
    [ClientLocationId("a9bd61ac-45cf-4d13-9441-43dcd01edf8d")]
    public void RemoveTestCasesFromSuite(int planId, int suiteId, string testCaseIds)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.RevisedTestSuitesHelper.RemoveTestCasesFromSuite(this.ProjectInfo, planId, suiteId, testCaseIds);
    }

    [HttpPost]
    [ClientLocationId("a9bd61ac-45cf-4d13-9441-43dcd01edf8d")]
    public List<TestCase> AddTestCasesToSuite(
      int planId,
      int suiteId,
      List<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdateParameters)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestSuitesHelper.AddTestCasesToSuite(this.ProjectInfo, planId, suiteId, suiteTestCaseCreateUpdateParameters);
    }

    [HttpPatch]
    [ClientLocationId("a9bd61ac-45cf-4d13-9441-43dcd01edf8d")]
    public List<TestCase> UpdateSuiteTestCases(
      int planId,
      int suiteId,
      List<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdateParameters)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestSuitesHelper.UpdateSuiteTestCases(this.ProjectInfo, planId, suiteId, suiteTestCaseCreateUpdateParameters);
    }
  }
}
