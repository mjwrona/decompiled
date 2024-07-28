// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LegacyResultGroupsByRelease4Controller
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
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "resultgroupsbyrelease", ResourceVersion = 1)]
  public class LegacyResultGroupsByRelease4Controller : TcmControllerBase
  {
    private ResultGroupsHelper m_resultsGroupsHelper;

    [HttpGet]
    [ClientLocationId("5E746C5C-4FB7-46F7-BC6C-913110A98FBF")]
    [PublicProjectRequestRestrictions]
    public TestResultsGroupsForRelease GetResultGroupsByRelease(
      int releaseId,
      string publishContext,
      int releaseEnvId = 0,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = "")
    {
      return this.ResultGroupsHelper.GetTestResultsGroupsByRelease(this.ProjectInfo, releaseId, releaseEnvId, publishContext, fields);
    }

    internal ResultGroupsHelper ResultGroupsHelper
    {
      get => this.m_resultsGroupsHelper ?? new ResultGroupsHelper(this.TestManagementRequestContext);
      set => this.m_resultsGroupsHelper = value;
    }
  }
}
