// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphProviderInfoHelper
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphProviderInfoHelper
  {
    private const string c_area = "Graph";
    private const string c_layer = "GraphProviderInfoHelper";

    public static GraphProviderInfo FetchGraphUserProviderInfo(
      IVssRequestContext requestContext,
      SubjectDescriptor userDescriptor)
    {
      GraphUser graphUser = GraphSubjectHelper.FetchGraphUser(requestContext, userDescriptor);
      if (graphUser == null)
        throw new GraphSubjectNotFoundException(userDescriptor);
      if (graphUser.Origin == "ghb")
      {
        GraphProviderInfo graphUserProviderInfo = new GraphProviderInfo("msa", GraphSubjectHelper.FetchSingleIdentityByDescriptor(requestContext, userDescriptor)?.GetProperty<string>("PUID", (string) null), graphUser.Descriptor, graphUser.Domain);
        requestContext.TraceDataConditionally(6307647, TraceLevel.Verbose, "Graph", nameof (GraphProviderInfoHelper), "Provider info for user", (Func<object>) (() => (object) new
        {
          graphUserProviderInfo = graphUserProviderInfo,
          graphUser = graphUser
        }), nameof (FetchGraphUserProviderInfo));
        return graphUserProviderInfo;
      }
      GraphProviderInfo graphUserProviderInfo1 = new GraphProviderInfo(graphUser.Origin, graphUser.OriginId, graphUser.Descriptor, graphUser.Domain);
      requestContext.TraceDataConditionally(6307647, TraceLevel.Verbose, "Graph", nameof (GraphProviderInfoHelper), "Provider info for user", (Func<object>) (() => (object) new
      {
        graphUserProviderInfo = graphUserProviderInfo1,
        graphUser = graphUser
      }), nameof (FetchGraphUserProviderInfo));
      return graphUserProviderInfo1;
    }
  }
}
