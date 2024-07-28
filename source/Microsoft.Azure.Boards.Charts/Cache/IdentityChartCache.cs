// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.Cache.IdentityChartCache
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;

namespace Microsoft.Azure.Boards.Charts.Cache
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IdentityChartCache : IdentityPropertiesView
  {
    public string GetChartCacheData(string entryName)
    {
      if (!string.IsNullOrWhiteSpace(entryName))
      {
        object propertyValue = (object) null;
        if (this.TryGetViewProperty(IdentityPropertyScope.Local, entryName, out propertyValue))
          return propertyValue as string;
      }
      return (string) null;
    }

    public void SetChartCacheItem(string entryName, string chartCacheData)
    {
      if (string.IsNullOrWhiteSpace(entryName))
        return;
      if (string.IsNullOrWhiteSpace(chartCacheData))
        this.RemoveViewProperty(IdentityPropertyScope.Local, entryName);
      else
        this.SetViewProperty(IdentityPropertyScope.Local, entryName, (object) chartCacheData);
    }
  }
}
