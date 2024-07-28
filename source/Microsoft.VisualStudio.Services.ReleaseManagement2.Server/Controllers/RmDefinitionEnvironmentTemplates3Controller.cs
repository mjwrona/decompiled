// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmDefinitionEnvironmentTemplates3Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "environmenttemplates", ResourceVersion = 3)]
  public class RmDefinitionEnvironmentTemplates3Controller : 
    RmDefinitionEnvironmentTemplates2Controller
  {
    [HttpPatch]
    [ClientInternalUseOnly(false)]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.DeleteReleaseDefinition)]
    public ReleaseDefinitionEnvironmentTemplate UndeleteReleaseDefinitionEnvironmentTemplate(
      Guid templateId)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmDefinitionsController.UndeleteReleaseDefinitionEnvironmentTemplate", 1961230, 5, true))
        return this.GetService<DefinitionEnvironmentTemplatesService>().UndeleteDefinitionEnvironmentTemplate(this.TfsRequestContext, this.ProjectId, templateId);
    }
  }
}
