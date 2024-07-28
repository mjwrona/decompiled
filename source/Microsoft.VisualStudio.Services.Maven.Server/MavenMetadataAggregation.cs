// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenMetadataAggregation
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public abstract class MavenMetadataAggregation : 
    IAggregation<MavenMetadataAggregation, IAggregationAccessor<MavenMetadataAggregation>>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly RemovedAggregation ItemStoreV1_Removed = new RemovedAggregation(AggregationDefinitions.MavenPackageMetadataAggregationDefinition, "itemv1");
    public static readonly MavenMetadataAggregation BlobStoreV1 = (MavenMetadataAggregation) new MavenMetadataAggregation.BlobV1Implementation();

    private MavenMetadataAggregation(string versionName) => this.VersionName = versionName;

    protected abstract IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>> GetMetadataServiceFactoryBootstrapper(
      IVssRequestContext requestContext);

    public AggregationDefinition Definition { get; } = AggregationDefinitions.MavenPackageMetadataAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>> metadataServiceFactory = this.GetMetadataServiceFactoryBootstrapper(requestContext).Bootstrap();
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      return (IAggregationAccessor) new MavenMetadataAggregationAccessor((IAggregation) this, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, metadataServiceFactory, environmentFacade);
    }

    private class BlobV1Implementation : MavenMetadataAggregation
    {
      protected override IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>> GetMetadataServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>>) new MavenByBlobMetadataServiceFactoryBootstrapper(requestContext);
      }

      public BlobV1Implementation()
        : base("blobv1")
      {
      }
    }
  }
}
