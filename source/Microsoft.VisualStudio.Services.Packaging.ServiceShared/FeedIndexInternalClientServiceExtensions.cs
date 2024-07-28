// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.FeedIndexInternalClientServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class FeedIndexInternalClientServiceExtensions
  {
    public static async Task UpdatePackageVersion(
      this IFeedIndexInternalClientService feedIndexService,
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      IPackageName packageName,
      IPackageVersion packageVersion,
      bool? isListed)
    {
      await feedIndexService.UpdatePackageVersionAsync(requestContext, feed, protocolType, packageName.NormalizedName, packageVersion.NormalizedVersion, isListed);
    }
  }
}
