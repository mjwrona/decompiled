// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.WindowsProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class WindowsProvider : IIdentityProvider, IVssFrameworkService
  {
    private static readonly string[] s_identityAttributes = new string[9]
    {
      "objectClass",
      "groupType",
      "objectSid",
      "objectGuid",
      "displayName",
      "description",
      "sAMAccountName",
      "distinguishedName",
      "mail"
    };
    private const string ADUserAccountControlProperty = "userAccountControl";
    private const string SAMUserAccountControlProperty = "UserFlags";
    private readonly DomainProperties m_domainProperties;
    private bool m_settingUseGlobalCatalog;
    private bool m_settingUseAccountDisplayMode;
    private bool m_deactivateMigratedIdentities;
    private bool m_deactivateIdentitiesWithDifferingSid;
    private DirectoryEntry m_globalCatalogEntry;
    private uint m_globalCatalogAccess;
    private static readonly string[] s_supportedTypes = new string[1]
    {
      "System.Security.Principal.WindowsIdentity"
    };
    private static readonly string s_localMachineName = Environment.MachineName.Trim();

    private void SyncADIdentity(
      IVssRequestContext requestContext,
      ref Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool includeMembers,
      SyncErrors syncErrors)
    {
      SecurityIdentifierInfo data = SidDescriptor.GetData(identity.Descriptor);
      SecurityIdentifier accountDomainSid = data.SecurityId.AccountDomainSid;
      string netbiosName;
      string fullDomainName;
      this.m_domainProperties.GetProperties(requestContext, accountDomainSid, out netbiosName, out fullDomainName, out string _);
      identity.SetProperty("Domain", (object) netbiosName);
      using (DirectoryEntry directoryEntry = DirectoryEntryFactory.CreateDirectoryEntry(WindowsProvider.BuildADSIPathFromSID(fullDomainName, data.GetBinaryForm())))
      {
        this.GetIdentityFromDirectoryEntry(requestContext, directoryEntry, ref identity, syncErrors, false);
        if (((identity == null ? 0 : (identity.IsActive ? 1 : 0)) & (includeMembers ? 1 : 0)) == 0 || !identity.IsContainer)
          return;
        Dictionary<string, IdentityDescriptor> members = new Dictionary<string, IdentityDescriptor>();
        this.GetMembersDirect(requestContext, false, directoryEntry, fullDomainName, members, (IIdentitySyncHelper) null, syncErrors);
        this.GetMembersPrimary(requestContext, false, data, members, (IIdentitySyncHelper) null, syncErrors);
        identity.Members = (ICollection<IdentityDescriptor>) members.Values;
      }
    }

    private void SyncADGroup(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity group,
      IIdentitySyncHelper syncHelper,
      IDictionary<string, IIdentityProvider> syncAgents,
      SyncErrors syncErrors)
    {
      SecurityIdentifierInfo data = SidDescriptor.GetData(group.Descriptor);
      SecurityIdentifier accountDomainSid = data.SecurityId.AccountDomainSid;
      string netbiosName;
      string fullDomainName;
      this.m_domainProperties.GetProperties(requestContext, accountDomainSid, out netbiosName, out fullDomainName, out string _);
      group.SetProperty("Domain", (object) netbiosName);
      using (DirectoryEntry directoryEntry = DirectoryEntryFactory.CreateDirectoryEntry(WindowsProvider.BuildADSIPathFromSID(fullDomainName, data.GetBinaryForm())))
      {
        this.GetIdentityFromDirectoryEntry(requestContext, directoryEntry, ref group, syncErrors, false);
        if (group == null || !group.IsActive || !group.IsContainer)
          return;
        string property = group.GetProperty<string>("DN", string.Empty);
        int num1 = this.GlobalCatalogSearch(requestContext, property, syncHelper, syncErrors);
        int num2 = 0;
        if (num1 == 0)
          num2 = this.AttributeScopeQuery(requestContext, directoryEntry, syncHelper, syncErrors);
        int membersDirect = this.GetMembersDirect(requestContext, true, directoryEntry, fullDomainName, (Dictionary<string, IdentityDescriptor>) null, syncHelper, syncErrors);
        int membersPrimary = this.GetMembersPrimary(requestContext, true, data, (Dictionary<string, IdentityDescriptor>) null, syncHelper, syncErrors);
        syncHelper.SyncCounters.Add("Global Catalog", num1);
        syncHelper.SyncCounters.Add("Attribute Scope Query", num2);
        syncHelper.SyncCounters.Add("Member Attribute", membersDirect);
        syncHelper.SyncCounters.Add("Primary Group", membersPrimary);
      }
    }

    private int GlobalCatalogSearch(
      IVssRequestContext requestContext,
      string groupDn,
      IIdentitySyncHelper syncHelper,
      SyncErrors syncErrors)
    {
      int num = 0;
      if (!this.m_settingUseGlobalCatalog)
      {
        IdentityServiceBase.Trace(TraceLevel.Info, "AdAccessor.GlobalCatalogSearch disabled");
        return num;
      }
      try
      {
        if (this.m_globalCatalogEntry == null)
        {
          ++this.m_globalCatalogAccess;
          if (this.m_globalCatalogAccess % 256U == 1U)
          {
            using (DirectoryEntry directoryEntry = DirectoryEntryFactory.CreateDirectoryEntry("GC:"))
            {
              IEnumerator enumerator = directoryEntry.Children.GetEnumerator();
              if (enumerator.MoveNext())
                this.m_globalCatalogEntry = (DirectoryEntry) enumerator.Current;
            }
          }
        }
        if (this.m_globalCatalogEntry != null)
        {
          IdentityServiceBase.Trace(TraceLevel.Info, "AdAccessor.GlobalCatalogSearch for {0}", (object) groupDn);
          DirectoryEntry directoryEntry = DirectoryEntryFactory.CreateDirectoryEntry(this.m_globalCatalogEntry.Path);
          string filter = "(memberOf=" + groupDn + ")";
          using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry, filter, WindowsProvider.s_identityAttributes, SearchScope.Subtree))
          {
            directorySearcher.PageSize = 200;
            directorySearcher.ClientTimeout = TimeSpan.FromMinutes(2.0);
            using (SearchResultCollection all = directorySearcher.FindAll())
            {
              foreach (SearchResult result in all)
              {
                Microsoft.VisualStudio.Services.Identity.Identity fromSearchResult = this.GetIdentityFromSearchResult(requestContext, result, syncErrors);
                if (fromSearchResult != null && fromSearchResult.IsActive)
                {
                  ++num;
                  IdentityServiceBase.Trace(TraceLevel.Verbose, "AdAccessor.GlobalCatalogSearch found member: {0}", (object) fromSearchResult.DisplayName);
                  syncHelper.ProcessIdentity(fromSearchResult);
                }
              }
            }
          }
          this.m_globalCatalogEntry = directoryEntry;
        }
      }
      catch (Exception ex)
      {
        this.m_globalCatalogEntry = (DirectoryEntry) null;
        IdentityServiceBase.Trace(TraceLevel.Warning, "ADAccessor.GetGroupMemberADSearch for {0}, {1}", (object) groupDn, (object) ex);
      }
      return num;
    }

    private int AttributeScopeQuery(
      IVssRequestContext requestContext,
      DirectoryEntry groupEntry,
      IIdentitySyncHelper syncHelper,
      SyncErrors syncErrors)
    {
      IdentityServiceBase.Trace(TraceLevel.Info, "AdAccessor.AttributeScopeQuery for {0}", (object) groupEntry.Path);
      int num = 0;
      using (DirectorySearcher directorySearcher = new DirectorySearcher(groupEntry, "(objectClass=*)", WindowsProvider.s_identityAttributes, SearchScope.Base))
      {
        directorySearcher.AttributeScopeQuery = "member";
        directorySearcher.PageSize = 200;
        directorySearcher.ClientTimeout = TimeSpan.FromMinutes(10.0);
        SearchResultCollection resultCollection = (SearchResultCollection) null;
        try
        {
          resultCollection = directorySearcher.FindAll();
          foreach (SearchResult result in resultCollection)
          {
            Microsoft.VisualStudio.Services.Identity.Identity fromSearchResult = this.GetIdentityFromSearchResult(requestContext, result, syncErrors);
            if (fromSearchResult != null && fromSearchResult.IsActive)
            {
              ++num;
              IdentityServiceBase.Trace(TraceLevel.Verbose, "AdAccessor.AttributeScopeQuery found member {0}", (object) fromSearchResult.DisplayName);
              syncHelper.ProcessIdentity(fromSearchResult);
            }
          }
        }
        catch
        {
        }
        finally
        {
          resultCollection?.Dispose();
        }
      }
      return num;
    }

    internal void GetIdentityFromDirectoryEntry(
      IVssRequestContext requestContext,
      DirectoryEntry entry,
      ref Microsoft.VisualStudio.Services.Identity.Identity identity,
      SyncErrors syncErrors,
      bool faultTolerant)
    {
      bool flag = false;
      try
      {
        entry.RefreshCache(WindowsProvider.s_identityAttributes);
        identity.SetProperty("SchemaClassName", (object) "User");
        if (entry.Properties.Contains("objectClass"))
        {
          PropertyValueCollection property = entry.Properties["objectClass"];
          for (int index = property.Count - 1; index >= 0; --index)
          {
            string a = property[index] as string;
            if (!string.IsNullOrEmpty(a) && string.Equals(a, "group", StringComparison.OrdinalIgnoreCase))
            {
              identity.IsContainer = true;
              identity.SetProperty("SchemaClassName", (object) "Group");
              break;
            }
          }
        }
        if (identity.IsContainer && entry.Properties.Contains("groupType") && ((int) entry.Properties["groupType"].Value & int.MinValue) != 0)
          identity.SetProperty("SecurityGroup", (object) "SecurityGroup");
        if (entry.Properties.Contains("sAMAccountName"))
        {
          string str = entry.Properties["sAMAccountName"].Value as string;
          identity.SetProperty("Account", (object) str);
          identity.ProviderDisplayName = str;
        }
        if (entry.Properties.Contains("description"))
          identity.SetProperty("Description", (object) (entry.Properties["description"].Value as string));
        if (entry.Properties.Contains("distinguishedName"))
          identity.SetProperty("DN", (object) (entry.Properties["distinguishedName"].Value as string));
        if (!this.m_settingUseAccountDisplayMode && entry.Properties.Contains("displayName"))
          identity.ProviderDisplayName = entry.Properties["displayName"].Value as string;
        if (entry.Properties.Contains("mail"))
          identity.SetProperty("Mail", (object) (entry.Properties["mail"].Value as string));
        if (entry.Properties.Contains("objectGuid"))
          identity.Id = entry.Guid;
        if (this.DoesIdentityWithSameIdButDifferentDescriptorExist(requestContext, identity))
          this.SetIdentityToNull(ref identity, "ADAccessor.GetIdentityFromDirectoryEntry");
        else if (string.IsNullOrEmpty(identity.GetProperty<string>("DN", (string) null)) || string.IsNullOrEmpty(identity.GetProperty<string>("Domain", (string) null)) || this.IsDirectoryEntryDisabled(entry, "userAccountControl"))
        {
          identity.IsActive = false;
          IdentityServiceBase.Trace(TraceLevel.Warning, "Invalid identity {0} from ADAccessor.GetIdentityFromDirectoryEntry", (object) entry.Path);
        }
        else
          identity.IsActive = true;
      }
      catch (COMException ex)
      {
        flag = true;
        ex.Data.Add((object) "GetIdentityFromDirectoryEntry Path", (object) entry.Path);
        IdentityServiceBase.Trace(TraceLevel.Warning, "ADAccessor.GetIdentityFromDirectoryEntry - error {0} retrieving identity {1} via ADSI path {2}. Proceeding to resolve using OS", (object) ex, (object) identity.Descriptor.Identifier, (object) entry.Path);
        this.GetIdentityBasic(identity);
        if (!identity.IsActive || ex.ErrorCode == -2147016656)
          return;
        if (!faultTolerant)
          throw;
        else
          syncErrors.Add(identity.Descriptor.Identifier, (Exception) ex);
      }
      finally
      {
        if (this.m_deactivateIdentitiesWithDifferingSid & flag)
          this.SetIdentityToNull(ref identity, "ADAccessor.GetIdentityFromDirectoryEntry.COMException");
      }
    }

    private void GetIdentityBasic(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string domain;
      string userName;
      bool isDeleted;
      bool migrated;
      SidIdentityHelper.ResolveSid(SidDescriptor.GetData(identity.Descriptor), out domain, out userName, out Microsoft.TeamFoundation.Common.Internal.NativeMethods.AccountType _, out isDeleted, out migrated);
      if (!isDeleted && !migrated)
      {
        identity.IsActive = true;
        identity.SetProperty("Domain", (object) domain);
        identity.SetProperty("Account", (object) userName);
        if (!this.m_settingUseAccountDisplayMode)
          return;
        identity.ProviderDisplayName = userName;
      }
      else
      {
        if (migrated)
          IdentityServiceBase.Trace(TraceLevel.Warning, "GetIdentityBasic treating migrated identity {0} as deleted.", (object) identity.Descriptor.Identifier);
        identity.IsActive = false;
      }
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity GetIdentityFromSearchResult(
      IVssRequestContext requestContext,
      SearchResult result,
      SyncErrors syncErrors)
    {
      if (!result.Properties.Contains("objectSid") || !result.Properties.Contains("sAMAccountName"))
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      SecurityIdentifier sid = WindowsProvider.ExtractSid(result.Properties["objectSid"][0]);
      Microsoft.VisualStudio.Services.Identity.Identity identity = WindowsProvider.CreateIdentity(IdentityHelper.CreateWindowsDescriptor(sid));
      bool flag = false;
      try
      {
        string netbiosName;
        this.m_domainProperties.GetProperties(requestContext, sid.AccountDomainSid, out netbiosName, out string _, out string _);
        identity.SetProperty("Domain", (object) netbiosName);
        string str = result.Properties["sAMAccountName"][0] as string;
        identity.SetProperty("Account", (object) str);
        identity.ProviderDisplayName = str;
        identity.IsActive = true;
        identity.SetProperty("SchemaClassName", (object) "User");
        if (result.Properties.Contains("objectClass"))
        {
          ResultPropertyValueCollection property = result.Properties["objectClass"];
          for (int index = property.Count - 1; index >= 0; --index)
          {
            string a = property[index] as string;
            if (!string.IsNullOrEmpty(a) && string.Equals(a, "group", StringComparison.OrdinalIgnoreCase))
            {
              identity.IsContainer = true;
              identity.SetProperty("SchemaClassName", (object) "Group");
              break;
            }
          }
        }
        if (identity.IsContainer && result.Properties.Contains("groupType") && ((int) result.Properties["groupType"][0] & int.MinValue) != 0)
          identity.SetProperty("SecurityGroup", (object) "SecurityGroup");
        if (result.Properties.Contains("description"))
          identity.SetProperty("Description", (object) (result.Properties["description"][0] as string));
        if (result.Properties.Contains("distinguishedName"))
          identity.SetProperty("DN", (object) (result.Properties["distinguishedName"][0] as string));
        if (!this.m_settingUseAccountDisplayMode && result.Properties.Contains("displayName"))
          identity.ProviderDisplayName = result.Properties["displayName"][0] as string;
        if (result.Properties.Contains("mail"))
          identity.SetProperty("Mail", (object) (result.Properties["mail"][0] as string));
        if (result.Properties.Contains("objectGuid"))
          identity.Id = new Guid((byte[]) result.Properties["objectGuid"][0]);
        if (this.DoesIdentityWithSameIdButDifferentDescriptorExist(requestContext, identity))
        {
          this.SetIdentityToNull(ref identity, "ADAccessor.GetIdentityFromSearchResult");
          return (Microsoft.VisualStudio.Services.Identity.Identity) null;
        }
        if (!string.IsNullOrEmpty(identity.GetProperty<string>("DN", (string) null)) && !string.IsNullOrEmpty(identity.GetProperty<string>("Domain", (string) null)))
        {
          if (!this.IsDirectoryEntryDisabled(result.GetDirectoryEntry(), "userAccountControl"))
            goto label_32;
        }
        identity.IsActive = false;
        IdentityServiceBase.Trace(TraceLevel.Warning, "Invalid identity {0} from ADAccessor.GetIdentityFromSearchResult", (object) identity.Descriptor.Identifier);
      }
      catch (COMException ex)
      {
        flag = true;
        IdentityServiceBase.Trace(TraceLevel.Warning, "ADAccessor.GetIdentityFromSearchResult - error {0} retrieving identity {1}. Proceeding to resolve using OS", (object) ex, (object) identity.Descriptor.Identifier);
        this.GetIdentityBasic(identity);
        if (identity.IsActive)
        {
          if (ex.ErrorCode != -2147016656)
            syncErrors.Add(identity.Descriptor.Identifier, (Exception) ex);
        }
      }
      finally
      {
        if (this.m_deactivateIdentitiesWithDifferingSid & flag)
          this.SetIdentityToNull(ref identity, "ADAccessor.GetIdentityFromSearchResult.COMException");
      }
label_32:
      return identity;
    }

    private int GetMembersDirect(
      IVssRequestContext requestContext,
      bool getProperties,
      DirectoryEntry groupEntry,
      string groupDomainName,
      Dictionary<string, IdentityDescriptor> members,
      IIdentitySyncHelper syncHelper,
      SyncErrors syncErrors)
    {
      int membersDirect = 0;
      using (DirectorySearcher directorySearcher = new DirectorySearcher(groupEntry))
      {
        using (DirectoryEntry directoryEntry = DirectoryEntryFactory.CreateDirectoryEntry((string) null))
        {
          directorySearcher.Filter = "(objectClass=*)";
          directorySearcher.ClientTimeout = TimeSpan.FromMinutes(10.0);
          uint num1 = 1000;
          uint num2 = 0;
          uint num3 = num2 + (num1 - 1U);
          bool flag = false;
          while (true)
          {
            do
            {
              string str1 = flag ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "member;range={0}-*", (object) num2) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "member;range={0}-{1}", (object) num2, (object) num3);
              directorySearcher.PropertiesToLoad.Clear();
              directorySearcher.ExtendedDN = ExtendedDN.Standard;
              directorySearcher.PropertiesToLoad.Add(str1);
              SearchResult one = directorySearcher.FindOne();
              if (one.Properties.Contains(str1))
              {
                foreach (object obj in (ReadOnlyCollectionBase) one.Properties[str1])
                {
                  string str2 = obj as string;
                  if (!string.IsNullOrEmpty(str2))
                  {
                    try
                    {
                      string sid;
                      string distinguishedName;
                      WindowsProvider.ParseExtendedDN(str2, out sid, out distinguishedName);
                      SecurityIdentifier securityId;
                      if (string.IsNullOrEmpty(sid))
                      {
                        directoryEntry.Path = WindowsProvider.BuildADSIPathFromDN(groupDomainName, distinguishedName);
                        this.GetContactInfo(requestContext, directoryEntry, out securityId, out distinguishedName);
                      }
                      else
                        securityId = new SecurityIdentifier(sid);
                      if (securityId != (SecurityIdentifier) null)
                      {
                        IdentityDescriptor windowsDescriptor = IdentityHelper.CreateWindowsDescriptor(securityId);
                        IdentityServiceBase.Trace(TraceLevel.Verbose, "AdAccessor.GetMembersDirect found member {0}", (object) securityId.Value);
                        if (!getProperties)
                          members[windowsDescriptor.Identifier] = windowsDescriptor;
                        else if (!syncHelper.HasIdentityBeenSeen(windowsDescriptor))
                        {
                          Microsoft.VisualStudio.Services.Identity.Identity identity = WindowsProvider.CreateIdentity(windowsDescriptor);
                          try
                          {
                            SecurityIdentifier accountDomainSid = securityId.AccountDomainSid;
                            string netbiosName;
                            string fullDomainName;
                            this.m_domainProperties.GetProperties(requestContext, accountDomainSid, out netbiosName, out fullDomainName, out string _);
                            identity.SetProperty("Domain", (object) netbiosName);
                            SecurityIdentifierInfo data = SidDescriptor.GetData(windowsDescriptor);
                            directoryEntry.Path = WindowsProvider.BuildADSIPathFromSID(fullDomainName, data.GetBinaryForm());
                            this.GetIdentityFromDirectoryEntry(requestContext, directoryEntry, ref identity, syncErrors, true);
                          }
                          catch
                          {
                            if (identity != null)
                              this.GetIdentityBasic(identity);
                            if (this.m_deactivateIdentitiesWithDifferingSid)
                              this.SetIdentityToNull(ref identity, "ADAccessor.GetMembersDirect.Exception");
                          }
                          if (identity != null)
                          {
                            if (identity.IsActive)
                            {
                              syncHelper.ProcessIdentity(identity);
                              ++membersDirect;
                            }
                          }
                        }
                      }
                    }
                    catch (Exception ex)
                    {
                      if (!string.IsNullOrEmpty(directoryEntry.Path))
                        ex.Data.Add((object) "GetMembersDirect Path", (object) directoryEntry.Path);
                      syncErrors.Add(str2, ex);
                    }
                  }
                }
                if (flag)
                  goto label_41;
              }
              else if (!flag)
                flag = true;
              else
                goto label_41;
            }
            while (flag);
            num2 = num3 + 1U;
            num3 = num2 + (num1 - 1U);
          }
        }
      }
