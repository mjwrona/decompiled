// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.PlatformGraphCachePolicyService
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PlatformGraphCachePolicyService : GraphCachePolicyService
  {
    private const string Area = "GraphService";
    private const string Layer = "PlatformGraphCachePolicyService";

    protected override int CalculateDynamicCacheSize(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.TraceEnter(10008210, "GraphService", nameof (PlatformGraphCachePolicyService), nameof (CalculateDynamicCacheSize));
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return 1024;
        requestContext.Trace(10008216, TraceLevel.Info, "GraphService", nameof (PlatformGraphCachePolicyService), "Request is at deployment level, returning 100k as default");
        return 100000;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008214, "GraphService", nameof (PlatformGraphCachePolicyService), ex);
      }
      requestContext.Trace(10008215, TraceLevel.Info, "GraphService", nameof (PlatformGraphCachePolicyService), "Cache value cannot be read from sql, returning -1");
      requestContext.TraceLeave(10008211, "GraphService", nameof (PlatformGraphCachePolicyService), nameof (CalculateDynamicCacheSize));
      return -1;
    }
  }
}
