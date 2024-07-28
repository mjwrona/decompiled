// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.IConnectedServiceProviderService
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  [DefaultServiceImplementation(typeof (ConnectedServiceProviderService))]
  public interface IConnectedServiceProviderService : IVssFrameworkService, IDisposable
  {
    IReadOnlyList<ConnectedServiceProvider> ProviderImplementations { get; }

    ConnectedServiceProvider GetProvider(IVssRequestContext requestContext, string providerId);

    IEnumerable<ConnectedServiceProvider> GetProviders(IVssRequestContext requestContext);
  }
}
