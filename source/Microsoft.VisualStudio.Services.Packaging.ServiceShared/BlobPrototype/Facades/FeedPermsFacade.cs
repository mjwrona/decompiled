// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.FeedPermsFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class FeedPermsFacade : IFeedPerms
  {
    private readonly Dictionary<FeedPermissionConstants, Action<FeedCore>> permToActionMap;
    private readonly Dictionary<FeedPermissionConstants, Func<FeedCore, bool>> permToFuncMap;

    public FeedPermsFacade(IVssRequestContext requestContext)
    {
      this.permToActionMap = new Dictionary<FeedPermissionConstants, Action<FeedCore>>()
      {
        {
          FeedPermissionConstants.AddPackage,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckAddPackagePermissions(requestContext, feed))
        },
        {
          FeedPermissionConstants.AddUpstreamPackage,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckAddUpstreamPackagePermissions(requestContext, feed))
        },
        {
          FeedPermissionConstants.AdministerFeed,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckAdministerFeedPermissions(requestContext, feed))
        },
        {
          FeedPermissionConstants.CreateFeed,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckCreateFeedPermissions(requestContext))
        },
        {
          FeedPermissionConstants.DeleteFeed,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckDeleteFeedPermissions(requestContext, feed))
        },
        {
          FeedPermissionConstants.DeletePackage,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckDeletePackagePermissions(requestContext, feed))
        },
        {
          FeedPermissionConstants.DelistPackage,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckDelistPackagePermissions(requestContext, feed))
        },
        {
          FeedPermissionConstants.EditFeed,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckEditFeedPermissions(requestContext, feed))
        },
        {
          FeedPermissionConstants.ReadPackages,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed))
        },
        {
          FeedPermissionConstants.UpdatePackage,
          (Action<FeedCore>) (feed => FeedSecurityHelper.CheckUpdatePackagePermissions(requestContext, feed))
        }
      };
      this.permToFuncMap = new Dictionary<FeedPermissionConstants, Func<FeedCore, bool>>()
      {
        {
          FeedPermissionConstants.AddPackage,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 64))
        },
        {
          FeedPermissionConstants.AddUpstreamPackage,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 2048))
        },
        {
          FeedPermissionConstants.AdministerFeed,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 1))
        },
        {
          FeedPermissionConstants.CreateFeed,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 8))
        },
        {
          FeedPermissionConstants.DeleteFeed,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 4))
        },
        {
          FeedPermissionConstants.DeletePackage,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 256))
        },
        {
          FeedPermissionConstants.DelistPackage,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 1024))
        },
        {
          FeedPermissionConstants.EditFeed,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 16))
        },
        {
          FeedPermissionConstants.ReadPackages,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 32))
        },
        {
          FeedPermissionConstants.UpdatePackage,
          (Func<FeedCore, bool>) (feed => FeedSecurityHelper.HasFeedPermissions(requestContext, feed, 128))
        }
      };
    }

    public bool HasPermissions(FeedCore feed, FeedPermissionConstants perm)
    {
      foreach (FeedPermissionConstants permissionConstants in Enum.GetValues(typeof (FeedPermissionConstants)))
      {
        Func<FeedCore, bool> func;
        if (perm.HasFlag((Enum) permissionConstants) && permissionConstants != FeedPermissionConstants.None && this.permToFuncMap.TryGetValue(permissionConstants, out func))
          return func(feed);
      }
      return false;
    }

    public void Validate(FeedCore feed, FeedPermissionConstants perm)
    {
      foreach (FeedPermissionConstants permissionConstants in Enum.GetValues(typeof (FeedPermissionConstants)))
      {
        Action<FeedCore> action;
        if (perm.HasFlag((Enum) permissionConstants) && permissionConstants != FeedPermissionConstants.None && this.permToActionMap.TryGetValue(permissionConstants, out action))
          action(feed);
      }
    }
  }
}
