// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.LegacyProjectsController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core
{
  [VersionedApiControllerCustomName("core", "projects", 1)]
  public class LegacyProjectsController : ServerCoreApiController
  {
    [HttpGet]
    public WebApiProject GetProject(string projectId, bool? includeCapabilities = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId));
      return ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, ((int) includeCapabilities ?? 1) != 0).ToWebApiProject(this.TfsRequestContext);
    }

    [HttpGet]
    public IEnumerable<WebApiProject> GetProjects(
      ProjectState? stateFilter = null,
      bool? includeCapabilities = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null)
    {
      int topValue;
      int skipValue;
      ServerCoreApiController.EvaluateTopSkip(top, skip, out topValue, out skipValue);
      return ProjectsUtility.GetWebApiProjects(this.TfsRequestContext, (ProjectState) ((int) stateFilter ?? 1), includeCapabilities.GetValueOrDefault(), topValue, skipValue);
    }
  }
}
