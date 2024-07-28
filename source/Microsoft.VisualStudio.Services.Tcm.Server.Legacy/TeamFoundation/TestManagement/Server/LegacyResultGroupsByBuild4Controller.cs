// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LegacyResultGroupsByBuild4Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "resultgroupsbybuild", ResourceVersion = 1)]
  public class LegacyResultGroupsByBuild4Controller : TcmControllerBase
  {
    private ResultGroupsHelper m_resultsGroupsHelper;

    [HttpGet]
    [ClientLocationId("AF70663F-E385-4D73-9D59-3F44A5D9E066")]
    [PublicProjectRequestRestrictions]
    public TestResultsGroupsForBuild GetResultGroupsByBuild(
      int buildId,
      string publishContext,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = "")
    {
      return this.ResultGroupsHelper.GetTestResultsGroupsByBuild(this.ProjectInfo, buildId, publishContext, fields);
    }

    internal ResultGroupsHelper ResultGroupsHelper
    {
      get => this.m_resultsGroupsHelper ?? new ResultGroupsHelper(this.TestManagementRequestContext);
      set => this.m_resultsGroupsHelper = value;
    }
  }
}
