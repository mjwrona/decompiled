// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ServicePrincipalProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal sealed class ServicePrincipalProvider : NonSyncableIdentityProvider
  {
    private static readonly string[] s_supportedIdentityTypes = new string[1]
    {
      "System:ServicePrincipal"
    };
    private const string c_layer = "ServicePrincipalProvider";

    protected override IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      requestContext.TraceEnter(14100001, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), nameof (CreateDescriptor));
      try
      {
        string tid;
        SecuritySubjectEntry entry;
        if (this.TryGetEntryFromSubjectStore(requestContext, identity, out tid, out entry))
          return entry.ToDescriptor();
        IdentityDescriptor descriptor;
        return this.TryParseDescriptor(requestContext, identity, out Guid _, out tid, out string _, out descriptor) ? descriptor : (IdentityDescriptor) null;
      }
      finally
      {
        requestContext.TraceLeave(14100002, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), nameof (CreateDescriptor));
      }
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      requestContext.TraceEnter(14100003, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), nameof (GetIdentity));
      try
      {
        string tid;
        SecuritySubjectEntry entry;
        if (this.TryGetEntryFromSubjectStore(requestContext, identity, out tid, out entry))
          return ServicePrincipals.CreateServicePrincipalIdentity(entry.Id, tid, entry.Identifier, entry.ToDescriptor(), entry.Description, identity);
        Guid spid;
        string identifier;
        IdentityDescriptor descriptor;
        return this.TryParseDescriptor(requestContext, identity, out spid, out tid, out identifier, out descriptor) ? ServicePrincipals.CreateServicePrincipalIdentity(spid, tid, identifier, descriptor, (string) null, identity) : (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        requestContext.TraceLeave(14100004, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), nameof (GetIdentity));
      }
    }

    protected override string[] SupportedIdentityTypes() => ServicePrincipalProvider.s_supportedIdentityTypes;

    private bool TryParseIdentityAttributes(
      IVssRequestContext requestContext,
      IIdentity identity,
      out Guid spId,
      out string tid,
      out string identifier)
    {
      spId = Guid.Empty;
      tid = identifier = (string) null;
      if (identity is ClaimsIdentity claimsIdentity)
      {
        Claim first1 = claimsIdentity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid");
        if (first1 != null)
          requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "TenantIdentifierClaim: claimType:{0}, claimValue:{1}", (object) first1.Type, (object) first1.Value);
        else
          requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "TenantIdentitfierClaim is null");
        Claim first2 = claimsIdentity.FindFirst("appid");
        if (first2 != null)
          requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "AppidClaim: claimType:{0}, claimValue:{1}", (object) first2.Type, (object) first2.Value);
        else
          requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "AppidClaim is null");
        Claim first3 = claimsIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (first3 != null)
          requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "NameIdentifierClaim: claimType:{0}, claimValue:{1}", (object) first3.Type, (object) first3.Value);
        else
          requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "NameIdentifierClaim is null");
        if (first1 != null && first2 != null && Guid.TryParse(first2.Value, out spId))
        {
          tid = first1.Value;
          if (first3 == null)
          {
            identifier = first2.Value + "@" + tid;
            requestContext.Trace(14100010, TraceLevel.Warning, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "No nameidentifier claim in the S2S Service Principal token.");
          }
          else
            identifier = first3.Value;
          requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "Computed identitfier: {0}", (object) identifier);
          return true;
        }
      }
      return false;
    }

    private bool TryGetEntryFromSubjectStore(
      IVssRequestContext requestContext,
      IIdentity identity,
      out string tid,
      out SecuritySubjectEntry entry)
    {
      tid = (string) null;
      entry = (SecuritySubjectEntry) null;
      Guid spId;
      string identifier;
      if (this.TryParseIdentityAttributes(requestContext, identity, out spId, out tid, out identifier))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        SecuritySubjectEntry securitySubjectEntry = vssRequestContext.GetService<IVssSecuritySubjectService>().GetSecuritySubjectEntry(vssRequestContext, spId);
        if (securitySubjectEntry != null)
        {
          if (string.Equals(identifier, securitySubjectEntry.Identifier, StringComparison.OrdinalIgnoreCase))
          {
            entry = securitySubjectEntry;
            return true;
          }
          requestContext.Trace(14100011, TraceLevel.Error, NonSyncableIdentityProvider.c_area, nameof (ServicePrincipalProvider), "The identifier parsed from the incoming S2S token - " + identifier + " - does not match the identifier for the same service principal id in the subject store: " + entry.Id.ToString("D") + ", " + entry.Identifier + ", This indicates corrupt data in the System Subject Store.");
        }
      }
      return false;
    }

    private bool TryParseDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity,
      out Guid spid,
      out string tid,
      out string identifier,
      out IdentityDescriptor descriptor)
    {
      spid = Guid.Empty;
      tid = identifier = (string) null;
      descriptor = (IdentityDescriptor) null;
      if (!this.TryParseIdentityAttributes(requestContext, identity, out spid, out tid, out identifier))
        return false;
      descriptor = new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", identifier);
      return true;
    }
  }
}
