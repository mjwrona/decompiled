// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.BlobPrototype.Aggregations.VersionListWithSize.MavenVersionListWithSizeAggregation
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.BlobPrototype.Aggregations.VersionListWithSize
{
  public class MavenVersionListWithSizeAggregation : 
    IAggregation<MavenVersionListWithSizeAggregation, IMavenVersionListWithSizeAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly MavenVersionListWithSizeAggregation V1 = new MavenVersionListWithSizeAggregation(nameof (V1));

    private MavenVersionListWithSizeAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = AggregationDefinitions.MavenVersionListWithSizeAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(requestContext, (IItemStoreBlobEncodingStrategy) new Base64ItemStoreBlobEncodingStrategy()).Bootstrap();
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      PackagingTracesFacade packagingTracesFacade = new PackagingTracesFacade(requestContext);
      IFeatureFlagService featureFlagFacade = requestContext.GetFeatureFlagFacade();
      IConverter<string, MavenPackageVersion> packageVersionConverter = ByFuncConverter.Create<string, MavenPackageVersion>((Func<string, MavenPackageVersion>) (displayVersion =>
      {
        ArgumentUtility.CheckStringForNullOrEmpty(displayVersion, nameof (displayVersion));
        return new MavenPackageVersion(displayVersion);
      }));
      IEqualityComparer<string> nameStringComparer = (IEqualityComparer<string>) MavenFileNameUtility.FileNameStringComparer;
      return (IAggregationAccessor) new MavenVersionListWithSizeAggregationAccessor((IAggregation) this, (IVersionListWithSizeFileProvider) new VersionListWithSizeFileProvider((IAggregation) this, blobServiceFactory, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, environmentFacade, (IPackagingTraces) packagingTracesFacade, tracerFacade, (IVersionCountsImplementationMetricsRecorder) new PackagingTracesVersionCountsWithSizeImplMetricsRecorder((IPackagingTraces) packagingTracesFacade), featureFlagFacade, (IConverter<string, IPackageName>) MavenIdentityResolver.Instance.NameResolver, (IConverter<string, IPackageVersion>) packageVersionConverter, nameStringComparer), tracerFacade, (IRetryCountProvider) new VersionListWithSizeRetryCountProvider(environmentFacade, (IRegistryService) requestContext.GetRegistryFacade(), (IProtocol) Protocol.Maven), featureFlagFacade, requestContext, (ICache<string, object>) new RequestContextItemsAsCacheFacade(requestContext));
    }
  }
}
