// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ClaimsProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class ClaimsProvider : NonSyncableIdentityProvider
  {
    protected string IdentityType;
    private static readonly HashSet<string> CoreClaims = new HashSet<string>((IEnumerable<string>) new string[6]
    {
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
      "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly string[] ExtendedClaims = new string[1]
    {
      "ConsumerPUID"
    };
    private static readonly string[] supportedIdentityTypes = new string[2]
    {
      "Microsoft.IdentityModel.Claims.ClaimsIdentity",
      "System.Security.Claims.ClaimsIdentity"
    };
    internal const string Area = "Identity";
    internal const string Layer = "ClaimsProvider";
    internal const string IdentityProviderClaimType = "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider";
    internal const string AccessControlServiceProviderName = "LOCAL AUTHORITY";
    internal const string DefaultClaimsProviderName = "ClaimsProvider";
    internal const string ServiceRoleClaim = "Service";
    internal const string BypassTarpittingClaim = "https://schemas.microsoft.com/teamfoundationserver/2010/12/claims/bypasstarpitting";
    internal const string AuthorizationIdClaim = "https://schemas.microsoft.com/teamfoundationserver/2010/12/claims/authorizationid";
    internal const string AuthorizationIdClaimFeature = "VisualStudio.Services.Identity.AuthorizationIdClaim";
    private static readonly VssPerformanceCounter s_GitHubSignInCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.GitHubIdentities.SignInCounter");

    public ClaimsProvider() => this.IdentityType = "Microsoft.IdentityModel.Claims.ClaimsIdentity";

    protected override string[] SupportedIdentityTypes() => ClaimsProvider.supportedIdentityTypes;

    protected override IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      string identifier = (string) null;
      ClaimsIdentity claimsIdentity = (ClaimsIdentity) identity;
      string identityType = this.IdentityType;
      Claim first = claimsIdentity.FindFirst((Predicate<Claim>) (x => x.Type == "IdentityTypeClaim"));
      if (!string.IsNullOrEmpty(first?.Value))
      {
        identityType = first.Value;
        claimsIdentity.RemoveClaim(first);
      }
      foreach (Claim claim in claimsIdentity.Claims)
      {
        if (claim.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))
          identifier = claim.Value;
      }
      return new IdentityDescriptor(identityType, identifier);
    }

    internal static ClaimsIdentity GetIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity, string authenticationType)
    {
      string property1 = identity.GetProperty<string>("Mail", string.Empty);
      string property2 = identity.GetProperty<string>("Domain", nameof (ClaimsProvider));
      string property3 = identity.GetProperty<string>("Account", identity.GetProperty<string>("Mail", identity.Descriptor.Identifier));
      Guid property4 = identity.GetProperty<Guid>("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid", Guid.Empty);
      ClaimsIdentity identity1 = new ClaimsIdentity((IEnumerable<Claim>) new Claim[4]
      {
        new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", property1),
        new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", property3),
        new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", identity.Descriptor.Identifier),
        new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", property2)
      }, authenticationType);
      if (property4 != Guid.Empty)
        identity1.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid", property4.ToString()));
      string str1 = identity.ProviderDisplayName ?? string.Empty;
      string str2 = string.Empty;
      int length = str1.IndexOf(' ');
      if (length >= 0)
      {
        str2 = str1.Substring(length + 1);
        str1 = str1.Substring(0, length);
      }
      if (str1.Length > 0 || str2.Length > 0)
      {
        identity1.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", str1));
        identity1.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", str2));
      }
      if (identity.IsMsaIdentity())
      {
        string property5 = identity.GetProperty<string>("PUID", (string) null);
        if (property5 != null)
          identity1.AddClaim(new Claim("PUID", property5));
      }
      else if (identity.IsExternalUser)
      {
        string property6 = identity.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", (string) null);
        if (property6 != null)
          identity1.AddClaim(new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", property6));
        if (identity.Descriptor.IsAadServicePrincipalType())
        {
          Guid property7 = identity.GetProperty<Guid>("ApplicationId", Guid.Empty);
          identity1.AddClaim(new Claim("IdentityTypeClaim", identity.Descriptor.IdentityType));
          identity1.AddClaim(new Claim("appid", property7.ToString()));
          identity1.AddClaim(new Claim("http://schemas.microsoft.com/identity/claims/tenantid", property2));
          identity1.AddClaim(new Claim("spmetatype", identity.MetaTypeId.ToString()));
          identity1.TryRemoveClaim(identity1.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"));
        }
      }
      if (identity.IsCspPartnerUser)
      {
        identity1.AddClaim(new Claim("CspPartner", identity.MetaTypeId.ToString()));
        identity1.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid", identity.MasterId.ToString()));
      }
      if (identity.IsServiceIdentity)
      {
        identity1.AddClaim(new Claim("IdentityTypeClaim", identity.Descriptor.IdentityType));
        identity1.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid", identity.MasterId.ToString()));
      }
      return identity1;
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      IdentityDescriptor descriptor = this.CreateDescriptor(requestContext, identity);
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ClaimsIdentity sourceIdentity = (ClaimsIdentity) identity;
      foreach (Claim claim in sourceIdentity.Claims)
        dictionary[claim.Type] = claim.Value;
      string valueOrDefault1 = dictionary.GetValueOrDefault<string, string>("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", nameof (ClaimsProvider));
      string valueOrDefault2 = dictionary.GetValueOrDefault<string, string>("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", string.Empty);
      string str = IdentityHelper.CleanProviderDisplayName(this.GetIdentityDisplayName(sourceIdentity, dictionary), descriptor);
      string stringVar1 = valueOrDefault1;
      string stringVar2 = string.Equals(valueOrDefault1, "LOCAL AUTHORITY", StringComparison.Ordinal) ? str : this.GetIdentityAccountName(sourceIdentity, descriptor, (IReadOnlyDictionary<string, string>) dictionary);
      ArgumentUtility.CheckStringForNullOrEmpty(stringVar1, "domain");
      ArgumentUtility.CheckStringForNullOrEmpty(stringVar2, "accountName");
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity1.Descriptor = descriptor;
      identity1.ProviderDisplayName = str;
      identity1.IsActive = true;
      identity1.UniqueUserId = 0;
      identity1.IsContainer = false;
      identity1.Members = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      identity1.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
      identity2.SetProperty("Domain", (object) stringVar1);
      identity2.SetProperty("Account", (object) stringVar2);
      if (!string.IsNullOrEmpty(valueOrDefault2))
        identity2.SetProperty("Mail", (object) valueOrDefault2);
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        if (!ClaimsProvider.CoreClaims.Contains(keyValuePair.Key))
          identity2.SetProperty(keyValuePair.Key, (object) keyValuePair.Value);
      }
      Guid? cuid;
      SubjectDescriptor subjectDescriptor;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSubjectDescriptorForDIMS") && this.TryCreateSubjectDescriptor(requestContext, identity, out cuid, out subjectDescriptor))
      {
        identity2.SetProperty("CUID", (object) cuid);
        identity2.SubjectDescriptor = subjectDescriptor;
      }
      this.PopulateGitHubIdentityData(requestContext, identity, identity2);
      return identity2;
    }

    private bool TryCreateSubjectDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity,
      out Guid? cuid,
      out SubjectDescriptor subjectDescriptor)
    {
      cuid = new Guid?();
      subjectDescriptor = new SubjectDescriptor();
      ClaimsIdentity claimsIdentity = (ClaimsIdentity) identity;
      Claim domainClaim = claimsIdentity.FindFirst((Predicate<Claim>) (x => x.Type == "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider"));
      Claim puidClaim = claimsIdentity.FindFirst((Predicate<Claim>) (x => x.Type == "PUID"));
      Claim oidClaim = claimsIdentity.FindFirst((Predicate<Claim>) (x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"));
      Claim cspClaim = claimsIdentity.FindFirst((Predicate<Claim>) (x => x.Type == "CspPartner"));
      requestContext.TraceDataConditionally(141514, TraceLevel.Verbose, "Identity", nameof (ClaimsProvider), "TryCreateSubjectDescriptor called with input", (Func<object>) (() => (object) new
      {
        domainClaim = domainClaim,
        puidClaim = puidClaim,
        oidClaim = oidClaim,
        cspClaim = cspClaim
      }), nameof (TryCreateSubjectDescriptor));
      try
      {
        Guid result;
        if (!string.IsNullOrEmpty(domainClaim?.Value) && Guid.TryParse(domainClaim.Value, out result))
        {
          if (!string.IsNullOrEmpty(oidClaim?.Value))
          {
            cuid = new Guid?(IdentityCuidHelper.ComputeCuid(requestContext, result, oidClaim.Value));
            subjectDescriptor = new SubjectDescriptor("aad", cuid.ToString());
            requestContext.Trace(141515, TraceLevel.Verbose, "Identity", nameof (ClaimsProvider), "TryCreateSubjectDescriptor found AAD user {0}", (object) subjectDescriptor);
            return true;
          }
          if (cspClaim != null)
          {
            if (!string.IsNullOrEmpty(puidClaim?.Value))
            {
              cuid = new Guid?(IdentityCuidHelper.ComputeCuid(requestContext, result, puidClaim.Value));
              subjectDescriptor = new SubjectDescriptor("csp", cuid.ToString());
              requestContext.Trace(141515, TraceLevel.Verbose, "Identity", nameof (ClaimsProvider), "TryCreateSubjectDescriptor found CSP user {0}", (object) subjectDescriptor);
              return true;
            }
          }
        }
        else if (!string.IsNullOrEmpty(puidClaim?.Value))
        {
          cuid = new Guid?(IdentityCuidHelper.ComputeCuid(requestContext, Guid.Empty, puidClaim.Value));
          subjectDescriptor = new SubjectDescriptor("msa", cuid.ToString());
          requestContext.Trace(141515, TraceLevel.Verbose, "Identity", nameof (ClaimsProvider), "TryCreateSubjectDescriptor found MSA user {0}", (object) subjectDescriptor);
          return true;
        }
      }
      catch (ArgumentException ex)
      {
        return false;
      }
      return false;
    }

    private void PopulateGitHubIdentityData(
      IVssRequestContext requestContext,
      IIdentity claimsIdentityFromToken,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!ProvDataClaimHelper.HasGitHubClaim(requestContext, claimsIdentityFromToken as ClaimsIdentity))
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IdentityService service = vssRequestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSubjectDescriptorForDIMS") && identity.SubjectDescriptor != new SubjectDescriptor())
          identity1 = service.ReadIdentities(vssRequestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
          {
            identity.SubjectDescriptor
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (identity1 == null)
        {
          identity1 = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            identity.Descriptor
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSubjectDescriptorForDIMS"))
            requestContext.TraceAlways(141513, TraceLevel.Error, "Identity", nameof (ClaimsProvider), "Cannot find identity using SubjectDescriptor {0}, falling back to read IdentityDescriptor {1}. Found deployment storage key {2}.", (object) identity.SubjectDescriptor, (object) identity.Descriptor, (object) identity1.Id);
        }
        identity.SocialDescriptor = identity1 != null ? identity1.SocialDescriptor : throw new IdentityNotFoundException(identity.Descriptor);
        identity.SetProperty("DirectoryAlias", (object) identity1.GetProperty<string>("DirectoryAlias", (string) null));
        requestContext.TraceSerializedConditionally(141511, TraceLevel.Verbose, "Identity", nameof (ClaimsProvider), "The social descriptor set is {0}", (object) identity.SocialDescriptor);
        ClaimsProvider.s_GitHubSignInCounter.Increment();
      }
      catch (Exception ex)
      {
        requestContext.TraceSerializedConditionally(141512, TraceLevel.Error, "Identity", nameof (ClaimsProvider), "Getting the GitHub data for identity failed due to : {0}", (object) ex.ToString());
      }
    }

    private string GetIdentityDisplayName(
      ClaimsIdentity sourceIdentity,
      Dictionary<string, string> claims)
    {
      string identityDisplayName;
      bool flag1 = claims.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", out identityDisplayName);
      string str;
      bool flag2 = claims.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", out str);
      if (flag1 & flag2)
        return identityDisplayName + " " + str;
      if (flag1)
        return identityDisplayName;
      return !string.IsNullOrEmpty(sourceIdentity.Name) ? sourceIdentity.Name : claims.GetValueOrDefault<string, string>("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", string.Empty);
    }

    private string GetIdentityAccountName(
      ClaimsIdentity sourceIdentity,
      IdentityDescriptor descriptor,
      IReadOnlyDictionary<string, string> claims)
    {
      if (!string.IsNullOrEmpty(sourceIdentity.Name))
        return sourceIdentity.Name;
      string str;
      return claims.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", out str) ? str : descriptor.Identifier;
    }

    protected override IEnumerable<string> AvailableIdentityAttributes => (IEnumerable<string>) ClaimsProvider.ExtendedClaims;
  }
}
