// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.ServiceEndpointResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class ServiceEndpointResolver : IServiceEndpointResolver
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;
    private readonly bool m_allowImplicitAuthorization;
    private HashSet<ServiceEndpointReference> m_authorizedReferences = new HashSet<ServiceEndpointReference>((IEqualityComparer<ServiceEndpointReference>) new PipelineResources.EndpointComparer());

    public ServiceEndpointResolver(
      IVssRequestContext requestContext,
      Guid projectId,
      bool allowImplicitAuthorization)
    {
      this.m_projectId = projectId;
      this.m_requestContext = requestContext;
      this.m_allowImplicitAuthorization = allowImplicitAuthorization;
    }

    public void Authorize(ServiceEndpointReference reference) => this.m_authorizedReferences.Add(reference);

    public IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> Resolve(
      ICollection<ServiceEndpointReference> references,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> resolvedEndpoints = ServiceEndpointResolver.Resolve(this.m_requestContext.Elevate(), this.m_projectId, references).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>();
      references.ForEach<ServiceEndpointReference>((Action<ServiceEndpointReference>) (reference =>
      {
        if (!(reference.Id == Guid.Empty))
          return;
        Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = resolvedEndpoints.Find((Predicate<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.Name.Equals(reference.Name.Literal, StringComparison.OrdinalIgnoreCase)));
        if (serviceEndpoint == null)
          return;
        reference.Id = serviceEndpoint.Id;
      }));
      this.m_authorizedReferences.ForEach<ServiceEndpointReference>((Action<ServiceEndpointReference>) (authorizedReference =>
      {
        if (!(authorizedReference.Id == Guid.Empty))
          return;
        Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = resolvedEndpoints.Find((Predicate<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.Name.Equals(authorizedReference.Name.Literal, StringComparison.OrdinalIgnoreCase)));
        if (serviceEndpoint == null)
          return;
        authorizedReference.Id = serviceEndpoint.Id;
      }));
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> collection1 = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>();
      List<ServiceEndpointReference> references1 = new List<ServiceEndpointReference>();
      foreach (ServiceEndpointReference reference1 in (IEnumerable<ServiceEndpointReference>) references)
      {
        ServiceEndpointReference reference = reference1;
        if (this.m_authorizedReferences.Any<ServiceEndpointReference>((Func<ServiceEndpointReference, bool>) (x => x.Id == reference.Id)))
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = resolvedEndpoints.Find((Predicate<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.Id == reference.Id));
          if (serviceEndpoint != null)
            collection1.Add(serviceEndpoint);
        }
        else
          references1.Add(reference);
      }
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> serviceEndpointList = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>();
      if (collection1.Count > 0)
        serviceEndpointList.AddRange((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) collection1);
      if (this.m_allowImplicitAuthorization && references1.Count > 0)
      {
        IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> collection2 = ServiceEndpointResolver.Resolve(this.m_requestContext, this.m_projectId, (ICollection<ServiceEndpointReference>) references1, actionFilter);
        serviceEndpointList.AddRange((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) collection2);
        collection2.ForEach<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Action<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint =>
        {
          this.Authorize(new ServiceEndpointReference()
          {
            Id = endpoint.Id,
            Name = (ExpressionValue<string>) endpoint.Name
          });
        }));
      }
      return (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) serviceEndpointList;
    }

    public IList<ServiceEndpointReference> GetAuthorizedReferences() => (IList<ServiceEndpointReference>) this.m_authorizedReferences.ToList<ServiceEndpointReference>();

    internal static IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> Resolve(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<ServiceEndpointReference> references,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      ServiceEndpointActionFilter endpointActionFilter = ServiceEndpointResolver.ParseToServiceEndpointActionFilter(actionFilter);
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> serviceEndpointList = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>();
      if (references != null && references.Count > 0)
      {
        List<Guid> endpointIds = new List<Guid>();
        List<Guid> list1 = references.Where<ServiceEndpointReference>((Func<ServiceEndpointReference, bool>) (x => x.Id != Guid.Empty)).Select<ServiceEndpointReference, Guid>((Func<ServiceEndpointReference, Guid>) (x => x.Id)).ToList<Guid>();
        if (list1.Count > 0)
        {
          IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> serviceEndpoints = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) list1, false, true, endpointActionFilter);
          if (requestContext.IsSystemContext)
            serviceEndpointList.AddRange(serviceEndpoints);
          else
            endpointIds.AddRange(serviceEndpoints.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Guid>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Guid>) (endpoint => endpoint.Id)));
        }
        List<string> list2 = references.Where<ServiceEndpointReference>((Func<ServiceEndpointReference, bool>) (x => x.Id == Guid.Empty && !string.IsNullOrEmpty(x.Name?.Literal))).Select<ServiceEndpointReference, string>((Func<ServiceEndpointReference, string>) (x => x.Name.Literal)).ToList<string>();
        if (list2.Count > 0)
        {
          IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> serviceEndpoints = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<string>) list2, false, true, ServiceEndpointActionFilter.Use);
          if (requestContext.IsSystemContext)
            serviceEndpointList.AddRange(serviceEndpoints);
          else
            endpointIds.AddRange(serviceEndpoints.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Guid>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Guid>) (endpoint => endpoint.Id)));
        }
        if (endpointIds.Count > 0)
          serviceEndpointList.AddRange(DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext.Elevate(), projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) endpointIds, false, true, ServiceEndpointActionFilter.Use));
      }
      return (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) serviceEndpointList;
    }

    private static ServiceEndpointActionFilter ParseToServiceEndpointActionFilter(
      ResourceActionFilter actionFilter)
    {
      ServiceEndpointActionFilter endpointActionFilter = ServiceEndpointActionFilter.Use;
      switch (actionFilter)
      {
        case ResourceActionFilter.None:
          endpointActionFilter = ServiceEndpointActionFilter.None;
          break;
        case ResourceActionFilter.Manage:
          endpointActionFilter = ServiceEndpointActionFilter.Manage;
          break;
        case ResourceActionFilter.Use:
          endpointActionFilter = ServiceEndpointActionFilter.Use;
          break;
      }
      return endpointActionFilter;
    }
  }
}
