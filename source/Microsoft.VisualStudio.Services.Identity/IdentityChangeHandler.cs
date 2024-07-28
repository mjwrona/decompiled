// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityChangeHandler
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityChangeHandler
  {
    private readonly PlatformIdentityStore m_identityStore;

    internal IdentityChangeHandler(PlatformIdentityStore identityStore) => this.m_identityStore = identityStore;

    public IEnumerable<IdentityChangeInfo> List(IVssRequestContext requestContext, string account)
    {
      System.Collections.Generic.List<IdentityChangeInfo> identityChangeInfoList = new System.Collections.Generic.List<IdentityChangeInfo>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in this.m_identityStore.ReadIdentitiesFromDatabase(requestContext, this.m_identityStore.Domain, IdentitySearchFilter.AccountName, string.Empty, (string) null, account, 0, QueryMembership.None))
      {
        if (identity != null)
        {
          IdentityChangeInfo identityChangeInfo = this.Analyze(identity, (string) null, (string) null);
          if (identityChangeInfo != null)
            identityChangeInfoList.Add(identityChangeInfo);
        }
      }
      return (IEnumerable<IdentityChangeInfo>) identityChangeInfoList;
    }

    public IEnumerable<IdentityChangeInfo> Change(
      IVssRequestContext requestContext,
      string sourceDomain,
      string targetDomain,
      string sourceAccount,
      string targetAccount)
    {
      if (string.IsNullOrEmpty(sourceDomain))
        throw new ArgumentNullException(nameof (sourceDomain));
      if (string.IsNullOrEmpty(targetDomain))
        throw new ArgumentNullException(nameof (targetDomain));
      Dictionary<Guid, IdentityChangeInfo> dictionary = new Dictionary<Guid, IdentityChangeInfo>();
      System.Collections.Generic.List<Microsoft.VisualStudio.Services.Identity.Identity> identities = new System.Collections.Generic.List<Microsoft.VisualStudio.Services.Identity.Identity>();
      System.Collections.Generic.List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.m_identityStore.ReadIdentitiesFromDatabase(requestContext, this.m_identityStore.Domain, IdentitySearchFilter.AccountName, string.Empty, sourceDomain, sourceAccount, 0, QueryMembership.None);
      if (sourceAccount != null && targetAccount == null)
        targetAccount = sourceAccount;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identityList)
      {
        if (identity != null)
        {
          IdentityChangeInfo identityChangeInfo = this.Analyze(identity, targetDomain, targetAccount);
          if (identityChangeInfo != null)
            dictionary.Add(identityChangeInfo.Identity.MasterId, identityChangeInfo);
        }
      }
      HashSet<string> propertiesToUpdate = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      propertiesToUpdate.Add("Descriptor");
      foreach (IdentityChangeInfo identityChangeInfo in dictionary.Values)
      {
        if (identityChangeInfo.TargetExists && !identityChangeInfo.Matched)
        {
          IdentityHelper.CreateWindowsDescriptor(identityChangeInfo.OldSid);
          IdentityDescriptor windowsDescriptor = IdentityHelper.CreateWindowsDescriptor(identityChangeInfo.NewSid);
          identityChangeInfo.Identity.Descriptor = windowsDescriptor;
          if (targetDomain != null)
          {
            identityChangeInfo.Identity.SetProperty("Domain", (object) targetDomain);
            propertiesToUpdate.Add("IdentityAttributeTags.Domain");
          }
          if (targetAccount != null)
          {
            if (dictionary.Values.Count > 1)
              throw new InvalidOperationException();
            identityChangeInfo.Identity.SetProperty("Account", (object) targetAccount);
            propertiesToUpdate.Add("IdentityAttributeTags.AccountName");
          }
          identities.Add(identityChangeInfo.Identity);
        }
      }
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in this.m_identityStore.UpdateIdentitiesInDatabase(requestContext, this.m_identityStore.Domain, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities, false, propertiesToUpdate))
      {
        IdentityChangeInfo identityChangeInfo;
        if (dictionary.TryGetValue(identity.MasterId, out identityChangeInfo))
          identityChangeInfo.Changed = true;
      }
      return (IEnumerable<IdentityChangeInfo>) dictionary.Values;
    }

    public bool ReplaceDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor oldDescriptor,
      IdentityDescriptor newDescriptor)
    {
      PlatformIdentityStore identityStore = this.m_identityStore;
      IVssRequestContext requestContext1 = requestContext;
      IdentityDomain domain = this.m_identityStore.Domain;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in identityStore.ReadIdentities(requestContext1, domain, (IList<IdentityDescriptor>) new System.Collections.Generic.List<IdentityDescriptor>()
      {
        oldDescriptor
      }, QueryMembership.None, false, (IEnumerable<string>) null))
      {
        if (readIdentity != null)
        {
          readIdentity.Descriptor = newDescriptor;
          this.m_identityStore.UpdateIdentitiesInDatabase(requestContext, this.m_identityStore.Domain, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            readIdentity
          }, false, new HashSet<string>() { "Descriptor" });
          return true;
        }
      }
      return false;
    }

    private IdentityChangeInfo Analyze(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string targetDomain,
      string targetAccount)
    {
      IdentityChangeInfo identityChangeInfo = (IdentityChangeInfo) null;
      if (string.Equals(identity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
      {
        if (targetDomain == null)
          targetDomain = identity.GetProperty<string>("Domain", string.Empty);
        if (targetAccount == null)
          targetAccount = identity.GetProperty<string>("Account", (string) null);
        NTAccount ntAccount = new NTAccount(targetDomain, targetAccount);
        identityChangeInfo = new IdentityChangeInfo(ntAccount.ToString(), identity.Descriptor.Identifier);
        identityChangeInfo.Identity = identity;
        try
        {
          SecurityIdentifier securityIdentifier = (SecurityIdentifier) ntAccount.Translate(typeof (SecurityIdentifier));
          identityChangeInfo.NewSid = securityIdentifier.ToString();
          identityChangeInfo.TargetExists = true;
          identityChangeInfo.Matched = string.Equals(identityChangeInfo.OldSid, identityChangeInfo.NewSid, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
          identityChangeInfo.TargetExists = false;
          identityChangeInfo.Matched = false;
        }
      }
      return identityChangeInfo;
    }
  }
}
