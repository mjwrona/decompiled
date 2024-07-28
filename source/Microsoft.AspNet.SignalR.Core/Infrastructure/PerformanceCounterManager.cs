// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.PerformanceCounterManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Tracing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class PerformanceCounterManager : IPerformanceCounterManager
  {
    public const string CategoryName = "SignalR";
    private static readonly PropertyInfo[] _counterProperties = PerformanceCounterManager.GetCounterPropertyInfo();
    private static readonly IPerformanceCounter _noOpCounter = (IPerformanceCounter) new NoOpPerformanceCounter();
    private volatile bool _initialized;
    private object _initLocker = new object();
    private readonly TraceSource _trace;

    public PerformanceCounterManager(DefaultDependencyResolver resolver)
      : this(resolver.Resolve<ITraceManager>())
    {
    }

    public PerformanceCounterManager(ITraceManager traceManager)
      : this()
    {
      this._trace = traceManager != null ? traceManager["SignalR.PerformanceCounterManager"] : throw new ArgumentNullException(nameof (traceManager));
    }

    public PerformanceCounterManager() => this.InitNoOpCounters();

    [PerformanceCounter(Name = "Connections Connected", Description = "The total number of connection Connect events since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ConnectionsConnected { get; private set; }

    [PerformanceCounter(Name = "Connections Reconnected", Description = "The total number of connection Reconnect events since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ConnectionsReconnected { get; private set; }

    [PerformanceCounter(Name = "Connections Disconnected", Description = "The total number of connection Disconnect events since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ConnectionsDisconnected { get; private set; }

    [PerformanceCounter(Name = "Connections Current ForeverFrame", Description = "The number of connections currently connected using the ForeverFrame transport.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ConnectionsCurrentForeverFrame { get; private set; }

    [PerformanceCounter(Name = "Connections Current LongPolling", Description = "The number of connections currently connected using the LongPolling transport.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ConnectionsCurrentLongPolling { get; private set; }

    [PerformanceCounter(Name = "Connections Current ServerSentEvents", Description = "The number of connections currently connected using the ServerSentEvents transport.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ConnectionsCurrentServerSentEvents { get; private set; }

    [PerformanceCounter(Name = "Connections Current WebSockets", Description = "The number of connections currently connected using the WebSockets transport.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ConnectionsCurrentWebSockets { get; private set; }

    [PerformanceCounter(Name = "Connections Current", Description = "The number of connections currently connected.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ConnectionsCurrent { get; private set; }

    [PerformanceCounter(Name = "Connection Messages Received Total", Description = "The toal number of messages received by connections (server to client) since the application was started.", CounterType = PerformanceCounterType.NumberOfItems64)]
    public IPerformanceCounter ConnectionMessagesReceivedTotal { get; private set; }

    [PerformanceCounter(Name = "Connection Messages Sent Total", Description = "The total number of messages sent by connections (client to server) since the application was started.", CounterType = PerformanceCounterType.NumberOfItems64)]
    public IPerformanceCounter ConnectionMessagesSentTotal { get; private set; }

    [PerformanceCounter(Name = "Connection Messages Received/Sec", Description = "The number of messages received by connections (server to client) per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter ConnectionMessagesReceivedPerSec { get; private set; }

    [PerformanceCounter(Name = "Connection Messages Sent/Sec", Description = "The number of messages sent by connections (client to server) per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter ConnectionMessagesSentPerSec { get; private set; }

    [PerformanceCounter(Name = "Message Bus Messages Received Total", Description = "The total number of messages received by subscribers since the application was started.", CounterType = PerformanceCounterType.NumberOfItems64)]
    public IPerformanceCounter MessageBusMessagesReceivedTotal { get; private set; }

    [PerformanceCounter(Name = "Message Bus Messages Received/Sec", Description = "The number of messages received by subscribers per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter MessageBusMessagesReceivedPerSec { get; private set; }

    [PerformanceCounter(Name = "Scaleout Message Bus Messages Received/Sec", Description = "The number of messages received by the scaleout message bus per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter ScaleoutMessageBusMessagesReceivedPerSec { get; private set; }

    [PerformanceCounter(Name = "Message Bus Messages Published Total", Description = "The total number of messages published to the message bus since the application was started.", CounterType = PerformanceCounterType.NumberOfItems64)]
    public IPerformanceCounter MessageBusMessagesPublishedTotal { get; private set; }

    [PerformanceCounter(Name = "Message Bus Messages Published/Sec", Description = "The number of messages published to the message bus per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter MessageBusMessagesPublishedPerSec { get; private set; }

    [PerformanceCounter(Name = "Message Bus Subscribers Current", Description = "The current number of subscribers to the message bus.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter MessageBusSubscribersCurrent { get; private set; }

    [PerformanceCounter(Name = "Message Bus Subscribers Total", Description = "The total number of subscribers to the message bus since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter MessageBusSubscribersTotal { get; private set; }

    [PerformanceCounter(Name = "Message Bus Subscribers/Sec", Description = "The number of new subscribers to the message bus per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter MessageBusSubscribersPerSec { get; private set; }

    [PerformanceCounter(Name = "Message Bus Allocated Workers", Description = "The number of workers allocated to deliver messages in the message bus.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter MessageBusAllocatedWorkers { get; private set; }

    [PerformanceCounter(Name = "Message Bus Busy Workers", Description = "The number of workers currently busy delivering messages in the message bus.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter MessageBusBusyWorkers { get; private set; }

    [PerformanceCounter(Name = "Message Bus Topics Current", Description = "The number of topics in the message bus.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter MessageBusTopicsCurrent { get; private set; }

    [PerformanceCounter(Name = "Errors: All Total", Description = "The total number of all errors processed since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ErrorsAllTotal { get; private set; }

    [PerformanceCounter(Name = "Errors: All/Sec", Description = "The number of all errors processed per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter ErrorsAllPerSec { get; private set; }

    [PerformanceCounter(Name = "Errors: Hub Resolution Total", Description = "The total number of hub resolution errors processed since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ErrorsHubResolutionTotal { get; private set; }

    [PerformanceCounter(Name = "Errors: Hub Resolution/Sec", Description = "The number of hub resolution errors per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter ErrorsHubResolutionPerSec { get; private set; }

    [PerformanceCounter(Name = "Errors: Hub Invocation Total", Description = "The total number of hub invocation errors processed since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ErrorsHubInvocationTotal { get; private set; }

    [PerformanceCounter(Name = "Errors: Hub Invocation/Sec", Description = "The number of hub invocation errors per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter ErrorsHubInvocationPerSec { get; private set; }

    [PerformanceCounter(Name = "Errors: Tranport Total", Description = "The total number of transport errors processed since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ErrorsTransportTotal { get; private set; }

    [PerformanceCounter(Name = "Errors: Transport/Sec", Description = "The number of transport errors per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter ErrorsTransportPerSec { get; private set; }

    [PerformanceCounter(Name = "Scaleout Streams Total", Description = "The number of logical streams in the currently configured scaleout message bus provider.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ScaleoutStreamCountTotal { get; private set; }

    [PerformanceCounter(Name = "Scaleout Streams Open", Description = "The number of logical streams in the currently configured scaleout message bus provider that are in the open state", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ScaleoutStreamCountOpen { get; private set; }

    [PerformanceCounter(Name = "Scaleout Streams Buffering", Description = "The number of logical streams in the currently configured scaleout message bus provider that are in the buffering state", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ScaleoutStreamCountBuffering { get; private set; }

    [PerformanceCounter(Name = "Scaleout Errors Total", Description = "The total number of scaleout errors since the application was started.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ScaleoutErrorsTotal { get; private set; }

    [PerformanceCounter(Name = "Scaleout Errors/Sec", Description = "The number of scaleout errors per second.", CounterType = PerformanceCounterType.RateOfCountsPerSecond32)]
    public IPerformanceCounter ScaleoutErrorsPerSec { get; private set; }

    [PerformanceCounter(Name = "Scaleout Send Queue Length", Description = "The current scaleout send queue length.", CounterType = PerformanceCounterType.NumberOfItems32)]
    public IPerformanceCounter ScaleoutSendQueueLength { get; private set; }

    internal string InstanceName { get; private set; }

    public void Initialize(string instanceName, CancellationToken hostShutdownToken)
    {
      if (this._initialized)
        return;
      bool flag = false;
      lock (this._initLocker)
      {
        if (!this._initialized)
        {
          this.InstanceName = PerformanceCounterManager.SanitizeInstanceName(instanceName);
          this.SetCounterProperties();
          if (hostShutdownToken != CancellationToken.None)
            flag = true;
          this._initialized = true;
        }
      }
      if (!flag)
        return;
      hostShutdownToken.Register(new Action(this.UnloadCounters));
    }

    private void UnloadCounters()
    {
      lock (this._initLocker)
      {
        if (!this._initialized)
          return;
      }
      foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>) this.GetType().GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.PropertyType == typeof (IPerformanceCounter))))
      {
        IPerformanceCounter performanceCounter = propertyInfo.GetValue((object) this, (object[]) null) as IPerformanceCounter;
        performanceCounter.Close();
        performanceCounter.RemoveInstance();
      }
    }

    private void InitNoOpCounters()
    {
      foreach (PropertyInfo counterProperty in PerformanceCounterManager._counterProperties)
        counterProperty.SetValue((object) this, (object) new NoOpPerformanceCounter(), (object[]) null);
    }

    private void SetCounterProperties()
    {
      bool flag = true;
      foreach (PropertyInfo counterProperty in PerformanceCounterManager._counterProperties)
      {
        PerformanceCounterAttribute counterAttribute = PerformanceCounterManager.GetPerformanceCounterAttribute(counterProperty);
        if (counterAttribute != null)
        {
          IPerformanceCounter performanceCounter1 = (IPerformanceCounter) null;
          if (flag)
          {
            performanceCounter1 = this.LoadCounter("SignalR", counterAttribute.Name, false);
            if (performanceCounter1 == null)
              flag = false;
          }
          IPerformanceCounter performanceCounter2 = performanceCounter1 ?? PerformanceCounterManager._noOpCounter;
          counterProperty.SetValue((object) this, (object) performanceCounter2, (object[]) null);
        }
      }
    }

    internal static PropertyInfo[] GetCounterPropertyInfo() => ((IEnumerable<PropertyInfo>) typeof (PerformanceCounterManager).GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.PropertyType == typeof (IPerformanceCounter))).ToArray<PropertyInfo>();

    internal static PerformanceCounterAttribute GetPerformanceCounterAttribute(PropertyInfo property) => property.GetCustomAttributes(typeof (PerformanceCounterAttribute), false).Cast<PerformanceCounterAttribute>().SingleOrDefault<PerformanceCounterAttribute>();

    private static string SanitizeInstanceName(string instanceName)
    {
      if (string.IsNullOrWhiteSpace(instanceName))
        instanceName = Guid.NewGuid().ToString();
      Dictionary<char, char> substMap = new Dictionary<char, char>()
      {
        {
          '(',
          '['
        },
        {
          ')',
          ']'
        },
        {
          '#',
          '-'
        },
        {
          '\\',
          '-'
        },
        {
          '/',
          '-'
        }
      };
      string str = new string(instanceName.Select<char, char>((Func<char, char>) (c => !substMap.ContainsKey(c) ? c : substMap[c])).ToArray<char>());
      int maxValue = (int) sbyte.MaxValue;
      return str.Length > maxValue ? str.Substring(0, maxValue) : str;
    }

    private IPerformanceCounter LoadCounter(
      string categoryName,
      string counterName,
      bool isReadOnly)
    {
      return this.LoadCounter(categoryName, counterName, this.InstanceName, isReadOnly);
    }

    public IPerformanceCounter LoadCounter(
      string categoryName,
      string counterName,
      string instanceName,
      bool isReadOnly)
    {
      try
      {
        PerformanceCounter counter = new PerformanceCounter(categoryName, counterName, instanceName, isReadOnly);
        counter.NextSample();
        return (IPerformanceCounter) new PerformanceCounterWrapper(counter);
      }
      catch (InvalidOperationException ex)
      {
        this._trace.TraceEvent(TraceEventType.Error, 0, "Performance counter failed to load: " + (object) ex.GetBaseException());
        return (IPerformanceCounter) null;
      }
      catch (UnauthorizedAccessException ex)
      {
        this._trace.TraceEvent(TraceEventType.Error, 0, "Performance counter failed to load: " + (object) ex.GetBaseException());
        return (IPerformanceCounter) null;
      }
      catch (Win32Exception ex)
      {
        this._trace.TraceEvent(TraceEventType.Error, 0, "Performance counter failed to load: " + (object) ex.GetBaseException());
        return (IPerformanceCounter) null;
      }
      catch (PlatformNotSupportedException ex)
      {
        this._trace.TraceEvent(TraceEventType.Error, 0, "Performance counter failed to load: " + (object) ex.GetBaseException());
        return (IPerformanceCounter) null;
      }
    }
  }
}
