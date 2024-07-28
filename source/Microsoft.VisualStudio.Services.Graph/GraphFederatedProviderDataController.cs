// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphFederatedProviderDataController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "FederatedProviderData")]
  [ClientInternalUseOnly(true)]
  [RestrictInternalGraphEndpoints]
  public class GraphFederatedProviderDataController : GraphControllerBase
  {
    private const string TraceLayer = "GraphFederatedProviderDataController";

    [HttpGet]
    [TraceFilter(6307500, 6307509)]
    public GraphFederatedProviderData GetFederatedProviderData(
      SubjectDescriptor subjectDescriptor,
      string providerName,
      long versionHint = -1)
    {
      this.TfsRequestContext.TraceDataConditionally(6307501, TraceLevel.Verbose, this.TraceArea, nameof (GraphFederatedProviderDataController), "Controller input", (Func<object>) (() => (object) new
      {
        subjectDescriptor = subjectDescriptor,
        providerName = providerName,
        versionHint = versionHint
      }), nameof (GetFederatedProviderData));
      GraphFederatedProviderData providerData = this.TfsRequestContext.GetService<IGraphFederatedProviderService>().AcquireProviderData(this.TfsRequestContext, subjectDescriptor, providerName, versionHint);
      this.TfsRequestContext.TraceDataConditionally(6307502, TraceLevel.Verbose, this.TraceArea, nameof (GraphFederatedProviderDataController), "Controller output", (Func<object>) (() => (object) new
      {
        providerData = providerData.Hashed(this.TfsRequestContext)
      }), nameof (GetFederatedProviderData));
      return providerData;
    }
  }
}
