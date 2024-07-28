// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.FrameworkGraphCachePolicyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkGraphCachePolicyService : GraphCachePolicyService
  {
    private const string Area = "GraphService";
    private const string Layer = "FrameworkGraphCachePolicyService";

    protected override int CalculateDynamicCacheSize(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.TraceEnter(10008010, "GraphService", nameof (FrameworkGraphCachePolicyService), nameof (CalculateDynamicCacheSize));
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          requestContext.Trace(10008016, TraceLevel.Info, "GraphService", nameof (FrameworkGraphCachePolicyService), "Request is at deployment level, returning 100k as default");
          return 100000;
        }
        GraphCachePolicies cachePolicies = requestContext.Elevate().GetClient<GraphHttpClient>().GetCachePoliciesAsync().SyncResult<GraphCachePolicies>();
        requestContext.TraceSerializedConditionally(10008012, TraceLevel.Info, "GraphService", nameof (FrameworkGraphCachePolicyService), "Cache policies read from remote: {0}", (object) cachePolicies);
        if (cachePolicies != null)
        {
          requestContext.TraceConditionally(10008013, TraceLevel.Info, "GraphService", nameof (FrameworkGraphCachePolicyService), (Func<string>) (() => string.Format("Cache policies are read from remote. Returning cache size : {0}", (object) cachePolicies.CacheSize)));
          return cachePolicies.CacheSize;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008014, "GraphService", nameof (FrameworkGraphCachePolicyService), ex);
      }
      requestContext.Trace(10008015, TraceLevel.Info, "GraphService", nameof (FrameworkGraphCachePolicyService), "Cache value cannot be read from remote, returning -1");
      requestContext.TraceLeave(10008011, "GraphService", nameof (FrameworkGraphCachePolicyService), nameof (CalculateDynamicCacheSize));
      return -1;
    }
  }
}
