// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebPerformanceTimerHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class WebPerformanceTimerHelpers
  {
    public static PerformanceTimer StartMeasure(
      WebContext webContext,
      string groupName,
      string entryContext = null)
    {
      return PerformanceTimer.StartMeasure(webContext == null ? (IVssRequestContext) null : webContext.TfsRequestContext, groupName, entryContext);
    }

    public static IDictionary<string, PerformanceTimingGroup> SendCustomerIntelligenceData(
      WebContext webContext,
      Action<CustomerIntelligenceData> addCustomProperties = null)
    {
      return PerformanceTimer.SendCustomerIntelligenceData(webContext.TfsRequestContext, (Action<CustomerIntelligenceData>) (data =>
      {
        Action<CustomerIntelligenceData> action = addCustomProperties;
        if (action != null)
          action(data);
        data.Add("Controller", webContext.NavigationContext.CurrentController);
        data.Add("Action", webContext.NavigationContext.CurrentAction);
        data.Add("Area", webContext.NavigationContext.Area);
        data.Add("NavigationLevel", (object) webContext.NavigationContext.TopMostLevel);
        data.Add("CorrelationId", (object) webContext.Diagnostics.SessionId);
        data.Add("ElapsedMillis", webContext.TfsRequestContext.LastTracedBlockElapsedMilliseconds());
        data.Add("ResponseCode", (double) webContext.RequestContext.HttpContext.Response.StatusCode);
        data.Add("CommandName", webContext.NavigationContext.CommandName ?? string.Empty);
        data.Add("RouteId", webContext.NavigationContext.RouteId ?? string.Empty);
      }));
    }
  }
}
