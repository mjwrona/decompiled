// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.VssTaskActivityInvoker`1
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  public sealed class VssTaskActivityInvoker<TActivity> : ITaskActivityInvoker
  {
    private readonly Guid m_hostId;
    private readonly CancellationToken m_cancellationToken;
    private readonly IVssDeploymentServiceHost m_deploymentHost;
    private readonly Func<IVssRequestContext, TActivity> m_activityCreator;

    public VssTaskActivityInvoker(
      IVssDeploymentServiceHost deploymentHost,
      Guid hostId,
      Func<IVssRequestContext, TActivity> activityCreator,
      CancellationToken cancellationToken)
    {
      this.m_hostId = hostId;
      this.m_deploymentHost = deploymentHost;
      this.m_activityCreator = activityCreator;
      this.m_cancellationToken = cancellationToken;
    }

    public async Task<object> Invoke(
      TaskContext context,
      string methodName,
      Func<object, Task<object>> invoke)
    {
      using (IVssRequestContext deploymentContext = this.m_deploymentHost.CreateSystemContext())
      {
        if (deploymentContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Orchestration.UseNewActivityId.Disabled"))
          return await this.InvokeActivity(deploymentContext, context, methodName, invoke);
        using (new VssActivityScope(Guid.NewGuid()))
          return await this.InvokeActivity(deploymentContext, context, methodName, invoke);
      }
    }

    private async Task<object> InvokeActivity(
      IVssRequestContext deploymentContext,
      TaskContext taskContext,
      string methodName,
      Func<object, Task<object>> invokeFunc)
    {
      ITeamFoundationHostManagementService service = deploymentContext.GetService<ITeamFoundationHostManagementService>();
      bool flag1 = deploymentContext.IsFeatureEnabled("VisualStudio.Services.HostManagement.CountCpuCyclesPerActivity");
      object obj;
      using (IVssRequestContext requestContext = service.BeginRequest(deploymentContext, this.m_hostId, RequestContextType.SystemContext))
      {
        using (CancellationTokenSource cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(this.m_cancellationToken))
        {
          ((IRequestContextInternal) requestContext).SetOrchestrationId(taskContext.OrchestrationInstance.InstanceId);
          requestContext.IsTracked = true;
          Exception exception = (Exception) null;
          try
          {
            methodName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) typeof (TActivity).FullName, (object) methodName);
            IVssRequestContext vssRequestContext = requestContext;
            string webMethodName = methodName;
            bool flag2 = flag1;
            TimeSpan timeout = new TimeSpan();
            int num = flag2 ? 1 : 0;
            MethodInformation methodInformation = new MethodInformation(webMethodName, MethodType.SystemTask, EstimatedMethodCost.Free, true, timeout: timeout, captureAsyncResourcesUsage: num != 0);
            vssRequestContext.EnterMethod(methodInformation);
            using (requestContext.LinkTokenSource(cancellationSource))
              obj = await invokeFunc((object) this.m_activityCreator(requestContext));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(15010025, "Orchestration", nameof (VssTaskActivityInvoker<TActivity>), ex);
            exception = ex;
            throw;
          }
          finally
          {
            requestContext.LeaveMethod();
            long cpuCycles = -1;
            long allocatedBytes = -1;
            if (requestContext.CPUCyclesAsync > 0L)
              cpuCycles = requestContext.CPUCyclesAsync;
            if (requestContext.AllocatedBytesAsync > 0L)
              allocatedBytes = requestContext.AllocatedBytesAsync;
            if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Orchestration.ActivityLog.Disabled"))
              requestContext.TracingService().TraceOrchestrationActivityLog(this.m_hostId, taskContext.OrchestrationInstance.InstanceId, taskContext.EventData.Name, taskContext.EventData.Version, taskContext.Message.FireAt ?? requestContext.StartTime(), requestContext.StartTime(), requestContext.ExecutionTime(), -1L, exception?.GetType().FullName, exception?.Message, requestContext.ActivityId, requestContext.E2EId, cpuCycles, allocatedBytes);
          }
        }
      }
      return obj;
    }
  }
}
