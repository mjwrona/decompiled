// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.DomainEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class DomainEntry : DomainEntryBase
  {
    private DirectoryEntry m_cachedEntry;
    private const int CacheRefreshSeconds = 300;
    private static readonly string[] s_cacheRefreshAttributes = new string[1]
    {
      "distinguishedName"
    };

    internal DomainEntry(IVssRequestContext requestContext, SecurityIdentifier domainSecurityId)
      : base(requestContext, domainSecurityId)
    {
    }

    protected override void Initialize(IVssRequestContext requestContext)
    {
      byte[] numArray = new byte[this.DomainSecurityId.BinaryLength];
      this.DomainSecurityId.GetBinaryForm(numArray, 0);
      string domainName;
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.SidToName(numArray, out domainName, out Microsoft.TeamFoundation.Common.Internal.NativeMethods.AccountType _);
      this.NetbiosName = domainName;
      try
      {
        this.Initialize(new DirectoryContext(DirectoryContextType.Domain, domainName));
      }
      catch (ActiveDirectoryObjectNotFoundException ex1)
      {
        IdentityServiceBase.Trace(TraceLevel.Warning, "Could not find Domain object using NetBios name {0} - {1}.", (object) domainName, (object) ex1.Message);
        try
        {
          this.DnsDomainName = DomainEntry.CrackFullDomainName(this.DomainSecurityId.ToString(), domainName);
        }
        catch (Exception ex2)
        {
          IdentityServiceBase.Trace(TraceLevel.Warning, "Could not find FQDN using DsCrackNames. {0}.", (object) ex2.Message);
        }
        if (string.IsNullOrEmpty(this.DnsDomainName))
          this.DnsDomainName = requestContext.GetService<CachedRegistryService>().GetValue(requestContext, (RegistryQuery) ("/Service/Integration/Settings" + "DnsDomainName-" + domainName), string.Empty);
        if (string.IsNullOrEmpty(this.DnsDomainName))
        {
          throw;
        }
        else
        {
          IdentityServiceBase.Trace(TraceLevel.Verbose, "DomainProperties resolved domain {0} to {1}", (object) domainName, (object) this.DnsDomainName);
          this.Initialize(new DirectoryContext(DirectoryContextType.Domain, this.DnsDomainName));
        }
      }
    }

    protected override void RefreshCache(long cacheRefreshTimeStampTicks)
    {
      DirectoryEntry cachedEntry = this.m_cachedEntry;
      this.m_cachedEntry = DirectoryEntryFactory.CreateDirectoryEntry(this.RootObjectPath);
      this.m_cachedEntry.RefreshCache(DomainEntry.s_cacheRefreshAttributes);
      this.CacheRefreshTimeStampTicks = cacheRefreshTimeStampTicks;
      cachedEntry?.Dispose();
    }

    private void Initialize(DirectoryContext context)
    {
      Domain domain = (Domain) null;
      try
      {
        domain = Domain.GetDomain(context);
        this.DnsDomainName = domain.Name;
        this.RefreshCache(DateTime.UtcNow.Ticks);
        using (DirectoryEntry directoryEntry = domain.GetDirectoryEntry())
          this.DomainRootPath = directoryEntry.Path;
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "Domain name", (object) context.Name);
        throw;
      }
      finally
      {
        domain?.Dispose();
      }
    }

    private string RootObjectPath => "LDAP://" + this.DnsDomainName + "/RootDSE";

    private static string CrackFullDomainName(string domainSid, string netbiosName)
    {
      string str = string.Empty;
      IntPtr phDS = IntPtr.Zero;
      try
      {
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsBind((string) null, (string) null, out phDS) != 0U)
          throw new Exception("Bind failed");
        string[] names1 = new string[1]{ domainSid };
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_RESULT_ITEM[] dsNameResultItemArray;
        do
        {
          dsNameResultItemArray = Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsCrackNames(phDS, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FLAGS.DS_NAME_FLAG_TRUST_REFERRAL, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FORMAT.DS_SID_OR_SID_HISTORY_NAME, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FORMAT.DS_CANONICAL_NAME, names1);
          if (dsNameResultItemArray[0].status == Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_ERROR.DS_NAME_ERROR_DOMAIN_ONLY || dsNameResultItemArray[0].status == Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_ERROR.DS_NAME_ERROR_TRUST_REFERRAL)
          {
            int num = (int) Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsUnBind(ref phDS);
          }
          else
            goto label_7;
        }
        while (Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsBind((string) null, dsNameResultItemArray[0].pDomain, out phDS) == 0U);
        throw new Exception("Bind failed");
label_7:
        if (dsNameResultItemArray[0].status == Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_ERROR.DS_NAME_ERROR_NOT_FOUND && !string.IsNullOrEmpty(netbiosName))
        {
          string[] names2 = new string[1]
          {
            netbiosName + "\\"
          };
          dsNameResultItemArray = Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsCrackNames(phDS, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FLAGS.DS_NAME_FLAG_TRUST_REFERRAL, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FORMAT.DS_NT4_ACCOUNT_NAME, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FORMAT.DS_CANONICAL_NAME, names2);
        }
        if (dsNameResultItemArray[0].status == Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_ERROR.DS_NAME_NO_ERROR)
        {
          string pName = dsNameResultItemArray[0].pName;
          str = pName.Substring(0, pName.Length - 1);
        }
      }
      finally
      {
        if (phDS != IntPtr.Zero)
        {
          int num = (int) Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsUnBind(ref phDS);
        }
      }
      return str;
    }
  }
}
