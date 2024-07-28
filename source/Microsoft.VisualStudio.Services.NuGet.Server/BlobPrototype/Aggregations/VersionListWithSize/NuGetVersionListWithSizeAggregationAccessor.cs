// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.VersionListWithSize.NuGetVersionListWithSizeAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.VersionListWithSize
{
  public class NuGetVersionListWithSizeAggregationAccessor : 
    VersionListWithSizeAggregationAccessor<VssNuGetPackageIdentity>,
    INuGetVersionListWithSizeAggregationAccessor,
    IAggregationAccessor<NuGetVersionListWithSizeAggregation>,
    IAggregationAccessor,
    INuGetVersionListWithSizeService,
    IVersionListWithSizeService
  {
    public NuGetVersionListWithSizeAggregationAccessor(
      IAggregation aggregation,
      IVersionListWithSizeFileProvider versionListWithSizeFileProvider,
      ITracerService tracer,
      IRetryCountProvider retryCountProvider,
      IFeatureFlagService featureFlagService,
      IVssRequestContext requestContext,
      ICache<string, object> requestContextItems)
      : base(aggregation, versionListWithSizeFileProvider, tracer, retryCountProvider, featureFlagService, requestContext, requestContextItems)
    {
    }
  }
}
