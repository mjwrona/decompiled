// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultGroupsByBuild5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultGroupsByBuild", ResourceVersion = 2)]
  public class ResultGroupsByBuild5Controller : TestManagementController
  {
    private ResultGroupsHelper m_resultsGroupsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<FieldDetailsForTestResults>), null, null)]
    [ClientLocationId("D279D052-C55A-4204-B913-42F733B52958")]
    [PublicProjectRequestRestrictions]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage GetResultGroupsByBuild(
      int buildId,
      string publishContext,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = "",
      string continuationToken = null)
    {
      IPagedList<FieldDetailsForTestResults> pagedList;
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        pagedList = this.ResultGroupsHelper.GetTestResultsGroupsByBuild(this.ProjectInfo, buildId, publishContext, fields, continuationToken);
      }
      else
      {
        List<string> groupByFields = ParsingHelper.ParseCommaSeparatedString(fields);
        pagedList = TestManagementController.InvokeAction<IPagedList<FieldDetailsForTestResults>>((Func<IPagedList<FieldDetailsForTestResults>>) (() => this.TestResultsHttpClient.GetResultGroupsByBuildWithContinuationTokenAsync(this.ProjectInfo.Id, buildId, publishContext, (IEnumerable<string>) groupByFields, continuationToken)?.Result));
        this.ResultGroupsHelper.SecureTestResultsGroups(pagedList, this.ProjectInfo.Id);
      }
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult secureObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      };
      HttpResponseMessage response = this.GenerateResponse<FieldDetailsForTestResults>((IEnumerable<FieldDetailsForTestResults>) pagedList, (ISecuredObject) secureObject);
      if (pagedList != null && pagedList.ContinuationToken != null)
        Utils.SetContinuationToken(response, pagedList.ContinuationToken);
      return response;
    }

    internal ResultGroupsHelper ResultGroupsHelper
    {
      get => this.m_resultsGroupsHelper ?? new ResultGroupsHelper(this.TestManagementRequestContext);
      set => this.m_resultsGroupsHelper = value;
    }
  }
}
