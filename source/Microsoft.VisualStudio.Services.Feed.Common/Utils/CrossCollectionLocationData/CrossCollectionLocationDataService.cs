// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.Utils.CrossCollectionLocationData.CrossCollectionLocationDataService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.Utils.CrossCollectionLocationData
{
  public class CrossCollectionLocationDataService : 
    VssMemoryCacheService<Uri, ApiResourceLocationCollection>,
    ICrossCollectionLocationDataService,
    IVssFrameworkService
  {
    private static readonly MemoryCacheConfiguration<Uri, ApiResourceLocationCollection> DefaultMemoryCacheConfiguration = new MemoryCacheConfiguration<Uri, ApiResourceLocationCollection>().WithCleanupInterval(TimeSpan.FromMinutes(30.0)).WithExpiryInterval(TimeSpan.FromMinutes(5.0));
    private readonly ConcurrencyConsolidator<Uri, ApiResourceLocationCollection> concurrencyConsolidator = new ConcurrencyConsolidator<Uri, ApiResourceLocationCollection>(false, 2);

    public CrossCollectionLocationDataService()
      : base((IEqualityComparer<Uri>) EqualityComparer<Uri>.Default, CrossCollectionLocationDataService.DefaultMemoryCacheConfiguration)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      base.ServiceStart(systemRequestContext);
    }

    public async Task<ApiResourceLocationCollection> GetLocationsForBaseAddressAsync(
      IVssRequestContext requestContext,
      Uri baseAddress,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CrossCollectionLocationDataService locationDataService = this;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Uri>(baseAddress, nameof (baseAddress));
      requestContext.CheckDeploymentRequestContext();
      ApiResourceLocationCollection locations;
      return locationDataService.TryGetValue(requestContext, baseAddress, out locations) ? locations : await locationDataService.concurrencyConsolidator.RunOnceAsync(baseAddress, (Func<Task<ApiResourceLocationCollection>>) (async () =>
      {
        IVssRequestContext requestContext1 = requestContext.Elevate();
        CrossCollectionLocationDataHttpClient client = ((ICreateClient) requestContext1.ClientProvider).CreateClient<CrossCollectionLocationDataHttpClient>(requestContext1, baseAddress, nameof (CrossCollectionLocationDataService), (ApiResourceLocationCollection) null, false);
        locations = new ApiResourceLocationCollection();
        ApiResourceLocationCollection locationCollection = locations;
        CancellationToken cancellationToken1 = cancellationToken;
        locationCollection.AddResourceLocations(await client.GetResourceLocationsAsync(cancellationToken: cancellationToken1));
        locationCollection = (ApiResourceLocationCollection) null;
        this.Set(requestContext, baseAddress, locations);
        return locations;
      }));
    }
  }
}
