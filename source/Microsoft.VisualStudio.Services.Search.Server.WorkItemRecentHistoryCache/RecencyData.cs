// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.RecencyData
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D4A0500-806F-44D4-BA97-D409A2311716
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache
{
  public class RecencyData
  {
    public RecencyData()
    {
      this.workItemIds = new List<string>();
      this.areaIds = new List<string>();
    }

    public List<string> workItemIds { get; set; }

    public List<string> areaIds { get; set; }

    public override string ToString() => string.Join(",", (IEnumerable<string>) new List<string>()
    {
      FormattableString.Invariant(FormattableStringFactory.Create("[WorkItemIds: {0}]", this.workItemIds == null ? (object) "" : (object) string.Join(",", (IEnumerable<string>) this.workItemIds))),
      FormattableString.Invariant(FormattableStringFactory.Create("[AreaIds: {0}]", this.areaIds == null ? (object) "" : (object) string.Join(",", (IEnumerable<string>) this.areaIds)))
    });
  }
}
