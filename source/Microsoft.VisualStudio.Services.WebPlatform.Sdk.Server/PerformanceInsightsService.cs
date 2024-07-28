// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.PerformanceInsightsService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class PerformanceInsightsService : IPerformanceInsightsService, IVssFrameworkService
  {
    private const string c_insightsItemsKey = "performanceInsights";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddPerformanceInsight(IVssRequestContext requestContext, PerformanceInsight insight) => this.GetPerformanceInsights(requestContext, true).Add(insight);

    public IEnumerable<PerformanceInsight> GetPerformanceInsights(IVssRequestContext requestContext) => (IEnumerable<PerformanceInsight>) this.GetPerformanceInsights(requestContext, false);

    private List<PerformanceInsight> GetPerformanceInsights(
      IVssRequestContext requestContext,
      bool createIfNotExists)
    {
      List<PerformanceInsight> performanceInsights = (List<PerformanceInsight>) null;
      object obj;
      if (requestContext.Items.TryGetValue("performanceInsights", out obj))
        performanceInsights = obj as List<PerformanceInsight>;
      else if (createIfNotExists)
      {
        performanceInsights = new List<PerformanceInsight>();
        requestContext.Items["performanceInsights"] = (object) performanceInsights;
      }
      return performanceInsights;
    }
  }
}
