// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.ServiceModelActivity
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class ServiceModelActivity : IDisposable
  {
    internal ActivityType ActivityType => ActivityType.Unknown;

    internal static ServiceModelActivity Current => (ServiceModelActivity) null;

    internal static Activity BoundOperation(ServiceModelActivity activity) => (Activity) null;

    internal static Activity BoundOperation(ServiceModelActivity activity, bool addTransfer) => (Activity) null;

    internal static ServiceModelActivity CreateActivity() => (ServiceModelActivity) null;

    internal static ServiceModelActivity CreateActivity(bool autoStop) => (ServiceModelActivity) null;

    internal static ServiceModelActivity CreateActivity(
      bool autoStop,
      string activityName,
      ActivityType activityType)
    {
      return (ServiceModelActivity) null;
    }

    internal static ServiceModelActivity CreateBoundedActivity() => (ServiceModelActivity) null;

    internal static ServiceModelActivity CreateBoundedActivity(bool suspendCurrent) => (ServiceModelActivity) null;

    internal static ServiceModelActivity CreateBoundedActivity(Guid activityId) => (ServiceModelActivity) null;

    internal static ServiceModelActivity CreateBoundedActivityWithTransferInOnly(Guid activityId) => (ServiceModelActivity) null;

    public void Dispose() => GC.SuppressFinalize((object) this);

    internal void Resume()
    {
    }

    internal void Resume(string activityName)
    {
    }

    internal static void Start(
      ServiceModelActivity activity,
      string activityName,
      ActivityType activityType)
    {
    }

    internal void Stop()
    {
    }

    internal static void Stop(ServiceModelActivity activity)
    {
    }

    internal void Suspend()
    {
    }
  }
}
