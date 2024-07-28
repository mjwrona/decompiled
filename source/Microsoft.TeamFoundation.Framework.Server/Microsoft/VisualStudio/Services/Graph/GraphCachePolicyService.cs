// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphCachePolicyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GraphCachePolicyService : IGraphCachePolicyService, IVssFrameworkService
  {
    private const string Area = "GraphService";
    private const string Layer = "GraphCachePolicyService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.ServiceHostId = systemRequestContext.ServiceHost.InstanceId;
      this.InitializeCachePolicies(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public GraphCachePolicies GetCachePolicies(IVssRequestContext requestContext)
    {
      requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
      return this.CachePolicies;
    }

    private void InitializeCachePolicies(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.TraceEnter(10008007, "GraphService", nameof (GraphCachePolicyService), nameof (InitializeCachePolicies));
        int defaultCacheSize = this.GetDefaultCacheSize(requestContext);
        requestContext.Trace(10008003, TraceLevel.Verbose, "GraphService", nameof (GraphCachePolicyService), "Default cache size: {0}", (object) defaultCacheSize);
        int num1 = this.CalculateDynamicCacheSize(requestContext);
        requestContext.Trace(10008004, TraceLevel.Verbose, "GraphService", nameof (GraphCachePolicyService), "Dynamic cache size: {0}", (object) num1);
        if (num1 == -1)
        {
          num1 = defaultCacheSize;
          requestContext.Trace(10008005, TraceLevel.Verbose, "GraphService", nameof (GraphCachePolicyService), "Dynamic cache size cannot be read. Taking default one: {0}", (object) defaultCacheSize);
        }
        int cacheSizeOverride = this.GetCacheSizeOverride(requestContext);
        requestContext.Trace(10008006, TraceLevel.Verbose, "GraphService", nameof (GraphCachePolicyService), "Cache size override: {0}", (object) cacheSizeOverride);
        int num2 = cacheSizeOverride != -1 ? cacheSizeOverride : num1;
        requestContext.Trace(10008002, TraceLevel.Verbose, "GraphService", nameof (GraphCachePolicyService), "CacheSize set to value: {0}", (object) num2);
        this.CachePolicies = new GraphCachePolicies()
        {
          CacheSize = num2
        };
      }
      finally
      {
        requestContext.TraceLeave(10008001, "GraphService", nameof (GraphCachePolicyService), nameof (InitializeCachePolicies));
      }
    }

    protected virtual int CalculateDynamicCacheSize(IVssRequestContext requestContext) => -1;

    private int GetDefaultCacheSize(IVssRequestContext requestContext) => requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? 100000 : 1024;

    private int GetCacheSizeOverride(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext;
      requestContext = requestContext.IsVirtualServiceHost() ? requestContext.To(TeamFoundationHostType.Parent) : requestContext;
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      int num = service1.GetValue<int>(requestContext, (RegistryQuery) "/Service/Identity/Settings/HostCacheSize", true, -1);
      RegistryQuery registryQuery;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        num = 10000;
      else if (vssRequestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
      {
        IVssRegistryService registryService = service1;
        IVssRequestContext requestContext1 = requestContext;
        registryQuery = (RegistryQuery) "/Service/Identity/Settings/DeploymentCacheSize";
        ref RegistryQuery local = ref registryQuery;
        num = registryService.GetValue<int>(requestContext1, in local, -1);
      }
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service2 = context.GetService<IVssRegistryService>();
      IVssRequestContext requestContext2 = context;
      registryQuery = (RegistryQuery) "/Service/Integration/Settings/IdentityHostCacheSize";
      ref RegistryQuery local1 = ref registryQuery;
      int defaultValue = num;
      return service2.GetValue<int>(requestContext2, in local1, defaultValue);
    }

    private GraphCachePolicies CachePolicies { get; set; }

    private Guid ServiceHostId { get; set; }
  }
}
