// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphProviderInfoController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "ProviderInfo")]
  public class GraphProviderInfoController : GraphControllerBase
  {
    [HttpGet]
    [TraceFilter(6307645, 6307646)]
    public GraphProviderInfo GetProviderInfo([ClientParameterType(typeof (string), false)] SubjectDescriptor userDescriptor)
    {
      this.CheckPermissions(this.TfsRequestContext, GraphSecurityConstants.SubjectsToken, 1);
      return GraphProviderInfoHelper.FetchGraphUserProviderInfo(this.TfsRequestContext, userDescriptor);
    }
  }
}
