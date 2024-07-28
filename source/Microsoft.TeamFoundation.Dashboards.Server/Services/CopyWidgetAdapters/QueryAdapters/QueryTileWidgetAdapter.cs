// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapters.QueryAdapters.QueryTileWidgetAdapter
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
  public class QueryTileWidgetAdapter : WidgetAdapterBase
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
      Dictionary<string, object> dictionary;
      if (widget1.Settings != null && JsonUtilities.TryDeserialize<Dictionary<string, object>>(widget1.Settings, out dictionary, true) && dictionary.Keys.Contains<string>("queryId"))
      {
        if (dictionary["queryId"] != null)
        {
          try
          {
            Guid result;
            if (Guid.TryParse(dictionary["queryId"].ToString(), out result))
            {
              QueryHierarchyItem query = copyDashboardParameterHandler.GetOrCreateQuery(requestContext, sourceDashboardConsumer, targetDashboardConsumer, result);
              dictionary["queryId"] = (object) query.Id;
              dictionary["queryName"] = (object) query.Name;
            }
          }
          catch (QueryItemNotFoundException ex)
          {
            requestContext.TraceException(10017702, "Dashboard", nameof (QueryTileWidgetAdapter), (Exception) ex);
            dictionary["queryId"] = (object) null;
            dictionary["queryName"] = (object) null;
          }
          widget1.Settings = dictionary.Serialize<Dictionary<string, object>>(true);
          goto label_7;
        }
      }
      widget1.Settings = (string) null;
label_7:
      return widget1;
    }
  }
}
