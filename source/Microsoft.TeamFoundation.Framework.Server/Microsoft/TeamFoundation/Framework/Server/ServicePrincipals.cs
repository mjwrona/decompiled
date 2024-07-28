// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicePrincipals
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServicePrincipals
  {
    private static readonly string[] s_viewServiceEndpointSecretsServicePrincipals = new string[4]
    {
      "0000000d",
      "00000030",
      "00000036",
      "0000003b"
    };
    private static readonly string[] s_readSecureFilesServicePrincipals = new string[1]
    {
      "0000000d"
    };
    private static readonly string[] s_readSecretVariableServicePrincipals = new string[1]
    {
      "0000000d"
    };
    private static string s_servicePrincipalSuffix;

    public static bool IsServicePrincipal(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool allowFirstPartyServicePrincipals = true)
    {
      return ServicePrincipals.IsServicePrincipal(requestContext, descriptor, allowFirstPartyServicePrincipals, out Guid _);
    }

    public static bool IsServicePrincipal(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool allowFirstPartyServicePrincipals,
      out Guid spId)
    {
      bool flag1 = false;
      spId = Guid.Empty;
      if (descriptor == (IdentityDescriptor) null)
        return flag1;
      Guid tenantGuid;
      if (ServicePrincipals.TryParse(descriptor, out spId, out tenantGuid))
        flag1 = ServicePrincipals.IsSystemOrFirstPartyServicePrincipal(requestContext, spId, tenantGuid, allowFirstPartyServicePrincipals);
      bool flag2 = descriptor.Identifier == "*" && descriptor.IsSystemServicePrincipalType();
      return flag1 | flag2;
    }

    public static bool IsSystemOrFirstPartyServicePrincipal(
      IVssRequestContext requestContext,
      Guid servicePrincipalAppId,
      Guid tenantId,
      bool allowFirstPartyServicePrincipals)
    {
      Guid s2StenantId = requestContext.ServiceHost.DeploymentServiceHost.DeploymentServiceHostInternal().S2STenantId;
      return ((!ServicePrincipals.IsInternalServicePrincipalId(servicePrincipalAppId) ? 0 : (tenantId == s2StenantId ? 1 : 0)) | (!allowFirstPartyServicePrincipals ? (false ? 1 : 0) : (FirstPartyS2SAuthTokenValidator.IsFirstPartyServicePrincipal(servicePrincipalAppId, tenantId) ? 1 : 0))) != 0;
    }

    public static bool IsServicePrincipalThatCanViewSeviceEndpointSecrets(
      IVssRequestContext requestContext)
    {
      return ServicePrincipals.IsServicePrincipal(requestContext, ServicePrincipals.s_viewServiceEndpointSecretsServicePrincipals);
    }

    public static bool IsServicePrincipalThatCanReadSecureFiles(IVssRequestContext requestContext) => ServicePrincipals.IsServicePrincipal(requestContext, ServicePrincipals.s_readSecureFilesServicePrincipals);

    public static bool IsServicePrincipalThatCanReadSecretVariables(
      IVssRequestContext requestContext)
    {
      return ServicePrincipals.IsServicePrincipal(requestContext, ServicePrincipals.s_readSecretVariableServicePrincipals);
    }

    public static bool TryParse(
      IdentityDescriptor descriptor,
      out Guid spGuid,
      out Guid tenantGuid)
    {
      if (descriptor != (IdentityDescriptor) null && descriptor.Identifier != null && descriptor.Identifier.Length == 73 && (descriptor.IsClaimsIdentityType() || descriptor.IsSystemServicePrincipalType()))
        return ServicePrincipals.TryParse(descriptor.Identifier, out spGuid, out tenantGuid);
      spGuid = Guid.Empty;
      tenantGuid = Guid.Empty;
      return false;
    }

    public static bool TryParse(SubjectDescriptor descriptor, out Guid spGuid, out Guid tenantGuid)
    {
      if (descriptor.Identifier != null && descriptor.Identifier.Length == 73 && descriptor.IsSystemServicePrincipalType())
        return ServicePrincipals.TryParse(descriptor.Identifier, out spGuid, out tenantGuid);
      spGuid = Guid.Empty;
      tenantGuid = Guid.Empty;
      return false;
    }

    internal static bool TryParse(string identifier, out Guid spGuid, out Guid tenantGuid)
    {
      if (!string.IsNullOrEmpty(identifier))
      {
        string[] strArray = identifier.Split('@');
        if (strArray.Length == 2 && Guid.TryParse(strArray[0], out spGuid) && Guid.TryParse(strArray[1], out tenantGuid))
          return true;
      }
      spGuid = Guid.Empty;
      tenantGuid = Guid.Empty;
      return false;
    }

    internal static unsafe bool IsInternalServicePrincipalId(Guid servicePrincipal)
    {
      uint* numPtr = (uint*) ((IntPtr) &servicePrincipal + 4);
      return *numPtr == 2290614272U && numPtr[1] == 128U && numPtr[2] == 0U;
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity CreateServicePrincipalIdentity(
      Guid spId,
      string tid,
      string description)
    {
      string identifier = string.Format("{0:D}@{1}", (object) spId, (object) tid);
      IdentityDescriptor descriptor = new IdentityDescriptor("System:ServicePrincipal", identifier);
      return ServicePrincipals.CreateServicePrincipalIdentity(spId, tid, identifier, descriptor, description, (IIdentity) null);
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity CreateServicePrincipalIdentity(
      SubjectDescriptor subjectDescriptor,
      string description)
    {
      Guid spGuid;
      Guid tenantGuid;
      IdentityDescriptor descriptor = ServicePrincipals.TryParse(subjectDescriptor, out spGuid, out tenantGuid) ? new IdentityDescriptor("System:ServicePrincipal", subjectDescriptor.Identifier) : throw new ArgumentException(string.Format("Descriptor {0} does not represent a valid Service Principal.", (object) subjectDescriptor));
      return ServicePrincipals.CreateServicePrincipalIdentity(spGuid, tenantGuid.ToString("D"), subjectDescriptor.Identifier, descriptor, description, (IIdentity) null);
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity CreateServicePrincipalIdentity(
      IdentityDescriptor descriptor,
      string description)
    {
      Guid spGuid;
      Guid tenantGuid;
      if (ServicePrincipals.TryParse(descriptor, out spGuid, out tenantGuid))
        return ServicePrincipals.CreateServicePrincipalIdentity(spGuid, tenantGuid.ToString("D"), descriptor.Identifier, descriptor, description, (IIdentity) null);
      throw new ArgumentException(string.Format("Descriptor {0} does not represent a valid Service Principal.", (object) descriptor));
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity CreateServicePrincipalIdentity(
      Guid spId,
      string tid,
      string identifier,
      IdentityDescriptor descriptor,
      string description,
      IIdentity identity)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity1.CustomDisplayName = description;
      identity1.Descriptor = descriptor;
      identity1.ProviderDisplayName = identifier;
      identity1.Id = spId;
      identity1.IsActive = true;
      identity1.IsContainer = false;
      identity1.MasterId = spId;
      identity1.SubjectDescriptor = new SubjectDescriptor("s2s", identifier);
      Microsoft.VisualStudio.Services.Identity.Identity principalIdentity = identity1;
      principalIdentity.SetProperty("SchemaClassName", (object) "User");
      principalIdentity.SetProperty("Description", (object) string.Empty);
      principalIdentity.SetProperty("Domain", (object) (OAuth2RegistryConstants.S2SWellKnownServicePrincipalName + "@" + tid));
      principalIdentity.SetProperty("Account", (object) identifier);
      principalIdentity.SetProperty("DN", (object) string.Empty);
      principalIdentity.SetProperty("Mail", (object) string.Empty);
      principalIdentity.SetProperty("SpecialType", (object) "Generic");
      principalIdentity.SetProperty("ComplianceValidated", (object) DateTime.UtcNow);
      if (identity != null)
      {
        Claim first = identity is ClaimsIdentity claimsIdentity ? claimsIdentity.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") : (Claim) null;
        if (first != null)
          principalIdentity.SetProperty(first.Type, (object) first.Value);
      }
      principalIdentity.ResetModifiedProperties();
      return principalIdentity;
    }

    private static bool IsServicePrincipal(IVssRequestContext requestContext, string[] prefixes)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      string identifier = requestContext.UserContext?.Identifier;
      if ((identifier != null ? (identifier.Length != 73 ? 1 : 0) : 1) != 0 || !identifier.EndsWith(ServicePrincipals.GetServicePrincipalSuffix(requestContext), StringComparison.OrdinalIgnoreCase))
        return false;
      for (int index = 0; index < prefixes.Length; ++index)
      {
        if (identifier.StartsWith(prefixes[index], StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static string GetServicePrincipalSuffix(IVssRequestContext requestContext)
    {
      if (ServicePrincipals.s_servicePrincipalSuffix == null)
        ServicePrincipals.s_servicePrincipalSuffix = "-0000-8888-8000-000000000000@" + requestContext.ServiceHost.DeploymentServiceHost.DeploymentServiceHostInternal().S2STenantId.ToString("D");
      return ServicePrincipals.s_servicePrincipalSuffix;
    }
  }
}
