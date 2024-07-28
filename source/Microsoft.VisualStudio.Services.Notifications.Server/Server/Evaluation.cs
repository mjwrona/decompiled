// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.Evaluation
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class Evaluation
  {
    public static bool EvaluateCondition(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      IReadOnlyList<IDynamicEventPredicate> dynamicPredicates = null)
    {
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      evaluationContext.Tracer.EvaluationTraceNoisy("Enter", evaluationContext.Subscription, nameof (EvaluateCondition));
      try
      {
        Subscription subscription = evaluationContext.Subscription;
        Microsoft.VisualStudio.Services.Identity.Identity user = evaluationContext.User;
        if (!subscription.IsEnabled())
          return false;
        string conditionString = subscription.ConditionString;
        evaluationContext.Tracer.EvaluationTraceNoisy("EvaluateCondition: String to evaluate is {0}", (object) conditionString, evaluationContext.Subscription, nameof (EvaluateCondition));
        string str1 = string.Empty;
        string str2 = string.Empty;
        bool flag = false;
        if (evaluationContext.Subscription.SubscriptionFilter.EventType.Equals("WorkItemChangedEvent") || subscription.SubscriptionFilter.EventType.Equals("BuildStatusChangeEvent") || subscription.SubscriptionFilter.EventType.Equals("CodeReviewChangedEvent"))
          flag = true;
        if (user != null)
        {
          if (flag)
          {
            Guid id = user.Id;
            str1 = str2 = id.ToString();
          }
          else
          {
            str1 = user.DisplayName;
            str2 = IdentityHelper.GetUniqueName(user);
          }
        }
        Dictionary<string, string> macros = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        macros.Add("@@MyDisplayName@@", str1);
        macros.Add("@@MyUniqueName@@", str2);
        evaluationContext.Tracer.EvaluationTraceNoisy("EvaluateCondition: Subscriber display name is {0}, Subscriber unique name is {1}", (object) str1, (object) str2, evaluationContext.Subscription, nameof (EvaluateCondition));
        if (flag)
        {
          Evaluation.AddMacro(macros, '\a'.ToString(), string.Empty);
          Evaluation.AddMacro(macros, '\v'.ToString(), string.Empty);
        }
        if (subscription.Condition == null)
        {
          if (!object.Equals((object) subscription.ProjectId, (object) Guid.Empty))
          {
            Stopwatch stopwatch2 = Stopwatch.StartNew();
            INotificationBridge cachedExtension = requestContext.GetCachedExtension<INotificationBridge>("@NotifBridge");
            if (cachedExtension != null)
              subscription.ProjectName = cachedExtension.GetProjectName(requestContext, subscription.ProjectId);
            subscription.SubsStats.ParseProjectTimeTaken += stopwatch2.ElapsedTicks;
          }
          else
            evaluationContext.Tracer.EvaluationTraceNoisy("EvaluateCondition: Condition does not reference a project", evaluationContext.Subscription, nameof (EvaluateCondition));
          Stopwatch stopwatch3 = Stopwatch.StartNew();
          StringFieldMode stringFieldMode = NotificationSubscriptionService.GetStringFieldMode(requestContext);
          TeamFoundationEventConditionParser parser = TeamFoundationEventConditionParser.GetParser(subscription.GetEventSerializerType(requestContext), subscription.ConditionString, dynamicPredicates, stringFieldMode, requestContext, subscription);
          subscription.Condition = parser.Parse();
          subscription.SubsStats.ParseSubscriptionTimeTaken += stopwatch3.ElapsedTicks;
        }
        Evaluation.AddMacro(macros, "@@MyProjectName@@", subscription.ProjectName);
        evaluationContext.MacrosLookup = macros;
        Stopwatch stopwatch4 = Stopwatch.StartNew();
        bool condition = !evaluationContext.Verify ? subscription.Condition.Evaluate(requestContext, evaluationContext) : subscription.Condition.EvaluateAllConditions(requestContext, evaluationContext);
        subscription.SubsStats.XpathEvaluationtimeTaken += stopwatch4.ElapsedTicks;
        evaluationContext.Tracer.EvaluationTraceNoisy(condition ? "EvaluateCondition: Condition match was successful" : "EvaluateCondition: Condition match was unsusccessful", evaluationContext.Subscription, nameof (EvaluateCondition));
        return condition;
      }
      catch (Exception ex)
      {
        if (evaluationContext.Verify)
        {
          throw;
        }
        else
        {
          evaluationContext.Subscription.UpdateDisabledStatusFromException(requestContext, ex);
          evaluationContext.Tracer.TraceSubscriptionException(1002011, ex, "Evaluation failed", evaluationContext.Subscription);
          return false;
        }
      }
      finally
      {
        evaluationContext.Subscription.SubsStats.EvaluationTimeTaken += stopwatch1.ElapsedTicks;
        evaluationContext.Tracer.EvaluationTraceNoisy("Leave", evaluationContext.Subscription, nameof (EvaluateCondition));
      }
    }

    internal static bool EvaluateFilter(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      if (!(evaluationContext.Subscription.SubscriptionFilter is ExpressionFilter subscriptionFilter))
        throw new InvalidFilterResultException("Invalid ExpressionFilter");
      return FilterConditionUtil.GetFilterConditionFromFilterModel(subscriptionFilter.FilterModel).Evaluate(requestContext, evaluationContext);
    }

    private static void AddMacro(Dictionary<string, string> macros, string key, string value)
    {
      if (macros.ContainsKey(key))
        return;
      macros.Add(key, value);
    }
  }
}
