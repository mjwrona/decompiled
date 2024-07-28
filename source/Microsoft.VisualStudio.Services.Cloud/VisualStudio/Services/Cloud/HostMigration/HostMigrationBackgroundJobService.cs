// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.HostMigrationBackgroundJobService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.HostMigration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class HostMigrationBackgroundJobService : 
    IHostMigrationBackgroundJobService,
    IVssFrameworkService
  {
    private static readonly string s_area = "HostMigration";
    private static readonly string s_layer = nameof (HostMigrationBackgroundJobService);
    private const string c_RetryJobRegistrationFailureSource = "RetryJobRegistrationFailureSource";
    private const string c_RetryJobRegistrationFailureTarget = "RetryJobRegistrationFailureTarget";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool HasRunningMigrationJobs(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry)
    {
      requestContext.CheckDeploymentRequestContext();
      List<ResourceMigrationJob> source;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        source = component.QueryResourceMigrationJobs(migrationEntry.MigrationId);
      return source.Any<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (x => x.Status != ResourceMigrationState.Complete && x.Status != ResourceMigrationState.Failed));
    }

    public void QueueStopRunningMigrationJobs(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry)
    {
      requestContext.CheckDeploymentRequestContext();
      List<ResourceMigrationJob> source;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        source = component.QueryResourceMigrationJobs(migrationEntry.MigrationId);
      if (!this.HasRunningMigrationJobs(requestContext, migrationEntry))
        return;
      ResourceMigrationJob job1 = source.SingleOrDefault<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (job => job.JobStage == MigrationJobStage.Rollback_StopMigrationJobs));
      bool flag;
      if (job1 == null)
      {
        flag = true;
      }
      else
      {
        ResourceMigrationJob resourceMigrationJob = this.UpdateMigrationJobState(requestContext, job1);
        if (resourceMigrationJob.Status == ResourceMigrationState.Complete || resourceMigrationJob.Status == ResourceMigrationState.Failed)
        {
          flag = true;
        }
        else
        {
          HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (QueueStopRunningMigrationJobs), "Received a request to stop running jobs, but a job is already running to stop jobs.  Allowing the existing job to continue execution");
          flag = false;
        }
      }
      if (!flag)
        return;
      this.QueueBackgroundMigrationJob(requestContext, new string[1]
      {
        ServicingOperationConstants.StopRunningMigrationJobs
      }, migrationEntry, true, MigrationJobStage.Rollback_StopMigrationJobs, 0);
    }

    public void QueueBackgroundMigrationJob(
      IVssRequestContext requestContext,
      string[] operations,
      IMigrationEntry migrationEntry,
      bool rollback,
      MigrationJobStage jobStage,
      int retries = 0)
    {
      requestContext.TraceEnter(15288071, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, nameof (QueueBackgroundMigrationJob));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        Guid jobId1 = Guid.NewGuid();
        string name = jobStage.ToString();
        this.RegisterResourceMigrationJob(requestContext, migrationEntry, jobId1, name, jobStage, retries);
        ServicingJobData servicingJobData1 = new ServicingJobData(operations);
        servicingJobData1.ServicingHostId = requestContext.ServiceHost.DeploymentServiceHost.InstanceId;
        servicingJobData1.OperationClass = "MigrateAccount";
        servicingJobData1.JobTitle = name;
        servicingJobData1.ServicingOptions = ServicingFlags.HostMustExist | ServicingFlags.NotAcquiringServicingLock;
        servicingJobData1.ServicingLocks = new TeamFoundationLockInfo[1]
        {
          new TeamFoundationLockInfo()
          {
            LockMode = TeamFoundationLockMode.Shared,
            LockName = "Servicing-" + migrationEntry.HostProperties.Id.ToString(),
            LockTimeout = -1
          }
        };
        ServicingJobData servicingJobData2 = servicingJobData1;
        servicingJobData2.ServicingTokens.Add(ServicingTokenConstants.MigrationId, migrationEntry.MigrationId.ToString("D"));
        servicingJobData2.ServicingTokens.Add(ServicingTokenConstants.HostId, migrationEntry.HostProperties.Id.ToString("D"));
        servicingJobData2.ServicingTokens.Add(ServicingTokenConstants.Rollback, XmlConvert.ToString(rollback));
        TeamFoundationServicingService service = requestContext.GetService<TeamFoundationServicingService>();
        if (rollback)
          HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (QueueBackgroundMigrationJob), string.Format("Queuing a rollback job with id: {0}", (object) jobId1));
        IVssRequestContext requestContext1 = requestContext;
        ServicingJobData servicingJobData3 = servicingJobData2;
        Guid? jobId2 = new Guid?(jobId1);
        service.QueueServicingJob(requestContext1, servicingJobData3, JobPriorityClass.AboveNormal, JobPriorityLevel.Normal, jobId2);
        requestContext.Trace(15288067, TraceLevel.Info, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, string.Format("Queued background migration job on stage {0} with ID {1}.\nMigrationId: {2}", (object) jobStage, (object) jobId1, (object) migrationEntry.MigrationId));
      }
      finally
      {
        requestContext.TraceLeave(15288072, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, nameof (QueueBackgroundMigrationJob));
      }
    }

    public void RegisterResourceMigrationJob(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry,
      Guid jobId,
      string name,
      MigrationJobStage jobStage,
      int retries)
    {
      requestContext.CheckDeploymentRequestContext();
      HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (RegisterResourceMigrationJob), string.Format("Registering a ResourceMigrationJob.  JobId: {0}.  Description:{1}", (object) jobId, (object) name));
      ResourceMigrationJob resourceMigrationJob = new ResourceMigrationJob()
      {
        MigrationId = migrationEntry.MigrationId,
        JobId = jobId,
        Name = name,
        Status = ResourceMigrationState.Queued,
        RetriesRemaining = retries,
        JobStage = jobStage
      };
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string injectionValue = HostMigrationInjectionUtil.CheckInjection<string>(requestContext, FrameworkServerConstants.MigrationFaultInjectionAction, string.Empty).InjectionValue;
      int num1 = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.HostMigrateJobRetryFailureTimeInterval, 5);
      int num2 = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.HostMigrateJobRetryFailureAttempts, 5);
      (bool IsInjectionEnabled1, int InjectionValue1) = HostMigrationInjectionUtil.CheckInjection<int>(requestContext, FrameworkServerConstants.HostMigrateJobRetryFailureTimeIntervalInjection, 5);
      if (IsInjectionEnabled1)
        num1 = InjectionValue1;
      (bool IsInjectionEnabled2, int InjectionValue2) = HostMigrationInjectionUtil.CheckInjection<int>(requestContext, FrameworkServerConstants.HostMigrateJobRetryFailureAttemptsInjection, 5);
      if (IsInjectionEnabled2)
        num2 = InjectionValue2;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
      {
        bool flag = false;
        int num3 = 0;
        while (!flag && num3 <= num2)
        {
          int num4 = num1 * num3;
          try
          {
            Thread.Sleep(TimeSpan.FromSeconds((double) num4));
            ++num3;
            if ((!injectionValue.Equals("RetryJobRegistrationFailureSource") || !name.Equals("Source_UpdateLocation")) && (!injectionValue.Equals("RetryJobRegistrationFailureTarget") || !name.Equals(TargetHostMigrationService.s_databasePartitionCopyJobName)))
            {
              component.CreateResourceMigrationJob(resourceMigrationJob);
            }
            else
            {
              requestContext.Trace(15288068, TraceLevel.Error, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, "Hit RegisterResourceMigrationJob test injection.");
              switch (injectionValue)
              {
                case "RetryJobRegistrationFailureSource":
                  HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (RegisterResourceMigrationJob), string.Format("Running Source RegisterResourceMigrationJob failure test this should never be in prod. Retry attempt: {0}, Sleeping for: {1} secs", (object) num3, (object) num4));
                  break;
                case "RetryJobRegistrationFailureTarget":
                  HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (RegisterResourceMigrationJob), string.Format("Running Target RegisterResourceMigrationJob failure test this should never be in prod. Retry attempt: {0}, Sleeping for: {1} secs", (object) num3, (object) num4));
                  break;
                default:
                  HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (RegisterResourceMigrationJob), string.Format("This should never be hit in prod or test. Retry attempt: {0}, Sleeping for: {1} secs", (object) num3, (object) (num1 * num3)), ServicingStepLogEntryKind.Error);
                  break;
              }
            }
            flag = component.QueryMigrationJobs(migrationEntry.MigrationId).Any<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (job => job.Name == name && job.JobId == jobId));
          }
          catch (Exception ex)
          {
            HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (RegisterResourceMigrationJob), string.Format("Unable to register or find resource migration job. JobId: {0}, Job Name: {1}, Migration Id: {2} on attempt {3}, sleeping for: {4} secs. Error: {5}", (object) jobId, (object) name, (object) migrationEntry.MigrationId, (object) num3, (object) num4, (object) ex.Message), ServicingStepLogEntryKind.Warning);
          }
        }
        if (!flag)
          throw new JobDefinitionNotFoundException(string.Format("Unable to register resource migration job, check migration logs for error. JobId: {0}, Job Name: {1}, Migration Id: {2}", (object) jobId, (object) name, (object) migrationEntry.MigrationId));
      }
      requestContext.Trace(15288069, TraceLevel.Info, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, string.Format("{0} ran and registered {1} with ID {2}.\nMigrationId: {3}", (object) nameof (RegisterResourceMigrationJob), (object) resourceMigrationJob.Name, (object) resourceMigrationJob.JobId, (object) resourceMigrationJob.MigrationId));
    }

    public void CheckBackgroundMigrationJobs(
      IVssRequestContext requestContext,
      Guid migrationId,
      MigrationJobStage? expectedJobStage,
      Action<Guid, string> onMigrationJobsFailed,
      Action<Guid> onMigrationJobsCompleted)
    {
      List<ResourceMigrationJob> source;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        source = component.QueryMigrationJobs(migrationId);
      List<ResourceMigrationJob> list = source.Where<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (x => x.JobStage != 0)).ToList<ResourceMigrationJob>();
      int num1 = 0;
      int num2 = 0;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ResourceMigrationJob job in list)
      {
        if (job.Status != ResourceMigrationState.Complete && job.Status != ResourceMigrationState.Failed && job.Status != ResourceMigrationState.Canceled)
        {
          ResourceMigrationJob resourceMigrationJob = this.UpdateMigrationJobState(requestContext, job);
          if (resourceMigrationJob.Status == ResourceMigrationState.Failed || resourceMigrationJob.Status == ResourceMigrationState.Canceled)
          {
            ++num1;
            string jobMessage = HostMigrationBackgroundJobService.GetJobMessage(requestContext, job);
            if (!string.IsNullOrWhiteSpace(jobMessage))
              stringBuilder.AppendLine(jobMessage);
          }
          else if (resourceMigrationJob.Status != ResourceMigrationState.Complete)
            ++num2;
        }
      }
      if (num1 > 0)
      {
        onMigrationJobsFailed(migrationId, stringBuilder.ToString());
      }
      else
      {
        if (num2 != 0)
          return;
        requestContext.Trace(15288070, TraceLevel.Info, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, string.Format("Found no running jobs and no failures.\nMigrationId: {0}", (object) migrationId));
        bool flag = false;
        if (expectedJobStage.HasValue && list.Where<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (x => x.JobStage == expectedJobStage.Value)).ToList<ResourceMigrationJob>().Count == 0)
        {
          onMigrationJobsFailed(migrationId, string.Format("Expected job in status {0} but no completed jobs were found", (object) expectedJobStage));
          flag = true;
        }
        if (flag)
          return;
        onMigrationJobsCompleted(migrationId);
      }
    }

    private ResourceMigrationJob UpdateMigrationJobState(
      IVssRequestContext requestContext,
      ResourceMigrationJob job)
    {
      requestContext.CheckDeploymentRequestContext();
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      if (service.QueryJobQueue(requestContext, job.JobId) != null)
      {
        job.Status = ResourceMigrationState.Queued;
      }
      else
      {
        TeamFoundationJobHistoryEntry foundationJobHistoryEntry = service.QueryLatestJobHistory(requestContext, job.JobId);
        if (foundationJobHistoryEntry == null)
        {
          requestContext.TraceAlways(6349080, TraceLevel.Warning, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, string.Format("History for job {0} corresponding to migration {1} was not found", (object) job.JobId, (object) job.MigrationId));
          job.Status = ResourceMigrationState.Failed;
        }
        else if (foundationJobHistoryEntry.Result == TeamFoundationJobResult.Succeeded)
        {
          job.Status = ResourceMigrationState.Complete;
        }
        else
        {
          requestContext.TraceAlways(6349082, TraceLevel.Info, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, string.Format("Job {0} corresponding to migration {1} failed with {2} retries remaining: {3}", (object) job.JobId, (object) job.MigrationId, (object) job.RetriesRemaining, (object) foundationJobHistoryEntry.ToString()));
          if (job.RetriesRemaining > 0)
          {
            --job.RetriesRemaining;
            job.Status = ResourceMigrationState.Queued;
            service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              job.JobId
            }, 3);
          }
          else
            job.Status = ResourceMigrationState.Failed;
        }
      }
      requestContext.Trace(6349083, TraceLevel.Info, HostMigrationBackgroundJobService.s_area, HostMigrationBackgroundJobService.s_layer, string.Format("Updating resource migration job entry for migration {0}: JobId={1}, Status={2}, RetriesRemaining={3}", (object) job.MigrationId, (object) job.JobId, (object) job.Status, (object) job.RetriesRemaining));
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        component.UpdateResourceMigrationJob(job);
      return job;
    }

    private static string GetJobMessage(IVssRequestContext requestContext, ResourceMigrationJob job) => requestContext.GetService<ITeamFoundationJobService>().QueryLatestJobHistory(requestContext, job.JobId)?.ResultMessage ?? string.Format("History for job {0} in stage {1} was not found", (object) job.JobId, (object) job.JobStage);
  }
}
