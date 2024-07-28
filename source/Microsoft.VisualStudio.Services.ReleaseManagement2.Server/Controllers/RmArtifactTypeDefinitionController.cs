// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmArtifactTypeDefinitionController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "types")]
  public class RmArtifactTypeDefinitionController : ReleaseManagementProjectControllerBase
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is a REST API.")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public IEnumerable<ArtifactTypeDefinition> GetArtifactTypeDefinitions() => (IEnumerable<ArtifactTypeDefinition>) this.GetService<ArtifactTypeDefinitionService>().GetArtifactTypeDefinitions(this.TfsRequestContext, this.ProjectId).ToList<ArtifactTypeDefinition>();
  }
}
