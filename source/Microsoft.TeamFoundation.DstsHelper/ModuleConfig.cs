// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DstsHelper.ModuleConfig
// Assembly: Microsoft.TeamFoundation.DstsHelper, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08D47267-3A15-4307-BBA0-1792E9C6BDF1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DstsHelper.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DstsHelper
{
  internal class ModuleConfig
  {
    private static readonly RegistryQuery SysQuery = new RegistryQuery("/Configuration/dSTSAuth/sys/*");

    public string Authority { get; }

    public string ServerId { get; }

    public bool PassiveAuthEnabled { get; }

    public ISet<string> RedirectAuthorities { get; }

    public Uri BaseUrlForOtherAuthority { get; }

    public static ModuleConfig Load(IVssRequestContext requestContext)
    {
      bool result1 = false;
      string authority = (string) null;
      string serverId = (string) null;
      bool result2 = false;
      string redirectDns = (string) null;
      bool result3 = false;
      foreach (RegistryItem registryItem in requestContext.GetService<IVssRegistryService>().Read(requestContext, in ModuleConfig.SysQuery))
      {
        if (registryItem.Path.Equals("/Configuration/dSTSAuth/sys/enabled", StringComparison.OrdinalIgnoreCase))
          bool.TryParse(registryItem.Value, out result1);
        else if (registryItem.Path.Equals("/Configuration/dSTSAuth/sys/authority", StringComparison.OrdinalIgnoreCase))
          authority = registryItem.Value;
        else if (registryItem.Path.Equals("/Configuration/dSTSAuth/sys/serverId", StringComparison.OrdinalIgnoreCase))
          serverId = registryItem.Value;
        else if (registryItem.Path.Equals("/Configuration/dSTSAuth/sys/passiveAuthEnabled", StringComparison.OrdinalIgnoreCase))
          bool.TryParse(registryItem.Value, out result2);
        else if (registryItem.Path.Equals("/Configuration/dSTSAuth/sys/redirectDns", StringComparison.OrdinalIgnoreCase))
          redirectDns = registryItem.Value;
        else if (registryItem.Path.Equals("/Configuration/dSTSAuth/sys/redirectOther", StringComparison.OrdinalIgnoreCase))
          bool.TryParse(registryItem.Value, out result3);
      }
      return !result1 ? (ModuleConfig) null : new ModuleConfig(authority, serverId, result2, redirectDns, result3);
    }

    public static string GetAccountNameByOid(IVssRequestContext requestContext, string oid)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery = new RegistryQuery("/Configuration/dSTSAuth/oid/" + oid, false);
      IVssRequestContext requestContext1 = requestContext;
      ref RegistryQuery local = ref registryQuery;
      return service.GetValue(requestContext1, in local);
    }

    public ModuleConfig(
      string authority,
      string serverId,
      bool passiveAuthEnabled,
      string redirectDns,
      bool redirectOther)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(authority, nameof (authority));
      if (!Uri.IsWellFormedUriString(authority, UriKind.Absolute))
        throw new ArgumentException("The authority must be a well-formed absolute URL.", nameof (authority));
      if (!authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("The authority must use https scheme.", nameof (authority));
      ArgumentUtility.CheckStringForNullOrEmpty(serverId, nameof (serverId));
      ISet<string> stringSet = (ISet<string>) null;
      Uri uri = (Uri) null;
      if (!string.IsNullOrEmpty(redirectDns))
      {
        List<string> stringList = new List<string>();
        string str1 = redirectDns;
        char[] chArray = new char[1]{ ',' };
        foreach (string str2 in str1.Split(chArray))
        {
          if (!string.IsNullOrWhiteSpace(str2))
          {
            string str3 = str2.Trim();
            int startIndex = str3.IndexOf(':');
            string name = startIndex < 0 ? str3 : str3.Remove(startIndex);
            string s = startIndex < 0 ? (string) null : str3.Substring(startIndex + 1);
            ushort result = 443;
            if (Uri.CheckHostName(name) != UriHostNameType.Dns || s != null && !ushort.TryParse(s, NumberStyles.None, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result) || !Uri.IsWellFormedUriString("https://" + str3 + "/_signedin", UriKind.Absolute))
              throw new ArgumentException("The redirectDns must be in format [<DNS>[:<port>][,<DNS>[:<port>]...]]].", nameof (redirectDns));
            if (result == (ushort) 443)
              str3 = name;
            if (!stringList.Contains<string>(str3, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
              stringList.Add(str3);
          }
        }
        if (stringList.Count > 0)
        {
          stringSet = (ISet<string>) new HashSet<string>((IEnumerable<string>) stringList, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          if (redirectOther)
            uri = new Uri("https://" + stringList[0]);
        }
      }
      this.Authority = authority;
      this.ServerId = serverId;
      this.PassiveAuthEnabled = passiveAuthEnabled;
      this.RedirectAuthorities = stringSet;
      this.BaseUrlForOtherAuthority = uri;
    }
  }
}
