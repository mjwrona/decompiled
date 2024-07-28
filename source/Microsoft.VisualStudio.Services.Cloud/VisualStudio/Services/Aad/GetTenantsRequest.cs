// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetTenantsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetTenantsRequest : AadServiceRequest
  {
    private const string AltSecdIdClaimType = "altsecid";

    public GetTenantsRequest()
    {
    }

    internal GetTenantsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    internal override void Validate()
    {
      if (!this.ToMicrosoftServicesTenant)
        throw new ArgumentException("GetTenants must be called with ToMicrosoftServicesTenant.", "ToMicrosoftServicesTenant");
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override bool UseBetaGraphVersion => true;

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      IdentityDescriptor userContext = vssRequestContext.UserContext;
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      IdentityDescriptor[] descriptors = new IdentityDescriptor[1]
      {
        userContext
      };
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) new string[0]);
      if (identityList.Count <= 0)
        throw new IdentityNotFoundException(userContext);
      if (!identityList[0].IsMsaOrFpmsa())
        return (AadServiceResponse) this.GetTenantsForAadUser(context);
      IdentityPuid puid = IdentityHelper.GetPuid(vssRequestContext, vssRequestContext.UserContext);
      return (AadServiceResponse) this.GetTenantsForMsaUser(context, puid);
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetResourceTenantsRequest resourceTenantsRequest = this.BuildMsGraphRequest(context);
      MsGraphGetResourceTenantsResponse resourceTenants = context.GetMsGraphClient().GetResourceTenants(context.VssRequestContext, resourceTenantsRequest);
      GetTenantsResponse getTenantsResponse = GetTenantsRequest.BuildMsGraphResponse(resourceTenantsRequest, resourceTenants);
      if (!string.IsNullOrEmpty(resourceTenantsRequest.HomeTenantId) && string.IsNullOrEmpty(getTenantsResponse.HomeTenant.ToString()))
        context.VssRequestContext.Trace(44750242, TraceLevel.Error, "VisualStudio.Services.Aad", "GetTenantsResponse", "HomeTenant is in request but not found in response under GetTenants in MsGraph. HomeTenantId: " + resourceTenantsRequest.HomeTenantId + ", " + string.Format("GetTenantsResponse:{0} ", (object) resourceTenants.Tenants.ToList<AadTenant>()));
      return (AadServiceResponse) getTenantsResponse;
    }

    private static GetTenantsResponse BuildMsGraphResponse(
      MsGraphGetResourceTenantsRequest getTenantsRequest,
      MsGraphGetResourceTenantsResponse graphResponseResourceTenants)
    {
      bool flag = !string.IsNullOrEmpty(getTenantsRequest.HomeTenantId);
      if (flag && !graphResponseResourceTenants.Tenants.Any<AadTenant>())
        throw new AadGraphException("No tenants returned from the Ms Graph API for an AAD user.");
      return new GetTenantsResponse()
      {
        HomeTenant = flag ? graphResponseResourceTenants.Tenants.Where<AadTenant>((Func<AadTenant, bool>) (t => t.ObjectId.ToString() == getTenantsRequest.HomeTenantId)).FirstOrDefault<AadTenant>() : (AadTenant) null,
        ForeignTenants = flag ? (IEnumerable<AadTenant>) graphResponseResourceTenants.Tenants.Where<AadTenant>((Func<AadTenant, bool>) (t => t.ObjectId.ToString() != getTenantsRequest.HomeTenantId)).ToList<AadTenant>() : graphResponseResourceTenants.Tenants,
        Tenants = graphResponseResourceTenants.Tenants
      };
    }

    private MsGraphGetResourceTenantsRequest BuildMsGraphRequest(AadServiceRequestContext context)
    {
      (bool flag, string str, Guid tenantId, JwtSecurityToken accessToken, string altSecId, string idpClaim) = this.GetParametesrForMSARequest(context);
      string identifierAad = string.Empty;
      string getTenantsByKeyTenantId = string.Empty;
      Guid homeTenantId = Guid.Empty;
      if (!flag)
        this.GetParametersForAADRequest(context, tenantId, accessToken, altSecId, idpClaim, out identifierAad, out homeTenantId, out getTenantsByKeyTenantId);
      MsGraphGetResourceTenantsRequest resourceTenantsRequest = new MsGraphGetResourceTenantsRequest();
      resourceTenantsRequest.AccessToken = flag ? context.GetAccessToken(true) : context.GetAccessToken(true, getTenantsByKeyTenantId);
      resourceTenantsRequest.IdentifierType = flag ? Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType.LiveId : Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType.OrgId;
      resourceTenantsRequest.Identifier = flag ? str : identifierAad;
      resourceTenantsRequest.HomeTenantId = flag ? (string) null : homeTenantId.ToString();
      return resourceTenantsRequest;
    }

    private void GetParametersForAADRequest(
      AadServiceRequestContext context,
      Guid tenantId,
      JwtSecurityToken accessToken,
      string altSecId,
      string idpClaim,
      out string identifierAad,
      out Guid homeTenantId,
      out string getTenantsByKeyTenantId)
    {
      if (string.IsNullOrEmpty(altSecId))
      {
        Claim claim = accessToken.Claims.Single<Claim>((Func<Claim, bool>) (x => x.Type == "puid"));
        identifierAad = claim.Value;
        homeTenantId = tenantId;
      }
      else
      {
        identifierAad = altSecId;
        homeTenantId = GetTenantsRequest.GetHomeTenantId(context.VssRequestContext, idpClaim);
      }
      getTenantsByKeyTenantId = context.TenantId;
      if (!this.IsTenantMicrosoftServices())
        return;
      getTenantsByKeyTenantId = context.GetSetting<string>("/Service/Aad/MicrosoftServicesTenantForGetTenantsByKey", context.TenantId);
    }

    private (bool, string, Guid, JwtSecurityToken, string, string) GetParametesrForMSARequest(
      AadServiceRequestContext context)
    {
      bool flag = false;
      string str = (string) null;
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      IdentityDescriptor userContext = vssRequestContext.UserContext;
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      IdentityDescriptor[] descriptors = new IdentityDescriptor[1]
      {
        userContext
      };
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) new string[0]);
      if (identityList.Count <= 0)
        throw new IdentityNotFoundException(userContext);
      if (identityList[0].IsMsaOrFpmsa())
        return (true, IdentityHelper.GetPuid(vssRequestContext, userContext).Value, Guid.Empty, (JwtSecurityToken) null, (string) null, (string) null);
      Guid currentTenantId = GetTenantsRequest.GetCurrentTenantId(vssRequestContext);
      JwtSecurityToken accessToken = context.GetAccessToken(currentTenantId.ToString(), false, true);
      string altSecId = this.GetAltSecId(accessToken);
      string claimValue = accessToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "idp"))?.Value;
      return !string.IsNullOrEmpty(altSecId) && claimValue != null && AuthenticationHelpers.ShouldTreatIdentityProviderAsMsa(vssRequestContext, claimValue) ? (true, new IdentityPuid(altSecId).Value, Guid.Empty, (JwtSecurityToken) null, (string) null, (string) null) : (flag, str, currentTenantId, accessToken, altSecId, claimValue);
    }

    private GetTenantsResponse GetTenantsForMsaUser(
      AadServiceRequestContext context,
      IdentityPuid puid)
    {
      if (!context.VssRequestContext.IsFeatureEnabled("VisualStudio.Services.Aad.DisableUseGetTenantsByKeyForMsa"))
      {
        IAadGraphClient graphClient = context.GetGraphClient();
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        GetTenantsByKeyRequest request = new GetTenantsByKeyRequest();
        request.AccessToken = context.GetAccessToken();
        request.IdentifierType = Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType.LiveId;
        request.Identifier = puid.Value;
        GetTenantsByKeyResponse tenantsByKey = graphClient.GetTenantsByKey(vssRequestContext, request);
        return new GetTenantsResponse()
        {
          HomeTenant = (AadTenant) null,
          ForeignTenants = tenantsByKey.Tenants,
          Tenants = tenantsByKey.Tenants
        };
      }
      IAadGraphClient graphClient1 = context.GetGraphClient();
      IVssRequestContext vssRequestContext1 = context.VssRequestContext;
      GetTenantsByAltSecIdRequest request1 = new GetTenantsByAltSecIdRequest();
      request1.AccessToken = context.GetAccessToken();
      request1.IdentifierType = Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType.LiveId;
      request1.Identifier = puid.Value;
      GetTenantsByAltSecIdResponse tenantsByAltSecId = graphClient1.GetTenantsByAltSecId(vssRequestContext1, request1);
      return new GetTenantsResponse()
      {
        HomeTenant = (AadTenant) null,
        ForeignTenants = tenantsByAltSecId.Tenants,
        Tenants = tenantsByAltSecId.Tenants
      };
    }

    private GetTenantsResponse GetTenantsForAadUser(AadServiceRequestContext context)
    {
      Guid currentTenantId = GetTenantsRequest.GetCurrentTenantId(context.VssRequestContext);
      JwtSecurityToken accessToken = context.GetAccessToken(currentTenantId.ToString(), false);
      string altSecId = this.GetAltSecId(accessToken);
      string str1 = accessToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "idp"))?.Value;
      if (!string.IsNullOrEmpty(altSecId) && str1 != null && AuthenticationHelpers.ShouldTreatIdentityProviderAsMsa(context.VssRequestContext, str1))
        return this.GetTenantsForMsaUser(context, new IdentityPuid(altSecId));
      string str2;
      Guid guid;
      if (string.IsNullOrEmpty(altSecId))
      {
        str2 = accessToken.Claims.Single<Claim>((Func<Claim, bool>) (x => x.Type == "puid")).Value;
        guid = currentTenantId;
      }
      else
      {
        str2 = altSecId;
        guid = GetTenantsRequest.GetHomeTenantId(context.VssRequestContext, str1);
      }
      string tenantId = context.TenantId;
      if (this.IsTenantMicrosoftServices())
        tenantId = context.GetSetting<string>("/Service/Aad/MicrosoftServicesTenantForGetTenantsByKey", context.TenantId);
      IAadGraphClient graphClient = context.GetGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      GetTenantsByKeyRequest request = new GetTenantsByKeyRequest();
      request.AccessToken = context.GetAccessToken(tenantId);
      request.IdentifierType = Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType.OrgId;
      request.Identifier = str2;
      request.HomeTenantId = guid.ToString();
      GetTenantsByKeyResponse tenantsByKey = graphClient.GetTenantsByKey(vssRequestContext, request);
      return tenantsByKey.Tenants.Any<AadTenant>() ? new GetTenantsResponse()
      {
        HomeTenant = tenantsByKey.Tenants.First<AadTenant>(),
        ForeignTenants = tenantsByKey.Tenants.Skip<AadTenant>(1),
        Tenants = tenantsByKey.Tenants
      } : throw new AadGraphException("No tenants returned from the Graph API for an AAD user.");
    }

    private bool IsTenantMicrosoftServices() => string.IsNullOrWhiteSpace(this.ToTenant) && !this.ToUserTenant && this.ToMicrosoftServicesTenant;

    private string GetAltSecId(JwtSecurityToken userAccessToken)
    {
      Claim claim = userAccessToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "altsecid"));
      if (claim != null)
      {
        string str = claim.Value;
        if (!string.IsNullOrEmpty(str) && str.LastIndexOf(':') > str.IndexOf(':'))
          return str.Substring(str.LastIndexOf(':') + 1);
      }
      return string.Empty;
    }

    private static Guid GetCurrentTenantId(IVssRequestContext context) => context.GetUserIdentity().GetProperty<Guid>("Domain", Guid.Empty);

    private static Guid GetHomeTenantId(IVssRequestContext context, string idpClaim)
    {
      Uri result1;
      if (idpClaim == null || !Uri.TryCreate(idpClaim, UriKind.Absolute, out result1))
        return Guid.Empty;
      string str;
      if (result1.Segments.Length <= 1)
        str = (string) null;
      else
        str = result1.Segments[1].TrimEnd('/');
      string input = str;
      Guid result2;
      return input == null || !Guid.TryParse(input, out result2) ? Guid.Empty : result2;
    }
  }
}
