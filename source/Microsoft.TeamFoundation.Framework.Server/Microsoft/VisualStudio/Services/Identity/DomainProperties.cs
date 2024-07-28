// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.DomainProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class DomainProperties
  {
    private readonly ConcurrentDictionary<string, IDomainEntry> m_cache = new ConcurrentDictionary<string, IDomainEntry>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private bool m_settingUseRFC2247;

    internal void GetProperties(
      IVssRequestContext requestContext,
      SecurityIdentifier domainSecurityId,
      out string netbiosName,
      out string fullDomainName,
      out string domainRootPath)
    {
      if (domainSecurityId == (SecurityIdentifier) null)
        throw new ArgumentNullException(nameof (domainSecurityId));
      IDomainEntry orAdd = this.m_cache.GetOrAdd(domainSecurityId.ToString(), (Func<string, IDomainEntry>) (sid => requestContext.GetService<IDomainEntryService>().CreateDomainEntry(requestContext, domainSecurityId)));
      orAdd.RecordAccess(requestContext);
      netbiosName = orAdd.NetbiosName;
      fullDomainName = orAdd.DnsDomainName;
      domainRootPath = orAdd.DomainRootPath;
    }

    internal string ConstructFullDomainName(
      string distinguishedName,
      IVssRequestContext requestContext)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(distinguishedName) && this.m_settingUseRFC2247)
      {
        string[] strArray = distinguishedName.Split(',');
        bool flag = true;
        foreach (string str in strArray)
        {
          if (str.StartsWith("DC=", StringComparison.OrdinalIgnoreCase))
          {
            if (flag)
              flag = false;
            else
              stringBuilder.Append(".");
            stringBuilder.Append(str.Substring(3));
          }
        }
      }
      return stringBuilder.ToString();
    }

    internal bool UseRFC2247
    {
      set => this.m_settingUseRFC2247 = value;
    }
  }
}
