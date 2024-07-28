// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.TestResultsTrendsAdvancedWidgetAdapter
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class TestResultsTrendsAdvancedWidgetAdapter : WidgetAdapterBase
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
      if (targetDashboardConsumer.GetDataspaceId() != sourceDashboardConsumer.GetDataspaceId() && JsonUtilities.TryDeserialize<Dictionary<string, object>>(widget1.Settings, out dictionary, true))
      {
        dictionary["buildPipelines"] = (object) null;
        dictionary["releasePipelines"] = (object) null;
        widget1.Settings = dictionary.Serialize<Dictionary<string, object>>();
      }
      return widget1;
    }
  }
}
