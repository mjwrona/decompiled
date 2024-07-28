// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.CountKpi
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class CountKpi : WorkItemTrackingKpi
  {
    public CountKpi(IVssRequestContext requestContext, string name, int resultCount)
      : base(requestContext, name, resultCount)
    {
      this.Skip = false;
      this.Value = (double) resultCount;
    }

    public CountKpi(IVssRequestContext requestContext, string name, double kpiValue)
      : base(requestContext, name)
    {
      this.Skip = false;
      this.Value = kpiValue;
    }
  }
}
