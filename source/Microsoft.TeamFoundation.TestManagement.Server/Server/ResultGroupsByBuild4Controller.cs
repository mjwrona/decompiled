// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultGroupsByBuild4Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultGroupsByBuild", ResourceVersion = 1)]
  public class ResultGroupsByBuild4Controller : TestManagementController
  {
    private ResultGroupsHelper m_resultsGroupsHelper;

    [HttpGet]
    [ClientLocationId("D279D052-C55A-4204-B913-42F733B52958")]
    [FeatureEnabled("TestManagement.Server.ResultGroupsByBuild")]
    [PublicProjectRequestRestrictions]
    public TestResultsGroupsForBuild GetResultGroupsByBuild(
      int buildId,
      string publishContext,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = "")
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultGroupsHelper.GetTestResultsGroupsByBuild(this.ProjectInfo, buildId, publishContext, fields);
      List<string> groupByFields = ParsingHelper.ParseCommaSeparatedString(fields);
      TestResultsGroupsForBuild resultGroupsByBuild = TestManagementController.InvokeAction<TestResultsGroupsForBuild>((Func<TestResultsGroupsForBuild>) (() => this.TestResultsHttpClient.GetResultGroupsByBuildV1Async(this.ProjectInfo.Id, buildId, publishContext, (IEnumerable<string>) groupByFields)?.Result));
      if (resultGroupsByBuild == null)
        return resultGroupsByBuild;
      resultGroupsByBuild.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      });
      return resultGroupsByBuild;
    }

    internal ResultGroupsHelper ResultGroupsHelper
    {
      get => this.m_resultsGroupsHelper ?? new ResultGroupsHelper(this.TestManagementRequestContext);
      set => this.m_resultsGroupsHelper = value;
    }
  }
}
