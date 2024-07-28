// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationInternetConnectivityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamFoundationInternetConnectivityService : IVssFrameworkService
  {
    private bool m_isConnectedToInternet;
    private const string c_area = "TeamFoundationInternetConnectivityService";
    private const string c_layer = "TeamFoundationInternetConnectivityService";
    private readonly string c_marketplaceUrlRegistryPath = "/Configuration/Service/Gallery/MarketplaceRootURL";
    private readonly string c_marketplaceUrlFormatDefault = "https://marketplace.visualstudio.com/";
    private readonly TimeSpan c_recheckTimespan = TimeSpan.FromMinutes(10.0);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckOnPremisesDeployment();
      systemRequestContext.CheckDeploymentRequestContext();
      TeamFoundationTaskService service = systemRequestContext.GetService<TeamFoundationTaskService>();
      TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.CheckIsConnectedToInternet), (object) null, DateTime.UtcNow, (int) this.c_recheckTimespan.TotalMilliseconds);
      Guid instanceId = systemRequestContext.ServiceHost.InstanceId;
      TeamFoundationTask task = teamFoundationTask;
      service.AddTask(instanceId, task);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool IsConnectedToInternet => this.m_isConnectedToInternet;

    internal void CheckIsConnectedToInternet(IVssRequestContext requestContext, object taskArgs)
    {
      string hostNameOrAddress = "";
      Uri baseUri;
      try
      {
        baseUri = new Uri(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) this.c_marketplaceUrlRegistryPath, this.c_marketplaceUrlFormatDefault));
        hostNameOrAddress = baseUri.Host;
        Dns.GetHostEntry(hostNameOrAddress);
      }
      catch
      {
        requestContext.Trace(1040001, TraceLevel.Info, nameof (TeamFoundationInternetConnectivityService), nameof (TeamFoundationInternetConnectivityService), "Cannot resolve " + hostNameOrAddress);
        this.m_isConnectedToInternet = false;
        return;
      }
      HttpStatusCode httpStatusCode = (HttpStatusCode) 0;
      string healthApiString = "";
      try
      {
        healthApiString = new Uri(baseUri, "_apis/health").ToString();
        using (HttpWebResponse httpWebResponse = new HttpRetryHelper(3).Invoke<HttpWebResponse>((Func<HttpWebResponse>) (() =>
        {
          WebRequest webRequest = WebRequest.Create(healthApiString);
          webRequest.Timeout = 10000;
          return (HttpWebResponse) webRequest.GetResponse();
        })))
        {
          httpStatusCode = httpWebResponse.StatusCode;
          requestContext.Trace(1040002, TraceLevel.Info, nameof (TeamFoundationInternetConnectivityService), nameof (TeamFoundationInternetConnectivityService), string.Format("Health api of {0} returned {1}", (object) healthApiString, (object) httpStatusCode));
        }
      }
      catch (WebException ex)
      {
        if (ex.Response is HttpWebResponse)
        {
          httpStatusCode = ((HttpWebResponse) ex.Response).StatusCode;
          requestContext.Trace(1040003, TraceLevel.Error, nameof (TeamFoundationInternetConnectivityService), nameof (TeamFoundationInternetConnectivityService), string.Format("Health api of {0} returned {1}", (object) healthApiString, (object) httpStatusCode));
        }
        else
        {
          string str = TeamFoundationExceptionFormatter.FormatException((Exception) ex, false);
          requestContext.Trace(1040003, TraceLevel.Error, nameof (TeamFoundationInternetConnectivityService), nameof (TeamFoundationInternetConnectivityService), "Health api of " + healthApiString + " failed. The following error was reported: " + str);
        }
      }
      if (httpStatusCode == HttpStatusCode.OK)
        this.m_isConnectedToInternet = true;
      else
        this.m_isConnectedToInternet = false;
    }
  }
}
