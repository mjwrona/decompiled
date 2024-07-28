// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemLinksRequest`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  public class ReportingWorkItemLinksRequest<T>
  {
    public string ContinuationToken { get; set; }

    public bool FromContinuationToken { get; set; } = true;

    public string Watermark { get; set; }

    public DateTime? StartDateTime { get; set; }

    public IEnumerable<string> Types { get; set; }

    public IEnumerable<string> LinkTypes { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.CreateLink<T> CreateLink { get; set; }
  }
}
