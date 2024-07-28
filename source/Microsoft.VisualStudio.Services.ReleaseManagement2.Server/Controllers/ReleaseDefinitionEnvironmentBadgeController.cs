// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseDefinitionEnvironmentBadgeController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "badge", ResourceVersion = 1)]
  public class ReleaseDefinitionEnvironmentBadgeController : TfsApiController
  {
    [HttpGet]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("1a60a35d-b8c9-45fb-bf67-da0829711147")]
    [ClientResponseType(typeof (string), null, null)]
    public HttpResponseMessage GetDeploymentBadge(
      Guid projectId,
      int releaseDefinitionId,
      int environmentId,
      string branchName = null)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      ReleaseDefinitionsService service = vssRequestContext.GetService<ReleaseDefinitionsService>();
      ReleaseDefinition releaseDefinition;
      try
      {
        releaseDefinition = service.GetReleaseDefinition(vssRequestContext, projectId, releaseDefinitionId);
      }
      catch (DataspaceNotFoundException ex)
      {
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ProjectNotProvisioned, (object) projectId));
      }
      if (releaseDefinition == null)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.ReleaseDefinitionWithGivenIdNotFound);
      DefinitionEnvironment environment = releaseDefinition.GetEnvironment(environmentId);
      if (environment == null)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.DefinitionEnvironmentNotFound, (object) environmentId, (object) releaseDefinitionId));
      if (!environment.EnvironmentOptions.BadgeEnabled)
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.BadgeNotEnabledForEnvironment, (object) environmentId));
      XDocument badgeForEnvironment = vssRequestContext.GetService<ReleaseDefinitionEnvironmentsService>().GetBadgeForEnvironment(vssRequestContext, projectId, releaseDefinitionId, environmentId, branchName);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StringContent(badgeForEnvironment.ToString(), Encoding.UTF8, "image/svg+xml");
      return response;
    }
  }
}
