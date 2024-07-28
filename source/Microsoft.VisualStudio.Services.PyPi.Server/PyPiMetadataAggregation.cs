// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PyPiMetadataAggregation
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

namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public class PyPiMetadataAggregation : 
    IAggregation<PyPiMetadataAggregation, IAggregationAccessor<PyPiMetadataAggregation>>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly PyPiMetadataAggregation V1 = new PyPiMetadataAggregation();

    private IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>>> GetMetadataServiceFactoryBootstrapper(
      IVssRequestContext requestContext)
    {
      return (IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>>>) new PyPiByBlobMetadataServiceFactoryBootstrapper(requestContext);
    }

    public AggregationDefinition Definition { get; } = AggregationDefinitions.PyPiPackageMetadataAggregationDefinition;

    public string VersionName { get; } = nameof (V1);

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>> metadataServiceFactory = this.GetMetadataServiceFactoryBootstrapper(requestContext).Bootstrap();
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      return (IAggregationAccessor) new PyPiMetadataAggregationAccessor((IAggregation) this, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, metadataServiceFactory, environmentFacade);
    }
  }
}
