// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitionBadgeController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "badge", ResourceVersion = 2)]
  public class BuildDefinitionBadgeController : TfsApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (string), null, null)]
    [Obsolete("This endpoint is deprecated. Please see the Build Status REST endpoint.")]
    public HttpResponseMessage GetBadge(Guid project, int definitionId, string branchName = null)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      IBuildDefinitionService service = vssRequestContext.GetService<IBuildDefinitionService>();
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition = (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
      try
      {
        buildDefinition = service.GetDefinition(vssRequestContext, project, definitionId);
      }
      catch (DataspaceNotFoundException ex)
      {
        this.TfsRequestContext.TraceInfo("Service", "Couldn't render definition badge because the project with id {0} was not found.", (object) project);
      }
      if (buildDefinition == null)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.DefinitionNotFound((object) definitionId));
      if (buildDefinition.Repository == null)
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.DefinitionMissingRepositoryInfo((object) definitionId));
      if (!buildDefinition.BadgeEnabled && vssRequestContext.GetService<IProjectService>().GetProjectVisibility(vssRequestContext, buildDefinition.ProjectId) != ProjectVisibility.Public)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.DefinitionNotFound((object) definitionId));
      if (string.IsNullOrWhiteSpace(branchName))
        branchName = buildDefinition.Repository.DefaultBranch;
      Microsoft.TeamFoundation.Build2.Server.BuildResult? branchStatus = vssRequestContext.GetService<IBuildService>().GetBranchStatus(vssRequestContext, project, definitionId, buildDefinition.Repository.Id, buildDefinition.Repository.Type, branchName);
      XDocument svg = StatusBadgeHelper.GetSVG(this.TfsRequestContext, buildDefinition != null, branchStatus, string.Empty);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StringContent(svg.ToString(), Encoding.UTF8, "image/svg+xml");
      return response;
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<DefinitionNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
