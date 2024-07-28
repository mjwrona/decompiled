// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.SecurityRoleDefintionsController
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "securityroles", ResourceName = "roledefinitions")]
  public class SecurityRoleDefintionsController : TfsApiController
  {
    [HttpGet]
    [ClientLocationId("F4CC9A86-453C-48D2-B44D-D3BD5C105F4F")]
    public List<SecurityRole> GetRoleDefinitions(string scopeId) => this.TfsRequestContext.GetService<ISecurityRoleMappingService>().GetRoles(this.TfsRequestContext, scopeId);
  }
}
