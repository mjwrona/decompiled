// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Project.ProjectAvatarController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Project
{
  [VersionedApiControllerCustomName("core", "avatar", 1)]
  [ControllerApiVersion(5.1)]
  public class ProjectAvatarController : ServerCoreApiController
  {
    [HttpPut]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage SetProjectAvatar(string projectId, ProjectAvatar avatarBlob)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId), this.TfsRequestContext.ServiceName);
      Guid result;
      if (!Guid.TryParse(projectId, out result))
        result = this.TfsRequestContext.GetService<IProjectService>().GetProjectId(this.TfsRequestContext, projectId);
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      service.SetTeamImage(this.TfsRequestContext, service.GetDefaultTeamId(this.TfsRequestContext, result), avatarBlob.Image);
      return this.Request.CreateResponse<ProjectAvatar>(HttpStatusCode.NoContent, avatarBlob);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage RemoveProjectAvatar(string projectId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId), this.TfsRequestContext.ServiceName);
      Guid result;
      if (!Guid.TryParse(projectId, out result))
        result = this.TfsRequestContext.GetService<IProjectService>().GetProjectId(this.TfsRequestContext, projectId);
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      service.RemoveTeamImage(this.TfsRequestContext, service.GetDefaultTeamId(this.TfsRequestContext, result));
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
