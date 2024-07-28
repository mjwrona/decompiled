// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapters.QueryAdapters.QueryResultsWidgetAdapter
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapters.QueryAdapters
{
  public class QueryResultsWidgetAdapter : WidgetAdapterBase
  {
    public override Widget ProcessAndCopyWidget(
      Widget widget,
      IVssRequestContext requestContext,
      IDashboardConsumer sourceDashboardConsumer,
      IDashboardConsumer targetDashboardConsumer,
      bool purgeSettings = false,
      CopyDashboardParameterHandler copyDashboardParameterHandler = null)
    {
      Widget widget1 = base.ProcessAndCopyWidget(widget, requestContext, sourceDashboardConsumer, targetDashboardConsumer, purgeSettings, copyDashboardParameterHandler);
      Dictionary<string, object> dictionary1;
      if (widget1.Settings != null && JsonUtilities.TryDeserialize<Dictionary<string, object>>(widget1.Settings, out dictionary1, true))
      {
        Dictionary<string, object> dictionary2;
        if (dictionary1.Keys.Contains<string>("query") && JsonUtilities.TryDeserialize<Dictionary<string, object>>(dictionary1["query"].ToString(), out dictionary2, true) && dictionary2.ContainsKey("queryId") && dictionary2["queryId"] != null)
        {
          Guid result;
          if (Guid.TryParse(dictionary2["queryId"].ToString(), out result))
          {
            try
            {
              QueryHierarchyItem query = copyDashboardParameterHandler.GetOrCreateQuery(requestContext, sourceDashboardConsumer, targetDashboardConsumer, result);
              dictionary2["queryId"] = (object) query.Id;
              dictionary2["queryName"] = (object) query.Name;
            }
            catch (QueryItemNotFoundException ex)
            {
              requestContext.TraceException(10017703, "Dashboard", nameof (QueryResultsWidgetAdapter), (Exception) ex);
              dictionary2["queryId"] = (object) null;
              dictionary2["queryName"] = (object) null;
            }
            dictionary1["query"] = (object) dictionary2;
            widget1.Settings = dictionary1.Serialize<Dictionary<string, object>>(true);
            goto label_8;
          }
        }
        widget1.Settings = (string) null;
      }
      else
        widget1.Settings = (string) null;
label_8:
      return widget1;
    }
  }
}
