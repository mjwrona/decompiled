// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OnPrem.AnalyticsStateService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Analytics.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Cleanup;
using Microsoft.VisualStudio.Services.Analytics.DataQuality;
using Microsoft.VisualStudio.Services.Analytics.Job;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OnPrem
{
  public class AnalyticsStateService : IAnalyticsStateService, IVssFrameworkService
  {
    private const string FeatureStateChangedDateRegistryPath = "/Service/Analytics/Settings/AnalyticsStateChangedDate";
    private static readonly RegistryQuery FeatureStateQuery = new RegistryQuery("/Service/Analytics/Settings/AnalyticsState");
    private static readonly RegistryQuery FeatureStateChangedDateQuery = new RegistryQuery("/Service/Analytics/Settings/AnalyticsStateChangedDate");
    private static readonly TimeSpan LeaseTime = TimeSpan.FromSeconds(30.0);
    private static readonly string LeaseName = "AnalyticsStateService.SetFeatureState";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public AnalyticsState GetState(IVssRequestContext requestContext, bool bypassCache = false)
    {
      this.ValidateOnPrem(requestContext);
      if (bypassCache)
      {
        this.ValidateOnPrem(requestContext);
        AnalyticsState state = requestContext.GetService<ISqlRegistryService>().GetValue<AnalyticsState>(requestContext, in AnalyticsStateService.FeatureStateQuery, AnalyticsState.Disabled);
        return this.InterpretState(requestContext, state);
      }
      AnalyticsState state1 = requestContext.GetService<IVssRegistryService>().GetValue<AnalyticsState>(requestContext, in AnalyticsStateService.FeatureStateQuery, AnalyticsState.Disabled);
      return this.InterpretState(requestContext, state1);
    }

    private AnalyticsState InterpretState(IVssRequestContext requestContext, AnalyticsState state)
    {
      if (state != AnalyticsState.Enabled)
        return state;
      return requestContext.GetService<IAnalyticsService>().IsModelReady(requestContext) ? AnalyticsState.Enabled : AnalyticsState.Preparing;
    }

    public DateTimeOffset GetChangedDate(IVssRequestContext requestContext)
    {
      this.ValidateOnPrem(requestContext);
      return (DateTimeOffset) requestContext.GetService<IVssRegistryService>().GetValue<DateTime>(requestContext, in AnalyticsStateService.FeatureStateChangedDateQuery, DateTime.SpecifyKind(Microsoft.VisualStudio.Services.Analytics.Constants.BeginningOfTimeDateTime, DateTimeKind.Utc));
    }

    public void MigrateFromExtension(
      IVssRequestContext requestContext,
      bool isInstalled,
      bool isDisabled)
    {
      this.ValidateOnPrem(requestContext);
      using (requestContext.GetService<ILeaseService>().AcquireLease(requestContext, AnalyticsStateService.LeaseName, AnalyticsStateService.LeaseTime, AnalyticsStateService.LeaseTime, true))
      {
        if (requestContext.GetService<ISqlRegistryService>().GetValue<AnalyticsState?>(requestContext, in AnalyticsStateService.FeatureStateQuery, new AnalyticsState?()).HasValue)
          throw new AnalyticsStateInvalidTransitionException("Tried to migrate state from extension when state already initialized.");
        if (isInstalled)
        {
          if (isDisabled)
            this.SetState(requestContext, AnalyticsState.Paused);
          else
            this.SetState(requestContext, AnalyticsState.Enabled);
          this.EnsureJobSchedules(requestContext);
        }
        else
          this.SetState(requestContext, AnalyticsState.Disabled);
      }
    }

    public void Enable(IVssRequestContext requestContext)
    {
      this.ValidateOnPrem(requestContext);
      using (requestContext.GetService<ILeaseService>().AcquireLease(requestContext, AnalyticsStateService.LeaseName, AnalyticsStateService.LeaseTime, AnalyticsStateService.LeaseTime, true))
      {
        AnalyticsState state = this.GetState(requestContext, true);
        switch (state)
        {
          case AnalyticsState.Disabled:
            requestContext.GetService<IAnalyticsCleanupService>().CleanupOnPrem(requestContext);
            this.SetState(requestContext, AnalyticsState.Enabled);
            this.EnsureJobSchedules(requestContext);
            this.EnsureModelReady(requestContext);
            break;
          case AnalyticsState.Enabled:
            break;
          case AnalyticsState.Paused:
            this.SetState(requestContext, AnalyticsState.Enabled);
            this.EnsureJobSchedules(requestContext);
            break;
          case AnalyticsState.Preparing:
            break;
          default:
            throw new AnalyticsStateInvalidTransitionException("Invalid state transition.", state, AnalyticsState.Paused);
        }
      }
    }

    public void Pause(IVssRequestContext requestContext)
    {
      this.ValidateOnPrem(requestContext);
      using (requestContext.GetService<ILeaseService>().AcquireLease(requestContext, AnalyticsStateService.LeaseName, AnalyticsStateService.LeaseTime, AnalyticsStateService.LeaseTime, true))
      {
        AnalyticsState state = this.GetState(requestContext, true);
        switch (state)
        {
          case AnalyticsState.Enabled:
          case AnalyticsState.Preparing:
            this.SetState(requestContext, AnalyticsState.Paused);
            break;
          case AnalyticsState.Paused:
            break;
          default:
            throw new AnalyticsStateInvalidTransitionException("Invalid state transition.", state, AnalyticsState.Paused);
        }
      }
    }

    public void Delete(IVssRequestContext requestContext)
    {
      this.ValidateOnPrem(requestContext);
      using (requestContext.GetService<ILeaseService>().AcquireLease(requestContext, AnalyticsStateService.LeaseName, AnalyticsStateService.LeaseTime, AnalyticsStateService.LeaseTime, true))
      {
        AnalyticsState state = this.GetState(requestContext, true);
        switch (state)
        {
          case AnalyticsState.Enabled:
          case AnalyticsState.Paused:
          case AnalyticsState.Preparing:
            this.SetState(requestContext, AnalyticsState.Deleting);
            this.DeleteAnalyticsData(requestContext);
            break;
          case AnalyticsState.Deleting:
            break;
          default:
            throw new AnalyticsStateInvalidTransitionException("Invalid state transition.", state, AnalyticsState.Deleting);
        }
      }
    }

    public void Disable(IVssRequestContext requestContext)
    {
      this.ValidateOnPrem(requestContext);
      using (requestContext.GetService<ILeaseService>().AcquireLease(requestContext, AnalyticsStateService.LeaseName, AnalyticsStateService.LeaseTime, AnalyticsStateService.LeaseTime, true))
      {
        AnalyticsState state = this.GetState(requestContext, true);
        switch (state)
        {
          case AnalyticsState.Disabled:
            break;
          case AnalyticsState.Deleting:
            ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
            if (service.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              Microsoft.VisualStudio.Services.Analytics.Constants.AnalyticsCleanupJobId
            }).Where<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (q => q.State != TeamFoundationJobState.Running && !q.QueuedReasons.HasFlag((Enum) TeamFoundationJobQueuedReasons.Scheduled))).Count<TeamFoundationJobQueueEntry>() > 0)
            {
              service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
              {
                Microsoft.VisualStudio.Services.Analytics.Constants.AnalyticsCleanupJobId
              });
              throw new InvalidOperationException("Cannot transition from deleting to disable when a cleanup job is still pending.");
            }
            this.SetState(requestContext, AnalyticsState.Disabled);
            break;
          default:
            throw new AnalyticsStateInvalidTransitionException("Invalid state transition.", state, AnalyticsState.Disabled);
        }
      }
    }

    private void DeleteAnalyticsData(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      Microsoft.VisualStudio.Services.Analytics.Constants.AnalyticsCleanupJobId
    });

    private void ValidateOnPrem(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException("SetFeatureState is only valid in on-premises deployments.");
    }

    private void EnsureModelReady(IVssRequestContext requestContext) => requestContext.GetService<IDataQualityService>().InitializeModelReady(requestContext);

    private void EnsureJobSchedules(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[2]
      {
        AnalyticsSdkConstants.AnalyticsMaintainStagingSchedulesJobGuid,
        AnalyticsSdkConstants.AnalyticsCalendarUpdateJobGuid
      });
      requestContext.GetService<IAnalyticsJobService>().UpdateAnalyticsJobSchedules(requestContext);
    }

    private void SetState(IVssRequestContext requestContext, AnalyticsState state)
    {
      ISqlRegistryService service = requestContext.GetService<ISqlRegistryService>();
      service.SetValue<AnalyticsState>(requestContext, "/Service/Analytics/Settings/AnalyticsState", state);
      service.SetValue<DateTimeOffset>(requestContext, "/Service/Analytics/Settings/AnalyticsStateChangedDate", DateTimeOffset.UtcNow);
    }
  }
}
