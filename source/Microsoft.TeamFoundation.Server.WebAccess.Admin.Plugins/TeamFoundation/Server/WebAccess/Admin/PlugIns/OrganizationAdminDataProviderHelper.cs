// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationAdminDataProviderHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class OrganizationAdminDataProviderHelper
  {
    public static AadTenant GetTenant(IVssRequestContext requestContext) => requestContext.GetService<AadService>().GetTenant(requestContext, new GetTenantRequest()).Tenant;

    public static IEnumerable<AadTenant> GetTenants(IVssRequestContext requestContext)
    {
      HashSet<AadTenant> tenants1 = new HashSet<AadTenant>();
      try
      {
        IVssRequestContext context1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        AadService service = context1.GetService<AadService>();
        IVssRequestContext context2 = context1;
        GetTenantsRequest request = new GetTenantsRequest();
        request.ToMicrosoftServicesTenant = true;
        GetTenantsResponse tenants2 = service.GetTenants(context2, request);
        if (tenants2.HomeTenant != null)
          tenants1.Add(tenants2.HomeTenant);
        if (tenants2.ForeignTenants != null)
          tenants1.UnionWith(tenants2.ForeignTenants);
        if (tenants2.Tenants != null)
          tenants1.UnionWith(tenants2.Tenants);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050050, TraceLevel.Error, "OrganizationAAD", "Service", ex);
      }
      return (IEnumerable<AadTenant>) tenants1;
    }

    public static void ValidateContext(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
    }

    public static WebSessionToken GetOrganizationSessionToken(IVssRequestContext requestContext) => OrganizationAdminDataProviderHelper.GetSessionToken(requestContext, TeamFoundationHostType.Application);

    public static WebSessionToken GetSessionToken(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType,
      string tokenName = "sessiontoken-organizationadmin")
    {
      IVssRequestContext context = requestContext.To(hostType);
      Guid instanceId = context.ServiceHost.InstanceId;
      IDelegatedAuthorizationService service = context.GetService<IDelegatedAuthorizationService>();
      SessionTokenResult sessionTokenResult;
      try
      {
        IDelegatedAuthorizationService authorizationService = service;
        IVssRequestContext requestContext1 = context;
        Guid? clientId = new Guid?(Guid.Empty);
        string str = tokenName;
        DateTime? nullable = new DateTime?(DateTime.UtcNow.AddHours(1.0));
        Guid[] guidArray;
        if (hostType == TeamFoundationHostType.Deployment)
          guidArray = (Guid[]) null;
        else
          guidArray = new Guid[1]{ instanceId };
        IList<Guid> guidList = (IList<Guid>) guidArray;
        Guid? userId = new Guid?();
        string name = str;
        DateTime? validTo = nullable;
        IList<Guid> targetAccounts = guidList;
        Guid? authorizationId = new Guid?();
        Guid? accessId = new Guid?();
        sessionTokenResult = authorizationService.IssueSessionToken(requestContext1, clientId, userId, name, validTo, targetAccounts: targetAccounts, authorizationId: authorizationId, accessId: accessId);
        if (sessionTokenResult.HasError)
          requestContext.Trace(10050002, TraceLevel.Error, "General", "Service", sessionTokenResult.SessionTokenError.ToString());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1005003, "General", "Service", ex);
        return (WebSessionToken) null;
      }
      return new WebSessionToken()
      {
        Token = sessionTokenResult.SessionToken?.Token,
        ValidTo = sessionTokenResult.SessionToken?.ValidTo
      };
    }

    public static bool CheckTenantLinkingPermission(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, OrganizationSecurity.NamespaceId);
      return securityNamespace.HasPermission(vssRequestContext, OrganizationSecurity.TenantLinkingToken, 4, false) || securityNamespace.HasPermission(vssRequestContext, OrganizationSecurity.CollectionsToken, 4);
    }

    public static int GetModifyPermissionBits(IVssRequestContext requestContext, Guid collectionId)
    {
      string collectionToken = OrganizationSecurity.GenerateCollectionToken(new Guid?(collectionId));
      return !OrganizationAdminDataProviderHelper.HasPermissions(requestContext, 4, collectionToken) ? 0 : 16;
    }

    public static int GetDeletePermissionBits(IVssRequestContext requestContext, Guid collectionId)
    {
      string collectionToken = OrganizationSecurity.GenerateCollectionToken(new Guid?(collectionId));
      return !OrganizationAdminDataProviderHelper.HasPermissions(requestContext, 8, collectionToken) ? 0 : 32;
    }

    private static bool HasPermissions(
      IVssRequestContext requestContext,
      int requestedPermissions,
      string token)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, OrganizationSecurity.NamespaceId).HasPermission(vssRequestContext, token, requestedPermissions, requestedPermissions < 16);
    }

    internal class OrganizationPermissions
    {
      public const int Modify = 1;
      public const int ModifyProperties = 2;
      public const int Delete = 4;
      public const int ModifyCollection = 16;
      public const int DeleteCollection = 32;
      public const int ReadCollectionDetails = 64;
    }
  }
}
