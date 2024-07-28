// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.SmartRouterBackgroundJob
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal class SmartRouterBackgroundJob : SmartRouterBase
  {
    public SmartRouterBackgroundJob(string timerName)
      : base(SmartRouterBase.TraceLayer.BackEnd)
    {
      this.TimerName = timerName.CheckArgumentIsNotNullOrEmpty(nameof (timerName));
    }

    public void Start(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, Task> callbackFuncAsync,
      TimeSpan period)
    {
      requestContext.CheckRequestContext(true);
      this.TaskCallbackAsync = callbackFuncAsync.CheckArgumentIsNotNull<Func<IVssRequestContext, Task>>(nameof (callbackFuncAsync));
      ITeamFoundationTaskService service = requestContext.GetService<ITeamFoundationTaskService>();
      this.RegisteredTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.RunTaskCallbackSynchronously), (object) null, DateTime.UtcNow, (int) period.TotalMilliseconds);
      IVssRequestContext requestContext1 = requestContext;
      TeamFoundationTask registeredTask = this.RegisteredTask;
      service.AddTask(requestContext1, registeredTask);
    }

    public void Invoke(IVssRequestContext requestContext)
    {
      requestContext = requestContext.CheckRequestContext(true);
      if (this.TaskCallbackAsync == null)
        throw new InvalidOperationException(this.TimerName + " is not started");
      this.RunTaskCallbackSynchronously(requestContext, (object) null);
    }

    public void Stop(IVssRequestContext requestContext)
    {
      TeamFoundationTask registeredTask = this.RegisteredTask;
      this.TaskCallbackAsync = (Func<IVssRequestContext, Task>) null;
      this.RegisteredTask = (TeamFoundationTask) null;
      if (registeredTask == null)
        return;
      requestContext = requestContext.CheckRequestContext(true);
      requestContext.GetService<ITeamFoundationTaskService>().RemoveTask(requestContext, registeredTask);
    }

    internal bool IsRunning => this.TaskCallbackAsync != null;

    private void RunTaskCallbackSynchronously(IVssRequestContext requestContext, object? state)
    {
      Func<IVssRequestContext, Task> callbackFuncAsync = this.TaskCallbackAsync;
      if (callbackFuncAsync == null)
        return;
      try
      {
        using (IVssRequestContext systemContext = requestContext.ServiceHost.DeploymentServiceHost.CreateSystemContext())
        {
          this.Tracer.TraceInfo(systemContext, SmartRouterBase.TracePoint.BackEndTimerTick, this.TimerName);
          systemContext.RunSynchronously((Func<Task>) (() => callbackFuncAsync(systemContext)));
        }
      }
      catch (Exception ex)
      {
        this.Tracer.TraceException(SmartRouterBase.TracePoint.BackEndTimerException, ex, "timer={0}, exception={1}", (object) this.TimerName, (object) this.Tracer.GetLazyStackTrace(ex));
        throw;
      }
    }

    private string TimerName { get; }

    private TeamFoundationTask? RegisteredTask { get; set; }

    private Func<IVssRequestContext, Task>? TaskCallbackAsync { get; set; }
  }
}
