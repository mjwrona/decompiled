// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IWorkItemTrendService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (WorkItemTrendService))]
  public interface IWorkItemTrendService : IVssFrameworkService
  {
    IList<TrendDataPoint<T>> GetTrendData<T>(
      IVssRequestContext requestContext,
      string referenceName,
      DateTime beginDate,
      TimeZoneInfo timeZone,
      TimeSpan interval,
      DateTime? endDate = null);

    void RebuildTrendData(
      IVssRequestContext requestContext,
      string referenceName,
      string markerFieldReferenceName,
      DateTime beginDate,
      bool isBlocking = false);
  }
}
