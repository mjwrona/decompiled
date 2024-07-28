// Decompiled with JetBrains decompiler
// Type: Azure.Core.Pipeline.ActivityExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;


#nullable enable
namespace Azure.Core.Pipeline
{
  internal static class ActivityExtensions
  {
    private static readonly Type? ActivitySourceType = Type.GetType("System.Diagnostics.ActivitySource, System.Diagnostics.DiagnosticSource");
    private static readonly Type? ActivityKindType = Type.GetType("System.Diagnostics.ActivityKind, System.Diagnostics.DiagnosticSource");
    private static readonly Type? ActivityTagsCollectionType = Type.GetType("System.Diagnostics.ActivityTagsCollection, System.Diagnostics.DiagnosticSource");
    private static readonly Type? ActivityLinkType = Type.GetType("System.Diagnostics.ActivityLink, System.Diagnostics.DiagnosticSource");
    private static readonly Type? ActivityContextType = Type.GetType("System.Diagnostics.ActivityContext, System.Diagnostics.DiagnosticSource");
    private static readonly ParameterExpression ActivityParameter = Expression.Parameter(typeof (Activity));
    private static bool SupportsActivitySourceSwitch;
    private static Action<Activity, int>? SetIdFormatMethod;
    private static Func<Activity, string?>? GetTraceStateStringMethod;
    private static Func<Activity, int>? GetIdFormatMethod;
    private static Action<Activity, string, object?>? ActivityAddTagMethod;
    private static Func<object, string, int, ICollection<KeyValuePair<string, object>>?, IList?, DateTimeOffset, Activity?>? ActivitySourceStartActivityMethod;
    private static Func<object, bool>? ActivitySourceHasListenersMethod;
    private static Func<string, string?, ICollection<KeyValuePair<string, object>>?, object?>? CreateActivityLinkMethod;
    private static Func<ICollection<KeyValuePair<string, object>>?>? CreateTagsCollectionMethod;

    static ActivityExtensions() => ActivityExtensions.ResetFeatureSwitch();

    public static void SetW3CFormat(this Activity activity)
    {
      if (ActivityExtensions.SetIdFormatMethod == null)
      {
        MethodInfo method = typeof (Activity).GetMethod("SetIdFormat");
        if (method == (MethodInfo) null)
        {
          ActivityExtensions.SetIdFormatMethod = (Action<Activity, int>) ((_1, _2) => { });
        }
        else
        {
          ParameterExpression parameterExpression = Expression.Parameter(typeof (int));
          UnaryExpression unaryExpression = Expression.Convert((Expression) parameterExpression, method.GetParameters()[0].ParameterType);
          ActivityExtensions.SetIdFormatMethod = Expression.Lambda<Action<Activity, int>>((Expression) Expression.Call((Expression) ActivityExtensions.ActivityParameter, method, (Expression) unaryExpression), ActivityExtensions.ActivityParameter, parameterExpression).Compile();
        }
      }
      ActivityExtensions.SetIdFormatMethod(activity, 2);
    }

    public static bool IsW3CFormat(this Activity activity)
    {
      if (ActivityExtensions.GetIdFormatMethod == null)
      {
        MethodInfo getMethod = typeof (Activity).GetProperty("IdFormat")?.GetMethod;
        if (getMethod == (MethodInfo) null)
          ActivityExtensions.GetIdFormatMethod = (Func<Activity, int>) (_ => -1);
        else
          ActivityExtensions.GetIdFormatMethod = Expression.Lambda<Func<Activity, int>>((Expression) Expression.Convert((Expression) Expression.Call((Expression) ActivityExtensions.ActivityParameter, getMethod), typeof (int)), ActivityExtensions.ActivityParameter).Compile();
      }
      return ActivityExtensions.GetIdFormatMethod(activity) == 2;
    }

    public static string? GetTraceState(this Activity activity)
    {
      if (ActivityExtensions.GetTraceStateStringMethod == null)
      {
        MethodInfo getMethod = typeof (Activity).GetProperty("TraceStateString")?.GetMethod;
        if (getMethod == (MethodInfo) null)
          ActivityExtensions.GetTraceStateStringMethod = (Func<Activity, string>) (_ => (string) null);
        else
          ActivityExtensions.GetTraceStateStringMethod = Expression.Lambda<Func<Activity, string>>((Expression) Expression.Call((Expression) ActivityExtensions.ActivityParameter, getMethod), ActivityExtensions.ActivityParameter).Compile();
      }
      return ActivityExtensions.GetTraceStateStringMethod(activity);
    }

