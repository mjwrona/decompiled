// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.CrossCollectionClientCreatorService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.Utils.CrossCollectionLocationData;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public class CrossCollectionClientCreatorService : 
    ICrossCollectionClientCreatorService,
    IVssFrameworkService
  {
    public async Task<TClient> CreateClientAsync<TClient>(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
      where TClient : VssHttpClientBase
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Uri baseUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, hostId, serviceIdentifier);
      if (baseUri == (Uri) null)
        throw new ServiceHostDoesNotExistException(hostId);
      TClient client = ((ICreateClient) requestContext.ClientProvider).CreateClient<TClient>(requestContext, baseUri, "Empty", await vssRequestContext.GetService<ICrossCollectionLocationDataService>().GetLocationsForBaseAddressAsync(vssRequestContext, baseUri, cancellationToken));
      baseUri = (Uri) null;
      return client;
    }

    public async Task<TClient> CreateClientAsync<TClient>(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceIdentifier,
      Func<Uri, TClient> makeClientWithCredentials,
      CancellationToken cancellationToken = default (CancellationToken))
      where TClient : VssHttpClientBase
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Uri baseUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, hostId, serviceIdentifier);
      if (baseUri == (Uri) null)
        throw new ServiceHostDoesNotExistException(hostId);
      ApiResourceLocationCollection baseAddressAsync = await vssRequestContext.GetService<ICrossCollectionLocationDataService>().GetLocationsForBaseAddressAsync(vssRequestContext, baseUri, cancellationToken);
      TClient client = makeClientWithCredentials(baseUri);
      client.SetResourceLocations(baseAddressAsync);
      TClient clientAsync = client;
      baseUri = (Uri) null;
      return clientAsync;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
