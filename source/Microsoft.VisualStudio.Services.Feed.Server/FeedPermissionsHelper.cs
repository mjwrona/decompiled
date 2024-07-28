// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedPermissionsHelper
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public static class FeedPermissionsHelper
  {
    public static IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesFromAces(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> accessControlEntries)
    {
      List<IdentityDescriptor> list = accessControlEntries.Select<Microsoft.VisualStudio.Services.Security.AccessControlEntry, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Security.AccessControlEntry, IdentityDescriptor>) (x => x.Descriptor)).ToList<IdentityDescriptor>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null);
      if (source == null || source.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => id == null)))
        throw new IdentityNullException();
      return (IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) source.ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
    }

    public static FeedPermission ConvertAceToFeedPermission(Microsoft.VisualStudio.Services.Security.AccessControlEntry accessControlEntry)
    {
      bool flag = accessControlEntry.ExtendedInfo != null && accessControlEntry.ExtendedInfo.InheritedAllow != 0;
      return new FeedPermission()
      {
        Role = FeedPermissionsHelper.GetRoleForAce(accessControlEntry),
        IsInheritedRole = flag,
        IdentityDescriptor = accessControlEntry.Descriptor
      };
    }

    public static FeedPermission ConvertPermissionsToFeedPermission(
      int permissions,
      IdentityDescriptor identityDescriptor)
    {
      return new FeedPermission()
      {
        Role = FeedPermissionsHelper.GetRoleForPermissions(permissions),
        IdentityDescriptor = identityDescriptor
      };
    }

    private static FeedRole GetRoleForAce(Microsoft.VisualStudio.Services.Security.AccessControlEntry ace) => ace.Deny != 0 ? FeedRole.Custom : FeedPermissionsHelper.GetRoleForPermissions(ace.ExtendedInfo == null || ace.ExtendedInfo.EffectiveAllow == 0 ? ace.Allow : ace.ExtendedInfo.EffectiveAllow);

    private static FeedRole GetRoleForPermissions(int permissions)
    {
      switch ((FeedPermissionConstants) (permissions & 3575))
      {
        case FeedPermissionConstants.None:
          return FeedRole.None;
        case FeedPermissionConstants.ReadPackages:
          return FeedRole.Reader;
        case FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddUpstreamPackage:
          return FeedRole.Collaborator;
        case FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddPackage | FeedPermissionConstants.UpdatePackage | FeedPermissionConstants.DelistPackage | FeedPermissionConstants.AddUpstreamPackage:
          return FeedRole.Contributor;
        case FeedPermissionConstants.AdministerFeed | FeedPermissionConstants.ArchiveFeed | FeedPermissionConstants.DeleteFeed | FeedPermissionConstants.EditFeed | FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddPackage | FeedPermissionConstants.UpdatePackage | FeedPermissionConstants.DeletePackage | FeedPermissionConstants.DelistPackage | FeedPermissionConstants.AddUpstreamPackage:
          return FeedRole.Administrator;
        default:
          return FeedRole.Custom;
      }
    }
  }
}
