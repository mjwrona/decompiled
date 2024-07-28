// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.EventSubscribers.AnalyticsEventSubscriberBase
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Analytics.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Analytics.Plugins.EventSubscribers
{
  public abstract class AnalyticsEventSubscriberBase : ISubscriber
  {
    private const string MaxJobYieldDelaySecondsPathTemplate = "/Service/Analytics/EventSubscribers/{0}/MaxJobYieldDelaySeconds";
    private const string MinimumDelayBetweenSchedulesInSecondsPathTemplate = "/Service/Analytics/EventSubscribers/{0}/MinimumDelayBetweenSchedulesInSeconds";
    private const string TimeWhenLatestJobScheduledTemplatePath = "/Service/Analytics/EventSubscribers/{0}/TimeWhenLatestJobScheduled";
    private readonly string _timeWhenLatestJobScheduledPath;
    private readonly string _maxJobYieldDelaySecondsPath;
    private readonly string _minimumDelayBetweenSchedulesInSecondsPath;
    private readonly string _name;
    private readonly string _featureName;

    public virtual string Name => this._name;

    public virtual SubscriberPriority Priority => SubscriberPriority.Normal;

    public virtual string FeatureName => this._featureName;

    public virtual int DefaultMaxJobYieldDelaySeconds { get; } = 3;

    public virtual int DefaultMinimumDelayBetweenSchedulesInSeconds { get; }

    public abstract Type[] EventTypes { get; }

    public abstract Guid[] JobIds { get; }

    public AnalyticsEventSubscriberBase()
    {
      this._name = this.GetType().Name;
      this._featureName = "Analytics." + this._name;
      this._maxJobYieldDelaySecondsPath = string.Format("/Service/Analytics/EventSubscribers/{0}/MaxJobYieldDelaySeconds", (object) this.Name);
      this._minimumDelayBetweenSchedulesInSecondsPath = string.Format("/Service/Analytics/EventSubscribers/{0}/MinimumDelayBetweenSchedulesInSeconds", (object) this.Name);
      this._timeWhenLatestJobScheduledPath = string.Format("/Service/Analytics/EventSubscribers/{0}/TimeWhenLatestJobScheduled", (object) this.Name);
    }

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      if (this.NotificationEventIsMatch(notificationEventArgs) && (string.IsNullOrEmpty(this.FeatureName) || requestContext.IsFeatureEnabled(this.FeatureName)))
      {
        if (requestContext.GetService<IAnalyticsFeatureService>().IsAnalyticsEnabled(requestContext))
        {
          try
          {
            this.PreProcessEvent(requestContext, notificationEventArgs);
            IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
            int num1 = service1.GetValue<int>(requestContext, (RegistryQuery) this._maxJobYieldDelaySecondsPath, this.DefaultMaxJobYieldDelaySeconds);
            int num2 = service1.GetValue<int>(requestContext, (RegistryQuery) this._minimumDelayBetweenSchedulesInSecondsPath, this.DefaultMinimumDelayBetweenSchedulesInSeconds);
            ITeamFoundationJobService service2 = requestContext.GetService<ITeamFoundationJobService>();
            if (num2 > 0)
            {
              DateTime dateTime = service1.GetValue<DateTime>(requestContext, (RegistryQuery) this._timeWhenLatestJobScheduledPath, DateTime.MinValue);
              DateTime utcNow = DateTime.UtcNow;
              if (!(utcNow < dateTime))
              {
                if (utcNow < dateTime.AddSeconds((double) num2))
                {
                  int maxDelaySeconds = (int) Math.Max((dateTime.AddSeconds((double) num2) - utcNow).TotalSeconds, (double) num1);
                  service2.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) this.JobIds, maxDelaySeconds);
                  service1.SetValue<DateTime>(requestContext, this._timeWhenLatestJobScheduledPath, utcNow.AddSeconds((double) maxDelaySeconds));
                }
                else
                {
                  service2.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) this.JobIds, num1);
                  service1.SetValue<DateTime>(requestContext, this._timeWhenLatestJobScheduledPath, utcNow.AddSeconds((double) num1));
                }
              }
            }
            else
              service2.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) this.JobIds, num1);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(14010003, "AnalyticsStaging", this.Name, ex);
          }
        }
      }
      return EventNotificationStatus.ActionPermitted;
    }

    public Type[] SubscribedTypes() => this.EventTypes;

    public virtual bool NotificationEventIsMatch(object notificationEventArgs) => true;

    protected virtual void PreProcessEvent(
      IVssRequestContext requestContext,
      object notificationEventArgs)
    {
    }
  }
}
