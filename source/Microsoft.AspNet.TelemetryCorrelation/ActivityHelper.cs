// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.ActivityHelper
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

using System.Collections;
using System.Diagnostics;
using System.Web;

namespace Microsoft.AspNet.TelemetryCorrelation
{
  internal static class ActivityHelper
  {
    public const string AspNetListenerName = "Microsoft.AspNet.TelemetryCorrelation";
    public const string AspNetActivityName = "Microsoft.AspNet.HttpReqIn";
    public const string AspNetActivityStartName = "Microsoft.AspNet.HttpReqIn.Start";
    public const string ActivityKey = "__AspnetActivity__";
    private static readonly DiagnosticListener AspNetListener = new DiagnosticListener("Microsoft.AspNet.TelemetryCorrelation");
    private static readonly object EmptyPayload = new object();

    public static void StopAspNetActivity(IDictionary contextItems)
    {
      Activity activity = Activity.Current;
      Activity contextItem = contextItems.Contains((object) "__AspnetActivity__") ? (Activity) contextItems[(object) "__AspnetActivity__"] : (Activity) null;
      if (activity != contextItem)
      {
        Activity.Current = contextItem;
        activity = contextItem;
      }
      if (activity != null)
      {
        ActivityHelper.AspNetListener.StopActivity(activity, ActivityHelper.EmptyPayload);
        contextItems[(object) "__AspnetActivity__"] = (object) null;
      }
      AspNetTelemetryCorrelationEventSource.Log.ActivityStopped(activity?.Id, activity?.OperationName);
    }

    public static Activity CreateRootActivity(HttpContext context, bool parseHeaders)
    {
      if (ActivityHelper.AspNetListener.IsEnabled() && ActivityHelper.AspNetListener.IsEnabled("Microsoft.AspNet.HttpReqIn"))
      {
        Activity activity = new Activity("Microsoft.AspNet.HttpReqIn");
        if (parseHeaders)
          activity.Extract(context.Request.Unvalidated.Headers);
        ActivityHelper.AspNetListener.OnActivityImport(activity, (object) null);
        if (ActivityHelper.StartAspNetActivity(activity))
        {
          context.Items[(object) "__AspnetActivity__"] = (object) activity;
          AspNetTelemetryCorrelationEventSource.Log.ActivityStarted(activity.Id);
          return activity;
        }
      }
      return (Activity) null;
    }

    internal static void RestoreActivityIfNeeded(IDictionary contextItems)
    {
      if (Activity.Current != null || !contextItems.Contains((object) "__AspnetActivity__"))
        return;
      Activity.Current = (Activity) contextItems[(object) "__AspnetActivity__"];
    }

    private static bool StartAspNetActivity(Activity activity)
    {
      if (!ActivityHelper.AspNetListener.IsEnabled("Microsoft.AspNet.HttpReqIn", (object) activity, ActivityHelper.EmptyPayload))
        return false;
      if (ActivityHelper.AspNetListener.IsEnabled("Microsoft.AspNet.HttpReqIn.Start"))
        ActivityHelper.AspNetListener.StartActivity(activity, ActivityHelper.EmptyPayload);
      else
        activity.Start();
      return true;
    }
  }
}
