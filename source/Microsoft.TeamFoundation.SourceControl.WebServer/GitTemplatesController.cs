// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitTemplatesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "templates")]
  [ClientInclude(RestClientLanguages.TypeScript | RestClientLanguages.TypeScriptWebPlatform)]
  public class GitTemplatesController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("F88D498E-52C3-422A-A5F2-994F4265A25B")]
    [ClientResponseType(typeof (IList<GitTemplate>), null, null)]
    public HttpResponseMessage GetTemplateList([ClientIgnore] string projectId = null, string type = null) => this.Request.CreateResponse<IList<GitTemplate>>(HttpStatusCode.OK, new GitTemplatesProvider().GetTemplatesOfType(type));
  }
}
