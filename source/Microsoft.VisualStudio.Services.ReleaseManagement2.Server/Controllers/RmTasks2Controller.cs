// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmTasks2Controller
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "tasks", ResourceVersion = 2)]
  public class RmTasks2Controller : RmTasksController
  {
    [HttpGet]
    [ClientLocationId("4259191D-4B0A-4409-9FB3-09F22AB9BC47")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [PublicProjectRequestRestrictions]
    public IEnumerable<ReleaseTask> GetTasksForTaskGroup(
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId)
    {
      IEnumerable<ReleaseTask> releaseTasks = this.GetReleaseTasks(releaseId, environmentId, releaseDeployPhaseId);
      this.TfsRequestContext.SetSecuredObjects<ReleaseTask>(releaseTasks);
      return releaseTasks;
    }

    [HttpGet]
    [ClientLocationId("4259291D-4B0A-4409-9FB3-04F22AB9BC47")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [PublicProjectRequestRestrictions]
    [ClientInternalUseOnly(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<ReleaseTask> GetTasks2(
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId)
    {
      IEnumerable<ReleaseTask> releaseTasks = this.GetReleaseTasks(releaseId, environmentId, attemptId, timelineId);
      this.TfsRequestContext.SetSecuredObjects<ReleaseTask>(releaseTasks);
      return releaseTasks;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "attemptId is not getting used presently. It is used here because it is part of API inputs and might be used in future.")]
    protected IEnumerable<ReleaseTask> GetReleaseTasks(
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId)
    {
      return this.GetService<ReleaseTasksService>().GetTasks(this.TfsRequestContext, this.ProjectId, releaseId, environmentId, timelineId);
    }
  }
}
