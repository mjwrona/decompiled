// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ProxyInfo
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ProxyInfo : IValidatable
  {
    internal int Id;
    internal ProxyFlags Flags;
    private string m_url;
    private string m_friendlyName;
    private string m_site;
    private string m_description;

    [XmlAttribute("url")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Url
    {
      set => this.m_url = value;
      get => this.m_url;
    }

    [XmlAttribute("fn")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string FriendlyName
    {
      set => this.m_friendlyName = value;
      get => this.m_friendlyName;
    }

    [XmlAttribute("site")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Site
    {
      set => this.m_site = value;
      get => this.m_site;
    }

    [XmlAttribute("desc")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Description
    {
      set => this.m_description = value;
      get => this.m_description;
    }

    [XmlAttribute("flags")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "Flags")]
    public int FlagsValue
    {
      set => this.Flags = (ProxyFlags) value;
      get => (int) this.Flags;
    }

    internal static void AddProxy(
      VersionControlRequestContext versionControlRequestContext,
      ProxyInfo proxy)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckGlobalPermission(versionControlRequestContext, GlobalPermissions.AdminConfiguration);
      TeamFoundationProxyService service = versionControlRequestContext.RequestContext.GetService<TeamFoundationProxyService>();
      Microsoft.TeamFoundation.Core.WebApi.Proxy proxy1 = new Microsoft.TeamFoundation.Core.WebApi.Proxy();
      proxy1.Url = proxy.Url;
      proxy1.Site = proxy.Site;
      proxy1.FriendlyName = proxy.FriendlyName;
      proxy1.Description = proxy.Description;
      if ((proxy.Flags & ProxyFlags.SiteDefault) == ProxyFlags.SiteDefault)
        proxy1.SiteDefault = new bool?(true);
      if ((proxy.Flags & ProxyFlags.GlobalDefault) == ProxyFlags.GlobalDefault)
        proxy1.GlobalDefault = new bool?(true);
      IVssRequestContext requestContext = versionControlRequestContext.RequestContext.Elevate();
      Microsoft.TeamFoundation.Core.WebApi.Proxy proxy2 = proxy1;
      service.AddProxy(requestContext, proxy2);
    }

    internal static void DeleteProxy(
      VersionControlRequestContext versionControlRequestContext,
      string proxyUrl)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckGlobalPermission(versionControlRequestContext, GlobalPermissions.AdminConfiguration);
      versionControlRequestContext.RequestContext.GetService<TeamFoundationProxyService>().DeleteProxy(versionControlRequestContext.RequestContext.Elevate(), proxyUrl, (string) null);
    }

    internal static ProxyInfo[] QueryProxies(
      VersionControlRequestContext versionControlRequestContext,
      string[] proxyUrls)
    {
      TeamFoundationProxyService service = versionControlRequestContext.RequestContext.GetService<TeamFoundationProxyService>();
      List<string> stringList = new List<string>();
      if (proxyUrls != null)
        stringList = ((IEnumerable<string>) proxyUrls).ToList<string>();
      IVssRequestContext requestContext = versionControlRequestContext.RequestContext;
      List<string> proxyUrls1 = stringList;
      List<Microsoft.TeamFoundation.Core.WebApi.Proxy> proxyList = service.QueryProxies(requestContext, (IList<string>) proxyUrls1);
      List<ProxyInfo> proxyInfoList = new List<ProxyInfo>();
      foreach (Microsoft.TeamFoundation.Core.WebApi.Proxy proxy in proxyList)
      {
        ProxyInfo proxyInfo = new ProxyInfo();
        proxyInfo.Url = proxy.Url;
        proxyInfo.Site = proxy.Site;
        proxyInfo.FriendlyName = proxy.FriendlyName;
        proxyInfo.Description = proxy.Description;
        proxyInfo.FlagsValue = 0;
        bool? nullable = proxy.SiteDefault;
        if (nullable.GetValueOrDefault())
          ++proxyInfo.FlagsValue;
        nullable = proxy.GlobalDefault;
        if (nullable.GetValueOrDefault())
          proxyInfo.FlagsValue += 2;
        proxyInfoList.Add(proxyInfo);
      }
      return proxyInfoList.ToArray();
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkProxyUrl(this.m_url, "url", false);
      if (string.IsNullOrEmpty(this.m_friendlyName))
        this.m_friendlyName = (string) null;
      if (string.IsNullOrEmpty(this.m_site))
      {
        this.m_site = (string) null;
        this.Flags &= ~ProxyFlags.SiteDefault;
      }
      if (!string.IsNullOrEmpty(this.m_description))
        return;
      this.m_description = (string) null;
    }
  }
}
