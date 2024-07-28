// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.ICrossCollectionClientCreatorService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  [DefaultServiceImplementation(typeof (CrossCollectionClientCreatorService))]
  public interface ICrossCollectionClientCreatorService : IVssFrameworkService
  {
    Task<TClient> CreateClientAsync<TClient>(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
      where TClient : VssHttpClientBase;

    Task<TClient> CreateClientAsync<TClient>(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceIdentifier,
      Func<Uri, TClient> makeClientWithCredentials,
      CancellationToken cancellationToken = default (CancellationToken))
      where TClient : VssHttpClientBase;
  }
}
