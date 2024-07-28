// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationActivityStatisticService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationActivityStatisticService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ActivityStatistic> QueryActivityLogEntries(
      IVssRequestContext requestContext,
      Guid activityId)
    {
      using (ActivityStatisticsComponent component = requestContext.CreateComponent<ActivityStatisticsComponent>())
      {
        ObjectBinder<ActivityStatistic> current = component.QueryActivityStatistics(activityId).GetCurrent<ActivityStatistic>();
        if (current.Items.Count > 0)
          return (IEnumerable<ActivityStatistic>) current.Items;
      }
      return (IEnumerable<ActivityStatistic>) new List<ActivityStatistic>();
    }
  }
}
