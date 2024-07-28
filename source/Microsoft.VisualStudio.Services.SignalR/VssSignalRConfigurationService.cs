// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRConfigurationService
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SignalR.Messaging;
using Microsoft.VisualStudio.Services.SignalR.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SignalR
{
  internal class VssSignalRConfigurationService : 
    VssBaseService,
    IVssSignalRConfigurationService,
    IVssFrameworkService
  {
    private ILockName m_callbackLock;
    private readonly HashSet<Action<IVssRequestContext, SourceLevels>> m_callbacks = new HashSet<Action<IVssRequestContext, SourceLevels>>();
    private const int c_defaultTransportConnectTimeout = 10;
    private const int c_defaultLongPollDelay = 0;
    private const int c_defaultConnectionTimeout = 110;
    private const int c_defaultDisconnectTimeout = 60;
    private const int c_defaultMessageBufferSize = 250;
    private const int c_defaultMaxScaleoutMappingsPerStream = 16383;
    private const int c_defaultHeartbeatInterval = 600;
    private const int c_defaultConnectionCleanupTimeout = 28800;
    private const int c_defaultGroupCleanupTimeout = 86400;
    private const string c_area = "SignalR";
    private const string c_layer = "ConfigurationService";
    private const string c_messageBusKeyRelativePath = "/MessageBus";
    private const string c_namespaceKeyRelativePath = "/Namespace";
    private const string c_topicCountKeyRelativePath = "/TopicCount";
    private const string c_topicSuffixKeyRelativePath = "/TopicSuffix";
    private const string c_tracingRelativePath = "/Tracing";
    private const string c_sourceLevelsRelativePath = "/SourceLevels";
    private const string c_cleanupTimeoutRelativePath = "/CleanupTimeout";
    private const string c_connectionTimeoutRelativePath = "/ConnectionTimeout";
    private const string c_disconnectTimeoutRelativePath = "/DisconnectTimeout";
    private const string c_transportConnectTimeoutRelativePath = "/TransportConnectTimeout";
    private const string c_longPollDelayRelativePath = "/LongPollDelay";
    private const string c_hearbeatIntervalRelativePath = "/HeartbeatInterval";
    private const string c_keepAliveRelativePath = "/KeepAlive";
    private const string c_messageBufferSizeRelativePath = "/MessageBufferSize";
    private const string c_maxScaleoutMappingsPerStreamRelativePath = "/MaxScaleoutMappingsPerStream";
    private const string c_configurationKey = "/Service/SignalR/Configuration";
    private const string c_connectionSettingsKey = "/Service/SignalR/Settings/Connections";
    private const string c_groupSettingsKey = "/Service/SignalR/Settings/Groups";
    private const string c_settingsKey = "/Service/SignalR/Settings";
    private const string c_connectionCleanupTimeoutKey = "/Service/SignalR/Settings/Connections/CleanupTimeout";
    private const string c_connectionTimeoutKey = "/Service/SignalR/Configuration/ConnectionTimeout";
    private const string c_disconnectTimeoutKey = "/Service/SignalR/Configuration/DisconnectTimeout";
    private const string c_transportConnectTimeoutKey = "/Service/SignalR/Configuration/TransportConnectTimeout";
    private const string c_longPollDelayKey = "/Service/SignalR/Configuration/LongPollDelay";
    private const string c_groupCleanupTimeoutKey = "/Service/SignalR/Settings/Groups/CleanupTimeout";
    private const string c_heartbeatIntervalKey = "/Service/SignalR/Settings/Connections/HeartbeatInterval";
    private const string c_keepAliveKey = "/Service/SignalR/Configuration/KeepAlive";
    private const string c_messageBusKey = "/Service/SignalR/Settings/MessageBus";
    private const string c_messageBusNamespaceKey = "/Service/SignalR/Settings/MessageBus/Namespace";
    private const string c_messageBusTopicCountKey = "/Service/SignalR/Settings/MessageBus/TopicCount";
    private const string c_messageBusTopicSuffixKey = "/Service/SignalR/Settings/MessageBus/TopicSuffix";
    private const string c_messageBufferSizeKey = "/Service/SignalR/Configuration/MessageBufferSize";
    private const string c_maxScaleoutMappingsPerStreamKey = "/Service/SignalR/Configuration/MaxScaleoutMappingsPerStream";
    private const string c_tracingKey = "/Service/SignalR/Settings/Tracing";
    private const string c_tracingSourceLevelsKey = "/Service/SignalR/Settings/Tracing/SourceLevels";

    public int GetConnectionCleanupTimeoutForMonitoring(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.SignalR.DisconnectOnlyIfKeepAliveIsLost") ? this.GetDisconnectTimeout(requestContext) + this.GetHeartbeatIntervalForMonitoringKeepAlive(requestContext) : requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Settings/Connections/CleanupTimeout", 28800);

    public int GetConnectionTimeout(IVssRequestContext requestContext)
    {
      int connectionTimeout = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Configuration/ConnectionTimeout", 110);
      if (connectionTimeout < 30)
      {
        requestContext.Trace(10017105, TraceLevel.Error, "SignalR", "ConfigurationService", "Incorrect value configured for ConnectionTimeout: {0}", (object) connectionTimeout);
        connectionTimeout = 30;
      }
      return connectionTimeout;
    }

    public int GetDisconnectTimeout(IVssRequestContext requestContext)
    {
      int disconnectTimeout = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Configuration/DisconnectTimeout", 60);
      if (disconnectTimeout < 30)
      {
        requestContext.Trace(10017102, TraceLevel.Error, "SignalR", "ConfigurationService", "Incorrect value configured for DisconnectTimeout: {0}", (object) disconnectTimeout);
        disconnectTimeout = 30;
      }
      return disconnectTimeout;
    }

    public int GetTransportConnectTimeout(IVssRequestContext requestContext)
    {
      int transportConnectTimeout = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Configuration/TransportConnectTimeout", 10);
      if (transportConnectTimeout < 5)
      {
        requestContext.Trace(10017104, TraceLevel.Error, "SignalR", "ConfigurationService", "Incorrect value configured for TransportConnectTimeout: {0}", (object) transportConnectTimeout);
        transportConnectTimeout = 10;
      }
      return transportConnectTimeout;
    }

    public int GetLongPollDelay(IVssRequestContext requestContext)
    {
      int longPollDelay = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Configuration/LongPollDelay", 0);
      if (longPollDelay < 0 || longPollDelay > 60)
      {
        requestContext.Trace(10017106, TraceLevel.Error, "SignalR", "ConfigurationService", "Incorrect value configured for LongPollDelay: {0}", (object) longPollDelay);
        longPollDelay = 0;
      }
      return longPollDelay;
    }

    public int GetMessageBufferSize(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Configuration/MessageBufferSize", 250);

    public int GetMaxScaleoutMappingsPerStream(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Configuration/MaxScaleoutMappingsPerStream", 16383);

    public int GetGroupCleanupTimeoutForMonitoring(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Settings/Groups/CleanupTimeout", 86400);

    public int GetHeartbeatIntervalForMonitoringKeepAlive(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.SignalR.DisconnectOnlyIfKeepAliveIsLost") ? this.GetDisconnectTimeout(requestContext) / 3 : requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/SignalR/Settings/Connections/HeartbeatInterval", 600);

    public VssMessageBusConfiguration GetMessageBusConfiguration(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      RegistryEntryCollection registryEntryCollection = vssRequestContext.GetService<IVssRegistryService>().ReadEntries(vssRequestContext, (RegistryQuery) "/Service/SignalR/Settings/MessageBus/...");
      string valueFromPath = registryEntryCollection.GetValueFromPath<string>("/Service/SignalR/Settings/MessageBus/Namespace", (string) null);
      int defaultValue = string.IsNullOrEmpty(valueFromPath) ? 16 : SignalRDefaultSettings.GetDefaultSignalRTopicCount(requestContext, valueFromPath);
      return new VssMessageBusConfiguration()
      {
        Namespace = valueFromPath,
        TopicCount = registryEntryCollection.GetValueFromPath<int>("/Service/SignalR/Settings/MessageBus/TopicCount", defaultValue),
        TopicSuffix = registryEntryCollection.GetValueFromPath<string>("/Service/SignalR/Settings/MessageBus/TopicSuffix", (string) null)
      };
    }

    public TraceListener GetTraceListener(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (TraceListener) new VssSignalRTraceListener();
    }

    public SourceLevels GetTraceSourceLevels(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<SourceLevels>(vssRequestContext, (RegistryQuery) "/Service/SignalR/Settings/Tracing/SourceLevels", SourceLevels.Warning);
    }

    public void RegisterTraceSettingsChangedNotification(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, SourceLevels> settingsChangedCallback)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Action<IVssRequestContext, SourceLevels>>(settingsChangedCallback, nameof (settingsChangedCallback));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      bool flag;
      using (vssRequestContext.Lock(this.m_callbackLock))
        flag = this.m_callbacks.Add(settingsChangedCallback);
      if (!flag)
        return;
      requestContext.GetService<IVssRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.TraceSettingsChanged), "/Service/SignalR/Settings/Tracing/SourceLevels");
    }

    public void SetMessageBusConfiguration(
      IVssRequestContext requestContext,
      VssMessageBusConfiguration configuration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<VssMessageBusConfiguration>(configuration, nameof (configuration));
      List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
      if (!string.IsNullOrEmpty(configuration.Namespace))
        registryEntryList.Add(new RegistryEntry()
        {
          Path = "/Service/SignalR/Settings/MessageBus/Namespace",
          Value = configuration.Namespace
        });
      if (configuration.TopicCount > 0)
        registryEntryList.Add(new RegistryEntry()
        {
          Path = "/Service/SignalR/Settings/MessageBus/TopicCount",
          Value = configuration.TopicCount.ToString("D")
        });
      if (!string.IsNullOrEmpty(configuration.TopicSuffix))
        registryEntryList.Add(new RegistryEntry()
        {
          Path = "/Service/SignalR/Settings/MessageBus/TopicSuffix",
          Value = configuration.TopicSuffix
        });
      if (registryEntryList.Count <= 0)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
    }

    public void UnregisterTraceSettingsChangedNotification(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, SourceLevels> settingsChangedCallback)
    {
      using (requestContext.To(TeamFoundationHostType.Deployment).Lock(this.m_callbackLock))
        this.m_callbacks.Remove(settingsChangedCallback);
    }

    private void TraceConfigurationError(IVssRequestContext requestContext, string message) => requestContext.Trace(10017110, TraceLevel.Error, "SignalR", "ConfigurationService", message);

    private void TraceSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection entries)
    {
      Action<IVssRequestContext, SourceLevels>[] array;
      using (requestContext.Lock(this.m_callbackLock))
        array = this.m_callbacks.ToArray<Action<IVssRequestContext, SourceLevels>>();
      SourceLevels valueFromPath = entries.GetValueFromPath<SourceLevels>("/Service/SignalR/Settings/Tracing/SourceLevels", SourceLevels.Warning);
      foreach (Action<IVssRequestContext, SourceLevels> action in array)
      {
        try
        {
          action(requestContext, valueFromPath);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10017100, "SignalR", "ConfigurationService", ex);
        }
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      using (requestContext.Lock(this.m_callbackLock))
        this.m_callbacks.Clear();
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.TraceSettingsChanged));
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_callbackLock = this.CreateLockName(requestContext, "callbacks");
  }
}
