// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.TargetedNotificationsCacheProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class TargetedNotificationsCacheProvider
  {
    private readonly ITargetedNotificationsTelemetry telemetry;
    private readonly TargetedNotificationsProviderBase tnProvider;
    private readonly bool enforceCourtesy;
    private readonly TimeSpan defaultMaxWaitTimeSpan = TimeSpan.FromDays(14.0);
    private bool responseUsesCachedRules;

    private ITargetedNotificationsCacheStorageProvider Storage { get; set; }

    public TargetedNotificationsCacheProvider(
      bool enforceCourtesy,
      TargetedNotificationsProviderBase tnProvider,
      RemoteSettingsInitializer initializer)
    {
      this.enforceCourtesy = enforceCourtesy;
      this.tnProvider = tnProvider;
      this.Storage = initializer.TargetedNotificationsCacheStorage;
      this.telemetry = initializer.TargetedNotificationsTelemetry;
      this.responseUsesCachedRules = false;
    }

    public void MergeNewResponse(
      ActionResponseBag newResponse,
      IEnumerable<string> previouslyCachedRuleIds,
      int? timeoutMs = null)
    {
      if (this.Storage.Lock(timeoutMs))
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          DateTime utcNow = DateTime.UtcNow;
          bool flag = false;
          CachedTargetedNotifications localCacheCopy = this.Storage.GetLocalCacheCopy();
          List<string> val1 = new List<string>();
          List<string> val2 = new List<string>();
          foreach (ActionResponse action in newResponse.Actions)
          {
            if (action.SendAlways)
            {
              if (localCacheCopy.Actions.Remove(action.RuleId))
                flag = true;
            }
            else if (!previouslyCachedRuleIds.Contains<string>(action.RuleId))
            {
              localCacheCopy.Actions[action.RuleId] = new CachedActionResponseTime()
              {
                CachedTime = localCacheCopy.Actions.ContainsKey(action.RuleId) ? localCacheCopy.Actions[action.RuleId].CachedTime : utcNow,
                MaxWaitTimeSpan = action.MaxWaitTimeSpan == null ? this.defaultMaxWaitTimeSpan : TimeSpan.Parse(action.MaxWaitTimeSpan)
              };
              val1.Add(action.RuleId);
              flag = true;
            }
          }
          foreach (string key in localCacheCopy.Actions.Keys.ToArray<string>())
          {
            CachedActionResponseTime action = localCacheCopy.Actions[key];
            if (utcNow >= action.CachedTime.Add(action.MaxWaitTimeSpan))
            {
              localCacheCopy.Actions.Remove(key);
              val2.Add(key);
              flag = true;
            }
          }
          foreach (ActionCategory category in newResponse.Categories)
          {
            if (localCacheCopy.Categories.ContainsKey(category.CategoryId))
            {
              TimeSpan timeSpan = TimeSpan.Parse(category.WaitTimeSpan);
              if (localCacheCopy.Categories[category.CategoryId].WaitTimeSpan != timeSpan)
              {
                localCacheCopy.Categories[category.CategoryId].WaitTimeSpan = timeSpan;
                flag = true;
              }
            }
          }
          foreach (string key in localCacheCopy.Categories.Keys.ToArray<string>())
          {
            CachedActionCategoryTime category = localCacheCopy.Categories[key];
            if (utcNow >= category.LastSent.Add(category.WaitTimeSpan))
            {
              localCacheCopy.Categories.Remove(key);
              flag = true;
            }
          }
          if (flag)
            this.Storage.SetLocalCache(localCacheCopy);
          if (val1.Count > 0)
            this.responseUsesCachedRules = true;
          stopwatch.Stop();
          if (val1.Count <= 0 && val2.Count <= 0 && localCacheCopy.Actions.Count <= 0 && localCacheCopy.Categories.Count <= 0)
            return;
          this.telemetry.PostSuccessfulOperation("VS/Core/TargetedNotifications/CacheMerged", new Dictionary<string, object>()
          {
            {
              "VS.Core.TargetedNotifications.AddedActions",
              (object) new TelemetryComplexProperty((object) val1)
            },
            {
              "VS.Core.TargetedNotifications.AddedActionsCount",
              (object) val1.Count
            },
            {
              "VS.Core.TargetedNotifications.ExpiredActions",
              (object) new TelemetryComplexProperty((object) val2)
            },
            {
              "VS.Core.TargetedNotifications.ExpiredActionsCount",
              (object) val2.Count
            },
            {
              "VS.Core.TargetedNotifications.CachedActions",
              (object) new TelemetryComplexProperty((object) localCacheCopy.Actions.Keys.ToList<string>())
            },
            {
              "VS.Core.TargetedNotifications.CachedActionsCount",
              (object) localCacheCopy.Actions.Count
            },
            {
              "VS.Core.TargetedNotifications.CachedCategories",
              (object) new TelemetryComplexProperty((object) localCacheCopy.Categories.Keys.ToList<string>())
            },
            {
              "VS.Core.TargetedNotifications.CachedCategoriesCount",
              (object) localCacheCopy.Categories.Count
            },
            {
              "VS.Core.TargetedNotifications.DurationMs",
              (object) stopwatch.ElapsedMilliseconds
            }
          });
        }
        catch (Exception ex)
        {
          stopwatch.Stop();
          string eventName = "VS/Core/TargetedNotifications/CacheMergeFailure";
          string[] array = newResponse.Actions.Where<ActionResponse>((Func<ActionResponse, bool>) (a => !a.SendAlways)).Select<ActionResponse, string>((Func<ActionResponse, string>) (a => a.RuleId)).Except<string>(previouslyCachedRuleIds).ToArray<string>();
          Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
          {
            {
              "VS.Core.TargetedNotifications.SendOnceActions",
              (object) new TelemetryComplexProperty((object) array)
            },
            {
              "VS.Core.TargetedNotifications.SendOnceActionsCount",
              (object) array.Length
            },
            {
              "VS.Core.TargetedNotifications.DurationMs",
              (object) stopwatch.ElapsedMilliseconds
            }
          };
          this.telemetry.PostCriticalFault(eventName, "Failed to merge new response with cache", ex, additionalProperties);
        }
        finally
        {
          this.Storage.Unlock();
        }
      }
      else
        this.telemetry.PostDiagnosticFault("VS/Core/TargetedNotifications/CacheLockTimeout", "Timeout acquiring cache lock", additionalProperties: new Dictionary<string, object>()
        {
          {
            "VS.Core.TargetedNotifications.Operation",
            (object) nameof (MergeNewResponse)
          },
          {
            "VS.Core.TargetedNotifications.TimeoutMs",
            (object) timeoutMs
          }
        });
    }

    public IEnumerable<string> GetAllCachedRuleIds(int? timeoutMs = null)
    {
      if (this.Storage.Lock(timeoutMs))
      {
        try
        {
          CachedTargetedNotifications localCacheCopy = this.Storage.GetLocalCacheCopy();
          if (localCacheCopy.Actions.Keys.Count > 0)
            this.responseUsesCachedRules = true;
          return (IEnumerable<string>) localCacheCopy.Actions.Keys;
        }
        catch (Exception ex)
        {
          this.telemetry.PostCriticalFault("VS/Core/TargetedNotifications/CacheFailure", "Failed to get rule ids from cache", ex);
          return Enumerable.Empty<string>();
        }
        finally
        {
          this.Storage.Unlock();
        }
      }
      else
      {
        this.telemetry.PostDiagnosticFault("VS/Core/TargetedNotifications/CacheLockTimeout", "Timeout acquiring cache lock", additionalProperties: new Dictionary<string, object>()
        {
          {
            "VS.Core.TargetedNotifications.Operation",
            (object) nameof (GetAllCachedRuleIds)
          },
          {
            "VS.Core.TargetedNotifications.TimeoutMs",
            (object) timeoutMs
          }
        });
        return Enumerable.Empty<string>();
      }
    }

    public ActionResponse GetSendableAction(ActionResponse action, int? timeoutMs = null) => this.GetSendableActionsFromSet(Enumerable.Repeat<ActionResponse>(action, 1), timeoutMs).FirstOrDefault<ActionResponse>();

    public IEnumerable<ActionResponse> GetSendableActionsFromSet(
      IEnumerable<ActionResponse> actions,
      int? timeoutMs = null)
    {
      if (!actions.Any<ActionResponse>((Func<ActionResponse, bool>) (a =>
      {
        if (a.Categories != null && a.Categories.Count > 0)
          return true;
        return this.responseUsesCachedRules && !a.SendAlways;
      })))
        return actions;
      Func<IEnumerable<ActionResponse>> func = (Func<IEnumerable<ActionResponse>>) (() => actions.Where<ActionResponse>((Func<ActionResponse, bool>) (a =>
      {
        if (a.Categories != null && a.Categories.Count != 0)
          return false;
        return !this.responseUsesCachedRules || a.SendAlways;
      })));
      if (this.Storage.Lock(timeoutMs))
      {
        try
        {
          bool flag = false;
          CachedTargetedNotifications localCacheCopy = this.Storage.GetLocalCacheCopy();
          List<ActionResponse> sendableActionsFromSet = new List<ActionResponse>();
          foreach (ActionResponse action in actions)
          {
            if (action.SendAlways || !this.responseUsesCachedRules || localCacheCopy.Actions.ContainsKey(action.RuleId))
            {
              List<string> val = new List<string>();
              DateTime utcNow = DateTime.UtcNow;
              TimeSpan timeSpan1 = TimeSpan.MinValue;
              if (action.Categories != null)
              {
                foreach (string category1 in (IEnumerable<string>) action.Categories)
                {
                  if (localCacheCopy.Categories.ContainsKey(category1))
                  {
                    CachedActionCategoryTime category2 = localCacheCopy.Categories[category1];
                    DateTime dateTime = category2.LastSent.Add(category2.WaitTimeSpan);
                    if (utcNow < dateTime)
                    {
                      val.Add(category1);
                      TimeSpan timeSpan2 = dateTime - utcNow;
                      if (timeSpan2 > timeSpan1)
                        timeSpan1 = timeSpan2;
                    }
                  }
                }
              }
              if (val.Count == 0 || !this.enforceCourtesy)
              {
                List<string> list = ((IEnumerable<string>) action.Categories ?? Enumerable.Empty<string>()).Where<string>((Func<string, bool>) (c => !this.tnProvider.ActionCategories.ContainsKey(c))).ToList<string>();
                if (list.Count<string>() == 0 || !this.enforceCourtesy)
                {
                  if (action.Categories != null)
                  {
                    foreach (string category in (IEnumerable<string>) action.Categories)
                    {
                      if (this.tnProvider.ActionCategories.ContainsKey(category))
                      {
                        flag = true;
                        localCacheCopy.Categories[category] = new CachedActionCategoryTime()
                        {
                          LastSent = utcNow,
                          WaitTimeSpan = TimeSpan.Parse(this.tnProvider.ActionCategories[category].WaitTimeSpan)
                        };
                      }
                    }
                  }
                  if (!action.SendAlways)
                  {
                    flag = true;
                    localCacheCopy.Actions.Remove(action.RuleId);
                  }
                  sendableActionsFromSet.Add(action);
                }
                else
                  this.telemetry.PostCriticalFault("VS/Core/TargetedNotifications/MissingCategories", "Rule requires category information that was not provided. It can never be sent.", additionalProperties: new Dictionary<string, object>()
                  {
                    {
                      "VS.Core.TargetedNotifications.RuleId",
                      (object) action.RuleId
                    },
                    {
                      "VS.Core.TargetedNotifications.MissingCategories",
                      (object) new TelemetryComplexProperty((object) list)
                    }
                  });
              }
              else
                this.telemetry.PostSuccessfulOperation("VS/Core/TargetedNotifications/CourtesyDeniedAction", new Dictionary<string, object>()
                {
                  {
                    "VS.Core.TargetedNotifications.RuleId",
                    (object) action.RuleId
                  },
                  {
                    "VS.Core.TargetedNotifications.Categories",
                    (object) new TelemetryComplexProperty((object) val)
                  },
                  {
                    "VS.Core.TargetedNotifications.MinimumWaitTimeSpan",
                    (object) timeSpan1.ToString()
                  }
                });
            }
          }
          if (flag)
            this.Storage.SetLocalCache(localCacheCopy);
          return (IEnumerable<ActionResponse>) sendableActionsFromSet;
        }
        catch (Exception ex)
        {
          IEnumerable<ActionResponse> source = func();
          List<string> list = actions.Select<ActionResponse, string>((Func<ActionResponse, string>) (a => a.RuleId)).Except<string>(source.Select<ActionResponse, string>((Func<ActionResponse, string>) (a => a.RuleId))).ToList<string>();
          string eventName = "VS/Core/TargetedNotifications/CacheFailure";
          Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
          {
            {
              "VS.Core.TargetedNotifications.BlockedRuleCount",
              (object) list.Count
            },
            {
              "VS.Core.TargetedNotifications.BlockedRuleIds",
              (object) new TelemetryComplexProperty((object) list)
            }
          };
          this.telemetry.PostCriticalFault(eventName, "Failed to get sendable actions", ex, additionalProperties);
          return source;
        }
        finally
        {
          this.Storage.Unlock();
        }
      }
      else
      {
        IEnumerable<ActionResponse> source = func();
        List<string> list = actions.Select<ActionResponse, string>((Func<ActionResponse, string>) (a => a.RuleId)).Except<string>(source.Select<ActionResponse, string>((Func<ActionResponse, string>) (a => a.RuleId))).ToList<string>();
        this.telemetry.PostDiagnosticFault("VS/Core/TargetedNotifications/CacheLockTimeout", "Timeout acquiring cache lock", additionalProperties: new Dictionary<string, object>()
        {
          {
            "VS.Core.TargetedNotifications.Operation",
            (object) nameof (GetSendableActionsFromSet)
          },
          {
            "VS.Core.TargetedNotifications.TimeoutMs",
            (object) timeoutMs
          },
          {
            "VS.Core.TargetedNotifications.BlockedRuleCount",
            (object) list.Count
          },
          {
            "VS.Core.TargetedNotifications.BlockedRuleIds",
            (object) new TelemetryComplexProperty((object) list)
          }
        });
        return source;
      }
    }
  }
}
