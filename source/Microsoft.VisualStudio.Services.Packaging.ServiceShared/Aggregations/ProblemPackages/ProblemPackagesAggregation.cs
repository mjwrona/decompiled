// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages.ProblemPackagesAggregation
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

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages
{
  public abstract class ProblemPackagesAggregation : 
    IAggregation<ProblemPackagesAggregation, ProblemPackagesAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    private readonly Func<string, IPackageName> nameParser;
    private readonly Func<string, IPackageVersion> versionParser;

    public static AggregationDefinition MakeDefinition(IProtocol protocol) => new AggregationDefinition()
    {
      Name = "ProblemPackages",
      Protocol = protocol,
      DependsOn = Enumerable.Empty<AggregationDefinition>()
    };

    protected ProblemPackagesAggregation(
      AggregationDefinition definition,
      Func<string, IPackageName> nameParser,
      Func<string, IPackageVersion> versionParser)
    {
      this.Definition = definition;
      this.nameParser = nameParser;
      this.versionParser = versionParser;
    }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return (IAggregationAccessor) new ProblemPackagesAggregationAccessor((IAggregation) this, (IETaggedDocumentUpdater) new ETaggedDocumentUpdater(), (IAggregationDocumentProvider<ProblemPackagesFile, NoSpecifier>) new AzureBlobAggregationDocumentProvider<ProblemPackagesFile, NoSpecifier>(requestContext.GetPackagingTracesFacade(), requestContext.GetTracerFacade(), (IAggregationDocumentProcessor<ProblemPackagesFile>) new ProblemPackagesDocumentProcessor(this.nameParser, this.versionParser), (IFactory<IBlobService>) new BlobServiceForAggregationFactory(BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(requestContext).Bootstrap(), (IAggregation) this, (IConverter<IAggregation, Locator>) new AggregationToLocatorConverter(), requestContext.GetExecutionEnvironmentFacade()), FeatureAvailabilityConstants.CompressProblemPackagesDocumentFeature.Bootstrap(requestContext), "LastProblemPackagesBlobSize", (IAggregationDocumentLocatorProvider<NoSpecifier>) new FeedLevelAggregationDocumentLocatorProvider("problemPackagesList.json")));
    }

    public AggregationDefinition Definition { get; }

    public string VersionName { get; } = "V1";
  }
}
