// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.AnalyticsJobScheduler
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public class AnalyticsJobScheduler
  {
    private const string c_retryAttemptPathTemplate = "/Service/Analytics/Jobs/{0}/RetryAttempt";

    public TeamFoundationJobDefinition JobDefinition { get; private set; }

    public string TraceLayer { get; private set; }

    public string RetryAttemptRegistryPath => string.Format("/Service/Analytics/Jobs/{0}/RetryAttempt", (object) this.JobDefinition.JobId);

    public AnalyticsJobScheduler(TeamFoundationJobDefinition jobDefinition, string traceLayer)
    {
      this.JobDefinition = jobDefinition;
      this.TraceLayer = traceLayer;
    }

    public void QueueWithDelay(IVssRequestContext requestContext, int delaySeconds) => requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
    {
      this.JobDefinition.ToJobReference()
    }, delaySeconds);

    public void Retry(IVssRequestContext requestContext, JobRetryConfiguration config)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int attempt;
      if ((attempt = service.GetValue<int>(requestContext, (RegistryQuery) this.RetryAttemptRegistryPath, 0) + 1) < config.MaxRetry)
      {
        int totalSeconds = (int) BackoffTimerHelper.GetExponentialBackoff(attempt, TimeSpan.FromSeconds((double) config.MinimumRetryIntervalSeconds), TimeSpan.FromSeconds((double) config.MaximumRetryIntervalSeconds), TimeSpan.FromSeconds((double) config.DeltaBaseSeconds)).TotalSeconds;
        requestContext.Trace(14000010, TraceLevel.Warning, "AnalyticsStaging", this.TraceLayer, string.Format("Retry attempt {0} of {1}. Will retry in {2} seconds. See job result for full exception details", (object) attempt, (object) config.MaxRetry, (object) totalSeconds));
        service.SetValue<int>(requestContext, this.RetryAttemptRegistryPath, attempt);
        this.QueueWithDelay(requestContext, totalSeconds);
      }
      else
      {
        requestContext.Trace(14000011, TraceLevel.Error, "AnalyticsStaging", this.TraceLayer, string.Format("Used up all {0} retried", (object) config.MaxRetry));
        this.DeleteRetryRegistryRecord(requestContext);
      }
    }

    public void DeleteRetryRegistryRecord(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (service.GetValue<int>(requestContext, (RegistryQuery) this.RetryAttemptRegistryPath, 0) == 0)
        return;
      service.SetValue(requestContext, this.RetryAttemptRegistryPath, (object) null);
    }
  }
}
