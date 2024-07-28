// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype.Aggregations.VersionListWithSize.NpmVersionListWithSizeAggregation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype.Aggregations.VersionListWithSize
{
  public class NpmVersionListWithSizeAggregation : 
    IAggregation<NpmVersionListWithSizeAggregation, INpmVersionListWithSizeAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly NpmVersionListWithSizeAggregation V1 = new NpmVersionListWithSizeAggregation(nameof (V1));

    private NpmVersionListWithSizeAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = NpmAggregationDefinitions.NpmVersionListWithSizeAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(requestContext, (IItemStoreBlobEncodingStrategy) new Base64ItemStoreBlobEncodingStrategy()).Bootstrap();
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      PackagingTracesFacade packagingTracesFacade = new PackagingTracesFacade(requestContext);
      IFeatureFlagService featureFlagFacade = requestContext.GetFeatureFlagFacade();
      IConverter<string, SemanticVersion> packageVersionConverter = ByFuncConverter.Create<string, SemanticVersion>((Func<string, SemanticVersion>) (displayVersion =>
      {
        ArgumentUtility.CheckStringForNullOrEmpty(displayVersion, nameof (displayVersion));
        SemanticVersion parsedVersion;
        if (!SemanticVersion.TryParse(displayVersion, out parsedVersion))
          throw new InvalidPackageException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageVersion((object) displayVersion));
        return parsedVersion;
      }));
      IEqualityComparer<string> ordinal = (IEqualityComparer<string>) StringComparer.Ordinal;
      return (IAggregationAccessor) new NpmVersionListWithSizeAggregationAccessor((IAggregation) this, (IVersionListWithSizeFileProvider) new VersionListWithSizeFileProvider((IAggregation) this, blobServiceFactory, (IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, environmentFacade, (IPackagingTraces) packagingTracesFacade, tracerFacade, (IVersionCountsImplementationMetricsRecorder) new PackagingTracesVersionCountsWithSizeImplMetricsRecorder((IPackagingTraces) packagingTracesFacade), featureFlagFacade, (IConverter<string, IPackageName>) NpmIdentityResolver.Instance.NameResolver, (IConverter<string, IPackageVersion>) packageVersionConverter, ordinal), tracerFacade, (IRetryCountProvider) new VersionListWithSizeRetryCountProvider(environmentFacade, (IRegistryService) requestContext.GetRegistryFacade(), (IProtocol) Protocol.npm), featureFlagFacade, requestContext, (ICache<string, object>) new RequestContextItemsAsCacheFacade(requestContext));
    }
  }
}
