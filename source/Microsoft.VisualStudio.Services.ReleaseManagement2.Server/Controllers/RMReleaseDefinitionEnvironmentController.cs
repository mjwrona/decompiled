// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RMReleaseDefinitionEnvironmentController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "definitionEnvironments")]
  public class RMReleaseDefinitionEnvironmentController : ReleaseManagementProjectControllerBase
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Defaults are required.")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.ViewReleaseDefinition)]
    public IEnumerable<DefinitionEnvironmentReference> GetDefinitionEnvironments(
      Guid? taskGroupId = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null)
    {
      return this.TfsRequestContext.GetService<ReleaseDefinitionEnvironmentsService>().GetReleaseDefinitionEnvironments(this.TfsRequestContext, this.ProjectId, taskGroupId, propertyFilters);
    }
  }
}
