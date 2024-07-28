// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.GetWorkItemKpi
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class GetWorkItemKpi : WorkItemTrackingKpi
  {
    public override int DefaultSamplingRate => 10;

    public GetWorkItemKpi(
      IVssRequestContext requestContext,
      WorkItem[] result,
      bool includeHistory)
      : base(requestContext, "GetWorkItem", result.Length)
    {
      GetWorkItemKpi getWorkItemKpi = this;
      if (!(!this.Skip & includeHistory))
        return;
      this.Skip = ((IEnumerable<WorkItem>) result).Where<WorkItem>((Func<WorkItem, bool>) (w => w != null)).Any<WorkItem>((Func<WorkItem, bool>) (w => w.Revision > getWorkItemKpi.GetThreshold(requestContext, string.Format("/Service/WorkItemTracking/Settings/Kpi/{0}", (object) "GetWorkItemRevLimit"), 10)));
    }
  }
}
