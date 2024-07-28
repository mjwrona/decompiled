// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public static class DirectoryUtils
  {
    public static bool IsEntityId(string entityId) => DirectoryEntityIdentifier.TryParse(entityId, out DirectoryEntityIdentifier _);

    internal static bool IsOrganizationAadBacked(IVssRequestContext context) => context.IsOrganizationAadBacked();

    internal static bool TryGetOrganizationTenantId(IVssRequestContext context, out Guid tenantId)
    {
      tenantId = context.GetOrganizationAadTenantId();
      return tenantId != Guid.Empty;
    }

    internal static bool IsRequestByAadGuestUser(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return userIdentity != null && IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity) && AadIdentityHelper.IsAadUser((IReadOnlyVssIdentity) userIdentity) && userIdentity.MetaType == IdentityMetaType.Guest;
    }

    internal static bool IsVisualStudioDirectory(IDirectory directory) => VssStringComparer.DirectoryName.Equals("vsd", directory.Name);

    internal static bool IsNotVisualStudioDirectory(IDirectory directory) => !DirectoryUtils.IsVisualStudioDirectory(directory);

    internal static bool IsAzureActiveDirectory(IDirectory directory) => VssStringComparer.DirectoryName.Equals("aad", directory.Name);

    internal static bool IsNotAzureActiveDirectory(IDirectory directory) => !DirectoryUtils.IsAzureActiveDirectory(directory);

    internal static bool IsActiveDirectory(IDirectory directory) => VssStringComparer.DirectoryName.Equals("ad", directory.Name);

    internal static bool IsNotActiveDirectory(IDirectory directory) => !DirectoryUtils.IsActiveDirectory(directory);
  }
}
