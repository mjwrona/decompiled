// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ExtensionsUtil
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class ExtensionsUtil
  {
    public static IReadOnlyList<T> GetStaticNotificationExtentions<T>(
      this IVssRequestContext requestContext)
    {
      return (IReadOnlyList<T>) requestContext.GetExtensions<T>(ExtensionLifetime.Service);
    }

    public static object CreateNewInstance(object o)
    {
      Type type = o.GetType();
      ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ConstructorInfo constructorInfo1 = (ConstructorInfo) null;
      foreach (ConstructorInfo constructorInfo2 in constructors)
      {
        ParameterInfo[] parameters = constructorInfo2.GetParameters();
        if (parameters.Length == 0)
          constructorInfo1 = constructorInfo2;
        else if (((IEnumerable<ParameterInfo>) parameters).All<ParameterInfo>((Func<ParameterInfo, bool>) (p => p.IsOptional)))
          constructorInfo1 = constructorInfo2;
      }
      return !(constructorInfo1 == (ConstructorInfo) null) ? constructorInfo1.Invoke((object[]) null) : throw new TypeHasNoDefaultConstructor(type.FullName);
    }

    public static bool IsContributedFeatureEnabled(
      this IVssRequestContext requestContext,
      string contributedFeature)
    {
      return requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, contributedFeature);
    }

    public static int SafeGetHashCode<T>(this T obj) => (object) obj == null ? 0 : obj.GetHashCode();

    public static HashSet<string> ToHashSet(this IEnumerable<string> items) => new HashSet<string>(items);

    public static void AddOrUpate<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value)
    {
      if (dictionary == null)
        return;
      dictionary[key] = value;
    }

    public static string EventNameSimplified(this string eventName)
    {
      if (eventName.StartsWith("ms."))
        eventName = eventName.Substring(eventName.LastIndexOf('.'));
      return eventName.Replace('-', '_');
    }

    public static bool ContributionsInFallbackMode(this IVssRequestContext requestContext)
    {
      bool flag = false;
      if (!requestContext.ExecutionEnvironment.IsDevFabricDeployment && !requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        requestContext.RootContext.TryGetItem<bool>("InExtensionFallbackMode", out flag);
      return flag;
    }

    public static void ThrowIfContributionsInFallbackMode(this IVssRequestContext requestContext)
    {
      if (requestContext.ContributionsInFallbackMode())
        throw new ContributionsInFallbackModeException();
    }

    public static void WarnIfContributionsInFallbackMode(
      this IVssRequestContext requestContext,
      [CallerMemberName] string caller = "")
    {
      bool flag;
      if (!requestContext.ContributionsInFallbackMode() || requestContext.TryGetItem<bool>("ContributionServiceInFallbackModeTraced", out flag) && flag)
        return;
      requestContext.Items["ContributionServiceInFallbackModeTraced"] = (object) true;
      requestContext.Trace(1002015, TraceLevel.Warning, "Notifications", "Util", caller + ": Contribution Service is operating in fallback mode");
    }

    public static void TracePerformanceTimers(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer)
    {
      if (!requestContext.IsTracing(tracepoint, level, area, layer))
        return;
      IDictionary<string, PerformanceTimingGroup> allTimings = PerformanceTimer.GetAllTimings(requestContext);
      if (allTimings == null)
        return;
      foreach (KeyValuePair<string, PerformanceTimingGroup> keyValuePair in (IEnumerable<KeyValuePair<string, PerformanceTimingGroup>>) allTimings)
      {
        string key = keyValuePair.Key;
        PerformanceTimingGroup performanceTimingGroup = keyValuePair.Value;
        StringBuilder stringBuilder1 = new StringBuilder();
        StringBuilder stringBuilder2 = stringBuilder1;
        object[] objArray = new object[4]
        {
          (object) key,
          null,
          null,
          null
        };
        int? nullable1;
        int? nullable2;
        if (performanceTimingGroup == null)
        {
          nullable1 = new int?();
          nullable2 = nullable1;
        }
        else
          nullable2 = new int?(performanceTimingGroup.Count);
        objArray[1] = (object) nullable2;
        objArray[2] = (object) performanceTimingGroup?.ElapsedTicks;
        int? nullable3;
        if (performanceTimingGroup == null)
        {
          nullable1 = new int?();
          nullable3 = nullable1;
        }
        else
        {
          List<PerformanceTimingEntry> timings = performanceTimingGroup.Timings;
          if (timings == null)
          {
            nullable1 = new int?();
            nullable3 = nullable1;
          }
          else
          {
            // ISSUE: explicit non-virtual call
            nullable3 = new int?(__nonvirtual (timings.Count));
          }
        }
        objArray[3] = (object) nullable3;
        string str = string.Format("{0} count={1} elapsed={2} timings{3}:", objArray);
        stringBuilder2.Append(str);
        if (performanceTimingGroup != null)
        {
          nullable1 = performanceTimingGroup.Timings?.Count;
          int num = 0;
          if (nullable1.GetValueOrDefault() > num & nullable1.HasValue)
          {
            foreach (PerformanceTimingEntry timing in performanceTimingGroup.Timings)
            {
              stringBuilder1.Append(string.Format("start={0} ticks={1} ", (object) timing.StartOffset, (object) timing.ElapsedTicks));
              IDictionary<string, object> properties = timing.Properties;
              if ((properties != null ? (properties.Count > 0 ? 1 : 0) : 0) != 0)
              {
                foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) timing.Properties)
                  stringBuilder1.Append(string.Format("{0}={1} ", (object) property.Key, property.Value));
              }
            }
          }
        }
        requestContext.Trace(tracepoint, level, area, layer, stringBuilder1.ToString());
      }
    }
  }
}
