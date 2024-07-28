// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.LimitedConcurrencyAnalyticsJobCommand
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public abstract class LimitedConcurrencyAnalyticsJobCommand : AnalyticsJobCommand
  {
    private const string EnableLiveJobConcurrencyLimitPathTemplate = "/Service/Analytics/Jobs/{0}/EnableLiveJobConcurrencyLimit";
    private const string JobConcurrencyStaticLimitPathTemplate = "/Service/Analytics/Jobs/{0}/JobConcurrencyStaticLimit";

    public abstract int? StaticConcurrencyLimit { get; }

    public abstract LimitedConcurrencyAnalyticsJobCommand.LimitPercentCores? PercentageOfCoresConcurrencyLimit { get; }

    public abstract int ConcurrencyRequeueSeconds { get; }

    public abstract LimitedConcurrencyAnalyticsJobCommand.AnalyticsJobCategory? JobCategory { get; }

    public override TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      if (!this.StaticConcurrencyLimit.HasValue && !this.PercentageOfCoresConcurrencyLimit.HasValue && !this.JobCategory.HasValue)
        return base.Run(requestContext, jobDefinition, queueTime, out resultMessage);
      bool flag = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) string.Format("/Service/Analytics/Jobs/{0}/EnableLiveJobConcurrencyLimit", (object) this.TableName), true, false);
      int? nullable = requestContext.GetService<IVssRegistryService>().GetValue<int?>(requestContext, (RegistryQuery) string.Format("/Service/Analytics/Jobs/{0}/JobConcurrencyStaticLimit", (object) this.TableName), true, this.StaticConcurrencyLimit);
      if (jobDefinition.JobId == this.LiveJobId && !flag)
        return base.Run(requestContext, jobDefinition, queueTime, out resultMessage);
      int concurrencyLimit = !nullable.HasValue ? (!this.JobCategory.HasValue ? (Environment.ProcessorCount > 2 ? (int) Math.Ceiling((Decimal) Environment.ProcessorCount * ((Decimal) (int) this.PercentageOfCoresConcurrencyLimit.Value / 100M)) : 1) : (Environment.ProcessorCount > 2 ? Environment.ProcessorCount - 1 : 1)) : nullable.Value;
      requestContext.TraceAlways(14010044, TraceLevel.Info, "Analytics", "AnalyticsStaging", string.Format("Concurrency Limit {0} for Job {1}", (object) concurrencyLimit, (object) jobDefinition.JobId));
      return this.RunWithConcurrencyLimit(requestContext, jobDefinition, queueTime, out resultMessage, concurrencyLimit);
    }

    private TeamFoundationJobExecutionResult RunWithConcurrencyLimit(
      IVssRequestContext rc,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage,
      int concurrencyLimit)
    {
      string innerResultMsg = (string) null;
      string str = this.JobCategory.HasValue ? this.JobCategory.ToString() : jobDefinition.ExtensionName;
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Analytics.").AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionMaxConcurrentRequests(concurrencyLimit));
      int num = (int) new CommandService<TeamFoundationJobExecutionResult>(rc, setter, new Func<TeamFoundationJobExecutionResult>(RunJob), new Func<TeamFoundationJobExecutionResult>(RequeueLaterFallback)).Execute();
      resultMessage = innerResultMsg;
      return (TeamFoundationJobExecutionResult) num;

      TeamFoundationJobExecutionResult RunJob()
      {
        try
        {
          return base.Run(rc, jobDefinition, queueTime, out innerResultMsg);
        }
        catch (Exception ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
      }

      TeamFoundationJobExecutionResult RequeueLaterFallback()
      {
        rc.GetService<ITeamFoundationJobService>().QueueDelayedJobs(rc, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
        {
          jobDefinition.ToJobReference()
        }, this.ConcurrencyRequeueSeconds);
        innerResultMsg = "Re-queued job because there are too many concurrent jobs running.";
        return TeamFoundationJobExecutionResult.Blocked;
      }
    }

    public enum LimitPercentCores
    {
      JobIsHighCPU = 25, // 0x00000019
      JobIsMediumCPU = 50, // 0x00000032
      JobIsLowCPU = 75, // 0x0000004B
    }

    public enum AnalyticsJobCategory
    {
      RealTime,
      NonRealTime,
    }
  }
}
