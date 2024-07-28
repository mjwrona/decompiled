// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.TelemetryCorrelationHttpModule
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

using System;
using System.ComponentModel;
using System.Reflection;
using System.Web;

namespace Microsoft.AspNet.TelemetryCorrelation
{
  public class TelemetryCorrelationHttpModule : IHttpModule
  {
    private const string BeginCalledFlag = "Microsoft.AspNet.TelemetryCorrelation.BeginCalled";
    private const string URLRewriteRewrittenRequest = "IIS_WasUrlRewritten";
    private const string URLRewriteModuleVersion = "IIS_UrlRewriteModule";
    private static MethodInfo onStepMethodInfo = typeof (HttpApplication).GetMethod("OnExecuteRequestStep");

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ParseHeaders { get; set; } = true;

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      context.BeginRequest += new EventHandler(this.Application_BeginRequest);
      context.EndRequest += new EventHandler(this.Application_EndRequest);
      if (TelemetryCorrelationHttpModule.onStepMethodInfo != (MethodInfo) null)
      {
        if (HttpRuntime.UsingIntegratedPipeline)
        {
          try
          {
            TelemetryCorrelationHttpModule.onStepMethodInfo.Invoke((object) context, new object[1]
            {
              (object) new Action<HttpContextBase, Action>(this.OnExecuteRequestStep)
            });
            return;
          }
          catch (Exception ex)
          {
            AspNetTelemetryCorrelationEventSource.Log.OnExecuteRequestStepInvokationError(ex.Message);
            return;
          }
        }
      }
      context.PreRequestHandlerExecute += new EventHandler(this.Application_PreRequestHandlerExecute);
    }

    internal void OnExecuteRequestStep(HttpContextBase context, Action step)
    {
      if (context.CurrentNotification == RequestNotification.ExecuteRequestHandler && !context.IsPostNotification)
        ActivityHelper.RestoreActivityIfNeeded(context.Items);
      step();
    }

    private void Application_BeginRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      AspNetTelemetryCorrelationEventSource.Log.TraceCallback(nameof (Application_BeginRequest));
      ActivityHelper.CreateRootActivity(context, this.ParseHeaders);
      context.Items[(object) "Microsoft.AspNet.TelemetryCorrelation.BeginCalled"] = (object) true;
    }

    private void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
      AspNetTelemetryCorrelationEventSource.Log.TraceCallback(nameof (Application_PreRequestHandlerExecute));
      ActivityHelper.RestoreActivityIfNeeded(((HttpApplication) sender).Context.Items);
    }

    private void Application_EndRequest(object sender, EventArgs e)
    {
      AspNetTelemetryCorrelationEventSource.Log.TraceCallback(nameof (Application_EndRequest));
      bool flag = true;
      HttpContext context = ((HttpApplication) sender).Context;
      if (!context.Items.Contains((object) "Microsoft.AspNet.TelemetryCorrelation.BeginCalled"))
      {
        if (context.Request.ServerVariables != null && context.Request.ServerVariables["IIS_WasUrlRewritten"] == null && context.Request.ServerVariables["IIS_UrlRewriteModule"] != null && context.Response.StatusCode == 200)
          flag = false;
        else
          ActivityHelper.CreateRootActivity(context, this.ParseHeaders);
      }
      if (!flag)
        return;
      ActivityHelper.StopAspNetActivity(context.Items);
    }
  }
}
