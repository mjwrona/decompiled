// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList.UpstreamVersionListAggregation`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList
{
  public abstract class UpstreamVersionListAggregation<TPackageName, TPackageVersion> : 
    IAggregation<UpstreamVersionListAggregation<TPackageName, TPackageVersion>, UpstreamVersionListAggregationAccessor<TPackageName, TPackageVersion>>,
    IAggregation,
    IAggregationAccessorBootstrapper
    where TPackageName : IPackageName
    where TPackageVersion : class, IPackageVersion
  {
    public static AggregationDefinition MakeDefinition(IProtocol protocol) => new AggregationDefinition()
    {
      Name = "UpstreamVersions",
      Protocol = protocol,
      DependsOn = Enumerable.Empty<AggregationDefinition>()
    };

    public abstract IAggregationAccessor Bootstrap(IVssRequestContext requestContext);

    public abstract AggregationDefinition Definition { get; }

    protected IAggregationAccessor BootstrapCore(
      IVssRequestContext requestContext,
      IConverter<string, TPackageVersion> versionParser)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return (IAggregationAccessor) new UpstreamVersionListAggregationAccessor<TPackageName, TPackageVersion>(this, (IAggregationDocumentProvider<UpstreamVersionListFile<TPackageVersion>, TPackageName>) new AzureBlobAggregationDocumentProvider<UpstreamVersionListFile<TPackageVersion>, TPackageName>(requestContext.GetPackagingTracesFacade(), requestContext.GetTracerFacade(), (IAggregationDocumentProcessor<UpstreamVersionListFile<TPackageVersion>>) new UpstreamVersionListDocumentProcessor<TPackageVersion>(versionParser), (IFactory<IBlobService>) new BlobServiceForAggregationFactory(BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(requestContext).Bootstrap(), (IAggregation) this, (IConverter<IAggregation, Locator>) new AggregationToLocatorConverter(), requestContext.GetExecutionEnvironmentFacade()), FeatureAvailabilityConstants.CompressUpstreamVersionListFeature.Bootstrap(requestContext), "LastUpstreamVersionListBlobSize", (IAggregationDocumentLocatorProvider<TPackageName>) new PackageLevelDocumentLocatorProvider<TPackageName>((Func<string, string>) (normalizedName => normalizedName + ".json"))), requestContext.GetFeatureFlagFacade(), (IETaggedDocumentUpdater) new ETaggedDocumentUpdater());
    }

    public string VersionName { get; } = "V1";
  }
}
