// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TeamFoundationBuildDispatcherService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class TeamFoundationBuildDispatcherService : 
    ITeamFoundationBuildDispatcherService,
    IVssFrameworkService
  {
    private int m_threadCount;
    private int m_threadConcurrency;
    private int m_fireTaskTimeoutSeconds;
    private VssTaskDispatcher m_dispatcher;
    private object m_dispatcherStartLock = new object();
    private const int c_threadCount = 2;
    private const int c_threadConcurrency = 2;
    private const int c_fireTaskTimeoutSeconds = 15;
    private const string c_threadCountRegistryPath = "FireEventThreadCount";
    private const string c_threadConcurrencyRegistryPath = "FireEventThreadConcurrency";
    private const string c_fireTimeoutRegistryPath = "FireEventTimeoutSeconds";
    private static readonly RegistryQuery s_buildSettingsQuery = (RegistryQuery) "/Service/Build2/Settings/...";

    public void EnqueueBuildTask(
      IVssRequestContext requestContext,
      string dispatcherQueueName,
      Func<IVssRequestContext, Task> func)
    {
      if (this.m_dispatcher == null)
      {
        this.m_dispatcher = new VssTaskDispatcher(requestContext.ServiceHost.DeploymentServiceHost, dispatcherQueueName, this.m_threadCount, this.m_threadConcurrency);
        lock (this.m_dispatcherStartLock)
          this.m_dispatcher.Start();
      }
      this.m_dispatcher.Run(requestContext.Elevate(), dispatcherQueueName, func);
    }

    internal void OnServiceSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_threadCount = changedEntries.GetValueFromPath<int>("FireEventThreadCount", 2);
      this.m_threadConcurrency = changedEntries.GetValueFromPath<int>("FireEventThreadConcurrency", 2);
      this.m_fireTaskTimeoutSeconds = changedEntries.GetValueFromPath<int>("FireEventTimeoutSeconds", 15);
      if (this.m_dispatcher == null || this.m_threadCount == this.m_dispatcher.MaxThreadCount && this.m_threadConcurrency == this.m_dispatcher.MaxConcurrencyPerThread)
        return;
      VssTaskDispatcher dispatcher = this.m_dispatcher;
      VssTaskDispatcher vssTaskDispatcher = new VssTaskDispatcher(requestContext.ServiceHost.DeploymentServiceHost, this.m_dispatcher.Name, this.m_threadCount, this.m_threadConcurrency);
      lock (this.m_dispatcherStartLock)
      {
        vssTaskDispatcher.Start();
        this.m_dispatcher = vssTaskDispatcher;
      }
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.ShutdownDispatcher), (object) dispatcher, 0));
    }

    internal VssTaskDispatcher Dispatcher => this.m_dispatcher;

    private void ShutdownDispatcher(IVssRequestContext requestContext, object taskArgs) => ((IVssTaskDispatcher) taskArgs).Stop(TimeSpan.FromSeconds((double) this.m_fireTaskTimeoutSeconds));

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnServiceSettingsChanged));
      if (this.m_dispatcher == null)
        return;
      this.m_dispatcher.Stop(TimeSpan.FromSeconds((double) this.m_fireTaskTimeoutSeconds));
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.ReadSettings(requestContext);

    internal void ReadSettings(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnServiceSettingsChanged), false, in TeamFoundationBuildDispatcherService.s_buildSettingsQuery);
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(requestContext, TeamFoundationBuildDispatcherService.s_buildSettingsQuery);
      this.m_threadCount = registryEntryCollection.GetValueFromPath<int>("FireEventThreadCount", 2);
      this.m_threadConcurrency = registryEntryCollection.GetValueFromPath<int>("FireEventThreadConcurrency", 2);
      this.m_fireTaskTimeoutSeconds = registryEntryCollection.GetValueFromPath<int>("FireEventTimeoutSeconds", 15);
    }
  }
}
