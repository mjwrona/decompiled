// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.ConfigService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Factory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  internal class ConfigService : IConfigService, IVssFrameworkService
  {
    private const string Area = "ConfigFramework";
    private const string ConfigPrefix = "/ConfigFramework/";
    private const string ConfigTree = "/ConfigFramework/**";
    private readonly IConfigFactory _configFactory;
    private readonly ConcurrentDictionary<string, IConfigReloadable> _registrations = new ConcurrentDictionary<string, IConfigReloadable>();

    public ConfigService()
      : this((IConfigFactory) new ConfigFactory())
    {
    }

    public ConfigService(IConfigFactory factory) => this._configFactory = factory;

    public static string GetConfigPath(string configName) => "/ConfigFramework/" + configName;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/ConfigFramework/**");
      systemRequestContext.GetService<ITeamFoundationTaskService>().AddTask(systemRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.PushConfig), (object) null, DateTime.UtcNow.Date + TimeSpan.FromDays(1.0), 86400000));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public IConfigQueryable<T> Register<T>(
      IVssRequestContext requestContext,
      IConfigPrototype<T> prototype)
    {
      using (requestContext.TraceBlock(37000000, 37000001, "ConfigFramework", nameof (ConfigService), nameof (Register)))
      {
        string configPath = ConfigService.GetConfigPath(prototype.Name);
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        IVssRegistryService registry = context.GetService<IVssRegistryService>();
        return (IConfigQueryable<T>) this._registrations.GetOrAdd<(IVssRequestContext, string, IConfigPrototype<T>)>(configPath, (Func<string, (IVssRequestContext, string, IConfigPrototype<T>), IConfigReloadable>) ((_, arg) =>
        {
          IConfig<T> reloadable = this._configFactory.Create<T>(arg.deploymentContext, arg.prototype);
          string str = registry.GetValue(arg.deploymentContext, (RegistryQuery) arg.path, false, (string) null);
          this.ReloadContent(arg.deploymentContext, (IConfigReloadable) reloadable, arg.path, str);
          return (IConfigReloadable) reloadable;
        }), (context, configPath, prototype));
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      using (requestContext.TraceBlock(37000002, 37000003, "ConfigFramework", nameof (ConfigService), nameof (OnRegistryChanged)))
      {
        foreach (RegistryEntry changedEntry in changedEntries)
        {
          IConfigReloadable reloadable;
          if (this._registrations.TryGetValue(changedEntry.Path, out reloadable))
            this.ReloadContent(requestContext, reloadable, changedEntry.Path, changedEntry.Value);
        }
      }
    }

    private void ReloadContent(
      IVssRequestContext requestContext,
      IConfigReloadable reloadable,
      string path,
      string value)
    {
      try
      {
        reloadable.Reload(requestContext, value ?? "[]");
        this.PrintConfig(requestContext, 37000007, path, reloadable);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(37000005, TraceLevel.Error, "ConfigFramework", nameof (ConfigService), ex, "Loading of path and value failed: '{0}'='{1}'", (object) path, (object) (value ?? "<null>"));
      }
    }

    private void PushConfig(IVssRequestContext requestContext, object _)
    {
      foreach (KeyValuePair<string, IConfigReloadable> registration in this._registrations)
        this.PrintConfig(requestContext, 37000008, registration.Key, registration.Value);
    }

    private void PrintConfig(
      IVssRequestContext requestContext,
      int tracePoint,
      string name,
      IConfigReloadable config)
    {
      requestContext.TraceAlways(tracePoint, TraceLevel.Info, "ConfigFramework", nameof (ConfigService), "'{0}'='{1}' Default: '{2}'", (object) name, (object) config.GetConfigPayload(), (object) config.GetConfigDefault());
    }
  }
}
