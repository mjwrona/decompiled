// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Utility.IdentityDisplayName
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Common.Utility
{
  public class IdentityDisplayName
  {
    private static readonly Regex s_scopeRegex = new Regex("^\\[[0-9A-Za-z -]+\\]\\\\(.+)<([0-9A-Fa-f]{8}(?:-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})>$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_vsidRegex = new Regex("^\\[[0-9A-Za-z -]+\\]\\\\(.+)<id:([0-9A-Fa-f]{8}(?:-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})>$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_domainAccountRegex = new Regex("^.+<(.+\\\\.+)>$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_accountNameRegex = new Regex("^.+<(.+@.+)>$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_displayNameRegex = new Regex("^[^<\\\\]*(?:<[^>]*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public const string VstsGroupDisambiguatedPartPrefix = "id:";

    public Guid ScopeId { get; internal set; }

    public Guid Vsid { get; internal set; }

    public string Domain { get; internal set; }

    public string AccountName { get; internal set; }

    public string DisplayName { get; internal set; }

    public SearchTermType Type { get; internal set; }

    private IdentityDisplayName() => this.Type = SearchTermType.Unknown;

    public static IdentityDisplayName GetDisambiguatedSearchTerm(string search)
    {
      IdentityDisplayName disambiguatedSearchTerm = new IdentityDisplayName();
      if (string.IsNullOrEmpty(search))
        return disambiguatedSearchTerm;
      Guid vsid;
      string displayName;
      if (IdentityDisplayName.TryGetVsid(search, out vsid, out displayName))
      {
        disambiguatedSearchTerm.Type = SearchTermType.Vsid;
        disambiguatedSearchTerm.DisplayName = displayName;
        disambiguatedSearchTerm.Vsid = vsid;
      }
      else
      {
        string str;
        if (IdentityDisplayName.TryGetDomainAndAccountName(search, out str))
        {
          string[] strArray = str.Split('\\');
          if (strArray.Length == 2)
          {
            disambiguatedSearchTerm.Domain = strArray[0];
            disambiguatedSearchTerm.AccountName = strArray[1];
            disambiguatedSearchTerm.Type = SearchTermType.DomainAndAccountName;
          }
        }
        else if (IdentityDisplayName.TryGetAccountName(search, out str))
        {
          disambiguatedSearchTerm.AccountName = str;
          disambiguatedSearchTerm.Type = SearchTermType.AccoutName;
        }
        else if (IdentityDisplayName.TryGetDisplayName(search, out str))
        {
          disambiguatedSearchTerm.DisplayName = str;
          disambiguatedSearchTerm.Type = SearchTermType.DisplayName;
        }
        else
        {
          Guid scopeId;
          if (IdentityDisplayName.TryGetScope(search, out scopeId, out displayName))
          {
            disambiguatedSearchTerm.ScopeId = scopeId;
            disambiguatedSearchTerm.DisplayName = displayName;
            disambiguatedSearchTerm.Type = SearchTermType.Scope;
          }
        }
      }
      return disambiguatedSearchTerm;
    }

    private static bool TryGetScope(string search, out Guid scopeId, out string displayName)
    {
      Match match = IdentityDisplayName.s_scopeRegex.Match(search);
      if (match.Success && match.Groups.Count > 1)
      {
        displayName = match.Groups[1].Value;
        Guid.TryParse(match.Groups[2].Value, out scopeId);
        return true;
      }
      scopeId = Guid.Empty;
      displayName = string.Empty;
      return false;
    }

    private static bool TryGetVsid(string search, out Guid vsid, out string displayName)
    {
      Match match = IdentityDisplayName.s_vsidRegex.Match(search);
      if (match.Success && match.Groups.Count > 1)
      {
        displayName = match.Groups[1].Value;
        Guid.TryParse(match.Groups[2].Value, out vsid);
        return true;
      }
      vsid = Guid.Empty;
      displayName = string.Empty;
      return false;
    }

    private static bool TryGetDomainAndAccountName(string search, out string domainAndAcccountName)
    {
      Match match = IdentityDisplayName.s_domainAccountRegex.Match(search);
      domainAndAcccountName = (string) null;
      if (!match.Success || match.Groups.Count <= 1 || !match.Groups[1].Value.Contains("\\"))
        return false;
      domainAndAcccountName = match.Groups[1].Value;
      return true;
    }

    private static bool TryGetAccountName(string search, out string acccountName)
    {
      Match match = IdentityDisplayName.s_accountNameRegex.Match(search);
      acccountName = (string) null;
      if (!match.Success || match.Groups.Count <= 1)
        return false;
      acccountName = match.Groups[1].Value;
      return true;
    }

    private static bool TryGetDisplayName(string search, out string displayName)
    {
      Match match = IdentityDisplayName.s_displayNameRegex.Match(search);
      displayName = (string) null;
      if (!match.Success || match.Groups.Count <= 0)
        return false;
      displayName = match.Groups[0].Value;
      return true;
    }
  }
}