    public static void AddObjectTag(this Activity activity, string name, object value)
    {
      if (ActivityExtensions.ActivityAddTagMethod == null)
      {
        MethodInfo method = typeof (Activity).GetMethod("AddTag", BindingFlags.Instance | BindingFlags.Public, (Binder) null, new Type[2]
        {
          typeof (string),
          typeof (object)
        }, (ParameterModifier[]) null);
        if (method == (MethodInfo) null)
        {
          ActivityExtensions.ActivityAddTagMethod = (Action<Activity, string, object>) ((_1, _2, _3) => { });
        }
        else
        {
          ParameterExpression parameterExpression1 = Expression.Parameter(typeof (string));
          ParameterExpression parameterExpression2 = Expression.Parameter(typeof (object));
          ActivityExtensions.ActivityAddTagMethod = Expression.Lambda<Action<Activity, string, object>>((Expression) Expression.Call((Expression) ActivityExtensions.ActivityParameter, method, (Expression) parameterExpression1, (Expression) parameterExpression2), ActivityExtensions.ActivityParameter, parameterExpression1, parameterExpression2).Compile();
        }
      }
      ActivityExtensions.ActivityAddTagMethod(activity, name, value);
    }

    public static bool SupportsActivitySource() => ActivityExtensions.SupportsActivitySourceSwitch && ActivityExtensions.ActivitySourceType != (Type) null;

    public static ICollection<KeyValuePair<string, object>>? CreateTagsCollection()
    {
      if (ActivityExtensions.CreateTagsCollectionMethod == null)
      {
        ConstructorInfo constructor = ActivityExtensions.ActivityTagsCollectionType?.GetConstructor(Array.Empty<Type>());
        ActivityExtensions.CreateTagsCollectionMethod = !(constructor == (ConstructorInfo) null) ? ((Expression<Func<ICollection<KeyValuePair<string, object>>>>) (() => Expression.New(constructor))).Compile() : (Func<ICollection<KeyValuePair<string, object>>>) (() => (ICollection<KeyValuePair<string, object>>) null);
      }
      return ActivityExtensions.CreateTagsCollectionMethod();
    }

    public static object? CreateActivityLink(
      string traceparent,
      string? tracestate,
      ICollection<KeyValuePair<string, object>>? tags)
    {
      if (ActivityExtensions.ActivityLinkType == (Type) null)
        return (object) null;
      if (ActivityExtensions.CreateActivityLinkMethod == null)
      {
        MethodInfo method = ActivityExtensions.ActivityContextType?.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
        Type activityLinkType = ActivityExtensions.ActivityLinkType;
        ConstructorInfo constructorInfo;
        if ((object) activityLinkType == null)
        {
          constructorInfo = (ConstructorInfo) null;
        }
        else
        {
          // ISSUE: explicit non-virtual call
          constructorInfo = __nonvirtual (activityLinkType.GetConstructor(new Type[2]
          {
            ActivityExtensions.ActivityContextType,
            ActivityExtensions.ActivityTagsCollectionType
          }));
        }
        ConstructorInfo constructor = constructorInfo;
        if (method == (MethodInfo) null || constructor == (ConstructorInfo) null || ActivityExtensions.ActivityTagsCollectionType == (Type) null || ActivityExtensions.ActivityContextType == (Type) null)
        {
          ActivityExtensions.CreateActivityLinkMethod = (Func<string, string, ICollection<KeyValuePair<string, object>>, object>) ((_1, _2, _3) => (object) null);
        }
        else
        {
          ParameterExpression parameterExpression1;
          ParameterExpression parameterExpression2;
          ParameterExpression parameterExpression3;
          ActivityExtensions.CreateActivityLinkMethod = ((Expression<Func<string, string, ICollection<KeyValuePair<string, object>>, object>>) ((str1, str2, keyValuePairs) => Expression.TryCatch((Expression) Expression.Convert((Expression) Expression.New(constructor, (Expression) Expression.Call(method, (Expression) parameterExpression1, (Expression) parameterExpression2), (Expression) Expression.Convert((Expression) parameterExpression3, ActivityExtensions.ActivityTagsCollectionType)), typeof (object)), Expression.Catch(typeof (Exception), (Expression) Expression.Default(typeof (object)))))).Compile();
        }
      }
      return ActivityExtensions.CreateActivityLinkMethod(traceparent, tracestate, tags);
    }

    public static bool ActivitySourceHasListeners(object? activitySource)
    {
      if (!ActivityExtensions.SupportsActivitySource() || activitySource == null)
        return false;
      if (ActivityExtensions.ActivitySourceHasListenersMethod == null)
      {
        MethodInfo method = ActivityExtensions.ActivitySourceType?.GetMethod("HasListeners", BindingFlags.Instance | BindingFlags.Public);
        if (method == (MethodInfo) null || ActivityExtensions.ActivitySourceType == (Type) null)
          ActivityExtensions.ActivitySourceHasListenersMethod = (Func<object, bool>) (_ => false);
        else
          ActivityExtensions.ActivitySourceHasListenersMethod = ((Expression<Func<object, bool>>) (obj => Expression.Call((Expression) Expression.Convert(obj, ActivityExtensions.ActivitySourceType), method))).Compile();
      }
      return ActivityExtensions.ActivitySourceHasListenersMethod(activitySource);
    }

