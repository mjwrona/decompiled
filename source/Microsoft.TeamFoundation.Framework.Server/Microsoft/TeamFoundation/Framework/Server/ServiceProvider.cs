// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.RegistryService.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceProvider : IVssFrameworkServiceProvider
  {
    private readonly ConcurrentDictionary<Type, ParameterizedLazy<IVssFrameworkService, IVssRequestContext>> m_managedServices = new ConcurrentDictionary<Type, ParameterizedLazy<IVssFrameworkService, IVssRequestContext>>();
    private readonly ServiceResolver m_serviceResolver;
    private static int s_unexpectedHostTypeReported;
    private static readonly IReadOnlyList<Type> s_coreServices = (IReadOnlyList<Type>) new \u003C\u003Ez__ReadOnlyArray<Type>(new Type[10]
    {
      typeof (TeamFoundationLockingService),
      typeof (VssHttpMessageHandlerService),
      typeof (CachedRegistryService),
      typeof (VirtualCachedRegistryService),
      typeof (DataspaceService),
      typeof (TeamFoundationSqlNotificationService),
      typeof (VirtualSqlNotificationService),
      typeof (SqlRegistryService),
      typeof (TeamFoundationTaskService),
      typeof (TeamFoundationTracingService)
    });
    private static readonly VssPerformanceCounter s_deploymentCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ServiceProvider_DeploymentGetServiceCallsPerSec");
    private static readonly VssPerformanceCounter s_enterpriseCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ServiceProvider_EnterpriseGetServiceCallsPerSec");
    private static readonly VssPerformanceCounter s_collectionCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ServiceProvider_ProjectCollectionGetServiceCallsPerSec");
    private const string c_Area = "HostManagement";
    private const string c_Layer = "ServiceProvider";

    internal ServiceProvider(ServiceResolver serviceResolver) => this.m_serviceResolver = serviceResolver;

    public T GetService<T>(IVssRequestContext requestContext, Func<IVssRequestContext, T> factory) where T : class, IVssFrameworkService
    {
      Type requestedType = typeof (T);
      ServiceProvider.IncrementGetServiceCounter(requestContext);
      return requestedType == typeof (ITeamFoundationHostManagementService) || requestedType == typeof (IInternalTeamFoundationHostManagementService) || requestedType == typeof (TeamFoundationHostManagementService) ? (T) ServiceProvider.GetHostManagementService(requestContext) : (T) this.GetOrAddService(requestContext, requestedType, (Func<IVssRequestContext, IVssFrameworkService>) factory);
    }

    public T GetService<T>(IVssRequestContext requestContext) where T : class, IVssFrameworkService => this.GetService<T>(requestContext, (Func<IVssRequestContext, T>) null);

    private IVssFrameworkService GetOrAddService(
      IVssRequestContext requestContext,
      Type requestedType,
      Func<IVssRequestContext, IVssFrameworkService> factory)
    {
      VssFrameworkServiceDescriptor serviceDescriptor = this.m_serviceResolver.GetServiceDescriptor(requestContext, requestedType);
      IVssFrameworkService managedService = serviceDescriptor.ImplementationInstance as IVssFrameworkService;
      return (managedService == null ? this.m_managedServices.GetOrAdd(serviceDescriptor.ImplementationType, (Func<Type, ParameterizedLazy<IVssFrameworkService, IVssRequestContext>>) (type => new ParameterizedLazy<IVssFrameworkService, IVssRequestContext>((Func<IVssRequestContext, IVssFrameworkService>) (context => ServiceProvider.CreateService(context, requestedType, serviceDescriptor.ImplementationType, factory))))) : this.m_managedServices.GetOrAdd(serviceDescriptor.ImplementationType, (Func<Type, ParameterizedLazy<IVssFrameworkService, IVssRequestContext>>) (type => new ParameterizedLazy<IVssFrameworkService, IVssRequestContext>(managedService)))).GetValue(requestContext);
    }

    internal static IVssFrameworkService CreateService(
      IVssRequestContext requestContext,
      Type requestedType,
      Type managedType,
      Func<IVssRequestContext, IVssFrameworkService> factory = null)
    {
      IVssFrameworkService service = (IVssFrameworkService) null;
      TeamFoundationTracingService.TraceRaw(16102, TraceLevel.Verbose, "HostManagement", nameof (ServiceProvider), "Begin Loading Service {0} for the host {1}", (object) managedType.Name, (object) requestContext.ServiceHost);
      bool flag = false;
      try
      {
        try
        {
          VssPerformanceEventSource.Log.CreateServiceInstanceBegin(requestContext.UniqueIdentifier, requestContext.ServiceHost.InstanceId, managedType.Name);
          if (factory != null)
          {
            TeamFoundationTracingService.TraceRaw(16104, TraceLevel.Verbose, "HostManagement", nameof (ServiceProvider), "Creating {0} with passed factory method.", (object) managedType);
            service = factory(requestContext);
          }
          else
            service = (IVssFrameworkService) Activator.CreateInstance(managedType, true);
        }
        catch (MissingMethodException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(16093, "HostManagement", nameof (ServiceProvider), (Exception) ex);
          throw new ArgumentException(FrameworkResources.GetServiceArgumentError((object) managedType), (Exception) ex);
        }
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          service.ServiceStart(requestContext.Elevate());
          flag = true;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(16095, TraceLevel.Error, "HostManagement", nameof (ServiceProvider), ex, string.Format("Error starting service type {0} with implementation {1}: {2}", (object) requestedType, (object) managedType, (object) ex.ToReadableStackTrace()));
          throw;
        }
        finally
        {
          stopwatch.Stop();
          VssPerformanceEventSource.Log.CreateServiceInstanceEnd(requestContext.UniqueIdentifier, requestContext.ServiceHost.InstanceId, managedType.Name, stopwatch.ElapsedMilliseconds);
          long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
          TeamFoundationTracingService.TraceRaw(16100, TraceLevel.Info, "HostManagement", nameof (ServiceProvider), "Service of type {0} started in {1} ms", (object) managedType, (object) stopwatch.ElapsedMilliseconds);
        }
      }
      catch (Exception ex1)
      {
        TeamFoundationTracingService.TraceExceptionRaw(16103, TraceLevel.Error, "HostManagement", nameof (ServiceProvider), ex1, string.Format("Error starting service type {0} with implementation {1}: {2}", (object) requestedType, (object) managedType, (object) ex1.ToReadableStackTrace()));
        switch (ex1)
        {
          case DatabaseConnectionException _:
          case HostShutdownException _:
label_11:
            if (flag)
            {
              try
              {
                service.ServiceEnd(requestContext.Elevate());
              }
              catch (Exception ex2)
              {
                TeamFoundationTracingService.TraceExceptionRaw(16125, TraceLevel.Error, "HostManagement", nameof (ServiceProvider), ex2, string.Format("Error stopping service type {0} with implementation {1}: {2}", (object) requestedType, (object) managedType, (object) ex2.ToReadableStackTrace()));
              }
            }
            ServiceProvider.DisposeService(service);
            throw;
          default:
            TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.ServiceInitFailed((object) managedType.FullName), ex1, TeamFoundationEventId.ApplicationInitialization, EventLogEntryType.Error);
            goto label_11;
        }
      }
      return service;
    }

    private static void IncrementGetServiceCounter(IVssRequestContext requestContext)
    {
      TeamFoundationHostType foundationHostType = requestContext.ServiceHost.HostType;
      if ((foundationHostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        foundationHostType = TeamFoundationHostType.Deployment;
      switch (foundationHostType)
      {
        case TeamFoundationHostType.Deployment:
          ServiceProvider.s_deploymentCounter.Increment();
          break;
        case TeamFoundationHostType.Application:
          ServiceProvider.s_enterpriseCounter.Increment();
          break;
        case TeamFoundationHostType.ProjectCollection:
          ServiceProvider.s_collectionCounter.Increment();
          break;
        default:
          if (Interlocked.CompareExchange(ref ServiceProvider.s_unexpectedHostTypeReported, 1, 0) != 0)
            break;
          requestContext.Trace(1612516125, TraceLevel.Error, "HostManagement", nameof (ServiceProvider), string.Format("Unexpected host type {0}, stack={1}", (object) foundationHostType, (object) Environment.StackTrace));
          break;
      }
    }

    public void StopServices(
      IVssRequestContext requestContext,
      bool stopCoreServices,
      bool retainServiceCollection = false)
    {
      if (!retainServiceCollection)
        this.m_serviceResolver.Reset();
      if (stopCoreServices)
      {
        try
        {
          ParameterizedLazy<IVssFrameworkService, IVssRequestContext> parameterizedLazy;
          if (this.m_managedServices.TryGetValue(typeof (TeamFoundationTaskService), out parameterizedLazy))
          {
            if (parameterizedLazy.IsInitialized)
              parameterizedLazy.GetValue(requestContext).ServiceEnd(requestContext);
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(16113, "HostManagement", nameof (ServiceProvider), ex);
        }
      }
      int maxValue = (int) sbyte.MaxValue;
      while (--maxValue > 0)
      {
        IEnumerable<Type> source = this.m_managedServices.Keys.Where<Type>((Func<Type, bool>) (type => !ServiceProvider.s_coreServices.Contains<Type>(type)));
        if (source.Any<Type>())
        {
          foreach (Type key in source)
          {
            ParameterizedLazy<IVssFrameworkService, IVssRequestContext> parameterizedLazy;
            if (this.m_managedServices.TryRemove(key, out parameterizedLazy) && parameterizedLazy.IsInitialized)
              this.EndService(requestContext, parameterizedLazy.GetValue(requestContext));
          }
        }
        else
        {
          if (!stopCoreServices || !this.m_managedServices.Any<KeyValuePair<Type, ParameterizedLazy<IVssFrameworkService, IVssRequestContext>>>())
            return;
          foreach (Type coreService in (IEnumerable<Type>) ServiceProvider.s_coreServices)
          {
            ParameterizedLazy<IVssFrameworkService, IVssRequestContext> parameterizedLazy;
            if (this.m_managedServices.TryRemove(coreService, out parameterizedLazy) && parameterizedLazy.IsInitialized)
              this.EndService(requestContext, parameterizedLazy.GetValue(requestContext));
          }
        }
      }
      TeamFoundationTracingService.TraceRaw(16115, TraceLevel.Error, "HostManagement", nameof (ServiceProvider), "Reached max iterations in StopServices - giving up");
    }

    private void EndService(IVssRequestContext requestContext, IVssFrameworkService service)
    {
      try
      {
        service.ServiceEnd(requestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(16114, "HostManagement", nameof (ServiceProvider), ex);
      }
      finally
      {
        ServiceProvider.DisposeService(service);
      }
    }

    private static void DisposeService(IVssFrameworkService service)
    {
      try
      {
        if (!(service is IDisposable disposable))
          return;
        disposable.Dispose();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(16124, "HostManagement", nameof (ServiceProvider), ex);
      }
    }

    private static IVssFrameworkService GetHostManagementService(IVssRequestContext requestContext) => (IVssFrameworkService) requestContext.ServiceHost.DeploymentServiceHost.DeploymentServiceHostInternal().HostManagement;
  }
}
