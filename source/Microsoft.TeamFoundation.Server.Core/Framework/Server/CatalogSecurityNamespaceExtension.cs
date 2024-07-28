// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CatalogSecurityNamespaceExtension
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Security;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class CatalogSecurityNamespaceExtension : DefaultSecurityNamespaceExtension
  {
    public override void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string token,
      int requestedPermissions)
    {
      if (requestedPermissions == 2)
        throw new AccessCheckException(identity.Descriptor, identity.DisplayName, token, requestedPermissions, FrameworkSecurity.CatalogNamespaceId, TFCommonResources.AccessCheckExceptionPrivilegeFormat((object) (identity?.Id.ToString() ?? FrameworkResources.AnonymousPrincipalName()), (object) securityNamespace.Description.GetLocalizedActions(2).FirstOrDefault<string>()));
      if (!securityNamespace.HasPermission(requestContext, token, 1))
        throw new CatalogNodeDoesNotExistException();
      throw new AccessCheckException(identity.Descriptor, identity.DisplayName, token, requestedPermissions, FrameworkSecurity.CatalogNamespaceId, TFCommonResources.AccessCheckExceptionPrivilegeFormat((object) (identity?.Id.ToString() ?? FrameworkResources.AnonymousPrincipalName()), (object) string.Join(", ", securityNamespace.Description.GetLocalizedActions(requestedPermissions))));
    }
  }
}
