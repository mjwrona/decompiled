// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobRequestContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobRequestContext : 
    VssRequestContext,
    IJobRequestContext,
    IVssRequestContext,
    IDisposable
  {
    private Guid m_jobId;
    private Guid m_agentId;
    private TeamFoundationJobState m_jobState;
    private object m_jobStateLock = new object();
    private static readonly string s_Area = "JobAgent";
    private static readonly string s_Layer = nameof (JobRequestContext);

    public JobRequestContext(
      IVssServiceHost serviceHost,
      RequestContextType type,
      TeamFoundationJobService jobService,
      TeamFoundationJobQueueEntry jobQueue,
      LockHelper helper,
      TimeSpan timeout)
      : base(serviceHost, type, helper, timeout, (VssRequestContext) null)
    {
      this.JobService = jobService;
      this.m_jobId = jobQueue.JobId;
      this.m_uniqueIdentifier = jobQueue.JobId;
      this.m_agentId = jobQueue.AgentId;
      this.ServiceName = "Team Foundation JobAgent";
      this.m_userAgent = JobApplication.UserAgent;
      this.m_jobState = TeamFoundationJobState.Running;
      this.IsTracked = true;
      this.ResetActivityId();
    }

    public Guid JobId => this.m_jobId;

    private TeamFoundationJobState JobState
    {
      get => this.m_jobState;
      set
      {
        lock (this.m_jobStateLock)
        {
          this.m_jobState = value;
          Monitor.PulseAll(this.m_jobStateLock);
        }
      }
    }

    private TeamFoundationJobService JobService { get; set; }

    public override void Cancel(string reason) => this.Cancel(reason, true);

    public void Cancel(string reason, bool tryUpdateQueueState)
    {
      TeamFoundationTracingService.TraceRaw(1452, TraceLevel.Info, JobRequestContext.s_Area, JobRequestContext.s_Layer, string.Format("Request Activity Id {0}, Context Id {1} has been canceled", (object) this.ActivityId, (object) this.ContextId));
      lock (this.m_jobStateLock)
      {
        if (this.JobState != TeamFoundationJobState.Stopping)
        {
          if (tryUpdateQueueState)
          {
            try
            {
              IVssServiceHost serviceHost = this.ServiceHost;
              if (serviceHost != null)
              {
                using (IVssRequestContext systemContext = serviceHost.DeploymentServiceHost.CreateSystemContext())
                  this.JobService.ChangeJobState(systemContext, this.m_agentId, serviceHost.InstanceId, this.m_jobId, TeamFoundationJobState.Stopping);
              }
            }
            catch (TeamFoundationServiceException ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(1451, JobRequestContext.s_Area, JobRequestContext.s_Layer, (Exception) ex);
            }
          }
          this.JobState = TeamFoundationJobState.Stopping;
        }
      }
      base.Cancel(reason);
    }

    public override bool IsCanceled
    {
      get
      {
        MethodInformation method = this.Method;
        return method != null && method.InProgress && this.JobState == TeamFoundationJobState.Stopping;
      }
    }
  }
}
