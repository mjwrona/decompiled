// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamPackageUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public static class UpstreamPackageUtils
  {
    public static int GetUnitsOfWork(
      IRegistryService registryService,
      int numPackagesToRefresh,
      FeedCore feed,
      IProtocol protocol)
    {
      return Convert.ToInt32(feed.GetSourcesForProtocol(protocol).Aggregate<UpstreamSource, double>(0.0, (Func<double, UpstreamSource, double>) ((lastVal, source) => lastVal + UpstreamPackageUtils.GetUnitsOfWorkMultiplier(registryService, source) * (double) numPackagesToRefresh)));
    }

    public static double GetUnitsOfWorkMultiplier(
      IRegistryService registryService,
      UpstreamSource source)
    {
      return source.UpstreamSourceType == UpstreamSourceType.Internal || !source.IsWellKnownSource(WellKnownSources.Npmjs) ? 1.0 : registryService.GetValue<double>((RegistryQuery) "/Configuration/Packaging/npm/UpstreamMetadataCache/UnitOfWorkMultiplier/npmjs", 1.0);
    }
  }
}
