// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata.NpmPackageMetadataAggregation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata
{
  public abstract class NpmPackageMetadataAggregation : 
    IAggregation<NpmPackageMetadataAggregation, IAggregationAccessor<NpmPackageMetadataAggregation>>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly NpmPackageMetadataAggregation V1 = (NpmPackageMetadataAggregation) new NpmPackageMetadataAggregation.BlobV1Implementation();

    private NpmPackageMetadataAggregation(string versionName) => this.VersionName = versionName;

    protected abstract IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>> GetMetadataServiceFactoryBootstrapper(
      IVssRequestContext requestContext);

    public AggregationDefinition Definition { get; } = NpmAggregationDefinitions.NpmPackageMetadataAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>> metadataServiceFactory = this.GetMetadataServiceFactoryBootstrapper(requestContext).Bootstrap();
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      return (IAggregationAccessor) new NpmPackageMetadataAggregationAccessor((IAggregation) this, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, metadataServiceFactory, environmentFacade);
    }

    private class BlobV1Implementation : NpmPackageMetadataAggregation
    {
      protected override IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>> GetMetadataServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>>) new NpmByBlobMetadataServiceFactoryBootstrapper(requestContext);
      }

      public BlobV1Implementation()
        : base("v1")
      {
      }
    }
  }
}
