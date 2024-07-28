// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RequestScopedFaultInjectionModule
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Web;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class RequestScopedFaultInjectionModule : IHttpModule
  {
    private const string s_layer = "FaultInjectionModule";

    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.Context_BeginRequest);

    private void Context_BeginRequest(object sender, EventArgs eventArgs)
    {
      if (!TeamFoundationApplicationCore.DeploymentInitialized)
        return;
      this.BeginRequestInternal((IHttpApplication) new HttpApplicationWrapper((HttpApplication) sender), TeamFoundationApplicationCore.DeploymentServiceHost);
    }

    internal void BeginRequestInternal(
      IHttpApplication application,
      IVssDeploymentServiceHost deploymentServiceHost)
    {
      HttpContextBase context = application.Context;
      string str = this.LoadFaultInjectionSettingsForRequest(context);
      if (string.IsNullOrWhiteSpace(str))
        return;
      using (IVssRequestContext systemContext = deploymentServiceHost.CreateSystemContext())
      {
        if (!systemContext.GetService<IFaultInjectionRequestSettingsService>().AreRequestScopedFaultsEnabled(systemContext))
          return;
        IVssRequestContext vssRequestContext = application.Context.Items[(object) HttpContextConstants.IVssRequestContext] as IVssRequestContext;
        string host = context.Request.Url.Host;
        try
        {
          if (systemContext.GetService<IFaultInjectionService>().IsFaultInjectionEnabled(systemContext))
            this.InjectRequestFaults(str, host);
        }
        catch (FaultInjectionException ex)
        {
          TeamFoundationApplicationCore.CompleteRequest(vssRequestContext, application, HttpStatusCode.ServiceUnavailable, (Exception) ex);
          return;
        }
        if (vssRequestContext == null)
          return;
        vssRequestContext.GetService<IFaultInjectionRequestSettingsService>().SetFaultInjectionSettingsForRequest(vssRequestContext, str);
      }
    }

    private string LoadFaultInjectionSettingsForRequest(HttpContextBase httpContext)
    {
      try
      {
        HttpRequestBase request = httpContext?.Request;
        string str = (string) null;
        if (request != null)
        {
          if (request.Cookies != null)
          {
            HttpCookie cookie = request.Cookies["AzDevRequestFaults"];
            if (cookie != null)
              str = cookie.Value;
          }
          if (string.IsNullOrWhiteSpace(str) && request.Headers != null)
            str = request.Headers.Get("AzDevRequestFaults");
          return str;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1440014, "VisualStudio.Services.FaultInjectionService", "FaultInjectionModule", ex);
      }
      return (string) null;
    }

    private void InjectRequestFaults(string faultConfiguration, string host)
    {
      StandardFaultInjection.InjectExceptionRaw(faultConfiguration, "Rest", host);
      StandardFaultInjection.InjectDelayRaw(faultConfiguration, "Rest", host);
    }

    public void Dispose()
    {
    }
  }
}
