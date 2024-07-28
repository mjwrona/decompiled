// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata.CargoPackageMetadataAggregation
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata
{
  [ExcludeFromCodeCoverage]
  public class CargoPackageMetadataAggregation : 
    IAggregation<CargoPackageMetadataAggregation, IAggregationAccessor<CargoPackageMetadataAggregation>>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly CargoPackageMetadataAggregation V1 = new CargoPackageMetadataAggregation(nameof (V1));

    public CargoPackageMetadataAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition => AggregationDefinitions.CargoPackageMetadataAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFactory<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>> metadataServiceFactory = this.GetMetadataServiceFactoryBootstrapper(requestContext).Bootstrap();
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      return (IAggregationAccessor) new CargoPackageMetadataAggregationAccessor((IAggregation) this, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, metadataServiceFactory, environmentFacade);
    }

    private IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>> GetMetadataServiceFactoryBootstrapper(
      IVssRequestContext requestContext)
    {
      return (IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>>) new CargoByBlobMetadataServiceFactoryBootstrapper(requestContext);
    }
  }
}
