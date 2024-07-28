// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusDefaultSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class ServiceBusDefaultSettings
  {
    public const int DefaultTopicMaxSizeInGBForTest = 1;
    public const int DefaultTopicMaxSizeInGB = 5;
    public const int DefaultTopicMaxSizeInGBForLargeTopic = 80;
    public const int DefaultSubscriberAlertInSeconds = 3600;
    public const string DefaultServiceBusSharedAccessKeySettingName = "ServiceBusManagementSharedAccessKeyValue";
    public const int DefaultUnprocessedMessageCount = 10000;
    internal const int DefaultMessageTimeToLive = 1440;
    internal const int DefaultMessageTimeToLiveTest = 60;
    public static readonly TimeSpan DefaultIdlePeriod = TimeSpan.MaxValue;
    public static readonly TimeSpan DefaultIdlePeriodForTestForRollingRuns = TimeSpan.FromHours(1.0);
    public static readonly TimeSpan DefaultIdlePeriodForTestForDev = TimeSpan.FromDays(5.0);
    public static readonly TimeSpan DefaultSubscriptionIdlePeriod = TimeSpan.FromDays(7.0);
    public static readonly TimeSpan DefaultSubscriptionIdlePeriodForTest = TimeSpan.FromDays(1.0);
    public static readonly TimeSpan DefaultSubscriptionIdlePeriodReducedDeleteOnIdleTime = TimeSpan.FromDays(1.0);

    public static int GetDefaultTopicMaxSizeInGB(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      if (ServiceBusWellKnownNamespaces.IsLargeTopicSizePremiumNamespace(requestContext, serviceBusNamespace))
        return 80;
      return ServiceBusWellKnownNamespaces.IsTestNamespace(requestContext, serviceBusNamespace) ? 1 : 5;
    }

    public static bool GetDefaultPrefixComputerName(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      return ServiceBusWellKnownNamespaces.IsTestNamespace(requestContext, serviceBusNamespace);
    }

    public static TimeSpan GetDefaultIdlePeriod(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      if (!ServiceBusWellKnownNamespaces.IsTestNamespace(requestContext, serviceBusNamespace))
        return ServiceBusDefaultSettings.DefaultIdlePeriod;
      return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Release_DefinitionId")) ? ServiceBusDefaultSettings.DefaultIdlePeriodForTestForDev : ServiceBusDefaultSettings.DefaultIdlePeriodForTestForRollingRuns;
    }

    public static TimeSpan GetDefaultSubscriptionIdlePeriod(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      if (ServiceBusWellKnownNamespaces.IsTestNamespace(requestContext, serviceBusNamespace))
        return ServiceBusDefaultSettings.DefaultSubscriptionIdlePeriodForTest;
      return ServiceBusWellKnownNamespaces.IsNamespaceWithReducedDeleteOnIdleTimeForSubscriptions(requestContext, serviceBusNamespace) ? ServiceBusDefaultSettings.DefaultSubscriptionIdlePeriodReducedDeleteOnIdleTime : ServiceBusDefaultSettings.DefaultSubscriptionIdlePeriod;
    }

    public static double GetDefaultMessageTimeToLive(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      return ServiceBusWellKnownNamespaces.IsTestNamespace(requestContext, serviceBusNamespace) ? 60.0 : 1440.0;
    }
  }
}
