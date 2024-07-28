// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySelfService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentitySelfService : IIdentitySelfService
  {
    private const string s_area = "Identity";
    private const string s_layer = "IdentitySelfController";

    public IdentitySelf Get(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(800052, "Identity", "IdentitySelfController", nameof (Get));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      IdentitySelf identitySelfResult;
      try
      {
        IEnumerable<TenantInfo> identityTenants = this.GetIdentityTenants(requestContext);
        Microsoft.VisualStudio.Services.Identity.Identity homeTenantIdentity = this.GetOrCreateHomeTenantIdentity(requestContext, userIdentity, identityTenants);
        identitySelfResult = this.GetIdentitySelfResult(requestContext, userIdentity, homeTenantIdentity, identityTenants);
      }
      catch (Exception ex)
      {
        if (!(ex is AadCredentialsNotFoundException))
          requestContext.TraceException(800051, "Identity", "IdentitySelfController", ex);
        identitySelfResult = this.GetIdentitySelfResult(requestContext, userIdentity, (Microsoft.VisualStudio.Services.Identity.Identity) null, (IEnumerable<TenantInfo>) new TenantInfo[0]);
      }
      requestContext.TraceLeave(800053, "Identity", "IdentitySelfController", nameof (Get));
      return identitySelfResult;
    }

    internal IdentitySelf GetIdentitySelfResult(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity homeTenantIdentity,
      IEnumerable<TenantInfo> tenants)
    {
      if (homeTenantIdentity == null)
        homeTenantIdentity = requestIdentity;
      string origin;
      string originId;
      string domain;
      IdentitySelfService.GetUserOriginOriginIdAndDomain(context, homeTenantIdentity, out origin, out originId, out domain);
      return new IdentitySelf()
      {
        Id = homeTenantIdentity.Id,
        DisplayName = homeTenantIdentity.DisplayName,
        AccountName = (string) requestIdentity.Properties["Account"],
        Origin = origin,
        OriginId = originId,
        Domain = domain,
        Tenants = tenants
      };
    }

    private static void GetUserOriginOriginIdAndDomain(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      out string origin,
      out string originId,
      out string domain)
    {
      if (identity.IsContainer || ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor))
      {
        origin = (string) null;
        originId = (string) null;
        domain = (string) null;
      }
      else
      {
        IdentityHelper.GetUserOriginAndOriginId(identity, out origin, out originId);
        domain = identity.GetProperty<string>("Domain", (string) null);
      }
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetOrCreateHomeTenantIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<TenantInfo> tenants)
    {
      requestContext.TraceEnter(800054, "Identity", "IdentitySelfController", nameof (GetOrCreateHomeTenantIdentity));
      if (tenants == null || !tenants.Any<TenantInfo>())
      {
        string message = string.Format("No tenants found for identity : {0}", (object) identity.ProviderDisplayName);
        requestContext.Trace(800055, TraceLevel.Info, "Identity", "IdentitySelfController", message);
        return identity;
      }
      object obj = (object) null;
      identity.TryGetProperty("Account", out obj);
      string accountName = obj.ToString();
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity homeTenantIdentity;
      if (tenants.Any<TenantInfo>((Func<TenantInfo, bool>) (t => t.HomeTenant)))
      {
        string message = string.Format("Home tenants found for identity : {0}", (object) identity.ProviderDisplayName);
        requestContext.Trace(800056, TraceLevel.Info, "Identity", "IdentitySelfController", message);
        Guid tenantId = tenants.First<TenantInfo>((Func<TenantInfo, bool>) (t => t.HomeTenant)).TenantId;
        homeTenantIdentity = IdentityHelper.GetOrCreateBindPendingIdentity(requestContext, tenantId.ToString(), accountName, callerName: nameof (GetOrCreateHomeTenantIdentity));
      }
      else
      {
        string message = string.Format("No Home tenants found for identity : {0}", (object) identity.ProviderDisplayName);
        requestContext.Trace(800057, TraceLevel.Info, "Identity", "IdentitySelfController", message);
        homeTenantIdentity = IdentityHelper.GetOrCreateBindPendingIdentity(requestContext, "Windows Live ID", accountName, callerName: nameof (GetOrCreateHomeTenantIdentity));
      }
      if (homeTenantIdentity.Id == Guid.Empty)
      {
        string message = string.Format("Create home tenants identity for : {0}", (object) identity.ProviderDisplayName);
        requestContext.Trace(800058, TraceLevel.Info, "Identity", "IdentitySelfController", message);
        service.UpdateIdentities(requestContext.Elevate(), (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
        {
          homeTenantIdentity
        });
        homeTenantIdentity = service.ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          homeTenantIdentity.Descriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      requestContext.TraceLeave(800059, "Identity", "IdentitySelfController", nameof (GetOrCreateHomeTenantIdentity));
      return homeTenantIdentity;
    }

    public IEnumerable<TenantInfo> GetIdentityTenants(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(2011601, "Identity", "IdentitySelfController", nameof (GetIdentityTenants));
      requestContext.CheckServiceHostType(TeamFoundationHostType.Deployment);
      IIdentityTenantCache identityTenantCache = (IIdentityTenantCache) new IdentityTenantCache(requestContext);
      List<TenantInfo> tenantsResponse;
      if (identityTenantCache.TryGetTenants(requestContext, requestContext.GetUserId(), out tenantsResponse))
      {
        string message = string.Format("Tenants return from cache for identity id : {0}", (object) requestContext.GetUserId());
        requestContext.Trace(2011603, TraceLevel.Info, "Identity", "IdentitySelfController", message);
        return (IEnumerable<TenantInfo>) tenantsResponse;
      }
      string message1 = string.Format("Tenants not return from cache for identity id : {0}", (object) requestContext.GetUserId());
      requestContext.Trace(2011604, TraceLevel.Info, "Identity", "IdentitySelfController", message1);
      AadService service = requestContext.GetService<AadService>();
      IVssRequestContext context = requestContext.Elevate();
      GetTenantsRequest request = new GetTenantsRequest();
      request.ToMicrosoftServicesTenant = true;
      GetTenantsResponse tenants1 = service.GetTenants(context, request);
      List<TenantInfo> tenants2 = new List<TenantInfo>();
      if (tenants1.Tenants != null && tenants1.Tenants.Any<AadTenant>())
      {
        foreach (AadTenant tenant in tenants1.Tenants)
        {
          TenantInfo tenantInfo = new TenantInfo();
          if (tenants1.HomeTenant != null && tenants1.HomeTenant.ObjectId == tenant.ObjectId)
            tenantInfo.HomeTenant = true;
          tenantInfo.TenantName = tenant.DisplayName;
          tenantInfo.TenantId = tenant.ObjectId;
          tenants2.Add(tenantInfo);
        }
      }
      else
      {
        string message2 = string.Format("No tenants return for identity id : {0}", (object) requestContext.GetUserId());
        requestContext.Trace(2011602, TraceLevel.Info, "Identity", "IdentitySelfController", message2);
      }
      identityTenantCache.SetTenants(requestContext, requestContext.GetUserId(), tenants2);
      requestContext.TraceLeave(2011604, "Identity", "IdentitySelfController", nameof (GetIdentityTenants));
      return (IEnumerable<TenantInfo>) tenants2;
    }
  }
}
