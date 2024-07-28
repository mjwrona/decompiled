// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.CrossCollectionClientCreator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework
{
  public class CrossCollectionClientCreator : ICrossCollectionClientCreator
  {
    private readonly ICreateClient clientCreator;
    private readonly IUrlHostResolutionService urlHostResolutionService;
    private readonly ICrossCollectionLocationDataService locationDataService;

    public CrossCollectionClientCreator(
      IUrlHostResolutionService urlHostResolutionService,
      ICrossCollectionLocationDataService locationDataService,
      ICreateClient clientCreator)
    {
      this.clientCreator = clientCreator;
      this.urlHostResolutionService = urlHostResolutionService;
      this.locationDataService = locationDataService;
    }

    public async Task<TClient> CreateClientAsync<TClient>(
      Guid hostId,
      Guid serviceIdentifier,
      Func<Uri, TClient> makeClientWithCredentials,
      CancellationToken cancellationToken = default (CancellationToken))
      where TClient : VssHttpClientBase
    {
      Uri baseUri = this.urlHostResolutionService.GetHostUri(hostId, serviceIdentifier);
      if (baseUri == (Uri) null)
        throw new ServiceHostDoesNotExistException(hostId);
      ApiResourceLocationCollection baseAddressAsync = await this.locationDataService.GetLocationsForBaseAddressAsync(baseUri, cancellationToken);
      TClient client = makeClientWithCredentials(baseUri);
      client.SetResourceLocations(baseAddressAsync);
      TClient clientAsync = client;
      baseUri = (Uri) null;
      return clientAsync;
    }

    public async Task<TClient> CreateClientAsync<TClient>(
      Guid hostId,
      Guid serviceIdentifier,
      bool createAsElevated,
      CancellationToken cancellationToken = default (CancellationToken))
      where TClient : VssHttpClientBase
    {
      Uri baseUri = this.urlHostResolutionService.GetHostUri(hostId, serviceIdentifier);
      if (baseUri == (Uri) null)
        throw new ServiceHostDoesNotExistException(hostId);
      ApiResourceLocationCollection baseAddressAsync = await this.locationDataService.GetLocationsForBaseAddressAsync(baseUri, cancellationToken);
      TClient client = this.clientCreator.CreateClient<TClient>(baseUri, "Empty", baseAddressAsync, createAsElevated: createAsElevated);
      baseUri = (Uri) null;
      return client;
    }
  }
}
