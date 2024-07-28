// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServiceEndpointStore : IServiceEndpointStore
  {
    private readonly Dictionary<Guid, ServiceEndpoint> m_endpointsById = new Dictionary<Guid, ServiceEndpoint>();
    private readonly Dictionary<string, List<ServiceEndpoint>> m_endpointsByName = new Dictionary<string, List<ServiceEndpoint>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public ServiceEndpointStore(IList<ServiceEndpoint> endpoints, IServiceEndpointResolver resolver = null)
    {
      this.Resolver = resolver;
      this.Add(endpoints != null ? endpoints.ToArray<ServiceEndpoint>() : (ServiceEndpoint[]) null);
    }

    public IServiceEndpointResolver Resolver { get; }

    public IList<ServiceEndpointReference> GetAuthorizedReferences() => this.Resolver == null ? (IList<ServiceEndpointReference>) new List<ServiceEndpointReference>() : this.Resolver.GetAuthorizedReferences();

    public void Authorize(ServiceEndpointReference reference) => this.Resolver?.Authorize(reference);

    public ServiceEndpoint Get(ServiceEndpointReference reference)
    {
      if (reference == null)
        return (ServiceEndpoint) null;
      Guid id = reference.Id;
      string literal = reference.Name?.Literal;
      if (id == Guid.Empty && string.IsNullOrEmpty(literal))
        return (ServiceEndpoint) null;
      ServiceEndpoint serviceEndpoint = (ServiceEndpoint) null;
      if (id != Guid.Empty)
      {
        if (this.m_endpointsById.TryGetValue(id, out serviceEndpoint))
          return serviceEndpoint;
      }
      else
      {
        List<ServiceEndpoint> serviceEndpointList;
        if (!string.IsNullOrEmpty(literal) && this.m_endpointsByName.TryGetValue(literal, out serviceEndpointList))
          return serviceEndpointList.Count <= 1 ? serviceEndpointList[0] : throw new AmbiguousResourceSpecificationException(PipelineStrings.AmbiguousServiceEndpointSpecification((object) id));
      }
      IServiceEndpointResolver resolver = this.Resolver;
      serviceEndpoint = resolver != null ? resolver.Resolve(reference) : (ServiceEndpoint) null;
      if (serviceEndpoint != null)
        this.Add(serviceEndpoint);
      return serviceEndpoint;
    }

    private void Add(params ServiceEndpoint[] endpoints)
    {
      if (endpoints == null || endpoints.Length == 0)
        return;
      foreach (ServiceEndpoint endpoint in endpoints)
      {
        if (!this.m_endpointsById.TryGetValue(endpoint.Id, out ServiceEndpoint _))
        {
          this.m_endpointsById.Add(endpoint.Id, endpoint);
          List<ServiceEndpoint> serviceEndpointList;
          if (!this.m_endpointsByName.TryGetValue(endpoint.Name, out serviceEndpointList))
          {
            serviceEndpointList = new List<ServiceEndpoint>();
            this.m_endpointsByName.Add(endpoint.Name, serviceEndpointList);
          }
          serviceEndpointList.Add(endpoint);
        }
      }
    }
  }
}
