// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServicingOrchestrationJobManager`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class ServicingOrchestrationJobManager<T> : ServicingOrchestrationJobManager where T : ServicingOrchestrationRequest
  {
    private const string c_area = "ServicingOrchestration";
    private const string c_layer = "ServicingOrchestrationJobManager";

    protected virtual JobPriorityClass PriorityClass => JobPriorityClass.Normal;

    protected virtual JobPriorityLevel PriorityLevel => JobPriorityLevel.Normal;

    public static Guid GetServicingJobId(TeamFoundationJobDefinition jobDefinition) => ServicingOrchestrationJobManager<T>.DeserializeJobData(jobDefinition.Data).Last<T>().ServicingJobId;

    public static TRequest DeserializeRequest<TRequest>(TeamFoundationJobDefinition jobDefinition) where TRequest : T => (TRequest) (object) ServicingOrchestrationJobManager<T>.DeserializeJobData(jobDefinition.Data).Last<T>();

    public static IEnumerable<TRequest> GetRequests<TRequest>(
      TeamFoundationJobDefinition jobDefinition)
      where TRequest : T
    {
      return ServicingOrchestrationJobManager<T>.DeserializeJobData(jobDefinition.Data).OfType<TRequest>();
    }

    public void ValidateString(string str)
    {
      if (str == null)
        return;
      try
      {
        XmlConvert.VerifyXmlChars(str);
      }
      catch (XmlException ex)
      {
        throw new ArgumentException(ex.Message);
      }
    }

    public void ValidateRequest(IVssRequestContext requestContext, T request)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition jobDefinition = service.QueryJobDefinition(requestContext, request.RequestId);
      TeamFoundationJobQueueEntry queryJob = service.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        request.RequestId
      })[0];
      this.ValidateRequest(requestContext, request, jobDefinition, queryJob);
    }

    public Guid QueueJob(IVssRequestContext requestContext, T request)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition jobDefinition1 = service.QueryJobDefinition(requestContext, request.RequestId);
      TeamFoundationJobQueueEntry queryJob = service.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        request.RequestId
      })[0];
      this.ValidateRequest(requestContext, request, jobDefinition1, queryJob);
      TeamFoundationJobDefinition jobDefinition2 = this.GetJobDefinition(requestContext, jobDefinition1, request);
      service.UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        jobDefinition2
      });
      service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        jobDefinition2.ToJobReference()
      });
      Guid servicingJobId = ServicingOrchestrationJobManager<T>.GetServicingJobId(jobDefinition2);
      requestContext.TraceAlways(15080314, TraceLevel.Info, "ServicingOrchestration", nameof (ServicingOrchestrationJobManager<T>), string.Format("For request {0} ({1}) queued {2}", (object) request.RequestId, (object) request.JobPluginName, (object) servicingJobId));
      return servicingJobId;
    }

    public virtual ServicingOrchestrationRequestStatus GetJobStatus(
      IVssRequestContext requestContext,
      Guid requestId)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, requestId);
      if (foundationJobDefinition == null)
        this.ThrowRequestNotFoundException(requestId);
      Guid servicingJobId = ServicingOrchestrationJobManager<T>.GetServicingJobId(foundationJobDefinition);
      TeamFoundationJobQueueEntry queryJob = service.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        requestId
      })[0];
      ServicingJobInfo job = requestContext.GetService<ITeamFoundationServicingService>().GetServicingJobsInfo(requestContext, servicingJobId).SingleOrDefault<ServicingJobInfo>();
      ServicingOrchestrationRequestStatus jobStatus;
      if (queryJob != null)
      {
        ServicingOrchestrationRequestStatus orchestrationRequestStatus = ServicingOrchestrationJobManager<T>.ToServicingOrchestrationRequestStatus(requestContext, queryJob);
        jobStatus = job == null ? orchestrationRequestStatus : ServicingOrchestrationJobManager<T>.ToServicingOrchestrationRequestStatus(requestContext, job);
        jobStatus.Status = orchestrationRequestStatus.Status;
        jobStatus.StatusMessage = (string) null;
      }
      else
      {
        jobStatus = job == null ? ServicingOrchestrationJobManager<T>.ToServicingOrchestrationRequestStatus(requestContext, foundationJobDefinition) : ServicingOrchestrationJobManager<T>.ToServicingOrchestrationRequestStatus(requestContext, job);
        TeamFoundationJobHistoryEntry foundationJobHistoryEntry = service.QueryLatestJobHistory(requestContext, requestId);
        if (foundationJobHistoryEntry != null && foundationJobHistoryEntry.Result == TeamFoundationJobResult.Disabled)
        {
          List<TeamFoundationJobHistoryEntry> source = service.QueryJobHistory(requestContext, requestId);
          foundationJobHistoryEntry = source != null ? source.LastOrDefault<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (x => x.Result != TeamFoundationJobResult.Disabled)) : (TeamFoundationJobHistoryEntry) null;
        }
        if (foundationJobHistoryEntry != null)
        {
          if (job == null || foundationJobHistoryEntry.Result != TeamFoundationJobResult.Succeeded)
            jobStatus.Status = foundationJobHistoryEntry.Result == TeamFoundationJobResult.Succeeded ? ServicingOrchestrationStatus.Completed : ServicingOrchestrationStatus.Failed;
          if (string.IsNullOrEmpty(jobStatus.StatusMessage))
            jobStatus.StatusMessage = foundationJobHistoryEntry.ResultMessage;
        }
      }
      jobStatus.RequestId = requestId;
      jobStatus.ServicingJobId = servicingJobId;
      return jobStatus;
    }

    private void ValidateRequest(
      IVssRequestContext requestContext,
      T request,
      TeamFoundationJobDefinition jobDefinition,
      TeamFoundationJobQueueEntry jobQueue)
    {
      if (jobDefinition == null && !request.IsRootRequest)
        this.ThrowRequestNotFoundException(request.RequestId);
      if (!(request.JobPluginName != jobDefinition?.ExtensionName))
        return;
      if (jobDefinition != null && jobDefinition.EnabledState != TeamFoundationJobEnabledState.FullyDisabled)
        this.ThrowRequestInProgressException(request);
      if (jobQueue != null)
        this.ThrowRequestInProgressException(request);
      this.ValidateRequest(requestContext, request, jobDefinition);
    }

    internal TeamFoundationJobDefinition GetJobDefinition(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      T request)
    {
      List<T> requests = jobDefinition != null ? ServicingOrchestrationJobManager<T>.DeserializeJobData(jobDefinition.Data) : new List<T>();
      requests.Add(request);
      XmlNode data = ServicingOrchestrationJobManager<T>.SerializeJobData(requests);
      jobDefinition = new TeamFoundationJobDefinition(request.RequestId, string.Format("{0}-{1}", (object) this.Area, (object) request.ServicingJobId), request.JobPluginName, data)
      {
        PriorityClass = this.PriorityClass
      };
      TimeSpan timeSpan = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) ServicingOrchestrationConstants.JobRetryInterval(this.Area), true, this.RetryInterval);
      jobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        ScheduledTime = DateTime.UtcNow,
        Interval = (int) timeSpan.TotalSeconds,
        PriorityLevel = this.PriorityLevel
      });
      return jobDefinition;
    }

    protected internal static XmlNode SerializeJobData(List<T> requests) => TeamFoundationSerializationUtility.SerializeToXml((object) requests);

    protected internal static List<T> DeserializeJobData(XmlNode serializedJobData) => TeamFoundationSerializationUtility.Deserialize<List<T>>(serializedJobData);

    protected abstract void ValidateRequest(
      IVssRequestContext requestContext,
      T request,
      TeamFoundationJobDefinition jobDefinition);

    protected virtual void ThrowRequestInProgressException(T request) => throw new ServicingOrchestrationRequestInProgressException(string.Format("Expected job {0} to be stopped and fully disabled before scheduling next activity {1}", (object) request.RequestId, (object) request.GetType().Name));

    private static ServicingOrchestrationRequestStatus ToServicingOrchestrationRequestStatus(
      IVssRequestContext requestContext,
      ServicingJobInfo job)
    {
      return new ServicingOrchestrationRequestStatus()
      {
        Status = (ServicingOrchestrationStatus) job.JobStatus,
        CreatedDate = job.QueueTime,
        StartDate = job.StartTime.Value,
        CompletedDate = job.EndTime.Value,
        TotalStepCount = job.TotalStepCount,
        CompletedStepCount = job.CompletedStepCount,
        StatusMessage = string.Join(", ", requestContext.GetService<ITeamFoundationServicingService>().GetServicingDetails(requestContext, job.HostId, job.JobId, ServicingStepDetailFilterOptions.LastStepDetails).OfType<ServicingStepLogEntry>().Where<ServicingStepLogEntry>((Func<ServicingStepLogEntry, bool>) (x => x.EntryKind == ServicingStepLogEntryKind.Error)).Select<ServicingStepLogEntry, string>((Func<ServicingStepLogEntry, string>) (x => x.Message)))
      };
    }

    private static ServicingOrchestrationRequestStatus ToServicingOrchestrationRequestStatus(
      IVssRequestContext requestContext,
      TeamFoundationJobQueueEntry job)
    {
      return new ServicingOrchestrationRequestStatus()
      {
        Status = job.ExecutionStartTime >= job.QueueTime ? ServicingOrchestrationStatus.Running : ServicingOrchestrationStatus.Queued,
        CreatedDate = job.QueueTime,
        StartDate = job.ExecutionStartTime,
        TotalStepCount = -1,
        CompletedStepCount = 0
      };
    }

    private static ServicingOrchestrationRequestStatus ToServicingOrchestrationRequestStatus(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition job)
    {
      return new ServicingOrchestrationRequestStatus()
      {
        Status = ServicingOrchestrationStatus.Created,
        CreatedDate = DateTime.UtcNow,
        TotalStepCount = -1,
        CompletedStepCount = 0
      };
    }
  }
}
