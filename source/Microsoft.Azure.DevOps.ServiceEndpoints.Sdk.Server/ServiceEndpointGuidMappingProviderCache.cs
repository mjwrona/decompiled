// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointGuidMappingProviderCache
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class ServiceEndpointGuidMappingProviderCache : IServiceEndpointGuidMappingProvider
  {
    private readonly IServiceEndpointGuidMappingProvider _provider;
    private readonly ConcurrentDictionary<Guid, IDictionary<string, Guid>> _cache;

    public ServiceEndpointGuidMappingProviderCache(IServiceEndpointGuidMappingProvider provider)
    {
      this._provider = provider;
      this._cache = new ConcurrentDictionary<Guid, IDictionary<string, Guid>>();
    }

    public IDictionary<string, Guid> Get(IVssRequestContext context, Guid scopeId) => this._cache.GetOrAdd(scopeId, (Func<Guid, IDictionary<string, Guid>>) (key => this._provider.Get(context, key)));
  }
}
