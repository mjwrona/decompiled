// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestContentTypeRestrictionAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public sealed class RequestContentTypeRestrictionAttribute : AuthorizationFilterAttribute
  {
    private HashSet<string> m_allowedContentTypes;
    private HashSet<string> m_methodsToRestrict;
    private const string c_ApplicationCspReportContentType = "application/csp-report";
    private const string c_ApplicationJsonContentType = "application/json";
    private const string c_ApplicationJsonPatchContentType = "application/json-patch+json";
    private const string c_ApplicationJsonAPIContentType = "application/vnd.api+json";
    private const string c_ApplicationXmlContentType = "application/xml";
    private const string c_ApplicationFormUrlEncodedContentType = "application/x-www-form-urlencoded";
    private const string c_ApplicationOctetStream = "application/octet-stream";
    private const string c_ApplicationZip = "application/zip";
    private const string c_MultipartRelated = "multipart/related";
    private const string c_TextPlain = "text/plain";
    private static readonly string[] s_defaultContentTypes = new string[2]
    {
      "application/json",
      "application/json-patch+json"
    };
    private static readonly string[] s_defaultMethods = new string[4]
    {
      "POST",
      "PUT",
      "PATCH",
      "DELETE"
    };
    private static readonly string[] s_allowedUserAgentSubStringsForEmptyContentType = new string[6]
    {
      "(TFSBuildServiceHost.exe)",
      "vsoagent.exe",
      "vso-task-api",
      "(ReleaseManagementConsole.exe)",
      "(ReleaseManagementGettingStarted.exe)",
      "VSServices"
    };
    private static readonly string s_emptyContentTypeUserAgentWhitelistPath = "/Services/WebServices/EmptyContentUserAgentWhitelist";

    public RequestContentTypeRestrictionAttribute()
    {
      this.m_allowedContentTypes = new HashSet<string>((IEnumerable<string>) RequestContentTypeRestrictionAttribute.s_defaultContentTypes, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_methodsToRestrict = new HashSet<string>((IEnumerable<string>) RequestContentTypeRestrictionAttribute.s_defaultMethods, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public bool AllowJson
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("application/json");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("application/json");
        else
          this.m_allowedContentTypes.Remove("application/json");
      }
    }

    public bool AllowJsonPatch
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("application/json-patch+json");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("application/json-patch+json");
        else
          this.m_allowedContentTypes.Remove("application/json-patch+json");
      }
    }

    public bool AllowJsonAPIContent
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("application/vnd.api+json");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("application/vnd.api+json");
        else
          this.m_allowedContentTypes.Remove("application/vnd.api+json");
      }
    }

    public bool AllowForm
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("application/x-www-form-urlencoded");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("application/x-www-form-urlencoded");
        else
          this.m_allowedContentTypes.Remove("application/x-www-form-urlencoded");
      }
    }

    public bool AllowXml
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("application/xml");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("application/xml");
        else
          this.m_allowedContentTypes.Remove("application/xml");
      }
    }

    public bool AllowZip
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("application/zip");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("application/zip");
        else
          this.m_allowedContentTypes.Remove("application/zip");
      }
    }

    public bool AllowStream
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("application/octet-stream");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("application/octet-stream");
        else
          this.m_allowedContentTypes.Remove("application/octet-stream");
      }
    }

    public bool AllowCspReport
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("application/csp-report");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("application/csp-report");
        else
          this.m_allowedContentTypes.Remove("application/csp-report");
      }
    }

    public bool AllowMultipartRelated
    {
      get => this.AllowAllContentTypes || this.m_allowedContentTypes.Contains("multipart/related");
      set
      {
        if (value)
          this.m_allowedContentTypes.Add("multipart/related");
        else
          this.m_allowedContentTypes.Remove("multipart/related");
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowEmptyContentType { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowEmptyOrPlainTextContentTypeForCompatServiceCalls { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowAllContentTypes { get; set; }

    public IEnumerable<string> HttpMethodsToRestrict
    {
      get => (IEnumerable<string>) this.m_methodsToRestrict;
      set => this.m_methodsToRestrict = new HashSet<string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public string[] SafeCrossOriginUserAgents { get; set; }

    public override void OnAuthorization(HttpActionContext actionContext)
    {
      if (this.AllowAllContentTypes)
        return;
      string method = actionContext.Request.Method.Method;
      if (!this.m_methodsToRestrict.Contains(method))
        return;
      IVssRequestContext ivssRequestContext = actionContext.Request.GetIVssRequestContext();
      IVssRequestContext context = ivssRequestContext.To(TeamFoundationHostType.Deployment);
      ICrossOriginManagementService service = context.GetService<ICrossOriginManagementService>();
      string simpleHeaderValue = actionContext.Request.GetSimpleHeaderValue("Origin");
      IVssRequestContext requestContext = context;
      string origin = simpleHeaderValue;
      if (service.IsUnsafeCrossOriginRequest(requestContext, origin))
      {
        Dictionary<string, string> additionalProperties = new Dictionary<string, string>()
        {
          {
            "Origin",
            simpleHeaderValue ?? string.Empty
          }
        };
        bool flag = ivssRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.RestApi.RejectUnsafeOriginRequests");
        if (!string.IsNullOrEmpty(simpleHeaderValue) && simpleHeaderValue.StartsWith("chrome-extension://"))
        {
          flag = false;
          additionalProperties["Extension"] = "true";
        }
        if (this.SafeCrossOriginUserAgents != null)
        {
          foreach (string crossOriginUserAgent in this.SafeCrossOriginUserAgents)
          {
            if (Regex.IsMatch(ivssRequestContext.UserAgent, crossOriginUserAgent))
            {
              flag = false;
              additionalProperties["SafeUserAgent"] = "true";
              break;
            }
          }
        }
        additionalProperties["Reject"] = flag.ToString();
        this.CreateCIEvent(actionContext, "RESTUnsafeCrossOriginRequest", "RequestUsedCookiesFromUnsafeOrigin", additionalProperties);
        if (flag)
          throw new VssApiUnsafeCrossOriginRequestException(simpleHeaderValue);
      }
      if (actionContext.Request.Content == null || actionContext.Request.Content.Headers.ContentLength.GetValueOrDefault() == 0L && string.IsNullOrEmpty(actionContext.Request.Content.Headers.ContentType?.MediaType))
        return;
      string contentType = (string) null;
      if (actionContext.Request.Content.Headers.ContentType != null)
        contentType = actionContext.Request.Content.Headers.ContentType.MediaType;
      bool flag1 = false;
      if (string.IsNullOrEmpty(contentType) || contentType.Equals("text/plain"))
      {
        if (this.AllowEmptyContentType && string.IsNullOrEmpty(contentType))
          flag1 = true;
        else if (this.AllowEmptyOrPlainTextContentTypeForCompatServiceCalls)
        {
          if (actionContext.Request.Headers.UserAgent != null)
          {
            string str1 = actionContext.Request.Headers.UserAgent.ToString();
            string str2 = ivssRequestContext.GetService<IVssRegistryService>().GetValue(ivssRequestContext, (RegistryQuery) RequestContentTypeRestrictionAttribute.s_emptyContentTypeUserAgentWhitelistPath, true, "");
            IEnumerable<string> second = Enumerable.Empty<string>();
            if (!string.IsNullOrEmpty(str2))
              second = (IEnumerable<string>) str2.Split(';');
            foreach (string str3 in ((IEnumerable<string>) RequestContentTypeRestrictionAttribute.s_allowedUserAgentSubStringsForEmptyContentType).Union<string>(second))
            {
              if (str1.IndexOf(str3, StringComparison.InvariantCultureIgnoreCase) >= 0)
              {
                flag1 = true;
                break;
              }
            }
          }
          if (!flag1)
          {
            Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = ivssRequestContext.GetAuthenticatedIdentity();
            flag1 = authenticatedIdentity != null && (ServicePrincipals.IsServicePrincipal(ivssRequestContext, authenticatedIdentity.Descriptor) || IdentityHelper.IsAcsServiceIdentity((IReadOnlyVssIdentity) authenticatedIdentity));
            if (!flag1 && (actionContext.Request.Headers.UserAgent == null || actionContext.Request.Headers.UserAgent.ToString() == string.Empty))
            {
              this.CreateCIEvent(actionContext, "RESTEmptyContentTypeNoUA", "RequestContainedContentWithEmptyContentType");
              flag1 = true;
            }
          }
        }
      }
      else
        flag1 = this.m_allowedContentTypes.Contains(contentType);
      if (!flag1)
      {
        if (string.IsNullOrEmpty(contentType) && this.m_allowedContentTypes.Contains("application/octet-stream"))
          this.CreateCIEvent(actionContext, "RESTEmptyContentTypeBlocked", "RequestContainedContentWithEmptyContentType");
        throw new VssRequestContentTypeNotSupportedException(contentType, method, (IEnumerable<string>) this.m_allowedContentTypes);
      }
    }

    private void CreateCIEvent(
      HttpActionContext actionContext,
      string feature,
      string action,
      Dictionary<string, string> additionalProperties = null)
    {
      ApiResourceVersion apiResourceVersion = actionContext.Request.GetApiResourceVersion();
      IVssRequestContext ivssRequestContext = actionContext.Request.GetIVssRequestContext();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, action);
      properties.Add("UserAgent", ivssRequestContext.UserAgent);
      properties.Add("HttpMethod", actionContext.Request.Method.ToString());
      properties.Add("Uri", (object) actionContext.Request.RequestUri);
      properties.Add("ControllerName", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName);
      properties.Add("ControllerType", actionContext.ActionDescriptor.ControllerDescriptor.ControllerType.ToString());
      properties.Add("ActionName", actionContext.ActionDescriptor.ActionName);
      properties.Add("ServiceName", ivssRequestContext.ServiceName);
      if (apiResourceVersion != null)
        properties.Add("ApiResourceVersion", actionContext.Request.GetApiResourceVersion().ToString());
      if (additionalProperties != null)
      {
        foreach (KeyValuePair<string, string> additionalProperty in additionalProperties)
          properties.Add(additionalProperty.Key, additionalProperty.Value);
      }
      ivssRequestContext.GetService<CustomerIntelligenceService>().Publish(ivssRequestContext, "REST", feature, properties);
    }
  }
}
