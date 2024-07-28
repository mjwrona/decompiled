// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseDefinitionHistory2Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(7.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "revisions", ResourceVersion = 4)]
  [ClientGroupByResource("definitions")]
  public class ReleaseDefinitionHistory2Controller : ReleaseDefinitionHistoryController
  {
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is disposed elsewhere")]
    public static HttpResponseMessage GetReleaseDefinitionRevision(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      HttpRequestMessage httpRequest,
      ReleaseDefinitionHistoryService releaseDefinitionHistoryService,
      int definitionId,
      int revision)
    {
      if (releaseDefinitionHistoryService == null)
        throw new ArgumentNullException(nameof (releaseDefinitionHistoryService));
      Stream revision1 = releaseDefinitionHistoryService.GetRevision(tfsRequestContext, projectId, definitionId, revision);
      releaseDefinitionHistoryService.CheckReleaseDefinitionRevisionStream(tfsRequestContext, definitionId, revision, revision1);
      HttpResponseMessage response = httpRequest.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(revision1);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
      return response;
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleaseDefinition, true)]
    public override HttpResponseMessage GetDefinitionRevision(int definitionId, int revision) => ReleaseDefinitionHistory2Controller.GetReleaseDefinitionRevision(this.TfsRequestContext, this.ProjectId, this.Request, this.GetService<ReleaseDefinitionHistoryService>(), definitionId, revision);
  }
}
