// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenSecurityHelper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public class MavenSecurityHelper
  {
    public static void CheckForReadAndAddPackagePermissions(
      IVssRequestContext vssRequestContext,
      FeedCore feed)
    {
      MavenSecurityHelper.CheckFeedPermissions(vssRequestContext, feed, FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddPackage);
    }

    public static void CheckForReadAndUpdatePackagePermissions(
      IVssRequestContext vssRequestContext,
      FeedCore feed)
    {
      MavenSecurityHelper.CheckFeedPermissions(vssRequestContext, feed, FeedPermissionConstants.ReadPackages | FeedPermissionConstants.UpdatePackage);
    }

    public static void CheckForReadFeedPermission(
      IVssRequestContext vssRequestContext,
      FeedCore feed)
    {
      MavenSecurityHelper.CheckFeedPermissions(vssRequestContext, feed, FeedPermissionConstants.ReadPackages);
    }

    private static void CheckFeedPermissions(
      IVssRequestContext vssRequestContext,
      FeedCore feed,
      FeedPermissionConstants feedPermissions)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(vssRequestContext, nameof (vssRequestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      if (feedPermissions.HasFlag((Enum) (FeedPermissionConstants.AdministerFeed | FeedPermissionConstants.AddPackage | FeedPermissionConstants.UpdatePackage | FeedPermissionConstants.DeletePackage)) && vssRequestContext.IsFeatureEnabledWithLogging("Packaging.Maven.ReadOnly"))
        throw new FeatureReadOnlyException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_MavenServiceReadonly());
      if (feedPermissions.HasFlag((Enum) FeedPermissionConstants.ReadPackages))
        FeedSecurityHelper.CheckReadFeedPermissions(vssRequestContext, feed);
      if (feedPermissions.HasFlag((Enum) FeedPermissionConstants.AddPackage))
        FeedSecurityHelper.CheckAddPackagePermissions(vssRequestContext, feed);
      if (feedPermissions.HasFlag((Enum) FeedPermissionConstants.UpdatePackage))
        FeedSecurityHelper.CheckUpdatePackagePermissions(vssRequestContext, feed);
      if (feedPermissions.HasFlag((Enum) FeedPermissionConstants.DeletePackage))
        FeedSecurityHelper.CheckDeletePackagePermissions(vssRequestContext, feed);
      if (!feedPermissions.HasFlag((Enum) FeedPermissionConstants.AdministerFeed))
        return;
      FeedSecurityHelper.CheckAddPackagePermissions(vssRequestContext, feed);
    }
  }
}
