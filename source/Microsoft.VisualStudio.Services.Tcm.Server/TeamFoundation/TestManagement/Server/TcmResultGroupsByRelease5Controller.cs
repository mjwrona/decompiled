// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmResultGroupsByRelease5Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "resultgroupsbyrelease", ResourceVersion = 1)]
  public class TcmResultGroupsByRelease5Controller : TcmControllerBase
  {
    private ResultGroupsHelper m_resultsGroupsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<FieldDetailsForTestResults>), null, null)]
    [ClientLocationId("3C2B6BB0-0620-434A-A5C3-26AA0FCFDA15")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetResultGroupsByRelease(
      int releaseId,
      string publishContext,
      int releaseEnvId = 0,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = "",
      string continuationToken = null)
    {
      IPagedList<FieldDetailsForTestResults> resultsGroupsByRelease = this.ResultGroupsHelper.GetTestResultsGroupsByRelease(this.ProjectInfo, releaseId, releaseEnvId, publishContext, fields, continuationToken);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult secureObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      };
      HttpResponseMessage response = this.GenerateResponse<FieldDetailsForTestResults>((IEnumerable<FieldDetailsForTestResults>) resultsGroupsByRelease, (ISecuredObject) secureObject);
      if (resultsGroupsByRelease != null && resultsGroupsByRelease.ContinuationToken != null)
        Utils.SetContinuationToken(response, resultsGroupsByRelease.ContinuationToken);
      return response;
    }

    internal ResultGroupsHelper ResultGroupsHelper
    {
      get => this.m_resultsGroupsHelper ?? new ResultGroupsHelper(this.TestManagementRequestContext);
      set => this.m_resultsGroupsHelper = value;
    }
  }
}
