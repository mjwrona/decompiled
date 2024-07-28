// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebRequestContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class WebRequestContext : 
    VssRequestContext,
    IVssWebRequestContext,
    IVssRequestContext,
    IDisposable,
    IWebRequestContextInternal
  {
    private readonly string m_remotePort = string.Empty;
    private readonly string m_remoteIPAddress = string.Empty;
    private readonly string m_remoteComputer = string.Empty;
    private string m_command = string.Empty;
    private const string c_referrerHeader = "Referer";
    private string m_referrer;
    private readonly HttpContextBase m_httpContextBase;
    private string m_rawUrl = string.Empty;
    private Uri m_requestUri;
    private IUrlTracer m_requestUriForTracing;
    private string m_httpMethod = string.Empty;
    private string m_uniqueAgentIdentifier = string.Empty;
    private string m_authenticationType;
    private RequestRestrictions m_requestRestrictions;
    private string m_hostVirtualPath;
    private string m_webApplicationPath;
    private static readonly char[] s_delimiter = new char[1]
    {
      ','
    };

    internal WebRequestContext(
      IVssServiceHost serviceHost,
      RequestContextType requestContextType,
      LockHelper helper,
      TimeSpan timeout)
      : base(serviceHost, requestContextType, helper, timeout, (VssRequestContext) null)
    {
    }

    protected WebRequestContext(
      IVssServiceHost serviceHost,
      RequestContextType requestContextType,
      HttpContextBase httpContextBase,
      LockHelper helper,
      TimeSpan timeout)
      : base(serviceHost, requestContextType, helper, timeout, (VssRequestContext) null)
    {
      this.m_httpContextBase = httpContextBase;
      if (httpContextBase.Items.Contains((object) "X-VSSF-RequestStartedTime") && httpContextBase.Items.Contains((object) "X-VSSF-RequestTimer"))
      {
        if (httpContextBase.Items[(object) "X-VSSF-RequestStartedTime"] is DateTime)
          this.m_timer.StartTime = (DateTime) httpContextBase.Items[(object) "X-VSSF-RequestStartedTime"];
        if (httpContextBase.Items[(object) "X-VSSF-RequestTimer"] is long)
          this.m_timer.StartTimestamp = (long) httpContextBase.Items[(object) "X-VSSF-RequestTimer"];
        if (httpContextBase.Items[(object) "X-VSSF-ManagedStartTime"] is long)
          this.m_timer.RequestTimerInternal().SetManagedStartTime((long) httpContextBase.Items[(object) "X-VSSF-ManagedStartTime"]);
      }
      try
      {
        this.m_remotePort = httpContextBase.Request.ServerVariables["REMOTE_PORT"];
      }
      catch (Exception ex)
      {
      }
      this.m_remoteIPAddress = httpContextBase.Request.ServerVariables[HttpContextConstants.ResolvedClientIp] ?? httpContextBase.Request.UserHostAddress;
      try
      {
        this.m_remoteComputer = httpContextBase.Request.UserHostName;
      }
      catch (Exception ex)
      {
      }
      this.ResetActivityId();
      this.ParseCoreFields(serviceHost, httpContextBase);
      this.ParseHeaders(httpContextBase.Request.Headers);
    }

    private void ParseCoreFields(IVssServiceHost serviceHost, HttpContextBase httpContext)
    {
      HostRouteContext hostRouteContext = (HostRouteContext) httpContext.Items[(object) HttpContextConstants.ServiceHostRouteContext];
      if (hostRouteContext != null)
      {
        this.m_hostVirtualPath = hostRouteContext.VirtualPath;
        this.m_webApplicationPath = hostRouteContext.WebApplicationPath;
      }
      try
      {
        this.m_userAgent = httpContext.Request.UserAgent;
      }
      catch (Exception ex)
      {
      }
      try
      {
        this.m_rawUrl = httpContext.Request.RawUrl;
      }
      catch (Exception ex)
      {
      }
      try
      {
        this.m_requestUri = httpContext.Request.Url;
      }
      catch (Exception ex)
      {
      }
      try
      {
        if (this.m_requestUri != (Uri) null)
          this.m_requestUriForTracing = (IUrlTracer) new UrlTracer(this.m_requestUri);
      }
      catch (Exception ex)
      {
      }
      try
      {
        this.m_httpMethod = httpContext.Request.RequestType;
      }
      catch (Exception ex)
      {
      }
    }

    private void ParseHeaders(NameValueCollection headers)
    {
      try
      {
        string header = headers["X-VSS-Agent"];
        if (!string.IsNullOrEmpty(header))
          this.m_uniqueAgentIdentifier = header;
      }
      catch (Exception ex)
      {
      }
      try
      {
        string header = headers["X-VSS-E2EID"];
        if (!string.IsNullOrEmpty(header))
        {
          Guid result = Guid.Empty;
          if (Guid.TryParse(header, out result))
            this.E2EId = result;
        }
      }
      catch (Exception ex)
      {
      }
      try
      {
        string header = headers["X-VSS-OrchestrationId"];
        if (!string.IsNullOrEmpty(header))
          this.OrchestrationId = header;
      }
      catch (Exception ex)
      {
      }
      try
      {
        string str = headers["X-TFS-Session"] ?? headers["X-TFS-Instance"] ?? headers["X-VersionControl-Instance"];
        if (str != null)
        {
          string[] strArray = str.Split(WebRequestContext.s_delimiter, StringSplitOptions.RemoveEmptyEntries);
          Guid result;
          if (strArray.Length >= 1 && Guid.TryParse(strArray[0].Trim(), out result))
            this.m_uniqueIdentifier = result;
          if (strArray.Length >= 2)
            this.m_command = strArray[1].Trim();
        }
        if (this.UniqueIdentifier == Guid.Empty)
        {
          this.m_uniqueIdentifier = this.ActivityId;
          headers["X-TFS-Session"] = this.m_uniqueIdentifier.ToString();
        }
      }
      catch (Exception ex)
      {
      }
      try
      {
        Guid result;
        if (!Guid.TryParse(headers["X-VSS-Audit-CorrelationId"], out result) || !(result != new Guid()) || !ServicePrincipals.IsServicePrincipal((IVssRequestContext) this, this.UserContext))
          return;
        this.Items[RequestContextItemsKeys.AuditLogCorrelationId] = (object) result;
      }
      catch (Exception ex)
      {
      }
    }

    public string AuthenticationType => this.m_authenticationType;

    public string Command => this.m_command;

    public string HttpMethod => this.m_httpMethod;

    public string RawUrl => this.m_rawUrl;

    public string RemoteComputer => this.m_remoteComputer;

    public string RemoteIPAddress => this.m_remoteIPAddress;

    public string RemotePort => this.m_remotePort;

    public Uri RequestUri => this.m_requestUri;

    public IUrlTracer RequestUriForTracing => this.m_requestUriForTracing;

    public string UniqueAgentIdentifier => this.m_uniqueAgentIdentifier;

    public string RequestPath
    {
      get
      {
        string requestPath = this.RawUrl;
        int length;
        if ((length = requestPath.IndexOf('?')) != -1)
          requestPath = requestPath.Substring(0, length);
        return requestPath;
      }
    }

    public string RelativePath => WebRequestContext.RemoveVirtualDirectory((IVssWebRequestContext) this, this.RequestPath);

    public string RelativeUrl => WebRequestContext.RemoveVirtualDirectory((IVssWebRequestContext) this, this.RawUrl);

    public string VirtualPath => this.m_hostVirtualPath;

    public string WebApplicationPath => this.m_webApplicationPath;

    public virtual bool GetSessionValue(string sessionKey, out string sessionValue)
    {
      sessionValue = string.Empty;
      return false;
    }

    public virtual bool SetSessionValue(string sessionKey, string sessionValue) => false;

    public virtual void PartialResultsReady()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    RequestRestrictions IWebRequestContextInternal.RequestRestrictions
    {
      get => this.RequestRestrictions;
      set => this.RequestRestrictions = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IWebRequestContextInternal.SetAuthenticationMechanismsToAdvertise(
      AuthenticationMechanisms newMechanismsToAdvertise)
    {
      this.m_requestRestrictions = this.m_requestRestrictions != null ? this.m_requestRestrictions.WithMechanismsToAdvertise(newMechanismsToAdvertise) : throw new InvalidOperationException("Cannot call SetAuthenticationMechanismsToAdvertise before RequestRestrictions is set.");
    }

    protected RequestRestrictions RequestRestrictions
    {
      get => this.m_requestRestrictions;
      set => this.m_requestRestrictions = value;
    }

    void IWebRequestContextInternal.SetAuthenticationType(string authenticationType)
    {
      if (string.Equals(authenticationType, "Negotiate", StringComparison.OrdinalIgnoreCase))
      {
        string serverVariable = this.m_httpContextBase.Request.ServerVariables["HTTP_AUTHORIZATION"];
        if (serverVariable != null && serverVariable.Length > 11)
        {
          switch (serverVariable[10])
          {
            case 'T':
              authenticationType = "NTLM";
              break;
            case 'Y':
              authenticationType = "Kerberos";
              break;
          }
        }
      }
      this.m_authenticationType = authenticationType;
    }

    public override RequestDetails GetRequestDetails(
      TeamFoundationLoggingLevel loggingLevel = TeamFoundationLoggingLevel.Normal,
      long executionTimeThreshold = 10000000,
      bool isExceptionExpected = false,
      bool canAggregate = true)
    {
      RequestDetails requestDetails = base.GetRequestDetails(loggingLevel, executionTimeThreshold, isExceptionExpected, canAggregate);
      HttpRequestBase request = this.WebRequestContextInternal().HttpContext?.Request;
      if (request != null)
      {
        requestDetails.UriStem = request.Url?.AbsolutePath;
        requestDetails.Referrer = this.GetReferrer(request);
      }
      return requestDetails;
    }

    internal string GetReferrer(HttpRequestBase httpRequest)
    {
      if (this.m_referrer == null)
        this.m_referrer = this.GetReferrerInternal(httpRequest);
      return this.m_referrer;
    }

    private string GetReferrerInternal(HttpRequestBase httpRequest)
    {
      string uriString = httpRequest.Headers.Get("Referer");
      if (string.IsNullOrWhiteSpace(uriString))
        return string.Empty;
      Uri result;
      if (Uri.TryCreate(uriString, UriKind.Absolute, out result))
      {
        string absoluteUri = result.AbsoluteUri;
        if (!string.IsNullOrWhiteSpace(absoluteUri))
        {
          foreach (AccessMapping accessMapping in this.GetService<ILocationService>().GetAccessMappings((IVssRequestContext) this))
          {
            if (absoluteUri.StartsWith(accessMapping.AccessPoint, StringComparison.OrdinalIgnoreCase))
              return absoluteUri;
          }
          return result.AbsolutePath;
        }
      }
      return uriString;
    }

    private static string GetHeaderValue(HttpRequestMessage message, string headerName)
    {
      IEnumerable<string> values;
      return message.Headers.TryGetValues(headerName, out values) ? values.FirstOrDefault<string>() : (string) null;
    }

    HttpContextBase IWebRequestContextInternal.HttpContext => this.HttpContext;

    protected HttpContextBase HttpContext => this.m_httpContextBase;

    private static string RemoveVirtualDirectory(
      IVssWebRequestContext requestContext,
      string rawUrl)
    {
      string path = requestContext.VirtualPath();
      if (!string.IsNullOrEmpty(rawUrl) && !string.IsNullOrEmpty(path))
      {
        string pathIfNeeded1 = UriUtility.AppendSlashToPathIfNeeded(rawUrl);
        string pathIfNeeded2 = UriUtility.AppendSlashToPathIfNeeded(path);
        string str = pathIfNeeded2;
        if (pathIfNeeded1.StartsWith(str, StringComparison.OrdinalIgnoreCase))
          rawUrl = rawUrl.Substring(pathIfNeeded2.Length - 1);
      }
      return rawUrl;
    }
  }
}
