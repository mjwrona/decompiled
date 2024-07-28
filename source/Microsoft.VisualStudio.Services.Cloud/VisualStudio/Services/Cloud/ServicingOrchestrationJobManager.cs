// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServicingOrchestrationJobManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class ServicingOrchestrationJobManager
  {
    public abstract string Area { get; }

    protected virtual TimeSpan RetryInterval => ServicingOrchestrationConstants.DefaultRetryInterval;

    public virtual void StopJob(IVssRequestContext requestContext, Guid requestId)
    {
      TeamFoundationJobDefinition foundationJobDefinition = this.QueryJobDefinition(requestContext, requestId);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      foundationJobDefinition.EnabledState = TeamFoundationJobEnabledState.FullyDisabled;
      foundationJobDefinition.Schedule.Clear();
      service.UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      try
      {
        service.StopJob(requestContext, requestId);
      }
      catch (JobCannotBeStoppedException ex)
      {
      }
    }

    public void RequeueJob(IVssRequestContext requestContext, Guid requestId)
    {
      TeamFoundationJobDefinition jobDefinition = this.QueryJobDefinition(requestContext, requestId);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      if (jobDefinition.EnabledState == TeamFoundationJobEnabledState.FullyDisabled)
      {
        jobDefinition = this.ReenableJob(jobDefinition);
        service.UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          jobDefinition
        });
      }
      service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        jobDefinition.ToJobReference()
      });
    }

    protected TeamFoundationJobDefinition QueryJobDefinition(
      IVssRequestContext requestContext,
      Guid requestId)
    {
      TeamFoundationJobDefinition foundationJobDefinition = requestContext.GetService<ITeamFoundationJobService>().QueryJobDefinition(requestContext, requestId);
      if (foundationJobDefinition == null)
        this.ThrowRequestNotFoundException(requestId);
      if (foundationJobDefinition.Name.StartsWith(this.Area + "-"))
        return foundationJobDefinition;
      throw new ArgumentException(string.Format("{0} is not a {1} job", (object) requestId, (object) this.Area));
    }

    protected TeamFoundationJobDefinition ReenableJob(TeamFoundationJobDefinition jobDefinition)
    {
      jobDefinition.EnabledState = TeamFoundationJobEnabledState.Enabled;
      jobDefinition.Schedule.Clear();
      jobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        ScheduledTime = DateTime.UtcNow,
        Interval = (int) this.RetryInterval.TotalSeconds
      });
      return jobDefinition;
    }

    protected virtual void ThrowRequestNotFoundException(Guid requestId) => throw new ServicingOrchestrationEntryDoesNotExistException(requestId);
  }
}
