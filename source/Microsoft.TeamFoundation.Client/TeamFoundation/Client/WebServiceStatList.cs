// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WebServiceStatList
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  public class WebServiceStatList : BindingList<WebServiceStats>
  {
    private WebServiceStats GetStats(string webServiceName)
    {
      foreach (WebServiceStats stats in (Collection<WebServiceStats>) this)
      {
        if (string.Equals(webServiceName, stats.WebService, StringComparison.Ordinal))
          return stats;
      }
      WebServiceStats stats1 = new WebServiceStats(webServiceName);
      this.Add(stats1);
      return stats1;
    }

    public void AddTime(string webServiceName, int runTime)
    {
      this.GetStats(webServiceName).AddTime(runTime);
      if (!(this.Items is List<WebServiceStats> items))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Comparison<WebServiceStats> comparison = WebServiceStatList.\u003C\u003EO.\u003C0\u003E__DescendingSort ?? (WebServiceStatList.\u003C\u003EO.\u003C0\u003E__DescendingSort = new Comparison<WebServiceStats>(WebServiceStats.DescendingSort));
      items.Sort(comparison);
    }
  }
}
