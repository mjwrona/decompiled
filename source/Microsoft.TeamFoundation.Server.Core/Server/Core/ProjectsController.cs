// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectsController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core
{
  [VersionedApiControllerCustomName("core", "projects", 3)]
  public class ProjectsController : LegacyProjects2Controller
  {
    [HttpPatch]
    [ClientResponseType(typeof (OperationReference), null, null)]
    public override HttpResponseMessage UpdateProject(Guid projectId, TeamProject projectUpdate)
    {
      ProjectsUtility.ThrowIfProjectOperationsNotAllowed(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<TeamProject>(projectUpdate, "updatedProject", this.TfsRequestContext.ServiceName);
      int num = projectUpdate.Capabilities != null || projectUpdate.Links != null || projectUpdate.Url != null || projectUpdate.State != ProjectState.Unchanged ? 0 : (object.Equals((object) Guid.Empty, (object) projectUpdate.Id) ? 1 : (object.Equals((object) projectId, (object) projectUpdate.Id) ? 1 : 0));
      bool flag1 = projectUpdate.Name == null && projectUpdate.Abbreviation == null && projectUpdate.Description == null && projectUpdate.Visibility == ProjectVisibility.Unchanged;
      bool flag2 = projectUpdate.Capabilities == null && projectUpdate.Links == null && projectUpdate.Url == null && projectUpdate.State == ProjectState.WellFormed && (object.Equals((object) Guid.Empty, (object) projectUpdate.Id) || object.Equals((object) projectId, (object) projectUpdate.Id));
      if (num != 0 && !flag1)
        return this.Request.CreateResponse<OperationReference>(HttpStatusCode.Accepted, ProjectsUtility.UpdateTeamProject(this.TfsRequestContext, projectId, projectUpdate.Name, projectUpdate.Abbreviation, projectUpdate.Description, projectUpdate.Visibility));
      if (flag2)
        return this.Request.CreateResponse<OperationReference>(HttpStatusCode.Accepted, JobOperationsUtility.GetOperationReference(this.TfsRequestContext, this.TfsRequestContext.GetService<PlatformProjectService>().RecoverProject(this.TfsRequestContext, projectId, projectUpdate, out ProjectInfo _)));
      throw new ArgumentException(Resources.InvalidProjectUpdate(), nameof (projectUpdate)).Expected(this.TfsRequestContext.ServiceName);
    }
  }
}
