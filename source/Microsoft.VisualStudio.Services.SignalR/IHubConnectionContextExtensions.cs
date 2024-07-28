// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.IHubConnectionContextExtensions
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using ImpromptuInterface;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.SignalR
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IHubConnectionContextExtensions
  {
    private static readonly Lazy<ConcurrentDictionary<string, SignalRHubProxyPerformanceCounters>> s_counters = new Lazy<ConcurrentDictionary<string, SignalRHubProxyPerformanceCounters>>((Func<ConcurrentDictionary<string, SignalRHubProxyPerformanceCounters>>) (() => new ConcurrentDictionary<string, SignalRHubProxyPerformanceCounters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
    private static readonly string _layer = "HubConnectionContextExtensions";

    public static object TrackedGroup<T>(
      this IHubConnectionContext<T> context,
      IVssRequestContext requestContext,
      string hubName,
      string groupName)
      where T : class
    {
      VssSignalRHubGroup group = requestContext.GetService<IVssSignalRHubGroupService>().GetGroup(requestContext, hubName, groupName);
      bool flag = requestContext.IsFeatureEnabled("VisualStudio.Services.SignalR.AlwaysPublish");
      if (!flag && group != null && group.Connections.Count == 0)
      {
        requestContext.Trace(10017111, TraceLevel.Verbose, "SignalR", IHubConnectionContextExtensions._layer, "Returning no-op proxy for group: {0} of hub: {1} as there are no active connections", (object) groupName, (object) hubName);
        return typeof (T).IsInterface ? (object) new IHubConnectionContextExtensions.VssTrackingProxy(hubName).ActLike<T>() : (object) new IHubConnectionContextExtensions.VssTrackingProxy(hubName);
      }
      requestContext.Trace(10017111, TraceLevel.Verbose, "SignalR", IHubConnectionContextExtensions._layer, "Returning client proxy for group: {0} of hub: {1}. isAlwaysPublish: {2} connections: {3}", (object) groupName, (object) hubName, (object) flag, (object) (group != null ? group.Connections.Count : 0));
      return typeof (T).IsInterface ? (object) new IHubConnectionContextExtensions.VssTrackingProxy(hubName, (IClientProxy) new IHubConnectionContextExtensions.ReflectedProxy<T>(context.Group(groupName))).ActLike<T>() : (object) new IHubConnectionContextExtensions.VssTrackingProxy(hubName, (IClientProxy) (object) context.Group(groupName));
    }

    private sealed class ReflectedProxy<T> : IClientProxy where T : class
    {
      private readonly T m_client;
      private static readonly Lazy<Dictionary<string, MethodInfo>> s_methods = new Lazy<Dictionary<string, MethodInfo>>(new Func<Dictionary<string, MethodInfo>>(IHubConnectionContextExtensions.ReflectedProxy<T>.LoadMethods));

      public ReflectedProxy(T client)
      {
        this.m_client = client;
        Dictionary<string, MethodInfo> dictionary = IHubConnectionContextExtensions.ReflectedProxy<T>.s_methods.Value;
      }

      public async Task Invoke(string method, params object[] args)
      {
        string key = method + "." + this.m_client.GetType().FullName;
        SignalRHubProxyPerformanceCounters counters = IHubConnectionContextExtensions.s_counters.Value.GetOrAdd(key, (Func<string, SignalRHubProxyPerformanceCounters>) (i => new SignalRHubProxyPerformanceCounters(i)));
        MethodInfo methodInfo;
        if (!IHubConnectionContextExtensions.ReflectedProxy<T>.s_methods.Value.TryGetValue(method, out methodInfo))
        {
          counters.SkippedHubMethodResolutionsPerSec.Increment();
          counters.SkippedHubMethodResolutionsTotal.Increment();
          throw new InvalidOperationException("The method " + method + " is not supported for type " + this.m_client.GetType().FullName);
        }
        counters.HubMethodResolutionsPerSec.Increment();
        VssPerformanceCounter performanceCounter1 = counters.HubMethodResolutionsTotal;
        performanceCounter1.Increment();
        try
        {
          if (methodInfo.Invoke((object) this.m_client, args) is Task task)
            await task;
          performanceCounter1 = counters.HubMethodInvocationsTotal;
          performanceCounter1.Increment();
          performanceCounter1 = counters.HubMethodInvocationsPerSec;
          performanceCounter1.Increment();
        }
        catch (Exception ex)
        {
          VssPerformanceCounter performanceCounter2 = counters.SkippedHubMethodInvocationsTotal;
          performanceCounter2.Increment();
          performanceCounter2 = counters.SkippedHubMethodInvocationsPerSec;
          performanceCounter2.Increment();
          throw;
        }
        counters = (SignalRHubProxyPerformanceCounters) null;
      }

      private static Dictionary<string, MethodInfo> LoadMethods()
      {
        Dictionary<string, MethodInfo> dictionary = new Dictionary<string, MethodInfo>((IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (MethodInfo method in typeof (T).GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
          if (!method.ReturnType.Equals(typeof (void)) && !method.ReturnType.Equals(typeof (Task)))
            throw new NotSupportedException("The method " + method.Name + " uses an unsupported signature. The return type must be void or Task.");
          dictionary.Add(method.Name, method);
        }
        return dictionary;
      }
    }

    private sealed class VssTrackingProxy : DynamicObject
    {
      private readonly string m_hubName;
      private readonly IClientProxy m_innerProxy;

      public VssTrackingProxy(string hubName)
        : this(hubName, (IClientProxy) null)
      {
      }

      public VssTrackingProxy(string hubName, IClientProxy innerProxy)
      {
        this.m_hubName = hubName;
        this.m_innerProxy = innerProxy;
      }

      public override bool TryGetMember(GetMemberBinder binder, out object result)
      {
        result = (object) null;
        return false;
      }

      public override bool TryInvokeMember(
        InvokeMemberBinder binder,
        object[] args,
        out object result)
      {
        string key = this.m_hubName + "." + binder.Name;
        SignalRHubProxyPerformanceCounters orAdd = IHubConnectionContextExtensions.s_counters.Value.GetOrAdd(key, (Func<string, SignalRHubProxyPerformanceCounters>) (i => new SignalRHubProxyPerformanceCounters(i)));
        if (this.m_innerProxy != null)
        {
          orAdd.HubMethodResolutionsPerSec.Increment();
          VssPerformanceCounter performanceCounter = orAdd.HubMethodResolutionsTotal;
          performanceCounter.Increment();
          try
          {
            result = (object) this.m_innerProxy.Invoke(binder.Name, args);
            performanceCounter = orAdd.HubMethodInvocationsPerSec;
            performanceCounter.Increment();
            performanceCounter = orAdd.HubMethodInvocationsTotal;
            performanceCounter.Increment();
          }
          catch (Exception ex)
          {
            orAdd.SkippedHubMethodInvocationsPerSec.Increment();
            orAdd.SkippedHubMethodInvocationsTotal.Increment();
            result = (object) null;
            return false;
          }
        }
        else
        {
          orAdd.SkippedHubMethodResolutionsPerSec.Increment();
          orAdd.SkippedHubMethodResolutionsTotal.Increment();
          result = (object) Task.CompletedTask;
        }
        return true;
      }
    }
  }
}
