// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageNames.NpmNamesAggregation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationCore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageNames
{
  public class NpmNamesAggregation : 
    IAggregation<NpmNamesAggregation, INpmNamesAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly NpmNamesAggregation V1 = new NpmNamesAggregation(nameof (V1));

    private IBootstrapper<IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>>> GetNameServiceFactoryBootstrapper(
      IVssRequestContext requestContext)
    {
      return (IBootstrapper<IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>>>) new NpmPackageNamesServiceBootstrapper(requestContext);
    }

    private NpmNamesAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = NpmAggregationDefinitions.NpmNamesAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>> factory = this.GetNameServiceFactoryBootstrapper(requestContext).Bootstrap();
      AggregationToLocatorConverter aggVersionToLocatorConverter = new AggregationToLocatorConverter();
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>> nameServiceFactory = factory;
      IExecutionEnvironment executionEnvironment = environmentFacade;
      return (IAggregationAccessor) new NpmNamesAggregationAccessor((IConverter<IAggregation, Locator>) aggVersionToLocatorConverter, nameServiceFactory, executionEnvironment, (IAggregation) this);
    }
  }
}
