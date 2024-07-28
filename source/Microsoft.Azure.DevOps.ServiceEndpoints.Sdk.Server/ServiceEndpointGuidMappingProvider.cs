// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointGuidMappingProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class ServiceEndpointGuidMappingProvider : IServiceEndpointGuidMappingProvider
  {
    public IDictionary<string, Guid> Get(IVssRequestContext context, Guid scopeId)
    {
      IEnumerable<ServiceEndpoint> source = context.RunSynchronously<IEnumerable<ServiceEndpoint>>((Func<Task<IEnumerable<ServiceEndpoint>>>) (() => ServiceEndpointGuidMappingProvider.LoadEndpoints(context, scopeId)));
      Dictionary<string, Guid> dictionary = new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (source == null)
        return (IDictionary<string, Guid>) dictionary;
      foreach (ServiceEndpoint serviceEndpoint in source.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (e => !string.IsNullOrEmpty(e.Name))))
      {
        if (!dictionary.ContainsKey(serviceEndpoint.Name))
          dictionary.Add(serviceEndpoint.Name, serviceEndpoint.Id);
      }
      return (IDictionary<string, Guid>) dictionary;
    }

    private static async Task<IEnumerable<ServiceEndpoint>> LoadEndpoints(
      IVssRequestContext context,
      Guid scopeId)
    {
      return await context.GetService<IServiceEndpointService2>().QueryServiceEndpointsAsync(context, scopeId, (string) null, Enumerable.Empty<string>(), Enumerable.Empty<Guid>(), (string) null, true, actionFilter: ServiceEndpointActionFilter.View);
    }
  }
}
