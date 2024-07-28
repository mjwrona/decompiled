// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ArtifactSourceBranchesController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "sourcebranches")]
  public class ArtifactSourceBranchesController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<string>), null, null)]
    [PublicProjectRequestRestrictions]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    public HttpResponseMessage GetSourceBranches(int definitionId)
    {
      IEnumerable<string> sourceBranches = this.GetService<ReleasesService>().GetSourceBranches(this.TfsRequestContext, this.ProjectId, definitionId);
      List<ReleaseManagementSecuredString> results = new List<ReleaseManagementSecuredString>();
      foreach (string val in sourceBranches)
        results.Add(new ReleaseManagementSecuredString(val));
      this.TfsRequestContext.SetSecuredObjects<ReleaseManagementSecuredString>((IEnumerable<ReleaseManagementSecuredString>) results);
      return this.Request.CreateResponse<List<ReleaseManagementSecuredString>>(HttpStatusCode.OK, results);
    }
  }
}
