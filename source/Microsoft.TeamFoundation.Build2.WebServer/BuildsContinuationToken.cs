// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildsContinuationToken
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  public class BuildsContinuationToken
  {
    private BuildsContinuationToken(DateTime time) => this.Time = new DateTime?(time);

    public BuildsContinuationToken(BuildData nextBuild, Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder? buildQueryOrder = null)
    {
      if (nextBuild == null)
        return;
      this.Initialize(nextBuild.QueueTime, nextBuild.StartTime, nextBuild.FinishTime, buildQueryOrder);
    }

    public BuildsContinuationToken(
      DateTime? queueTime,
      DateTime? startTime,
      DateTime? finishTime,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder? buildQueryOrder = null)
    {
      this.Initialize(queueTime, startTime, finishTime, buildQueryOrder);
    }

    public DateTime? Time { get; private set; }

    public override string ToString() => this.Time.HasValue ? this.Time.Value.ToString("o") : string.Empty;

    public static bool TryParse(string value, out BuildsContinuationToken token)
    {
      token = (BuildsContinuationToken) null;
      DateTime result;
      if (string.IsNullOrEmpty(value) || !DateTime.TryParse(value, out result))
        return false;
      token = new BuildsContinuationToken(result);
      return true;
    }

    private void Initialize(
      DateTime? queueTime,
      DateTime? startTime,
      DateTime? finishTime,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder? buildQueryOrder = null)
    {
      if (buildQueryOrder.HasValue)
      {
        Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder? nullable = buildQueryOrder;
        Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder1 = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending;
        if (!(nullable.GetValueOrDefault() == buildQueryOrder1 & nullable.HasValue))
        {
          nullable = buildQueryOrder;
          Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder2 = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending;
          if (!(nullable.GetValueOrDefault() == buildQueryOrder2 & nullable.HasValue))
          {
            nullable = buildQueryOrder;
            Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder3 = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeAscending;
            if (!(nullable.GetValueOrDefault() == buildQueryOrder3 & nullable.HasValue))
            {
              nullable = buildQueryOrder;
              Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder4 = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeDescending;
              if (!(nullable.GetValueOrDefault() == buildQueryOrder4 & nullable.HasValue))
              {
                nullable = buildQueryOrder;
                Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder5 = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeAscending;
                if (!(nullable.GetValueOrDefault() == buildQueryOrder5 & nullable.HasValue))
                {
                  nullable = buildQueryOrder;
                  Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder6 = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeDescending;
                  if (!(nullable.GetValueOrDefault() == buildQueryOrder6 & nullable.HasValue))
                    return;
                }
                this.Time = queueTime;
                return;
              }
            }
            this.Time = startTime;
            return;
          }
        }
      }
      this.Time = finishTime;
    }
  }
}
