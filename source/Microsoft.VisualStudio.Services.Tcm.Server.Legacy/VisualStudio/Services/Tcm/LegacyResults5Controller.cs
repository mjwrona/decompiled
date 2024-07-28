// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyResults5Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "resultsandruns", ResourceVersion = 1)]
  public class LegacyResults5Controller : TcmControllerBase
  {
    [HttpPost]
    [ClientLocationId("CE47A161-B387-4F55-B22E-2D3D5956FC1D")]
    public List<LegacyTestCaseResult> CreateBatchedBlockedResults(
      List<LegacyTestCaseResult> testCaseResults)
    {
      NewProjectStepsPerformer.InitializeNewProject(this.TestManagementRequestContext, this.ProjectId);
      return this.TestManagementRequestContext.RequestContext.GetService<ITestManagementLegacyResultService>().CreateBlockedResults(this.TestManagementRequestContext, new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), testCaseResults);
    }
  }
}
