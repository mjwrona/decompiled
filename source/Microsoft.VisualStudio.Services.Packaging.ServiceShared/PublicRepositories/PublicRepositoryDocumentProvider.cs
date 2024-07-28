// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepositoryDocumentProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public static class PublicRepositoryDocumentProvider
  {
    public static IAggregationDocumentProvider<TDoc, TPackageName> Bootstrap<TDoc, TPackageName>(
      IVssRequestContext deploymentContext,
      WellKnownUpstreamSource wellKnownUpstreamSource,
      IAggregationDocumentProcessor<TDoc> serializer,
      ICacheUniverseProvider cacheUniverseProvider)
      where TPackageName : class, IPackageName
    {
      deploymentContext.CheckDeploymentRequestContext();
      ContainerAddress input = new ContainerAddress((CollectionId) null, new Locator(new string[2]
      {
        "pubRepo",
        wellKnownUpstreamSource.TagName
      }));
      IBlobService instance = BlobServiceFactoryBootstrapper.CreateSharded(deploymentContext, (IItemStoreBlobEncodingStrategy) new Base64ItemStoreBlobEncodingStrategy()).Bootstrap().Get(input);
      return (IAggregationDocumentProvider<TDoc, TPackageName>) new AzureBlobAggregationDocumentProvider<TDoc, TPackageName>(deploymentContext.GetPackagingTracesFacade(), deploymentContext.GetTracerFacade(), serializer, ReturnSameInstanceFactory.For<IBlobService>(instance), (IOrgLevelPackagingSetting<bool>) ConstantPackagingSetting.From<bool>(true), "PubCacheLastBlobSize_" + wellKnownUpstreamSource.TagName, (IAggregationDocumentLocatorProvider<TPackageName>) new PublicRepositoryLocatorProvider(cacheUniverseProvider));
    }
  }
}
