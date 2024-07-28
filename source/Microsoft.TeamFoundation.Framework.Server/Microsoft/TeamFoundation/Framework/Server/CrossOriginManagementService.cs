// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CrossOriginManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CrossOriginManagementService : 
    VssBaseService,
    ICrossOriginManagementService,
    IVssFrameworkService
  {
    private IList<CrossOriginEntry> m_allowedOrigins;
    private string m_xFrameOptionsValue;
    private const string c_registryRootPath = "/Configuration/WebSecurity";
    private const string c_registryAccountManagementAllowedKey = "/Configuration/WebSecurity/EnableAccountOriginManagement";
    private const string c_registryAllowedOriginsKey = "/Configuration/WebSecurity/AllowedOrigins";
    private const string c_registryFrameOptionsDefaultKey = "/Configuration/WebSecurity/SecureFrameOptions";
    private const string c_frameOptionsDefault = "SAMEORIGIN";
    private const string c_frameOptionsHeader = "X-FRAME-OPTIONS";
    private const string c_corsResponseHeaderAllowOrigin = "Access-Control-Allow-Origin";
    private const string c_corsResponseHeaderMaxAge = "Access-Control-Max-Age";
    private const string c_corsResponseHeaderAllowCredentials = "Access-Control-Allow-Credentials";
    private const string c_corsResponseHeaderAllowMethods = "Access-Control-Allow-Methods";
    private const string c_corsResponseHeaderAllowHeaders = "Access-Control-Allow-Headers";
    private const string c_corsResponseHeaderExposeHeaders = "Access-Control-Expose-Headers";
    private const string c_maxAgeInSeconds = "3600";
    private const string c_allowedHeaders = "ActivityId,X-TFS-Session,X-MS-ContinuationToken,X-VSS-GlobalMessage,ETag";
    private const string c_corsAuthorizationHeader = "authorization";
    private static readonly char[] s_originEntriesSeparator = new char[1]
    {
      ';'
    };
    private static readonly char[] s_propertiesSeparator = new char[1]
    {
      ','
    };
    private static readonly char[] s_propertyValueSeparator = new char[1]
    {
      '='
    };
    private const string c_propertyNamePort = "port";
    private const string c_propertyNameScheme = "scheme";
    private const string c_propertyNameSubdomains = "subdomains";
    private const string c_propertyNameAllowed = "allowed";
    private const string c_Area = "CrossOriginManagement";
    private const string c_Layer = "Service";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "CrossOriginManagement", "Service", "ServiceStart");
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, "/Configuration/WebSecurity/*");
        this.LoadSettings(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(0, "CrossOriginManagement", "Service", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(0, "CrossOriginManagement", "Service", "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "CrossOriginManagement", "Service", "ServiceEnd");
      try
      {
        systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(0, "CrossOriginManagement", "Service", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(0, "CrossOriginManagement", "Service", "ServiceEnd");
      }
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSettings(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(0, "CrossOriginManagement", "Service", nameof (LoadSettings));
      try
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<CachedRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/WebSecurity/*");
        string valueFromPath = registryEntryCollection.GetValueFromPath<string>("/Configuration/WebSecurity/AllowedOrigins", string.Empty);
        if (!string.IsNullOrEmpty(valueFromPath))
        {
          List<CrossOriginEntry> crossOriginEntryList = new List<CrossOriginEntry>();
          foreach (string registryEntry in valueFromPath.Split(CrossOriginManagementService.s_originEntriesSeparator, StringSplitOptions.RemoveEmptyEntries))
          {
            if (!string.IsNullOrWhiteSpace(registryEntry))
            {
              CrossOriginEntry crossOriginEntry = this.ParseCrossOriginEntry(requestContext, registryEntry);
              if (crossOriginEntry != null)
                crossOriginEntryList.Add(crossOriginEntry);
            }
          }
          this.m_allowedOrigins = (IList<CrossOriginEntry>) crossOriginEntryList;
        }
        else
          this.m_allowedOrigins = (IList<CrossOriginEntry>) null;
        this.m_xFrameOptionsValue = registryEntryCollection.GetValueFromPath<string>("/Configuration/WebSecurity/SecureFrameOptions", "SAMEORIGIN");
        if (!string.IsNullOrWhiteSpace(this.m_xFrameOptionsValue))
          return;
        this.m_xFrameOptionsValue = "SAMEORIGIN";
      }
      catch (Exception ex)
      {
        requestContext.TraceException(7028, "CrossOriginManagement", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(7029, "CrossOriginManagement", "Service", nameof (LoadSettings));
      }
    }

    public HttpStatusCode? ProcessCORSOptionsRequest(
      IVssRequestContext requestContext,
      HttpRequestBase request,
      NameValueCollection responseHeaders)
    {
      if (string.IsNullOrWhiteSpace(request.Headers["Origin"]) || !string.Equals("options", request.HttpMethod, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(request.Headers["Access-Control-Request-Method"]))
        return new HttpStatusCode?();
      this.ProcessCORSHeaders(requestContext, request, responseHeaders);
      return new HttpStatusCode?(HttpStatusCode.OK);
    }

    public void ProcessCORSHeaders(
      IVssRequestContext requestContext,
      HttpRequestBase request,
      NameValueCollection responseHeaders)
    {
      string header = request.Headers["Origin"];
      if (string.IsNullOrWhiteSpace(header))
        return;
      if (this.IsAllowedOrigin(header, CrossOriginEntryOptions.CorsWithCredentials))
        this.AddCORSResponseHeaders(header, request.Headers, responseHeaders, true);
      else
        this.AddCORSResponseHeaders("*", request.Headers, responseHeaders, false);
    }

    public bool IsUnsafeCrossOriginRequest(IVssRequestContext requestContext, string origin)
    {
      if (string.IsNullOrWhiteSpace(origin))
        return false;
      bool flag = AuthenticationHelpers.IsRequestUsingCookieBasedAuthentication(requestContext);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && string.IsNullOrEmpty(requestContext.GetAuthenticationMechanism()))
        flag = true;
      if (!flag)
        return false;
      Uri uri = requestContext.RequestUri();
      if (uri != (Uri) null && uri.AbsoluteUri.StartsWith(origin + "/", StringComparison.OrdinalIgnoreCase))
        return false;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFSOnPremises, AccessMappingConstants.PublicAccessMappingMoniker);
        if (!string.IsNullOrEmpty(locationServiceUrl) && (locationServiceUrl + "/").StartsWith(origin + "/", StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return !this.IsAllowedOrigin(origin, CrossOriginEntryOptions.CorsWithCredentials);
    }

    public void AddXFrameOptionsHeader(HttpContextBase context, bool overwriteExistingValue)
    {
      if (!overwriteExistingValue && !string.IsNullOrWhiteSpace(context.Response.Headers["X-FRAME-OPTIONS"]))
        return;
      Uri uri = (Uri) null;
      try
      {
        uri = context.Request.UrlReferrer;
      }
      catch (UriFormatException ex)
      {
      }
      if (uri != (Uri) null && this.IsAllowedOrigin(uri, CrossOriginEntryOptions.Framing))
        context.Response.Headers.Remove("X-FRAME-OPTIONS");
      else
        context.Response.Headers.Set("X-FRAME-OPTIONS", this.m_xFrameOptionsValue);
    }

    private bool IsAllowedOrigin(string origin, CrossOriginEntryOptions requiredFeature)
    {
      bool flag = false;
      Uri result;
      if (Uri.TryCreate(origin, UriKind.Absolute, out result))
        flag = this.IsAllowedOrigin(result, requiredFeature);
      return flag;
    }

    private bool IsAllowedOrigin(Uri uri, CrossOriginEntryOptions feature)
    {
      IList<CrossOriginEntry> allowedOrigins = this.m_allowedOrigins;
      if (allowedOrigins != null && uri != (Uri) null)
      {
        string host = uri.Host;
        foreach (CrossOriginEntry crossOriginEntry in (IEnumerable<CrossOriginEntry>) allowedOrigins)
        {
          if (crossOriginEntry.IsAllowed(uri, feature))
            return true;
        }
      }
      return false;
    }

    private void AddCORSResponseHeaders(
      string origin,
      NameValueCollection requestHeaders,
      NameValueCollection responseHeaders,
      bool includeAuth)
    {
      responseHeaders["Access-Control-Allow-Origin"] = origin;
      responseHeaders["Access-Control-Max-Age"] = "3600";
      responseHeaders["Access-Control-Allow-Methods"] = "OPTIONS,GET,POST,PATCH,PUT,DELETE";
      responseHeaders["Access-Control-Expose-Headers"] = "ActivityId,X-TFS-Session,X-MS-ContinuationToken,X-VSS-GlobalMessage,ETag";
      if (includeAuth)
        responseHeaders["Access-Control-Allow-Credentials"] = "true";
      string str = requestHeaders["Access-Control-Request-Headers"];
      if (!string.IsNullOrEmpty(str))
      {
        if (str.IndexOf("authorization", StringComparison.OrdinalIgnoreCase) == -1)
          str = string.Format("{0}, {1}", (object) str, (object) "authorization");
        responseHeaders["Access-Control-Allow-Headers"] = str;
      }
      else
        responseHeaders["Access-Control-Allow-Headers"] = "authorization";
    }

    private CrossOriginEntry ParseCrossOriginEntry(
      IVssRequestContext requestContext,
      string registryEntry)
    {
      string[] strArray1 = registryEntry.Split(CrossOriginManagementService.s_propertiesSeparator, StringSplitOptions.RemoveEmptyEntries);
      if (strArray1.Length == 0 || string.IsNullOrWhiteSpace(strArray1[0]))
        return (CrossOriginEntry) null;
      CrossOriginEntry crossOriginEntry1 = new CrossOriginEntry();
      crossOriginEntry1.Host = strArray1[0].Trim('.');
      crossOriginEntry1.AllowSubdomains = true;
      CrossOriginEntry crossOriginEntry2 = crossOriginEntry1;
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      int num = executionEnvironment.IsHostedDeployment ? 1 : 3;
      crossOriginEntry2.AllowedOptions = (CrossOriginEntryOptions) num;
      executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsSslOnly)
        crossOriginEntry1.Scheme = "https";
      for (int index = 1; index < strArray1.Length; ++index)
      {
        string[] strArray2 = strArray1[index].Split(CrossOriginManagementService.s_propertyValueSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (strArray2.Length != 2)
          requestContext.Trace(0, TraceLevel.Error, "CrossOriginManagement", "Service", "Cross Origin entry {0} has bad value {1}.", (object) registryEntry, (object) strArray1[index]);
        else if (strArray2[0].Equals("port", StringComparison.OrdinalIgnoreCase))
        {
          int result;
          if (int.TryParse(strArray2[1], out result))
            crossOriginEntry1.Port = new int?(result);
        }
        else if (strArray2[0].Equals("scheme", StringComparison.OrdinalIgnoreCase))
          crossOriginEntry1.Scheme = strArray2[1];
        else if (strArray2[0].Equals("subdomains", StringComparison.OrdinalIgnoreCase))
        {
          bool result;
          if (bool.TryParse(strArray2[1], out result))
            crossOriginEntry1.AllowSubdomains = result;
        }
        else if (strArray2[0].Equals("allowed", StringComparison.OrdinalIgnoreCase))
        {
          CrossOriginEntryOptions result;
          if (System.Enum.TryParse<CrossOriginEntryOptions>(strArray2[1], true, out result))
            crossOriginEntry1.AllowedOptions = result;
        }
        else
          requestContext.Trace(0, TraceLevel.Error, "CrossOriginManagement", "Service", "Cross Origin entry {0} has unknown value {1}.", (object) registryEntry, (object) strArray1[index]);
      }
      return crossOriginEntry1;
    }

    internal IList<CrossOriginEntry> UnitTestAllowedOrigins
    {
      set => this.m_allowedOrigins = value;
    }
  }
}
