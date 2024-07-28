// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemViewOptions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class WorkItemViewOptions
  {
    public AnalyticsViewScope ViewScope { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public WorkItemsViewType Type { get; set; }

    public List<string> WorkItemTypes { get; set; }

    public bool IncludeFilterForNonHiddenWorkItemTypes { get; set; }

    public int Period { get; set; }

    public WorkItemHistoricalInterval Interval { get; set; }

    public List<string> BacklogNames { get; set; }
  }
}
