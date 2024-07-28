// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MdmService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MdmService : IVssFrameworkService
  {
    private INotificationRegistration m_registryRegistration;
    internal static IDisposableReadOnlyList<IMdmPublisher> s_mdmPlugins;
    private const string c_mdmEmptyStringDefaultValue = "NA";
    private const string c_eventBasedMetricPrefix = "\\Event\\";
    private const string c_kpiBasedMetricPrefix = "\\Kpi\\";
    private const string c_sliBasedMetricPrefix = "\\Sli\\";
    private const string c_sliBasedMetricNamespace = "QoS";
    private bool m_mdmKpiMetricPublishEnabled;
    private bool m_mdmDatabaseMetricPublishEnabled;
    internal static bool s_mdmEventMetricPublishEnabled;
    internal static bool s_mdmMetricsPublishViaIfxEnabled;
    private const string c_locationIdEnvVar = "MONITORING_SLI_LOCATIONID";
    private const string c_newLocationIdEnvVar = "MONITORING_SLI_NEWLOCATIONID";
    private string m_locationId = Environment.GetEnvironmentVariable("MONITORING_SLI_LOCATIONID", EnvironmentVariableTarget.Machine);
    private string m_regionOnlyLocationId;
    private static readonly string[] s_kpiDimensionNames = new string[2]
    {
      "Area",
      "Scope"
    };
    private static readonly string[] s_eventDimensionNames = new string[3]
    {
      "EventSource",
      "Scope",
      "EventType"
    };
    private static readonly string[] s_databaseMetricDimensionNames = new string[4]
    {
      "ServerName",
      "DatabasePoolName",
      "DatabaseName",
      "Intent"
    };
    private static readonly RegistryQuery[] s_notificationFilters = new RegistryQuery[5]
    {
      new RegistryQuery("/Service/MdmService/MdmKpiMetricPublishEnabled"),
      new RegistryQuery("/Service/MdmService/MdmEventMetricPublishEnabled"),
      new RegistryQuery("/Service/MdmService/MdmDatabaseMetricPublishEnabled"),
      new RegistryQuery("/Service/MdmService/MdmMetricsPublishViaIfxEnabled"),
      new RegistryQuery("/Service/MdmService/MetricScalingFactor")
    };
    private static readonly RegistryQuery[] s_mdmPublisherNotificationFilters = new RegistryQuery[7]
    {
      new RegistryQuery("/Service/MdmService/MdmMetricsPublishViaIfxEnabled"),
      new RegistryQuery("/Diagnostics/Hosting/MdmDiagnostics/MdmEnabled"),
      new RegistryQuery("/Diagnostics/Hosting/MdmDiagnostics/MdmAccountName"),
      new RegistryQuery("/Diagnostics/Hosting/MdmDiagnostics/MdmNamespace"),
      new RegistryQuery("/Diagnostics/Hosting/MdmDiagnostics/MdmRegion"),
      new RegistryQuery("/Diagnostics/Hosting/MdmDiagnostics/MdmScaleUnit"),
      new RegistryQuery("/Diagnostics/Hosting/MdmDiagnostics/MdmService")
    };
    private object m_sli_lock_object = new object();
    private const string c_area = "MdmService";
    private const string c_layer = "MetricTracing";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      (this.m_locationId, this.m_regionOnlyLocationId) = MdmService.GetLocationId(systemRequestContext);
      this.m_registryRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.RegistrySettingsChanged, new SqlNotificationCallback(this.OnRegistrySettingsChanged), false, false);
      this.LoadMdmServiceRegistrySettings(systemRequestContext);
      this.InitializeMdmPublisher(systemRequestContext);
      lock (this.m_sli_lock_object)
        this.LoadMdmSliSettings(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_registryRegistration.Unregister(systemRequestContext);
      MdmService.s_mdmPlugins?.Dispose();
      MdmService.s_mdmPlugins = (IDisposableReadOnlyList<IMdmPublisher>) null;
    }

    internal void LoadMdmServiceRegistrySettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12040230, nameof (MdmService), "MetricTracing", nameof (LoadMdmServiceRegistrySettings));
      try
      {
        SqlRegistryService service = requestContext.GetService<SqlRegistryService>();
        SqlRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) "/Service/MdmService/MdmDatabaseMetricPublishEnabled";
        ref RegistryQuery local1 = ref registryQuery;
        this.m_mdmDatabaseMetricPublishEnabled = registryService1.GetValue<bool>(requestContext1, in local1, false);
        SqlRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Service/MdmService/MdmKpiMetricPublishEnabled";
        ref RegistryQuery local2 = ref registryQuery;
        this.m_mdmKpiMetricPublishEnabled = registryService2.GetValue<bool>(requestContext2, in local2, false);
        SqlRegistryService registryService3 = service;
        IVssRequestContext requestContext3 = requestContext;
        registryQuery = (RegistryQuery) "/Service/MdmService/MdmEventMetricPublishEnabled";
        ref RegistryQuery local3 = ref registryQuery;
        MdmService.s_mdmEventMetricPublishEnabled = registryService3.GetValue<bool>(requestContext3, in local3, false);
        SqlRegistryService registryService4 = service;
        IVssRequestContext requestContext4 = requestContext;
        registryQuery = (RegistryQuery) "/Service/MdmService/MdmMetricsPublishViaIfxEnabled";
        ref RegistryQuery local4 = ref registryQuery;
        MdmService.s_mdmMetricsPublishViaIfxEnabled = registryService4.GetValue<bool>(requestContext4, in local4, false);
        requestContext.Trace(12040231, TraceLevel.Verbose, nameof (MdmService), "MetricTracing", "Registry setting loaded: m_mdmKpiMetricPublishEnabled - {0}, s_mdmEventMetricPublishEnabled - {1}, m_mdmDatabaseMetricPublishEnabled - {2}, s_mdmMetricsPublishViaIfxEnabled - {3}", (object) this.m_mdmKpiMetricPublishEnabled, (object) MdmService.s_mdmEventMetricPublishEnabled, (object) this.m_mdmDatabaseMetricPublishEnabled, (object) MdmService.s_mdmMetricsPublishViaIfxEnabled);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12040233, nameof (MdmService), "MetricTracing", ex);
      }
      finally
      {
        requestContext.TraceLeave(12040232, nameof (MdmService), "MetricTracing", nameof (LoadMdmServiceRegistrySettings));
      }
    }

    internal void InitializeMdmPublisher(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12040240, nameof (MdmService), "MetricTracing", nameof (InitializeMdmPublisher));
      try
      {
        if (MdmService.s_mdmMetricsPublishViaIfxEnabled)
        {
          if (MdmService.s_mdmPlugins == null)
          {
            MdmService.s_mdmPlugins = requestContext.GetExtensions<IMdmPublisher>();
          }
          else
          {
            IDisposableReadOnlyList<IMdmPublisher> mdmPlugins = MdmService.s_mdmPlugins;
            MdmService.s_mdmPlugins = requestContext.GetExtensions<IMdmPublisher>();
            mdmPlugins.Dispose();
          }
          foreach (IMdmPublisher mdmPlugin in (IEnumerable<IMdmPublisher>) MdmService.s_mdmPlugins)
          {
            try
            {
              mdmPlugin.Initialize(requestContext);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12040241, nameof (MdmService), "MetricTracing", ex);
            }
          }
        }
        else
          requestContext.Trace(12040242, TraceLevel.Info, nameof (MdmService), "MetricTracing", "Publishing Mdm metrics through Ifx is disabled.");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12040244, nameof (MdmService), "MetricTracing", ex);
      }
      finally
      {
        requestContext.TraceLeave(12040243, nameof (MdmService), "MetricTracing", nameof (InitializeMdmPublisher));
      }
    }

    private void LoadMdmSliSettings(IVssRequestContext systemRequestContext)
    {
      if (MdmService.MdmSliConfigurations != null)
        return;
      MdmService.MdmSliConfigurations = new List<MdmSliConfiguration>();
      MdmService.MdmSliConfigurations.Add(new MdmSliConfiguration()
      {
        MdmMetric = "Default"
      });
      string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\MdmSliConfiguration.json");
      if (!File.Exists(path))
      {
        systemRequestContext.Trace(12040300, TraceLevel.Info, nameof (MdmService), "MetricTracing", "Custom Sli configuration is not found.");
      }
      else
      {
        string str = File.ReadAllText(path);
        List<MdmSliConfiguration> source = new List<MdmSliConfiguration>();
        try
        {
          source = JsonConvert.DeserializeObject<List<MdmSliConfiguration>>(str);
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(12040301, nameof (MdmService), "MetricTracing", ex);
        }
        MdmService.MdmSliConfigurations.AddRange((IEnumerable<MdmSliConfiguration>) source.Where<MdmSliConfiguration>((Func<MdmSliConfiguration, bool>) (x => x.MdmMetric != "Default")).ToList<MdmSliConfiguration>());
      }
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      IEnumerable<RegistryItem> entries = SqlRegistryService.DeserializeSqlNotification(requestContext, eventData);
      (this.m_locationId, this.m_regionOnlyLocationId) = MdmService.GetLocationId(requestContext);
      if (((IEnumerable<RegistryQuery>) MdmService.s_notificationFilters).SelectMany<RegistryQuery, RegistryItem>((Func<RegistryQuery, IEnumerable<RegistryItem>>) (s => entries.Filter(s))).Any<RegistryItem>())
        this.LoadMdmServiceRegistrySettings(requestContext);
      if (!((IEnumerable<RegistryQuery>) MdmService.s_mdmPublisherNotificationFilters).SelectMany<RegistryQuery, RegistryItem>((Func<RegistryQuery, IEnumerable<RegistryItem>>) (s => entries.Filter(s))).Any<RegistryItem>())
        return;
      this.InitializeMdmPublisher(requestContext);
    }

    private static (string locationId, string regionOnlyLocationId) GetLocationId(
      IVssRequestContext requestContext)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      if (requestContext.IsFeatureEnabled("VisualStudio.Diagnostics.UseNewLocationIdFormat"))
        str1 = Environment.GetEnvironmentVariable("MONITORING_SLI_NEWLOCATIONID", EnvironmentVariableTarget.Machine);
      if (string.IsNullOrWhiteSpace(str1))
        str1 = Environment.GetEnvironmentVariable("MONITORING_SLI_LOCATIONID", EnvironmentVariableTarget.Machine);
      if (requestContext.IsFeatureEnabled("VisualStudio.Diagnostics.UseRegionOnlyLocationIdFormat"))
        str2 = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Service/AzureRegion", false, (string) null);
      return (str1, str2);
    }

    public void PublishKpi(
      IVssRequestContext requestContext,
      DateTime eventTime,
      Guid hostId,
      string area,
      string scope,
      string displayName,
      string description,
      string metricName,
      long metricValue)
    {
      requestContext.TraceEnter(12040210, nameof (MdmService), "MetricTracing", nameof (PublishKpi));
      try
      {
        if (!this.m_mdmKpiMetricPublishEnabled)
          return;
        TeamFoundationTracingService.TraceRaw(12040214, TraceLevel.Verbose, nameof (MdmService), "MetricTracing", "Kpi metric publish flag is enabled.");
        MdmService.NormalizeMdmString(ref area);
        MdmService.NormalizeMdmString(ref scope);
        MdmService.NormalizeMdmString(ref displayName);
        MdmService.NormalizeMdmString(ref description);
        MdmService.GenerateMetricName(ref metricName, "\\Kpi\\");
        if (MdmService.s_mdmMetricsPublishViaIfxEnabled)
        {
          string[] dimensionValues = new string[2]
          {
            area,
            scope
          };
          if (MdmService.s_mdmPlugins == null)
            return;
          foreach (IMdmPublisher mdmPlugin in (IEnumerable<IMdmPublisher>) MdmService.s_mdmPlugins)
          {
            try
            {
              mdmPlugin.TraceMdmMetric(requestContext, metricName, metricValue, MdmService.s_kpiDimensionNames, dimensionValues);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12040211, nameof (MdmService), "MetricTracing", ex);
            }
          }
        }
        else
          requestContext.TracingService().TraceKpiMetric(eventTime, hostId, area, scope, displayName, description, metricName, (double) metricValue);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12040213, nameof (MdmService), "MetricTracing", ex);
      }
      finally
      {
        requestContext.TraceLeave(12040212, nameof (MdmService), "MetricTracing", nameof (PublishKpi));
      }
    }

    public void PublishDatabaseMetrics(
      IVssRequestContext requestContext,
      List<DatabaseMetric> metrics)
    {
      requestContext.TraceEnter(12040250, nameof (MdmService), "MetricTracing", "PublishDatabaseMetric");
      try
      {
        if (!this.m_mdmDatabaseMetricPublishEnabled)
          return;
        TeamFoundationTracingService.TraceRaw(12040254, TraceLevel.Verbose, nameof (MdmService), "MetricTracing", "Database metric publish flag is enabled.");
        if (!MdmService.s_mdmMetricsPublishViaIfxEnabled)
          return;
        foreach (DatabaseMetric metric in metrics)
        {
          string name = metric.Name;
          string serverName = metric.ServerName;
          string databasePoolName = metric.DatabasePoolName;
          string databaseName = metric.DatabaseName;
          MdmService.NormalizeMdmString(ref name);
          MdmService.NormalizeMdmString(ref serverName);
          MdmService.NormalizeMdmString(ref databasePoolName);
          MdmService.NormalizeMdmString(ref databaseName);
          string[] dimensionValues = new string[4]
          {
            serverName,
            databasePoolName,
            databaseName,
            metric.Intent
          };
          foreach (IMdmPublisher mdmPlugin in (IEnumerable<IMdmPublisher>) MdmService.s_mdmPlugins)
          {
            try
            {
              mdmPlugin.TraceMdmMetric(requestContext, metric.TimeStamp, name, (long) metric.Value, MdmService.s_databaseMetricDimensionNames, dimensionValues);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12040251, nameof (MdmService), "MetricTracing", ex);
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12040253, nameof (MdmService), "MetricTracing", ex);
      }
      finally
      {
        requestContext.TraceLeave(12040252, nameof (MdmService), "MetricTracing", "PublishDatabaseMetric");
      }
    }

    public static void PublishEvent(
      DateTime eventTime,
      int databaseId,
      string deploymentId,
      Guid hostId,
      string machineName,
      string roleInstanceId,
      string eventSource,
      string scope,
      string eventType,
      int eventId,
      string metricName,
      long metricValue)
    {
      TeamFoundationTracingService.TraceEnterRaw(12040220, nameof (MdmService), "MetricTracing", nameof (PublishEvent), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        if (!MdmService.s_mdmEventMetricPublishEnabled)
          return;
        TeamFoundationTracingService.TraceRaw(12040221, TraceLevel.Verbose, nameof (MdmService), "MetricTracing", "Event metric publish flag is enabled.");
        MdmService.NormalizeMdmString(ref deploymentId);
        MdmService.NormalizeMdmString(ref machineName);
        MdmService.NormalizeMdmString(ref roleInstanceId);
        MdmService.NormalizeMdmString(ref eventSource);
        MdmService.NormalizeMdmString(ref scope);
        MdmService.GenerateMetricName(ref metricName, "\\Event\\");
        if (MdmService.s_mdmMetricsPublishViaIfxEnabled)
        {
          string[] dimensionValues = new string[3]
          {
            eventSource,
            scope,
            eventType
          };
          foreach (IMdmPublisher mdmPlugin in (IEnumerable<IMdmPublisher>) MdmService.s_mdmPlugins)
          {
            try
            {
              mdmPlugin.TraceMdmMetric((IVssRequestContext) null, DateTime.UtcNow, metricName, metricValue, MdmService.s_eventDimensionNames, dimensionValues);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(12040210, nameof (MdmService), nameof (PublishEvent), ex);
            }
          }
          TeamFoundationTracingService.TraceRaw(12040222, TraceLevel.Verbose, nameof (MdmService), "MetricTracing", "Published Event (Event Source - {0}; Event Id - {1}) via Ifx", (object) eventSource, (object) eventId);
        }
        else
        {
          TeamFoundationTracingService.TraceEventMetric(eventTime, databaseId, deploymentId, hostId, machineName, roleInstanceId, eventSource, scope, eventType, eventId, metricName, (double) metricValue);
          TeamFoundationTracingService.TraceRaw(12040223, TraceLevel.Verbose, nameof (MdmService), "MetricTracing", "Published Event (Event Source - {0}; Event Id - {1}) via ETW", (object) eventSource, (object) eventId);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(12040224, nameof (MdmService), "MetricTracing", ex);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(12040225, nameof (MdmService), "MetricTracing", nameof (PublishEvent));
      }
    }

    public static void PublishMetricRaw(
      string metricName,
      long metricValue,
      string[] dimensionNames,
      string[] dimensionValues)
    {
      if (!MdmService.s_mdmEventMetricPublishEnabled || !MdmService.s_mdmMetricsPublishViaIfxEnabled || MdmService.s_mdmPlugins == null)
        return;
      foreach (IMdmPublisher mdmPlugin in (IEnumerable<IMdmPublisher>) MdmService.s_mdmPlugins)
      {
        try
        {
          mdmPlugin.TraceMdmMetricNoTrace((IVssRequestContext) null, metricName, metricValue, dimensionNames, dimensionValues);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public void PublishSli(
      IVssRequestContext systemRequestContext,
      IEnumerable<RequestDetails> requestList)
    {
      if (!MdmService.s_mdmMetricsPublishViaIfxEnabled || !systemRequestContext.IsFeatureEnabled("VisualStudio.Diagnostics.MdmDefaultSliMetric") && !systemRequestContext.IsFeatureEnabled("VisualStudio.Diagnostics.MdmCustomSliMetric"))
        return;
      foreach (MdmSliConfiguration sliConfiguration in MdmService.MdmSliConfigurations)
      {
        foreach (RequestDetails request in requestList)
        {
          if (request.HostType != TeamFoundationHostType.ProjectCollection)
          {
            if (!systemRequestContext.IsFeatureEnabled("VisualStudio.Diagnostics.MdmSliHostTypeFilterDisabled"))
              continue;
          }
          else if (request.IdentityTracingItems == null || request.IdentityTracingItems.Cuid == Guid.Empty)
            continue;
          if (sliConfiguration.ShouldApplicationCommandPublishSli(request.ServiceName, request.Title) && (!(sliConfiguration.MdmMetric == "Default") || systemRequestContext.IsFeatureEnabled("VisualStudio.Diagnostics.MdmDefaultSliMetric")) && (!(sliConfiguration.MdmMetric != "Default") || systemRequestContext.IsFeatureEnabled("VisualStudio.Diagnostics.MdmCustomSliMetric")))
          {
            string str = request.ActivityStatus.ToString();
            this.PublishSli(systemRequestContext, request.IdentityTracingItems.TenantId, request.InstanceId, sliConfiguration.MdmMetric + "\\CommandStatusSli", (long) request.Count, new string[1]
            {
              "ActivityStatus"
            }, new string[1]{ str });
          }
        }
      }
    }

    public void PublishSli(
      IVssRequestContext requestContext,
      Guid aadTenantId,
      Guid hostId,
      string metricName,
      long metricValue,
      string[] dimensionNames,
      string[] dimensionValues)
    {
      requestContext.TraceEnter(12040260, nameof (MdmService), "MetricTracing", nameof (PublishSli));
      if (!MdmService.s_mdmMetricsPublishViaIfxEnabled)
        return;
      try
      {
        if (dimensionNames == null)
          dimensionNames = Array.Empty<string>();
        if (dimensionValues == null)
          dimensionValues = Array.Empty<string>();
        string str1 = string.Format("/tenants/{0}", (object) aadTenantId);
        if (requestContext.IsFeatureEnabled("VisualStudio.Diagnostics.UseNewCustomerResourceIdFormat"))
          str1 += string.Format("/organizations/{0}", (object) hostId);
        string str2 = requestContext.IsFeatureEnabled("VisualStudio.Diagnostics.UseRegionOnlyLocationIdFormat") ? this.m_regionOnlyLocationId : this.m_locationId;
        string[] second1 = new string[2]
        {
          "CustomerResourceId",
          "LocationId"
        };
        string[] second2 = new string[2]{ str1, str2 };
        dimensionNames = ((IEnumerable<string>) dimensionNames).Concat<string>((IEnumerable<string>) second1).ToArray<string>();
        dimensionValues = ((IEnumerable<string>) dimensionValues).Concat<string>((IEnumerable<string>) second2).ToArray<string>();
        for (int index = 0; index < dimensionValues.Length; ++index)
          MdmService.NormalizeMdmString(ref dimensionValues[index]);
        MdmService.GenerateMetricName(ref metricName, "\\Sli\\");
        if (MdmService.s_mdmPlugins == null)
          return;
        foreach (IMdmPublisher mdmPlugin in (IEnumerable<IMdmPublisher>) MdmService.s_mdmPlugins)
        {
          try
          {
            mdmPlugin.TraceMdmMetric(requestContext, "QoS", metricName, metricValue, dimensionNames, dimensionValues);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(12040262, nameof (MdmService), "MetricTracing", ex);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12040263, nameof (MdmService), "MetricTracing", ex);
      }
      finally
      {
        requestContext.TraceLeave(12040264, nameof (MdmService), "MetricTracing", nameof (PublishSli));
      }
    }

    private static void NormalizeMdmString(ref string message)
    {
      if (!string.IsNullOrEmpty(message))
        return;
      message = "NA";
    }

    private static void GenerateMetricName(ref string metricName, string prefix)
    {
      MdmService.NormalizeMdmString(ref metricName);
      metricName = prefix + metricName;
    }

    public static List<MdmSliConfiguration> MdmSliConfigurations { get; set; }
  }
}
