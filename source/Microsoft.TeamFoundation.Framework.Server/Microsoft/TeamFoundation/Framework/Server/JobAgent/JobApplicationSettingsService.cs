// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobAgent.JobApplicationSettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.JobAgent
{
  public class JobApplicationSettingsService : IJobApplicationSettingsService, IVssFrameworkService
  {
    private JobApplicationSettings m_jobApplicationSettings;
    private static readonly string s_area = "JobApplication";
    private static readonly string s_layer = "BusinessLogic";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), RegistryQueryConstants.JobServiceSettings, "/Diagnostics/Hosting/...");
      Interlocked.CompareExchange<JobApplicationSettings>(ref this.m_jobApplicationSettings, this.LoadJobApplicationSettings(requestContext), (JobApplicationSettings) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (changedEntries.Count <= 0)
        return;
      Interlocked.Exchange<JobApplicationSettings>(ref this.m_jobApplicationSettings, this.LoadJobApplicationSettings(requestContext));
    }

    private JobApplicationSettings LoadJobApplicationSettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) (FrameworkServerConstants.JobServiceRegistryRootPath + "/**"));
      JobApplicationSettings applicationSettings = new JobApplicationSettings(Math.Abs(registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.MaxJobsTotalPath, int.MaxValue)), Math.Abs(registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.MaxJobsPerProcessorPath, 8)), registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.JobAgentRetryOnExceptionDelaySeconds, 20), registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.MaxJobResultMessageLengthPath, 8000), registryEntryCollection.GetValueFromPath<bool>(FrameworkServerConstants.UnregisterLocalInactiveProcessesPath, true), registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.SlowQueueThresholdMilliseconds, 1000));
      requestContext.TraceAlways(1102, TraceLevel.Info, JobApplicationSettingsService.s_area, JobApplicationSettingsService.s_layer, "Settings read from tf registry. \r\nMaxJobsTotal: {0},\r\nMaxJobsPerProcessor: {1},\r\nRetryDelaySeconds: {2}, \r\nSlowQueueThresholdMilliseconds: {3}, \r\nMaxJobResultMessageLength: {4}, \r\nUnregisterLocalInactiveProcesses: {5}", (object) applicationSettings.MaxJobsTotal, (object) applicationSettings.MaxJobsPerProcessor, (object) applicationSettings.RetryDelaySeconds, (object) applicationSettings.SlowQueueThresholdMilliseconds, (object) applicationSettings.MaxJobResultMessageLength, (object) applicationSettings.UnregisterLocalInnactiveProcesses);
      requestContext.TraceAlways(1103, TraceLevel.Info, JobApplicationSettingsService.s_area, JobApplicationSettingsService.s_layer, "Max Jobs Runners is {0}", (object) applicationSettings.MaxJobRunners);
      return applicationSettings;
    }

    public JobApplicationSettings GetJobApplicationSettings() => this.m_jobApplicationSettings;
  }
}
