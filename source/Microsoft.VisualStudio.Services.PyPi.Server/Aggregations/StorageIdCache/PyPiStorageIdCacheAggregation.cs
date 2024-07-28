// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.StorageIdCache.PyPiStorageIdCacheAggregation
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.PyPi.Server.Download;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.StorageIdCache
{
  public class PyPiStorageIdCacheAggregation : 
    IAggregation<PyPiStorageIdCacheAggregation, IPyPiStorageIdCacheAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly PyPiStorageIdCacheAggregation PyPiBlobMetadataStorageIdCache = new PyPiStorageIdCacheAggregation(nameof (PyPiBlobMetadataStorageIdCache));

    public PyPiStorageIdCacheAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = AggregationDefinitions.PyPiStorageIdCacheAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => (IAggregationAccessor) new PyPiStorageIdCacheAggregationAccessor(this, (IMetadataCacheService) new PackageMetadataCacheServiceFacade<IPyPiPackageMetadataCacheService>(requestContext));
  }
}
