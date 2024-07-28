// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.TargetedNotificationsProviderBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Experimentation;
using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Telemetry.Notification;
using Microsoft.VisualStudio.Telemetry.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal abstract class TargetedNotificationsProviderBase : 
    RemoteSettingsProviderBase,
    IRemoteSettingsProvider,
    ISettingsCollection,
    IDisposable
  {
    internal const string TargetedNotificationsTelemetryEventPath = "VS/Core/TargetedNotifications/";
    internal const string TargetedNotificationsTelemetryPropertyPath = "VS.Core.TargetedNotifications.";
    internal readonly IDictionary<string, ActionCategory> ActionCategories = (IDictionary<string, ActionCategory>) new Dictionary<string, ActionCategory>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    protected const string RemoteSettingsActionPath = "VS\\Core\\RemoteSettings";
    protected readonly bool useCache;
    protected readonly bool enforceCourtesy;
    protected readonly int cacheTimeoutMs;
    protected readonly TimeSpan serviceQueryLoopTimeSpan;
    protected readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    protected readonly IRemoteSettingsTelemetry remoteSettingsTelemetry;
    protected readonly ITargetedNotificationsTelemetry targetedNotificationsTelemetry;
    protected readonly TargetedNotificationsCacheProvider notificationAndCourtesyCache;
    protected Stopwatch apiTimer;
    protected int queryIteration;
    private const string RemoteSettingsTelemetryEventPath = "VS/Core/RemoteSettings/Targeted";
    private const string RemoteSettingsTelemetryPropertyPath = "VS.Core.RemoteSettings.Targeted";
    private const string TargetedNotificationsCacheableStorageCollectionPath = "TargetedNotifications";
    private readonly IDictionary<string, Dictionary<string, ActionResponse>> tnActions = (IDictionary<string, Dictionary<string, ActionResponse>>) new Dictionary<string, Dictionary<string, ActionResponse>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim actionsAndCategoriesLock = new SemaphoreSlim(1, 1);
    private readonly IDictionary<string, List<int>> tnSubscriptionIds = (IDictionary<string, List<int>>) new Dictionary<string, List<int>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly IDictionary<string, Dictionary<System.Type, IList>> tnSubscriptionCallbacks = (IDictionary<string, Dictionary<System.Type, IList>>) new Dictionary<string, Dictionary<System.Type, IList>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly object subscriptionLockObject = new object();
    private readonly IRemoteSettingsStorageHandler cacheableStorageHandler;
    private readonly IRemoteSettingsStorageHandler liveStorageHandler;
    private readonly IRemoteSettingsParser remoteSettingsParser;
    private readonly IExperimentationService experimentationService;
    private readonly ITelemetryNotificationService telemetryNotificationService;

    public TargetedNotificationsProviderBase(
      IRemoteSettingsStorageHandler cacheableStorageHandler,
      RemoteSettingsInitializer initializer)
      : base(cacheableStorageHandler, initializer.RemoteSettingsLogger)
    {
      this.cacheableStorageHandler = cacheableStorageHandler;
      this.useCache = this.GetValueOrDefaultFromCacheableStorage<bool>("TargetedNotifications", "UseCache", true);
      this.enforceCourtesy = this.GetValueOrDefaultFromCacheableStorage<bool>("TargetedNotifications", "EnforceCourtesy", true);
      this.cacheTimeoutMs = this.GetValueOrDefaultFromCacheableStorage<int>("TargetedNotifications", "CacheTimeoutMs", 750);
      this.serviceQueryLoopTimeSpan = TimeSpan.FromMinutes((double) this.GetValueOrDefaultFromCacheableStorage<int>("TargetedNotifications", "ServiceQueryLoopMinutes", 1440));
      this.liveStorageHandler = initializer.LiveRemoteSettingsStorageHandlerFactory();
      this.remoteSettingsParser = initializer.RemoteSettingsParser;
      this.remoteSettingsTelemetry = initializer.Telemetry;
      this.targetedNotificationsTelemetry = initializer.TargetedNotificationsTelemetry;
      this.experimentationService = initializer.ExperimentationService;
      this.telemetryNotificationService = initializer.TelemetryNotificationService;
      this.notificationAndCourtesyCache = new TargetedNotificationsCacheProvider(this.enforceCourtesy, this, initializer);
    }

    public override Task<GroupedRemoteSettings> Start()
    {
      Interlocked.Exchange<Task<GroupedRemoteSettings>>(ref this.startTask, Task.Run<GroupedRemoteSettings>((Func<Task<GroupedRemoteSettings>>) (async () =>
      {
        GroupedRemoteSettings result = (GroupedRemoteSettings) null;
        ++this.queryIteration;
        ActionResponseBag response = await this.GetTargetedNotificationActionsAsync().ConfigureAwait(false);
        if (response != null)
        {
          this.ProcessActionResponseBag(response);
          result = await this.ProcessRemoteSettingsFromTargetedNotificationsAsync().ConfigureAwait(false);
        }
        this.StartAgainAfter(this.serviceQueryLoopTimeSpan);
        GroupedRemoteSettings groupedRemoteSettings = result;
        result = (GroupedRemoteSettings) null;
        return groupedRemoteSettings;
      })));
      return this.startTask;
    }

    internal async Task StartAgainAfter(TimeSpan delayTime)
    {
      TargetedNotificationsProviderBase notificationsProviderBase = this;
      if (!(delayTime > TimeSpan.Zero))
        return;
      if (notificationsProviderBase.cancellationTokenSource.IsCancellationRequested)
        return;
      try
      {
        await Task.Delay(delayTime, notificationsProviderBase.cancellationTokenSource.Token).ConfigureAwait(false);
        notificationsProviderBase.Start();
      }
      catch (TaskCanceledException ex)
      {
      }
    }

    public override async Task<IEnumerable<ActionWrapper<T>>> GetActionsAsync<T>(string actionPath)
    {
      TargetedNotificationsProviderBase notificationsProviderBase = this;
      notificationsProviderBase.RequiresNotDisposed();
      GroupedRemoteSettings groupedRemoteSettings = await notificationsProviderBase.startTask.ConfigureAwait(false);
      IEnumerable<LoggingContext<ActionWrapper<T>>> source;
      try
      {
        await notificationsProviderBase.actionsAndCategoriesLock.WaitAsync().ConfigureAwait(false);
        source = await notificationsProviderBase.GetActionsInternalAsync<T>(actionPath, true).ConfigureAwait(false);
      }
      finally
      {
        notificationsProviderBase.actionsAndCategoriesLock.Release();
      }
      return source.Select<LoggingContext<ActionWrapper<T>>, ActionWrapper<T>>((Func<LoggingContext<ActionWrapper<T>>, ActionWrapper<T>>) (x => x.Value));
    }

    public override async Task SubscribeActionsAsync<T>(
      string actionPath,
      Action<ActionWrapper<T>> callback)
    {
      TargetedNotificationsProviderBase notificationsProviderBase = this;
      if (notificationsProviderBase.IsDisposed)
        return;
      GroupedRemoteSettings groupedRemoteSettings = await notificationsProviderBase.startTask.ConfigureAwait(false);
      try
      {
        await notificationsProviderBase.actionsAndCategoriesLock.WaitAsync().ConfigureAwait(false);
        await notificationsProviderBase.SubscribeActionsInternalAsync<T>(actionPath, callback).ConfigureAwait(false);
      }
      finally
      {
        notificationsProviderBase.actionsAndCategoriesLock.Release();
      }
    }

    public override void UnsubscribeActions(string actionPath)
    {
      this.RequiresNotDisposed();
      actionPath = actionPath.NormalizePath();
      lock (this.subscriptionLockObject)
      {
        if (!this.tnSubscriptionIds.ContainsKey(actionPath))
          return;
        foreach (int subscriptionId in this.tnSubscriptionIds[actionPath])
          this.telemetryNotificationService.Unsubscribe(subscriptionId);
        this.tnSubscriptionIds.Remove(actionPath);
      }
    }

    protected override void DisposeManagedResources()
    {
      this.cancellationTokenSource.Cancel();
      lock (this.subscriptionLockObject)
      {
        foreach (string actionPath in this.tnSubscriptionIds.Keys.ToArray<string>())
          this.UnsubscribeActions(actionPath);
        this.tnSubscriptionIds.Clear();
        this.tnSubscriptionCallbacks.Clear();
      }
    }

    protected abstract Task<ActionResponseBag> GetTargetedNotificationActionsAsync();

    private void ProcessActionResponseBag(ActionResponseBag response)
    {
      this.logger.LogInfo(string.Format("Received {0} actions from {1}", (object) response.Actions.Count<ActionResponse>(), (object) this.Name));
      List<string> list1;
      try
      {
        this.actionsAndCategoriesLock.Wait();
        list1 = this.tnActions.Values.SelectMany<Dictionary<string, ActionResponse>, string>((Func<Dictionary<string, ActionResponse>, IEnumerable<string>>) (x => (IEnumerable<string>) x.Keys)).ToList<string>();
        foreach (ActionResponse action in response.Actions)
        {
          Dictionary<string, ActionResponse> dictionary;
          if (!this.tnActions.TryGetValue(action.ActionPath, out dictionary))
          {
            dictionary = new Dictionary<string, ActionResponse>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            this.tnActions[action.ActionPath] = dictionary;
          }
          if (action.RuleId != null)
            dictionary[action.RuleId] = action;
          else
            this.logger.LogInfo("Skipping action that has no RuleId " + action.ActionPath + " from " + this.Name);
        }
        foreach (ActionCategory category in response.Categories)
          this.ActionCategories[category.CategoryId] = category;
      }
      finally
      {
        this.actionsAndCategoriesLock.Release();
      }
      lock (this.subscriptionLockObject)
      {
        try
        {
          foreach (string key1 in (IEnumerable<string>) this.tnSubscriptionCallbacks.Keys)
          {
            foreach (System.Type key2 in this.tnSubscriptionCallbacks[key1].Keys)
            {
              MethodInfo methodInfo = typeof (TargetedNotificationsProviderBase).GetMethod("SubscribeActionsInternalAsync", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(key2);
              foreach (object obj in (IEnumerable) this.tnSubscriptionCallbacks[key1][key2])
                methodInfo.Invoke((object) this, new object[3]
                {
                  (object) key1,
                  obj,
                  (object) list1
                });
            }
          }
        }
        catch (Exception ex)
        {
          this.targetedNotificationsTelemetry.PostCriticalFault("VS/Core/TargetedNotifications/ReSubscribeFailure", "failure", ex);
        }
      }
      string eventName = "VS/Core/TargetedNotifications/ActionsReceived";
      IEnumerable<ActionResponse> source = response.Actions ?? Enumerable.Empty<ActionResponse>();
      Dictionary<string, IEnumerable<string>> dictionary1 = source.GroupBy<ActionResponse, string>((Func<ActionResponse, string>) (x => x.ActionPath)).ToDictionary<IGrouping<string, ActionResponse>, string, IEnumerable<string>>((Func<IGrouping<string, ActionResponse>, string>) (x => x.Key), (Func<IGrouping<string, ActionResponse>, IEnumerable<string>>) (x => x.Select<ActionResponse, string>((Func<ActionResponse, string>) (y => y.RuleId))));
      List<string> list2 = this.ActionCategories.Keys.ToList<string>();
      Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
      {
        {
          "VS.Core.TargetedNotifications.ProviderName",
          (object) this.Name
        },
        {
          "VS.Core.TargetedNotifications.Actions",
          (object) new TelemetryComplexProperty((object) dictionary1)
        },
        {
          "VS.Core.TargetedNotifications.ActionCount",
          (object) source.Count<ActionResponse>()
        },
        {
          "VS.Core.TargetedNotifications.Categories",
          (object) new TelemetryComplexProperty((object) list2)
        },
        {
          "VS.Core.TargetedNotifications.CategoryCount",
          (object) list2.Count
        },
        {
          "VS.Core.TargetedNotifications.ApiResponseMs",
          (object) this.apiTimer?.ElapsedMilliseconds
        },
        {
          "VS.Core.TargetedNotifications.Iteration",
          (object) this.queryIteration
        }
      };
      this.targetedNotificationsTelemetry.PostSuccessfulOperation(eventName, additionalProperties);
    }

    private async Task<GroupedRemoteSettings> ProcessRemoteSettingsFromTargetedNotificationsAsync()
    {
      TargetedNotificationsProviderBase notificationsProviderBase = this;
      IEnumerable<LoggingContext<ActionWrapper<JObject>>> source1 = await notificationsProviderBase.GetActionsInternalAsync<JObject>("VS\\Core\\RemoteSettings", false).ConfigureAwait(false);
      if (source1.Any<LoggingContext<ActionWrapper<JObject>>>())
      {
        List<GroupedRemoteSettings> source2 = new List<GroupedRemoteSettings>();
        List<Tuple<ActionWrapper<JObject>, DeserializedRemoteSettings>> source3 = new List<Tuple<ActionWrapper<JObject>, DeserializedRemoteSettings>>();
        foreach (LoggingContext<ActionWrapper<JObject>> loggingContext in (IEnumerable<LoggingContext<ActionWrapper<JObject>>>) source1.OrderBy<LoggingContext<ActionWrapper<JObject>>, int>((Func<LoggingContext<ActionWrapper<JObject>>, int>) (x => x.Value.Precedence)))
        {
          DeserializedRemoteSettings fromJobject = notificationsProviderBase.remoteSettingsParser.TryParseFromJObject(loggingContext.Value.Action, string.IsNullOrEmpty(loggingContext.Value.FlightName) ? (string) null : "Flight." + loggingContext.Value.FlightName);
          if (fromJobject.Successful)
          {
            source2.Add(new GroupedRemoteSettings(fromJobject, notificationsProviderBase.Name + "-" + loggingContext.Context));
          }
          else
          {
            notificationsProviderBase.logger.LogError("Error deserializing TN rule " + loggingContext.Context + ": " + fromJobject.Error);
            source3.Add(Tuple.Create<ActionWrapper<JObject>, DeserializedRemoteSettings>(loggingContext.Value, fromJobject));
          }
        }
        if (source3.Any<Tuple<ActionWrapper<JObject>, DeserializedRemoteSettings>>())
        {
          string name = "VS/Core/RemoteSettings/TargetedParseSettings";
          notificationsProviderBase.remoteSettingsTelemetry.PostEvent(name, (IDictionary<string, object>) new Dictionary<string, object>()
          {
            ["VS.Core.RemoteSettings.TargetedRuleIds"] = (object) string.Join(",", source3.Select<Tuple<ActionWrapper<JObject>, DeserializedRemoteSettings>, string>((Func<Tuple<ActionWrapper<JObject>, DeserializedRemoteSettings>, string>) (x => x.Item1.RuleId))),
            ["VS.Core.RemoteSettings.TargetedErrors"] = (object) string.Join(",", source3.Select<Tuple<ActionWrapper<JObject>, DeserializedRemoteSettings>, string>((Func<Tuple<ActionWrapper<JObject>, DeserializedRemoteSettings>, string>) (x => x.Item2.Error)))
          });
        }
        if (source2.Any<GroupedRemoteSettings>())
        {
          // ISSUE: reference to a compiler-generated method
          GroupedRemoteSettings remoteSettings = source2.Aggregate<GroupedRemoteSettings>(new Func<GroupedRemoteSettings, GroupedRemoteSettings, GroupedRemoteSettings>(notificationsProviderBase.\u003CProcessRemoteSettingsFromTargetedNotificationsAsync\u003Eb__36_3));
          notificationsProviderBase.liveStorageHandler.SaveSettings(remoteSettings);
          Interlocked.Exchange<IRemoteSettingsStorageHandler>(ref notificationsProviderBase.currentStorageHandler, notificationsProviderBase.liveStorageHandler);
          notificationsProviderBase.cacheableStorageHandler.DeleteAllSettings();
          notificationsProviderBase.cacheableStorageHandler.SaveSettings(remoteSettings);
          return remoteSettings;
        }
      }
      else
      {
        Interlocked.Exchange<IRemoteSettingsStorageHandler>(ref notificationsProviderBase.currentStorageHandler, notificationsProviderBase.liveStorageHandler);
        notificationsProviderBase.cacheableStorageHandler.DeleteAllSettings();
      }
      return (GroupedRemoteSettings) null;
    }

    private async Task<IEnumerable<LoggingContext<ActionWrapper<T>>>> GetActionsInternalAsync<T>(
      string actionPath,
      bool shouldCheckFlight)
    {
      TargetedNotificationsProviderBase notificationsProviderBase = this;
      actionPath = actionPath.NormalizePath();
      IEnumerable<ActionResponse> filteredActionsForPath = (IEnumerable<ActionResponse>) new List<ActionResponse>();
      List<LoggingContext<ActionWrapper<T>>> returnedActions = new List<LoggingContext<ActionWrapper<T>>>();
      Dictionary<string, ActionResponse> dictionary;
      if (notificationsProviderBase.tnActions.TryGetValue(actionPath, out dictionary))
      {
        foreach (ActionResponse actionResponse1 in dictionary.Values)
        {
          ActionResponse actionResponse = actionResponse1;
          if (string.IsNullOrWhiteSpace(actionResponse.TriggerJson))
          {
            bool flag = true;
            if (shouldCheckFlight && !string.IsNullOrEmpty(actionResponse.FlightName))
            {
              try
              {
                flag = await notificationsProviderBase.experimentationService.IsFlightEnabledAsync(actionResponse.FlightName, notificationsProviderBase.cancellationTokenSource.Token).ConfigureAwait(false);
              }
              catch
              {
                flag = false;
              }
              notificationsProviderBase.logger.LogVerbose(string.Format("{0}-{1} depends on flight {2}, isFlightEnabled: {3}", (object) notificationsProviderBase.Name, (object) actionResponse, (object) actionResponse.FlightName, (object) flag));
            }
            if (flag)
              (filteredActionsForPath as List<ActionResponse>).Add(actionResponse);
            actionResponse = (ActionResponse) null;
          }
        }
        if (notificationsProviderBase.useCache)
          filteredActionsForPath = notificationsProviderBase.notificationAndCourtesyCache.GetSendableActionsFromSet(filteredActionsForPath, new int?(notificationsProviderBase.cacheTimeoutMs));
        foreach (ActionResponse actionResponse in filteredActionsForPath)
        {
          try
          {
            returnedActions.Add(new LoggingContext<ActionWrapper<T>>(actionResponse.ToString(), actionResponse.AsTypedAction<T>()));
          }
          catch (TargetedNotificationsException ex)
          {
            string eventName = "VS/Core/TargetedNotifications/ActionJsonParseFailure";
            string str = string.Format("Error handling {0}-{1}", (object) notificationsProviderBase.Name, (object) actionResponse);
            Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
            {
              {
                "VS.Core.TargetedNotifications.RuleId",
                (object) actionResponse.RuleId
              },
              {
                "VS.Core.TargetedNotifications.FlightName",
                (object) actionResponse.FlightName
              }
            };
            notificationsProviderBase.logger.LogError(str, (Exception) ex);
            notificationsProviderBase.targetedNotificationsTelemetry.PostCriticalFault(eventName, str, (Exception) ex, additionalProperties);
          }
        }
      }
      notificationsProviderBase.logger.LogVerbose(string.Format("Got {0} actions for path {1} from provider {2}", (object) returnedActions.Count<LoggingContext<ActionWrapper<T>>>(), (object) actionPath, (object) notificationsProviderBase.Name));
      IEnumerable<LoggingContext<ActionWrapper<T>>> actionsInternalAsync = (IEnumerable<LoggingContext<ActionWrapper<T>>>) returnedActions;
      filteredActionsForPath = (IEnumerable<ActionResponse>) null;
      returnedActions = (List<LoggingContext<ActionWrapper<T>>>) null;
      return actionsInternalAsync;
    }

    private bool IsValidTriggerConfiguration(
      Dictionary<string, ITelemetryEventMatch> triggers,
      Dictionary<string, ActionTriggerOptions> options)
    {
      if (triggers == null || options == null || triggers.Count == 0 || !triggers.ContainsKey("start") || triggers.Count != options.Count)
        return false;
      foreach (string key in triggers.Keys)
      {
        if (!options.ContainsKey(key))
          return false;
        if (string.Equals(key, "start", StringComparison.OrdinalIgnoreCase) && options[key].TriggerOnSubscribe)
        {
          if (triggers[key].GetType() != typeof (TelemetryManifestInvalidMatchItem))
            return false;
        }
        else if (triggers[key].GetType() == typeof (TelemetryManifestInvalidMatchItem) || options[key].TriggerOnSubscribe)
          return false;
      }
      return true;
    }

    private async Task SubscribeActionsInternalAsync<T>(
      string actionPath,
      Action<ActionWrapper<T>> callback,
      IEnumerable<string> previouslySubscribedRuleIds = null)
    {
      TargetedNotificationsProviderBase notificationsProviderBase = this;
      actionPath = actionPath.NormalizePath();
      if (previouslySubscribedRuleIds == null)
      {
        lock (notificationsProviderBase.subscriptionLockObject)
        {
          if (!notificationsProviderBase.tnSubscriptionCallbacks.ContainsKey(actionPath))
            notificationsProviderBase.tnSubscriptionCallbacks[actionPath] = new Dictionary<System.Type, IList>();
          if (!notificationsProviderBase.tnSubscriptionCallbacks[actionPath].ContainsKey(typeof (T)))
            notificationsProviderBase.tnSubscriptionCallbacks[actionPath][typeof (T)] = (IList) new List<Action<ActionWrapper<T>>>();
          notificationsProviderBase.tnSubscriptionCallbacks[actionPath][typeof (T)].Add((object) callback);
        }
      }
      List<Tuple<ActionResponse, string>> tupleList = new List<Tuple<ActionResponse, string>>();
      int returnedActionCount = 0;
      Dictionary<string, ActionResponse> dictionary;
      if (notificationsProviderBase.tnActions.TryGetValue(actionPath, out dictionary))
      {
        foreach (ActionResponse actionResponse1 in dictionary.Values)
        {
          ActionResponse actionResponse = actionResponse1;
          try
          {
            if (previouslySubscribedRuleIds != null)
            {
              if (previouslySubscribedRuleIds.Contains<string>(actionResponse.RuleId))
                continue;
            }
            if (!string.IsNullOrWhiteSpace(actionResponse.TriggerJson))
            {
              bool flag1 = true;
              if (!string.IsNullOrEmpty(actionResponse.FlightName))
              {
                try
                {
                  flag1 = await notificationsProviderBase.experimentationService.IsFlightEnabledAsync(actionResponse.FlightName, notificationsProviderBase.cancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch
                {
                  flag1 = false;
                }
                notificationsProviderBase.logger.LogVerbose(string.Format("{0}-{1} depends on flight {2}, isFlightEnabled: {3}", (object) notificationsProviderBase.Name, (object) actionResponse, (object) actionResponse.FlightName, (object) flag1));
              }
              if (flag1)
              {
                Dictionary<string, ActionTriggerOptions> triggerOptions = actionResponse.GetTriggerOptions();
                Dictionary<string, ITelemetryEventMatch> triggers = actionResponse.GetTriggers();
                if (!notificationsProviderBase.IsValidTriggerConfiguration(triggers, triggerOptions))
                {
                  string eventName = "VS/Core/TargetedNotifications/TriggerJsonInvalid";
                  string description = string.Format("Error handling {0}-{1}", (object) notificationsProviderBase.Name, (object) actionResponse);
                  Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
                  {
                    {
                      "VS.Core.TargetedNotifications.RuleId",
                      (object) actionResponse.RuleId
                    },
                    {
                      "VS.Core.TargetedNotifications.FlightName",
                      (object) actionResponse.FlightName
                    }
                  };
                  notificationsProviderBase.targetedNotificationsTelemetry.PostCriticalFault(eventName, description, additionalProperties: additionalProperties);
                  continue;
                }
                object triggerLockObject = new object();
                bool startTriggerInvoked = false;
                ActionWrapper<T> baseActionWrapper = actionResponse.AsTypedAction<T>();
                Dictionary<string, int> triggerSubscriptions = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                foreach (string key in triggers.Keys)
                {
                  string triggerName = key;
                  bool triggerInvoked = false;
                  Action<TelemetryEvent> wrappedCallback = (Action<TelemetryEvent>) (telemetryEvent =>
                  {
                    lock (triggerLockObject)
                    {
                      bool flag2 = string.Equals(triggerName, "start", StringComparison.OrdinalIgnoreCase);
                      if (flag2 && !startTriggerInvoked)
                      {
                        if (this.useCache)
                        {
                          try
                          {
                            this.actionsAndCategoriesLock.Wait();
                            if (this.notificationAndCourtesyCache.GetSendableAction(actionResponse, new int?(this.cacheTimeoutMs)) == null)
                              return;
                          }
                          finally
                          {
                            this.actionsAndCategoriesLock.Release();
                          }
                        }
                      }
                      if (!(startTriggerInvoked | flag2) || !triggerOptions[triggerName].TriggerAlways && triggerInvoked)
                        return;
                      triggerInvoked = true;
                      ActionWrapper<T> actionWrapper = baseActionWrapper.WithSubscriptionDetails<T>(new ActionSubscriptionDetails()
                      {
                        TriggerName = triggerName,
                        TelemetryEvent = telemetryEvent,
                        NotificationService = this.telemetryNotificationService,
                        TriggerSubscriptions = (IDictionary<string, int>) triggerSubscriptions,
                        TriggerAlways = triggerOptions[triggerName].TriggerAlways,
                        TriggerOnSubscribe = triggerOptions[triggerName].TriggerOnSubscribe,
                        TriggerLockObject = triggerLockObject,
                        RegisteredTriggerNames = triggers.Keys.Distinct<string>()
                      });
                      if (!actionWrapper.Subscription.TriggerAlways && triggerSubscriptions.ContainsKey(triggerName) && !actionWrapper.Subscription.TriggerOnSubscribe)
                      {
                        this.telemetryNotificationService.Unsubscribe(triggerSubscriptions[triggerName]);
                        triggerSubscriptions.Remove(triggerName);
                      }
                      this.targetedNotificationsTelemetry.PostSuccessfulOperation("VS/Core/TargetedNotifications/SubscriptionTriggered", new Dictionary<string, object>()
                      {
                        {
                          "VS.Core.TargetedNotifications.RuleId",
                          (object) actionResponse.RuleId
                        },
                        {
                          "VS.Core.TargetedNotifications.TriggerName",
                          (object) triggerName
                        }
                      });
                      Task.Run((Action) (() => callback(actionWrapper)));
                      if (!flag2)
                        return;
                      startTriggerInvoked = true;
                    }
                  });
                  if (triggers[triggerName].GetType() != typeof (TelemetryManifestInvalidMatchItem) && !triggerOptions[triggerName].TriggerOnSubscribe)
                  {
                    lock (triggerLockObject)
                    {
                      triggerSubscriptions[triggerName] = notificationsProviderBase.telemetryNotificationService.Subscribe(triggers[triggerName], wrappedCallback, false);
                      lock (notificationsProviderBase.subscriptionLockObject)
                      {
                        if (!notificationsProviderBase.tnSubscriptionIds.ContainsKey(actionPath))
                          notificationsProviderBase.tnSubscriptionIds[actionPath] = new List<int>();
                        notificationsProviderBase.tnSubscriptionIds[actionPath].Add(triggerSubscriptions[triggerName]);
                      }
                    }
                  }
                  else
                  {
                    if (!(triggerName == "start") || !triggerOptions[triggerName].TriggerOnSubscribe)
                      throw new TargetedNotificationsException("Invalid trigger: " + triggerName);
                    if (previouslySubscribedRuleIds == null)
                      Task.Run((Action) (() => wrappedCallback((TelemetryEvent) null)));
                  }
                }
              }
            }
            else
              continue;
          }
          catch (TargetedNotificationsException ex)
          {
            string eventName = "VS/Core/TargetedNotifications/ActionJsonParseFailure";
            string str = string.Format("Error handling {0}-{1}", (object) notificationsProviderBase.Name, (object) actionResponse);
            Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
            {
              {
                "VS.Core.TargetedNotifications.RuleId",
                (object) actionResponse.RuleId
              },
              {
                "VS.Core.TargetedNotifications.FlightName",
                (object) actionResponse.FlightName
              }
            };
            notificationsProviderBase.logger.LogError(str, (Exception) ex);
            notificationsProviderBase.targetedNotificationsTelemetry.PostCriticalFault(eventName, str, (Exception) ex, additionalProperties);
          }
        }
      }
      notificationsProviderBase.logger.LogVerbose(string.Format("Subscribe got {0} actions for path {1} from provider {2}", (object) returnedActionCount, (object) actionPath, (object) notificationsProviderBase.Name));
    }

    private T GetValueOrDefaultFromCacheableStorage<T>(
      string collectionPath,
      string key,
      T defaultValue)
    {
      T obj;
      return this.cacheableStorageHandler.TryGetValue<T>(collectionPath, key, out obj) ? obj : defaultValue;
    }
  }
}
