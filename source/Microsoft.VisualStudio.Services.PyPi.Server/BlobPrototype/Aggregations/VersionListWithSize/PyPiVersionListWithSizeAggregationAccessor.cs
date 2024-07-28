// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.BlobPrototype.Aggregations.VersionListWithSize.PyPiVersionListWithSizeAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;

namespace Microsoft.VisualStudio.Services.PyPi.Server.BlobPrototype.Aggregations.VersionListWithSize
{
  public class PyPiVersionListWithSizeAggregationAccessor : 
    VersionListWithSizeAggregationAccessor<PyPiPackageIdentity>,
    IPyPiVersionListWithSizeAggregationAccessor,
    IAggregationAccessor<PyPiVersionListWithSizeAggregation>,
    IAggregationAccessor,
    IPyPiVersionListWithSizeService,
    IVersionListWithSizeService
  {
    public PyPiVersionListWithSizeAggregationAccessor(
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
