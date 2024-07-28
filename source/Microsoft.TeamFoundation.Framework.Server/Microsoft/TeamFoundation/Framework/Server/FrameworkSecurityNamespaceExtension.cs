// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FrameworkSecurityNamespaceExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class FrameworkSecurityNamespaceExtension : DefaultSecurityNamespaceExtension
  {
    public override void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string token,
      int requestedPermissions)
    {
      throw new AccessCheckException(identity.Descriptor, identity.DisplayName, token, requestedPermissions, FrameworkSecurity.FrameworkNamespaceId, TFCommonResources.AccessCheckExceptionPrivilegeFormat((object) identity.Id.ToString(), (object) string.Join(", ", securityNamespace.Description.GetLocalizedActions(requestedPermissions))));
    }
  }
}
