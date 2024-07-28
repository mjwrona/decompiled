// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RequestContextExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class RequestContextExtensions
  {
    private static readonly Guid PipelinesServiceInstanceType = new Guid("0000005a-0000-8888-8000-000000000000");

    public static DateTime GetCurrentCollectionTime(
      this IVssRequestContext requestContext,
      DateTime utcTime)
    {
      TimeZoneInfo destinationTimeZone = requestContext.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(requestContext) ?? (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? TimeZoneInfo.Utc : TimeZoneInfo.Local);
      return TimeZoneInfo.ConvertTimeFromUtc(utcTime, destinationTimeZone);
    }

    public static void AddCIEntry(this IVssRequestContext requestContext, string key, object value)
    {
      try
      {
        CustomerIntelligenceData intelligenceData;
        if (!requestContext.Items.TryGetValue<CustomerIntelligenceData>("Build2.CustomerIntelligenceAttribute.Data", out intelligenceData))
          return;
        intelligenceData.Add(key, value);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (RequestContextExtensions), ex);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable CITimer(this IVssRequestContext requestContext, string key) => (IDisposable) new RequestContextExtensions.CITimerScope(requestContext, key);

    public static bool IsBuildFeatureEnabled(this IVssRequestContext requestContext) => requestContext.IsPipelines() || requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, BuildServerConstants.FeatureContributionId);

    public static bool IsPipelines(this IVssRequestContext requestContext) => requestContext.ServiceInstanceType() == RequestContextExtensions.PipelinesServiceInstanceType;

    public static void UsingBuild2ComponentCall(
      this IVssRequestContext requestContext,
      Action<Build2Component> method)
    {
      using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        method(component);
    }

    public static async Task UsingBuild2ComponentCallAsync(
      this IVssRequestContext requestContext,
      Func<Build2Component, Task> method)
    {
      using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        await method(bc);
    }

    public static T UsingBuild2ComponentCall<T>(
      this IVssRequestContext requestContext,
      Func<Build2Component, T> method)
    {
      using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        return method(component);
    }

    public static async Task<T> UsingBuild2ComponentCallAsync<T>(
      this IVssRequestContext requestContext,
      Func<Build2Component, Task<T>> method)
    {
      T obj;
      using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        obj = await method(bc);
      return obj;
    }

    private struct CITimerScope : IDisposable
    {
      private readonly Stopwatch m_timer;
      private readonly string m_key;
      private readonly IVssRequestContext m_requestContext;

      public CITimerScope(IVssRequestContext requestContext, string key)
      {
        this.m_requestContext = requestContext;
        this.m_key = key;
        this.m_timer = Stopwatch.StartNew();
      }

      public void Dispose()
      {
        this.m_timer.Stop();
        this.m_requestContext.AddCIEntry(this.m_key, (object) this.m_timer.ElapsedMilliseconds);
      }
    }
  }
}
