// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssTaskService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  public abstract class VssTaskService : IVssTaskService, IVssFrameworkService
  {
    private string m_registryPath;
    private readonly ConcurrentStack<Tuple<VssTaskDispatcher, TimeSpan>> m_taskDispatchers = new ConcurrentStack<Tuple<VssTaskDispatcher, TimeSpan>>();

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_registryPath = "/Configuration/Threading/ThreadPool/" + this.Name;
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnConfigurationUpdated), false, this.m_registryPath + "/**");
      this.LoadSettings(systemRequestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnConfigurationUpdated));
      Tuple<VssTaskDispatcher, TimeSpan> result;
      while (this.m_taskDispatchers.TryPop(out result))
        result.Item1.Stop(result.Item2);
    }

    protected virtual string Name => this.GetType().Name;

    protected abstract int DefaultThreadCount { get; }

    protected abstract TimeSpan DefaultTaskTimeout { get; }

    public Task RunAsync(
      IVssRequestContext requestContext,
      string actionName,
      Func<IVssRequestContext, Task> action)
    {
      return this.RunWithDetachedCancellationAsync(requestContext, actionName, action, requestContext.CancellationToken);
    }

    public Task RunWithDetachedCancellationAsync(
      IVssRequestContext requestContext,
      string actionName,
      Func<IVssRequestContext, Task> action,
      CancellationToken cancellationToken)
    {
      Tuple<VssTaskDispatcher, TimeSpan> result;
      if (this.m_taskDispatchers.TryPeek(out result))
        return result.Item1.RunAsync(requestContext, actionName, action, cancellationToken);
      throw new InvalidOperationException("Unexpected empty dispatcher stack in " + this.Name);
    }

    public void Schedule(
      IVssRequestContext requestContext,
      string actionName,
      Func<IVssRequestContext, Task> action,
      TimeSpan delay)
    {
      Tuple<VssTaskDispatcher, TimeSpan> result;
      if (!this.m_taskDispatchers.TryPeek(out result))
        throw new InvalidOperationException("Unexpected empty dispatcher stack in " + this.Name);
      result.Item1.ScheduleAsync(requestContext, actionName, action, delay, requestContext.CancellationToken);
    }

    private void OnConfigurationUpdated(
      IVssRequestContext requestContext,
      RegistryEntryCollection ce)
    {
      this.LoadSettings(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, new RegistryQuery(this.m_registryPath, "", 1));
      int valueFromPath1 = registryEntryCollection.GetValueFromPath<int>("ThreadCount", this.DefaultThreadCount);
      TimeSpan valueFromPath2 = registryEntryCollection.GetValueFromPath<TimeSpan>("TaskTimeout", this.DefaultTaskTimeout);
      Tuple<VssTaskDispatcher, TimeSpan> result;
      if (this.m_taskDispatchers.TryPeek(out result) && result.Item1.MaxThreadCount == valueFromPath1 && !(result.Item2 != valueFromPath2))
        return;
      requestContext.TraceAlways(27177, TraceLevel.Info, nameof (VssTaskService), this.Name, "Creating VssTaskDispatcher: threadCount={0}, taskTimeout={1}", (object) valueFromPath1, (object) valueFromPath2);
      VssTaskDispatcher vssTaskDispatcher = new VssTaskDispatcher(requestContext.ServiceHost.DeploymentServiceHost, this.Name, valueFromPath1, 1);
      vssTaskDispatcher.Start();
      this.m_taskDispatchers.Push(Tuple.Create<VssTaskDispatcher, TimeSpan>(vssTaskDispatcher, valueFromPath2));
    }
  }
}
