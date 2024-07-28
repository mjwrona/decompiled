// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ResourceUtilizationSettingsCleanupService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ResourceUtilizationSettingsCleanupService : IVssFrameworkService
  {
    private const int c_daysToLookForwardToScheduleDeletionJob = 7;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal (int, int, DateTime) DeleteExpiredThresholdsAndQueueJobForFutureExpirations(
      IVssRequestContext requestContext,
      bool fireThresholdChangedNotification = true)
    {
      int num1 = 0;
      int num2 = 0;
      DateTime dateTime1 = DateTime.MaxValue;
      List<RUThreshold> thresholds;
      using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
        component.ReadRURulesAndThresholds(out List<RUMacro> _, out thresholds);
      foreach (RUThreshold ruThreshold in thresholds.Where<RUThreshold>((Func<RUThreshold, bool>) (t =>
      {
        DateTime? expiration = t.Expiration;
        DateTime minValue = DateTime.MinValue;
        return expiration.HasValue && expiration.GetValueOrDefault() > minValue;
      })))
      {
        DateTime? expiration = ruThreshold.Expiration;
        DateTime utcNow = DateTime.UtcNow;
        if ((expiration.HasValue ? (expiration.GetValueOrDefault() < utcNow ? 1 : 0) : 0) != 0)
        {
          string str = string.Format("HostId={0}; HostType={1}; Threshold={2}[{3}];", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.HostType, (object) ruThreshold.RuleName, (object) ruThreshold.Entity);
          TeamFoundationTracingService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTracingService>();
          try
          {
            using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
            {
              int num3 = component.DeleteRUThreshold(ruThreshold.RuleName, ruThreshold.Entity);
              num1 += num3;
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(522304016, "ResourceUtilizationService", "Service", ex);
            str = "Exception=[" + ex.Message + "]; " + str;
          }
          string dataFeed = "Command=DeleteExpiredRUThreshold; " + str;
          service.TraceResourceUtilization(requestContext, 3, dataFeed);
        }
        else
        {
          ++num2;
          expiration = ruThreshold.Expiration;
          DateTime dateTime2 = dateTime1;
          dateTime1 = (expiration.HasValue ? (expiration.GetValueOrDefault() < dateTime2 ? 1 : 0) : 0) != 0 ? ruThreshold.Expiration.Value : dateTime1;
        }
      }
      if (fireThresholdChangedNotification && num1 > 0)
        requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, ResourceUtilizationConstants.RulesOrThresholdsChanged, string.Empty);
      if (num2 > 0 && (dateTime1 - DateTime.UtcNow).Days < 7)
      {
        TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
        TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(ResourceUtilizationConstants.DeleteExpiredThresholdsJobId, "Resource Utilization delete expired thresholds on a host", "Microsoft.VisualStudio.Services.Cloud.Extensions.ResourceUtilizationDeleteExpiredThresholdsJob");
        foundationJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          ScheduledTime = dateTime1
        });
        IVssRequestContext requestContext1 = requestContext;
        service.UpdateJobDefinitions(requestContext1, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
        {
          foundationJobDefinition
        });
      }
      return (num1, num2, dateTime1);
    }
  }
}
