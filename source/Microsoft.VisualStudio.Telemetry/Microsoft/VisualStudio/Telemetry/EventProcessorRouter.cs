// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.EventProcessorRouter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class EventProcessorRouter : 
    TelemetryDisposableObject,
    IEventProcessorRouter,
    IDisposable
  {
    private const string ChannelUsedProperty = "ChannelUsed";
    private readonly IPersistentPropertyBag persistentPropertyBag;
    private Stopwatch disposeLatencyTimer;
    private EventProcessorRouter.RouteInformation[] routeInformation = new EventProcessorRouter.RouteInformation[0];
    private Dictionary<string, int> channelMapping = new Dictionary<string, int>();
    private ConcurrentBag<ISessionChannel> channelList = new ConcurrentBag<ISessionChannel>();

    public EventProcessorRouter(IPersistentPropertyBag persistentPropertyBag)
    {
      persistentPropertyBag.RequiresArgumentNotNull<IPersistentPropertyBag>(nameof (persistentPropertyBag));
      this.persistentPropertyBag = persistentPropertyBag;
    }

    public void Reset()
    {
      for (int index = this.routeInformation.Length - 1; index >= 0; --index)
      {
        this.routeInformation[index].IsChannelAvailable = false;
        this.routeInformation[index].IsChannelDisabled = false;
        this.routeInformation[index].RouteArgs.Clear();
      }
    }

    public bool TryGetRouteArgument(
      string channelId,
      out IEnumerable<ITelemetryManifestRouteArgs> routeArguments)
    {
      int index;
      if (this.channelMapping.TryGetValue(channelId, out index) && this.routeInformation[index].IsChannelAvailable)
      {
        routeArguments = (IEnumerable<ITelemetryManifestRouteArgs>) this.routeInformation[index].RouteArgs;
        return true;
      }
      routeArguments = (IEnumerable<ITelemetryManifestRouteArgs>) null;
      return false;
    }

    public bool TryAddRouteArgument(string channelId, ITelemetryManifestRouteArgs routeArguments)
    {
      int index;
      if (!this.channelMapping.TryGetValue(channelId, out index))
        return false;
      this.routeInformation[index].IsChannelAvailable = true;
      this.routeInformation[index].RouteArgs.Add(routeArguments);
      return true;
    }

    public void DisableChannel(string channelId)
    {
      int index;
      if (!this.channelMapping.TryGetValue(channelId, out index))
        return;
      this.routeInformation[index].IsChannelDisabled = true;
    }

    public bool IsChannelDisabled(string channelId)
    {
      int index;
      return this.channelMapping.TryGetValue(channelId, out index) && this.routeInformation[index].IsChannelDisabled;
    }

    public void AddChannel(ISessionChannel channel)
    {
      channel.RequiresArgumentNotNull<ISessionChannel>(nameof (channel));
      this.channelList.Add(channel);
      this.OnUpdateChannelList();
    }

    public void RouteEvent(TelemetryEvent telemetryEvent, string sessionId, bool isDropped)
    {
      List<Tuple<ISessionChannel, IEnumerable<ITelemetryManifestRouteArgs>>> source = new List<Tuple<ISessionChannel, IEnumerable<ITelemetryManifestRouteArgs>>>();
      foreach (ISessionChannel channel in this.channelList)
      {
        bool flag = (channel.Properties & ChannelProperties.DevChannel) != 0;
        if (flag || !isDropped && !this.IsChannelDisabled(channel.ChannelId))
        {
          IEnumerable<ITelemetryManifestRouteArgs> routeArguments = (IEnumerable<ITelemetryManifestRouteArgs>) null;
          bool routeArgument = this.TryGetRouteArgument(channel.ChannelId, out routeArguments);
          if (flag | routeArgument || (channel.Properties & (ChannelProperties.Default | ChannelProperties.Test | ChannelProperties.DevChannel)) != ChannelProperties.None)
          {
            if (!channel.IsStarted)
              channel.Start(sessionId);
            source.Add(Tuple.Create<ISessionChannel, IEnumerable<ITelemetryManifestRouteArgs>>(channel, routeArguments));
          }
        }
      }
      if (source.Count <= 0)
        return;
      telemetryEvent.Properties["Reserved.ChannelUsed"] = (object) source.Where<Tuple<ISessionChannel, IEnumerable<ITelemetryManifestRouteArgs>>>((Func<Tuple<ISessionChannel, IEnumerable<ITelemetryManifestRouteArgs>>, bool>) (item => (item.Item1.Properties & ChannelProperties.DevChannel) == ChannelProperties.None)).Select<Tuple<ISessionChannel, IEnumerable<ITelemetryManifestRouteArgs>>, string>((Func<Tuple<ISessionChannel, IEnumerable<ITelemetryManifestRouteArgs>>, string>) (item => item.Item1.TransportUsed)).Join(",");
      foreach (Tuple<ISessionChannel, IEnumerable<ITelemetryManifestRouteArgs>> tuple in source)
      {
        if (tuple.Item2 == null)
          tuple.Item1.PostEvent(telemetryEvent);
        else
          tuple.Item1.PostEvent(telemetryEvent, tuple.Item2);
      }
    }

    public async Task DisposeAndTransmitAsync(CancellationToken token)
    {
      this.DisposeStart();
      List<Task> taskList = new List<Task>();
      foreach (ISessionChannel channel in this.channelList)
      {
        if (channel is IDisposeAndTransmit)
          taskList.Add(((IDisposeAndTransmit) channel).DisposeAndTransmitAsync(token));
      }
      foreach (ISessionChannel channel in this.channelList)
      {
        if (!(channel is IDisposeAndTransmit) && channel is IDisposable)
          ((IDisposable) channel).Dispose();
      }
      await Task.WhenAll((IEnumerable<Task>) taskList).ConfigureAwait(false);
      this.DisposeEnd();
    }

    protected override void DisposeManagedResources()
    {
      this.DisposeStart();
      foreach (ISessionChannel channel in this.channelList)
      {
        if (channel is IDisposable)
          ((IDisposable) channel).Dispose();
      }
      this.DisposeEnd();
    }

    private void DisposeStart()
    {
      base.DisposeManagedResources();
      this.disposeLatencyTimer = new Stopwatch();
      this.disposeLatencyTimer.Start();
    }

    private void DisposeEnd()
    {
      this.persistentPropertyBag.SetProperty("VS.TelemetryApi.ChannelsDisposeLatency", (int) this.disposeLatencyTimer.ElapsedMilliseconds);
      this.RemoveAllChannels();
    }

    private void RemoveAllChannels()
    {
      this.channelList = new ConcurrentBag<ISessionChannel>();
      this.OnUpdateChannelList();
    }

    private void OnUpdateChannelList()
    {
      this.routeInformation = new EventProcessorRouter.RouteInformation[this.channelList.Count<ISessionChannel>()];
      this.channelMapping = new Dictionary<string, int>();
      int index = 0;
      foreach (ISessionChannel channel in this.channelList)
      {
        this.routeInformation[index].RouteArgs = new List<ITelemetryManifestRouteArgs>();
        this.channelMapping[channel.ChannelId] = index++;
      }
    }

    public void UpdateDefaultChannel(bool useCollector)
    {
      string key = "collector";
      if (useCollector)
      {
        this.DisableChannel("aivortex");
      }
      else
      {
        this.DisableChannel("collector");
        key = "aivortex";
      }
      int index;
      if (!this.channelMapping.TryGetValue(key, out index))
        return;
      this.routeInformation[index].IsChannelDisabled = false;
      this.routeInformation[index].IsChannelAvailable = true;
    }

    private struct RouteInformation
    {
      public bool IsChannelAvailable;
      public bool IsChannelDisabled;
      public List<ITelemetryManifestRouteArgs> RouteArgs;
    }
  }
}
