// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobTemplateService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationJobTemplateService : 
    ITeamFoundationJobTemplateService,
    IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ITeamFoundationJobDefinitionTemplate> GetJobTemplates(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (IEnumerable<ITeamFoundationJobDefinitionTemplate>) vssRequestContext.GetService<JobTemplateCacheService>().GetJobTemplates(vssRequestContext).Where<KeyValuePair<Guid, TeamFoundationJobDefinitionTemplate>>((Func<KeyValuePair<Guid, TeamFoundationJobDefinitionTemplate>, bool>) (x => this.IsApplicable(requestContext, (ITeamFoundationJobDefinitionTemplate) x.Value))).Select<KeyValuePair<Guid, TeamFoundationJobDefinitionTemplate>, TeamFoundationJobDefinitionTemplate>((Func<KeyValuePair<Guid, TeamFoundationJobDefinitionTemplate>, TeamFoundationJobDefinitionTemplate>) (x => x.Value));
    }

    public bool TryGetJobTemplate(
      IVssRequestContext requestContext,
      Guid jobId,
      out ITeamFoundationJobDefinitionTemplate jobTemplate)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationJobDefinitionTemplate jobTemplate1;
      if (vssRequestContext.GetService<JobTemplateCacheService>().GetJobTemplates(vssRequestContext).TryGetValue(jobId, out jobTemplate1) && this.IsApplicable(requestContext, (ITeamFoundationJobDefinitionTemplate) jobTemplate1))
      {
        jobTemplate = (ITeamFoundationJobDefinitionTemplate) jobTemplate1;
        return true;
      }
      jobTemplate = (ITeamFoundationJobDefinitionTemplate) null;
      return false;
    }

    public TeamFoundationJobDefinition CreateJobDefinition(
      IVssRequestContext requestContext,
      ITeamFoundationJobDefinitionTemplate jobTemplate)
    {
      TeamFoundationJobDefinition jobDefinition = new TeamFoundationJobDefinition()
      {
        IsTemplateJob = true
      };
      jobDefinition.JobId = jobTemplate.JobId;
      jobDefinition.Name = jobTemplate.JobName;
      jobDefinition.ExtensionName = jobTemplate.PluginType;
      jobDefinition.Data = jobTemplate.JobData;
      jobDefinition.EnabledState = jobTemplate.EnabledState;
      jobDefinition.Flags = (TeamFoundationJobDefinitionFlags) jobTemplate.Flags;
      jobDefinition.PriorityClass = jobTemplate.PriorityClass;
      Guid guid = requestContext.ServiceHost.InstanceId;
      int hashCode1 = guid.GetHashCode();
      guid = jobTemplate.JobId;
      int hashCode2 = guid.GetHashCode();
      int num1 = hashCode1 ^ hashCode2;
      foreach (TeamFoundationJobScheduleTemplate schedule in jobTemplate.Schedules)
      {
        TeamFoundationJobSchedule foundationJobSchedule1 = new TeamFoundationJobSchedule()
        {
          JobId = jobDefinition.JobId
        };
        TimeSpan timeSpan;
        int num2;
        if (!(schedule.StaggerInterval == TimeSpan.Zero))
        {
          int num3 = num1;
          timeSpan = schedule.StaggerInterval;
          int totalSeconds = (int) timeSpan.TotalSeconds;
          num2 = num3 % totalSeconds;
        }
        else
          num2 = 0;
        int num4 = num2;
        foundationJobSchedule1.ScheduledTime = schedule.BaseTime.AddSeconds((double) num4);
        TeamFoundationJobSchedule foundationJobSchedule2 = foundationJobSchedule1;
        timeSpan = schedule.ScheduleInterval;
        int totalSeconds1 = (int) timeSpan.TotalSeconds;
        foundationJobSchedule2.Interval = totalSeconds1;
        foundationJobSchedule1.TimeZoneId = schedule.TimeZoneId;
        foundationJobSchedule1.PriorityLevel = schedule.PriorityLevel;
        jobDefinition.Schedule.Add(foundationJobSchedule1);
      }
      return jobDefinition;
    }

    private bool IsApplicable(
      IVssRequestContext requestContext,
      ITeamFoundationJobDefinitionTemplate jobTemplate)
    {
      TeamFoundationHostType foundationHostType = requestContext.ServiceHost.HostType;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        foundationHostType = TeamFoundationHostType.Deployment;
      bool flag = (jobTemplate.HostType & foundationHostType) != 0;
      if (requestContext.IsVirtualServiceHost())
        flag &= jobTemplate.Flags.HasFlag((Enum) TeamFoundationJobDefinitionTemplateFlags.SupportsVirtualServiceHosts);
      return flag;
    }
  }
}
