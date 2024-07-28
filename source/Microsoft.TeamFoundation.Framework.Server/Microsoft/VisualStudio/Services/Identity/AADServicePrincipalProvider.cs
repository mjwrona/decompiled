// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.AADServicePrincipalProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class AADServicePrincipalProvider : NonSyncableIdentityProvider
  {
    private const string layer = "AADServicePrincipalProvider";
    private static readonly string[] supportedIdentityTypes = new string[1]
    {
      "Microsoft.VisualStudio.Services.Claims.AadServicePrincipal"
    };

    protected override IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      requestContext.TraceEnter(14100001, NonSyncableIdentityProvider.c_area, nameof (AADServicePrincipalProvider), nameof (CreateDescriptor));
      try
      {
        return AADServicePrincipalProvider.CreateDescriptorInternal(AADServicePrincipalProvider.ParseIdentityAttributes(requestContext, identity));
      }
      finally
      {
        requestContext.TraceLeave(14100002, NonSyncableIdentityProvider.c_area, nameof (AADServicePrincipalProvider), nameof (CreateDescriptor));
      }
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      requestContext.TraceEnter(14100001, NonSyncableIdentityProvider.c_area, nameof (AADServicePrincipalProvider), nameof (GetIdentity));
      try
      {
        AADServicePrincipalProvider.WellKnownClaims identityAttributes = AADServicePrincipalProvider.ParseIdentityAttributes(requestContext, identity);
        IdentityDescriptor descriptorInternal = AADServicePrincipalProvider.CreateDescriptorInternal(identityAttributes);
        return AADServicePrincipalProvider.CreateIdentityInternal(requestContext, descriptorInternal, identityAttributes);
      }
      finally
      {
        requestContext.TraceLeave(14100002, NonSyncableIdentityProvider.c_area, nameof (AADServicePrincipalProvider), nameof (GetIdentity));
      }
    }

    protected override string[] SupportedIdentityTypes() => AADServicePrincipalProvider.supportedIdentityTypes;

    private static AADServicePrincipalProvider.WellKnownClaims ParseIdentityAttributes(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      if (!(identity is ClaimsIdentity identity1))
        return (AADServicePrincipalProvider.WellKnownClaims) null;
      string tid = AADServicePrincipalProvider.ResolveClaim(requestContext, identity1, "http://schemas.microsoft.com/identity/claims/tenantid");
      string str1 = AADServicePrincipalProvider.ResolveClaim(requestContext, identity1, "http://schemas.microsoft.com/identity/claims/objectidentifier");
      string str2 = AADServicePrincipalProvider.ResolveClaim(requestContext, identity1, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
      string str3 = AADServicePrincipalProvider.ResolveClaim(requestContext, identity1, "appid");
      string str4 = AADServicePrincipalProvider.ResolveClaim(requestContext, identity1, "spmetatype");
      string str5 = AADServicePrincipalProvider.ResolveClaim(requestContext, identity1, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
      string str6 = AADServicePrincipalProvider.ResolveClaim(requestContext, identity1, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
      string nameIdentifier = str2;
      string oid = str1;
      string appid = str3;
      string metaType = str4;
      string givenName = str5;
      string surname = str6;
      return new AADServicePrincipalProvider.WellKnownClaims(tid, nameIdentifier, oid, appid, metaType, givenName, surname);
    }

    private static IdentityDescriptor CreateDescriptorInternal(
      AADServicePrincipalProvider.WellKnownClaims claims)
    {
      return claims != null ? new IdentityDescriptor("Microsoft.VisualStudio.Services.Claims.AadServicePrincipal", claims.NameIdentifier) : (IdentityDescriptor) null;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity CreateIdentityInternal(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      AADServicePrincipalProvider.WellKnownClaims claims)
    {
      return descriptor != (IdentityDescriptor) null ? AADServicePrincipalIdentityUtility.CreateAADServicePrincipalIdentity(requestContext, claims.Oid, claims.Tid, claims.Appid, claims.NameIdentifier, claims.MetaType, claims.GivenName, claims.Surname, descriptor) : (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    private static string ResolveClaim(
      IVssRequestContext requestContext,
      ClaimsIdentity identity,
      string claimType)
    {
      Claim first = identity.FindFirst(claimType);
      if (first != null)
      {
        requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (AADServicePrincipalProvider), claimType + ": " + first.Value);
        return first.Value;
      }
      requestContext.Trace(14100005, TraceLevel.Verbose, NonSyncableIdentityProvider.c_area, nameof (AADServicePrincipalProvider), string.Format("{0} doesn't exist.", (object) first));
      return (string) null;
    }

    private class WellKnownClaims
    {
      public readonly string Tid;
      public readonly string NameIdentifier;
      public readonly string Oid;
      public readonly string Appid;
      public readonly string MetaType;
      public readonly string GivenName;
      public readonly string Surname;

      public WellKnownClaims(
        string tid,
        string nameIdentifier,
        string oid,
        string appid,
        string metaType,
        string givenName,
        string surname)
      {
        if (tid.IsNullOrEmpty<char>() || nameIdentifier.IsNullOrEmpty<char>() || oid.IsNullOrEmpty<char>() || appid.IsNullOrEmpty<char>())
          throw new MissingClaimsException("Identity has missing mandatory claims.");
        this.Tid = tid;
        this.NameIdentifier = nameIdentifier;
        this.Oid = oid;
        this.Appid = appid;
        this.MetaType = metaType;
        this.GivenName = givenName;
        this.Surname = surname;
      }
    }
  }
}
