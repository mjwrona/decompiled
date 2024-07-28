// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.Permissions2Controller
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
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Security", ResourceName = "Permissions", ResourceVersion = 2)]
  public class Permissions2Controller : PermissionsControllerBase
  {
    [HttpGet]
    [PublicCollectionRequestRestrictions(false, true, null)]
    [ClientResponseType(typeof (IQueryable<bool>), null, null)]
    [ClientExample("GET__permissions__securityNamespaceId__8__token-_token1__alwaysAllowAdministrators-False.json", "Singular token", null, null)]
    [ClientExample("GET__permissions__securityNamespaceId__8__api-version-2.2_tokens-_token1_,_token2_,_token3__alwaysAllowAdministrators-False.json", "A list of tokens", null, null)]
    public IQueryable<PermissionsControllerBase.HasPermissionResult> HasPermissions(
      Guid securityNamespaceId,
      int permissions = 0,
      string tokens = "",
      bool alwaysAllowAdministrators = false,
      string delimiter = ",")
    {
      tokens = tokens ?? string.Empty;
      string[] tokens1;
      if (delimiter != null && delimiter.Length > 0 && delimiter[0] != char.MinValue)
        tokens1 = tokens.Split(new char[1]{ delimiter[0] }, StringSplitOptions.None);
      else
        tokens1 = new string[1]{ tokens };
      return this.HasPermission(securityNamespaceId, (IEnumerable<string>) tokens1, permissions, alwaysAllowAdministrators).Select<bool, PermissionsControllerBase.HasPermissionResult>((Func<bool, PermissionsControllerBase.HasPermissionResult>) (s => new PermissionsControllerBase.HasPermissionResult(s))).AsQueryable<PermissionsControllerBase.HasPermissionResult>();
    }
  }
}
