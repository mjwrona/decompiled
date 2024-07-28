// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationTraceReadingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationTraceReadingService : IVssFrameworkService, IDisposable
  {
    private readonly object m_internalLock = new object();
    private ITraceDataProvider m_traceDataProvider;
    private const string c_tracingService = "TraceReadingService";
    private const string c_serviceLayer = "IVssFrameworkService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        if (systemRequestContext.ExecutionEnvironment.IsOnPremisesProxy)
          return;
        CachedRegistryService service = systemRequestContext.GetService<CachedRegistryService>();
        RegistryEntryCollection registryEntries = service.ReadEntries(systemRequestContext, (RegistryQuery) (FrameworkServerConstants.TracingServiceRegistryRootPath + "/..."));
        this.OnRegistrySettingsChanged(systemRequestContext, registryEntries);
        service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), FrameworkServerConstants.TracingServiceRegistryRootPath + "/...");
        this.LoadTraceDataProvider(systemRequestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(5001, TraceLevel.Error, "TraceReadingService", "IVssFrameworkService", "Caught Exception {0} while attempting start", (object) ex);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_traceDataProvider == null)
        return;
      this.m_traceDataProvider.ServiceEnd();
    }

    void IDisposable.Dispose()
    {
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      this.LoadTraceDataProvider(requestContext);
    }

    private void LoadTraceDataProvider(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      string str = vssRequestContext.GetService<CachedRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.TracingServiceTraceDataProvider, false, (string) null);
      string traceProviderType;
      string traceProviderAssembly;
      if (!string.IsNullOrEmpty(str))
      {
        string[] array = ((IEnumerable<string>) str.Split(',')).Select<string, string>((Func<string, string>) (part => part.Trim())).ToArray<string>();
        traceProviderType = array[0];
        traceProviderAssembly = array.Length >= 2 ? array[1] : (string) null;
      }
      else
      {
        traceProviderType = (string) null;
        traceProviderAssembly = (string) null;
      }
      if (string.IsNullOrEmpty(str))
        return;
      try
      {
        lock (this.m_internalLock)
        {
          ITraceDataProvider extension = vssRequestContext.GetExtension<ITraceDataProvider>((Func<ITraceDataProvider, bool>) (x =>
          {
            Type type = x.GetType();
            if (!type.FullName.Equals(traceProviderType, StringComparison.Ordinal))
              return false;
            return string.IsNullOrEmpty(traceProviderAssembly) || traceProviderAssembly.Equals(type.Assembly.GetName().Name, StringComparison.Ordinal);
          }));
          if (extension == null)
            return;
          extension.ServiceStart(vssRequestContext);
          this.m_traceDataProvider = extension;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(5000, TraceLevel.Error, "TraceReadingService", "IVssFrameworkService", "Caught Exception {0} while attempting to load trace data provider", (object) ex);
      }
    }

    public IEnumerable<TraceEvent> QueryTraceData(
      IVssRequestContext requestContext,
      Guid traceId,
      DateTime since,
      int pageSize)
    {
      if (this.m_traceDataProvider == null)
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !requestContext.ExecutionEnvironment.IsOnPremisesProxy)
          this.LoadTraceDataProvider(requestContext);
        if (this.m_traceDataProvider == null)
          throw new InvalidOperationException();
      }
      return this.m_traceDataProvider.QueryTraceData(requestContext, traceId, since, pageSize);
    }
  }
}
