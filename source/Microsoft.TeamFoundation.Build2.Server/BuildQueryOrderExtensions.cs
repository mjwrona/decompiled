// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildQueryOrderExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildQueryOrderExtensions
  {
    public static BuildQueryOrder ToServerBuildQueryOrder(this Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder)
    {
      switch (buildQueryOrder)
      {
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending:
          return BuildQueryOrder.FinishTimeAscending;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending:
          return BuildQueryOrder.FinishTimeDescending;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeDescending:
          return BuildQueryOrder.QueueTimeDescending;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeAscending:
          return BuildQueryOrder.QueueTimeAscending;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeDescending:
          return BuildQueryOrder.StartTimeDescending;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeAscending:
          return BuildQueryOrder.StartTimeAscending;
        default:
          throw new ArgumentException(nameof (buildQueryOrder));
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder ToWebApiBuildQueryOrder(
      this BuildQueryOrder buildQueryOrder,
      BuildStatus status = BuildStatus.Completed)
    {
      switch (buildQueryOrder)
      {
        case BuildQueryOrder.Ascending:
          switch (status)
          {
            case BuildStatus.InProgress:
              return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeAscending;
            case BuildStatus.NotStarted:
            case BuildStatus.All:
              return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeAscending;
            default:
              return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending;
          }
        case BuildQueryOrder.Descending:
          switch (status)
          {
            case BuildStatus.InProgress:
              return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeDescending;
            case BuildStatus.NotStarted:
            case BuildStatus.All:
              return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeDescending;
            default:
              return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending;
          }
        case BuildQueryOrder.QueueTimeDescending:
          return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeDescending;
        case BuildQueryOrder.QueueTimeAscending:
          return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeAscending;
        case BuildQueryOrder.StartTimeDescending:
          return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeDescending;
        case BuildQueryOrder.StartTimeAscending:
          return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeAscending;
        case BuildQueryOrder.FinishTimeDescending:
          return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending;
        case BuildQueryOrder.FinishTimeAscending:
          return Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending;
        default:
          throw new ArgumentException(string.Format("Unrecognized BuildQueryOrder in ToWebApiBuildQueryOrder: {0}", (object) buildQueryOrder));
      }
    }
  }
}
