// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata.NuGetPackageMetadataAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata
{
  public class NuGetPackageMetadataAggregationAccessor : 
    PackageMetadataAggregationAccessor<
    #nullable disable
    VssNuGetPackageIdentity, INuGetMetadataEntry>,
    INuGetPackageMetadataAggregationAccessor,
    IAggregationAccessor<NuGetPackageMetadataAggregation>,
    IAggregationAccessor,
    INuGetMetadataService,
    IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>,
    IMetadataService<
    #nullable enable
    VssNuGetPackageIdentity, INuGetMetadataEntry>,
    IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>,
    IReadSingleVersionMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>,
    IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>,
    IReadMetadataDocumentService
  {
    public NuGetPackageMetadataAggregationAccessor(

      #nullable disable
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>> metadataServiceFactory,
      IExecutionEnvironment executionEnvironment,
      IAggregation aggregation)
      : base(aggregation, aggVersionToLocatorConverter, metadataServiceFactory, executionEnvironment)
    {
    }
  }
}
