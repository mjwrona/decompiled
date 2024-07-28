// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Compliance.SecureFlowLocation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.VisualStudio.Services.Compliance
{
  public class SecureFlowLocation
  {
    internal const string JavaScriptNotifyProtocol = "javascriptnotify";
    internal const string ParameterDelimiter = "&";
    internal const string ProtocolParamter = "protocol";
    internal const string QueryDelimiter = "?";
    internal const string RealmParameter = "realm";
    public const string ReplyToParameter = "reply_to";
    internal const string SchemeDelimiter = "://";
    internal const string ValueDelimiter = "=";

    public static SecureFlowLocation GetDefaultLocation(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, "request context");
      return new SecureFlowLocation(new Uri(requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext).AccessPoint));
    }

    internal static void SetHostToDefault(
      SecureFlowLocation location,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<SecureFlowLocation>(location, nameof (location));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, "request context");
      Uri uri = new Uri(requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext).AccessPoint);
      location.Host = uri.Authority;
    }

    public static SecureFlowLocationState TryCreate(
      IVssRequestContext requestContext,
      Uri url,
      SecureFlowLocation defaultLocation,
      out SecureFlowLocation location)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, "request context");
      ArgumentUtility.CheckForNull<SecureFlowLocation>(defaultLocation, nameof (defaultLocation));
      location = defaultLocation.Clone();
      if (url == (Uri) null || url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps && url.Scheme != "urn")
        return SecureFlowLocationState.InvalidLocation;
      SecureFlowLocation secureFlowLocation1 = new SecureFlowLocation(url);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IInstanceManagementService service = vssRequestContext.GetService<IInstanceManagementService>();
      if (!service.IsRegisteredServiceDomain(vssRequestContext, secureFlowLocation1.HostName))
        return SecureFlowLocationState.UnregisteredLocation;
      if (requestContext.ExecutionEnvironment.IsSslOnly && secureFlowLocation1.Insecure)
        return SecureFlowLocationState.InsecureNextLocation;
      SecureFlowLocation secureFlowLocation2 = secureFlowLocation1;
      for (SecureFlowLocation nextLocation = secureFlowLocation1.NextLocation; nextLocation != null; nextLocation = secureFlowLocation2.NextLocation)
      {
        if (!service.IsRegisteredServiceDomain(vssRequestContext, nextLocation.HostName))
        {
          if (defaultLocation != null)
          {
            secureFlowLocation2.NextLocation = defaultLocation.Clone();
            location = secureFlowLocation1;
          }
          return SecureFlowLocationState.UnregisteredNextLocation;
        }
        if (requestContext.ExecutionEnvironment.IsSslOnly && nextLocation.Insecure)
        {
          if (defaultLocation != null)
          {
            secureFlowLocation2.NextLocation = defaultLocation.Clone();
            location = secureFlowLocation1;
          }
          return SecureFlowLocationState.InsecureNextLocation;
        }
        secureFlowLocation2 = nextLocation;
      }
      location = secureFlowLocation1;
      return SecureFlowLocationState.Valid;
    }

    public static SecureFlowLocationState TryCreate(
      IVssRequestContext requestContext,
      string urlText,
      SecureFlowLocation defaultLocation,
      out SecureFlowLocation location)
    {
      location = (SecureFlowLocation) null;
      Uri result;
      if (Uri.TryCreate(urlText, UriKind.Absolute, out result))
        return SecureFlowLocation.TryCreate(requestContext, result, defaultLocation, out location);
      location = defaultLocation != null ? defaultLocation.Clone() : throw new ArgumentNullException(nameof (defaultLocation));
      return SecureFlowLocationState.InvalidLocation;
    }

    internal static SecureFlowLocation ForceCreateInsecure(
      IVssRequestContext requestContext,
      string urlText)
    {
      Uri result;
      if (!Uri.TryCreate(urlText, UriKind.Absolute, out result))
        return (SecureFlowLocation) null;
      SecureFlowLocation insecure = new SecureFlowLocation(result);
      if (insecure.NextLocation == null)
        return insecure;
      string urlText1 = insecure.NextLocation.ToString();
      SecureFlowLocation location;
      int num = (int) SecureFlowLocation.TryCreate(requestContext, urlText1, SecureFlowLocation.GetDefaultLocation(requestContext), out location);
      insecure.NextLocation = location;
      return insecure;
    }

    public SecureFlowLocation GetInnermostReplyTo()
    {
      SecureFlowLocation nextLocation = this.NextLocation;
      if (nextLocation == null)
        return this;
      while (nextLocation.NextLocation != null)
        nextLocation = nextLocation.NextLocation;
      return nextLocation;
    }

    private SecureFlowLocation(Uri url)
    {
      this.Host = url.Authority;
      this.Insecure = url.Scheme == Uri.UriSchemeHttp;
      this.Parameters = url.ParseQueryString();
      this.Path = url.AbsolutePath;
      string parameter = this.Parameters["reply_to"];
      if (parameter == null)
        return;
      Uri result = (Uri) null;
      if (Uri.TryCreate(parameter, UriKind.Absolute, out result))
        this.NextLocation = new SecureFlowLocation(result);
      this.Parameters.Remove("reply_to");
    }

    private SecureFlowLocation()
    {
    }

    public SecureFlowLocation Clone()
    {
      SecureFlowLocation secureFlowLocation = new SecureFlowLocation();
      secureFlowLocation.Host = this.Host;
      secureFlowLocation.Insecure = this.Insecure;
      secureFlowLocation.Parameters = new NameValueCollection(this.Parameters);
      secureFlowLocation.Path = this.Path;
      if (this.NextLocation != null)
        secureFlowLocation.NextLocation = this.NextLocation.Clone();
      return secureFlowLocation;
    }

    public string Host { get; private set; }

    public string HostName
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.Host))
          return this.Host;
        int length = this.Host.IndexOf(':');
        return length == -1 ? this.Host : this.Host.Substring(0, length);
      }
    }

    public string Realm
    {
      get => this.Parameters["realm"];
      set
      {
        if (value == null)
          this.Parameters.Remove("realm");
        else
          this.Parameters["realm"] = value;
      }
    }

    public bool Insecure { get; set; }

    public SecureFlowLocation NextLocation { get; set; }

    public NameValueCollection Parameters { get; set; }

    public string Path { get; set; }

    public void ApplyParameters(
      NameValueCollection parameters,
      IDictionary<string, string> overrides = null)
    {
      foreach (string parameter in (NameObjectCollectionBase) parameters)
      {
        if (parameter != "reply_to")
          this.Parameters[parameter] = parameters[parameter];
      }
      if (overrides == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) overrides)
      {
        if (keyValuePair.Value == null)
          this.Parameters.Remove(keyValuePair.Key);
        else if (keyValuePair.Key != "reply_to")
          this.Parameters[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public string GetQuery(string prefix = "?", bool includeNextLocation = true)
    {
      string query = this.Parameters.ToQuery(prefix);
      if (includeNextLocation && this.NextLocation != null)
      {
        string str1 = Uri.EscapeDataString(this.NextLocation.ToString());
        string str2 = string.IsNullOrEmpty(query) ? prefix : "&";
        query = query + str2 + "reply_to" + "=" + str1;
      }
      return query;
    }

    public string GetScheme() => !this.Insecure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

    public bool HasSameTarget(SecureFlowLocation location, bool useHostName = false)
    {
      ArgumentUtility.CheckForNull<SecureFlowLocation>(location, nameof (location));
      return VssStringComparer.DomainName.Equals(useHostName ? this.HostName : this.Host, useHostName ? location.HostName : location.Host) && VssStringComparer.UrlPath.Equals(this.Path, location.Path);
    }

    public override string ToString() => this.GetScheme() + "://" + this.Host + this.Path + this.GetQuery();
  }
}
