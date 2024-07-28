// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildBadgeController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "buildbadge", ResourceVersion = 1)]
  [ClientGroupByResource("badge")]
  public class BuildBadgeController : BuildApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (string), null, null, MethodName = "GetBuildBadgeData")]
    [ClientResponseType(typeof (BuildBadge), null, null, MethodName = "GetBuildBadge")]
    public HttpResponseMessage GetBuildBadge(string repoType, [ClientQueryParameter] string repoId = null, [ClientQueryParameter] string branchName = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repoType, nameof (repoType));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(branchName, nameof (branchName));
      IBuildService service1 = this.TfsRequestContext.GetService<IBuildService>();
      bool isDefinitionConfigured = true;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      string repositoryIdentifier = repoId;
      string repositoryType = repoType;
      string branchName1 = branchName;
      BuildData latestCompletedBuild = service1.GetLatestCompletedBuild(tfsRequestContext, projectId, repositoryIdentifier, repositoryType, branchName1);
      if (latestCompletedBuild == null && this.TfsRequestContext.GetService<IBuildDefinitionService>().GetDefinitionsForRepository(this.TfsRequestContext, this.ProjectId, repoType, repoId, count: 1).IsNullOrEmpty<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>())
        isDefinitionConfigured = false;
      foreach (MediaTypeHeaderValue mediaTypeHeaderValue in this.Request.Headers.Accept)
      {
        if (mediaTypeHeaderValue.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
        {
          BuildBadge buildBadge = new BuildBadge();
          if (latestCompletedBuild != null)
            buildBadge.BuildId = latestCompletedBuild.Id;
          IBuildRouteService service2 = this.TfsRequestContext.GetService<IBuildRouteService>();
          buildBadge.ImageUrl = service2.GetBranchBadgeRestUrl(this.TfsRequestContext, this.ProjectId, repoType, branchName, repoId);
          return this.Request.CreateResponse<BuildBadge>(HttpStatusCode.OK, buildBadge);
        }
      }
      XDocument svg = StatusBadgeHelper.GetSVG(this.TfsRequestContext, isDefinitionConfigured, (Microsoft.TeamFoundation.Build2.Server.BuildResult?) latestCompletedBuild?.Result, string.Empty);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StringContent(svg.ToString(), Encoding.UTF8, "image/svg+xml");
      return response;
    }
  }
}
