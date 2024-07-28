// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype.Aggregations.VersionListWithSize.NpmVersionListWithSizeAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;

namespace Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype.Aggregations.VersionListWithSize
{
  public class NpmVersionListWithSizeAggregationAccessor : 
    VersionListWithSizeAggregationAccessor<NpmPackageIdentity>,
    INpmVersionListWithSizeAggregationAccessor,
    IAggregationAccessor<NpmVersionListWithSizeAggregation>,
    IAggregationAccessor,
    INpmVersionListWithSizeService,
    IVersionListWithSizeService
  {
    public NpmVersionListWithSizeAggregationAccessor(
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