label_41:
      return membersDirect;
    }

    private void GetContactInfo(
      IVssRequestContext requestContext,
      DirectoryEntry entry,
      out SecurityIdentifier securityId,
      out string realDN)
    {
      securityId = (SecurityIdentifier) null;
      realDN = (string) null;
      string fullDomainName = (string) null;
      try
      {
        if (!entry.SchemaClassName.Equals("contact", StringComparison.OrdinalIgnoreCase) || !entry.Properties.Contains("msDS-SourceObjectDN"))
          return;
        realDN = entry.Properties["msDS-SourceObjectDN"].Value.ToString();
        fullDomainName = this.m_domainProperties.ConstructFullDomainName(realDN, requestContext);
        entry.Path = WindowsProvider.BuildADSIPathFromDN(fullDomainName, realDN);
        if (!entry.Properties.Contains("objectSid"))
          return;
        securityId = WindowsProvider.ExtractSid(entry.Properties["objectSid"].Value);
        this.m_domainProperties.GetProperties(requestContext, securityId.AccountDomainSid, out string _, out fullDomainName, out string _);
      }
      catch (DirectoryServicesCOMException ex)
      {
        if (ex.ErrorCode == -2147016656)
          return;
        requestContext.TraceAlways(535134, TraceLevel.Warning, nameof (WindowsProvider), nameof (WindowsProvider), string.Format("GetContactInfo failed.\r\nPath: '{0}'\r\nError message: {1}\r\nExtendedError: {2}\r\nExtendedErrorMessage: {3}\r\nrealDN: {4}\r\ndnsName: {5}", (object) entry.Path, (object) ex.Message, (object) ex.ExtendedError, (object) ex.ExtendedErrorMessage, (object) (realDN ?? "<null>"), (object) (fullDomainName ?? "<null>")));
        throw;
      }
    }

    private int GetMembersPrimary(
      IVssRequestContext requestContext,
      bool getProperties,
      SecurityIdentifierInfo groupSecurityIdInfo,
      Dictionary<string, IdentityDescriptor> members,
      IIdentitySyncHelper syncHelper,
      SyncErrors syncErrors)
    {
      int membersPrimary = 0;
      uint sidRid = SidIdentityHelper.GetSidRid(groupSecurityIdInfo.GetBinaryForm());
      if (sidRid != 0U)
      {
        SecurityIdentifier accountDomainSid = groupSecurityIdInfo.SecurityId.AccountDomainSid;
        string domainRootPath;
        this.m_domainProperties.GetProperties(requestContext, accountDomainSid, out string _, out string _, out domainRootPath);
        using (DirectoryEntry directoryEntry = DirectoryEntryFactory.CreateDirectoryEntry(domainRootPath))
        {
          using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry))
          {
            directorySearcher.SearchScope = SearchScope.Subtree;
            directorySearcher.Filter = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(PrimaryGroupID={0})", (object) sidRid);
            directorySearcher.ClientTimeout = TimeSpan.FromMinutes(10.0);
            directorySearcher.PageSize = 200;
            directorySearcher.PropertiesToLoad.Clear();
            if (getProperties)
            {
              directorySearcher.PropertiesToLoad.AddRange(WindowsProvider.s_identityAttributes);
            }
            else
            {
              directorySearcher.PropertiesToLoad.Add("distinguishedName");
              directorySearcher.PropertiesToLoad.Add("objectSid");
            }
            using (SearchResultCollection all = directorySearcher.FindAll())
            {
              foreach (SearchResult result in all)
              {
                if (getProperties)
                {
                  Microsoft.VisualStudio.Services.Identity.Identity fromSearchResult = this.GetIdentityFromSearchResult(requestContext, result, syncErrors);
                  if (fromSearchResult != null && fromSearchResult.IsActive)
                  {
                    IdentityServiceBase.Trace(TraceLevel.Verbose, "AdAccessor.GetMembersPrimary has found member {0}", (object) fromSearchResult.DisplayName);
                    syncHelper.ProcessIdentity(fromSearchResult);
                    ++membersPrimary;
                  }
                }
                else if (result.Properties.Contains("distinguishedName") && result.Properties.Contains("objectSid"))
                {
                  string str = (string) result.Properties["distinguishedName"][0];
                  SecurityIdentifier sid = WindowsProvider.ExtractSid(result.Properties["objectSid"][0]);
                  IdentityServiceBase.Trace(TraceLevel.Verbose, "AdAccessor.GetMembersPrimary has found member {0}, {1}", (object) str, (object) sid.Value);
                  members[sid.Value] = IdentityHelper.CreateWindowsDescriptor(sid);
                }
              }
            }
          }
        }
      }
      return membersPrimary;
    }

    private static string BuildADSIPathFromDN(string dnsDomainName, string distinguishedName)
    {
      if (distinguishedName == null)
        throw new ArgumentNullException(nameof (distinguishedName));
      return string.IsNullOrEmpty(dnsDomainName) ? "LDAP://" + distinguishedName.Replace("/", "\\/") : "LDAP://" + dnsDomainName + "/" + distinguishedName.Replace("/", "\\/");
    }

    internal static string BuildADSIPathFromSID(string dnsDomainName, byte[] binarySid)
    {
      if (binarySid == null)
        throw new ArgumentNullException(nameof (binarySid));
      return "LDAP://" + dnsDomainName + "/<SID=" + HexConverter.ToString(binarySid) + ">";
    }

    internal static void ParseExtendedDN(string extendedDN, out string sid, out string dn)
    {
      sid = (string) null;
      dn = (string) null;
      string str1 = extendedDN;
      char[] separator = new char[1]{ ';' };
      foreach (string str2 in str1.Split(separator, 3))
      {
        if (!str2.StartsWith("<GUID=", StringComparison.OrdinalIgnoreCase))
        {
          if (str2.StartsWith("<SID=", StringComparison.OrdinalIgnoreCase))
            sid = str2.Substring(5, str2.Length - 6);
          else
            dn = str2;
        }
      }
    }

    private void SyncSAMIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool includeMembers,
      SyncErrors syncErrors)
    {
      SecurityIdentifier securityId = SidDescriptor.GetData(identity.Descriptor).SecurityId;
      string property1 = identity.GetProperty<string>("Account", string.Empty);
      string property2 = identity.GetProperty<string>("Domain", string.Empty);
      using (DirectoryEntry directoryEntry1 = DirectoryEntryFactory.CreateDirectoryEntry((string) null))
      {
        directoryEntry1.AuthenticationType |= AuthenticationTypes.ReadonlyServer;
        string groupOrUser;
        if (!string.IsNullOrEmpty(property2))
        {
          if (!string.Equals(property2, "BuiltIn", StringComparison.OrdinalIgnoreCase))
          {
            try
            {
              groupOrUser = WindowsProvider.GetGroupOrUser(directoryEntry1, "WinNT://" + property2 + "/" + property1);
              goto label_6;
            }
            catch
            {
              groupOrUser = WindowsProvider.GetGroupOrUser(directoryEntry1, "WinNT://" + "localhost/" + property1);
              goto label_6;
            }
          }
        }
        groupOrUser = WindowsProvider.GetGroupOrUser(directoryEntry1, "WinNT://" + "localhost/" + property1);
label_6:
        if (string.Equals(groupOrUser, "Group", StringComparison.OrdinalIgnoreCase))
        {
          identity.IsContainer = true;
          identity.SetProperty("SchemaClassName", (object) "Group");
          identity.SetProperty("SecurityGroup", (object) "SecurityGroup");
        }
        else
          identity.SetProperty("SchemaClassName", (object) "User");
        if (!(directoryEntry1.Properties["name"].Value is string str2))
        {
          IdentityServiceBase.Trace(TraceLevel.Warning, "Invalid identity {0} (accountName null) from SAMAccessor.SyncSAMIdentity", (object) identity.Descriptor.Identifier);
          identity.IsActive = false;
        }
        else
        {
          identity.SetProperty("Account", (object) str2);
          identity.ProviderDisplayName = str2;
          SecurityIdentifier sid1 = WindowsProvider.ExtractSid(directoryEntry1.Properties["objectSid"].Value);
          if (sid1 == (SecurityIdentifier) null || !securityId.Equals(sid1))
          {
            IdentityServiceBase.Trace(TraceLevel.Warning, "Invalid identity {0} (SID null or mismatched) from SAMAccessor.SyncSAMIdentity", (object) identity.Descriptor.Identifier);
            identity.IsActive = false;
          }
          else if (this.IsDirectoryEntryDisabled(directoryEntry1, "UserFlags"))
          {
            identity.IsActive = false;
          }
          else
          {
            if (!this.m_settingUseAccountDisplayMode && directoryEntry1.Properties.Contains("fullName"))
            {
              string originalDisplayName = directoryEntry1.Properties["fullName"].Value as string;
              if (originalDisplayName.Length != 0)
              {
                string str = IdentityHelper.CleanProviderDisplayName(originalDisplayName, identity.Descriptor);
                if (!string.IsNullOrWhiteSpace(str))
                  identity.ProviderDisplayName = str;
              }
            }
            if (directoryEntry1.Properties.Contains("description"))
              identity.SetProperty("Description", (object) (directoryEntry1.Properties["description"].Value as string));
            string str1 = string.IsNullOrEmpty(property2) || !property2.Equals(WindowsProvider.s_localMachineName, StringComparison.OrdinalIgnoreCase) ? "WinNT://" + str2 : "WinNT://" + property2 + "/" + str2;
            identity.SetProperty("DN", (object) str1);
            if (!includeMembers || !identity.IsContainer)
              return;
            Dictionary<string, IdentityDescriptor> dictionary = new Dictionary<string, IdentityDescriptor>();
            foreach (object adsObject in directoryEntry1.Invoke("members", (object[]) null) as IEnumerable)
            {
              using (DirectoryEntry directoryEntry2 = DirectoryEntryFactory.CreateDirectoryEntry(adsObject))
              {
                if (directoryEntry2.Properties.Contains("objectSid"))
                {
                  SecurityIdentifier sid2 = WindowsProvider.ExtractSid(directoryEntry2.Properties["objectSid"].Value);
                  if (sid2 != (SecurityIdentifier) null)
                    dictionary[sid2.Value] = IdentityHelper.CreateWindowsDescriptor(sid2);
                }
              }
            }
            identity.Members = (ICollection<IdentityDescriptor>) dictionary.Values;
          }
        }
      }
    }

    private static string GetGroupOrUser(DirectoryEntry de, string path)
    {
      try
      {
        de.Path = path + ",user";
        return de.SchemaClassName;
      }
      catch (COMException ex)
      {
        de.Path = path + ",group";
        return de.SchemaClassName;
      }
    }

    private void SyncSAMGroup(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity,
      IIdentitySyncHelper syncHelper,
      IDictionary<string, IIdentityProvider> syncAgents,
      SyncErrors syncErrors)
    {
      this.SyncSAMIdentity(requestContext, groupIdentity, true, syncErrors);
      if (groupIdentity.Members == null)
        return;
      foreach (IdentityDescriptor member in (IEnumerable<IdentityDescriptor>) groupIdentity.Members)
      {
        try
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (this.TrySyncIdentity(requestContext, member, false, (string) null, syncErrors, out identity))
          {
            if (identity != null)
            {
              if (identity.IsActive)
                syncHelper.ProcessIdentity(identity);
            }
          }
        }
        catch (Exception ex)
        {
          IdentityServiceBase.Trace(TraceLevel.Info, "Failed to sync identity {0}", (object) member.Identifier);
          syncErrors.Add(member.Identifier, ex);
          syncHelper.PreserveMember(member);
        }
      }
    }

    internal WindowsProvider()
    {
      this.m_domainProperties = new DomainProperties();
      this.m_settingUseGlobalCatalog = true;
      this.m_settingUseAccountDisplayMode = false;
      this.m_domainProperties.UseRFC2247 = true;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.GetService<CachedRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), "/Service/Integration/Settings/*");
      this.OnSettingsChanged(requestContext, (RegistryEntryCollection) null);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<CachedRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<CachedRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/Integration/Settings/*");
      this.m_domainProperties.UseRFC2247 = registryEntryCollection.GetValueFromPath<bool>("/Service/Integration/Settings" + "/useRFC2247", true);
      this.m_settingUseGlobalCatalog = registryEntryCollection.GetValueFromPath<bool>("/Service/Integration/Settings" + "/useADMemberQuery", true);
      this.m_settingUseAccountDisplayMode = registryEntryCollection.GetValueFromPath<bool>("/Service/Integration/Settings" + "/useAccountDisplayMode", false);
      this.m_deactivateMigratedIdentities = registryEntryCollection.GetValueFromPath<bool>("/Service/Integration/Settings" + "/deactivateMigratedIdentities", false);
      this.m_deactivateIdentitiesWithDifferingSid = registryEntryCollection.GetValueFromPath<bool>("/Service/Integration/Settings" + "/deactivateIdentitiesWithDifferingSid", false);
    }

    public string[] SupportedIdentityTypes() => WindowsProvider.s_supportedTypes;

    public virtual IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      return !(identity is WindowsIdentity windowsIdentity) ? (IdentityDescriptor) null : IdentityHelper.CreateWindowsDescriptor(windowsIdentity.User);
    }

    public virtual IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      string displayName)
    {
      try
      {
        return IdentityHelper.CreateWindowsDescriptor((SecurityIdentifier) new NTAccount(displayName).Translate(typeof (SecurityIdentifier)));
      }
      catch (Exception ex)
      {
        IdentityServiceBase.Trace(TraceLevel.Info, ex.Message);
      }
      return (IdentityDescriptor) null;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      IdentityDescriptor descriptor = this.CreateDescriptor(requestContext, identity);
      Microsoft.VisualStudio.Services.Identity.Identity identity1;
      this.TrySyncIdentity(requestContext, descriptor, false, (string) null, new SyncErrors(), out identity1);
      return identity1;
    }

    public virtual bool TrySyncIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool includeMembership,
      string providerInfo,
      SyncErrors syncErrors,
      out Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IdentityServiceBase.Trace(TraceLevel.Info, "SyncIdentity: Enter");
      WindowsProvider.AccountSubType subType;
      bool migrated;
      identity = this.ResolveIdentity(descriptor, providerInfo, out subType, out migrated);
      try
      {
        switch (subType)
        {
          case WindowsProvider.AccountSubType.SAM:
            this.SyncSAMIdentity(requestContext, identity, includeMembership, syncErrors);
            break;
          case WindowsProvider.AccountSubType.AD:
            this.SyncADIdentity(requestContext, ref identity, includeMembership, syncErrors);
            break;
        }
      }
      catch (Exception ex)
      {
        if (includeMembership | migrated || this.m_deactivateIdentitiesWithDifferingSid && subType == WindowsProvider.AccountSubType.AD)
          throw;
        else
          syncErrors.Add(descriptor.Identifier, ex);
      }
      IdentityServiceBase.Trace(TraceLevel.Info, "SyncIdentity: Exit");
      return this.IsSyncable;
    }

    public bool IsSyncable => true;

    public void SyncMembers(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      IIdentitySyncHelper syncHelper,
      IDictionary<string, IIdentityProvider> syncAgents,
      string providerInfo,
      SyncErrors syncErrors)
    {
      WindowsProvider.AccountSubType subType;
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.ResolveIdentity(descriptor, providerInfo, out subType, out bool _);
      if (subType == WindowsProvider.AccountSubType.SAM)
      {
        this.SyncSAMGroup(requestContext, identity, syncHelper, syncAgents, syncErrors);
      }
      else
      {
        if (subType != WindowsProvider.AccountSubType.AD)
          return;
        this.SyncADGroup(requestContext, identity, syncHelper, syncAgents, syncErrors);
      }
    }

    public IEnumerable<string> AvailableIdentityAttributes => Enumerable.Empty<string>();

    private Microsoft.VisualStudio.Services.Identity.Identity ResolveIdentity(
      IdentityDescriptor descriptor,
      string providerInfo,
      out WindowsProvider.AccountSubType subType,
      out bool migrated)
    {
      subType = WindowsProvider.AccountSubType.Unknown;
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      SecurityIdentifierInfo securityIdInfo = string.Equals("System.Security.Principal.WindowsIdentity", descriptor.IdentityType, StringComparison.Ordinal) ? SidDescriptor.GetData(descriptor) : throw new ArgumentException("descriptor.IdentityType");
      string domain;
      string userName;
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.AccountType type;
      bool isDeleted;
      SidIdentityHelper.ResolveSid(securityIdInfo, out domain, out userName, out type, out isDeleted, out migrated);
      if (migrated && this.m_deactivateMigratedIdentities)
      {
        IdentityServiceBase.Trace(TraceLevel.Warning, "Legacy behavior: Treating migrated identity {0} as deleted regardless of existence of source identity.", (object) descriptor.Identifier);
        isDeleted = true;
      }
      if (isDeleted)
      {
        if (providerInfo != null && providerInfo.StartsWith("WinNT://", StringComparison.OrdinalIgnoreCase) && !providerInfo.Substring("WinNT://".Length).StartsWith(WindowsProvider.s_localMachineName, StringComparison.OrdinalIgnoreCase))
          throw new IdentityProviderUnavailableException(descriptor);
      }
      else if (migrated)
      {
        if (!SidIdentityHelper.IsNTAccount(securityIdInfo.GetBinaryForm()))
          throw new NotSupportedException();
        identity = WindowsProvider.CreateIdentity(descriptor);
        subType = WindowsProvider.AccountSubType.AD;
      }
      else
      {
        identity = WindowsProvider.CreateIdentity(descriptor);
        identity.SetProperty("Domain", (object) domain);
        identity.SetProperty("Account", (object) userName);
        if (this.m_settingUseAccountDisplayMode)
          identity.ProviderDisplayName = userName;
        subType = !string.IsNullOrEmpty(domain) ? (string.Equals(domain, WindowsProvider.s_localMachineName, StringComparison.OrdinalIgnoreCase) || SidIdentityHelper.IsBuiltInAccount(securityIdInfo.GetBinaryForm()) ? WindowsProvider.AccountSubType.SAM : (!SidIdentityHelper.IsNTAccount(securityIdInfo.GetBinaryForm()) ? WindowsProvider.AccountSubType.Wellknown : WindowsProvider.AccountSubType.AD)) : WindowsProvider.AccountSubType.Wellknown;
        if (subType == WindowsProvider.AccountSubType.Wellknown)
        {
          identity.ProviderDisplayName = userName;
          identity.SetProperty("SchemaClassName", (object) "User");
          if ((type == Microsoft.TeamFoundation.Common.Internal.NativeMethods.AccountType.SidTypeGroup || type == Microsoft.TeamFoundation.Common.Internal.NativeMethods.AccountType.SidTypeWellKnownGroup) && !SidIdentityHelper.IsServiceAccount(securityIdInfo.GetBinaryForm()))
          {
            identity.IsContainer = true;
            identity.SetProperty("SchemaClassName", (object) "Group");
            identity.SetProperty("SecurityGroup", (object) "SecurityGroup");
          }
        }
        IdentityServiceBase.Trace(TraceLevel.Info, "Windows provider resolved {0} to {1}", (object) descriptor.Identifier, (object) userName);
      }
      return identity;
    }

    internal bool IsDirectoryEntryDisabled(DirectoryEntry directoryEntry, string propertyName)
    {
      if (directoryEntry.NativeGuid == null)
        return true;
      return directoryEntry.Properties[propertyName]?.Value != null && ((UserAccountControl) directoryEntry.Properties[propertyName].Value & UserAccountControl.ACCOUNTDISABLE) != 0;
    }

    private bool DoesIdentityWithSameIdButDifferentDescriptorExist(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!this.m_deactivateIdentitiesWithDifferingSid || identity == null || Guid.Empty.Equals(identity.Id) || identity.Descriptor == (IdentityDescriptor) null)
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) new List<Guid>()
      {
        identity.Id
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      return readIdentity != null && !IdentityDescriptorComparer.Instance.Equals(readIdentity.Descriptor, identity.Descriptor);
    }

    private void SetIdentityToNull(ref Microsoft.VisualStudio.Services.Identity.Identity identity, string methodName)
    {
      if (identity == null)
        return;
      IdentityServiceBase.Trace(TraceLevel.Warning, "{0} treating identity {1} as migrated and null.", (object) methodName, (object) identity.Descriptor.Identifier);
      identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity CreateIdentity(
      IdentityDescriptor descriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity.Descriptor = descriptor;
      identity.ProviderDisplayName = string.Empty;
      identity.IsActive = true;
      identity.UniqueUserId = 0;
      identity.IsContainer = false;
      identity.Members = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      identity.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      return identity;
    }

    private static SecurityIdentifier ExtractSid(object objectSid)
    {
      SecurityIdentifier sid = (SecurityIdentifier) null;
      if (objectSid is byte[] binaryForm)
        sid = new SecurityIdentifier(binaryForm, 0);
      return sid;
    }

    [Flags]
    internal enum ADS_GROUP_TYPE_ENUM : uint
    {
      ADS_GROUP_TYPE_GLOBAL_GROUP = 2,
      ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = 4,
      ADS_GROUP_TYPE_LOCAL_GROUP = ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP, // 0x00000004
      ADS_GROUP_TYPE_UNIVERSAL_GROUP = 8,
      ADS_GROUP_TYPE_SECURITY_ENABLED = 2147483648, // 0x80000000
    }

    private enum AccountSubType
    {
      Unknown,
      Wellknown,
      SAM,
      AD,
    }
  }
}
