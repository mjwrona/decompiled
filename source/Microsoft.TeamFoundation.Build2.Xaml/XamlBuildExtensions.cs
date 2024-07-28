// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.XamlBuildExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  public static class XamlBuildExtensions
  {
    public static Microsoft.TeamFoundation.Build.Server.BuildQueryOrder ToXamlBuildQueryOrder(
      this Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder queryOrder)
    {
      return queryOrder != Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending ? Microsoft.TeamFoundation.Build.Server.BuildQueryOrder.FinishTimeDescending : Microsoft.TeamFoundation.Build.Server.BuildQueryOrder.FinishTimeAscending;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildStatus ToBuildStatus(
      this QueueStatus xamlQueueStatus)
    {
      switch (xamlQueueStatus)
      {
        case QueueStatus.InProgress:
          return Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress;
        case QueueStatus.Retry:
        case QueueStatus.Queued:
          return Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted;
        case QueueStatus.Postponed:
          return Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed;
        case QueueStatus.Completed:
        case QueueStatus.Canceled:
          return Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
        case QueueStatus.All:
          return Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All;
        default:
          return Microsoft.TeamFoundation.Build.WebApi.BuildStatus.None;
      }
    }

    public static Microsoft.TeamFoundation.Build.Server.BuildReason ToXamlBuildReason(
      this Microsoft.TeamFoundation.Build.WebApi.BuildReason reason)
    {
      switch (reason)
      {
        case Microsoft.TeamFoundation.Build.WebApi.BuildReason.Manual:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.Manual;
        case Microsoft.TeamFoundation.Build.WebApi.BuildReason.IndividualCI:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.IndividualCI;
        case Microsoft.TeamFoundation.Build.WebApi.BuildReason.BatchedCI:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.BatchedCI;
        case Microsoft.TeamFoundation.Build.WebApi.BuildReason.Schedule:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.Schedule;
        case Microsoft.TeamFoundation.Build.WebApi.BuildReason.UserCreated:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.UserCreated;
        case Microsoft.TeamFoundation.Build.WebApi.BuildReason.ValidateShelveset:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.ValidateShelveset;
        case Microsoft.TeamFoundation.Build.WebApi.BuildReason.CheckInShelveset:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.CheckInShelveset;
        case Microsoft.TeamFoundation.Build.WebApi.BuildReason.Triggered:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.Triggered;
        default:
          return Microsoft.TeamFoundation.Build.Server.BuildReason.All;
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildReason ToBuildReason(
      this Microsoft.TeamFoundation.Build.Server.BuildReason reason)
    {
      Microsoft.TeamFoundation.Build.WebApi.BuildReason buildReason = Microsoft.TeamFoundation.Build.WebApi.BuildReason.None;
      if ((Microsoft.TeamFoundation.Build.Server.BuildReason.BatchedCI & reason) > Microsoft.TeamFoundation.Build.Server.BuildReason.None)
        buildReason |= Microsoft.TeamFoundation.Build.WebApi.BuildReason.BatchedCI;
      if ((Microsoft.TeamFoundation.Build.Server.BuildReason.IndividualCI & reason) > Microsoft.TeamFoundation.Build.Server.BuildReason.None)
        buildReason |= Microsoft.TeamFoundation.Build.WebApi.BuildReason.IndividualCI;
      if ((Microsoft.TeamFoundation.Build.Server.BuildReason.CheckInShelveset & reason) > Microsoft.TeamFoundation.Build.Server.BuildReason.None)
        buildReason |= Microsoft.TeamFoundation.Build.WebApi.BuildReason.CheckInShelveset;
      if ((Microsoft.TeamFoundation.Build.Server.BuildReason.Manual & reason) > Microsoft.TeamFoundation.Build.Server.BuildReason.None)
        buildReason |= Microsoft.TeamFoundation.Build.WebApi.BuildReason.Manual;
      if ((Microsoft.TeamFoundation.Build.Server.BuildReason.Schedule & reason) > Microsoft.TeamFoundation.Build.Server.BuildReason.None)
        buildReason |= Microsoft.TeamFoundation.Build.WebApi.BuildReason.Schedule;
      else if ((Microsoft.TeamFoundation.Build.Server.BuildReason.ScheduleForced & reason) > Microsoft.TeamFoundation.Build.Server.BuildReason.None)
        buildReason |= Microsoft.TeamFoundation.Build.WebApi.BuildReason.Schedule;
      if ((Microsoft.TeamFoundation.Build.Server.BuildReason.UserCreated & reason) > Microsoft.TeamFoundation.Build.Server.BuildReason.None)
        buildReason |= Microsoft.TeamFoundation.Build.WebApi.BuildReason.UserCreated;
      if ((Microsoft.TeamFoundation.Build.Server.BuildReason.ValidateShelveset & reason) > Microsoft.TeamFoundation.Build.Server.BuildReason.None)
        buildReason |= Microsoft.TeamFoundation.Build.WebApi.BuildReason.ValidateShelveset;
      return buildReason;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.QueuePriority ToQueuePriority(
      this Microsoft.TeamFoundation.Build.Server.QueuePriority xamlQueuePriority)
    {
      switch (xamlQueuePriority)
      {
        case Microsoft.TeamFoundation.Build.Server.QueuePriority.High:
          return Microsoft.TeamFoundation.Build.WebApi.QueuePriority.High;
        case Microsoft.TeamFoundation.Build.Server.QueuePriority.AboveNormal:
          return Microsoft.TeamFoundation.Build.WebApi.QueuePriority.AboveNormal;
        case Microsoft.TeamFoundation.Build.Server.QueuePriority.BelowNormal:
          return Microsoft.TeamFoundation.Build.WebApi.QueuePriority.BelowNormal;
        case Microsoft.TeamFoundation.Build.Server.QueuePriority.Low:
          return Microsoft.TeamFoundation.Build.WebApi.QueuePriority.Low;
        default:
          return Microsoft.TeamFoundation.Build.WebApi.QueuePriority.Normal;
      }
    }

    public static bool IsStarted(this Microsoft.TeamFoundation.Build.Server.BuildStatus buildStatus) => (buildStatus & Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress) == Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress || (buildStatus & Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed) == Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed || (buildStatus & Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded) == Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded || (buildStatus & Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped) == Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped || (buildStatus & Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded) == Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded;

    public static XamlBuildReference ToReference(
      this BuildDetail buildDetail,
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      int buildId = int.Parse(LinkingUtilities.DecodeUri(buildDetail.Uri).ToolSpecificId, (IFormatProvider) CultureInfo.InvariantCulture);
      return XamlBuildExtensions.GetReference(requestContext, projectInfo.Id, buildId);
    }

    public static XamlBuildReference GetReference(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      return new XamlBuildReference()
      {
        Id = buildId,
        Url = service.GetBuildRestUrl(requestContext, projectId, buildId)
      };
    }

    public static Microsoft.TeamFoundation.Build.Server.BuildStatus GetXamlBuildStatus(
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? buildStatus,
      BuildResult? buildResult)
    {
      Microsoft.TeamFoundation.Build.Server.BuildStatus xamlBuildStatus = Microsoft.TeamFoundation.Build.Server.BuildStatus.None;
      if (!buildStatus.HasValue)
        buildStatus = !buildResult.HasValue || buildResult.Value == BuildResult.None ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed);
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable1 = buildStatus;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable2 = nullable1.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable1.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling;
      if (nullable2.GetValueOrDefault() == buildStatus1 & nullable2.HasValue)
        xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable3 = buildStatus;
      nullable2 = nullable3.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable3.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus2 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress;
      if (nullable2.GetValueOrDefault() == buildStatus2 & nullable2.HasValue)
        xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable4 = buildStatus;
      nullable2 = nullable4.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable4.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus3 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted;
      if (nullable2.GetValueOrDefault() == buildStatus3 & nullable2.HasValue)
        xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable5 = buildStatus;
      nullable2 = nullable5.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable5.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus4 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed;
      if (nullable2.GetValueOrDefault() == buildStatus4 & nullable2.HasValue)
        xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable6 = buildStatus;
      nullable2 = nullable6.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable6.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus5 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All;
      if (nullable2.GetValueOrDefault() == buildStatus5 & nullable2.HasValue)
        xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.All;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable7 = buildStatus;
      nullable2 = nullable7.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable7.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus6 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
      if (nullable2.GetValueOrDefault() == buildStatus6 & nullable2.HasValue)
      {
        if (!buildResult.HasValue)
        {
          xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed | Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped;
        }
        else
        {
          if ((buildResult.Value & BuildResult.Canceled) == BuildResult.Canceled)
            xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped;
          if ((buildResult.Value & BuildResult.Failed) == BuildResult.Failed)
            xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed;
          if ((buildResult.Value & BuildResult.PartiallySucceeded) == BuildResult.PartiallySucceeded)
            xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded;
          if ((buildResult.Value & BuildResult.Succeeded) == BuildResult.Succeeded)
            xamlBuildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded;
        }
      }
      return xamlBuildStatus;
    }

    public static QueueStatus GetXamlQueueStatus(Microsoft.TeamFoundation.Build.WebApi.BuildStatus? buildStatus, BuildResult? buildResult)
    {
      QueueStatus xamlQueueStatus = QueueStatus.None;
      buildStatus = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?((Microsoft.TeamFoundation.Build.WebApi.BuildStatus) ((int) buildStatus ?? 47));
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable1 = buildStatus;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable2 = nullable1.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable1.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress;
      if (nullable2.GetValueOrDefault() == buildStatus1 & nullable2.HasValue)
        xamlQueueStatus |= QueueStatus.InProgress;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable3 = buildStatus;
      nullable2 = nullable3.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable3.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus2 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling;
      if (nullable2.GetValueOrDefault() == buildStatus2 & nullable2.HasValue)
        xamlQueueStatus |= QueueStatus.InProgress;
      nullable3 = buildStatus;
      nullable2 = nullable3.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable3.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus3 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed;
      if (nullable2.GetValueOrDefault() == buildStatus3 & nullable2.HasValue)
        xamlQueueStatus |= QueueStatus.Postponed;
      nullable3 = buildStatus;
      nullable2 = nullable3.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable3.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus4 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted;
      if (nullable2.GetValueOrDefault() == buildStatus4 & nullable2.HasValue)
        xamlQueueStatus |= QueueStatus.Queued;
      nullable3 = buildStatus;
      nullable2 = nullable3.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(nullable3.GetValueOrDefault() & Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus5 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
      if (nullable2.GetValueOrDefault() == buildStatus5 & nullable2.HasValue)
      {
        if (!buildResult.HasValue || buildResult.Value == BuildResult.None)
        {
          xamlQueueStatus |= QueueStatus.Completed | QueueStatus.Canceled;
        }
        else
        {
          BuildResult? nullable4 = buildResult;
          BuildResult? nullable5 = nullable4.HasValue ? new BuildResult?(nullable4.GetValueOrDefault() & BuildResult.Canceled) : new BuildResult?();
          BuildResult buildResult1 = BuildResult.Canceled;
          if (nullable5.GetValueOrDefault() == buildResult1 & nullable5.HasValue)
            xamlQueueStatus |= QueueStatus.Canceled;
          nullable4 = buildResult;
          nullable5 = nullable4.HasValue ? new BuildResult?(nullable4.GetValueOrDefault() & BuildResult.Failed) : new BuildResult?();
          BuildResult buildResult2 = BuildResult.Failed;
          if (nullable5.GetValueOrDefault() == buildResult2 & nullable5.HasValue)
            xamlQueueStatus |= QueueStatus.Completed;
          nullable4 = buildResult;
          nullable5 = nullable4.HasValue ? new BuildResult?(nullable4.GetValueOrDefault() & BuildResult.PartiallySucceeded) : new BuildResult?();
          BuildResult buildResult3 = BuildResult.PartiallySucceeded;
          if (nullable5.GetValueOrDefault() == buildResult3 & nullable5.HasValue)
            xamlQueueStatus |= QueueStatus.Completed;
          nullable4 = buildResult;
          nullable5 = nullable4.HasValue ? new BuildResult?(nullable4.GetValueOrDefault() & BuildResult.Succeeded) : new BuildResult?();
          BuildResult buildResult4 = BuildResult.Succeeded;
          if (nullable5.GetValueOrDefault() == buildResult4 & nullable5.HasValue)
            xamlQueueStatus |= QueueStatus.Completed;
        }
      }
      return xamlQueueStatus;
    }

    public static Microsoft.TeamFoundation.Build.Server.QueryDeletedOption GetXamlQueryDeletedOption(
      Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption queryDeletedOption)
    {
      switch (queryDeletedOption)
      {
        case Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.IncludeDeleted:
          return Microsoft.TeamFoundation.Build.Server.QueryDeletedOption.IncludeDeleted;
        case Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.OnlyDeleted:
          return Microsoft.TeamFoundation.Build.Server.QueryDeletedOption.OnlyDeleted;
        default:
          return Microsoft.TeamFoundation.Build.Server.QueryDeletedOption.ExcludeDeleted;
      }
    }

    public static void DecodeXamlBuildStatus(
      Microsoft.TeamFoundation.Build.Server.BuildStatus xamlStatus,
      out Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus,
      out BuildResult? buildResult)
    {
      switch (xamlStatus)
      {
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.None:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.None;
          buildResult = new BuildResult?();
          break;
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress;
          buildResult = new BuildResult?();
          break;
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
          buildResult = new BuildResult?(BuildResult.Succeeded);
          break;
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
          buildResult = new BuildResult?(BuildResult.PartiallySucceeded);
          break;
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
          buildResult = new BuildResult?(BuildResult.Failed);
          break;
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
          buildResult = new BuildResult?(BuildResult.Canceled);
          break;
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted;
          buildResult = new BuildResult?();
          break;
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.All:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All;
          buildResult = new BuildResult?(BuildResult.Succeeded);
          break;
        default:
          buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.None;
          buildResult = new BuildResult?();
          break;
      }
    }
  }
}
