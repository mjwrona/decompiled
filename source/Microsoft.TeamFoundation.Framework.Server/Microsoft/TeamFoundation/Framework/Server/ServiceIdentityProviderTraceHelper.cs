// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceIdentityProviderTraceHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceIdentityProviderTraceHelper : DelegatingHandler
  {
    private IVssDeploymentServiceHost m_deploymentServiceHost;

    internal ServiceIdentityProviderTraceHelper(IVssDeploymentServiceHost deploymentHost) => this.m_deploymentServiceHost = deploymentHost;

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage message,
      CancellationToken token)
    {
      uint startMilliseconds = this.TickCount;
      bool continueOnCapturedContext = false;
      message.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out continueOnCapturedContext);
      HttpResponseMessage httpResponseMessage;
      try
      {
        this.PerfCounterBeginRequest();
        httpResponseMessage = await base.SendAsync(message, token).ConfigureAwait(continueOnCapturedContext);
      }
      finally
      {
        this.PerfCounterEndRequest(this.TickCount - startMilliseconds);
      }
      return httpResponseMessage;
    }

    private uint TickCount => (uint) Environment.TickCount;

    private void PerfCounterBeginRequest()
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentACSCallsExecuting").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentACSCallsPerSec").Increment();
    }

    private void PerfCounterEndRequest(uint elapsedTimeInMilliseconds)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentACSCallsExecuting").Decrement();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageACSResponseTime").IncrementMilliseconds((long) elapsedTimeInMilliseconds);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageACSResponseTimeBase").Increment();
    }
  }
}
