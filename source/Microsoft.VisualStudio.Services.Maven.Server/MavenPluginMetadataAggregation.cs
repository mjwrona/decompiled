// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPluginMetadataAggregation
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores.ItemStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenPluginMetadataAggregation : 
    IAggregation<MavenPluginMetadataAggregation, IAggregationAccessor<MavenPluginMetadataAggregation>>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly MavenPluginMetadataAggregation ByItemStore = new MavenPluginMetadataAggregation();

    public AggregationDefinition Definition { get; } = AggregationDefinitions.MavenPluginMetadataAggregationDefinition;

    public string VersionName { get; } = nameof (ByItemStore);

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      RegistryServiceFacade registryServiceFacade = new RegistryServiceFacade(requestContext);
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      FeedPermsFacade permsFacade = new FeedPermsFacade(requestContext);
      return (IAggregationAccessor) new MavenPluginMetadataAggregationAccessor((IAggregation) this, (IWritableMavenPluginMetadataStore) new MavenPluginMetadataStoreByItemStore((IContentItemstore) new ItemStoreFacade<MavenItemStore>(requestContext, tracerFacade), (IFeedPerms) permsFacade, tracerFacade, (IRegistryService) registryServiceFacade));
    }
  }
}
