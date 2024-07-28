// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityCacheWarmer
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityCacheWarmer
  {
    private const string s_area = "IdentityCacheWarmup";
    private const string s_layer = "IdentityCacheWarmup";
    private const string s_retryOnErrorAttemptsRegistryKey = "/Configuration/Identity/CacheWarmupTask/RetryOnErrorAttempts";
    private const string s_unconditionalRetryAttemptsRegistryKey = "/Configuration/Identity/CacheWarmupTask/UnconditionalRetryAttempts";
    internal static readonly TimeSpan ScheduleIntervalDefault = TimeSpan.FromMinutes(5.0);
    private static readonly Guid s_expandedDownCacheWarmerTaskId = new Guid("DCA7CA57-BDBB-4ADD-A1F5-8259FC59A4E4");
    internal const string ScheduleIntervalRegistryPath = "/Configuration/Identity/CacheWarmupTask/ScheduleInterval";
    internal const string FeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache.Warmup";

    internal static void WarmUpExpandedDownCaches(
      IVssRequestContext requestContext,
      object taskArgs)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache.Warmup"))
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Configuration/Identity/CacheWarmupTask/RetryOnErrorAttempts";
      ref RegistryQuery local1 = ref registryQuery;
      int num1 = service.GetValue<int>(requestContext1, in local1, 0);
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) "/Configuration/Identity/CacheWarmupTask/UnconditionalRetryAttempts";
      ref RegistryQuery local2 = ref registryQuery;
      int num2 = service.GetValue<int>(requestContext2, in local2, 0);
      IdentityCacheWarmer.WarmUpExpandedDownCacheRequestContext taskArgs1 = new IdentityCacheWarmer.WarmUpExpandedDownCacheRequestContext()
      {
        RetryOnErrorAttempts = num1,
        UnconditionalRetryAttempts = num2
      };
      requestContext.Trace(589025, TraceLevel.Info, "IdentityCacheWarmup", "IdentityCacheWarmup", "Scheduling task to warm IMS expanded down cache for host {0}", (object) requestContext.ServiceHost);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      requestContext.GetService<LongRunningTaskService>().ScheduleLongRunningTask(requestContext, IdentityCacheWarmer.s_expandedDownCacheWarmerTaskId, IdentityCacheWarmer.\u003C\u003EO.\u003C0\u003E__WarmUpExpandedDownCachesInternal ?? (IdentityCacheWarmer.\u003C\u003EO.\u003C0\u003E__WarmUpExpandedDownCachesInternal = new TeamFoundationTaskCallback(IdentityCacheWarmer.WarmUpExpandedDownCachesInternal)), (object) taskArgs1);
    }

    internal static void WarmUpExpandedDownCachesInternal(
      IVssRequestContext accountContext,
      object taskArgs)
    {
      using (accountContext.TraceBlock(589031, 589039, "IdentityCacheWarmup", "IdentityCacheWarmup", nameof (WarmUpExpandedDownCachesInternal)))
      {
        IdentityCacheWarmer.WarmUpExpandedDownCacheRequestContext var = taskArgs as IdentityCacheWarmer.WarmUpExpandedDownCacheRequestContext;
        ArgumentUtility.CheckForNull<IdentityCacheWarmer.WarmUpExpandedDownCacheRequestContext>(var, "requestOptions");
        int unconditionalRetryAttempts = var.UnconditionalRetryAttempts;
        int retryOnErrorAttempts = var.RetryOnErrorAttempts;
        IdentityService service = accountContext.GetService<IdentityService>();
        List<IdentityCacheWarmer.Result> resultList = new List<IdentityCacheWarmer.Result>();
        for (int index = 0; index <= unconditionalRetryAttempts; ++index)
        {
          bool flag = false;
          int num1 = 0;
          while (!flag && num1 <= retryOnErrorAttempts)
          {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
              stopwatch.Restart();
              Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(accountContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
              {
                GroupWellKnownIdentityDescriptors.EveryoneGroup
              }, QueryMembership.ExpandedDown, (IEnumerable<string>) null)[0];
              stopwatch.Stop();
              int num2 = 0;
              int num3 = 0;
              int num4 = 0;
              int num5 = 0;
              foreach (IdentityDescriptor member in (IEnumerable<IdentityDescriptor>) readIdentity.Members)
              {
                if (member == (IdentityDescriptor) null)
                  ++num2;
                else if (AadIdentityHelper.IsAadGroup(member))
                  ++num4;
                else if (AadIdentityHelper.IsTfsGroup(member))
                  ++num3;
                else
                  ++num5;
              }
              resultList.Add(new IdentityCacheWarmer.Result()
              {
                VsoGroups = num3,
                AadGroups = num4,
                Rest = num5,
                Unknowns = num2,
                TimeTakenInMilliseconds = stopwatch.ElapsedMilliseconds
              });
              flag = true;
            }
            catch (Exception ex)
            {
              if (stopwatch.IsRunning)
                stopwatch.Stop();
              resultList.Add(new IdentityCacheWarmer.Result()
              {
                TimeTakenInMilliseconds = stopwatch.ElapsedMilliseconds,
                Exception = ex
              });
              ++num1;
            }
          }
        }
        string empty = string.Empty;
        foreach (IdentityCacheWarmer.Result result in resultList)
        {
          if (result.Exception == null)
          {
            int num = result.Unknowns + result.AadGroups + result.VsoGroups + result.Rest;
            empty += string.Format("Found {0} members; {1} Aad groups; {2} VSTS groups; {3} rest. It took {4} milliseconds.\n", (object) num, (object) result.AadGroups, (object) result.VsoGroups, (object) result.Rest, (object) result.TimeTakenInMilliseconds);
          }
          else
          {
            empty += string.Format("Failed with exception message: {0}. It took {1} milliseconds.\n", (object) result.Exception.Message, (object) result.TimeTakenInMilliseconds);
            accountContext.TraceException(589038, "IdentityCacheWarmup", "IdentityCacheWarmup", result.Exception);
          }
        }
        accountContext.Trace(589037, TraceLevel.Info, "IdentityCacheWarmup", "IdentityCacheWarmup", empty);
        var.ResultMessage = empty;
      }
    }

    private class Result
    {
      internal int Unknowns { get; set; }

      internal int VsoGroups { get; set; }

      internal int AadGroups { get; set; }

      internal int Rest { get; set; }

      internal long TimeTakenInMilliseconds { get; set; }

      internal Exception Exception { get; set; }
    }

    internal class WarmUpExpandedDownCacheRequestContext
    {
      internal int RetryOnErrorAttempts { get; set; }

      internal int UnconditionalRetryAttempts { get; set; }

      internal string ResultMessage { get; set; }
    }
  }
}
