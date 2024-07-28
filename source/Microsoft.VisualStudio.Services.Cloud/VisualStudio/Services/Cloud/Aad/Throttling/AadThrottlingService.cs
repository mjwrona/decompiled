// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Aad.Throttling.AadThrottlingService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.Aad.Throttling
{
  internal class AadThrottlingService : IVssFrameworkService
  {
    private const string GlobalThrottlingKey = "AadThrottlingService_GlobalThrottlingKey";
    private const string TraceArea = "AzureActiveDirectory";
    private const string TraceLayer = "AadThrottlingService";
    private Dictionary<AadServiceType, AadThrottlingCache> AadThrottlingCaches;
    private Dictionary<AadServiceType, AadThrottlingConfiguration> AadThrottlingConfigurations;
    private Random randomTime = new Random();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/AadThrottlingService/...");
      Interlocked.CompareExchange<Dictionary<AadServiceType, AadThrottlingConfiguration>>(ref this.AadThrottlingConfigurations, this.InitAadThrottlingConfigurations(systemRequestContext), (Dictionary<AadServiceType, AadThrottlingConfiguration>) null);
      Interlocked.CompareExchange<Dictionary<AadServiceType, AadThrottlingCache>>(ref this.AadThrottlingCaches, this.InitAadThrottlingCaches(systemRequestContext), (Dictionary<AadServiceType, AadThrottlingCache>) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public T Execute<T>(
      IVssRequestContext requestContext,
      string tenantId,
      AadServiceType aadServiceType,
      Func<T> run,
      Func<Exception, AadThrottleInfo> getThrottlingInfo)
    {
      requestContext.TraceEnter(9002600, "AzureActiveDirectory", nameof (AadThrottlingService), nameof (Execute));
      if (!this.GetThrottlingConfiguration(aadServiceType).IsEnabled)
      {
        requestContext.Trace(9002601, TraceLevel.Warning, "AzureActiveDirectory", nameof (AadThrottlingService), "Aad Service {0} throttling has been Skipped", (object) aadServiceType);
        return run();
      }
      DateTime retryAfterDateTime1;
      if (this.IsTenantIdThrottled(tenantId, aadServiceType, out retryAfterDateTime1))
        throw new AadThrottlingException(HostingResources.AadSpecialTenantBeenThrottled((object) tenantId, (object) aadServiceType, (object) retryAfterDateTime1), retryAfterDateTime1);
      if (this.IsTenantIdThrottled("AadThrottlingService_GlobalThrottlingKey", aadServiceType, out retryAfterDateTime1))
        throw new AadThrottlingException(HostingResources.AadGloballyBeenThrottled((object) aadServiceType, (object) retryAfterDateTime1), retryAfterDateTime1);
      try
      {
        return run();
      }
      catch (Exception ex)
      {
        AadThrottleInfo aadThrottleInfo = getThrottlingInfo(ex);
        if (aadThrottleInfo != null && aadThrottleInfo.IsThrottled)
        {
          TimeSpan expireTimeSpan = !aadThrottleInfo.RetryAfter.HasValue || !(aadThrottleInfo.RetryAfter.Value > TimeSpan.Zero) ? this.GetThrottlingConfiguration(aadServiceType).DefaultThrottlingTimeSpan : aadThrottleInfo.RetryAfter.Value;
          TimeSpan throttlingTimeSpan = this.GetThrottlingConfiguration(aadServiceType).MaxThrottlingTimeSpan;
          if (expireTimeSpan > throttlingTimeSpan)
            expireTimeSpan = throttlingTimeSpan;
          DateTime retryAfterDateTime2 = DateTime.UtcNow.Add(expireTimeSpan);
          this.EnableTenantThrottling(tenantId, aadServiceType, expireTimeSpan);
          requestContext.TraceAlways(9002602, TraceLevel.Warning, "AzureActiveDirectory", nameof (AadThrottlingService), "tenant Id {0} for Aad Service Type {1} throttling has been Enabled", (object) tenantId, (object) aadServiceType);
          if (this.SetGlobalThrottlingIfLimitReached(aadServiceType, expireTimeSpan))
          {
            requestContext.TraceAlways(9002603, TraceLevel.Warning, "AzureActiveDirectory", nameof (AadThrottlingService), "Global throttling for api {0} has been Enabled", (object) aadServiceType);
            throw new AadThrottlingException(HostingResources.AadSpecialTenantBeenThrottled((object) tenantId, (object) aadServiceType, (object) retryAfterDateTime2), retryAfterDateTime2, ex);
          }
          throw new AadThrottlingException(HostingResources.AadSpecialTenantBeenThrottled((object) tenantId, (object) aadServiceType, (object) retryAfterDateTime2), retryAfterDateTime2, ex);
        }
        throw;
      }
      finally
      {
        requestContext.TraceLeave(9002609, "AzureActiveDirectory", nameof (AadThrottlingService), nameof (Execute));
      }
    }

    internal void EnableTenantThrottling(
      string tenantId,
      AadServiceType aadServiceType,
      TimeSpan expireTimeSpan)
    {
      AadThrottlingCache throttlingList = this.GetThrottlingList(aadServiceType);
      TimeSpan timeSpan = expireTimeSpan.Add(TimeSpan.FromSeconds((double) this.randomTime.Next(1, 5)));
      string tenantId1 = tenantId;
      TimeSpan expireTimeSpan1 = timeSpan;
      throttlingList.Set(tenantId1, expireTimeSpan1);
    }

    internal bool IsTenantIdThrottled(
      string tenantId,
      AadServiceType aadServiceType,
      out DateTime retryAfterDateTime)
    {
      retryAfterDateTime = DateTime.MinValue;
      DateTime throttlingExpireTime;
      if (!this.GetThrottlingList(aadServiceType).TryGetValue(tenantId, out throttlingExpireTime))
        return false;
      retryAfterDateTime = throttlingExpireTime;
      return true;
    }

    private AadThrottlingConfiguration GetThrottlingConfiguration(AadServiceType aadServiceType)
    {
      AadThrottlingConfiguration throttlingConfiguration;
      if (this.AadThrottlingConfigurations.TryGetValue(aadServiceType, out throttlingConfiguration))
        return throttlingConfiguration;
      throw new ArgumentException(string.Format("Unrecognized AadServiceType {0}", (object) aadServiceType), nameof (aadServiceType));
    }

    private AadThrottlingCache GetThrottlingList(AadServiceType aadServiceType)
    {
      AadThrottlingCache throttlingList;
      if (this.AadThrottlingCaches.TryGetValue(aadServiceType, out throttlingList))
        return throttlingList;
      throw new ArgumentException(string.Format("Unrecognized AadServiceType {0}", (object) aadServiceType), nameof (aadServiceType));
    }

    private Dictionary<AadServiceType, AadThrottlingCache> InitAadThrottlingCaches(
      IVssRequestContext systemRequestContext)
    {
      Dictionary<AadServiceType, AadThrottlingCache> dictionary = new Dictionary<AadServiceType, AadThrottlingCache>();
      foreach (AadServiceType aadServiceType in Enum.GetValues(typeof (AadServiceType)).Cast<AadServiceType>())
        dictionary[aadServiceType] = new AadThrottlingCache(systemRequestContext, this.GetThrottlingConfiguration(aadServiceType).GlobalThrottlingLimit * 2, this.GetThrottlingConfiguration(aadServiceType).CleanupInterval);
      return dictionary;
    }

    private Dictionary<AadServiceType, AadThrottlingConfiguration> InitAadThrottlingConfigurations(
      IVssRequestContext systemRequestContext)
    {
      Dictionary<AadServiceType, AadThrottlingConfiguration> dictionary = new Dictionary<AadServiceType, AadThrottlingConfiguration>();
      foreach (AadServiceType aadServiceType in Enum.GetValues(typeof (AadServiceType)).Cast<AadServiceType>())
        dictionary[aadServiceType] = AadThrottlingConfiguration.GetAadThrottlingConfiguration(systemRequestContext, aadServiceType);
      return dictionary;
    }

    private void OnRegistryChanged(
      IVssRequestContext requestcontext,
      RegistryEntryCollection changedentries)
    {
      Dictionary<AadServiceType, AadThrottlingConfiguration> oldconfigurations = Interlocked.CompareExchange<Dictionary<AadServiceType, AadThrottlingConfiguration>>(ref this.AadThrottlingConfigurations, this.InitAadThrottlingConfigurations(requestcontext), this.AadThrottlingConfigurations);
      foreach (KeyValuePair<AadServiceType, AadThrottlingConfiguration> keyValuePair in oldconfigurations)
      {
        AadThrottlingConfiguration throttlingConfiguration;
        if (this.AadThrottlingConfigurations.TryGetValue(keyValuePair.Key, out throttlingConfiguration) && throttlingConfiguration.GlobalThrottlingLimit != keyValuePair.Value.GlobalThrottlingLimit)
        {
          Interlocked.CompareExchange<Dictionary<AadServiceType, AadThrottlingCache>>(ref this.AadThrottlingCaches, this.InitAadThrottlingCaches(requestcontext), this.AadThrottlingCaches);
          break;
        }
      }
      foreach (KeyValuePair<AadServiceType, AadThrottlingConfiguration> throttlingConfiguration in this.AadThrottlingConfigurations)
      {
        IReadOnlyDictionary<string, TimeSpan> throttlingBlackList = throttlingConfiguration.Value.ThrottlingBlackList;
        if ((throttlingBlackList != null ? (throttlingBlackList.Count > 0 ? 1 : 0) : 0) != 0)
        {
          foreach (KeyValuePair<string, TimeSpan> throttlingBlack in (IEnumerable<KeyValuePair<string, TimeSpan>>) throttlingConfiguration.Value.ThrottlingBlackList)
          {
            if (!AadThrottlingService.IsTenantIdAlreadyThrottledBefore(oldconfigurations, throttlingConfiguration.Key, throttlingBlack))
              this.EnableTenantThrottling(throttlingBlack.Key, throttlingConfiguration.Key, throttlingBlack.Value);
          }
        }
      }
    }

    private static bool IsTenantIdAlreadyThrottledBefore(
      Dictionary<AadServiceType, AadThrottlingConfiguration> oldconfigurations,
      AadServiceType aadServiceType,
      KeyValuePair<string, TimeSpan> newThrottlingInfo)
    {
      AadThrottlingConfiguration throttlingConfiguration;
      TimeSpan timeSpan;
      return oldconfigurations.TryGetValue(aadServiceType, out throttlingConfiguration) && throttlingConfiguration?.ThrottlingBlackList != null && throttlingConfiguration.ThrottlingBlackList.TryGetValue(newThrottlingInfo.Key, out timeSpan) && timeSpan == newThrottlingInfo.Value;
    }

    private bool SetGlobalThrottlingIfLimitReached(
      AadServiceType aadServiceType,
      TimeSpan expireTimeSpan)
    {
      AadThrottlingCache throttlingList = this.GetThrottlingList(aadServiceType);
      if (throttlingList.Count >= this.GetThrottlingConfiguration(aadServiceType).GlobalThrottlingLimit)
      {
        throttlingList.Sweep();
        if (throttlingList.Count >= this.GetThrottlingConfiguration(aadServiceType).GlobalThrottlingLimit)
        {
          TimeSpan expireTimeSpan1 = expireTimeSpan.Add(TimeSpan.FromSeconds((double) this.randomTime.Next(1, 5)));
          throttlingList.Set("AadThrottlingService_GlobalThrottlingKey", expireTimeSpan1);
          return true;
        }
      }
      return false;
    }
  }
}
