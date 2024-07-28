// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata.NuGetPackageMetadataAggregation
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata
{
  public class NuGetPackageMetadataAggregation : 
    IAggregation<NuGetPackageMetadataAggregation, INuGetPackageMetadataAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly NuGetPackageMetadataAggregation V1 = new NuGetPackageMetadataAggregation(nameof (V1));
    public static readonly NuGetPackageMetadataAggregation V2 = new NuGetPackageMetadataAggregation(nameof (V2));
    public static readonly NuGetPackageMetadataAggregation V3 = new NuGetPackageMetadataAggregation(nameof (V3));

    private IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>> GetMetadataServiceFactoryBootstrapper(
      IVssRequestContext requestContext)
    {
      return this == NuGetPackageMetadataAggregation.V1 || this == NuGetPackageMetadataAggregation.V3 ? (IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>>) new BasicNuGetMetadataByBlobServiceBootstrapper(requestContext) : (IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>>) new BasicNuGetMetadataByBlobServiceDifferentPathBootstrapper(requestContext);
    }

    private NuGetPackageMetadataAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = NuGetAggregationDefinitions.NuGetPackageMetadataAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>> factory = this.GetMetadataServiceFactoryBootstrapper(requestContext).Bootstrap();
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      IFactory<ContainerAddress, IMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>> metadataServiceFactory = factory;
      IExecutionEnvironment executionEnvironment = environmentFacade;
      return (IAggregationAccessor) new NuGetPackageMetadataAggregationAccessor((IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, metadataServiceFactory, executionEnvironment, (IAggregation) this);
    }
  }
}
