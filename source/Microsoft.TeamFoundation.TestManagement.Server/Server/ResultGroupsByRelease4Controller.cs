// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultGroupsByRelease4Controller
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
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultGroupsByRelease", ResourceVersion = 1)]
  public class ResultGroupsByRelease4Controller : TestManagementController
  {
    private ResultGroupsHelper m_resultsGroupsHelper;

    [HttpGet]
    [ClientLocationId("EF5CE5D4-A4E5-47EE-804C-354518F8D03F")]
    [FeatureEnabled("TestManagement.Server.ResultGroupsByRelease")]
    [PublicProjectRequestRestrictions]
    public TestResultsGroupsForRelease GetResultGroupsByRelease(
      int releaseId,
      string publishContext,
      int releaseEnvId = 0,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = "")
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultGroupsHelper.GetTestResultsGroupsByRelease(this.ProjectInfo, releaseId, releaseEnvId, publishContext, fields);
      List<string> groupByFields = ParsingHelper.ParseCommaSeparatedString(fields);
      TestResultsGroupsForRelease resultGroupsByRelease = TestManagementController.InvokeAction<TestResultsGroupsForRelease>((Func<TestResultsGroupsForRelease>) (() => this.TestResultsHttpClient.GetResultGroupsByReleaseV1Async(this.ProjectId.ToString(), releaseId, publishContext, new int?(releaseEnvId), (IEnumerable<string>) groupByFields)?.Result));
      if (resultGroupsByRelease == null)
        return resultGroupsByRelease;
      resultGroupsByRelease.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      });
      return resultGroupsByRelease;
    }

    internal ResultGroupsHelper ResultGroupsHelper
    {
      get => this.m_resultsGroupsHelper ?? new ResultGroupsHelper(this.TestManagementRequestContext);
      set => this.m_resultsGroupsHelper = value;
    }
  }
}
