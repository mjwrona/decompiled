// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.PermissionsController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Security", ResourceName = "Permissions")]
  public class PermissionsController : PermissionsControllerBase
  {
    [HttpGet]
    [PublicCollectionRequestRestrictions(false, true, null)]
    [ClientResponseType(typeof (bool), null, null)]
    public PermissionsControllerBase.HasPermissionResult HasPermission(
      Guid securityNamespaceId,
      int permissions = 0,
      string token = "",
      bool alwaysAllowAdministrators = false)
    {
      token = token ?? string.Empty;
      return new PermissionsControllerBase.HasPermissionResult(this.HasPermission(securityNamespaceId, (IEnumerable<string>) new string[1]
      {
        token
      }, permissions, (alwaysAllowAdministrators ? 1 : 0) != 0).First<bool>());
    }
  }
}
