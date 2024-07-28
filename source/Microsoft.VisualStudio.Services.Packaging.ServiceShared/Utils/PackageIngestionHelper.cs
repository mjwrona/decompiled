// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.PackageIngestionHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class PackageIngestionHelper
  {
    public const int DefaultMaxVersionsPerPackageName = 5000;
    public static readonly RegistryFrotocolLevelPackagingSettingDefinition<int> MaxVersionsPerPackageSettingDefinition = new RegistryFrotocolLevelPackagingSettingDefinition<int>((Func<IProtocol, RegistryQuery>) (protocol => (RegistryQuery) ("/Configuration/" + protocol.CorrectlyCasedName + "/MaxVersionsPerPackageName")), (Func<IProtocol, FeedIdentity, RegistryQuery>) ((protocol, feed) => (RegistryQuery) string.Format("/Configuration/{0}/MaxVersionsPerPackageName/{1}", (object) protocol.CorrectlyCasedName, (object) feed.Id)), 5000);

    public static async Task<bool> HasTooManyVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      IProtocol protocol,
      string packageName)
    {
      try
      {
        int forFeedAndProtocol = PackageIngestionHelper.GetMaxVersionsForFeedAndProtocol(requestContext, feed, protocol);
        if (forFeedAndProtocol <= 0)
          return false;
        IVssRequestContext context = requestContext.Elevate();
        IFeedIndexClientService service = context.GetService<IFeedIndexClientService>();
        IVssRequestContext requestContext1 = context;
        FeedCore feed1 = feed;
        string correctlyCasedName = protocol.CorrectlyCasedName;
        string normalizedPackageName = packageName;
        int? nullable = new int?(forFeedAndProtocol - 1);
        int? top = new int?(1);
        int? skip = nullable;
        bool? isListed = new bool?();
        bool? isRelease = new bool?();
        bool? isCached = new bool?();
        return (await service.GetPackagesAsync(requestContext1, feed1, correctlyCasedName, normalizedPackageName: normalizedPackageName, top: top, skip: skip, includeAllVersions: true, includeUrls: false, isListed: isListed, isRelease: isRelease, getTopPackageVersions: true, isCached: isCached)).Any<Package>();
      }
      catch (PackageNotFoundException ex)
      {
        return false;
      }
    }

    public static int GetMaxVersionsForFeedAndProtocol(
      IVssRequestContext requestContext,
      FeedCore feed,
      IProtocol protocol)
    {
      return PackageIngestionHelper.MaxVersionsPerPackageSettingDefinition.Bootstrap(requestContext).Get((IFeedRequest) new FeedRequest(feed, protocol));
    }
  }
}
