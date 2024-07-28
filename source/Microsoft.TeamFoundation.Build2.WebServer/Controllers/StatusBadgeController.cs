// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.StatusBadgeController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "status", ResourceVersion = 1)]
  public class StatusBadgeController : TfsApiController
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpExceptionMapping = new Dictionary<Type, HttpStatusCode>()
    {
      [typeof (AmbiguousDefinitionNameException)] = HttpStatusCode.BadRequest
    };

    [HttpGet]
    [ClientResponseType(typeof (string), null, null)]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public HttpResponseMessage GetStatusBadge(
      string definition,
      string branchName = null,
      string stageName = null,
      string jobName = null,
      string configuration = null,
      string label = null)
    {
      ArgumentUtility.CheckForNull<string>(definition, nameof (definition));
      Guid projectId;
      if (!this.TryGetProjectId(out projectId) || projectId == new Guid())
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.DefinitionNotFound((object) definition));
      IVssRequestContext requestContext1 = this.TfsRequestContext.Elevate();
      IVssRequestContext requestContext2 = new ProjectPipelineGeneralSettingsHelper(requestContext1, projectId, true).StatusBadgesArePublic ? requestContext1 : this.TfsRequestContext;
      (Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition, Microsoft.TeamFoundation.Build2.Server.BuildResult? nullable) = this.TfsRequestContext.GetService<ILatestBuildResultCacheService>().GetBuildResult(requestContext2, projectId, definition, branchName, stageName, jobName, configuration);
      XDocument svg = StatusBadgeHelper.GetSVG(this.TfsRequestContext, buildDefinition != null, nullable, label);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerStringContent(svg.ToString(), Encoding.UTF8, "image/svg+xml", (object) buildDefinition);
      return response;
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) StatusBadgeController.s_httpExceptionMapping;

    protected IBuildService BuildService => this.TfsRequestContext.GetService<IBuildService>();

    protected IBuildDefinitionService DefinitionService => this.TfsRequestContext.GetService<IBuildDefinitionService>();

    protected virtual bool TryGetProjectId(out Guid projectId)
    {
      projectId = new Guid();
      string str;
      if (!this.ControllerContext.RouteData.Values.TryGetValue<string>("project", out str))
        return false;
      if (!Guid.TryParse(str, out projectId))
      {
        IProjectService service = this.TfsRequestContext.GetService<IProjectService>();
        try
        {
          projectId = service.GetProjectId(this.TfsRequestContext.Elevate(), str, true);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          this.TfsRequestContext.TraceException(0, TraceLevel.Info, "Build2", nameof (StatusBadgeController), (Exception) ex);
          return false;
        }
      }
      return true;
    }
  }
}