    public static Activity? ActivitySourceStartActivity(
      object? activitySource,
      string activityName,
      int kind,
      DateTimeOffset startTime,
      ICollection<KeyValuePair<string, object>>? tags,
      IList? links)
    {
      if (activitySource == null)
        return (Activity) null;
      if (ActivityExtensions.ActivitySourceStartActivityMethod == null)
      {
        if (ActivityExtensions.ActivityLinkType == (Type) null || ActivityExtensions.ActivitySourceType == (Type) null || ActivityExtensions.ActivityContextType == (Type) null || ActivityExtensions.ActivityKindType == (Type) null)
        {
          ActivityExtensions.ActivitySourceStartActivityMethod = (Func<object, string, int, ICollection<KeyValuePair<string, object>>, IList, DateTimeOffset, Activity>) ((_1, _2, _3, _4, _5, _6) => (Activity) null);
        }
        else
        {
          Type activitySourceType = ActivityExtensions.ActivitySourceType;
          MethodInfo methodInfo;
          if ((object) activitySourceType == null)
          {
            methodInfo = (MethodInfo) null;
          }
          else
          {
            // ISSUE: explicit non-virtual call
            methodInfo = __nonvirtual (activitySourceType.GetMethod("StartActivity", BindingFlags.Instance | BindingFlags.Public, (Binder) null, new Type[6]
            {
              typeof (string),
              ActivityExtensions.ActivityKindType,
              ActivityExtensions.ActivityContextType,
              typeof (IEnumerable<KeyValuePair<string, object>>),
              typeof (IEnumerable<>).MakeGenericType(ActivityExtensions.ActivityLinkType),
              typeof (DateTimeOffset)
            }, (ParameterModifier[]) null));
          }
          MethodInfo method = methodInfo;
          if (method == (MethodInfo) null)
          {
            ActivityExtensions.ActivitySourceStartActivityMethod = (Func<object, string, int, ICollection<KeyValuePair<string, object>>, IList, DateTimeOffset, Activity>) ((_7, _8, _9, _10, _11, _12) => (Activity) null);
          }
          else
          {
            ParameterExpression parameterExpression1 = Expression.Parameter(typeof (object));
            ParameterExpression parameterExpression2 = Expression.Parameter(typeof (string));
            ParameterExpression parameterExpression3 = Expression.Parameter(typeof (int));
            ParameterExpression parameterExpression4 = Expression.Parameter(typeof (DateTimeOffset));
            ParameterExpression parameterExpression5 = Expression.Parameter(typeof (ICollection<KeyValuePair<string, object>>));
            ParameterExpression parameterExpression6 = Expression.Parameter(typeof (IList));
            ParameterInfo[] parameters = method.GetParameters();
            ActivityExtensions.ActivitySourceStartActivityMethod = Expression.Lambda<Func<object, string, int, ICollection<KeyValuePair<string, object>>, IList, DateTimeOffset, Activity>>((Expression) Expression.Call((Expression) Expression.Convert((Expression) parameterExpression1, method.DeclaringType), method, (Expression) parameterExpression2, (Expression) Expression.Convert((Expression) parameterExpression3, parameters[1].ParameterType), (Expression) Expression.Default(ActivityExtensions.ActivityContextType), (Expression) Expression.Convert((Expression) parameterExpression5, parameters[3].ParameterType), (Expression) Expression.Convert((Expression) parameterExpression6, parameters[4].ParameterType), (Expression) Expression.Convert((Expression) parameterExpression4, parameters[5].ParameterType)), parameterExpression1, parameterExpression2, parameterExpression3, parameterExpression5, parameterExpression6, parameterExpression4).Compile();
          }
        }
      }
      return ActivityExtensions.ActivitySourceStartActivityMethod(activitySource, activityName, kind, tags, links, startTime);
    }

    public static object? CreateActivitySource(string name)
    {
      if (ActivityExtensions.ActivitySourceType == (Type) null)
        return (object) null;
      return Activator.CreateInstance(ActivityExtensions.ActivitySourceType, (object) name, null);
    }

    public static IList? CreateLinkCollection()
    {
      if (ActivityExtensions.ActivityLinkType == (Type) null)
        return (IList) null;
      return Activator.CreateInstance(typeof (List<>).MakeGenericType(ActivityExtensions.ActivityLinkType)) as IList;
    }

    public static bool TryDispose(this Activity activity)
    {
      if (!(activity is IDisposable disposable))
        return false;
      disposable.Dispose();
      return true;
    }

    public static void ResetFeatureSwitch() => ActivityExtensions.SupportsActivitySourceSwitch = AppContextSwitchHelper.GetConfigValue("Azure.Experimental.EnableActivitySource", "AZURE_EXPERIMENTAL_ENABLE_ACTIVITY_SOURCE");
  }
}
