// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.FrameworkGraphFederatedProviderService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal class FrameworkGraphFederatedProviderService : GraphFederatedProviderServiceBase
  {
    private const string Layer = "FrameworkGraphFederatedProviderService";

    protected override GraphFederatedProviderData AcquireProviderDataFromRemote(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName,
      long versionHint)
    {
      context.TraceEnter(60330420, "Graph", nameof (FrameworkGraphFederatedProviderService), nameof (AcquireProviderDataFromRemote));
      try
      {
        context.TraceDataConditionally(60330421, TraceLevel.Verbose, "Graph", nameof (FrameworkGraphFederatedProviderService), "Retrieving runtime provider data", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName
        }), nameof (AcquireProviderDataFromRemote));
        context.CheckOrganizationRequestContext();
        GraphFederatedProviderData providerData = context.GetClient<GraphHttpClient>().GetFederatedProviderDataAsync(descriptor, providerName, new long?(versionHint)).SyncResult<GraphFederatedProviderData>();
        context.TraceDataConditionally(60330428, TraceLevel.Verbose, "Graph", nameof (FrameworkGraphFederatedProviderService), "Retrieved runtime provider data", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName,
          providerData = providerData.Hashed(context)
        }), nameof (AcquireProviderDataFromRemote));
        return providerData;
      }
      finally
      {
        context.TraceLeave(60330429, "Graph", nameof (FrameworkGraphFederatedProviderService), nameof (AcquireProviderDataFromRemote));
      }
    }
  }
}
