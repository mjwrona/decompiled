// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LegacyResultGroupsByBuild5Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

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
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "resultgroupsbybuild", ResourceVersion = 2)]
  public class LegacyResultGroupsByBuild5Controller : TcmControllerBase
  {
    private ResultGroupsHelper m_resultsGroupsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<FieldDetailsForTestResults>), null, null)]
    [ClientLocationId("AF70663F-E385-4D73-9D59-3F44A5D9E066")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetResultGroupsByBuild(
      int buildId,
      string publishContext,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = "",
      string continuationToken = null)
    {
      IPagedList<FieldDetailsForTestResults> resultsGroupsByBuild = this.ResultGroupsHelper.GetTestResultsGroupsByBuild(this.ProjectInfo, buildId, publishContext, fields, continuationToken);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult secureObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      };
      HttpResponseMessage response = this.GenerateResponse<FieldDetailsForTestResults>((IEnumerable<FieldDetailsForTestResults>) resultsGroupsByBuild, (ISecuredObject) secureObject);
      if (resultsGroupsByBuild != null && resultsGroupsByBuild.ContinuationToken != null)
        Utils.SetContinuationToken(response, resultsGroupsByBuild.ContinuationToken);
      return response;
    }

    internal ResultGroupsHelper ResultGroupsHelper
    {
      get => this.m_resultsGroupsHelper ?? new ResultGroupsHelper(this.TestManagementRequestContext);
      set => this.m_resultsGroupsHelper = value;
    }
  }
}
