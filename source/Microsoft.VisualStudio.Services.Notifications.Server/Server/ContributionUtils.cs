// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ContributionUtils
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class ContributionUtils
  {
    public static string GetCacheKey(
      string associatedDataName,
      IEnumerable<string> contributionIds,
      HashSet<string> contributionTypes,
      ContributionQueryOptions? queryOptions)
    {
      StringBuilder sb = new StringBuilder(associatedDataName);
      if (contributionIds != null)
        contributionIds.ForEach<string>((Action<string>) (s => sb.Append(string.Format("/{0:x8}", (object) s.GetHashCode()))));
      if (contributionTypes != null)
        contributionTypes.ForEach<string>((Action<string>) (s => sb.Append(string.Format(".{0:x8}", (object) s.GetHashCode()))));
      if (queryOptions.HasValue)
        sb.Append(string.Format("-{0:x8}", (object) (int) queryOptions.Value));
      return sb.ToString();
    }
  }
}
