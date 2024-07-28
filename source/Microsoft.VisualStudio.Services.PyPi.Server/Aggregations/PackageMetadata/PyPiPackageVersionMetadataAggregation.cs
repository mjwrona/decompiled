// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.PackageMetadata.PyPiPackageVersionMetadataAggregation
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.PackageMetadata
{
  public class PyPiPackageVersionMetadataAggregation : 
    IAggregation<PyPiPackageVersionMetadataAggregation, IAggregationAccessor<PyPiPackageVersionMetadataAggregation>>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly PyPiPackageVersionMetadataAggregation V1 = new PyPiPackageVersionMetadataAggregation(nameof (V1));
    public static readonly PyPiPackageVersionMetadataAggregation V2 = new PyPiPackageVersionMetadataAggregation(nameof (V2));

    public PyPiPackageVersionMetadataAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = AggregationDefinitions.PyPiPackageVersionMetadataAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>> metadataServiceFactory = this.GetMetadataServiceFactoryBootstrapper(requestContext).Bootstrap();
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      return (IAggregationAccessor) new PyPiVersionMetadataAggregationAccessor((IAggregation) this, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, metadataServiceFactory, environmentFacade);
    }

    private IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>>> GetMetadataServiceFactoryBootstrapper(
      IVssRequestContext requestContext)
    {
      return (IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>>>) new PyPiByBlobVersionMetadataServiceFactoryBootstrapper(requestContext);
    }
  }
}
